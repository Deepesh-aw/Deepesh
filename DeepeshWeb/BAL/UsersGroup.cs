using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.SharePoint.Client;
using DeepeshWeb.DAL;
using DeepeshWeb.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Web.Mvc;

namespace DeepeshWeb.BAL
{
    public class UsersGroup
    {
        public List<UserGroupModel> GetGroup(ClientContext clientContext, string ID)
        {
            List<UserGroupModel> userGroup = new List<UserGroupModel>();
            JArray jArray = new JArray();
            jArray = RESTGetGroup(clientContext, ID);
            foreach (JObject j in jArray)
            {
                //string ass = j["ID"].ToString();
                //string Title = j["Title"].ToString();
                userGroup.Add(new UserGroupModel
                {
                    ID = j["Id"].ToString(),
                    Title = j["Title"].ToString(),
                    LoginName = j["LoginName"].ToString()
                });
            }
            return userGroup;
        }



        private JArray RESTGetGroup(ClientContext clientContext, string ID)
        {
            RestService restService = new RestService();
            JArray jArray = new JArray();
            jArray = restService.GetAllUserGroupList(clientContext, ID);

            return jArray;
        }


        private JArray RESTGetAllUser(ClientContext clientContext)
        {
            RestService restService = new RestService();
            JArray jArray = new JArray();
            jArray = restService.GetAllUserListList(clientContext);

            return jArray;
        }


    }

}