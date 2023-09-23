using Data.NLimit.Common.DataContext.SqlServer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NLimit.Web.Areas.MvcPages.Controllers;
using NLimit.Web.Models;
using System.Diagnostics;

namespace NLimit.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHttpClientFactory clientFactory;
        private readonly NLimitContext db;
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly UserManager<IdentityUser> userManager;
        private readonly ILogger<PersonalDataController> logger;

        public HomeController(NLimitContext injectedContext, SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager, IHttpClientFactory httpClientFactory, ILogger<PersonalDataController> logger)
        {
            clientFactory = httpClientFactory;
            db = injectedContext;
            this.signInManager = signInManager;
            this.userManager = userManager;
            this.logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpGet]
        public async Task<IActionResult> CourseSelection()
        {
            //var identityUser = await userManager.GetUserAsync(User);
            bool signUser = signInManager.IsSignedIn(User);

            if (!signUser)
            {
                return RedirectToAction("Login", "Identity");
            }

            //return View("~/Areas/Courses/Views/AllCourses/Courses.cshtml");
            return View();
        }
    }
}