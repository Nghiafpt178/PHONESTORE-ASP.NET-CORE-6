using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyRazorPage.Models;

namespace MyRazorPage.Pages.Product
{
    public class DetailModel : PageModel
    {
        private readonly PRN221_DBContext db;

        public DetailModel(PRN221_DBContext db)
        {
            this.db = db;
        }
        [BindProperty]
        public Models.Product Product { get; set; }
        [BindProperty]
        public string MainImage { get; set; }
        [BindProperty]
        public string[] imageSplit { get; set; }
       
        public void OnGet(int? pid)
        {
            Product = db.Products.Find(pid);
            var productImages = Product.ProductImage;
            if(productImages != null)
            {
                imageSplit = Convert.ToString(productImages).Split(",");
                MainImage = imageSplit[0];
            }               
            
            

        }
    }
}
