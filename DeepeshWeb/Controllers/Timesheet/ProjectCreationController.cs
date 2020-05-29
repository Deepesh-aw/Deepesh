﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DeepeshWeb.BAL.Timesheet;
using DeepeshWeb.Models.Timesheet;

namespace DeepeshWeb.Controllers.TimeSheet
{
    public class ProjectCreationController : Controller
    {
        ProjectCreationBal BalProjectCreation = new ProjectCreationBal();
        ProjectTypeBal BalProjectType = new ProjectTypeBal();
        ClientBal BalClient = new ClientBal();
        EmpBal BalEmp = new EmpBal();
        WorkFlowBal BalWorkflow = new WorkFlowBal();

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
        public JsonResult SaveProject(ProjectCreationModel Project)
        {
            List<object> obj = new List<object>();
            var spContext = SharePointContextProvider.Current.GetSharePointContext(HttpContext);
            using (var clientContext = spContext.CreateUserClientContextForSPHost())
            {
                List<WorkFlowModel> lstWorkFlow = new List<WorkFlowModel>();
                lstWorkFlow = BalWorkflow.GetWorkFlowForProjectCreation(clientContext);
                string returnID = "0";
                //Project.Members = Request["Members"].Split(',').Select(int.Parse).ToArray();
                string itemdata = " 'ProjectName': '" + Project.ProjectName + "'";
                itemdata += " ,'MembersId': ''results':[1,4]}'";
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
            }

            return Json(obj, JsonRequestBehavior.AllowGet);
        }

    }
}