namespace MedicalSystem.API.Controllers
{
	public class ExaminationCreateDto
	{
		public int PatientId { get; set; }
		public int DoctorId { get; set; }
		public int Type { get; set; }
		public DateTime ExaminationDateTime { get; set; }
		public string Notes { get; set; }
	}
}