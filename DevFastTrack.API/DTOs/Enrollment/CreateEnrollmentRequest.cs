using System.ComponentModel.DataAnnotations;

namespace DevFastTrack.API.DTOs.Enrollment;

public class CreateEnrollmentRequest
{
    [Required] public int CourseId { get; set; }
    [Required] public int BatchId { get; set; }
    public string PaymentId { get; set; } = string.Empty;
    public decimal AmountPaid { get; set; }
}