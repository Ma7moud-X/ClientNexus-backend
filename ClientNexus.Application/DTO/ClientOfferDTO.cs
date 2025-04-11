namespace ClientNexus.Application.DTO;

public class ClientOfferDTO
{
    public required decimal Price { get; init; }
    public required int TimeForArrival { get; init; }
    public required string TimeUnit { get; init; }

    public required int ServiceProviderId { get; init; }
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public required float Rating { get; init; }
    public required int YearsOfExperience { get; init; }
    public required string ImageUrl { get; init; }

    public required DateTime ExpiresAt { get; init; }
}
