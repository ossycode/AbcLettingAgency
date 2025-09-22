using Microsoft.AspNetCore.Identity;

namespace AbcLettingAgency.EntityModel;

public class AppUser : IdentityUser<Guid>
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime RefreshTokenExpiresAtUtc { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public static AppUser Create(string email, string firstName, string lastName)
    {
        return new AppUser
        { 
            Email = email,
            FirstName = firstName,
            LastName = lastName,
            CreatedAt = DateTime.UtcNow,
        };
    }

    public override string ToString()
    {
        return FirstName + " " + LastName;
    }
}
