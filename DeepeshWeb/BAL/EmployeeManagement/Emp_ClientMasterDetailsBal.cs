using Microsoft.SharePoint.Client;
using Newtonsoft.Json.Linq;
using DeepeshWeb.DAL;
using DeepeshWeb.Models;
using DeepeshWeb.Models.EmployeeManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DeepeshWeb.BAL.EmployeeManagement
{
    public class Emp_ClientMasterDetailsBal
    {
        public List<Emp_ClientMasterDetailsModel> GetClient(ClientContext clientContext)
        {
            List<Emp_ClientMasterDetailsModel> lstClient = new List<Emp_ClientMasterDetailsModel>();
            JArray jArray = RESTGet(clientContext,null);
            foreach (JObject j in jArray)
            {
                lstClient.Add(new Emp_ClientMasterDetailsModel
                {
                    ID = Convert.ToInt32(j["Id"]),
                    ClientName = j["ClientName"].ToString(),
                }); ;
            }
            return lstClient;
        }


        #region Get all client
        public List<Emp_ClientMasterDetailsModel> GetAllClient(ClientContext clientContext)
        {
            List<Emp_ClientMasterDetailsModel> emp_ClientMasterDetailsModels = new List<Emp_ClientMasterDetailsModel>();

            JArray jArray = RESTGet(clientContext, null);

            foreach (JObject j in jArray)
            {
                emp_ClientMasterDetailsModels.Add(new Emp_ClientMasterDetailsModel
                {
                    ID = Convert.ToInt32(j["ID"]),
                    ClientName = j["ClientName"].ToString(),
                    ClientAddress = j["ClientAddress"].ToString(),
                    ClientContact = j["ClientContact"].ToString(),
                    ClientMailID = j["ClientMailID"].ToString(),
                    ClientDesignation = j["ClientDesignation"].ToString(),
                    ClientGSTNO = j["ClientGSTNO"].ToString(),
                    ClientPanCardNo = j["ClientPanCardNo"].ToString(),
                    ClientState = j["ClientState"].ToString(),
                    ClientCity = j["ClientCity"].ToString(),
                    ClientRemark = j["ClientRemark"].ToString(),
                    ClientCountry = j["ClientCountry"].ToString()
                });
            }


            return emp_ClientMasterDetailsModels;

        }
        #endregion

        #region Get client by ID
        public List<Emp_ClientMasterDetailsModel> GetClientById(ClientContext clientContext, string id)
        {
            List<Emp_ClientMasterDetailsModel> emp_ClientMasterDetailsModels = new List<Emp_ClientMasterDetailsModel>();
            var filter = "ID eq " + id;
            JArray jArray = RESTGet(clientContext, filter);

            foreach (JObject j in jArray)
            {
                emp_ClientMasterDetailsModels.Add(new Emp_ClientMasterDetailsModel
                {
                    ID = Convert.ToInt32(j["ID"]),
                    ClientName = j["ClientName"].ToString(),
                    ClientAddress = j["ClientAddress"].ToString(),
                    ClientContact = j["ClientContact"].ToString(),
                    ClientMailID = j["ClientMailID"].ToString(),
                    ClientDesignation = j["ClientDesignation"].ToString(),
                    ClientGSTNO = j["ClientGSTNO"].ToString(),
                    ClientPanCardNo = j["ClientPanCardNo"].ToString(),
                    ClientState = j["ClientState"].ToString(),
                    ClientCity = j["ClientCity"].ToString(),
                    ClientRemark = j["ClientRemark"].ToString(),
                    ClientCountry = Convert.ToString(j["ClientCountry"]["CountryName"]).Trim(),
                });
            }


            return emp_ClientMasterDetailsModels;

        } 
        #endregion

        public string SaveClient(ClientContext clientContext, string ItemData)
        {

            string response = RESTSave(clientContext, ItemData);

            return response;
        }

        public string UpdateClient(ClientContext clientContext, string ItemData, string ID)
        {
            string response = RESTUpdate(clientContext, ItemData, ID);
            return response;
        }

        #region Common method of Get Client from list
        private JArray RESTGet(ClientContext clientContext, string filter)
        {
            RestService restService = new RestService();
            JArray jArray = new JArray();
            RESTOption rESTOption = new RESTOption();

            rESTOption.filter = filter;
            rESTOption.select = "ID,ClientName,ClientAddress,ClientContact,ClientMailID,ClientDesignation,ClientGSTNO,ClientPanCardNo,ClientState,ClientCity,ClientRemark,ClientCountry/ID,ClientCountry/CountryName";
            rESTOption.expand = "ClientCountry";
            rESTOption.top = "5000";
            jArray = restService.GetAllItemFromList(clientContext, "Emp_ClientMasterDetails", rESTOption);

            return jArray;
        } 
        #endregion

        private string RESTSave(ClientContext clientContext, string ItemData)
        {
            RestService restService = new RestService();

            return restService.SaveItem(clientContext, "Emp_ClientMasterDetails", ItemData);
        }

        private string RESTUpdate(ClientContext clientContext, string ItemData, string ID)
        {
            RestService restService = new RestService();

            return restService.UpdateItem(clientContext, "Emp_ClientMasterDetails", ItemData, ID);
        }
    }
}