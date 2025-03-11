using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedicalSystem.Core.Models
{
	[Table("Patient")]
	public class Patient
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
		[Column("PersonalIdNumber")]
		[StringLength(11, MinimumLength = 11)]
		[RegularExpression("^[0-9]*$")]
		//[Index(IsUnique = true)]
		public string PersonalIdNumber { get; set; }

		[Required]
		[Column("DateOfBirth")]
		[DataType(DataType.Date)]
		public DateTime DateOfBirth { get; set; }

		[Required]
		[Column("Gender")]
		[StringLength(1)]
		[RegularExpression("^[MFmf]$")]
		public string Gender { get; set; }

		[NotMapped]
		public int Age
		{
			get
			{
				var today = DateTime.Today;
				var age = today.Year - DateOfBirth.Year;
				if (DateOfBirth.Date > today.AddYears(-age))
					age--;
				return age;
			}
		}

		// Navigation properties
		[InverseProperty("Patient")]
		public virtual ICollection<MedicalHistory> MedicalHistories { get; set; } = new List<MedicalHistory>();

		[InverseProperty("Patient")]
		public virtual ICollection<Examination> Examinations { get; set; } = new List<Examination>();

		[InverseProperty("Patient")]
		public virtual ICollection<Prescription> Prescriptions { get; set; } = new List<Prescription>();
	}
}