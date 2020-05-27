using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DeepeshWeb.Models.Timesheet
{
    public class ProjectCreationModel
    {
        public int Id { get; set; }
        public string ProjectName { get; set; }
        public int ClientName { get; set; }
        public string ClientProjectManager { get; set; }
        public string Description { get; set; }
        public DateTime EndDate { get; set; }
        public string InternalStatus { get; set; }
        public int Members { get; set; }
        public string MembersText { get; set; }
        public int NoOfDays { get; set; }
        public int ProjectManager { get; set; }
        public DateTime StartDate { get; set; }
        public int ProjectType { get; set; }
        public int Status { get; set; }
    }
}