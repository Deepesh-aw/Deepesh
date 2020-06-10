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
    public class TIM_AddSubTaskController : Controller
    {
        // GET: TIM_AddSubTask
        TIM_ProjectCreationBal BalProjectCreation = new TIM_ProjectCreationBal();
        TIM_TaskBal BalTask = new TIM_TaskBal();
        TIM_MilestoneBal BalMilestone = new TIM_MilestoneBal();
        TIM_StatusMasterBal BalStatus = new TIM_StatusMasterBal();
        Emp_BasicInfoBal BalEmp = new Emp_BasicInfoBal();
        TIM_WorkFlowMasterBal BalWorkflow = new TIM_WorkFlowMasterBal();


        public ActionResult Index()
        {
            List<TIM_ProjectCreationModel> lstProjectCreation = new List<TIM_ProjectCreationModel>();
            try
            {
                int TaskId = Convert.ToInt32(Request.Cookies["TaskId"].Value);
                int ProjectId = Convert.ToInt32(Request.Cookies["ProjectId"].Value);
                int MilestoneId = Convert.ToInt32(Request.Cookies["MilestoneId"].Value);
                var spContext = SharePointContextProvider.Current.GetSharePointContext(HttpContext);
                using (var clientContext = spContext.CreateUserClientContextForSPHost())
                {
                    lstProjectCreation = BalProjectCreation.GetProjectCreationById(clientContext, ProjectId);
                    ViewBag.MilestoneData = BalMilestone.GetMilestoneByMilestoneId(clientContext, MilestoneId);
                    ViewBag.TaskData = BalTask.GetTaskByTaskId(clientContext, TaskId);
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