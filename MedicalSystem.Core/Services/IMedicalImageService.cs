using Microsoft.AspNetCore.Http;
using MedicalSystem.Core.Models;
using MedicalSystem.Core.Interfaces;
using System.Net.Mime;
using Microsoft.Extensions.Configuration;

namespace MedicalSystem.Core.Services
{
	public interface IMedicalImageService
	{
		Task<IEnumerable<MedicalImage>> GetAllMedicalImagesAsync();
		Task<MedicalImage> GetMedicalImageByIdAsync(int id);
		Task<IEnumerable<MedicalImage>> GetImagesByExaminationAsync(int examinationId);
		Task<MedicalImage> UploadMedicalImageAsync(int examinationId, IFormFile file);
		Task DeleteMedicalImageAsync(int id);
		Task<(byte[] FileContents, string ContentType, string FileName)> GetMedicalImageFileAsync(int id);
	}

	public class MedicalImageService : IMedicalImageService
	{
		private readonly IMedicalImageRepository _medicalImageRepository;
		private readonly IExaminationRepository _examinationRepository;
		private readonly string _uploadDirectory;

		public MedicalImageService(
			IMedicalImageRepository medicalImageRepository,
			IExaminationRepository examinationRepository,
			IConfiguration configuration)
		{
			_medicalImageRepository = medicalImageRepository;
			_examinationRepository = examinationRepository;
			
			// Determine base path - use Azure's home directory if available
			string basePath;
			if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("HOME")))
			{
				// Azure environment
				basePath = Path.Combine(Environment.GetEnvironmentVariable("HOME"), "site", "wwwroot");
			}
			else
			{
				// Local environment
				basePath = Directory.GetCurrentDirectory();
			}
			
			_uploadDirectory = Path.Combine(
				basePath, 
				configuration["UploadSettings:MedicalImagesPath"] ?? "Uploads/MedicalImages"
			);
			
			// Ensure upload directory exists
			if (!Directory.Exists(_uploadDirectory))
			{
				Directory.CreateDirectory(_uploadDirectory);
			}
		}

		public async Task<IEnumerable<MedicalImage>> GetAllMedicalImagesAsync()
		{
			return await _medicalImageRepository.GetAllAsync();
		}

		public async Task<MedicalImage> GetMedicalImageByIdAsync(int id)
		{
			var image = await _medicalImageRepository.GetByIdAsync(id);
			if (image == null)
				throw new KeyNotFoundException($"Medical image with ID {id} not found.");
			return image;
		}

		public async Task<IEnumerable<MedicalImage>> GetImagesByExaminationAsync(int examinationId)
		{
			var examination = await _examinationRepository.GetByIdAsync(examinationId);
			if (examination == null)
				throw new KeyNotFoundException($"Examination with ID {examinationId} not found.");

			return await _medicalImageRepository.GetImagesByExaminationAsync(examinationId);
		}

		public async Task<MedicalImage> UploadMedicalImageAsync(int examinationId, IFormFile file)
		{
			if (file == null || file.Length == 0)
				throw new ArgumentException("No file was uploaded.");

			var examination = await _examinationRepository.GetByIdAsync(examinationId);
			if (examination == null)
				throw new KeyNotFoundException($"Examination with ID {examinationId} not found.");

			// Validate file type
			var allowedTypes = new[] { "image/jpeg", "image/png", "image/dicom" };
			if (!allowedTypes.Contains(file.ContentType.ToLower()))
				throw new ArgumentException("Invalid file type. Only JPEG, PNG, and DICOM images are allowed.");

			// Generate unique filename
			var fileExtension = Path.GetExtension(file.FileName);
			var uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";
			var filePath = Path.Combine(_uploadDirectory, uniqueFileName);

			// Save file
			using (var stream = new FileStream(filePath, FileMode.Create))
			{
				await file.CopyToAsync(stream);
			}

			// Create medical image record
			var medicalImage = new MedicalImage
			{
				ExaminationId = examinationId,
				FileName = uniqueFileName,
				FileType = file.ContentType,
				UploadDateTime = DateTime.UtcNow
			};

			return await _medicalImageRepository.AddAsync(medicalImage);
		}

		public async Task DeleteMedicalImageAsync(int id)
		{
			var image = await _medicalImageRepository.GetByIdAsync(id);
			if (image == null)
				throw new KeyNotFoundException($"Medical image with ID {id} not found.");

			// Delete file
			var filePath = Path.Combine(_uploadDirectory, image.FileName);
			if (File.Exists(filePath))
			{
				File.Delete(filePath);
			}

			// Delete database record
			await _medicalImageRepository.DeleteAsync(image);
		}

		public async Task<(byte[] FileContents, string ContentType, string FileName)> GetMedicalImageFileAsync(int id)
		{
			var image = await _medicalImageRepository.GetByIdAsync(id);
			if (image == null)
				throw new KeyNotFoundException($"Medical image with ID {id} not found.");

			var filePath = Path.Combine(_uploadDirectory, image.FileName);
			if (!File.Exists(filePath))
				throw new FileNotFoundException($"File not found for medical image with ID {id}");

			var fileContents = await File.ReadAllBytesAsync(filePath);
			return (fileContents, image.FileType, image.FileName);
		}
	}
}