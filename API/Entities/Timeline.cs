namespace API.Entities;

public class Timeline : BaseEntity
{
    public ICollection<TimelineEvent> Events { get; set; } = new List<TimelineEvent>();
    // opcjonalnie: ostatnie metryki zbiorcze, żeby nie liczyć za każdym razem
    public int TotalWords { get; set; }
    public int ChaptersDone { get; set; }
    public int CharactersDone { get; set; }
}