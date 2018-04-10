//AndrewG8460
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Logistics.Models;
using System.Data.Entity.Validation;
using WebMatrix.WebData;

namespace Logistics.Controllers
{
    public class OrderController : Controller
    {
        private LogisticsContainer db;
        
        public OrderController()
            : this(new LogisticsContainer())
        {
        }

        public OrderController(LogisticsContainer logisticsContainer)
        {
            this.db = logisticsContainer;
        }
        
        //Return the Order view data from the list
        public ActionResult Index()
        {
            //Check user is logged in
            if (WebSecurity.IsAuthenticated)
            {
            	return View(db.Orders.Include(x => x.Store).Include(x => x.OrderLines).ToList());
            }
            else
            {
                //Return the user to the login page if there is no user id
                return RedirectToAction("Login", "Account");
            }              
        }

        //Get the Order detail from the inherited order model and post the return view for order and order line item from the ids
        public ActionResult Details(int id = 0)
        {
            //Check user is logged in
            if (WebSecurity.IsAuthenticated)
            {
                Order order = db.Orders.Find(id);

                if (order == null)
                {
                    return HttpNotFound();
                }

                db.Entry(order).Reference(x => x.Store).Load();
                db.Entry(order).Collection(x => x.OrderLines).Load();

                //Get the store ids from view
                ViewBag.BookList = new SelectList(db.Books, "BookId", "Title");
                ViewBag.StoreList = new SelectList(db.Stores, "StoreId", "StoreOwner", order.StoreId);

                //Get the order details view page
                return View(order);
            }
            else
            {
                //Return the user to the login page if there is no user id
                return RedirectToAction("Login", "Account");
            }              
        }

        //Get the view up for creating an order form
        public ActionResult Create()
        {
            //Check user is logged in
            if (WebSecurity.IsAuthenticated)
            {
                //Get the store ids from view
                ViewBag.BookList = new SelectList(db.Books, "BookId", "Title");
                ViewBag.StoreList = new SelectList(db.Stores, "StoreId", "StoreOwner");

                //Get the Add Order view page
                return View();
            }
            else
            {
                //Return the user to the login page if there is no user id
                return RedirectToAction("Login", "Account");
            }
        }

        //Post the order form for the requested order number and id
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Order order)
        {
            //Check user is logged in
            if (WebSecurity.IsAuthenticated)
            {
                if (ModelState.IsValid)
                {
                    db.Database.ExecuteSqlCommand("INSERT into dbo.Orders VALUES(dbo.OrderId, dbo.StoreId, dbo.OrderNumber");
                    db.SaveChanges();

                    foreach (OrderLine line in order.OrderLines) 
                    {
                        line.OrderId = order.OrderId;
                        db.OrderLines.Add(line);
                    }

                    db.SaveChanges();

                    return RedirectToAction("Edit", new { id = order.OrderId });
                }

                ViewBag.BookList = new SelectList(db.Books, "BookId", "Title");
                ViewBag.StoreList = new SelectList(db.Stores, "StoreId", "StoreOwner", order.StoreId);
                return View(order);
            }
            else
            {
                //Return the user to the login page if there is no user id
                return RedirectToAction("Login", "Account");
            }              
        }

        //Allow the user to edit an order
        public ActionResult Edit(int id = 0)
        {
            //Check user is logged in
            if (WebSecurity.IsAuthenticated)
            {
                Order order = db.Orders.SqlQuery("SELECT * FROM dbo.Orders WHERE OrderId = " + id);

                if (order == null)
                {
                    return HttpNotFound();
                }

                db.Entry(order).Reference(x => x.Store).Load();
                db.Entry(order).Collection(x => x.OrderLines).Load();

                ViewBag.BookList = new SelectList(db.Books, "BookId", "Title");
                ViewBag.StoreList = new SelectList(db.Stores, "StoreId", "StoreOwner", order.StoreId);
                return View(order);
            }
            else
            {
                //Return the user to the login page if there is no user id
                return RedirectToAction("Login", "Account");
            }
        }

