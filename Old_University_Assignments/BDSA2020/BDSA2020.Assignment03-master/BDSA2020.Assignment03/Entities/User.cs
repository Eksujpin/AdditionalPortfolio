using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BDSA2020.Assignment03
{
    public partial class User
    {
        public User()
        {
            Task = new HashSet<Task>();
        }

        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(100)]
        public string Name { get; set; }
        [Required]
        [StringLength(100)]
        public string Email { get; set; }

        [InverseProperty("AssignedToNavigation")]
        public virtual ICollection<Task> Task { get; set; }
    }
}
