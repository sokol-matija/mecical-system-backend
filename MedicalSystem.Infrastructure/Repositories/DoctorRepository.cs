using Microsoft.EntityFrameworkCore;
using MedicalSystem.Core.Models;
using MedicalSystem.Core.Interfaces;
using MedicalSystem.Infrastructure.Data;

namespace MedicalSystem.Infrastructure.Repositories
{
	public class DoctorRepository : Repository<Doctor>, IDoctorRepository
	{
		public DoctorRepository(MedicalSystemDbContext context) : base(context)
		{
		}

		public async Task<IEnumerable<Doctor>> GetDoctorsWithExaminationsAsync()
		{
			return await _dbSet
				.Include(d => d.Examinations)
					.ThenInclude(e => e.Patient)
				.Include(d => d.Prescriptions)
					.ThenInclude(p => p.Patient)
				.ToListAsync();
		}

		public async Task<Doctor> GetDoctorWithDetailsAsync(int id)
		{
			return await _dbSet
				.Include(d => d.Examinations)
					.ThenInclude(e => e.Patient)
				.Include(d => d.Prescriptions)
					.ThenInclude(p => p.Patient)
				.FirstOrDefaultAsync(d => d.Id == id);
		}

		public override async Task<IEnumerable<Doctor>> GetAllAsync()
		{
			return await _dbSet
				.Include(d => d.Examinations)
				.ToListAsync();
		}

		public override async Task<Doctor> GetByIdAsync(int id)
		{
			return await _dbSet
				.Include(d => d.Examinations)
				.FirstOrDefaultAsync(d => d.Id == id);
		}
	}
}