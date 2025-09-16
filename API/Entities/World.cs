namespace API.Entities;

public class World : BaseEntity
{
    public required string Name { get; set; }
    public required string Description { get; set; }
    public string? Image { get; set; }
    
    public required int ProjectId { get; set; }
    public virtual Project Project { get; set; }
}