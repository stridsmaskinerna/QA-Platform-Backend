namespace QAPlatformAPI.Filters;

using System.Collections.Generic;
using System.Text.Json;
using Domain.Constants;
using Domain.DTO.Header;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

public class CustomHeadersOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        if (operation.Responses.ContainsKey($"{StatusCodes.Status200OK}"))
        {
            ApplyHeadersWith200StatusCode(operation);
        }
    }

    private void ApplyHeadersWith200StatusCode(OpenApiOperation operation)
    {
        operation.Responses[$"{StatusCodes.Status200OK}"].Headers = new Dictionary<string, OpenApiHeader>()
        {
            {
                CustomHeaders.Pagination, PaginationHeaderSpec()
            }
        };
    }

    private OpenApiHeader PaginationHeaderSpec()
    {
        var example = new PaginationMetaDTO()
        {
            PageNr = 1,
            Limit = 20,
            TotalItemCount = 200
        };

        var serializedExample = JsonSerializer.Serialize(example, new JsonSerializerOptions()
        {
            WriteIndented = true
        });

        return new OpenApiHeader()
        {
            Description = "Pagination meta data",
            Schema = new OpenApiSchema
            {
                Type = "string",
                Example = OpenApiAnyFactory.CreateFromJson(serializedExample)
            }
        };
    }
}

