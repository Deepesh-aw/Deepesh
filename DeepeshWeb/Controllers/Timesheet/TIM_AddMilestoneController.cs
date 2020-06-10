using DeepeshWeb.BAL.Timesheet;
using DeepeshWeb.Models.Timesheet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DeepeshWeb.Controllers.TimeSheet
{
    public class TIM_AddMilestoneController : Controller
    {
        TIM_ProjectCreationBal BalProjectCreation = new TIM_ProjectCreationBal();
        TIM_WorkFlowMasterBal BalWorkflow = new TIM_WorkFlowMasterBal();
        TIM_MilestoneBal BalAddMilestone = new TIM_MilestoneBal();

        // GET: TIM_AddMilestone
        public ActionResult Index()
        {
            List<TIM_ProjectCreationModel> lstProjectCreation = new List<TIM_ProjectCreationModel>();
            try
            {
                int ProjectId = Convert.ToInt32(Request.Cookies["ProjectId"].Value);
                var spContext = SharePointContextProvider.Current.GetSharePointContext(HttpContext);
                using (var clientContext = spContext.CreateUserClientContextForSPHost())
                {
                    lstProjectCreation = BalProjectCreation.GetProjectCreationById(clientContext, ProjectId);
                }
            }
            catch(Exception ex)
            {
                throw new Exception(string.Format("An error occured while performing action. GUID: {0}", ex.ToString()));
            }

            return View(lstProjectCreation[0]);
        }

        [HttpPost]
        [ActionName("AddMilestone")]
        public JsonResult AddMilestone(List<TIM_MilestoneModel> AddMilestone)
        {
            List<object> obj = new List<object>();
            int i = 0;
            var spContext = SharePointContextProvider.Current.GetSharePointContext(HttpContext);
            using (var clientContext = spContext.CreateUserClientContextForSPHost())
            {
                List<TIM_WorkFlowMasterModel> lstWorkFlow = new List<TIM_WorkFlowMasterModel>();
                lstWorkFlow = BalWorkflow.GetWorkFlowForAddMilestone(clientContext);
                string returnID = "0";
                foreach (var item in AddMilestone)
                {
                    string arr = String.Join(",", item.Members); 
                    
                    string itemdata = " 'MileStone': '" + item.MileStone + "'";
                    itemdata += " ,'MembersId': {'results': ["+arr+"] }";
                    itemdata += " ,'Description': '" + item.Description + "'";
                    itemdata += " ,'StartDate': '" + item.StartDate + "'";
                    itemdata += " ,'EndDate': '" + item.EndDate + "'";
                    itemdata += " ,'NoOfDays': '" + item.NoOfDays + "'";
                    itemdata += " ,'ProjectId': '" + item.Project + "'";
                    itemdata += " ,'ProjectManagerId': '" + item.ProjectManager + "'";
                    itemdata += " ,'MembersText': '" + item.MembersText + "'";

                    if (lstWorkFlow.Count > 0)
                    {
                        itemdata += " ,'StatusId': '" + lstWorkFlow[0].ToStatusID + "'";
                        itemdata += " ,'InternalStatus': '" + lstWorkFlow[0].InternalStatus + "'";
                    }
                    returnID = BalAddMilestone.SaveMilestone(clientContext, itemdata);
                    if (Convert.ToInt32(returnID) > 0)
                        i++;
                    
                }
                if (i == AddMilestone.Count)
                    obj.Add("OK");
            }
            return Json(obj, JsonRequestBehavior.AllowGet);
        }
    }
}