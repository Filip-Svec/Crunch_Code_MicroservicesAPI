using MicroservicesAPI.Shared.Entities;
using MongoDB.Bson;
using MongoDB.Driver;

namespace MicroservicesAPI.Shared.Repository;

public class PythonTemplateRepository(IMongoCollection<PythonTemplates> pythonTemplatesCollection) 
    : MongoRepository<PythonTemplates>(pythonTemplatesCollection)
{
    public async Task<PythonTemplates> GetPythonTemplatesByTaskId(ObjectId taskId)
    {
        var filter = _filterBuilder.Eq(pythonTemplate => pythonTemplate.TaskId, taskId.ToString());
        return await _dbCollection.Find(filter).FirstOrDefaultAsync();
    }
}