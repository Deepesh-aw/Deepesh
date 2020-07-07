using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DeepeshWeb.Models.Timesheet
{
    public class TIM_EmployeeTimesheetModel
    {
        public int ID { get; set; }
        public string Description { get; set; }
        public string ApproveDate { get; set; }
        public string ApproveDescription { get; set; }
        public int Employee { get; set; }
        public string EmployeeName { get; set; }
        public int EstimatedHours { get; set; }
        public int Hours { get; set; }
        public int Manager { get; set; }
        public string ManagerName { get; set; }
        public int MileStone { get; set; }
        public string MileStoneName { get; set; }
        public int Project { get; set; }
        public string ProjectName { get; set; }
        public int RemainingHours { get; set; }
        public int Status { get; set; }
        public string StatusName { get; set; }
        public int Task { get; set; }
        public string TaskName { get; set; }
        public int SubTask { get; set; }
        public string SubTaskName { get; set; }
        public string TimesheetAddedDate { get; set; }
        public int UtilizedHours { get; set; }
        public int Client { get; set; }
        public string TimesheetID { get; set; }


    }
}