using BlogWebApi.Dtos;
using BlogWebApi.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BlogWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        private readonly IImageService _imageService;
        public ImageController(IImageService imageService)
        {
            _imageService = imageService;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> Upload([FromForm] IFormFile file)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _imageService.UploadImageAsync(file);
            if (result == null)
                return BadRequest();
            return Ok(result.Url.ToString());
        }

        [HttpPost("delete")]
        public IActionResult Delete([FromBody] DeleteImageDto image)
        {
            if (!string.IsNullOrEmpty(image.Url))
            {
                _ = _imageService.DeleteImageAsync(image.Url);
            }

            return Ok();
        }
    }
}
