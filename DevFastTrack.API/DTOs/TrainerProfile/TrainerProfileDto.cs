namespace DevFastTrack.API.DTOs.TrainerProfile;

public class TrainerProfileDto
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Skills { get; set; } = string.Empty;
    public int YearsOfExperience { get; set; }
    public string Education { get; set; } = string.Empty;
    public string Certifications { get; set; } = string.Empty;
    public decimal MinSalary { get; set; }
    public decimal MaxSalary { get; set; }
    public string SalaryType { get; set; } = string.Empty;
    public string Availability { get; set; } = string.Empty;
    public string WorkingHoursPreference { get; set; } = string.Empty;
    public string LocationPreference { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string ResumeUrl { get; set; } = string.Empty;
    public string PortfolioUrl { get; set; } = string.Empty;
    public string LinkedInUrl { get; set; } = string.Empty;
    public string GitHubUrl { get; set; } = string.Empty;
    public string Bio { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? RejectionReason { get; set; }
    public DateTime? HiredDate { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateTrainerProfileDto
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Skills { get; set; } = string.Empty;
    public int YearsOfExperience { get; set; }
    public string Education { get; set; } = string.Empty;
    public string Certifications { get; set; } = string.Empty;
    public decimal MinSalary { get; set; }
    public decimal MaxSalary { get; set; }
    public string SalaryType { get; set; } = "Monthly";
    public string Availability { get; set; } = "Full-time";
    public string WorkingHoursPreference { get; set; } = string.Empty;
    public string LocationPreference { get; set; } = "Remote";
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string ResumeUrl { get; set; } = string.Empty;
    public string PortfolioUrl { get; set; } = string.Empty;
    public string LinkedInUrl { get; set; } = string.Empty;
    public string GitHubUrl { get; set; } = string.Empty;
    public string Bio { get; set; } = string.Empty;
}
