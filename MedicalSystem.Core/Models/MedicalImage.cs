using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedicalSystem.Core.Models
{
	[Table("MedicalImage")]
	public class MedicalImage
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
		[Column("FileName")]
		[StringLength(255)]
		public string FileName { get; set; }

		[Required]
		[Column("FileType")]
		[StringLength(50)]
		public string FileType { get; set; }

		[Required]
		[Column("UploadDateTime")]
		public DateTime UploadDateTime { get; set; }

		// Navigation property
		public virtual Examination Examination { get; set; }
	}
}