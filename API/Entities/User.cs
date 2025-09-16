namespace API.Entities;

public class User : BaseEntity
{
    public required string Name { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }
    public required int WriteSettingsId { get; set; }
    public virtual WriteSettings WriteSettings { get; set; }
    public ERole Role { get; set; }
    public ESubscription ESubscription { get; set; }
    
    public int GenerateLeft { get; set; } // or tokens or create premium currency
    
    public virtual List<Project> Projects { get; set; }
}