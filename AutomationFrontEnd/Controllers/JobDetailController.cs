using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AutomationSuiteFrontEnd.Controllers
{
    public class JobDetailController : Controller
    {
        //
        // GET: /JobDetail/

        public ActionResult Index()
        {
            return View();
        }

        //
        // GET: /JobDetail/Details/5

        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /JobDetail/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /JobDetail/Create

        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /JobDetail/Edit/5

        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /JobDetail/Edit/5

        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /JobDetail/Delete/5

        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /JobDetail/Delete/5

        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
