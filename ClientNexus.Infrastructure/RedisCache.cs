using System.Text.Json;
using ClientNexus.Domain.Enums;
using ClientNexus.Domain.Interfaces;
using ClientNexus.Domain.ValueObjects;
using StackExchange.Redis;

namespace ClientNexus.Infrastructure;

public class RedisCache : ICache
{
    private readonly IConnectionMultiplexer _redis;
    private readonly IDatabase _database;
    private ITransaction? _transaction = null;

    public RedisCache(IConnectionMultiplexer redis)
    {
        _redis = redis;
        _database = _redis.GetDatabase();
    }

    public async Task<bool> AddGeoLocationAsync(
        string key,
        double longitude,
        double latitude,
        string identifier
    )
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            throw new ArgumentException("key cannot be null or whitespace", nameof(key));
        }

        if (string.IsNullOrWhiteSpace(identifier))
        {
            throw new ArgumentException(
                "identifier cannot be null or whitespace",
                nameof(identifier)
            );
        }

        if (longitude < -180 || longitude > 180)
        {
            throw new ArgumentException(
                "longitude must be between -180 and 180",
                nameof(longitude)
            );
        }

        if (latitude < -90 || latitude > 90)
        {
            throw new ArgumentException("latitude must be between -90 and 90", nameof(latitude));
        }

        IDatabaseAsync executor = _transaction is not null ? _transaction : _database;
        return await executor.GeoAddAsync(key, longitude, latitude, identifier);
    }

    public async Task<long> AddToListObjectAsync<T>(string key, T value, bool onlyIfExists = false)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            throw new ArgumentException("key cannot be null or whitespace", nameof(key));
        }

        if (value is null)
        {
            throw new ArgumentException("value cannot be null", nameof(value));
        }

        IDatabaseAsync executor = _transaction is not null ? _transaction : _database;
        return await executor.ListRightPushAsync(
            key,
            JsonSerializer.Serialize(value),
            when: onlyIfExists ? When.Exists : When.Always
        );
    }

    public async Task<long> AddToListStringAsync(
        string key,
        string value,
        bool onlyIfExists = false
    )
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            throw new ArgumentException("key cannot be null or whitespace", nameof(key));
        }

        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("value cannot be null or whitespace", nameof(value));
        }

        IDatabaseAsync executor = _transaction is not null ? _transaction : _database;
        return await executor.ListRightPushAsync(
            key,
            value,
            when: onlyIfExists ? When.Exists : When.Always
        );
    }

    public async Task<bool> CommitTransactionAsync()
    {
        if (_transaction is null)
        {
            throw new InvalidOperationException("No transaction is currently active.");
        }

        bool result = await _transaction.ExecuteAsync();
        _transaction = null;

        return result;
    }

    public async Task<IEnumerable<T?>?> GetListObjectAsync<T>(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            throw new ArgumentException("key cannot be null or whitespace", nameof(key));
        }

        IDatabaseAsync executor = _transaction is not null ? _transaction : _database;
        RedisValue[] res = await executor.ListRangeAsync(key);
        return res.Length == 0
            ? null
            : res.Select(v => JsonSerializer.Deserialize<T>(v.ToString()));
    }

    public async Task<IEnumerable<string>?> GetListStringAsync(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            throw new ArgumentException("key cannot be null or whitespace", nameof(key));
        }

        IDatabaseAsync executor = _transaction is not null ? _transaction : _database;
        RedisValue[] res = await executor.ListRangeAsync(key);
        return res.Length == 0 ? null : res.Select(v => v.ToString());
    }

    public async Task<T?> GetObjectAsync<T>(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            throw new ArgumentException("key cannot be null or whitespace", nameof(key));
        }

        IDatabaseAsync executor = _transaction is not null ? _transaction : _database;
        string? res = await executor.StringGetAsync(key);
        return res is null ? default : JsonSerializer.Deserialize<T>(res);
    }

    public async Task<string?> GetStringAsync(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            throw new ArgumentException("key cannot be null or whitespace", nameof(key));
        }

        IDatabaseAsync executor = _transaction is not null ? _transaction : _database;
        return await executor.StringGetAsync(key);
    }

    public async Task<bool> RemoveGeoLocationAsync(string key, string identifier)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            throw new ArgumentException("key cannot be null or whitespace", nameof(key));
        }

        if (string.IsNullOrWhiteSpace(identifier))
        {
            throw new ArgumentException(
                "identifier cannot be null or whitespace",
                nameof(identifier)
            );
        }

        IDatabaseAsync executor = _transaction is not null ? _transaction : _database;
        return await executor.GeoRemoveAsync(key, identifier);
    }

    public async Task<bool> RemoveKeyAsync(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            throw new ArgumentException("key cannot be null or whitespace", nameof(key));
        }

        IDatabaseAsync executor = _transaction is not null ? _transaction : _database;
        return await executor.KeyDeleteAsync(key);
    }

    public async Task<string?> GetListStringAsync(string key, int index)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            throw new ArgumentException("key cannot be null or whitespace", nameof(key));
        }

        // if (index < 0)
        // {
        //     throw new ArgumentException("index cannot be less than zero", nameof(index));
        // }

        IDatabaseAsync executor = _transaction is not null ? _transaction : _database;
        return await executor.ListGetByIndexAsync(key, index);
    }

    public async Task<T?> GetListObjectAsync<T>(string key, int index)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            throw new ArgumentException("key cannot be null or whitespace", nameof(key));
        }

        // if (index < 0)
        // {
        //     throw new ArgumentException("index cannot be less than zero", nameof(index));
        // }

        IDatabaseAsync executor = _transaction is not null ? _transaction : _database;
        var res = await executor.ListGetByIndexAsync(key, index);
        if (res.IsNull)
        {
            return default;
        }

        return JsonSerializer.Deserialize<T>(res.ToString());
    }

    public async Task<bool> SetExpiryAsync(string key, TimeSpan expiration)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            throw new ArgumentException("key cannot be null or whitespace", nameof(key));
        }

        if (expiration <= TimeSpan.Zero)
        {
            throw new ArgumentException(
                "expiration cannot be less than or equal to zero",
                nameof(expiration)
            );
        }

        IDatabaseAsync executor = _transaction is not null ? _transaction : _database;
        return await executor.KeyExpireAsync(key, expiration);
    }

    public async Task<bool> SetObjectAsync<T>(
        string key,
        T value,
        TimeSpan? expiration = null,
        bool @override = true
    )
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            throw new ArgumentException("key cannot be null or whitespace", nameof(key));
        }

        if (value is null)
        {
            throw new ArgumentException("value cannot be null", nameof(value));
        }

        IDatabaseAsync executor = _transaction is not null ? _transaction : _database;
        return await executor.StringSetAsync(
            key,
            JsonSerializer.Serialize(value),
            expiration,
            @override ? When.Always : When.NotExists
        );
    }

    public async Task<bool> SetStringAsync(
        string key,
        string value,
        TimeSpan? expiration = null,
        bool @override = true
    )
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            throw new ArgumentException("key cannot be null or whitespace", nameof(key));
        }

        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("value cannot be null or whitespace", nameof(value));
        }

        if (expiration.HasValue && expiration.Value.TotalSeconds <= 0)
        {
            throw new ArgumentException("expiration must be greater than zero", nameof(expiration));
        }

        IDatabaseAsync executor = _transaction is not null ? _transaction : _database;
        return await executor.StringSetAsync(
            key,
            value,
            expiration,
            @override ? When.Always : When.NotExists
        );
    }

    public bool StartTransaction()
    {
        if (_transaction is not null)
        {
            throw new InvalidOperationException("A transaction is already active.");
        }

        _transaction = _database.CreateTransaction();
        return true;
    }

    public async Task<bool> AddToSetStringAsync(string key, string value)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            throw new ArgumentException("key cannot be null or whitespace", nameof(key));
        }

        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("value cannot be null or whitespace", nameof(value));
        }

        IDatabaseAsync executor = _transaction is not null ? _transaction : _database;
        return await executor.SetAddAsync(key, value);
    }

    public async Task<bool> RemoveFromSetStringAsync(string key, string value)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            throw new ArgumentException("key cannot be null or whitespace", nameof(key));
        }

        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("value cannot be null or whitespace", nameof(value));
        }

        IDatabaseAsync executor = _transaction is not null ? _transaction : _database;
        return await executor.SetRemoveAsync(key, value);
    }

    public async Task<bool> CheckExistsInSetAsync(string key, string value)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            throw new ArgumentException("key cannot be null or whitespace", nameof(key));
        }

        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("value cannot be null or whitespace", nameof(value));
        }

        IDatabaseAsync executor = _transaction is not null ? _transaction : _database;
        return await executor.SetContainsAsync(key, value);
    }

    public async Task<IEnumerable<RelativeLocation>?> GetGeoLocationsInRadiusAsync(
        string key,
        double longitude,
        double latitude,
        double radius,
        DistanceUnit unit = DistanceUnit.Meters
    )
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            throw new ArgumentException("key cannot be null or whitespace", nameof(key));
        }

        if (longitude < -180 || longitude > 180)
        {
            throw new ArgumentException(
                "longitude must be between -180 and 180",
                nameof(longitude)
            );
        }

        if (latitude < -90 || latitude > 90)
        {
            throw new ArgumentException("latitude must be between -90 and 90", nameof(latitude));
        }

        if (radius <= 0)
        {
            throw new ArgumentException("radius must be greater than zero", nameof(radius));
        }

        GeoUnit geoUnit = unit switch
        {
            DistanceUnit.Meters => GeoUnit.Meters,
            DistanceUnit.Kilometers => GeoUnit.Kilometers,
            DistanceUnit.Miles => GeoUnit.Miles,
            _ => throw new ArgumentOutOfRangeException(nameof(unit), unit, null),
        };

        IDatabaseAsync executor = _transaction is not null ? _transaction : _database;
        var res = await executor.GeoRadiusAsync(
            key,
            longitude,
            latitude,
            radius,
            geoUnit,
            options: GeoRadiusOptions.WithCoordinates | GeoRadiusOptions.WithDistance
        );
        if (res.Length == 0)
        {
            return null;
        }

        return res.Select(c => new RelativeLocation
        {
            Identifier = c.Member.ToString(),
            Longitude = c.Position!.Value.Longitude,
            Latitude = c.Position.Value.Latitude,
            Distance = c.Distance!.Value,
        });
    }

    public async Task<IEnumerable<RelativeLocation>?> GetGeoLocationsInRadiusAsync(
        string key,
        string identifier,
        double radius,
        DistanceUnit unit = DistanceUnit.Meters
    )
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            throw new ArgumentException("key cannot be null or whitespace", nameof(key));
        }

        if (string.IsNullOrWhiteSpace(identifier))
        {
            throw new ArgumentException(
                "identifier cannot be null or whitespace",
                nameof(identifier)
            );
        }

        if (radius <= 0)
        {
            throw new ArgumentException("radius must be greater than zero", nameof(radius));
        }

        GeoUnit geoUnit = unit switch
        {
            DistanceUnit.Meters => GeoUnit.Meters,
            DistanceUnit.Kilometers => GeoUnit.Kilometers,
            DistanceUnit.Miles => GeoUnit.Miles,
            _ => throw new ArgumentOutOfRangeException(nameof(unit), unit, null),
        };

        IDatabaseAsync executor = _transaction is not null ? _transaction : _database;
        var res = await executor.GeoRadiusAsync(
            key,
            identifier,
            radius,
            geoUnit,
            options: GeoRadiusOptions.WithCoordinates | GeoRadiusOptions.WithDistance
        );

        if (res.Length == 0)
        {
            return null;
        }

        return res.Select(c => new RelativeLocation
        {
            Identifier = c.Member.ToString(),
            Longitude = c.Position!.Value.Longitude,
            Latitude = c.Position.Value.Latitude,
            Distance = c.Distance!.Value,
        });
    }

    public async Task<Location?> GetGeoLocationAsync(string key, string identifier)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            throw new ArgumentException("key cannot be null or whitespace", nameof(key));
        }

        if (string.IsNullOrWhiteSpace(identifier))
        {
            throw new ArgumentException(
                "identifier cannot be null or whitespace",
                nameof(identifier)
            );
        }

        IDatabaseAsync executor = _transaction is not null ? _transaction : _database;
        var res = await executor.GeoPositionAsync(key, identifier);
        return res is null
            ? null
            : new Location
            {
                Identifier = identifier,
                Longitude = res.Value.Longitude,
                Latitude = res.Value.Latitude,
            };
    }

    // public async Task<string?> BlockPopListStringAsync(string key, TimeSpan? timeout = null)
    // {
    //     if (string.IsNullOrWhiteSpace(key))
    //     {
    //         throw new ArgumentException("key cannot be null or whitespace", nameof(key));
    //     }

    //     if (timeout is not null && timeout <= TimeSpan.Zero)
    //     {
    //         throw new ArgumentException("timeout must be greater than zero", nameof(timeout));
    //     }

    //     IDatabaseAsync executor = _transaction is not null ? _transaction : _database;

    //     RedisResult res = await executor.ExecuteAsync(
    //         "BLPOP",
    //         key,
    //         timeout ?? TimeSpan.FromSeconds(0)
    //     );

    //     if (res.IsNull)
    //     {
    //         return null;
    //     }

    //     return res.ToString();
    // }

    // public Task<T?> BlockPopListObjectAsync<T>(string key, TimeSpan? timeout = null)
    // {
    //     throw new NotImplementedException();
    // }

    public async Task<bool> SetHashStringAsync(
        string key,
        string field,
        string value,
        bool @override = true
    )
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            throw new ArgumentException("key cannot be null or whitespace", nameof(key));
        }

        if (string.IsNullOrWhiteSpace(field))
        {
            throw new ArgumentException("field cannot be null or whitespace", nameof(field));
        }

        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("value cannot be null or whitespace", nameof(value));
        }

        IDatabaseAsync executor = _transaction is not null ? _transaction : _database;
        return await executor.HashSetAsync(
            key,
            field,
            value,
            @override ? When.Always : When.NotExists
        );
    }

    public async Task<bool> SetHashObjectAsync<T>(
        string key,
        string field,
        T value,
        bool @override = true
    )
    {
        ArgumentNullException.ThrowIfNull(key);
        return await SetHashStringAsync(key, field, JsonSerializer.Serialize(value), @override);
    }

    public async Task<string?> GetHashStringAsync(string key, string field)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            throw new ArgumentException("key cannot be null or whitespace", nameof(key));
        }

        if (string.IsNullOrWhiteSpace(field))
        {
            throw new ArgumentException("field cannot be null or whitespace", nameof(field));
        }

        IDatabaseAsync executor = _transaction is not null ? _transaction : _database;
        return await executor.HashGetAsync(key, field);
    }

    public async Task<T?> GetHashObjectAsync<T>(string key, string field)
    {
        var res = await GetHashStringAsync(key, field);
        return res is null ? default : JsonSerializer.Deserialize<T>(res);
    }

    public async Task<string?> LeftPopListStringAsync(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            throw new ArgumentException("key cannot be null or whitespace", nameof(key));
        }

        IDatabaseAsync executor = _transaction is not null ? _transaction : _database;
        return await executor.ListLeftPopAsync(key);
    }

    public async Task<T?> LeftPopListObjectAsync<T>(string key)
    {
        var res = await LeftPopListStringAsync(key);
        return res is null ? default : JsonSerializer.Deserialize<T>(res);
    }

    public async Task<bool> RemoveHashFieldAsync(string key, string field)
    {
        if (string.IsNullOrWhiteSpace(key)) {
            throw new ArgumentException("Key cannot be null or whitespace.", nameof(key));
        }

        if (string.IsNullOrWhiteSpace(field)) {
            throw new ArgumentException("Field cannot be null or whitespace.", nameof(field));
        }

        IDatabaseAsync executor = _transaction is not null ? _transaction : _database;
        return await executor.HashDeleteAsync(key, field);
    }
}
