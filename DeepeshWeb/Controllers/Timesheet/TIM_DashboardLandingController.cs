using DeepeshWeb.BAL.EmployeeManagement;
using DeepeshWeb.BAL.Timesheet;
using DeepeshWeb.Models.Timesheet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DeepeshWeb.Controllers.TimeSheet
{
    public class TIM_DashboardLandingController : Controller
    {
        // GET: TIM_DashboardLanding
        TIM_ProjectCreationBal BalProjectCreation = new TIM_ProjectCreationBal();
        TIM_MilestoneBal BalMilestone = new TIM_MilestoneBal();
        TIM_StatusMasterBal BalStatus = new TIM_StatusMasterBal();
        Emp_BasicInfoBal BalEmp = new Emp_BasicInfoBal();
        TIM_ProjectTypeMasterBal BalProjectType = new TIM_ProjectTypeMasterBal();

        public ActionResult Index()
        {
            List<TIM_ProjectCreationModel> lstProjectCreation = new List<TIM_ProjectCreationModel>();
            try
            {
                List<TIM_ProjectTypeMasterModel> lstProjectType = new List<TIM_ProjectTypeMasterModel>();
                List<TIM_StatusMasterModel> lstStatus = new List<TIM_StatusMasterModel>();
                //int ProjectId = Convert.ToInt32(Request.Cookies["ProjectId"].Value);
                var spContext = SharePointContextProvider.Current.GetSharePointContext(HttpContext);
                using (var clientContext = spContext.CreateUserClientContextForSPHost())
                {

                    //lstProjectCreation = BalProjectCreation.GetProjectCreationById(clientContext, ProjectId);
                    //ViewBag.MilestoneData = BalMilestone.GetMilestoneByProjectId(clientContext, ProjectId);
                    lstStatus = BalStatus.GetStatusForAction(clientContext);
                    lstProjectType = BalProjectType.GetProjectType(clientContext);
                    //var allProductsWithNoDuplicates = lstStatus.Union(lstProjectType).DistinctBy(x => x.ID);
                    //ViewBag.EmpData = BalEmp.GetEmp(clientContext);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("An error occured while performing action. GUID: {0}", ex.ToString()));
            }

            return View(lstProjectCreation[0]);
        }

        [HttpPost]
        [ActionName("GetMilestoneData")]
        public JsonResult GetMilestoneData()
        {
            List<object> obj = new List<object>();
            try
            {
                int ProjectId = Convert.ToInt32(Request.Cookies["ProjectId"].Value);
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
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("An error occured while performing action. GUID: {0}", ex.ToString()));
            }
            return Json(obj, JsonRequestBehavior.AllowGet);
        }
    }
}