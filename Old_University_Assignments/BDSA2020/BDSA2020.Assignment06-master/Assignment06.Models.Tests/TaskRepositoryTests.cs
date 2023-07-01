using System;
using System.Linq;
using Assignment06.Entities;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;
using static Assignment06.Entities.State;
using static Assignment06.Models.Status;
using Task = System.Threading.Tasks.Task;

namespace Assignment06.Models.Tests
{
    public class TaskRepositoryTests : IDisposable
    {
        private readonly SqliteConnection _connection;
        private readonly KanbanContext _context;
        private readonly TaskRepository _repository;

        public TaskRepositoryTests()
        {
            _connection = new SqliteConnection("Filename=:memory:");
            _connection.Open();
            var builder = new DbContextOptionsBuilder<KanbanContext>().UseSqlite(_connection);
            _context = new KanbanTestContext(builder.Options);
            _context.Database.EnsureCreated();
            _repository = new TaskRepository(_context);
        }

        [Fact]
        public async Task Create_returns_Created_with_id()
        {
            var task = new TaskCreateDTO
            {
                Title = "title",
                Description = "description"
            };

            var (response, id) = await _repository.Create(task);

            var created = _context.Tasks.Find(id);

            Assert.Equal(Created, response);
            Assert.Equal(id, created.Id);
        }

        [Fact]
        public async Task Create_creates_a_task_with_title_and_description_and_state_New()
        {
            var task = new TaskCreateDTO
            {
                Title = "title",
                Description = "description"
            };

            var (_, id) = await _repository.Create(task);

            var created = await _context.Tasks.FindAsync(id);

            Assert.Equal("title", created.Title);
            Assert.Equal("description", created.Description);
            Assert.Equal(New, created.State);
        }

        [Fact]
        public async Task Create_assigns_user()
        {
            var task = new TaskCreateDTO
            {
                Title = "title",
                Description = "description",
                AssignedToId = 1
            };

            var (_, id) = await _repository.Create(task);

            var created = _context.Tasks.Find(id);

            Assert.Equal(1, created.AssignedToId);
        }

        [Fact]
        public async Task Create_given_non_existing_user_returns_Conflict_and_id_0()
        {
            var task = new TaskCreateDTO
            {
                Title = "title",
                Description = "description",
                AssignedToId = 42
            };

            var (response, id) = await _repository.Create(task);

            Assert.Equal(Conflict, response);
            Assert.Equal(0, id);
        }

        [Fact]
        public async Task Create_creates_and_assigns_tasks()
        {
            var task = new TaskCreateDTO
            {
                Title = "title",
                Description = "description",
                Tags = new[] { "tag1", "tag4" }
            };

            var (_, id) = await _repository.Create(task);

            var created = _context.Tasks.Include(i => i.Tags).ThenInclude(i => i.Tag).FirstOrDefault(i => i.Id == id);

            Assert.Collection(created.Tags,
                i => Assert.Equal("tag1", i.Tag.Name),
                i => Assert.Equal("tag4", i.Tag.Name)
            );
        }

        [Fact]
        public async Task Read_given_id_returns_task_with_mapped_properties()
        {
            var task = await _repository.Read(2);

            Assert.Equal("title2", task.Title);
            Assert.Equal("description2", task.Description);
            Assert.Equal(1, task.AssignedToId);
            Assert.Equal(Active, task.State);
            Assert.Collection(task.Tags,
                t => { Assert.Equal(1, t.Key); Assert.Equal("tag1", t.Value); },
                t => { Assert.Equal(2, t.Key); Assert.Equal("tag2", t.Value); }
            );
        }

