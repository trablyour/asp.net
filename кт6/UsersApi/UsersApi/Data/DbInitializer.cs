using Microsoft.Data.SqlClient;

namespace UsersApi.Data
{
    public static class DbInitializer
    {
        public static void Initialize(string connectionString)
        {
            // сначала подключаемся к master и создаем базу
            var builder = new SqlConnectionStringBuilder(connectionString);
            string dbName = builder.InitialCatalog;
            builder.InitialCatalog = "master";

            using (var connection = new SqlConnection(builder.ConnectionString))
            {
                connection.Open();
                Execute(connection, $"IF DB_ID(N'{dbName}') IS NULL CREATE DATABASE [{dbName}]");
            }

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // таблица из презентации
                Execute(connection, @"
                    IF OBJECT_ID('Users') IS NULL
                    BEGIN
                        CREATE TABLE Users (
                            Id INT PRIMARY KEY,
                            Name NVARCHAR(100),
                            Email NVARCHAR(100)
                        );

                        INSERT INTO Users (Id, Name, Email) VALUES
                            (1, N'Иван', N'ivan@mail.ru'),
                            (2, N'Анна', N'anna@mail.ru'),
                            (3, N'Петр', N'petr@mail.ru');
                    END");

                // задание 1: новое поле Age
                Execute(connection, @"
                    IF COL_LENGTH('Users', 'Age') IS NULL
                        ALTER TABLE Users ADD Age INT NULL");

                Execute(connection, @"
                    UPDATE Users SET Age = CASE Id WHEN 1 THEN 25 WHEN 2 THEN 30 WHEN 3 THEN 19 END
                    WHERE Age IS NULL AND Id IN (1, 2, 3)");

                // задание 3: таблица истории изменений, пишется в одной транзакции с обновлением
                Execute(connection, @"
                    IF OBJECT_ID('UserChanges') IS NULL
                        CREATE TABLE UserChanges (
                            Id INT IDENTITY PRIMARY KEY,
                            UserId INT NOT NULL,
                            OldName NVARCHAR(100),
                            OldEmail NVARCHAR(100),
                            OldAge INT,
                            ChangedAt DATETIME2 NOT NULL
                        )");
            }
        }

        private static void Execute(SqlConnection connection, string sql)
        {
            using (var command = new SqlCommand(sql, connection))
            {
                command.ExecuteNonQuery();
            }
        }
    }
}
