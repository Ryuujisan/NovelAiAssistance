namespace API.Entities;

public class Chapter : BaseEntity
{
    public string? Name { get; set; }
    public required int Order{ get; set; }
    public required int ProjectId { get; set; }
    public virtual Project Project { get; set; }
    public string? Content { get; set; }
    public string? DrawContent { get; set; }
} 