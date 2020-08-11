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
        TIM_StatusMasterBal BalStatus = new TIM_StatusMasterBal();

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
            try
            {
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

                    if (Project.ID == 0)
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
                        {
                            if (Project.InternalStatus == "ProjectDeleted")
                            {
                                List<TIM_WorkFlowMasterModel> lstWorkFlow = new List<TIM_WorkFlowMasterModel>();
                                lstWorkFlow = BalWorkflow.GetWorkFlowForProjectDeletion(clientContext);
                                if (lstWorkFlow.Count > 0)
                                {
                                    string projectdata = "'StatusId': '" + lstWorkFlow[0].ToStatusID + "'";
                                    projectdata += " ,'InternalStatus': '" + lstWorkFlow[0].InternalStatus + "'";
                                    returnID = BalProjectCreation.UpdateProject(clientContext, projectdata, Project.ID.ToString());
                                    if (returnID == "Update")
                                    {
                                        List<TIM_MilestoneModel> lstMilestone = new List<TIM_MilestoneModel>();
                                        lstMilestone = BalMilestone.GetMilestoneByProjectId(clientContext, Project.ID);
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
                                                lstTask = BalTask.GetTaskByProjectId(clientContext, Project.ID);
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
                                                        lstSubTask = BalSubTask.GetSubTaskByProjectId(clientContext, Project.ID);
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
                        itemdata += " ,'Description': '" + item.Description + "'";
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
                    string returnID = "0";
                    foreach (var item in AddTask)
                    {
                        string itemdata = " 'MileStoneId': '" + item.MileStone + "'";
                        itemdata += " ,'MembersId': '" + item.Members + "'";
                        itemdata += " ,'StartDate': '" + item.StartDate + "'";
                        itemdata += " ,'EndDate': '" + item.EndDate + "'";
                        itemdata += " ,'NoOfDays': '" + item.NoOfDays + "'";
                        itemdata += " ,'ProjectId': '" + item.Project + "'";
                        itemdata += " ,'Task': '" + item.Task + "'";
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
                                i++;
                        }

                        //returnID = BalTask.SaveTask(clientContext, itemdata);
                        //if (Convert.ToInt32(returnID) > 0)
                        //    i++;

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
                        itemdata += " ,'SubTask': '" + item.SubTask + "'";
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
                                i++;
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
                    lstProject = BalProjectCreation.GetProjectCreationById(clientContext, ProjectId);
                    List<TIM_MilestoneModel> lstMilestone = new List<TIM_MilestoneModel>();
                    lstMilestone = BalMilestone.GetMilestoneByMilestoneId(clientContext, MilestoneId);
                    if(lstProject.Count>0 && lstMilestone.Count > 0)
                    {
                        obj.Add("OK");
                        obj.Add(lstProject[0]);
                        obj.Add(lstMilestone[0]);
                    }
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