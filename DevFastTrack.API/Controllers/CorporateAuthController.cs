using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DevFastTrack.API.Data;
using DevFastTrack.API.DTOs.CorporateCompany;
using DevFastTrack.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace DevFastTrack.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CorporateAuthController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _config;

    public CorporateAuthController(AppDbContext context, IConfiguration config)
    {
        _context = context;
        _config = config;
    }

    [HttpPost("register")]
    public async Task<ActionResult<CorporateAuthResponse>> Register(RegisterCorporateCompanyDto dto)
    {
        // Check if company already exists
        if (await _context.CorporateCompanies.AnyAsync(c => c.Email == dto.Email))
        {
            return BadRequest(new { message = "Company with this email already exists" });
        }

        // Hash password
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

        var company = new CorporateCompany
        {
            CompanyName = dto.CompanyName,
            Industry = dto.Industry,
            ContactPerson = dto.ContactPerson,
            Designation = dto.Designation,
            Email = dto.Email,
            Phone = dto.Phone,
            CompanySize = dto.CompanySize,
            PasswordHash = passwordHash,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        _context.CorporateCompanies.Add(company);
        await _context.SaveChangesAsync();

        var token = GenerateJwtToken(company);

        return Ok(new CorporateAuthResponse
        {
            Token = token,
            Company = new CorporateCompanyDto
            {
                Id = company.Id,
                CompanyName = company.CompanyName,
                Industry = company.Industry,
                ContactPerson = company.ContactPerson,
                Designation = company.Designation,
                Email = company.Email,
                Phone = company.Phone,
                CompanySize = company.CompanySize,
                CreatedAt = company.CreatedAt,
                IsActive = company.IsActive
            }
        });
    }

    [HttpPost("login")]
    public async Task<ActionResult<CorporateAuthResponse>> Login(CorporateLoginDto dto)
    {
        var company = await _context.CorporateCompanies
            .FirstOrDefaultAsync(c => c.Email == dto.Email);

        if (company == null)
        {
            return Unauthorized(new { message = "Invalid email or password" });
        }

        if (!BCrypt.Net.BCrypt.Verify(dto.Password, company.PasswordHash))
        {
            return Unauthorized(new { message = "Invalid email or password" });
        }

        if (!company.IsActive)
        {
            return Unauthorized(new { message = "Company account is inactive" });
        }

        var token = GenerateJwtToken(company);

        return Ok(new CorporateAuthResponse
        {
            Token = token,
            Company = new CorporateCompanyDto
            {
                Id = company.Id,
                CompanyName = company.CompanyName,
                Industry = company.Industry,
                ContactPerson = company.ContactPerson,
                Designation = company.Designation,
                Email = company.Email,
                Phone = company.Phone,
                CompanySize = company.CompanySize,
                CreatedAt = company.CreatedAt,
                IsActive = company.IsActive
            }
        });
    }

    private string GenerateJwtToken(CorporateCompany company)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, company.Id.ToString()),
            new Claim(ClaimTypes.Email, company.Email),
            new Claim(ClaimTypes.Name, company.CompanyName),
            new Claim(ClaimTypes.Role, "Corporate")
        };

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddDays(30),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
