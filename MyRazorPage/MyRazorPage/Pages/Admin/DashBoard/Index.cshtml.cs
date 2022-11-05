using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using MyRazorPage.Models;

namespace MyRazorPage.Pages.Admin.DashBoard
{

    [Authorize]
    public class IndexModel : PageModel
    {

        private readonly PRN221_DBContext db;

        public IndexModel(PRN221_DBContext db)
        {
            this.db = db;
        }

        [BindProperty]
        public int TotalOrder { get; set; }

        [BindProperty]
        public decimal WeeklySales { get; set; }

        [BindProperty]
        public List<decimal> SumFreightByMonth { get; set; }

        [BindProperty]
        public int TotalCustomers { get; set; }
        [BindProperty]
        public int[] CustomersStatics { get; set; }
        [BindProperty]
        public int NewCustomer { get; set; }

        [BindProperty]
        public int TotalGuest { get; set; }

        [BindProperty]
        public List<int> YearFromDB { get; set; }
        [BindProperty]
        public int? YearFilterCurrent { get; set; }

        public void OnGet(int? yearFilter)
        {

            YearFromDB = db.Orders.Select(o => o.OrderDate.Value.Year).Distinct().ToList();
            DateTime today = DateTime.Today;
            DateTime oneWeek = today.AddDays(-7);
            DateTime dailyAvg = today.AddDays(-30);
            TotalOrder = db.Orders.Count();
            TotalCustomers = db.Customers.Count(x => x.CustomerId != null);
            var TotalCus = db.Accounts.Select(x => x.CustomerId).ToArray();
            TotalGuest = db.Customers.Where(x => !TotalCus.Contains(x.CustomerId)).Count();
            NewCustomer = db.Customers.Where(c => c.CreateDate >= dailyAvg && c.CreateDate <= today).Count();
            WeeklySales = (decimal)db.Orders.Where(o => o.OrderDate >= oneWeek && o.OrderDate <= today).Sum(x => x.Freight);
            decimal totalPricePerMonth = 0;
            List<decimal> listTotal = new List<decimal>();

            for (int i = 1; i <= 12; i++)
            {
                if (yearFilter != null)
                {
                    totalPricePerMonth = (decimal)db.Orders.Where(x => x.OrderDate.Value.Month == i && x.OrderDate.Value.Year == yearFilter).Sum(x => x.Freight);
                    YearFilterCurrent = yearFilter;
                }
                else
                {
                    totalPricePerMonth = (decimal)db.Orders.Where(x => x.OrderDate.Value.Month == i).Sum(x => x.Freight);
                }
                listTotal.Add(totalPricePerMonth);
            }

            SumFreightByMonth = listTotal;
            List<int> termList = new List<int>();
            termList.Add(TotalCustomers);
            termList.Add(NewCustomer);
            termList.Add(TotalCustomers + NewCustomer);
            CustomersStatics = termList.ToArray();


        }
    }
}
