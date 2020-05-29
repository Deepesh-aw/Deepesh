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
    public class WorkFlowBal
    {
        public List<WorkFlowModel> GetWorkFlowForProjectCreation(ClientContext clientContext)
        {
            List<WorkFlowModel> _WorkFlowModel = new List<WorkFlowModel>();

            string filter = "TransactionType eq 'ProjectCreation' and Action eq 'Forward'";

            JArray jArray = RESTGet(clientContext, filter);

            foreach (JObject j in jArray)
            {
                _WorkFlowModel.Add(new WorkFlowModel
                {
                    TransactionType = j["TransactionType"] == null ? "" : j["TransactionType"].ToString(),
                    FromStatus = j["FromStatus"]["StatusName"] == null ? "" : Convert.ToString(j["FromStatus"]["StatusName"].ToString()),
                    ToStatus = j["ToStatus"]["StatusName"] == null ? "" : Convert.ToString(j["ToStatus"]["StatusName"].ToString()),
                    ToStatusID = j["ToStatus"] == null ? "" : Convert.ToString(j["ToStatus"]["ID"].ToString()),
                    InternalStatus = j["InternalStatus"] == null ? "" : Convert.ToString(j["InternalStatus"].ToString())
                });
            }

            return _WorkFlowModel;
        }

        private JArray RESTGet(ClientContext clientContext, string filter)
        {
            RestService restService = new RestService();
            JArray jArray = new JArray();
            RESTOption rESTOption = new RESTOption();

            rESTOption.filter = filter;
            rESTOption.select = "ID,TransactionType,Action,FromStatus/StatusName,ToStatus/StatusName,ToStatus/ID,FromStatus/ID,InternalStatus";
            rESTOption.expand = "FromStatus,ToStatus";
            rESTOption.top = "5000";


            jArray = restService.GetAllItemFromList(clientContext, "TIM_WorkFlowMaster", rESTOption);

            return jArray;
        }
    }
}