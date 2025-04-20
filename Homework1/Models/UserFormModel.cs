using System.ComponentModel.DataAnnotations;

namespace Homework1.Models
{
    public class UserFormModel
    {
        [Required]
        [MinLength(3, ErrorMessage = "Username must be at least 3 characters.")]
        public string Username { get; set; }

        [Required]
        [EmailAddress(ErrorMessage = "Email is not valid.")]
        public string Email { get; set; }

        [Required]
        [StrongPassword("Username")]
        public string Password { get; set; }

        [Required]
        [PastDate(ErrorMessage = "Date of birth must be in the past.")]
        public DateTime DateOfBirth { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be a positive number.")]
        public int Quantity { get; set; }

        [Required]
        [Range(typeof(decimal), "0.01", "999999999", ErrorMessage = "Price must be a decimal value.")]
        public decimal Price { get; set; }

        [Required]
        [Range(0, 49, ErrorMessage = "Amount must be less than 50.")]
        public int Amount { get; set; }
    }


}
