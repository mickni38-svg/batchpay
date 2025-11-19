using System.Collections.Generic;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace BatchPay.Api.Swagger
{
    /// Tilføjer en X-UserId header i alle endpoints i Swagger UI.
    public class XUserIdHeaderOperationFilter : IOperationFilter
    {
        public void Apply( OpenApiOperation operation, OperationFilterContext context )
        {
            if (operation.Parameters == null)
                operation.Parameters = new List<OpenApiParameter>();

            operation.Parameters.Add( new OpenApiParameter
            {
                Name = "X-UserId",
                In = ParameterLocation.Header,
                Required = false,
                Description = "Midlertidigt bruger-id (int) for demo",
                Schema = new OpenApiSchema { Type = "string" }
            } );
        }
    }
}
