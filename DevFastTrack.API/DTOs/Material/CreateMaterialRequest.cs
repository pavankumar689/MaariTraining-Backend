using System.ComponentModel.DataAnnotations;

namespace DevFastTrack.API.DTOs.Material;

public class CreateMaterialRequest
{
    [Required] public int CourseId { get; set; }
    [Required] public string Title { get; set; } = string.Empty;
    [Required] public string Type { get; set; } = string.Empty;
    [Required] public string FileUrl { get; set; } = string.Empty;
}