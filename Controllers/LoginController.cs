using DataBase_RH_BanderaBlanca.Models;
using System;
using System.Collections.Generic; using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RH_BanderaBlanca.Controllers
{
    public class LoginController : Controller
    {
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
