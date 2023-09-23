using Data.NLimit.Common.EntitiesModels.SqlServer;

namespace NLimit.WebApi.Repositoires.Works;

public interface IWorkRepository
{
    Task<Work?> CreateAsync(Work work);
    Task<IEnumerable<Work>> RetrieveAllAsync();
    Task<Work> RetrieveAsync(string id);
    Task<Work?> UpdateAsync(string id, Work user);
    Task<bool?> DeleteAsync(string id);
}
