using MedicalSystem.Core.Models;
using MedicalSystem.Core.Interfaces;

namespace MedicalSystem.Core.Services
{
	public interface IPatientService
	{
		Task<IEnumerable<Patient>> GetAllPatientsAsync();
		Task<Patient> GetPatientByIdAsync(int id);
		Task<Patient> GetPatientWithFullDetailsAsync(int id);
		Task<Patient> GetPatientByPersonalIdNumberAsync(string personalIdNumber);
		Task<IEnumerable<Patient>> SearchPatientsByLastNameAsync(string lastName);
		Task<Patient> CreatePatientAsync(Patient patient);
		Task UpdatePatientAsync(Patient patient);
		Task DeletePatientAsync(int id);
	}

	public class PatientService : IPatientService
	{
		private readonly IPatientRepository _patientRepository;

		public PatientService(IPatientRepository patientRepository)
		{
			_patientRepository = patientRepository;
		}

		public async Task<IEnumerable<Patient>> GetAllPatientsAsync()
		{
			return await _patientRepository.GetAllAsync();
		}

		public async Task<Patient> GetPatientByIdAsync(int id)
		{
			var patient = await _patientRepository.GetByIdAsync(id);
			if (patient == null)
				throw new KeyNotFoundException($"Patient with ID {id} not found.");
			return patient;
		}

		public async Task<Patient> GetPatientWithFullDetailsAsync(int id)
		{
			var patient = await _patientRepository.GetPatientWithFullDetailsAsync(id);
			if (patient == null)
				throw new KeyNotFoundException($"Patient with ID {id} not found.");
			return patient;
		}

		public async Task<Patient> GetPatientByPersonalIdNumberAsync(string personalIdNumber)
		{
			var patient = await _patientRepository.GetByPersonalIdNumberAsync(personalIdNumber);
			if (patient == null)
				throw new KeyNotFoundException($"Patient with Personal ID Number {personalIdNumber} not found.");
			return patient;
		}

		public async Task<IEnumerable<Patient>> SearchPatientsByLastNameAsync(string lastName)
		{
			if (string.IsNullOrWhiteSpace(lastName))
				throw new ArgumentException("Last name cannot be empty or whitespace.");

			return await _patientRepository.SearchByLastNameAsync(lastName);
		}

		public async Task<Patient> CreatePatientAsync(Patient patient)
		{
			ValidatePatient(patient);

			// Check if patient with same PersonalIdNumber already exists
			var existingPatient = await _patientRepository.GetByPersonalIdNumberAsync(patient.PersonalIdNumber);
			if (existingPatient != null)
				throw new InvalidOperationException($"Patient with Personal ID Number {patient.PersonalIdNumber} already exists.");

			return await _patientRepository.AddAsync(patient);
		}

		public async Task UpdatePatientAsync(Patient patient)
		{
			ValidatePatient(patient);

			var existingPatient = await _patientRepository.GetByIdAsync(patient.Id);
			if (existingPatient == null)
				throw new KeyNotFoundException($"Patient with ID {patient.Id} not found.");

			// Check if trying to update to an existing PersonalIdNumber
			var patientWithSameOib = await _patientRepository.GetByPersonalIdNumberAsync(patient.PersonalIdNumber);
			if (patientWithSameOib != null && patientWithSameOib.Id != patient.Id)
				throw new InvalidOperationException($"Patient with Personal ID Number {patient.PersonalIdNumber} already exists.");

			await _patientRepository.UpdateAsync(patient);
		}

		public async Task DeletePatientAsync(int id)
		{
			var patient = await _patientRepository.GetByIdAsync(id);
			if (patient == null)
				throw new KeyNotFoundException($"Patient with ID {id} not found.");

			await _patientRepository.DeleteAsync(patient);
		}

		private void ValidatePatient(Patient patient)
		{
			if (patient == null)
				throw new ArgumentNullException(nameof(patient));

			if (string.IsNullOrWhiteSpace(patient.FirstName))
				throw new ArgumentException("First name is required.");

			if (string.IsNullOrWhiteSpace(patient.LastName))
				throw new ArgumentException("Last name is required.");

			if (string.IsNullOrWhiteSpace(patient.PersonalIdNumber))
				throw new ArgumentException("Personal ID Number is required.");

			if (patient.PersonalIdNumber.Length != 11 || !patient.PersonalIdNumber.All(char.IsDigit))
				throw new ArgumentException("Personal ID Number must be exactly 11 digits.");

			if (patient.DateOfBirth >= DateTime.Now)
				throw new ArgumentException("Date of birth cannot be in the future.");

			if (!"MFmf".Contains(patient.Gender))
				throw new ArgumentException("Gender must be either 'M' or 'F'.");
		}
	}
}