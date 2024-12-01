using MicroservicesAPI.Shared.Entities;
using MongoDB.Bson;
using MongoDB.Driver;

namespace MicroservicesAPI.Shared.Repository;

public class JavaScriptTemplateRepository(IMongoCollection<JavaScriptTemplates> javaScriptTemplatesCollection) 
    : MongoRepository<JavaScriptTemplates>(javaScriptTemplatesCollection)
{
    public async Task<JavaScriptTemplates> GetJsTemplatesByTaskId(ObjectId taskId)
    {
        var filter = _filterBuilder.Eq(javaScriptTemplate => javaScriptTemplate.TaskId, taskId.ToString());
        return await _dbCollection.Find(filter).FirstOrDefaultAsync();
    }
}