using DeepeshWeb.BAL;
using DeepeshWeb.BAL.EmployeeManagement;
using DeepeshWeb.BAL.Timesheet;
using DeepeshWeb.Models;
using DeepeshWeb.Models.Timesheet;
using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace DeepeshWeb.Controllers.TimeSheet
{
    public class TIM_TimesheetDashboardController : Controller
    {
        TIM_ProjectCreationBal BalProjectCreation = new TIM_ProjectCreationBal();
        TIM_MilestoneBal BalMilestone = new TIM_MilestoneBal();
        TIM_TaskBal BalTask = new TIM_TaskBal();
        TIM_SubTaskBal BalSubTask = new TIM_SubTaskBal();
        TIM_WorkFlowMasterBal BalWorkflow = new TIM_WorkFlowMasterBal();
        TIM_ProjectTypeMasterBal BalProjectType = new TIM_ProjectTypeMasterBal();
        Emp_ClientMasterDetailsBal BalClient = new Emp_ClientMasterDetailsBal();
        Emp_BasicInfoBal BalEmp = new Emp_BasicInfoBal();
        TIM_StatusMasterBal BalStatus = new TIM_StatusMasterBal();
        TIM_WorkingHoursBal BalWorkinghours = new TIM_WorkingHoursBal();
        TIM_EmployeeTimesheetBal BalEmpTimesheet = new TIM_EmployeeTimesheetBal();
        GEN_ApproverMasterBal BalApprover = new GEN_ApproverMasterBal();

        // GET: TIM_TimesheetDashboard
        public ActionResult Index()
        {
            //List<TIM_TaskModel> lstTask = new List<TIM_TaskModel>();
            //List<TIM_SubTaskModel> lstSubTask = new List<TIM_SubTaskModel>();
            //try
            //{
            //    var spContext = SharePointContextProvider.Current.GetSharePointContext(HttpContext);
            //    using (var clientContext = spContext.CreateUserClientContextForSPHost())
            //    {
            //        //lstProjectCreation = BalProjectCreation.GetProjectCreationAllItems(clientContext, BalEmp.GetEmpByLogIn(clientContext), BalEmp.GetEmpCodeByLogIn(clientContext));
            //        lstTask = BalTask.GetAllTask(clientContext, BalEmp.GetEmpByLogIn(clientContext));
            //        lstSubTask = BalSubTask.GetAllSubTask(clientContext, BalEmp.GetEmpByLogIn(clientContext));
            //        ViewBag.AllTask = lstTask.Cast<object>().Concat(lstSubTask).ToList();

            //    }
            //}
            //catch (Exception ex)
            //{
            //    throw new Exception(string.Format("An error occured while performing action. GUID: {0}", ex.ToString()));
            //}
            //lstProjectCreation.Distinct(new EqualityComparer())
            return View();
        }

        [HttpPost]
        [ActionName("OnLoadData")]
        public JsonResult OnLoadData()
        {
            List<object> obj = new List<object>();
            List<TIM_TaskModel> lstTask = new List<TIM_TaskModel>();
            List<TIM_SubTaskModel> lstSubTask = new List<TIM_SubTaskModel>();
            List<TIM_WorkingHoursModel> lstWorkingHours = new List<TIM_WorkingHoursModel>();
            List<TIM_EmployeeTimesheetModel> lstEmployeeTimesheet = new List<TIM_EmployeeTimesheetModel>();
            try
            {
                var spContext = SharePointContextProvider.Current.GetSharePointContext(HttpContext);
                using (var clientContext = spContext.CreateUserClientContextForSPHost())
                {
                    lstTask = BalTask.GetAllTask(clientContext, BalEmp.GetEmpByLogIn(clientContext));
                    lstSubTask = BalSubTask.GetAllSubTask(clientContext, BalEmp.GetEmpByLogIn(clientContext));
                    ViewBag.AllTask = lstTask.Cast<object>().Concat(lstSubTask).ToList();
                    lstWorkingHours = BalWorkinghours.GetWorkingHour(clientContext);
                    lstEmployeeTimesheet = BalEmpTimesheet.GetEmpTimesheetByEmpId(clientContext, BalEmp.GetEmpByLogIn(clientContext));
                    lstEmployeeTimesheet = lstEmployeeTimesheet.DistinctBy(x => x.TimesheetID).ToList();
                    obj.Add("OK");
                    obj.Add(ViewBag.AllTask);
                    obj.Add(lstWorkingHours);
                    obj.Add(lstEmployeeTimesheet);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("An error occured while performing action. GUID: {0}", ex.ToString()));
            }
            return Json(obj, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ActionName("GetProjectDataByClient")]
        public JsonResult GetProjectDataByClient(int Client)
        {
            List<object> obj = new List<object>();
            List<TIM_ProjectCreationModel> lstProject = new List<TIM_ProjectCreationModel>();
            var spContext = SharePointContextProvider.Current.GetSharePointContext(HttpContext);
            using (var clientContext = spContext.CreateUserClientContextForSPHost())
            {
                //lstProject = bal.GetProjectCreationByClientId(clientContext, Client, BalEmp.GetEmpByLogIn(clientContext), BalEmp.GetEmpCodeByLogIn(clientContext));
                if (lstProject.Count > 0)
                {
                    obj.Add("OK");
                    obj.Add(lstProject);
                }
            }
            return Json(obj, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ActionName("GetPrevTimesheet")]
        public JsonResult GetPrevTimesheet(int AllTaskId)
        {
            List<object> obj = new List<object>();
            List<TIM_EmployeeTimesheetModel> lstEmployeeTimesheet = new List<TIM_EmployeeTimesheetModel>();
            try
            {
                var spContext = SharePointContextProvider.Current.GetSharePointContext(HttpContext);
                using (var clientContext = spContext.CreateUserClientContextForSPHost())
                {
                    lstEmployeeTimesheet = BalEmpTimesheet.GetEmpTimesheetByAllTaskId(clientContext, AllTaskId);
                    obj.Add("OK");
                    obj.Add(lstEmployeeTimesheet);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("An error occured while performing action. GUID: {0}", ex.ToString()));
            }
            return Json(obj, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ActionName("AddTimesheet")]
        public JsonResult AddTimesheet(List<TIM_EmployeeTimesheetModel> EmpTimesheet)
        {
            List<object> obj = new List<object>();
            int i = 0;
            string InternalStatus = "Pending";
            var spContext = SharePointContextProvider.Current.GetSharePointContext(HttpContext);
            using (var clientContext = spContext.CreateUserClientContextForSPHost())
            {
                List<TIM_StatusMasterModel> lstPendingStatus = new List<TIM_StatusMasterModel>();
                lstPendingStatus = BalStatus.GetPendingStatus(clientContext);
                List<GEN_ApproverRoleNameModel> lstApprover = BalApprover.getApproverData(clientContext, BalEmp.GetEmpCodeByLogIn(clientContext), "Timesheet", "Main");

                string returnID = "0";
                foreach (var item in EmpTimesheet)
                {
                    string itemdata = " 'MileStoneId': '" + item.MileStone + "'";

                    itemdata += " ,'Description': '" + item.Description + "'";
                    itemdata += " ,'Hours': '" + item.Hours + "'";
                    itemdata += " ,'EstimatedHours': '" + item.EstimatedHours + "'";
                    itemdata += " ,'UtilizedHours': '" + item.UtilizedHours + "'";
                    itemdata += " ,'RemainingHours': '" + item.RemainingHours + "'";
                    itemdata += " ,'TimesheetAddedDate': '" + item.TimesheetAddedDate + "'";
                    itemdata += " ,'EmployeeId': '" + BalEmp.GetEmpByLogIn(clientContext) + "'";
                    itemdata += " ,'ManagerId': '" + lstApprover[0].ID + "'";

                    itemdata += " ,'ProjectId': '" + item.Project + "'";
                    itemdata += " ,'TaskId': '" + item.Task + "'";
                    itemdata += " ,'SubTaskId': '" + item.SubTask + "'";
                    itemdata += " ,'ClientId': '" + item.Client + "'";

                    itemdata += " ,'TimesheetID': '" + item.TimesheetID + "'";
                    itemdata += " ,'StatusId': '" + lstPendingStatus[0].ID + "'";
                    itemdata += " ,'InternalStatus': '"+ InternalStatus + "'";
                    
                    returnID = BalEmpTimesheet.SaveTimesheet(clientContext, itemdata);
                        if (Convert.ToInt32(returnID) > 0)
                            i++;

                }
                
                if (i == EmpTimesheet.Count)
                    obj.Add("OK");
            }
            return Json(obj, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ActionName("GetEditTimesheet")]
        public JsonResult GetEditTimesheet(string TimesheetId)
        {
            List<object> obj = new List<object>();
            List<TIM_EmployeeTimesheetModel> lstEmployeeTimesheet = new List<TIM_EmployeeTimesheetModel>();
            try
            {
                var spContext = SharePointContextProvider.Current.GetSharePointContext(HttpContext);
                using (var clientContext = spContext.CreateUserClientContextForSPHost())
                {
                    lstEmployeeTimesheet = BalEmpTimesheet.GetEmpTimesheetByTimesheetId(clientContext, TimesheetId);
                    obj.Add("OK");
                    obj.Add(lstEmployeeTimesheet);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("An error occured while performing action. GUID: {0}", ex.ToString()));
            }
            return Json(obj, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ActionName("ClearTimesheet")]
        public JsonResult ClearTimesheet( List<TIM_EmployeeTimesheetModel> ClearTimesheet)
        {
            List<object> obj = new List<object>();
            int i = 0;
            string returnID = "0";
            var spContext = SharePointContextProvider.Current.GetSharePointContext(HttpContext);
            using (var clientContext = spContext.CreateUserClientContextForSPHost())
            {
                foreach (var item in ClearTimesheet)
                {
                    returnID = BalEmpTimesheet.DeleteTimesheet(clientContext,item.ID.ToString());
                    if(returnID == "Delete")
                        i++;
                }
                if (i == ClearTimesheet.Count)
                    obj.Add("OK");
            }
            return Json(obj, JsonRequestBehavior.AllowGet);
        }
    }
}