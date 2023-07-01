using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Assignment05.Entities
{
    public interface IKanbanContext
    {
        DbSet<Task> Tasks { get; set; }
        DbSet<Tag> Tags { get; set; }
        DbSet<TaskTag> TaskTags { get; set; }
        DbSet<User> Users { get; set; }

        int SaveChanges();

        //self added not sure if allowed ?
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
