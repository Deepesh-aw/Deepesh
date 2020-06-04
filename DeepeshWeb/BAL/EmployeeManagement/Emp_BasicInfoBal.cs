using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.SharePoint.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using DeepeshWeb.Models.EmployeeManagement;
using DeepeshWeb.DAL;
using DeepeshWeb.Models;

namespace DeepeshWeb.BAL.EmployeeManagement
{
    public class Emp_BasicInfoBal
    {
            public string saveEmp(ClientContext clientContext, string ItemData)
            {

                string response = RESTSave(clientContext, ItemData);

                return response;
            }
        public List<Emp_BasicInfoModel> GetEmp(ClientContext clientContext)
        {
            List<Emp_BasicInfoModel> lstEmp = new List<Emp_BasicInfoModel>();
            JArray jArray = RESTGet(clientContext,null);
            foreach (JObject j in jArray)
            {
                lstEmp.Add(new Emp_BasicInfoModel
                {
                    ID = Convert.ToInt32(j["Id"]),
                    FullName = j["FirstName"].ToString() +" "+ j["LastName"].ToString(),
                }); ;
            }
            return lstEmp;
        }

        public List<Emp_BasicInfoModel> GetAllEmployee(ClientContext clientContext)
        {
            List<Emp_BasicInfoModel> EmpBasicinfo = new List<Emp_BasicInfoModel>();



            JArray jArray = RESTGet(clientContext, null);



            foreach (JObject j in jArray)
            {
                EmpBasicinfo.Add(new Emp_BasicInfoModel
                {
                     ID= Convert.ToInt32(j["ID"]),
                  
                    FirstName = j["FirstName"].ToString(),
                    MiddleName = j["MiddleName"].ToString(),
                    LastName = j["LastName"].ToString(),
                    Company = j["Company"]["CompanyName"].ToString(),
                    // EmpCode = j["EmpCode"].ToString(),
                    EmpCode = j["EmpCode"].ToString(),
                    Designation = j["Designation"]["Designation"].ToString(),
                    Department = j["Department"].ToString(),
                  //   Manager= j["Manager"]["FirstName"].ToString()
                });
            }
            return EmpBasicinfo;

        }

        public List<Emp_BasicInfoModel> GetManager(ClientContext clientContext)
        {
            List<Emp_BasicInfoModel> EmpManager = new List<Emp_BasicInfoModel>();
            JArray jArray = RESTGet(clientContext, null);
            foreach (JObject j in jArray)
            {
                EmpManager.Add(new Emp_BasicInfoModel
                {
                    ID = Convert.ToInt32(j["ID"]),
                    Manager= j["Manager"]["FirstName"].ToString(),
                });
            }
            return EmpManager;
        }



