using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Services;
using Newtonsoft.Json.Linq;
using RestSharp;
using RestSharp.Authenticators;

namespace WebService
{
    /// <summary>
    /// Summary description for WebService2
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class WebService2 : System.Web.Services.WebService
    {        

        [WebMethod]
        public string Createsite(string siteTitle,string templateID, string destinationURL)
        {
            if (destinationURL.EndsWith("/"))
                destinationURL += "_api/";
            else
                destinationURL += "/_api/";

            RestClient RC = new RestClient(destinationURL);
            System.Net.NetworkCredential NCredential = new System.Net.NetworkCredential("asharma", "span@123", "spivpc");
            RC.Authenticator = new NtlmAuthenticator(NCredential);

            RestRequest Request = new RestRequest("contextinfo?$select=FormDigestValue", Method.POST);
            Request.AddHeader("Accept", "application/json;odata=verbose");
            Request.AddHeader("Body", "");

            //rethink this!!!!!!!!
            string ReturnedStr = RC.Execute(Request).Content;
            int StartPos = ReturnedStr.IndexOf("FormDigestValue") + 18;
            int length = ReturnedStr.IndexOf(@""",", StartPos) - StartPos;
            string FormDigestValue = ReturnedStr.Substring(StartPos, length);

            //Console.WriteLine(FormDigestValue);

            var Data = string.Concat(
                "{'parameters':{'__metadata':{'type':'SP.WebCreationInformation'},",
                "'Title':'" + siteTitle + "','Url':'" + siteTitle + "','WebTemplate':'" + templateID + "',"+
                "'UseSamePermissionsAsParentSite': true}}");

            Request = new RestRequest("web/webs/add", Method.POST);
            Request.RequestFormat = DataFormat.Json;
            Request.AddHeader("Accept", "application/json;odata=verbose");
            Request.AddHeader("X-RequestDigest", FormDigestValue);
            Request.AddParameter("application/json;odata=verbose", Data, ParameterType.RequestBody); 
            var t=RC.Execute(Request);        
            return RC.Execute(Request).Content;//"Site Created Successfully";
        }

        [WebMethod]
        public string GetFilesFromDocumentLibrary(string baseSiteUrl)
        {
            Uri uri = new Uri(baseSiteUrl);//fullUrl is absoluteUrl  
            string relativeUrl = uri.AbsolutePath;

            if (baseSiteUrl.EndsWith("/"))
                baseSiteUrl += "_api/web/";
            else
                baseSiteUrl += "/_api/web/";

            string data = string.Empty;
           HttpWebRequest endpointRequest = (HttpWebRequest)HttpWebRequest.Create(baseSiteUrl + "GetFolderByServerRelativeUrl('" + relativeUrl + "Dokumenter/')/files");
            //HttpWebRequest endpointRequest = (HttpWebRequest)HttpWebRequest.Create("http://spivpc:9004/_api/Web/GetFolderByServerRelativeUrl('/documents')?");
   
            endpointRequest.Method = "GET";
            endpointRequest.Accept = "application/json;odata=verbose";
            NetworkCredential cred = new System.Net.NetworkCredential("asharma", "span@123", "spivpc");
            endpointRequest.Credentials = cred;
            HttpWebResponse endpointResponse = (HttpWebResponse)endpointRequest.GetResponse();
            try
            {
                WebResponse webResponse = endpointRequest.GetResponse();
                Stream webStream = webResponse.GetResponseStream();
                StreamReader responseReader = new StreamReader(webStream);
                string response = responseReader.ReadToEnd();
                JObject jobj = JObject.Parse(response);
                JArray jarr = (JArray)jobj["d"]["results"];
                foreach (JObject j in jarr)
                {
                    data += j["Name"] + "---ID---" + j["Name"] + "----" + " , ";
                }

                responseReader.Close();
            }
            catch (Exception e)
            {
                //Console.Out.WriteLine(e.Message); Console.ReadLine();
            }
            return data;
        }

        [WebMethod]
        public string Regular(string input)
        {          
            string key = string.Empty;
            // Here we call Regex.Match.
      //      Match match = Regex.Match(input, @"ADOK-([0-9\-]+)\-([0-9\-]+)$",RegexOptions.IgnoreCase);

            Regex regex = new Regex(@"^\d$");

            Match match = Regex.Match(input, @"^\d$", RegexOptions.IgnoreCase);
            // Here we check the Match instance.
            if (match.Success)
            {
                // Finally, we get the Group value and display it.
                key = match.Groups[1].Value;             
            }
            return key;
        }

        [WebMethod]
        public string HelloWorld1()
        {
            string data = string.Empty;
            //HttpWebRequest endpointRequest = (HttpWebRequest)HttpWebRequest.Create("http://spivpc:9004/sites/DCN/_api/web/lists");
            //HttpWebRequest endpointRequest = (HttpWebRequest)HttpWebRequest.Create("http://spivpc:9004/sites/DCN/_api/web/GetFolderByServerRelativeUrl('/Dokumenter/')/folders");
            ///_api/web/lists/getbytitle('Dokumenter')/items?$select=File&$expand=File&$filter=ID eq 2
     
            HttpWebRequest endpointRequest = (HttpWebRequest)HttpWebRequest.Create("http://spivpc:9004/_api/site/rootWeb/webinfos");
          
            endpointRequest.Method = "GET";
            endpointRequest.Accept = "application/json;odata=verbose";
            NetworkCredential cred = new System.Net.NetworkCredential("asharma", "span@123", "spivpc");
            endpointRequest.Credentials = cred;
            HttpWebResponse endpointResponse = (HttpWebResponse)endpointRequest.GetResponse();
            try
            {
                WebResponse webResponse = endpointRequest.GetResponse();
                Stream webStream = webResponse.GetResponseStream();
                StreamReader responseReader = new StreamReader(webStream);
                string response = responseReader.ReadToEnd();
                JObject jobj = JObject.Parse(response);
                JArray jarr = (JArray)jobj["d"]["results"];
                foreach (JObject j in jarr)
                {
                    data += j["Title"]+", ";
                }

                responseReader.Close();
            }
            catch (Exception e)
            {
                //Console.Out.WriteLine(e.Message); Console.ReadLine();
            }
            return "result confirmed";// data;
        }

        [WebMethod]
        public string ActivateFeature(string baseSiteUrl, string user, string password, string domain)
        {
         
            if (baseSiteUrl.EndsWith("/"))
                baseSiteUrl += "_api/";
            else
                baseSiteUrl += "/_api/";

            RestClient rc = new RestClient(baseSiteUrl);
            NetworkCredential nCredential = new NetworkCredential(user, password, domain);
            rc.Authenticator = new NtlmAuthenticator(nCredential);

            RestRequest request = new RestRequest("contextinfo?$select=FormDigestValue", Method.POST);
            request.AddHeader("Accept", "application/json;odata=verbose");
            request.AddHeader("Body", "");

            string returnedStr = rc.Execute(request).Content;
            int startPos = returnedStr.IndexOf("FormDigestValue", StringComparison.Ordinal) + 18;
            int length = returnedStr.IndexOf(@""",", startPos, StringComparison.Ordinal) - startPos;
            string formDigestValue = returnedStr.Substring(startPos, length);

            request = new RestRequest("web/features/add('de646322-53f3-474d-96bf-0ea3670a0722',false)", Method.POST);
            request.RequestFormat = DataFormat.Json;
            request.AddHeader("Accept", "application/json;odata=verbose");
            //request.AddHeader("Body", "");
            request.AddHeader("Content-Type", "application/json;odata=verbose");
            request.AddHeader("X-RequestDigest", formDigestValue);
            IRestResponse response = rc.Execute(request);
            string content = response.Content;

            return content;

        }

        [WebMethod]        
        public string SetListPermission(string folderName, string baseSiteUrl, string user, string password, string domain)
        {
            string targetSiteUrl = baseSiteUrl;
            if (targetSiteUrl.EndsWith("/"))
                targetSiteUrl += "_api/";
            else
                targetSiteUrl +="/_api/";

            string list = string.Empty;
          
            string NemndGroupName = string.Empty, UtvalgGroupName = string.Empty;

                NemndGroupName = "NemndGroup";
                 UtvalgGroupName = "UtvalgGroup";

                 list = "Saksdokumenter";

            // break the role inheritance on list.
            BreakRoleInheritance(list, targetSiteUrl, user, password, domain);
            BreakItemRoleInheritance(list, targetSiteUrl, user, password, domain);
            // get the ID of the target group.
            var NemndGroupId = getTargetGroupId(targetSiteUrl, folderName, user, password, domain, NemndGroupName);
            var UtvalgGroupId = getTargetGroupId(targetSiteUrl, folderName, user, password, domain, UtvalgGroupName);
            string content = string.Empty;
            if (folderName == "Nemnd")
            {
                //DeletePermissions(caseId, list, targetSiteUrl, user, password, domain, UtvalgGroupId);
              content=  AssignPermissions(targetSiteUrl, list, NemndGroupId, user, password, domain);    
            }
            if (folderName == "Utvalg")
            {
                //DeletePermissions(caseId, list, targetSiteUrl, user, password, domain, NemndGroupId);
             content=   AssignPermissions(targetSiteUrl, list, UtvalgGroupId, user, password, domain);    
            }

            return content;
        }

        private string getTargetGroupId(string destinationUrl, string name, string user, string password, string domain,string groupName)
        {
            
            RestClient RC = new RestClient(destinationUrl);
            NetworkCredential NCredential = new NetworkCredential(user, password, domain);
            RC.Authenticator = new NtlmAuthenticator(NCredential);

            RestRequest Request = new RestRequest("web/sitegroups/getbyname('" + groupName + "')/id", Method.GET);
            Request.RequestFormat = DataFormat.Json;
            Request.AddHeader("Accept", "application/json;odata=verbose");
            string content = RC.Execute(Request).Content;

            JObject jobj = JObject.Parse(content);
            string groupId = Convert.ToString(jobj["d"]["Id"]);
            return groupId;
        }

         [WebMethod]   
        public void BreakRoleInheritance(string listTitle, string baseSiteUrl, string user, string password, string domain)
        {
            try
            {
                RestClient RC = new RestClient(baseSiteUrl);
                NetworkCredential NCredential = new NetworkCredential(user, password, domain);
                RC.Authenticator = new NtlmAuthenticator(NCredential);

                RestRequest Request = new RestRequest("contextinfo?$select=FormDigestValue", Method.POST);
                Request.AddHeader("Accept", "application/json;odata=verbose");
                Request.AddHeader("Body", "");

                string ReturnedStr = RC.Execute(Request).Content;
                int StartPos = ReturnedStr.IndexOf("FormDigestValue") + 18;
                int length = ReturnedStr.IndexOf(@""",", StartPos) - StartPos;
                string FormDigestValue = ReturnedStr.Substring(StartPos, length);

                Request = new RestRequest("web/lists/GetByTitle('" + listTitle + "')/breakroleinheritance(true)", Method.POST);
                Request.RequestFormat = DataFormat.Json;
                Request.AddHeader("Accept", "application/json;odata=verbose");
                Request.AddHeader("X-RequestDigest", FormDigestValue);
                string content = RC.Execute(Request).Content;

                //return "Permission breaked successfully";
            }
            catch (Exception)
            {

                throw;
            }
        }

        public void BreakItemRoleInheritance(string listTitle, string baseSiteUrl, string user, string password, string domain)
        {
            try
            {
                RestClient RC = new RestClient(baseSiteUrl);
                NetworkCredential NCredential = new NetworkCredential(user, password, domain);
                RC.Authenticator = new NtlmAuthenticator(NCredential);

                RestRequest Request = new RestRequest("contextinfo?$select=FormDigestValue", Method.POST);
                Request.AddHeader("Accept", "application/json;odata=verbose");
                Request.AddHeader("Body", "");

                string ReturnedStr = RC.Execute(Request).Content;
                int StartPos = ReturnedStr.IndexOf("FormDigestValue") + 18;
                int length = ReturnedStr.IndexOf(@""",", StartPos) - StartPos;
                string FormDigestValue = ReturnedStr.Substring(StartPos, length);

                Request = new RestRequest("web/lists/GetByTitle('" + listTitle + "')/getItemById(12)/breakroleinheritance", Method.POST);
              //  Request = new RestRequest("web/lists/GetByTitle('" + listTitle + "')/breakroleinheritance(true)", Method.POST);
                Request.RequestFormat = DataFormat.Json;
                Request.AddHeader("Accept", "application/json;odata=verbose");
                Request.AddHeader("X-RequestDigest", FormDigestValue);
                string content = RC.Execute(Request).Content;

                //return "Permission breaked successfully";
            }
            catch (Exception)
            {

                throw;
            }
        }
       
        public void DeletePermissions(string caseId, string listTitle, string baseSiteUrl, string user, string password, string domain, string groupId)
        {
            try
            {               
                RestClient RC = new RestClient(baseSiteUrl);
                NetworkCredential NCredential = new NetworkCredential(user, password, domain);
                RC.Authenticator = new NtlmAuthenticator(NCredential);

                RestRequest Request = new RestRequest("contextinfo?$select=FormDigestValue", Method.POST);
                Request.AddHeader("Accept", "application/json;odata=verbose");
                Request.AddHeader("Body", "");

                string ReturnedStr = RC.Execute(Request).Content;
                int StartPos = ReturnedStr.IndexOf("FormDigestValue") + 18;
                int length = ReturnedStr.IndexOf(@""",", StartPos) - StartPos;
                string FormDigestValue = ReturnedStr.Substring(StartPos, length);

                Request = new RestRequest("web/lists/GetByTitle('" + listTitle + "')/roleassignments/getbyprincipalid('" + groupId + "')", Method.POST);
                Request.RequestFormat = DataFormat.Json;
                Request.AddHeader("Accept", "application/json;odata=verbose");
                Request.AddHeader("X-HTTP-Method", "DELETE");
                Request.AddHeader("X-RequestDigest", FormDigestValue);
                string content = RC.Execute(Request).Content;
            }
            catch (Exception)
            {

                throw;
            }
        }

        private string AssignPermissions(string destinationUrl, string listTitle, string groupId, string user, string password, string domain)
        {
            var targetRoleDefinitionId = "1073741827";  // for contribute.
            
            RestClient RC = new RestClient(destinationUrl);
            NetworkCredential NCredential = new NetworkCredential(user, password, domain);
            RC.Authenticator = new NtlmAuthenticator(NCredential);

            RestRequest Request = new RestRequest("contextinfo?$select=FormDigestValue", Method.POST);
            Request.AddHeader("Accept", "application/json;odata=verbose");
            Request.AddHeader("Body", "");

            string ReturnedStr = RC.Execute(Request).Content;
            int StartPos = ReturnedStr.IndexOf("FormDigestValue") + 18;
            int length = ReturnedStr.IndexOf(@""",", StartPos) - StartPos;
            string FormDigestValue = ReturnedStr.Substring(StartPos, length);
            
           Request = new RestRequest("web/lists/getbytitle('" + listTitle + "')/Items(12)/roleassignments/addroleassignment(principalid='" + groupId + "',roledefid='" + targetRoleDefinitionId + "')", Method.POST);
           // Request = new RestRequest("web/lists/getbytitle('" + listTitle + "')/roleassignments/addroleassignment(principalid='" + groupId + "',roledefid='" + targetRoleDefinitionId + "')", Method.POST);
            Request.RequestFormat = DataFormat.Json;
            Request.AddHeader("Accept", "application/json;odata=verbose");
            Request.AddHeader("X-RequestDigest", FormDigestValue);
            string content = RC.Execute(Request).Content;

            return content;
        }

        [WebMethod]
        public string CreateCaseFolder(string title, string baseSiteUrl, string user, string password, string domain)
        {
            Uri uri = new Uri(baseSiteUrl);//fullUrl is absoluteUrl
            string listItemId = string.Empty;
            string relativeUrl = uri.AbsolutePath;
            string siteUrl;

            if (baseSiteUrl.EndsWith("/"))
                siteUrl = baseSiteUrl + Constants.KeywordApi + "/";
            else
                siteUrl = baseSiteUrl + "/" + Constants.KeywordApi + "/";

            RestRequest request = null;
            string formDigestValue;
            var rc = RestClient(siteUrl, user, password, domain, out formDigestValue);          
            string libaries = string.Empty;
            string folderServerRelativeUrl = relativeUrl + "Dokumenter/Saker/" + title;

            var body = string.Concat(
                "{'__metadata':{'type':'SP.Folder'},",
                "'ServerRelativeUrl':'" + folderServerRelativeUrl + "'}");

            request = new RestRequest("Web/Folders", Method.POST);
            request.RequestFormat = DataFormat.Json;
            request.AddHeader("Accept", "application/json;odata=verbose");
            request.AddHeader("X-RequestDigest", formDigestValue);
            request.AddHeader("Content-Type", "application/json;odata=verbose");
            request.AddParameter("application/json;odata=verbose", body, ParameterType.RequestBody);
            IRestResponse restRequest = rc.Execute(request);

            string response = restRequest.Content;
            string resultData = "folder created ";

            //get case folder id
            var caseFolderId = GetFolderId(folderServerRelativeUrl, rc);
            string breakInheritanceResponse = BreakChildFolderInheritance(siteUrl, user, password, domain, "Dokumenter", caseFolderId);
           
            var ownersGroup = Convert.ToString((GetTargetGroupId(siteUrl, title, user, password, domain, Constants.OwnersGroup)));
            var readersGroup = Convert.ToString((GetTargetGroupId(siteUrl, title, user, password, domain, Constants.ReadersGroup)));

            //var nemndContributorGroupId = Convert.ToString((GetTargetGroupId(siteUrl, title, user, password, domain, Constants.NemndContributorGroup)));
            //var utvalgContributorGroupId = Convert.ToString(GetTargetGroupId(siteUrl, title, user, password, domain, Constants.UtvalgContributorGroup));

            if (breakInheritanceResponse.ToLower().Equals("OK".ToLower()))
            {
                AssignOwnerPermissions(siteUrl, Constants.DocumentLibraryName, caseFolderId, ownersGroup, Constants.FULLCONTROL, user, password, domain);
                AssignFolderPermissions(siteUrl, Constants.DocumentLibraryName, caseFolderId, readersGroup, Constants.READ, user, password, domain);
               // AssignFolderPermissions(siteUrl, Constants.DocumentLibraryName, caseFolderId, utvalgContributorGroupId, Constants.READ, user, password, domain);

                CreateCaseChildFolder(title, baseSiteUrl, user, password, domain, folderServerRelativeUrl);
            }
          
            return resultData;
        }

        public string CreateCaseChildFolder(string title, string baseSiteUrl, string user, string password, string domain, string folderServerRelativeUrl)
        {
            string siteUrl = string.Empty;
            if (baseSiteUrl.EndsWith("/"))
                siteUrl = baseSiteUrl + Constants.KeywordApi + "/";
            else
                siteUrl = baseSiteUrl + "/" + Constants.KeywordApi + "/";

            RestRequest request = null;
            string formDigestValue;
            var rc = RestClient(siteUrl, user, password, domain, out formDigestValue);
            string libaries = string.Empty;

            List<string> templateFolders = new List<string>
            {
                Constants.TempUtvalgLib,
                Constants.TempNemndLib,
                Constants.TempSaksdokumenter,
                Constants.TempBeslutning,
                Constants.TempSladdetBeslutning
            };



            foreach (string folderTitle in templateFolders)
            {
                var body = string.Concat(
                    "{'__metadata':{'type':'SP.Folder'},",
                    "'ServerRelativeUrl':'" + folderServerRelativeUrl + "/" + folderTitle + "'}");

                request = new RestRequest("Web/Folders", Method.POST);
                request.RequestFormat = DataFormat.Json;
                request.AddHeader("Accept", "application/json;odata=verbose");
                request.AddHeader("X-RequestDigest", formDigestValue);
                request.AddHeader("Content-Type", "application/json;odata=verbose");
                request.AddParameter("application/json;odata=verbose", body, ParameterType.RequestBody);
                IRestResponse restRequest = rc.Execute(request);
                string response = restRequest.Content;

                string caseFolderId = GetFolderId(folderServerRelativeUrl + "/" + folderTitle, rc);
                BreakChildFolderInheritance(siteUrl, user, password, domain, "Dokumenter", caseFolderId);
                SetFolderPermissions(folderTitle, baseSiteUrl, caseFolderId, user, password, domain);
             
            }
            string resultData = "Child folder created ";
            return resultData;
        }

        private static string GetTargetGroupId(string destinationUrl, string name, string user, string password, string domain, string groupName)
        {
            RestClient restClient = new RestClient(destinationUrl);
            NetworkCredential nCredential = new NetworkCredential(user, password, domain);
            restClient.Authenticator = new NtlmAuthenticator(nCredential);

            RestRequest request = new RestRequest("web/sitegroups/getbyname('" + groupName + "')/id", Method.GET);
            request.RequestFormat = DataFormat.Json;
            request.AddHeader("Accept", "application/json;odata=verbose");
            string content = restClient.Execute(request).Content;

            JObject jobj = JObject.Parse(content);
            string groupId = Convert.ToString(jobj["d"]["Id"]);
            return groupId;
        }

        private void BreakItemRoleInheritance(string listTitle, string baseSiteUrl, string listItemId, string user, string password, string domain)
        {
            try
            {
                RestRequest request;
                string formDigestValue;
                var rc = RestClient(baseSiteUrl, user, password, domain, out formDigestValue);

                request = new RestRequest("web/lists/GetByTitle('" + listTitle + "')/getItemById(" + listItemId + ")/breakroleinheritance", Method.POST);
                request.RequestFormat = DataFormat.Json;
                request.AddHeader("Accept", "application/json;odata=verbose");
                request.AddHeader("X-RequestDigest", formDigestValue);
                var cc = rc.Execute(request);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public string SetFolderPermissions(string folderTitle, string baseSiteUrl, string caseFolderId, string user, string password, string domain)
        {
            string targetSiteUrl = baseSiteUrl;
            if (targetSiteUrl.EndsWith("/"))
                targetSiteUrl += Constants.KeywordApi + "/";
            else
                targetSiteUrl += "/" + Constants.KeywordApi + "/";

            // get the ID of the target group.
            var ownerGroup = Convert.ToString((GetTargetGroupId(targetSiteUrl, folderTitle, user, password, domain, Constants.OwnersGroup)));

            if (!folderTitle.ToLower().Equals(Constants.TempSladdetBeslutning.ToLower()))
            {
                AssignOwnerPermissions(targetSiteUrl, Constants.DocumentLibraryName, caseFolderId, ownerGroup, Constants.FULLCONTROL, user, password, domain);
            }
            //var readerGroup = Convert.ToString((GetTargetGroupId(targetSiteUrl, folderTitle, user, password, domain, Constants.ReadersGroup)));            
            //var utvalgOwnerGroupId = Convert.ToString((GetTargetGroupId(targetSiteUrl, folderTitle, user, password, domain, Constants.UtvalgOwnerGroup)));
            //var nemndContributorGroupId = Convert.ToString((GetTargetGroupId(targetSiteUrl, folderTitle, user, password, domain, Constants.NemndContributorGroup)));
            //var utvalgContributorGroupId = Convert.ToString(GetTargetGroupId(targetSiteUrl, folderTitle, user, password, domain, Constants.UtvalgContributorGroup));

            string result = string.Empty;

            if (folderTitle.ToLower().Equals(Constants.TempNemndLib.ToLower()))
            {
                var nemndRead = Convert.ToString(GetTargetGroupId(targetSiteUrl, folderTitle, user, password, domain, Constants.NemndReaderGroup));
                var nemndContribute = Convert.ToString(GetTargetGroupId(targetSiteUrl, folderTitle, user, password, domain, Constants.NemndContributeGroup));
                result += AssignFolderPermissions(targetSiteUrl, Constants.DocumentLibraryName, caseFolderId, nemndContribute, Constants.CONTRIBUTE, user, password, domain);
               result += AssignFolderPermissions(targetSiteUrl, Constants.DocumentLibraryName, caseFolderId, nemndRead, Constants.READ, user, password, domain);
            }

            if (folderTitle.ToLower().Equals(Constants.TempUtvalgLib.ToLower()))
            {
                var utvalgContribute = Convert.ToString(GetTargetGroupId(targetSiteUrl, folderTitle, user, password, domain, Constants.UtvalgContributeGroup));
                var utvalgRead = Convert.ToString(GetTargetGroupId(targetSiteUrl, folderTitle, user, password, domain, Constants.UtvalgReaderGroup));
                result += AssignFolderPermissions(targetSiteUrl, Constants.DocumentLibraryName, caseFolderId, utvalgContribute, Constants.CONTRIBUTE, user, password, domain);
                result += AssignFolderPermissions(targetSiteUrl, Constants.DocumentLibraryName, caseFolderId, utvalgRead, Constants.READ, user, password, domain);
            }

            //if (result.ToLower().Contains("error"))
            //    return result;
            //else
            //    return "Permissions assigned successfully for " + folderTitle + " group.";

            return string.Empty;
        }

        private static string AssignFolderPermissions(string destinationUrl, string listTitle, string listItemId, string groupId,string roleDefiId, string user, string password, string domain)
        {
            RestRequest Request;
            string formDigestValue;
            var rc = RestClient(destinationUrl, user, password, domain, out formDigestValue);

            Request = new RestRequest("web/lists/getbytitle('" + listTitle + "')/Items(" + listItemId + ")/roleassignments/addroleassignment(principalid='" + groupId + "',roledefid='"+roleDefiId+"')", Method.POST);
            Request.RequestFormat = DataFormat.Json;
            Request.AddHeader("Accept", "application/json;odata=verbose");
            Request.AddHeader("Content-Type", "application/json;odata=verbose");
            Request.AddHeader("X-RequestDigest", formDigestValue);
            var data = rc.Execute(Request);
            string content = data.Content;
            return content;

        }

        private static string AssignOwnerPermissions(string destinationUrl, string listTitle, string listItemId, string groupId,string roleDefiId, string user, string password, string domain)
        {
            RestRequest Request;
            string formDigestValue;
            var rc = RestClient(destinationUrl, user, password, domain, out formDigestValue);

            Request = new RestRequest("web/lists/getbytitle('" + listTitle + "')/Items(" + listItemId + ")/roleassignments/addroleassignment(principalid='" + groupId + "',roledefid='" + roleDefiId + "')", Method.POST);
            Request.RequestFormat = DataFormat.Json;
            Request.AddHeader("Accept", "application/json;odata=verbose");
            Request.AddHeader("Content-Type", "application/json;odata=verbose");
            Request.AddHeader("X-RequestDigest", formDigestValue);
            var data = rc.Execute(Request);
            string content = data.Content;
            return content;

        }

        private static RestClient RestClient(string baseSiteUrl, string user, string password, string domain, out string formDigestValue)
        {
            RestClient restClient = new RestClient(baseSiteUrl);
            NetworkCredential nCredential = new NetworkCredential(user, password, domain);
            restClient.Authenticator = new NtlmAuthenticator(nCredential);

            RestRequest request = new RestRequest("contextinfo?$select=FormDigestValue", Method.POST);
            request.AddHeader("Accept", "application/json;odata=verbose");
            request.AddHeader("Body", "");

            string returnedStr = restClient.Execute(request).Content; // change variable names
            int startPos = returnedStr.IndexOf("FormDigestValue", StringComparison.Ordinal) + 18;
            int length = returnedStr.IndexOf(@""",", startPos, StringComparison.Ordinal) - startPos;
            formDigestValue = returnedStr.Substring(startPos, length);
            return restClient;
        }

        [WebMethod]
        public string GetListItemId(string baseSiteUrl, string user, string password, string domain)
        {
            Uri uri = new Uri(baseSiteUrl);//fullUrl is absoluteUrl
            string relativeUrl = uri.AbsolutePath;

            string targetSiteUrl = baseSiteUrl;
            if (targetSiteUrl.EndsWith("/"))
                targetSiteUrl += Constants.KeywordApi;
            else
                targetSiteUrl += "/" + Constants.KeywordApi;

            string formDigestValue;
            var rc = RestClient(targetSiteUrl, user, password, domain, out formDigestValue);
            string ItemServerRelativeUrl = "/sites/DCM/Dokumenter/Saker/4444/QMJCHD6S4SA7-1-225.txt";
            RestRequest request;
            request =
                new RestRequest(
                    "web/GetFolderByServerRelativeUrl('" + ItemServerRelativeUrl + "')/ListItemAllFields?$select=id",
                    Method.GET);
            request.RequestFormat = DataFormat.Json;
            request.AddHeader("Accept", "application/json;odata=verbose");
            IRestResponse restRequest1 = rc.Execute(request);
            string content = restRequest1.Content;
            JObject jobj = JObject.Parse(content);
            string createdListItemId = Convert.ToString(jobj["d"]["Id"]);
            return createdListItemId;
        }
      
        private static string GetFolderId(string folderServerRelativeUrl, RestClient rc)
        {
            RestRequest request;
            request =
                new RestRequest(
                    "web/GetFolderByServerRelativeUrl('" + folderServerRelativeUrl + "')/ListItemAllFields?$select=id",
                    Method.GET);         
            request.RequestFormat = DataFormat.Json;
            request.AddHeader("Accept", "application/json;odata=verbose");
            IRestResponse restRequest1 = rc.Execute(request);
            string content = restRequest1.Content;
            JObject jobj = JObject.Parse(content);
            string createdFolderId = Convert.ToString(jobj["d"]["Id"]);
            return createdFolderId;
        }

        private static string GetCaseDocumentsFolder(string baseSiteUrl,string folderName, RestClient rc)
        {          
            RestRequest request;
            request =
                new RestRequest(
                    "/web/lists/getbytitle('Dokumenter')/Items?$select=ID&$filter=Authority eq '" + folderName + "'",
                    Method.GET);
            request.RequestFormat = DataFormat.Json;
            request.AddHeader("Accept", "application/json;odata=verbose");
            IRestResponse restRequest1 = rc.Execute(request);
            string content = restRequest1.Content;
            JObject jobj = JObject.Parse(content);
            string data = string.Empty;
            JArray jarr = (JArray)jobj["d"]["results"];
            foreach (JObject j in jarr)
            {
                data = Convert.ToString(j["Id"]);
            }

            string createdFolderId = data;
            return createdFolderId;
        }

        public string BreakChildFolderInheritance(string baseSiteUrl, string user, string password, string domain, string listTitle, string listItemId)
        {
            RestRequest request = null;
            string formDigestValue;
            var rc = RestClient(baseSiteUrl, user, password, domain, out formDigestValue);

            request = new RestRequest("web/lists/GetByTitle('" + listTitle + "')/getItemById(" + listItemId + ")/breakroleinheritance", Method.POST);
            request.RequestFormat = DataFormat.Json;
            request.AddHeader("Accept", "application/json;odata=verbose");
            request.AddHeader("X-RequestDigest", formDigestValue);
            IRestResponse response = rc.Execute(request);
            return Convert.ToString(response.StatusCode);
        }

        [WebMethod]
        public string Getfolder()
        {
            //Uri uri = new Uri(baseSiteUrl);//fullUrl is absoluteUrl  
            //string relativeUrl = uri.AbsolutePath;

            //if (baseSiteUrl.EndsWith("/"))
            //    baseSiteUrl += "_api/web/";
            //else
            //    baseSiteUrl += "/_api/web/";

            string baseSiteUrl = "http://spivpc:9004/sites/DCN/_api/web/lists/getbytitle('Saksdokumenter')/items?$filter=FileLeafRef eq '2222'";
            string data = string.Empty;
            HttpWebRequest endpointRequest = (HttpWebRequest)HttpWebRequest.Create(baseSiteUrl);            
            endpointRequest.Method = "GET";
            endpointRequest.Accept = "application/json;odata=verbose";
            NetworkCredential cred = new System.Net.NetworkCredential("asharma", "span@123", "spivpc");
            endpointRequest.Credentials = cred;
            HttpWebResponse endpointResponse = (HttpWebResponse)endpointRequest.GetResponse();
            try
            {
                WebResponse webResponse = endpointRequest.GetResponse();
                Stream webStream = webResponse.GetResponseStream();
                StreamReader responseReader = new StreamReader(webStream);
                string response = responseReader.ReadToEnd();
                JObject jobj = JObject.Parse(response);
                JArray jarr = (JArray)jobj["d"]["results"];



                foreach (JObject j in jarr)
                {
                   // data = j["uri"];
                }

                responseReader.Close();
            }
            catch (Exception e)
            {
                //Console.Out.WriteLine(e.Message); Console.ReadLine();
            }
            return data;
        }

        //[WebMethod]
        //public string UploadDocumentInCaseFolder(string caseId, string fileUrl,string baseSiteUrl, string user, string password, string domain)
        //{
        //    Uri uri = new Uri(baseSiteUrl);//fullUrl is absoluteUrl
        //    string relativeUrl = uri.AbsolutePath;

        //    if (baseSiteUrl.EndsWith("/"))
        //        baseSiteUrl += Constants.KeywordApi;
        //    else
        //        baseSiteUrl += "/" + Constants.KeywordApi;

        //    string formDigestValue;
        //    var rc = RestClient(baseSiteUrl, user, password, domain, out formDigestValue);
            
        //    string filePath = fileUrl;
        //    string fileName = Path.GetFileName(filePath);

        //    var byteArray = File.ReadAllBytes(filePath);
        //    string fileString = Encoding.UTF8.GetString(byteArray, 0, byteArray.Length);


        //    string folderServerRelativeUrl = relativeUrl + "Dokumenter/Saker/" + caseId + "/Saksdokumenter";

        //    //RestRequest request = new RestRequest("web/lists/getbytitle('" + listTitle + "')/RootFolder/files/add(url='1.docx',overwrite='true')", Method.POST);
        //    RestRequest request = new RestRequest("web/GetFolderByServerRelativeUrl('" + folderServerRelativeUrl + "')/files/add(url='" + fileName + "',overwrite='true')", Method.POST);
        //    request.RequestFormat = DataFormat.Json;
        //    request.AddHeader("Accept", "application/json;odata=verbose");
        //    request.AddHeader("Content-Type", "application/json;odata=verbose");
        //    request.AddHeader("content-length", Convert.ToString(byteArray.Length));
        //    request.AddHeader("X-RequestDigest", formDigestValue);
        //    request.AddHeader("binaryStringRequestBody", "true");
        //    request.AddParameter("application/json;odata=verbose", fileString, ParameterType.RequestBody);
        //    var data = rc.Execute(request);
        //    string content = data.Content;

        //    return content;
        //}

        [WebMethod]
        public string SetPermission(string caseId, string folderName, string baseSiteUrl, string user, string password, string domain)
        {
            Uri uri = new Uri(baseSiteUrl);//fullUrl is absoluteUrl
            string relativeUrl = uri.AbsolutePath;

            string targetSiteUrl = baseSiteUrl;
            if (targetSiteUrl.EndsWith("/"))
                targetSiteUrl += Constants.KeywordApi;
            else
                targetSiteUrl += "/" + Constants.KeywordApi;

            string formDigestValue;
            var rc = RestClient(targetSiteUrl, user, password, domain, out formDigestValue);

            string folderServerRelativeUrl = relativeUrl + "Dokumenter/Saker/" + caseId + "/Saksdokumenter";
            var caseDocumentFolderId = GetFolderId(folderServerRelativeUrl, rc);

            //string casedocumentServerRelativeUrl = relativeUrl + "Dokumenter/Saker/" + caseId + "/Saksdokumenter";
            //var casedocumentFolderId = GetFolderId(casedocumentServerRelativeUrl, rc);

            //string besultningServerRelativeUrl = relativeUrl + "Dokumenter/Saker/" + caseId + "/Beslutning";
            //var besultningFolderId = GetFolderId(besultningServerRelativeUrl, rc);

            //string sladdetServerRelativeUrl = relativeUrl + "Dokumenter/Saker/" + caseId + "/Sladdet beslutning";
            //var sladdetFolderId = GetFolderId(sladdetServerRelativeUrl, rc);

            List<string> templateFolders = new List<string>
            {                            
                Constants.TempBeslutning,
                Constants.TempSladdetBeslutning
            };
         
            string result = string.Empty;
            string removeExistingPermissions = string.Empty;

           

            var utvalgContribute = Convert.ToString(GetTargetGroupId(targetSiteUrl, "Saksdokumenter", user, password, domain, Constants.UtvalgContributeGroup));
            var utvalgRead = Convert.ToString(GetTargetGroupId(targetSiteUrl, "Saksdokumenter", user, password, domain, Constants.UtvalgReaderGroup));
            var nemndRead = Convert.ToString(GetTargetGroupId(targetSiteUrl, "Saksdokumenter", user, password, domain, Constants.NemndReaderGroup));
            var nemndContribute = Convert.ToString(GetTargetGroupId(targetSiteUrl, "Saksdokumenter", user, password, domain, Constants.NemndContributeGroup));

            if (folderName.ToLower().Equals(Constants.NemndResource))
            {
                removeExistingPermissions = RemoveFolderPermissions(targetSiteUrl, Constants.DocumentLibraryName, caseDocumentFolderId, utvalgRead, user, password, domain);
                removeExistingPermissions = RemoveFolderPermissions(targetSiteUrl, Constants.DocumentLibraryName, caseDocumentFolderId, utvalgContribute, user, password, domain);

                result += AssignFolderPermissions(targetSiteUrl, Constants.DocumentLibraryName, caseDocumentFolderId, nemndContribute, Constants.CONTRIBUTE, user, password, domain);
                result += AssignFolderPermissions(targetSiteUrl, Constants.DocumentLibraryName, caseDocumentFolderId, nemndRead, Constants.READ, user, password, domain);

            }

            if (folderName.ToLower().Equals(Constants.UtvalgResource))
            {
                removeExistingPermissions = RemoveFolderPermissions(targetSiteUrl, Constants.DocumentLibraryName, caseDocumentFolderId, nemndContribute, user, password, domain);
                removeExistingPermissions = RemoveFolderPermissions(targetSiteUrl, Constants.DocumentLibraryName, caseDocumentFolderId, nemndRead, user, password, domain);

                result += AssignFolderPermissions(targetSiteUrl, Constants.DocumentLibraryName, caseDocumentFolderId, utvalgContribute, Constants.CONTRIBUTE, user, password, domain);
                result += AssignFolderPermissions(targetSiteUrl, Constants.DocumentLibraryName, caseDocumentFolderId, utvalgRead, Constants.READ, user, password, domain);

            }

            foreach (string folderTitle in templateFolders)
            {
                string caseFolderServerRelativeUrl = relativeUrl + "Dokumenter/Saker/" + caseId + "/" + folderTitle;
                var caseFolderId = GetFolderId(caseFolderServerRelativeUrl, rc);

                if (folderName.ToLower().Equals(Constants.NemndResource))
                {
                    removeExistingPermissions = RemoveFolderPermissions(targetSiteUrl, Constants.DocumentLibraryName, caseFolderId, nemndRead, user, password, domain);
                    removeExistingPermissions = RemoveFolderPermissions(targetSiteUrl, Constants.DocumentLibraryName, caseFolderId, utvalgContribute, user, password, domain);

                    result += AssignFolderPermissions(targetSiteUrl, Constants.DocumentLibraryName, caseFolderId, nemndContribute, Constants.CONTRIBUTE, user, password, domain);
                    result += AssignFolderPermissions(targetSiteUrl, Constants.DocumentLibraryName, caseFolderId, utvalgRead, Constants.READ, user, password, domain);

                    //result += AssignFolderPermissions(targetSiteUrl, Constants.DocumentLibraryName, besultningFolderId, nemndContribute, Constants.CONTRIBUTE, user, password, domain);
                    //result += AssignFolderPermissions(targetSiteUrl, Constants.DocumentLibraryName, besultningFolderId, nemndRead, Constants.READ, user, password, domain);

                    //result += AssignFolderPermissions(targetSiteUrl, Constants.DocumentLibraryName, sladdetFolderId, nemndContribute, Constants.CONTRIBUTE, user, password, domain);
                    //result += AssignFolderPermissions(targetSiteUrl, Constants.DocumentLibraryName, sladdetFolderId, nemndRead, Constants.READ, user, password, domain);
                }

                if (folderName.ToLower().Equals(Constants.UtvalgResource))
                {
                    removeExistingPermissions = RemoveFolderPermissions(targetSiteUrl, Constants.DocumentLibraryName, caseFolderId, utvalgRead, user, password, domain);
                    removeExistingPermissions += RemoveFolderPermissions(targetSiteUrl, Constants.DocumentLibraryName, caseFolderId, nemndContribute, user, password, domain);

                    result += AssignFolderPermissions(targetSiteUrl, Constants.DocumentLibraryName, caseFolderId, utvalgContribute, Constants.CONTRIBUTE, user, password, domain);
                    result += AssignFolderPermissions(targetSiteUrl, Constants.DocumentLibraryName, caseFolderId, nemndRead, Constants.READ, user, password, domain);

                    //result += AssignFolderPermissions(targetSiteUrl, Constants.DocumentLibraryName, besultningFolderId, utvalgContribute, Constants.CONTRIBUTE, user, password, domain);
                    //result += AssignFolderPermissions(targetSiteUrl, Constants.DocumentLibraryName, besultningFolderId, utvalgRead, Constants.READ, user, password, domain);

                    //result += AssignFolderPermissions(targetSiteUrl, Constants.DocumentLibraryName, sladdetFolderId, utvalgContribute, Constants.CONTRIBUTE, user, password, domain);
                   // result += AssignFolderPermissions(targetSiteUrl, Constants.DocumentLibraryName, sladdetFolderId, utvalgRead, Constants.READ, user, password, domain);
                }
            }

            if (result.ToLower().Contains("error"))
                return result;
            else
                return "Permissions assigned successfully for " + folderName + " group.";



        }

        private static string RemoveFolderPermissions(string destinationUrl, string listTitle, string listItemId, string groupId, string user, string password, string domain)
        {
            RestRequest Request;
            string formDigestValue;
            var rc = RestClient(destinationUrl, user, password, domain, out formDigestValue);

            Request = new RestRequest("web/lists/getbytitle('" + listTitle + "')/Items(" + listItemId + ")/roleassignments/getbyprincipalid('" + groupId + "')", Method.POST);
            Request.RequestFormat = DataFormat.Json;
            Request.AddHeader("Accept", "application/json;odata=verbose");
            Request.AddHeader("Content-Type", "application/json;odata=verbose");
            Request.AddHeader("X-RequestDigest", formDigestValue);
            Request.AddHeader("X-HTTP-Method", "DELETE");
            IRestResponse response = rc.Execute(Request);
            string content = response.Content;          
            return Convert.ToString(response.StatusCode);           
        }

        [WebMethod]
        public string DeleteGroups(string caseId, string folderName, string baseSiteUrl)
        {
            string user="asharma";
            string password="span@123";
            string domain = "SPIVPC";          

            Uri uri = new Uri(baseSiteUrl);//fullUrl is absoluteUrl
            string relativeUrl = uri.AbsolutePath;

            string targetSiteUrl = baseSiteUrl;
            if (targetSiteUrl.EndsWith("/"))
                targetSiteUrl += Constants.KeywordApi;
            else
                targetSiteUrl += "/" + Constants.KeywordApi;

            string formDigestValue;
            var rc = RestClient(targetSiteUrl, user, password, domain, out formDigestValue);
           
            string besultningServerRelativeUrl = relativeUrl + "Dokumenter/Saker/" + caseId + "/Beslutning";
            var besultningFolderId = GetFolderId(besultningServerRelativeUrl, rc);

            string result = string.Empty;
            string listTitle = "Dokumenter";
            string listItemId = "";
            RestRequest Request; 
            string removeExistingPermissions = string.Empty;
          
            if (folderName.ToLower().Equals(Constants.UtvalgResource))
            {
                var nemndRead = Convert.ToString(GetTargetGroupId(targetSiteUrl, "Saksdokumenter", user, password, domain, Constants.NemndReaderGroup));
                var nemndContribute = Convert.ToString(GetTargetGroupId(targetSiteUrl, "Saksdokumenter", user, password, domain, Constants.NemndContributeGroup));
                
                removeExistingPermissions = RemoveFolderPermissions(targetSiteUrl, Constants.DocumentLibraryName, besultningFolderId, nemndRead, user, password, domain);
                removeExistingPermissions+= RemoveFolderPermissions(targetSiteUrl, Constants.DocumentLibraryName, besultningFolderId, nemndContribute, user, password, domain);
            }
            else if (folderName.ToLower().Equals(Constants.NemndResource))
            {

                var utvalgContribute = Convert.ToString(GetTargetGroupId(targetSiteUrl, "Saksdokumenter", user, password, domain, Constants.UtvalgContributeGroup));
                var utvalgRead = Convert.ToString(GetTargetGroupId(targetSiteUrl, "Saksdokumenter", user, password, domain, Constants.UtvalgReaderGroup));
                removeExistingPermissions = RemoveFolderPermissions(targetSiteUrl, Constants.DocumentLibraryName, besultningFolderId, utvalgRead, user, password, domain);
                removeExistingPermissions = RemoveFolderPermissions(targetSiteUrl, Constants.DocumentLibraryName, besultningFolderId, utvalgContribute, user, password, domain);

            }
            if (removeExistingPermissions.ToLower().Equals("OK".ToLower()))
            {
                return "Permissions Deleted successfully for " + folderName + " group.";
            }
            else if (removeExistingPermissions.ToLower().Equals("NotFound".ToLower()))
            {
                return "Group for folder " + folderName + " not found";
            }
            else
                return "Some issue occurred while deleting groups";
        }

        public class ArchiveCaseFileModel
        {
            public string FileName { get; set; }

            public string FileData { get; set; }
        }
    }
}
