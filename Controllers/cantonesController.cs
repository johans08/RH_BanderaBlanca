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
    public class cantonesController : Controller
    {
        private BD_BanderaBlancaEntities db = new BD_BanderaBlancaEntities();

        // GET: cantones
        public ActionResult Index()
        {
            var cantones = db.cantones.Include(c => c.provincias);
            return View(cantones.ToList());
        }

        // GET: cantones/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            cantones cantones = db.cantones.Find(id);
            if (cantones == null)
            {
                return HttpNotFound();
            }
            return View(cantones);
        }

        // GET: cantones/Create
        public ActionResult Create()
        {
            ViewBag.idProvincia = new SelectList(db.provincias, "idProvincia", "Nombre_Provincia");
            return View();
        }

        // POST: cantones/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "idCanton,Nombre_Canton,idProvincia")] cantones cantones)
        {
            if (ModelState.IsValid)
            {
                db.cantones.Add(cantones);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.idProvincia = new SelectList(db.provincias, "idProvincia", "Nombre_Provincia", cantones.idProvincia);
            return View(cantones);
        }

        // GET: cantones/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            cantones cantones = db.cantones.Find(id);
            if (cantones == null)
            {
                return HttpNotFound();
            }
            ViewBag.idProvincia = new SelectList(db.provincias, "idProvincia", "Nombre_Provincia", cantones.idProvincia);
            return View(cantones);
        }

        // POST: cantones/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "idCanton,Nombre_Canton,idProvincia")] cantones cantones)
        {
            if (ModelState.IsValid)
            {
                db.Entry(cantones).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.idProvincia = new SelectList(db.provincias, "idProvincia", "Nombre_Provincia", cantones.idProvincia);
            return View(cantones);
        }

        // GET: cantones/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            cantones cantones = db.cantones.Find(id);
            if (cantones == null)
            {
                return HttpNotFound();
            }
            return View(cantones);
        }

        // POST: cantones/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            cantones cantones = db.cantones.Find(id);
            db.cantones.Remove(cantones);
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
