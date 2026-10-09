using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using UsersApi.Dtos;
using UsersApi.Models;
using UsersApi.Services;

namespace UsersApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly string _connectionString;
        private readonly UpdateLogger _log;

        public UsersController(IConfiguration configuration, UpdateLogger log)
        {
            _connectionString = configuration.GetConnectionString("UsersDb")!;
            _log = log;
        }

        // GET api/users
        [HttpGet]
        public IActionResult GetAll()
        {
            var users = new List<User>();

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    string query = "SELECT Id, Name, Email, Age FROM Users";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                users.Add(ReadUser(reader));
                            }
                        }
                    }
                }
            }
            catch (SqlException)
            {
                return StatusCode(500, "Ошибка при обращении к базе данных.");
            }

            return Ok(users);
        }

        // GET api/users/1
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            User? user = null;

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    string query = "SELECT Id, Name, Email, Age FROM Users WHERE Id = @Id";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Id", id);
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                                user = ReadUser(reader);
                        }
                    }
                }
            }
            catch (SqlException)
            {
                return StatusCode(500, "Ошибка при обращении к базе данных.");
            }

            if (user == null)
                return NotFound("Пользователь не найден.");

            return Ok(user);
        }

        // Задания 1, 3, 4, 5
        // PUT api/users/1  -  обновляет имя, email и возраст
        // в одной транзакции: сохраняем старые значения в UserChanges и обновляем Users
        [HttpPut("{id}")]
        public IActionResult UpdateUser(int id, UpdateUserDto user)
        {
            const string action = "UpdateUser";

            if (!ModelState.IsValid)
            {
                _log.Write(action, id, false, "некорректные входные данные");
                return BadRequest(ModelState);
            }

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    using (SqlTransaction transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            // шаг 1: сохраняем старые данные в историю
                            string historyQuery = @"INSERT INTO UserChanges (UserId, OldName, OldEmail, OldAge, ChangedAt)
                                                    SELECT Id, Name, Email, Age, @ChangedAt FROM Users WHERE Id = @Id";
                            int saved;
                            using (SqlCommand command = new SqlCommand(historyQuery, connection, transaction))
                            {
                                command.Parameters.AddWithValue("@Id", id);
                                command.Parameters.AddWithValue("@ChangedAt", DateTime.Now);
                                saved = command.ExecuteNonQuery();
                            }

                            if (saved < 1)
                            {
                                transaction.Rollback();
                                _log.Write(action, id, false, "пользователь не найден");
                                return BadRequest("Обновление не удалось.");
                            }

                            // шаг 2: обновляем пользователя
                            string query = "UPDATE Users SET Name = @Name, Email = @Email, Age = @Age WHERE Id = @Id";
                            using (SqlCommand command = new SqlCommand(query, connection, transaction))
                            {
                                command.Parameters.AddWithValue("@Id", id);
                                command.Parameters.AddWithValue("@Name", user.Name);
                                command.Parameters.AddWithValue("@Email", user.Email);
                                command.Parameters.AddWithValue("@Age", user.Age);
                                command.ExecuteNonQuery();
                            }

                            // оба шага прошли - фиксируем
                            transaction.Commit();
                        }
                        catch
                        {
                            // если что-то упало - откатываем оба шага
                            transaction.Rollback();
                            throw;
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                _log.Write(action, id, false, "ошибка БД: " + ex.Message);
                return StatusCode(500, "Ошибка при обращении к базе данных.");
            }
            catch (Exception ex)
            {
                _log.Write(action, id, false, "ошибка: " + ex.Message);
                return StatusCode(500, "Внутренняя ошибка сервера.");
            }

            _log.Write(action, id, true, $"Name={user.Name}, Email={user.Email}, Age={user.Age}");
            return Ok("Данные пользователя обновлены.");
        }

        // Задания 2, 4, 5
        // PUT api/users/1/email  -  меняет email, только если имя совпадает с переданным
        [HttpPut("{id}/email")]
        public IActionResult UpdateEmail(int id, UpdateEmailDto data)
        {
            const string action = "UpdateEmail";

            if (!ModelState.IsValid)
            {
                _log.Write(action, id, false, "некорректные входные данные");
                return BadRequest(ModelState);
            }

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    string query = "UPDATE Users SET Email = @Email WHERE Id = @Id AND Name = @Name";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Id", id);
                        command.Parameters.AddWithValue("@Name", data.CurrentName);
                        command.Parameters.AddWithValue("@Email", data.NewEmail);

                        connection.Open();
                        int result = command.ExecuteNonQuery();

                        if (result < 1)
                        {
                            _log.Write(action, id, false, $"имя '{data.CurrentName}' не совпало или пользователь не найден");
                            return BadRequest("Обновление не удалось: имя пользователя не совпадает или пользователь не найден.");
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                _log.Write(action, id, false, "ошибка БД: " + ex.Message);
                return StatusCode(500, "Ошибка при обращении к базе данных.");
            }
            catch (Exception ex)
            {
                _log.Write(action, id, false, "ошибка: " + ex.Message);
                return StatusCode(500, "Внутренняя ошибка сервера.");
            }

            _log.Write(action, id, true, "новый email: " + data.NewEmail);
            return Ok("Email пользователя обновлен.");
        }

        private static User ReadUser(SqlDataReader reader)
        {
            return new User
            {
                Id = reader.GetInt32(0),
                Name = reader.IsDBNull(1) ? null : reader.GetString(1),
                Email = reader.IsDBNull(2) ? null : reader.GetString(2),
                Age = reader.IsDBNull(3) ? null : reader.GetInt32(3)
            };
        }
    }
}
