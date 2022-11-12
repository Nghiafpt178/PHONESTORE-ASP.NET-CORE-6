
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using MyRazorPage.HubServer;
using MyRazorPage.Models;
using OfficeOpenXml;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace MyRazorPage.Pages.Admin.Product
{
    [Authorize]
    public class ManagerModel : PageModel
    {
        private int PageSize = 7;
        private readonly PRN221_DBContext db;
        private readonly IHubContext<SignalRServer> _signalrHub;
        private IHostingEnvironment _hostingEnv;


        public ManagerModel(PRN221_DBContext db, IHubContext<SignalRServer> signalrHub, IHostingEnvironment hostingEnvironment)
        {
            this.db = db;
            _signalrHub = signalrHub;
            _hostingEnv = hostingEnvironment;
        }
        public int PageIndex { get; set; }
        public int PageTotal { get; set; }
        public List<Models.Product> Products { get; set; }
        [BindProperty]
        public List<Category> Categories { get; set; }
        [BindProperty]
        public int? CurrentFilter { get; set; }

        [BindProperty]
        public List<OrderDetail> OrderDetail { get; set; }



        public Models.Product Product { get; set; }
        public async Task<IActionResult> OnGetAsync(int? pageIndex, int? cateID, string searchString, int? productID)
        {
            if (productID != null)
            {
                OrderDetail = db.OrderDetails.Where(od => od.ProductId == productID).ToList();
                if (OrderDetail.Count > 0)
                {
                    TempData["msg"] = "ProductID: " + productID + " exist in order detail!";
                }
                if (OrderDetail.Count == 0)
                {
                    Product = db.Products.SingleOrDefault(p => p.ProductId == productID);
                    db.Products.Remove(Product);
                    db.SaveChanges();
                    await _signalrHub.Clients.All.SendAsync("ReloadProduct");
                    TempData["msg"] = "Delete successful";
                }
                return RedirectToAction("/Admin/Product/Manager");
            }
            else
            {
                Categories = db.Categories.ToList();
                var products = db.Products.Include(p => p.Category).OrderByDescending(p => p.ProductId).ToList();
                if (pageIndex == null)
                {
                    pageIndex = 1;
                }
                if (cateID == null)
                {
                    Products = products.ToList().Skip((pageIndex.Value - 1) * PageSize).Take(PageSize).ToList();
                }
                if (cateID != null)
                {
                    Products = products.Where(p => p.CategoryId == cateID).ToList();
                    products = Products;
                    Products = Products.Skip((pageIndex.Value - 1) * PageSize).Take(PageSize).ToList();
                    CurrentFilter = cateID;
                }
                if (searchString != null)
                {
                    Products = products.Where(p => p.ProductName.ToUpper().Contains(searchString.ToUpper())).ToList();
                    products = Products;
                    Products = Products.Skip((pageIndex.Value - 1) * PageSize).Take(PageSize).ToList();

                }
                if (cateID != null && searchString != null)
                {
                    CurrentFilter = cateID;
                    Products = products.Where(p => p.CategoryId == cateID && p.ProductName.ToUpper().Contains(searchString.ToUpper())).ToList();
                    products = Products;
                    Products = Products.Skip((pageIndex.Value - 1) * PageSize).Take(PageSize).ToList();

                }
                PageIndex = pageIndex.Value;
                @ViewData["key"] = searchString;
                PageTotal = (products.Count() / PageSize);
                if (products.Count() % PageSize != 0)
                {
                    PageTotal++;
                }

                return Page();

            }
        }
        public async Task<IActionResult> OnPostImportData(IFormFile fileImport)
        {  

            var stream = fileImport.OpenReadStream();
            try
            {
                using (var package = new ExcelPackage(stream))
                {
                    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                    ExcelWorksheet worksheet = package.Workbook.Worksheets.First();
                    var rowcount = worksheet.Dimension.Rows;
                    for (int row = 2; row <= rowcount; row++)
                    {
                        try
                        {
                            db.Products.Add(new Models.Product
                            {
                                ProductName = worksheet.Cells[row, 1].Value.ToString().Trim(),
                                UnitPrice = (decimal)Convert.ToSingle(worksheet.Cells[row, 2].Value),
                                QuantityPerUnit = worksheet.Cells[row, 3].Value.ToString().Trim(),
                                UnitsInStock = (short?)Convert.ToSingle(worksheet.Cells[row, 4].Value),
                                CategoryId = (int?)Convert.ToSingle(worksheet.Cells[row, 5].Value),
                                Discontinued = (bool)worksheet.Cells[row, 6].Value,
                            });

                           
                            db.SaveChanges();
                        }
                        catch(OverflowException ex)
                        {
                            Console.WriteLine(ex.Message);
                        }                       
                    }
                    await _signalrHub.Clients.All.SendAsync("ReloadProduct");
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }


            return Redirect(Request.Headers["Referer"].ToString());

        }
    }
}
