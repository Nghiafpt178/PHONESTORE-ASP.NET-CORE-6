using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MyRazorPage.Models;
using System.Security.Claims;
using System.Text.Json;

namespace MyRazorPage.Pages.Account
{
    public class SignInModel : PageModel
    {
        private readonly PRN221_DBContext db;

        public SignInModel(PRN221_DBContext db)
        {
            this.db = db;
        }

        [BindProperty]
        public Models.Account Account { get; set; }

        public IActionResult OnGet(string ReturnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                
                var role = HttpContext.User.Claims.FirstOrDefault(p => p.Type == ClaimTypes.Role).Value;
                if (role == "1")
                {
                    return RedirectToPage("/admin/dashboard/index");
                }
                else
                {
                    return RedirectToPage("/index");
                }

            }
            return Page();
        }
        public async Task<IActionResult> OnPost()
        {
            if (ModelState.IsValid)
            {

                var account = await db.Accounts.SingleOrDefaultAsync(p => p.Email.Equals(Account.Email) && p.Password.Equals(Account.Password));

                if (account != null)
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Email, account.Email),
                        new Claim(ClaimTypes.Role, Convert.ToString(account.Role)),
                    };
                    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var principal = new ClaimsPrincipal(identity);
                    await HttpContext.SignInAsync(principal);
                    HttpContext.Session.SetString("account", JsonSerializer.Serialize(account));
                    
                    if (account.Role == 1)
                    {                      
                        return RedirectToPage("/admin/dashboard/index");
                    }
                    else if (account.Role == 2)
                        return RedirectToPage("/index");
                    else
                        return Page();

                }
                else
                {
                    ViewData["msg"] = "This account does not exist";
                    return RedirectToPage("/account/signin");
                }
            }
            else
            {
                ViewData["msg"] = "Sign In fail!";
            }
            return Page();

        }

    }
}
