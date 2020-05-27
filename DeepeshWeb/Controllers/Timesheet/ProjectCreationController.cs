using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DeepeshWeb.BAL.Timesheet;
using DeepeshWeb.Models.Timesheet;

namespace DeepeshWeb.Controllers.TimeSheet
{
    public class ProjectCreationController : Controller
    {
        ProjectTypeBal BalprojectType = new ProjectTypeBal();
        ClientBal BalClient = new ClientBal();
        EmpBal BalEmp = new EmpBal();

        // GET: ProjectCreation
        public ActionResult Index()
        {
            var spContext = SharePointContextProvider.Current.GetSharePointContext(HttpContext);
            using (var clientContext = spContext.CreateUserClientContextForSPHost())
            {
                ViewBag.ProjectTypeData = BalprojectType.GetProjectType(clientContext);
                ViewBag.ClientData = BalClient.GetClient(clientContext);
                ViewBag.EmpData = BalEmp.GetEmp(clientContext);
            }
            return View();
        }

        [SharePointContextFilter]
        [ActionName("SaveProject")]
        public JsonResult SaveProject(ProjectCreationModel Project)
        {
            List<object> obj = new List<object>();
            return Json(obj, JsonRequestBehavior.AllowGet);
        }

    }
}