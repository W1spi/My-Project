using Data.NLimit.Common.EntitiesModels.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.NLimit.Common.DataContext.Sqlite
{
    public static class NLimitContextExtensions
    {
        /// <summary>
        /// Adds NLimitContext to the specified IserviceCollection. Uses the Sqlite database provider.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="relativePath">Set to override the default of ".."</param>
        /// <returns>An IServiceCollection that can be used to add more services.</returns>

        // метод расширения для добавления контекста БД NLimit в коллекцию сервисов зависимостей

        public static IServiceCollection AddNLimitContext(this IServiceCollection services, string relativePath = "..")
        {
            string dbPath = @"C:\Users\stepa\source\repos\CognitionVer3\Data\DbNLimit\NLimit.db"; // Path.Combine(relativePath, "NLimit.db"); //
            services.AddDbContext<NlimitContext>(options => options.UseSqlite($"Data Source={dbPath}"));

            return services;
        }
    }
}
