using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Assignment06.Models;
using Assignment06.Api.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Assignment06.Api.Tests
{
    public class TasksControllerTests
    {

        [Fact]
        public async Task Get_given_existing_returns_task()
        {
            var task = new TaskDetailsDTO();

            var repository = new Mock<ITaskRepository>();
            repository.Setup(t => t.Read(4)).ReturnsAsync(task);

            var controller = new TasksController(repository.Object);

            var actual = await controller.Get(4);

            Assert.Equal(task, actual.Value);
        }

        [Fact]
        public async Task Get_given_non_existing_returns_404()
        {
            var repository = new Mock<ITaskRepository>();

            var controller = new TasksController(repository.Object);

            var actual = await controller.Get(4);

            Assert.IsType<NotFoundResult>(actual.Result);
        }

        [Fact]
        public async Task Get_All_return()
        {
            var task = new TaskListDTO();
            var asList = new List<TaskListDTO>{task};
            IQueryable<TaskListDTO> asQueryable = asList.AsQueryable();
            var repository = new Mock<ITaskRepository>();
            repository.Setup(t => t.Read(true)).Returns(asQueryable);

            var controller = new TasksController(repository.Object);

            var actual = await controller.Get();

            Assert.Equal(asList, actual.Value);
        }



        [Fact]
        public async Task Post_given_id()
        {
            var task = new TaskCreateDTO { Title = "dis is task" };
            var repository = new Mock<ITaskRepository>();

            var controller = new TasksController(repository.Object);

            var result = await controller.Post(task);

            repository.Verify(mock => mock.Create(task));
        }




    }
}
