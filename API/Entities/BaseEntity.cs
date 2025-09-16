namespace API.Entities;

public abstract class BaseEntity
{
    public int Id { get; set; }
    public DateTime CreateAt { get; set; }
    public DateTime UpdateAt { get; set; }
    
    public override bool Equals(object? obj) =>
        obj is BaseEntity other && Id == other.Id;
}