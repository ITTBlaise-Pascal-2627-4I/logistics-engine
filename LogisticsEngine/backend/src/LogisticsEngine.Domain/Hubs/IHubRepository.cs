using LogisticsEngine.Domain.Hubs.Entities;

namespace LogisticsEngine.Domain.Hubs;

public interface IHubRepository
{
    Hub? GetById(Guid id);
    IReadOnlyList<Hub> GetAll();
    void Add(Hub hub);
    bool Update(Hub hub);
    bool Delete(Guid id);
}
