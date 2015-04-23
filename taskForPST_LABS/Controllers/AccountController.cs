using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using taskForPST_LABS.DAL.Models;

namespace taskForPST_LABS.Controllers
{
    public class AccountController : Controller
    {
        // GET: Account
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(User model)
        {
            if (ModelState.IsValid)
            {
                using (var entities = new Entities())
                {

                    bool userExists = entities.Users.Any(user => user.Username == model.Username);
                    bool emailExists = entities.Users.Any(user => user.Email == model.Email);

                    // если такой  пользователь существует
                    if (userExists)
                    {
                        ModelState.AddModelError("Username", "Такой пользователь существует");
                    }
                    // аналогично email
                    if (emailExists)
                    {
                        ModelState.AddModelError("Email", "Данный email занят другим пользователем");
                    }

                    if (!emailExists && !userExists)
                    {
                        entities.Users.Add(model);
                        entities.SaveChanges();
                        FormsAuthentication.SetAuthCookie(model.Username, false);
                        return RedirectToAction("Index", "Home");
                    }      
                }
            }
            return View();
        }
     
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(User model, string returnUrl)
        {

            // Lets first check if the Model is valid or not
            if (ModelState.IsValidField("Username") && ModelState.IsValidField("Password"))
            {
                using (var entities = new Entities())
                {
                    string username = model.Username;
                    string password = model.Password;

                    bool userValid = entities.Users.Any(user => user.Username == username && user.Password == password);


                    if (userValid)
                    {

                        FormsAuthentication.SetAuthCookie(username, false);
                        if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
                            && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
                        {
                            return Redirect(returnUrl);
                        }
                        else
                        {
                            return RedirectToAction("Index", "Home");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "Не правильный пользователь/пароль.");
                    }
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();

            return RedirectToAction("Index", "Home");
        }
    }
}