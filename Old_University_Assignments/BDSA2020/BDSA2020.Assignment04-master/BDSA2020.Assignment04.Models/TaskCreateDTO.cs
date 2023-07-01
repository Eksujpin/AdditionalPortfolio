using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BDSA2020.Assignment04.Models
{
    public class TaskCreateDTO
    {
        [Required]
        [StringLength(100)]
        public string Title { get; set; }

        public int? AssignedToId { get; set; }

        public string Description { get; set; }

        public ICollection<string> Tags { get; set; }
    }
}