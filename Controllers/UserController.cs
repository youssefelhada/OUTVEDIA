using final_project_depi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace final_project_depi.Controllers
{
    [Authorize(Roles = "admin")]

    [Route("/Admin/[controller]/{action=Index}/{id?}")]
    [Route("Users/{action=Index}/{id?}")]
    public class UserController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly int pageSize = 5;

        public UserController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {

            this.userManager = userManager;
            this.roleManager = roleManager;

        }
        public IActionResult Index(int? pageIndex)
        {

            IQueryable<ApplicationUser> query = userManager.Users.OrderByDescending(u => u.CreatedAt);

            if (pageIndex == null || pageIndex < 1)
            {
                pageIndex = 1;
            }
            decimal count = query.Count();
            int totalPages = (int)Math.Ceiling(count / pageSize);
            query = query.Skip(((int)pageIndex - 1) * pageSize).Take(pageSize);

            var users = query.ToList();
            ViewBag.PageIndex = pageIndex;
            ViewBag.TotalPages = totalPages;

            return View("~/Views/Users/Index.cshtml", users);
        }

        public async Task<IActionResult> Details(string? id)
        {
            if (id == null)
            {

                return RedirectToAction("Index", "Home");


            }
            var appUser = await userManager.FindByIdAsync(id);
            if (appUser == null)
            {
                return RedirectToAction("Index", "Home");
            }
            ViewBag.Roles = await userManager.GetRolesAsync(appUser);


            return View("~/Views/Users/Details.cshtml", appUser);
        }


        public async Task<IActionResult> DeleteAccount(string? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index", "Home");
            }
            var appUser = await userManager.FindByIdAsync(id);
            if (appUser == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var currentUser = await userManager.GetUserAsync(User);
            if (currentUser!.Id == appUser.Id)
            {
                TempData["ErrorMessage"] = "You cannot delete your own account.";
                return RedirectToAction("Details", "User", new { id });
            }

            var result = await userManager.DeleteAsync(appUser);
            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "User deleted successfully.";
                return RedirectToAction("Index", "User");
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to delete user.";
                return RedirectToAction("Details", "User", new { id });

            }
        }
    }
}