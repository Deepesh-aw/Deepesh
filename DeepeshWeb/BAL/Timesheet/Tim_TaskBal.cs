using DeepeshWeb.DAL;
using DeepeshWeb.Models;
using DeepeshWeb.Models.Timesheet;
using Microsoft.SharePoint.Client;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DeepeshWeb.BAL.Timesheet
{
    public class TIM_TaskBal
    {
        public List<TIM_TaskModel> GetAllTask(ClientContext clientContext, int LogInId)
        {
            List<TIM_TaskModel> lstTask = new List<TIM_TaskModel>();
            string filter = "MembersId eq " + LogInId + " and InternalStatus ne 'TaskDeleted' and InternalStatus ne 'ProjectDeleted'";
            JArray jArray = RESTGet(clientContext, filter);
            lstTask = BindList(jArray);
            return lstTask;
        }

        public List<TIM_TaskModel> GetTaskByMilestoneId(ClientContext clientContext, int MilestoneId)
        {
            List<TIM_TaskModel> lstTask = new List<TIM_TaskModel>();
            string filter = "MileStoneId eq " + MilestoneId + " and InternalStatus ne 'TaskDeleted' and InternalStatus ne 'ProjectDeleted'";
            JArray jArray = RESTGet(clientContext, filter);
            lstTask = BindList(jArray);
            return lstTask;
        }

        public List<TIM_TaskModel> GetTaskByTaskId(ClientContext clientContext, int TaskId)
        {
            List<TIM_TaskModel> lstTask = new List<TIM_TaskModel>();
            string filter = "ID eq " + TaskId + " and InternalStatus ne 'TaskDeleted' and InternalStatus ne 'ProjectDeleted'";
            JArray jArray = RESTGet(clientContext, filter);
            lstTask = BindList(jArray);
            return lstTask;
        }

        public List<TIM_TaskModel> GetTaskByProjectId(ClientContext clientContext, int ProjectId)
        {
            List<TIM_TaskModel> lstTask = new List<TIM_TaskModel>();
            string filter = "ProjectId eq " + ProjectId + " and InternalStatus ne 'TaskDeleted' and InternalStatus ne 'ProjectDeleted'";
            JArray jArray = RESTGet(clientContext, filter);
            lstTask = BindList(jArray);
            return lstTask;
        }

        public List<TIM_TaskModel> BindList(JArray jArray)
        {
            List<TIM_TaskModel> lstTask = new List<TIM_TaskModel>();
            foreach (JObject j in jArray)
            {
                TIM_TaskModel data = new TIM_TaskModel();
                
                data.ID = j["ID"] == null ? 0 : Convert.ToInt32(j["ID"]);
                data.Task = j["Task"] == null ? "" : Convert.ToString(j["Task"]);
                data.StartDate = j["StartDate"] == null ? "" : Convert.ToString(j["StartDate"]);
                data.EndDate = j["EndDate"] == null ? "" : Convert.ToString(j["EndDate"]);
                data.NoOfDays = j["NoOfDays"] == null ? 0 : Convert.ToInt32(j["NoOfDays"]);
                data.InternalStatus = j["InternalStatus"] == null ? "" : Convert.ToString(j["InternalStatus"]);
                data.Status = j["Status"]["ID"] == null ? 0 : Convert.ToInt32(j["Status"]["ID"]);
                data.StatusName = j["Status"]["StatusName"] == null ? "" : j["Status"]["StatusName"].ToString();
                data.MileStone = j["MileStone"]["ID"] == null ? 0 : Convert.ToInt32(j["MileStone"]["ID"]);
                data.MileStoneName = j["MileStone"]["MileStone"] == null ? "" : Convert.ToString(j["MileStone"]["MileStone"]);
                data.Project = j["Project"]["Id"] == null ? 0 : Convert.ToInt32(j["Project"]["Id"]);
                data.ProjectName = j["Project"]["ProjectName"] == null ? "" : j["Project"]["ProjectName"].ToString();
                data.TaskStatus = j["TaskStatus"]["ID"] == null ? 0 : Convert.ToInt32(j["TaskStatus"]["ID"]);
                data.TaskStatusName = j["TaskStatus"]["StatusName"] == null ? "" : j["TaskStatus"]["StatusName"].ToString();
                data.Members = j["Members"]["ID"] == null ? 0 : Convert.ToInt32(j["Members"]["ID"]);
                data.MembersName = j["Members"]["FirstName"] == null ? "" : j["Members"]["FirstName"].ToString() +" "+ j["Members"]["LastName"].ToString();
                data.ClientName = j["Client"]["ClientName"] == null ? "" : Convert.ToString(j["Client"]["ClientName"].ToString());
                data.Client = j["Client"]["ID"] == null ? 0 : Convert.ToInt32(j["Client"]["ID"].ToString());
                lstTask.Add(data);
            }

            return lstTask;
        }

        public string SaveTask(ClientContext clientContext, string ItemData)
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
            rESTOption.select = "ID,Task,StartDate,EndDate,Client/ID,Client/ClientName,Status/StatusName,Status/ID,TaskStatus/StatusName,TaskStatus/ID,InternalStatus,NoOfDays,Members/ID,Members/FirstName,Members/LastName,Project/Id,Project/ProjectName,MileStone/ID,MileStone/MileStone";
            rESTOption.expand = "Project,MileStone,Members,Status,TaskStatus,Client";
            rESTOption.top = "5000";


            jArray = restService.GetAllItemFromList(clientContext, "TIM_Task", rESTOption);

            return jArray;
        }

        private string RESTSave(ClientContext clientContext, string ItemData)
        {
            RestService restService = new RestService();
            return restService.SaveItem(clientContext, "TIM_Task", ItemData);
        }

        public string UpdateTask(ClientContext clientContext, string ItemData, string ID)
        {

            string response = RESTUpdate(clientContext, ItemData, ID);

            return response;
        }

        private string RESTUpdate(ClientContext clientContext, string ItemData, string ID)
        {
            RestService restService = new RestService();

            return restService.UpdateItem(clientContext, "TIM_Task", ItemData, ID);
        }
    }
}