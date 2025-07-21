using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace StellarBooks.Domain.Entities
{
    [Table("Users")]
    public class User
    {
        [Key]
        public int Id { get; set; }

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

        [Column(TypeName = "date")]
        public DateTime RegistrationDate { get; set; } = DateTime.Today;

        public bool IsActive { get; set; } = true;

        public virtual ICollection<Favorite> Favorites { get; set; }
    }
}