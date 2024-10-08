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
    public class catalogo_movimientosController : Controller
    {
        private BD_BanderaBlancaEntities db = new BD_BanderaBlancaEntities();

        // GET: catalogo_movimientos
        public ActionResult Index()
        {
            return View(db.catalogo_movimientos.ToList());
        }

        // GET: catalogo_movimientos/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            catalogo_movimientos catalogo_movimientos = db.catalogo_movimientos.Find(id);
            if (catalogo_movimientos == null)
            {
                return HttpNotFound();
            }
            return View(catalogo_movimientos);
        }

        // GET: catalogo_movimientos/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: catalogo_movimientos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "idCatalogo_Movimientos,Descripcion_Movimientos")] catalogo_movimientos catalogo_movimientos)
        {
            if (ModelState.IsValid)
            {
                db.catalogo_movimientos.Add(catalogo_movimientos);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(catalogo_movimientos);
        }

        // GET: catalogo_movimientos/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            catalogo_movimientos catalogo_movimientos = db.catalogo_movimientos.Find(id);
            if (catalogo_movimientos == null)
            {
                return HttpNotFound();
            }
            return View(catalogo_movimientos);
        }

        // POST: catalogo_movimientos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "idCatalogo_Movimientos,Descripcion_Movimientos")] catalogo_movimientos catalogo_movimientos)
        {
            if (ModelState.IsValid)
            {
                db.Entry(catalogo_movimientos).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(catalogo_movimientos);
        }

        // GET: catalogo_movimientos/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            catalogo_movimientos catalogo_movimientos = db.catalogo_movimientos.Find(id);
            if (catalogo_movimientos == null)
            {
                return HttpNotFound();
            }
            return View(catalogo_movimientos);
        }

        // POST: catalogo_movimientos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            catalogo_movimientos catalogo_movimientos = db.catalogo_movimientos.Find(id);
            db.catalogo_movimientos.Remove(catalogo_movimientos);
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
