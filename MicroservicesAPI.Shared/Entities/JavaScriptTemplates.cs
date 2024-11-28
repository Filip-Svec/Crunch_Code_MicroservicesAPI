using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MicroservicesAPI.Shared.Entities;

public class JavaScriptTemplates : BaseEntity
{
    [BsonRepresentation(BsonType.ObjectId)]
    public string TaskId { get; set; }
    public string Template { get; set; }
    public string Solution { get; set; }
    public string SolutionDescription { get; set; }
}