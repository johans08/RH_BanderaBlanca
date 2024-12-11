using System;
using System.Collections.Generic; using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using DataBase_RH_BanderaBlanca.Models;

namespace RH_BanderaBlanca.Controllers
{
    public class incapacidadesController : Controller
    {
        private BD_BanderaBlancaEntities db = new BD_BanderaBlancaEntities();

        // GET: incapacidades
        public ActionResult Index()
        {
            var viewModelList = new List<Incapacidad_Comprobante>();

            try
            {
                Persona userSesion = (Persona)Session["user"];
                var comprobantes = db.comprobantes_incapacidad.ToList();

                foreach (var comprobante in comprobantes)
                {
                    // Obtener todas las coincidencias en lugar de una sola
                    var incapacidades = db.incapacidades
                                          .Where(i => i.Fecha_Incapacidad == comprobante.Incapacidades_Fecha_Incapacidad && i.idEmpleado != userSesion.empleados.idEmpleado)
                                          .ToList();

                    // Iterar sobre todas las coincidencias
                    foreach (var incapacidad in incapacidades)
                    {
                        var empleado = db.empleados
                                         .FirstOrDefault(e => e.idEmpleado == incapacidad.idEmpleado);

                        var persona = db.personas
                                        .FirstOrDefault(p => p.Identificador == empleado.Personas_Identificador);

                        if (empleado != null && empleado.idJefaturaDirecta == userSesion.empleados.idEmpleado)
                        {
                            var viewModel = new Incapacidad_Comprobante
                            {
                                incapacidad = incapacidad,
                                comprobantes = comprobante,
                                personas = persona,
                                empleados = empleado
                            };
                            var existingItem = viewModelList.FirstOrDefault(vm => vm.comprobantes.Incapacidades_Fecha_Incapacidad == comprobante.Incapacidades_Fecha_Incapacidad && vm.incapacidad.idEmpleado == incapacidad.idEmpleado);
                            if (existingItem == null)
                            {
                                viewModelList.Add(viewModel);
                            }
                        }
                    }
                }

                return View(viewModelList);
            }
            catch (Exception ex)
            {
                // Registrar el error para depuración
                ViewBag.ErrorMessage = ex.Message;
                return View(viewModelList);
            }
        }




