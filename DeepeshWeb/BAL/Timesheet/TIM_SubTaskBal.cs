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
        public List<TIM_SubTaskModel> GetSubTaskByTaskId(ClientContext clientContext, int TaskId)
        {
            List<TIM_SubTaskModel> lstSubTask = new List<TIM_SubTaskModel>();
            string filter = "TaskId eq " + TaskId + "";
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
                data.Status = j["Status"]["Id"] == null ? 0 : Convert.ToInt32(j["Status"]["Id"]);
                data.StatusName = j["Status"]["StatusName"] == null ? "" : j["Status"]["StatusName"].ToString();
                data.MileStone = j["MileStone"]["Id"] == null ? 0 : Convert.ToInt32(j["MileStone"]["Id"]);
                data.MileStoneName = j["MileStone"]["MileStone"] == null ? "" : Convert.ToString(j["MileStone"]["MileStone"]);
                data.Task = j["Task"]["Id"] == null ? 0 : Convert.ToInt32(j["Task"]["Id"]);
                data.TaskName = j["Task"]["Task"] == null ? "" : j["Task"]["Task"].ToString();
                data.Project = j["Project"]["Id"] == null ? 0 : Convert.ToInt32(j["Project"]["Id"]);
                data.ProjectName = j["Project"]["ProjectName"] == null ? "" : j["Project"]["ProjectName"].ToString();
                data.SubTaskStatus = j["SubTaskStatus"]["Id"] == null ? 0 : Convert.ToInt32(j["SubTaskStatus"]["Id"]);
                data.SubTaskStatusName = j["SubTaskStatus"]["StatusName"] == null ? "" : j["SubTaskStatus"]["StatusName"].ToString();
                data.Members = j["Members"]["Id"] == null ? 0 : Convert.ToInt32(j["Members"]["Id"]);
                data.MembersName = j["Members"]["FirstName"] == null ? "" : j["Members"]["FirstName"].ToString() + " " + j["Members"]["LastName"].ToString();

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
            rESTOption.select = "ID,SubTask,StartDate,EndDate,Task/Id,Task/Task,Status/StatusName,Status/ID,SubTaskStatus/StatusName,SubTaskStatus/ID,InternalStatus,NoOfDays,Members/ID,Members/FirstName,Members/LastName,Project/Id,Project/ProjectName,MileStone/ID,MileStone/MileStone";
            rESTOption.expand = "Project,MileStone,Members,Status,SubTaskStatus,Task";
            rESTOption.top = "5000";


            jArray = restService.GetAllItemFromList(clientContext, "TIM_SubTask", rESTOption);

            return jArray;
        }

        private string RESTSave(ClientContext clientContext, string ItemData)
        {
            RestService restService = new RestService();
            return restService.SaveItem(clientContext, "TIM_SubTask", ItemData);
        }
    }
}