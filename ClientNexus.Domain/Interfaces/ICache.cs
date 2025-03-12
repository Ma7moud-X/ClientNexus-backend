using ClientNexus.Domain.Enums;
using ClientNexus.Domain.ValueObjects;

namespace ClientNexus.Domain.Interfaces;

public interface ICache
{
    public Task<bool> SetStringAsync(
        string key,
        string value,
        TimeSpan? expiration = null,
        bool @override = true
    );
    public Task<string?> GetStringAsync(string key);

    public Task<bool> SetObjectAsync<T>(
        string key,
        T value,
        TimeSpan? expiration = null,
        bool @override = true
    );
    public Task<T?> GetObjectAsync<T>(string key);

    public Task<long> AddToListStringAsync(string key, string value, bool onlyIfExists = false);
    public Task<long> AddToListObjectAsync<T>(string key, T value, bool onlyIfExists = false);
    public Task<IEnumerable<string>?> GetListStringAsync(string key);
    public Task<IEnumerable<T?>?> GetListObjectAsync<T>(string key);
    public Task<string?> GetListStringAsync(string key, int index);
    public Task<T?> GetListObjectAsync<T>(string key, int index);

    // public Task<string?> BlockPopListStringAsync(string key, TimeSpan? timeout = null);
    // public Task<T?> BlockPopListObjectAsync<T>(string key, TimeSpan? timeout = null);
    Task<string?> LeftPopListStringAsync(string key);
    Task<T?> LeftPopListObjectAsync<T>(string key);

    public Task<bool> AddGeoLocationAsync(
        string key,
        double longitude,
        double latitude,
        string identifier
    );
    public Task<bool> RemoveGeoLocationAsync(string key, string identifier);
    public Task<IEnumerable<RelativeLocation>?> GetGeoLocationsInRadiusAsync(
        string key,
        double longitude,
        double latitude,
        double radius,
        DistanceUnit unit = DistanceUnit.Meters
    );
    public Task<IEnumerable<RelativeLocation>?> GetGeoLocationsInRadiusAsync(
        string key,
        string identifier,
        double radius,
        DistanceUnit unit = DistanceUnit.Meters
    );
    public Task<Location?> GetGeoLocationAsync(string key, string identifier);

    public Task<bool> AddToSetStringAsync(string key, string value);
    public Task<bool> RemoveFromSetStringAsync(string key, string value);
    public Task<bool> CheckExistsInSetAsync(string key, string value);

    public bool StartTransaction();
    public Task<bool> CommitTransactionAsync();

    public Task<bool> RemoveKeyAsync(string key);

    public Task<bool> SetExpiryAsync(string key, TimeSpan expiration);

    public Task<bool> SetHashStringAsync(
        string key,
        string field,
        string value,
        bool @override = true
    );
    public Task<bool> SetHashObjectAsync<T>(
        string key,
        string field,
        T value,
        bool @override = true
    );

    public Task<string?> GetHashStringAsync(string key, string field);
    public Task<T?> GetHashObjectAsync<T>(string key, string field);
    public Task<bool> RemoveHashFieldAsync(string key, string field);

    public Task<TimeSpan?> GetTTLAsync(string key);
}
