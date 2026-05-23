using System.ComponentModel.DataAnnotations;

namespace DevFastTrack.API.DTOs.CorporateRequest;

public class CreateCorporateRequestDto
{
    [Required]
    public string CompanyName { get; set; } = string.Empty;
    
    [Required]
    [EmailAddress]
    public string CompanyEmail { get; set; } = string.Empty;
    
    [Required]
    public string TrainingType { get; set; } = string.Empty;
    
    [Required]
    public string Technology { get; set; } = string.Empty;
    
    [Required]
    [Range(1, 10000)]
    public int EmployeeCount { get; set; }
    
    [Required]
    public DateTime StartDate { get; set; }
    
    [Required]
    [Range(1, 52)]
    public int Duration { get; set; }
    
    [Required]
    public string Mode { get; set; } = string.Empty;
    
    public string Requirements { get; set; } = string.Empty;
}
