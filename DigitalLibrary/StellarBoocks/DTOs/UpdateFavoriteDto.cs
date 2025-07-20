using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace StellarBoocks.DTOs
{
    public class UpdateFavoriteDto : CreateFavoriteDto
    {
        public int Id { get; set; }
    }
}
