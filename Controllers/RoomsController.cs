using Microsoft.AspNetCore.Mvc;
using InteriorBackend.Models;
using Microsoft.EntityFrameworkCore;
using InteriorBackend.Data;
using System.Threading.Tasks;
using System.Linq;

namespace InteriorBackend.Controllers
{
    [ApiController]
    [Route("api/rooms")]
    public class RoomsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public RoomsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> CreateRoom([FromBody] Room room)
        {
            if (room == null || string.IsNullOrEmpty(room.UserId))
            {
                return BadRequest(new { message = "Invalid room data or missing UserId." });
            }

            _context.Rooms.Add(room);
            await _context.SaveChangesAsync();
            return Ok(room);
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetRooms(string userId) 
        {
            var rooms = await _context.Rooms.Where(r => r.UserId == userId).ToListAsync();
            if (rooms == null || rooms.Count == 0)
            {
                return NotFound(new { message = "No rooms found for this user." });
            }

            return Ok(rooms);
        }
    }
}
