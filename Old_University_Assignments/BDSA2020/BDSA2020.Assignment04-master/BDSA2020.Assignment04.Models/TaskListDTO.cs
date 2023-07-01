using System.Collections.Generic;
using BDSA2020.Assignment04.Entities;

namespace BDSA2020.Assignment04.Models
{
    public class TaskListDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }

        public int? AssignedToId { get; set; }

        public string AssignedToName { get; set; }

        public State State { get; set; }

        public ICollection<KeyValuePair<int, string>> Tags { get; set; }
    }
}