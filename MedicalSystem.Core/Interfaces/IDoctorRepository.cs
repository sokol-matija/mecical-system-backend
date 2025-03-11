using MedicalSystem.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicalSystem.Core.Interfaces
{
	public interface IDoctorRepository : IRepository<Doctor>
	{
		Task<IEnumerable<Doctor>> GetDoctorsWithExaminationsAsync();
		Task<Doctor> GetDoctorWithDetailsAsync(int id);
	}
}
