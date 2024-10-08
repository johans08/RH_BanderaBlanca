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
    public class catalogo_horas_extrasController : Controller
    {
        private BD_BanderaBlancaEntities db = new BD_BanderaBlancaEntities();

        // GET: catalogo_horas_extras
        public ActionResult Index()
        {
            return View(db.catalogo_horas_extras.ToList());
        }

        // GET: catalogo_horas_extras/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            catalogo_horas_extras catalogo_horas_extras = db.catalogo_horas_extras.Find(id);
            if (catalogo_horas_extras == null)
            {
                return HttpNotFound();
            }
            return View(catalogo_horas_extras);
        }

        // GET: catalogo_horas_extras/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: catalogo_horas_extras/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "idCatalogo_Horas_Extras,Tipo_Hora_Extra,Porcentaje")] catalogo_horas_extras catalogo_horas_extras)
        {
            if (ModelState.IsValid)
            {
                db.catalogo_horas_extras.Add(catalogo_horas_extras);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(catalogo_horas_extras);
        }

        // GET: catalogo_horas_extras/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            catalogo_horas_extras catalogo_horas_extras = db.catalogo_horas_extras.Find(id);
            if (catalogo_horas_extras == null)
            {
                return HttpNotFound();
            }
            return View(catalogo_horas_extras);
        }

        // POST: catalogo_horas_extras/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "idCatalogo_Horas_Extras,Tipo_Hora_Extra,Porcentaje")] catalogo_horas_extras catalogo_horas_extras)
        {
            if (ModelState.IsValid)
            {
                db.Entry(catalogo_horas_extras).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(catalogo_horas_extras);
        }

        // GET: catalogo_horas_extras/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            catalogo_horas_extras catalogo_horas_extras = db.catalogo_horas_extras.Find(id);
            if (catalogo_horas_extras == null)
            {
                return HttpNotFound();
            }
            return View(catalogo_horas_extras);
        }

        // POST: catalogo_horas_extras/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            catalogo_horas_extras catalogo_horas_extras = db.catalogo_horas_extras.Find(id);
            db.catalogo_horas_extras.Remove(catalogo_horas_extras);
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
