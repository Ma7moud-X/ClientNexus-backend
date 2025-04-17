using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ClientNexus.API.Utilities.SwaggerUtilities
{
    public class AuthorizeOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var hasAuthorize = context
                .MethodInfo.DeclaringType.GetCustomAttributes(true)
                .Union(context.MethodInfo.GetCustomAttributes(true))
                .OfType<AuthorizeAttribute>()
                .Any();

            if (hasAuthorize)
            {
                operation.Security = new List<OpenApiSecurityRequirement>
                {
                    new OpenApiSecurityRequirement
                    {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "Bearer",
                                },
                            },
                            new List<string>()
                        },
                    },
                };

                // Optionally, add role/policy information in the description
                var authorizeAttributes = context
                    .MethodInfo.DeclaringType.GetCustomAttributes(true)
                    .Union(context.MethodInfo.GetCustomAttributes(true))
                    .OfType<AuthorizeAttribute>();

                var roles = authorizeAttributes
                    .SelectMany(attr => attr.Roles?.Split(',') ?? Array.Empty<string>())
                    .Distinct();
                var policies = authorizeAttributes
                    .Select(attr => attr.Policy)
                    .Where(p => !string.IsNullOrEmpty(p))
                    .Distinct();

                if (roles.Any() || policies.Any())
                {
                    operation.Description += "<br/><b>Authorization Requirements:</b><ul>";
                    if (roles.Any())
                    {
                        operation.Description += $"<li>Roles: {string.Join(", ", roles)}</li>";
                    }
                    if (policies.Any())
                    {
                        operation.Description +=
                            $"<li>Policies: {string.Join(", ", policies)}</li>";
                    }
                    operation.Description += "</ul>";
                }
            }
        }
    }
}
