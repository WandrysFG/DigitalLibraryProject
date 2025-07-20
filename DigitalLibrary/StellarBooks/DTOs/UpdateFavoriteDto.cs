using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace StellarBooks.DTOs
{
    public class UpdateFavoriteDto : CreateFavoriteDto
    {
        public int Id { get; set; }
    }
}