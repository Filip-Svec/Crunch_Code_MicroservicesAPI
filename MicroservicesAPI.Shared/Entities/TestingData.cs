using MicroservicesAPI.Shared.Entities.TestDatasetNested;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MicroservicesAPI.Shared.Entities;

public class TestingData : BaseEntity
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string TaskId { get; set; }
    public string ExecutionMethod { get; set; }
    public List<TaskDataset> TaskDatasets { get; set; }
}