using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Collections.Specialized;
using System.Net.Cache;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using log4net;
using WebMVCTest.Model;
using System.Globalization;

namespace WebMVCTest.Engine
{
	public class HttpSession : IDisposable, IResponse
	{
		private static ILog LOG = LogManager.GetLogger (typeof(HttpSession));

		private CookieContainer cookieJar = new CookieContainer ();

        /// <summary>
        /// Timeout, miliseconds before the request is cancelled.
        /// </summary>
        private int timeout = 30000;

		private int statusCode;

		private string statusDescription;

        private bool timedOut;

		private string baseUrl;

		private string responseText;

        private DateTime startTime;

        private TimeSpan? executionTime;

		public HttpSession (string baseUrl)
		{
			LOG.Debug ("Create HttpSession with baseUrl: " + baseUrl);
			this.SetBaseUrl (baseUrl);
		}

		public void SetBaseUrl (string baseUrl)
		{
			this.baseUrl = baseUrl;
		}

		private string GetUrl (string urlPart)
		{
			return this.baseUrl + urlPart;
		}

		public void Post (string url, NameValueCollection data)
		{
			HttpWebResponse response = GetResponse (url, "POST", data);

            if (response != null)
            {
                using (StreamReader stream = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                {
                    this.responseText = stream.ReadToEnd();
                    stream.Close();
                }
            }

            SetExecutionTime();
        }

		public void Get (string url)
		{
			HttpWebResponse response = GetResponse (url, "GET", null);

            if (response != null)
            {
                StreamReader stream = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                this.responseText = stream.ReadToEnd();
                stream.Close();
            }
            
            SetExecutionTime();
		}

		public int GetStatusCode ()
		{
			return this.statusCode;
		}

		public string GetStatusDescription ()
		{
			return this.statusDescription;
		}

		public string GetResponseText()
		{
			return this.responseText;
		}

        public TimeSpan? GetExecutionTime()
        {
            return this.executionTime;
        }

        public bool IsTimedOut()
        {
            return this.timedOut;
        }

        private void SetExecutionTime()
        {
            this.executionTime = DateTime.Now.Subtract(this.startTime);

            if (!this.timedOut)
            {
                LOG.DebugFormat("Execution time: {0}", this.executionTime);
            }
            else
            {
                LOG.Debug("Execution time: Timed out");
            }
        }

		private HttpWebResponse GetResponse (string urlPart, string method, NameValueCollection data)
		{
			HttpWebRequest request = (HttpWebRequest)WebRequest.Create (this.GetUrl (urlPart));
			request.CookieContainer = this.cookieJar;
            request.Timeout = this.timeout;
			request.Method = method;

            this.statusCode = 0;
            this.statusDescription = null;
            this.timedOut = false;

            // set the start time
            this.startTime = DateTime.Now;
			LOG.InfoFormat ("{0} {1}", method, urlPart);
			
			if ("POST".Equals (method))
			{
				byte[] postData = ASCIIEncoding.UTF8.GetBytes (CreateRequestString (data));
				
				request.ContentType = "application/x-www-form-urlencoded";
				request.ContentLength = postData.Length;
				
				LOG.DebugFormat ("Post data: {0}", CreateRequestString (data));

                using (Stream newStream = request.GetRequestStream())
                {
                    newStream.Write(postData, 0, postData.Length);
                    newStream.Close();
                }
			}
			
			HttpWebResponse response = null;
			
			try
			{
				response = (HttpWebResponse)request.GetResponse();
			}
			catch (WebException we)
			{
                if (we.Status == WebExceptionStatus.Timeout) 
                {
                    this.timedOut = true;

                    // no response so we can leave
                    return null;
                }

				response = (HttpWebResponse)we.Response;
			}            

			if (response != null)
			{
				this.statusCode = (int)response.StatusCode;
				this.statusDescription = response.StatusDescription;
                this.timedOut = false;
			}

			LOG.DebugFormat("{0} {1}", this.statusCode, this.statusDescription);

			return response;
		}

		private string CreateRequestString (NameValueCollection data)
		{
			StringBuilder requestString = new StringBuilder ();
			
			foreach (string key in data.Keys)
			{
				if (requestString.Length > 0)
				{
					requestString.Append ("&");
				}
				
				requestString.Append (key);
				requestString.Append ("=");
				requestString.Append (data[key]);
			}
			
			return requestString.ToString ();
		}

		public void End ()
		{
			this.cookieJar = null;
		}

		#region IDisposable Members

		public void Dispose ()
		{
			this.End ();
		}
		
		#endregion
	}
}
