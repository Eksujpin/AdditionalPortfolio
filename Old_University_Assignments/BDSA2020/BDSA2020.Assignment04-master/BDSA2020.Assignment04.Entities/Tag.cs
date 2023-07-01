using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BDSA2020.Assignment04.Entities
{
    public class Tag
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        public ICollection<TaskTag> Tasks { get; set; }
    }
}