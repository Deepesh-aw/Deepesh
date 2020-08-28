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
    public class TIM_TimesheetParentBal
    {
        public List<TIM_TimesheetParentModel> GetEmpTimesheetByTimesheetId(ClientContext clientContext, string TimesheetId)
        {
            List<TIM_TimesheetParentModel> lstTIM_TimesheetParent = new List<TIM_TimesheetParentModel>();
            string filter = "TimesheetID eq '" + TimesheetId + "'";
            JArray jArray = RESTGet(clientContext, filter);
            lstTIM_TimesheetParent = BindList(jArray);
            return lstTIM_TimesheetParent;
        }


        public List<TIM_TimesheetParentModel> BindList(JArray jArray)
        {
            List<TIM_TimesheetParentModel> lstEmployeeParentTimesheet = new List<TIM_TimesheetParentModel>();
            foreach (JObject j in jArray)
            {
                TIM_TimesheetParentModel data = new TIM_TimesheetParentModel();

                data.ID = j["ID"] == null ? 0 : Convert.ToInt32(j["ID"]);
                data.ApproveDescription = j["ApproveDescription"] == null ? "" : Convert.ToString(j["ApproveDescription"]);
                data.RejectDescription = j["RejectDescription"] == null ? "" : Convert.ToString(j["RejectDescription"]);
                data.InternalStatus = j["InternalStatus"] == null ? "" : Convert.ToString(j["InternalStatus"]);
                data.Employee = j["Employee"]["ID"] == null ? 0 : Convert.ToInt32(j["Employee"]["ID"]);
                data.EmployeeName = j["Employee"]["FirstName"] == null ? "" : j["Employee"]["FirstName"].ToString() + " " + j["Employee"]["LastName"].ToString();
                data.Manager = j["Manager"]["ID"] == null ? 0 : Convert.ToInt32(j["Manager"]["ID"]);
                data.ManagerName = j["Manager"]["FirstName"] == null ? "" : j["Manager"]["FirstName"].ToString() + " " + j["Manager"]["LastName"].ToString();
                data.Status = j["Status"]["ID"] == null ? 0 : Convert.ToInt32(j["Status"]["ID"]);
                data.StatusName = j["Status"]["StatusName"] == null ? "" : j["Status"]["StatusName"].ToString();

                DateTime TimesheetAdded = Convert.ToDateTime(j["TimesheetAddedDate"]);
                data.TimesheetAddedDate = TimesheetAdded.ToString("dd-MM-yyyy");

                data.ModifyName = j["Editor"]["Title"] == null ? "" : j["Editor"]["Title"].ToString();
                if (j["ApproveDate"].ToString() != "")
                {
                    DateTime TimesheetApproved = Convert.ToDateTime(j["ApproveDate"]);
                    data.ApproveDate = TimesheetApproved.ToString("dd-MM-yyyy");
                }

                if (j["RejectDate"].ToString() != "")
                {
                    DateTime TimesheetRejected = Convert.ToDateTime(j["RejectDate"]);
                    data.RejectDate = TimesheetRejected.ToString("dd-MM-yyyy");
                }


                DateTime TimesheetModify = Convert.ToDateTime(j["Modified"]);
                data.ModifyDate = TimesheetModify.ToString("dd-MM-yyyy");

                data.TimesheetID = j["TimesheetID"] == null ? "" : Convert.ToString(j["TimesheetID"]);


                lstEmployeeParentTimesheet.Add(data);
            }

            return lstEmployeeParentTimesheet;
        }

        private JArray RESTGet(ClientContext clientContext, string filter)
        {
            RestService restService = new RestService();
            JArray jArray = new JArray();
            RESTOption rESTOption = new RESTOption();

            rESTOption.filter = filter;
            rESTOption.select = "*,Employee/ID,Employee/FirstName,Employee/LastName,Manager/ID,Manager/FirstName,Manager/LastName,Status/StatusName,Status/ID,Editor/Title";
            rESTOption.expand = "Employee,Manager,Editor,Status";
            rESTOption.orderby = "ID desc";
            rESTOption.top = "5000";


            jArray = restService.GetAllItemFromList(clientContext, "TIM_TimesheetParent", rESTOption);

            return jArray;
        }

        public string SaveTimesheet(ClientContext clientContext, string ItemData)
        {
            string response = RESTSave(clientContext, ItemData);
            return response;
        }

        private string RESTSave(ClientContext clientContext, string ItemData)
        {
            RestService restService = new RestService();
            return restService.SaveItem(clientContext, "TIM_TimesheetParent", ItemData);
        }

        public string UpdateTimesheet(ClientContext clientContext, string ItemData, string ID)
        {

            string response = RESTUpdate(clientContext, ItemData, ID);

            return response;
        }

        private string RESTUpdate(ClientContext clientContext, string ItemData, string ID)
        {
            RestService restService = new RestService();

            return restService.UpdateItem(clientContext, "TIM_TimesheetParent", ItemData, ID);
        }

        public string DeleteTimesheet(ClientContext clientContext, string ID)
        {
            string response = RESTDelete(clientContext, ID);

            return response;
        }

        private string RESTDelete(ClientContext clientContext, string ID)
        {
            RestService restService = new RestService();

            return restService.DeleteItem(clientContext, "TIM_TimesheetParent", ID);
        }

    }
}