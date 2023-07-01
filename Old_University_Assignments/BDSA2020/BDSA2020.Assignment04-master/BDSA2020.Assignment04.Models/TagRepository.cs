using System;
using System.Collections.Generic;
using System.Linq;
using BDSA2020.Assignment04.Entities;
using static BDSA2020.Assignment04.Models.Response;

namespace BDSA2020.Assignment04.Models
{
    public class TagRepository : ITagRepository
    {
        private readonly KanbanContext _context;
        public TagRepository(KanbanContext context)
        {
            _context = context;
        }
        
        public (Response response, int tagId) Create(TagCreateDTO tag)
        {
            var lastId = _context.Tags.Select(t => t.Id).Count();
            var nameList = _context.Tags.Select(t => t.Name).ToList();

            if (nameList.Contains(tag.Name))
            {
                var index = _context.Tags.Where(t => t.Name == tag.Name).Select(t => t.Id).Single();
                return (Conflict, index);
            }
            Tag newTag = new Tag
            {
                Id = lastId + 1,
                Name = tag.Name,
            };
            _context.Tags.Add(newTag);
            return (Created, lastId + 1);

        }
       
        public IQueryable<TagDTO> Read()
        {
            if (_context.Tags.Count() <= 0) return null;
            return _context.Tags.Select(t => new TagDTO { Id = t.Id, Name = t.Name }).AsQueryable();
        }

        public TagDTO Read(int tagId)
        {
            var idList = _context.Tags.Select(t => t.Id).ToList();
            if (idList.Contains(tagId))
            {
                var tag = _context.Tags.Where(t => t.Id == tagId).First();
                return new TagDTO
                {
                    Id = tag.Id,
                    Name = tag.Name,
                    // might need to add new, closed, removed mm.
                };
            }
            return null;
        }
        public Response Update(TagUpdateDTO tag)
        {
            var idList = _context.Tags.Select(t => t.Id).ToList();
            if (idList.Contains(tag.Id))
            {
                //99% sure there is a smatter way to do this, but idk that way
                var oldTag = _context.Tags.Where(t => t.Id == tag.Id).First();
                _context.Tags.Remove(oldTag);
                oldTag.Name = tag.Name;
                _context.Tags.Add(oldTag);
                return Updated;
            }
            return NotFound;
        }

        public Response Delete(int tagId, bool force = false)
        {
            var idList = _context.Tags.Select(t => t.Id).ToList();
            if (idList.Contains(tagId))
            {
                Tag tag = _context.Tags.Where(t => t.Id == tagId).First();
                int taskCount = tag.Tasks.Count;
                if (taskCount > 0 && force || taskCount <= 0)
                {
                    _context.Tags.Remove(tag);
                    return Deleted;
                }
                else if (taskCount > 0 && !force)
                {
                    return Conflict;
                }
            }
            return NotFound;
        }
    }

}