using System;
using System.Collections.Specialized;
using WebMVCTest.Model.Assertion;
using WebMVCTest.Model.Processor;
using System.Collections.Generic;
using WebMVCTest.Model.Result;

namespace WebMVCTest.Model
{
	public class Function : IKeyValueContainer
	{
		private string url;

		private int waitInSeconds;

		private string postbody;

		private Dictionary<string, string> postData = new Dictionary<string, string>();

		private Dictionary<string, string> headers = new Dictionary<string, string>();

		private List<AbstractAssertion> assertions = new List<AbstractAssertion>();

		private List<AbstractProcessor> processors = new List<AbstractProcessor>();

		private FunctionResult result = new FunctionResult();

		public string Name { get; set; }

		public string Description { get; set; }

		private bool whenPreviousFunctionFailed = false;

		/// <summary>
		/// Either 'GET' or 'POST'
		/// </summary>
		public string Method { get; set; }

		private Project project;

		public Function(Project project)
		{
			this.project = project;
		}

		public void AddKeyValueData(string key, string value)
		{
			this.postData.Add(key, value);
		}

		public void AddAssertion(AbstractAssertion assertion)
		{
			this.assertions.Add(assertion);
		}

		public void AddProcessor(AbstractProcessor processor)
		{
			this.processors.Add(processor);
		}

		public void Assert(IResponse response)
		{
            // when there are no assertions it is a success
            bool success = (this.assertions.Count == 0);
			
			// when the response was timedout, there is no data to test/assert
			if (!response.IsTimedOut())
			{
				foreach (AbstractAssertion assertion in this.assertions)
				{
					success = assertion.Assert(response);
					
					if (!success)
					{
						break;
					}
				}
				
			}
			
			// record results
			this.result.Executed = true;
			this.result.ExecutionTime = response.GetExecutionTime();
			this.result.Success = success;
			this.result.TimedOut = response.IsTimedOut();
			this.result.StatusCode = response.GetStatusCode();
			this.result.StatusDescription = response.GetStatusDescription();
			this.result.ResponseText = response.GetResponseText();
		}

		public Dictionary<string, string> GetHeaders()
		{
			Dictionary<string, string> newHeaders = new Dictionary<string, string>();

			foreach (KeyValuePair<string, string> pair in this.headers)
			{
				newHeaders.Add(pair.Key, pair.Value);
			}
			foreach (KeyValuePair<string, string> pair in this.project.GetDefaultHeaders())
			{
				newHeaders.Add(pair.Key, pair.Value);
			}

			return newHeaders;
		}

		public string GetUrl(IResolver resolver)
		{
			string url = resolver.Resolve(this.url);
			this.result.ExecutedUrl = url;
			
			return url;
		}

		public NameValueCollection GetPostData(IResolver resolver)
		{
			NameValueCollection postData = new NameValueCollection();
			
			foreach (string key in this.postData.Keys)
			{
				postData[key] = resolver.Resolve(this.postData[key].ToString());
			}
			
			return postData;
		}

		public FunctionResult GetResult()
		{
			return this.result;
		}

		public int GetWaitInSeconds()
		{
			return this.waitInSeconds;
		}

		public string GetPostBody()
		{
			return this.postbody;
		}

		public bool HasAssertions()
		{
			return this.assertions.Count > 0;
		}

		public bool HasProcessors()
		{
			return this.processors.Count > 0;
		}

		public void Process(IResponse response, Context context)
		{
			foreach (AbstractProcessor processor in this.processors)
			{
				processor.Process(response, context);
			}
		}

		public void Skip()
		{
			this.result.Executed = false;
			this.result.ExecutedUrl = this.url;
		}

		public void SetUrl(string url)
		{
			this.url = url;
		}

		public void SetHeaders(Dictionary<string, string> headers)
		{
			this.headers = headers;
		}

		public void SetPostBody(string postbody)
		{
			this.postbody = postbody;
		}

		public void SetWaitInSeconds(int seconds)
		{
			this.waitInSeconds = seconds;
		}

		public void SetWhenPreviousFunctionFailed(bool whenPreviousFunctionFailed)
		{
			this.whenPreviousFunctionFailed = whenPreviousFunctionFailed;
		}

		public bool WhenPreviousFunctionFailed()
		{
			return this.whenPreviousFunctionFailed;
		}

		public Function Copy()
		{
			Function function = new Function(this.project);
			function.Name = this.Name;
			function.Description = this.Description;
			function.Method = this.Method;
			function.headers = this.headers;
			function.assertions = this.assertions;
			function.processors = this.processors;
			function.postData = this.postData;
			function.url = this.url;
			function.waitInSeconds = this.waitInSeconds;
			function.whenPreviousFunctionFailed = this.whenPreviousFunctionFailed;
			
			return function;
		}
	}
}

