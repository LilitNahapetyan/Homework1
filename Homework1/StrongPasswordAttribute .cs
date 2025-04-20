using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Homework1
{
    public class StrongPasswordAttribute : ValidationAttribute
    {
        public string UsernameProperty { get; }

        public StrongPasswordAttribute(string usernameProperty)
        {
            UsernameProperty = usernameProperty;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var password = value as string;
            if (string.IsNullOrEmpty(password))
                return new ValidationResult("Password is required.");

            var usernameProp = validationContext.ObjectType.GetProperty(UsernameProperty);
            var username = usernameProp?.GetValue(validationContext.ObjectInstance) as string ?? string.Empty;

            if (password.Length < 6)
                return new ValidationResult("Password must be at least 6 characters.");

            if (!password.Any(char.IsUpper))
                return new ValidationResult("Password must contain at least one uppercase letter.");
            if (!password.Any(char.IsLower))
                return new ValidationResult("Password must contain at least one lowercase letter.");

            if (!password.Any(char.IsDigit))
                return new ValidationResult("Password must contain at least one number.");

            if (!Regex.IsMatch(password, @"[\u0531-\u0587]")) 
                return new ValidationResult("Password must contain at least one Armenian letter.");

            if (!password.Any(ch => !char.IsLetterOrDigit(ch)))
                return new ValidationResult("Password must contain at least one symbol.");

            if (!string.IsNullOrEmpty(username) && password.ToLower().Contains(username.ToLower()))
                return new ValidationResult("Password must not contain the username.");

            return ValidationResult.Success;
        }
    }
}
