using DeepeshWeb.BAL.EmployeeManagement;
using DeepeshWeb.BAL.Timesheet;
using DeepeshWeb.Models.Timesheet;
using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DeepeshWeb.Controllers.TimeSheet
{
    public class TIM_ManagerDashboardController : Controller
    {
        Emp_BasicInfoBal BalEmp = new Emp_BasicInfoBal();
        TIM_EmployeeTimesheetBal BalEmpTimesheet = new TIM_EmployeeTimesheetBal();
        TIM_WorkFlowMasterBal BalWorkflow = new TIM_WorkFlowMasterBal();
        TIM_ProjectCreationBal BalProjectCreation = new TIM_ProjectCreationBal();
        TIM_MilestoneBal BalMilestone = new TIM_MilestoneBal();
        TIM_TaskBal BalTask = new TIM_TaskBal();
        TIM_SubTaskBal BalSubTask = new TIM_SubTaskBal();
        TIM_SendEmailController EmailCtrl = new TIM_SendEmailController();
        TIM_TimesheetParentBal BalTimesheetParent = new TIM_TimesheetParentBal();

        // GET: TIM_ManagerDashboard
        public ActionResult Index()
        {
            
            return View();
        }

        

        [HttpPost]
        [ActionName("LoadTimesheetData")]
        public JsonResult LoadTimesheetData()
        {
            List<object> obj = new List<object>();

            try
            {
                var spContext = SharePointContextProvider.Current.GetSharePointContext(HttpContext);
                using (var clientContext = spContext.CreateUserClientContextForSPHost())
                {
                    List<TIM_TimesheetParentModel> lstTimesheetParent = BalTimesheetParent.GetEmpTimesheetByManagerId(clientContext, BalEmp.GetEmpByLogIn(clientContext));
                    obj.Add("OK");
                    obj.Add(lstTimesheetParent);

                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("An error occured while performing action. GUID: {0}", ex.ToString()));
            }
            return Json(obj, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ActionName("ApproveTimesheet")]
        public JsonResult ApproveTimesheet(List<TIM_EmployeeTimesheetModel> TimesheetData, string Action, string Descrition)
        {
            List<object> obj = new List<object>();
            try
            {
                var spContext = SharePointContextProvider.Current.GetSharePointContext(HttpContext);
                using (var clientContext = spContext.CreateUserClientContextForSPHost())
                {
                    if(Action == "Approve")
                    {
                        List<TIM_WorkFlowMasterModel> lstWorkFlowForApproveTimesheet = new List<TIM_WorkFlowMasterModel>();
                        lstWorkFlowForApproveTimesheet = BalWorkflow.GetWorkFlowForTimesheetApprove(clientContext);
                        int i = 0;
                        var itemdata = "'StatusId': '" + lstWorkFlowForApproveTimesheet[0].ToStatusID + "'";
                        itemdata += " ,'InternalStatus': '" + lstWorkFlowForApproveTimesheet[0].InternalStatus + "'";
                        itemdata += " ,'ApproveDescription': '" + Descrition.Replace("'", @"\'") + "'";
                        itemdata += " ,'ApproveDate': '" + DateTime.Today.ToString("MM-dd-yyyy hh:mm:ss") + "'";

                        foreach (var item in TimesheetData)
                        {
                            string returnID = BalEmpTimesheet.UpdateTimesheet(clientContext, itemdata, item.ID.ToString());
                            if (returnID == "Update")
                            {
                                i++;
                                if (item.Task > 0)
                                {
                                    if(item.AllTaskStatusName == "Completed")
                                    {
                                        var taskdata = "'TaskStatusId': '" + lstWorkFlowForApproveTimesheet[0].ToStatusID + "'";
                                        string TaskUpdate = BalTask.UpdateTask(clientContext, taskdata, item.Task.ToString());
                                    }

                                }
                                else if(item.SubTask > 0)
                                {
                                    if (item.AllTaskStatusName == "Completed")
                                    {
                                        var taskdata = "'TaskStatusId': '" + lstWorkFlowForApproveTimesheet[0].ToStatusID + "'";
                                        string SubTaskUpdate = BalSubTask.UpdateSubTask(clientContext, taskdata, item.SubTask.ToString());
                                    }
                                }
                            }
                        }
                        if (i == TimesheetData.Count)
                        {
                            string returnID = BalTimesheetParent.UpdateTimesheet(clientContext, itemdata, TimesheetData[0].ParentID.ToString());
                            if(returnID == "Update")
                            {
                                string Mailres = EmailCtrl.TimesheetApproveAndRejectNotification(clientContext, TimesheetData[0], "Approved");
                                if (Convert.ToInt32(Mailres) > 0)
                                    obj.Add("OK");
                            }
                            
                        }
                            
                    }
                    else if (Action == "Reject")
                    {
                        List<TIM_WorkFlowMasterModel> lstWorkFlowForRejectTimesheet = new List<TIM_WorkFlowMasterModel>();
                        lstWorkFlowForRejectTimesheet = BalWorkflow.GetWorkFlowForTimesheetReject(clientContext);
                        string taskdata = "'StatusId': '" + lstWorkFlowForRejectTimesheet[0].ToStatusID + "'";
                        taskdata += " ,'InternalStatus': '" + lstWorkFlowForRejectTimesheet[0].InternalStatus + "'";
                        int i = 0;
                        var itemdata = "'StatusId': '" + lstWorkFlowForRejectTimesheet[0].ToStatusID + "'";
                        itemdata += " ,'InternalStatus': '" + lstWorkFlowForRejectTimesheet[0].InternalStatus + "'";
                        itemdata += " ,'RejectDescription': '" + Descrition.Replace("'", @"\'") + "'";
                        itemdata += " ,'RejectedDate': '" + DateTime.Today.ToString("MM/dd/yyyy HH:mm:ss") + "'";

                        foreach (var item in TimesheetData)
                        {
                            string returnID = BalEmpTimesheet.UpdateTimesheet(clientContext, itemdata, item.ID.ToString());
                            if (returnID == "Update")
                                i++;
                        }
                        if (i == TimesheetData.Count)
                        {
                            string returnID = BalTimesheetParent.UpdateTimesheet(clientContext, itemdata, TimesheetData[0].ParentID.ToString());
                            if (returnID == "Update")
                            {
                                string Mailres = EmailCtrl.TimesheetApproveAndRejectNotification(clientContext, TimesheetData[0], "Rejected");
                                if (Convert.ToInt32(Mailres) > 0)
                                    obj.Add("OK");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                obj.Add("ERROR");
                return Json(obj, JsonRequestBehavior.AllowGet);
                throw new Exception(string.Format("An error occured while performing action. GUID: {0}", ex.ToString()));
            }
            return Json(obj, JsonRequestBehavior.AllowGet);
        }
        
        public void UpdateAllStatus(string taskdata, TIM_EmployeeTimesheetModel item, List<TIM_WorkFlowMasterModel> lstWorkFlowForApproveTimesheet)
        {
            var spContext = SharePointContextProvider.Current.GetSharePointContext(HttpContext);
            using (var clientContext = spContext.CreateUserClientContextForSPHost())
            {
                List<TIM_TaskModel> lstTask = new List<TIM_TaskModel>();
                lstTask = BalTask.GetTaskByMilestoneId(clientContext, item.MileStone);
                if (lstTask.Count == 0)
                {
                    string MilestoneUpdate = BalMilestone.UpdateMilestone(clientContext, taskdata, item.MileStone.ToString());
                    if (MilestoneUpdate == "Update")
                    {
                        List<TIM_MilestoneModel> lstMilestone = new List<TIM_MilestoneModel>();
                        lstMilestone = BalMilestone.GetMilestoneByProjectId(clientContext, item.Project);
                        if (lstMilestone.Count == 0)
                        {
                            string ProjectUpdate = BalMilestone.UpdateMilestone(clientContext, taskdata, item.Project.ToString());
                        }
                    }
                }
            }
        }
    }
}