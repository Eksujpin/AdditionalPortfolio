using System;
using Xunit;
using Moq;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore; //dotnet add package Microsoft.EntityFrameworkCore.Design
using System.Collections.Generic;
using BDSA2020.Assignment04.Entities;
using static BDSA2020.Assignment04.Models.Response;
using System.Linq;

namespace BDSA2020.Assignment04.Models.Tests
{
    public class TaskRepositoryTests : IDisposable
    {
        private readonly SqliteConnection _connection;
        private readonly KanbanContext _context;
        private readonly TaskRepository _repository;
        public TaskRepositoryTests()
        {
            //Arrange
            _connection = new SqliteConnection("Filename=:memory:");
            _connection.Open();
            var builder = new DbContextOptionsBuilder<KanbanContext>().UseSqlite(_connection);
            _context = new KanbanContext(builder.Options);
            _context.Database.EnsureCreated();
            _context.GenerateTestData();

            _repository = new TaskRepository(_context);
        }

        //_________________________________CREATE-TESTS_________________________________
        [Fact]
        public void Task_Create_valid_Name()
        {
            //Act
            var res = _repository.Create(new TaskCreateDTO { Title = "NewTask" });
            _context.SaveChanges();
            var amount = _context.Tasks.ToList().Count();
            //Assert
            Assert.Equal(Created, res.response);
            Assert.Equal(6, res.taskId);
            Assert.Equal(6, amount);
        }

        [Fact]
        public void Task_Create_Allready_Exists_Name()
        {
            //Act
            var res = _repository.Create(new TaskCreateDTO { Title = "Schedule party" });
            //Assert
            Assert.Equal(Conflict, res.response);
            Assert.Equal(2, res.taskId);

        }

        [Fact]
        public void Task_Create_User_Not_Exist()
        {
            //Act
            var res = _repository.Create(new TaskCreateDTO { Title = "NewTask", AssignedToId = 8923 });
            //Assert
            Assert.Equal(BadRequest, res.response);
            Assert.Equal(8923, res.taskId);

        }

        [Fact]
        public void Task_Create_User_Exist()
        {
            //Act
            var res = _repository.Create(new TaskCreateDTO { Title = "NewTask", AssignedToId = 2, });
            _context.SaveChanges();
            var amount = _context.Tasks.ToList().Count();
            var user = _context.Tasks.Where(t => t.Id == amount).Select(t => t.AssignedTo).Single();
            //Assert
            Assert.Equal(Created, res.response);
            Assert.Equal(6, res.taskId);
            Assert.Equal(6, amount);
            Assert.Equal(2, user.Id);

        }
        //_________________________________READ-TESTS_________________________________
        [Fact]
        public void Task_Read_all_NOT_State_Removed()
        {
            //Act
            var res = _repository.Read();
            //Assert
            Assert.Equal(4, res.Count());
        }

        [Fact]
        public void Task_Read_all_Forced()
        {
            //Act
            var res = _repository.Read(true);
            //Assert
            Assert.Equal(5, res.Count());
        }

        [Fact]
        public void Task_Read_Id_Invalid()
        {
            //Act
            var res = _repository.Read(123);
            //Assert
            Assert.Null(res);
        }

        [Fact]
        public void Task_Read_Id_Valid()
        {
            //Act
            var res = _repository.Read(2);
            //Assert
            Assert.Equal("Schedule party", res.Title);
        }

        //_________________________________DELETE-TESTS_________________________________
        [Fact]
        public void Task_Delete_Invalid_id()
        {
            //Act
            var res = _repository.Delete(42);
            //Assert
            Assert.Equal(NotFound, res);
        }

        [Fact]
        public void Task_Delete_valid_id_State_NEW()
        {
            //Act
            var res = _repository.Delete(1);
            _context.SaveChanges();
            var taskList = _context.Tasks.Select(t => t.Id).ToList();
            //Assert
            Assert.Equal(Deleted, res);
            Assert.Equal(4, taskList.Count());
        }

