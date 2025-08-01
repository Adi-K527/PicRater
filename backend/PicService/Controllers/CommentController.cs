using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using PicService.Models;

namespace PicService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly CommentContext _context;
        private readonly ILogger<CommentController> _logger;

        public CommentController(CommentContext context, ILogger<CommentController> logger)
        {
            _context = context;
            _logger = logger;
            _logger.LogInformation("CommentController initialized.");
        }

        // GET: api/Comment/5
        [Authorize]
        [HttpGet("{picid}")]
        public async Task<ActionResult<IEnumerable<CommentModel>>> GetCommentModel(int picid)
        {
            var commentModel = await _context.CommentModel.Where(c => c.PicId == picid).ToListAsync();

            if (commentModel == null || !commentModel.Any())
            {
                _logger.LogWarning("No comments found for picture ID {Id}.", picid);
                return NotFound();
            }

            _logger.LogInformation("Request {Method} {Path}: Retrieved {Count} comments for picture ID {Id}", HttpContext.Request.Method, HttpContext.Request.Path, commentModel.Count, picid);
            return StatusCode(200, new { data = commentModel });
        }

        // POST: api/Comment
        [Authorize]
        [HttpPost("{picid}")]
        public async Task<ActionResult<CommentModel>> PostCommentModel(int picid, CommentModel commentModel)
        {

            var newCommentModel = new CommentModel
            {
                Content = commentModel.Content,
                PicId = picid,
                UserId = commentModel.UserId,
                CommentDate = DateTime.UtcNow
            };

            _context.CommentModel.Add(newCommentModel);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Request {Method} {Path}: New comment added for picture ID {Id} with Comment ID {CommentId}", HttpContext.Request.Method, HttpContext.Request.Path, picid, newCommentModel.CommentId);
            return StatusCode(201, new { id = newCommentModel.CommentId });
        }

        // DELETE: api/Comment/5
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCommentModel(int id)
        {
            var commentModel = await _context.CommentModel.FindAsync(id);
            if (commentModel == null)
            {
                return NotFound();
            }

            _context.CommentModel.Remove(commentModel);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Request {Method} {Path}: Comment with ID {Id} deleted", HttpContext.Request.Method, HttpContext.Request.Path, id);
            return StatusCode(200, new { message = "Comment deleted successfully" });
        }
    }
}
