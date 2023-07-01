using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Assignment06.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Assignment06.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class TasksController : ControllerBase
    {

        private readonly ITaskRepository _repository;

        public TasksController(ITaskRepository repository)
        {
            _repository = repository;
        }

        // GET: tasks
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TaskListDTO>>> Get()
        {
            return await _repository.Read(true).ToListAsync();
        }

        // GET: tasks/5
        [HttpGet("{id}", Name = "Get")]
        public async Task<ActionResult<TaskDetailsDTO>> Get(int id)
        {
            var task = await _repository.Read(id);

            if (task == null)
            {
                return NotFound();
            }

            return task;
        }

        // POST: tasks
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] TaskCreateDTO task)
        {
            var id = await _repository.Create(task);

            return CreatedAtAction(nameof(Get), new { id }, default);
        }

        // PUT: tasks/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] TaskUpdateDTO task)
        {
            throw new NotImplementedException();
        }

        // DELETE: tasks/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            throw new NotImplementedException();
        }
    }
}
