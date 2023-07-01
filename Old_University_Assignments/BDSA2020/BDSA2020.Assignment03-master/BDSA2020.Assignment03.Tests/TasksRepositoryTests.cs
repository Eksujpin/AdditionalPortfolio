using Xunit;
using System.Collections.Generic;

namespace BDSA2020.Assignment03.Tests
{
    public class TasksRepositoryTests
    {
        [Fact]
        public void Create_parse_DTO_and_get_return_entity_id()
        {
            TaskDTO dto = new TaskDTO();
            dto.Id = 1;
            dto.Title = "MyTask";
            dto.Description = "My first task";
            dto.Tags = new List<string>{"cool"};
            dto.State = State.Active;

            using var context = new Entities.kanbanContext();
            TasksRepository tr = new TasksRepository(context);

            var result = tr.Create(dto);

            Assert.Equal(1,result);

        }

        [Fact]
        public void Test_GetAllTasks()
        {
            

            using var context = new Entities.kanbanContext();
            TasksRepository tr = new TasksRepository(context);

            var result = tr.All();
            
            
            Assert.Equal(10,result.Count);
            

        }

        [Fact]
        public void Test_FindById()
        {
            using var context = new Entities.kanbanContext();
            TasksRepository tr = new TasksRepository(context);
            var result = tr.FindById(1);
            Assert.Equal(1, result.Id);
            
        }

        
        

    }
}
