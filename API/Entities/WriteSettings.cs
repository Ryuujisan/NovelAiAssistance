namespace API.Entities;

public class WriteSettings : BaseEntity
{
    public virtual User User { get; set; }
    public string Language { get; set; } = "Polish";
    public string? Tags {get; set;}
    public string? Prompt {get; set;}
    
    public string? NSFW {get; set;}
    public string? Ecchi {get; set;}
    public string? Gore {get; set;}
    
    public float NSFWThreshold {get; set;}
    public float EcchiThreshold {get; set;}
    public float GoreThreshold {get; set;}
}