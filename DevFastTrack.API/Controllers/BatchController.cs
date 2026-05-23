using DevFastTrack.API.Data;
using DevFastTrack.API.DTOs.Batch;
using DevFastTrack.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DevFastTrack.API.Controllers;

[ApiController]
[Route("api/batches")]
public class BatchesController : ControllerBase
{
    private readonly AppDbContext _db;

    public BatchesController(AppDbContext db) { _db = db; }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var batches = await _db.Batches
            .Include(b => b.Course)
            .Include(b => b.Enrollments)
            .Where(b => b.IsActive)
            .ToListAsync();

        return Ok(batches.Select(MapToResponse));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var batch = await _db.Batches
            .Include(b => b.Course)
            .Include(b => b.Enrollments)
            .FirstOrDefaultAsync(b => b.Id == id);

        if (batch == null) throw new KeyNotFoundException("Batch not found.");
        return Ok(MapToResponse(batch));
    }

    [HttpGet("course/{courseId}")]
    public async Task<IActionResult> GetByCourse(int courseId)
    {
        var batches = await _db.Batches
            .Include(b => b.Course)
            .Include(b => b.Enrollments)
            .Where(b => b.CourseId == courseId && b.IsActive)
            .ToListAsync();

        return Ok(batches.Select(MapToResponse));
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(CreateBatchRequest request)
    {
        var course = await _db.Courses.FindAsync(request.CourseId);
        if (course == null) throw new KeyNotFoundException("Course not found.");

        var batch = new Batch
        {
            CourseId = request.CourseId,
            BatchName = request.BatchName,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            Timing = request.Timing,
            MeetingLink = request.MeetingLink,
            SeatsTotal = request.SeatsTotal
        };

        _db.Batches.Add(batch);
        await _db.SaveChangesAsync();

        var created = await _db.Batches
            .Include(b => b.Course)
            .Include(b => b.Enrollments)
            .FirstAsync(b => b.Id == batch.Id);

        return Ok(MapToResponse(created));
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, CreateBatchRequest request)
    {
        var batch = await _db.Batches.FindAsync(id);
        if (batch == null) throw new KeyNotFoundException("Batch not found.");

        batch.BatchName = request.BatchName;
        batch.StartDate = request.StartDate;
        batch.EndDate = request.EndDate;
        batch.Timing = request.Timing;
        batch.MeetingLink = request.MeetingLink;
        batch.SeatsTotal = request.SeatsTotal;

        await _db.SaveChangesAsync();
        return Ok(new { message = "Batch updated." });
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var batch = await _db.Batches.FindAsync(id);
        if (batch == null) throw new KeyNotFoundException("Batch not found.");

        batch.IsActive = false;
        await _db.SaveChangesAsync();
        return Ok(new { message = "Batch deleted." });
    }

    private static BatchResponse MapToResponse(Batch b) => new()
    {
        Id = b.Id,
        CourseId = b.CourseId,
        CourseName = b.Course?.Title ?? string.Empty,
        BatchName = b.BatchName,
        StartDate = b.StartDate,
        EndDate = b.EndDate,
        Timing = b.Timing,
        MeetingLink = b.MeetingLink,
        SeatsTotal = b.SeatsTotal,
        SeatsLeft = b.SeatsTotal - (b.Enrollments?.Count ?? 0),
        IsActive = b.IsActive
    };
}