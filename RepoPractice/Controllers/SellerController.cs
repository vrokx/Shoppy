﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Runtime.CompilerServices;
using System.Web;
using System.Web.Mvc;
using RepoPractice.Models;
using RepoPractice.Models.DAL;
using RepoPractice.Models.DAL.Product;

namespace RepoPractice.Controllers
{
    [Authorize]
    public class SellerController : Controller
    {
        private IRepository<UserModel> userObj;
        private IRepository<ProductModel> InterFaceObj;

        public SellerController()
        {
            this.InterFaceObj = new GenericRepository<ProductModel>();
            this.userObj = new GenericRepository<UserModel>();
        }

        #region
        /// <summary>
        /// to add products in Product Table
        /// </summary>
        /// <returns></returns>
        public ActionResult AddProduct()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddProduct(ProductModel collection)
        {
            try
            {
                InterFaceObj.Add(collection);
                InterFaceObj.Save();
                return RedirectToAction("DisplayAllProducts");
            }
            catch(Exception e)
            {
                return Content(e.Message);
            }
        }
        #endregion
        public ActionResult DisplayAllProducts()
        {
            return View(from i in InterFaceObj.GetAll() select i); 
        }
        
        public ActionResult UpdateProducts(int id)
        {
            ProductModel p = InterFaceObj.GetAllById(id);
            return View(p);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateProducts(int id, ProductModel collection)
        {
            InterFaceObj.Update(collection);
            InterFaceObj.Save();
            return RedirectToAction("DisplayAllProducts");
        }

        public ActionResult DeleteProduct(int id)
        {
            ProductModel p = InterFaceObj.GetAllById(id);
            return View(p);
        }

        [HttpPost]
        public ActionResult DeleteProduct(int id , ProductModel collection)
        {
            InterFaceObj.Delete(id);
            InterFaceObj.Save();
            return RedirectToAction("DisplayAllProducts");
        }



    }
}