using MedicalSystem.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicalSystem.Core.Interfaces
{
	public interface IExaminationRepository : IRepository<Examination>
	{
		Task<IEnumerable<Examination>> GetExaminationsByPatientAsync(int patientId);
		Task<IEnumerable<Examination>> GetExaminationsByDoctorAsync(int doctorId);
		Task<Examination> GetExaminationWithDetailsAsync(int id);
	}
}
