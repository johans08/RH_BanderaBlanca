using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DataBase_RH_BanderaBlanca.Models;
using ZXing;

namespace RH_BanderaBlanca.Controllers
{
    public class marcas_tiempoController : Controller
    {
        private BD_BanderaBlancaEntities db = new BD_BanderaBlancaEntities();

        [HttpPost]
        public ActionResult ValidarMarca(int identificador)
        {
            try
            {
                int tipoMarca = 1;
                DateTime hora = DateTime.Now;

                Qr qr = new Qr();
                bool ok = qr.ValidarQR(identificador);

                if (ok)
                {
                    string diaHoy = ObtenerNombreDiaSemana(DateTime.Today);
                    empleados _empleado = db.empleados.FirstOrDefault(e => e.Personas_Identificador.Equals(identificador));
                    horarios _horario = db.horarios.FirstOrDefault(t => t.idEmpleado == _empleado.idEmpleado && t.dias.Nombre_Dia == diaHoy);

                    marcas_tiempo _marcas_Tiempo = db.marcas_tiempo.FirstOrDefault(m => m.idEmpleado.Equals(_empleado.idEmpleado) && m.Fecha_Marca.Equals(DateTime.Today));

                    if (_marcas_Tiempo != null)
                    {
                        tipoMarca = 2;
                    }

                    _marcas_Tiempo =  new marcas_tiempo
                    { 
                        Fecha_Marca = DateTime.Today,
                        idCatalogo_Movimientos = tipoMarca,
                        idEmpleado = _empleado.idEmpleado,
                        Marca_Hora = DateTime.Now.TimeOfDay,
                        Justificada = false,
                        Tardia = ValidarTardia(_horario, DateTime.Now.TimeOfDay)
                    };
                    db.marcas_tiempo.Add(_marcas_Tiempo);
                    db.SaveChanges();
                }
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        private bool ValidarTardia(horarios _horario, TimeSpan marca)
        {
            bool ok = false;

            if (_horario != null && _horario.Hora_Entrada <= marca)
            {
                ok = true;
            }

            return ok;
        }


        string ObtenerNombreDiaSemana(DateTime fecha)
        {
            switch (fecha.DayOfWeek)
            {
                case DayOfWeek.Sunday:
                    return "Domingo";
                case DayOfWeek.Monday:
                    return "Lunes";
                case DayOfWeek.Tuesday:
                    return "Martes";
                case DayOfWeek.Wednesday:
                    return "Miercoles";
                case DayOfWeek.Thursday:
                    return "Jueves";
                case DayOfWeek.Friday:
                    return "Viernes";
                case DayOfWeek.Saturday:
                    return "Sabado";
                default:
                    return string.Empty; // En caso de error o fecha inválida
            }
        }

        // GET: marcas_tiempo
        public ActionResult Index()
        {
            var marcas_tiempo = db.marcas_tiempo.Include(m => m.catalogo_movimientos);
            return View(marcas_tiempo.ToList());
        }

        // GET: marcas_tiempo/Details/5
        public ActionResult Details(DateTime id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            marcas_tiempo marcas_tiempo = db.marcas_tiempo.Find(id);
            if (marcas_tiempo == null)
            {
                return HttpNotFound();
            }
            return View(marcas_tiempo);
        }

        // GET: marcas_tiempo/Create
        public ActionResult Create()
        {
            ViewBag.idCatalogo_Movimientos = new SelectList(db.catalogo_movimientos, "idCatalogo_Movimientos", "Descripcion_Movimientos");
            return View();
        }

        // POST: marcas_tiempo/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Fecha_Marca,idEmpleado,idCatalogo_Movimientos,Marca_Hora,Tardia,Justificada")] marcas_tiempo marcas_tiempo)
        {
            if (ModelState.IsValid)
            {
                db.marcas_tiempo.Add(marcas_tiempo);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.idCatalogo_Movimientos = new SelectList(db.catalogo_movimientos, "idCatalogo_Movimientos", "Descripcion_Movimientos", marcas_tiempo.idCatalogo_Movimientos);
            return View(marcas_tiempo);
        }

        // GET: marcas_tiempo/Edit/5
        public ActionResult Edit(DateTime id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            marcas_tiempo marcas_tiempo = db.marcas_tiempo.Find(id);
            if (marcas_tiempo == null)
            {
                return HttpNotFound();
            }
            ViewBag.idCatalogo_Movimientos = new SelectList(db.catalogo_movimientos, "idCatalogo_Movimientos", "Descripcion_Movimientos", marcas_tiempo.idCatalogo_Movimientos);
            return View(marcas_tiempo);
        }

        // POST: marcas_tiempo/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Fecha_Marca,idEmpleado,idCatalogo_Movimientos,Marca_Hora,Tardia,Justificada")] marcas_tiempo marcas_tiempo)
        {
            if (ModelState.IsValid)
            {
                db.Entry(marcas_tiempo).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.idCatalogo_Movimientos = new SelectList(db.catalogo_movimientos, "idCatalogo_Movimientos", "Descripcion_Movimientos", marcas_tiempo.idCatalogo_Movimientos);
            return View(marcas_tiempo);
        }

        // GET: marcas_tiempo/Delete/5
        public ActionResult Delete(DateTime id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            marcas_tiempo marcas_tiempo = db.marcas_tiempo.Find(id);
            if (marcas_tiempo == null)
            {
                return HttpNotFound();
            }
            return View(marcas_tiempo);
        }

        // POST: marcas_tiempo/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(DateTime id)
        {
            marcas_tiempo marcas_tiempo = db.marcas_tiempo.Find(id);
            db.marcas_tiempo.Remove(marcas_tiempo);
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
