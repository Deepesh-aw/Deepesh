using DeepeshWeb.BAL.Timesheet;
using DeepeshWeb.Models.Timesheet;
using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DeepeshWeb.Models.EmployeeManagement;
using DeepeshWeb.Models;

namespace DeepeshWeb.Controllers.TimeSheet
{
    public class TIM_SendEmailController : Controller
    {
        TIM_SendEmailBal BalEmail = new TIM_SendEmailBal();
        // GET: TIM_SendEmail
        public ActionResult Index()
        {
            return View();
        }

        public string getDescriptionFormatted(string d)
        {
            return d.Replace("'", @"\'").Replace("“", "&ldquo;").Replace("”", "&rdquo;");
        }

        public string ProjectCreationNotification(ClientContext clientContext, TIM_ProjectCreationModel project)
        {
            string returnID = "";
            try
            {
                string Fromdate= DateTime.ParseExact(project.StartDate, "MM-dd-yyyy hh:mm:ss", CultureInfo.InvariantCulture).ToString("dd-MM-yyyy", CultureInfo.InvariantCulture);

                string Todate = DateTime.ParseExact(project.EndDate, "MM-dd-yyyy hh:mm:ss", CultureInfo.InvariantCulture).ToString("dd-MM-yyyy", CultureInfo.InvariantCulture);

                string Subject = "New Project  Assignment Notification";
                string Body = "<div>Dear Member,<br /><br />The following new project has been assigned to you.<br /><br /><table style='width: 100%; border: 1px solid black'><tbody><tr><th>Project</th><td>:</td><td>"+project.ProjectName+ "</td></tr><tr><th>Project Manager</th><td>:</td><td>" + project.ProjectManagerName + "</td></tr><tr><th>Members</th><td>:</td><td>" + project.MembersText + "</td></tr><tr><th>Start Date</th><td>:</td><td>"+ Fromdate + "</td></tr><tr><th>End Date</th><td>:</td><td>" + Todate + "</td></tr></tbody></table>";

                string ItemData = "'To': '" + project.MembersEmail + "'";
                ItemData += " ,'Subject': '" + Subject + "'";
                ItemData += " ,'Body': '" + getDescriptionFormatted(Body) + "'";
                returnID = BalEmail.SaveMail(clientContext, ItemData);

            }
            catch
            {

            }
            return returnID;
        }
        public string TaskAssignmentNotification(ClientContext clientContext, TIM_TaskModel item, Emp_BasicInfoModel AssignedBy)
        {
            string returnID = "";
            try
            {
                string Fromdate = DateTime.ParseExact(item.StartDate, "MM-dd-yyyy hh:mm:ss", CultureInfo.InvariantCulture).ToString("dd-MM-yyyy", CultureInfo.InvariantCulture);

                string Todate = DateTime.ParseExact(item.EndDate, "MM-dd-yyyy hh:mm:ss", CultureInfo.InvariantCulture).ToString("dd-MM-yyyy", CultureInfo.InvariantCulture);

                string Subject = "Task Assignment Notification";
                string BodyMain = "<table style='width: 100%; border: 1px solid black'><tbody><tr><th>Project</th><td>:</td><td>" + item.ProjectName + "</td></tr><tr><th>Milestone</th><td>:</td><td>" + item.MileStoneName + "</td></tr><tr><th>Task</th><td>:</td><td>" + item.Task + "</td></tr><tr><th>Member</th><td>:</td><td>" + item.MembersName + "</td></tr><tr><th>Start Date</th><td>:</td><td>" + Fromdate + "</td></tr><tr><th>End Date</th><td>:</td><td>" + Todate + "</td></tr></tbody></table>";

                string Body = "Dear Member,<br /><br />The following new task has been assigned to you by " + AssignedBy.FullName + ".<br /><br />";

                string ItemData = "'To': '" + item.MembersEmail + "'";
                ItemData += " ,'Subject': '" + Subject + "'";
                ItemData += " ,'Body': '" + Body +getDescriptionFormatted(BodyMain) + "'";
                returnID = BalEmail.SaveMail(clientContext, ItemData);
                if (Convert.ToInt32(returnID) > 0)
                {
                    string NewBody = "Dear Member,<br /><br />You have assigned the following new task to "+ item.MembersName + ".<br /><br />";
                    string ItemDataNew = "'To': '" + AssignedBy.OfficeEmail + "'";
                    ItemDataNew += " ,'Subject': '" + Subject + "'";
                    ItemDataNew += " ,'Body': '" + NewBody + getDescriptionFormatted(BodyMain) + "'";
                    returnID = BalEmail.SaveMail(clientContext, ItemDataNew);

                }

            }
            catch
            {

            }
            return returnID;
        }
        public string SubTaskAssignmentNotification(ClientContext clientContext, TIM_SubTaskModel item, Emp_BasicInfoModel AssignedBy)
        {
            string returnID = "";
            try
            {
                string Fromdate = DateTime.ParseExact(item.StartDate, "MM-dd-yyyy hh:mm:ss", CultureInfo.InvariantCulture).ToString("dd-MM-yyyy", CultureInfo.InvariantCulture);

                string Todate = DateTime.ParseExact(item.EndDate, "MM-dd-yyyy hh:mm:ss", CultureInfo.InvariantCulture).ToString("dd-MM-yyyy", CultureInfo.InvariantCulture);

                string Subject = "Sub Task Assignment Notification";
                string MainBody = "<table style='width: 100%; border: 1px solid black'><tbody><tr><th>Project</th><td>:</td><td>" + item.ProjectName + "</td></tr><tr><th>Milestone</th><td>:</td><td>" + item.MileStoneName + "</td></tr><tr><th>Task</th><td>:</td><td>" + item.TaskName + "</td></tr><tr><th>Subtask</th><td>:</td><td>" + item.SubTask + "</td></tr><tr><th>Member</th><td>:</td><td>" + item.MembersName + "</td></tr><tr><th>Start Date</th><td>:</td><td>" + Fromdate + "</td></tr><tr><th>End Date</th><td>:</td><td>" + Todate + "</td></tr></tbody></table>";

                string Body = "Dear Member,<br /><br />The following new subtask has been assigned to you by " + AssignedBy.FullName + ".<br /><br />";

                string ItemData = "'To': '" + item.MembersEmail + "'";
                ItemData += " ,'Subject': '" + Subject + "'";
                ItemData += " ,'Body': '" + Body + getDescriptionFormatted(MainBody) + "'";
                returnID = BalEmail.SaveMail(clientContext, ItemData);
                if (Convert.ToInt32(returnID) > 0)
                {
                    string NewBody = "<div>Dear Member,<br /><br />You have assigned the following new subtask to " + item.MembersName + ".<br /><br />";
                    string ItemDataNew = "'To': '" + AssignedBy.OfficeEmail + "'";
                    ItemDataNew += " ,'Subject': '" + Subject + "'";
                    ItemDataNew += " ,'Body': '" + NewBody + getDescriptionFormatted(MainBody) + "'";
                    returnID = BalEmail.SaveMail(clientContext, ItemDataNew);
                }

            }
            catch
            {

            }
            return returnID;
        }

