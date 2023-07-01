using Microsoft.EntityFrameworkCore;

namespace Assignment06.Entities
{
    public class KanbanContext : DbContext, IKanbanContext
    {
        public DbSet<Task> Tasks { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<TaskTag> TaskTags { get; set; }
        public DbSet<User> Users { get; set; }

        public KanbanContext() { }

        public KanbanContext(DbContextOptions<KanbanContext> options)
            : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var connectionString = @"Server=(localdb)\MSSQLLocalDB;Database=Kanban;Trusted_Connection=True;MultipleActiveResultSets=true";

                optionsBuilder.UseSqlServer(connectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TaskTag>()
                        .HasKey(t => new { t.TaskId, t.TagId });


            modelBuilder.Entity<Tag>()
                        .HasIndex(t => t.Name)
                        .IsUnique();

            modelBuilder.Entity<User>()
                        .HasIndex(t => t.EmailAddress)
                        .IsUnique();
        }
    }
}
