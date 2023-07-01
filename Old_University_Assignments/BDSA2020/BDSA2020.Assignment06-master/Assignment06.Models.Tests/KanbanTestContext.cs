using Microsoft.EntityFrameworkCore;
using Assignment06.Entities;
using static Assignment06.Entities.State;

namespace Assignment06.Models.Tests
{
    public class KanbanTestContext : KanbanContext
    {
        public KanbanTestContext(DbContextOptions<KanbanContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>().HasData(
                new User { Id = 1, Name = "user1", EmailAddress = "user1@kanban.com" },
                new User { Id = 2, Name = "user2", EmailAddress = "user2@kanban.com" }
            );

            modelBuilder.Entity<Tag>().HasData(
                new Tag { Id = 1, Name = "tag1" },
                new Tag { Id = 2, Name = "tag2" },
                new Tag { Id = 3, Name = "tag3" }
            );

            modelBuilder.Entity<Task>().HasData(
                new Task { Id = 1, Title = "title1", Description = "description1", AssignedToId = null, State = New },
                new Task { Id = 2, Title = "title2", Description = "description2", AssignedToId = 1, State = Active },
                new Task { Id = 3, Title = "title3", Description = "description3", AssignedToId = 2, State = Resolved },
                new Task { Id = 4, Title = "title4", Description = "description4", AssignedToId = 1, State = Closed },
                new Task { Id = 5, Title = "title5", Description = "description5", AssignedToId = 2, State = Removed }
            );

            modelBuilder.Entity<TaskTag>().HasData(
                new TaskTag { TaskId = 2, TagId = 1 },
                new TaskTag { TaskId = 2, TagId = 2 },
                new TaskTag { TaskId = 3, TagId = 1 },
                new TaskTag { TaskId = 4, TagId = 2 }
            );
        }
    }
}
