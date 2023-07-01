using Assignment05.Entities;

namespace Assignment05.Models
{
    public class UserTaskDTO
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public State State { get; set; }
    }
}
