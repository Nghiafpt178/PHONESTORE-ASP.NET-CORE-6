using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyRazorPage.Models;


namespace MyRazorPage.Pages.Product
{
    public class ProductsModel : PageModel
    {
        private readonly PRN221_DBContext db;
        private const int PAGE_SIZE = 12;
        public ProductsModel(PRN221_DBContext db)
        {
            this.db = db;
        }
        [BindProperty]
        public List<Models.Product> Products { get; set; }

        [BindProperty]
        public List<Category> Categories { get; set; }

        public string PriceSort { get; set; }

        [BindProperty]
        public string CurrentSort { get; set; }

        public string CurrentFilter { get; set; }
        public int PageIndex { get; set; }
        public int PageTotal { get; set; }

        public IActionResult OnGet(int cid, string sortOrder, int? pageIndex)
        {
            if(pageIndex == null)
            {
                pageIndex = 1;
            }
            var products = db.Products.Where(x => x.CategoryId == cid);
            if (sortOrder == null || sortOrder.Equals("asc"))
            {
                Products = products.OrderBy(x => x.UnitPrice).Skip((pageIndex.Value - 1) * PAGE_SIZE).Take(PAGE_SIZE).ToList();
                CurrentSort = "asc";
            }
            else
            {
                Products = products.OrderByDescending(x => x.UnitPrice).Skip((pageIndex.Value - 1) * PAGE_SIZE).Take(PAGE_SIZE).ToList();
                CurrentSort = "desc";
            }

            Categories = db.Categories.ToList();
            PageIndex = pageIndex.Value;
            PageTotal = (products.Count() / PAGE_SIZE);
            if (products.Count() % PAGE_SIZE != 0)
            {
                PageTotal++;
            }
            return Page();


        }


    }
}
