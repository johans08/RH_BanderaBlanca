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
    public class vacacionesController : Controller
    {
        private BD_BanderaBlancaEntities db = new BD_BanderaBlancaEntities();

        // GET: vacaciones
        public ActionResult Index()
        {
            var vacaciones = db.vacaciones.Include(v => v.estados_solicitudes);
            return View(vacaciones.ToList());
        }

        public ActionResult Gestion()
        {
            try
            {
                Persona userSesion = (Persona)Session["user"];
                if (userSesion == null || userSesion.empleados == null)
                {
                    throw new Exception("User session or employee is null.");
                }

                var vacacions = db.vacaciones.Include(v => v.estados_solicitudes).Where(i => i.idEmpleado == userSesion.empleados.idEmpleado).ToList();

                if (vacacions == null || !vacacions.Any())
                {
                    vacacions = new List<vacaciones>();
                }

                (int diasDisponibles, int diasDescontados) = CalcularVacaciones(userSesion.empleados.idEmpleado);

                ViewBag.diasDisponibles = diasDisponibles;
                ViewBag.diasDescontados = diasDescontados;

                return View(vacacions);
            }
            catch (Exception ex)
            {
                return View(); // or redirect to an error page
            }

        }

        public (int, int) CalcularVacaciones(int idEmpleado)
        {
            try
            {
                int cantidadDiasDisponibles = 0;
                int cantidadDiasDisfrutados = 0;

                var _vacacionPersonal = db.registro_vacaciones_personales.FirstOrDefault(vp => vp.idEmpleado == idEmpleado);
                var _empleado = db.empleados.FirstOrDefault(e => e.idEmpleado == idEmpleado);

                if (_empleado == null)
                {
                    return (cantidadDiasDisponibles, cantidadDiasDisfrutados);
                }

                // Calcular meses completos trabajados
                DateTime fechaIngreso = _empleado.Fecha_Ingreso;
                DateTime fechaActual = DateTime.Now;
                int totalMeses = (fechaActual.Year - fechaIngreso.Year) * 12 + fechaActual.Month - fechaIngreso.Month;

                // Ajustar si el día actual es menor que el día de ingreso
                if (fechaActual.Day < fechaIngreso.Day)
                {
                    totalMeses--;
                }

                int mesesLaborales = totalMeses;

                // Suponiendo que los empleados tienen 1 día de vacaciones por cada mes trabajado
                cantidadDiasDisponibles = mesesLaborales;

                // Si no existe registro en vacacionpersonal, crearlo
                if (_vacacionPersonal == null)
                {
                    var _addVacacionPersonal = new registro_vacaciones_personales
                    {
                        idEmpleado = idEmpleado,
                        Vacaciones_Disponibles = cantidadDiasDisponibles,
                        Vacaciones_Consumidas = cantidadDiasDisfrutados,
                    };

                    db.registro_vacaciones_personales.Add(_addVacacionPersonal);
                    db.SaveChanges();

                    return (cantidadDiasDisponibles, cantidadDiasDisfrutados);
                }

                // Actualizar las vacaciones disponibles si hay una discrepancia
                if (cantidadDiasDisponibles != _vacacionPersonal.Vacaciones_Disponibles)
                {
                    _vacacionPersonal.Vacaciones_Disponibles = _vacacionPersonal.Vacaciones_Disponibles;
                    db.Entry(_vacacionPersonal).State = EntityState.Modified;
                    db.SaveChanges();
                }

                cantidadDiasDisponibles = _vacacionPersonal.Vacaciones_Disponibles;
                cantidadDiasDisfrutados = _vacacionPersonal.Vacaciones_Consumidas;

                return (cantidadDiasDisponibles, cantidadDiasDisfrutados);
            }
            catch (Exception)
            {
                return (0, 0);
            }
        }

        public void DescontarVacaciones(int idEmpleado, int cantidadDias)
        {
            try
            {
                registro_vacaciones_personales _vacacionPersonal = db.registro_vacaciones_personales.FirstOrDefault(vp => vp.idEmpleado == idEmpleado);

                _vacacionPersonal.Vacaciones_Disponibles = _vacacionPersonal.Vacaciones_Disponibles - cantidadDias;
                _vacacionPersonal.Vacaciones_Consumidas += cantidadDias;

                db.Entry(_vacacionPersonal).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch (Exception)
            {

            }

        }


        public void SumarVacaciones(int idEmpleado, int cantidadDias)
        {
            try
            {
                registro_vacaciones_personales _vacacionPersonal = db.registro_vacaciones_personales.FirstOrDefault(vp => vp.idEmpleado == idEmpleado);

                _vacacionPersonal.Vacaciones_Disponibles = _vacacionPersonal.Vacaciones_Disponibles + cantidadDias;
                _vacacionPersonal.Vacaciones_Consumidas -= cantidadDias;

                db.Entry(_vacacionPersonal).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch (Exception)
            {

            }

        }

        // GET: vacaciones/Details/5
        public ActionResult Details(DateTime id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            vacaciones vacaciones = db.vacaciones.Find(id);
            if (vacaciones == null)
            {
                return HttpNotFound();
            }
            return View(vacaciones);
        }

        // GET: vacaciones/Create
        public ActionResult Create()
        {
            vacaciones vacaciones = new vacaciones();

            ViewBag.idEstados_Solicitudes = new SelectList(db.estados_solicitudes, "idEstados_Solicitudes", "Estados_Solicitud");
            return View(vacaciones);
        }

        // POST: vacaciones/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Fecha_Inicio,Fecha_Final")] vacaciones vacaciones)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    vacaciones.Fecha_Solicitud = DateTime.Now;

                    Persona userSesion = (Persona)Session["user"];
                    int empleadoId = userSesion.empleados.idEmpleado;

                    // Verificar si ya hay vacaciones solicitadas en esas fechas
                    bool tieneVacacionesEnFechas = db.vacaciones.Any(v => v.idEmpleado == empleadoId &&
                                                                         ((vacaciones.Fecha_Inicio >= v.Fecha_Inicio && vacaciones.Fecha_Inicio <= v.Fecha_Final) ||
                                                                          (vacaciones.Fecha_Final >= v.Fecha_Inicio && vacaciones.Fecha_Final <= v.Fecha_Final) ||
                                                                          (vacaciones.Fecha_Inicio <= v.Fecha_Inicio && vacaciones.Fecha_Final >= v.Fecha_Final)));

                    if (tieneVacacionesEnFechas)
                    {
                        ModelState.AddModelError("", "Ya tienes vacaciones solicitadas dentro de estas fechas.");
                    }
                    else
                    {
                        TimeSpan diferencia = vacaciones.Fecha_Final - vacaciones.Fecha_Inicio;
                        int dias = diferencia.Days;
                        int finesDeSemana = 0;

                        for (DateTime fecha = vacaciones.Fecha_Inicio; fecha <= vacaciones.Fecha_Final; fecha = fecha.AddDays(1))
                        {
                            if (fecha.DayOfWeek == DayOfWeek.Saturday || fecha.DayOfWeek == DayOfWeek.Sunday)
                            {
                                finesDeSemana++;
                            }
                        }


                        // fdsafasdsdfdasfdadsa
                        int diasFeriados = 0;

                        List<feriado> feriados = db.feriado.ToList();

                        for (DateTime dia = vacaciones.Fecha_Inicio; dia <= vacaciones.Fecha_Final; dia = dia.AddDays(1))
                        {
                            foreach (feriado feriado in feriados)
                            {
                                if (dia == feriado.Fecha_Feriados)
                                {
                                    diasFeriados++;
                                }
                            }
                        }


                        int diferenciaSinFinesDeSemana = dias - finesDeSemana - diasFeriados;

                        vacaciones.Cantidad_Dias = diferenciaSinFinesDeSemana + 1;
                        vacaciones.idEmpleado = empleadoId;
                        vacaciones.idEstados_Solicitudes = 3;

                        (int diasDisponibles, int diasDescontados) = CalcularVacaciones(empleadoId);

                        var consultaferiados = ObtenerFeriado(vacaciones.Fecha_Inicio, vacaciones.Fecha_Final);
                        vacaciones.Cantidad_Dias -= consultaferiados.Count;

                        if (vacaciones.Cantidad_Dias < 0)
                        {
                            ModelState.AddModelError("", "El día final debe ser mayor al día de inicio.");
                        }
                        else
                        {

                            if (diasDisponibles >= vacaciones.Cantidad_Dias)
                            {
                                db.vacaciones.Add(vacaciones);// Seleccionar de la tabla feriados, los dias que se encuentre entre la finicio y ffin seleccionada
                                db.SaveChanges();

                                db.registro_vacaciones_personales.FirstOrDefault().Vacaciones_Disponibles -= vacaciones.Cantidad_Dias;
                                db.SaveChanges();
                                return RedirectToAction("Gestion");
                            }
                            else
                            {
                                ModelState.AddModelError("", "No tienes suficientes días disponibles para estas vacaciones.");
                            }
                        }
                    }
                }

                CargarViewBags(vacaciones);
                return View(vacaciones);
            }
            catch (Exception)
            {
                CargarViewBags(vacaciones);
                return View(vacaciones);
            }
        }

        private void CargarViewBags(vacaciones _vacaciones)
        {
            ViewBag.idEmpleado = new SelectList(
                db.empleados.Include(e => e.personas)
                .Select(e => new
                {
                    e.idEmpleado,
                    NombreCompleto = e.personas.Nombre + " " + e.personas.Primer_Apellido + " " + e.personas.Segundo_Apellido
                }).ToList(),
                "idEmpleado",
                "NombreCompleto", _vacaciones.idEmpleado);
            ViewBag.idEstados_Solicitudes = new SelectList(db.estados_solicitudes, "idEstados_Solicitudes", "Estados_Solicitud", _vacaciones.idEstados_Solicitudes);
        }


        public List<feriado> ObtenerFeriado(DateTime finicio, DateTime ffinal)
        {
            // var feriado = db.Feriados.Where(x => (x.Fecha.Year >= finicio.Year && x.Fecha.Month >= finicio.Month && x.Fecha.Day >= finicio.Day) && (x.Fecha.Year <= ffinal.Year && x.Fecha.Month <= ffinal.Month && x.Fecha.Day <= ffinal.Day)).ToList();
            DateTime fechaInicio = new DateTime(finicio.Year, finicio.Month, finicio.Day);
            DateTime fechaFin = new DateTime(ffinal.Year, ffinal.Month, ffinal.Day);
            var feriado = db.feriado.Where(x => x.Fecha_Feriados >= fechaInicio && x.Fecha_Feriados <= fechaFin).ToList();
            return feriado;
        }

        // GET: vacaciones/Edit/5
        public ActionResult Edit(DateTime date, int idEmpleado)
        {
            if (date == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            vacaciones vacacion = db.vacaciones.FirstOrDefault(p => p.Fecha_Inicio == date && p.idEmpleado == idEmpleado);
            if (vacacion == null)
            {
                return HttpNotFound();
            }
            
            CargarViewBags(vacacion);
            return View(vacacion);
        }

        // POST: vacaciones/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Fecha_Inicio,Fecha_Final,idEmpleado,idEstados_Solicitudes,Cantidad_Dias,Fecha_Solicitud,observacion")] vacaciones vacacion)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (vacacion.idEstados_Solicitudes == 1)
                    {
                        (int diasDisponibles, int diasDescontados) = CalcularVacaciones(vacacion.idEmpleado);

                        if (diasDisponibles >= vacacion.Cantidad_Dias)
                        {
                            DescontarVacaciones(vacacion.idEmpleado, vacacion.Cantidad_Dias);

                            db.Entry(vacacion).State = EntityState.Modified;
                            db.SaveChanges();
                            return RedirectToAction("Index");
                        }

                    }

                    if (vacacion.idEstados_Solicitudes == 2 || vacacion.idEstados_Solicitudes == 3)
                    {
                        (int diasDisponibles, int diasDescontados) = CalcularVacaciones(vacacion.idEmpleado);

                        if (diasDisponibles >= vacacion.Cantidad_Dias)
                        {
                            SumarVacaciones(vacacion.idEmpleado, vacacion.Cantidad_Dias);

                            db.Entry(vacacion).State = EntityState.Modified;
                            db.SaveChanges();
                            return RedirectToAction("Index");
                        }

                    }


                }
               
                CargarViewBags(vacacion);
                return View(vacacion);
            }
            catch (Exception)
            {
                CargarViewBags(vacacion);
                return View(vacacion);
            }
        }

        // GET: vacaciones/Delete/5
        public ActionResult Delete(DateTime id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            vacaciones vacaciones = db.vacaciones.Find(id);
            if (vacaciones == null)
            {
                return HttpNotFound();
            }
            return View(vacaciones);
        }

        // POST: vacaciones/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(DateTime id)
        {
            vacaciones vacaciones = db.vacaciones.Find(id);
            db.vacaciones.Remove(vacaciones);
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
