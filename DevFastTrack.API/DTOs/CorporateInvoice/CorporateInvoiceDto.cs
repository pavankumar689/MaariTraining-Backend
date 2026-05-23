namespace DevFastTrack.API.DTOs.CorporateInvoice;

public class CorporateInvoiceDto
{
    public int Id { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public string CompanyName { get; set; } = string.Empty;
    public string ProgramName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime IssueDate { get; set; }
    public DateTime DueDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public int EmployeeCount { get; set; }
    public DateTime? PaidDate { get; set; }
}

public class CreateCorporateInvoiceDto
{
    public string CompanyEmail { get; set; } = string.Empty;
    public string CompanyName { get; set; } = string.Empty;
    public int TrainingRequestId { get; set; }
    public string ProgramName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public int EmployeeCount { get; set; }
    public DateTime DueDate { get; set; }
}
