using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ClientNexus.API.Filters.AuthFilters
{
    public class OrAuthorizationFilter : IAuthorizationFilter
    {
        private readonly string[] _policies;

        public OrAuthorizationFilter(params string[] policies)
        {
            if (policies is null || policies.Length == 0)
            {
                throw new ArgumentException("You need to provide policies");
            }

            _policies = policies;
        }

        public async void OnAuthorization(AuthorizationFilterContext context)
        {
            var authService =
                context.HttpContext.RequestServices.GetRequiredService<IAuthorizationService>();

            foreach (var policy in _policies)
            {
                AuthorizationResult res = await authService.AuthorizeAsync(
                    context.HttpContext.User,
                    policy
                );
                if (res.Succeeded)
                {
                    return; // Authorized by at least one policy
                }
            }

            // Not authorized by any policy
            context.Result = new ForbidResult();
        }
    }
}
