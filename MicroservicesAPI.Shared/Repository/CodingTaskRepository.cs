using MicroservicesAPI.Shared.Entities;
using MongoDB.Driver;

namespace MicroservicesAPI.Shared.Repository;

public class CodingTaskRepository(IMongoCollection<CodingTask> codingTaskCollection) 
    : MongoRepository<CodingTask>(codingTaskCollection)
{
    public async Task<CodingTask> GetCodingTaskByNameAsync(string taskName)
    {
        var filter = _filterBuilder.Eq(codingTask => codingTask.Name, taskName);
        return await _dbCollection.Find(filter).FirstOrDefaultAsync();
    }
    
    public async Task<List<string>> GetAllCodingTasksNames()
    {
        return await _dbCollection
            .Find(FilterDefinition<CodingTask>.Empty) // No filter, return all documents
            .Project(codingTask => codingTask.Name)  // Project only the 'Name' field
            .ToListAsync();   
    }
    
}
