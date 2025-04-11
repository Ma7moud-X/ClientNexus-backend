using ClientNexus.Application.Interfaces;
using ClientNexus.Application.Services;

namespace ClientNexus.API.Extensions
{
    public static class OfferListenerExtensions
    {
        public static void AddOfferListenerServices(this IServiceCollection services)
        {
            services.AddTransient<IChannelOfferListenerService, ChannelOfferListenerService>();
            services.AddTransient<IMissedOfferGetterService, MissedOfferGetterService>();
            services.AddTransient<IGeneralOfferListenerService, GeneralOfferListenerService>();
        }
    }
}
