using System;
using System.Collections.Generic; using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
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
                DateTime horaActual = DateTime.Now;

                Qr qr = new Qr();
                bool ok = qr.ValidarQR(identificador);

                if (ok)
                {
                    string diaHoy = ObtenerNombreDiaSemana(DateTime.Today);
                    empleados _empleado = db.empleados.FirstOrDefault(e => e.Personas_Identificador.Equals(identificador));
                    horarios _horario = db.horarios.FirstOrDefault(t => t.idEmpleado == _empleado.idEmpleado && t.dias.Nombre_Dia == diaHoy);

                    // Obtener la última marca del empleado
                    marcas_tiempo ultimaMarca = db.marcas_tiempo
                        .Where(m => m.idEmpleado == _empleado.idEmpleado)
                        .OrderByDescending(m => m.Fecha_Marca).ThenByDescending(m => m.Marca_Hora)
                        .FirstOrDefault();

                    // Definir el tiempo de espera entre marcas (ejemplo: 10 minutos)
                    int tiempoEsperaMinutos = 10;

                    // Verificar si existe una marca reciente dentro del período de espera
                    if (ultimaMarca != null)
                    {
                        TimeSpan tiempoTranscurrido = horaActual - ultimaMarca.Fecha_Marca.Add(ultimaMarca.Marca_Hora);
                        if (tiempoTranscurrido.TotalMinutes < tiempoEsperaMinutos)
                        {
                            return Json(new { success = false, message = $"Debe esperar al menos {tiempoEsperaMinutos} minutos antes de marcar nuevamente." });
                        }
                    }

                    // Determinar el tipo de marca (si ya tiene una marca registrada hoy, se asume salida)
                    if (ultimaMarca != null && ultimaMarca.Fecha_Marca == DateTime.Today)
                    {
                        tipoMarca = 2;
                    }

                    if (ultimaMarca != null && ultimaMarca.idCatalogo_Movimientos.Equals(tipoMarca))
                    {
                        return Json(new { success = false, message = $"Ya has marcado tu salida y entrada el dia de hoy" });
                    }
                

                    // Registrar la nueva marca
                    marcas_tiempo _marcas_Tiempo = new marcas_tiempo
                    {
                        Fecha_Marca = DateTime.Today,
                        idCatalogo_Movimientos = tipoMarca,
                        idEmpleado = _empleado.idEmpleado,
                        Marca_Hora = horaActual.TimeOfDay,
                        Justificada = false,
                        Tardia = ValidarTardia(_horario, horaActual.TimeOfDay)
                    };
                    db.marcas_tiempo.Add(_marcas_Tiempo);
                    db.SaveChanges();

                    TieneHorasExtra(_empleado.idEmpleado, DateTime.Today, _marcas_Tiempo.Marca_Hora, _horario, tipoMarca);


                    return Json(new { success = true });
                }

                return Json(new { success = false, message = "El código QR no es válido." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        private void TieneHorasExtra(int idEmpleado, DateTime fecha, TimeSpan Marca_Hora, horarios horario, int tipoMarca)
        {
            horas_extras horaExtra = db.horas_extras.FirstOrDefault(h => h.Fecha_HoraExtra.Equals(fecha) && h.idEmpleado.Equals(idEmpleado));

            int cantidadHoras = ValidarHoraExtra(horario, Marca_Hora);

            if (horaExtra == null)
            {
                return;
            }

            if (cantidadHoras >= horaExtra.Cantidad_Horas && horaExtra.Aprobada == true && tipoMarca == 2)
            {
                horaExtra.HizoHoras = true;
                db.Entry(horaExtra).State = EntityState.Modified;
                db.SaveChanges();
            }
        }

        private int ValidarHoraExtra(horarios _horario, TimeSpan marca)
        {
            int cantidadHoras = 0;

            // Verifica si el horario no es nulo y si la marca es posterior a la hora de salida
            if (_horario != null && marca > _horario.Hora_Salida)
            {
                TimeSpan diferencia = marca - _horario.Hora_Salida;
                cantidadHoras = (int)diferencia.TotalHours;
            }

            return cantidadHoras;
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
