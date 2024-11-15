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
    public class diasController : Controller
    {
        private BD_BanderaBlancaEntities db = new BD_BanderaBlancaEntities();

        // GET: dias
        public ActionResult Index()
        {
            return View(db.dias.ToList());
        }

        // GET: dias/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            dias dias = db.dias.Find(id);
            if (dias == null)
            {
                return HttpNotFound();
            }
            return View(dias);
        }

        // GET: dias/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: dias/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "idDias,Nombre_Dia")] dias dias)
        {
            if (ModelState.IsValid)
            {
                db.dias.Add(dias);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(dias);
        }

        // GET: dias/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            dias dias = db.dias.Find(id);
            if (dias == null)
            {
                return HttpNotFound();
            }
            return View(dias);
        }

        // POST: dias/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "idDias,Nombre_Dia")] dias dias)
        {
            if (ModelState.IsValid)
            {
                db.Entry(dias).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(dias);
        }

        // GET: dias/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            dias dias = db.dias.Find(id);
            if (dias == null)
            {
                return HttpNotFound();
            }
            return View(dias);
        }

        // POST: dias/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            dias dias = db.dias.Find(id);
            db.dias.Remove(dias);
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
