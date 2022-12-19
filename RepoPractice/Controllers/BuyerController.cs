using RepoPractice.Models.DAL.Product;
using RepoPractice.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net.Mail;
using System.Net;
using System.Collections;
using System.Web.Http.Tracing;

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
            // return View(db.ProductSet.OrderByDescending(x=>x.ProductId).ToList());
        }

        //public ActionResult SendMail(int? id)
        //{
        //    UserModel i = userObj.GetAllById(Convert.ToInt32(id));
        //    return View(i);
        //}

        //[HttpPost]
        [Authorize]
        public ActionResult SendMail()
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

        public ActionResult BuyerDisplayAllProduct()
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

        [Authorize]

        public ActionResult AddToCart(int id)
        {
            ProductModel p = productObj.GetAllById(Convert.ToInt32(id));

            CartModel co = new CartModel();

            co.ProductModel_ProductId = p.ProductId;

            TempData["id"] = co.ProductModel_ProductId;

            cartObj.Add(co);
            cartObj.Save();
            return View(p);

        }
        List<CartModel> li = new List<CartModel>();
        [HttpPost]
        public ActionResult AddToCart(/*ProductModel pi*/ string qty, int? Id)
        {
            // ProductModel po = db.ProductSet.Where(x=> x.ProductId == Id).Single();
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


        public ActionResult Remove(int? id)
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
        [Authorize]
        public ActionResult ViewCart()
        {
            int id = Convert.ToInt32(TempData["id"]);

            var data = productObj.GetAllById(id);

            return View(data);
        }

        [Authorize]
        public ActionResult UpdateProfile(int id)
        {
            UserModel u = userObj.GetAllById(id);
            return View(u);
        }

        [Authorize]
        public ActionResult AddBalance(UserModel user)
        {
            WalletModel w = walletObj.GetAllById(user.UserId);
            return View(w);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddBalance(WalletModel collection)
        {
            walletObj.Update(collection);
            walletObj.Save();
            return RedirectToAction("Checkout");
        }

        public ActionResult Checkout()
        {
            TempData.Keep();


            return View();
        }

        [Authorize]
        public ActionResult PaymentMode()
        {
            return View();
        }

        [HttpPost]
        public ActionResult PaymentMode(OrderModel order)
        {
            return View();
        }

        [Authorize]
        public ActionResult OrderConfirmed()
        {
            return View(from i in orderObj.GetAll() select i);
        }

    }
}