using DeepeshWeb.DAL;
using DeepeshWeb.Models;
using DeepeshWeb.Models.Timesheet;
using Microsoft.SharePoint.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DeepeshWeb.BAL.Timesheet
{
    public class TIM_ProjectCreationBal
    {
        public List<TIM_ProjectCreationModel> GetProjectCreationById(ClientContext clientContext, int ProjectId = 2)
        {
            List<TIM_ProjectCreationModel> lstProjectCreation = new List<TIM_ProjectCreationModel>();
            string filter = "ID eq "+ ProjectId + " and InternalStatus ne 'ProjectDeleted'";
            JArray jArray = RESTGet(clientContext, filter);
            lstProjectCreation = GetProjectCreationListItems(jArray);
            return lstProjectCreation;
        }

        public List<TIM_ProjectCreationModel> GetProjectCreationAllItems(ClientContext clientContext, int LogInId, string LogInCode)
        {
            List<TIM_ProjectCreationModel> lstProjectCreation = new List<TIM_ProjectCreationModel>();
            string filter = "(InternalStatus ne 'ProjectDeleted') and (ProjectManagerId eq "+ LogInId + " or substringof('"+ LogInCode + "',MembersCodeText))";
            JArray jArray = RESTGet(clientContext, filter);
            lstProjectCreation = GetProjectCreationListItems(jArray);
            return lstProjectCreation;
        }

        public string SaveProjectCreation(ClientContext clientContext, string ItemData)
        {
            string response = RESTSave(clientContext, ItemData);
            return response;
        }

        public List<TIM_ProjectCreationModel> GetProjectCreationListItems(JArray jArray)
        {
            List<TIM_ProjectCreationModel> lstProjectCreation = new List<TIM_ProjectCreationModel>();
            foreach (JObject j in jArray)
            {
                TIM_ProjectCreationModel data = new TIM_ProjectCreationModel();
                int i = 0;
                data.Members = new Int32[((Newtonsoft.Json.Linq.JContainer)j["Members"]["results"]).Count];
                foreach (var item in j["Members"]["results"])
                {
                    data.Members[i] = Convert.ToInt32(item["ID"]);
                    i++;
                }
                data.ID = j["ID"] == null ? 0 : Convert.ToInt32(j["ID"]);
                data.ProjectManager = j["ProjectManager"]["Id"] == null ? 0 : Convert.ToInt32(j["ProjectManager"]["Id"]);
                data.MembersText = j["MembersText"] == null ? "" : Convert.ToString(j["MembersText"]);
                data.ProjectName = j["ProjectName"] == null ? "" : Convert.ToString(j["ProjectName"]);
                data.ProjectTypeName = j["ProjectType"]["TypeName"] == null ? "" : Convert.ToString(j["ProjectType"]["TypeName"]);
                data.Client = j["ClientName"]["ClientName"] == null ? "" : Convert.ToString(j["ClientName"]["ClientName"].ToString());
                data.ClientName = j["ClientName"]["ID"] == null ? 0 : Convert.ToInt32(j["ClientName"]["ID"].ToString());
                data.Status = j["Status"]["ID"] == null ? 0 : Convert.ToInt32(j["Status"]["ID"].ToString());
                data.ProjectType = j["ProjectType"]["ID"] == null ? 0 : Convert.ToInt32(j["ProjectType"]["ID"].ToString());
                data.ProjectManagerName = j["ProjectManager"]["FirstName"] == null ? "" : j["ProjectManager"]["FirstName"].ToString() + " " + j["ProjectManager"]["LastName"].ToString();
                data.ClientProjectManager = j["ClientProjectManager"] == null ? "" : Convert.ToString(j["ClientProjectManager"]);
                data.StartDate = j["StartDate"] == null ? "" : Convert.ToString(j["StartDate"]);
                data.Modified = j["Modified"] == null ? (DateTime?)null : Convert.ToDateTime(j["Modified"]);
                data.EndDate = j["EndDate"] == null ? "" : Convert.ToString(j["EndDate"]);
                data.NoOfDays = j["NoOfDays"] == null ? 0 : Convert.ToInt32(j["NoOfDays"]);
                data.Description = j["Description"] == null ? "" : Convert.ToString(j["Description"]);
                data.InternalStatus = j["InternalStatus"] == null ? "" : Convert.ToString(j["InternalStatus"]);
                lstProjectCreation.Add(data);
            }
            return lstProjectCreation;

        }
 
        private JArray RESTGet(ClientContext clientContext, string filter)
        {
            RestService restService = new RestService();
            JArray jArray = new JArray();
            RESTOption rESTOption = new RESTOption();

            rESTOption.filter = filter;
            rESTOption.select = "ID,ClientProjectManager,Modified,ProjectName,StartDate,EndDate,Description,Status/StatusName,Status/ID,InternalStatus,NoOfDays,ProjectType/TypeName,ProjectType/ID,ClientName/ClientName,ClientName/ID,MembersText,Members/ID,Members/FirstName,Members/LastName,ProjectManager/FirstName,ProjectManager/LastName,ProjectManager/Id";
            rESTOption.expand = "ProjectType,ClientName,Members,ProjectManager,Status";
            rESTOption.orderby = "ID desc";
            rESTOption.top = "5000";


            jArray = restService.GetAllItemFromList(clientContext, "TIM_ProjectCreation", rESTOption);

            return jArray;
        }

        private string RESTSave(ClientContext clientContext, string ItemData)
        {
            RestService restService = new RestService();
            return restService.SaveItem(clientContext, "TIM_ProjectCreation", ItemData);
        }

        public string UpdateProject(ClientContext clientContext, string ItemData, string ID)
        {

            string response = RESTUpdate(clientContext, ItemData, ID);

            return response;
        }

        private string RESTUpdate(ClientContext clientContext, string ItemData, string ID)
        {
            RestService restService = new RestService();

            return restService.UpdateItem(clientContext, "TIM_ProjectCreation", ItemData, ID);
        }
    }
}