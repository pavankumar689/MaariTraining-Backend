using System.ComponentModel.DataAnnotations;

namespace DevFastTrack.API.DTOs.Announcement;

public class CreateAnnouncementRequest
{
    [Required] public string Title { get; set; } = string.Empty;
    [Required] public string Description { get; set; } = string.Empty;
    public int? CourseId { get; set; }
}