namespace DevFastTrack.API.Models;

public class TrainerProfile
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Skills { get; set; } = string.Empty; // Comma-separated
    public int YearsOfExperience { get; set; }
    public string Education { get; set; } = string.Empty;
    public string Certifications { get; set; } = string.Empty;
    public decimal MinSalary { get; set; }
    public decimal MaxSalary { get; set; }
    public string SalaryType { get; set; } = "Monthly"; // Monthly, Hourly, Per Session
    public string Availability { get; set; } = "Full-time"; // Full-time, Part-time, Freelance
    public string WorkingHoursPreference { get; set; } = string.Empty; // e.g., "9 AM - 5 PM"
    public string LocationPreference { get; set; } = "Remote"; // Remote, On-site, Hybrid
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string ResumeUrl { get; set; } = string.Empty;
    public string PortfolioUrl { get; set; } = string.Empty;
    public string LinkedInUrl { get; set; } = string.Empty;
    public string GitHubUrl { get; set; } = string.Empty;
    public string Bio { get; set; } = string.Empty;
    public string Status { get; set; } = "Pending"; // Pending, Under Review, Hired, Rejected
    public string? RejectionReason { get; set; }
    public DateTime? HiredDate { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}
