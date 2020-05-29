using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DeepeshWeb.Models.Timesheet
{
    public class WorkFlowModel
    {
        public string TransactionType { get; set; }
        public string FromStatusID { get; set; }
        public string FromStatus { get; set; }
        public string ToStatusID { get; set; }
        public string ToStatus { get; set; }
        public string InternalStatus { get; set; }
        public string Action { get; set; }
        public string Remark { get; set; }

    }
}