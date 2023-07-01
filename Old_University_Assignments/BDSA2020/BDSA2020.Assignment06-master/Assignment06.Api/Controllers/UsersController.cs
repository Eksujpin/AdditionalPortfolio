using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Assignment06.Models;
using Microsoft.AspNetCore.Mvc;

namespace Assignment06.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        // GET: users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserListDTO>>> Get()
        {
            throw new NotImplementedException();
        }

        // GET: users/5
        [HttpGet("{id}", Name = "Get")]
        public async Task<ActionResult<UserDetailsDTO>> Get(int id)
        {
            throw new NotImplementedException();
        }

        // POST: users
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] UserCreateDTO user)
        {
            throw new NotImplementedException();
        }

        // PUT: users/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] UserUpdateDTO user)
        {
            throw new NotImplementedException();
        }

        // DELETE: users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            throw new NotImplementedException();
        }
    }
}
