using DevFastTrack.API.Data;
using DevFastTrack.API.DTOs.CorporateEmployee;
using DevFastTrack.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DevFastTrack.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CorporateEmployeeController : ControllerBase
{
    private readonly AppDbContext _context;

    public CorporateEmployeeController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CorporateEmployeeDto>>> GetAll()
    {
        var employees = await _context.CorporateEmployees
            .Include(e => e.Course)
            .Include(e => e.Batch)
            .Select(e => new CorporateEmployeeDto
            {
                Id = e.Id,
                EmployeeName = e.EmployeeName,
                EmployeeEmail = e.EmployeeEmail,
                CourseName = e.Course != null ? e.Course.Title : "",
                BatchName = e.Batch != null ? e.Batch.BatchName : null,
                Progress = e.Progress,
                Status = e.Status,
                EnrollmentDate = e.EnrollmentDate,
                LastActiveDate = e.LastActiveDate,
                CompletedModules = e.CompletedModules,
                TotalModules = e.TotalModules
            })
            .ToListAsync();

        return Ok(employees);
    }

    [HttpGet("company/{companyEmail}")]
    public async Task<ActionResult<IEnumerable<CorporateEmployeeDto>>> GetByCompany(string companyEmail)
    {
        var employees = await _context.CorporateEmployees
            .Include(e => e.Course)
            .Include(e => e.Batch)
            .Where(e => e.CompanyEmail == companyEmail)
            .Select(e => new CorporateEmployeeDto
            {
                Id = e.Id,
                EmployeeName = e.EmployeeName,
                EmployeeEmail = e.EmployeeEmail,
                CourseName = e.Course != null ? e.Course.Title : "",
                BatchName = e.Batch != null ? e.Batch.BatchName : null,
                Progress = e.Progress,
                Status = e.Status,
                EnrollmentDate = e.EnrollmentDate,
                LastActiveDate = e.LastActiveDate,
                CompletedModules = e.CompletedModules,
                TotalModules = e.TotalModules
            })
            .ToListAsync();

        return Ok(employees);
    }

    [HttpPost]
    public async Task<ActionResult<CorporateEmployeeDto>> Create(CreateCorporateEmployeeDto dto)
    {
        var employee = new CorporateEmployee
        {
            CompanyEmail = dto.CompanyEmail,
            EmployeeName = dto.EmployeeName,
            EmployeeEmail = dto.EmployeeEmail,
            CourseId = dto.CourseId,
            BatchId = dto.BatchId,
            TotalModules = dto.TotalModules,
            EnrollmentDate = DateTime.UtcNow,
            Status = "Not Started",
            Progress = 0,
            CompletedModules = 0
        };

        _context.CorporateEmployees.Add(employee);
        await _context.SaveChangesAsync();

        var course = await _context.Courses.FindAsync(dto.CourseId);
        var batch = dto.BatchId.HasValue ? await _context.Batches.FindAsync(dto.BatchId.Value) : null;

        var result = new CorporateEmployeeDto
        {
            Id = employee.Id,
            EmployeeName = employee.EmployeeName,
            EmployeeEmail = employee.EmployeeEmail,
            CourseName = course?.Title ?? "",
            BatchName = batch?.BatchName,
            Progress = employee.Progress,
            Status = employee.Status,
            EnrollmentDate = employee.EnrollmentDate,
            LastActiveDate = employee.LastActiveDate,
            CompletedModules = employee.CompletedModules,
            TotalModules = employee.TotalModules
        };

        return CreatedAtAction(nameof(GetAll), new { id = employee.Id }, result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, CorporateEmployeeDto dto)
    {
        var employee = await _context.CorporateEmployees.FindAsync(id);
        if (employee == null)
            return NotFound();

        employee.Progress = dto.Progress;
        employee.Status = dto.Status;
        employee.CompletedModules = dto.CompletedModules;
        employee.LastActiveDate = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var employee = await _context.CorporateEmployees.FindAsync(id);
        if (employee == null)
            return NotFound();

        _context.CorporateEmployees.Remove(employee);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
