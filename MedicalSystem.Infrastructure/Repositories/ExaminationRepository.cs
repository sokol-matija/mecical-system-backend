using Microsoft.EntityFrameworkCore;
using MedicalSystem.Core.Models;
using MedicalSystem.Core.Interfaces;
using MedicalSystem.Infrastructure.Data;

namespace MedicalSystem.Infrastructure.Repositories
{
	public class ExaminationRepository : Repository<Examination>, IExaminationRepository
	{
		public ExaminationRepository(MedicalSystemDbContext context) : base(context)
		{
		}

		public async Task<IEnumerable<Examination>> GetExaminationsByPatientAsync(int patientId)
		{
			return await _dbSet
				.Include(e => e.Doctor)
				.Include(e => e.MedicalImages)
				.Include(e => e.Prescriptions)
				.Where(e => e.PatientId == patientId)
				.OrderByDescending(e => e.ExaminationDateTime)
				.ToListAsync();
		}

		public async Task<IEnumerable<Examination>> GetExaminationsByDoctorAsync(int doctorId)
		{
			return await _dbSet
				.Include(e => e.Patient)
				.Include(e => e.MedicalImages)
				.Include(e => e.Prescriptions)
				.Where(e => e.DoctorId == doctorId)
				.OrderByDescending(e => e.ExaminationDateTime)
				.ToListAsync();
		}

		public async Task<Examination> GetExaminationWithDetailsAsync(int id)
		{
			return await _dbSet
				.Include(e => e.Patient)
				.Include(e => e.Doctor)
				.Include(e => e.MedicalImages)
				.Include(e => e.Prescriptions)
					.ThenInclude(p => p.Doctor)
				.FirstOrDefaultAsync(e => e.Id == id);
		}

		public override async Task<IEnumerable<Examination>> GetAllAsync()
		{
			return await _dbSet
				.Include(e => e.Patient)
				.Include(e => e.Doctor)
				.OrderByDescending(e => e.ExaminationDateTime)
				.ToListAsync();
		}

		public override async Task<Examination> GetByIdAsync(int id)
		{
			return await _dbSet
				.Include(e => e.Patient)
				.Include(e => e.Doctor)
				.FirstOrDefaultAsync(e => e.Id == id);
		}
	}
}