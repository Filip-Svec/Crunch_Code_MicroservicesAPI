using MicroservicesAPI.Shared.Entities;

namespace MicroservicesAPI.Shared.Repository.Interfaces;

public interface IRepository<T> where T : BaseEntity
{
    Task<T> GetByIdAsync(string id);
}