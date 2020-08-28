using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DeepeshWeb.Models.Timesheet
{
    public class TIM_TimesheetParentModel
    {
        public int ID { get; set; }
        public string RejectDescription { get; set; }
        public string ApproveDate { get; set; }
        public string ApproveDescription { get; set; }
        public int Employee { get; set; }
        public string EmployeeName { get; set; }
        public int Manager { get; set; }
        public string ManagerName { get; set; }
        public int Status { get; set; }
        public string StatusName { get; set; }
        public string TimesheetAddedDate { get; set; }
        public string UtilizedHours { get; set; }
        public string TimesheetID { get; set; }
        public string InternalStatus { get; set; }
        public string ModifyDate { get; set; }
        public string ModifyName { get; set; }
        public string RejectDate { get; set; }

    }
}