using DeepeshWeb.BAL.Timesheet;
using DeepeshWeb.Models.Timesheet;
using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

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
            return d.Replace("'", "&apos;").Replace("“", "&ldquo;").Replace("”", "&rdquo;");
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
        public string TaskAssignmentNotification(ClientContext clientContext, string ToWhom, string ByWhom, string ToWhomName, string ByWhomName)
        {
            string returnID = "";
            try
            {
                string Subject = "Task Assignment Notification";
                string Body = "<div>Dear " + ToWhomName + ",<br /><br />The following new task has been assigned to you by " + ByWhomName + ".<br /><br /><table style='width: 100%; border: 1px solid black'><tbody><tr><th>Project</th><td>:</td><td>Avrio</td></tr><tr><th>Project Manager</th><td>:</td><td>Ram</td></tr><tr><th>Members</th><td>:</td><td>Santosh Patel</td></tr><tr><th>Start Date</th><td>:</td><td>28-08-2020</td></tr><tr><th>End Date</th><td>:</td><td>28-09-2020</td></tr></tbody></table>";

                string ItemData = "'To': '" + ToWhom + "'";
                ItemData += " ,'Subject': '" + Subject + "'";
                ItemData += " ,'Body': '" + getDescriptionFormatted(Body) + "'";
                returnID = BalEmail.SaveMail(clientContext, ItemData);

            }
            catch
            {

            }
            return returnID;
        }
    }
}