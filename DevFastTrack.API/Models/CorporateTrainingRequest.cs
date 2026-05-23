namespace DevFastTrack.API.Models;

public class CorporateTrainingRequest
{
    public int Id { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public string CompanyEmail { get; set; } = string.Empty;
    public string TrainingType { get; set; } = string.Empty;
    public string Technology { get; set; } = string.Empty;
    public int EmployeeCount { get; set; }
    public DateTime StartDate { get; set; }
    public int Duration { get; set; }
    public string Mode { get; set; } = string.Empty;
    public string Requirements { get; set; } = string.Empty;
    public string Status { get; set; } = "Pending"; // Pending, Approved, Rejected
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
