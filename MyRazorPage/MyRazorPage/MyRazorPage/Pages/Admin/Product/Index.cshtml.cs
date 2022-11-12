using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using MyRazorPage.Models;

namespace MyRazorPage.Pages.Product
{
    public class IndexModel : PageModel
    {
        private readonly PRN221_DBContext db;
        

        public IndexModel(PRN221_DBContext db)
        {
            this.db = db;
        }

        [BindProperty]
        public List<Models.Product> Products { get; set; }  

        public List<Category> Categories { get; set; } 
        public string CurrentSort { get; set; }

        public string ProductIdSort { get; set; }
        public string ProductNameSort { get; set; }

        public async Task<IActionResult> OnGet()
        {
            Products = await db.Products.Include(x => x.Category).ToListAsync();
            Categories = await db.Categories.ToListAsync();
            return Page();
        }
    }
}
