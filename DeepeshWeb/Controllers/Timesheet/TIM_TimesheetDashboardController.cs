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

        // GET: TIM_TimesheetDashboard
        public ActionResult Index()
        {
            dynamic mymodel = new ExpandoObject();
            List<TIM_TaskModel> lstTask = new List<TIM_TaskModel>();
            List<TIM_SubTaskModel> lstSubTask = new List<TIM_SubTaskModel>();
            try
            {
                var spContext = SharePointContextProvider.Current.GetSharePointContext(HttpContext);
                using (var clientContext = spContext.CreateUserClientContextForSPHost())
                {
                    //lstProjectCreation = BalProjectCreation.GetProjectCreationAllItems(clientContext, BalEmp.GetEmpByLogIn(clientContext), BalEmp.GetEmpCodeByLogIn(clientContext));
                    lstTask = BalTask.GetAllTask(clientContext, BalEmp.GetEmpByLogIn(clientContext));
                    lstSubTask = BalSubTask.GetAllSubTask(clientContext, BalEmp.GetEmpByLogIn(clientContext));
                    ViewBag.AllTask = lstTask.Cast<object>().Concat(lstSubTask).ToList();

                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("An error occured while performing action. GUID: {0}", ex.ToString()));
            }
            //lstProjectCreation.Distinct(new EqualityComparer())
            return View(mymodel);
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
                lstProject = BalProjectCreation.GetProjectCreationByClientId(clientContext, Client, BalEmp.GetEmpByLogIn(clientContext), BalEmp.GetEmpCodeByLogIn(clientContext));
                if (lstProject.Count > 0)
                {
                    obj.Add("OK");
                    obj.Add(lstProject);
                }
            }
            return Json(obj, JsonRequestBehavior.AllowGet);
        }


    }
}