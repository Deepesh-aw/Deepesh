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
    public class TIM_DocumentLibraryBal
    {
        public List<TIM_DocumentLibraryModel> GetDocumentByLookUpId(ClientContext clientContext, string id, string path)
        {
            List<TIM_DocumentLibraryModel> emp_Client_Documents = new List<TIM_DocumentLibraryModel>();
            var filter = "LID eq " + id;
            JArray jArray = RESTGet(clientContext, filter);
            if (jArray.Count() > 0)
            {
                foreach (JObject j in jArray)
                {
                    emp_Client_Documents.Add(new TIM_DocumentLibraryModel
                    {
                        ID = Convert.ToInt32(j["ID"]),
                        Name = path + j["File"]["ServerRelativeUrl"].ToString(),
                        LID = j["LID"].ToString(),
                        DocumentPath = j["DocumentPath"].ToString(),
                    });
                }
            }
            return emp_Client_Documents;
        }

        public List<TIM_DocumentLibraryModel> GetDocumentByTimesheetId(ClientContext clientContext, string TimesheetID, string path)
        {
            List<TIM_DocumentLibraryModel> emp_Client_Documents = new List<TIM_DocumentLibraryModel>();
            var filter = "TimesheetID eq '" +TimesheetID+"'";
            JArray jArray = RESTGet(clientContext, filter);
            if (jArray.Count() > 0)
            {
                foreach (JObject j in jArray)
                {
                    emp_Client_Documents.Add(new TIM_DocumentLibraryModel
                    {
                        ID = Convert.ToInt32(j["ID"]),
                        Name = path + j["File"]["ServerRelativeUrl"].ToString(),
                        LID = j["LID"].ToString(),
                        DocumentPath = j["DocumentPath"].ToString(),
                    });
                }
            }
            return emp_Client_Documents;
        }
        public string DeleteDocument(ClientContext client, string id)
        {
            RestService restService = new RestService();
            string delete = restService.DeleteItem(client, "TIM_DocumentLibrary", id);
            return delete;
        }
        private JArray RESTGet(ClientContext clientContext, string filter)
        {
            RestService restService = new RestService();
            JArray jArray = new JArray();
            RESTOption rESTOption = new RESTOption();
            rESTOption.filter = filter;
            rESTOption.select = "*,File";
            rESTOption.expand = "File";
            rESTOption.top = "5000";
            jArray = restService.GetAllItemFromList(clientContext, "TIM_DocumentLibrary", rESTOption);
            return jArray;
        }

    }
}