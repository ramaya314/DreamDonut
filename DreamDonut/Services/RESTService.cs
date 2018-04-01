using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml.Linq;


using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace DreamDonut.Services
{

    /// <summary>
    /// Base class for all rest services. handles low level sending of data
    /// and management of http clients.
    /// </summary>
    public class RESTService
    {


        HttpClient client;

        public static async Task<HttpResponseMessage> ExecutePostService(Uri uri, JObject parameters, HttpClient client = null)
        {
            client = client ?? new HttpClient();

            try
            {
                var json = JsonConvert.SerializeObject(parameters);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = null;

                response = await client.PostAsync(uri, content);

                if (response.IsSuccessStatusCode)
                {
                    return response;
                }
                else throw new  RESTServiceFailException(response.ToString());
            }
            catch (Exception ex)
            {
                //just wrap our exception
                throw new RESTServiceFailException(ex.Message);
            }
        }


        public static async Task<HttpContent> ExecuteGetService(Uri uri, HttpClient client = null)
        {
            client = client ?? new HttpClient();

            try
            {
                HttpResponseMessage response = null;

                response = await client.GetAsync(uri);

                if (response.IsSuccessStatusCode)
                {
                    return response.Content;
                } 
                else if(response.StatusCode == HttpStatusCode.RequestTimeout) 
                {
                    throw new  RESTTimeoutException(response.ToString());
                }
                else throw new  RESTServiceFailException(response.ToString());
            }
            catch (RESTTimeoutException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                //just wrap our exception
                throw new RESTServiceFailException(ex.Message);
            }
        }

        public static async Task<dynamic> ExecuteGetServiceForJSONResult(Uri uri, HttpClient client = null) {

            HttpContent content = await ExecuteGetService(uri, client);
            string responseContent = WebUtility.HtmlDecode(await content.ReadAsStringAsync());
            dynamic results = JsonConvert.DeserializeObject(responseContent);
            return results;
        }


        public static async Task<XDocument> ExecuteGetServiceForXMLResult(Uri uri, HttpClient client = null) {

            HttpContent content = await ExecuteGetService(uri, client);

            var stringContent = await content.ReadAsStringAsync();

            XDocument xmlDoc = XDocument.Parse(stringContent);
            return xmlDoc;

        }
    }

    public class RESTException : Exception {
        public RESTException(string message) : base(message){}
    }

    /// <summary>
    /// Exception identifier of rest exceptions. This should be thrown when the response is not a succes status code or the call fails in any way
    /// </summary>
    public class RESTServiceFailException : RESTException {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:DOMA.RESTServiceFailException"/> class.
        /// </summary>
        /// <param name="message">The error message.</param>
        public RESTServiceFailException(string message) : base(message){}
    }

    public class RESTTimeoutException : RESTException {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:DOMA.RESTServiceFailException"/> class.
        /// </summary>
        /// <param name="message">The error message.</param>
        public RESTTimeoutException(string message) : base(message){}
    }
}
