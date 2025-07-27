using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StellarBooks.Domain.Entities
{
    [Table("Favorites")]
    public class Favorite
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public User? User { get; set; }

        [Required]
        public int TaleId { get; set; }

        [ForeignKey("TaleId")]
        public Tale? Tale { get; set; }

        [Column(TypeName = "date")]
        public DateTime DateAdded { get; set; } = DateTime.Today;
    }
}