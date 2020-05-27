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
    public class EmpBal
    {
        private JArray RESTGet(ClientContext clientContext, string filter = "")
        {
            RestService restService = new RestService();
            JArray jArray = new JArray();
            RESTOption rESTOption = new RESTOption();

            rESTOption.filter = filter;
            rESTOption.select = "ID, FirstName, LastName";
            //rESTOption.expand = "TypeName,ApproverName,Employee";
            rESTOption.top = "5000";

            jArray = restService.GetAllItemFromList(clientContext, "Emp_BasicInfo", rESTOption);
            return jArray;
        }

        public List<EmpModel> GetEmp(ClientContext clientContext)
        {
            List<EmpModel> lstEmp = new List<EmpModel>();
            JArray jArray = RESTGet(clientContext);
            foreach (JObject j in jArray)
            {
                lstEmp.Add(new EmpModel
                {
                    Id = Convert.ToInt32(j["Id"]),
                    FullName = j["FirstName"].ToString()+ j["LastName"].ToString(),
                }); ;
            }
            return lstEmp;
        }
    }
}