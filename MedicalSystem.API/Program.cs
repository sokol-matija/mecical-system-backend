using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using MedicalSystem.Infrastructure.Data;
using MedicalSystem.Core.Interfaces;
using MedicalSystem.Infrastructure.Repositories;
using MedicalSystem.Core.Services;
using Microsoft.Extensions.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

// Configure services
// Database Context
var connectionString = Environment.GetEnvironmentVariable("POSTGRESQLCONNSTR_DefaultConnection") 
    ?? builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<MedicalSystemDbContext>(options =>
	options.UseNpgsql(connectionString));

// Register repositories
builder.Services.AddScoped<IPatientRepository, PatientRepository>();
builder.Services.AddScoped<IDoctorRepository, DoctorRepository>();
builder.Services.AddScoped<IExaminationRepository, ExaminationRepository>();
builder.Services.AddScoped<IMedicalHistoryRepository, MedicalHistoryRepository>();
builder.Services.AddScoped<IMedicalImageRepository, MedicalImageRepository>();
builder.Services.AddScoped<IPrescriptionRepository, PrescriptionRepository>();

// Register services
builder.Services.AddScoped<IPatientService, PatientService>();
builder.Services.AddScoped<IDoctorService, DoctorService>();
builder.Services.AddScoped<IExaminationService, ExaminationService>();
builder.Services.AddScoped<IMedicalHistoryService, MedicalHistoryService>();
builder.Services.AddScoped<IMedicalImageService, MedicalImageService>();
builder.Services.AddScoped<IPrescriptionService, PrescriptionService>();

// Configure CORS
builder.Services.AddCors(options =>
{
	options.AddPolicy("AllowAll", builder =>
	{
		builder.AllowAnyOrigin()
			   .AllowAnyMethod()
			   .AllowAnyHeader();
	});
});

// Add API Controllers
builder.Services.AddControllers()
	.AddJsonOptions(options =>
	{
		// Configure JSON serialization options if needed
		options.JsonSerializerOptions.ReferenceHandler =
			System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
	});

// Configure Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
	c.SwaggerDoc("v1", new OpenApiInfo
	{
		Title = "Medical System API",
		Version = "v1",
		Description = "API for managing medical records, patients, and examinations"
	});
});

// Add health checks
builder.Services.AddHealthChecks();

// Configure Logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

var app = builder.Build();

// Configure middleware pipeline
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI(c =>
	{
		c.SwaggerEndpoint("/swagger/v1/swagger.json", "Medical System API V1");
		c.RoutePrefix = "swagger";
	});
	app.UseDeveloperExceptionPage();
}
else
{
	// Configure error handling for production
	app.UseExceptionHandler("/Error");
	app.UseHsts();
}

// Use CORS
app.UseCors("AllowAll");

// Use HTTPS redirection
//app.UseHttpsRedirection();

// Use static files (if needed for file uploads)
app.UseStaticFiles();

// Use routing
app.UseRouting();

// Use authentication and authorization (if implemented)
app.UseAuthentication();
app.UseAuthorization();

// Map controllers
app.MapControllers();

// Add health checks endpoint
app.MapHealthChecks("/health");

// Optional: Add health checks endpoint
app.MapGet("/health", () => Results.Ok(new { Status = "Healthy" }));

try
{
	// Create a separate scope just for migrations
	using (var scope = app.Services.CreateScope())
	{
		var dbContext = scope.ServiceProvider
			.GetRequiredService<MedicalSystemDbContext>();
		
		// Apply any pending migrations
		dbContext.Database.Migrate();
		
		// Log successful migration
		var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
		logger.LogInformation("Database migrations applied successfully");
	}

	// After migrations are complete, run the app
	app.Run();
}
catch (Exception ex)
{
    // Log any startup errors
    var logger = app.Services.GetRequiredService<ILogger<Program>>();
    logger.LogError(ex, "An error occurred during application startup");
    Console.WriteLine(ex.Message);
	throw;
}