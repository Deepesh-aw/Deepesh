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
    public class TIM_TaskBal
    {
        public string SaveTask(ClientContext clientContext, string ItemData)
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
            rESTOption.select = "ID,Task,StartDate,EndDate,Status/StatusName,InternalStatus,NoOfDays,Members/ID,Members/FirstName,Members/LastName,Project/Id,Project/ProjectName,MileStone/ID,MileStone/MileStone";
            rESTOption.expand = "Project,Members,ProjectManager,Status";
            rESTOption.top = "5000";


            jArray = restService.GetAllItemFromList(clientContext, "TIM_Task", rESTOption);

            return jArray;
        }

        private string RESTSave(ClientContext clientContext, string ItemData)
        {
            RestService restService = new RestService();
            return restService.SaveItem(clientContext, "TIM_Task", ItemData);
        }
    }
}