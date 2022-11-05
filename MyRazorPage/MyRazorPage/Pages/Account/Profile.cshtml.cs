using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyRazorPage.Models;
using System.Text.Json;

namespace MyRazorPage.Pages.Account
{
    public class ProfileModel : PageModel
    {

        private readonly PRN221_DBContext db;

        public ProfileModel(PRN221_DBContext db)
        {
            this.db = db;
        }
        [BindProperty]
        public Customer Customer { get; set; }

        [BindProperty]
        public Models.Account Account { get; set; }
        public void OnGet()
        {
            if (HttpContext.Session.GetString("account") != null)
            {
                var acc = JsonSerializer.Deserialize<Models.Account>(HttpContext.Session.GetString("account"));
                Customer = db.Customers.SingleOrDefault(x => x.CustomerId == acc.CustomerId);

            }
        }

        public async Task<IActionResult> OnPost(Customer customer)
        {
            if (HttpContext.Session.GetString("account") != null)
            {
                var acc = JsonSerializer.Deserialize<Models.Account>(HttpContext.Session.GetString("account"));
                Customer = db.Customers.SingleOrDefault(x => x.CustomerId == acc.CustomerId);
                Account = db.Accounts.SingleOrDefault(a => a.CustomerId == acc.CustomerId);
                Customer.CompanyName = customer.CompanyName;
                Customer.ContactName = customer.ContactName;
                Customer.ContactTitle = customer.ContactTitle;
                Customer.Address = customer.Address;
                Account.Email = Request.Form["Email"];
                db.Accounts.Update(Account);
                db.Customers.Update(Customer);
                await db.SaveChangesAsync();
                TempData["mess"] = "Update successful";
                return RedirectToPage("/account/profile");
            }
            return Page();
        }
    }
}
