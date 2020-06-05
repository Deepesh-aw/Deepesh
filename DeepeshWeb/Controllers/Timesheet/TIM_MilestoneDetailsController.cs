using DeepeshWeb.BAL.Timesheet;
using DeepeshWeb.Models.Timesheet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DeepeshWeb.Controllers.TimeSheet
{
    public class TIM_MilestoneDetailsController : Controller
    {
        // GET: MilestoneDetails
        TIM_ProjectCreationBal BalProjectCreation = new TIM_ProjectCreationBal();
        TIM_MilestoneBal BalMilestone = new TIM_MilestoneBal();
        public ActionResult Index()
        {
            List<TIM_ProjectCreationModel> lstProjectCreation = new List<TIM_ProjectCreationModel>();
            try
            {
                var spContext = SharePointContextProvider.Current.GetSharePointContext(HttpContext);
                using (var clientContext = spContext.CreateUserClientContextForSPHost())
                {
                    lstProjectCreation = BalProjectCreation.GetProjectCreationById(clientContext);
                    ViewBag.MilestoneData = BalMilestone.GetMilestoneByProjectId(clientContext);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("An error occured while performing action. GUID: {0}", ex.ToString()));
            }

            return View(lstProjectCreation[0]);
        }
    }
}