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
    public class catalogo_tipo_liquidacionesController : Controller
    {
        private BD_BanderaBlancaEntities db = new BD_BanderaBlancaEntities();

        // GET: catalogo_tipo_liquidaciones
        public ActionResult Index()
        {
            return View(db.catalogo_tipo_liquidaciones.ToList());
        }

        // GET: catalogo_tipo_liquidaciones/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            catalogo_tipo_liquidaciones catalogo_tipo_liquidaciones = db.catalogo_tipo_liquidaciones.Find(id);
            if (catalogo_tipo_liquidaciones == null)
            {
                return HttpNotFound();
            }
            return View(catalogo_tipo_liquidaciones);
        }

        // GET: catalogo_tipo_liquidaciones/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: catalogo_tipo_liquidaciones/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "idCatalogo_Tipo_Liquidaciones,Tipo_Liquidaciones")] catalogo_tipo_liquidaciones catalogo_tipo_liquidaciones)
        {
            if (ModelState.IsValid)
            {
                db.catalogo_tipo_liquidaciones.Add(catalogo_tipo_liquidaciones);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(catalogo_tipo_liquidaciones);
        }

        // GET: catalogo_tipo_liquidaciones/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            catalogo_tipo_liquidaciones catalogo_tipo_liquidaciones = db.catalogo_tipo_liquidaciones.Find(id);
            if (catalogo_tipo_liquidaciones == null)
            {
                return HttpNotFound();
            }
            return View(catalogo_tipo_liquidaciones);
        }

        // POST: catalogo_tipo_liquidaciones/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "idCatalogo_Tipo_Liquidaciones,Tipo_Liquidaciones")] catalogo_tipo_liquidaciones catalogo_tipo_liquidaciones)
        {
            if (ModelState.IsValid)
            {
                db.Entry(catalogo_tipo_liquidaciones).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(catalogo_tipo_liquidaciones);
        }

        // GET: catalogo_tipo_liquidaciones/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            catalogo_tipo_liquidaciones catalogo_tipo_liquidaciones = db.catalogo_tipo_liquidaciones.Find(id);
            if (catalogo_tipo_liquidaciones == null)
            {
                return HttpNotFound();
            }
            return View(catalogo_tipo_liquidaciones);
        }

        // POST: catalogo_tipo_liquidaciones/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            catalogo_tipo_liquidaciones catalogo_tipo_liquidaciones = db.catalogo_tipo_liquidaciones.Find(id);
            db.catalogo_tipo_liquidaciones.Remove(catalogo_tipo_liquidaciones);
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
