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

        public ActionResult Details()
        { 
            /* create object of type customer */
            /* lista de interese care vinde dintr un tabel din db ..*/
             List<string> interese = new List<string>();
             interese.Add("belit");
             interese.Add("asfasdfas");
             interese.Add("asfasdfas");
             interese.Add("asfasdfas");
             interese.Add("asfasdfas");
             interese.Add("asfasdfas");
             interese.Add("asfasdfas");

            var customer = new UserModel();
            customer.Name = "Alex";
            customer.Email = "xxx@gmail.com";
            customer.Age = 20;
            customer.Interests = new List<string>();// asta ii hack sa ai lista initializata ca altfel adaugi elementre intr o lista care nu exista
            //cand le aduci din db nu ai problema asta

            // mai jos ar fi defapt, pt fiecare interes al userului din db, baga-l in lista, iar in view, pt firecare element
            //din lista de interese, baga l in <ul>

            foreach (var item in interese)
            {
                //if(item != null)
                    customer.Interests.Add(item);
            }
    
            return View(customer);
        }
    }
}
