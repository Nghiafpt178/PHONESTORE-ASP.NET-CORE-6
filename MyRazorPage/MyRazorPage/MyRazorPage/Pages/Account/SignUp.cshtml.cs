using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MyRazorPage.Models;
using System;
using System.Text;

namespace MyRazorPage.Pages.Account
{
    public class SignUpModel : PageModel
    {
        private readonly PRN221_DBContext dBContext;

        public SignUpModel(PRN221_DBContext dBContext)
        {
            this.dBContext = dBContext;
        }
        [BindProperty]
        public Customer Customer { get; set; }

        [BindProperty]
        public Models.Account Account { get; set; }

        public static string RandomCID(int length)
        {
            StringBuilder str_build = new StringBuilder();
            Random random = new Random();
            char letter;

            for (int i = 0; i < length; i++)
            {
                double flt = random.NextDouble();
                int shift = Convert.ToInt32(Math.Floor(25 * flt));
                letter = Convert.ToChar(shift + 65);
                str_build.Append(letter);
            }
            return str_build.ToString();
        }
        public void OnGet()
        {

        }

        public async Task<IActionResult> OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            else
            {
                var acc = await dBContext.Accounts.SingleOrDefaultAsync(a => a.Email.Equals(Account.Email));
                if (acc == null)
                {
                    var cust = new Customer()
                    {
                        CustomerId = RandomCID(5),
                        CompanyName = Customer.CompanyName,
                        ContactName = Customer.ContactName,
                        ContactTitle = Customer.ContactTitle,
                        Address = Customer.Address
                    };

                    var newAcc = new Models.Account()
                    {
                        Email = Account.Email,
                        Password = Account.Password,
                        CustomerId = cust.CustomerId,
                        Role = 2
                    };
                    await dBContext.Customers.AddAsync(cust);
                    await dBContext.Accounts.AddAsync(newAcc);
                    await dBContext.SaveChangesAsync();
                    ViewData["msg"] = "Sign up successful!";
                    return RedirectToPage("/account/signup");
                }
                else
                {
                    ViewData["msg"] = "This account exists.";
                    return Page();
                }
            }
        }
    }
}
