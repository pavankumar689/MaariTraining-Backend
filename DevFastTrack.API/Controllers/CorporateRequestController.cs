using DevFastTrack.API.Data;
using DevFastTrack.API.DTOs.CorporateRequest;
using DevFastTrack.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DevFastTrack.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CorporateRequestController : ControllerBase
{
    private readonly AppDbContext _context;

    public CorporateRequestController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/CorporateRequest
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CorporateTrainingRequest>>> GetAll()
    {
        var requests = await _context.CorporateTrainingRequests
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
        return Ok(requests);
    }

    // GET: api/CorporateRequest/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<CorporateTrainingRequest>> GetById(int id)
    {
        var request = await _context.CorporateTrainingRequests.FindAsync(id);
        if (request == null)
            return NotFound();
        
        return Ok(request);
    }

    // POST: api/CorporateRequest
    [HttpPost]
    public async Task<ActionResult<CorporateTrainingRequest>> Create(CreateCorporateRequestDto dto)
    {
        var request = new CorporateTrainingRequest
        {
            CompanyName = dto.CompanyName,
            CompanyEmail = dto.CompanyEmail,
            TrainingType = dto.TrainingType,
            Technology = dto.Technology,
            EmployeeCount = dto.EmployeeCount,
            StartDate = dto.StartDate,
            Duration = dto.Duration,
            Mode = dto.Mode,
            Requirements = dto.Requirements,
            Status = "Pending",
            CreatedAt = DateTime.UtcNow
        };

        _context.CorporateTrainingRequests.Add(request);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = request.Id }, request);
    }

    // PUT: api/CorporateRequest/{id}/status
    [HttpPut("{id}/status")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] string status)
    {
        var request = await _context.CorporateTrainingRequests.FindAsync(id);
        if (request == null)
            return NotFound();

        if (status != "Pending" && status != "Approved" && status != "Rejected")
            return BadRequest("Invalid status. Must be Pending, Approved, or Rejected");

        request.Status = status;
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // DELETE: api/CorporateRequest/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var request = await _context.CorporateTrainingRequests.FindAsync(id);
        if (request == null)
            return NotFound();

        _context.CorporateTrainingRequests.Remove(request);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
