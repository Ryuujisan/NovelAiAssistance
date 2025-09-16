namespace API.Entities;

public class Project : BaseEntity
{
    public required string Name { get; set; }
    public required string Description { get; set; }
    public string Language { get; set; } = "Polish";
    public string? Image { get; set; }
    public string? Tags {get; set;}
    
    public required int UserId { get; set; }
    public virtual User User { get; set; }
    
    public virtual World World { get; set; }
    public virtual List<Character> Characters { get; set; } = new();
    public virtual List<Chapter> Chapters { get; set; } = new();
    public required int TimelineId { get; set; }
    public virtual Timeline Timeline { get; set; }
}