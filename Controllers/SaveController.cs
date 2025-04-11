using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using InteriorBackend.Data; // Add this to access ApplicationDbContext
using InteriorBackend.Models;
using System.Linq;

namespace InteriorBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SaveController : ControllerBase
    {
        private readonly string _uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");

        private readonly ApplicationDbContext _context;

        public SaveController(ApplicationDbContext context)
        {
            _context = context;

            if (!Directory.Exists(_uploadsFolder))
            {
                Directory.CreateDirectory(_uploadsFolder);
            }
        }

        // POST: api/Save/upload
        [HttpPost("upload")]
        [Authorize]
        public async Task<IActionResult> UploadFiles([FromForm] string userId, [FromForm] IFormFileCollection files)
        {
            if (files == null || files.Count == 0)
                return BadRequest("No files provided.");

            var savedFiles = new List<string>();

            foreach (var file in files)
            {
                if (file.Length > 0)
                {
                    var originalName = Path.GetFileName(file.FileName);
                    var uniqueName = $"{Guid.NewGuid()}_{originalName}";
                    var filePath = Path.Combine(_uploadsFolder, uniqueName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    savedFiles.Add(uniqueName);
                }
            }

            var save = new UserSave
            {
                UserId = userId,
                ImageUrl = savedFiles
            };

            // Persist to database
            _context.UserSaves.Add(save);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Upload successful", data = save });
        }

        // GET: api/Save/files/{userId}
        [HttpGet("files/{userId}")]
        //[Authorize]
        public IActionResult GetUserFiles(string userId)
        {
            var saves = _context.UserSaves
                .Where(u => u.UserId == userId)
                .Select(s => new
                {
                    s.Id,
                    s.UserId,
                    s.ImageUrl
                }).ToList();

            if (!saves.Any())
                return NotFound(new { message = "No saved files found for this user." });

            return Ok(new { message = "Files retrieved", data = saves });
        }
    }
}
