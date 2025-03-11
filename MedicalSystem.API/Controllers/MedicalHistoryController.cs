using Microsoft.AspNetCore.Mvc;
using MedicalSystem.Core.Models;
using MedicalSystem.Core.Services;

namespace MedicalSystem.API.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class MedicalHistoryController : ControllerBase
	{
		private readonly IMedicalHistoryService _medicalHistoryService;
		private readonly ILogger<MedicalHistoryController> _logger;

		public MedicalHistoryController(
			IMedicalHistoryService medicalHistoryService,
			ILogger<MedicalHistoryController> logger)
		{
			_medicalHistoryService = medicalHistoryService;
			_logger = logger;
		}

		[HttpGet]
		public async Task<ActionResult<IEnumerable<MedicalHistory>>> GetAllMedicalHistories()
		{
			try
			{
				var histories = await _medicalHistoryService.GetAllMedicalHistoriesAsync();
				return Ok(histories);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error retrieving all medical histories");
				return StatusCode(500, "An error occurred while retrieving medical histories");
			}
		}

		[HttpGet("{id}")]
		public async Task<ActionResult<MedicalHistory>> GetMedicalHistory(int id)
		{
			try
			{
				var history = await _medicalHistoryService.GetMedicalHistoryByIdAsync(id);
				return Ok(history);
			}
			catch (KeyNotFoundException ex)
			{
				return NotFound(ex.Message);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error retrieving medical history with ID {HistoryId}", id);
				return StatusCode(500, "An error occurred while retrieving the medical history");
			}
		}

		[HttpGet("patient/{patientId}")]
		public async Task<ActionResult<IEnumerable<MedicalHistory>>> GetPatientHistory(int patientId)
		{
			try
			{
				var histories = await _medicalHistoryService.GetPatientHistoryAsync(patientId);
				return Ok(histories);
			}
			catch (KeyNotFoundException ex)
			{
				return NotFound(ex.Message);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error retrieving medical histories for patient ID {PatientId}", patientId);
				return StatusCode(500, "An error occurred while retrieving patient medical histories");
			}
		}

		[HttpGet("patient/{patientId}/active")]
		public async Task<ActionResult<IEnumerable<MedicalHistory>>> GetActiveConditions(int patientId)
		{
			try
			{
				var activeConditions = await _medicalHistoryService.GetActiveConditionsAsync(patientId);
				return Ok(activeConditions);
			}
			catch (KeyNotFoundException ex)
			{
				return NotFound(ex.Message);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error retrieving active conditions for patient ID {PatientId}", patientId);
				return StatusCode(500, "An error occurred while retrieving active conditions");
			}
		}

		[HttpPost]
		public async Task<ActionResult<MedicalHistory>> CreateMedicalHistory(MedicalHistory medicalHistory)
		{
			try
			{
				var createdHistory = await _medicalHistoryService.CreateMedicalHistoryAsync(medicalHistory);
				return CreatedAtAction(
					nameof(GetMedicalHistory),
					new { id = createdHistory.Id },
					createdHistory);
			}
			catch (ArgumentException ex)
			{
				return BadRequest(ex.Message);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error creating new medical history");
				return StatusCode(500, "An error occurred while creating the medical history");
			}
		}

		[HttpPut("{id}")]
		public async Task<IActionResult> UpdateMedicalHistory(int id, MedicalHistory medicalHistory)
		{
			if (id != medicalHistory.Id)
				return BadRequest("ID mismatch");

			try
			{
				await _medicalHistoryService.UpdateMedicalHistoryAsync(medicalHistory);
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
				_logger.LogError(ex, "Error updating medical history with ID {HistoryId}", id);
				return StatusCode(500, "An error occurred while updating the medical history");
			}
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteMedicalHistory(int id)
		{
			try
			{
				await _medicalHistoryService.DeleteMedicalHistoryAsync(id);
				return NoContent();
			}
			catch (KeyNotFoundException ex)
			{
				return NotFound(ex.Message);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error deleting medical history with ID {HistoryId}", id);
				return StatusCode(500, "An error occurred while deleting the medical history");
			}
		}
	}
}