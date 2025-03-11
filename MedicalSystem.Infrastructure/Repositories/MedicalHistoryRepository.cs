using Microsoft.EntityFrameworkCore;
using MedicalSystem.Core.Models;
using MedicalSystem.Core.Interfaces;
using MedicalSystem.Infrastructure.Data;

namespace MedicalSystem.Infrastructure.Repositories
{
	public class MedicalHistoryRepository : Repository<MedicalHistory>, IMedicalHistoryRepository
	{
		public MedicalHistoryRepository(MedicalSystemDbContext context) : base(context)
		{
		}

		public async Task<IEnumerable<MedicalHistory>> GetPatientHistoryAsync(int patientId)
		{
			return await _dbSet
				.Where(mh => mh.PatientId == patientId)
				.OrderByDescending(mh => mh.StartDate)
				.Include(mh => mh.Patient)
				.ToListAsync();
		}

		public async Task<IEnumerable<MedicalHistory>> GetActiveConditionsAsync(int patientId)
		{
			return await _dbSet
				.Where(mh => mh.PatientId == patientId && mh.EndDate == null)
				.OrderByDescending(mh => mh.StartDate)
				.Include(mh => mh.Patient)
				.ToListAsync();
		}

		public override async Task<IEnumerable<MedicalHistory>> GetAllAsync()
		{
			return await _dbSet
				.Include(mh => mh.Patient)
				.OrderByDescending(mh => mh.StartDate)
				.ToListAsync();
		}

		public override async Task<MedicalHistory> GetByIdAsync(int id)
		{
			return await _dbSet
				.Include(mh => mh.Patient)
				.FirstOrDefaultAsync(mh => mh.Id == id);
		}
	}
}