using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace StellarBooks.DTOs
{
    public class UpdateActivityDto : CreateActivityDto
    {
        public int Id { get; set; }
    }
}
