namespace QAPlatformAPI.Filters;

using System.Collections.Generic;
using System.Text.Json;
using Domain.Constants;
using Domain.DTO.Header;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.OpenApi.Models;
using Presentation.Controllers;
using Swashbuckle.AspNetCore.SwaggerGen;

public class CustomHeadersOperationFilter : IOperationFilter
{
    private static readonly HashSet<(string Controller, string Action)> TargetEndpoints =
    [
        (nameof(QuestionController).Replace("Controller", ""), nameof(QuestionController.GetQuestions)),
        (nameof(QuestionController).Replace("Controller", ""), nameof(QuestionController.GetPublicQuestions)),
        (nameof(SubjectController).Replace("Controller", ""), nameof(SubjectController.GetTeacherSubjectList))
    ];

    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        if (operation.Responses.ContainsKey($"{StatusCodes.Status200OK}") &&
            context.ApiDescription.ActionDescriptor is
            ControllerActionDescriptor actionDescriptor)
        {
            var controllerName = actionDescriptor.ControllerName;
            var actionName = actionDescriptor.ActionName;

            if (TargetEndpoints.Contains((controllerName, actionName)))
            {
                operation.Responses[$"{StatusCodes.Status200OK}"].Headers = new Dictionary<string, OpenApiHeader>()
                {
                    {
                        CustomHeaders.Pagination, PaginationHeaderSpec()
                    }
                };
            }
        }
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

