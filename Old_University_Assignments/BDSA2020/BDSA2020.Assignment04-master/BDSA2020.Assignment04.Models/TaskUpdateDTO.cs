using BDSA2020.Assignment04.Entities;

namespace BDSA2020.Assignment04.Models
{
    public class TaskUpdateDTO : TaskCreateDTO
    {
        public int Id { get; set; }

        public State State { get; set; }
    }
}