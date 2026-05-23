using DevFastTrack.API.Data;
using DevFastTrack.API.DTOs.CorporateInvoice;
using DevFastTrack.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DevFastTrack.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CorporateInvoiceController : ControllerBase
{
    private readonly AppDbContext _context;

    public CorporateInvoiceController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CorporateInvoiceDto>>> GetAll()
    {
        var invoices = await _context.CorporateInvoices
            .Select(i => new CorporateInvoiceDto
            {
                Id = i.Id,
                InvoiceNumber = i.InvoiceNumber,
                CompanyName = i.CompanyName,
                ProgramName = i.ProgramName,
                Amount = i.Amount,
                IssueDate = i.IssueDate,
                DueDate = i.DueDate,
                Status = i.Status,
                EmployeeCount = i.EmployeeCount,
                PaidDate = i.PaidDate
            })
            .ToListAsync();

        return Ok(invoices);
    }

    [HttpGet("company/{companyEmail}")]
    public async Task<ActionResult<IEnumerable<CorporateInvoiceDto>>> GetByCompany(string companyEmail)
    {
        var invoices = await _context.CorporateInvoices
            .Where(i => i.CompanyEmail == companyEmail)
            .Select(i => new CorporateInvoiceDto
            {
                Id = i.Id,
                InvoiceNumber = i.InvoiceNumber,
                CompanyName = i.CompanyName,
                ProgramName = i.ProgramName,
                Amount = i.Amount,
                IssueDate = i.IssueDate,
                DueDate = i.DueDate,
                Status = i.Status,
                EmployeeCount = i.EmployeeCount,
                PaidDate = i.PaidDate
            })
            .ToListAsync();

        return Ok(invoices);
    }

    [HttpPost]
    public async Task<ActionResult<CorporateInvoiceDto>> Create(CreateCorporateInvoiceDto dto)
    {
        // Generate invoice number
        var count = await _context.CorporateInvoices.CountAsync();
        var invoiceNumber = $"INV-{DateTime.UtcNow.Year}-{(count + 1):D3}";

        var invoice = new CorporateInvoice
        {
            InvoiceNumber = invoiceNumber,
            CompanyEmail = dto.CompanyEmail,
            CompanyName = dto.CompanyName,
            TrainingRequestId = dto.TrainingRequestId,
            ProgramName = dto.ProgramName,
            Amount = dto.Amount,
            IssueDate = DateTime.UtcNow,
            DueDate = dto.DueDate,
            Status = "Pending",
            EmployeeCount = dto.EmployeeCount
        };

        _context.CorporateInvoices.Add(invoice);
        await _context.SaveChangesAsync();

        var result = new CorporateInvoiceDto
        {
            Id = invoice.Id,
            InvoiceNumber = invoice.InvoiceNumber,
            CompanyName = invoice.CompanyName,
            ProgramName = invoice.ProgramName,
            Amount = invoice.Amount,
            IssueDate = invoice.IssueDate,
            DueDate = invoice.DueDate,
            Status = invoice.Status,
            EmployeeCount = invoice.EmployeeCount,
            PaidDate = invoice.PaidDate
        };

        return CreatedAtAction(nameof(GetAll), new { id = invoice.Id }, result);
    }

    [HttpPut("{id}/pay")]
    public async Task<IActionResult> MarkAsPaid(int id, [FromBody] string paymentId)
    {
        var invoice = await _context.CorporateInvoices.FindAsync(id);
        if (invoice == null)
            return NotFound();

        invoice.Status = "Paid";
        invoice.PaymentId = paymentId;
        invoice.PaidDate = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var invoice = await _context.CorporateInvoices.FindAsync(id);
        if (invoice == null)
            return NotFound();

        _context.CorporateInvoices.Remove(invoice);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
