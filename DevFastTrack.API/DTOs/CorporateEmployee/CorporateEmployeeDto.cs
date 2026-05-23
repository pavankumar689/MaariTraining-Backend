namespace DevFastTrack.API.DTOs.CorporateEmployee;

public class CorporateEmployeeDto
{
    public int Id { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public string EmployeeEmail { get; set; } = string.Empty;
    public string CourseName { get; set; } = string.Empty;
    public string? BatchName { get; set; }
    public int Progress { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime EnrollmentDate { get; set; }
    public DateTime? LastActiveDate { get; set; }
    public int CompletedModules { get; set; }
    public int TotalModules { get; set; }
}

public class CreateCorporateEmployeeDto
{
    public string CompanyEmail { get; set; } = string.Empty;
    public string EmployeeName { get; set; } = string.Empty;
    public string EmployeeEmail { get; set; } = string.Empty;
    public int CourseId { get; set; }
    public int? BatchId { get; set; }
    public int TotalModules { get; set; }
}