        [Fact]
        public void Read_given_returns_all_active_tasks_with_mapped_properties()
        {
            var tasks = _repository.Read();

            Assert.Equal(4, tasks.Count());
            Assert.All(tasks, t => Assert.NotEqual(Removed, t.State));

            var task = tasks.FirstOrDefault(t => t.Id == 2);

            Assert.Equal("title2", task.Title);
            Assert.Equal(1, task.AssignedToId);
            Assert.Equal(Active, task.State);
            Assert.Collection(task.Tags,
                t => { Assert.Equal(1, t.Key); Assert.Equal("tag1", t.Value); },
                t => { Assert.Equal(2, t.Key); Assert.Equal("tag2", t.Value); }
            );
        }

        [Fact]
        public void Read_given_true_returns_all_tasks_with_mapped_properties()
        {
            var tasks = _repository.Read(true);

            Assert.Equal(5, tasks.Count());

            var task = tasks.FirstOrDefault(t => t.Id == 5);

            Assert.Equal("title5", task.Title);
            Assert.Equal(2, task.AssignedToId);
            Assert.Equal(Removed, task.State);
            Assert.Empty(task.Tags);
        }

        [Fact]
        public async Task Update_given_non_existing_task_returns_NotFound()
        {
            var task = new TaskUpdateDTO
            {
                Id = 42,
                Title = "title",
                Description = "description"
            };

            var response = await _repository.Update(task);

            Assert.Equal(NotFound, response);
        }

        [Fact]
        public async Task Update_given_non_existing_user_returns_Conflict()
        {
            var task = new TaskUpdateDTO
            {
                Id = 1,
                Title = "title",
                Description = "description",
                AssignedToId = 42
            };

            var response = await _repository.Update(task);

            Assert.Equal(Conflict, response);
        }

        [Fact]
        public async Task Update_updates_all_properties_and_returns_updated()
        {
            var task = new TaskUpdateDTO
            {
                Id = 2,
                Title = "title",
                Description = "description",
                AssignedToId = 2,
                State = Resolved,
                Tags = new[] { "tag2", "tag4", "tag5" }
            };

            var response = await _repository.Update(task);

            var updated = await _context.Tasks.Include(i => i.Tags).ThenInclude(i => i.Tag).FirstOrDefaultAsync(i => i.Id == task.Id);

            Assert.Equal(Updated, response);
            Assert.Equal("title", updated.Title);
            Assert.Equal("description", updated.Description);
            Assert.Equal(2, updated.AssignedToId);
            Assert.Equal(Resolved, updated.State);
            Assert.Collection(updated.Tags,
                i => Assert.Equal("tag2", i.Tag.Name),
                i => Assert.Equal("tag4", i.Tag.Name),
                i => Assert.Equal("tag5", i.Tag.Name)
            );
        }

        [Fact]
        public async Task Delete_given_non_existing_task_returns_NotFound()
        {
            var response = await _repository.Delete(42);

            Assert.Equal(NotFound, response);
        }

        [Fact]
        public async Task Delete_given_existing_New_task_returns_Deleted()
        {
            var response = await _repository.Delete(1);

            Assert.Equal(Deleted, response);
        }

        [Fact]
        public async Task Delete_given_existing_New_task_deletes_task()
        {
            await _repository.Delete(1);

            var entity = await _context.Tasks.FindAsync(1);

            Assert.Null(entity);
        }

        [Fact]
        public async Task Delete_given_existing_Active_task_sets_state_to_Removed()
        {
            await _repository.Delete(2);

            var entity = await _context.Tasks.FindAsync(2);

            Assert.Equal(Removed, entity.State);
        }

        [Fact]
        public async Task Delete_given_existing_Resolved_task_returns_Conflict()
        {
            var response = await _repository.Delete(3);

            Assert.Equal(Conflict, response);
        }

        [Fact]
        public async Task Delete_given_existing_Closed_task_returns_Conflict()
        {
            var response = await _repository.Delete(4);

            Assert.Equal(Conflict, response);
        }

        [Fact]
        public async Task  Delete_given_existing_Removed_task_returns_Conflict()
        {
            var response = await _repository.Delete(5);

            Assert.Equal(Conflict, response);
        }

        public void Dispose()
        {
            _context.Dispose();
            _connection.Dispose();
        }
    }
}
