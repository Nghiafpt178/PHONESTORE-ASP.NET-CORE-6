using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Scaffolding.Metadata;
using MyRazorPage.Models;
using System.Linq;
using System.Text.Json;

namespace MyRazorPage.Pages
{
    
    public class IndexModel : PageModel
    {

        private readonly PRN221_DBContext db;

        public IndexModel(PRN221_DBContext db)
        {
            this.db = db;
        }
        [BindProperty]
        public List<Models.Product> BestSaleProducts { get; set; }

        [BindProperty]
        public List<Category> Categories { get; set; }

        [BindProperty]
        public List<Models.Product> NewProducts { get; set; }

        [BindProperty]
        public List<Models.Product> HotProducts { get; set; }

        [BindProperty]
        public List<OrderDetail> OrderDetails { get; set;}

        public List<CartItem> CartItems { get; set; }
        
        public void OnGet(int? cid)
        {

            
            Categories = db.Categories.ToList();
            if(cid != null)
            {
                BestSaleProducts = db.Products.Include(p => p.OrderDetails).Where(p => p.CategoryId == cid)
                               .OrderByDescending(x => x.OrderDetails.Count()).Take(4).ToList();
                NewProducts = db.Products.Where(p => p.CategoryId == cid).Take(4).OrderByDescending(p => p.ProductId).ToList();
                HotProducts = db.Products.Include(p => p.OrderDetails).Where(p => p.CategoryId == cid)
                               .OrderByDescending(x => x.OrderDetails.Count()).Take(4).ToList();
            }
            else
            {
                BestSaleProducts = db.Products.Include(p => p.OrderDetails)
                               .OrderByDescending(x => x.OrderDetails.Count()).Take(4).ToList();
                NewProducts = db.Products.Take(4).OrderByDescending(p => p.ProductId).ToList();
                HotProducts = db.Products.Include(p => p.OrderDetails)
                               .OrderByDescending(x => x.OrderDetails.Count()).Take(4).ToList();

            }

            
            


        }
    }
}
