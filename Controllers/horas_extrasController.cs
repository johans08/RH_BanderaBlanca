﻿using System;
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
            var viewModelList = new List<Hora_Extra>();
            try
            {
                Persona userSesion = new Persona();
                userSesion = (Persona)Session["user"];

                var _horasextras = db.horas_extras.ToList();


                foreach (var _horaExtra in _horasextras)
                {
                    // Utilizar SingleOrDefault o FirstOrDefault para obtener un único objeto en lugar de una colección
                    var catalogo = db.catalogo_horas_extras
                                        .SingleOrDefault(i => i.idCatalogo_Horas_Extras == _horaExtra.idCatalogo_Horas_Extras);

                    // Verificar si incapacidad no es nulo antes de crear el viewModel
                    if (catalogo != null)
                    {
                        var empleado = db.empleados
                                      .SingleOrDefault(i => i.idEmpleado == _horaExtra.idEmpleado);

                        var persona = db.personas
                                            .SingleOrDefault(i => i.Identificador == empleado.Personas_Identificador);

                        if (empleado.idJefaturaDirecta == userSesion.empleados.idEmpleado)
                        {
                            var viewModel = new Hora_Extra
                            {
                                horas_extras = _horaExtra,
                                catalogo_horas_extras = catalogo,
                                personas = persona,
                                empleados = empleado
                            };

                            viewModelList.Add(viewModel);
                        }
                    }
                }

                return View(viewModelList);
            }
            catch (Exception)
            {
                return View(viewModelList);
            }
        }

        public ActionResult Gestion()
        {
            var viewModelList = new List<Hora_Extra>();
            try
            {
                Persona userSesion = new Persona();
                userSesion = (Persona)Session["user"];
                var _horasextras = db.horas_extras.Include(h => h.catalogo_horas_extras).Where(i => i.idEmpleado.Equals(userSesion.empleados.idEmpleado)).ToList();


                foreach (var _horaExtra in _horasextras)
                {
                    // Utilizar SingleOrDefault o FirstOrDefault para obtener un único objeto en lugar de una colección
                    var catalogo = db.catalogo_horas_extras
                                        .SingleOrDefault(i => i.idCatalogo_Horas_Extras == _horaExtra.idCatalogo_Horas_Extras);

                    // Verificar si incapacidad no es nulo antes de crear el viewModel
                    if (catalogo != null)
                    {
                        var empleado = db.empleados
                                      .SingleOrDefault(i => i.idEmpleado == _horaExtra.idEmpleado);

                        var persona = db.personas
                                            .SingleOrDefault(i => i.Identificador == empleado.Personas_Identificador);

                        var viewModel = new Hora_Extra
                        {
                            horas_extras = _horaExtra,
                            catalogo_horas_extras = catalogo,
                            personas = persona,
                            empleados = empleado
                        };

                        viewModelList.Add(viewModel);
                    }
                }

                return View(viewModelList);
            }
            catch (Exception)
            {
                return View(viewModelList);
            }

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
                _horas_extras.idEmpleado = userSesion.empleados.idEmpleado;

                empleados _empleado = db.empleados.FirstOrDefault(e => e.idEmpleado.Equals(userSesion.empleados.idEmpleado));
                
                puestos_laborales _puestoLaboral = db.puestos_laborales.Find(_empleado.idPuestos_Laboral);
                horarios horarios = db.horarios.FirstOrDefault(h => h.idEmpleado.Equals(_empleado.idEmpleado));
                _horas_extras.idCatalogo_Horas_Extras = horarios.idTipo_Horario;
                catalogo_horas_extras _tipoHoraExtra = db.catalogo_horas_extras.Find(_horas_extras.idCatalogo_Horas_Extras);

                if (_empleado == null || _tipoHoraExtra == null || _puestoLaboral == null || horarios == null)
                {
                    ModelState.AddModelError("FechaPermiso", "Ha ocurrido un error al procesar la solicitud");
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

            ModelState.AddModelError("FechaPermiso", "No es posible crear la solicitud.");
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
