using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace StellarBooks.DTOs
{
    public class UpdateTaleDto : CreateTaleDto
    {
        public int Id { get; set; }
    }
}
