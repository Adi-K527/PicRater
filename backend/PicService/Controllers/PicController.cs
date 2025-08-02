using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using PicService.Models;
using Amazon.S3;
using Amazon.S3.Transfer;
using Amazon.S3.Model;

namespace PicService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PicController : ControllerBase
    {
        private readonly PicContext _context;
        private readonly ILogger<PicController> _logger;
        private readonly string _bucketName = "picrater-pics-8164";
        private readonly string? _accessKey = Environment.GetEnvironmentVariable("Access__Key");
        private readonly string? _secretKey = Environment.GetEnvironmentVariable("Secret__Key");

        public PicController(PicContext context, ILogger<PicController> logger)
        {
            _context = context;
            _logger = logger;
            _logger.LogInformation("PicController initialized.");
        }

        // GET: api/Pic/health
        [HttpGet("health")]
        public IActionResult HealthCheck()
        {
            return Ok(new { status = "Healthy" });
        }

        // GET: api/Pic
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PicModel>>> GetPicModel()
        {
            try
            {
                var data = await _context.PicModel.ToListAsync();
                _logger.LogInformation("Request {Method} {Path}: Retrieved {Count} pictures", HttpContext.Request.Method, HttpContext.Request.Path, data.Count);
                return Ok(new { data });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving pictures.");
                return StatusCode(500, $"Error retrieving data: {ex.Message}");
            }
        }

        // GET: api/Pic/5
        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<PicModel>> GetPicModel(int id)
        {
            try
            {
                var picModel = await _context.PicModel.FindAsync(id);
                if (picModel == null)
                {
                    _logger.LogWarning("Picture with ID {Id} not found.", id);
                    return NotFound();
                }

                _logger.LogInformation("Request {Method} {Path}: Picture with ID {Id} retrieved", HttpContext.Request.Method, HttpContext.Request.Path, id);
                return Ok(new { data = picModel });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving picture with ID {Id}.", id);
                return StatusCode(500, $"Error retrieving data: {ex.Message}");
            }
        }

        // POST: api/Pic
        [Authorize]
        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult> PostPicModel([FromForm] IFormFile file, [FromForm] string title, [FromForm] string description, [FromForm] string userId)
        {
            if (file == null || file.Length == 0)
            {
                _logger.LogWarning("Image upload failed: file is null or empty.");
                return BadRequest("File is required.");
            }

            if (string.IsNullOrEmpty(title))
            {
                return BadRequest("Title is required.");
            }

            try
            {
                var fileUrl = await UploadFileAsync(file);
                _logger.LogInformation("File uploaded to S3: {Url}", fileUrl);

                var newPicModel = new PicModel
                {
                    Title = title,
                    Description = description ?? "",
                    ImageUrl = fileUrl,
                    UserId = userId ?? "",
                    UploadDate = DateTime.UtcNow
                };

                _context.PicModel.Add(newPicModel);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Request {Method} {Path}: File uploaded and new picture saved with ID {Id}", HttpContext.Request.Method, HttpContext.Request.Path, newPicModel.PicId);
                return StatusCode(201, new { id = newPicModel.PicId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading image.");
                return StatusCode(500, $"Error saving data: {ex.Message}");
            }
        }
        

        // DELETE: api/Pic/5
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePicModel(int id)
        {
            var picModel = await _context.PicModel.FindAsync(id);
            if (picModel == null)
            {
                _logger.LogWarning("Delete failed: Picture with ID {Id} not found.", id);
                return NotFound();
            }

            try
            {
                await DeleteFileAsync(picModel.ImageUrl);
                _context.PicModel.Remove(picModel);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Request {Method} {Path}: Picture with ID {Id} deleted", HttpContext.Request.Method, HttpContext.Request.Path, id);
                return Ok(new { message = "Picture deleted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting picture with ID {Id}.", id);
                return StatusCode(500, $"Error deleting image: {ex.Message}");
            }
        }

        private bool PicModelExists(int id)
        {
            return _context.PicModel.Any(e => e.PicId == id);
        }

        private async Task<string> UploadFileAsync(IFormFile file)
        {
            if (_accessKey == null || _secretKey == null)
            {
                throw new InvalidOperationException("AWS credentials are not configured");
            }

            var keyName = "pics/" +Guid.NewGuid() + "_" + file.FileName;

            using var client = new AmazonS3Client(_accessKey, _secretKey, Amazon.RegionEndpoint.USEast1);
            using var stream = file.OpenReadStream();

            var uploadRequest = new TransferUtilityUploadRequest
            {
                InputStream = stream,
                Key = keyName,
                BucketName = _bucketName,
                ContentType = file.ContentType
            };

            var transferUtility = new TransferUtility(client);
            await transferUtility.UploadAsync(uploadRequest);

            return $"https://{_bucketName}.s3.amazonaws.com/{keyName}";
        }

        private async Task DeleteFileAsync(string imageUrl)
        {
            if (string.IsNullOrWhiteSpace(imageUrl))
            {
                _logger.LogWarning("DeleteFileAsync called with null or empty URL.");
                return;
            }

            if (_accessKey == null || _secretKey == null)
            {
                _logger.LogWarning("AWS credentials are not configured for file deletion.");
                return;
            }

            var keyName = imageUrl.Substring(imageUrl.LastIndexOf('/') + 1);

            using var client = new AmazonS3Client(_accessKey, _secretKey, Amazon.RegionEndpoint.USEast1);
            var deleteRequest = new DeleteObjectRequest
            {
                BucketName = _bucketName,
                Key = keyName
            };

            await client.DeleteObjectAsync(deleteRequest);
            _logger.LogInformation("Deleted image from S3: {Key}", keyName);
        }
    }
}