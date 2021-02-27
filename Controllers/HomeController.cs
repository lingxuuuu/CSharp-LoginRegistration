using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using loginreg.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;

namespace loginreg.Controllers
{
    public class HomeController : Controller
    {
        private LoginContext DbContext;

        public HomeController(LoginContext context)
        {
            DbContext = context;
        }
        [HttpGet("")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [Route("register")]
        public IActionResult Register(User user)
        {
            if (ModelState.IsValid)
            {
                if (DbContext.User.Any(u => u.Email == user.Email))
                {
                    ModelState.AddModelError("Email", "Email is already in use! Please login.");
                    return View("Index");
                }
                else
                {
                    PasswordHasher<User> Hasher = new PasswordHasher<User>();
                    user.Password = Hasher.HashPassword(user, user.Password);
                    user.CreatedAt = DateTime.Now;
                    user.UpdatedAt = DateTime.Now;
                    DbContext.Add(user);
                    DbContext.SaveChanges();
                    HttpContext.Session.SetString("Username", user.FirstName);
                    System.Console.WriteLine("********************** Username ********************");
                    System.Console.WriteLine(user.FirstName);
                    return RedirectToAction("success");
                }
            }
            else
            {
                return View("Index");
            }

        }

        [HttpGet("login")]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [Route("login")]
        public IActionResult Login(Login userSubmission)
        {
            System.Console.WriteLine(ModelState.IsValid);
            if (ModelState.IsValid)
            {
                User userInDb = DbContext.User.FirstOrDefault(u => u.Email == userSubmission.Email);
                if (userInDb == null)
                {
                    ModelState.AddModelError("Email", "Invalid Email/Password");
                    return View("login");
                }
                PasswordHasher<Login> hasher = new PasswordHasher<Login>();

                PasswordVerificationResult result = hasher.VerifyHashedPassword(userSubmission, userInDb.Password, userSubmission.Password);

                if (result == 0)
                {
                    ModelState.AddModelError("Password", "Invalid Email/Password");
                    return View("login");
                }
                else
                {
                    HttpContext.Session.SetString("Username", userInDb.FirstName);
                    return RedirectToAction("Success");
                }
            }
            else
            {
                return View("login");
            }
        }

        [HttpGet("success")]
        public IActionResult Success()
        {
            ViewBag.Username = HttpContext.Session.GetString("Username");
            if (ViewBag.Username == null)
            {
                return RedirectToAction("Index");
            }
            return View();

        }

        [HttpGet("logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("login");
        }
    }
}
