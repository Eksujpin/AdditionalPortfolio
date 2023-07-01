using System.Collections.Generic;
using System.Linq;
using Assignment05.Entities;

namespace Assignment05.Models
{
    public class TaskListDTO
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public int? AssignedToId { get; set; }

        public string AssignedToName { get; set; }

        public State State { get; set; }

        public IDictionary<int, string> Tags { get; set; } = new Dictionary<int, string>();

        /// <summary>
        /// Required as .ToDictionary() is not allowed in an Entity Framework Core Linq query
        /// </summary>
        internal IEnumerable<KeyValuePair<int, string>> InnerTags
        {
            set => Tags = value.ToDictionary(t => t.Key, t => t.Value);
        }
    }
}
