using Assignment05.Entities;

namespace Assignment05.Models
{
    public class TaskUpdateDTO : TaskCreateDTO
    {
        public int Id { get; set; }
        public State State { get; set; }
    }
}
