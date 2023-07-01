using System.Linq;
using System.Threading.Tasks;
using Assignment06.Entities;
using Microsoft.EntityFrameworkCore;
using static Assignment06.Entities.State;
using static Assignment06.Models.Status;

namespace Assignment06.Models
{
    public class TagRepository : ITagRepository
    {
        private readonly IKanbanContext _context;

        public TagRepository(IKanbanContext context)
        {
            _context = context;
        }

        public async Task<(Status response, int taskId)> Create(TagCreateDTO tag)
        {
            var tagExists = await _context.Tags.AnyAsync(t => t.Name == tag.Name);

            if (tagExists)
            {
                return (Conflict, 0);
            }

            var entity = new Tag
            {
                Name = tag.Name
            };

            _context.Tags.Add(entity);
            await _context.SaveChangesAsync();

            return (Created, entity.Id);
        }

        public IQueryable<TagDTO> Read()
        {
            return from t in _context.Tags
                   select new TagDTO
                   {
                       Id = t.Id,
                       Name = t.Name,
                       New = t.Tasks.Count(a => a.Task.State == New),
                       Active = t.Tasks.Count(a => a.Task.State == Active),
                       Resolved = t.Tasks.Count(a => a.Task.State == Resolved),
                       Closed = t.Tasks.Count(a => a.Task.State == Closed),
                       Removed = t.Tasks.Count(a => a.Task.State == New)
                   };
        }

        public async Task<TagDTO> Read(int tagId)
        {
            var tags = from t in _context.Tags
                       where t.Id == tagId
                       select new TagDTO
                       {
                           Id = t.Id,
                           Name = t.Name,
                           New = t.Tasks.Count(a => a.Task.State == New),
                           Active = t.Tasks.Count(a => a.Task.State == Active),
                           Resolved = t.Tasks.Count(a => a.Task.State == Resolved),
                           Closed = t.Tasks.Count(a => a.Task.State == Closed),
                           Removed = t.Tasks.Count(a => a.Task.State == New)
                       };

            return await tags.FirstOrDefaultAsync();
        }

        public async Task<Status> Update(TagUpdateDTO tag)
        {
            var tagExists = await _context.Tags.AnyAsync(t => t.Id != tag.Id && t.Name == tag.Name);

            if (tagExists)
            {
                return Conflict;
            }

            var entity = await _context.Tags.FindAsync(tag.Id);

            if (entity == null)
            {
                return NotFound;
            }

            entity.Name = tag.Name;

            await _context.SaveChangesAsync();

            return Updated;
        }

        public async Task<Status> Delete(int tagId, bool force = false)
        {
            var entity = await _context.Tags.Include(t => t.Tasks).FirstOrDefaultAsync(t => t.Id == tagId);

            if (entity == null)
            {
                return NotFound;
            }

            if (!force && entity.Tasks.Any())
            {
                return Conflict;
            }

            _context.Tags.Remove(entity);

            await _context.SaveChangesAsync();

            return Deleted;
        }
    }
}
