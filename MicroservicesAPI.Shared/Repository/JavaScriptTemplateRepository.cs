using MicroservicesAPI.Shared.Entities;
using MongoDB.Driver;

namespace MicroservicesAPI.Shared.Repository;

public class JavaScriptTemplateRepository(IMongoCollection<JavaScriptTemplates> javaScriptTemplatesCollection) 
    : MongoRepository<JavaScriptTemplates>(javaScriptTemplatesCollection)
{
    public async Task<JavaScriptTemplates> GetJsTemplatesByTaskId(string taskId)
    {
        var filter = _filterBuilder.Eq(javaScriptTemplate => javaScriptTemplate.TaskId, taskId);
        return await _dbCollection.Find(filter).FirstOrDefaultAsync();
    }
}