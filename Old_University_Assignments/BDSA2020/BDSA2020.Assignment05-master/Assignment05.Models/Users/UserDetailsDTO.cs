using System.Collections.Generic;
using Assignment05.Entities;

namespace Assignment05.Models
{
    public class UserDetailsDTO : UserListDTO
    {
        public ICollection<UserTaskDTO> Tasks { get; set; } = new HashSet<UserTaskDTO>();
    }
}
