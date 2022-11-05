using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MyRazorPage.Models;
using System.Text.Json;

namespace MyRazorPage.Pages.Account
{
    public class OrdersModel : PageModel
    {
        private readonly PRN221_DBContext db;

        public OrdersModel(PRN221_DBContext db)
        {
            this.db = db;
        }
        [BindProperty]
        public List<Order> Orders { get; set; }

        [BindProperty]
        public Models.Account Account { get; set; }

        public void OnGet()
        {
            if (HttpContext.Session.GetString("account") != null)
            {
                var acc = JsonSerializer.Deserialize<Models.Account>(HttpContext.Session.GetString("account"));
                Account = acc;

                Orders = db.Orders
                    .Include(o => o.OrderDetails)
                    .Where(o => o.CustomerId == acc.CustomerId)
                    .ToList();

            }


        }
    }
}
