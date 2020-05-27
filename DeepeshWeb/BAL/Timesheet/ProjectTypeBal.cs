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
    public class ProjectTypeBal
    {
        private JArray RESTGet(ClientContext clientContext)
        {
            RestService restService = new RestService();
            JArray jArray = new JArray();
            RESTOption rESTOption = new RESTOption();

            rESTOption.select = "ID,TypeName";
            rESTOption.top = "5000";

            jArray = restService.GetAllItemFromList(clientContext, "TIM_ProjectTypeMaster", rESTOption);
            return jArray;
        }
        public List<ProjectTypeModel> GetProjectType(ClientContext clientContext)
        {
            List<ProjectTypeModel> lstProjectType = new List<ProjectTypeModel>();
            JArray jArray = RESTGet(clientContext);
            foreach (JObject j in jArray)
            {
                lstProjectType.Add(new ProjectTypeModel
                {
                    Id = Convert.ToInt32(j["Id"]),
                    TypeName = j["TypeName"].ToString(),
                }); ;
            }
            return lstProjectType;
        }
    }
}