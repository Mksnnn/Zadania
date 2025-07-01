using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Zadanie_5.Models
{
    public class Klient
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Surname { get; set; }

        [Required]
        [StringLength(11, MinimumLength = 11, ErrorMessage = "PESEL musi mieć dokładnie 11 znaków.")]
        [RegularExpression(@"^\d{11}$", ErrorMessage = "PESEL musi składać się z 11 cyfr.")]
        [CustomValidation(typeof(Klient), nameof(ValidatePesel))]
        public string PESEL { get; set; }
        public int BirthYear { get; set; }
        public int Gender { get; set; }

        public static ValidationResult ValidatePesel(string pesel, ValidationContext context)
        {
            if (pesel == null) return ValidationResult.Success;

            if (!Regex.IsMatch(pesel, @"^\d{11}$"))
                return new ValidationResult("PESEL musi mieć 11 cyfr.");

            int[] weights = { 1, 3, 7, 9, 1, 3, 7, 9, 1, 3 };
            int sum = 0;

            for (int i = 0; i < 10; i++)
                sum += weights[i] * (pesel[i] - '0');

            int control = (10 - (sum % 10)) % 10;

            if (control != (pesel[10] - '0'))
                return new ValidationResult("Niepoprawny numer PESEL.");

            return ValidationResult.Success;
        }
    }
}
