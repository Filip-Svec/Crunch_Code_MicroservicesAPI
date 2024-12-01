using MicroservicesAPI.Shared.Entities;
using MongoDB.Driver;

namespace MicroservicesAPI.Shared.Repository;

public class PythonTemplateRepository(IMongoCollection<PythonTemplates> pythonTemplatesCollection) 
    : MongoRepository<PythonTemplates>(pythonTemplatesCollection)
{
    public async Task<PythonTemplates> GetPythonTemplatesByTaskId(string taskId)
    {
        var filter = _filterBuilder.Eq(pythonTemplate => pythonTemplate.TaskId, taskId);
        return await _dbCollection.Find(filter).FirstOrDefaultAsync();
    }
}