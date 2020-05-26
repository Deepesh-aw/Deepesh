using System;

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.SharePoint.Client;
using DeepeshWeb.DAL;
using DeepeshWeb.BAL;
using DeepeshWeb.Models;

namespace DeepeshWeb.Controllers
{
    public class NavigationController : Controller
    {
        // GET: Navigation
        public ActionResult Index()
        {
            if (Session["Navigation"].ToString() == "" || Session["Navigation"].ToString() == null)
            {

                string ht = "";

                List<NavigationModel> Duplicatenavigation = new List<NavigationModel>();
                List<NavigationModel> navigation = new List<NavigationModel>();
                List<NavigationModel> Parrentnavigation = new List<NavigationModel>();
                List<NavigationModel> Subnavigation = new List<NavigationModel>();

                var spContext = SharePointContextProvider.Current.GetSharePointContext(HttpContext);
                using (var clientContext = spContext.CreateUserClientContextForSPHost())
                {

                    UsersGroup usersGroup = new UsersGroup();
                    List<UserGroupModel> userGroupModels = new List<UserGroupModel>();
                    userGroupModels = usersGroup.GetGroup(clientContext, Session["UserID"].ToString());
                    NavigationBal navigationBal = new NavigationBal();


                    Duplicatenavigation = navigationBal.GetAllApproverType(clientContext, userGroupModels, Session["UserID"].ToString());

                    navigation = Duplicatenavigation.GroupBy(i => i.ID)
                                    .Select(g => g.First()).ToList();

                    for (int i = 0; i < navigation.Count; i++)
                    {
                        if (navigation[i].ParentManu == "0")
                        {
                            Parrentnavigation.Add(navigation[i]);
                        }
                        else
                        {
                            Subnavigation.Add(navigation[i]);
                        }
                    }


                    ht += "<nav class='bottom-navbar'>";
                    ht += "<div class='container'>";
                    ht += "<ul class='nav page-navigation'>";
                    for (int i = 0; i < Parrentnavigation.Count; i++)
                    {
                        List<NavigationModel> Temp = new List<NavigationModel>();

                        for (int x = 0; x < Subnavigation.Count; x++)
                        {
                            if (Convert.ToString(Parrentnavigation[i].ID) == Subnavigation[x].ParentMenuNameID)
                            {
                                Temp.Add(Subnavigation[x]);
                            }
                        }

                        if (Temp.Count > 0)
                        {
                            ht += "<li class='nav-item'>";
                            ht += "<a href = '#' class='nav-link'>";
                            ht += "<i class='mdi mdi-file-document-box-outline menu-icon'></i>";
                            ht += "<span class='menu-title'>" + Parrentnavigation[i].ManuName + "</span>";
                            ht += " <i class='menu-arrow'></i></a>";
                            ht += "<div class='submenu'>";
                            ht += "<ul class='submenu-item'>";
                            for (int y = 0; y < Temp.Count; y++)
                            {
                                ht += "<li class='nav-item'><a class='nav-link' href='" + Temp[y].URL + "'>" + Temp[y].ManuName + "</a></li>";
                            }
                            ht += "</ul>";
                            ht += "</div>";
                            ht += "</li>";
                        }
                        else
                        {

                            ht += "<li class='nav-item'>";
                            ht += "<a class='nav-link' href='" + Parrentnavigation[i].URL + "'>";
                            ht += "<i class='mdi mdi-view-dashboard-outline menu-icon'></i>";
                            ht += "<span class='menu-title'>" + Parrentnavigation[i].ManuName + "</span>";
                            ht += "</a ></li >";

                        }



                    }
                    ht += "</ul>";
                    ht += "</div>";
                    ht += "</nav>";

                    Session["Navigation"] = ht;
                }

            }




            return Json(Session["Navigation"].ToString(), JsonRequestBehavior.AllowGet);

        }
    }
}