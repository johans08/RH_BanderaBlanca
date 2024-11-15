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
    public class tipo_horarioController : Controller
    {
        private BD_BanderaBlancaEntities db = new BD_BanderaBlancaEntities();

        // GET: tipo_horario
        public ActionResult Index()
        {
            return View(db.tipo_horario.ToList());
        }

        // GET: tipo_horario/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tipo_horario tipo_horario = db.tipo_horario.Find(id);
            if (tipo_horario == null)
            {
                return HttpNotFound();
            }
            return View(tipo_horario);
        }

        // GET: tipo_horario/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: tipo_horario/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "idTipo_Horario,Tipo_Horario1")] tipo_horario tipo_horario)
        {
            if (ModelState.IsValid)
            {
                db.tipo_horario.Add(tipo_horario);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(tipo_horario);
        }

        // GET: tipo_horario/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tipo_horario tipo_horario = db.tipo_horario.Find(id);
            if (tipo_horario == null)
            {
                return HttpNotFound();
            }
            return View(tipo_horario);
        }

        // POST: tipo_horario/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "idTipo_Horario,Tipo_Horario1")] tipo_horario tipo_horario)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tipo_horario).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tipo_horario);
        }

        // GET: tipo_horario/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tipo_horario tipo_horario = db.tipo_horario.Find(id);
            if (tipo_horario == null)
            {
                return HttpNotFound();
            }
            return View(tipo_horario);
        }

        // POST: tipo_horario/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            tipo_horario tipo_horario = db.tipo_horario.Find(id);
            db.tipo_horario.Remove(tipo_horario);
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
