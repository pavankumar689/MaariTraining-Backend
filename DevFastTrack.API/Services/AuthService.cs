using DevFastTrack.API.Data;
using DevFastTrack.API.DTOs.Auth;
using DevFastTrack.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DevFastTrack.API.Services;

public class AuthService : IAuthService
{
    private readonly AppDbContext _db;
    private readonly ITokenService _tokenService;

    public AuthService(AppDbContext db, ITokenService tokenService)
    {
        _db = db;
        _tokenService = tokenService;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        var exists = await _db.Users
            .AnyAsync(u => u.Email == request.Email.ToLower());

        if (exists)
            throw new InvalidOperationException("Email already registered.");

        var user = new User
        {
            Name = request.Name.Trim(),
            Email = request.Email.ToLower().Trim(),
            Phone = request.Phone.Trim(),
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Role = "Student"
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        return BuildAuthResponse(user);
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        var user = await _db.Users
            .FirstOrDefaultAsync(u => u.Email == request.Email.ToLower());

        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("Invalid email or password.");

        return BuildAuthResponse(user);
    }

    public async Task<AuthResponse> ExternalLoginAsync(ExternalLoginRequest request)
    {
        // In a real app, you would verify the idToken from Google/GitHub here first.
        var email = request.Email.ToLower().Trim();
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == email);

        if (user == null)
        {
            // Auto-register the user if they don't exist
            user = new User
            {
                Name = request.Name.Trim(),
                Email = email,
                Phone = "", // External auth doesn't usually provide phone
                PasswordHash = "", // No password for OAuth users
                Role = "Student"
            };

            _db.Users.Add(user);
            await _db.SaveChangesAsync();
        }

        return BuildAuthResponse(user);
    }

    private AuthResponse BuildAuthResponse(User user)
    {
        return new AuthResponse
        {
            Token = _tokenService.GenerateToken(user),
            User = new UserDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Phone = user.Phone,
                Role = user.Role
            }
        };
    }
}