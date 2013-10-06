using Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using TurnMeOff.Repository;
using TurnMeOff.Repository.Models;
using WebMatrix.WebData;

namespace TurnMeOff.Controllers
{

    [Authorize]
    public class DevicesController : Controller
    {
        protected readonly IRepository _repository;

        public DevicesController()
        {
            _repository = new TurnMeOff.Repository.Repository();
        }


        public ActionResult List()
        {
            var email = System.Web.HttpContext.Current.User.Identity.Name.ToString();
            var currentUser = _repository.GetUserByEmail(email);

            List<Device> devicesList = _repository.GetAllDevicesByUserId(currentUser.userID);

            ViewBag.Lista = devicesList;

            return View();
        }

        public void AddPendingDevice(Guid userId, Guid deviceId)
        {
            var device = new PendingDevice
            {
                deviceID = deviceId,
                userID = userId,
            };

            _repository.AddPendingDevice(device);
        }

        public ActionResult Add()
        {
            var pairingIsValid = false;
            var device = _repository.GetPendingDevice();

            var email = System.Web.HttpContext.Current.User.Identity.Name.ToString();
            var currentUser = _repository.GetUserByEmail(email);



            /* check if the current user is setting up this particular device */
            if (device != null && device.userID == currentUser.userID)
            {
                pairingIsValid = true;
                ViewBag.PendingDeviceUserId = device.userID;
                ViewBag.PendingDeviceId = device.deviceID;
            }

            if (device != null && !pairingIsValid)
            {
                ModelState.AddModelError("", "Woops. Looks like you're trying to add someone else's device:D");
            }
            return View();
        }

        [HttpPost]
        public ActionResult Add(DeviceModel model)
        {
            var email = System.Web.HttpContext.Current.User.Identity.Name.ToString();
            var currentUser = _repository.GetUserByEmail(email);
            try
            {
                _repository.AddDevice(model, currentUser.userID);
                _repository.DeletePendingDevices();
                return RedirectToAction("List");
            }
            catch
            {
                return View();
            }
        }

        public ActionResult Edit(Guid id)
        {
            var currentDevice = _repository.GetDeviceById(id);

            if (currentDevice == null)
            {
                return HttpNotFound();
            }
            ViewBag.Device = currentDevice;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(DeviceModel model)
        {
            var email = System.Web.HttpContext.Current.User.Identity.Name.ToString();
            var currentUser = _repository.GetUserByEmail(email);

            Device device = new Device
                {
                    deviceID = model.deviceID,
                    Name = model.Name,
                    Description = model.Description,
                    isSensor = model.isSensor,
                    isOn = model.isOn,
                    userID = currentUser.userID,
                };
            if (ModelState.IsValid)
            {
                _repository.EditDevice(device);
                return RedirectToAction("List");
            }
            return View(model);
        }

        [Authorize]
        public ActionResult Delete(Guid id)
        {
            if (ModelState.IsValid)
            {

                _repository.RemoveDevice(id);
                return RedirectToAction("List");

            }
            return View(id);
        }

        public ActionResult Settings()
        {
            return View();
        }
    }
}
