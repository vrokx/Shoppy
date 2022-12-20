using RepoPractice.Models.DAL.Product;
using RepoPractice.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Net.Mail;
using System.Net;
using System.CodeDom;
using System.Web.Http.Results;
using System.Collections.ObjectModel;

namespace RepoPractice.Controllers
{
    public class BuyerController : Controller
    {
        ShoppingCartDBContext db = new ShoppingCartDBContext();
        private IRepository<UserModel> userObj;
        private IRepository<ProductModel> productObj;
        private IRepository<CartModel> cartObj;
        private IRepository<OrderModel> orderObj;
        private IRepository<WalletModel> walletObj;


        public BuyerController()
        {
            
                this.productObj = new GenericRepository<ProductModel>();
                this.userObj = new GenericRepository<UserModel>();
                this.cartObj = new GenericRepository<CartModel>();
                this.orderObj = new GenericRepository<OrderModel>();
                this.walletObj = new GenericRepository<WalletModel>();
            
            
        }

        [Authorize]
         public ActionResult Index()
        {
            try
            {
                if (TempData["cart"] != null)
                {
                    float x = 0;
                    List<CartModel> li2 = TempData["cart"] as List<CartModel>;
                    foreach (var item in li2)
                    {
                        x += item.TotalAmount;

                    }

                    TempData["total"] = x;
                }
                TempData.Keep();
                return View(from i in productObj.GetAll().OrderByDescending(x => x.ProductId) select i);
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }

        [Authorize]
        public ActionResult SendMail()
        {
            try
            {

                MailMessage mm = new MailMessage("radhakrishna36495@gmail.com", User.Identity.Name);
                mm.Subject = "Order Confirmed";
                mm.Body = "Your Order Has Been Confirmed";
                mm.IsBodyHtml = false;
                SmtpClient smtp = new SmtpClient();
                smtp.Host = "smtp.gmail.com";
                smtp.Port = 587;
                smtp.EnableSsl = true;


                NetworkCredential nc = new NetworkCredential("radhakrishna36495@gmail.com", "iufedzbfhqlpypdl");
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = nc;
                smtp.Send(mm);
                return RedirectToAction("BuyerDisplayAllProduct");
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }

        public ActionResult BuyerDisplayAllProduct()
        {
            try
            {
                if (TempData["cart"] != null)
                {
                    float x = 0;
                    List<CartModel> li2 = TempData["cart"] as List<CartModel>;
                    foreach (var item in li2)
                    {
                        x += item.TotalAmount;

                    }

                    TempData["total"] = x;
                }
                TempData.Keep();

                return View(from i in productObj.GetAll() select i);
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }

        [Authorize]

        public ActionResult AddToCart(int id)
        {
            try
            {
                ProductModel p = productObj.GetAllById(Convert.ToInt32(id));

                CartModel co = new CartModel();

                co.ProductModel_ProductId = p.ProductId;

                TempData["id"] = co.ProductModel_ProductId;

                cartObj.Add(co);
                cartObj.Save();
                return View(p);
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }
        List<CartModel> li = new List<CartModel>();
        [HttpPost]
        public ActionResult AddToCart(/*ProductModel pi*/ string qty, int? Id)
        {
            try
            {
                ProductModel po = productObj.GetAllById(Convert.ToInt32(Id));
                CartModel co = new CartModel();
                co.ProductModel_ProductId = po.ProductId;
                co.productname = po.ProductName;
                co.price = (int)po.Price;
                co.Quantity = Convert.ToInt32(qty);
                co.TotalAmount = co.price * co.Quantity;

                if (TempData["cart"] == null)
                {
                    li.Add(co);
                    TempData["cart"] = li;
                }
                else
                {
                    List<CartModel> li2 = TempData["cart"] as List<CartModel>;
                    int flag = 0;
                    foreach (var item in li2)
                    {
                        if (item.CartId == co.ProductModel_ProductId)
                        {
                            item.Quantity = co.Quantity;
                            item.TotalAmount = co.TotalAmount;
                            flag = 1;
                        }
                    }
                    if (flag == 0)
                    {
                        li2.Add(co);
                    }

                    TempData["cart"] = li2;
                }
                TempData.Keep();

                return RedirectToAction("ViewCart");
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }


        public ActionResult Remove(int? id)
        {
            try
            {
                li = TempData["cart"] as List<CartModel>;

                CartModel c = li.Where(x => x.ProductModel_ProductId == id).SingleOrDefault();

                li.Remove(c);
                float h = 0;
                foreach (var item in li)
                {
                    h += item.TotalAmount;

                }
                TempData["total"] = h;
                TempData.Keep();
                return RedirectToAction("checkout");
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }
        [Authorize]
        public ActionResult ViewCart()
        {
            try
            {
                int id = Convert.ToInt32(TempData["id"]);

                var data = productObj.GetAllById(id);

                return View(data);
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }

        [Authorize]
        public ActionResult UpdateProfile()
        {
            var id = (int)Session["UserId"];

            UserModel u = userObj.GetAllById(id);
            return View(u);
        }

        [HttpPost]
        public ActionResult UpdateProfile(UserModel collection)
        {
            try
            {
                userObj.Update(collection);
                userObj.Save();
                return RedirectToAction("Checkout");
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }

        [Authorize]
        public ActionResult AddBalance(UserModel user)
        {
            try
            {
                WalletModel w = walletObj.GetAllById(user.UserId);
                return View(w);
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddBalance(WalletModel collection)
        {
            try
            {
                walletObj.Update(collection);
                walletObj.Save();
                return RedirectToAction("Checkout");
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }

        public ActionResult Checkout()
        {
            try
            {
                TempData.Keep();
                return View();
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }

        [Authorize]
        public ActionResult PaymentMode()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }

        [HttpPost]
        public ActionResult PaymentMode(OrderModel order)
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }

        [Authorize]
        public ActionResult OrderConfirmed()
        {
            try
            {
                return View(from i in orderObj.GetAll() select i);
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }

        public ActionResult Search(string searchString)
        {
            try
            {
                var det = db.ProductSet.Where(d => d.ProductName.ToUpper().Contains(searchString.ToUpper())).ToList();

                return View(det);
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }

    }
}