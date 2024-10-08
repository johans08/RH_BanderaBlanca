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
    public class catalogo_incapacidadesController : Controller
    {
        private BD_BanderaBlancaEntities db = new BD_BanderaBlancaEntities();

        // GET: catalogo_incapacidades
        public ActionResult Index()
        {
            return View(db.catalogo_incapacidades.ToList());
        }

        // GET: catalogo_incapacidades/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            catalogo_incapacidades catalogo_incapacidades = db.catalogo_incapacidades.Find(id);
            if (catalogo_incapacidades == null)
            {
                return HttpNotFound();
            }
            return View(catalogo_incapacidades);
        }

        // GET: catalogo_incapacidades/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: catalogo_incapacidades/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "idCatalogo_Incapacidad,Descripcion,Porcentaje_Deduccion")] catalogo_incapacidades catalogo_incapacidades)
        {
            if (ModelState.IsValid)
            {
                db.catalogo_incapacidades.Add(catalogo_incapacidades);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(catalogo_incapacidades);
        }

        // GET: catalogo_incapacidades/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            catalogo_incapacidades catalogo_incapacidades = db.catalogo_incapacidades.Find(id);
            if (catalogo_incapacidades == null)
            {
                return HttpNotFound();
            }
            return View(catalogo_incapacidades);
        }

        // POST: catalogo_incapacidades/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "idCatalogo_Incapacidad,Descripcion,Porcentaje_Deduccion")] catalogo_incapacidades catalogo_incapacidades)
        {
            if (ModelState.IsValid)
            {
                db.Entry(catalogo_incapacidades).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(catalogo_incapacidades);
        }

        // GET: catalogo_incapacidades/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            catalogo_incapacidades catalogo_incapacidades = db.catalogo_incapacidades.Find(id);
            if (catalogo_incapacidades == null)
            {
                return HttpNotFound();
            }
            return View(catalogo_incapacidades);
        }

        // POST: catalogo_incapacidades/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            catalogo_incapacidades catalogo_incapacidades = db.catalogo_incapacidades.Find(id);
            db.catalogo_incapacidades.Remove(catalogo_incapacidades);
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
