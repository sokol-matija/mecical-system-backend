using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedicalSystem.Core.Models
{
	[Table("MedicalHistory")]
	public class MedicalHistory
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		[Column("Id")]
		public int Id { get; set; }

		[Required]
		[Column("PatientId")]
		[ForeignKey("Patient")]
		public int PatientId { get; set; }

		[Required]
		[Column("DiseaseName")]
		[StringLength(200)]
		public string DiseaseName { get; set; }

		[Required]
		[Column("StartDate")]
		[DataType(DataType.DateTime)]
		public DateTime StartDate { get; set; }

		[Column("EndDate")]
		[DataType(DataType.DateTime)]
		public DateTime? EndDate { get; set; }

		// Navigation property
		public virtual Patient Patient { get; set; }
	}
}