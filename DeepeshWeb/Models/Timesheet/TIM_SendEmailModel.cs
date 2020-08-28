using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DeepeshWeb.Models.Timesheet
{
    public class TIM_SendEmailModel
    {
        public int ID { get; set; }
        public string To { get; set; }
        public string CC { get; set; }
        public string Body { get; set; }
        public string Subject { get; set; }
    }
}