using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DeepeshWeb.Models.AssetManagement
{
    public class GEN_ApproverMasterModel
    {
        public int ID { get; set; }

        public string Module { get; set; }

        public string Approver_Type { get; set; }

        public string Rule_For_Filter_Type { get; set; }

        public string Rule_For_Filter_Data { get; set; }

        public string ApproverRoleName { get; set; }

        public string ApproverRoleInternalName { get; set; }

    }
}