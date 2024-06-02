using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace NLimit.Web.Models;

public class ApplicationUser //: IdentityUser
{
    public string UserId { get; set; }
    public string FirstName { get; set; }

    public string Surname { get; set; }

    public string? Patronymic { get; set; }

    public DateTime BirthDate { get; set; }

    public DateTime StartDate { get; set; }

    public string? Address { get; set; }
    public string Email { get; set; }

    public string? MobilePhone { get; set; }

    public string? AdditionalPhone { get; set; }
}
