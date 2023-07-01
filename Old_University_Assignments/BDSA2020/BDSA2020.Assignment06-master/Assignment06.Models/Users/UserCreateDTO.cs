using System.ComponentModel.DataAnnotations;

namespace Assignment06.Models
{
    public class UserCreateDTO
    {
        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [EmailAddress]
        [Required]
        [StringLength(50)]
        public string EmailAddress { get; set; }
    }
}
