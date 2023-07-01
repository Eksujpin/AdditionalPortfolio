using System;
using Xunit;
using Moq;
using System.Linq;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore; //dotnet add package Microsoft.EntityFrameworkCore.Design
using System.Collections.Generic;
using BDSA2020.Assignment04.Entities;
using static BDSA2020.Assignment04.Models.Response;

namespace BDSA2020.Assignment04.Models.Tests
{
    public class TagRepositoryTests : IDisposable
    {
        private readonly SqliteConnection _connection;
        private readonly KanbanContext _context;
        private readonly TagRepository _repository;
        public TagRepositoryTests()
        {
            //Arrange
            _connection = new SqliteConnection("Filename=:memory:");
            _connection.Open();
            var builder = new DbContextOptionsBuilder<KanbanContext>().UseSqlite(_connection);
            _context = new KanbanContext(builder.Options);
            _context.Database.EnsureCreated();
            _context.GenerateTestData();

            _repository = new TagRepository(_context);
        }

        //_________________________________CREATE-TESTS_________________________________
        [Fact]
        public void Tag_Create_valid_id()
        {
            //Act
            var res = _repository.Create(new TagCreateDTO { Name = "NewTag" });
            _context.SaveChanges();
            var amount = _context.Tags.ToList().Count();
            //Assert
            Assert.Equal(Created, res.response);
            Assert.Equal(4, res.tagId);
            Assert.Equal(4, amount);
        }

        [Fact]
        public void Tag_Create_Allready_Exists_id()
        {
            //Act
            var res = _repository.Create(new TagCreateDTO { Name = "medium difficulty" });
            //Assert
            Assert.Equal(Conflict, res.response);
            Assert.Equal(1, res.tagId);

        }

        //_________________________________READ-TESTS_________________________________
        [Fact]
        public void Tag_Read_Valid_Id()
        {
            //Act
            var res = _repository.Read(2);
            //Assert
            Assert.Equal("ROUGH LIFE difficulty", res.Name);
        }

        [Fact]
        public void Tag_Read_Invalid_Id()
        {
            //Act
            var res = _repository.Read(177);
            //Assert
            Assert.Null(res);
        }

        [Fact]
        public void Tag_Read_All()
        {
            //Act
            var res = _repository.Read();
            //Assert
            Assert.Equal(3, res.Count());
        }

        //_________________________________DELETE-TESTS_________________________________
        [Fact]
        public void Tag_Delete_Invalid_id()
        {
            //Act
            var res = _repository.Delete(42);

            //Assert
            Assert.Equal(NotFound, res);
        }

        [Fact]
        public void Tag_Delete_valid_id_Not_forced_Linked_Tasks()
        {
            //Act
            var res = _repository.Delete(2);

            //Assert
            Assert.Equal(Conflict, res);
        }

        [Fact]
        public void Tag_Delete_valid_id_forced_Linked_Tasks()
        {
            //Act
            var res = _repository.Delete(2, true);
            _context.SaveChanges();
            var tagList = _context.Tags.Select(t => t.Id).ToList();
            //Assert
            Assert.Equal(Deleted, res);
            Assert.False(tagList.Contains(2));
        }

        [Fact]
        public void Tag_Delete_valid_id_No_Linked_Tasks()
        {
            //Act
            var res = _repository.Delete(1);
            _context.SaveChanges();
            var tagList = _context.Tags.Select(t => t.Id).ToList();
            //Assert
            Assert.Equal(Deleted, res);
            Assert.False(tagList.Contains(1));
        }
        
        [Fact]
        public void Tag_Delete_invalid_id()
        {
            //Act
            var res = _repository.Delete(7);
            //Assert
            Assert.Equal(NotFound, res);
        }

        //_________________________________UPDATE-TESTS_________________________________
        [Fact]
        public void Tag_Update_Invalid_id()
        {
            //Act
            var input = new TagUpdateDTO { Id = 42 };
            var res = _repository.Update(input);
            //Assert
            Assert.Equal(NotFound, res);
        }

        [Fact]
        public void Tag_Update_valid_id()
        {
            //Act
            var input = new TagUpdateDTO { Id = 1, Name = "ChangedName" };
            var res = _repository.Update(input);
            var tag = _context.Tags.Where(t => t.Id == 1).First();
            //Assert
            Assert.Equal(Updated, res);
            Assert.Equal("ChangedName", tag.Name);
        }


        //_________________________________DISPOSE_________________________________
        public void Dispose()
        {
            _connection.Dispose();
            _context.Dispose();
        }
    }
}
