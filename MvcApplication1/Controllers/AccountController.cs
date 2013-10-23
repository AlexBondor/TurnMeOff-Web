
using TurnMeOff.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using TurnMeOff.Repository.Models;
using WebMatrix.WebData;
using Repository;

namespace TurnMeOff.Controllers
{
    public class AccountController : Controller
    {
        protected readonly IRepository _repository;

        public AccountController()
        {
            _repository = new TurnMeOff.Repository.Repository();
        }

        public ActionResult Register()
        {
            RegisterModel register = new RegisterModel();
            return View(register);
        }

        [HttpPost]
        public ActionResult Register(RegisterModel model, string returnURL)
        {
            if (ModelState.IsValid)
            {
                if (_repository.GetUserByEmail(model.Email) == null)
                {
                    if (_repository.GetMasterDeviceById(model.MasterDeviceID) != null)
                    {
                        _repository.AddUser(model);
                        _repository.RemoveMasterDeviceFromMasterDevices(model.MasterDeviceID);
                        FormsAuthentication.SetAuthCookie(model.Email, true);
                        return RedirectToAction("List", "Devices");
                    }
                    ModelState.AddModelError("masterDeviceID", "Code not found! Please try again");

                    return View(model);
                }
                ModelState.AddModelError("Email", "An account has already been created for this Email!");
            }

            return View(model);
        }
       
        public ActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
                return RedirectToAction("List", "Devices");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel model, string returnURL)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (_repository.GetUserMatchPassword(model.Email, model.Password))
                    {
                        FormsAuthentication.SetAuthCookie(model.Email, model.RememberMe);
                        if (Url.IsLocalUrl(returnURL))
                        {
                            return Redirect(returnURL);
                        }
                        return RedirectToAction("List", "Devices");
                    }
                    if (_repository.GetUserByEmail(model.Email) == null)
                    {
                        ModelState.AddModelError("Email", "Please check out Email spelling!");
                    }
                    else
                    {
                        ModelState.AddModelError("Password", "The provided password is incorrect!");
                    }
                }
                catch
                {
                    if (_repository.GetUserByEmail(model.Email) == null)
                    {
                        ModelState.AddModelError("Email", "Check out Email spelling!");
                    }
                    else
                    {
                        ModelState.AddModelError("Password", "The provided password is incorrect!");
                    }
                }
            }

            return View(model);
        }
        
        [Authorize]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "Account");
        }

        [Authorize]
        public ActionResult Edit()
        {
            var email = System.Web.HttpContext.Current.User.Identity.Name.ToString();
            var currentUser = _repository.GetUserByEmail(email);

            UserModel model = new UserModel
            {
                UserID = currentUser.userID,
                Firstname = currentUser.Firstname,
                Lastname = currentUser.Lastname,
                Email = currentUser.Email,
                Password = currentUser.Password,
            };

            return View(model);
        }

        [Authorize]
        [HttpPost]
        public ActionResult Edit(UserModel model)
        {
            User user = new User
            {
                userID = model.UserID,
                Firstname = model.Firstname,
                Lastname = model.Lastname,
                Email = model.Email,
                Password = model.Password,
            };

            if (ModelState.IsValid)
            {
                _repository.EditUser(user);
                return RedirectToAction("List", "Devices");
            }

            return View(model);
        }

        [Authorize]
        public ActionResult Delete(Guid id)
        {
            if (ModelState.IsValid)
            {
                FormsAuthentication.SignOut();
                _repository.RemoveUser(id);
                return RedirectToAction("Login", "Account");
            }

            return View(id);
        }
    }
}