        public ActionResult Gestion()
        {
            var viewModelList = new List<Incapacidad_Comprobante>();

            try
            {
                Persona userSesion = (Persona)Session["user"];
                var comprobantes = db.comprobantes_incapacidad.ToList();

                foreach (var comprobante in comprobantes)
                {
                    var incapacidad = db.incapacidades
                        .FirstOrDefault(i => i.Fecha_Incapacidad == comprobante.Incapacidades_Fecha_Incapacidad && i.idEmpleado == userSesion.empleados.idEmpleado);

                    if (incapacidad != null)
                    {
                        var empleado = db.empleados.FirstOrDefault(i => i.idEmpleado == incapacidad.idEmpleado);
                        var persona = db.personas.FirstOrDefault(i => i.Identificador == empleado.Personas_Identificador);

                        var viewModel = new Incapacidad_Comprobante
                        {
                            incapacidad = incapacidad,
                            comprobantes = comprobante,
                            personas = persona,
                            empleados = empleado
                        };

                        // Verificar si ya existe un registro similar
                        var existingItem = viewModelList.FirstOrDefault(vm => vm.comprobantes.Incapacidades_Fecha_Incapacidad == comprobante.Incapacidades_Fecha_Incapacidad && vm.incapacidad.idEmpleado == incapacidad.idEmpleado);
                        if (existingItem == null)
                        {
                            viewModelList.Add(viewModel);
                        }
                    }
                }

                return View(viewModelList);
            }
            catch (Exception ex)
            {
                // Manejar el error aquí (opcional)
                return View(viewModelList);
            }
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
            Incapacidad_Comprobante incapacidad = new Incapacidad_Comprobante();
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

        private void CargarViewBags(Incapacidad_Comprobante incapacidad)
        {
            ViewBag.idEmpleado = new SelectList(
                db.empleados.Include(e => e.personas)
                .Select(e => new
                {
                    e.idEmpleado,
                    NombreCompleto = e.personas.Nombre + " " + e.personas.Primer_Apellido + " " + e.personas.Segundo_Apellido
                }).ToList(),
                "idEmpleado",
                "NombreCompleto", incapacidad.incapacidad.idEmpleado);
            ViewBag.idCatalogo_Incapacidad = new SelectList(db.catalogo_incapacidades, "idCatalogo_Incapacidad", "Descripcion");
            ViewBag.Estados_Solicitudes_idEstados_Solicitudes = new SelectList(db.estados_solicitudes, "idCatalogo_Incapacidad", "Descripcion");
        }

        // POST: incapacidades/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Incapacidad_Comprobante _incapacidad, HttpPostedFileBase fileUpload)
        {
            try
            {
                Persona userSesion = new Persona();
                userSesion = (Persona)Session["user"];

                // Verificar si ya existe una incapacidad para el mismo empleado y dentro del mismo rango de fechas
                var existingIncapacidad = db.incapacidades.FirstOrDefault(i =>
                    i.idEmpleado == userSesion.empleados.idEmpleado &&
                    (
                        (_incapacidad.incapacidad.Fecha_Incapacidad >= i.Fecha_Incapacidad && _incapacidad.incapacidad.Fecha_Incapacidad <= i.Fecha_Final_Incapacidad) || // Nueva fecha inicial está dentro del rango existente
                        (_incapacidad.incapacidad.Fecha_Final_Incapacidad >= i.Fecha_Incapacidad && _incapacidad.incapacidad.Fecha_Final_Incapacidad <= i.Fecha_Final_Incapacidad) || // Nueva fecha final está dentro del rango existente
                        (_incapacidad.incapacidad.Fecha_Incapacidad <= i.Fecha_Incapacidad && _incapacidad.incapacidad.Fecha_Final_Incapacidad >= i.Fecha_Final_Incapacidad) // Nuevo rango engloba al rango existente
                    ));

                if (existingIncapacidad != null)
                {
                    ModelState.AddModelError("", "Ya existe una incapacidad dentro del rango de fechas especificado.");
                    CargarViewBags(_incapacidad);
                    return View(_incapacidad);
                }



                // Resto del código para calcular días, salario, y guardar la incapacidad
                TimeSpan diferencia = _incapacidad.incapacidad.Fecha_Final_Incapacidad - _incapacidad.incapacidad.Fecha_Incapacidad;
                int dias = diferencia.Days + 1;

                int finesDeSemana = 0;

                for (DateTime fecha = _incapacidad.incapacidad.Fecha_Incapacidad; fecha <= _incapacidad.incapacidad.Fecha_Final_Incapacidad; fecha = fecha.AddDays(1))
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

                    

                    _incapacidad.incapacidad.Cantidad_Dias = diferenciaSinFinesDeSemana;

                    empleados _empleado = db.empleados.FirstOrDefault(e => e.idEmpleado.Equals(userSesion.empleados.idEmpleado));
                    catalogo_incapacidades _tipoIncapacidad = db.catalogo_incapacidades.Find(_incapacidad.incapacidad.idCatalogo_Incapacidad);
                    puestos_laborales _puestoLaboral = db.puestos_laborales.Find(_empleado.idPuestos_Laboral);

                    if (_empleado == null || _tipoIncapacidad == null || _puestoLaboral == null)
                    {
                        CargarViewBags(_incapacidad);
                        return View(_incapacidad);
                    }

                    // Calcular monto descontado
                    int horasTrabajadasPorDia = 8;
                    float salarioDiario = (float)(_puestoLaboral.Salario_Puesto / 30.0 / horasTrabajadasPorDia);
                    _incapacidad.incapacidad.Monto_Deduccion = (_tipoIncapacidad.Porcentaje_Deduccion * salarioDiario) * _incapacidad.incapacidad.Cantidad_Dias;
                    _incapacidad.incapacidad.Estados_Solicitudes_idEstados_Solicitudes = 3;
                    _incapacidad.incapacidad.idEmpleado = userSesion.empleados.idEmpleado;

                    // Guardar la incapacidad en la base de datos
                    db.incapacidades.Add(_incapacidad.incapacidad);
                    db.SaveChanges();

                    if (fileUpload != null && fileUpload.ContentLength > 0)
                    {
                        // Convertir la imagen a Base64
                        using (var ms = new MemoryStream())
                        {
                            fileUpload.InputStream.CopyTo(ms);
                            byte[] fileBytes = ms.ToArray();
                            string base64String = Convert.ToBase64String(fileBytes);
                            _incapacidad.comprobantes.Imagen_Comprobante = base64String;
                        }
                    }


                    _incapacidad.comprobantes.Incapacidades_Fecha_Incapacidad = _incapacidad.incapacidad.Fecha_Incapacidad;
                    db.comprobantes_incapacidad.Add(_incapacidad.comprobantes);
                    db.SaveChanges();

                    return RedirectToAction("Gestion");
                }

                CargarViewBags(_incapacidad);
                return View(_incapacidad);
            }
            catch (Exception ex)
            {
                // Capturar cualquier excepción y mostrar un mensaje genérico
                ModelState.AddModelError("", "No se ha creado la incapacidad, por favor revisa las fechas si ya tienes una incapacidad o si el numero de boleta ya fue utilizado");
                CargarViewBags(_incapacidad);
                return View(_incapacidad);
            }
        }

