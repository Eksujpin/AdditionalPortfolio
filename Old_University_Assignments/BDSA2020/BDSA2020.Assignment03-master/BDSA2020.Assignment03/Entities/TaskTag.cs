using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BDSA2020.Assignment03
{
    public partial class TaskTag
    {
        [Key]
        public int Id { get; set; }
        [Column("task")]
        public int Task { get; set; }
        [Column("tag")]
        public int Tag { get; set; }

        [ForeignKey(nameof(Tag))]
        [InverseProperty("TaskTag")]
        public virtual Tag TagNavigation { get; set; }
        [ForeignKey(nameof(Task))]
        [InverseProperty("TaskTag")]
        public virtual Task TaskNavigation { get; set; }
    }
}
