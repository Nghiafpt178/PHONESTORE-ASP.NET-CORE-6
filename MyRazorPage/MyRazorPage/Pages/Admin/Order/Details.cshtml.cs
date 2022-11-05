using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MyRazorPage.Models;

namespace MyRazorPage.Pages.Admin.Order
{
    public class DetailsModel : PageModel
    {
        private readonly PRN221_DBContext db;

        public DetailsModel(PRN221_DBContext db)
        {
            this.db = db;
        }
        public List<Models.OrderDetail> OrderDetails { get; set; }

        [BindProperty]
        public decimal TotalPrice { get; set; }

        public Models.Order Order { get; set; }
        public void OnGet(int? oid)
        {
            Order = db.Orders.Find(oid);
            OrderDetails = db.OrderDetails.Include(p => p.Product).
                Where(o => o.OrderId == oid).ToList();
            TotalPrice = 0;
            foreach(var item in OrderDetails)
            {
                TotalPrice += item.Quantity * item.UnitPrice;
            }
            
                   
        }
    }
}
