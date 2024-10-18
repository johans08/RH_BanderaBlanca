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
    public class incapacidadesController : Controller
    {
        private BD_BanderaBlancaEntities db = new BD_BanderaBlancaEntities();

        // GET: incapacidades
        public ActionResult Index()
        {
            var incapacidades = db.incapacidades.Include(i => i.catalogo_incapacidades);
            return View(incapacidades.ToList());
        }

        public ActionResult Gestion()
        {
            Persona userSesion = new Persona();
            userSesion = (Persona)Session["user"];

            var incapacidades = db.incapacidades.Include(i => i.catalogo_incapacidades).Where(i => i.idEmpleado.Equals(userSesion.empleados.idEmpleado));
            return View(incapacidades.ToList());
        }

        // GET: incapacidades/Details/5
        public ActionResult Details(DateTime id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            incapacidades incapacidades = db.incapacidades.Find(id);
            if (incapacidades == null)
            {
                return HttpNotFound();
            }
            return View(incapacidades);
        }

        // GET: incapacidades/Create
        public ActionResult Create()
        {
            incapacidades incapacidad = new incapacidades();
            ViewBag.idEmpleado = new SelectList(
                db.empleados.Include(e => e.personas)
                .Select(e => new
                {
                    e.idEmpleado,
                    NombreCompleto = e.personas.Nombre + " " + e.personas.Primer_Apellido + " " + e.personas.Segundo_Apellido
                }).ToList(),
                "idEmpleado",
                "NombreCompleto");
            ViewBag.idCatalogo_Incapacidad = new SelectList(db.catalogo_incapacidades, "idCatalogo_Incapacidad", "Descripcion");
            return View(incapacidad);
        }

        private void CargarViewBags(incapacidades _incapacidades)
        {
            ViewBag.idEmpleado = new SelectList(
                db.empleados.Include(e => e.personas)
                .Select(e => new
                {
                    e.idEmpleado,
                    NombreCompleto = e.personas.Nombre + " " + e.personas.Primer_Apellido + " " + e.personas.Segundo_Apellido
                }).ToList(),
                "idEmpleado",
                "NombreCompleto", _incapacidades.idEmpleado);
            ViewBag.idCatalogo_Incapacidad = new SelectList(db.catalogo_incapacidades, "idCatalogo_Incapacidad", "Descripcion");
        }

        // POST: incapacidades/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Fecha_Incapacidad,Fecha_Final_Incapacidad,idCatalogo_Incapacidad,Detalle_Incapacidad, idEmpleado")] incapacidades _incapacidades)
        {
            try
            {
                // Verificar si ya existe una incapacidad para el mismo empleado y dentro del mismo rango de fechas
                var existingIncapacidad = db.incapacidades.FirstOrDefault(i =>
                    i.idEmpleado == _incapacidades.idEmpleado &&
                    i.Fecha_Incapacidad <= _incapacidades.Fecha_Final_Incapacidad &&
                    i.Fecha_Final_Incapacidad >= _incapacidades.Fecha_Incapacidad);

                if (existingIncapacidad != null)
                {
                    ModelState.AddModelError("", "Ya existe una incapacidad dentro del rango de fechas especificado.");
                    CargarViewBags(_incapacidades);
                    return View(_incapacidades);
                }

                // Resto del código para calcular días, salario, y guardar la incapacidad
                TimeSpan diferencia = _incapacidades.Fecha_Final_Incapacidad - _incapacidades.Fecha_Incapacidad;
                int dias = diferencia.Days + 1;

                int finesDeSemana = 0;

                for (DateTime fecha = _incapacidades.Fecha_Incapacidad; fecha <= _incapacidades.Fecha_Final_Incapacidad; fecha = fecha.AddDays(1))
                {
                    if (fecha.DayOfWeek == DayOfWeek.Saturday || fecha.DayOfWeek == DayOfWeek.Sunday)
                    {
                        finesDeSemana++;
                    }
                }

                int diferenciaSinFinesDeSemana = dias - finesDeSemana;

                if (diferenciaSinFinesDeSemana <= 0)
                {
                    ModelState.AddModelError("", "El día final debe ser mayor al día de inicio.");
                }
                else
                {

                    _incapacidades.Cantidad_Dias = diferenciaSinFinesDeSemana;

                    empleados _empleado = db.empleados.FirstOrDefault(e => e.idEmpleado.Equals(_incapacidades.idEmpleado));
                    catalogo_incapacidades _tipoIncapacidad = db.catalogo_incapacidades.Find(_incapacidades.idCatalogo_Incapacidad);
                    puestos_laborales _puestoLaboral = db.puestos_laborales.Find(_empleado.idPuestos_Laboral);

                    if (_empleado == null || _tipoIncapacidad == null || _puestoLaboral == null)
                    {
                        CargarViewBags(_incapacidades);
                        return View(_incapacidades);
                    }

                    // Calcular monto descontado
                    int horasTrabajadasPorDia = 8;
                    float salarioDiario = (float)(_puestoLaboral.Salario_Puesto / 30.0 / horasTrabajadasPorDia);
                    _incapacidades.Monto_Deduccion = (_tipoIncapacidad.Porcentaje_Deduccion * salarioDiario) * _incapacidades.Cantidad_Dias;

                    // Guardar la incapacidad en la base de datos
                    db.incapacidades.Add(_incapacidades);
                    db.SaveChanges();

                    return RedirectToAction("Index");
                }

                CargarViewBags(_incapacidades);
                return View(_incapacidades);
            }
            catch (Exception)
            {
                // Capturar cualquier excepción y mostrar un mensaje genérico
                ModelState.AddModelError("", "No se ha creado la incapacidad, por favor revisa las fechas si ya tienes una incapacidad");
                CargarViewBags(_incapacidades);
                return View(_incapacidades);
            }
        }

        // GET: incapacidades/Edit/5
        public ActionResult Edit(DateTime id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            incapacidades incapacidades = db.incapacidades.Find(id);
            if (incapacidades == null)
            {
                return HttpNotFound();
            }
            ViewBag.idCatalogo_Incapacidad = new SelectList(db.catalogo_incapacidades, "idCatalogo_Incapacidad", "Descripcion", incapacidades.idCatalogo_Incapacidad);
            return View(incapacidades);
        }

        // POST: incapacidades/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Fecha_Incapacidad,Fecha_Final_Incapacidad,idEmpleado,idCatalogo_Incapacidad,Cantidad_Dias,Monto_Deduccion,Detalle_Incapacidad")] incapacidades incapacidades)
        {
            if (ModelState.IsValid)
            {
                db.Entry(incapacidades).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.idCatalogo_Incapacidad = new SelectList(db.catalogo_incapacidades, "idCatalogo_Incapacidad", "Descripcion", incapacidades.idCatalogo_Incapacidad);
            return View(incapacidades);
        }

        // GET: incapacidades/Delete/5
        public ActionResult Delete(DateTime id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            incapacidades incapacidades = db.incapacidades.Find(id);
            if (incapacidades == null)
            {
                return HttpNotFound();
            }
            return View(incapacidades);
        }

        // POST: incapacidades/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(DateTime id)
        {
            incapacidades incapacidades = db.incapacidades.Find(id);
            db.incapacidades.Remove(incapacidades);
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
