using Microsoft.AspNetCore.Mvc;
using MedicalSystem.Core.Models;
using MedicalSystem.Core.Services;

namespace MedicalSystem.API.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class PatientController : ControllerBase
	{
		private readonly IPatientService _patientService;
		private readonly ILogger<PatientController> _logger;

		public PatientController(IPatientService patientService, ILogger<PatientController> logger)
		{
			_patientService = patientService;
			_logger = logger;
		}

		[HttpGet]
		public async Task<ActionResult<IEnumerable<Patient>>> GetAllPatients()
		{
			try
			{
				var patients = await _patientService.GetAllPatientsAsync();
				return Ok(patients);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error retrieving all patients");
				return StatusCode(500, "An error occurred while retrieving patients");
			}
		}

		[HttpGet("{id}")]
		public async Task<ActionResult<Patient>> GetPatient(int id)
		{
			try
			{
				var patient = await _patientService.GetPatientByIdAsync(id);
				return Ok(patient);
			}
			catch (KeyNotFoundException ex)
			{
				return NotFound(ex.Message);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error retrieving patient with ID {PatientId}", id);
				return StatusCode(500, "An error occurred while retrieving the patient");
			}
		}

		[HttpGet("{id}/details")]
		public async Task<ActionResult<Patient>> GetPatientDetails(int id)
		{
			try
			{
				var patient = await _patientService.GetPatientWithFullDetailsAsync(id);
				return Ok(patient);
			}
			catch (KeyNotFoundException ex)
			{
				return NotFound(ex.Message);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error retrieving patient details with ID {PatientId}", id);
				return StatusCode(500, "An error occurred while retrieving patient details");
			}
		}

		[HttpGet("search")]
		public async Task<ActionResult<IEnumerable<Patient>>> SearchPatients([FromQuery] string lastName)
		{
			try
			{
				var patients = await _patientService.SearchPatientsByLastNameAsync(lastName);
				return Ok(patients);
			}
			catch (ArgumentException ex)
			{
				return BadRequest(ex.Message);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error searching patients by last name {LastName}", lastName);
				return StatusCode(500, "An error occurred while searching for patients");
			}
		}

		[HttpGet("oib/{personalIdNumber}")]
		public async Task<ActionResult<Patient>> GetPatientByOib(string personalIdNumber)
		{
			try
			{
				var patient = await _patientService.GetPatientByPersonalIdNumberAsync(personalIdNumber);
				return Ok(patient);
			}
			catch (KeyNotFoundException ex)
			{
				return NotFound(ex.Message);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error retrieving patient by OIB {PersonalIdNumber}", personalIdNumber);
				return StatusCode(500, "An error occurred while retrieving the patient");
			}
		}

		[HttpPost]
		public async Task<ActionResult<Patient>> CreatePatient(Patient patient)
		{
			try
			{
				var createdPatient = await _patientService.CreatePatientAsync(patient);
				return CreatedAtAction(nameof(GetPatient), new { id = createdPatient.Id }, createdPatient);
			}
			catch (ArgumentException ex)
			{
				return BadRequest(ex.Message);
			}
			catch (InvalidOperationException ex)
			{
				return Conflict(ex.Message);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error creating new patient");
				return StatusCode(500, "An error occurred while creating the patient");
			}
		}

		[HttpPut("{id}")]
		public async Task<IActionResult> UpdatePatient(int id, Patient patient)
		{
			if (id != patient.Id)
				return BadRequest("ID mismatch");

			try
			{
				await _patientService.UpdatePatientAsync(patient);
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
			catch (InvalidOperationException ex)
			{
				return Conflict(ex.Message);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error updating patient with ID {PatientId}", id);
				return StatusCode(500, "An error occurred while updating the patient");
			}
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> DeletePatient(int id)
		{
			try
			{
				await _patientService.DeletePatientAsync(id);
				return NoContent();
			}
			catch (KeyNotFoundException ex)
			{
				return NotFound(ex.Message);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error deleting patient with ID {PatientId}", id);
				return StatusCode(500, "An error occurred while deleting the patient");
			}
		}
	}
}