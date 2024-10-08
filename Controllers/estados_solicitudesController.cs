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
    public class estados_solicitudesController : Controller
    {
        private BD_BanderaBlancaEntities db = new BD_BanderaBlancaEntities();

        // GET: estados_solicitudes
        public ActionResult Index()
        {
            return View(db.estados_solicitudes.ToList());
        }

        // GET: estados_solicitudes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            estados_solicitudes estados_solicitudes = db.estados_solicitudes.Find(id);
            if (estados_solicitudes == null)
            {
                return HttpNotFound();
            }
            return View(estados_solicitudes);
        }

        // GET: estados_solicitudes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: estados_solicitudes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "idEstados_Solicitudes,Estados_Solicitud")] estados_solicitudes estados_solicitudes)
        {
            if (ModelState.IsValid)
            {
                db.estados_solicitudes.Add(estados_solicitudes);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(estados_solicitudes);
        }

        // GET: estados_solicitudes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            estados_solicitudes estados_solicitudes = db.estados_solicitudes.Find(id);
            if (estados_solicitudes == null)
            {
                return HttpNotFound();
            }
            return View(estados_solicitudes);
        }

        // POST: estados_solicitudes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "idEstados_Solicitudes,Estados_Solicitud")] estados_solicitudes estados_solicitudes)
        {
            if (ModelState.IsValid)
            {
                db.Entry(estados_solicitudes).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(estados_solicitudes);
        }

        // GET: estados_solicitudes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            estados_solicitudes estados_solicitudes = db.estados_solicitudes.Find(id);
            if (estados_solicitudes == null)
            {
                return HttpNotFound();
            }
            return View(estados_solicitudes);
        }

        // POST: estados_solicitudes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            estados_solicitudes estados_solicitudes = db.estados_solicitudes.Find(id);
            db.estados_solicitudes.Remove(estados_solicitudes);
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
