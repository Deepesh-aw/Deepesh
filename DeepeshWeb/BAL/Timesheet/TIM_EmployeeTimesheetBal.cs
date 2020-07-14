﻿using DeepeshWeb.DAL;
using DeepeshWeb.Models;
using DeepeshWeb.Models.Timesheet;
using Microsoft.SharePoint.Client;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace DeepeshWeb.BAL.Timesheet
{
    public class TIM_EmployeeTimesheetBal
    {
        public List<TIM_EmployeeTimesheetModel> GetEmpTimesheetByTaskId(ClientContext clientContext, int TaskId)
        {
            List<TIM_EmployeeTimesheetModel> lstTIM_EmployeeTimesheet = new List<TIM_EmployeeTimesheetModel>();
            string filter = "TaskId eq " + TaskId + "";
            JArray jArray = RESTGet(clientContext, filter);
            lstTIM_EmployeeTimesheet = BindList(jArray);
            return lstTIM_EmployeeTimesheet;
        }

        public List<TIM_EmployeeTimesheetModel> GetEmpTimesheetBySubTaskId(ClientContext clientContext, int SubTaskId)
        {
            List<TIM_EmployeeTimesheetModel> lstTIM_EmployeeTimesheet = new List<TIM_EmployeeTimesheetModel>();
            string filter = "SubTaskId eq " + SubTaskId + "";
            JArray jArray = RESTGet(clientContext, filter);
            lstTIM_EmployeeTimesheet = BindList(jArray);
            return lstTIM_EmployeeTimesheet;
        }

        public List<TIM_EmployeeTimesheetModel> GetEmpTimesheetByManagerIdAndPending(ClientContext clientContext, int ManagerId)
        {
            List<TIM_EmployeeTimesheetModel> lstTIM_EmployeeTimesheet = new List<TIM_EmployeeTimesheetModel>();
            string filter = "ManagerId eq " + ManagerId + " and InternalStatus eq 'Pending'";
            JArray jArray = RESTGet(clientContext, filter);
            lstTIM_EmployeeTimesheet = BindList(jArray);
            return lstTIM_EmployeeTimesheet;
        }

        public List<TIM_EmployeeTimesheetModel> GetEmpTimesheetByManagerIdAndApprove(ClientContext clientContext, int ManagerId)
        {
            List<TIM_EmployeeTimesheetModel> lstTIM_EmployeeTimesheet = new List<TIM_EmployeeTimesheetModel>();
            string filter = "ManagerId eq " + ManagerId + " and InternalStatus eq 'Approved'";
            JArray jArray = RESTGet(clientContext, filter);
            lstTIM_EmployeeTimesheet = BindList(jArray);
            return lstTIM_EmployeeTimesheet;
        }

        public List<TIM_EmployeeTimesheetModel> GetEmpTimesheetByEmpIdAndPending(ClientContext clientContext, int EmpId)
        {
            List<TIM_EmployeeTimesheetModel> lstTIM_EmployeeTimesheet = new List<TIM_EmployeeTimesheetModel>();
            string filter = "EmployeeId eq " + EmpId + " and InternalStatus eq 'Pending'";
            JArray jArray = RESTGet(clientContext, filter);
            lstTIM_EmployeeTimesheet = BindList(jArray);
            return lstTIM_EmployeeTimesheet;
        }

        public List<TIM_EmployeeTimesheetModel> GetEmpTimesheetByEmpIdAndApprove(ClientContext clientContext, int EmpId)
        {
            List<TIM_EmployeeTimesheetModel> lstTIM_EmployeeTimesheet = new List<TIM_EmployeeTimesheetModel>();
            string filter = "EmployeeId eq " + EmpId + " and InternalStatus eq 'Approved'";
            JArray jArray = RESTGet(clientContext, filter);
            lstTIM_EmployeeTimesheet = BindList(jArray);
            return lstTIM_EmployeeTimesheet;
        }

        public List<TIM_EmployeeTimesheetModel> GetEmpTimesheetByAllTaskId(ClientContext clientContext, int AllTaskId)
        {
            List<TIM_EmployeeTimesheetModel> lstTIM_EmployeeTimesheet = new List<TIM_EmployeeTimesheetModel>();
            string filter = "(TaskId eq " + AllTaskId + " or  SubTaskId eq " + AllTaskId + " ) and  InternalStatus ne 'Approved'";
            JArray jArray = RESTGet(clientContext, filter);
            lstTIM_EmployeeTimesheet = BindList(jArray);
            return lstTIM_EmployeeTimesheet;
        }

        public List<TIM_EmployeeTimesheetModel> GetEmpTimesheetByTimesheetId(ClientContext clientContext, string TimesheetId)
        {
            List<TIM_EmployeeTimesheetModel> lstTIM_EmployeeTimesheet = new List<TIM_EmployeeTimesheetModel>();
            string filter = "TimesheetID eq '" + TimesheetId + "'";
            JArray jArray = RESTGet(clientContext, filter);
            lstTIM_EmployeeTimesheet = BindList(jArray);
            return lstTIM_EmployeeTimesheet;
        }

        public List<TIM_EmployeeTimesheetModel> BindList(JArray jArray)
        {
            List<TIM_EmployeeTimesheetModel> lstEmployeeTimesheet = new List<TIM_EmployeeTimesheetModel>();
            foreach (JObject j in jArray)
            {
                TIM_EmployeeTimesheetModel data = new TIM_EmployeeTimesheetModel();

                data.ID = j["ID"] == null ? 0 : Convert.ToInt32(j["ID"]);
                data.ApproveDate = j["ApproveDate"] == null ? "" : Convert.ToString(j["ApproveDate"]);
                data.ApproveDescription = j["ApproveDescription"] == null ? "" : Convert.ToString(j["ApproveDescription"]);
                data.Description = j["Description"] == null ? "" : Convert.ToString(j["Description"]);
                data.Employee = j["Employee"]["ID"] == null ? 0 : Convert.ToInt32(j["Employee"]["ID"]);
                data.EmployeeName = j["Employee"]["FirstName"] == null ? "" : j["Employee"]["FirstName"].ToString() + " " + j["Employee"]["LastName"].ToString();
                data.EstimatedHours = j["EstimatedHours"] == null ? "" : Convert.ToString(j["EstimatedHours"]);
                data.Hours = j["Hours"] == null ? "" : Convert.ToString(j["Hours"]);
                data.Manager = j["Manager"]["ID"] == null ? 0 : Convert.ToInt32(j["Manager"]["ID"]);
                data.ManagerName = j["Manager"]["FirstName"] == null ? "" : j["Manager"]["FirstName"].ToString() + " " + j["Manager"]["LastName"].ToString();
                data.MileStone = j["MileStone"]["ID"] == null ? 0 : Convert.ToInt32(j["MileStone"]["ID"]);
                data.MileStoneName = j["MileStone"]["MileStone"] == null ? "" : Convert.ToString(j["MileStone"]["MileStone"]);
                data.Project = j["Project"]["Id"] == null ? 0 : Convert.ToInt32(j["Project"]["Id"]);
                data.ProjectName = j["Project"]["ProjectName"] == null ? "" : j["Project"]["ProjectName"].ToString();
                data.RemainingHours = j["RemainingHours"] == null ? "" : Convert.ToString(j["RemainingHours"]);
                data.Status = j["Status"]["ID"] == null ? 0 : Convert.ToInt32(j["Status"]["ID"]);
                data.StatusName = j["Status"]["StatusName"] == null ? "" : j["Status"]["StatusName"].ToString();
                data.SubTask = j["SubTask"]["Id"] == null ? 0 : Convert.ToInt32(j["SubTask"]["Id"]);
                data.SubTaskName = j["SubTask"]["SubTask"] == null ? "" : Convert.ToString(j["SubTask"]["SubTask"]);
                data.Task = j["Task"]["Id"] == null ? 0 : Convert.ToInt32(j["Task"]["Id"]);
                data.TaskName = j["Task"]["Task"] == null ? "" : j["Task"]["Task"].ToString();
                data.TimesheetAddedDate = j["TimesheetAddedDate"] == null ? "" : Convert.ToString(j["TimesheetAddedDate"]);
                data.UtilizedHours = j["UtilizedHours"] == null ? "" : Convert.ToString(j["UtilizedHours"]);
                data.TimesheetID = j["TimesheetID"] == null ? "" : Convert.ToString(j["TimesheetID"]);
                data.ClientName = j["Client"]["ClientName"] == null ? "" : Convert.ToString(j["Client"]["ClientName"].ToString());
                data.Client = j["Client"]["ID"] == null ? 0 : Convert.ToInt32(j["Client"]["ID"].ToString());


                DateTime utcfrmdate = DateTime.ParseExact(Convert.ToString(j["FromTime"]), "dd-MM-yyyy hh:mm:ss", CultureInfo.InvariantCulture);
                var frmDate = TimeZoneInfo.ConvertTimeFromUtc(utcfrmdate, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"));

                DateTime utctodate = DateTime.ParseExact(Convert.ToString(j["ToTime"]), "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                var toDate = TimeZoneInfo.ConvertTimeFromUtc(utctodate, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"));

                data.FromTime = frmDate.ToString();
                data.ToTime = toDate.ToString();

                //data.FromTime = j["FromTime"] == null ? "" : Convert.ToString(j["FromTime"]);
                //data.ToTime = j["ToTime"] == null ? "" : Convert.ToString(j["ToTime"]);

                lstEmployeeTimesheet.Add(data);
            }

            return lstEmployeeTimesheet;
        }

        private JArray RESTGet(ClientContext clientContext, string filter)
        {
            RestService restService = new RestService();
            JArray jArray = new JArray();
            RESTOption rESTOption = new RESTOption();

            rESTOption.filter = filter;
            rESTOption.select = "*,Employee/ID,Client/ClientName,Client/ID,Employee/FirstName,Employee/LastName,Manager/ID,Manager/FirstName,Manager/LastName,MileStone/ID,MileStone/MileStone,Project/Id,Project/ProjectName,Task/Id,Task/Task,SubTask/Id,SubTask/SubTask,Status/StatusName,Status/ID,FromTime,ToTime";
            rESTOption.expand = "Employee,Manager,MileStone,Project,Task,SubTask,Status,Client";
            rESTOption.orderby = "ID desc";
            rESTOption.top = "5000";


            jArray = restService.GetAllItemFromList(clientContext, "TIM_EmployeeTimesheet", rESTOption);

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
            return restService.SaveItem(clientContext, "TIM_EmployeeTimesheet", ItemData);
        }

        public string UpdateTimesheet(ClientContext clientContext, string ItemData, string ID)
        {

            string response = RESTUpdate(clientContext, ItemData, ID);

            return response;
        }

        private string RESTUpdate(ClientContext clientContext, string ItemData, string ID)
        {
            RestService restService = new RestService();

            return restService.UpdateItem(clientContext, "TIM_EmployeeTimesheet", ItemData, ID);
        }

        public string DeleteTimesheet(ClientContext clientContext, string ID)
        {
            string response = RESTDelete(clientContext, ID);

            return response;
        }

        private string RESTDelete(ClientContext clientContext, string ID)
        {
            RestService restService = new RestService();

            return restService.DeleteItem(clientContext, "TIM_EmployeeTimesheet", ID);
        }

    }
}