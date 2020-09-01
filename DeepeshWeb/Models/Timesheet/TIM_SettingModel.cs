using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DeepeshWeb.Models.Timesheet
{
    public class TIM_SettingModel
    {
        public int ID { get; set; }
        public string WorkingHour { get; set; }
        public string TimesheetCount { get; set; }
        public string MinTimesheetEditDays { get; set; }

    }
}