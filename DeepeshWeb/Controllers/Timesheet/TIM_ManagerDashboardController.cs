﻿using DeepeshWeb.BAL.EmployeeManagement;
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
            List<TIM_EmployeeTimesheetModel> lstEmployeeTimesheetPending = new List<TIM_EmployeeTimesheetModel>();
            List<TIM_EmployeeTimesheetModel> lstEmployeeTimesheetApproved = new List<TIM_EmployeeTimesheetModel>();
            try
            {
                var spContext = SharePointContextProvider.Current.GetSharePointContext(HttpContext);
                using (var clientContext = spContext.CreateUserClientContextForSPHost())
                {
                    lstEmployeeTimesheetPending = BalEmpTimesheet.GetEmpTimesheetByManagerIdAndPending(clientContext, BalEmp.GetEmpByLogIn(clientContext));
                    lstEmployeeTimesheetPending = lstEmployeeTimesheetPending.DistinctBy(x => x.TimesheetID).ToList();
                    lstEmployeeTimesheetApproved = BalEmpTimesheet.GetEmpTimesheetByManagerIdAndApprove(clientContext, BalEmp.GetEmpByLogIn(clientContext));
                    lstEmployeeTimesheetApproved = lstEmployeeTimesheetApproved.DistinctBy(x => x.TimesheetID).ToList();
                    obj.Add("OK");
                    obj.Add(lstEmployeeTimesheetPending);
                    obj.Add(lstEmployeeTimesheetApproved);

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
                        string taskdata = "'StatusId': '" + lstWorkFlowForApproveTimesheet[0].ToStatusID + "'";
                        taskdata += " ,'InternalStatus': '" + lstWorkFlowForApproveTimesheet[0].InternalStatus + "'";
                        int i = 0;
                        foreach (var item in TimesheetData)
                        {
                            var itemdata = "'StatusId': '" + lstWorkFlowForApproveTimesheet[0].ToStatusID + "'";
                            itemdata += " ,'InternalStatus': '" + lstWorkFlowForApproveTimesheet[0].InternalStatus + "'";
                            itemdata += " ,'ApproveDescription': '" + Descrition + "'";

                            string returnID = BalEmpTimesheet.UpdateTimesheet(clientContext, itemdata, item.ID.ToString());
                            if (returnID == "Update")
                            {
                                i++;
                                if (item.Task > 0)
                                {
                                    if(item.AllTaskStatusName == "Completed")
                                    {
                                        taskdata += " ,'TaskStatusId': '" + lstWorkFlowForApproveTimesheet[0].ToStatusID + "'";
                                        string TaskUpdate = BalTask.UpdateTask(clientContext, taskdata, item.Task.ToString());
                                    }
                                    //List<TIM_EmployeeTimesheetModel> lstTimesheetForTask = new List<TIM_EmployeeTimesheetModel>();
                                    //lstTimesheetForTask = BalEmpTimesheet.GetEmpTimesheetByTaskId(clientContext, item.Task);
                                    //if (lstTimesheetForTask.Count > 0)
                                    //{
                                    //    if (lstTimesheetForTask[0].RemainingHours.Split(':')[0] == "0" || lstTimesheetForTask[0].RemainingHours.Split(':')[0] == "00")
                                    //    {
                                    //        List<TIM_SubTaskModel> lstSubTask = new List<TIM_SubTaskModel>();
                                    //        lstSubTask = BalSubTask.GetSubTaskByTaskId(clientContext, item.Task);
                                    //        if(lstSubTask.Count == 0)
                                    //        {
                                    //            taskdata += " ,'TaskStatusId': '" + lstWorkFlowForApproveTimesheet[0].ToStatusID + "'";
                                    //            string TaskUpdate = BalTask.UpdateTask(clientContext, taskdata, item.Task.ToString());
                                    //            if (TaskUpdate == "Update")
                                    //            {
                                    //                UpdateAllStatus(taskdata, item, lstWorkFlowForApproveTimesheet);
                                    //            }
                                    //        }

                                    //    }
                                    //}

                                }
                                else if(item.SubTask > 0)
                                {
                                    if (item.AllTaskStatusName == "Completed")
                                    {
                                        taskdata += " ,'TaskStatusId': '" + lstWorkFlowForApproveTimesheet[0].ToStatusID + "'";
                                        string SubTaskUpdate = BalSubTask.UpdateSubTask(clientContext, taskdata, item.SubTask.ToString());
                                    }
                                    //List<TIM_EmployeeTimesheetModel> lstTimesheetForSubTask = new List<TIM_EmployeeTimesheetModel>();
                                    //lstTimesheetForSubTask = BalEmpTimesheet.GetEmpTimesheetBySubTaskId(clientContext, item.SubTask);
                                    //if (lstTimesheetForSubTask.Count > 0)
                                    //{
                                    //    if (lstTimesheetForSubTask[0].RemainingHours.Split(':')[0] == "0" || lstTimesheetForSubTask[0].RemainingHours.Split(':')[0] == "00")
                                    //    {
                                    //        taskdata += " ,'SubTaskStatusId': '" + lstWorkFlowForApproveTimesheet[0].ToStatusID + "'";
                                    //        string SubTaskUpdate = BalSubTask.UpdateSubTask(clientContext, taskdata, item.SubTask.ToString());
                                    //        if (SubTaskUpdate == "Update")
                                    //        {
                                    //            List<TIM_SubTaskModel> lstSubTask = new List<TIM_SubTaskModel>();
                                    //            lstSubTask = BalSubTask.GetSubTaskBySubTaskId(clientContext, item.SubTask);
                                    //            if(lstSubTask.Count > 0)
                                    //            {
                                    //                List<TIM_SubTaskModel> lstSubTaskCount = new List<TIM_SubTaskModel>();
                                    //                lstSubTaskCount = BalSubTask.GetSubTaskByTaskId(clientContext, lstSubTask[0].Task);
                                    //                if(lstSubTask.Count == 0)
                                    //                {
                                    //                    taskdata += " ,'SubTaskStatus': '" + lstWorkFlowForApproveTimesheet[0].ToStatusID + "'";
                                    //                    string TaskUpdate = BalTask.UpdateTask(clientContext, taskdata, lstSubTaskCount[0].Task.ToString());
                                    //                    if(TaskUpdate == "Update")
                                    //                    {
                                    //                        UpdateAllStatus(taskdata, item, lstWorkFlowForApproveTimesheet);
                                    //                    }
                                    //                }
                                    //            }
                                    //        }
                                    //    }
                                    //}
                                }
                            }
                        }
                        if (i == TimesheetData.Count)
                            obj.Add("OK");
                    }
                    else if (Action == "Reject")
                    {
                        List<TIM_WorkFlowMasterModel> lstWorkFlowForRejectTimesheet = new List<TIM_WorkFlowMasterModel>();
                        lstWorkFlowForRejectTimesheet = BalWorkflow.GetWorkFlowForTimesheetReject(clientContext);
                        string taskdata = "'StatusId': '" + lstWorkFlowForRejectTimesheet[0].ToStatusID + "'";
                        taskdata += " ,'InternalStatus': '" + lstWorkFlowForRejectTimesheet[0].InternalStatus + "'";
                        int i = 0;
                        foreach (var item in TimesheetData)
                        {
                            var itemdata = "'StatusId': '" + lstWorkFlowForRejectTimesheet[0].ToStatusID + "'";
                            itemdata += " ,'InternalStatus': '" + lstWorkFlowForRejectTimesheet[0].InternalStatus + "'";
                            string returnID = BalEmpTimesheet.UpdateTimesheet(clientContext, itemdata, item.ID.ToString());
                            if (returnID == "Update")
                                i++;
                        }
                        if (i == TimesheetData.Count)
                            obj.Add("OK");
                    }
                }
            }
            catch (Exception ex)
            {
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