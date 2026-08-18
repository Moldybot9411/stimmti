using Backend.Models.Enums;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Backend.Filters;

public class IncludeEnumDocumentFilter : IDocumentFilter
{
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        context.SchemaGenerator.GenerateSchema(typeof(BodyProfileEnum), context.SchemaRepository);
        context.SchemaGenerator.GenerateSchema(typeof(ColorProfileEnum), context.SchemaRepository);
        context.SchemaGenerator.GenerateSchema(typeof(FaceProfileEnum), context.SchemaRepository);
        context.SchemaGenerator.GenerateSchema(typeof(HatProfileEnum), context.SchemaRepository);
    }
}