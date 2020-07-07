using DeepeshWeb.DAL;
using DeepeshWeb.Models;
using DeepeshWeb.BAL.EmployeeManagement;
using DeepeshWeb.Models.AssetManagement;
using DeepeshWeb.Models.EmployeeManagement;
using Microsoft.SharePoint.Client;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DeepeshWeb.BAL
{
    public class GEN_ApproverMasterBal
    {
        public List<GEN_ApproverRoleNameModel> getApproverData(ClientContext clientContext,string empcode,string module,string approvertype)
        {
            // approver details model
            List<GEN_ApproverRoleNameModel> approverRoleNameModel = new List<GEN_ApproverRoleNameModel>();

            // create object for gen_approver master
            GEN_ApproverMasterModel approverMasterModel = new GEN_ApproverMasterModel();

            string filter = "Module eq '" + module + "' and Approver_Type eq '" + approvertype +"'";

            JArray jArray = RestGetApproverLevels(clientContext, filter);

            //List<GEN_ApproverRoleListModel> approverRoleListModels = new List<GEN_ApproverRoleListModel>();

            //GEN_ApproverRoleListBal approverRoleListBal = new GEN_ApproverRoleListBal();

            //approverRoleListModels = approverRoleListBal.GetApproverRoleListBals(clientContext);

            Emp_BasicInfoBal basicInfoBal = new Emp_BasicInfoBal();            

            approverMasterModel = new GEN_ApproverMasterModel
            {
                ID = Convert.ToInt32(jArray[0]["ID"]),
                Module = jArray[0]["Module"] == null ? "" : Convert.ToString(jArray[0]["Module"]),
                Approver_Type = jArray[0]["Approver_Type"] == null ? "" : Convert.ToString(jArray[0]["Approver_Type"]),
                Rule_For_Filter_Type = jArray[0]["Rule_For_Filter_Type"] == null ? "" : Convert.ToString(jArray[0]["Rule_For_Filter_Type"]),
                Rule_For_Filter_Data = jArray[0]["Rule_For_Filter_Data"] == null ? "" : Convert.ToString(jArray[0]["Rule_For_Filter_Data"]),
                ApproverRoleName = jArray[0]["ApproverRoleName"] == null ? "" : Convert.ToString(jArray[0]["ApproverRoleName"]),
                ApproverRoleInternalName = jArray[0]["ApproverRoleInternalName"] == null ? "" : Convert.ToString(jArray[0]["ApproverRoleInternalName"])
            };

            List<string> rolenamearr = new List<string>();

            rolenamearr = approverMasterModel.ApproverRoleInternalName.Split(',').ToList();

            // call Emp-basicinfimodel class
            Emp_BasicInfoBal emp_BasicInfo = new Emp_BasicInfoBal();

            Emp_BasicInfoModel basicInfoManager = new Emp_BasicInfoModel();

            basicInfoManager = emp_BasicInfo.GetEmpManager(clientContext, empcode);

            for (int i=0;i<rolenamearr.Count;i++)
            {
                if(rolenamearr[i] == "Manager")
                {                   
                    if (basicInfoManager.ManagerCode != null)
                    {
                        approverRoleNameModel.Add(new GEN_ApproverRoleNameModel
                        {
                            Sequence = i,
                            Role = rolenamearr[i],
                            Empcode = basicInfoManager.ManagerCode,
                            ID = basicInfoManager.ManagerId
                        });
                     }
                }
                else if (rolenamearr[i] == "ManagersManager")
                {
                    if (basicInfoManager.Manager_Code != null)
                    {
                        approverRoleNameModel.Add(new GEN_ApproverRoleNameModel
                        {
                            Sequence = i,
                            Role = rolenamearr[i],
                            Empcode = basicInfoManager.Manager_Code
                        });
                    }
                }
                else if (rolenamearr[i] == "DepartmentHead")
                {
                    if(basicInfoManager.Department!=null)
                    {
                        Emp_DepartmentModel departmentModel = new Emp_DepartmentModel();
                        Emp_DepartmentBal departmentBal = new Emp_DepartmentBal();
                        departmentModel = departmentBal.GetDepartmentHead(clientContext,basicInfoManager.Department);

                        approverRoleNameModel.Add(new GEN_ApproverRoleNameModel
                        {
                            Sequence = i,
                            Role = rolenamearr[i],
                            Empcode = departmentModel.HeadOfDepartment
                        });
                    }
                }
                else if (rolenamearr[i] == "AssetAllocateDepartment")
                {
                    GEN_ApproverRoleListModel _ApproverRoleListModel = new GEN_ApproverRoleListModel();
                    GEN_ApproverRoleListBal _ApproverRoleListBal = new GEN_ApproverRoleListBal();
                    _ApproverRoleListModel = _ApproverRoleListBal.GetEmpByRole(clientContext, rolenamearr[i]);

                    approverRoleNameModel.Add(new GEN_ApproverRoleNameModel
                    {
                        Sequence = i,
                        Role = rolenamearr[i],
                        Empcode = _ApproverRoleListModel.Empcode
                    });
                }
            }


            return approverRoleNameModel;
        }

        public JArray RestGetApproverLevels(ClientContext clientContext, string filter)
        {
            RestService restService = new RestService();
            JArray jArray = new JArray();
            RESTOption rESTOption = new RESTOption();

            rESTOption.select = "Module,Approver_Type,Rule_For_Filter_Type,Rule_For_Filter_Data,ApproverRoleName,ApproverRoleInternalName";

            if (filter != null)
            {
                rESTOption.filter = filter;
            }

            jArray = restService.GetAllItemFromList(clientContext, "GEN_ApproverMaster", rESTOption);

            return jArray;
        }
    }
}