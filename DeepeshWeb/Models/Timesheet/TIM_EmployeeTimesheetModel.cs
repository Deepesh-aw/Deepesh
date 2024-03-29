﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DeepeshWeb.Models.Timesheet
{
    public class TIM_EmployeeTimesheetModel
    {
        public int ID { get; set; }
        public int ParentID { get; set; }
        public string Description { get; set; }
        public string RejectDescription { get; set; }
        public string ApproveDate { get; set; }
        public string ApproveDescription { get; set; }
        public int Employee { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeeEmail { get; set; }
        public string EstimatedHours { get; set; }
        public string Hours { get; set; }
        public int Manager { get; set; }
        public string ManagerName { get; set; }
        public string ManagerEmail { get; set; }
        public int MileStone { get; set; }
        public string MileStoneName { get; set; }
        public int Project { get; set; }
        public string ProjectName { get; set; }
        public string RemainingHours { get; set; }
        public int Status { get; set; }
        public string StatusName { get; set; }
        public int Task { get; set; }
        public string TaskName { get; set; }
        public int SubTask { get; set; }
        public string SubTaskName { get; set; }
        public string TimesheetAddedDate { get; set; }
        public string UtilizedHours { get; set; }
        public int Client { get; set; }
        public string ClientName { get; set; }
        public string TimesheetID { get; set; }
        public string FromTime { get; set; }
        public string ToTime { get; set; }
        public int AllTaskStatus { get; set; }
        public string AllTaskStatusName { get; set; }
        public string AllTaskInternalStatus { get; set; }

        public string OtherClient { get; set; }
        public string OtherProject { get; set; }
        public string OtherMilestone { get; set; }
        public string OtherTask { get; set; }
        public string ModifyDate { get; set; }
        public string ModifyName { get; set; }
        public List<TimeDoc> ListTimeDoc { get; set; }
        public string FileCount { get; set; }
        public string Flag { get; set; }
        public int MinAlterDay { get; set; }
        public string AlterUtilizeHour { get; set; }

    }

    public class TimeDoc
    {
        public string Name { get; set; }
        public FileObj File { get; set; }
    }
    public class FileObj
    {
        public string name { get; set; }
        public  Int64 size { get; set; }
        public string type { get; set; }
        public string ByteArr { get; set; }
    }
}