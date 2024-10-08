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
    public class tipo_evaluacionController : Controller
    {
        private BD_BanderaBlancaEntities db = new BD_BanderaBlancaEntities();

        // GET: tipo_evaluacion
        public ActionResult Index()
        {
            return View(db.tipo_evaluacion.ToList());
        }

        // GET: tipo_evaluacion/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tipo_evaluacion tipo_evaluacion = db.tipo_evaluacion.Find(id);
            if (tipo_evaluacion == null)
            {
                return HttpNotFound();
            }
            return View(tipo_evaluacion);
        }

        // GET: tipo_evaluacion/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: tipo_evaluacion/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "idTipo_Evaluacion,Tipo_Evaluacion1")] tipo_evaluacion tipo_evaluacion)
        {
            if (ModelState.IsValid)
            {
                db.tipo_evaluacion.Add(tipo_evaluacion);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(tipo_evaluacion);
        }

        // GET: tipo_evaluacion/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tipo_evaluacion tipo_evaluacion = db.tipo_evaluacion.Find(id);
            if (tipo_evaluacion == null)
            {
                return HttpNotFound();
            }
            return View(tipo_evaluacion);
        }

        // POST: tipo_evaluacion/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "idTipo_Evaluacion,Tipo_Evaluacion1")] tipo_evaluacion tipo_evaluacion)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tipo_evaluacion).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tipo_evaluacion);
        }

        // GET: tipo_evaluacion/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tipo_evaluacion tipo_evaluacion = db.tipo_evaluacion.Find(id);
            if (tipo_evaluacion == null)
            {
                return HttpNotFound();
            }
            return View(tipo_evaluacion);
        }

        // POST: tipo_evaluacion/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            tipo_evaluacion tipo_evaluacion = db.tipo_evaluacion.Find(id);
            db.tipo_evaluacion.Remove(tipo_evaluacion);
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
