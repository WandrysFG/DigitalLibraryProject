using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using StellarBoocks.DTOs;

namespace StellarBoocks.DTOs
{
    public class UpdateUserDto : CreateUserDto
    {
        public int Id { get; set; }
    }
}
