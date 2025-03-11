using Microsoft.AspNetCore.Mvc;
using MedicalSystem.Core.Models;
using MedicalSystem.Core.Services;

namespace MedicalSystem.API.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class MedicalImageController : ControllerBase
	{
		private readonly IMedicalImageService _medicalImageService;
		private readonly ILogger<MedicalImageController> _logger;

		public MedicalImageController(
			IMedicalImageService medicalImageService,
			ILogger<MedicalImageController> logger)
		{
			_medicalImageService = medicalImageService;
			_logger = logger;
		}

		[HttpGet]
		public async Task<ActionResult<IEnumerable<MedicalImage>>> GetAllMedicalImages()
		{
			try
			{
				var images = await _medicalImageService.GetAllMedicalImagesAsync();
				return Ok(images);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error retrieving all medical images");
				return StatusCode(500, "An error occurred while retrieving medical images");
			}
		}

		[HttpGet("{id}")]
		public async Task<ActionResult<MedicalImage>> GetMedicalImage(int id)
		{
			try
			{
				var image = await _medicalImageService.GetMedicalImageByIdAsync(id);
				return Ok(image);
			}
			catch (KeyNotFoundException ex)
			{
				return NotFound(ex.Message);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error retrieving medical image with ID {ImageId}", id);
				return StatusCode(500, "An error occurred while retrieving the medical image");
			}
		}

		[HttpGet("examination/{examinationId}")]
		public async Task<ActionResult<IEnumerable<MedicalImage>>> GetImagesByExamination(int examinationId)
		{
			try
			{
				var images = await _medicalImageService.GetImagesByExaminationAsync(examinationId);
				return Ok(images);
			}
			catch (KeyNotFoundException ex)
			{
				return NotFound(ex.Message);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error retrieving medical images for examination ID {ExaminationId}", examinationId);
				return StatusCode(500, "An error occurred while retrieving medical images");
			}
		}

		[HttpGet("{id}/download")]
		public async Task<IActionResult> DownloadMedicalImage(int id)
		{
			try
			{
				var (fileContents, contentType, fileName) = await _medicalImageService.GetMedicalImageFileAsync(id);
				return File(fileContents, contentType, fileName);
			}
			catch (KeyNotFoundException ex)
			{
				return NotFound(ex.Message);
			}
			catch (FileNotFoundException ex)
			{
				return NotFound(ex.Message);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error downloading medical image with ID {ImageId}", id);
				return StatusCode(500, "An error occurred while downloading the medical image");
			}
		}

		[HttpPost("examination/{examinationId}/upload")]
		public async Task<ActionResult<MedicalImage>> UploadMedicalImage(int examinationId, IFormFile file)
		{
			try
			{
				if (file == null || file.Length == 0)
					return BadRequest("No file was uploaded.");

				var uploadedImage = await _medicalImageService.UploadMedicalImageAsync(examinationId, file);
				return CreatedAtAction(
					nameof(GetMedicalImage),
					new { id = uploadedImage.Id },
					uploadedImage);
			}
			catch (KeyNotFoundException ex)
			{
				return NotFound(ex.Message);
			}
			catch (ArgumentException ex)
			{
				return BadRequest(ex.Message);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error uploading medical image for examination ID {ExaminationId}", examinationId);
				return StatusCode(500, "An error occurred while uploading the medical image");
			}
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteMedicalImage(int id)
		{
			try
			{
				await _medicalImageService.DeleteMedicalImageAsync(id);
				return NoContent();
			}
			catch (KeyNotFoundException ex)
			{
				return NotFound(ex.Message);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error deleting medical image with ID {ImageId}", id);
				return StatusCode(500, "An error occurred while deleting the medical image");
			}
		}
	}
}