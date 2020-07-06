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
    public class TIM_WorkingHoursBal
    {
        public List<TIM_WorkingHoursModel> GetWorkingHour(ClientContext clientContext)
        {
            List<TIM_WorkingHoursModel> TIM_WorkingHours = new List<TIM_WorkingHoursModel>();
            JArray jArray = RESTGet(clientContext, null);
            TIM_WorkingHours = BindList(jArray);
            return TIM_WorkingHours;
        }

        public List<TIM_WorkingHoursModel> BindList(JArray jArray)
        {
            List<TIM_WorkingHoursModel> TIM_WorkingHours = new List<TIM_WorkingHoursModel>();
            foreach (JObject j in jArray)
            {
                TIM_WorkingHoursModel data = new TIM_WorkingHoursModel();

                data.ID = j["ID"] == null ? 0 : Convert.ToInt32(j["ID"]);
                data.Hour = j["Hour"] == null ? 0 : Convert.ToInt32(j["Hour"]);

                TIM_WorkingHours.Add(data);
            }

            return TIM_WorkingHours;
        }

        private JArray RESTGet(ClientContext clientContext, string filter)
        {
            RestService restService = new RestService();
            JArray jArray = new JArray();
            RESTOption rESTOption = new RESTOption();

            rESTOption.filter = filter;
            rESTOption.select = "ID,Hour";
            rESTOption.orderby = "ID desc";
            rESTOption.top = "5000";

            jArray = restService.GetAllItemFromList(clientContext, "TIM_WorkingHours", rESTOption);

            return jArray;
        }

        private string RESTSave(ClientContext clientContext, string ItemData)
        {
            RestService restService = new RestService();
            return restService.SaveItem(clientContext, "TIM_WorkingHours", ItemData);
        }

        public string UpdateProject(ClientContext clientContext, string ItemData, string ID)
        {

            string response = RESTUpdate(clientContext, ItemData, ID);

            return response;
        }

        private string RESTUpdate(ClientContext clientContext, string ItemData, string ID)
        {
            RestService restService = new RestService();

            return restService.UpdateItem(clientContext, "TIM_WorkingHours", ItemData, ID);
        }

    }
}