namespace RideOn.Contracts;

public record PatronLeft
{
    public Guid PatronId { get; set; }
    public DateTime Timestamp { get; set; }
}
