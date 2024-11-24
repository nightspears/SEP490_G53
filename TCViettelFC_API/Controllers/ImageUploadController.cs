using Microsoft.AspNetCore.Mvc;
using TCViettelFC_API.Repositories.Interfaces;

namespace TCViettelFC_API.Controllers
{
    public class ImageUploadController : ControllerBase
    {
        private readonly ICloudinarySetting _cloudinarySetting;

        public ImageUploadController(ICloudinarySetting cloudinarySetting)
        {
            _cloudinarySetting = cloudinarySetting;
        }

        [HttpPost("upload-multiple")]
        public async Task<IActionResult> UploadImages(List<IFormFile> files)
        {
            if (files == null || files.Count == 0)
                return BadRequest("No files were provided.");

            var uploadResults = new List<string>();

            foreach (var file in files)
            {
                var uploadResult = _cloudinarySetting.CloudinaryUpload(file);
                if (uploadResult.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    uploadResults.Add(uploadResult.SecureUrl.ToString());
                }
                else
                {
                    return StatusCode((int)uploadResult.StatusCode, $"Failed to upload image: {file.FileName}");
                }
            }

            return Ok(new
            {
                urls = uploadResults
            });
        }
    }
}
