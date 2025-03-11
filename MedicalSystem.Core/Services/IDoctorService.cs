using MedicalSystem.Core.Models;
using MedicalSystem.Core.Interfaces;

namespace MedicalSystem.Core.Services
{
	public interface IDoctorService
	{
		Task<IEnumerable<Doctor>> GetAllDoctorsAsync();
		Task<Doctor> GetDoctorByIdAsync(int id);
		Task<Doctor> GetDoctorWithDetailsAsync(int id);
		Task<IEnumerable<Doctor>> GetDoctorsWithExaminationsAsync();
		Task<Doctor> CreateDoctorAsync(Doctor doctor);
		Task UpdateDoctorAsync(Doctor doctor);
		Task DeleteDoctorAsync(int id);
	}

	public class DoctorService : IDoctorService
	{
		private readonly IDoctorRepository _doctorRepository;

		public DoctorService(IDoctorRepository doctorRepository)
		{
			_doctorRepository = doctorRepository;
		}

		public async Task<IEnumerable<Doctor>> GetAllDoctorsAsync()
		{
			return await _doctorRepository.GetAllAsync();
		}

		public async Task<Doctor> GetDoctorByIdAsync(int id)
		{
			var doctor = await _doctorRepository.GetByIdAsync(id);
			if (doctor == null)
				throw new KeyNotFoundException($"Doctor with ID {id} not found.");
			return doctor;
		}

		public async Task<Doctor> GetDoctorWithDetailsAsync(int id)
		{
			var doctor = await _doctorRepository.GetDoctorWithDetailsAsync(id);
			if (doctor == null)
				throw new KeyNotFoundException($"Doctor with ID {id} not found.");
			return doctor;
		}

		public async Task<IEnumerable<Doctor>> GetDoctorsWithExaminationsAsync()
		{
			return await _doctorRepository.GetDoctorsWithExaminationsAsync();
		}

		public async Task<Doctor> CreateDoctorAsync(Doctor doctor)
		{
			ValidateDoctor(doctor);
			return await _doctorRepository.AddAsync(doctor);
		}

		public async Task UpdateDoctorAsync(Doctor doctor)
		{
			ValidateDoctor(doctor);

			var existingDoctor = await _doctorRepository.GetByIdAsync(doctor.Id);
			if (existingDoctor == null)
				throw new KeyNotFoundException($"Doctor with ID {doctor.Id} not found.");

			await _doctorRepository.UpdateAsync(doctor);
		}

		public async Task DeleteDoctorAsync(int id)
		{
			var doctor = await _doctorRepository.GetByIdAsync(id);
			if (doctor == null)
				throw new KeyNotFoundException($"Doctor with ID {id} not found.");

			// Check if doctor has any associated examinations or prescriptions
			var doctorWithDetails = await _doctorRepository.GetDoctorWithDetailsAsync(id);
			if (doctorWithDetails.Examinations.Any() || doctorWithDetails.Prescriptions.Any())
				throw new InvalidOperationException("Cannot delete doctor with associated examinations or prescriptions.");

			await _doctorRepository.DeleteAsync(doctor);
		}

		private void ValidateDoctor(Doctor doctor)
		{
			if (doctor == null)
				throw new ArgumentNullException(nameof(doctor));

			if (string.IsNullOrWhiteSpace(doctor.FirstName))
				throw new ArgumentException("First name is required.");

			if (string.IsNullOrWhiteSpace(doctor.LastName))
				throw new ArgumentException("Last name is required.");

			if (string.IsNullOrWhiteSpace(doctor.Specialization))
				throw new ArgumentException("Specialization is required.");
		}
	}
}