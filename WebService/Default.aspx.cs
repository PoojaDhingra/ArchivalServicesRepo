using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Newtonsoft.Json.Linq;
using RestSharp;
using RestSharp.Authenticators;

namespace WebService
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

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

        [System.Web.Services.WebMethod]
        public static string GetCurrentTime(string name)
        {
            return "Hello " + name + Environment.NewLine + "The Current Time is: "
                + DateTime.Now.ToString();
        }

        [System.Web.Services.WebMethod]
        public static string UploadFile(string caseId, string fileName, string fileData)
        {
            string dataReturned = string.Empty;
            string user = "asharma"; string password = "span@123"; string domain = "SPIVPC";
            string baseSiteUrl = "http://spivpc:9004/sites/DCM";
            Uri uri = new Uri(baseSiteUrl);//fullUrl is absoluteUrl
            string relativeUrl = uri.AbsolutePath;

            if (baseSiteUrl.EndsWith("/", StringComparison.Ordinal))
                baseSiteUrl += Constants.KeywordApi;
            else
                baseSiteUrl += "/" + Constants.KeywordApi;

            string formDigestValue, libraryName = string.Empty, caseFolderName = string.Empty;
            var restClient = RestClient(baseSiteUrl, user, password, domain, out formDigestValue);
            libraryName = "Dokumenter";
            caseFolderName = "Saker";
            
            string folderServerRelativeUrl = string.Concat(relativeUrl, libraryName, "/", caseFolderName, "/", caseId, "/Saksdokumenter");
          
            RestRequest request = new RestRequest("web/GetFolderByServerRelativeUrl('" + folderServerRelativeUrl + "')/files/add(url='" + fileName + "',overwrite='true')", Method.POST)
            {
                RequestFormat = DataFormat.Json
            };
            request.AddHeader("Accept", "application/json;odata=verbose");
            request.AddHeader("Content-Type", "application/json;odata=verbose");
            //request.AddHeader("content-length", Convert.ToString(byteArray.Length));
            request.AddHeader("X-RequestDigest", formDigestValue);
            request.AddHeader("binaryStringRequestBody", "true");
            request.AddParameter("application/json;odata=verbose", fileData, ParameterType.RequestBody);
            IRestResponse response = restClient.Execute(request);

            JObject jobj = JObject.Parse(response.Content);
            string eTag = Convert.ToString(jobj["d"]["ETag"]);

            string itemID = GetListItemId(user, password, domain, fileName);

            return dataReturned + "Item ID for fileName: " + itemID;
        }

        public static string GetListItemId(string user, string password, string domain,string fileName)
        {
            string baseSiteUrl = "http://spivpc:9004/sites/DCM";
            Uri uri = new Uri(baseSiteUrl);//fullUrl is absoluteUrl
            string relativeUrl = uri.AbsolutePath;

            if (baseSiteUrl.EndsWith("/", StringComparison.Ordinal))
                baseSiteUrl += Constants.KeywordApi;
            else
                baseSiteUrl += "/" + Constants.KeywordApi;

            string formDigestValue;
            var restClient = RestClient(baseSiteUrl, user, password, domain, out formDigestValue);

            string ItemServerRelativeUrl = relativeUrl + "/Dokumenter/Saker/4444/Saksdokumenter/" + fileName;
           
            RestRequest request;
            request =
                new RestRequest(
                    "web/GetFolderByServerRelativeUrl('" + ItemServerRelativeUrl + "')/ListItemAllFields?$select=id",
                    Method.GET);
            request.RequestFormat = DataFormat.Json;
            request.AddHeader("Accept", "application/json;odata=verbose");
            IRestResponse restRequest1 = restClient.Execute(request);
            string content = restRequest1.Content;
            JObject jobj = JObject.Parse(content);
            string createdListItemId = Convert.ToString(jobj["d"]["Id"]);
            return createdListItemId;
        }
    }
}