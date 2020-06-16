using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DeepeshWeb.Models.Timesheet
{
    public class Pagination
    {
        public int START { get; set; }
        public int LENGTH { get; set; }
        public string SORTCOLUMN { get; set; }
        public string SORTCOLUMNDIR { get; set; }
        public string SEARCHVALUE { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string OrderStatus { get; set; }

    }
}