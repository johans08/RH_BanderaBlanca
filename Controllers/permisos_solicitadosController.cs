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
    public class permisos_solicitadosController : Controller
    {
        private BD_BanderaBlancaEntities db = new BD_BanderaBlancaEntities();

        // GET: permisos_solicitados
        public ActionResult Index()
        {
            var permisos_solicitados = db.permisos_solicitados.Include(p => p.estados_solicitudes);
            return View(permisos_solicitados.ToList());
        }

        public ActionResult Gestion()
        {
            Persona userSesion = new Persona();
            userSesion = (Persona)Session["user"];

            var permisos_solicitados = db.permisos_solicitados.Include(p => p.estados_solicitudes).Where(i => i.idEmpleado.Equals(userSesion.empleados.idEmpleado));
            return View(permisos_solicitados.ToList());
        }

        // GET: permisos_solicitados/Details/5
        public ActionResult Details(DateTime id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            permisos_solicitados permisos_solicitados = db.permisos_solicitados.Find(id);
            if (permisos_solicitados == null)
            {
                return HttpNotFound();
            }
            return View(permisos_solicitados);
        }



        // GET: permisos_solicitados/Create
        public ActionResult Create()
        {
            permisos_solicitados permisos = new permisos_solicitados();

            ViewBag.idEstados_Solicitudes = new SelectList(db.estados_solicitudes, "idEstados_Solicitudes", "Estados_Solicitud");
            return View(permisos);
        }

        // POST: permisos_solicitados/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Fecha_Permiso,Detalle_Permiso")] permisos_solicitados permisos_solicitados)
        {

            if (ModelState.IsValid)
            {
                Persona userSesion = new Persona();
                userSesion = (Persona)Session["user"];

                permisos_solicitados.Fecha_Solicitud = DateTime.Now;
                permisos_solicitados.descontado = false;

                // Validación de fecha pasada
                if (permisos_solicitados.Fecha_Permiso < DateTime.Today)
                {
                    ModelState.AddModelError("FechaPermiso", "No se puede crear una solicitud para una fecha pasada.");
                }

                // Validación de duplicado en el mismo período o fecha
                bool existePermiso = db.permisos_solicitados.Any(p =>
                    p.idEmpleado == userSesion.empleados.idEmpleado &&
                    DbFunctions.TruncateTime(p.Fecha_Permiso) == permisos_solicitados.Fecha_Permiso.Date);

                if (existePermiso)
                {
                    ModelState.AddModelError("FechaPermiso", "Ya existe una solicitud de permiso para esta fecha.");
                }

                if (!ModelState.IsValid)
                {
                    CargarViewBags(permisos_solicitados);
                    return View(permisos_solicitados);
                }

                permisos_solicitados.idEstados_Solicitudes = 3;
                permisos_solicitados.idEmpleado = userSesion.empleados.idEmpleado;
                permisos_solicitados.descontado = false;

                db.permisos_solicitados.Add(permisos_solicitados);
                db.SaveChanges();
                return RedirectToAction("Gestion");
            }

            CargarViewBags(permisos_solicitados);
            return View(permisos_solicitados);

            
        }

        // GET: permisos_solicitados/Edit/5
        public ActionResult Edit(DateTime date, int idEmpleado)
        {
            if (date == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            permisos_solicitados permisosolicitado = db.permisos_solicitados.FirstOrDefault(p => p.Fecha_Permiso == date && p.idEmpleado == idEmpleado);
            if (permisosolicitado == null)
            {
                return HttpNotFound();
            }
            CargarViewBags(permisosolicitado);
            return View(permisosolicitado);
        }

        // POST: permisos_solicitados/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Fecha_Permiso,idEmpleado,idEstados_Solicitudes,Detalle_Permiso,Fecha_Solicitud,observacion,descontado")] permisos_solicitados permisos_solicitados)
        {
            if (ModelState.IsValid)
            {
                db.Entry(permisos_solicitados).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            CargarViewBags(permisos_solicitados);

            return View(permisos_solicitados);
        }

        // GET: permisos_solicitados/Delete/5
        public ActionResult Delete(DateTime id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            permisos_solicitados permisos_solicitados = db.permisos_solicitados.Find(id);
            if (permisos_solicitados == null)
            {
                return HttpNotFound();
            }
            return View(permisos_solicitados);
        }

        // POST: permisos_solicitados/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(DateTime id)
        {
            permisos_solicitados permisos_solicitados = db.permisos_solicitados.Find(id);
            db.permisos_solicitados.Remove(permisos_solicitados);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        private void CargarViewBags(permisos_solicitados _permisos_solicitados)
        {
            ViewBag.idEmpleado = new SelectList(
                db.empleados.Include(e => e.personas)
                .Select(e => new
                {
                    e.idEmpleado,
                    NombreCompleto = e.personas.Nombre + " " + e.personas.Primer_Apellido + " " + e.personas.Segundo_Apellido
                }).ToList(),
                "idEmpleado",
                "NombreCompleto", _permisos_solicitados.idEmpleado);
            ViewBag.idEstados_Solicitudes = new SelectList(db.estados_solicitudes, "idEstados_Solicitudes", "Estados_Solicitud", _permisos_solicitados.idEstados_Solicitudes);
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
