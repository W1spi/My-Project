using Data.NLimit.Common.DataContext.SqlServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace NLimit.Common.DataContext.SqlServer;

public static class NLimitContextExtensions
{
    public static IServiceCollection AddNLimitContext(this IServiceCollection services)
    {
        string connectionString = @"Data Source=.;" +
            "Initial Catalog=NLimit;" +
            "Integrated Security=true;" +
            "Trust Server Certificate=true;" +
            "MultipleActiveResultSets=true;";

        services.AddDbContext<NLimitContext>(options => options.UseSqlServer(connectionString));

        return services;
    }
}
