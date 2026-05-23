namespace DevFastTrack.API.Models;

public class Batch
{
    public int Id { get; set; }
    public int CourseId { get; set; }
    public string BatchName { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Timing { get; set; } = string.Empty;
    public string MeetingLink { get; set; } = string.Empty;
    public int SeatsTotal { get; set; } = 15;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Course Course { get; set; } = null!;
    public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
}