using Microsoft.EntityFrameworkCore;
using MedicalSystem.Core.Models;
using MedicalSystem.Core.Interfaces;
using MedicalSystem.Infrastructure.Data;

namespace MedicalSystem.Infrastructure.Repositories
{
	public class MedicalImageRepository : Repository<MedicalImage>, IMedicalImageRepository
	{
		public MedicalImageRepository(MedicalSystemDbContext context) : base(context)
		{
		}

		public async Task<IEnumerable<MedicalImage>> GetImagesByExaminationAsync(int examinationId)
		{
			return await _dbSet
				.Where(mi => mi.ExaminationId == examinationId)
				.Include(mi => mi.Examination)
				.OrderByDescending(mi => mi.UploadDateTime)
				.ToListAsync();
		}

		public override async Task<IEnumerable<MedicalImage>> GetAllAsync()
		{
			return await _dbSet
				.Include(mi => mi.Examination)
				.OrderByDescending(mi => mi.UploadDateTime)
				.ToListAsync();
		}

		public override async Task<MedicalImage> GetByIdAsync(int id)
		{
			return await _dbSet
				.Include(mi => mi.Examination)
				.FirstOrDefaultAsync(mi => mi.Id == id);
		}
	}
}