using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace StellarBooks.Applications.DTOs
{
    public class UpdateTaleDto : CreateTaleDto
    {
        public int Id { get; set; }
    }
}
