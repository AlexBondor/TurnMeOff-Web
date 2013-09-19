using MvcApplication1.Models.UserModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcApplication1.Controllers
{
    public class DevicesController : Controller
    {
        //
        // GET: /Devices/

        public ActionResult List()
        {
            List<string> devices = new List<string>();
            devices.Add("bec dormitor");
            devices.Add("priza birou");
            devices.Add("bec baie");
            devices.Add("priza dormitor");
            devices.Add("bec birou");
            devices.Add("temperatura pivnita");
            devices.Add("senzor miscare");
            devices.Add("bec afara");
            devices.Add("bec bucatarie");
            devices.Add("bec pivnita");
            devices.Add("priza baie");
            devices.Sort();


            var customer = new UserModel();
            customer.Name = "Alex";
            customer.Email = "xxx@gmail.com";
            customer.Devices = new List<string>();

            foreach (var item in devices)
            {
                //if(item != null)
                customer.Devices.Add(item[0].ToString().ToUpper() + item.Substring(1, item.Length - 1));
            }
            return View(customer);
        }
        
        public ActionResult Add()
        {
            return View();
        }
        
        public ActionResult Settings()
        {
            return View();
        }

    }
}
