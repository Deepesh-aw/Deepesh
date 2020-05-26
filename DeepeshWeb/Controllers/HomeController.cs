using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DeepeshWeb.DAL;

 

namespace DeepeshWeb.Controllers
{
    public class HomeController : Controller
    {
        [SharePointContextFilter]
        public ActionResult Index()
        {
            User spUser = null;

            var spContext = SharePointContextProvider.Current.GetSharePointContext(HttpContext);
            AuthenticateUser authenticateUser = new AuthenticateUser();
            List<object> obj = new List<object>();

            using (var clientContext = spContext.CreateUserClientContextForSPHost())
            {
                if (clientContext != null)
                {
                    string SPHostUrl = spContext.SPHostUrl.ToString();
                    //  obj = authenticateUser.CheckUser(SPHostUrl);
                    obj.Add("Authanticated");
                    if (obj.Count > 0)
                    {
                        if (obj[0].ToString() == "Authanticated")
                        {
                            Session["Authanticated"] = "Yes";
                        }
                        else
                        {
                            Session["Authanticated"] = "No";
                            return Redirect("/Error");
                        }

                    }
                    if (Session["Authanticated"].ToString() == "Yes")
                    {
                        spUser = clientContext.Web.CurrentUser;
                        clientContext.Load(spUser, user => user.Id);
                        clientContext.ExecuteQuery();


                        Session["UserID"] = spUser.Id;
                        Session["Navigation"] = "";
                        ViewBag.UserName = spUser.Id;
                    }
                }
            }

            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}
