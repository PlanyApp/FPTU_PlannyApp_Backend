using Microsoft.AspNetCore.Mvc;
using PlanyApp.Repository.Models;
using PlanyApp.Repository.UnitOfWork;
using System.Threading.Tasks;

namespace PlanyApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUnitOfWork _uow;

        public UsersController( IUnitOfWork uow)
        {
            _uow = uow;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var users = await _uow.UserRepository.GetAllAsync();
            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            var user = await _uow.UserRepository.GetByIdAsync(id);
            if (user == null)
                return NotFound();
            return Ok(user);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] User user)
        {
            await _uow.UserRepository.AddAsync(user);
            return CreatedAtAction(nameof(Get), new { id = user.UserId }, user);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var user = await _uow.UserRepository.GetByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            await _uow.UserRepository.DeleteAsync(id);
            return NoContent();
        }
    }
}