        // GET: incapacidades/Edit/5
        public ActionResult Edit(DateTime idInicio, DateTime idFinal, int idEmpleado)
        {
            if (idInicio == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            incapacidades incapacidades = db.incapacidades.Find(idInicio, idFinal, idEmpleado);
            if (incapacidades == null)
            {
                return HttpNotFound();
            }

            ViewBag.idEmpleado = new SelectList(
                db.empleados.Include(e => e.personas)
                .Select(e => new
                {
                    e.idEmpleado,
                    NombreCompleto = e.personas.Nombre + " " + e.personas.Primer_Apellido + " " + e.personas.Segundo_Apellido
                }).ToList(),
                "idEmpleado",
                "NombreCompleto", incapacidades.idEmpleado);
            ViewBag.idCatalogo_Incapacidad = new SelectList(db.catalogo_incapacidades, "idCatalogo_Incapacidad", "Descripcion");
            ViewBag.Estados_Solicitudes_idEstados_Solicitudes = new SelectList(db.estados_solicitudes, "idEstados_Solicitudes", "Estados_Solicitud");
            return View(incapacidades);
        }

        // POST: incapacidades/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Fecha_Incapacidad,Fecha_Final_Incapacidad,idEmpleado,idCatalogo_Incapacidad,Cantidad_Dias,Monto_Deduccion,Detalle_Incapacidad,Estados_Solicitudes_idEstados_Solicitudes, Observacion, Numero_Boleta")] incapacidades incapacidades)
        {
            if (ModelState.IsValid)
            {
                Persona userSesion = new Persona();
                userSesion = (Persona)Session["user"];

                if (incapacidades.idEmpleado == userSesion.empleados.idEmpleado)
                {
                    ModelState.AddModelError("", "No puede aprobar sus incapacidades");
                    ViewBag.idEmpleado = new SelectList(
                        db.empleados.Include(e => e.personas)
                        .Select(e => new
                        {
                            e.idEmpleado,
                            NombreCompleto = e.personas.Nombre + " " + e.personas.Primer_Apellido + " " + e.personas.Segundo_Apellido
                        }).ToList(),
                        "idEmpleado",
                        "NombreCompleto", incapacidades.idEmpleado);
                    ViewBag.idCatalogo_Incapacidad = new SelectList(db.catalogo_incapacidades, "idCatalogo_Incapacidad", "Descripcion");
                    ViewBag.Estados_Solicitudes_idEstados_Solicitudes = new SelectList(db.estados_solicitudes, "idEstados_Solicitudes", "Estados_Solicitud");
                    return View(incapacidades);
                }

                db.Entry(incapacidades).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.idEmpleado = new SelectList(
                db.empleados.Include(e => e.personas)
                .Select(e => new
                {
                    e.idEmpleado,
                    NombreCompleto = e.personas.Nombre + " " + e.personas.Primer_Apellido + " " + e.personas.Segundo_Apellido
                }).ToList(),
                "idEmpleado",
                "NombreCompleto", incapacidades.idEmpleado);
            ViewBag.idCatalogo_Incapacidad = new SelectList(db.catalogo_incapacidades, "idCatalogo_Incapacidad", "Descripcion");
            ViewBag.Estados_Solicitudes_idEstados_Solicitudes = new SelectList(db.estados_solicitudes, "idEstados_Solicitudes", "Estados_Solicitud");
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
