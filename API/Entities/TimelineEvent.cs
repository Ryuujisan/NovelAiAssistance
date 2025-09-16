namespace API.Entities;

public class TimelineEvent : BaseEntity
{
    public required int TimelineId { get; set; }
    public Timeline Timeline { get; set; } = default!;

    public TimelineEventType Type { get; set; }
    public string? Title { get; set; }           // krótki opis
    public string? Body { get; set; }            // dłuższy opis
    public string? PayloadJson { get; set; }     // dowolne dane do AI (np. { chapterId, fromStatus, toStatus, deltaWords })

    // Powiązania kontekstowe (opcjonalne, nullable)
    public int? ProjectId { get; set; }
    public int? ChapterId { get; set; }
    public int? CharacterId { get; set; }

    public DateTime OccurredAt { get; set; } = DateTime.UtcNow;
}