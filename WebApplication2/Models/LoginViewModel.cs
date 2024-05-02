using System.ComponentModel.DataAnnotations;

namespace WebApplication2.Models
{
    public class LoginViewModel : IValidatableObject
    {
        public AppTenantInfo TenantInfo { get; set; }

        [Required]
        public string TenantId { get; set; }

        [Required]
        public string Login { get; set; }

        [Required]
        public string Password { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrEmpty(TenantId))
            {
                yield return new ValidationResult("Field is required.", new[] { nameof(TenantId) });
            }
            if (string.IsNullOrEmpty(Login))
            {
                yield return new ValidationResult("Field is required.", new[] { nameof(Login) });
            }
            if (string.IsNullOrEmpty(Password))
            {
                yield return new ValidationResult("Field is required.", new[] { nameof(Password) });
            }
        }
    }
}
