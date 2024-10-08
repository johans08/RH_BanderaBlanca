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
    public class catalogo_identificadorController : Controller
    {
        private BD_BanderaBlancaEntities db = new BD_BanderaBlancaEntities();

        // GET: catalogo_identificador
        public ActionResult Index()
        {
            return View(db.catalogo_identificador.ToList());
        }

        // GET: catalogo_identificador/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            catalogo_identificador catalogo_identificador = db.catalogo_identificador.Find(id);
            if (catalogo_identificador == null)
            {
                return HttpNotFound();
            }
            return View(catalogo_identificador);
        }

        // GET: catalogo_identificador/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: catalogo_identificador/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "idCatalogo_Identificador,Descripcion_Identificador")] catalogo_identificador catalogo_identificador)
        {
            if (ModelState.IsValid)
            {
                db.catalogo_identificador.Add(catalogo_identificador);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(catalogo_identificador);
        }

        // GET: catalogo_identificador/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            catalogo_identificador catalogo_identificador = db.catalogo_identificador.Find(id);
            if (catalogo_identificador == null)
            {
                return HttpNotFound();
            }
            return View(catalogo_identificador);
        }

        // POST: catalogo_identificador/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "idCatalogo_Identificador,Descripcion_Identificador")] catalogo_identificador catalogo_identificador)
        {
            if (ModelState.IsValid)
            {
                db.Entry(catalogo_identificador).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(catalogo_identificador);
        }

        // GET: catalogo_identificador/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            catalogo_identificador catalogo_identificador = db.catalogo_identificador.Find(id);
            if (catalogo_identificador == null)
            {
                return HttpNotFound();
            }
            return View(catalogo_identificador);
        }

        // POST: catalogo_identificador/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            catalogo_identificador catalogo_identificador = db.catalogo_identificador.Find(id);
            db.catalogo_identificador.Remove(catalogo_identificador);
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
