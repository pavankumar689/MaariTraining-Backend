using System.Security.Claims;
using DevFastTrack.API.Data;
using DevFastTrack.API.DTOs.Enrollment;
using DevFastTrack.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DevFastTrack.API.Controllers;

[ApiController]
[Route("api/enrollments")]
[Authorize]
public class EnrollmentsController : ControllerBase
{
    private readonly AppDbContext _db;

    public EnrollmentsController(AppDbContext db) { _db = db; }



    [HttpGet("student/{studentId}")]
    public async Task<IActionResult> GetStudentEnrollments(int studentId)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var role = User.FindFirstValue(ClaimTypes.Role);

        if (role != "Admin" && userId != studentId)
            return Forbid();

        var enrollments = await _db.Enrollments
            .Include(e => e.Course)
            .Include(e => e.Batch)
            .Include(e => e.User)
            .Where(e => e.UserId == studentId)
            .Select(e => new
            {
                e.Id,
                e.UserId,
                e.CourseId,
                e.BatchId,
                e.PaymentId,
                e.AmountPaid,
                e.Status,
                EnrolledAt = e.EnrollmentDate,
                CourseName = e.Course.Title,
                BatchName = e.Batch.BatchName,
                Timing = e.Batch.Timing,
                MeetingLink = e.Batch.MeetingLink,
                MentorName = e.Course.MentorName,
                StudentName = e.User.Name,
                StudentEmail = e.User.Email
            })
            .ToListAsync();

        return Ok(enrollments);
    }

    [HttpGet("all")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAll()
    {
        var enrollments = await _db.Enrollments
            .Include(e => e.User)
            .Include(e => e.Course)
            .Include(e => e.Batch)
            .OrderByDescending(e => e.EnrollmentDate)
            .Select(e => new
            {
                e.Id,
                e.UserId,
                e.CourseId,
                e.BatchId,
                e.PaymentId,
                e.AmountPaid,
                e.Status,
                EnrolledAt = e.EnrollmentDate,
                CourseName = e.Course.Title,
                BatchName = e.Batch.BatchName,
                Timing = e.Batch.Timing,
                MeetingLink = e.Batch.MeetingLink,
                MentorName = e.Course.MentorName,
                StudentName = e.User.Name,
                StudentEmail = e.User.Email
            })
            .ToListAsync();

        return Ok(enrollments);
    }

    [HttpGet("stats")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetStats()
    {
        var totalStudents = await _db.Users.CountAsync(u => u.Role == "Student");
        var activeBatches = await _db.Batches.CountAsync(b => b.IsActive);
        var totalEnrollments = await _db.Enrollments.CountAsync();
        var totalRevenue = await _db.Enrollments.SumAsync(e => (decimal?)e.AmountPaid) ?? 0;

        return Ok(new
        {
            totalStudents,
            activeBatches,
            totalEnrollments,
            totalRevenue
        });
    }
}