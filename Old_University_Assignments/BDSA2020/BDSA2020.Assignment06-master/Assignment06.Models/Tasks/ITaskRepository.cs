using System.Linq;
using System.Threading.Tasks;

namespace Assignment06.Models
{
    public interface ITaskRepository
    {
        Task<(Status response, int createdId)> Create(TaskCreateDTO task);
        IQueryable<TaskListDTO> Read(bool includeRemoved = false);
        Task<TaskDetailsDTO> Read(int taskId);
        Task<Status> Update(TaskUpdateDTO task);
        Task<Status> Delete(int taskId);
    }
}
