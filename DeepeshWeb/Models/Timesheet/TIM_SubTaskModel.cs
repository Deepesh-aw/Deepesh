using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
namespace DeepeshWeb.BAL.Timesheet
{
    public class TIM_SubTaskModel
    {
        public int ID { get; set; }
        public string SubTask { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public int NoOfDays { get; set; }
        public string InternalStatus { get; set; }
        public int Task { get; set; }
        public string TaskName { get; set; }
        public int Project { get; set; }
        public string ProjectName { get; set; }
        public int MileStone { get; set; }
        public string MileStoneName { get; set; }
        public int Delegate { get; set; }
        public int Members { get; set; }
        public string MembersName { get; set; }
        public string MembersEmail { get; set; }

        public int SubTaskStatus { get; set; }
        public string SubTaskStatusName { get; set; }
        public int Status { get; set; }
        public string StatusName { get; set; }
        public int Client { get; set; }
        public string ClientName { get; set; }
    }
}