namespace DevFastTrack.API.DTOs.CorporateCompany;

public class CorporateCompanyDto
{
    public int Id { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public string Industry { get; set; } = string.Empty;
    public string ContactPerson { get; set; } = string.Empty;
    public string Designation { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string CompanySize { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public bool IsActive { get; set; }
}

public class RegisterCorporateCompanyDto
{
    public string CompanyName { get; set; } = string.Empty;
    public string Industry { get; set; } = string.Empty;
    public string ContactPerson { get; set; } = string.Empty;
    public string Designation { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string CompanySize { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class CorporateLoginDto
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class CorporateAuthResponse
{
    public string Token { get; set; } = string.Empty;
    public CorporateCompanyDto Company { get; set; } = null!;
}
