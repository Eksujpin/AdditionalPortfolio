using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Assignment06.Entities
{
    public class Task
    {
        public int Id { get; set; }

        [Required]
        [StringLength(250)]
        public string Title { get; set; }

        public int? AssignedToId { get; set; }

        public User AssignedTo { get; set; }

        public State State { get; set; }

        public string Description { get; set; }

        public ICollection<TaskTag> Tags { get; set; }
    }
}
