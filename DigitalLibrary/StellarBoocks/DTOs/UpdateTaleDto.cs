using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace StellarBoocks.DTOs
{
    public class UpdateTaleDto : CreateTaleDto
    {
        public int Id { get; set; }
    }
}
