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
    public class ClientBal
    {
        private JArray RESTGet(ClientContext clientContext, string filter="")
        {
            RestService restService = new RestService();
            JArray jArray = new JArray();
            RESTOption rESTOption = new RESTOption();

            rESTOption.filter = filter;
            rESTOption.select = "ID,ClientName";
            //rESTOption.expand = "TypeName,ApproverName,Employee";
            rESTOption.top = "5000";

            jArray = restService.GetAllItemFromList(clientContext, "Emp_ClientMasterDetails", rESTOption);
            return jArray;
        }

        public List<ClientModel> GetClient(ClientContext clientContext)
        {
            List<ClientModel> lstClient = new List<ClientModel>();
            JArray jArray = RESTGet(clientContext);
            foreach (JObject j in jArray)
            {
                lstClient.Add(new ClientModel
                {
                    Id = Convert.ToInt32(j["Id"]),
                    ClientName = j["ClientName"].ToString(),
                }); ;
            }
            return lstClient;
        }
    }
}