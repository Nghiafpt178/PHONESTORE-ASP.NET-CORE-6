using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using MyRazorPage.HubServer;
using MyRazorPage.Models;

namespace MyRazorPage.Pages.Admin.Product
{
    public class AddProductModel : PageModel
    {

        private readonly PRN221_DBContext _dbContext;

        private readonly PRN221_DBContext db;
        private readonly IHubContext<SignalRServer> _signalrHub;

        public AddProductModel(PRN221_DBContext db, IHubContext<SignalRServer> signalrHub)
        {
            this.db = db;
            _signalrHub = signalrHub;
        }

        [BindProperty]
        public Models.Product Product { get; set; }

        public List<Category> Categories { get; set; }
        public void OnGet()
        {
            Categories = db.Categories.ToList();
        }

        public async Task<IActionResult> OnPost()
        {
            Categories = db.Categories.ToList();
            if (!ModelState.IsValid)
            {
                @TempData["msgFail"] = "Add product fail!";
                return Page();
            }
            db.Products.Add(Product);
            await db.SaveChangesAsync();
            await _signalrHub.Clients.All.SendAsync("ReloadProduct");
            @TempData["msg"] = "Add product successfully!";
            return RedirectToPage("/admin/product/manager");

        }
    }
}
