namespace API.Entities;

public enum EOrientation
{
    Lesbian,
    Hetero,
    Gay,
    Aseksual
}
public class Character : BaseEntity
{
    public required string Name { get; set; }
    public required string Gender { get; set; }
    public required EOrientation Orientation { get; set; }
    public required string Description { get; set; }
    public string? Image { get; set; }
    public int? Age { get; set; }
    public string? NSFW { get; set; }
    
    public virtual List<Character> Relationships { get; set; } = new();
    public required int ProjectId { get; set; }
    public virtual Project Project { get; set; }
}