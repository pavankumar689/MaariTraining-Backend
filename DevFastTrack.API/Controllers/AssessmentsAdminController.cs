using DevFastTrack.API.Data;
using DevFastTrack.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DevFastTrack.API.Controllers;

[ApiController]
[Route("api/admin/assessments")]
[Authorize(Roles = "Admin")]
public class AssessmentsAdminController : ControllerBase
{
    private readonly AppDbContext _db;

    public AssessmentsAdminController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        // Fetch raw data first (DateTime.SpecifyKind cannot be translated to SQL)
        var raw = await _db.Assessments
            .Include(a => a.Course)
            .Include(a => a.Questions)
            .OrderByDescending(a => a.CreatedAt)
            .Select(a => new
            {
                a.Id,
                a.Title,
                a.Description,
                a.StartTime,
                a.EndTime,
                a.DurationMinutes,
                a.TotalMarks,
                a.CourseId,
                Course = a.Course == null ? null : new { a.Course.Title },
                Questions = a.Questions.Select(q => new { q.Id, q.Text, q.IsMultiSelect, q.Marks }).ToList()
            })
            .ToListAsync();

        // Tag as UTC so JSON serializer adds 'Z' suffix → browser correctly interprets as UTC and converts to local time
        var result = raw.Select(a => new
        {
            a.Id,
            a.Title,
            a.Description,
            StartTime = DateTime.SpecifyKind(a.StartTime, DateTimeKind.Utc),
            EndTime   = DateTime.SpecifyKind(a.EndTime,   DateTimeKind.Utc),
            a.DurationMinutes,
            a.TotalMarks,
            a.CourseId,
            a.Course,
            a.Questions
        });

        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateAssessmentDto request)
    {
        var assessment = new Assessment
        {
            CourseId = request.CourseId,
            Title = request.Title,
            Description = request.Description,
            StartTime = request.StartTime,
            EndTime = request.EndTime,
            DurationMinutes = request.DurationMinutes,
            TotalMarks = request.Questions.Sum(q => q.Marks),
            CreatedAt = DateTime.UtcNow
        };

        foreach (var qDto in request.Questions)
        {
            var question = new Question
            {
                Text = qDto.Text,
                IsMultiSelect = qDto.IsMultiSelect,
                Marks = qDto.Marks
            };

            foreach (var oDto in qDto.Options)
            {
                question.Options.Add(new QuestionOption
                {
                    Text = oDto.Text,
                    IsCorrect = oDto.IsCorrect
                });
            }
            assessment.Questions.Add(question);
        }

        _db.Assessments.Add(assessment);
        await _db.SaveChangesAsync();
        
        return Ok(new { 
            id = assessment.Id,
            title = assessment.Title,
            courseId = assessment.CourseId,
            startTime = assessment.StartTime,
            endTime = assessment.EndTime,
            durationMinutes = assessment.DurationMinutes,
            totalMarks = assessment.TotalMarks
        });
    }

    [HttpPatch("{id}/schedule")]
    public async Task<IActionResult> UpdateSchedule(int id, [FromBody] UpdateScheduleDto dto)
    {
        var assessment = await _db.Assessments.FindAsync(id);
        if (assessment == null) return NotFound();

        if (dto.StartTime.HasValue) assessment.StartTime = dto.StartTime.Value;
        if (dto.EndTime.HasValue)   assessment.EndTime   = dto.EndTime.Value;
        if (dto.DurationMinutes.HasValue) assessment.DurationMinutes = dto.DurationMinutes.Value;

        await _db.SaveChangesAsync();
        return Ok(new { message = "Schedule updated.", id = assessment.Id });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var assessment = await _db.Assessments.FindAsync(id);
        if (assessment == null) return NotFound();

        // Delete in dependency order to avoid FK constraint errors
        // 1. AttemptAnswers for this assessment's attempts
        var attemptIds = await _db.AssessmentAttempts
            .Where(a => a.AssessmentId == id)
            .Select(a => a.Id)
            .ToListAsync();
        
        if (attemptIds.Any())
        {
            await _db.AttemptAnswers
                .Where(aa => attemptIds.Contains(aa.AssessmentAttemptId))
                .ExecuteDeleteAsync();
        }

        // 2. AssessmentAttempts
        await _db.AssessmentAttempts
            .Where(a => a.AssessmentId == id)
            .ExecuteDeleteAsync();

        // 3. QuestionOptions for this assessment's questions
        var questionIds = await _db.Questions
            .Where(q => q.AssessmentId == id)
            .Select(q => q.Id)
            .ToListAsync();
        
        if (questionIds.Any())
        {
            await _db.QuestionOptions
                .Where(o => questionIds.Contains(o.QuestionId))
                .ExecuteDeleteAsync();
        }

        // 4. Questions
        await _db.Questions
            .Where(q => q.AssessmentId == id)
            .ExecuteDeleteAsync();

        // 5. Assessment itself
        _db.Assessments.Remove(assessment);
        await _db.SaveChangesAsync();

        return Ok(new { message = "Assessment deleted successfully." });
    }
}

public class UpdateScheduleDto
{
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public int? DurationMinutes { get; set; }
}
public class CreateAssessmentDto
{
    public int CourseId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public int DurationMinutes { get; set; }
    public List<CreateQuestionDto> Questions { get; set; } = new List<CreateQuestionDto>();
}

public class CreateQuestionDto
{
    public string Text { get; set; } = string.Empty;
    public bool IsMultiSelect { get; set; }
    public decimal Marks { get; set; }
    public List<CreateOptionDto> Options { get; set; } = new List<CreateOptionDto>();
}

public class CreateOptionDto
{
    public string Text { get; set; } = string.Empty;
    public bool IsCorrect { get; set; }
}
