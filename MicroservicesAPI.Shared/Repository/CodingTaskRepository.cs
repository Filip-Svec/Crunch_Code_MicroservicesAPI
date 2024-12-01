using MicroservicesAPI.Shared.Entities;
using MongoDB.Driver;

namespace MicroservicesAPI.Shared.Repository;

public class CodingTaskRepository(IMongoCollection<CodingTask> codingTaskCollection) 
    : MongoRepository<CodingTask>(codingTaskCollection)
{
    public async Task<CodingTask> GetCodingTaskByNameAsync(string taskName)
    {
        var filter = _filterBuilder.Eq(codingTask => codingTask.TaskName, taskName);
        return await _dbCollection.Find(filter).FirstOrDefaultAsync();
    }
}
