namespace DevFastTrack.API.DTOs.Batch;

public class BatchResponse
{
    public int Id { get; set; }
    public int CourseId { get; set; }
    public string CourseName { get; set; } = string.Empty;
    public string BatchName { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Timing { get; set; } = string.Empty;
    public string MeetingLink { get; set; } = string.Empty;
    public int SeatsTotal { get; set; }
    public int SeatsLeft { get; set; }
    public bool IsActive { get; set; }
}