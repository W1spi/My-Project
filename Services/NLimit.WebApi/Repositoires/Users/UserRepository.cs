using Data.NLimit.Common.DataContext.SqlServer;
using Data.NLimit.Common.EntitiesModels.SqlServer;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace NLimit.WebApi.Repositoires.Users;

public class UserRepository : IUserRepository
{
    private static ConcurrentDictionary<string, User?> usersCache;
    private static ConcurrentDictionary<string, Work?> worksCache;

    private NLimitContext db;

    private readonly ILogger<UserRepository> logger;

    public UserRepository(NLimitContext injectedContext, ILogger<UserRepository> injectedLogger)
    {
        db = injectedContext;
        logger = injectedLogger;


        if (usersCache is null)
        {
            usersCache = new ConcurrentDictionary<string, User?>(db.Users.ToDictionary(s => s.UserId));
        }
        if (worksCache is null)
        {
            worksCache = new ConcurrentDictionary<string, Work?>(db.Works.ToDictionary(s => s.WorkId));
        }
    }

    public async Task<User?> CreateAsync(User user)
    {
        //user.UserId = user.UserId.ToUpper();


        EntityEntry<User> added = await db.Users.AddAsync(user);
        int affected = await db.SaveChangesAsync();

        if (affected == 1)
        {
            if (usersCache is null)
            {
                return user;
            }

            return usersCache.AddOrUpdate(user.UserId, user, UpdateCache!);
        }
        else
        {
            return null;
        }
    }

    public Task<IEnumerable<User>> RetrieveAllAsync()
    {
        // извлекаем из кэша в целях производительности
        return Task.FromResult(usersCache is null
            ? Enumerable.Empty<User>() : usersCache.Values!);
    }

    public Task<User?> RetrieveAsync(string id)
    {
        if (usersCache is null)
        {
            return null!;
        }

        usersCache.TryGetValue(id, out User? user);
        return Task.FromResult(user);
    }

    private User UpdateCache(string id, User user)
    {
        User? old;
        if (usersCache is not null)
        {
            if (usersCache.TryGetValue(id, out old))
            {
                if (usersCache.TryUpdate(id, user, old))
                {
                    return user;
                }
            }
        }
        return null!;
    }

    public async Task<User?> UpdateUserAsync(string id, User user)
    {
        db.Users.Update(user);
        int affected = default;

        try
        {
            affected = await db.SaveChangesAsync();

            if (affected == 1)
            {
                return UpdateCache(id, user);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }

        return null;
    }

    public async Task<User?> UpdateProfileUserAsync(string id, string firstName, string surname, string? patronymic,
        DateTime? birthDate, string? mobilePhone, string? address)
    {
        User? user = await db.Users.FindAsync(id);

        user.FirstName = firstName;
        user.Surname = surname;
        user.Patronymic = patronymic;
        user.BirthDate = birthDate;
        user.MobilePhone = mobilePhone;
        user.Address = address;

        db.Users.Update(user);

        int affected = await db.SaveChangesAsync();

        if (affected == 1)
        {
            return UpdateCache(id, user);
        }
        else
        {
            return null!;
        }
    }

    public async Task<User?> UpdateEmailAsync(string id, string newEmail)
    {
        User? user = await db.Users.FindAsync(id);

        user.Email = newEmail;

        db.Users.Update(user);

        int affected = await db.SaveChangesAsync();

        if (affected == 1)
        {
            return UpdateCache(id, user);
        }
        else
        {
            return null!;
        }
    }

    public async Task<bool?> DeleteAsync(string id)
    {
        User? user = db.Users.Find(id);
        if (user is null)
        {
            return null;
        }

        db.Users.Remove(user);
        int affected = await db.SaveChangesAsync();

        if (affected == 1)
        {
            if (usersCache is null)
            {
                return null;
            }
            return usersCache.TryRemove(id, out user);
        }
        else
        {
            return null;
        }
    }
}
