using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace NLimit.Web.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
            Database.SetConnectionString(@"Data Source=(localdb)\MSSQLLocalDB;" +
                "Initial Catalog=master;" +
                "Integrated Security=true;" +
                "Trust Server Certificate=false;");
        }
    }
}