        public string TimesheetCreationNotification(ClientContext clientContext, TIM_EmployeeTimesheetModel item, GEN_ApproverRoleNameModel Manager, Emp_BasicInfoModel Employee)
        {
            string returnID = "";
            try
            {
                string AddedDate = DateTime.ParseExact(item.TimesheetAddedDate, "MM-dd-yyyy hh:mm:ss", CultureInfo.InvariantCulture).ToString("dd-MM-yyyy", CultureInfo.InvariantCulture);

                string InternalStatus = "Inprogress";

                string Subject = "Timesheet  Creation  Notification";
                string MainBody = "<table style='width: 100%; border: 1px solid black'><tbody><tr><th>Timesheet ID</th><td>:</td><td>" + item.TimesheetID + "</td></tr><tr><th>Date</th><td>:</td><td>" + AddedDate + "</td></tr><tr><th>Manager</th><td>:</td><td>" + Manager.FullName + "</td></tr><tr><th>Employee</th><td>:</td><td>" + Employee.FullName + "</td></tr><tr><th>Status</th><td>:</td><td>" + InternalStatus + "</td></tr></tbody></table>";

                string Body = "Dear Member,<br /><br />The following new timesheet has been created by you.<br /><br />";

                string ItemData = "'To': '" + Employee.OfficeEmail + "'";
                ItemData += " ,'Subject': '" + Subject + "'";
                ItemData += " ,'Body': '" + Body + getDescriptionFormatted(MainBody) + "'";
                returnID = BalEmail.SaveMail(clientContext, ItemData);
                if (Convert.ToInt32(returnID) > 0)
                {
                    string NewBody = "<div>Dear Member,<br /><br />The following timesheet has been created by "+Employee.FullName+" and awaiting  for your approval. <br /><br />";
                    string ItemDataNew = "'To': '" + Manager.Email + "'";
                    ItemDataNew += " ,'Subject': '" + Subject + "'";
                    ItemDataNew += " ,'Body': '" + NewBody + getDescriptionFormatted(MainBody) + "'";
                    returnID = BalEmail.SaveMail(clientContext, ItemDataNew);
                }

            }
            catch
            {

            }
            return returnID;
        }

        public string TimesheetApproveAndRejectNotification(ClientContext clientContext, TIM_EmployeeTimesheetModel item, string Action)
        {
            string returnID = "";
            try
            {
                string Subject = "Timesheet  Creation  Notification";
                string MainBody = "<table style='width: 100%; border: 1px solid black'><tbody><tr><th>Timesheet ID</th><td>:</td><td>" + item.TimesheetID + "</td></tr><tr><th>Date</th><td>:</td><td>" + item.TimesheetAddedDate + "</td></tr><tr><th>Manager</th><td>:</td><td>" + item.ManagerName + "</td></tr><tr><th>Employee</th><td>:</td><td>" + item.EmployeeName + "</td></tr><tr><th>Status</th><td>:</td><td>" + Action + "</td></tr></tbody></table>";

                string Body = "Dear Member,<br /><br />The following  timesheet has been "+Action+" by you.<br /><br />";

                string ItemData = "'To': '" + item.ManagerEmail + "'";
                ItemData += " ,'Subject': '" + Subject + "'";
                ItemData += " ,'Body': '" + Body + getDescriptionFormatted(MainBody) + "'";
                returnID = BalEmail.SaveMail(clientContext, ItemData);
                if (Convert.ToInt32(returnID) > 0)
                {
                    string NewBody = "<div>Dear Member,<br /><br />Your created timesheet has been "+Action+" by "+item.ManagerName+". Please find details below.<br /><br />";
                    string ItemDataNew = "'To': '" + item.EmployeeEmail + "'";
                    ItemDataNew += " ,'Subject': '" + Subject + "'";
                    ItemDataNew += " ,'Body': '" + NewBody + getDescriptionFormatted(MainBody) + "'";
                    returnID = BalEmail.SaveMail(clientContext, ItemDataNew);
                }

            }
            catch
            {

            }
            return returnID;
        }



    }
}