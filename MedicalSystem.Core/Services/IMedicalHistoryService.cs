using MedicalSystem.Core.Models;
using MedicalSystem.Core.Interfaces;

namespace MedicalSystem.Core.Services
{
	public interface IMedicalHistoryService
	{
		Task<IEnumerable<MedicalHistory>> GetAllMedicalHistoriesAsync();
		Task<MedicalHistory> GetMedicalHistoryByIdAsync(int id);
		Task<IEnumerable<MedicalHistory>> GetPatientHistoryAsync(int patientId);
		Task<IEnumerable<MedicalHistory>> GetActiveConditionsAsync(int patientId);
		Task<MedicalHistory> CreateMedicalHistoryAsync(MedicalHistory medicalHistory);
		Task UpdateMedicalHistoryAsync(MedicalHistory medicalHistory);
		Task DeleteMedicalHistoryAsync(int id);
	}

	public class MedicalHistoryService : IMedicalHistoryService
	{
		private readonly IMedicalHistoryRepository _medicalHistoryRepository;
		private readonly IPatientRepository _patientRepository;

		public MedicalHistoryService(
			IMedicalHistoryRepository medicalHistoryRepository,
			IPatientRepository patientRepository)
		{
			_medicalHistoryRepository = medicalHistoryRepository;
			_patientRepository = patientRepository;
		}

		public async Task<IEnumerable<MedicalHistory>> GetAllMedicalHistoriesAsync()
		{
			return await _medicalHistoryRepository.GetAllAsync();
		}

		public async Task<MedicalHistory> GetMedicalHistoryByIdAsync(int id)
		{
			var medicalHistory = await _medicalHistoryRepository.GetByIdAsync(id);
			if (medicalHistory == null)
				throw new KeyNotFoundException($"Medical history with ID {id} not found.");
			return medicalHistory;
		}

		public async Task<IEnumerable<MedicalHistory>> GetPatientHistoryAsync(int patientId)
		{
			var patient = await _patientRepository.GetByIdAsync(patientId);
			if (patient == null)
				throw new KeyNotFoundException($"Patient with ID {patientId} not found.");

			return await _medicalHistoryRepository.GetPatientHistoryAsync(patientId);
		}

		public async Task<IEnumerable<MedicalHistory>> GetActiveConditionsAsync(int patientId)
		{
			var patient = await _patientRepository.GetByIdAsync(patientId);
			if (patient == null)
				throw new KeyNotFoundException($"Patient with ID {patientId} not found.");

			return await _medicalHistoryRepository.GetActiveConditionsAsync(patientId);
		}

		public async Task<MedicalHistory> CreateMedicalHistoryAsync(MedicalHistory medicalHistory)
		{
			await ValidateMedicalHistoryAsync(medicalHistory);

			medicalHistory.StartDate = DateTime.SpecifyKind(medicalHistory.StartDate, DateTimeKind.Utc);
			if (medicalHistory.EndDate.HasValue)
				medicalHistory.EndDate = DateTime.SpecifyKind(medicalHistory.EndDate.Value, DateTimeKind.Utc);

			return await _medicalHistoryRepository.AddAsync(medicalHistory);
		}

		public async Task UpdateMedicalHistoryAsync(MedicalHistory medicalHistory)
		{
			var existingHistory = await _medicalHistoryRepository.GetByIdAsync(medicalHistory.Id);
			if (existingHistory == null)
				throw new KeyNotFoundException($"Medical history with ID {medicalHistory.Id} not found.");

			await ValidateMedicalHistoryAsync(medicalHistory);

			medicalHistory.StartDate = DateTime.SpecifyKind(medicalHistory.StartDate, DateTimeKind.Utc);
			if (medicalHistory.EndDate.HasValue)
				medicalHistory.EndDate = DateTime.SpecifyKind(medicalHistory.EndDate.Value, DateTimeKind.Utc);

			await _medicalHistoryRepository.UpdateAsync(medicalHistory);
		}

		public async Task DeleteMedicalHistoryAsync(int id)
		{
			var medicalHistory = await _medicalHistoryRepository.GetByIdAsync(id);
			if (medicalHistory == null)
				throw new KeyNotFoundException($"Medical history with ID {id} not found.");

			await _medicalHistoryRepository.DeleteAsync(medicalHistory);
		}

		private async Task ValidateMedicalHistoryAsync(MedicalHistory medicalHistory)
		{
			if (medicalHistory == null)
				throw new ArgumentNullException(nameof(medicalHistory));

			if (string.IsNullOrWhiteSpace(medicalHistory.DiseaseName))
				throw new ArgumentException("Disease name is required.");

			// Validate Patient exists
			var patient = await _patientRepository.GetByIdAsync(medicalHistory.PatientId);
			if (patient == null)
				throw new ArgumentException($"Patient with ID {medicalHistory.PatientId} not found.");

			// Validate dates
			if (medicalHistory.StartDate > DateTime.UtcNow)
				throw new ArgumentException("Start date cannot be in the future.");

			if (medicalHistory.EndDate.HasValue)
			{
				if (medicalHistory.EndDate.Value > DateTime.UtcNow)
					throw new ArgumentException("End date cannot be in the future.");

				if (medicalHistory.EndDate.Value < medicalHistory.StartDate)
					throw new ArgumentException("End date cannot be before start date.");
			}
		}
	}
}