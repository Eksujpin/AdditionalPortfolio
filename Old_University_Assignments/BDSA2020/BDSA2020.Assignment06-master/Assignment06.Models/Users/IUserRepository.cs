using System.Linq;
using System.Threading.Tasks;

namespace Assignment06.Models
{
    public interface IUserRepository
    {
        Task<(Status response, int taskId)> CreateAsync(UserCreateDTO user);
        IQueryable<UserListDTO> ReadAsync();
        Task<UserDetailsDTO> ReadAsync(int userId);
        Task<Status> UpdateAsync(UserUpdateDTO user);
        Task<Status> DeleteAsync(int userId, bool force = false);
    }
}
