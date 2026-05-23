using DevFastTrack.API.Data;
using DevFastTrack.API.DTOs.Announcement;
using DevFastTrack.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace DevFastTrack.API.Controllers;

[ApiController]
[Route("api/announcements")]
public class AnnouncementsController : ControllerBase
{
    private readonly AppDbContext _db;
    public AnnouncementsController(AppDbContext db) { _db = db; }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var currentUserIdStr = User.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier);
        var role = User.FindFirstValue(System.Security.Claims.ClaimTypes.Role);

        int? currentUserId = string.IsNullOrEmpty(currentUserIdStr) ? null : int.Parse(currentUserIdStr);

        List<int> enrolledCourseIds = new List<int>();
        if (currentUserId.HasValue && role != "Admin")
        {
            enrolledCourseIds = await _db.Enrollments.Where(e => e.UserId == currentUserId).Select(e => e.CourseId).ToListAsync();
        }

        var announcementsQuery = _db.Announcements.AsQueryable();

        if (role != "Admin")
        {
            // Only show global announcements or announcements for enrolled courses
            announcementsQuery = announcementsQuery.Where(a => 
                a.CourseId == null || 
                (currentUserId.HasValue && enrolledCourseIds.Contains(a.CourseId.Value)));
        }

        var announcements = await announcementsQuery
            .OrderByDescending(a => a.CreatedAt)
            .Select(a => new
            {
                a.Id,
                a.Title,
                a.Description,
                a.CourseId,
                CourseName = a.CourseId.HasValue 
                    ? _db.Courses.Where(c => c.Id == a.CourseId).Select(c => c.Title).FirstOrDefault()
                    : null,
                a.CreatedAt
            })
            .ToListAsync();

        return Ok(announcements);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(CreateAnnouncementRequest request)
    {
        var announcement = new Announcement
        {
            Title = request.Title,
            Description = request.Description,
            CourseId = request.CourseId
        };

        _db.Announcements.Add(announcement);
        await _db.SaveChangesAsync();

        var courseName = announcement.CourseId.HasValue
            ? await _db.Courses.Where(c => c.Id == announcement.CourseId).Select(c => c.Title).FirstOrDefaultAsync()
            : null;

        return Ok(new
        {
            announcement.Id,
            announcement.Title,
            announcement.Description,
            announcement.CourseId,
            CourseName = courseName,
            announcement.CreatedAt
        });
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var a = await _db.Announcements.FindAsync(id);
        if (a == null) throw new KeyNotFoundException("Announcement not found.");

        _db.Announcements.Remove(a);
        await _db.SaveChangesAsync();
        return Ok(new { message = "Deleted." });
    }
}