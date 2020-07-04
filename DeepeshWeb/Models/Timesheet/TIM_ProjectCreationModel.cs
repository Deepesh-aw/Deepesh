using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DeepeshWeb.Models.Timesheet
{
    public class TIM_ProjectCreationModel
    {
        public int ID { get; set; }
        public string ProjectName { get; set; }
        public int ClientName { get; set; }
        public string Client { get; set; }
        public string ClientProjectManager { get; set; }
        public string Description { get; set; }
        public string EndDate { get; set; }
        public string InternalStatus { get; set; }
        public Int32[] Members { get; set; }
        public string MembersText { get; set; }
        public string MembersCodeText { get; set; }
        public int NoOfDays { get; set; }
        public int ProjectManager { get; set; }
        public string ProjectManagerName { get; set; }
        public string StartDate { get; set; }
        public Nullable<System.DateTime> Modified { get; set; }
        public int ProjectType { get; set; }
        public string ProjectTypeName { get; set; }
        public int Status { get; set; }
    }

    public class EqualityComparer : IEqualityComparer<TIM_ProjectCreationModel>
    {
        public bool Equals(TIM_ProjectCreationModel x, TIM_ProjectCreationModel y)
        {
            return x.ClientName.Equals(y.ClientName);
        }

        public int GetHashCode(TIM_ProjectCreationModel obj)
        {
            return obj.ClientName.GetHashCode();
        }
    }
}