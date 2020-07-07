using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DeepeshWeb.Models.EmployeeManagement
{
    public class Emp_DepartmentModel
    {
        public int ID { get; set; }
        public string DepartmentName { get; set; }
        public string HeadOfDepartment { get; set; }
        public string Description { get; set; }
    }
}