using MedicalSystem.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicalSystem.Core.Interfaces
{
	public interface IMedicalHistoryRepository : IRepository<MedicalHistory>
	{
		Task<IEnumerable<MedicalHistory>> GetPatientHistoryAsync(int patientId);
		Task<IEnumerable<MedicalHistory>> GetActiveConditionsAsync(int patientId);
	}
}
