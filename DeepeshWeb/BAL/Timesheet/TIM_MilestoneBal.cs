﻿using DeepeshWeb.DAL;
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
    public class TIM_MilestoneBal
    {
        public List<TIM_MilestoneModel> GetMilestoneByProjectId(ClientContext clientContext, int Id = 2)
        {
            List<TIM_MilestoneModel> lstMilestone = new List<TIM_MilestoneModel>();
            string filter = "ProjectId eq " + Id + "";
            JArray jArray = RESTGet(clientContext, filter);
            lstMilestone = BindList(jArray);
            return lstMilestone;
        }

        public List<TIM_MilestoneModel> GetMilestoneByMilestoneId(ClientContext clientContext, int Id)
        {
            List<TIM_MilestoneModel> lstMilestone = new List<TIM_MilestoneModel>();
            string filter = "ID eq " + Id + "";
            JArray jArray = RESTGet(clientContext, filter);
            lstMilestone = BindList(jArray);
            return lstMilestone;
        }

        public List<TIM_MilestoneModel> BindList(JArray jArray)
        {
            List<TIM_MilestoneModel> lstMilestone = new List<TIM_MilestoneModel>();
            foreach (JObject j in jArray)
            {
                TIM_MilestoneModel data = new TIM_MilestoneModel();
                int i = 0;
                data.Members = new Int32[((Newtonsoft.Json.Linq.JContainer)j["Members"]["results"]).Count];
                foreach (var item in j["Members"]["results"])
                {
                    data.Members[i] = Convert.ToInt32(item["ID"]);
                    i++;
                }
                data.ID = j["ID"] == null ? 0 : Convert.ToInt32(j["ID"]);
                data.Project = j["Project"]["Id"] == null ? 0 : Convert.ToInt32(j["Project"]["Id"]);
                data.Status = j["Status"]["Id"] == null ? 0 : Convert.ToInt32(j["Status"]["Id"]);
                data.ProjectManager = j["ProjectManager"]["Id"] == null ? 0 : Convert.ToInt32(j["ProjectManager"]["Id"]);
                data.MembersText = j["MembersText"] == null ? "" : Convert.ToString(j["MembersText"]);
                data.InternalStatus = j["InternalStatus"] == null ? "" : Convert.ToString(j["InternalStatus"]);
                data.MileStone = j["MileStone"] == null ? "" : Convert.ToString(j["MileStone"]);
                data.Description = j["Description"] == null ? "" : Convert.ToString(j["Description"]);
                data.StartDate = j["StartDate"] == null ? "" : Convert.ToString(j["StartDate"]);
                data.EndDate = j["EndDate"] == null ? "" : Convert.ToString(j["EndDate"]);
                data.NoOfDays = j["NoOfDays"] == null ? 0 : Convert.ToInt32(j["NoOfDays"]);
                data.StatusName = j["Status"]["StatusName"] == null ? "" : j["Status"]["StatusName"].ToString();
                data.ProjectName = j["Project"]["ProjectName"] == null ? "" : j["Project"]["ProjectName"].ToString();
                data.ProjectManagerName = j["ProjectManager"]["FirstName"] == null ? "" : j["ProjectManager"]["FirstName"].ToString() + " " + j["ProjectManager"]["LastName"].ToString();

                lstMilestone.Add(data);
            }

            return lstMilestone;
        }
        public string SaveMilestone(ClientContext clientContext, string ItemData)
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
            rESTOption.select = "ID,MileStone,Description,StartDate,EndDate,Status/StatusName,InternalStatus,NoOfDays,MembersText,Members/ID,Members/FirstName,Members/LastName,ProjectManager/FirstName,ProjectManager/LastName,ProjectManager/Id,Project/Id,Project/ProjectName";
            rESTOption.expand = "Project,Members,ProjectManager,Status";
            rESTOption.top = "5000";


            jArray = restService.GetAllItemFromList(clientContext, "TIM_MileStone", rESTOption);

            return jArray;
        }

        private string RESTSave(ClientContext clientContext, string ItemData)
        {
            RestService restService = new RestService();
            return restService.SaveItem(clientContext, "TIM_MileStone", ItemData);
        }

        public string UpdateMilestone(ClientContext clientContext, string ItemData, string ID)
        {

            string response = RESTUpdate(clientContext, ItemData, ID);

            return response;
        }

        private string RESTUpdate(ClientContext clientContext, string ItemData, string ID)
        {
            RestService restService = new RestService();

            return restService.UpdateItem(clientContext, "TIM_MileStone", ItemData, ID);
        }

    }
}