using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Assignment06.Models
{
    public class TaskCreateDTO
    {
        [Required]
        [RegularExpression(@"[a-zA-Z0-9-]+")]
        [StringLength(250)]
        public string Title { get; set; }

        public int? AssignedToId { get; set; }

        public string Description { get; set; }

        public ICollection<string> Tags { get; set; } = new HashSet<string>();
    }
}
