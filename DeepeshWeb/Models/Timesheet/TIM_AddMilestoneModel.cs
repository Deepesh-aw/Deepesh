using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DeepeshWeb.Models.Timesheet
{
    public class TIM_AddMilestoneModel
    {
        public int ID { get; set; }
        public string Milestone { get; set; }
        public string Description { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string NoOfDays { get; set; }
        public string InternalStatus { get; set; }
        public int Project { get; set; }
        public int Status { get; set; }
        public int[] Members { get; set; }
        public string MembersText { get; set; }
        public int ProjectManager { get; set; }
    }
}