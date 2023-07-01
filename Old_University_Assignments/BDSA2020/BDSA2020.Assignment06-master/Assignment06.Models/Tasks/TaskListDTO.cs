using System.Collections.Generic;
using System.Linq;
using Assignment06.Entities;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Assignment06.Models.Tests")]

namespace Assignment06.Models
{
    public class TaskListDTO
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public int? AssignedToId { get; set; }

        public string AssignedToName { get; set; }

        public State State { get; set; }

        public IEnumerable<KeyValuePair<int, string>> Tags { get; set; } = new Dictionary<int, string>();

        /// <summary>
        /// Required as .ToDictionary() is not allowed in an Entity Framework Core Linq query
        /// </summary>
        internal IEnumerable<KeyValuePair<int, string>> InnerTags
        {
            set => Tags = value.ToDictionary(t => t.Key, t => t.Value);
        }
    }
}
