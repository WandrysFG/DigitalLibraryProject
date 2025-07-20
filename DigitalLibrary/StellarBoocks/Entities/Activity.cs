using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace StellarBoocks.Entities
{
    [Table("Activities")]
    public class Activity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int TaleId { get; set; }

        [ForeignKey("TaleId")]
        public Tale Tale { get; set; }

        [Required, StringLength(50)]
        public string ActivityType { get; set; }

        public string Description { get; set; }

        [StringLength(255)]
        public string MultimediaResource { get; set; }
    }
}
