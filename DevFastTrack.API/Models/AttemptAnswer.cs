namespace DevFastTrack.API.Models;

public class AttemptAnswer
{
    public int Id { get; set; }
    public int AssessmentAttemptId { get; set; }
    public AssessmentAttempt AssessmentAttempt { get; set; } = null!;
    
    public int QuestionId { get; set; }
    public Question Question { get; set; } = null!;
    
    // Comma-separated string of selected QuestionOption IDs (e.g., "1,4")
    public string SelectedOptionIds { get; set; } = string.Empty;
    
    public decimal MarksObtained { get; set; }
}
