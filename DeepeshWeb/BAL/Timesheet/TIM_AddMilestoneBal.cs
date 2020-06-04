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
    public class TIM_AddMilestoneBal
    {
        public string SaveMilestone(ClientContext clientContext, string ItemData)
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


            jArray = restService.GetAllItemFromList(clientContext, "TIM_MileStone", rESTOption);

            return jArray;
        }

        private string RESTSave(ClientContext clientContext, string ItemData)
        {
            RestService restService = new RestService();
            return restService.SaveItem(clientContext, "TIM_MileStone", ItemData);
        }


    }
}