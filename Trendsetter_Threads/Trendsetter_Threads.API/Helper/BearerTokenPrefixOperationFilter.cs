using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Trendsetter_Threads.API.Helper;
public class BearerTokenPrefixOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        if (operation.Security == null)
            operation.Security = new List<OpenApiSecurityRequirement>();

        var bearerScheme = new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                Array.Empty<string>()
            }
        };

        operation.Security.Add(bearerScheme);

        var bearerAuthParameter = operation.Parameters.FirstOrDefault(p => p.Name == "Authorization" && p.In == ParameterLocation.Header);
        if (bearerAuthParameter != null)
        {
            bearerAuthParameter.Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\"";
            bearerAuthParameter.Schema.Default = new Microsoft.OpenApi.Any.OpenApiString("Bearer ");
        }
    }
}
