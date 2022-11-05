using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using MyRazorPage.HubServer;
using MyRazorPage.Models;
using System.Linq;

namespace MyRazorPage.Pages.Product
{
    public class CreateModel : PageModel
    {
        private readonly PRN221_DBContext db;
        private readonly IHubContext<SignalRServer> _signalrHub;

        public CreateModel(PRN221_DBContext db, IHubContext<SignalRServer> signalrHub)
        {
            this.db = db;
            _signalrHub = signalrHub;
        }

        [BindProperty]
        public Models.Product Product { get; set; }

        public List<Category> Categories { get; set; }
        public void OnGet()
        {
            ViewData["CategoryId"] = new SelectList(db.Categories, "CategoryId", "CategoryName");
        }

        public async Task<IActionResult> OnPostAsync()
        {
         
            await db.Products.AddAsync(Product);
            await db.SaveChangesAsync();
            var products = db.Products.Include(x => x.Category).Select(x => new
            {
                productId = x.ProductId,
                productName = x.ProductName,
                category = x.Category.CategoryName,
            });
            await _signalrHub.Clients.All.SendAsync("LoadProducts", await products.ToListAsync());
            return RedirectToPage("/admin/product/index");
        }
    }
}
