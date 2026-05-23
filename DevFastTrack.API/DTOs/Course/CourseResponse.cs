namespace DevFastTrack.API.DTOs.Course;

public class CourseResponse
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public decimal OriginalPrice { get; set; }
    public string Duration { get; set; } = string.Empty;
    public string Level { get; set; } = string.Empty;
    public string ThumbnailUrl { get; set; } = string.Empty;
    public string Syllabus { get; set; } = string.Empty;
    public string Prerequisites { get; set; } = string.Empty;
    public string Outcomes { get; set; } = string.Empty;
    public string MentorName { get; set; } = string.Empty;
    public string MentorBio { get; set; } = string.Empty;
    public string SoftwareRequirements { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public int TotalBatches { get; set; }
    public int SeatsLeft { get; set; }
    public DateTime CreatedAt { get; set; }
}