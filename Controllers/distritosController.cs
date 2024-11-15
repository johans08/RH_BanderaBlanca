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
    public class distritosController : Controller
    {
        private BD_BanderaBlancaEntities db = new BD_BanderaBlancaEntities();

        // GET: distritos
        public ActionResult Index()
        {
            var distritos = db.distritos.Include(d => d.cantones);
            return View(distritos.ToList());
        }

        // GET: distritos/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            distritos distritos = db.distritos.Find(id);
            if (distritos == null)
            {
                return HttpNotFound();
            }
            return View(distritos);
        }

        // GET: distritos/Create
        public ActionResult Create()
        {
            ViewBag.idProvincia = new SelectList(db.provincias, "idProvincia", "Nombre_Provincia");
            ViewBag.idCanton = new SelectList(db.cantones, "idCanton", "Nombre_Canton");
            return View();
        }

        // POST: distritos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "idDistrito,Nombre_Distrito,idCanton,idProvincia")] distritos distritos)
        {
            if (ModelState.IsValid)
            {
                db.distritos.Add(distritos);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.idProvincia = new SelectList(db.provincias, "idProvincia", "Nombre_Provincia", distritos.idProvincia);
            ViewBag.idCanton = new SelectList(db.cantones, "idCanton", "Nombre_Canton", distritos.idCanton);
            return View(distritos);
        }

        // GET: distritos/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            distritos distritos = db.distritos.Find(id);
            if (distritos == null)
            {
                return HttpNotFound();
            }
            ViewBag.idProvincia = new SelectList(db.provincias, "idProvincia", "Nombre_Provincia", distritos.idProvincia);
            ViewBag.idCanton = new SelectList(db.cantones, "idCanton", "Nombre_Canton", distritos.idCanton);
            return View(distritos);
        }

        // POST: distritos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "idDistrito,Nombre_Distrito,idCanton,idProvincia")] distritos distritos)
        {
            if (ModelState.IsValid)
            {
                db.Entry(distritos).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.idProvincia = new SelectList(db.provincias, "idProvincia", "Nombre_Provincia", distritos.idProvincia);
            ViewBag.idCanton = new SelectList(db.cantones, "idCanton", "Nombre_Canton", distritos.idCanton);
            return View(distritos);
        }

        // GET: distritos/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            distritos distritos = db.distritos.Find(id);
            if (distritos == null)
            {
                return HttpNotFound();
            }
            return View(distritos);
        }

        // POST: distritos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            distritos distritos = db.distritos.Find(id);
            db.distritos.Remove(distritos);
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
