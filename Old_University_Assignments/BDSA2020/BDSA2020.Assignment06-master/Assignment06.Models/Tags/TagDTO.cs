namespace Assignment06.Models
{
    public class TagDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }

        // Number of tasks with this tag, which are New
        public int New { get; set; }

        // Number of tasks with this tag, which are Active
        public int Active { get; set; }

        // Number of tasks with this tag, which are Resolved
        public int Resolved { get; set; }

        // Number of tasks with this tag, which are Closed
        public int Closed { get; set; }

        // Number of tasks with this tag, which are Removed
        public int Removed { get; set; }
    }
}
