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
    public class ProjectCreationBal
    {
        private JArray RESTGet(ClientContext clientContext, string filter)
        {
            RestService restService = new RestService();
            JArray jArray = new JArray();
            RESTOption rESTOption = new RESTOption();

            rESTOption.filter = filter;
            rESTOption.select = "ID,Employee/Title,Employee/ID,ApproverName/Title,ApproverName/ID,TypeName/TypeName";
            rESTOption.expand = "TypeName,ApproverName,Employee";
            rESTOption.top = "5000";


            jArray = restService.GetAllItemFromList(clientContext, "ApproverName", rESTOption);

            return jArray;
        }
    }
}