using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicalSystem.Core.Interfaces
{
	public interface IRepositoryFactory
	{
		IPatientRepository CreatePatientRepository();
		IDoctorRepository CreateDoctorRepository();
		IExaminationRepository CreateExaminationRepository();
		IMedicalHistoryRepository CreateMedicalHistoryRepository();
		IMedicalImageRepository CreateMedicalImageRepository();
		IPrescriptionRepository CreatePrescriptionRepository();
	}
}
