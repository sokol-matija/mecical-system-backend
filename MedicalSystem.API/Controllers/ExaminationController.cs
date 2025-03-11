using Microsoft.AspNetCore.Mvc;
using MedicalSystem.Core.Models;
using MedicalSystem.Core.Services;
using static MedicalSystem.Core.Models.Examination;

namespace MedicalSystem.API.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class ExaminationController : ControllerBase
	{
		private readonly IExaminationService _examinationService;
		private readonly ILogger<ExaminationController> _logger;

		public ExaminationController(IExaminationService examinationService, ILogger<ExaminationController> logger)
		{
			_examinationService = examinationService;
			_logger = logger;
		}

		[HttpGet]
		public async Task<ActionResult<IEnumerable<Examination>>> GetAllExaminations()
		{
			try
			{
				var examinations = await _examinationService.GetAllExaminationsAsync();
				return Ok(examinations);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error retrieving all examinations");
				return StatusCode(500, "An error occurred while retrieving examinations");
			}
		}

		[HttpGet("{id}")]
		public async Task<ActionResult<Examination>> GetExamination(int id)
		{
			try
			{
				var examination = await _examinationService.GetExaminationByIdAsync(id);
				return Ok(examination);
			}
			catch (KeyNotFoundException ex)
			{
				return NotFound(ex.Message);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error retrieving examination with ID {ExaminationId}", id);
				return StatusCode(500, "An error occurred while retrieving the examination");
			}
		}

		[HttpGet("{id}/details")]
		public async Task<ActionResult<Examination>> GetExaminationDetails(int id)
		{
			try
			{
				var examination = await _examinationService.GetExaminationWithDetailsAsync(id);
				return Ok(examination);
			}
			catch (KeyNotFoundException ex)
			{
				return NotFound(ex.Message);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error retrieving examination details with ID {ExaminationId}", id);
				return StatusCode(500, "An error occurred while retrieving examination details");
			}
		}

		[HttpGet("patient/{patientId}")]
		public async Task<ActionResult<IEnumerable<Examination>>> GetExaminationsByPatient(int patientId)
		{
			try
			{
				var examinations = await _examinationService.GetExaminationsByPatientAsync(patientId);
				return Ok(examinations);
			}
			catch (KeyNotFoundException ex)
			{
				return NotFound(ex.Message);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error retrieving examinations for patient ID {PatientId}", patientId);
				return StatusCode(500, "An error occurred while retrieving patient examinations");
			}
		}

		[HttpGet("doctor/{doctorId}")]
		public async Task<ActionResult<IEnumerable<Examination>>> GetExaminationsByDoctor(int doctorId)
		{
			try
			{
				var examinations = await _examinationService.GetExaminationsByDoctorAsync(doctorId);
				return Ok(examinations);
			}
			catch (KeyNotFoundException ex)
			{
				return NotFound(ex.Message);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error retrieving examinations for doctor ID {DoctorId}", doctorId);
				return StatusCode(500, "An error occurred while retrieving doctor examinations");
			}
		}

		[HttpPost]
		public async Task<ActionResult<Examination>> CreateExamination(ExaminationCreateDto dto)
		{
			try
			{
				// Convert DTO to domain model
				var examination = new Examination
				{
					PatientId = dto.PatientId,
					DoctorId = dto.DoctorId,
					Type = (ExaminationType)dto.Type,
					ExaminationDateTime = dto.ExaminationDateTime,
					Notes = dto.Notes
				};

				var createdExamination = await _examinationService.CreateExaminationAsync(examination);
				return CreatedAtAction(nameof(GetExamination), new { id = createdExamination.Id }, createdExamination);
			}
			catch (ArgumentException ex)
			{
				return BadRequest(ex.Message);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error creating new examination");
				return StatusCode(500, "An error occurred while creating the examination");
			}
		}

		[HttpPut("{id}")]
		public async Task<IActionResult> UpdateExamination(int id, Examination examination)
		{
			if (id != examination.Id)
				return BadRequest("ID mismatch");

			try
			{
				await _examinationService.UpdateExaminationAsync(examination);
				return NoContent();
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
				_logger.LogError(ex, "Error updating examination with ID {ExaminationId}", id);
				return StatusCode(500, "An error occurred while updating the examination");
			}
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteExamination(int id)
		{
			try
			{
				await _examinationService.DeleteExaminationAsync(id);
				return NoContent();
			}
			catch (KeyNotFoundException ex)
			{
				return NotFound(ex.Message);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error deleting examination with ID {ExaminationId}", id);
				return StatusCode(500, "An error occurred while deleting the examination");
			}
		}
	}
}