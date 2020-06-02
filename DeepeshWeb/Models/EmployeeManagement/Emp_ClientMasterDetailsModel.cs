using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DeepeshWeb.Models.EmployeeManagement
{
    public class Emp_ClientMasterDetailsModel
    {
        public int ID { get; set; }

        [DisplayName("Client Name")]
        public string ClientName { get; set; }

        [DisplayName("Address")]
        public string ClientAddress { get; set; }

        [DisplayName("Contact")]
        public string ClientContact { get; set; }

        [DisplayName("E-Mail ID")]
        public string ClientMailID { get; set; }


        [DisplayName("Designation")]
        public string ClientDesignation { get; set; }


        [DisplayName("GST No")]
        public string ClientGSTNO { get; set; }

        [DisplayName("PAN Card No")]
        public string ClientPanCardNo { get; set; }

        [DisplayName("State")]
        public string ClientState { get; set; }


        [DisplayName("City")]
        public string ClientCity { get; set; }


        [DisplayName("Remark")]
        public string ClientRemark { get; set; }


        [DisplayName("Country")]
        public string ClientCountry { get; set; }
    }
}