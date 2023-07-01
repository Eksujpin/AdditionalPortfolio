using System;
using System.Collections.Generic;
using System.Linq;
using BDSA2020.Assignment04.Entities;
using static BDSA2020.Assignment04.Models.Response;


namespace BDSA2020.Assignment04.Models
{
    public class TaskRepository : ITaskRepository
    {
        private readonly KanbanContext _context;
        public TaskRepository(KanbanContext context)
        {
            _context = context;
        }
        public (Response response, int taskId) Create(TaskCreateDTO task)
        {
            var lastId = _context.Tasks.Select(t => t.Id).Count();
            var titleList = _context.Tasks.Select(t => t.Title).ToList();
            if (titleList.Contains(task.Title))
            {
                var index = _context.Tasks.Where(t => t.Title == task.Title).Select(t => t.Id).Single();
                return (Conflict, index);
            }
            Task newTask = new Task
            {
                Id = lastId + 1,
                Title = task.Title,
                State = State.New,
                Description = task.Description
                //Tags = M.I.A 
            };

            if (task.AssignedToId.HasValue)
            {
                if (!_context.Users.Select(t => t.Id).ToList().Contains(task.AssignedToId.Value))
                {
                    return (BadRequest, task.AssignedToId.Value);
                }
                newTask.AssignedToId = task.AssignedToId;
                newTask.AssignedTo = _context.Users.Where(t => t.Id == task.AssignedToId).Single();
            }
            _context.Tasks.Add(newTask);
            return (Created, newTask.Id);
        }

        public IQueryable<TaskListDTO> Read(bool includeRemoved = false)
        {
            if (_context.Tasks.Count() <= 0) return null;
            if (includeRemoved)
            {
                return _context.Tasks.Select(t => new TaskListDTO
                {
                    Id = t.Id,
                    Title = t.Title,
                    AssignedToId = t.AssignedToId,
                    AssignedToName = t.AssignedTo.Name,
                    State = t.State,
                    //Tags = M.I.A 
                }).AsQueryable();
            }
            return _context.Tasks.Where(t => t.State != State.Removed).Select(t => new TaskListDTO
            {
                Id = t.Id,
                Title = t.Title,
                AssignedToId = t.AssignedToId,
                AssignedToName = t.AssignedTo.Name,
                State = t.State,
                //Tags = M.I.A 
            }).AsQueryable();
        }
        public TaskDetailsDTO Read(int taskId)
        {
            var idList = _context.Tasks.Select(t => t.Id).ToList();
            if (idList.Contains(taskId))
            {
                var task = _context.Tasks.Where(t => t.Id == taskId).First();
                var res = new TaskDetailsDTO
                {
                    Id = task.Id,
                    Title = task.Title,
                    AssignedToId = task.AssignedToId,
                    State = task.State,
                    Description = task.Description
                };
                if (task.AssignedTo != null) res.AssignedToName = task.AssignedTo.Name;
                if (task.AssignedToId == null && task.AssignedTo != null) res.AssignedToId = task.AssignedTo.Id;
                if (task.AssignedTo == null && task.AssignedToId != null) res.AssignedToName = _context.Users.Where(t => t.Id == task.AssignedToId).Select(t => t.Name).Single();
                return res;
            }
            return null;
        }

        public Response Update(TaskUpdateDTO task)
        {
            var idList = _context.Tasks.Select(t => t.Id).ToList();
            if (idList.Contains(task.Id))
            {
                var oldTask = _context.Tasks.Where(t => t.Id == task.Id).First();
                var newTask = new Task
                {
                    Id = task.Id,
                    State = task.State,
                    Description = task.Description,
                    AssignedToId = task.AssignedToId,
                    //Tags = M.I.A 
                };

                if (task.Title != null) newTask.Title = task.Title;
                else newTask.Title = oldTask.Title;

                if (newTask.AssignedToId.HasValue)
                {
                    if (_context.Users.Select(u => u.Id).ToList().Contains(newTask.AssignedToId.Value))
                    {
                        newTask.AssignedTo = _context.Users.Where(t => t.Id == task.AssignedToId).Single();
                        _context.Remove(oldTask);
                        _context.Tasks.Add(newTask);
                        return Updated;
                    }
                    return BadRequest;
                }
                _context.Remove(oldTask);
                _context.Tasks.Add(newTask);
                return Updated;
            }
            return NotFound;
        }

        public Response Delete(int taskId)
        {
            var taskList = _context.Tasks.Select(t => t.Id).ToList();

            if (taskList.Contains(taskId))
            {
                var task = _context.Tasks.Where(t => t.Id == taskId).First();
                if (task.State == State.New)
                {
                    _context.Tasks.Remove(task);
                    return Deleted;
                }
                if (task.State == State.Active)
                {
                    task.State = State.Removed;
                    return Updated;
                }
                if (task.State == State.Resolved
                || task.State == State.Removed
                || task.State == State.Closed)
                {
                    return Conflict;
                }
            }
            return NotFound;
        }
    }

}