        //Commit the edited order
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Order order)
        {
            //Check user is logged in
            if (WebSecurity.IsAuthenticated)
            {
                if (ModelState.IsValid)
                {
                    var edited = order.OrderLines;

                    foreach (OrderLine line in order.OrderLines)
                    {
                        OrderLine existing = db.OrderLines.Find(order.OrderId, line.BookId);

                        if (existing != null)
                        {
                            db.OrderLines.Remove(existing);
                        }

                        db.OrderLines.Add(line);
                    }

                    db.Entry(order).State = EntityState.Modified;
                    db.SaveChanges();

                    return RedirectToAction("Index");
                }

                db.Entry(order).Reference(x => x.Store).Load();
                db.Entry(order).Collection(x => x.OrderLines).Load();

                ViewBag.BookList = new SelectList(db.Books, "BookId", "Title");
                ViewBag.StoreList = new SelectList(db.Stores, "StoreId", "StoreOwner", order.StoreId);
                return View(order);
            }
            else
            {
                //Return the user to the login page if there is no user id
                return RedirectToAction("Login", "Account");
            }
        }

        //Allow the user to delete an order
        public ActionResult Delete(int id = 0)
        {
            //Check user is logged in
            if (WebSecurity.IsAuthenticated)
            {
                Order order = db.Orders.Find(id);

                if (order == null)
                {
                    return HttpNotFound();
                }

                db.Entry(order).Reference(x => x.Store).Load();
                db.Entry(order).Collection(x => x.OrderLines).Load();

                ViewBag.BookList = new SelectList(db.Books, "BookId", "Title");
                ViewBag.StoreList = new SelectList(db.Stores, "StoreId", "StoreOwner", order.StoreId);
                return View(order);
            }
            else
            {
                //Return the user to the login page if there is no user id
                return RedirectToAction("Login", "Account");
            }
        }

        //Commit the delete order to the database
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            //Check user is logged in
            if (WebSecurity.IsAuthenticated)
            {
                Order order = db.Orders.Find(id);

                db.Orders.Remove(order);
                db.SaveChanges();

                return RedirectToAction("Index");
            }
            else
            {
                //Return the user to the login page if there is no user id
                return RedirectToAction("Login", "Account");
            }
        }

        public ActionResult AddBook(int id)
        {
            //Check user is logged in
            if (WebSecurity.IsAuthenticated)
            {
                Order order = db.Orders.Find(id);

                if (order == null)
                {
                    return HttpNotFound();
                }

                db.Entry(order).Reference(x => x.Store).Load();
                db.Entry(order).Collection(x => x.OrderLines).Load();
                order.OrderLines.Add(new OrderLine() { OrderId = id, Order = order });

                ViewBag.BookList = new SelectList(db.Books, "BookId", "Title");
                ViewBag.StoreList = new SelectList(db.Stores, "StoreId", "StoreOwner", order.StoreId);
                return View("Edit", order);
            }
            else
            {
                //Return the user to the login page if there is no user id
                return RedirectToAction("Login", "Account");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddBook(Order order)
        {
            // Got here because last Action was AddBook(int)
            return Edit(order);
        }

        public ActionResult RemoveBook(int orderId, int bookId)
        {
            //Check user is logged in
            if (WebSecurity.IsAuthenticated)
            {
                OrderLine orderLine = db.OrderLines.Find(orderId, bookId);

                if (orderLine == null)
                {
                    return HttpNotFound();
                }

                Order order = orderLine.Order;

                db.OrderLines.Remove(orderLine);
                db.SaveChanges();

                ViewBag.BookList = new SelectList(db.Books, "BookId", "Title");
                ViewBag.StoreList = new SelectList(db.Stores, "StoreId", "StoreOwner", order.StoreId);
                return View("Edit", order);
            }
            else
            {
                //Return the user to the login page if there is no user id
                return RedirectToAction("Login", "Account");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RemoveBook(Order order)
        {
            // Got here because last Action was RemoveBook(int)
            return Edit(order);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}