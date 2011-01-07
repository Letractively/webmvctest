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
		private static ILog LOG = LogManager.GetLogger(typeof(HttpSession));

		private CookieContainer cookieJar = new CookieContainer();

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

		private Authentication authentication;

		public HttpSession(string baseUrl, Authentication authentication)
		{
			LOG.Debug("Create HttpSession with baseUrl: " + baseUrl);
			if (authentication != null)
			{
				LOG.Debug("Using authentication: " + authentication.Name);
			}
			
			this.SetBaseUrl(baseUrl);
			this.SetAuthentication(authentication);
		}

		public void SetAuthentication(Authentication authentication)
		{
			this.authentication = authentication;
		}

		public void SetBaseUrl(string baseUrl)
		{
			this.baseUrl = baseUrl;
		}

		private string GetUrl(string urlPart)
		{
			return this.baseUrl + urlPart;
		}

		public void Execute(string url, string method, Dictionary<string, string> headers, NameValueCollection data, string postbody)
		{
			HttpWebResponse response = GetResponse(url, method, headers, data, postbody);
			
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

		public void Execute(string url, string method, Dictionary<string, string> headers)
		{
			HttpWebResponse response = GetResponse(url, method, headers, null, null);
			
			if (response != null)
			{
				StreamReader stream = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
				this.responseText = stream.ReadToEnd();
				stream.Close();
			}
			
			SetExecutionTime();
		}

		public int GetStatusCode()
		{
			return this.statusCode;
		}

		public string GetStatusDescription()
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

		private HttpWebResponse GetResponse(string urlPart, string method, Dictionary<string, string> headers, NameValueCollection data, string postbody)
		{
			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(this.GetUrl(urlPart));
			
			if (this.authentication != null)
			{
				LOG.DebugFormat("Using authentication '{0}'", this.authentication.Name);
				CredentialCache cc = new CredentialCache();
				cc.Add(new Uri(this.baseUrl), "BASIC", new NetworkCredential(this.authentication.Name, this.authentication.Password));
				
				request.Credentials = cc;
			}

            request.AllowAutoRedirect = false;
			request.CookieContainer = this.cookieJar;
			request.Timeout = this.timeout;
			request.Method = method;
			
			// add all headers from the function
			foreach (string key in headers.Keys)
			{
				if (key.Equals("Content-Type"))
				{
					request.ContentType = headers[key];
				}
				else if (key.Equals("Content-Length"))
				{
					request.ContentLength = long.Parse(headers[key]);
				}
				else if (key.Equals("Accept"))
				{
					request.Accept = headers[key];
				}
				else if (key.Equals("User-Agent"))
				{
					request.UserAgent = headers[key];
				}
				else if (key.Equals("Transfer-Encoding"))
				{
					request.TransferEncoding = headers[key];
				}
				else if (key.Equals("Referer"))
				{
					request.Referer = headers[key];
				}
				else if (key.Equals("If-Modified-Since"))
				{
					request.IfModifiedSince = DateTime.Parse(headers[key]);
				}
				else
				{
					LOG.WarnFormat("Header '{0}' unknown, skipped.", key);
				}
			}
			
			this.statusCode = 0;
			this.statusDescription = null;
			this.timedOut = false;

			// set the start time
			this.startTime = DateTime.Now;
			LOG.InfoFormat("{0} {1}", method, urlPart);

            if ("POST".Equals(method) || "PUT".Equals(method))
			{
				string postData = null;

				if (data.Count > 0)
				{
					postData = CreateRequestString(data);
				}
				else
				{
                    if (postbody == null) 
                    {
                        postbody = "";
                    }

					postData = postbody;
				}

                if (request.ContentType == null)
				{
					request.ContentType = "application/x-www-form-urlencoded";
				}
				request.ContentLength = postData.Length;

				LOG.DebugFormat("Post data: {0}", postData);

				using (Stream newStream = request.GetRequestStream())
				{
					newStream.Write(ASCIIEncoding.UTF8.GetBytes(postData), 0, postData.Length);
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

		private string CreateRequestString(NameValueCollection data)
		{
			StringBuilder requestString = new StringBuilder();
			
			foreach (string key in data.Keys)
			{
				if (requestString.Length > 0)
				{
					requestString.Append("&");
				}
				
				requestString.Append(key);
				requestString.Append("=");
				requestString.Append(data[key]);
			}
			
			return requestString.ToString();
		}

		public void End()
		{
			this.cookieJar = null;
		}

		#region IDisposable Members

		public void Dispose()
		{
			this.End();
		}
		
		#endregion
    }
}
