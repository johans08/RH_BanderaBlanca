using DataBase_RH_BanderaBlanca.Models;
using System;
using System.Collections.Generic; using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;


namespace RH_BanderaBlanca.Controllers
{
    public class personasController : Controller
    {
        private BD_BanderaBlancaEntities db = new BD_BanderaBlancaEntities();

        // GET: personas
        public ActionResult Index()
        {
            var personas = db.personas
               .Include(u => u.empleados)
               .Include(u => u.direcciones_personas)
               .Include(u => u.telefonos_personales)
               .Include(u => u.correos)
               .ToList();

            var viewModelList = new List<Persona>();

            foreach (var persona in personas)
            {
                var direccion = persona.direcciones_personas?.FirstOrDefault();
                var telefono = persona.telefonos_personales?.FirstOrDefault();
                var correo = persona.correos?.FirstOrDefault();
                var empleado = persona.empleados?.FirstOrDefault();

                // Verificar si usuario, empleado y persona no son nulos antes de crear el viewModel
                if (persona != null && persona.empleados != null)
                {
                    var viewModel = new Persona
                    {
                        empleados = empleado,
                        personas = persona,
                        direcciones = direccion,
                        telefonos = telefono,
                        correos = correo
                    };

                    viewModelList.Add(viewModel);
                }
            }

            if (TempData["QrCodeImage"] != null)
            {
                ViewBag.QrCodeImage = TempData["QrCodeImage"];
                ViewBag.Usuario = TempData["Usuario"];
                ViewBag.Contrasena = TempData["Contrasena"];
            }
            if (TempData["Error"] != null)
            {
                ViewBag.Error = TempData["Error"];
            }

            return View(viewModelList);


            

        }

