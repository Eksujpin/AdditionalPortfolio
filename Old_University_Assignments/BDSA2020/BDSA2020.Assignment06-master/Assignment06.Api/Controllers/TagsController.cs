using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Assignment06.Models;
using Microsoft.AspNetCore.Mvc;

namespace Assignment06.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class TagsController : ControllerBase
    {
        // GET: tags
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TagDTO>>> Get()
        {
            throw new NotImplementedException();
        }

        // GET: tags/5
        [HttpGet("{id}", Name = "Get")]
        public async Task<ActionResult<TagDTO>> Get(int id)
        {
            throw new NotImplementedException();
        }

        // POST: tags
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] TagCreateDTO tag)
        {
            throw new NotImplementedException();
        }

        // PUT: tags/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] TagUpdateDTO tag)
        {
            throw new NotImplementedException();
        }

        // DELETE: tags/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            throw new NotImplementedException();
        }
    }
}
