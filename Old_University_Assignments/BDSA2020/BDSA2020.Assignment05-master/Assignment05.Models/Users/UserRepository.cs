using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Assignment05.Entities;
using Microsoft.EntityFrameworkCore;
using static Assignment05.Entities.State;
using static Assignment05.Models.Response;
namespace Assignment05.Models.Users
{
    public class UserRepository : IUserRepository
    {
        private readonly IKanbanContext _context;
        public UserRepository(IKanbanContext context)
        {
            _context = context;
        }
        public async Task<(Response response, int userId)> CreateAsync(UserCreateDTO user)
        {
            var lastId = _context.Users.Select(t => t.Id).Count();
            var userList = _context.Users.Select(t => t.Name).ToList();
            if (userList.Contains(user.Name))
            {
                var index = _context.Users.Where(t => t.Name == user.Name).Select(t => t.Id).Single();
                return (Conflict, index);
            }
            var entity = new User
            {
                Id = lastId + 1,
                EmailAddress = user.EmailAddress,
                Name = user.Name,
            };
            _context.Users.Add(entity);
            await _context.SaveChangesAsync();
            return (Created, entity.Id);
        }
        public async Task<ICollection<UserListDTO>> ReadAsync()
        {
            var users = from u in _context.Users
                        select new UserListDTO
                        {
                            Id = u.Id,
                            Name = u.Name,
                            EmailAddress = u.EmailAddress,
                        };
            if (users.Count() <= 0) return null;
            return await users.ToListAsync();
        }
        public async Task<UserDetailsDTO> ReadAsync(int userId)
        {
            var users = from u in _context.Users
                        where u.Id == userId
                        select new UserDetailsDTO
                        {
                            Id = u.Id,
                            Name = u.Name,
                            EmailAddress = u.EmailAddress,
                            //Tasks = M.I.A
                        };
            return await users.FirstOrDefaultAsync();
        }

        public async Task<Response> UpdateAsync(UserUpdateDTO user)
        {
            var entity = _context.Users.FirstOrDefault(t => t.Id == user.Id);
            if (entity == null) return NotFound;
            entity.Id = user.Id;
            entity.Name = user.Name;
            entity.EmailAddress = user.EmailAddress;
            await _context.SaveChangesAsync();
            return Updated;
        }
        public async Task<Response> DeleteAsync(int userId, bool force = false)
        {
            var entity = await _context.Users.FindAsync(userId);

            if (entity == null)
            {
                return NotFound;
            }

            if (entity.Tasks == null)
            {
                foreach (Entities.Task task in _context.Tasks)
                {
                    if (task.AssignedToId == userId) { task.AssignedToId = null; task.AssignedTo = null; }
                }
                _context.Users.Remove(entity);
                await _context.SaveChangesAsync();
                return Deleted;
            }

            if (entity.Tasks.Count() <= 0 || force)
            {
                foreach (Entities.Task task in _context.Tasks)
                {
                    if (task.AssignedToId == userId) { task.AssignedToId = null; task.AssignedTo = null; }
                }
                entity.Tasks = null; 
                _context.Users.Remove(entity);
                await _context.SaveChangesAsync();
                return Deleted;
            }
            return Conflict;
        }
    }
}