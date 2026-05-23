namespace DevFastTrack.API.Models;

public class Enrollment
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int CourseId { get; set; }
    public int BatchId { get; set; }
    public string Status { get; set; } = "Active";
    public string PaymentId { get; set; } = string.Empty;
    public decimal AmountPaid { get; set; }
    public DateTime EnrollmentDate { get; set; } = DateTime.UtcNow;

    public User User { get; set; } = null!;
    public Course Course { get; set; } = null!;
    public Batch Batch { get; set; } = null!;
}