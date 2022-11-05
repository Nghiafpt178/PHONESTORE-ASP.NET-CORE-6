
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MyRazorPage.Models;
using System.Security.Principal;
using System.Text;
using System.Text.Json;

namespace MyRazorPage.Pages.Account
{
    public class CartModel : PageModel
    {
        private readonly PRN221_DBContext db;
        private const string CART_COOKIE_NAME = "CartCookie";
        public CartModel(PRN221_DBContext db)
        {
            this.db = db;
        }

        public SignUpModel SignUpModel;

        [BindProperty]
        public Customer Customer { get; set; }

        [BindProperty]
        public Models.Account Account { get; set; }
        public Models.Product Product { get; set; }

        [BindProperty]
        public List<CartItem> CartItems
        {
            get
            {
                //Cookie handle
                string key = CART_COOKIE_NAME;
                var cooieValue = Request.Cookies[key];
                if (cooieValue != null)
                {
                    var data = JsonSerializer.Deserialize<List<CartItem>>(cooieValue);
                    if (data == null)
                    {
                        data = new List<CartItem>();
                    }
                    return data;
                }
                return new List<CartItem>();
            }


        }

        public void OnGet()
        {

        }
        public void CookieHandle(List<CartItem> myCart)
        {
            string key = CART_COOKIE_NAME;
            string value = JsonSerializer.Serialize(myCart);
            CookieOptions options = new CookieOptions
            {
                Expires = DateTime.Now.AddDays(5)
            };
            HttpContext.Response.Cookies.Append(key, value, options);
        }
        public IActionResult OnGetAddToCartHandle(int? pid)
        {

            if (HttpContext.Session.GetString("account") != null)
            {
                var acc = JsonSerializer.Deserialize<Models.Account>(HttpContext.Session.GetString("account"));
                Customer = db.Customers.SingleOrDefault(x => x.CustomerId == acc.CustomerId);
            }
            if (pid != null)
            {

                var myCart = CartItems;
                var item = myCart.SingleOrDefault(p => p.ProductId == pid);
                if (item == null)
                {
                    var productFind = db.Products.SingleOrDefault(p => p.ProductId == pid);
                    item = new CartItem
                    {
                        ProductId = (int)pid,
                        ProductName = productFind.ProductName,
                        Price = (decimal)productFind.UnitPrice,
                        Quantity = 1,

                    };
                    myCart.Add(item);
                    decimal totalPrice = 0;
                    foreach (var p in myCart)
                    {
                        totalPrice += p.TotalPrice;
                    }
                    HttpContext.Session.SetString("totalPrice", JsonSerializer.Serialize(totalPrice));
                }
                else
                {
                    item.Quantity++;
                }
                CookieHandle(myCart);
                HttpContext.Session.SetString("CartSession", JsonSerializer.Serialize(myCart));
            }
            return RedirectToPage("/account/cart");

        }

        public async Task<IActionResult> OnPost()
        {
            DateTime today = DateTime.Today;
            DateTime oneNextWeek = today.AddDays(7);

            if (HttpContext.Session.GetString("CartSession") != null
                && HttpContext.Session.GetString("account") != null)
            {
                var data = JsonSerializer.Deserialize<List<CartItem>>(HttpContext.Session.GetString("CartSession"));
                var acccount = JsonSerializer.Deserialize<Models.Account>(HttpContext.Session.GetString("account"));
                var order = new Order()
                {
                    CustomerId = acccount.CustomerId,
                    OrderDate = DateTime.Now,
                    EmployeeId = 1,
                    RequiredDate = oneNextWeek,

                };
                db.Orders.Add(order);
                db.SaveChanges();
                foreach (var item in data)
                {
                    var orderDetails = new OrderDetail()
                    {
                        OrderId = order.OrderId,
                        ProductId = item.ProductId,
                        UnitPrice = (decimal)item.Price,
                        Quantity = (short)item.Quantity,
                        Discount = 0,
                    };
                    db.OrderDetails.Add(orderDetails);
                    db.SaveChanges();
                }
                HttpContext.Session.Remove("CartSession");
                HttpContext.Session.Remove("totalPrice");
                TempData["msgOrder"] = "Order successful";


            }
            else if (HttpContext.Session.GetString("CartSession") != null
                && HttpContext.Session.GetString("account") == null)
            {
                var cust = new Customer()
                {
                    CustomerId = SignUpModel.RandomCID(5),
                    CompanyName = Customer.CompanyName,
                    ContactName = Customer.ContactName,
                    ContactTitle = Customer.ContactTitle,
                    Address = Customer.Address
                };
                await db.Customers.AddAsync(cust);
                await db.SaveChangesAsync();

                var data = JsonSerializer.Deserialize<List<CartItem>>(HttpContext.Session.GetString("CartSession"));
                var order = new Order()
                {
                    CustomerId = cust.CustomerId,
                    OrderDate = DateTime.Now,
                    EmployeeId = 1,
                    RequiredDate = oneNextWeek,

                };
                db.Orders.Add(order);
                db.SaveChanges();
                foreach (var item in data)
                {
                    var orderDetails = new OrderDetail()
                    {
                        OrderId = order.OrderId,
                        ProductId = item.ProductId,
                        UnitPrice = (decimal)item.Price,
                        Quantity = (short)item.Quantity,
                        Discount = 0,
                    };
                    db.OrderDetails.Add(orderDetails);
                    db.SaveChanges();
                }
                HttpContext.Session.Remove("CartSession");
                HttpContext.Session.Remove("totalPrice");
                TempData["msgOrder"] = "Order successful";
            }
            return RedirectToPage("/account/cart");


        }

