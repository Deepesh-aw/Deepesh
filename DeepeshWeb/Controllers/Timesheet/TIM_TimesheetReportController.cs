using DeepeshWeb.BAL.EmployeeManagement;
using DeepeshWeb.BAL.Timesheet;
using DeepeshWeb.Models.Timesheet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DeepeshWeb.Controllers.TimeSheet
{
    public class TIM_TimesheetReportController : Controller
    {
        // GET: TIM_TimesheetReport
        TIM_EmployeeTimesheetBal BalEmpTimesheet = new TIM_EmployeeTimesheetBal();
        Emp_BasicInfoBal BalEmp = new Emp_BasicInfoBal();

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ActionName("GetReportData")]
        public JsonResult GetReportData(string From, string To)
        {
            List<object> obj = new List<object>();
            try
            {
                var spContext = SharePointContextProvider.Current.GetSharePointContext(HttpContext);
                using (var clientContext = spContext.CreateUserClientContextForSPHost())
                {
                    List<TIM_EmployeeTimesheetModel> lstEmployeeTimesheet = new List<TIM_EmployeeTimesheetModel>();
                    lstEmployeeTimesheet = BalEmpTimesheet.GetEmpTimesheetByEmpIdAndDateFilter(clientContext, BalEmp.GetEmpByLogIn(clientContext), From, To);
                    if (lstEmployeeTimesheet.Count > 0)
                    {
                        obj.Add("OK");
                        obj.Add(lstEmployeeTimesheet);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("An error occured while performing action. GUID: {0}", ex.ToString()));
            }

            return Json(obj, JsonRequestBehavior.AllowGet);
        }
    }
}