        // GET: personas/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        public JsonResult ObtenerCantones(int idProvincia)
        {
            var cantones = db.cantones.Where(c => c.idProvincia == idProvincia).Select(c => new
            {
                id = c.idCanton,
                nombre = c.Nombre_Canton
            }).ToList();

            return Json(cantones, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ObtenerDistritos(int idCanton)
        {
            var distritos = db.distritos.Where(d => d.idCanton == idCanton).Select(d => new
            {
                id = d.idDistrito,
                nombre = d.Nombre_Distrito
            }).ToList();

            return Json(distritos, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ObtenerJefatura(int idPuesto)
        {
            var personaList = db.empleados.Where(d => d.idPuestos_Laboral == idPuesto).ToList();
            var personasListXPermiso = new List<object>(); // Lista para almacenar los resultados

            foreach (var persona in personaList)
            {
                var usuario = db.usuarios.FirstOrDefault(u => u.idEmpleado == persona.idEmpleado);

                // Verificar si se encontró el usuario asociado
                if (usuario != null)
                {
                    var personaEntity = db.personas.FirstOrDefault(u => u.Identificador == persona.Personas_Identificador);

                    if (personaEntity != null)
                    {
                        Persona _persona = new Persona();
                        _persona.personas = personaEntity;
                        _persona.usuarios = usuario;
                        _persona.listaAccesos = _persona.GenerarListaAccesos(_persona);

                        // Verificar si el usuario tiene el permiso con ID 7
                        if (_persona.ObtenerIdsDeAccesos(_persona.listaAccesos).Contains(7))
                        {
                            personasListXPermiso.Add(new
                            {
                                id = persona.idEmpleado,
                                nombre = $"{_persona.personas.Nombre} {_persona.personas.Primer_Apellido} {_persona.personas.Segundo_Apellido}"
                            });
                        }
                    }
                }
            }

            // Devolver la lista en formato JSON
            return Json(personasListXPermiso, JsonRequestBehavior.AllowGet);
        }




        // GET: personas/Create
        public ActionResult Create()
        {

            var model = new Persona();

            ViewBag.idTipo_Horario = new SelectList(db.tipo_horario.ToList(), "idTipo_Horario", "Tipo_Horario1");
            ViewBag.idEstado = new SelectList(db.estados_sistema.ToList(), "idEstado", "Estado");
            ViewBag.idRol = new SelectList(db.roles.ToList(), "idRol", "Descripcion_Rol");
            ViewBag.idSucursal = new SelectList(db.sucursales.ToList(), "Identificador_Sucursal", "Nombre_Sucursal");
            ViewBag.TipoCorreo = new SelectList(db.catalogo_correos.ToList(), "idCatalogo_Correo", "Tipo_Correo");
            ViewBag.TipoPuesto = new SelectList(db.puestos_laborales.ToList(), "idPuestos_Laboral", "Nombre_Puesto");
            ViewBag.idCatalogo_Identificador = new SelectList(db.catalogo_identificador.ToList(), "idCatalogo_Identificador", "Descripcion_Identificador");
            ViewBag.catalogo_telefonicos = new SelectList(db.catalogo_telefonicos.ToList(), "idCatalogo_Telefonico", "Catalogo_Telefonico");
            ViewBag.Distritos = new SelectList(db.distritos.ToList(), "idDistrito", "Nombre_Distrito");
            ViewBag.Cantones = new SelectList(db.cantones.ToList(), "idCanton", "Nombre_Canton");
            ViewBag.Provincias = new SelectList(db.provincias.ToList(), "idProvincia", "Nombre_Provincia");
            ViewBag.TipoTelefono = new SelectList(db.catalogo_telefonicos.ToList(), "idCatalogo_Telefonico", "Catalogo_Telefonico");

            return View(model);
        }

        // POST: personas/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Persona _persona)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    bool ok = false;
                    string qr = "";
                    string usuario = "";
                    string contrasena = "";

                    (ok, qr, usuario, contrasena) = _persona.CrearPersona(_persona);
                    if (ok)
                    {
                        TempData["QrCodeImage"] = "data:image/png;base64," + qr;
                        TempData["Usuario"] = usuario;
                        TempData["Contrasena"] = contrasena;
                    }
                    else
                    {
                        TempData["Error"] = "Ocurrió un error al crear la persona.";
                    }
                    return RedirectToAction("Index");
                }

                ModelState.AddModelError("", "No se ha guardado la persona");

                CargarViewBags(_persona);

                return View(_persona);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Ocurrió un error al guardar los datos: " + ex.Message);

                CargarViewBags(_persona);

                return View(_persona);
            }
        }

        private void CargarViewBags(Persona _persona)
        {
            ViewBag.idTipo_Horario = new SelectList(db.tipo_horario.ToList(), "idTipo_Horario", "Tipo_Horario1", _persona.horarios.idTipo_Horario);
            ViewBag.idEstado = new SelectList(db.estados_sistema.ToList(), "idEstado", "Estado", _persona.usuarios.Estados_Sistema_idEstado);
            ViewBag.idRol = new SelectList(db.roles.ToList(), "idRol", "Descripcion_Rol", _persona.usuarios.idRol);
            ViewBag.idSucursal = new SelectList(db.sucursales.ToList(), "Identificador_Sucursal", "Nombre_Sucursal", _persona.empleados.Identificador_Sucursal);
            ViewBag.TipoCorreo = new SelectList(db.catalogo_correos.ToList(), "idCatalogo_Correo", "Tipo_Correo", _persona.correos.idCatalogo_Correo);
            ViewBag.TipoPuesto = new SelectList(db.puestos_laborales.ToList(), "idPuestos_Laboral", "Nombre_Puesto", _persona.empleados.idPuestos_Laboral);
            ViewBag.idCatalogo_Identificador = new SelectList(db.catalogo_identificador.ToList(), "idCatalogo_Identificador", "Descripcion_Identificador", _persona.personas.idCatalogo_Identificador);
            ViewBag.catalogo_telefonicos = new SelectList(db.catalogo_telefonicos.ToList(), "idCatalogo_Telefonico", "Catalogo_Telefonico", _persona.telefonos.idCatalogo_Telefonico);
            ViewBag.Distritos = new SelectList(db.distritos.ToList(), "idDistrito", "Nombre_Distrito", _persona.direcciones.idDistrito);
            ViewBag.Cantones = new SelectList(db.cantones.ToList(), "idCanton", "Nombre_Canton", _persona.direcciones.idCanton);
            ViewBag.Provincias = new SelectList(db.provincias.ToList(), "idProvincia", "Nombre_Provincia", _persona.direcciones.idProvincia);
            ViewBag.TipoTelefono = new SelectList(db.catalogo_telefonicos.ToList(), "idCatalogo_Telefonico", "Catalogo_Telefonico", _persona.telefonos.idCatalogo_Telefonico);
        }

        // GET: personas/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: personas/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: personas/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: personas/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
