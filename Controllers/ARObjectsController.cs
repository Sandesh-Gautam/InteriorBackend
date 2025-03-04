using InteriorBackend.Data;
using InteriorBackend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;

namespace InteriorBackend.Controllers
{
    [ApiController]
    [Route("api/arobjects")]
    public class ARObjectsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ARObjectsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> AddARObject([FromBody] ARObject arObject)
        {
            _context.ARObjects.Add(arObject);
            await _context.SaveChangesAsync();
            return Ok(arObject);
        }

        [HttpGet("{roomId}")]
        public async Task<IActionResult> GetARObjects(int roomId)
        {
            var arObjects = await _context.ARObjects.Where(o => o.RoomId == roomId).ToListAsync();
            return Ok(arObjects);
        }
    }
}
