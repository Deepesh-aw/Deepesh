using DeepeshWeb.BAL.Timesheet;
using DeepeshWeb.Models.Timesheet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DeepeshWeb.Controllers.TimeSheet
{
    public class TIM_ProjectDashboardController : Controller
    {
        // GET: TIM_ProjectDashboard
        TIM_ProjectCreationBal BalProjectCreation = new TIM_ProjectCreationBal();
        TIM_MilestoneBal BalMilestone = new TIM_MilestoneBal();
        TIM_TaskBal BalTask = new TIM_TaskBal();

        public ActionResult Index()
        {
            List<TIM_ProjectCreationModel> lstProjectCreation = new List<TIM_ProjectCreationModel>();
            try
            {
                var spContext = SharePointContextProvider.Current.GetSharePointContext(HttpContext);
                using (var clientContext = spContext.CreateUserClientContextForSPHost())
                {
                    lstProjectCreation = BalProjectCreation.GetProjectCreationAllItems(clientContext);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("An error occured while performing action. GUID: {0}", ex.ToString()));
            }
            return View(lstProjectCreation);
        }

        [HttpPost]
        [ActionName("GetMilestone")]
        public JsonResult GetMilestone(int ProjectId)
        {
            List<object> obj = new List<object>();
            List<TIM_MilestoneModel> lstMilestone = new List<TIM_MilestoneModel>();
            var spContext = SharePointContextProvider.Current.GetSharePointContext(HttpContext);
            using (var clientContext = spContext.CreateUserClientContextForSPHost())
            {
                lstMilestone = BalMilestone.GetMilestoneByProjectId(clientContext, ProjectId);
                if (lstMilestone.Count > 0)
                {
                    obj.Add("OK");
                    obj.Add(lstMilestone);
                }
            }
            return Json(obj, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ActionName("GetTask")]
        public JsonResult GetTask(int MilestoneId)
        {
            List<object> obj = new List<object>();
            List<TIM_TaskModel> lstTask = new List<TIM_TaskModel>();
            var spContext = SharePointContextProvider.Current.GetSharePointContext(HttpContext);
            using (var clientContext = spContext.CreateUserClientContextForSPHost())
            {
                lstTask = BalTask.GetTaskByMilestoneId(clientContext, MilestoneId);
                if (lstTask.Count > 0)
                {
                    obj.Add("OK");
                    obj.Add(lstTask);
                }
            }
            return Json(obj, JsonRequestBehavior.AllowGet);
        }
    }
}