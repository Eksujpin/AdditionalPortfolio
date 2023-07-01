using System;
using System.Linq;
using Assignment05.Entities;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;
using static Assignment05.Entities.State;
using static Assignment05.Models.Response;
using Assignment05.Models.Users;
using TaskTR = System.Threading.Tasks.Task;
using System.Collections.Generic;

namespace Assignment05.Models.Tests
{
    public class UserRepositoryTests : IDisposable
    {
        private readonly SqliteConnection _connection;
        private readonly KanbanContext _context;
        private readonly UserRepository _repository;
        public UserRepositoryTests()
        {
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();
            var builder = new DbContextOptionsBuilder<KanbanContext>().UseSqlite(_connection);
            _context = new KanbanTestContext(builder.Options);
            _context.Database.EnsureCreated();
            _repository = new UserRepository(_context);
        }

        [Fact]
        public async TaskTR Create_Given_non_Existent_user()
        {
            var user = new UserCreateDTO
            {
                Name = "Luke Peterson",
                EmailAddress = "LUPE@Gmail.com"
            };
            var res = await _repository.CreateAsync(user);

            var created = _context.Users.Find(3);

            Assert.Equal(Created, res.response);
            Assert.Equal("Luke Peterson", created.Name);
        }

        [Fact]
        public async TaskTR Create_Given_Existent_user()
        {
            var user = new UserCreateDTO
            {
                Name = "user2",
            };
            var res = await _repository.CreateAsync(user);

            Assert.Equal(Conflict, res.response);
            Assert.Equal(2, res.userId);
        }

        [Fact]
        public async TaskTR Read_All_()
        {
            var res = await _repository.ReadAsync();

            Assert.Equal(2, res.Count);
        }

        [Fact]
        public async TaskTR Read_All_Existing_User()
        {
            var res = await _repository.ReadAsync(2);

            Assert.Equal("user2", res.Name);
        }
        [Fact]
        public async TaskTR Read_All_non_Existing_User()
        {
            var res = await _repository.ReadAsync(2231);

            Assert.Null(res);
        }

        [Fact]
        public async TaskTR Update_existing_user()
        {
            var input = new UserUpdateDTO
            {
                EmailAddress = "bob@Builder.css",
                Name = "Bob The Builder",
                Id = 1,
            };
            var res = await _repository.UpdateAsync(input);
            var user = _context.Users.Find(1);
            Assert.Equal(Updated, res);
            Assert.Equal("Bob The Builder", user.Name);
        }

        [Fact]
        public async TaskTR Update_non_existing_user()
        {
            var input = new UserUpdateDTO
            {
                EmailAddress = "bob@Builder.css",
                Name = "Bob The Builder",
                Id = 112351,
            };
            var res = await _repository.UpdateAsync(input);

            Assert.Equal(NotFound, res);
        }

        [Fact]
        public async TaskTR Delete_non_Existing_User()
        {
            var res = await _repository.DeleteAsync(2231);

            Assert.Equal(NotFound, res);
        }

        [Fact]
        public async TaskTR Delete_Existing_User()
        {
            var res = await _repository.DeleteAsync(1);
            Assert.Equal(Deleted, res);
            Assert.Null(_context.Users.Find(1));
        }

        [Fact]
        public async TaskTR Delete_Existing_User_with_task_no_force()
        {
            List<Task> tasks = new List<Task>{new Task{Id = 6, Title = "linked1", Description = "first linked task", AssignedToId = 1, State = Active }};
            var target = _context.Users.Find(1);
            target.Tasks = tasks;
            var res = await _repository.DeleteAsync(1);
            Assert.Equal(Conflict, res);
        }

        [Fact]
        public async TaskTR Delete_Existing_User_with_task_force()
        {
            List<Task> tasks = new List<Task>{new Task{Id = 6, Title = "linked1", Description = "first linked task", AssignedToId = 1, State = Active }};
            var target = _context.Users.Find(1);
            target.Tasks = tasks;
            var res = await _repository.DeleteAsync(1, true);
            Assert.Equal(Deleted, res);
            Assert.Null(_context.Users.Find(1));
        }


        public void Dispose()
        {
            _connection.Dispose();
            _context.Dispose();
        }
    }
}
