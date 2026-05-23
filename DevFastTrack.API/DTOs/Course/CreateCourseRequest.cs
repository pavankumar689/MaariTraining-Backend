using System.ComponentModel.DataAnnotations;

namespace DevFastTrack.API.DTOs.Course;

public class CreateCourseRequest
{
    [Required] public string Title { get; set; } = string.Empty;
    [Required] public string Description { get; set; } = string.Empty;
    [Required] public decimal Price { get; set; }
    public decimal OriginalPrice { get; set; }
    [Required] public string Duration { get; set; } = string.Empty;
    [Required] public string Level { get; set; } = string.Empty;
    public string ThumbnailUrl { get; set; } = string.Empty;
    public string Syllabus { get; set; } = string.Empty;
    public string Prerequisites { get; set; } = string.Empty;
    public string Outcomes { get; set; } = string.Empty;
    public string MentorName { get; set; } = string.Empty;
    public string MentorBio { get; set; } = string.Empty;
    public string SoftwareRequirements { get; set; } = string.Empty;
}