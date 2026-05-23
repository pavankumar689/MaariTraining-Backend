using DevFastTrack.API.DTOs.Auth;

namespace DevFastTrack.API.Services;

public interface IAuthService
{
    Task<AuthResponse> RegisterAsync(RegisterRequest request);
    Task<AuthResponse> LoginAsync(LoginRequest request);
    Task<AuthResponse> ExternalLoginAsync(ExternalLoginRequest request);
}

public class ExternalLoginRequest
{
    public string Email { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Provider { get; set; } = string.Empty; // "Google" or "GitHub"
}