using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Assignment06.Entities;
using Microsoft.EntityFrameworkCore;
using static Assignment06.Entities.State;
using static Assignment06.Models.Status;
using KanbanTask = Assignment06.Entities.Task;

namespace Assignment06.Models
{
    public class TaskRepository : ITaskRepository
    {
        private readonly IKanbanContext _context;

        public TaskRepository(IKanbanContext context)
        {
            _context = context;
        }

        public async Task<(Status response, int createdId)> Create(TaskCreateDTO task)
        {
            if (task.AssignedToId.HasValue && !await UserExists(task.AssignedToId.Value))
            {
                return (Conflict, 0);
            }

            var entity = new KanbanTask
            {
                Title = task.Title,
                Description = task.Description,
                AssignedToId = task.AssignedToId,
                Tags = await MapTags(0, task.Tags)
            };

            _context.Tasks.Add(entity);

            await _context.SaveChangesAsync();

            return (Created, entity.Id);
        }

        public async Task<TaskDetailsDTO> Read(int taskId)
        {
            var task = from t in _context.Tasks
                       where t.Id == taskId
                       select new TaskDetailsDTO
                       {
                           Id = t.Id,
                           Title = t.Title,
                           Description = t.Description,
                           AssignedToId = t.AssignedToId,
                           AssignedToName = t.AssignedTo.Name,
                           State = t.State,
                           InnerTags = t.Tags.Select(a => new KeyValuePair<int, string>(a.TagId, a.Tag.Name))
                       };

            return await task.FirstOrDefaultAsync();
        }

        public IQueryable<TaskListDTO> Read(bool includeRemoved = false)
        {
            return from t in _context.Tasks
                   where includeRemoved || t.State != Removed
                   select new TaskListDTO
                   {
                       Id = t.Id,
                       Title = t.Title,
                       AssignedToId = t.AssignedToId,
                       AssignedToName = t.AssignedTo.Name,
                       State = t.State,
                       InnerTags = t.Tags.Select(a => new KeyValuePair<int, string>(a.TagId, a.Tag.Name))
                   };
        }

        public async Task<Status> Update(TaskUpdateDTO task)
        {
            var entity = await _context.Tasks.Include(t => t.Tags).FirstOrDefaultAsync(t => t.Id == task.Id);

            if (entity == null)
            {
                return NotFound;
            }

            if (task.AssignedToId.HasValue && !await UserExists(task.AssignedToId.Value))
            {
                return (Conflict);
            }

            entity.Title = task.Title;
            entity.Description = task.Description;
            entity.AssignedToId = task.AssignedToId;
            entity.State = task.State;
            entity.Tags = await MapTags(task.Id, task.Tags);

            await _context.SaveChangesAsync();

            return Updated;
        }

        public async Task<Status> Delete(int taskId)
        {
            var entity = await _context.Tasks.FindAsync(taskId);

            if (entity == null)
            {
                return NotFound;
            }

            Status response;

            switch (entity.State)
            {
                case New:
                    _context.Tasks.Remove(entity);
                    response = Deleted;
                    break;
                case Active:
                    entity.State = Removed;
                    response = Deleted;
                    break;
                default:
                    response = Conflict;
                    break;
            }

            await _context.SaveChangesAsync();

            return response;
        }

        private Task<bool> UserExists(int userId) => _context.Users.AnyAsync(u => u.Id == userId);

        private async Task<ICollection<TaskTag>> MapTags(int taskId, IEnumerable<string> tags)
        {
            var taskTags = new List<TaskTag>();

            foreach (var tag in tags)
            {
                var entity = await _context.Tags.FirstOrDefaultAsync(t => t.Name == tag) ?? new Tag { Name = tag };

                taskTags.Add(new TaskTag { TaskId = taskId, Tag = entity });
            }

            return taskTags;
        }
    }
}
