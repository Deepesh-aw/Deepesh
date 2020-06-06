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

        [HttpPost]
        [ActionName("AddTask")]
        public JsonResult AddTask(List<TIM_TaskModel> AddTask)
        {
            List<object> obj = new List<object>();
            int i = 0;
            var spContext = SharePointContextProvider.Current.GetSharePointContext(HttpContext);
            using (var clientContext = spContext.CreateUserClientContextForSPHost())
            {
                List<TIM_WorkFlowMasterModel> lstWorkFlow = new List<TIM_WorkFlowMasterModel>();
                lstWorkFlow = BalWorkflow.GetWorkFlowForAddTask(clientContext);
                string returnID = "0";
                foreach (var item in AddTask)
                {
                    string arr = String.Join(",", item.Members);

                    string itemdata = " 'MileStoneId': '" + item.MileStone + "'";
                    itemdata += " ,'MembersId': '" + item.Members + "'";
                    itemdata += " ,'StartDate': '" + item.StartDate + "'";
                    itemdata += " ,'EndDate': '" + item.EndDate + "'";
                    itemdata += " ,'NoOfDays': '" + item.NoOfDays + "'";
                    itemdata += " ,'ProjectId': '" + item.Project + "'";
                    itemdata += " ,'Task': '" + item.Task + "'";
                    itemdata += " ,'TaskStatusId': '" + item.TaskStatus + "'";

                    if (lstWorkFlow.Count > 0)
                    {
                        itemdata += " ,'StatusId': '" + lstWorkFlow[0].ToStatusID + "'";
                        itemdata += " ,'InternalStatus': '" + lstWorkFlow[0].InternalStatus + "'";
                    }
                    returnID = BalTask.SaveTask(clientContext, itemdata);
                    if (Convert.ToInt32(returnID) > 0)
                        i++;

                }
                if (i == AddTask.Count)
                    obj.Add("OK");
            }
            return Json(obj, JsonRequestBehavior.AllowGet);
        }

    }
}