using AMChat.Common.Constants;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace AMChat.Filters;

public class SwaggerUserIdFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        operation.Parameters ??= new List<OpenApiParameter>();

        operation.Parameters.Add(new OpenApiParameter()
        {
            Name = CustomHeaders.UserIdHeader,
            In = ParameterLocation.Header,
            Description = "Mock of authentication token",
            Required = false
        });
    }
}
