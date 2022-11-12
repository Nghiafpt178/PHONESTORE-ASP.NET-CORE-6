using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using MyRazorPage.HubServer;
using MyRazorPage.Models;

namespace MyRazorPage.Pages.Product
{
    public class EditModel : PageModel
    {
        private readonly PRN221_DBContext db;
        private readonly IHubContext<SignalRServer> _signalrHub;

        public EditModel(PRN221_DBContext db, IHubContext<SignalRServer> signalrHub)
        {
            this.db = db;
            _signalrHub = signalrHub;
        }

        public Models.Product Product { get; set; }

        public List<Category> Categories { get; set; }
        public void OnGet(int? pid)
        {
            if(pid != null)
            {
                Categories = db.Categories.ToList();
                Product = db.Products.SingleOrDefault(p => p.ProductId == pid);

            } 
        }

        public async Task<IActionResult> OnPost(Models.Product product)
        {
            Categories = db.Categories.ToList();
            if (!ModelState.IsValid)
            {
                TempData["msgUpdate"] = "Update fail";
                return Page();
            }
            Product = db.Products.SingleOrDefault(p => p.ProductId == product.ProductId);
            Product.ProductName = product.ProductName;
            Product.UnitPrice = product.UnitPrice;
            Product.QuantityPerUnit = product.QuantityPerUnit;
            Product.UnitsInStock = product.UnitsInStock;
            Product.CategoryId = product.CategoryId;
            Product.Discontinued = product.Discontinued;
            db.Products.Update(Product);
            await db.SaveChangesAsync();
            await _signalrHub.Clients.All.SendAsync("ReloadProduct");
            TempData["msgUpdate"] = "Update successful";
            return RedirectToPage("/admin/product/manager");
        }
    }
}
