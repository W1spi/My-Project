using Data.NLimit.Common.DataContext.SqlServer;
using Data.NLimit.Common.EntitiesModels.SqlServer;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using NLimit.WebApi.Repositoires.Users;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace NLimit.WebApi.Repositoires.Works;
public class WorkRepository : IWorkRepository
{
    private static ConcurrentDictionary<string, Work?> worksCache;

    private NLimitContext db;

    private readonly ILogger<UserRepository> logger;

    public WorkRepository(NLimitContext injectedContext, ILogger<UserRepository> injectedLogger)
    {
        db = injectedContext;
        logger = injectedLogger;


        if (worksCache is null)
        {
            worksCache = new ConcurrentDictionary<string, Work?>(db.Works.ToDictionary(s => s.WorkId));
        }
    }

    public async Task<Work?> CreateAsync(Work work)
    {
        //user.UserId = user.UserId.ToUpper();

        EntityEntry<Work> added = await db.Works.AddAsync(work);
        int affected = await db.SaveChangesAsync();

        if (affected == 1)
        {
            if (worksCache is null)
            {
                return work;
            }

            return worksCache.AddOrUpdate(work.WorkId, work, UpdateCache!);
        }
        else
        {
            return null;
        }
    }

    public Task<IEnumerable<Work>> RetrieveAllAsync()
    {
        // извлекаем из кэша в целях производительности
        return Task.FromResult(worksCache is null
            ? Enumerable.Empty<Work>() : worksCache.Values!);
    }

    public Task<Work?> RetrieveAsync(string id)
    {
        //id = id.ToUpper();

        /*User? user = db.Users
        .Where(s => s.UserId == id)
        .SingleOrDefault();

        return Task.FromResult(user); */

        if (worksCache is null)
        {
            return null!;
        }

        worksCache.TryGetValue(id, out Work? work);
        return Task.FromResult(work);
    }

    private Work UpdateCache(string id, Work work)
    {
        Work? old;
        if (worksCache is not null)
        {
            if (worksCache.TryGetValue(id, out old))
            {
                if (worksCache.TryUpdate(id, work, old))
                {
                    return work;
                }
            }
        }
        return null!;
    }

    public async Task<Work?> UpdateAsync(string id, Work work)
    {
        db.Works.Update(work);
        int affected = default;

        try
        {
            affected = await db.SaveChangesAsync();

            if (affected == 1)
            {
                return UpdateCache(id, work);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }

        return null;
    }

    public async Task<bool?> DeleteAsync(string id)
    {
        //id = id.ToUpper();

        Work? work = db.Works.Find(id);
        if (work is null)
        {
            return null;
        }

        db.Works.Remove(work);
        int affected = await db.SaveChangesAsync();

        if (affected == 1)
        {
            if (worksCache is null)
            {
                return null;
            }
            return worksCache.TryRemove(id, out work);
        }
        else
        {
            return null;
        }
    }
}

