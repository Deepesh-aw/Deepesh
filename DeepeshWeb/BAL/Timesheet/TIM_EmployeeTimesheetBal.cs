using DeepeshWeb.DAL;
using DeepeshWeb.Models;
using DeepeshWeb.Models.Timesheet;
using Microsoft.SharePoint.Client;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DeepeshWeb.BAL.Timesheet
{
    public class TIM_EmployeeTimesheetBal
    {
        public List<TIM_EmployeeTimesheetModel> GetEmpTimesheetByAllTaskId(ClientContext clientContext, int AllTaskId)
        {
            List<TIM_EmployeeTimesheetModel> lstTIM_EmployeeTimesheet = new List<TIM_EmployeeTimesheetModel>();
            string filter = "(TaskId eq " + AllTaskId + " or  SubTaskId eq " + AllTaskId + " ) and  InternalStatus ne 'Approved'";
            JArray jArray = RESTGet(clientContext, filter);
            lstTIM_EmployeeTimesheet = BindList(jArray);
            return lstTIM_EmployeeTimesheet;
        }

        public List<TIM_EmployeeTimesheetModel> BindList(JArray jArray)
        {
            List<TIM_EmployeeTimesheetModel> lstEmployeeTimesheet = new List<TIM_EmployeeTimesheetModel>();
            foreach (JObject j in jArray)
            {
                TIM_EmployeeTimesheetModel data = new TIM_EmployeeTimesheetModel();

                data.ID = j["ID"] == null ? 0 : Convert.ToInt32(j["ID"]);
                data.ApproveDate = j["ApproveDate"] == null ? "" : Convert.ToString(j["ApproveDate"]);
                data.ApproveDescription = j["ApproveDescription"] == null ? "" : Convert.ToString(j["ApproveDescription"]);
                data.Description = j["Description"] == null ? "" : Convert.ToString(j["Description"]);
                data.Employee = j["Employee"]["ID"] == null ? 0 : Convert.ToInt32(j["Employee"]["ID"]);
                data.EmployeeName = j["Employee"]["FirstName"] == null ? "" : j["Employee"]["FirstName"].ToString() + " " + j["Employee"]["LastName"].ToString();
                data.EstimatedHours = j["EstimatedHours"] == null ? 0 : Convert.ToInt32(j["EstimatedHours"]);
                data.Hours = j["Hours"] == null ? 0 : Convert.ToInt32(j["Hours"]);
                data.Manager = j["Manager"]["ID"] == null ? 0 : Convert.ToInt32(j["Manager"]["ID"]);
                data.ManagerName = j["Manager"]["FirstName"] == null ? "" : j["Manager"]["FirstName"].ToString() + " " + j["Manager"]["LastName"].ToString();
                data.MileStone = j["MileStone"]["ID"] == null ? 0 : Convert.ToInt32(j["MileStone"]["ID"]);
                data.MileStoneName = j["MileStone"]["MileStone"] == null ? "" : Convert.ToString(j["MileStone"]["MileStone"]);
                data.Project = j["Project"]["Id"] == null ? 0 : Convert.ToInt32(j["Project"]["Id"]);
                data.ProjectName = j["Project"]["ProjectName"] == null ? "" : j["Project"]["ProjectName"].ToString();
                data.RemainingHours = j["RemainingHours"] == null ? 0 : Convert.ToInt32(j["RemainingHours"]);
                data.Status = j["Status"]["ID"] == null ? 0 : Convert.ToInt32(j["Status"]["ID"]);
                data.StatusName = j["Status"]["StatusName"] == null ? "" : j["Status"]["StatusName"].ToString();
                data.SubTask = j["SubTask"]["ID"] == null ? 0 : Convert.ToInt32(j["SubTask"]["ID"]);
                data.SubTaskName = j["SubTask"]["SubTask"] == null ? "" : Convert.ToString(j["SubTask"]["SubTask"]);
                data.Task = j["Task"]["Id"] == null ? 0 : Convert.ToInt32(j["Task"]["Id"]);
                data.TaskName = j["Task"]["Task"] == null ? "" : j["Task"]["Task"].ToString();
                data.TimesheetAddedDate = j["TimesheetAddedDate"] == null ? "" : Convert.ToString(j["TimesheetAddedDate"]);
                data.UtilizedHours = j["UtilizedHours"] == null ? 0 : Convert.ToInt32(j["UtilizedHours"]);

                lstEmployeeTimesheet.Add(data);
            }

            return lstEmployeeTimesheet;
        }

        private JArray RESTGet(ClientContext clientContext, string filter)
        {
            RestService restService = new RestService();
            JArray jArray = new JArray();
            RESTOption rESTOption = new RESTOption();

            rESTOption.filter = filter;
            rESTOption.select = "*,Employee/ID,Employee/FirstName,Employee/LastName,Manager/ID,Manager/FirstName,Manager/LastName,MileStone/ID,MileStone/MileStone,Project/Id,Project/ProjectName,Task/Id,Task/Task,SubTask/Id,SubTask/SubTask,Status/StatusName,Status/ID";
            rESTOption.expand = "Employee,Manager,MileStone,Project,Task,SubTask";
            rESTOption.orderby = "ID desc";
            rESTOption.top = "5000";


            jArray = restService.GetAllItemFromList(clientContext, "TIM_EmployeeTimesheet", rESTOption);

            return jArray;
        }

        private string RESTSave(ClientContext clientContext, string ItemData)
        {
            RestService restService = new RestService();
            return restService.SaveItem(clientContext, "TIM_EmployeeTimesheet", ItemData);
        }

        public string UpdateProject(ClientContext clientContext, string ItemData, string ID)
        {

            string response = RESTUpdate(clientContext, ItemData, ID);

            return response;
        }

        private string RESTUpdate(ClientContext clientContext, string ItemData, string ID)
        {
            RestService restService = new RestService();

            return restService.UpdateItem(clientContext, "TIM_EmployeeTimesheet", ItemData, ID);
        }

    }
}