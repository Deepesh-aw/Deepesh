using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DeepeshWeb.Models.Timesheet
{
    public class TIM_MilestoneModel
    {
        public int ID { get; set; }
        public string MileStone { get; set; }
        public string Description { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public int NoOfDays { get; set; }
        public string InternalStatus { get; set; }
        public int Project { get; set; }
        public string ProjectName { get; set; }
        public int Status { get; set; }
        public string StatusName { get; set; }
        public int[] Members { get; set; }
        public string MembersText { get; set; }
        public int ProjectManager { get; set; }
        public string ProjectManagerName { get; set; }
    }
}