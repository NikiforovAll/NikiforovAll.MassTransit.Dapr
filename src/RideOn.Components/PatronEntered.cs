namespace RideOn.Contracts;

public record PatronEntered
{
    public Guid PatronId { get; init; }
    public DateTime Timestamp { get; init; }
}
