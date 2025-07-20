using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace StellarBoocks.DTOs
{
    public class UpdateActivityDto : CreateActivityDto
    {
        public int Id { get; set; }
    }
}
