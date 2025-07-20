using System;
using System.ComponentModel.DataAnnotations;

namespace StellarBoocks.DTOs
{
    public class CreateFavoriteDto
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        public int TaleId { get; set; }

        public DateTime DateAdded { get; set; } = DateTime.Today;
    }
}