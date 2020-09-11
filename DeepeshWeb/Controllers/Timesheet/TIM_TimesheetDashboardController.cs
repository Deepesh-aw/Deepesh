using DeepeshWeb.BAL;
using DeepeshWeb.BAL.EmployeeManagement;
using DeepeshWeb.BAL.Timesheet;
using DeepeshWeb.Models;
using DeepeshWeb.Models.EmployeeManagement;
using DeepeshWeb.Models.Timesheet;
using Microsoft.Ajax.Utilities;
using Microsoft.SharePoint.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DeepeshWeb.Controllers.TimeSheet
{
    public class TIM_TimesheetDashboardController : Controller
    {
        TIM_ProjectCreationBal BalProjectCreation = new TIM_ProjectCreationBal();
        TIM_MilestoneBal BalMilestone = new TIM_MilestoneBal();
        TIM_TaskBal BalTask = new TIM_TaskBal();
        TIM_SubTaskBal BalSubTask = new TIM_SubTaskBal();
        TIM_WorkFlowMasterBal BalWorkflow = new TIM_WorkFlowMasterBal();
        TIM_ProjectTypeMasterBal BalProjectType = new TIM_ProjectTypeMasterBal();
        Emp_ClientMasterDetailsBal BalClient = new Emp_ClientMasterDetailsBal();
        Emp_BasicInfoBal BalEmp = new Emp_BasicInfoBal();
        TIM_StatusMasterBal BalStatus = new TIM_StatusMasterBal();
        TIM_WorkingHoursBal BalWorkinghours = new TIM_WorkingHoursBal();
        TIM_EmployeeTimesheetBal BalEmpTimesheet = new TIM_EmployeeTimesheetBal();
        GEN_ApproverMasterBal BalApprover = new GEN_ApproverMasterBal();
        TIM_DocumentLibraryBal BalDocument = new TIM_DocumentLibraryBal();
        TIM_TimesheetParentBal BalParentTimesheet = new TIM_TimesheetParentBal();
        TIM_SendEmailController EmailCtrl = new TIM_SendEmailController();
        TIM_SettingBal BalSetting = new TIM_SettingBal();

        // GET: TIM_TimesheetDashboard
        public ActionResult Index()
        {
            try
            {
                var spContext = SharePointContextProvider.Current.GetSharePointContext(HttpContext);
                using (var clientContext = spContext.CreateUserClientContextForSPHost())
                {
                    ViewBag.StatusData = BalStatus.GetStatusForAction(clientContext);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("An error occured while performing action. GUID: {0}", ex.ToString()));
            }
           
            return View();
        }

        [HttpPost]
        [ActionName("TimesheetLoadData")]
        public JsonResult TimesheetLoadData()
        {
            List<object> obj = new List<object>();
            try
            {
                var spContext = SharePointContextProvider.Current.GetSharePointContext(HttpContext);
                using (var clientContext = spContext.CreateUserClientContextForSPHost())
                {
                    List<TIM_TaskModel> lstTask = new List<TIM_TaskModel>();
                    List<TIM_SubTaskModel> lstSubTask = new List<TIM_SubTaskModel>();
                    List<TIM_TimesheetParentModel> lstTimesheetParent = BalParentTimesheet.GetEmpTimesheetByEmpId(clientContext, BalEmp.GetEmpByLogIn(clientContext));
                    lstTask = BalTask.GetAllTask(clientContext, BalEmp.GetEmpByLogIn(clientContext));
                    lstSubTask = BalSubTask.GetAllSubTask(clientContext, BalEmp.GetEmpByLogIn(clientContext));
                    ViewBag.AllTask = lstTask.Cast<object>().Concat(lstSubTask).ToList();

                    ViewBag.Setting = BalSetting.GetSettingData(clientContext);


                    obj.Add("OK");
                    obj.Add(lstTimesheetParent);
                    obj.Add(ViewBag.AllTask);
                    obj.Add(ViewBag.Setting);

                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("An error occured while performing action. GUID: {0}", ex.ToString()));
            }
            return Json(obj, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ActionName("OnLoadData")]
        public JsonResult OnLoadData()
        {
            List<object> obj = new List<object>();
            //List<TIM_WorkingHoursModel> lstWorkingHours = new List<TIM_WorkingHoursModel>();
            try
            {
                var spContext = SharePointContextProvider.Current.GetSharePointContext(HttpContext);
                using (var clientContext = spContext.CreateUserClientContextForSPHost())
                {
                    //lstWorkingHours = BalWorkinghours.GetWorkingHour(clientContext);

                    //ViewBag.AllTimesheet = BalEmpTimesheet.GetAllEmpTimesheet(clientContext);
                    //ViewBag.Setting = BalSetting.GetSettingData(clientContext);

                    obj.Add("OK");
                    //obj.Add(lstWorkingHours);
                    //obj.Add(ViewBag.Setting);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("An error occured while performing action. GUID: {0}", ex.ToString()));
            }
            return Json(obj, JsonRequestBehavior.AllowGet);
        }

        //[HttpPost]
        //[ActionName("GetProjectDataByClient")]
        //public JsonResult GetProjectDataByClient(int Client)
        //{
        //    List<object> obj = new List<object>();
        //    List<TIM_ProjectCreationModel> lstProject = new List<TIM_ProjectCreationModel>();
        //    var spContext = SharePointContextProvider.Current.GetSharePointContext(HttpContext);
        //    using (var clientContext = spContext.CreateUserClientContextForSPHost())
        //    {
        //        //lstProject = bal.GetProjectCreationByClientId(clientContext, Client, BalEmp.GetEmpByLogIn(clientContext), BalEmp.GetEmpCodeByLogIn(clientContext));
        //        if (lstProject.Count > 0)
        //        {
        //            obj.Add("OK");
        //            obj.Add(lstProject);
        //        }
        //    }
        //    return Json(obj, JsonRequestBehavior.AllowGet);
        //}

        [HttpPost]
        [ActionName("GetPrevTimesheet")]
        public JsonResult GetPrevTimesheet(int AllTaskId, string TimesheetAddedDate, string ParentID)
        {
            List<object> obj = new List<object>();
            List<TIM_EmployeeTimesheetModel> lstEmployeeTimesheet = new List<TIM_EmployeeTimesheetModel>();
            try
            {
                var spContext = SharePointContextProvider.Current.GetSharePointContext(HttpContext);
                using (var clientContext = spContext.CreateUserClientContextForSPHost())
                {
                    lstEmployeeTimesheet = BalEmpTimesheet.GetEmpTimesheetByAllTaskId(clientContext, AllTaskId, TimesheetAddedDate, ParentID);
                    obj.Add("OK");
                    obj.Add(lstEmployeeTimesheet);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("An error occured while performing action. GUID: {0}", ex.ToString()));
            }
            return Json(obj, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ActionName("AddTimesheet")]
        public JsonResult AddTimesheet(System.Web.Mvc.FormCollection formCollection)
        {
            List<object> obj = new List<object>();
            List<TIM_DocumentLibraryModel> DeleteDocument = new List<TIM_DocumentLibraryModel>();
            List<TIM_EmployeeTimesheetModel> DeleteEmpTimesheet = new List<TIM_EmployeeTimesheetModel>();

            var Timesheet = formCollection["TimesheetDetails"];
            List<TIM_EmployeeTimesheetModel> EmpTimesheet = JsonConvert.DeserializeObject<List<TIM_EmployeeTimesheetModel>>(Timesheet);

            var DeleteTimesheet = formCollection["DeleteTimesheet"];
            if(DeleteTimesheet != null)
                DeleteEmpTimesheet = JsonConvert.DeserializeObject<List<TIM_EmployeeTimesheetModel>>(DeleteTimesheet);

            var deletedoc = formCollection["DeleteDocument"];
             if(deletedoc != null)
               DeleteDocument = JsonConvert.DeserializeObject<List<TIM_DocumentLibraryModel>>(deletedoc);

            int i = 0;
            string InternalStatus = "Inprogress";
            var spContext = SharePointContextProvider.Current.GetSharePointContext(HttpContext);
            try
            {
                using (var clientContext = spContext.CreateUserClientContextForSPHost())
                {
                    List<TIM_StatusMasterModel> lstPendingStatus = new List<TIM_StatusMasterModel>();
                    lstPendingStatus = BalStatus.GetPendingStatus(clientContext);
                    List<GEN_ApproverRoleNameModel> lstApprover = BalApprover.getApproverData(clientContext, BalEmp.GetEmpCodeByLogIn(clientContext), "Timesheet", "Main");
                    Emp_BasicInfoModel Employee = BalEmp.GetEmpMailByLogIn(clientContext);

                    if (DeleteDocument.Count > 0)
                    {
                        int z = 0;
                        foreach (var deleteItem in DeleteDocument)
                        {
                            string Result = BalDocument.DeleteDocument(clientContext, deleteItem.ID.ToString());
                            if (Result == "Delete")
                                z++;
                        }
                        if (z != DeleteDocument.Count)
                        {
                            obj.Add("ERROR");
                            return Json(obj, JsonRequestBehavior.AllowGet);
                        }
                    }

                    if (DeleteEmpTimesheet.Count > 0)
                    {
                        int z = 0;
                        List<TIM_WorkFlowMasterModel> lstTimesheetDeletion = new List<TIM_WorkFlowMasterModel>();
                        lstTimesheetDeletion = BalWorkflow.GetTimesheetDeletion(clientContext);
                        var DeleteItem = "'StatusId': '" + lstTimesheetDeletion[0].ToStatusID + "'";
                        DeleteItem += " ,'InternalStatus': '" + lstTimesheetDeletion[0].InternalStatus + "'";
                        foreach (var deleteEmpItem in DeleteEmpTimesheet)
                        {
                            string Result = BalEmpTimesheet.UpdateTimesheet(clientContext, DeleteItem, deleteEmpItem.ID.ToString());
                            if (Result == "Update")
                            {
                                var hours = deleteEmpItem.Hours.Split(':');
                                var h = Convert.ToInt32(hours[0]) * (-1);
                                var m = Convert.ToInt32(hours[1]) * (-1);
                                var NewHours = h.ToString() + ":" + m.ToString();
                                deleteEmpItem.AlterUtilizeHour = NewHours;
                                string status = GetAndEditPrevTimesheet(clientContext, deleteEmpItem);
                                if (status != "OK")
                                {
                                    obj.Add("ERROR");
                                    return Json(obj, JsonRequestBehavior.AllowGet);
                                }
                                else
                                    z++;
                            }
                        }
                        if (z != DeleteEmpTimesheet.Count)
                        {
                            obj.Add("ERROR");
                            return Json(obj, JsonRequestBehavior.AllowGet);
                        }
                    }
                   
                    int ParentID = 0;
                    List<TIM_TimesheetParentModel> PrevParentTimesheet = new List<TIM_TimesheetParentModel>();
                    PrevParentTimesheet = BalParentTimesheet.GetEmpTimesheetByTimesheetId(clientContext, EmpTimesheet[0].TimesheetID);
                    if (PrevParentTimesheet.Count == 0)
                    {
                        string ParentItemData = " 'EmployeeId': '" + Employee.ID + "'";
                        ParentItemData += " ,'TimesheetID': '" + EmpTimesheet[0].TimesheetID + "'";
                        ParentItemData += " ,'StatusId': '" + lstPendingStatus[0].ID + "'";
                        ParentItemData += " ,'InternalStatus': '" + InternalStatus + "'";
                        ParentItemData += " ,'ManagerId': '" + lstApprover[0].ID + "'";
                        ParentItemData += " ,'TimesheetAddedDate': '" + EmpTimesheet[0].TimesheetAddedDate + "'";
                        ParentID = Convert.ToInt32(BalParentTimesheet.SaveTimesheet(clientContext, ParentItemData));
                        if(ParentID > 0)
                        {
                            var count = Convert.ToInt32(EmpTimesheet[0].TimesheetID.Split('-')[1]) + 1;
                            string SettingItemData = " 'TimesheetCount': '" + count + "'";
                            string resID = BalSetting.UpdateSetting(clientContext, SettingItemData, "1");
                            if (resID != "Update")
                            {
                                obj.Add("ERROR");
                                return Json(obj, JsonRequestBehavior.AllowGet);
                            }
                        }
                    }
                    else
                    {
                        ParentID = PrevParentTimesheet[0].ID;
                    }

                    string returnID = "0";
                    foreach (var item in EmpTimesheet)
                    {
                        string itemdata = " 'Description': '" + item.Description.Replace("'", @"\'") + "'";

                        itemdata += " ,'Hours': '" + item.Hours + "'";
                        itemdata += " ,'EstimatedHours': '" + item.EstimatedHours + "'";
                        itemdata += " ,'UtilizedHours': '" + item.UtilizedHours + "'";
                        itemdata += " ,'RemainingHours': '" + item.RemainingHours + "'";
                        itemdata += " ,'TimesheetAddedDate': '" + item.TimesheetAddedDate + "'";
                        itemdata += " ,'EmployeeId': '" + Employee.ID + "'";
                        itemdata += " ,'ManagerId': '" + lstApprover[0].ID + "'";
                        itemdata += " ,'FromTime': '" + item.FromTime + "'";
                        itemdata += " ,'ToTime': '" + item.ToTime + "'";
                        itemdata += " ,'AllTaskStatusId': '" + item.AllTaskStatus + "'";
                        itemdata += " ,'TimesheetID': '" + item.TimesheetID + "'";
                        itemdata += " ,'StatusId': '" + lstPendingStatus[0].ID + "'";
                        itemdata += " ,'InternalStatus': '" + InternalStatus + "'"; 
                        itemdata += " ,'ParentIDId': '" + ParentID + "'";

                        itemdata += " ,'OtherClient': '" + item.OtherClient + "'";
                        itemdata += " ,'OtherProject': '" + item.OtherProject + "'";
                        itemdata += " ,'OtherMilestone': '" + item.OtherMilestone + "'";
                        itemdata += " ,'OtherTask': '" + item.OtherTask + "'";

                        if (item.ID > 0)
                        {
                            if (item.AlterUtilizeHour != null && item.AlterUtilizeHour != "")
                            {
                               string status = GetAndEditPrevTimesheet(clientContext, item);
                                if (status != "OK")
                                {
                                    obj.Add("ERROR");
                                    return Json(obj, JsonRequestBehavior.AllowGet);
                                }
                            }

                            returnID = BalEmpTimesheet.UpdateTimesheet(clientContext, itemdata, item.ID.ToString());
                            if (returnID == "Update")
                            {
                                if (Request.Files.Count > 0)
                                    UploadTimesheetDoc(clientContext, Request.Files, item, item.ID.ToString());
                                i++;
                            }
                        }
                        else
                        {
                            itemdata += " ,'ProjectId': '" + item.Project + "'";
                            itemdata += " ,'TaskId': '" + item.Task + "'";
                            itemdata += " ,'SubTaskId': '" + item.SubTask + "'";
                            itemdata += " ,'ClientId': '" + item.Client + "'";
                            itemdata += " ,'MileStoneId': '" + item.MileStone + "'";
                            returnID = BalEmpTimesheet.SaveTimesheet(clientContext, itemdata);
                            if (Convert.ToInt32(returnID) > 0)
                            {
                                item.ParentID = ParentID;
                                string status = EditPrevTimesheetForNew(clientContext, item);
                                if (status != "OK")
                                {
                                    obj.Add("ERROR");
                                    return Json(obj, JsonRequestBehavior.AllowGet);
                                }
                                else
                                {
                                    if (Request.Files.Count > 0)
                                        UploadTimesheetDoc(clientContext, Request.Files, item, returnID);

                                    string Mailres = EmailCtrl.TimesheetCreationNotification(clientContext, EmpTimesheet[0], lstApprover[0], Employee);
                                    if (Convert.ToInt32(Mailres) > 0)
                                        i++;
                                }
                                
                            }
                            
                        }

                    }

                    if (i == EmpTimesheet.Count)
                    {
                        obj.Add("OK");
                    }
                }
            }
            catch (Exception ex)
            {
                obj.Add("ERROR");
                return Json(obj, JsonRequestBehavior.AllowGet);
                throw new Exception(string.Format("An error occured while performing action. GUID: {0}", ex.ToString()));
            }
            return Json(obj, JsonRequestBehavior.AllowGet);
        }

        public string EditPrevTimesheetForNew(ClientContext clientContext, TIM_EmployeeTimesheetModel item)
        {
            string result = "ERROR";
            int count = 0;
            List<TIM_EmployeeTimesheetModel> lstPrevtimesheet = BalEmpTimesheet.GetTIMForAlterExist(clientContext, item);
            if (lstPrevtimesheet.Count > 0)
            {
                foreach (var data in lstPrevtimesheet)
                {
                    string NewUtilized = GetTimeDiffForUtilizedHours(data.UtilizedHours, item.Hours);
                    string NewRemaining = GetTimeDiffForRemainingHours(data.RemainingHours, item.Hours);
                    string itemdata = " 'UtilizedHours': '" + NewUtilized + "'";
                    itemdata += " ,'RemainingHours': '" + NewRemaining + "'";
                    string returnID = BalEmpTimesheet.UpdateTimesheet(clientContext, itemdata, data.ID.ToString());
                    if (returnID == "Update")
                    {
                        count++;
                    }

                }

                if (lstPrevtimesheet.Count == count)
                    result = "OK";
            }
            else
            {
                result = "OK";
            }

            return result;
        }

        public string GetAndEditPrevTimesheet(ClientContext clientContext, TIM_EmployeeTimesheetModel item)
        {
            string result = "ERROR";
            int count = 0;
            List<TIM_EmployeeTimesheetModel> lstPrevtimesheet = BalEmpTimesheet.AlterPrevEmpTimesheet(clientContext, item);
            if (lstPrevtimesheet.Count > 0)
            {
                foreach (var data in lstPrevtimesheet)
                {
                    string NewUtilized = GetTimeDiffForUtilizedHours(data.UtilizedHours, item.AlterUtilizeHour);
                    string NewRemaining = GetTimeDiffForRemainingHours(data.RemainingHours, item.AlterUtilizeHour);
                    string itemdata = " 'UtilizedHours': '" + NewUtilized + "'";
                    itemdata += " ,'RemainingHours': '" + NewRemaining + "'";
                    string returnID = BalEmpTimesheet.UpdateTimesheet(clientContext, itemdata, data.ID.ToString());
                    if (returnID == "Update")
                    {
                        count++;
                    }

                }

                if (lstPrevtimesheet.Count == count)
                    result = "OK";
            }
            else
            {
                result = "OK";
            }

            return result;
        }

        public string GetTimeDiffForRemainingHours(string before, string after)
        {
            string result = "";
            var b = before.Split(':');
            var a = after.Split(':');

            var hour_carry = 0;
            var Ihour = 0;
            var Imin = 0;
            var Smin = string.Empty;

            Imin = Convert.ToInt32(b[1]) - Convert.ToInt32(a[1]);
            Smin = Imin.ToString();

            
            if (Imin < 0)
            {
                Imin += 60;
                if (Imin.ToString().Length == 1)
                    Smin = "0" + Imin.ToString();
                hour_carry += 1;

                Ihour = Convert.ToInt32(b[0]) - Convert.ToInt32(a[0]) - Convert.ToInt32(hour_carry);

            }
            else if (Imin > 59)
            {
                hour_carry = Imin / 60 | 0;
                Imin = Imin % 60 | 0;

                Ihour = Convert.ToInt32(b[0]) - Convert.ToInt32(a[0]) + Convert.ToInt32(hour_carry);

                if (Imin.ToString().Length == 1)
                    Smin = "0" + Imin.ToString();
            }
            else
            {
                Ihour = Convert.ToInt32(b[0]) - Convert.ToInt32(a[0]);
            }

            var Shour = Ihour.ToString();
            result = Shour + ":" + Smin;

            return result;
        }

        public string GetTimeDiffForUtilizedHours(string before, string after)
        {
            string result = "";
            var b = before.Split(':');
            var a = after.Split(':');

            var hour_carry = 0;
            var Ihour = 0;

            var Imin = Convert.ToInt32(b[1]) + Convert.ToInt32(a[1]);
            var Smin = Imin.ToString();

            if (Imin < 0)
            {
                Imin += 60;
                if (Imin.ToString().Length == 1)
                    Smin = "0" + Imin.ToString();
                hour_carry += 1;

                Ihour = Convert.ToInt32(b[0]) + Convert.ToInt32(a[0]) - Convert.ToInt32(hour_carry);

            }
            else if (Imin > 59)
            {
                hour_carry = Imin / 60 | 0;
                Imin = Imin % 60 | 0;

                Ihour = Convert.ToInt32(b[0]) + Convert.ToInt32(a[0]) + Convert.ToInt32(hour_carry);

                if (Imin.ToString().Length == 1)
                    Smin = "0" + Imin.ToString();
            }
            else
            {
                Ihour = Convert.ToInt32(b[0]) + Convert.ToInt32(a[0]);
            }

            var Shour = Ihour.ToString();
            result = Shour + ":" + Smin;

            return result;
        }

        public void UploadTimesheetDoc(ClientContext clientContext, HttpFileCollectionBase files, TIM_EmployeeTimesheetModel item, string returnID)
        {
            try
            {
                string Type = "TimesheetCreation";
                for (int j = 0; j < files.Count; j++)
                {
                    var DocName = files[j].FileName.Split('_')[0];
                    if (item.FileCount == DocName)
                    {
                        var postedFile = files[j];
                        string Docitem = "'LID' : '" + returnID + "'";
                        Docitem += ",'TimesheetID' : '" + item.TimesheetID + "'";
                        Docitem += ",'DocumentType' : '" + Type + "'";
                        Docitem += ",'DocumentPath' : '" + files[j].FileName.Substring(DocName.Length + 1) + "'";
                        int res = BalProjectCreation.UploadDocument(clientContext, postedFile, Docitem);
                    }
                }
            }
            catch (Exception ex)
            {

            }
            
        }

        [HttpPost]
        [ActionName("GetEditTimesheet")]
        public JsonResult GetEditTimesheet(string TimesheetId)
        {
            List<object> obj = new List<object>();
            List<TIM_EmployeeTimesheetModel> lstEmployeeTimesheet = new List<TIM_EmployeeTimesheetModel>();
            List<TIM_DocumentLibraryModel> lstDocument = new List<TIM_DocumentLibraryModel>();
            try
            {
                var spContext = SharePointContextProvider.Current.GetSharePointContext(HttpContext);
                using (var clientContext = spContext.CreateUserClientContextForSPHost())
                {
                    lstEmployeeTimesheet = BalEmpTimesheet.GetEmpTimesheetByTimesheetId(clientContext, TimesheetId);
                    var path = spContext.SPHostUrl.Scheme + "://" + spContext.SPHostUrl.Host.ToString();
                    lstDocument = BalDocument.GetDocumentByTimesheetId(clientContext, TimesheetId, path);

                    obj.Add("OK");
                    obj.Add(lstEmployeeTimesheet);
                    obj.Add(lstDocument);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("An error occured while performing action. GUID: {0}", ex.ToString()));
            }
            return Json(obj, JsonRequestBehavior.AllowGet);
        }

        //[HttpPost]
        //[ActionName("ClearTimesheet")]
        //public JsonResult ClearTimesheet( List<TIM_EmployeeTimesheetModel> ClearTimesheet)
        //{
        //    List<object> obj = new List<object>();
        //    int i = 0;
        //    string returnID = "0";
        //    var spContext = SharePointContextProvider.Current.GetSharePointContext(HttpContext);
        //    using (var clientContext = spContext.CreateUserClientContextForSPHost())
        //    {
        //        foreach (var item in ClearTimesheet)
        //        {
        //            returnID = BalEmpTimesheet.DeleteTimesheet(clientContext,item.ID.ToString());
        //            if(returnID == "Delete")
        //                i++;
        //        }
        //        if (i == ClearTimesheet.Count)
        //            obj.Add("OK");
        //    }
        //    return Json(obj, JsonRequestBehavior.AllowGet);
        //}
    }
}