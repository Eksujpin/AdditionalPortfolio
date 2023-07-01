using System.Collections.Generic;

namespace BDSA2020.Assignment03
{
    public class TaskDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int? AssignedToId { get; set; }
        public ICollection<string> Tags { get; set; }
        public State State { get; set; }
    }
}