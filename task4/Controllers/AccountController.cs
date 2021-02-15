using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using task4.Filters;
using task4.Models;
using task4.ViewModel;

namespace task4.Controllers {

    public class AccountController : Controller {
        private UsersContext db;

        public AccountController(UsersContext context) {
            db = context;
        }

        private async Task Authenticate(string name) {
            var claims = new List<Claim> {
                new Claim(ClaimsIdentity.DefaultNameClaimType, name),
            };
            ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
        }

        [HttpPost]
        [NonAuthorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model) {
            if (ModelState.IsValid) {
                User user = db.Users.FirstOrDefault(user => user.Email == model.Email && user.Password == model.Password);

                if (user != null) {
                    await Authenticate(user.Email);

                    user.LastLoginDate = DateTime.Now;
                    await db.SaveChangesAsync();

                    return RedirectToAction("Index", "Users");
                }

                ModelState.AddModelError("", "Wrong login or password");
            }

            return View(model);
        }

        [HttpGet]
        [NonAuthorize]
        public IActionResult Login() {
            return View();
        }

        [HttpPost]
        [NonAuthorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model) {
            if (User.Identity.IsAuthenticated) {
                return RedirectToAction("Index", "Users");
            }

            if (ModelState.IsValid) {
                User user = db.Users.FirstOrDefault(user => user.Email == model.Email);

                if (user == null) {
                    db.Users.Add(new User() {
                        Login = model.Login,
                        Email = model.Email,
                        Password = model.Password,
                        LastLoginDate = DateTime.Now,
                        RegistrationDate = DateTime.Now,
                        IsBlocked = false,
                    });

                    await db.SaveChangesAsync();
                    await Authenticate(model.Email);
                    return RedirectToAction("Index", "Users");
                } else {
                    ModelState.AddModelError("", "The email already exists");
                }
            }

            return View(model);
        }

        [HttpGet]
        [NonAuthorize]
        public IActionResult Register() {
            if (User.Identity.IsAuthenticated) {
                return RedirectToAction("Index", "Users");
            }

            return View();
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> LogOut() {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }
    }
}