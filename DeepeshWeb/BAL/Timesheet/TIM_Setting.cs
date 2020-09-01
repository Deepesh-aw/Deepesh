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
    public class TIM_Setting
    {
        public List<TIM_SettingModel> GetWorkingHour(ClientContext clientContext)
        {
            List<TIM_SettingModel> lstSetting = new List<TIM_SettingModel>();
            JArray jArray = RESTGet(clientContext, null);
            lstSetting = BindList(jArray);
            return lstSetting;
        }

        public List<TIM_SettingModel> BindList(JArray jArray)
        {
            List<TIM_SettingModel> lstSetting = new List<TIM_SettingModel>();
            foreach (JObject j in jArray)
            {
                TIM_SettingModel data = new TIM_SettingModel();

                data.ID = j["ID"] == null ? 0 : Convert.ToInt32(j["ID"]);
                data.WorkingHour = j["WorkingHour"] == null ? "" : Convert.ToString(j["WorkingHour"]);
                data.TimesheetCount = j["TimesheetCount"] == null ? "" : Convert.ToString(j["TimesheetCount"]);
                data.MinTimesheetEditDays = j["MinTimesheetEditDays"] == null ? "" : Convert.ToString(j["MinTimesheetEditDays"]);
                lstSetting.Add(data);
            }

            return lstSetting;
        }

        private JArray RESTGet(ClientContext clientContext, string filter)
        {
            RestService restService = new RestService();
            JArray jArray = new JArray();
            RESTOption rESTOption = new RESTOption();

            rESTOption.filter = filter;
            rESTOption.select = "*";
            rESTOption.orderby = "ID desc";
            rESTOption.top = "5000";

            jArray = restService.GetAllItemFromList(clientContext, "TIM_Setting", rESTOption);

            return jArray;
        }

        private string RESTSave(ClientContext clientContext, string ItemData)
        {
            RestService restService = new RestService();
            return restService.SaveItem(clientContext, "TIM_Setting", ItemData);
        }

        public string UpdateProject(ClientContext clientContext, string ItemData, string ID)
        {

            string response = RESTUpdate(clientContext, ItemData, ID);

            return response;
        }

        private string RESTUpdate(ClientContext clientContext, string ItemData, string ID)
        {
            RestService restService = new RestService();

            return restService.UpdateItem(clientContext, "TIM_Setting", ItemData, ID);
        }
    }
}