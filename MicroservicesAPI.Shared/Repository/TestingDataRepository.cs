using MicroservicesAPI.Shared.Entities;
using MongoDB.Driver;

namespace MicroservicesAPI.Shared.Repository;

public class TestingDataRepository(IMongoCollection<TestingData> testingDataCollection)
    : MongoRepository<TestingData>(testingDataCollection)
{
    public async Task<TestingData> GetTestingDataByTaskIdAsync(string taskId)
    {
        var filter = _filterBuilder.Eq(testingData => testingData.TaskId, taskId);
        return await _dbCollection.Find(filter).FirstOrDefaultAsync();
    }
}