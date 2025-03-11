using MedicalSystem.Core.Models;
using MedicalSystem.Core.Interfaces;

namespace MedicalSystem.Core.Services
{
	public interface IExaminationService
	{
		Task<IEnumerable<Examination>> GetAllExaminationsAsync();
		Task<Examination> GetExaminationByIdAsync(int id);
		Task<Examination> GetExaminationWithDetailsAsync(int id);
		Task<IEnumerable<Examination>> GetExaminationsByPatientAsync(int patientId);
		Task<IEnumerable<Examination>> GetExaminationsByDoctorAsync(int doctorId);
		Task<Examination> CreateExaminationAsync(Examination examination);
		Task UpdateExaminationAsync(Examination examination);
		Task DeleteExaminationAsync(int id);
	}

	public class ExaminationService : IExaminationService
	{
		private readonly IExaminationRepository _examinationRepository;
		private readonly IPatientRepository _patientRepository;
		private readonly IDoctorRepository _doctorRepository;

		public ExaminationService(
			IExaminationRepository examinationRepository,
			IPatientRepository patientRepository,
			IDoctorRepository doctorRepository)
		{
			_examinationRepository = examinationRepository;
			_patientRepository = patientRepository;
			_doctorRepository = doctorRepository;
		}

		public async Task<IEnumerable<Examination>> GetAllExaminationsAsync()
		{
			return await _examinationRepository.GetAllAsync();
		}

		public async Task<Examination> GetExaminationByIdAsync(int id)
		{
			var examination = await _examinationRepository.GetByIdAsync(id);
			if (examination == null)
				throw new KeyNotFoundException($"Examination with ID {id} not found.");
			return examination;
		}

		public async Task<Examination> GetExaminationWithDetailsAsync(int id)
		{
			var examination = await _examinationRepository.GetExaminationWithDetailsAsync(id);
			if (examination == null)
				throw new KeyNotFoundException($"Examination with ID {id} not found.");
			return examination;
		}

		public async Task<IEnumerable<Examination>> GetExaminationsByPatientAsync(int patientId)
		{
			var patient = await _patientRepository.GetByIdAsync(patientId);
			if (patient == null)
				throw new KeyNotFoundException($"Patient with ID {patientId} not found.");

			return await _examinationRepository.GetExaminationsByPatientAsync(patientId);
		}

		public async Task<IEnumerable<Examination>> GetExaminationsByDoctorAsync(int doctorId)
		{
			var doctor = await _doctorRepository.GetByIdAsync(doctorId);
			if (doctor == null)
				throw new KeyNotFoundException($"Doctor with ID {doctorId} not found.");

			return await _examinationRepository.GetExaminationsByDoctorAsync(doctorId);
		}

		public async Task<Examination> CreateExaminationAsync(Examination examination)
		{
			await ValidateExaminationAsync(examination);

			examination.ExaminationDateTime = DateTime.SpecifyKind(examination.ExaminationDateTime, DateTimeKind.Utc);
			return await _examinationRepository.AddAsync(examination);
		}

		public async Task UpdateExaminationAsync(Examination examination)
		{
			var existingExamination = await _examinationRepository.GetByIdAsync(examination.Id);
			if (existingExamination == null)
				throw new KeyNotFoundException($"Examination with ID {examination.Id} not found.");

			await ValidateExaminationAsync(examination);

			examination.ExaminationDateTime = DateTime.SpecifyKind(examination.ExaminationDateTime, DateTimeKind.Utc);
			await _examinationRepository.UpdateAsync(examination);
		}

		public async Task DeleteExaminationAsync(int id)
		{
			var examination = await _examinationRepository.GetByIdAsync(id);
			if (examination == null)
				throw new KeyNotFoundException($"Examination with ID {id} not found.");

			await _examinationRepository.DeleteAsync(examination);
		}

		private async Task ValidateExaminationAsync(Examination examination)
		{
			if (examination == null)
				throw new ArgumentNullException(nameof(examination));

			if (examination.ExaminationDateTime > DateTime.UtcNow.AddYears(1))
				throw new ArgumentException("Examination date cannot be more than one year in the future.");

			if (string.IsNullOrWhiteSpace(examination.Notes))
				throw new ArgumentException("Examination notes are required.");

			// Validate Patient exists
			var patient = await _patientRepository.GetByIdAsync(examination.PatientId);
			if (patient == null)
				throw new ArgumentException($"Patient with ID {examination.PatientId} not found.");

			// Validate Doctor exists
			var doctor = await _doctorRepository.GetByIdAsync(examination.DoctorId);
			if (doctor == null)
				throw new ArgumentException($"Doctor with ID {examination.DoctorId} not found.");
		}
	}
}