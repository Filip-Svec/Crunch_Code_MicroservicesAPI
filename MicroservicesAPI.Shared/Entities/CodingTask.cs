using MicroservicesAPI.Shared.Entities.CodingTaskNested;

namespace MicroservicesAPI.Shared.Entities;

public class CodingTask : BaseEntity
{
    public string TaskName { get; set; }
    public string Description { get; set; }
    public List<Example> Examples { get; set; }
    public List<string> Constraints { get; set; }
    public List<string> Hints { get; set; }
    
}