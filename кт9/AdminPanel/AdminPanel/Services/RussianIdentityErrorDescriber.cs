using Microsoft.AspNetCore.Identity;

namespace AdminPanel.Services
{
    // русские сообщения об ошибках Identity
    public class RussianIdentityErrorDescriber : IdentityErrorDescriber
    {
        public override IdentityError DuplicateUserName(string userName)
            => new IdentityError { Code = nameof(DuplicateUserName), Description = $"Пользователь {userName} уже существует" };

        public override IdentityError DuplicateEmail(string email)
            => new IdentityError { Code = nameof(DuplicateEmail), Description = $"Email {email} уже используется" };

        public override IdentityError InvalidEmail(string? email)
            => new IdentityError { Code = nameof(InvalidEmail), Description = "Некорректный email" };

        public override IdentityError PasswordTooShort(int length)
            => new IdentityError { Code = nameof(PasswordTooShort), Description = $"Пароль должен быть не короче {length} символов" };

        public override IdentityError PasswordRequiresDigit()
            => new IdentityError { Code = nameof(PasswordRequiresDigit), Description = "Пароль должен содержать хотя бы одну цифру" };

        public override IdentityError PasswordRequiresLower()
            => new IdentityError { Code = nameof(PasswordRequiresLower), Description = "Пароль должен содержать хотя бы одну строчную букву" };

        public override IdentityError PasswordRequiresUpper()
            => new IdentityError { Code = nameof(PasswordRequiresUpper), Description = "Пароль должен содержать хотя бы одну заглавную букву" };

        public override IdentityError PasswordRequiresNonAlphanumeric()
            => new IdentityError { Code = nameof(PasswordRequiresNonAlphanumeric), Description = "Пароль должен содержать спецсимвол" };
    }
}
