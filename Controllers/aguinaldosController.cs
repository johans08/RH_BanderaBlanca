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
    public class aguinaldosController : Controller
    {
        private BD_BanderaBlancaEntities db = new BD_BanderaBlancaEntities();

        public ActionResult Index()
        {
            var viewModelList = new List<Aguinaldo>();

            try
            {
                Persona userSesion = new Persona();
                userSesion = (Persona)Session["user"];

                var aguinaldos = db.aguinaldos.Include(a => a.catalogo_aguinaldo).Include(a => a.estados_solicitudes).ToList();


                foreach (var aguinaldo in aguinaldos)
                {


                    // Verificar si incapacidad no es nulo antes de crear el viewModel
                    if (aguinaldo != null)
                    {
                        var empleado = db.empleados
                                       .SingleOrDefault(i => i.idEmpleado == aguinaldo.Empleados_idEmpleado);

                        var persona = db.personas
                                            .SingleOrDefault(i => i.Identificador == empleado.Personas_Identificador);

                        if (userSesion.ObtenerIdsDeAccesos(userSesion.listaAccesos).Contains(10))
                        {
                            var viewModel = new Aguinaldo
                            {
                                personas = persona,
                                empleados = empleado,
                                aguinaldo = aguinaldo
                            };

                            viewModelList.Add(viewModel);
                        }
                        else
                        {
                            if (empleado.idJefaturaDirecta == userSesion.empleados.idEmpleado)
                            {
                                var viewModel = new Aguinaldo
                                {
                                    personas = persona,
                                    empleados = empleado,
                                    aguinaldo = aguinaldo
                                };

                                viewModelList.Add(viewModel);
                            }
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
            var viewModelList = new List<Aguinaldo>();

            try
            {
                Persona userSesion = new Persona();
                userSesion = (Persona)Session["user"];

                var aguinaldos = db.aguinaldos.Where(p => p.Empleados_idEmpleado.Equals(userSesion.empleados.idEmpleado)).Include(a => a.catalogo_aguinaldo).Include(a => a.estados_solicitudes).ToList();


                foreach (var aguinaldo in aguinaldos)
                {


                    // Verificar si incapacidad no es nulo antes de crear el viewModel
                    if (aguinaldo != null)
                    {

                        var empleado = db.empleados
                                       .SingleOrDefault(i => i.idEmpleado == aguinaldo.Empleados_idEmpleado);

                        var persona = db.personas
                                            .SingleOrDefault(i => i.Identificador == empleado.Personas_Identificador);

                        var viewModel = new Aguinaldo
                        {
                            personas = persona,
                            empleados = empleado,
                            aguinaldo = aguinaldo
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

        public ActionResult UpdateEstado(DateTime id, int estado, int idEmpleado)
        {
            // Buscar la liquidación por ID
            var pago = db.aguinaldos.FirstOrDefault(l => l.FechaAguinaldo == id && l.Empleados_idEmpleado == idEmpleado);
            if (pago != null)
            {
                // Actualizar el estado de la liquidación
                pago.idEstados_Solicitudes = estado;
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        // GET: planillas/Create
        public ActionResult PagarAguinaldos()
        {
            Aguinaldo aguinaldo = new Aguinaldo();
            aguinaldo.GenerarAguinaldos();

            return RedirectToAction("Index");
        }

        public ActionResult RegenerarAguinaldos()
        {
            Aguinaldo aguinaldo = new Aguinaldo();
            aguinaldo.RecalcularAguinaldos();

            return RedirectToAction("Index");
        }

        public ActionResult AprobarAguinaldos()
        {
            Persona userSesion = (Persona)Session["user"];
            int idJefaturaDirecta = userSesion.empleados.idEmpleado;

            Aguinaldo aguinaldo = new Aguinaldo();
            aguinaldo.AprobarAguinaldos(idJefaturaDirecta);

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
