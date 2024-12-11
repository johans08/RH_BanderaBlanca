using DataBase_RH_BanderaBlanca.Models;
using System;
using System.Collections.Generic; using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RH_BanderaBlanca.Controllers
{
    public class LoginController : Controller
    {

        private BD_BanderaBlancaEntities db = new BD_BanderaBlancaEntities();

        // GET: Login
        public ActionResult Login()
        {
            Login login = new Login();
            return View(login);
        }

        [HttpPost]
        public ActionResult ValidarCredenciales(Login login)
        {
            if (!ModelState.IsValid)
            {
                Session["Error"] = "Usuario o contraseña invalidas";
                return View("Login", login);
            }
            bool valido = false;
            Persona _personaSesion = null;

            (valido, _personaSesion) = login.ValidarUsuario(login);

            Session["user"] = _personaSesion;

            if (valido == false)
            {
                Session["Error"] = "Usuario o contraseña incorrectas";
                return View("Login", login);
            }

            if (_personaSesion.usuarios.Estados_Sistema_idEstado != 1)
            {
                if (_personaSesion.empleados.Fecha_Ingreso <= DateTime.Today)
                {
                    try
                    {
                        usuarios _user = new usuarios
                        {
                            // Crea una nueva instancia con los valores necesarios
                            idUsuario = _personaSesion.usuarios.idUsuario, // Incluye la clave primaria
                            Nombre_Usuario = _personaSesion.usuarios.Nombre_Usuario,
                            Clave = _personaSesion.usuarios.Clave,
                            Fecha_Creacion = _personaSesion.usuarios.Fecha_Creacion,
                            idEmpleado = _personaSesion.usuarios.idEmpleado,
                            Identificador_Sucursal = _personaSesion.usuarios.Identificador_Sucursal,
                            PrimerIngreso = _personaSesion.usuarios.PrimerIngreso,
                            idRol = _personaSesion.usuarios.idRol,
                            Estados_Sistema_idEstado = 1 // Establece el estado actualizado
                        };

                        db.Entry(_user).State = EntityState.Modified; // Marca la entidad como modificada
                        db.SaveChanges(); // Guarda los cambios
                        return RedirectToAction("Index", "Home");
                    }
                    catch (DbEntityValidationException ex)
                    {
                        foreach (var validationErrors in ex.EntityValidationErrors)
                        {
                            foreach (var validationError in validationErrors.ValidationErrors)
                            {
                                System.Diagnostics.Debug.WriteLine($"Property: {validationError.PropertyName} Error: {validationError.ErrorMessage}");
                            }
                        }
                        throw;
                    }


                }

                Session["Error"] = "El usuario no esta activo";
                return View("Login", login);
            }

            //if (_personaSesion.usuarios.PrimerIngreso)
            //{
            //    return RedirectToAction("Edit", "Usuario");
            //}

            return RedirectToAction("Index", "Home");
        }


        public ActionResult LogOut()
        {
            Session["user"] = null;

            return RedirectToAction("Login");
        }
    }
}
