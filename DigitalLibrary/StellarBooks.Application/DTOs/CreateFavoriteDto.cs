using System.ComponentModel.DataAnnotations;

namespace StellarBooks.Applications.DTOs
{
    public class CreateFavoriteDto
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        public int TaleId { get; set; }

        public DateTime DateAdded { get; set; } = DateTime.Today;

        //public List<UpdateUserDto>? Users { get; set; }
        //public List<UpdateTaleDto>? Tales { get; set; }
        //public UpdateUserDto User { get; set; }
        //public UpdateTaleDto Tale { get; set; }

    }
}