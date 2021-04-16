using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models;
using System.Net;
using System.Collections.Specialized;
using System.Net.Http;
using System.Threading;
using System.IO;
using Newtonsoft.Json;
using Shared;
namespace YoutubeUtils
{
    public class RecognizeProxy
    {
        string AuddApiToken = System.Configuration.ConfigurationManager.AppSettings["AuddApiToken"] ?? "test";
        string AuddApiUrl = System.Configuration.ConfigurationManager.AppSettings["AuddApiUrl"] ?? "https://api.audd.io/";
        string AuddApiMethod = System.Configuration.ConfigurationManager.AppSettings["AuddApiMethod"] ?? "recognize";

        public Artist GetArtist(string filePath)
        {
            RequestHelper requestHelper = new RequestHelper();
            NameValueCollection parameters = new NameValueCollection();
            parameters.Add("method", AuddApiMethod);
            parameters.Add("api_token", AuddApiToken);
            string response = requestHelper.ExecuteRequestSendFile(AuddApiUrl, parameters, null, filePath);
           
            ArtistWrapper jsonResult = null;
            try
            {
                jsonResult = JsonConvert.DeserializeObject<ArtistWrapper>(response);
                return jsonResult.result;
            }
            catch (Exception ex) { }
            return null;
        }
    }
}
