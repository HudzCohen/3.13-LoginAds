﻿using _3._13_LoginAds.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace _3._13_LoginAds.Web.Controllers
{
    public class AccountController : Controller
    {
        private string _connectionString = @"Data Source=.\sqlexpress; Initial Catalog=LoginAds; Integrated Security=True;";

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Login()
        {
            if (TempData["Message"] != null)
            {
                ViewBag.Message = (string)TempData["Message"];
            }
            return View();
        }

        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            var repo = new ListingRepository(_connectionString);
            var user = repo.Login(email, password);
            if (user == null)
            {
                TempData["Message"] = "Invalid Login!";
                return RedirectToAction("Login");
            }

            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Email, email)
            };

            HttpContext.SignInAsync(new ClaimsPrincipal(
                new ClaimsIdentity(claims, "Cookies", ClaimTypes.Email, "roles"))).Wait();


            return Redirect("/home/newad");
        }

        public IActionResult Logout()
        {
            HttpContext.SignOutAsync().Wait();
            return Redirect("/");
        }

        public IActionResult Signup()
        {
            if (TempData["InvalidSignup"] != null)
            {
                ViewBag.Message = (string)TempData["InvalidSignup"];
                return RedirectToAction("Signup");
            }
            return View();
        }

        [HttpPost]
        public IActionResult Signup(User user, string password)
        {
            var repo = new ListingRepository(_connectionString);
            List<string> emails = repo.GetAllEmails();
            bool usedEmail = emails.Contains(user.Email);
            if (!usedEmail)
            {
                repo.AddUser(user, password);
                return Redirect("/account/login");
            }
            else
            {
                TempData["InvalidSignup"] = "Email account unavailable";
                return RedirectToAction("Login");
            }

        }


    }
}
