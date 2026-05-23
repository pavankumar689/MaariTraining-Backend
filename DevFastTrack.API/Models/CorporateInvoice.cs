namespace DevFastTrack.API.Models;

public class CorporateInvoice
{
    public int Id { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public string CompanyEmail { get; set; } = string.Empty;
    public string CompanyName { get; set; } = string.Empty;
    public int TrainingRequestId { get; set; }
    public string ProgramName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime IssueDate { get; set; }
    public DateTime DueDate { get; set; }
    public string Status { get; set; } = "Pending"; // Pending, Paid, Overdue
    public int EmployeeCount { get; set; }
    public string? PaymentId { get; set; }
    public DateTime? PaidDate { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation property
    public CorporateTrainingRequest? TrainingRequest { get; set; }
}
