using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DeepeshWeb.Models.Timesheet
{
    public class TIM_TaskModel
    {
        public int ID { get; set; }
        public string Task { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public int NoOfDays { get; set; }
        public string InternalStatus { get; set; }
        public int Project { get; set; }
        public string ProjectName { get; set; }
        public int MileStone { get; set; }
        public string MileStoneName { get; set; }
        public int Delegate { get; set; }
        public int Members { get; set; }
        public string MembersName { get; set; }
        public string MembersEmail { get; set; }
        public int TaskStatus { get; set; }
        public string TaskStatusName { get; set; }
        public int Status { get; set; }
        public string StatusName { get; set; }
        public string DescriptionText { get; set; }
        public int Client { get; set; }
        public string ClientName { get; set; }

    }
}