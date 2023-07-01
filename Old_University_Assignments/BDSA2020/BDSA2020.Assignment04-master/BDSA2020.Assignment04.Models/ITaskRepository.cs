using System.Linq;

namespace BDSA2020.Assignment04.Models
{
    public interface ITaskRepository
    {
        (Response response, int taskId) Create(TaskCreateDTO task);
        IQueryable<TaskListDTO> Read(bool includeRemoved = false);
        TaskDetailsDTO Read(int taskId);
        Response Update(TaskUpdateDTO task);
        Response Delete(int taskId);
    }
}
