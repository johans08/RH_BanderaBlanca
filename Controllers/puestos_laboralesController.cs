using System;
using System.Collections.Generic; using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DataBase_RH_BanderaBlanca.Models;

namespace RH_BanderaBlanca.Controllers
{
    public class puestos_laboralesController : Controller
    {
        private BD_BanderaBlancaEntities db = new BD_BanderaBlancaEntities();

        // GET: puestos_laborales
        public ActionResult Index()
        {
            return View(db.puestos_laborales.ToList());
        }

        // GET: puestos_laborales/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            puestos_laborales puestos_laborales = db.puestos_laborales.Find(id);
            if (puestos_laborales == null)
            {
                return HttpNotFound();
            }
            return View(puestos_laborales);
        }

        // GET: puestos_laborales/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: puestos_laborales/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "idPuestos_Laboral,Nombre_Puesto,Salario_Puesto")] puestos_laborales puestos_laborales)
        {
            if (ModelState.IsValid)
            {
                db.puestos_laborales.Add(puestos_laborales);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(puestos_laborales);
        }

        // GET: puestos_laborales/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            puestos_laborales puestos_laborales = db.puestos_laborales.Find(id);
            if (puestos_laborales == null)
            {
                return HttpNotFound();
            }
            return View(puestos_laborales);
        }

        // POST: puestos_laborales/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "idPuestos_Laboral,Nombre_Puesto,Salario_Puesto")] puestos_laborales puestos_laborales)
        {
            if (ModelState.IsValid)
            {
                db.Entry(puestos_laborales).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(puestos_laborales);
        }

        // GET: puestos_laborales/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            puestos_laborales puestos_laborales = db.puestos_laborales.Find(id);
            if (puestos_laborales == null)
            {
                return HttpNotFound();
            }
            return View(puestos_laborales);
        }

        // POST: puestos_laborales/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            puestos_laborales puestos_laborales = db.puestos_laborales.Find(id);
            db.puestos_laborales.Remove(puestos_laborales);
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
