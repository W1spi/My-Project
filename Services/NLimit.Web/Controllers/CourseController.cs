using Data.NLimit.Common.DataContext.SqlServer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace NLimit.Web.Controllers
{
    public class CourseController : Controller
    {
        private readonly IHttpClientFactory clientFactory;
        private readonly NLimitContext db;
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly UserManager<IdentityUser> userManager;

        public CourseController(NLimitContext injectedContext, SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager, IHttpClientFactory httpClientFactory)
        {
            clientFactory = httpClientFactory;
            db = injectedContext;
            this.signInManager = signInManager;
            this.userManager = userManager;
        }

        [HttpGet]
        public IActionResult MyCourses()
        {
            bool signUser = signInManager.IsSignedIn(User);

            if (!signUser)
            {
                return RedirectToAction("Login", "Identity");
            }

            return View();
        }

        [HttpGet]
        public IActionResult AllCourses()
        {
            bool signUser = signInManager.IsSignedIn(User);

            if (!signUser)
            {
                return RedirectToAction("Login", "Identity");
            }

            return View();
        }

        [HttpGet]
        public IActionResult Works()
        {
            bool signUser = signInManager.IsSignedIn(User);

            if (!signUser)
            {
                return RedirectToAction("Login", "Identity");
            }

            return View();
        }

        [HttpGet]
        public IActionResult Attestations()
        {
            bool signUser = signInManager.IsSignedIn(User);

            if (!signUser)
            {
                return RedirectToAction("Login", "Identity");
            }

            return View();
        }

        [HttpGet]
        public IActionResult Calendar()
        {
            bool signUser = signInManager.IsSignedIn(User);

            if (!signUser)
            {
                return RedirectToAction("Login", "Identity");
            }

            return View();
        }

        [HttpGet]
        public IActionResult AboutCourse()
        {
            bool signUser = signInManager.IsSignedIn(User);

            if (!signUser)
            {
                return RedirectToAction("Login", "Identity");
            }

            return View();
        }
    }
}
