using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedicalSystem.Core.Models
{
	[Table("Prescription")]
	public class Prescription
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		[Column("Id")]
		public int Id { get; set; }

		[Required]
		[Column("ExaminationId")]
		[ForeignKey("Examination")]
		public int ExaminationId { get; set; }

		[Required]
		[Column("PatientId")]
		[ForeignKey("Patient")]
		public int PatientId { get; set; }

		[Required]
		[Column("DoctorId")]
		[ForeignKey("Doctor")]
		public int DoctorId { get; set; }

		[Required]
		[Column("Medication")]
		[StringLength(200)]
		public string Medication { get; set; }

		[Required]
		[Column("Dosage")]
		[StringLength(100)]
		public string Dosage { get; set; }

		[Column("Instructions")]
		public string Instructions { get; set; }

		[Required]
		[Column("PrescriptionDate")]
		public DateTime PrescriptionDate { get; set; }

		// Navigation properties
		public virtual Examination Examination { get; set; }
		public virtual Patient Patient { get; set; }
		public virtual Doctor Doctor { get; set; }
	}
}