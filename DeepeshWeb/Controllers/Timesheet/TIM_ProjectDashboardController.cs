using DeepeshWeb.BAL.Timesheet;
using DeepeshWeb.Models.Timesheet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DeepeshWeb.BAL.EmployeeManagement;

namespace DeepeshWeb.Controllers.TimeSheet
{
    public class TIM_ProjectDashboardController : Controller
    {
        // GET: TIM_ProjectDashboard
        TIM_ProjectCreationBal BalProjectCreation = new TIM_ProjectCreationBal();
        TIM_MilestoneBal BalMilestone = new TIM_MilestoneBal();
        TIM_TaskBal BalTask = new TIM_TaskBal();
        TIM_SubTaskBal BalSubTask = new TIM_SubTaskBal();
        TIM_WorkFlowMasterBal BalWorkflow = new TIM_WorkFlowMasterBal();
        TIM_ProjectTypeMasterBal BalProjectType = new TIM_ProjectTypeMasterBal();
        Emp_ClientMasterDetailsBal BalClient = new Emp_ClientMasterDetailsBal();
        Emp_BasicInfoBal BalEmp = new Emp_BasicInfoBal();

        public ActionResult Index()
        {
            try
            {
                var spContext = SharePointContextProvider.Current.GetSharePointContext(HttpContext);
                using (var clientContext = spContext.CreateUserClientContextForSPHost())
                {
                    ViewBag.ProjectTypeData = BalProjectType.GetProjectType(clientContext);
                    ViewBag.ClientData = BalClient.GetClient(clientContext);
                    ViewBag.EmpData = BalEmp.GetEmp(clientContext);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("An error occured while performing action. GUID: {0}", ex.ToString()));
            }
            return View();
        }

        [HttpPost]
        [ActionName("GetProjectData")]
        public JsonResult GetProjectData()
        {
            List<TIM_ProjectCreationModel> lstProjectCreation = new List<TIM_ProjectCreationModel>();
            List<object> obj = new List<object>();
            var spContext = SharePointContextProvider.Current.GetSharePointContext(HttpContext);
            using (var clientContext = spContext.CreateUserClientContextForSPHost())
            {
                lstProjectCreation = BalProjectCreation.GetProjectCreationAllItems(clientContext);
                if (lstProjectCreation.Count > 0)
                {
                    obj.Add("OK");
                    obj.Add(lstProjectCreation);
                }
            }
            return Json(obj, JsonRequestBehavior.AllowGet);
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

        [HttpPost]
        [ActionName("GetSubTask")]
        public JsonResult GetSubTask(int TaskId)
        {
            List<object> obj = new List<object>();
            List<TIM_SubTaskModel> lstSubTask = new List<TIM_SubTaskModel>();
            var spContext = SharePointContextProvider.Current.GetSharePointContext(HttpContext);
            using (var clientContext = spContext.CreateUserClientContextForSPHost())
            {
                lstSubTask = BalSubTask.GetSubTaskByTaskId(clientContext, TaskId);
                if (lstSubTask.Count > 0)
                {
                    obj.Add("OK");
                    obj.Add(lstSubTask);
                }
            }
            return Json(obj, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ActionName("DeleteProject")]
        public JsonResult DeleteProject()
        {
            List<object> obj = new List<object>();
            var spContext = SharePointContextProvider.Current.GetSharePointContext(HttpContext);
            using (var clientContext = spContext.CreateUserClientContextForSPHost())
            {
                if (Request.Cookies["ProjectId"] != null)
                {
                    int ProjectId = Convert.ToInt32(Request.Cookies["ProjectId"].Value);
                    string returnID = "0";
                    List<TIM_WorkFlowMasterModel> lstWorkFlow = new List<TIM_WorkFlowMasterModel>();
                    lstWorkFlow = BalWorkflow.GetWorkFlowForProjectDeletion(clientContext);
                    if (lstWorkFlow.Count > 0)
                    {
                        string itemdata = "'StatusId': '" + lstWorkFlow[0].ToStatusID + "'";
                        itemdata += " ,'InternalStatus': '" + lstWorkFlow[0].InternalStatus + "'";
                        returnID = BalProjectCreation.UpdateProject(clientContext, itemdata, ProjectId.ToString());
                    }
                    if (returnID == "Update")
                    {
                        obj.Add("OK");
                    }
                }
            }
            return Json(obj, JsonRequestBehavior.AllowGet);
        }

    }
}