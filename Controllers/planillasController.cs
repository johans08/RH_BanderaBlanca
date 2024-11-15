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
    public class planillasController : Controller
    {
        private BD_BanderaBlancaEntities db = new BD_BanderaBlancaEntities();

        // GET: planillas
        public ActionResult Index()
        {
            try
            {
                // Obtener el mes y año actuales
                var mesActual = DateTime.Now.Month;
                var anioActual = DateTime.Now.Year;

                // Obtener solo las planillas del mes y año actuales con sus detalles
                var detalle_planillas = db.detalle_maestro_planillas
                                           .Where(dp => dp.Fecha_Planilla.Month == mesActual && dp.Fecha_Planilla.Year == anioActual)
                                           .ToList();

                var viewModelList = new List<Planilla>();

                foreach (var detalle_planilla in detalle_planillas)
                {
                    var idEmpleado = detalle_planilla.idEmpleado;

                    // Filtrar planillas por mes, año y empleado actual
                    var planilla = db.planillas
                                     .SingleOrDefault(i => i.Fecha_Planilla.Month == mesActual
                                                         && i.Fecha_Planilla.Year == anioActual
                                                         && i.idEmpleado == idEmpleado);

                    if (planilla != null)
                    {
                        var empleado = db.empleados
                                         .SingleOrDefault(i => i.idEmpleado == planilla.idEmpleado);

                        var persona = empleado != null
                            ? db.personas.SingleOrDefault(i => i.Identificador == empleado.Personas_Identificador)
                            : null;

                        var sucursal = empleado != null
                            ? db.sucursales.SingleOrDefault(i => i.Identificador_Sucursal == empleado.Identificador_Sucursal)
                            : null;

                        var viewModel = new Planilla
                        {
                            detalle_planillas = detalle_planilla,
                            planillas = planilla,
                            personas = persona,
                            empleados = empleado,
                            sucursales = sucursal
                        };

                        viewModelList.Add(viewModel);
                    }
                }

                return View(viewModelList);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en la acción Index: {ex.Message}");
                return View(new List<Planilla>());
            }
        }




        public ActionResult Gestion()
        {
            Persona userSesion = new Persona();
            userSesion = (Persona)Session["user"];

            var detalle_planillas = db.detalle_maestro_planillas.Where(p => p.idEmpleado.Equals(userSesion.empleados.idEmpleado)).ToList();
            var viewModelList = new List<Planilla>();

            foreach (var detalle_planilla in detalle_planillas)
            {
                // Verificar si incapacidad no es nulo antes de crear el viewModel
                if (detalle_planilla != null)
                {
                    // Utilizar SingleOrDefault o FirstOrDefault para obtener un único objeto en lugar de una colección
                    var planilla = db.planillas
                                        .SingleOrDefault(i => i.Fecha_Planilla == detalle_planilla.Fecha_Planilla && i.idEmpleado == userSesion.empleados.idEmpleado);
                    var empleado = db.empleados
                                   .SingleOrDefault(i => i.idEmpleado == planilla.idEmpleado);

                    var persona = db.personas
                                        .SingleOrDefault(i => i.Identificador == empleado.Personas_Identificador);

                    var sucursales = db.sucursales
                                        .SingleOrDefault(i => i.Identificador_Sucursal == empleado.Identificador_Sucursal);

                    var viewModel = new Planilla
                    {
                        detalle_planillas = detalle_planilla,
                        planillas = planilla,
                        personas = persona,
                        empleados = empleado,
                        sucursales = sucursales
                    };

                    viewModelList.Add(viewModel);
                }
            }

            return View(viewModelList);
        }

        [HttpPost]
        public ActionResult UpdateEstado(DateTime id, int estado, int idEmpleado)
        {
            // Buscar la liquidación por ID
            var pago = db.planillas.FirstOrDefault(l => l.Fecha_Planilla == id && l.idEmpleado == idEmpleado);
            if (pago != null)
            {
                // Actualizar el estado de la liquidación
                pago.idEstados_Solicitudes = estado;
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        // GET: planillas/Create
        public ActionResult PagarPlanillas()
        {
            Planilla planilla = new Planilla();
            planilla.PagarPlanillas();

            var planillas = db.planillas.Include(p => p.estados_solicitudes);
            return RedirectToAction("Index", planillas.ToList());
        }

        public ActionResult RegenerarPlanillas()
        {
            Planilla planilla = new Planilla();
            planilla.RegenerarPlanillas();

            var planillas = db.planillas.Include(p => p.estados_solicitudes);
            return RedirectToAction("Index", planillas.ToList());
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
