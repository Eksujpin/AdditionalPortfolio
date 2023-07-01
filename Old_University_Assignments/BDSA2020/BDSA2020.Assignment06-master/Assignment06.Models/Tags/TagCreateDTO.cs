using System.ComponentModel.DataAnnotations;

namespace Assignment06.Models
{
    public class TagCreateDTO
    {
        [Required]
        [RegularExpression(@"[a-zA-Z0-9-]+")]
        [StringLength(50)]
        public string Name { get; set; }
    }
}
