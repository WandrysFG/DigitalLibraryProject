using System.ComponentModel.DataAnnotations;

namespace StellarBooks.Applications.DTOs
{
    public class CreateUserDto
    {
        [Required, StringLength(50)]
        public string FirstName { get; set; }

        [Required, StringLength(50)]
        public string LastName { get; set; }

        [Required, StringLength(100), EmailAddress]
        public string Email { get; set; }

        [Required, StringLength(255)]
        public string Password { get; set; }

        [Required, StringLength(20)]
        public string UserType { get; set; } = "Reader";

        public bool IsActive { get; set; } = true;

        public DateTime RegistrationDate { get; set; }
    }
}