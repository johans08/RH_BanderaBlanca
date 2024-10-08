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
    public class catalogo_telefonicosController : Controller
    {
        private BD_BanderaBlancaEntities db = new BD_BanderaBlancaEntities();

        // GET: catalogo_telefonicos
        public ActionResult Index()
        {
            return View(db.catalogo_telefonicos.ToList());
        }

        // GET: catalogo_telefonicos/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            catalogo_telefonicos catalogo_telefonicos = db.catalogo_telefonicos.Find(id);
            if (catalogo_telefonicos == null)
            {
                return HttpNotFound();
            }
            return View(catalogo_telefonicos);
        }

        // GET: catalogo_telefonicos/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: catalogo_telefonicos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "idCatalogo_Telefonico,Catalogo_Telefonico")] catalogo_telefonicos catalogo_telefonicos)
        {
            if (ModelState.IsValid)
            {
                db.catalogo_telefonicos.Add(catalogo_telefonicos);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(catalogo_telefonicos);
        }

        // GET: catalogo_telefonicos/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            catalogo_telefonicos catalogo_telefonicos = db.catalogo_telefonicos.Find(id);
            if (catalogo_telefonicos == null)
            {
                return HttpNotFound();
            }
            return View(catalogo_telefonicos);
        }

        // POST: catalogo_telefonicos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "idCatalogo_Telefonico,Catalogo_Telefonico")] catalogo_telefonicos catalogo_telefonicos)
        {
            if (ModelState.IsValid)
            {
                db.Entry(catalogo_telefonicos).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(catalogo_telefonicos);
        }

        // GET: catalogo_telefonicos/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            catalogo_telefonicos catalogo_telefonicos = db.catalogo_telefonicos.Find(id);
            if (catalogo_telefonicos == null)
            {
                return HttpNotFound();
            }
            return View(catalogo_telefonicos);
        }

        // POST: catalogo_telefonicos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            catalogo_telefonicos catalogo_telefonicos = db.catalogo_telefonicos.Find(id);
            db.catalogo_telefonicos.Remove(catalogo_telefonicos);
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
