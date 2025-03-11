using Microsoft.EntityFrameworkCore;
using MedicalSystem.Core.Models;
using MedicalSystem.Core.Interfaces;
using MedicalSystem.Infrastructure.Data;

namespace MedicalSystem.Infrastructure.Repositories
{
	public class PatientRepository : Repository<Patient>, IPatientRepository
	{
		public PatientRepository(MedicalSystemDbContext context) : base(context)
		{
		}

		public async Task<IEnumerable<Patient>> SearchByLastNameAsync(string lastName)
		{
			return await _dbSet
				.Where(p => p.LastName.ToLower().Contains(lastName.ToLower()))
				.ToListAsync();
		}

		public async Task<Patient> GetByPersonalIdNumberAsync(string personalIdNumber)
		{
			return await _dbSet
				.FirstOrDefaultAsync(p => p.PersonalIdNumber == personalIdNumber);
		}

		public async Task<Patient> GetPatientWithFullDetailsAsync(int id)
		{
			return await _dbSet
				.Include(p => p.MedicalHistories)
				.Include(p => p.Examinations)
					.ThenInclude(e => e.Doctor)
				.Include(p => p.Examinations)
					.ThenInclude(e => e.MedicalImages)
				.Include(p => p.Prescriptions)
					.ThenInclude(p => p.Doctor)
				.FirstOrDefaultAsync(p => p.Id == id);
		}

		public override async Task<IEnumerable<Patient>> GetAllAsync()
		{
			return await _dbSet
				.Include(p => p.MedicalHistories)
				.ToListAsync();
		}

		public override async Task<Patient> GetByIdAsync(int id)
		{
			return await _dbSet
				.Include(p => p.MedicalHistories)
				.FirstOrDefaultAsync(p => p.Id == id);
		}
	}
}