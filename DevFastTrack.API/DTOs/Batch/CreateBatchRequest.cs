using System.ComponentModel.DataAnnotations;

namespace DevFastTrack.API.DTOs.Batch;

public class CreateBatchRequest
{
    [Required] public int CourseId { get; set; }
    [Required] public string BatchName { get; set; } = string.Empty;
    [Required] public DateTime StartDate { get; set; }
    [Required] public DateTime EndDate { get; set; }
    public string Timing { get; set; } = string.Empty;
    public string MeetingLink { get; set; } = string.Empty;
    public int SeatsTotal { get; set; } = 15;
}