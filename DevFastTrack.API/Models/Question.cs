namespace DevFastTrack.API.Models;

public class Question
{
    public int Id { get; set; }
    public int AssessmentId { get; set; }
    public Assessment Assessment { get; set; } = null!;
    
    public string Text { get; set; } = string.Empty;
    public bool IsMultiSelect { get; set; }
    public decimal Marks { get; set; }

    public ICollection<QuestionOption> Options { get; set; } = new List<QuestionOption>();
}
