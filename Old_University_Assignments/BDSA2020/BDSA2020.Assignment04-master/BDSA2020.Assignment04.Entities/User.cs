using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BDSA2020.Assignment04.Entities
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [EmailAddress]
        [Required]
        [StringLength(100)]
        public string EmailAddress { get; set; }

        public ICollection<Task> Tasks { get; set; }
    }
}