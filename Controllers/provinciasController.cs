using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DataBase_RH_BanderaBlanca.Models;

namespace RH_BanderaBlanca.Controllers
{
    public class provinciasController : Controller
    {
        private BD_BanderaBlancaEntities db = new BD_BanderaBlancaEntities();

        // GET: provincias
        public ActionResult Index()
        {
            return View(db.provincias.ToList());
        }

        // GET: provincias/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            provincias provincias = db.provincias.Find(id);
            if (provincias == null)
            {
                return HttpNotFound();
            }
            return View(provincias);
        }

        // GET: provincias/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: provincias/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "idProvincia,Nombre_Provincia")] provincias provincias)
        {
            if (ModelState.IsValid)
            {
                db.provincias.Add(provincias);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(provincias);
        }

        // GET: provincias/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            provincias provincias = db.provincias.Find(id);
            if (provincias == null)
            {
                return HttpNotFound();
            }
            return View(provincias);
        }

        // POST: provincias/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "idProvincia,Nombre_Provincia")] provincias provincias)
        {
            if (ModelState.IsValid)
            {
                db.Entry(provincias).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(provincias);
        }

        // GET: provincias/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            provincias provincias = db.provincias.Find(id);
            if (provincias == null)
            {
                return HttpNotFound();
            }
            return View(provincias);
        }

        // POST: provincias/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            provincias provincias = db.provincias.Find(id);
            db.provincias.Remove(provincias);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
