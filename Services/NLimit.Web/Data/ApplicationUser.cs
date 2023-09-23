using Microsoft.AspNetCore.Identity;

namespace NLimit.Web.Data;

public class ApplicationUser //: IdentityUser
{
    //public string CustomTag { get; set; }

    public string FirstName { get; set; }
    public string Surname { get; set; }
    public string? Patronymic { get; set; }
    public string Email { get; set; }
    public string? Address { get; set; }
    public DateTime? BirthDate { get; set; }
    public string MobilePhone { get; set; }
    public bool EmailConfirmed { get; set; }
}
