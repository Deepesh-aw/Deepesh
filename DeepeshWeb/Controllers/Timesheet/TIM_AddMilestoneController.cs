using DeepeshWeb.BAL.Timesheet;
using DeepeshWeb.Models.Timesheet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DeepeshWeb.Controllers.TimeSheet
{
    public class TIM_AddMilestoneController : Controller
    {
        TIM_ProjectCreationBal BalProjectCreation = new TIM_ProjectCreationBal();
        TIM_StatusMasterBal BalStatus = new TIM_StatusMasterBal();
        // GET: TIM_AddMilestone
        public ActionResult Index()
        {
            List<TIM_ProjectCreationModel> lstProjectCreation = new List<TIM_ProjectCreationModel>();
            var spContext = SharePointContextProvider.Current.GetSharePointContext(HttpContext);
            using (var clientContext = spContext.CreateUserClientContextForSPHost())
            {
                lstProjectCreation = BalProjectCreation.GetProjectCreationById(clientContext);
                ViewBag.StatusData = BalStatus.GetStatusForAction(clientContext);
            }
            return View(lstProjectCreation[0]);
        }
    }
}