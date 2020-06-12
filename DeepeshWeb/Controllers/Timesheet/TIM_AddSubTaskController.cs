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
        TIM_SubTaskBal BalSubTask = new TIM_SubTaskBal();
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

        [HttpPost]
        [ActionName("AddSubTask")]
        public JsonResult AddSubTask(List<TIM_SubTaskModel> AddSubTask)
        {
            List<object> obj = new List<object>();
            int i = 0;
            var spContext = SharePointContextProvider.Current.GetSharePointContext(HttpContext);
            using (var clientContext = spContext.CreateUserClientContextForSPHost())
            {
                List<TIM_WorkFlowMasterModel> lstWorkFlow = new List<TIM_WorkFlowMasterModel>();
                lstWorkFlow = BalWorkflow.GetWorkFlowForAddSubTask(clientContext);
                string returnID = "0";
                foreach (var item in AddSubTask)
                {

                    string itemdata = " 'MileStoneId': '" + item.MileStone + "'";
                    itemdata += " ,'TaskId': '" + item.Task + "'";
                    itemdata += " ,'MembersId': '" + item.Members + "'";
                    itemdata += " ,'StartDate': '" + item.StartDate + "'";
                    itemdata += " ,'EndDate': '" + item.EndDate + "'";
                    itemdata += " ,'NoOfDays': '" + item.NoOfDays + "'";
                    itemdata += " ,'ProjectId': '" + item.Project + "'";
                    itemdata += " ,'SubTask': '" + item.SubTask + "'";
                    itemdata += " ,'SubTaskStatusId': '" + item.SubTaskStatus + "'";

                    if (lstWorkFlow.Count > 0)
                    {
                        itemdata += " ,'StatusId': '" + lstWorkFlow[0].ToStatusID + "'";
                        itemdata += " ,'InternalStatus': '" + lstWorkFlow[0].InternalStatus + "'";
                    }
                    returnID = BalSubTask.SaveSubTask(clientContext, itemdata);
                    if (Convert.ToInt32(returnID) > 0)
                        i++;

                }
                if (i == AddSubTask.Count)
                {
                    string taskdata = "'StatusId': '" + lstWorkFlow[0].ToStatusID + "'";
                    taskdata += " ,'InternalStatus': '" + lstWorkFlow[0].InternalStatus + "'";
                    string returnTaskText = BalTask.UpdateTask(clientContext, taskdata, AddSubTask[0].Task.ToString());
                    if (returnTaskText == "Update")
                    {
                        string milestonedata = "'StatusId': '" + lstWorkFlow[0].ToStatusID + "'";
                        milestonedata += " ,'InternalStatus': '" + lstWorkFlow[0].InternalStatus + "'";
                        string returnMileText = BalMilestone.UpdateMilestone(clientContext, milestonedata, AddSubTask[0].MileStone.ToString());
                        if (returnMileText == "Update")
                        {
                            string projectdata = "'StatusId': '" + lstWorkFlow[0].ToStatusID + "'";
                            projectdata += " ,'InternalStatus': '" + lstWorkFlow[0].InternalStatus + "'";
                            string returnText = BalProjectCreation.UpdateProject(clientContext, projectdata, AddSubTask[0].Project.ToString());
                            if (returnText == "Update")
                                obj.Add("OK");
                        }
                    }
                }
            }
            return Json(obj, JsonRequestBehavior.AllowGet);
        }

    }
}