namespace DevFastTrack.API.Models;

public class AssessmentAttempt
{
    public int Id { get; set; }
    public int AssessmentId { get; set; }
    public Assessment Assessment { get; set; } = null!;
    
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    
    public DateTime StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    
    public decimal Score { get; set; }
    public bool IsCompleted { get; set; }

    public ICollection<AttemptAnswer> Answers { get; set; } = new List<AttemptAnswer>();
}
