using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.SharePoint.Client;
using DeepeshWeb.Models;
using DeepeshWeb.DAL;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Web.Mvc;

namespace DeepeshWeb.BAL
{
    public class NavigationBal
    {
        public List<NavigationModel> GetAllApproverType(ClientContext clientContext, List<UserGroupModel> groupname, string UserID)
        {
            List<NavigationModel> navigation = new List<NavigationModel>();
            string dinamicurl = "";
            dinamicurl = dinamicurl + " Permission/ID eq " + UserID + " ";

            for (int i = 0; i < groupname.Count; i++)
            {

                if (i == groupname.Count - 1)
                {
                    dinamicurl = dinamicurl + " or Permission/ID eq " + groupname[i].ID + " ";
                }
                else
                {
                    dinamicurl = dinamicurl + " or  Permission/ID eq " + groupname[i].ID + " ";

                }

            }

            string filter = "ShowMenu eq 'Yes' and(" + dinamicurl + ")";
            JArray jArray = RESTGet(clientContext, filter);
            foreach (JObject j in jArray)
            {
                navigation.Add(new NavigationModel
                {
                    ID = Convert.ToInt32(j["ID"]),
                    ManuName = j["MenuName"] == null ? "" : Convert.ToString(j["MenuName"]).Trim(),
                    URL = j["URL"] == null ? "" : Convert.ToString(j["URL"]).Trim() + "?SPHostUrl=" + clientContext.Url,
                    ParentManu = j["ParentMenuId"] == null ? "" : Convert.ToString(j["ParentMenuId"]).Trim(),
                    ParentMenuName = j["ParentMenuName"]["MenuName"] == null ? "" : j["ParentMenuName"]["MenuName"].ToString(),
                    ParentMenuNameID = j["ParentMenuName"]["ID"] == null ? "" : j["ParentMenuName"]["ID"].ToString()
                });

            }

            return navigation;
        }



        private JArray RESTGet(ClientContext clientContext, string filter)
        {
            RestService restService = new RestService();
            JArray jArray = new JArray();
            RESTOption rESTOption = new RESTOption();
            rESTOption.filter = filter;
            rESTOption.select = "URL,MenuName,ID,ParentMenuId,Permission/Title,Permission/ID,ParentMenuName/ID,ParentMenuName/MenuName,OrderNo";
            rESTOption.expand = "Permission,ParentMenuName";
            rESTOption.top = "5000";


            jArray = restService.GetAllItemFromList(clientContext, "Navigation", rESTOption);

            return jArray;
        }

    }

}