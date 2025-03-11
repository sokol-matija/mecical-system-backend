using Microsoft.AspNetCore.Mvc;
using MedicalSystem.Core.Models;
using MedicalSystem.Core.Services;

namespace MedicalSystem.API.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class PrescriptionController : ControllerBase
	{
		private readonly IPrescriptionService _prescriptionService;
		private readonly ILogger<PrescriptionController> _logger;

		public PrescriptionController(
			IPrescriptionService prescriptionService,
			ILogger<PrescriptionController> logger)
		{
			_prescriptionService = prescriptionService;
			_logger = logger;
		}

		[HttpGet]
		public async Task<ActionResult<IEnumerable<Prescription>>> GetAllPrescriptions()
		{
			try
			{
				var prescriptions = await _prescriptionService.GetAllPrescriptionsAsync();
				return Ok(prescriptions);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error retrieving all prescriptions");
				return StatusCode(500, "An error occurred while retrieving prescriptions");
			}
		}

		[HttpGet("{id}")]
		public async Task<ActionResult<Prescription>> GetPrescription(int id)
		{
			try
			{
				var prescription = await _prescriptionService.GetPrescriptionByIdAsync(id);
				return Ok(prescription);
			}
			catch (KeyNotFoundException ex)
			{
				return NotFound(ex.Message);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error retrieving prescription with ID {PrescriptionId}", id);
				return StatusCode(500, "An error occurred while retrieving the prescription");
			}
		}

		[HttpGet("patient/{patientId}")]
		public async Task<ActionResult<IEnumerable<Prescription>>> GetPrescriptionsByPatient(int patientId)
		{
			try
			{
				var prescriptions = await _prescriptionService.GetPrescriptionsByPatientAsync(patientId);
				return Ok(prescriptions);
			}
			catch (KeyNotFoundException ex)
			{
				return NotFound(ex.Message);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error retrieving prescriptions for patient ID {PatientId}", patientId);
				return StatusCode(500, "An error occurred while retrieving patient prescriptions");
			}
		}

		[HttpGet("doctor/{doctorId}")]
		public async Task<ActionResult<IEnumerable<Prescription>>> GetPrescriptionsByDoctor(int doctorId)
		{
			try
			{
				var prescriptions = await _prescriptionService.GetPrescriptionsByDoctorAsync(doctorId);
				return Ok(prescriptions);
			}
			catch (KeyNotFoundException ex)
			{
				return NotFound(ex.Message);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error retrieving prescriptions for doctor ID {DoctorId}", doctorId);
				return StatusCode(500, "An error occurred while retrieving doctor prescriptions");
			}
		}

		[HttpPost]
		public async Task<ActionResult<Prescription>> CreatePrescription(Prescription prescription)
		{
			try
			{
				var createdPrescription = await _prescriptionService.CreatePrescriptionAsync(prescription);
				return CreatedAtAction(
					nameof(GetPrescription),
					new { id = createdPrescription.Id },
					createdPrescription);
			}
			catch (ArgumentException ex)
			{
				return BadRequest(ex.Message);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error creating new prescription");
				return StatusCode(500, "An error occurred while creating the prescription");
			}
		}

		[HttpPut("{id}")]
		public async Task<IActionResult> UpdatePrescription(int id, Prescription prescription)
		{
			if (id != prescription.Id)
				return BadRequest("ID mismatch");

			try
			{
				await _prescriptionService.UpdatePrescriptionAsync(prescription);
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
				_logger.LogError(ex, "Error updating prescription with ID {PrescriptionId}", id);
				return StatusCode(500, "An error occurred while updating the prescription");
			}
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> DeletePrescription(int id)
		{
			try
			{
				await _prescriptionService.DeletePrescriptionAsync(id);
				return NoContent();
			}
			catch (KeyNotFoundException ex)
			{
				return NotFound(ex.Message);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error deleting prescription with ID {PrescriptionId}", id);
				return StatusCode(500, "An error occurred while deleting the prescription");
			}
		}

		[HttpGet("{id}/pdf")]
		public async Task<IActionResult> ExportPrescriptionToPdf(int id)
		{
			try
			{
				var pdfBytes = await _prescriptionService.ExportPrescriptionToPdfAsync(id);
				return File(pdfBytes, "application/pdf", $"prescription_{id}.pdf");
			}
			catch (KeyNotFoundException ex)
			{
				return NotFound(ex.Message);
			}
			catch (NotImplementedException)
			{
				return StatusCode(501, "PDF export functionality is not yet implemented");
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error exporting prescription ID {PrescriptionId} to PDF", id);
				return StatusCode(500, "An error occurred while exporting the prescription to PDF");
			}
		}
	}
}