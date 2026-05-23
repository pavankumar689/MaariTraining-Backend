using DevFastTrack.API.Data;
using DevFastTrack.API.DTOs.Course;
using DevFastTrack.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DevFastTrack.API.Controllers;

[ApiController]
[Route("api/courses")]
public class CoursesController : ControllerBase
{
    private readonly AppDbContext _db;

    public CoursesController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var courses = await _db.Courses
            .Where(c => c.IsActive)
            .Include(c => c.Batches)
            .Include(c => c.Enrollments)
            .ToListAsync();

        var result = courses.Select(c => MapToResponse(c));
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var course = await _db.Courses
            .Include(c => c.Batches)
            .Include(c => c.Enrollments)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (course == null)
            throw new KeyNotFoundException("Course not found.");

        return Ok(MapToResponse(course));
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(CreateCourseRequest request)
    {
        var course = new Course
        {
            Title = request.Title,
            Description = request.Description,
            Price = request.Price,
            OriginalPrice = request.OriginalPrice,
            Duration = request.Duration,
            Level = request.Level,
            ThumbnailUrl = request.ThumbnailUrl,
            Syllabus = request.Syllabus,
            Prerequisites = request.Prerequisites,
            Outcomes = request.Outcomes,
            MentorName = request.MentorName,
            MentorBio = request.MentorBio,
            SoftwareRequirements = request.SoftwareRequirements
        };

        _db.Courses.Add(course);
        await _db.SaveChangesAsync();
        return Ok(MapToResponse(course));
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, CreateCourseRequest request)
    {
        var course = await _db.Courses.FindAsync(id);
        if (course == null) throw new KeyNotFoundException("Course not found.");

        course.Title = request.Title;
        course.Description = request.Description;
        course.Price = request.Price;
        course.OriginalPrice = request.OriginalPrice;
        course.Duration = request.Duration;
        course.Level = request.Level;
        course.ThumbnailUrl = request.ThumbnailUrl;
        course.Syllabus = request.Syllabus;
        course.Prerequisites = request.Prerequisites;
        course.Outcomes = request.Outcomes;
        course.MentorName = request.MentorName;
        course.MentorBio = request.MentorBio;
        course.SoftwareRequirements = request.SoftwareRequirements;

        await _db.SaveChangesAsync();
        return Ok(MapToResponse(course));
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var course = await _db.Courses.FindAsync(id);
        if (course == null) throw new KeyNotFoundException("Course not found.");

        course.IsActive = false;
        await _db.SaveChangesAsync();
        return Ok(new { message = "Course deleted." });
    }

    private static CourseResponse MapToResponse(Course c) => new()
    {
        Id = c.Id,
        Title = c.Title,
        Description = c.Description,
        Price = c.Price,
        OriginalPrice = c.OriginalPrice,
        Duration = c.Duration,
        Level = c.Level,
        ThumbnailUrl = c.ThumbnailUrl,
        Syllabus = c.Syllabus,
        Prerequisites = c.Prerequisites,
        Outcomes = c.Outcomes,
        MentorName = c.MentorName,
        MentorBio = c.MentorBio,
        SoftwareRequirements = c.SoftwareRequirements,
        IsActive = c.IsActive,
        TotalBatches = c.Batches?.Count ?? 0,
        SeatsLeft = c.Batches != null
            ? c.Batches.Where(b => b.IsActive)
                       .Sum(b => b.SeatsTotal - (c.Enrollments?.Count(e => e.BatchId == b.Id) ?? 0))
            : 0,
        CreatedAt = c.CreatedAt
    };
}