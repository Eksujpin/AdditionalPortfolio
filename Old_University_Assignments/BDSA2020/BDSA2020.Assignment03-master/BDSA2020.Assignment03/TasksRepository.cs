using BDSA2020.Assignment03.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
namespace BDSA2020.Assignment03
{
    public class TasksRepository : IDisposable
    {
        private readonly kanbanContext _context;

        public TasksRepository(kanbanContext context)
        {
            _context = context;
        
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="task"></param>
        /// <returns>The id of the newly created task</returns>
        public int Create(TaskDTO task)
        {
            Task newTask = new Task{
                Id = task.Id,
                Title = task.Title,
                AssignedTo = task.AssignedToId,
                Description = task.Description,
                State = task.State
            };

            return newTask.Id;
        }

        public TaskDetailsDTO FindById(int id)
        {
           var task = _context.Task.Where(t => t.Id == id).First();
           var assignedUser = _context.User.Where(u => u.Id == task.AssignedTo).First();
           var tagId = _context.TaskTag.Where(tg => tg.Task == task.Id).Select(tg => tg.Tag).ToList();
           var tagName = _context.Tag.Where(tmp => tagId.Contains(id)).Select(t => t.Name).ToList();
           //var tagStr = _context.Tag.Where(t => t.Id.)
            return new TaskDetailsDTO{
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                AssignedToId = task.AssignedTo,
                AssignedToName = assignedUser.Name,
                AssignedToEmail = assignedUser.Email,
                State = task.State,
                Tags = tagName
            };
        }

        public ICollection<TaskDTO> All()
        {
            //ICollection<TaskDTO> dtos = new List<TaskDTO>();
            return _context.Task.Select(t => new TaskDTO{
                Id = t.Id,
                Title = t.Title,
                Description = t.Description,
                AssignedToId = t.AssignedTo,
                Tags = t.TaskTag.Select(ts => ts.TagNavigation.Name).ToList(),
                State = t.State
            }).ToList();
            /*
            foreach(Task t in _context.Task){
                //this gives an System.InvalidOperationException : There is already an open DataReader associated with this Connection which must be closed first.
                var tagId = _context.TaskTag.Where(tg => tg.Task == t.Id).Select(tg => tg.Tag).ToList();
                var tagName = _context.Tag.Where(tmp => tagId.Contains(t.Id)).Select(t => t.Name).ToList();
                TaskDTO dto = new TaskDTO {
                    Id = t.Id,
                    Title = t.Title,
                    Description = t.Description,
                    AssignedToId = t.AssignedTo,
                    Tags = null,
                    State = t.State
                };
                dtos.Add(dto);
                
            }
            */
        }

        public void Update(TaskDTO task)
        {
            throw new NotImplementedException();
        }

        public void Delete(int taskId)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
