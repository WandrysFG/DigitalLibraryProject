using System;
using System.ComponentModel.DataAnnotations;

namespace StellarBooks.DTOs
{
    public class CreateTaleDto
    {
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

        public DateTime PublicationDate { get; set; } = DateTime.Today;
    }
}