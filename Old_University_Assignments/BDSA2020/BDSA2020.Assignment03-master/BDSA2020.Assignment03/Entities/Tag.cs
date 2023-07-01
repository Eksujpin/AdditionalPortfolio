using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BDSA2020.Assignment03
{
    public partial class Tag
    {
        public Tag()
        {
            TaskTag = new HashSet<TaskTag>();
        }

        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [InverseProperty("TagNavigation")]
        public virtual ICollection<TaskTag> TaskTag { get; set; }
    }
}
