namespace ClientNexus.Application.Models;

public class ServiceProviderOverview
{
    public required int ServiceProviderId { get; init; }
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public required float Rating { get; init; }
    public required int YearsOfExperience { get; init; }
    public required string ImageUrl { get; init; }
}
