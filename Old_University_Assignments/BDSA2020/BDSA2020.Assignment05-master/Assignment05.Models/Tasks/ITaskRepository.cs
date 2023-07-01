using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Assignment05.Models
{
    public interface ITaskRepository
    {
        Task<(Response response, int userId)> Create(TaskCreateDTO task);
        Task<ICollection<TaskListDTO>> Read(bool includeRemoved = false);
        Task<TaskDetailsDTO> Read(int taskId);
        Task<Response> Update(TaskUpdateDTO task);
        Task<Response> Delete(int taskId);
    }
}
