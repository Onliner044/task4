using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Threading.Tasks;

namespace task4.Filters {
    public class NonAuthorize : Attribute, IAsyncActionFilter {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next) {
            if (context.HttpContext.User.Identity.IsAuthenticated) {
                context.HttpContext.Response.Redirect("/Users/Index");
            } else {
                await next();
            }
        }
    }
}
