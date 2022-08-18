namespace RideOn.Contracts;

public record PatronVisited
{
    public Guid PatronId { get; }
    public DateTime Entered { get; }
    public DateTime Left { get; }
}
