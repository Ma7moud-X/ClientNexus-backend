using ClientNexus.Domain.Enums;
using ClientNexus.Domain.ValueObjects;

namespace ClientNexus.Domain.Interfaces;

public interface ICache
{
    public Task<bool> SetStringAsync(string key, string value, TimeSpan? expiration = null);
    public Task<string?> GetStringAsync(string key);

    public Task<bool> SetObjectAsync<T>(string key, T value, TimeSpan? expiration = null);
    public Task<T?> GetObjectAsync<T>(string key);

    public Task<long> AddToListStringAsync(string key, string value);
    public Task<long> AddToListObjectAsync<T>(string key, T value);
    public Task<IEnumerable<string>?> GetListStringAsync(string key);
    public Task<IEnumerable<T?>?> GetListObjectAsync<T>(string key);
    public Task<string?> GetListStringAsync(string key, int index);
    public Task<T?> GetListObjectAsync<T>(string key, int index);
    public Task<string?> PopListStringAsync(string key, TimeSpan? timeout = null);
    public Task<T?> PopListObjectAsync<T>(string key, TimeSpan? timeout = null);

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

    public Task<bool> SetHashStringAsync(string key, string field, string value);
    public Task<bool> SetHashObjectAsync<T>(string key, string field, T value);

    public Task<string?> GetHashStringAsync(string key, string field);
    public Task<T?> GetHashObjectAsync<T>(string key, string field);
}
