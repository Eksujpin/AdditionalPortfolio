using System.ComponentModel.DataAnnotations;

namespace BDSA2020.Assignment04.Models
{
    public class TagCreateDTO
    {
        [Required]
        [StringLength(50)]
        public string Name { get; set; }
    }
}