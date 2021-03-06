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
        public List<TIM_EmployeeTimesheetModel> AlterPrevEmpTimesheet(ClientContext clientContext, TIM_EmployeeTimesheetModel item)
        {
            DateTime dt = DateTime.ParseExact(item.TimesheetAddedDate, "MM-dd-yyyy hh:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
            string TimesheetDate = dt.ToString("yyyy-MM-dd");
            List<TIM_EmployeeTimesheetModel> lstTIM_EmployeeTimesheet = new List<TIM_EmployeeTimesheetModel>();
            string filter = "ClientId eq " + item.Client + " and ProjectId eq  " + item.Project + " and MileStoneId eq  " + item.MileStone + " and TaskId eq  " + item.Task + " and SubTaskId eq  " + item.SubTask + " and TimesheetAddedDate gt '" + TimesheetDate + "' and InternalStatus ne 'TimesheetDeleted'";
            JArray jArray = RESTGet(clientContext, filter);
            lstTIM_EmployeeTimesheet = BindList(jArray);
            return lstTIM_EmployeeTimesheet;
        }

        public List<TIM_EmployeeTimesheetModel> ValidatePrevEmpTimesheet(ClientContext clientContext, TIM_EmployeeTimesheetModel item)
        {
            List<TIM_EmployeeTimesheetModel> lstTIM_EmployeeTimesheet = new List<TIM_EmployeeTimesheetModel>();
            string filter = "ClientId eq " + item.Client + " and ProjectId eq  " + item.Project + " and MileStoneId eq  " + item.MileStone + " and TaskId eq  " + item.Task + " and SubTaskId eq  " + item.SubTask + " and ID lt " + item.ID + " and InternalStatus ne 'TimesheetDeleted'";
            JArray jArray = RESTGet(clientContext, filter);
            lstTIM_EmployeeTimesheet = BindList(jArray);
            return lstTIM_EmployeeTimesheet;
        }

        public List<TIM_EmployeeTimesheetModel> GetEmpTimesheetByTaskId(ClientContext clientContext, int TaskId)
        {
            List<TIM_EmployeeTimesheetModel> lstTIM_EmployeeTimesheet = new List<TIM_EmployeeTimesheetModel>();
            string filter = "TaskId eq " + TaskId + " and InternalStatus ne 'TimesheetDeleted'";
            JArray jArray = RESTGet(clientContext, filter);
            lstTIM_EmployeeTimesheet = BindList(jArray);
            return lstTIM_EmployeeTimesheet;
        }

        public List<TIM_EmployeeTimesheetModel> GetEmpTimesheetBySubTaskId(ClientContext clientContext, int SubTaskId)
        {
            List<TIM_EmployeeTimesheetModel> lstTIM_EmployeeTimesheet = new List<TIM_EmployeeTimesheetModel>();
            string filter = "SubTaskId eq " + SubTaskId + " and InternalStatus ne 'TimesheetDeleted'";
            JArray jArray = RESTGet(clientContext, filter);
            lstTIM_EmployeeTimesheet = BindList(jArray);
            return lstTIM_EmployeeTimesheet;
        }

        public List<TIM_EmployeeTimesheetModel> GetEmpTimesheetByManagerIdAndPending(ClientContext clientContext, int ManagerId)
        {
            List<TIM_EmployeeTimesheetModel> lstTIM_EmployeeTimesheet = new List<TIM_EmployeeTimesheetModel>();
            string filter = "ManagerId eq " + ManagerId + " and InternalStatus eq 'Inprogress'";
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

        public List<TIM_EmployeeTimesheetModel> GetEmpTimesheetByManagerIdAndReject(ClientContext clientContext, int ManagerId)
        {
            List<TIM_EmployeeTimesheetModel> lstTIM_EmployeeTimesheet = new List<TIM_EmployeeTimesheetModel>();
            string filter = "ManagerId eq " + ManagerId + " and InternalStatus eq 'Reject'";
            JArray jArray = RESTGet(clientContext, filter);
            lstTIM_EmployeeTimesheet = BindList(jArray);
            return lstTIM_EmployeeTimesheet;
        }

        public List<TIM_EmployeeTimesheetModel> GetEmpTimesheetByEmpIdAndPending(ClientContext clientContext, int EmpId)
        {
            List<TIM_EmployeeTimesheetModel> lstTIM_EmployeeTimesheet = new List<TIM_EmployeeTimesheetModel>();
            string filter = "EmployeeId eq " + EmpId + " and InternalStatus eq 'Inprogress'";
            JArray jArray = RESTGet(clientContext, filter);
            lstTIM_EmployeeTimesheet = BindList(jArray);
            return lstTIM_EmployeeTimesheet;
        }

        public List<TIM_EmployeeTimesheetModel> GetEmpTimesheetByEmpIdAndDateFilter(ClientContext clientContext, int EmpId, string From, string To)
        {
            List<TIM_EmployeeTimesheetModel> lstTIM_EmployeeTimesheet = new List<TIM_EmployeeTimesheetModel>();
            string filter = "EmployeeId eq " + EmpId + " and InternalStatus ne 'TimesheetDeleted' and (TimesheetAddedDate ge '" + From + "' and TimesheetAddedDate le '" + To + "')";
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

        public List<TIM_EmployeeTimesheetModel> GetEmpTimesheetByEmpIdAndRejected(ClientContext clientContext, int EmpId)
        {
            List<TIM_EmployeeTimesheetModel> lstTIM_EmployeeTimesheet = new List<TIM_EmployeeTimesheetModel>();
            string filter = "EmployeeId eq " + EmpId + " and InternalStatus eq 'Reject'";
            JArray jArray = RESTGet(clientContext, filter);
            lstTIM_EmployeeTimesheet = BindList(jArray);
            return lstTIM_EmployeeTimesheet;
        }

        public List<TIM_EmployeeTimesheetModel> GetEmpTimesheetByAllTaskId(ClientContext clientContext, int AllTaskId, string TimesheetAddedDate, string ParentID)
        {
            DateTime dt = DateTime.ParseExact(TimesheetAddedDate, "MM-dd-yyyy hh:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
            string TimesheetDate = dt.ToString("yyyy-MM-dd");
            string filter = string.Empty;
            if(ParentID == "0")
            {
                filter = "(TaskId eq " + AllTaskId + " or  SubTaskId eq " + AllTaskId + " ) and InternalStatus ne 'TimesheetDeleted'";
            }
            else
            {
                filter = "(TaskId eq " + AllTaskId + " or  SubTaskId eq " + AllTaskId + " ) and InternalStatus ne 'TimesheetDeleted' and TimesheetAddedDate gt '" + TimesheetDate + "' and ParentID lt "+ParentID+"";
            }

            List<TIM_EmployeeTimesheetModel> lstTIM_EmployeeTimesheet = new List<TIM_EmployeeTimesheetModel>();

            RestService restService = new RestService();
            JArray jArray = new JArray();
            RESTOption rESTOption = new RESTOption();

            rESTOption.filter = filter;
            rESTOption.select = "*,Employee/ID,Client/ClientName,Client/ID,Employee/FirstName,Employee/LastName,Employee/OfficeEmail,Manager/ID,Manager/FirstName,Manager/LastName,Manager/OfficeEmail,MileStone/ID,MileStone/MileStone,Project/Id,Project/ProjectName,Task/Id,Task/Task,SubTask/Id,SubTask/SubTask,Status/StatusName,Status/ID,FromTime,ToTime,AllTaskStatus/StatusName,AllTaskStatus/ID,OtherClient,OtherMilestone,OtherProject,OtherTask,ApproveDate,Modified,Editor/Title";
            rESTOption.expand = "Employee,Manager,MileStone,Project,Task,SubTask,Status,Client,AllTaskStatus,Editor";
            rESTOption.orderby = "ParentID desc";
            rESTOption.top = "5000";
            jArray = restService.GetAllItemFromList(clientContext, "TIM_EmployeeTimesheet", rESTOption);


            //JArray jArray = RESTGet(clientContext, filter);
            lstTIM_EmployeeTimesheet = BindList(jArray);
            return lstTIM_EmployeeTimesheet;
        }

        public List<TIM_EmployeeTimesheetModel> GetTIMForAlterExist(ClientContext clientContext, TIM_EmployeeTimesheetModel item)
        {
            string filter = "TaskId eq " + item.Task + " and  SubTaskId eq " + item.SubTask + "  and InternalStatus ne 'TimesheetDeleted' and ParentID gt " + item.ParentID + "";

            List<TIM_EmployeeTimesheetModel> lstTIM_EmployeeTimesheet = new List<TIM_EmployeeTimesheetModel>();
            JArray jArray = RESTGet(clientContext, filter);
            lstTIM_EmployeeTimesheet = BindList(jArray);
            return lstTIM_EmployeeTimesheet;
        }

        public List<TIM_EmployeeTimesheetModel> GetEmpTimesheetByTimesheetId(ClientContext clientContext, string TimesheetId)
        {
            List<TIM_EmployeeTimesheetModel> lstTIM_EmployeeTimesheet = new List<TIM_EmployeeTimesheetModel>();
            string filter = "TimesheetID eq '" + TimesheetId + "' and InternalStatus ne 'TimesheetDeleted'";
            JArray jArray = RESTGet(clientContext, filter);
            lstTIM_EmployeeTimesheet = BindList(jArray);
            return lstTIM_EmployeeTimesheet;
        }

        public List<TIM_EmployeeTimesheetModel> GetCompletedTimesheet(ClientContext clientContext, int MilestoneId)
        {
            List<TIM_EmployeeTimesheetModel> lstTIM_EmployeeTimesheet = new List<TIM_EmployeeTimesheetModel>();
            string filter = "MileStoneId eq '" + MilestoneId + "' and InternalStatus ne 'TimesheetDeleted' and AllTaskStatus/StatusName eq 'Completed'";
            JArray jArray = RESTGet(clientContext, filter);
            lstTIM_EmployeeTimesheet = BindList(jArray);
            return lstTIM_EmployeeTimesheet;
        }

        public List<TIM_EmployeeTimesheetModel> GetAllEmpTimesheet(ClientContext clientContext)
        {
            List<TIM_EmployeeTimesheetModel> lstTIM_EmployeeTimesheet = new List<TIM_EmployeeTimesheetModel>();
            string filter = "";
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
                data.ParentID = j["ParentIDId"] == null ? 0 : Convert.ToInt32(j["ParentIDId"]);
                data.ApproveDescription = j["ApproveDescription"] == null ? "" : Convert.ToString(j["ApproveDescription"]);
                data.RejectDescription = j["RejectDescription"] == null ? "" : Convert.ToString(j["RejectDescription"]);
                data.Description = j["Description"] == null ? "" : Convert.ToString(j["Description"]);
                data.Employee = j["Employee"]["ID"] == null ? 0 : Convert.ToInt32(j["Employee"]["ID"]);
                data.EmployeeName = j["Employee"]["FirstName"] == null ? "" : j["Employee"]["FirstName"].ToString() + " " + j["Employee"]["LastName"].ToString();
                data.EmployeeEmail = j["Employee"]["OfficeEmail"] == null ? "" : j["Employee"]["OfficeEmail"].ToString();
                data.EstimatedHours = j["EstimatedHours"] == null ? "" : Convert.ToString(j["EstimatedHours"]);
                data.Hours = j["Hours"] == null ? "" : Convert.ToString(j["Hours"]);
                data.Manager = j["Manager"]["ID"] == null ? 0 : Convert.ToInt32(j["Manager"]["ID"]);
                data.ManagerName = j["Manager"]["FirstName"] == null ? "" : j["Manager"]["FirstName"].ToString() + " " + j["Manager"]["LastName"].ToString();
                data.ManagerEmail = j["Manager"]["OfficeEmail"] == null ? "" : j["Manager"]["OfficeEmail"].ToString();
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

                DateTime TimesheetAdded = Convert.ToDateTime(j["TimesheetAddedDate"]);
                data.TimesheetAddedDate = TimesheetAdded.ToString("dd-MM-yyyy");

                data.ModifyName = j["Editor"]["Title"] == null ? "" : j["Editor"]["Title"].ToString();
                DateTime TimesheetModify = Convert.ToDateTime(j["Modified"]);
                data.ModifyDate = TimesheetModify.ToString("dd-MM-yyyy");

                if (j["ApproveDate"].ToString() != "") {
                    DateTime TimesheetApproved = Convert.ToDateTime(j["ApproveDate"]);
                    data.ApproveDate = TimesheetApproved.ToString("dd-MM-yyyy");
                }


                data.UtilizedHours = j["UtilizedHours"] == null ? "" : Convert.ToString(j["UtilizedHours"]);
                data.TimesheetID = j["TimesheetID"] == null ? "" : Convert.ToString(j["TimesheetID"]);
                data.ClientName = j["Client"]["ClientName"] == null ? "" : Convert.ToString(j["Client"]["ClientName"].ToString());
                data.Client = j["Client"]["ID"] == null ? 0 : Convert.ToInt32(j["Client"]["ID"].ToString());
                data.AllTaskStatusName = j["AllTaskStatus"]["StatusName"] == null ? "" : j["AllTaskStatus"]["StatusName"].ToString();
                data.AllTaskStatus = j["AllTaskStatus"]["ID"] == null ? 0 : Convert.ToInt32(j["AllTaskStatus"]["ID"]);

                DateTime dt = Convert.ToDateTime(j["FromTime"]);
                string frm = dt.ToString("dd-MM-yyyy hh:mm:ss tt");

                DateTime utcfrmdate = DateTime.ParseExact(frm, "dd-MM-yyyy hh:mm:ss tt", CultureInfo.InvariantCulture);
                DateTime frmDate = TimeZoneInfo.ConvertTimeFromUtc(utcfrmdate, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"));
                var frmDateFinal = frmDate.ToString("dd-MM-yyyy HH:mm:ss");

                DateTime dtTo = Convert.ToDateTime(j["ToTime"]);
                string to = dtTo.ToString("dd-MM-yyyy hh:mm:ss tt");

                DateTime utctodate = DateTime.ParseExact(to, "dd-MM-yyyy hh:mm:ss tt", CultureInfo.InvariantCulture);
                DateTime toDate = TimeZoneInfo.ConvertTimeFromUtc(utctodate, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"));
                var toDateFinal = toDate.ToString("dd-MM-yyyy HH:mm:ss");

                //DateTime utctodate = DateTime.ParseExact(Convert.ToString(j["ToTime"]), "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                //var toDate = TimeZoneInfo.ConvertTimeFromUtc(utctodate, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"));

                data.FromTime = frmDateFinal.ToString();
                data.ToTime = toDateFinal.ToString();

                //data.FromTime = j["FromTime"] == null ? "" : Convert.ToString(j["FromTime"]);
                //data.ToTime = j["ToTime"] == null ? "" : Convert.ToString(j["ToTime"]);

                data.OtherClient = j["OtherClient"] == null ? "" : Convert.ToString(j["OtherClient"]);
                data.OtherMilestone = j["OtherMilestone"] == null ? "" : Convert.ToString(j["OtherMilestone"]);
                data.OtherProject = j["OtherProject"] == null ? "" : Convert.ToString(j["OtherProject"]);
                data.OtherTask = j["OtherTask"] == null ? "" : Convert.ToString(j["OtherTask"]);

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
            rESTOption.select = "*,Employee/ID,Client/ClientName,Client/ID,Employee/FirstName,Employee/LastName,Employee/OfficeEmail,Manager/ID,Manager/FirstName,Manager/LastName,Manager/OfficeEmail,MileStone/ID,MileStone/MileStone,Project/Id,Project/ProjectName,Task/Id,Task/Task,SubTask/Id,SubTask/SubTask,Status/StatusName,Status/ID,FromTime,ToTime,AllTaskStatus/StatusName,AllTaskStatus/ID,OtherClient,OtherMilestone,OtherProject,OtherTask,ApproveDate,Modified,Editor/Title";
            rESTOption.expand = "Employee,Manager,MileStone,Project,Task,SubTask,Status,Client,AllTaskStatus,Editor";
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