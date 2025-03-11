using MedicalSystem.Core.Models;
using MedicalSystem.Core.Interfaces;

namespace MedicalSystem.Core.Services
{
	public interface IPrescriptionService
	{
		Task<IEnumerable<Prescription>> GetAllPrescriptionsAsync();
		Task<Prescription> GetPrescriptionByIdAsync(int id);
		Task<IEnumerable<Prescription>> GetPrescriptionsByPatientAsync(int patientId);
		Task<IEnumerable<Prescription>> GetPrescriptionsByDoctorAsync(int doctorId);
		Task<Prescription> CreatePrescriptionAsync(Prescription prescription);
		Task UpdatePrescriptionAsync(Prescription prescription);
		Task DeletePrescriptionAsync(int id);
		Task<byte[]> ExportPrescriptionToPdfAsync(int id);
	}

	public class PrescriptionService : IPrescriptionService
	{
		private readonly IPrescriptionRepository _prescriptionRepository;
		private readonly IPatientRepository _patientRepository;
		private readonly IDoctorRepository _doctorRepository;
		private readonly IExaminationRepository _examinationRepository;

		public PrescriptionService(
			IPrescriptionRepository prescriptionRepository,
			IPatientRepository patientRepository,
			IDoctorRepository doctorRepository,
			IExaminationRepository examinationRepository)
		{
			_prescriptionRepository = prescriptionRepository;
			_patientRepository = patientRepository;
			_doctorRepository = doctorRepository;
			_examinationRepository = examinationRepository;
		}

		public async Task<IEnumerable<Prescription>> GetAllPrescriptionsAsync()
		{
			return await _prescriptionRepository.GetAllAsync();
		}

		public async Task<Prescription> GetPrescriptionByIdAsync(int id)
		{
			var prescription = await _prescriptionRepository.GetByIdAsync(id);
			if (prescription == null)
				throw new KeyNotFoundException($"Prescription with ID {id} not found.");
			return prescription;
		}

		public async Task<IEnumerable<Prescription>> GetPrescriptionsByPatientAsync(int patientId)
		{
			var patient = await _patientRepository.GetByIdAsync(patientId);
			if (patient == null)
				throw new KeyNotFoundException($"Patient with ID {patientId} not found.");

			return await _prescriptionRepository.GetPrescriptionsByPatientAsync(patientId);
		}

		public async Task<IEnumerable<Prescription>> GetPrescriptionsByDoctorAsync(int doctorId)
		{
			var doctor = await _doctorRepository.GetByIdAsync(doctorId);
			if (doctor == null)
				throw new KeyNotFoundException($"Doctor with ID {doctorId} not found.");

			return await _prescriptionRepository.GetPrescriptionsByDoctorAsync(doctorId);
		}

		public async Task<Prescription> CreatePrescriptionAsync(Prescription prescription)
		{
			await ValidatePrescriptionAsync(prescription);

			prescription.PrescriptionDate = DateTime.SpecifyKind(prescription.PrescriptionDate, DateTimeKind.Utc);
			return await _prescriptionRepository.AddAsync(prescription);
		}

		public async Task UpdatePrescriptionAsync(Prescription prescription)
		{
			var existingPrescription = await _prescriptionRepository.GetByIdAsync(prescription.Id);
			if (existingPrescription == null)
				throw new KeyNotFoundException($"Prescription with ID {prescription.Id} not found.");

			await ValidatePrescriptionAsync(prescription);

			prescription.PrescriptionDate = DateTime.SpecifyKind(prescription.PrescriptionDate, DateTimeKind.Utc);
			await _prescriptionRepository.UpdateAsync(prescription);
		}

		public async Task DeletePrescriptionAsync(int id)
		{
			var prescription = await _prescriptionRepository.GetByIdAsync(id);
			if (prescription == null)
				throw new KeyNotFoundException($"Prescription with ID {id} not found.");

			await _prescriptionRepository.DeleteAsync(prescription);
		}

		public async Task<byte[]> ExportPrescriptionToPdfAsync(int id)
		{
			var prescription = await _prescriptionRepository.GetByIdAsync(id);
			if (prescription == null)
				throw new KeyNotFoundException($"Prescription with ID {id} not found.");

			// TODO: Implement PDF generation logic
			throw new NotImplementedException("PDF export functionality will be implemented soon.");
		}

		private async Task ValidatePrescriptionAsync(Prescription prescription)
		{
			if (prescription == null)
				throw new ArgumentNullException(nameof(prescription));

			if (string.IsNullOrWhiteSpace(prescription.Medication))
				throw new ArgumentException("Medication name is required.");

			if (string.IsNullOrWhiteSpace(prescription.Dosage))
				throw new ArgumentException("Dosage information is required.");

			if (string.IsNullOrWhiteSpace(prescription.Instructions))
				throw new ArgumentException("Instructions are required.");

			// Validate Patient exists
			var patient = await _patientRepository.GetByIdAsync(prescription.PatientId);
			if (patient == null)
				throw new ArgumentException($"Patient with ID {prescription.PatientId} not found.");

			// Validate Doctor exists
			var doctor = await _doctorRepository.GetByIdAsync(prescription.DoctorId);
			if (doctor == null)
				throw new ArgumentException($"Doctor with ID {prescription.DoctorId} not found.");

			// Validate Examination exists
			var examination = await _examinationRepository.GetByIdAsync(prescription.ExaminationId);
			if (examination == null)
				throw new ArgumentException($"Examination with ID {prescription.ExaminationId} not found.");

			// Validate prescription date
			if (prescription.PrescriptionDate > DateTime.UtcNow)
				throw new ArgumentException("Prescription date cannot be in the future.");

			// Validate examination and prescription belong to same patient and doctor
			if (examination.PatientId != prescription.PatientId)
				throw new ArgumentException("Prescription patient does not match examination patient.");

			if (examination.DoctorId != prescription.DoctorId)
				throw new ArgumentException("Prescription doctor does not match examination doctor.");
		}
	}
}