using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace BDSA2020.Assignment03.Entities
{
    public partial class kanbanContext : DbContext
    {
        public kanbanContext()
        {
        }

        public kanbanContext(DbContextOptions<kanbanContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Tag> Tag { get; set; }
        public virtual DbSet<Task> Task { get; set; }
        public virtual DbSet<TaskTag> TaskTag { get; set; }
        public virtual DbSet<User> User { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("Server=.;Database=kanban;User Id=sa;Password=beecf3da-ff67-4b42-93ed-2f2bb57e917a");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Tag>(entity =>
            {
                entity.HasIndex(e => e.Name)
                    .HasName("UQ__Tag__737584F6A119F308")
                    .IsUnique();

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Name).IsUnicode(false);
            });

            modelBuilder.Entity<Task>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Description).IsUnicode(false);

                entity.Property(e => e.State).IsUnicode(false);

                entity.Property(e => e.Title).IsUnicode(false);

                entity.HasOne(d => d.AssignedToNavigation)
                    .WithMany(p => p.Task)
                    .HasForeignKey(d => d.AssignedTo)
                    .HasConstraintName("fk_user_tasks");
            });

            modelBuilder.Entity<TaskTag>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.HasOne(d => d.TagNavigation)
                    .WithMany(p => p.TaskTag)
                    .HasForeignKey(d => d.Tag)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__TaskTag__tag__2E1BDC42");

                entity.HasOne(d => d.TaskNavigation)
                    .WithMany(p => p.TaskTag)
                    .HasForeignKey(d => d.Task)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__TaskTag__task__2D27B809");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(e => e.Email)
                    .HasName("UQ__User__A9D1053435401C23")
                    .IsUnique();

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Email).IsUnicode(false);

                entity.Property(e => e.Name).IsUnicode(false);
            });

            modelBuilder.Entity<Tag>()
                .HasData(
                    new Tag{
                        Id = 1,
                        Name = "Easy",
                    },
                    new Tag{
                        Id = 2,
                        Name = "Medium",
                    },
                    new Tag{
                        Id = 3,
                        Name = "Hard",
                    },
                    new Tag{
                        Id = 4,
                        Name = "Programming",
                    },
                    new Tag{
                        Id = 5,
                        Name = "Admin work",
                    },
                    new Tag{
                        Id = 6,
                        Name = "Setup",
                    },
                    new Tag{
                        Id = 7,
                        Name = "Cleaning",
                    }
                )
            ;

            modelBuilder.Entity<Task>()
                .HasData(
                    new Task{
                        Id = 1,
                        Title = "Clean up code",
                        Description = "deploy global platforms",
                        AssignedTo = 1,
                        State = State.New
                    },
                    new Task{
                        Id = 2,
                        Title = "Program good stuff",
                        Description = "deploy best-of-breed niches",
                        State = State.New
                    },
                    new Task{
                        Id = 3,
                        Title = "Make citations",
                        Description = "disintermediate 24/365 niches",
                        State = State.Active
                    },
                    new Task{
                        Id = 4,
                        Title = "Cook dinner",
                        Description = "productize integrated infomediaries",
                        State = State.Active
                    },
                    new Task{
                        Id = 5,
                        Title = "Save the world",
                        Description = "monetize customized experiences",
                        State = State.Resolved
                    },
                    new Task{
                        Id = 6,
                        Title = "Destroy the world",
                        Description = "incubate killer web-readiness",
                        State = State.Resolved
                    },
                    new Task{
                        Id = 7,
                        Title = "Become famous",
                        Description = "deploy visionary e-business",
                        AssignedTo = 2,
                        State = State.Closed
                    },
                    new Task{
                        Id = 8,
                        Title = "Delete the sun",
                        State = State.Closed
                    },
                    new Task{
                        Id = 9,
                        Title = "Assassinate trump",
                        Description = "enable 24/7 architectures",
                        AssignedTo = 2,
                        State = State.Removed
                    },
                    new Task{
                        Id = 10,
                        Title = "Make america great again",
                        AssignedTo = 3,
                        State = State.Removed
                    }
                )
            ;

            modelBuilder.Entity<TaskTag>()
                .HasData(
                    new TaskTag{
                        Id = 1,
                        Task = 1,
                        Tag = 3
                    },
                    new TaskTag{
                        Id = 2,
                        Task = 1,
                        Tag = 4
                    },
                    new TaskTag{
                        Id = 3,
                        Task = 1,
                        Tag = 7
                    },
                    new TaskTag{
                        Id = 4,
                        Task = 2,
                        Tag = 1
                    },
                    new TaskTag{
                        Id = 5,
                        Task = 2,
                        Tag = 4
                    },
                    new TaskTag{
                        Id = 6,
                        Task = 5,
                        Tag = 1
                    },
                    new TaskTag{
                        Id = 7,
                        Task = 9,
                        Tag = 3
                    },
                    new TaskTag{
                        Id = 8,
                        Task = 9,
                        Tag = 7
                    },
                    new TaskTag{
                        Id = 9,
                        Task = 9,
                        Tag = 5
                    },
                    new TaskTag{
                        Id = 10,
                        Task = 10,
                        Tag = 1
                    }
                )
            ;

            modelBuilder.Entity<User>()
                .HasData(
                    new User{
                        Id = 1, 
                        Name = "Tania Poxson", 
                        Email = "tpoxson0@gnu.org"
                    },
                    new User{
                        Id = 2, 
                        Name = "Janos Larraway", 
                        Email = "jlarraway1@huffingtonpost.com"
                    },
                    new User{
                        Id = 3, 
                        Name = "Ingram Byre", 
                        Email = "ibyre2@dyndns.org"
                    }
                );
            ;

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
