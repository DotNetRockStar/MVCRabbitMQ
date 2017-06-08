using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Jerrod.RabbitCommon.Management
{
    /// <summary>
    /// Client used to get administrative information from the RabbitMQ server API.
    /// </summary>
    public class ManagementApiClient
    {
        private readonly string _url;

        public ManagementApiClient()
            : this("http://" + ConfigSectionUtility.Host + ":15672/api/")
        {
        }
        internal ManagementApiClient(string apiUrl)
        {
            if (string.IsNullOrWhiteSpace(apiUrl))
                throw new ArgumentNullException("apiUrl");

            _url = apiUrl;
        }

        private string ConstructUrl(string relativeUri)
        {
            return _url.TrimEnd(new char[] { '/' }) + "/" + relativeUri;
        }

        private string GetBasicAuthHeaderString()
        {
            return "Basic " + System.Convert.ToBase64String(System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes(ConfigSectionUtility.Username + ":" + ConfigSectionUtility.Password));
        }

        public List<Queue> GetQueues()
        {
            var url = ConstructUrl("queues");
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
            request.Headers.Add("Authorization", GetBasicAuthHeaderString());

            var response = request.GetResponse();
            string responseFromServer = null;

            using (var dataStream = response.GetResponseStream())
            {
                using (var reader = new StreamReader(dataStream))
                {
                    responseFromServer = reader.ReadToEnd();
                }
            }

            if (!string.IsNullOrWhiteSpace(responseFromServer))
                return JsonConvert.DeserializeObject<List<Queue>>(responseFromServer);
            return null;
        }
    }
}
