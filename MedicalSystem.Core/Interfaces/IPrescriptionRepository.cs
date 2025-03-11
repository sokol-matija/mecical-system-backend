using MedicalSystem.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicalSystem.Core.Interfaces
{
	public interface IPrescriptionRepository : IRepository<Prescription>
	{
		Task<IEnumerable<Prescription>> GetPrescriptionsByPatientAsync(int patientId);
		Task<IEnumerable<Prescription>> GetPrescriptionsByDoctorAsync(int doctorId);
	}
}
