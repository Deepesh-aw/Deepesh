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
    public class TIM_SendEmailBal
    {
        public List<TIM_SendEmailModel> BindList(JArray jArray)
        {
            List<TIM_SendEmailModel> lstEmail = new List<TIM_SendEmailModel>();
            foreach (JObject j in jArray)
            {
                TIM_SendEmailModel data = new TIM_SendEmailModel();

                data.ID = j["ID"] == null ? 0 : Convert.ToInt32(j["ID"]);
                data.To = j["To"] == null ? "" : Convert.ToString(j["To"]);
                data.CC = j["CC"] == null ? "" : Convert.ToString(j["CC"]);
                data.Subject = j["Subject"] == null ? "" : Convert.ToString(j["Subject"]);
                data.Body = j["Body"] == null ? "" : Convert.ToString(j["Body"]);
                lstEmail.Add(data);
            }

            return lstEmail;
        }

        private JArray RESTGet(ClientContext clientContext, string filter)
        {
            RestService restService = new RestService();
            JArray jArray = new JArray();
            RESTOption rESTOption = new RESTOption();

            rESTOption.filter = filter;
            rESTOption.select = "*";
            //rESTOption.expand = "Employee,Manager,Editor,Status";
            rESTOption.orderby = "ID desc";
            rESTOption.top = "5000";


            jArray = restService.GetAllItemFromList(clientContext, "TIM_SendEmail", rESTOption);

            return jArray;
        }

        public string SaveMail(ClientContext clientContext, string ItemData)
        {
            string response = RESTSave(clientContext, ItemData);
            return response;
        }

        private string RESTSave(ClientContext clientContext, string ItemData)
        {
            RestService restService = new RestService();
            return restService.SaveItem(clientContext, "TIM_SendEmail", ItemData);
        }

        public string UpdateMail(ClientContext clientContext, string ItemData, string ID)
        {

            string response = RESTUpdate(clientContext, ItemData, ID);

            return response;
        }

        private string RESTUpdate(ClientContext clientContext, string ItemData, string ID)
        {
            RestService restService = new RestService();

            return restService.UpdateItem(clientContext, "TIM_SendEmail", ItemData, ID);
        }

        public string DeleteMail(ClientContext clientContext, string ID)
        {
            string response = RESTDelete(clientContext, ID);

            return response;
        }

        private string RESTDelete(ClientContext clientContext, string ID)
        {
            RestService restService = new RestService();

            return restService.DeleteItem(clientContext, "TIM_SendEmail", ID);
        }

    }
}