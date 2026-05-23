namespace DevFastTrack.API.Models;

public class Course
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
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Batch> Batches { get; set; } = new List<Batch>();
    public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
    public ICollection<Assessment> Assessments { get; set; } = new List<Assessment>();
    public ICollection<Material> Materials { get; set; } = new List<Material>();
}