using Assignment06.Entities;

namespace Assignment06.Models
{
    public class TaskUpdateDTO : TaskCreateDTO
    {
        public int Id { get; set; }
        public State State { get; set; }
    }
}