        public List<Emp_BasicInfoModel> GeEmployeeById(ClientContext clientContext, string empid)
        {
            List<Emp_BasicInfoModel> empdetails = new List<Emp_BasicInfoModel>();
            string filter = "ID eq " + empid;
            JArray jArray = RESTGet(clientContext, filter);

            foreach (JObject j in jArray)
            {
                empdetails.Add(new Emp_BasicInfoModel
                {
                    ID = Convert.ToInt32(j["ID"]),
                    FirstName = j["FirstName"] == null ? "" : j["FirstName"].ToString(),
                    MiddleName = j["MiddleName"] == null ? "" : j["MiddleName"].ToString(),
                    LastName = j["LastName"] == null ? "" : j["LastName"].ToString(),
                    EmpCode = j["EmpCode"] == null ? "" : j["EmpCode"].ToString(),
                    JoiningDate = j["JoiningDate"] == null ? "" : Convert.ToString(j["JoiningDate"]).Trim(),
                    DOB = j["DOB"] == null ? "" : Convert.ToString(j["DOB"]).Trim(),
                    Gender = j["Gender"] == null ? "" : j["Gender"].ToString(),
                    MaritalStatus = j["MaritalStatus"] == null ? "" : j["MaritalStatus"].ToString(),
                    OnProbationTill = j["OnProbationTill"] == null ? "" : Convert.ToString(j["OnProbationTill"]).Trim(),
                    ProbationStatus = j["ProbationStatus"] == null ? "" : j["ProbationStatus"].ToString(),
                    Manager = j["Manager"]["FirstName"] == null ? "" : j["Manager"]["FirstName"].ToString(),
                    OfficeEmail = j["OfficeEmail"] == null ? "" : j["OfficeEmail"].ToString(),
                    ContactNumber = j["ContactNumber"] == null ? "" : j["ContactNumber"].ToString(),
                    EmpStatus = j["EmpStatus"] == null ? "" : j["EmpStatus"].ToString(),
                    Company = j["Company"]["CompanyName"] == null ? "" : j["Company"]["CompanyName"].ToString(),
                    Designation = j["Designation"]["Designation"] == null ? "" : j["Designation"]["Designation"].ToString(),
                    Department = j["Department"]["DepartmentName"] == null ? "" : j["Department"]["DepartmentName"].ToString(),
                    Division = j["Division"]["Division"] == null ? "" : j["Division"]["Division"].ToString(),
                    Region = j["Region"]["Region"] == null ? "" : j["Region"]["Region"].ToString(),
                    Branch = j["Branch"]["Branch"] == null ? "" : j["Branch"]["Branch"].ToString(),
                    User_Name = j["User_Name"] ["Title"] == null ? "" : j["User_Name"]["Title"].ToString(),
                   // Manager_Code = j["Manager_Code"] == null ? "" : j["Manager_Code"].ToString()
                });
            }


            return empdetails;

        }

        private JArray RESTGet(ClientContext clientContext, string filter)
            {
                RestService restService = new RestService();
                JArray jArray = new JArray();
                RESTOption rESTOption = new RESTOption();
                rESTOption.filter = filter;
            rESTOption.select = "ID,FirstName,MiddleName,LastName,EmpCode,Gender,MaritalStatus,DOB,JoiningDate,OnProbationTill,ProbationStatus,OfficeEmail,ContactNumber,EmpStatus,Designation/Id,Designation/Designation,Department/Id,Department/DepartmentName,Division/Id,Division/Division,Region/Id,Region/Region,Branch/Id,Branch/Branch,Company/Id,Company/CompanyName,Manager/Id,Manager/FirstName,User_Name/Id,User_Name/Title,Profile_Pic_Url,Facebook,Google,LinkedIn,Twitter,Instagram,Tiktok";
            rESTOption.expand = "Company,Designation,Department,Division,Region,Branch,User_Name,Manager";
            rESTOption.top = "5000";


                jArray = restService.GetAllItemFromList(clientContext, "Emp_BasicInfo", rESTOption);

                return jArray;
            }

         /*   private JArray RESTGetByID(ClientContext clientContext, string ID)
            {
                RestService restService = new RestService();
                JArray jArray = new JArray();
                RESTOption rESTOption = new RESTOption();

            rESTOption.select = "ID,FirstName,MiddleName,LastName,EmpCode,Gender,MaritalStatus,DOB,JoiningDate,OnProbationTill,ProbationStatus,OfficeEmail,ContactNumber,EmpStatus,Designation/Id,Designation/Designation,Department/Id,Department/DepartmentName,Division/Id,Division/Division,Region/Id,Region/Region,Branch/Id,Branch/Branch,Manager/Id,Manager/Title,Company/Id,Company/CompanyName";
            rESTOption.expand = "Company,Designation,Manager,Department,Division,Region,Branch";
            rESTOption.top = "5000";


                jArray = restService.GetItemByID(clientContext, "Emp_BasicInfo", rESTOption, ID);

                return jArray;
            } */




            private string RESTSave(ClientContext clientContext, string ItemData)
            {
                RestService restService = new RestService();

                return restService.SaveItem(clientContext, "Emp_BasicInfo", ItemData);
            }



        }
    }
