
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MyRazorPage.Models;
using System.Text.Json;

namespace MyRazorPage.Pages.Admin.Order
{
    
    public class IndexModel : PageModel
    {
        private const int PAGE_SIZE = 7;
        private readonly PRN221_DBContext db;

        public IndexModel(PRN221_DBContext db)
        {
            this.db = db;
        }
        [BindProperty]
        public DateTime datFrom { get; set; }
        public List<Models.Order> Orders { get; set; }
        public int PageIndex { get; set; }
        public int PageTotal { get; set; }
      
        public Models.Order Order { get; set; }
        public IActionResult OnGet(int? pageIndex, DateTime? dateFrom, DateTime? dateTo)
        {
            if (pageIndex == null)
            {
                pageIndex = 1;
            }
            if (dateFrom.HasValue && dateTo.HasValue)
            {

                var ordersFilterByDate = db.Orders.Where(o => o.OrderDate >= dateFrom && o.OrderDate <= dateTo).
                Include(o => o.Employee).Include(ord => ord.Customer);
                Orders = ordersFilterByDate.Skip((pageIndex.Value - 1) * PAGE_SIZE).Take(PAGE_SIZE).ToList();
                ViewData["dateFrom"] = dateFrom.Value.ToString("yyyy-MM-dd");
                ViewData["dateTo"] = dateTo.Value.ToString("yyyy-MM-dd");
                PageIndex = pageIndex.Value;
                PageTotal = (ordersFilterByDate.Count() / PAGE_SIZE);
                if (ordersFilterByDate.Count() % PAGE_SIZE != 0)
                {
                    PageTotal++;
                }


            }
            else
            {
                var orders = db.Orders.Include(o => o.Employee).OrderByDescending(o => o.OrderId).Include(x => x.Customer);
                Orders = orders.Skip((pageIndex.Value - 1) * PAGE_SIZE).Take(PAGE_SIZE).ToList();
                PageIndex = pageIndex.Value;
                PageTotal = (orders.Count() / PAGE_SIZE);
                if (orders.Count() % PAGE_SIZE != 0)
                {
                    PageTotal++;
                }


            }
            return Page();
        }

        public IActionResult OnGetChangeStatusHandler(int? oid)
        {
            if (oid != null)
            {
                Order = db.Orders.SingleOrDefault(o => o.OrderId == oid);
                Order.RequiredDate = null;
                db.Orders.Update(Order);
                db.SaveChanges();
            }
            return RedirectToPage("/admin/order/index");
        }

    }
}
