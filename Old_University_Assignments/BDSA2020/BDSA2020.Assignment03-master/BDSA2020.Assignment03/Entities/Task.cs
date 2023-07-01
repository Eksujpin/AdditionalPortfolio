using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BDSA2020.Assignment03
{
    public partial class Task
    {
        public Task()
        {
            TaskTag = new HashSet<TaskTag>();
        }

        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(100)]
        public string Title { get; set; }
        public int? AssignedTo { get; set; }
        public string Description { get; set; }
        [Required]
        [StringLength(8)]
        public State State { get; set; }

        [ForeignKey(nameof(AssignedTo))]
        [InverseProperty(nameof(User.Task))]
        public virtual User AssignedToNavigation { get; set; }
        [InverseProperty("TaskNavigation")]
        public virtual ICollection<TaskTag> TaskTag { get; set; }
    }
}
