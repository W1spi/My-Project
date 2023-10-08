using Data.NLimit.Common.EntitiesModels.SqlServer;
using System.ComponentModel;

namespace NLimit.WebApi.Repositoires.Users;
public interface IUserRepository
{
    Task<User?> CreateAsync(User user);
    Task<IEnumerable<User>> RetrieveAllAsync();
    Task<User?> RetrieveAsync(string id);
    Task<User?> UpdateUserAsync(string id, User user);

    //TODO: здесь наверн тоже нужно определить два метода по обновлению: профиля и почты
    Task<User?> UpdateProfileUserAsync(string id, string firstName, string surname, string? patronymic,
        DateTime? birthDate, string? mobilePhone, string? address);

    Task<User?> UpdateEmailAsync(string id, string newEmail);

    Task<bool?> DeleteAsync(string id);
}
