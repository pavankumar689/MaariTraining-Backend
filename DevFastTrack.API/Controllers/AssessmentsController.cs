using DevFastTrack.API.Data;
using DevFastTrack.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace DevFastTrack.API.Controllers;

[ApiController]
[Route("api/assessments")]
[Authorize]
public class AssessmentsController : ControllerBase
{
    private readonly AppDbContext _db;

    public AssessmentsController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet("course/{courseId}")]
    public async Task<IActionResult> GetByCourse(int courseId)
    {
        var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var isEnrolled = await _db.Enrollments.AnyAsync(e => e.UserId == currentUserId && e.CourseId == courseId);
        if (!isEnrolled) return Forbid();

        // Fetch raw data from DB first (DateTime.SpecifyKind can't be translated to SQL)
        var rawList = await _db.Assessments
            .Where(a => a.CourseId == courseId)
            .Select(a => new
            {
                a.Id,
                a.Title,
                a.Description,
                a.StartTime,
                a.EndTime,
                a.DurationMinutes,
                a.TotalMarks,
                Attempt = _db.AssessmentAttempts
                    .Where(att => att.AssessmentId == a.Id && att.UserId == currentUserId)
                    .Select(att => new { att.Id, att.IsCompleted, att.Score })
                    .FirstOrDefault()
            })
            .ToListAsync();

        // Apply UTC kind in memory so JSON serializer adds 'Z' suffix
        var result = rawList.Select(a => new
        {
            a.Id,
            a.Title,
            a.Description,
            StartTime = DateTime.SpecifyKind(a.StartTime, DateTimeKind.Utc),
            EndTime   = DateTime.SpecifyKind(a.EndTime,   DateTimeKind.Utc),
            a.DurationMinutes,
            a.TotalMarks,
            a.Attempt
        });

        return Ok(result);
    }

    [HttpPost("{id}/start")]
    public async Task<IActionResult> StartAttempt(int id)
    {
        var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var assessment = await _db.Assessments.FindAsync(id);
        
        if (assessment == null) return NotFound("Assessment not found.");

        var nowUtc = DateTime.UtcNow;
        // Normalize StartTime/EndTime to UTC for comparison
        var startUtc = assessment.StartTime.Kind == DateTimeKind.Utc 
            ? assessment.StartTime 
            : DateTime.SpecifyKind(assessment.StartTime, DateTimeKind.Utc);
        var endUtc = assessment.EndTime.Kind == DateTimeKind.Utc 
            ? assessment.EndTime 
            : DateTime.SpecifyKind(assessment.EndTime, DateTimeKind.Utc);

        if (nowUtc < startUtc || nowUtc > endUtc)
        {
            return BadRequest(new { 
                message = "Assessment is not currently available.",
                startTime = startUtc,
                endTime = endUtc,
                serverTime = nowUtc
            });
        }

        var existingAttempt = await _db.AssessmentAttempts
            .FirstOrDefaultAsync(a => a.AssessmentId == id && a.UserId == currentUserId);

        if (existingAttempt != null)
        {
            if (existingAttempt.IsCompleted) return BadRequest("You have already completed this assessment.");
            // Resume existing attempt — return clean DTO
            return Ok(new { id = existingAttempt.Id, isCompleted = existingAttempt.IsCompleted, startedAt = existingAttempt.StartedAt });
        }

        var attempt = new AssessmentAttempt
        {
            AssessmentId = id,
            UserId = currentUserId,
            StartedAt = DateTime.UtcNow,
            IsCompleted = false
        };

        _db.AssessmentAttempts.Add(attempt);
        await _db.SaveChangesAsync();

        // Return clean DTO to avoid circular reference in JSON serialization
        return Ok(new { id = attempt.Id, isCompleted = attempt.IsCompleted, startedAt = attempt.StartedAt });
    }

    [HttpGet("{id}/questions")]
    public async Task<IActionResult> GetQuestions(int id)
    {
        var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var attempt = await _db.AssessmentAttempts.FirstOrDefaultAsync(a => a.AssessmentId == id && a.UserId == currentUserId);
        
        if (attempt == null || attempt.IsCompleted) return Forbid();

        var questions = await _db.Questions
            .Where(q => q.AssessmentId == id)
            .Select(q => new
            {
                q.Id,
                q.Text,
                q.IsMultiSelect,
                q.Marks,
                Options = q.Options.Select(o => new { o.Id, o.Text }).ToList() // Do not send IsCorrect!
            })
            .ToListAsync();

        return Ok(new { attemptId = attempt.Id, questions });
    }

    [HttpPost("submit")]
    public async Task<IActionResult> Submit([FromBody] SubmitAssessmentDto request)
    {
        var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var attempt = await _db.AssessmentAttempts
            .Include(a => a.Answers)
            .FirstOrDefaultAsync(a => a.Id == request.AttemptId && a.UserId == currentUserId);

        if (attempt == null) return NotFound();
        if (attempt.IsCompleted) return BadRequest("Already submitted.");

        decimal totalScore = 0;

        foreach (var answerDto in request.Answers)
        {
            var question = await _db.Questions
                .Include(q => q.Options)
                .FirstOrDefaultAsync(q => q.Id == answerDto.QuestionId);

            if (question == null) continue;

            var correctOptionIds = question.Options.Where(o => o.IsCorrect).Select(o => o.Id).ToList();
            var userOptionIds = answerDto.SelectedOptionIds;

            decimal marksObtained = 0;

            if (question.IsMultiSelect)
            {
                // Partial marking logic
                int correctSelected = userOptionIds.Count(id => correctOptionIds.Contains(id));
                int incorrectSelected = userOptionIds.Count(id => !correctOptionIds.Contains(id));
                
                if (correctOptionIds.Any() && incorrectSelected == 0)
                {
                    marksObtained = question.Marks * ((decimal)correctSelected / correctOptionIds.Count);
                }
            }
            else
            {
                // Single select
                if (userOptionIds.Count == 1 && correctOptionIds.Contains(userOptionIds.First()))
                {
                    marksObtained = question.Marks;
                }
            }

            totalScore += marksObtained;

            attempt.Answers.Add(new AttemptAnswer
            {
                QuestionId = question.Id,
                SelectedOptionIds = string.Join(",", userOptionIds),
                MarksObtained = marksObtained
            });
        }

        attempt.Score = totalScore;
        attempt.IsCompleted = true;
        attempt.CompletedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();

        return Ok(new { score = totalScore });
    }

    [HttpGet("attempts")]
    public async Task<IActionResult> GetMyAttempts()
    {
        var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        
        var attempts = await _db.AssessmentAttempts
            .Include(a => a.Assessment)
            .Where(a => a.UserId == currentUserId && a.IsCompleted)
            .Select(a => new
            {
                a.Id,
                AssessmentTitle = a.Assessment.Title,
                a.StartedAt,
                a.CompletedAt,
                a.Score,
                a.Assessment.TotalMarks
            })
            .OrderByDescending(a => a.CompletedAt)
            .ToListAsync();

        return Ok(attempts);
    }

    [HttpGet("{id}/review")]
    public async Task<IActionResult> GetReview(int id)
    {
        var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        
        var attempt = await _db.AssessmentAttempts
            .Include(a => a.Answers)
            .FirstOrDefaultAsync(a => a.AssessmentId == id && a.UserId == currentUserId);

        if (attempt == null || !attempt.IsCompleted)
            return BadRequest("You have not completed this assessment yet.");

        var rawQuestions = await _db.Questions
            .Where(q => q.AssessmentId == id)
            .Select(q => new
            {
                q.Id,
                q.Text,
                q.IsMultiSelect,
                q.Marks,
                Options = q.Options.Select(o => new { o.Id, o.Text, o.IsCorrect }).ToList()
            })
            .ToListAsync();

        var questions = rawQuestions.Select(q => new
        {
            q.Id,
            q.Text,
            q.IsMultiSelect,
            q.Marks,
            q.Options,
            UserAnswer = attempt.Answers.FirstOrDefault(aa => aa.QuestionId == q.Id)
        });

        var questionsList = questions.Select(q => new
        {
            q.Id,
            q.Text,
            q.IsMultiSelect,
            q.Marks,
            q.Options,
            marksObtained = q.UserAnswer?.MarksObtained ?? 0,
            selectedOptionIds = string.IsNullOrWhiteSpace(q.UserAnswer?.SelectedOptionIds) 
                ? new List<int>() 
                : q.UserAnswer.SelectedOptionIds.Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(s => int.TryParse(s, out var i) ? i : (int?)null)
                    .Where(i => i.HasValue)
                    .Select(i => i.Value)
                    .ToList()
        }).ToList();

        var result = new
        {
            attemptId = attempt.Id,
            score = attempt.Score,
            completedAt = attempt.CompletedAt,
            questions = questionsList
        };

        return Ok(result);
    }
}

public class SubmitAssessmentDto
{
    public int AttemptId { get; set; }
    public List<QuestionAnswerDto> Answers { get; set; } = new List<QuestionAnswerDto>();
}

public class QuestionAnswerDto
{
    public int QuestionId { get; set; }
    public List<int> SelectedOptionIds { get; set; } = new List<int>();
}
