using _3._13_LoginAds.Data;
using _3._13_LoginAds.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace _3._13_LoginAds.Web.Controllers
{
    public class HomeController : Controller
    {
        private string _connectionString = @"Data Source=.\sqlexpress; Initial Catalog=LoginAds; Integrated Security=True;";

        public IActionResult Index()
        {
            IndexViewModel vm = new();
            var repo = new ListingRepository(_connectionString);

            if (User.Identity.IsAuthenticated)
            {
                var user = repo.GetByEmail(User.Identity.Name);
                vm.Message = $"Welcome back, {user.Name}!";
                vm.User = user;
            }
            else
            {
                vm.Message = "Welcome, Guest";
            }

            vm.Ads = repo.GetAds();

            return View(vm);
        }

        [Authorize]
        public IActionResult NewAd()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        public IActionResult NewAd(Ad ad)
        {
            var repo = new ListingRepository(_connectionString);
            var user = repo.GetByEmail(User.Identity.Name);
            ad.UserId = user.Id;
            repo.AddAd(ad);
            ad.Date = DateTime.Now;
            return Redirect("/home/index");
        }

        [Authorize]
        public IActionResult MyAccount()
        {
            var repo = new ListingRepository(_connectionString);
            var user = repo.GetByEmail(User.Identity.Name);

            MyAccountViewModel vm = new();
            vm.Ads = repo.GetAdsByUserId(user.Id);
            return View(vm);
        }

        [HttpPost]
        [Authorize]
        public IActionResult DeleteAd(int id)
        {
            var repo = new ListingRepository(_connectionString);
            repo.DeleteAdById(id);
            return Redirect("/");
        }

        
    }
}
