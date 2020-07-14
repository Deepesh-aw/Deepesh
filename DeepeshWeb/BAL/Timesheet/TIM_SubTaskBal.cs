using DeepeshWeb.DAL;
using DeepeshWeb.Models;
using Microsoft.SharePoint.Client;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DeepeshWeb.BAL.Timesheet
{
    public class TIM_SubTaskBal
    {
        public List<TIM_SubTaskModel> GetSubTaskBySubTaskId(ClientContext clientContext, int SubTaskId)
        {
            List<TIM_SubTaskModel> lstSubTask = new List<TIM_SubTaskModel>();
            string filter = "Id eq " + SubTaskId + " and InternalStatus ne 'SubTaskDeleted' and InternalStatus ne 'ProjectDeleted' and InternalStatus ne 'Approved'";
            JArray jArray = RESTGet(clientContext, filter);
            lstSubTask = BindList(jArray);
            return lstSubTask;
        }

        public List<TIM_SubTaskModel> GetAllSubTask(ClientContext clientContext, int LogInId)
        {
            List<TIM_SubTaskModel> lstSubTask = new List<TIM_SubTaskModel>();
            string filter = "MembersId eq " + LogInId + " and InternalStatus ne 'TaskDeleted' and InternalStatus ne 'ProjectDeleted'";
            JArray jArray = RESTGet(clientContext, filter);
            lstSubTask = BindList(jArray);
            return lstSubTask;
        }

        public List<TIM_SubTaskModel> GetSubTaskByTaskId(ClientContext clientContext, int TaskId)
        {
            List<TIM_SubTaskModel> lstSubTask = new List<TIM_SubTaskModel>();
            string filter = "TaskId eq " + TaskId + " and InternalStatus ne 'SubTaskDeleted' and InternalStatus ne 'ProjectDeleted' and InternalStatus ne 'Approved'";
            JArray jArray = RESTGet(clientContext, filter);
            lstSubTask = BindList(jArray);
            return lstSubTask;
        }

        public List<TIM_SubTaskModel> GetSubTaskByProjectId(ClientContext clientContext, int ProjectId)
        {
            List<TIM_SubTaskModel> lstSubTask = new List<TIM_SubTaskModel>();
            string filter = "ProjectId eq " + ProjectId + " and InternalStatus ne 'SubTaskDeleted' and InternalStatus ne 'ProjectDeleted'";
            JArray jArray = RESTGet(clientContext, filter);
            lstSubTask = BindList(jArray);
            return lstSubTask;
        }

        public List<TIM_SubTaskModel> BindList(JArray jArray)
        {
            List<TIM_SubTaskModel> lstSubTask = new List<TIM_SubTaskModel>();
            foreach (JObject j in jArray)
            {
                TIM_SubTaskModel data = new TIM_SubTaskModel();

                data.ID = j["ID"] == null ? 0 : Convert.ToInt32(j["ID"]);
                data.SubTask = j["SubTask"] == null ? "" : Convert.ToString(j["SubTask"]);
                data.StartDate = j["StartDate"] == null ? "" : Convert.ToString(j["StartDate"]);
                data.EndDate = j["EndDate"] == null ? "" : Convert.ToString(j["EndDate"]);
                data.NoOfDays = j["NoOfDays"] == null ? 0 : Convert.ToInt32(j["NoOfDays"]);
                data.InternalStatus = j["InternalStatus"] == null ? "" : Convert.ToString(j["InternalStatus"]);
                data.Status = j["Status"]["ID"] == null ? 0 : Convert.ToInt32(j["Status"]["ID"]);
                data.StatusName = j["Status"]["StatusName"] == null ? "" : j["Status"]["StatusName"].ToString();
                data.MileStone = j["MileStone"]["ID"] == null ? 0 : Convert.ToInt32(j["MileStone"]["ID"]);
                data.MileStoneName = j["MileStone"]["MileStone"] == null ? "" : Convert.ToString(j["MileStone"]["MileStone"]);
                data.Task = j["Task"]["Id"] == null ? 0 : Convert.ToInt32(j["Task"]["Id"]);
                data.TaskName = j["Task"]["Task"] == null ? "" : j["Task"]["Task"].ToString();
                data.Project = j["Project"]["Id"] == null ? 0 : Convert.ToInt32(j["Project"]["Id"]);
                data.ProjectName = j["Project"]["ProjectName"] == null ? "" : j["Project"]["ProjectName"].ToString();
                data.SubTaskStatus = j["SubTaskStatus"]["ID"] == null ? 0 : Convert.ToInt32(j["SubTaskStatus"]["ID"]);
                data.SubTaskStatusName = j["SubTaskStatus"]["StatusName"] == null ? "" : j["SubTaskStatus"]["StatusName"].ToString();
                data.Members = j["Members"]["ID"] == null ? 0 : Convert.ToInt32(j["Members"]["ID"]);
                data.MembersName = j["Members"]["FirstName"] == null ? "" : j["Members"]["FirstName"].ToString() + " " + j["Members"]["LastName"].ToString();
                data.ClientName = j["Client"]["ClientName"] == null ? "" : Convert.ToString(j["Client"]["ClientName"].ToString());
                data.Client = j["Client"]["ID"] == null ? 0 : Convert.ToInt32(j["Client"]["ID"].ToString());
                lstSubTask.Add(data);
            }

            return lstSubTask;
        }

        public string SaveSubTask(ClientContext clientContext, string ItemData)
        {
            string response = RESTSave(clientContext, ItemData);
            return response;
        }

        private JArray RESTGet(ClientContext clientContext, string filter)
        {
            RestService restService = new RestService();
            JArray jArray = new JArray();
            RESTOption rESTOption = new RESTOption();

            rESTOption.filter = filter;
            rESTOption.select = "ID,SubTask,StartDate,EndDate,Client/ID,Client/ClientName,Task/Id,Task/Task,Status/StatusName,Status/ID,SubTaskStatus/StatusName,SubTaskStatus/ID,InternalStatus,NoOfDays,Members/ID,Members/FirstName,Members/LastName,Project/Id,Project/ProjectName,MileStone/ID,MileStone/MileStone";
            rESTOption.expand = "Project,MileStone,Members,Status,SubTaskStatus,Task,Client";
            rESTOption.top = "5000";


            jArray = restService.GetAllItemFromList(clientContext, "TIM_SubTask", rESTOption);

            return jArray;
        }

        private string RESTSave(ClientContext clientContext, string ItemData)
        {
            RestService restService = new RestService();
            return restService.SaveItem(clientContext, "TIM_SubTask", ItemData);
        }

        public string UpdateSubTask(ClientContext clientContext, string ItemData, string ID)
        {

            string response = RESTUpdate(clientContext, ItemData, ID);

            return response;
        }

        private string RESTUpdate(ClientContext clientContext, string ItemData, string ID)
        {
            RestService restService = new RestService();

            return restService.UpdateItem(clientContext, "TIM_SubTask", ItemData, ID);
        }
    }
}