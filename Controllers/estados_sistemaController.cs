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
    public class estados_sistemaController : Controller
    {
        private BD_BanderaBlancaEntities db = new BD_BanderaBlancaEntities();

        // GET: estados_sistema
        public ActionResult Index()
        {
            return View(db.estados_sistema.ToList());
        }

        // GET: estados_sistema/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            estados_sistema estados_sistema = db.estados_sistema.Find(id);
            if (estados_sistema == null)
            {
                return HttpNotFound();
            }
            return View(estados_sistema);
        }

        // GET: estados_sistema/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: estados_sistema/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "idEstado,Estado")] estados_sistema estados_sistema)
        {
            if (ModelState.IsValid)
            {
                db.estados_sistema.Add(estados_sistema);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(estados_sistema);
        }

        // GET: estados_sistema/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            estados_sistema estados_sistema = db.estados_sistema.Find(id);
            if (estados_sistema == null)
            {
                return HttpNotFound();
            }
            return View(estados_sistema);
        }

        // POST: estados_sistema/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "idEstado,Estado")] estados_sistema estados_sistema)
        {
            if (ModelState.IsValid)
            {
                db.Entry(estados_sistema).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(estados_sistema);
        }

        // GET: estados_sistema/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            estados_sistema estados_sistema = db.estados_sistema.Find(id);
            if (estados_sistema == null)
            {
                return HttpNotFound();
            }
            return View(estados_sistema);
        }

        // POST: estados_sistema/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            estados_sistema estados_sistema = db.estados_sistema.Find(id);
            db.estados_sistema.Remove(estados_sistema);
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
