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

        [SharePointContextFilter]
        [ActionName("GetEditProject")]
        public JsonResult GetEditProject(TIM_ProjectCreationModel Project)
        {
            List<object> obj = new List<object>();
            List<TIM_ProjectCreationModel> lstProjectCreation = new List<TIM_ProjectCreationModel>();
            try
            {
                var spContext = SharePointContextProvider.Current.GetSharePointContext(HttpContext);
                using (var clientContext = spContext.CreateUserClientContextForSPHost())
                {
                    if (Request.Cookies["ProjectId"] != null)
                    {
                        int ProjectId = Convert.ToInt32(Request.Cookies["ProjectId"].Value);
                        lstProjectCreation = BalProjectCreation.GetProjectCreationById(clientContext, ProjectId);
                        if (lstProjectCreation.Count > 0)
                        {
                            obj.Add("OK");
                            obj.Add(lstProjectCreation);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("An error occured while performing action. GUID: {0}", ex.ToString()));
            }
            return Json(obj, JsonRequestBehavior.AllowGet);
        }

        [SharePointContextFilter]
        [ActionName("SaveProject")]
        public JsonResult SaveProject(TIM_ProjectCreationModel Project)
        {
            List<object> obj = new List<object>();
            var spContext = SharePointContextProvider.Current.GetSharePointContext(HttpContext);
            using (var clientContext = spContext.CreateUserClientContextForSPHost())
            {
                string returnID = "0";
                string arr = String.Join(",", Project.Members);
                //Project.Members = Request["Members"].Split(',').Select(int.Parse).ToArray();
                //itemdata += " ,'MembersId': {'results': [1,3] }";
                string itemdata = " 'ProjectName': '" + Project.ProjectName + "'";
                itemdata += " ,'MembersId': {'results': [" + arr + "] }";
                itemdata += " ,'ClientProjectManager': '" + Project.ClientProjectManager + "'";
                itemdata += " ,'StartDate': '" + Project.StartDate + "'";
                itemdata += " ,'EndDate': '" + Project.EndDate + "'";
                itemdata += " ,'Description': '" + Project.Description + "'";
                itemdata += " ,'ProjectTypeId': '" + Project.ProjectType + "'";
                itemdata += " ,'MembersText': '" + Project.MembersText + "'";
                itemdata += " ,'MembersCodeText': '" + Project.MembersCodeText + "'";
                itemdata += " ,'ClientNameId': '" + Project.ClientName + "'";
                itemdata += " ,'ProjectManagerId': '" + Project.ProjectManager + "'";
                itemdata += " ,'NoOfDays': '" + Project.NoOfDays + "'";

                if(Project.ID == 0)
                {
                    List<TIM_WorkFlowMasterModel> lstWorkFlow = new List<TIM_WorkFlowMasterModel>();
                    lstWorkFlow = BalWorkflow.GetWorkFlowForProjectCreation(clientContext);
                    if (lstWorkFlow.Count > 0)
                    {
                        itemdata += " ,'StatusId': '" + lstWorkFlow[0].ToStatusID + "'";
                        itemdata += " ,'InternalStatus': '" + lstWorkFlow[0].InternalStatus + "'";
                    }
                    returnID = BalProjectCreation.SaveProjectCreation(clientContext, itemdata);
                    if (Convert.ToInt32(returnID) > 0)
                        obj.Add("OK");
                }
                else
                {
                    returnID = BalProjectCreation.UpdateProject(clientContext, itemdata, Project.ID.ToString());
                    if (returnID == "Update")
                        obj.Add("OK");
                }

            }
            return Json(obj, JsonRequestBehavior.AllowGet);
        }

    }
}