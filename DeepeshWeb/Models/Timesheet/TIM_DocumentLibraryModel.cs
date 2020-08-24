using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DeepeshWeb.Models.Timesheet
{
    public class TIM_DocumentLibraryModel
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string LID { get; set; }
        public string DocumentPath { get; set; }
    }
}