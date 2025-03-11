using MedicalSystem.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicalSystem.Core.Interfaces
{
	public interface IPatientRepository : IRepository<Patient>
	{
		Task<IEnumerable<Patient>> SearchByLastNameAsync(string lastName);
		Task<Patient> GetByPersonalIdNumberAsync(string personalIdNumber);
		Task<Patient> GetPatientWithFullDetailsAsync(int id);
	}
}
