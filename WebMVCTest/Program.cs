using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Specialized;
using WebMVCTest.Model;
using WebMVCTest.Engine;
using WebMVCTest.Model.Assertion;
using WebMVCTest.Model.Processor;
using WebMVCTest.XML;
using WebMVCTest.Report;

namespace WebMVCTest
{
	public class Program
	{
		static void Main(string[] args)
		{
			Program program = new Program();
			
			Settings settings = new Settings();
            if (program.ParseCommandLine(settings, args))
            {
                program.Run(settings);
            }
            else
            {
                Console.WriteLine("Usage: WebMvcText.exe [options] [file]");
                Console.WriteLine("Options:");
                Console.WriteLine(" -? (or -h)    - this information");
                Console.WriteLine(" -l            - write all logging output to a file");
                Console.WriteLine(" -t [testSet]  - execute a specific testSet");
                Console.WriteLine(" -d            - show debug information");
                Console.WriteLine(" -r            - create a PDF report");
            }
		}

		private bool ParseCommandLine(Settings settings, string[] args)
		{
			bool noerrors = true;
			
			for (int index = 0; index < args.Length; index++)
			{
				string arg = args[index];
				bool argumentParsed = false;
				
				if (!string.IsNullOrEmpty(arg))
				{
					if (arg[0].Equals('-'))
					{
						if (arg.Equals("-d"))
						{
							argumentParsed = true;
							settings.ShowDebug = true;
						}
						else if (arg.Equals("-l"))
						{
							argumentParsed = true;
							settings.LogToFile = true;
						}
						else if (arg.Equals("-t"))
						{
							argumentParsed = true;
							settings.TestSet = args[++index];
						}
						else if (arg.Equals("-r"))
						{
							argumentParsed = true;
							settings.Report = true;
						}
                        else if (arg.Equals("-h") || arg.Equals("-?"))
                        {
                            argumentParsed = true;
                            return false;
                        }
                    }
					else
					{
						if (settings.FileName != null)
						{
							noerrors = false;
							Console.WriteLine("Filename allready specified, only one allowed.");
							break;
						}
						else
						{
							argumentParsed = true;
							settings.FileName = arg;
						}
					}
					
					if (!argumentParsed)
					{
						noerrors = false;
						Console.WriteLine(string.Format("Unparsable argument '{0}'.", arg));
					}
				}
			}
			
			if (string.IsNullOrEmpty(settings.FileName))
			{
				noerrors = false;
				Console.WriteLine("No filename specified.");
			}
			
			return noerrors;
		}

		public void Run(Settings settings)
		{
			ProjectValidator validator = new ProjectValidator();
			
			if (validator.IsValid(settings.FileName))
			{
				ProjectReader reader = new ProjectReader(settings.FileName);
				Project project = reader.ReadProject();
				reader.Close();

				ScriptRunner.GetInstance().SetSettings(settings);
				ScriptRunner.GetInstance().Execute(project);

				if (settings.Report)
				{
                    ReportGenerator.GetInstance().SetSettings(settings);
                    ReportGenerator.GetInstance().CreatePDF(project);
				}
			}
		}
	}
}
