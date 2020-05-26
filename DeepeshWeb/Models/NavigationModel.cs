using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DeepeshWeb.Models
{
    public class NavigationModel
    {
        public int ID { get; set; }
        public string ManuName { get; set; }
        public string URL { get; set; }
        public string ParentManu { get; set; }
        public string Show { get; set; }
        public string ParentMenuName { get; set; }
        public string ParentMenuNameID { get; set; }
        public string OrderNo { get; set; }
    }
}