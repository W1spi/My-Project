using Microsoft.AspNetCore.Mvc.Rendering;

namespace NLimit.Web.ClassServices;

public static class ManageNavigationPages
{
    // Профиль
    public static string Profile => "Profile";

    public static string Email => "Email";

    public static string ChangePassword => "ChangePassword";

    public static string DownloadPersonalData => "DownloadPersonalData";

    public static string DeletePersonalData => "DeletePersonalData";

    public static string ExternalLogins => "ExternalLogins";

    public static string PersonalData => "PersonalData";

    public static string ProfileNavClass(ViewContext viewContext) => PageNavigationClass(viewContext, Profile);

    public static string EmailNavClass(ViewContext viewContext) => PageNavigationClass(viewContext, Email);

    public static string ChangePasswordNavClass(ViewContext viewContext) => PageNavigationClass(viewContext, ChangePassword);

    public static string DownloadPersonalDataNavClass(ViewContext viewContext) => PageNavigationClass(viewContext, DownloadPersonalData);

    public static string DeletePersonalDataNavClass(ViewContext viewContext) => PageNavigationClass(viewContext, DeletePersonalData);

    public static string ExternalLoginsNavClass(ViewContext viewContext) => PageNavigationClass(viewContext, ExternalLogins);

    public static string PersonalDataNavClass(ViewContext viewContext) => PageNavigationClass(viewContext, PersonalData);

    // Курсы
    public static string MyCourses => "MyCourses";
    public static string AllCourses => "AllCourses";
    public static string Works => "Works";
    public static string Attestations => "Attestations";
    public static string Calendar => "Calendar";
    public static string AboutCourse => "AboutCourse";

    public static string MyCoursesNavClass(ViewContext viewContext) => PageNavigationClass(viewContext, MyCourses);
    public static string AllCoursesNavClass(ViewContext viewContext) => PageNavigationClass(viewContext, AllCourses);
    public static string TasksNavClass(ViewContext viewContext) => PageNavigationClass(viewContext, Works);
    public static string AttestationsNavClass(ViewContext viewContext) => PageNavigationClass(viewContext, Attestations);
    public static string CalendarNavClass(ViewContext viewContext) => PageNavigationClass(viewContext, Calendar);
    public static string AboutCourseNavClass(ViewContext viewContext) => PageNavigationClass(viewContext, AboutCourse);


    public static string PageNavigationClass (ViewContext viewContext, string page)
    {
        var activePage = viewContext.ViewData["ActivePage"] as string
            ?? Path.GetFileNameWithoutExtension(viewContext.ActionDescriptor.DisplayName);

        return string.Equals(activePage, page, StringComparison.OrdinalIgnoreCase) ? "active" : null!;
    }
}
