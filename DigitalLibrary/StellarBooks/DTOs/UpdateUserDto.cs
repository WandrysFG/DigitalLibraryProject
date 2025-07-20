using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using StellarBooks.DTOs;

namespace StellarBooks.DTOs
{
    public class UpdateUserDto : CreateUserDto
    {
        public int Id { get; set; }
    }
}