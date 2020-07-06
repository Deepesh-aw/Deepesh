using DeepeshWeb.BAL.EmployeeManagement;
using DeepeshWeb.BAL.Timesheet;
using DeepeshWeb.Models.Timesheet;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Web;
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
            try
            {
                var spContext = SharePointContextProvider.Current.GetSharePointContext(HttpContext);
                using (var clientContext = spContext.CreateUserClientContextForSPHost())
                {
                    lstTask = BalTask.GetAllTask(clientContext, BalEmp.GetEmpByLogIn(clientContext));
                    lstSubTask = BalSubTask.GetAllSubTask(clientContext, BalEmp.GetEmpByLogIn(clientContext));
                    ViewBag.AllTask = lstTask.Cast<object>().Concat(lstSubTask).ToList();

                    obj.Add("OK");
                    obj.Add(lstTask);
                    obj.Add(lstSubTask);
                    obj.Add(ViewBag.AllTask);

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
        [ActionName("GetHour")]
        public JsonResult GetHour(int AllTaskId)
        {
            List<object> obj = new List<object>();
            List<TIM_EmployeeTimesheetModel> lstEmployeeTimesheet = new List<TIM_EmployeeTimesheetModel>();
            List<TIM_WorkingHoursModel> lstWorkingHoursModel = new List<TIM_WorkingHoursModel>();
            try
            {
                var spContext = SharePointContextProvider.Current.GetSharePointContext(HttpContext);
                using (var clientContext = spContext.CreateUserClientContextForSPHost())
                {
                    lstWorkingHoursModel = BalWorkinghours.GetWorkingHour(clientContext);
                    lstEmployeeTimesheet = BalEmpTimesheet.GetEmpTimesheetByAllTaskId(clientContext, AllTaskId);
                    obj.Add("OK");
                    obj.Add(lstWorkingHoursModel);
                    obj.Add(lstEmployeeTimesheet);
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