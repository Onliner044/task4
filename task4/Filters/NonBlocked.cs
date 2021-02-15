using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Linq;
using System.Threading.Tasks;
using task4.Models;

namespace task4.Filters {
    public class NonBlocked : Attribute, IAsyncActionFilter {
        public UsersContext db { get; }

        public NonBlocked(UsersContext db) {
            this.db = db;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next) {
            var user = db.Users.Where(user => user.Email == context.HttpContext.User.Identity.Name && user.IsBlocked).FirstOrDefault();

            if (user != null) {
                user.IsBlocked = false;
                await db.SaveChangesAsync();

                await context.HttpContext.SignOutAsync();
                context.HttpContext.Response.Redirect("/Account/Login");
                return;
            }

            await next();
        }
    }
}
