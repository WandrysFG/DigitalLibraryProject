using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StellarBooks.Domain.Entities
{
    [Table("Tales")]
    public class Tale
    {
        [Key]
        public int Id { get; set; }

        [Required, StringLength(100)]
        public string Title { get; set; }

        [Required]
        public int RecommendedAge { get; set; }

        [Required, StringLength(50)]
        public string Theme { get; set; }

        [Required]
        public string Content { get; set; }

        [StringLength(255)]
        public string CoverImage { get; set; }

        [StringLength(255)]
        public string NarrationAudio { get; set; }

        public bool IsAvailable { get; set; } = true;

        [Column(TypeName = "date")]
        public DateTime PublicationDate { get; set; } = DateTime.Today;

        public virtual ICollection<Activity> Activities { get; set; } = new List<Activity>();

        public virtual ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();
    }
}