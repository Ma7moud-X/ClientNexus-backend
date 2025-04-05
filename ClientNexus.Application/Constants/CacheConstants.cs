using System;

namespace ClientNexus.Application.Constants;

public static class CacheConstants
{
    // public const string Prefix = "clientnexus:";
    // public const string ServiceKeyPrefixTemplate = "clientnexus:services:{0}:";
    public const string MissedOffersKeyTemplate = "clientnexus:services:{0}:offersList";

    // public const string OffersHashKeyTemplate = "clientnexus:services:{0}:offersHash";
    public const string ServiceOfferPriceKeyTemplate = "clientnexus:services:{0}:offersPrices:{1}";
    public const string OffersChannelKeyTemplate = "clientnexus:services:{0}:offersChannel";
    public const string ServiceRequestKeyTemplate = "clientnexus:services:{0}:request";

    public const string AvailableForEmergencyServiceProvidersLocationsKey =
        "clientnexus:locations:serviceProviders:availableForEmergency";
}
