using DeepeshWeb.BAL.Timesheet;
using DeepeshWeb.Models.Timesheet;
using DeepeshWeb.BAL.EmployeeManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DeepeshWeb.Controllers.TimeSheet
{
    public class TIM_AddTaskController : Controller
    {
        // GET: TIM_Task
        TIM_ProjectCreationBal BalProjectCreation = new TIM_ProjectCreationBal();
        TIM_MilestoneBal BalMilestone = new TIM_MilestoneBal();
        TIM_StatusMasterBal BalStatus = new TIM_StatusMasterBal();
        Emp_BasicInfoBal BalEmp = new Emp_BasicInfoBal();
        public ActionResult Index()
        {
            List<TIM_ProjectCreationModel> lstProjectCreation = new List<TIM_ProjectCreationModel>();
            try
            {
                int MilestoneId = Convert.ToInt32(Request.Cookies["MilestoneId"].Value);
                var spContext = SharePointContextProvider.Current.GetSharePointContext(HttpContext);
                using (var clientContext = spContext.CreateUserClientContextForSPHost())
                {
                    lstProjectCreation = BalProjectCreation.GetProjectCreationById(clientContext);
                    ViewBag.MilestoneData = BalMilestone.GetMilestoneByMilestoneId(clientContext, MilestoneId);
                    ViewBag.StatusData = BalStatus.GetStatusForAction(clientContext);
                    ViewBag.EmpData = BalEmp.GetEmp(clientContext);
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