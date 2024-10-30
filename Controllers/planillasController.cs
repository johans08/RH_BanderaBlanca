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
    public class planillasController : Controller
    {
        private BD_BanderaBlancaEntities db = new BD_BanderaBlancaEntities();

        // GET: planillas
        public ActionResult Index()
        {
            var detalle_planillas = db.detalle_maestro_planillas.ToList();
            var viewModelList = new List<Planilla>();

            foreach (var detalle_planilla in detalle_planillas)
            {
                // Utilizar SingleOrDefault o FirstOrDefault para obtener un único objeto en lugar de una colección
                var planilla = db.planillas
                                    .SingleOrDefault(i => i.Fecha_Planilla == detalle_planilla.Fecha_Planilla);

                var empleado = db.empleados
                                   .SingleOrDefault(i => i.idEmpleado == planilla.idEmpleado);

                var persona = db.personas
                                    .SingleOrDefault(i => i.Identificador == empleado.Personas_Identificador);

                var sucursales = db.sucursales
                                    .SingleOrDefault(i => i.Identificador_Sucursal == empleado.Identificador_Sucursal);

                // Verificar si incapacidad no es nulo antes de crear el viewModel
                if (planilla != null)
                {
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

        public ActionResult Gestion()
        {
            Persona userSesion = new Persona();
            userSesion = (Persona)Session["user"];

            var detalle_planillas = db.detalle_maestro_planillas.Where(p => p.idEmpleado.Equals(userSesion.empleados.idEmpleado)).ToList();
            var viewModelList = new List<Planilla>();

            foreach (var detalle_planilla in detalle_planillas)
            {
                // Utilizar SingleOrDefault o FirstOrDefault para obtener un único objeto en lugar de una colección
                var planilla = db.planillas
                                    .SingleOrDefault(i => i.Fecha_Planilla == detalle_planilla.Fecha_Planilla);

                var empleado = db.empleados
                                   .SingleOrDefault(i => i.idEmpleado == planilla.idEmpleado);

                var persona = db.personas
                                    .SingleOrDefault(i => i.Identificador == empleado.Personas_Identificador);

                var sucursales = db.sucursales
                                    .SingleOrDefault(i => i.Identificador_Sucursal == empleado.Identificador_Sucursal);

                // Verificar si incapacidad no es nulo antes de crear el viewModel
                if (planilla != null)
                {
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
