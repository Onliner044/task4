using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using task4.Filters;
using task4.Models;
using task4.ViewModel;

namespace task4.Controllers {
    [Authorize]
    [TypeFilter(typeof(NonBlocked))]
    public class UsersController : Controller {
        private UsersContext db;

        public UsersController(UsersContext db) {
            this.db = db;
        }

        [HttpGet]
        public IActionResult Index() {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Delete(UserListViewModel userListViewModel) {
            userListViewModel.Users.Where(userView => userView.Checked).ToList().ForEach(async checkedUser => {
                User findedUser = db.Users.FirstOrDefault(user => user.Id == checkedUser.UserId);

                if (findedUser == null) {
                    RedirectToAction("Index", "Users");
                }

                db.Remove(findedUser);

                if (findedUser.Email == User.Identity.Name) {
                    await HttpContext.SignOutAsync();
                }
            });

            await db.SaveChangesAsync();
            return RedirectToAction("Index", "Users");
        }

        [HttpPost]
        public async Task<IActionResult> SetBlock(UserListViewModel userListViewModel, bool block) {
            userListViewModel.Users.Where(userView => userView.Checked).ToList().ForEach(checkedUser => {
                db.Users.Where(user => user.Id == checkedUser.UserId).ToList().ForEach(async checkeUser => {
                    checkeUser.IsBlocked = block;

                    if (checkeUser.Email == User.Identity.Name && block) {
                        await HttpContext.SignOutAsync();
                    }
                });
            });

            await db.SaveChangesAsync();
            return RedirectToAction("Index", "Users");
        }
    }
}