using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyRazorPage.Models;

namespace MyRazorPage.Pages.Admin.Customer
{
    public class IndexModel : PageModel
    {
        private const int PAGE_SIZE = 7;
        private readonly PRN221_DBContext db;

        public IndexModel(PRN221_DBContext db)
        {
            this.db = db;
        }
        public int PageIndex { get; set; }
        public int PageTotal { get; set; }

        public List<Models.Customer> Customers { get; set; }
        public void OnGet(int? pageIndex ,string searchString)
        {
            if (pageIndex == null)
            {
                pageIndex = 1;
            }
            if(searchString != null)
            {
                var customers = db.Customers.Where(c => c.ContactName.Contains(searchString)).OrderByDescending(c => c.CreateDate).ToList();
                Customers = customers.Skip((pageIndex.Value - 1) * PAGE_SIZE).Take(PAGE_SIZE).ToList();
                ViewData["searchKey"] = searchString;
                PageIndex = pageIndex.Value;
                PageTotal = (customers.Count() / PAGE_SIZE);
                if (customers.Count() % PAGE_SIZE != 0)
                {
                    PageTotal++;
                }
            }
            else
            {
                var customers = db.Customers.OrderByDescending(c => c.CreateDate).ToList();
                Customers = customers.Skip((pageIndex.Value - 1) * PAGE_SIZE).Take(PAGE_SIZE).ToList();
                PageIndex = pageIndex.Value;
                PageTotal = (customers.Count() / PAGE_SIZE);
                if (customers.Count() % PAGE_SIZE != 0)
                {
                    PageTotal++;
                }
            }
            
        }
    }
}
