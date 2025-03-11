using MedicalSystem.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicalSystem.Core.Interfaces
{
	public interface IMedicalImageRepository : IRepository<MedicalImage>
	{
		Task<IEnumerable<MedicalImage>> GetImagesByExaminationAsync(int examinationId);
	}
}
