using System.Collections.Generic;
using Assignment06.Entities;

namespace Assignment06.Models
{
    public class UserDetailsDTO : UserListDTO
    {
        public ICollection<UserTaskDTO> Tasks { get; set; } = new HashSet<UserTaskDTO>();
    }
}
