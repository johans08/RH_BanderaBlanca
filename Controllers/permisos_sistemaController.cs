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
    public class permisos_sistemaController : Controller
    {
        private BD_BanderaBlancaEntities db = new BD_BanderaBlancaEntities();

        // GET: permisos_sistema
        public ActionResult Index()
        {
            return View(db.permisos_sistema.ToList());
        }

        // GET: permisos_sistema/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            permisos_sistema permisos_sistema = db.permisos_sistema.Find(id);
            if (permisos_sistema == null)
            {
                return HttpNotFound();
            }
            return View(permisos_sistema);
        }

        // GET: permisos_sistema/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: permisos_sistema/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "idPermisos_Sistema,Detalle_Permiso_Sistema")] permisos_sistema permisos_sistema)
        {
            if (ModelState.IsValid)
            {
                db.permisos_sistema.Add(permisos_sistema);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(permisos_sistema);
        }

        // GET: permisos_sistema/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            permisos_sistema permisos_sistema = db.permisos_sistema.Find(id);
            if (permisos_sistema == null)
            {
                return HttpNotFound();
            }
            return View(permisos_sistema);
        }

        // POST: permisos_sistema/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "idPermisos_Sistema,Detalle_Permiso_Sistema")] permisos_sistema permisos_sistema)
        {
            if (ModelState.IsValid)
            {
                db.Entry(permisos_sistema).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(permisos_sistema);
        }

        // GET: permisos_sistema/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            permisos_sistema permisos_sistema = db.permisos_sistema.Find(id);
            if (permisos_sistema == null)
            {
                return HttpNotFound();
            }
            return View(permisos_sistema);
        }

        // POST: permisos_sistema/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            permisos_sistema permisos_sistema = db.permisos_sistema.Find(id);
            db.permisos_sistema.Remove(permisos_sistema);
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
