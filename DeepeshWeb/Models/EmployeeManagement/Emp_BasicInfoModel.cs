using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DeepeshWeb.Models.EmployeeManagement
{
    public class Emp_BasicInfoModel
    {
        public int ID { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string EmpCode { get; set; }
        public string Gender { get; set; }
        public string MaritalStatus { get; set; }
        public string DOB { get; set; }
        public string JoiningDate { get; set; }
        public string OnProbationTill { get; set; }
        public string ProbationStatus { get; set; }
        public string OfficeEmail { get; set; }
        public string ContactNumber { get; set; }
        public string EmpStatus { get; set; }
        public int CompanyId { get; set; }
        public string Company { get; set; }
        public int DesignationId { get; set; }
        public string Designation { get; set; }
        public int DepartmentId { get; set; }
        public string Department { get; set; }
        public int DivisionId { get; set; }
        public string Division { get; set; }
        public int RegionId { get; set; }
        public string Region { get; set; }
        public int BranchId { get; set; }
        public string Branch { get; set; }
        public string ManagerCode { get; set; }
        public string Manager_Code { get; set; }
        public string User_Name { get; set; }
        public string UserNameId { get; set; }
        public string Manager { get; set; }
        public int ManagerId { get; set; }
        public string Profile_pic_url { get; set; }
        public string Facebook { get; set; }
        public string Google { get; set; }
        public string Linkedln { get; set; }
        public string Twitter { get; set; }
        public string Instagram { get; set; }
        public string Tiktok { get; set; }
        public string DOB_Months { get; set; }
        public string JoiningDate_Month { get; set; }
        public string FullName { get; set; }
        public string MobileNo { get; set; }
    }
}