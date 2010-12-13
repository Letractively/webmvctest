using System;
using System.Collections.Generic;
using System.Text;

namespace WebMVCTest.Engine
{
	public class Settings
	{
		public string FileName { get; set; }

		public string TestSet { get; set; }

		public bool ShowDebug { get; set; }

		public bool LogToFile { get; set; }

		public bool Report { get; set; }
	}
}
