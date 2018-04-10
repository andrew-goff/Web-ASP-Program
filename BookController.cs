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
    public class BookController : Controller
    {
        private LogisticsContainer db;

        public BookController()
            : this(new LogisticsContainer())
        {
        }

        public BookController(LogisticsContainer logisticsContainer)
        {
            this.db = logisticsContainer;
        }
        
        //Get the book result onto the book index page
        public ActionResult Index()
        {
            //Check user is logged in
            if (WebSecurity.IsAuthenticated)
            {
                return View(db.Books.Include(x => x.Author).Include(x => x.Supplier).ToList());
            }
            else
            {
                //Return the user to the login page if there is no user id
                return RedirectToAction("Login", "Account");
            }              
        }

        //Get the book details out onto the web page when a book ID is found
        public ActionResult Details(int id = 0)
        {
            //Check user is logged in
            if (WebSecurity.IsAuthenticated)
            {
            	Book book = db.Books.Find(id);

            	if (book == null)
            	{
            	    return HttpNotFound();
            	}

                ViewBag.AuthorList = new SelectList(db.Authors, "AuthorId", "Publisher");
                ViewBag.SupplierList = new SelectList(db.Suppliers, "SupplierId", "SupplierName");

                //Get the order details view page
            	return View(book);
            }
            else
            {
                //Return the user to the login page if there is no user id
                return RedirectToAction("Login", "Account");
            }              
        }

        //Allow the user to add a new book once they have been checked they are logged in
        public ActionResult Create()
        {
            //Check user is logged in
            if (WebSecurity.IsAuthenticated)
            {
                //Get the Supplier and Author ids from view
                ViewBag.AuthorList = new SelectList(db.Authors, "AuthorId", "Publisher");
                ViewBag.SupplierList = new SelectList(db.Suppliers, "SupplierId", "SupplierName");
                
                //Get the Add Book view page
                return View(); 
            }
            else 
            {
                //Return the user to the login page if there is no user id
                return RedirectToAction("Login", "Account");
            }
        }

        //Post the book to the web page
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Book book)
        {
            //Check user is logged in
            if (WebSecurity.IsAuthenticated)
            {
                if (ModelState.IsValid)
                {
                    db.Books.Add(book);
                    db.SaveChanges();

                    return RedirectToAction("Index");
                }

                ViewBag.AuthorList = new SelectList(db.Authors, "AuthorId", "Publisher", book.AuthorId);
                ViewBag.SupplierList = new SelectList(db.Suppliers, "SupplierId", "SupplierName", book.SupplierId);
                return View(book);
            }
            else
            {
                //Return the user to the login page if there is no user id
                return RedirectToAction("Login", "Account");
            }              
        }

        //Find the current book from the book id
        public ActionResult Edit(int id = 0)
        {
            //Check user is logged in
            if (WebSecurity.IsAuthenticated)
            {
            	Book book = db.Books.Find(id);

            	if (book == null)
            	{
            	    return HttpNotFound();
            	}

            	ViewBag.AuthorList = new SelectList(db.Authors, "AuthorId", "Publisher", book.AuthorId);
            	ViewBag.SupplierList = new SelectList(db.Suppliers, "SupplierId", "SupplierName", book.SupplierId);
            	return View(book);
            }
            else
            {
                //Return the user to the login page if there is no user id
                return RedirectToAction("Login", "Account");
            }              
        }

        //Allow the user to edit the book if they have login credentials
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Book book)
        {
            //Check user is logged in
            if (WebSecurity.IsAuthenticated)
            {
            	if (ModelState.IsValid)
            	{
            	    db.Entry(book).State = EntityState.Modified;
            	    db.SaveChanges();
            	    return RedirectToAction("Index");
            	}

            	ViewBag.AuthorList = new SelectList(db.Authors, "AuthorId", "Publisher", book.AuthorId);
            	ViewBag.SupplierList = new SelectList(db.Suppliers, "SupplierId", "SupplierName", book.SupplierId);
            	return View(book);
            }
            else
            {
                //Return the user to the login page if there is no user id
                return RedirectToAction("Login", "Account");
            }              
        }

        //Allow the user to delete a book
        public ActionResult Delete(int id = 0)
        {
            //Check user is logged in
            if (WebSecurity.IsAuthenticated)
            {
                //Find the id of the current book and return HTTP error if not found
                Book book = db.Books.Find(id);

         	    if (book == null)
            	{
     	            return HttpNotFound();
            	}

                ViewBag.AuthorList = new SelectList(db.Authors, "AuthorId", "Publisher");
                ViewBag.SupplierList = new SelectList(db.Suppliers, "SupplierId", "SupplierName");
                return View(book);
            }
            else
            {
                //Return the user to the login page if there is no user id
                return RedirectToAction("Login", "Account");
            }              
        }

        //Post the delete book statement and remove selected book from the table
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            //Check user is logged in
            if (WebSecurity.IsAuthenticated)
            {
            	Book book = db.Books.Find(id);

            	db.Books.Remove(book);
            	db.SaveChanges();

            	return RedirectToAction("Index");
            }
            else
            {
                //Return the user to the login page if there is no user id
                return RedirectToAction("Login", "Account");
            }              
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}