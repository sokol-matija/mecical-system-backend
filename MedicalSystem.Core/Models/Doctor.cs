using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedicalSystem.Core.Models
{
	[Table("Doctor")]
	public class Doctor
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		[Column("Id")]
		public int Id { get; set; }

		[Required]
		[Column("FirstName")]
		[StringLength(100)]
		public string FirstName { get; set; }

		[Required]
		[Column("LastName")]
		[StringLength(100)]
		public string LastName { get; set; }

		[Required]
		[Column("Specialization")]
		[StringLength(100)]
		public string Specialization { get; set; }

		// Navigation properties
		[InverseProperty("Doctor")]
		public virtual ICollection<Examination> Examinations { get; set; } = new List<Examination>();

		[InverseProperty("Doctor")]
		public virtual ICollection<Prescription> Prescriptions { get; set; } = new List<Prescription>();
	}
}