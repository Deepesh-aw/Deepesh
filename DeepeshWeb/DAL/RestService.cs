using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.SharePoint.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using DeepeshWeb.Models;
using System.Xml;

namespace DeepeshWeb.DAL
{
    public class RestService
    {
        string accessToken;


        private string URLBuilder(string url, RESTOption restoption)
        {


            if (restoption.filter != null)
            {
                url += ((url.IndexOf('?') > -1) ? "&" : "?") + "$filter=" + restoption.filter;
            }


            if (restoption.select != null)
            {
                url += ((url.IndexOf('?') > -1) ? "&" : "?") + "$select=" + restoption.select;
            }


            if (restoption.expand != null)
            {
                url += ((url.IndexOf('?') > -1) ? "&" : "?") + "$expand=" + restoption.expand;
            }


            if (restoption.top != null)
            {
                url += ((url.IndexOf('?') > -1) ? "&" : "?") + "$top=" + restoption.top;
            }

            if (restoption.orderby != null)
            {
                url += ((url.IndexOf('?') > -1) ? "&" : "?") + "$orderby=" + restoption.orderby;
            }




            return url;
        }


        public JArray GetAllItemFromList(ClientContext ctx, string listname, RESTOption restoption)
        {
            try
            {
                RetrieveAccessToken(ctx);

                var Url = ctx.Url + "/_api/web/lists/getByTitle('" + listname + "')/items";
                Url = URLBuilder(Url, restoption);

                HttpWebRequest request = HttpWebRequest.CreateHttp(Url);
                request.Accept = "application/json;odata=verbose";
                request.Headers.Add("Authorization", accessToken);
                Stream webStream = request.GetResponse().GetResponseStream();
                StreamReader responseReader = new StreamReader(webStream);
                string response = responseReader.ReadToEnd();
                JObject jobj = JObject.Parse(response);
                JArray jarr = (JArray)jobj["d"]["results"];


                return jarr;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("An error occured while reading data. GUID: {0}", ex.ToString()));
            }
        }

