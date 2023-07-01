using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Assignment06.Entities
{
    public class Tag
    {
        public int Id { get; set; }

        [Required]
        [StringLength(250)]
        public string Name { get; set; }

        public ICollection<TaskTag> Tasks { get; set; }
    }
}
