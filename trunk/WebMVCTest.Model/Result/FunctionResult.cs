using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebMVCTest.Model.Result
{
	public class FunctionResult
	{
		public bool Executed { get; set; }

		public TimeSpan? ExecutionTime { get; set; }

		public bool TimedOut { get; set; }

		public bool Success { get; set; }

        public string ResponseText { get; set; }

        public string StatusDescription { get; set; }

        public int StatusCode { get; set; }

		public string ExecutedUrl { get; set; }

		public FunctionResult()
		{
		}
	}
}
