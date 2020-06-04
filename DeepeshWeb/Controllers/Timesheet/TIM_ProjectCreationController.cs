using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DeepeshWeb.BAL.EmployeeManagement;
using DeepeshWeb.BAL.Timesheet;
using DeepeshWeb.Models.Timesheet;

namespace DeepeshWeb.Controllers.TimeSheet
{
    public class TIM_ProjectCreationController : Controller
    {
        TIM_ProjectCreationBal BalProjectCreation = new TIM_ProjectCreationBal();
        TIM_ProjectTypeMasterBal BalProjectType = new TIM_ProjectTypeMasterBal();
        Emp_ClientMasterDetailsBal BalClient = new Emp_ClientMasterDetailsBal();
        Emp_BasicInfoBal BalEmp = new Emp_BasicInfoBal();
        TIM_WorkFlowMasterBal BalWorkflow = new TIM_WorkFlowMasterBal();

        // GET: ProjectCreation
        public ActionResult Index()
        {
            var spContext = SharePointContextProvider.Current.GetSharePointContext(HttpContext);
            using (var clientContext = spContext.CreateUserClientContextForSPHost())
            {
                ViewBag.ProjectTypeData = BalProjectType.GetProjectType(clientContext);
                ViewBag.ClientData = BalClient.GetClient(clientContext);
                ViewBag.EmpData = BalEmp.GetEmp(clientContext);
            }
            return View();
        }

        [SharePointContextFilter]
        [ActionName("SaveProject")]
        public JsonResult SaveProject(TIM_ProjectCreationModel Project)
        {
            List<object> obj = new List<object>();
            var spContext = SharePointContextProvider.Current.GetSharePointContext(HttpContext);
            using (var clientContext = spContext.CreateUserClientContextForSPHost())
            {
                List<TIM_WorkFlowMasterModel> lstWorkFlow = new List<TIM_WorkFlowMasterModel>();
                lstWorkFlow = BalWorkflow.GetWorkFlowForProjectCreation(clientContext);
                string returnID = "0";
                string arr = String.Join(",", Project.Members);
                //Project.Members = Request["Members"].Split(',').Select(int.Parse).ToArray();
                //itemdata += " ,'MembersId': {'results': [1,3] }";
                string itemdata = " 'ProjectName': '" + Project.ProjectName + "'";
                itemdata += " ,'MembersId': {'results': [" + arr + "] }";
                itemdata += " ,'MembersId': {'results': "+Project.Members+" }";
                itemdata += " ,'ClientProjectManager': '" + Project.ClientProjectManager + "'";
                itemdata += " ,'StartDate': '" + Project.StartDate + "'";
                itemdata += " ,'EndDate': '" + Project.EndDate + "'";
                itemdata += " ,'Description': '" + Project.Description + "'";
                itemdata += " ,'ProjectTypeId': '" + Project.ProjectType + "'";
                itemdata += " ,'MembersText': '" + Project.MembersText + "'";
                itemdata += " ,'ClientNameId': '" + Project.ClientName + "'";
                itemdata += " ,'ProjectManagerId': '" + Project.ProjectManager + "'";
                itemdata += " ,'NoOfDays': '" + Project.NoOfDays + "'";

                if (lstWorkFlow.Count > 0)
                {
                    itemdata += " ,'StatusId': '" + lstWorkFlow[0].ToStatusID + "'";
                    itemdata += " ,'InternalStatus': '" + lstWorkFlow[0].InternalStatus + "'";
                }
                returnID = BalProjectCreation.SaveProjectCreation(clientContext, itemdata);
                if (Convert.ToInt32(returnID) > 0)
                    obj.Add("OK");
            }

            return Json(obj, JsonRequestBehavior.AllowGet);
        }

    }
}