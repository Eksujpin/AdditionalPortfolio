using Assignment06.Entities;

namespace Assignment06.Models
{
    public class UserTaskDTO
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public State State { get; set; }
    }
}
