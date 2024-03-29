﻿using DeepeshWeb.BAL.Timesheet;
using DeepeshWeb.Models.Timesheet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DeepeshWeb.BAL.EmployeeManagement;
using Newtonsoft.Json;
using DeepeshWeb.Models.EmployeeManagement;

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
        TIM_StatusMasterBal BalStatus = new TIM_StatusMasterBal();
        TIM_DocumentLibraryBal BalDocument = new TIM_DocumentLibraryBal();
        TIM_SendEmailController EmailCtrl = new TIM_SendEmailController();
        TIM_EmployeeTimesheetBal BalEmpTimesheet = new TIM_EmployeeTimesheetBal();


        public ActionResult Index()
        {
            try
            {
                var spContext = SharePointContextProvider.Current.GetSharePointContext(HttpContext);
                using (var clientContext = spContext.CreateUserClientContextForSPHost())
                {
                    ViewBag.ProjectTypeData = BalProjectType.GetProjectType(clientContext);
                    ViewBag.ClientData = BalClient.GetClient(clientContext);
                    ViewBag.ProjectEmpData = BalEmp.GetEmp(clientContext);
                    ViewBag.StatusData = BalStatus.GetStatusForAction(clientContext);
                    //ViewBag.StatusData = BalStatus.GetStatusForAction(clientContext);
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
            List<object> obj = new List<object>();
            try
            {
                List<TIM_ProjectCreationModel> lstProjectCreation = new List<TIM_ProjectCreationModel>();
                List<TIM_TaskModel> lstTask = new List<TIM_TaskModel>();
                var spContext = SharePointContextProvider.Current.GetSharePointContext(HttpContext);
                using (var clientContext = spContext.CreateUserClientContextForSPHost())
                {
                    lstProjectCreation = BalProjectCreation.GetProjectCreationAllItems(clientContext, BalEmp.GetEmpByLogIn(clientContext), BalEmp.GetEmpCodeByLogIn(clientContext));
                    lstTask = BalTask.GetAllTask(clientContext, BalEmp.GetEmpByLogIn(clientContext));

                    if (lstProjectCreation.Count > 0)
                    {
                        obj.Add("OK");
                        obj.Add(lstProjectCreation);
                        obj.Add(lstTask);
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
        [ActionName("GetEditProject")]
        public JsonResult GetEditProject(TIM_ProjectCreationModel Project)
        {
            List<object> obj = new List<object>();
            List<TIM_ProjectCreationModel> lstProjectCreation = new List<TIM_ProjectCreationModel>();
            List<TIM_DocumentLibraryModel> lstDocument = new List<TIM_DocumentLibraryModel>();

            try
            {
                var spContext = SharePointContextProvider.Current.GetSharePointContext(HttpContext);
                using (var clientContext = spContext.CreateUserClientContextForSPHost())
                {
                    if (Request.Cookies["ProjectId"] != null)
                    {
                        int ProjectId = Convert.ToInt32(Request.Cookies["ProjectId"].Value);
                        lstProjectCreation = BalProjectCreation.GetProjectCreationById(clientContext, ProjectId);
                        var path = spContext.SPHostUrl.Scheme + "://" + spContext.SPHostUrl.Host.ToString();
                        lstDocument = BalDocument.GetDocumentByLookUpId(clientContext, ProjectId.ToString(), path);
                        if (lstProjectCreation.Count > 0)
                        {
                            obj.Add("OK");
                            obj.Add(lstProjectCreation);
                            obj.Add(lstDocument);
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

        public List<Emp_BasicInfoModel> GetEmpMembers(int[] Members, int ProjectManager)
        {
            List<Emp_BasicInfoModel> lstEmp = new List<Emp_BasicInfoModel>();
            try
            {
                var spContext = SharePointContextProvider.Current.GetSharePointContext(HttpContext);
                using (var clientContext = spContext.CreateUserClientContextForSPHost())
                {
                    var filter = "(ID eq " + ProjectManager + " ) or ";
                    for (int i = 0; i < Members.Length; i++)
                    {
                        if (i == Members.Length - 1)
                            filter += "(ID eq " + Members[i] + ")";
                        else
                            filter += "(ID eq " + Members[i] + ") or ";
                    }
                    lstEmp = BalEmp.GetEmpCodeByLogIn(clientContext, filter);
                }
                
            }
            catch (Exception ex)
            {

            }
            return lstEmp;
        }

        [HttpPost]
        [ActionName("GetProjectDoc")]
        public JsonResult GetProjectDoc(int ProjectId, int[] Members, int ProjectManager)
        {
            List<object> obj = new List<object>();
            List<TIM_DocumentLibraryModel> lstDocument = new List<TIM_DocumentLibraryModel>();
            try
            {
                var spContext = SharePointContextProvider.Current.GetSharePointContext(HttpContext);
                using (var clientContext = spContext.CreateUserClientContextForSPHost())
                {
                    var path = spContext.SPHostUrl.Scheme + "://" + spContext.SPHostUrl.Host.ToString();
                    lstDocument = BalDocument.GetDocumentByLookUpId(clientContext, ProjectId.ToString(), path);
                    
                    ViewBag.EmpData = GetEmpMembers(Members, ProjectManager);
                    obj.Add("OK");
                    obj.Add(lstDocument);
                    obj.Add(ViewBag.EmpData);
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

        [SharePointContextFilter]
        [ActionName("SaveProject")]
        public JsonResult SaveProject(FormCollection formCollection)
        {
            List<object> obj = new List<object>();
            var spContext = SharePointContextProvider.Current.GetSharePointContext(HttpContext);
            var name = formCollection["ProjectDetails"];
            List<TIM_ProjectCreationModel> Project = JsonConvert.DeserializeObject<List<TIM_ProjectCreationModel>>(name);
            var doc = formCollection["PrevDocument"];
            List<TIM_DocumentLibraryModel> DeleteDocument = JsonConvert.DeserializeObject<List<TIM_DocumentLibraryModel>>(doc);

            try
            {
                using (var clientContext = spContext.CreateUserClientContextForSPHost())
                {

                    string returnID = "0";
                    string arr = String.Join(",", Project[0].Members);
                    //Project.Members = Request["Members"].Split(',').Select(int.Parse).ToArray();
                    //itemdata += " ,'MembersId': {'results': [1,3] }";
                    string itemdata = " 'ProjectName': '" + Project[0].ProjectName.Replace("'", @"\'") + "'";
                    itemdata += " ,'MembersId': {'results': [" + arr + "] }";
                    itemdata += " ,'ClientProjectManager': '" + Project[0].ClientProjectManager.Replace("'", @"\'") + "'";
                    itemdata += " ,'StartDate': '" + Project[0].StartDate + "'";
                    itemdata += " ,'EndDate': '" + Project[0].EndDate + "'";
                    itemdata += " ,'Description': '" + Project[0].Description.Replace("'", @"\'") + "'";
                    itemdata += " ,'ProjectTypeId': '" + Project[0].ProjectType + "'";
                    itemdata += " ,'MembersText': '" + Project[0].MembersText + "'";
                    itemdata += " ,'MembersCodeText': '" + Project[0].MembersCodeText + "'";
                    itemdata += " ,'ClientNameId': '" + Project[0].ClientName + "'";
                    itemdata += " ,'ProjectManagerId': '" + Project[0].ProjectManager + "'";
                    itemdata += " ,'NoOfDays': '" + Project[0].NoOfDays + "'";

                    if (Project[0].ID == 0)
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
                        {
                            if (Request.Files.Count > 0)
                            {
                                HttpFileCollectionBase files = Request.Files;
                                string Type = "ProjectCreation";
                                for (int i = 0; i < files.Count; i++)
                                {
                                    var postedFile = files[i];
                                    string item = "'LID' : '" + returnID + "'";
                                    item += ",'DocumentType' : '" + Type + "'";
                                    item += ",'DocumentPath' : '" + files[i].FileName + "'";
                                    int res = BalProjectCreation.UploadDocument(clientContext, postedFile, item);
                                    if (res > 0)
                                    {
                                        string Mailres = EmailCtrl.ProjectCreationNotification(clientContext, Project[0]);
                                        if (Convert.ToInt32(Mailres) > 0)
                                            obj.Add("OK");
                                    }

                                }

                            }
                            else
                            {
                                string Mailres = EmailCtrl.ProjectCreationNotification(clientContext, Project[0]);
                                if (Convert.ToInt32(Mailres) > 0)
                                    obj.Add("OK");
                            }
                        }
                        else
                            obj.Add("ERROR");
                    }
                    else
                    {
                        if (DeleteDocument.Count > 0)
                        {
                            int z = 0;
                            foreach (var item in DeleteDocument)
                            {
                                string Result = BalDocument.DeleteDocument(clientContext, item.ID.ToString());
                                if (Result == "Delete")
                                    z++;
                            }
                            if (z != DeleteDocument.Count)
                            {
                                obj.Add("ERROR");
                                return Json(obj, JsonRequestBehavior.AllowGet);
                            }
                        }

                        if (Request.Files.Count > 0)
                        {
                            int y = 0;
                            HttpFileCollectionBase files = Request.Files;
                            string Type = "ProjectCreation";
                            for (int i = 0; i < files.Count; i++)
                            {
                                var postedFile = files[i];
                                string item = "'LID' : '" + Project[0].ID + "'";
                                item += ",'DocumentType' : '" + Type + "'";
                                item += ",'DocumentPath' : '" + files[i].FileName + "'";
                                int res = BalProjectCreation.UploadDocument(clientContext, postedFile, item);
                                if (res > 0)
                                    y++;
                            }
                            if (y != files.Count)
                            {
                                obj.Add("ERROR");
                                return Json(obj, JsonRequestBehavior.AllowGet);
                            }

                        }

                        returnID = BalProjectCreation.UpdateProject(clientContext, itemdata, Project[0].ID.ToString());
                        if (returnID == "Update")
                        {
                            if (Project[0].InternalStatus == "ProjectDeleted")
                            {
                                List<TIM_WorkFlowMasterModel> lstWorkFlow = new List<TIM_WorkFlowMasterModel>();
                                lstWorkFlow = BalWorkflow.GetWorkFlowForProjectDeletion(clientContext);
                                if (lstWorkFlow.Count > 0)
                                {
                                    string projectdata = "'StatusId': '" + lstWorkFlow[0].ToStatusID + "'";
                                    projectdata += " ,'InternalStatus': '" + lstWorkFlow[0].InternalStatus + "'";
                                    returnID = BalProjectCreation.UpdateProject(clientContext, projectdata, Project[0].ID.ToString());
                                    if (returnID == "Update")
                                    {
                                        List<TIM_MilestoneModel> lstMilestone = new List<TIM_MilestoneModel>();
                                        lstMilestone = BalMilestone.GetMilestoneByProjectId(clientContext, Project[0].ID);
                                        if (lstMilestone.Count > 0)
                                        {
                                            int mile = 0;
                                            foreach (var item in lstMilestone)
                                            {
                                                mile++;
                                                BalMilestone.UpdateMilestone(clientContext, projectdata, item.ID.ToString());
                                            }
                                            if (mile == lstMilestone.Count)
                                            {
                                                List<TIM_TaskModel> lstTask = new List<TIM_TaskModel>();
                                                lstTask = BalTask.GetTaskByProjectId(clientContext, Project[0].ID);
                                                if (lstTask.Count > 0)
                                                {
                                                    int task = 0;
                                                    foreach (var item in lstTask)
                                                    {
                                                        task++;
                                                        BalTask.UpdateTask(clientContext, projectdata, item.ID.ToString());
                                                    }
                                                    if (task == lstTask.Count)
                                                    {
                                                        List<TIM_SubTaskModel> lstSubTask = new List<TIM_SubTaskModel>();
                                                        lstSubTask = BalSubTask.GetSubTaskByProjectId(clientContext, Project[0].ID);
                                                        if (lstSubTask.Count > 0)
                                                        {
                                                            int subtask = 0;
                                                            foreach (var item in lstSubTask)
                                                            {
                                                                subtask++;
                                                                BalSubTask.UpdateSubTask(clientContext, projectdata, item.ID.ToString());
                                                            }
                                                            if (subtask == lstSubTask.Count)
                                                            {
                                                                obj.Add("OK");
                                                            }
                                                        }
                                                        else
                                                        {
                                                            obj.Add("OK");
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    obj.Add("OK");
                                                }
                                            }
                                        }
                                        else
                                        {
                                            obj.Add("OK");
                                        }
                                    }
                                }
                            }
                            else
                            {
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

        [HttpPost]
        [ActionName("GetMilestone")]
        public JsonResult GetMilestone(int ProjectId)
        {
            List<object> obj = new List<object>();
            List<TIM_MilestoneModel> lstMilestone = new List<TIM_MilestoneModel>();
            List<TIM_DocumentLibraryModel> lstDocument = new List<TIM_DocumentLibraryModel>();
            var spContext = SharePointContextProvider.Current.GetSharePointContext(HttpContext);
            using (var clientContext = spContext.CreateUserClientContextForSPHost())
            {
                lstMilestone = BalMilestone.GetMilestoneByProjectId(clientContext, ProjectId);
                var path = spContext.SPHostUrl.Scheme + "://" + spContext.SPHostUrl.Host.ToString();
                lstDocument = BalDocument.GetDocumentByLookUpId(clientContext, ProjectId.ToString(), path);
                if (lstMilestone.Count > 0)
                {
                    obj.Add("OK");
                    obj.Add(lstMilestone);
                    obj.Add(lstDocument);
                }
            }
            return Json(obj, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ActionName("GetTask")]
        public JsonResult GetTask(int MilestoneId, int[] Members, int ProjectManager)
        {
            List<object> obj = new List<object>();
            List<TIM_TaskModel> lstTask = new List<TIM_TaskModel>();
            List<TIM_DocumentLibraryModel> lstDocument = new List<TIM_DocumentLibraryModel>();
            var spContext = SharePointContextProvider.Current.GetSharePointContext(HttpContext);
            using (var clientContext = spContext.CreateUserClientContextForSPHost())
            {
                lstTask = BalTask.GetTaskByMilestoneId(clientContext, MilestoneId);
                var path = spContext.SPHostUrl.Scheme + "://" + spContext.SPHostUrl.Host.ToString();
                lstDocument = BalDocument.GetDocumentByLookUpId(clientContext, lstTask[0].Project.ToString(), path);
                ViewBag.EmpData = GetEmpMembers(Members, ProjectManager);

                if (lstTask.Count > 0)
                {
                    obj.Add("OK");
                    obj.Add(lstTask);
                    obj.Add(lstDocument);
                    obj.Add(ViewBag.EmpData);
                }
            }
            return Json(obj, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ActionName("GetSubTask")]
        public JsonResult GetSubTask(int TaskId, int[] Members = null, int ProjectManager = 0)
        {
            List<object> obj = new List<object>();
            List<TIM_SubTaskModel> lstSubTask = new List<TIM_SubTaskModel>();
            List<TIM_DocumentLibraryModel> lstDocument = new List<TIM_DocumentLibraryModel>();
            var spContext = SharePointContextProvider.Current.GetSharePointContext(HttpContext);
            using (var clientContext = spContext.CreateUserClientContextForSPHost())
            {
                lstSubTask = BalSubTask.GetSubTaskByTaskId(clientContext, TaskId);
                var path = spContext.SPHostUrl.Scheme + "://" + spContext.SPHostUrl.Host.ToString();
                lstDocument = BalDocument.GetDocumentByLookUpId(clientContext, lstSubTask[0].Project.ToString(), path);
                if(ProjectManager != 0)
                    ViewBag.EmpData = GetEmpMembers(Members, ProjectManager);
                if (lstSubTask.Count > 0)
                {
                    obj.Add("OK");
                    obj.Add(lstSubTask);
                    obj.Add(lstDocument);
                    obj.Add(ViewBag.EmpData);
                }
            }
            return Json(obj, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ActionName("AddMilestone")]
        public JsonResult AddMilestone(List<TIM_MilestoneModel> AddMilestone, string Action)
        {
            List<object> obj = new List<object>();
            int i = 0;
            var spContext = SharePointContextProvider.Current.GetSharePointContext(HttpContext);
            try
            {
                using (var clientContext = spContext.CreateUserClientContextForSPHost())
                {
                    List<TIM_WorkFlowMasterModel> lstWorkFlow = new List<TIM_WorkFlowMasterModel>();
                    lstWorkFlow = BalWorkflow.GetWorkFlowForAddMilestone(clientContext);
                    foreach (var item in AddMilestone)
                    {
                        string returnID = "0";
                        string arr = String.Join(",", item.Members);

                        string itemdata = " 'MileStone': '" + item.MileStone.Replace("'", @"\'") + "'";
                        itemdata += " ,'MembersId': {'results': [" + arr + "] }";
                        itemdata += " ,'Description': '" + item.Description.Replace("'", @"\'") + "'";
                        itemdata += " ,'StartDate': '" + item.StartDate + "'";
                        itemdata += " ,'EndDate': '" + item.EndDate + "'";
                        itemdata += " ,'NoOfDays': '" + item.NoOfDays + "'";
                        itemdata += " ,'ProjectId': '" + item.Project + "'";
                        itemdata += " ,'ProjectManagerId': '" + item.ProjectManager + "'";
                        itemdata += " ,'MembersText': '" + item.MembersText + "'";
                        itemdata += " ,'ClientId': '" + item.Client + "'";

                        if (item.Status == 0)
                        {
                            itemdata += " ,'StatusId': '" + lstWorkFlow[0].ToStatusID + "'";
                            itemdata += " ,'InternalStatus': '" + lstWorkFlow[0].InternalStatus + "'";
                        }
                        else
                        {
                            itemdata += " ,'StatusId': '" + item.Status + "'";
                            itemdata += " ,'InternalStatus': '" + item.InternalStatus + "'";
                        }

                        if (item.ID > 0)
                        {
                            returnID = BalMilestone.UpdateMilestone(clientContext, itemdata, item.ID.ToString());
                            if (returnID == "Update")
                                i++;
                        }
                        else
                        {
                            returnID = BalMilestone.SaveMilestone(clientContext, itemdata);
                            if (Convert.ToInt32(returnID) > 0)
                                i++;
                        }

                    }
                    if (i == AddMilestone.Count && Action == "Insert")
                    {
                        string projectdata = "'StatusId': '" + lstWorkFlow[0].ToStatusID + "'";
                        projectdata += " ,'InternalStatus': '" + lstWorkFlow[0].InternalStatus + "'";
                        string returnText = BalProjectCreation.UpdateProject(clientContext, projectdata, AddMilestone[0].Project.ToString());
                        if (returnText == "Update")
                            obj.Add("OK");
                    }
                    else if (i == AddMilestone.Count)
                        obj.Add("OK");
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

        [HttpPost]
        [ActionName("DeleteAllMilestone")]
        public JsonResult DeleteAllMilestone(int ProjectId)
        {
            List<object> obj = new List<object>();
            try
            {
                var spContext = SharePointContextProvider.Current.GetSharePointContext(HttpContext);
                using (var clientContext = spContext.CreateUserClientContextForSPHost())
                {
                    List<TIM_WorkFlowMasterModel> lstWorkFlowProject = new List<TIM_WorkFlowMasterModel>();
                    lstWorkFlowProject = BalWorkflow.GetWorkFlowForProjectCreation(clientContext);
                    string projectdata = "'StatusId': '" + lstWorkFlowProject[0].ToStatusID + "'";
                    projectdata += " ,'InternalStatus': '" + lstWorkFlowProject[0].InternalStatus + "'";


                    List<TIM_WorkFlowMasterModel> lstWorkFlowMile = new List<TIM_WorkFlowMasterModel>();
                    lstWorkFlowMile = BalWorkflow.GetWorkFlowForMilestoneDeletion(clientContext);
                    string miledata = "'StatusId': '" + lstWorkFlowMile[0].ToStatusID + "'";
                    miledata += " ,'InternalStatus': '" + lstWorkFlowMile[0].InternalStatus + "'";

                    List<TIM_MilestoneModel> lstMilestone = new List<TIM_MilestoneModel>();
                    lstMilestone = BalMilestone.GetMilestoneByProjectId(clientContext, ProjectId);
                    if (lstMilestone.Count > 0)
                    {
                        int mile = 0;
                        foreach (var item in lstMilestone)
                        {
                            mile++;
                            BalMilestone.UpdateMilestone(clientContext, miledata ,item.ID.ToString());
                        }
                        if(mile == lstMilestone.Count)
                        {
                            List<TIM_WorkFlowMasterModel> lstWorkFlowForTask = new List<TIM_WorkFlowMasterModel>();
                            lstWorkFlowForTask = BalWorkflow.GetWorkFlowForTaskDeletion(clientContext);
                            if (lstWorkFlowForTask.Count > 0)
                            {
                                string itemdata = "'StatusId': '" + lstWorkFlowForTask[0].ToStatusID + "'";
                                itemdata += " ,'InternalStatus': '" + lstWorkFlowForTask[0].InternalStatus + "'";

                                List<TIM_TaskModel> lstTask = new List<TIM_TaskModel>();
                                lstTask = BalTask.GetTaskByProjectId(clientContext, ProjectId);
                                if (lstTask.Count > 0)
                                {
                                    int task = 0;
                                    foreach (var item in lstTask)
                                    {
                                        task++;
                                        BalTask.UpdateTask(clientContext, itemdata, item.ID.ToString());
                                    }
                                    if (task == lstTask.Count)
                                    {

                                        List<TIM_SubTaskModel> lstSubTask = new List<TIM_SubTaskModel>();
                                        lstSubTask = BalSubTask.GetSubTaskByProjectId(clientContext, ProjectId);
                                        if (lstSubTask.Count > 0)
                                        {
                                            List<TIM_WorkFlowMasterModel> lstWorkFlowForSubTask = new List<TIM_WorkFlowMasterModel>();
                                            lstWorkFlowForSubTask = BalWorkflow.GetWorkFlowForSubTaskDeletion(clientContext);

                                            if (lstWorkFlowForSubTask.Count > 0)
                                            {
                                                string itemsubdata = "'StatusId': '" + lstWorkFlowForSubTask[0].ToStatusID + "'";
                                                itemsubdata += " ,'InternalStatus': '" + lstWorkFlowForSubTask[0].InternalStatus + "'";

                                                int subtask = 0;
                                                foreach (var item in lstSubTask)
                                                {
                                                    subtask++;
                                                    BalSubTask.UpdateSubTask(clientContext, itemsubdata, item.ID.ToString());
                                                }
                                                if (subtask == lstSubTask.Count)
                                                {
                                                    string returnID = BalProjectCreation.UpdateProject(clientContext, projectdata, ProjectId.ToString());
                                                    if (returnID == "Update")
                                                        obj.Add("OK");
                                                }
                                            }

                                        }
                                        else
                                        {
                                            string returnID = BalProjectCreation.UpdateProject(clientContext, projectdata, ProjectId.ToString());
                                            if (returnID == "Update")
                                                obj.Add("OK");

                                        }
                                    }
                                }
                                else
                                {
                                    string returnID = BalProjectCreation.UpdateProject(clientContext, projectdata, ProjectId.ToString());
                                    if (returnID == "Update")
                                        obj.Add("OK");
                                }

                            }
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


        [HttpPost]
        [ActionName("AddTask")]
        public JsonResult AddTask(List<TIM_TaskModel> AddTask, string Action)
        {
            List<object> obj = new List<object>();
            int i = 0;
            var spContext = SharePointContextProvider.Current.GetSharePointContext(HttpContext);
            try
            {
                using (var clientContext = spContext.CreateUserClientContextForSPHost())
                {
                    List<TIM_StatusMasterModel> lstPendingStatus = new List<TIM_StatusMasterModel>();
                    lstPendingStatus = BalStatus.GetPendingStatus(clientContext);
                    List<TIM_WorkFlowMasterModel> lstWorkFlow = new List<TIM_WorkFlowMasterModel>();
                    lstWorkFlow = BalWorkflow.GetWorkFlowForAddTask(clientContext);
                    Emp_BasicInfoModel AssignedBy = BalEmp.GetEmpMailByLogIn(clientContext);
                    string returnID = "0";
                    foreach (var item in AddTask)
                    {
                        string itemdata = " 'MileStoneId': '" + item.MileStone + "'";
                        itemdata += " ,'MembersId': '" + item.Members + "'";
                        itemdata += " ,'StartDate': '" + item.StartDate + "'";
                        itemdata += " ,'EndDate': '" + item.EndDate + "'";
                        itemdata += " ,'NoOfDays': '" + item.NoOfDays + "'";
                        itemdata += " ,'ProjectId': '" + item.Project + "'";
                        itemdata += " ,'Task': '" + item.Task.Replace("'", @"\'") + "'";
                        itemdata += " ,'ClientId': '" + item.Client + "'";

                        //if (lstWorkFlow.Count > 0)
                        //{
                        //    itemdata += " ,'StatusId': '" + lstWorkFlow[0].ToStatusID + "'";
                        //    itemdata += " ,'InternalStatus': '" + lstWorkFlow[0].InternalStatus + "'";
                        //}

                        if (item.Status == 0)
                        {
                            itemdata += " ,'StatusId': '" + lstWorkFlow[0].ToStatusID + "'";
                            itemdata += " ,'InternalStatus': '" + lstWorkFlow[0].InternalStatus + "'";
                            itemdata += " ,'TaskStatusId': '" + lstPendingStatus[0].ID + "'";

                        }
                        else
                        {
                            itemdata += " ,'StatusId': '" + item.Status + "'";
                            itemdata += " ,'InternalStatus': '" + item.InternalStatus + "'";
                        }

                        if (item.ID > 0)
                        {
                            returnID = BalTask.UpdateTask(clientContext, itemdata, item.ID.ToString());
                            if (returnID == "Update")
                                i++;
                        }
                        else
                        {
                            returnID = BalTask.SaveTask(clientContext, itemdata);
                            if (Convert.ToInt32(returnID) > 0)
                            {
                                string Mailres = EmailCtrl.TaskAssignmentNotification(clientContext, item, AssignedBy);
                                if(Convert.ToInt32(Mailres) > 0)
                                    i++;
                            }
                                
                        }


                    }
                    if (i == AddTask.Count && Action == "Insert")
                    {
                        string milestonedata = "'StatusId': '" + lstWorkFlow[0].ToStatusID + "'";
                        milestonedata += " ,'InternalStatus': '" + lstWorkFlow[0].InternalStatus + "'";
                        string returnMileText = BalMilestone.UpdateMilestone(clientContext, milestonedata, AddTask[0].MileStone.ToString());
                        if (returnMileText == "Update")
                        {
                            string projectdata = "'StatusId': '" + lstWorkFlow[0].ToStatusID + "'";
                            projectdata += " ,'InternalStatus': '" + lstWorkFlow[0].InternalStatus + "'";
                            string returnText = BalProjectCreation.UpdateProject(clientContext, projectdata, AddTask[0].Project.ToString());
                            if (returnText == "Update")
                                obj.Add("OK");
                        }
                    }
                    else if (i == AddTask.Count)
                        obj.Add("OK");
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

        [HttpPost]
        [ActionName("DeleteAllTask")]
        public JsonResult DeleteAllTask(int ProjectId, int MilestoneId)
        {
            List<object> obj = new List<object>();
            try
            {
                var spContext = SharePointContextProvider.Current.GetSharePointContext(HttpContext);
                using (var clientContext = spContext.CreateUserClientContextForSPHost())
                {
                    List<TIM_WorkFlowMasterModel> lstWorkFlowForMilestone = new List<TIM_WorkFlowMasterModel>();
                    lstWorkFlowForMilestone = BalWorkflow.GetWorkFlowForAddMilestone(clientContext);
                    string miledata = "'StatusId': '" + lstWorkFlowForMilestone[0].ToStatusID + "'";
                    miledata += " ,'InternalStatus': '" + lstWorkFlowForMilestone[0].InternalStatus + "'";

                    List<TIM_WorkFlowMasterModel> lstWorkFlowForTask = new List<TIM_WorkFlowMasterModel>();
                    lstWorkFlowForTask = BalWorkflow.GetWorkFlowForTaskDeletion(clientContext);
                    if (lstWorkFlowForTask.Count > 0)
                    {
                        string itemdata = "'StatusId': '" + lstWorkFlowForTask[0].ToStatusID + "'";
                        itemdata += " ,'InternalStatus': '" + lstWorkFlowForTask[0].InternalStatus + "'";

                        List<TIM_TaskModel> lstTask = new List<TIM_TaskModel>();
                        lstTask = BalTask.GetTaskByProjectId(clientContext, ProjectId);
                        if (lstTask.Count > 0)
                        {
                            int task = 0;
                            foreach (var item in lstTask)
                            {
                                task++;
                                BalTask.UpdateTask(clientContext, itemdata, item.ID.ToString());
                            }
                            if (task == lstTask.Count)
                            {

                                List<TIM_SubTaskModel> lstSubTask = new List<TIM_SubTaskModel>();
                                lstSubTask = BalSubTask.GetSubTaskByProjectId(clientContext, ProjectId);
                                if (lstSubTask.Count > 0)
                                {
                                    List<TIM_WorkFlowMasterModel> lstWorkFlowForSubTask = new List<TIM_WorkFlowMasterModel>();
                                    lstWorkFlowForSubTask = BalWorkflow.GetWorkFlowForSubTaskDeletion(clientContext);

                                    if (lstWorkFlowForSubTask.Count > 0)
                                    {
                                        string itemsubdata = "'StatusId': '" + lstWorkFlowForSubTask[0].ToStatusID + "'";
                                        itemsubdata += " ,'InternalStatus': '" + lstWorkFlowForSubTask[0].InternalStatus + "'";

                                        int subtask = 0;
                                        foreach (var item in lstSubTask)
                                        {
                                            subtask++;
                                            BalSubTask.UpdateSubTask(clientContext, itemsubdata, item.ID.ToString());
                                        }
                                        if (subtask == lstSubTask.Count)
                                        {
                                            string returnID = BalMilestone.UpdateMilestone(clientContext, miledata, MilestoneId.ToString());
                                            if (returnID == "Update")
                                                    obj.Add("OK");
                                        }
                                    }
                                    
                                }
                                else
                                {
                                    string returnID = BalMilestone.UpdateMilestone(clientContext, miledata, MilestoneId.ToString());
                                    if (returnID == "Update")
                                            obj.Add("OK");

                                }
                            }
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

        [HttpPost]
        [ActionName("AddSubTask")]
        public JsonResult AddSubTask(List<TIM_SubTaskModel> AddSubTask, string Action)
        {
            List<object> obj = new List<object>();
            int i = 0;
            var spContext = SharePointContextProvider.Current.GetSharePointContext(HttpContext);
            try
            {
                using (var clientContext = spContext.CreateUserClientContextForSPHost())
                {
                    List<TIM_StatusMasterModel> lstPendingStatus = new List<TIM_StatusMasterModel>();
                    lstPendingStatus = BalStatus.GetPendingStatus(clientContext);

                    List<TIM_WorkFlowMasterModel> lstWorkFlow = new List<TIM_WorkFlowMasterModel>();
                    lstWorkFlow = BalWorkflow.GetWorkFlowForAddSubTask(clientContext);
                    Emp_BasicInfoModel AssignedBy = BalEmp.GetEmpMailByLogIn(clientContext);

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
                        itemdata += " ,'SubTask': '" + item.SubTask.Replace("'", @"\'") + "'";
                        itemdata += " ,'ClientId': '" + item.Client + "'";
                        itemdata += " ,'SubTaskStatusId': '" + lstPendingStatus[0].ID + "'";

                        if (item.Status == 0)
                        {
                            itemdata += " ,'StatusId': '" + lstWorkFlow[0].ToStatusID + "'";
                            itemdata += " ,'InternalStatus': '" + lstWorkFlow[0].InternalStatus + "'";
                        }
                        else
                        {
                            itemdata += " ,'StatusId': '" + item.Status + "'";
                            itemdata += " ,'InternalStatus': '" + item.InternalStatus + "'";
                        }

                        if (item.ID > 0)
                        {
                            returnID = BalSubTask.UpdateSubTask(clientContext, itemdata, item.ID.ToString());
                            if (returnID == "Update")
                                i++;
                        }
                        else
                        {
                            returnID = BalSubTask.SaveSubTask(clientContext, itemdata);
                            if (Convert.ToInt32(returnID) > 0)
                            {
                                string Mailres = EmailCtrl.SubTaskAssignmentNotification(clientContext, item, AssignedBy);
                                if (Convert.ToInt32(Mailres) > 0)
                                    i++;
                            }
                        }

                        //if (lstWorkFlow.Count > 0)
                        //{
                        //    itemdata += " ,'StatusId': '" + lstWorkFlow[0].ToStatusID + "'";
                        //    itemdata += " ,'InternalStatus': '" + lstWorkFlow[0].InternalStatus + "'";
                        //}


                    }
                    if (i == AddSubTask.Count && Action == "Insert")
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
                    else if (i == AddSubTask.Count)
                        obj.Add("OK");
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

        [HttpPost]
        [ActionName("DeleteAllSubTask")]
        public JsonResult DeleteAllSubTask(int TaskId, int ProjectId, int MilestoneId)
        {
            List<object> obj = new List<object>();
            try
            {
                var spContext = SharePointContextProvider.Current.GetSharePointContext(HttpContext);
                using (var clientContext = spContext.CreateUserClientContextForSPHost())
                {
                    List<TIM_WorkFlowMasterModel> lstWorkFlowForTask = new List<TIM_WorkFlowMasterModel>();
                    lstWorkFlowForTask = BalWorkflow.GetWorkFlowForAddTask(clientContext);
                    string taskdata = "'StatusId': '" + lstWorkFlowForTask[0].ToStatusID + "'";
                    taskdata += " ,'InternalStatus': '" + lstWorkFlowForTask[0].InternalStatus + "'";

                    List<TIM_WorkFlowMasterModel> lstWorkFlowForSubTask = new List<TIM_WorkFlowMasterModel>();
                    lstWorkFlowForSubTask = BalWorkflow.GetWorkFlowForSubTaskDeletion(clientContext);
                    if (lstWorkFlowForSubTask.Count > 0)
                    {
                        string itemdata = "'StatusId': '" + lstWorkFlowForSubTask[0].ToStatusID + "'";
                        itemdata += " ,'InternalStatus': '" + lstWorkFlowForSubTask[0].InternalStatus + "'";

                        List<TIM_SubTaskModel> lstSubTask = new List<TIM_SubTaskModel>();
                        lstSubTask = BalSubTask.GetSubTaskByProjectId(clientContext, ProjectId);
                        if (lstSubTask.Count > 0)
                        {
                            int subtask = 0;
                            foreach (var item in lstSubTask)
                            {
                                subtask++;
                                BalSubTask.UpdateSubTask(clientContext, itemdata, item.ID.ToString());
                            }
                            if (subtask == lstSubTask.Count)
                            {
                                string returnID = BalTask.UpdateTask(clientContext, taskdata, TaskId.ToString());
                                if (returnID == "Update")
                                    obj.Add("OK");
                            }
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

        [HttpPost]
        [ActionName("AddSubTaskMain")]
        public JsonResult AddSubTaskMain(int TaskId, int ProjectId, int MilestoneId)
        {
            List<object> obj = new List<object>();
            try
            {
                var spContext = SharePointContextProvider.Current.GetSharePointContext(HttpContext);
                using (var clientContext = spContext.CreateUserClientContextForSPHost())
                {
                    List<TIM_ProjectCreationModel> lstProject = new List<TIM_ProjectCreationModel>();
                    List<TIM_DocumentLibraryModel> lstDocument = new List<TIM_DocumentLibraryModel>();
                    lstProject = BalProjectCreation.GetProjectCreationById(clientContext, ProjectId);
                    List<TIM_MilestoneModel> lstMilestone = new List<TIM_MilestoneModel>();
                    lstMilestone = BalMilestone.GetMilestoneByMilestoneId(clientContext, MilestoneId);
                    var path = spContext.SPHostUrl.Scheme + "://" + spContext.SPHostUrl.Host.ToString();
                    lstDocument = BalDocument.GetDocumentByLookUpId(clientContext, ProjectId.ToString(), path);
                    ViewBag.EmpData = GetEmpMembers(lstProject[0].Members, lstProject[0].ProjectManager);

                    if (lstProject.Count>0 && lstMilestone.Count > 0)
                    {
                        obj.Add("OK");
                        obj.Add(lstProject[0]);
                        obj.Add(lstMilestone[0]);
                        obj.Add(lstDocument);
                        obj.Add(ViewBag.EmpData);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("An error occured while performing action. GUID: {0}", ex.ToString()));
            }

            return Json(obj, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ActionName("GetCompletedTimesheet")]
        public JsonResult GetCompletedTimesheet(int MilestoneId)
        {
            List<object> obj = new List<object>();
            List<TIM_EmployeeTimesheetModel> lstEmployeeTimesheet = new List<TIM_EmployeeTimesheetModel>();
            List<TIM_DocumentLibraryModel> lstDocument = new List<TIM_DocumentLibraryModel>();
            try
            {
                var spContext = SharePointContextProvider.Current.GetSharePointContext(HttpContext);
                using (var clientContext = spContext.CreateUserClientContextForSPHost())
                {
                    lstEmployeeTimesheet = BalEmpTimesheet.GetCompletedTimesheet(clientContext, MilestoneId);
                    var path = spContext.SPHostUrl.Scheme + "://" + spContext.SPHostUrl.Host.ToString();
                    if(lstEmployeeTimesheet.Count>0)
                        lstDocument = BalDocument.GetDocumentByTimesheetId(clientContext, lstEmployeeTimesheet[0].TimesheetID, path);

                    obj.Add("OK");
                    obj.Add(lstEmployeeTimesheet);
                    obj.Add(lstDocument);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("An error occured while performing action. GUID: {0}", ex.ToString()));
            }
            return Json(obj, JsonRequestBehavior.AllowGet);
        }

        //[HttpPost]
        //[ActionName("DeleteProject")]
        //public JsonResult DeleteProject()
        //{
        //    List<object> obj = new List<object>();
        //    var spContext = SharePointContextProvider.Current.GetSharePointContext(HttpContext);
        //    using (var clientContext = spContext.CreateUserClientContextForSPHost())
        //    {
        //        if (Request.Cookies["ProjectId"] != null)
        //        {
        //            int ProjectId = Convert.ToInt32(Request.Cookies["ProjectId"].Value);
        //            string returnID = "0";
        //            List<TIM_WorkFlowMasterModel> lstWorkFlow = new List<TIM_WorkFlowMasterModel>();
        //            lstWorkFlow = BalWorkflow.GetWorkFlowForProjectDeletion(clientContext);
        //            if (lstWorkFlow.Count > 0)
        //            {
        //                string itemdata = "'StatusId': '" + lstWorkFlow[0].ToStatusID + "'";
        //                itemdata += " ,'InternalStatus': '" + lstWorkFlow[0].InternalStatus + "'";
        //                returnID = BalProjectCreation.UpdateProject(clientContext, itemdata, ProjectId.ToString());
        //            }
        //            if (returnID == "Update")
        //            {
        //                obj.Add("OK");
        //            }
        //        }
        //    }
        //    return Json(obj, JsonRequestBehavior.AllowGet);
        //}

    }
}