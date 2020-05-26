using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DeepeshWeb.Models
{
    public class RESTOption
    {
        public string select { get; set; }
        public string expand { get; set; }
        public string filter { get; set; }
        public string orderby { get; set; }
        public string top { get; set; }
    }
}