        [Fact]
        public void Task_Delete_valid_id_State_Active()
        {
            //Act
            var res = _repository.Delete(2);
            var task = _context.Tasks.Where(t => t.Id == 2).First();
            //Assert
            Assert.Equal(Updated, res);
            Assert.Equal(State.Removed, task.State);
        }

        [Fact]
        public void Task_Delete_valid_id_State_Resolved()
        {
            //Act
            var res = _repository.Delete(3);
            //Assert
            Assert.Equal(Conflict, res);
        }

        [Fact]
        public void Task_Delete_valid_id_State_Closed()
        {
            //Act
            var res = _repository.Delete(4);
            //Assert
            Assert.Equal(Conflict, res);
        }

        [Fact]
        public void Task_Delete_valid_id_State_Removed()
        {
            //Act
            var res = _repository.Delete(5);
            //Assert
            Assert.Equal(Conflict, res);
        }


        //_________________________________UPDATE-TESTS_________________________________
        [Fact]
        public void Task_Update_Invalid_id()
        {
            //Act
            TaskUpdateDTO input = new TaskUpdateDTO { Id = 42 };
            var res = _repository.Update(input);
            //Assert
            Assert.Equal(NotFound, res);
        }

        [Fact]
        public void Task_Update_Empty()
        {
            //Act
            TaskUpdateDTO input = new TaskUpdateDTO { };
            var res = _repository.Update(input);
            //Assert
            Assert.Equal(NotFound, res);
        }

        [Fact]
        public void Task_Update_valid_id_No_User_change()
        //*note during update to keep the same user that Id has to be specified when updating, otherwise the user will be removed
        {
            //Act
            TaskUpdateDTO input = new TaskUpdateDTO { Id = 2, Title = "SURPRISE F****R", State = State.Resolved, Description = "mistaken movie quote", AssignedToId = 2 };
            var res = _repository.Update(input);
            _context.SaveChanges();
            var task = _context.Tasks.Where(t => t.Id == 2).First();
            //Assert
            Assert.Equal(Updated, res);
            Assert.Equal("SURPRISE F****R", task.Title);
            Assert.Equal(State.Resolved, task.State);
            Assert.Equal("mistaken movie quote", task.Description);
            Assert.Equal(2, task.AssignedToId);
        }

        [Fact]
        public void Task_Update_valid_id_ADD_User()
        {
            //Act
            TaskUpdateDTO input = new TaskUpdateDTO { Id = 1, AssignedToId = 1 };
            var res = _repository.Update(input);
            _context.SaveChanges();
            var user = _context.Tasks.Where(t => t.Id == 1).Select(t => t.AssignedTo).Single();
            //Assert
            Assert.Equal(Updated, res);
            Assert.Equal("Pascal Furlong", user.Name);
        }

        [Fact]
        public void Task_Update_valid_id_Replace_User()
        {
            //Act
            TaskUpdateDTO input = new TaskUpdateDTO { Id = 2, AssignedToId = 1 };
            var res = _repository.Update(input);
            _context.SaveChanges();
            var user = _context.Tasks.Where(t => t.Id == 2).Select(t => t.AssignedTo).Single();
            //Assert
            Assert.Equal(Updated, res);
            Assert.Equal("Pascal Furlong", user.Name);
        }

        [Fact]
        public void Task_Update_valid_id_Remove_User()
        {
            //Act
            TaskUpdateDTO input = new TaskUpdateDTO { Id = 2, AssignedToId = null };
            var res = _repository.Update(input);
            _context.SaveChanges();
            var user = _context.Tasks.Where(t => t.Id == 2).Select(t => t.AssignedTo).Single();
            //Assert
            Assert.Equal(Updated, res);
            Assert.Null(user);
        }

        [Fact]
        public void Task_Update_valid_id_Invalid_User()
        {
            //Act
            TaskUpdateDTO input = new TaskUpdateDTO { Id = 1, AssignedToId = 112, };
            var res = _repository.Update(input);
            _context.SaveChanges();
            //Assert
            Assert.Equal(BadRequest, res);
        }

        //_________________________________DISPOSE_________________________________
        public void Dispose()
        {
            _connection.Dispose();
            _context.Dispose();
        }
    }
}
