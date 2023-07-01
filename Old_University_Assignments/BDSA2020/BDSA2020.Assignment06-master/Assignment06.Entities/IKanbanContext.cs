using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Assignment06.Entities
{
    public interface IKanbanContext : IDisposable
    {
        DbSet<Task> Tasks { get; set; }
        DbSet<Tag> Tags { get; set; }
        DbSet<TaskTag> TaskTags { get; set; }
        DbSet<User> Users { get; set; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
