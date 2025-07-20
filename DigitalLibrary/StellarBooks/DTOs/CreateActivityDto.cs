using System.ComponentModel.DataAnnotations;

namespace StellarBooks.DTOs
{
    public class CreateActivityDto
    {
        [Required]
        public int TaleId { get; set; }

        [Required, StringLength(50)]
        public string ActivityType { get; set; }

        public string Description { get; set; }

        [StringLength(255)]
        public string MultimediaResource { get; set; }
    }
}