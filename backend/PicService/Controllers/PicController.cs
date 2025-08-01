using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PicService.Models;

namespace PicService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PicController : ControllerBase
    {
        private readonly PicContext _context;

        public PicController(PicContext context)
        {
            _context = context;
        }

        // GET: api/Pic
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PicModel>>> GetPicModel()
        {
            try
            {
                var data = await _context.PicModel.ToListAsync();
                return StatusCode(StatusCodes.Status200OK, new { data = data });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error retrieving data: {ex.Message}");
            }
        }

        // GET: api/Pic/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PicModel>> GetPicModel(int id)
        {
            try
            {
                var picModel = await _context.PicModel.FindAsync(id);
                if (picModel == null)
                {
                    return NotFound();
                }
                return StatusCode(200, new { data = picModel });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error retrieving data: {ex.Message}");
            }
        }

        // POST: api/Pic
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<PicModel>> PostPicModel(IFormFile file, PicDto picModel)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("File is required.");
            }

            // Assuming you have a method to handle file upload and set the PicUrl
            picModel.PicUrl = await UploadFileAsync(file);

            try
            {
                _context.PicModel.Add(picModel);
                await _context.SaveChangesAsync();
                return CreatedAtAction("GetPicModel", new { id = picModel.PicId }, picModel);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error saving data: {ex.Message}");
            }
        }


        // DELETE: api/Pic/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePicModel(int id)
        {
            var picModel = await _context.PicModel.FindAsync(id);
            if (picModel == null)
            {
                return NotFound();
            }

            _context.PicModel.Remove(picModel);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PicModelExists(int id)
        {
            return _context.PicModel.Any(e => e.PicId == id);
        }
    }
}