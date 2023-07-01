using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Assignment06.Entities
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [EmailAddress]
        [Required]
        [StringLength(50)]
        public string EmailAddress { get; set; }

        public ICollection<Task> Tasks { get; set; }
    }
}
