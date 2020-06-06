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
    public class TIM_ProjectTypeMasterBal
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
        public List<TIM_ProjectTypeMasterModel> GetProjectType(ClientContext clientContext)
        {
            List<TIM_ProjectTypeMasterModel> lstProjectType = new List<TIM_ProjectTypeMasterModel>();
            JArray jArray = RESTGet(clientContext);
            foreach (JObject j in jArray)
            {
                lstProjectType.Add(new TIM_ProjectTypeMasterModel
                {
                    ID = Convert.ToInt32(j["ID"]),
                    TypeName = j["TypeName"].ToString(),
                }); ;
            }
            return lstProjectType;
        }
    }
}