        public JArray GetItemByID(ClientContext ctx, string listname, RESTOption restoption, string ID)
        {
            try
            {
                RetrieveAccessToken(ctx);

                var Url = ctx.Url + "/_api/web/lists/getByTitle('" + listname + "')/items(" + ID + ")";
                Url = URLBuilder(Url, restoption);

                HttpWebRequest request = HttpWebRequest.CreateHttp(Url);
                request.Accept = "application/json;odata=verbose";
                request.Headers.Add("Authorization", accessToken);
                Stream webStream = request.GetResponse().GetResponseStream();
                StreamReader responseReader = new StreamReader(webStream);
                string response = responseReader.ReadToEnd();
                JObject jobj = JObject.Parse(response);
                JArray jarr = (JArray)jobj["d"]["results"];


                return jarr;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("An error occured while reading data. GUID: {0}", ex.ToString()));
            }
        }


        public string SaveItem(ClientContext ctx, string ListName, string Itemdate)
        {
            var responseText = "";
            try
            {
                RetrieveAccessToken(ctx);
                XmlNamespaceManager xmlnspm = AddXmlNameSpaces();
                string listUrl = RestUrlList(ListName);

                string Metadata = GetEntityTypeName(ctx.Url, listUrl, xmlnspm);


                // string Metadata = String.Format("SP.Data.{0}ListItem", ListName);

                string listname = "'__metadata':{ 'type': '" + Metadata + "'}";

                string itemPostBody = "{" + listname + "," + Itemdate + "}";
                Byte[] itemPostData = System.Text.Encoding.ASCII.GetBytes(itemPostBody);
                HttpWebRequest ItemRequest;
                ItemRequest = (HttpWebRequest)HttpWebRequest.Create(String.Format("{0}_api/web/lists/getByTitle('" + ListName + "')/items", ctx.Url));
                ItemRequest.Method = "POST";
                ItemRequest.ContentLength = itemPostBody.Length;
                ItemRequest.ContentType = "application/json;odata=verbose;charset=UTF-8";
                ItemRequest.Accept = "application/json;odata=verbose";
                ItemRequest.Headers.Add("Authorization", accessToken);

                using (Stream itemRequestStream = ItemRequest.GetRequestStream())
                {
                    itemRequestStream.Write(itemPostData, 0, itemPostData.Length);
                    itemRequestStream.Close();
                }
                using (HttpWebResponse itemResponse = (HttpWebResponse)ItemRequest.GetResponse())
                {
                    using (StreamReader responseReader = new StreamReader(itemResponse.GetResponseStream()))
                    {
                        string response = responseReader.ReadToEnd();
                        JObject jobj = JObject.Parse(response);
                        responseText = Convert.ToString(jobj["d"]["Id"]);
                    }
                }
                return responseText;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("An error occured while reading data. GUID: {0}", ex.ToString()));
            }
        }

        public string UpdateItem(ClientContext ctx, string ListName, string Itemdate, string ID)
        {
            var responseText = "";
            try
            {
                RetrieveAccessToken(ctx);

                string listUrl = RestUrlList(ListName);
                XmlNamespaceManager xmlnspm = AddXmlNameSpaces();
                string Metadata = GetEntityTypeName(ctx.Url, listUrl, xmlnspm);

                string listname = "'__metadata':{ 'type': '" + Metadata + "'}";

                string itemPostBody = "{" + listname + "," + Itemdate + "}";
                Byte[] itemPostData = System.Text.Encoding.ASCII.GetBytes(itemPostBody);
                HttpWebRequest ItemRequest;
                ItemRequest = (HttpWebRequest)HttpWebRequest.Create(String.Format("{0}_api/web/lists/getByTitle('" + ListName + "')/items(" + ID + ")", ctx.Url));
                ItemRequest.Method = "POST";
                ItemRequest.ContentLength = itemPostBody.Length;
                ItemRequest.ContentType = "application/json;odata=verbose;charset=UTF-8";
                ItemRequest.Accept = "application/json;odata=verbose";

                ItemRequest.Headers.Add("Authorization", accessToken);
                ItemRequest.Headers.Add("X-HTTP-Method", "MERGE");
                ItemRequest.Headers.Add("If-Match", "*");

                using (Stream itemRequestStream = ItemRequest.GetRequestStream())
                {
                    itemRequestStream.Write(itemPostData, 0, itemPostData.Length);
                    itemRequestStream.Close();
                }
                using (HttpWebResponse itemResponse = (HttpWebResponse)ItemRequest.GetResponse())
                {
                    using (StreamReader responseReader = new StreamReader(itemResponse.GetResponseStream()))
                    {
                        //string response = responseReader.ReadToEnd();
                        //JObject jobj = JObject.Parse(response);
                        //responseText = Convert.ToString(jobj["d"]["Id"]);
                        responseText = "Update";
                    }
                }
                return responseText;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("An error occured while reading data. GUID: {0}", ex.ToString()));
            }
        }

        public JArray GetAllUserListList(ClientContext ctx)
        {
            try
            {
                RetrieveAccessToken(ctx);

                var Url = ctx.Url + "/_api/web/siteusers";

                HttpWebRequest request = HttpWebRequest.CreateHttp(Url);
                request.Accept = "application/json;odata=verbose";
                request.Headers.Add("Authorization", accessToken);
                Stream webStream = request.GetResponse().GetResponseStream();
                StreamReader responseReader = new StreamReader(webStream);
                string response = responseReader.ReadToEnd();
                JObject jobj = JObject.Parse(response);
                JArray jarr = (JArray)jobj["d"]["results"];


                return jarr;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("An error occured while reading data. GUID: {0}", ex.ToString()));
            }
        }

        public JArray GetAllUserGroupList(ClientContext ctx, string ID)
        {
            try
            {
                RetrieveAccessToken(ctx);

                var Url = ctx.Url + "/_api/Web/GetUserById(" + ID + ")/Groups";

                HttpWebRequest request = HttpWebRequest.CreateHttp(Url);
                request.Accept = "application/json;odata=verbose";
                request.Headers.Add("Authorization", accessToken);
                Stream webStream = request.GetResponse().GetResponseStream();
                StreamReader responseReader = new StreamReader(webStream);
                string response = responseReader.ReadToEnd();
                JObject jobj = JObject.Parse(response);
                JArray jarr = (JArray)jobj["d"]["results"];


                return jarr;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("An error occured while reading data. GUID: {0}", ex.ToString()));
            }
        }

        private void RetrieveAccessToken(ClientContext ctx)
        {
            ctx.ExecutingWebRequest += ctx_ExecutingWebRequest;
            ctx.ExecuteQuery();
        }
        private void ctx_ExecutingWebRequest(object sender, WebRequestEventArgs e)
        {
            accessToken = e.WebRequestExecutor.RequestHeaders.Get("Authorization");
        }

        public string GetEntityTypeName(string siteUrl, string listUrl, XmlNamespaceManager xmlnspm)
        {
            string entitytypeName = string.Empty;
            try
            {
                HttpWebRequest listRequest = (HttpWebRequest)HttpWebRequest.Create(siteUrl + listUrl);
                listRequest.Method = "GET";
                listRequest.Accept = "application/atom+xml";
                listRequest.ContentType = "application/atom+xml;type=entry";
                // listRequest.Credentials = System.Net.CredentialCache.DefaultCredentials;
                listRequest.Headers.Add("Authorization", accessToken);
                HttpWebResponse listResponse = (HttpWebResponse)listRequest.GetResponse();
                using (StreamReader listReader = new StreamReader(listResponse.GetResponseStream()))
                {
                    var listXml = new XmlDocument();
                    listXml.LoadXml(listReader.ReadToEnd());
                    var entityTypeNode = listXml.SelectSingleNode("//atom:entry/atom:content/m:properties/d:ListItemEntityTypeFullName", xmlnspm);
                    var listNameNode = listXml.SelectSingleNode("//atom:entry/atom:content/m:properties/d:Title", xmlnspm);
                    entitytypeName = entityTypeNode.InnerXml;
                }
                //listRequest.KeepAlive = false;

            }
            catch (Exception ex)
            {
                //string method = string.Concat(MethodBase.GetCurrentMethod().Name, " - SiteUrl - ", siteUrl, " - ListUrl - ", listUrl);
                //string guid = WriteException(ex, method, MethodBase.GetCurrentMethod().DeclaringType.Name, string.Empty);
                //throw new Exception(string.Format("An error occured while reading data. GUID: {0}", guid));
            }
            return entitytypeName;
        }

        public static string RestUrlList(string listName)
        {
            return string.Format("/_api/web/lists/GetByTitle('{0}')", listName);
        }

        public static XmlNamespaceManager AddXmlNameSpaces()
        {

            try
            {
                XmlNamespaceManager xmlnspm = new XmlNamespaceManager(new NameTable());

                xmlnspm.AddNamespace("atom", "http://www.w3.org/2005/Atom");
                xmlnspm.AddNamespace("d", "http://schemas.microsoft.com/ado/2007/08/dataservices");
                xmlnspm.AddNamespace("m", "http://schemas.microsoft.com/ado/2007/08/dataservices/metadata");
                return xmlnspm;

            }
            catch (Exception ex)
            {
                // string guid = WriteException(ex, MethodBase.GetCurrentMethod().Name, MethodBase.GetCurrentMethod().DeclaringType.Name, string.Empty);
                throw new Exception(string.Format("An error occured while reading data. GUID: {0}", ""));
            }
        }

        public string DeleteItem(ClientContext ctx, string ListName, string ID)
        {
            var responseText = "";
            try
            {

                RetrieveAccessToken(ctx);
                XmlNamespaceManager xmlnspm = AddXmlNameSpaces();
                var formDigestNode = GetFormDigest(ctx.Url, xmlnspm);
                string formDigest = formDigestNode.InnerXml;

                string listUrl = RestUrlList(ListName);

                var Url = ctx.Url + "/_api/web/lists/getByTitle('" + ListName + "')/items(" + ID + ")";
                HttpWebRequest ItemRequest;
                ItemRequest = (HttpWebRequest)HttpWebRequest.Create(Url);
                ItemRequest.Method = "POST";
                ItemRequest.Headers.Add("X-HTTP-Method", "DELETE");
                ItemRequest.Headers.Add("IF-MATCH", "*");
                ItemRequest.Headers.Add("Authorization", accessToken);
                ItemRequest.Headers.Add("X-RequestDigest", formDigest);
                using (Stream itemRequestStream = ItemRequest.GetRequestStream())
                {
                    itemRequestStream.Close();
                }
                using (HttpWebResponse itemResponse = (HttpWebResponse)ItemRequest.GetResponse())
                {
                    responseText = "Delete";
                }

                return responseText;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("An error occured while reading data. GUID: {0}", ex.ToString()));
            }
        }

        public XmlNode GetFormDigest(string siteUrl, XmlNamespaceManager xmlnspm)
        {
            try
            {
                var formDigestXML = new XmlDocument();
                HttpWebRequest contextinfoRequest =
                    (HttpWebRequest)HttpWebRequest.Create(siteUrl + "/_api/contextinfo");
                //System.Threading.Thread.Sleep(10000);
                contextinfoRequest.Method = "POST";
                contextinfoRequest.ContentType = "application/json;odata=verbose;charset=utf-8";
                contextinfoRequest.ContentLength = 0;

                contextinfoRequest.Headers.Set(HttpRequestHeader.AcceptLanguage, "en-US");
                contextinfoRequest.Headers.Add("Authorization", accessToken);
                CredentialCache myCache = new CredentialCache();

                contextinfoRequest.Credentials = System.Net.CredentialCache.DefaultCredentials;
                HttpWebResponse contextinfoResponse = (HttpWebResponse)contextinfoRequest.GetResponse();

                using (StreamReader contextinfoReader = new StreamReader(contextinfoResponse.GetResponseStream(), System.Text.Encoding.UTF8))
                {
                    formDigestXML.LoadXml(contextinfoReader.ReadToEnd());
                }
                //contextinfoRequest.KeepAlive = false;
                var formDigestNode = formDigestXML.SelectSingleNode("//d:FormDigestValue", xmlnspm);
                return formDigestNode;
            }
            catch (WebException ex)
            {

                throw new Exception(string.Format("An error occured while reading data. GUID: {0}", ex.ToString()));
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("An error occured while reading data. GUID: {0}", ex.ToString()));
            }
        }

        public int UploadDocument(ClientContext ctx, string ListName, HttpPostedFileBase files, string ItemData)
        {
            RetrieveAccessToken(ctx);
            int fileId = 0;
            XmlNamespaceManager xmlnspm = AddXmlNameSpaces();
            var formDigestNode = GetFormDigest(ctx.Url, xmlnspm);
            string formDigest = formDigestNode.InnerXml;
            var postedFile = files;

            byte[] fileData = null;
            using (var binaryReader = new BinaryReader(postedFile.InputStream))
            {
                fileData = binaryReader.ReadBytes(postedFile.ContentLength);
            }

            string NewFileWithExtention = string.Empty;
            var FileName = new FileInfo(postedFile.FileName).Name;

            string filename = string.Concat(FileName.Substring(0, FileName.Length - Path.GetExtension(FileName).Length), "_", DateTime.Now.ToString("yyyyMMddHHmmss"));
            NewFileWithExtention = string.Concat(filename, Path.GetExtension(FileName));

            string url = ctx.Url + "/_api/Web/Lists/getByTitle('" + ListName + "')/RootFolder/Files/Add(url='" + NewFileWithExtention + "', overwrite=false)?$expand=ListItemAllFields";

            HttpWebRequest endpointRequest = (HttpWebRequest)HttpWebRequest.Create(url);
            endpointRequest.Method = "POST";
            endpointRequest.Accept = "application/json;odata=verbose";
            endpointRequest.ContentType = "application/json;odata=verbose";
            endpointRequest.Headers.Add("binaryStringRequestBody", "true");
            endpointRequest.Headers.Add("Authorization", accessToken);
            endpointRequest.Headers.Add("X-RequestDigest", formDigest);
            endpointRequest.Credentials = System.Net.CredentialCache.DefaultCredentials;
            endpointRequest.GetRequestStream().Write(fileData, 0, fileData.Length);
            try
            {
                HttpWebResponse endpointresponse = (HttpWebResponse)endpointRequest.GetResponse();
                using (StreamReader reader = new StreamReader(endpointresponse.GetResponseStream()))
                {
                    string response = reader.ReadToEnd();
                    JObject jobj = JObject.Parse(response);
                    if (!string.IsNullOrEmpty(Convert.ToString(jobj)))
                        fileId = Convert.ToInt32(Convert.ToString(jobj["d"]["ListItemAllFields"]["ID"]));
                    if (ItemData != "" && ItemData != null)
                    {
                        UpdateItem(ctx, ListName, ItemData, fileId.ToString());
                    }

                }

            }
            catch (WebException ex)
            {


            }

            return fileId;
        }

    }

}