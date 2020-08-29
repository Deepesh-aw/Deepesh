using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DeepeshWeb.Models
{
    public class GEN_ApproverRoleNameModel
    {
        public int ID { get; set; }
        public int Sequence { get; set; }

        public string Role { get; set; }

        public string Empcode { get; set; }

        public string ApproverId { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
    }
}