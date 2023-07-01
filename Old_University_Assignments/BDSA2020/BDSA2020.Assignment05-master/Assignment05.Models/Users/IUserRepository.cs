using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Assignment05.Models
{
    public interface IUserRepository
    {
        Task<(Response response, int userId)> CreateAsync(UserCreateDTO user);
        Task<ICollection<UserListDTO>> ReadAsync();
        Task<UserDetailsDTO> ReadAsync(int userId);
        Task<Response> UpdateAsync(UserUpdateDTO user);
        Task<Response> DeleteAsync(int userId, bool force = false);
    }
}
