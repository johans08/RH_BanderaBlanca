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
    public class liquidacionesController : Controller
    {
        private BD_BanderaBlancaEntities db = new BD_BanderaBlancaEntities();

        // GET: liquidaciones
        public ActionResult Index()
        {

            Persona userSesion = new Persona();
            userSesion = (Persona)Session["user"];

            var liquidaciones = db.liquidaciones.Where(l => l.idEmpleado != userSesion.empleados.idEmpleado).Include(l => l.catalogo_tipo_liquidaciones).Include(l => l.detalle_maestro_liquidaciones).Include(l => l.estados_solicitudes);
            return View(liquidaciones.ToList());
        }

        // GET: liquidaciones/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            liquidaciones liquidaciones = db.liquidaciones.Find(id);
            if (liquidaciones == null)
            {
                return HttpNotFound();
            }
            return View(liquidaciones);
        }

        // GET: liquidaciones/Create
        public ActionResult Create()
        {
            Persona userSesion = new Persona();
            userSesion = (Persona)Session["user"];

            liquidaciones liquidacion = new liquidaciones();
            ViewBag.idEmpleado = new SelectList(db.empleados.Where(e => e.idEmpleado != userSesion.empleados.idEmpleado).Include(e => e.personas).ToList(), "idEmpleado", "personas.Nombre");
            ViewBag.IdTipo_Liquidaciones = new SelectList(db.catalogo_tipo_liquidaciones, "idCatalogo_Tipo_Liquidaciones", "Tipo_Liquidaciones");
            ViewBag.Fecha_Liquidacion = new SelectList(db.detalle_maestro_liquidaciones, "Fecha_Liquidacion", "Fecha_Liquidacion");
            ViewBag.idEstados_Solicitudes = new SelectList(db.estados_solicitudes, "idEstados_Solicitudes", "Estados_Solicitud");

            return View(liquidacion);
        }

        // POST: liquidaciones/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Fecha_Liquidacion,idEmpleado,Preaviso,DiasPreaviso,IdTipo_Liquidaciones,idEstados_Solicitudes, Fecha_Salida")] liquidaciones liquidaciones)
        {
            Persona userSesion = new Persona();
            userSesion = (Persona)Session["user"];


            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Asegurate de llenar bien los datos");
                ViewBag.idEmpleado = new SelectList(db.empleados.Where(e => e.idEmpleado != userSesion.empleados.idEmpleado).Include(e => e.personas).ToList(), "idEmpleado", "personas.Nombre", liquidaciones.idEmpleado);
                ViewBag.IdTipo_Liquidaciones = new SelectList(db.catalogo_tipo_liquidaciones, "idCatalogo_Tipo_Liquidaciones", "Tipo_Liquidaciones", liquidaciones.IdTipo_Liquidaciones);
                ViewBag.Fecha_Liquidacion = new SelectList(db.detalle_maestro_liquidaciones, "Fecha_Liquidacion", "Fecha_Liquidacion", liquidaciones.Fecha_Liquidacion);
                ViewBag.idEstados_Solicitudes = new SelectList(db.estados_solicitudes, "idEstados_Solicitudes", "Estados_Solicitud", liquidaciones.idEstados_Solicitudes);
                return View(liquidaciones);
            }


            // Verificar si el empleado ya tiene una liquidación existente
            var liquidacionExistente = db.liquidaciones
                .FirstOrDefault(l => l.idEmpleado == liquidaciones.idEmpleado);

            if (liquidacionExistente != null)
            {
                // Opcional: Agregar un mensaje de error que indique que el empleado ya tiene una liquidación
                ModelState.AddModelError("", "El empleado ya tiene una liquidación existente.");
                ViewBag.idEmpleado = new SelectList(db.empleados.Where(e => e.idEmpleado != userSesion.empleados.idEmpleado).Include(e => e.personas).ToList(), "idEmpleado", "personas.Nombre", liquidaciones.idEmpleado);
                ViewBag.IdTipo_Liquidaciones = new SelectList(db.catalogo_tipo_liquidaciones, "idCatalogo_Tipo_Liquidaciones", "Tipo_Liquidaciones", liquidaciones.IdTipo_Liquidaciones);
                ViewBag.Fecha_Liquidacion = new SelectList(db.detalle_maestro_liquidaciones, "Fecha_Liquidacion", "Fecha_Liquidacion", liquidaciones.Fecha_Liquidacion);
                ViewBag.idEstados_Solicitudes = new SelectList(db.estados_solicitudes, "idEstados_Solicitudes", "Estados_Solicitud", liquidaciones.idEstados_Solicitudes);
                return View(liquidaciones);
            }



            Liquidacion liquidacion = new Liquidacion();
            bool ok = liquidacion.CrearLiquidacion(liquidaciones);

            if (!ok)
            {
                ModelState.AddModelError("", "El empleado ya tiene una liquidación existente.");
                ViewBag.idEmpleado = new SelectList(db.empleados.Where(e => e.idEmpleado != userSesion.empleados.idEmpleado).Include(e => e.personas).ToList(), "idEmpleado", "personas.Nombre", liquidaciones.idEmpleado);
                ViewBag.IdTipo_Liquidaciones = new SelectList(db.catalogo_tipo_liquidaciones, "idCatalogo_Tipo_Liquidaciones", "Tipo_Liquidaciones", liquidaciones.IdTipo_Liquidaciones);
                ViewBag.Fecha_Liquidacion = new SelectList(db.detalle_maestro_liquidaciones, "Fecha_Liquidacion", "Fecha_Liquidacion", liquidaciones.Fecha_Liquidacion);
                ViewBag.idEstados_Solicitudes = new SelectList(db.estados_solicitudes, "idEstados_Solicitudes", "Estados_Solicitud", liquidaciones.idEstados_Solicitudes);
                return View(liquidaciones);
            }
            return RedirectToAction("Index");

        }

        [HttpPost]
        public ActionResult UpdateEstado(int id, int estado)
        {
            // Buscar la liquidación por ID
            var liquidacion = db.liquidaciones.FirstOrDefault(l => l.Fecha_Liquidacion == id);
            if (liquidacion != null)
            {
                // Actualizar el estado de la liquidación
                liquidacion.idEstados_Solicitudes = estado;
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }


        // GET: liquidaciones/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            liquidaciones liquidaciones = db.liquidaciones.Find(id);
            if (liquidaciones == null)
            {
                return HttpNotFound();
            }
            ViewBag.IdTipo_Liquidaciones = new SelectList(db.catalogo_tipo_liquidaciones, "idCatalogo_Tipo_Liquidaciones", "Tipo_Liquidaciones", liquidaciones.IdTipo_Liquidaciones);
            ViewBag.Fecha_Liquidacion = new SelectList(db.detalle_maestro_liquidaciones, "Fecha_Liquidacion", "Fecha_Liquidacion", liquidaciones.Fecha_Liquidacion);
            ViewBag.idEstados_Solicitudes = new SelectList(db.estados_solicitudes, "idEstados_Solicitudes", "Estados_Solicitud", liquidaciones.idEstados_Solicitudes);
            return View(liquidaciones);
        }

        // POST: liquidaciones/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Fecha_Liquidacion,idEmpleado,Preaviso,DiasPreaviso,IdTipo_Liquidaciones,idEstados_Solicitudes")] liquidaciones liquidaciones)
        {
            if (ModelState.IsValid)
            {
                db.Entry(liquidaciones).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.IdTipo_Liquidaciones = new SelectList(db.catalogo_tipo_liquidaciones, "idCatalogo_Tipo_Liquidaciones", "Tipo_Liquidaciones", liquidaciones.IdTipo_Liquidaciones);
            ViewBag.Fecha_Liquidacion = new SelectList(db.detalle_maestro_liquidaciones, "Fecha_Liquidacion", "Fecha_Liquidacion", liquidaciones.Fecha_Liquidacion);
            ViewBag.idEstados_Solicitudes = new SelectList(db.estados_solicitudes, "idEstados_Solicitudes", "Estados_Solicitud", liquidaciones.idEstados_Solicitudes);
            return View(liquidaciones);
        }

        // GET: liquidaciones/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            liquidaciones liquidaciones = db.liquidaciones.Find(id);
            if (liquidaciones == null)
            {
                return HttpNotFound();
            }
            return View(liquidaciones);
        }

        // POST: liquidaciones/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            liquidaciones liquidaciones = db.liquidaciones.Find(id);
            db.liquidaciones.Remove(liquidaciones);
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
