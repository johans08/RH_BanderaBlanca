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
    public class sucursalesController : Controller
    {
        private BD_BanderaBlancaEntities db = new BD_BanderaBlancaEntities();

        // GET: sucursales
        public ActionResult Index()
        {
            var sucursales = db.sucursales
            .Include(p => p.telefonos_sucursales) // Incluir la relación con telefonos
            .Include(p => p.direcciones_empresas) // Incluir la relación con direcciones
            .ToList();

            var viewModelList = new List<Sucursal>();

            foreach (var sucursal in sucursales)
            {
                var direccion = sucursal.direcciones_empresas.FirstOrDefault();
                var telefono = sucursal.telefonos_sucursales.FirstOrDefault();

                var viewModel = new Sucursal
                {
                    sucursales = sucursal,
                    telefonos = telefono,
                    direcciones_Empresas = direccion
                };

                viewModelList.Add(viewModel);
            }


            return View(viewModelList);

        }

        // GET: sucursales/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            sucursales sucursales = db.sucursales.Find(id);
            if (sucursales == null)
            {
                return HttpNotFound();
            }
            return View(sucursales);
        }

        // GET: sucursales/Create
        public ActionResult Create()
        {
            var sucursalModel = new Sucursal(); // Inicializa el modelo Sucursal

            ViewBag.catalogo_telefonicos = new SelectList(db.catalogo_telefonicos.ToList(), "idCatalogo_Telefonico", "Catalogo_Telefonico");
            ViewBag.Distritos = new SelectList(db.distritos.ToList(), "idDistrito", "Nombre_Distrito");
            ViewBag.Cantones = new SelectList(db.cantones.ToList(), "idCanton", "Nombre_Canton");
            ViewBag.Provincias = new SelectList(db.provincias.ToList(), "idProvincia", "Nombre_Provincia");
            ViewBag.TipoTelefono = new SelectList(db.catalogo_telefonicos.ToList(), "idCatalogo_Telefonico", "Catalogo_Telefonico");

            return View(sucursalModel); // Pasa el modelo a la vista
        }

        // POST: sucursales/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Sucursal post_sucursales)
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    if (ModelState.IsValid)
                    {
                        if (post_sucursales.CrearSucursal(post_sucursales))
                        {
                            return RedirectToAction("Index");
                        }
                    }

                    ModelState.AddModelError("", "No se ha guardado la sucursal ");

                    ViewBag.catalogo_telefonicos = new SelectList(db.catalogo_telefonicos.ToList(), "idCatalogo_Telefonico", "Catalogo_Telefonico", post_sucursales.telefonos.idCatalogo_Telefonico);
                    ViewBag.Distritos = new SelectList(db.distritos.ToList(), "idDistrito", "Nombre_Distrito", post_sucursales.direcciones_Empresas.idDistrito);
                    ViewBag.Cantones = new SelectList(db.cantones.ToList(), "idCanton", "Nombre_Canton", post_sucursales.direcciones_Empresas.idCanton);
                    ViewBag.Provincias = new SelectList(db.provincias.ToList(), "idProvincia", "Nombre_Provincia", post_sucursales.direcciones_Empresas.idProvincia);
                    ViewBag.TipoTelefono = new SelectList(db.catalogo_telefonicos.ToList(), "idCatalogo_Telefonico", "Catalogo_Telefonico", post_sucursales.telefonos.idCatalogo_Telefonico);

                    return View(post_sucursales);
                }
                catch (Exception ex)
                {
                    // Si ocurre algún error, deshacer la transacción
                    transaction.Rollback();

                    // Aquí puedes loguear el error si es necesario
                    ModelState.AddModelError("", "Ocurrió un error al guardar los datos: " + ex.Message);

                    ViewBag.catalogo_telefonicos = new SelectList(db.catalogo_telefonicos.ToList(), "idCatalogo_Telefonico", "Catalogo_Telefonico", post_sucursales.telefonos.idCatalogo_Telefonico);
                    ViewBag.Distritos = new SelectList(db.distritos.ToList(), "idDistrito", "Nombre_Distrito", post_sucursales.direcciones_Empresas.idDistrito);
                    ViewBag.Cantones = new SelectList(db.cantones.ToList(), "idCanton", "Nombre_Canton", post_sucursales.direcciones_Empresas.idCanton);
                    ViewBag.Provincias = new SelectList(db.provincias.ToList(), "idProvincia", "Nombre_Provincia", post_sucursales.direcciones_Empresas.idProvincia);
                    ViewBag.TipoTelefono = new SelectList(db.catalogo_telefonicos.ToList(), "idCatalogo_Telefonico", "Catalogo_Telefonico", post_sucursales.telefonos.idCatalogo_Telefonico);


                    return View(post_sucursales);
                }
            }
        }


        // GET: sucursales/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            sucursales sucursales = db.sucursales.Find(id);
            if (sucursales == null)
            {
                return HttpNotFound();
            }
            return View(sucursales);
        }

        // POST: sucursales/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Identificador_Sucursal,Nombre_Sucursal,Fecha_Creacion")] sucursales sucursales)
        {
            if (ModelState.IsValid)
            {
                db.Entry(sucursales).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(sucursales);
        }

        // GET: sucursales/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            sucursales sucursales = db.sucursales.Find(id);
            if (sucursales == null)
            {
                return HttpNotFound();
            }
            return View(sucursales);
        }

        // POST: sucursales/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            sucursales sucursales = db.sucursales.Find(id);
            db.sucursales.Remove(sucursales);
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
