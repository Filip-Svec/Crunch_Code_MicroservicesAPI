using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MicroservicesAPI.Shared.Entities;

public class JavaScriptTemplates : BaseEntity
{
    [BsonRepresentation(BsonType.ObjectId)]
    public string TaskId { get; set; }
    public string TemplateCode { get; set; }
    public string SolutionCode { get; set; }
    public string SolutionCodeDescription { get; set; }
}