        public IActionResult OnGetIncreaseHandler(int? pid)
        {
            string key = CART_COOKIE_NAME;
            var cooieValue = Request.Cookies[key];
            if (cooieValue != null)
            {
                var CartItemsCookie = JsonSerializer.Deserialize<List<CartItem>>(cooieValue);
                if (pid != null)
                {
                    var carts = CartItemsCookie;
                    var productFind = db.Products.SingleOrDefault(p => p.ProductId == pid);
                    foreach (var item in carts)
                    {
                        if (item.ProductId == pid && item.Quantity < productFind.UnitsInStock)
                        {
                            item.Quantity = item.Quantity + 1;

                        }
                        else if (item.ProductId == pid && item.Quantity >= productFind.UnitsInStock)
                        {
                            TempData["outOfStockMsg"] = "Over quantity available";
                        }

                    }
                    decimal totalPrice = 0;
                    foreach (var p in carts)
                    {
                        totalPrice += p.TotalPrice;
                    }
                    HttpContext.Session.SetString("totalPrice", JsonSerializer.Serialize(totalPrice));
                    //HttpContext.Session.SetString("CartSession", JsonSerializer.Serialize(carts));
                    CookieHandle(carts);

                }
            }
            return RedirectToPage("/account/cart");
        }


        public IActionResult OnGetDecreaseHandler(int? pid)
        {
            string key = CART_COOKIE_NAME;
            var cooieValue = Request.Cookies[key];
            if (cooieValue != null)
            {
                var CartItemsCookie = JsonSerializer.Deserialize<List<CartItem>>(cooieValue);
                if (pid != null)
                {
                    var carts = CartItemsCookie;
                    var productFind = db.Products.SingleOrDefault(p => p.ProductId == pid);
                    foreach (var item in carts)
                    {
                        if (item.ProductId == pid && item.Quantity > 1)
                        {
                            item.Quantity = item.Quantity - 1;
                        }
                        else if (item.ProductId == pid && item.Quantity == 1)
                        {
                            item.Quantity = 1;
                        }


                    }
                    decimal totalPrice = 0;
                    foreach (var p in carts)
                    {
                        totalPrice += p.TotalPrice;
                    }
                    HttpContext.Session.SetString("totalPrice", JsonSerializer.Serialize(totalPrice));
                    //HttpContext.Session.SetString("CartSession", JsonSerializer.Serialize(carts));
                    CookieHandle(carts);

                }
            }

            return RedirectToPage("/account/cart");
        }

        public IActionResult OnGetRemoveHandler(int? pid)
        {
            if (pid != null)
            {
                string key = CART_COOKIE_NAME;
                var cooieValue = Request.Cookies[key];
                if (cooieValue != null)
                {
                    var data = JsonSerializer.Deserialize<List<CartItem>>(cooieValue);
                    var cartItems = data;
                    foreach (var item in cartItems.ToList())
                    {
                        if (item.ProductId == pid)
                        {
                            cartItems.Remove(item);
                        }
                    }
                    decimal totalPrice = 0;
                    foreach (var p in cartItems.ToList())
                    {
                        totalPrice += p.TotalPrice;
                    }
                    HttpContext.Session.SetString("totalPrice", JsonSerializer.Serialize(totalPrice));
                    //HttpContext.Session.SetString("CartSession", JsonSerializer.Serialize(cartItems));
                    //Cookies handle
                    CookieHandle(cartItems);
                }
                return RedirectToPage("/account/cart");
            }
            else
            {
                return Page();
            }

        }


    }
}


