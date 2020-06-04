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
        public List<TIM_ProjectCreationModel> GetProjectCreationById(ClientContext clientContext, int Id = 2)
        {
            List<TIM_ProjectCreationModel> lstProjectCreation = new List<TIM_ProjectCreationModel>();
            string filter = "Id eq "+Id+"";
            JArray jArray = RESTGet(clientContext, filter);
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
                data.Id = j["Id"] == null ? 0 : Convert.ToInt32(j["Id"]);
                data.ProjectManager = j["ProjectManager"]["Id"] == null ? 0 : Convert.ToInt32(j["ProjectManager"]["Id"]);
                data.MembersText = j["MembersText"] == null ? "" : Convert.ToString(j["MembersText"]);
                data.ProjectName = j["ProjectName"] == null ? "" : Convert.ToString(j["ProjectName"]);
                data.ProjectTypeName = j["ProjectType"]["TypeName"] == null ? "" : Convert.ToString(j["ProjectType"]["TypeName"]);
                data.Client = j["ClientName"]["ClientName"] == null ? "" : Convert.ToString(j["ClientName"]["ClientName"].ToString());
                data.ProjectManagerName = j["ProjectManager"]["FirstName"] == null ? "" : j["ProjectManager"]["FirstName"].ToString()+" "+j["ProjectManager"]["LastName"].ToString();
                data.ClientProjectManager = j["ClientProjectManager"] == null ? "" : Convert.ToString(j["ClientProjectManager"]);
                data.StartDate = j["StartDate"] == null ? "" : Convert.ToString(j["StartDate"]);
                data.EndDate = j["EndDate"] == null ? "" : Convert.ToString(j["EndDate"]);
                data.NoOfDays = j["NoOfDays"] == null ? 0 : Convert.ToInt32(j["NoOfDays"]);
                data.Description = j["Description"] == null ? "" : Convert.ToString(j["Description"]);

                lstProjectCreation.Add(data);
            }
            return lstProjectCreation;
        }

        public string SaveProjectCreation(ClientContext clientContext, string ItemData)
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
            rESTOption.select = "ID,ClientProjectManager,ProjectName,StartDate,EndDate,Description,Status/StatusName,InternalStatus,NoOfDays,ProjectType/TypeName,ClientName/ClientName,MembersText,Members/ID,Members/FirstName,Members/LastName,ProjectManager/FirstName,ProjectManager/LastName,ProjectManager/Id";
            rESTOption.expand = "ProjectType,ClientName,Members,ProjectManager,Status";
            rESTOption.top = "5000";


            jArray = restService.GetAllItemFromList(clientContext, "TIM_ProjectCreation", rESTOption);

            return jArray;
        }

        private string RESTSave(ClientContext clientContext, string ItemData)
        {
            RestService restService = new RestService();
            return restService.SaveItem(clientContext, "TIM_ProjectCreation", ItemData);
        }


    }
}