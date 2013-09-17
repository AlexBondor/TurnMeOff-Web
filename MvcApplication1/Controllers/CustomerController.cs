using MvcApplication1.Models.UserModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcApplication1.Controllers
{
    public class CustomerController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        //public ActionResult Details()
        //{ 
        //    /* create object of type customer */
        //    /* lista de interese care vinde dintr un tabel din db ..*/
        //     List<string> interese = new List<string>();
        //     interese.Add("belit");
        //     interese.Add("asfasdfas");
        //     interese.Add("asfasdfas");
        //     interese.Add("asfasdfas");
        //     interese.Add("asfasdfas");
        //     interese.Add("asfasdfas");
        //     interese.Add("asfasdfas");

        //    var customer = new UserModel();
        //    customer.Name = "Alex";
        //    customer.Email = "xxx@gmail.com";
        //    customer.Age = 20;
        //    customer.Interests = new List<string>();// asta ii hack sa ai lista initializata ca altfel adaugi elementre intr o lista care nu exista
        //    //cand le aduci din db nu ai problema asta

        //    // mai jos ar fi defapt, pt fiecare interes al userului din db, baga-l in lista, iar in view, pt firecare element
        //    //din lista de interese, baga l in <ul>

        //    foreach (var item in interese)
        //    {
        //        //if(item != null)
        //            customer.Interests.Add(item);
        //    }

        //    return View(customer);
        //}
        public ActionResult Dashboard()
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

        public ActionResult AccountSettings()
        {
            var customer = new UserModel();
            customer.Name = "Alex Bondor";
            customer.Username = "alex";
            customer.Email = "xxx@gmail.com";
            return View(customer);
        }

        public ActionResult AddDevice()
        {
            return View();
        }
    }
}
