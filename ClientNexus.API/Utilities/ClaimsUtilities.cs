using System.Security.Claims;
using ClientNexus.Domain.Enums;

namespace ClientNexus.API.Utilities
{
    public static class ClaimsUtilities
    {
        public static UserType? GetRole(this ClaimsPrincipal userClaims)
        {
            var res = userClaims.Claims.SingleOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            return res is null
                ? null
                : res switch
                {
                    "Client" => UserType.Client,
                    "ServiceProvider" => UserType.ServiceProvider,
                    "Admin" => UserType.Admin,
                    _ => null,
                };
        }

        public static int? GetId(this ClaimsPrincipal userClaims)
        {
            var res = userClaims
                .Claims.SingleOrDefault(c => c.Type == ClaimTypes.NameIdentifier)
                ?.Value;

            if (res is null)
            {
                return null;
            }

            bool parsed = int.TryParse(res, out var id);
            if (!parsed)
            {
                return null;
            }

            return id;
        }
    }
}
