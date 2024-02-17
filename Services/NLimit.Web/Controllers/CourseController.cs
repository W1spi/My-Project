using Data.NLimit.Common.DataContext.SqlServer;
using Data.NLimit.Common.EntitiesModels.SqlServer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using NLimit.Web.Models;
using System;

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
        public async Task<IActionResult> Works(string? userId)
        {
            bool signUser = signInManager.IsSignedIn(User);

            if (!signUser)
            {
                return RedirectToAction("Login", "Identity");
            }

            IEnumerable<Work> allWorks = await GetWorks(userId);

            if (!allWorks.IsNullOrEmpty())
            {
                AboutWorkViewModel workModel = new()
                {
                    WorkIsPresent = true
                };

                return View(workModel);
            }
            else
            {
                AboutWorkViewModel workModel = new()
                {
                    WorkIsPresent = false
                };

                return View(workModel);
            }
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

        private async Task<IEnumerable<Work>> GetWorks(string? userId)
        {
            string uri;

            HttpClient client = clientFactory.CreateClient("NLimit.WebApi");

            if (string.IsNullOrEmpty(userId))
            {
                uri = "api/Works/GetAllWorks";
            }
            else
            {
                uri = $"api/Works/GetInfoAboutWork?id={userId}";
            }

            HttpRequestMessage request = new(
                method: HttpMethod.Get,
                requestUri: uri);

            HttpResponseMessage response = await client.SendAsync(request);

            IEnumerable<Work>? works = await response.Content.ReadFromJsonAsync<IEnumerable<Work>>();

            return works;
        }
    }
}
