using Microsoft.EntityFrameworkCore;
using MedicalSystem.Core.Models;
using MedicalSystem.Core.Interfaces;
using MedicalSystem.Infrastructure.Data;

namespace MedicalSystem.Infrastructure.Repositories
{
	public class PrescriptionRepository : Repository<Prescription>, IPrescriptionRepository
	{
		public PrescriptionRepository(MedicalSystemDbContext context) : base(context)
		{
		}

		public async Task<IEnumerable<Prescription>> GetPrescriptionsByPatientAsync(int patientId)
		{
			return await _dbSet
				.Where(p => p.PatientId == patientId)
				.Include(p => p.Doctor)
				.Include(p => p.Examination)
				.OrderByDescending(p => p.PrescriptionDate)
				.ToListAsync();
		}

		public async Task<IEnumerable<Prescription>> GetPrescriptionsByDoctorAsync(int doctorId)
		{
			return await _dbSet
				.Where(p => p.DoctorId == doctorId)
				.Include(p => p.Patient)
				.Include(p => p.Examination)
				.OrderByDescending(p => p.PrescriptionDate)
				.ToListAsync();
		}

		public override async Task<IEnumerable<Prescription>> GetAllAsync()
		{
			return await _dbSet
				.Include(p => p.Patient)
				.Include(p => p.Doctor)
				.Include(p => p.Examination)
				.OrderByDescending(p => p.PrescriptionDate)
				.ToListAsync();
		}

		public override async Task<Prescription> GetByIdAsync(int id)
		{
			return await _dbSet
				.Include(p => p.Patient)
				.Include(p => p.Doctor)
				.Include(p => p.Examination)
				.FirstOrDefaultAsync(p => p.Id == id);
		}
	}
}