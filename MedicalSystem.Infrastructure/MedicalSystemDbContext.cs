using Microsoft.EntityFrameworkCore;
using MedicalSystem.Core.Models;

namespace MedicalSystem.Infrastructure.Data
{
	public class MedicalSystemDbContext : DbContext
	{
		public MedicalSystemDbContext(DbContextOptions<MedicalSystemDbContext> options)
			: base(options)
		{
		}

		public DbSet<Patient> Patients { get; set; }
		public DbSet<Doctor> Doctors { get; set; }
		public DbSet<Examination> Examinations { get; set; }
		public DbSet<MedicalHistory> MedicalHistories { get; set; }
		public DbSet<MedicalImage> MedicalImages { get; set; }
		public DbSet<Prescription> Prescriptions { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			// Configure Patient
			modelBuilder.Entity<Patient>(entity =>
			{
				entity.HasIndex(e => e.PersonalIdNumber).IsUnique();
				// Gender is now a string with max length 1, so we don't need this configuration
				// entity.Property(e => e.Gender).HasMaxLength(1);
			});

			// Configure relationships for Doctor
			modelBuilder.Entity<Doctor>()
				.HasMany(d => d.Examinations)
				.WithOne(e => e.Doctor)
				.HasForeignKey(e => e.DoctorId)
				.OnDelete(DeleteBehavior.Restrict);

			modelBuilder.Entity<Doctor>()
				.HasMany(d => d.Prescriptions)
				.WithOne(p => p.Doctor)
				.HasForeignKey(p => p.DoctorId)
				.OnDelete(DeleteBehavior.Restrict);

			// Configure relationships for Patient
			modelBuilder.Entity<Patient>()
				.HasMany(p => p.MedicalHistories)
				.WithOne(mh => mh.Patient)
				.HasForeignKey(mh => mh.PatientId)
				.OnDelete(DeleteBehavior.Cascade);

			modelBuilder.Entity<Patient>()
				.HasMany(p => p.Examinations)
				.WithOne(e => e.Patient)
				.HasForeignKey(e => e.PatientId)
				.OnDelete(DeleteBehavior.Cascade);

			modelBuilder.Entity<Patient>()
				.HasMany(p => p.Prescriptions)
				.WithOne(p => p.Patient)
				.HasForeignKey(p => p.PatientId)
				.OnDelete(DeleteBehavior.Cascade);

			// Configure relationships for Examination
			modelBuilder.Entity<Examination>()
				.HasMany(e => e.MedicalImages)
				.WithOne(mi => mi.Examination)
				.HasForeignKey(mi => mi.ExaminationId)
				.OnDelete(DeleteBehavior.Cascade);

			modelBuilder.Entity<Examination>()
				.HasMany(e => e.Prescriptions)
				.WithOne(p => p.Examination)
				.HasForeignKey(p => p.ExaminationId)
				.OnDelete(DeleteBehavior.Cascade);
		}
	}
}