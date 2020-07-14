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
    public class TIM_WorkFlowMasterBal
    {
        public List<TIM_WorkFlowMasterModel> GetWorkFlowForTimesheetApprove(ClientContext clientContext)
        {
            List<TIM_WorkFlowMasterModel> _WorkFlowModel = new List<TIM_WorkFlowMasterModel>();

            string filter = "TransactionType eq 'TimesheetApprove' and Action eq 'Forward'";

            JArray jArray = RESTGet(clientContext, filter);

            foreach (JObject j in jArray)
            {
                _WorkFlowModel.Add(new TIM_WorkFlowMasterModel
                {
                    TransactionType = j["TransactionType"] == null ? "" : j["TransactionType"].ToString(),
                    FromStatus = j["FromStatus"]["StatusName"] == null ? "" : Convert.ToString(j["FromStatus"]["StatusName"].ToString()),
                    ToStatus = j["ToStatus"]["StatusName"] == null ? "" : Convert.ToString(j["ToStatus"]["StatusName"].ToString()),
                    ToStatusID = j["ToStatus"] == null ? "" : Convert.ToString(j["ToStatus"]["ID"].ToString()),
                    InternalStatus = j["InternalStatus"] == null ? "" : Convert.ToString(j["InternalStatus"].ToString())
                });
            }

            return _WorkFlowModel;
        }

        public List<TIM_WorkFlowMasterModel> GetWorkFlowForTimesheetReject(ClientContext clientContext)
        {
            List<TIM_WorkFlowMasterModel> _WorkFlowModel = new List<TIM_WorkFlowMasterModel>();

            string filter = "TransactionType eq 'TimesheetReject' and Action eq 'Forward'";

            JArray jArray = RESTGet(clientContext, filter);

            foreach (JObject j in jArray)
            {
                _WorkFlowModel.Add(new TIM_WorkFlowMasterModel
                {
                    TransactionType = j["TransactionType"] == null ? "" : j["TransactionType"].ToString(),
                    FromStatus = j["FromStatus"]["StatusName"] == null ? "" : Convert.ToString(j["FromStatus"]["StatusName"].ToString()),
                    ToStatus = j["ToStatus"]["StatusName"] == null ? "" : Convert.ToString(j["ToStatus"]["StatusName"].ToString()),
                    ToStatusID = j["ToStatus"] == null ? "" : Convert.ToString(j["ToStatus"]["ID"].ToString()),
                    InternalStatus = j["InternalStatus"] == null ? "" : Convert.ToString(j["InternalStatus"].ToString())
                });
            }

            return _WorkFlowModel;
        }


        public List<TIM_WorkFlowMasterModel> GetWorkFlowForProjectCreation(ClientContext clientContext)
        {
            List<TIM_WorkFlowMasterModel> _WorkFlowModel = new List<TIM_WorkFlowMasterModel>();

            string filter = "TransactionType eq 'ProjectCreation' and Action eq 'Forward'";

            JArray jArray = RESTGet(clientContext, filter);

            foreach (JObject j in jArray)
            {
                _WorkFlowModel.Add(new TIM_WorkFlowMasterModel
                {
                    TransactionType = j["TransactionType"] == null ? "" : j["TransactionType"].ToString(),
                    FromStatus = j["FromStatus"]["StatusName"] == null ? "" : Convert.ToString(j["FromStatus"]["StatusName"].ToString()),
                    ToStatus = j["ToStatus"]["StatusName"] == null ? "" : Convert.ToString(j["ToStatus"]["StatusName"].ToString()),
                    ToStatusID = j["ToStatus"] == null ? "" : Convert.ToString(j["ToStatus"]["ID"].ToString()),
                    InternalStatus = j["InternalStatus"] == null ? "" : Convert.ToString(j["InternalStatus"].ToString())
                });
            }

            return _WorkFlowModel;
        }

        public List<TIM_WorkFlowMasterModel> GetWorkFlowForProjectDeletion(ClientContext clientContext)
        {
            List<TIM_WorkFlowMasterModel> _WorkFlowModel = new List<TIM_WorkFlowMasterModel>();

            string filter = "TransactionType eq 'ProjectDeletion' and Action eq 'Forward'";

            JArray jArray = RESTGet(clientContext, filter);

            foreach (JObject j in jArray)
            {
                _WorkFlowModel.Add(new TIM_WorkFlowMasterModel
                {
                    TransactionType = j["TransactionType"] == null ? "" : j["TransactionType"].ToString(),
                    FromStatus = j["FromStatus"]["StatusName"] == null ? "" : Convert.ToString(j["FromStatus"]["StatusName"].ToString()),
                    ToStatus = j["ToStatus"]["StatusName"] == null ? "" : Convert.ToString(j["ToStatus"]["StatusName"].ToString()),
                    ToStatusID = j["ToStatus"] == null ? "" : Convert.ToString(j["ToStatus"]["ID"].ToString()),
                    InternalStatus = j["InternalStatus"] == null ? "" : Convert.ToString(j["InternalStatus"].ToString())
                });
            }

            return _WorkFlowModel;
        }

        public List<TIM_WorkFlowMasterModel> GetWorkFlowForMilestoneDeletion(ClientContext clientContext)
        {
            List<TIM_WorkFlowMasterModel> _WorkFlowModel = new List<TIM_WorkFlowMasterModel>();

            string filter = "TransactionType eq 'MilestoneDeletion' and Action eq 'Forward'";

            JArray jArray = RESTGet(clientContext, filter);

            foreach (JObject j in jArray)
            {
                _WorkFlowModel.Add(new TIM_WorkFlowMasterModel
                {
                    TransactionType = j["TransactionType"] == null ? "" : j["TransactionType"].ToString(),
                    FromStatus = j["FromStatus"]["StatusName"] == null ? "" : Convert.ToString(j["FromStatus"]["StatusName"].ToString()),
                    ToStatus = j["ToStatus"]["StatusName"] == null ? "" : Convert.ToString(j["ToStatus"]["StatusName"].ToString()),
                    ToStatusID = j["ToStatus"] == null ? "" : Convert.ToString(j["ToStatus"]["ID"].ToString()),
                    InternalStatus = j["InternalStatus"] == null ? "" : Convert.ToString(j["InternalStatus"].ToString())
                });
            }

            return _WorkFlowModel;
        }

        public List<TIM_WorkFlowMasterModel> GetWorkFlowForTaskDeletion(ClientContext clientContext)
        {
            List<TIM_WorkFlowMasterModel> _WorkFlowModel = new List<TIM_WorkFlowMasterModel>();

            string filter = "TransactionType eq 'TaskDeletion' and Action eq 'Forward'";

            JArray jArray = RESTGet(clientContext, filter);

            foreach (JObject j in jArray)
            {
                _WorkFlowModel.Add(new TIM_WorkFlowMasterModel
                {
                    TransactionType = j["TransactionType"] == null ? "" : j["TransactionType"].ToString(),
                    FromStatus = j["FromStatus"]["StatusName"] == null ? "" : Convert.ToString(j["FromStatus"]["StatusName"].ToString()),
                    ToStatus = j["ToStatus"]["StatusName"] == null ? "" : Convert.ToString(j["ToStatus"]["StatusName"].ToString()),
                    ToStatusID = j["ToStatus"] == null ? "" : Convert.ToString(j["ToStatus"]["ID"].ToString()),
                    InternalStatus = j["InternalStatus"] == null ? "" : Convert.ToString(j["InternalStatus"].ToString())
                });
            }

            return _WorkFlowModel;
        }

        public List<TIM_WorkFlowMasterModel> GetWorkFlowForSubTaskDeletion(ClientContext clientContext)
        {
            List<TIM_WorkFlowMasterModel> _WorkFlowModel = new List<TIM_WorkFlowMasterModel>();

            string filter = "TransactionType eq 'SubTaskDeletion' and Action eq 'Forward'";

            JArray jArray = RESTGet(clientContext, filter);

            foreach (JObject j in jArray)
            {
                _WorkFlowModel.Add(new TIM_WorkFlowMasterModel
                {
                    TransactionType = j["TransactionType"] == null ? "" : j["TransactionType"].ToString(),
                    FromStatus = j["FromStatus"]["StatusName"] == null ? "" : Convert.ToString(j["FromStatus"]["StatusName"].ToString()),
                    ToStatus = j["ToStatus"]["StatusName"] == null ? "" : Convert.ToString(j["ToStatus"]["StatusName"].ToString()),
                    ToStatusID = j["ToStatus"] == null ? "" : Convert.ToString(j["ToStatus"]["ID"].ToString()),
                    InternalStatus = j["InternalStatus"] == null ? "" : Convert.ToString(j["InternalStatus"].ToString())
                });
            }

            return _WorkFlowModel;
        }

        public List<TIM_WorkFlowMasterModel> GetWorkFlowForAddMilestone(ClientContext clientContext)
        {
            List<TIM_WorkFlowMasterModel> _WorkFlowModel = new List<TIM_WorkFlowMasterModel>();

            string filter = "TransactionType eq 'MilestoneCreation' and Action eq 'Forward'";

            JArray jArray = RESTGet(clientContext, filter);

            foreach (JObject j in jArray)
            {
                _WorkFlowModel.Add(new TIM_WorkFlowMasterModel
                {
                    TransactionType = j["TransactionType"] == null ? "" : j["TransactionType"].ToString(),
                    FromStatus = j["FromStatus"]["StatusName"] == null ? "" : Convert.ToString(j["FromStatus"]["StatusName"].ToString()),
                    ToStatus = j["ToStatus"]["StatusName"] == null ? "" : Convert.ToString(j["ToStatus"]["StatusName"].ToString()),
                    ToStatusID = j["ToStatus"] == null ? "" : Convert.ToString(j["ToStatus"]["ID"].ToString()),
                    InternalStatus = j["InternalStatus"] == null ? "" : Convert.ToString(j["InternalStatus"].ToString())
                });
            }

            return _WorkFlowModel;
        }

        public List<TIM_WorkFlowMasterModel> GetWorkFlowForAddTask(ClientContext clientContext)
        {
            List<TIM_WorkFlowMasterModel> _WorkFlowModel = new List<TIM_WorkFlowMasterModel>();

            string filter = "TransactionType eq 'TaskCreation' and Action eq 'Forward'";

            JArray jArray = RESTGet(clientContext, filter);

            foreach (JObject j in jArray)
            {
                _WorkFlowModel.Add(new TIM_WorkFlowMasterModel
                {
                    TransactionType = j["TransactionType"] == null ? "" : j["TransactionType"].ToString(),
                    FromStatus = j["FromStatus"]["StatusName"] == null ? "" : Convert.ToString(j["FromStatus"]["StatusName"].ToString()),
                    ToStatus = j["ToStatus"]["StatusName"] == null ? "" : Convert.ToString(j["ToStatus"]["StatusName"].ToString()),
                    ToStatusID = j["ToStatus"] == null ? "" : Convert.ToString(j["ToStatus"]["ID"].ToString()),
                    InternalStatus = j["InternalStatus"] == null ? "" : Convert.ToString(j["InternalStatus"].ToString())
                });
            }

            return _WorkFlowModel;
        }

        public List<TIM_WorkFlowMasterModel> GetWorkFlowForAddSubTask(ClientContext clientContext)
        {
            List<TIM_WorkFlowMasterModel> _WorkFlowModel = new List<TIM_WorkFlowMasterModel>();

            string filter = "TransactionType eq 'SubTaskCreation' and Action eq 'Forward'";

            JArray jArray = RESTGet(clientContext, filter);

            foreach (JObject j in jArray)
            {
                _WorkFlowModel.Add(new TIM_WorkFlowMasterModel
                {
                    TransactionType = j["TransactionType"] == null ? "" : j["TransactionType"].ToString(),
                    FromStatus = j["FromStatus"]["StatusName"] == null ? "" : Convert.ToString(j["FromStatus"]["StatusName"].ToString()),
                    ToStatus = j["ToStatus"]["StatusName"] == null ? "" : Convert.ToString(j["ToStatus"]["StatusName"].ToString()),
                    ToStatusID = j["ToStatus"] == null ? "" : Convert.ToString(j["ToStatus"]["ID"].ToString()),
                    InternalStatus = j["InternalStatus"] == null ? "" : Convert.ToString(j["InternalStatus"].ToString())
                });
            }

            return _WorkFlowModel;
        }

        private JArray RESTGet(ClientContext clientContext, string filter)
        {
            RestService restService = new RestService();
            JArray jArray = new JArray();
            RESTOption rESTOption = new RESTOption();

            rESTOption.filter = filter;
            rESTOption.select = "ID,TransactionType,Action,FromStatus/StatusName,ToStatus/StatusName,ToStatus/ID,FromStatus/ID,InternalStatus";
            rESTOption.expand = "FromStatus,ToStatus";
            rESTOption.top = "5000";


            jArray = restService.GetAllItemFromList(clientContext, "TIM_WorkFlowMaster", rESTOption);

            return jArray;
        }

    }
}