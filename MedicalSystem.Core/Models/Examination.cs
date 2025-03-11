using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedicalSystem.Core.Models
{
	[Table("Examination")]
	public class Examination
	{
		public enum ExaminationType
		{
			GP,         // General physical examination
			KRV,        // Blood test
			XRAY,       // X-Ray scan
			CT,         // CT scan
			MR,         // MRI scan
			ULTRA,      // Ultrasound
			EKG,        // Electrocardiogram
			ECHO,       // Echocardiogram
			EYE,        // Eye examination
			DERM,       // Dermatological examination
			DENTA,      // Dental examination
			MAMMO,      // Mammography
			NEURO       // Neurological examination
		}

		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		[Column("Id")]
		public int Id { get; set; }

		[Required]
		[Column("PatientId")]
		[ForeignKey("Patient")]
		public int PatientId { get; set; }

		[Required]
		[Column("DoctorId")]
		[ForeignKey("Doctor")]
		public int DoctorId { get; set; }

		[Required]
		[Column("ExaminationType")]
		public ExaminationType Type { get; set; }

		[Required]
		[Column("ExaminationDateTime")]
		public DateTime ExaminationDateTime { get; set; }

		[Column("Notes")]
		public string Notes { get; set; }

		// Navigation properties
		public virtual Patient Patient { get; set; }
		public virtual Doctor Doctor { get; set; }

		[InverseProperty("Examination")]
		public virtual ICollection<MedicalImage> MedicalImages { get; set; } = new List<MedicalImage>();

		[InverseProperty("Examination")]
		public virtual ICollection<Prescription> Prescriptions { get; set; } = new List<Prescription>();
	}
}