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
    public class horas_extrasController : Controller
    {
        private BD_BanderaBlancaEntities db = new BD_BanderaBlancaEntities();

        // GET: horas_extras
        public ActionResult Index()
        {
            var horas_extras = db.horas_extras.Include(h => h.catalogo_horas_extras);
            return View(horas_extras.ToList());
        }

        public ActionResult Gestion()
        {
            Persona userSesion = new Persona();
            userSesion = (Persona)Session["user"];

            var horas_extras = db.horas_extras.Include(h => h.catalogo_horas_extras).Where(i => i.idEmpleado.Equals(userSesion.empleados.idEmpleado));
            return View(horas_extras.ToList());
        }

        // GET: horas_extras/Details/5
        public ActionResult Details(DateTime id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            horas_extras horas_extras = db.horas_extras.Find(id);
            if (horas_extras == null)
            {
                return HttpNotFound();
            }
            return View(horas_extras);
        }

        // GET: horas_extras/Create
        public ActionResult Create()
        {
            horas_extras horas = new horas_extras();
            ViewBag.idCatalogo_Horas_Extras = new SelectList(db.catalogo_horas_extras, "idCatalogo_Horas_Extras", "Tipo_Hora_Extra");
            return View(horas);
        }

        // POST: horas_extras/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Fecha_HoraExtra,Cantidad_Horas,Detalle_Hora_Extra,observacion")] horas_extras _horas_extras)
        {
            if (ModelState.IsValid)
            {
                Persona userSesion = new Persona();
                userSesion = (Persona)Session["user"];

                _horas_extras.Aprobada = false;
                _horas_extras.HizoHoras = false;
                _horas_extras.idCatalogo_Horas_Extras = 1;
                _horas_extras.idEmpleado = userSesion.empleados.idEmpleado;

                empleados _empleado = db.empleados.FirstOrDefault(e => e.idEmpleado.Equals(userSesion.empleados.idEmpleado));
                catalogo_horas_extras _tipoHoraExtra = db.catalogo_horas_extras.Find(_horas_extras.idCatalogo_Horas_Extras);
                puestos_laborales _puestoLaboral = db.puestos_laborales.Find(_empleado.idPuestos_Laboral);

                if (_empleado == null || _tipoHoraExtra == null || _puestoLaboral == null)
                {
                    CargarViewBags(_horas_extras);
                    return View(_horas_extras);
                }

                // Calcular monto descontado
                int horasTrabajadasPorDia = 8;
                float salarioDiario = (float)(_puestoLaboral.Salario_Puesto / 30.0 / horasTrabajadasPorDia);
                _horas_extras.Monto_HoraExtra = (_tipoHoraExtra.Porcentaje * salarioDiario) * _horas_extras.Cantidad_Horas;


                db.horas_extras.Add(_horas_extras);
                db.SaveChanges();
                return RedirectToAction("Gestion");
            }

            CargarViewBags(_horas_extras);
            return View(_horas_extras);
        }

        // POST: horas_extras/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Fecha_HoraExtra,idEmpleado,Cantidad_Horas,Detalle_Hora_Extra,Monto_HoraExtra,idCatalogo_Horas_Extras,observacion,aprobada,hizoHoras")] horas_extras horas_extras)
        {
            if (ModelState.IsValid)
            {
                db.Entry(horas_extras).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            CargarViewBags(horas_extras);
            return View(horas_extras);
        }

        // GET: horas_extras/Edit/5
        public ActionResult Edit(DateTime idDate, int idTipoHora, int idEmpleado)
        {
            if (idDate == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            horas_extras horasextra = db.horas_extras.FirstOrDefault(c => c.Fecha_HoraExtra == idDate && c.idCatalogo_Horas_Extras == idTipoHora && c.idEmpleado == idEmpleado);
            if (horasextra == null)
            {
                return HttpNotFound();
            }
            CargarViewBags(horasextra);
            return View(horasextra);
        }

        

        // GET: horas_extras/Delete/5
        public ActionResult Delete(DateTime id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            horas_extras horas_extras = db.horas_extras.Find(id);
            if (horas_extras == null)
            {
                return HttpNotFound();
            }
            return View(horas_extras);
        }

        // POST: horas_extras/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(DateTime id)
        {
            horas_extras horas_extras = db.horas_extras.Find(id);
            db.horas_extras.Remove(horas_extras);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        private void CargarViewBags(horas_extras _horas_extras)
        {
            ViewBag.idEmpleado = new SelectList(
                db.empleados.Include(e => e.personas)
                .Select(e => new
                {
                    e.idEmpleado,
                    NombreCompleto = e.personas.Nombre + " " + e.personas.Primer_Apellido + " " + e.personas.Segundo_Apellido
                }).ToList(),
                "idEmpleado",
                "NombreCompleto", _horas_extras.idEmpleado);
            ViewBag.idCatalogo_Horas_Extras = new SelectList(db.catalogo_horas_extras, "idCatalogo_Horas_Extras", "Tipo_Hora_Extra", _horas_extras.idCatalogo_Horas_Extras);
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
