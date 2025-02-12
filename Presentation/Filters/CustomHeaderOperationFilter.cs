using System.Text.Json;
using Domain.Constants;
using Domain.DTO.Header;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Presentation.Filters;

public class CustomHeadersOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        if (!operation.Responses.TryGetValue("200", out OpenApiResponse? res)) { return; }

        res.Headers ??= new Dictionary<string, OpenApiHeader>();
        res.Headers[CustomHeaders.Pagination] = PaginationHeaderSpec();
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

