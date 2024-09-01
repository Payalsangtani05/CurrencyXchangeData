using Microsoft.AspNetCore.Mvc;

namespace CurrencyXchange.Controllers
{

	[ApiController]
	[Route("api/[controller]")]
	public class ProfilePhotoController : ControllerBase
	{
		private readonly string _storagePath;

		public ProfilePhotoController(IConfiguration configuration)
		{
			_storagePath = configuration["ProfilePhotoSettings:StoragePath"];
		}

		[HttpPost("{userId}")]
		public async Task<IActionResult> UploadProfilePhoto(string userId, IFormFile file)
		{
			if (file == null || file.Length == 0)
			{
				return BadRequest("No file uploaded.");
			}

			// Ensure the storage path exists
			var userDirectory = Path.Combine(_storagePath, userId);
			if (!Directory.Exists(userDirectory))
			{
				Directory.CreateDirectory(userDirectory);
			}

			// Create a unique filename
			var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
			var filePath = Path.Combine(userDirectory, fileName);

			// Save the file to the storage path
			using (var stream = new FileStream(filePath, FileMode.Create))
			{
				await file.CopyToAsync(stream);
			}

			// Return the file path or a URL to access the file
			var fileUrl = Path.Combine(userId, fileName);
			return Ok(new { FilePath = fileUrl });
		}

		[HttpGet("{userId}/{fileName}")]
		public IActionResult GetProfilePhoto(string userId, string fileName)
		{
			var filePath = Path.Combine(_storagePath, userId, fileName);

			if (!System.IO.File.Exists(filePath))
			{
				return NotFound();
			}

			var fileBytes = System.IO.File.ReadAllBytes(filePath);
			return File(fileBytes, "image/jpeg"); // or "image/png" depending on the file type
		}
	}
}