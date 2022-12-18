using RepoPractice.Models.DAL.Product;
using RepoPractice.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace RepoPractice.App_Start
{
    public class LoginController : Controller
    {
        private IRepository<UserModel> userObj;

        public LoginController()
        {
            this.userObj = new GenericRepository<UserModel>();
        }

        public ActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Register(UserModel user)
        {
            userObj.Add(user);
            userObj.Save();
            return RedirectToAction("Login");
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(string email, string password , UserModel user)
        {
            //bool isValid = UserModel.Any(x => x.Email == email);

            //var myList = (from i in userObj.GetAll() select i).ToList();
            //var dbEmail = (from i in myList
            //              select i.Email).ToString();
            //var dbPassword = (from i in myList
            //                 select i.Password).ToString();

            if (email == user.Email && password == user.Password) 
            {

                if (email == "admin@admin.com" && password == "admin@123")
                {
                    FormsAuthentication.SetAuthCookie(email, false);
                    return RedirectToAction("DisplayAllProducts", "Seller");
                }
                else
                {
                    FormsAuthentication.SetAuthCookie(email, false);
                    return RedirectToAction("BuyerDisplayAllProduct", "Buyer");
                }
            }
            return Content("Logged In !");
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
        }
    }
}