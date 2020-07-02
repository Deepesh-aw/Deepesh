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
    public class TIM_StatusMasterBal
    {
        private JArray RESTGet(ClientContext clientContext, string filter="")
        {
            RestService restService = new RestService();
            JArray jArray = new JArray();
            RESTOption rESTOption = new RESTOption();
            rESTOption.filter = filter;
            rESTOption.select = "ID,StatusName";
            rESTOption.top = "5000";

            jArray = restService.GetAllItemFromList(clientContext, "TIM_StatusMaster", rESTOption);
            return jArray;
        }
        public List<TIM_StatusMasterModel> GetStatusForAction(ClientContext clientContext)
        {
            List<TIM_StatusMasterModel> lstStatus = new List<TIM_StatusMasterModel>();
            string filter = "(StatusName eq 'Pending') or (StatusName eq 'Approved')";
            JArray jArray = RESTGet(clientContext, filter);
            foreach (JObject j in jArray)
            {
                lstStatus.Add(new TIM_StatusMasterModel
                {
                    ID = Convert.ToInt32(j["Id"]),
                    StatusName = j["StatusName"].ToString(),
                }); ;
            }
            return lstStatus;
        }

        public List<TIM_StatusMasterModel> GetPendingStatus(ClientContext clientContext)
        {
            List<TIM_StatusMasterModel> lstStatus = new List<TIM_StatusMasterModel>();
            string filter = "(StatusName eq 'Pending')";
            JArray jArray = RESTGet(clientContext, filter);
            foreach (JObject j in jArray)
            {
                lstStatus.Add(new TIM_StatusMasterModel
                {
                    ID = Convert.ToInt32(j["Id"]),
                    StatusName = j["StatusName"].ToString(),
                }); ;
            }
            return lstStatus;
        }

    }
}