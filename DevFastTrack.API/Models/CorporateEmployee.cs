namespace DevFastTrack.API.Models;

public class CorporateEmployee
{
    public int Id { get; set; }
    public string CompanyEmail { get; set; } = string.Empty;
    public string EmployeeName { get; set; } = string.Empty;
    public string EmployeeEmail { get; set; } = string.Empty;
    public int CourseId { get; set; }
    public int? BatchId { get; set; }
    public int Progress { get; set; } = 0;
    public string Status { get; set; } = "Not Started"; // Not Started, Active, Completed
    public DateTime EnrollmentDate { get; set; }
    public DateTime? LastActiveDate { get; set; }
    public int CompletedModules { get; set; } = 0;
    public int TotalModules { get; set; } = 0;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public Course? Course { get; set; }
    public Batch? Batch { get; set; }
}
