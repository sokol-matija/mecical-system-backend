using Microsoft.AspNetCore.Mvc;
using MedicalSystem.Core.Models;
using MedicalSystem.Core.Services;

namespace MedicalSystem.API.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class DoctorController : ControllerBase
	{
		private readonly IDoctorService _doctorService;
		private readonly ILogger<DoctorController> _logger;

		public DoctorController(
			IDoctorService doctorService,
			ILogger<DoctorController> logger)
		{
			_doctorService = doctorService;
			_logger = logger;
		}

		[HttpGet]
		public async Task<ActionResult<IEnumerable<Doctor>>> GetAllDoctors()
		{
			try
			{
				var doctors = await _doctorService.GetAllDoctorsAsync();
				return Ok(doctors);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error retrieving all doctors");
				return StatusCode(500, "An error occurred while retrieving doctors");
			}
		}

		[HttpGet("{id}")]
		public async Task<ActionResult<Doctor>> GetDoctor(int id)
		{
			try
			{
				var doctor = await _doctorService.GetDoctorByIdAsync(id);
				return Ok(doctor);
			}
			catch (KeyNotFoundException ex)
			{
				return NotFound(ex.Message);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error retrieving doctor with ID {DoctorId}", id);
				return StatusCode(500, "An error occurred while retrieving the doctor");
			}
		}

		[HttpGet("{id}/details")]
		public async Task<ActionResult<Doctor>> GetDoctorDetails(int id)
		{
			try
			{
				var doctor = await _doctorService.GetDoctorWithDetailsAsync(id);
				return Ok(doctor);
			}
			catch (KeyNotFoundException ex)
			{
				return NotFound(ex.Message);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error retrieving doctor details with ID {DoctorId}", id);
				return StatusCode(500, "An error occurred while retrieving doctor details");
			}
		}

		[HttpGet("with-examinations")]
		public async Task<ActionResult<IEnumerable<Doctor>>> GetDoctorsWithExaminations()
		{
			try
			{
				var doctors = await _doctorService.GetDoctorsWithExaminationsAsync();
				return Ok(doctors);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error retrieving doctors with examinations");
				return StatusCode(500, "An error occurred while retrieving doctors with examinations");
			}
		}

		[HttpPost]
		public async Task<ActionResult<Doctor>> CreateDoctor(Doctor doctor)
		{
			try
			{
				var createdDoctor = await _doctorService.CreateDoctorAsync(doctor);
				return CreatedAtAction(
					nameof(GetDoctor),
					new { id = createdDoctor.Id },
					createdDoctor);
			}
			catch (ArgumentException ex)
			{
				return BadRequest(ex.Message);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error creating new doctor");
				return StatusCode(500, "An error occurred while creating the doctor");
			}
		}

		[HttpPut("{id}")]
		public async Task<IActionResult> UpdateDoctor(int id, Doctor doctor)
		{
			if (id != doctor.Id)
				return BadRequest("ID mismatch");

			try
			{
				await _doctorService.UpdateDoctorAsync(doctor);
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
				_logger.LogError(ex, "Error updating doctor with ID {DoctorId}", id);
				return StatusCode(500, "An error occurred while updating the doctor");
			}
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteDoctor(int id)
		{
			try
			{
				await _doctorService.DeleteDoctorAsync(id);
				return NoContent();
			}
			catch (KeyNotFoundException ex)
			{
				return NotFound(ex.Message);
			}
			catch (InvalidOperationException ex)
			{
				return Conflict(ex.Message);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error deleting doctor with ID {DoctorId}", id);
				return StatusCode(500, "An error occurred while deleting the doctor");
			}
		}
	}
}