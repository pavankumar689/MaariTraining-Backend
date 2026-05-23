using DevFastTrack.API.Data;
using DevFastTrack.API.DTOs.Material;
using DevFastTrack.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace DevFastTrack.API.Controllers;

[ApiController]
[Route("api/materials")]
public class MaterialsController : ControllerBase
{
    private readonly AppDbContext _db;
    public MaterialsController(AppDbContext db) { _db = db; }

    [HttpGet("course/{courseId}")]
    [Authorize]
    public async Task<IActionResult> GetByCourse(int courseId)
    {
        var currentUserId = int.Parse(User.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier)!);
        var role = User.FindFirstValue(System.Security.Claims.ClaimTypes.Role);

        if (role != "Admin")
        {
            var isEnrolled = await _db.Enrollments.AnyAsync(e => e.CourseId == courseId && e.UserId == currentUserId);
            if (!isEnrolled) return Forbid();
        }

        var materials = await _db.Materials
            .Where(m => m.CourseId == courseId)
            .OrderByDescending(m => m.CreatedAt)
            .Select(m => new
            {
                m.Id,
                m.CourseId,
                m.Title,
                m.Type,
                m.FileUrl,
                UploadedAt = m.CreatedAt
            })
            .ToListAsync();

        return Ok(materials);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(CreateMaterialRequest request)
    {
        var material = new Material
        {
            CourseId = request.CourseId,
            Title = request.Title,
            Type = request.Type,
            FileUrl = request.FileUrl
        };

        _db.Materials.Add(material);
        await _db.SaveChangesAsync();
        
        return Ok(new
        {
            material.Id,
            material.CourseId,
            material.Title,
            material.Type,
            material.FileUrl,
            UploadedAt = material.CreatedAt
        });
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var material = await _db.Materials.FindAsync(id);
        if (material == null) throw new KeyNotFoundException("Material not found.");

        _db.Materials.Remove(material);
        await _db.SaveChangesAsync();
        return Ok(new { message = "Material deleted." });
    }
}