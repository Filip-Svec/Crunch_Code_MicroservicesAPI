using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MicroservicesAPI.Shared.Entities;

public abstract class BaseEntity
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public ObjectId Id { get; set; }
}