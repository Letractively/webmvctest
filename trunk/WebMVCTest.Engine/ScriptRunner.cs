using System;
using WebMVCTest.Model;
using System.Collections.Specialized;
using WebMVCTest.Model.Assertion;
using WebMVCTest.Model.Processor;
using log4net;
using log4net.Core;
using log4net.Config;
using log4net.Appender;
using log4net.Layout;
using System.IO;
using log4net.Repository.Hierarchy;
using System.Threading;

namespace WebMVCTest.Engine
{
	public class ScriptRunner
	{
		private static ILog LOG = LogManager.GetLogger(typeof(ScriptRunner));

		private static ScriptRunner current = new ScriptRunner();

		private Settings settings;

		private Project project;

		private bool lastFunctionSucceeded;

		private ScriptRunner()
		{
		}

		public static ScriptRunner GetInstance()
		{
			return current;
		}

		public void SetSettings(Settings settings)
		{
			this.settings = settings;
		}

		private void Initialize()
		{
			PatternLayout layout = new PatternLayout("%date %-5level %-15.15logger{1} - %message%newline");
			Level defaultLevel = this.settings.ShowDebug ? Level.Debug : Level.Info;
			
			ConsoleAppender consoleAppender = new ConsoleAppender();
			consoleAppender.Layout = layout;
			consoleAppender.Threshold = defaultLevel;
			
			Hierarchy hierarchy = (Hierarchy)LogManager.GetRepository();
			hierarchy.Root.Additivity = true;
			hierarchy.Root.AddAppender(consoleAppender);
			hierarchy.Root.Level = Level.All;
			
			if (this.settings.LogToFile)
			{
				string fileName = this.project.GetSaveName() + ".log";
				
				RollingFileAppender fileAppender = new RollingFileAppender();
				fileAppender.File = fileName;
				fileAppender.Layout = layout;
				fileAppender.ImmediateFlush = true;
				fileAppender.RollingStyle = RollingFileAppender.RollingMode.Size;
				fileAppender.MaximumFileSize = "10MB";
				fileAppender.AppendToFile = true;
				fileAppender.MaxSizeRollBackups = 10;
				fileAppender.Threshold = defaultLevel;
				fileAppender.ActivateOptions();
				
				hierarchy.Root.AddAppender(fileAppender);
			}
			
			hierarchy.Configured = true;
		}

		public void Execute(Project project)
		{
			this.project = project;
			
			Initialize();
			
			LOG.InfoFormat("Start testing: {0}", project.Name);
			
			if (!string.IsNullOrEmpty(this.settings.TestSet))
			{
				LOG.InfoFormat("Only executing testset {0}", this.settings.TestSet);
				
				Execute(this.project.GetTestSetByName(this.settings.TestSet));
			}

			else
			{
				foreach (TestSet testSet in this.project.TestSets)
				{
					Execute(testSet);
				}
			}
		}

		private void Execute(TestSet testSet)
		{
			// Each testset will run in it's own http session
			HttpSession session = new HttpSession(this.project.BaseUrl);
			
			LOG.InfoFormat("TestSet: {0}", testSet.Name);
			
			foreach (Function function in testSet.Functions)
			{
				Execute(session, testSet.GetContext(), function);
			}

            testSet.Executed = true;

			session.End();
			session = null;
		}

		private void Execute(HttpSession session, Context context, Function function)
		{
			LOG.InfoFormat("Function: {0}", function.Name);
			
			if (function.WhenPreviousFunctionFailed() && this.lastFunctionSucceeded)
			{
				LOG.InfoFormat("Function: {0} is not required to run, previous function succeeded.", function.Name);
				function.Skip();
				return;
			}
			
			if (function.GetWaitInSeconds() > 0)
			{
				LOG.InfoFormat("Waiting for {0} seconds...", function.GetWaitInSeconds());
				Thread.Sleep(function.GetWaitInSeconds() * 1000);
			}
			
			string url = function.GetUrl(context.GetResolver());
			
			if ("GET".Equals(function.Method))
			{
				session.Get(url);
			}
			else if ("POST".Equals(function.Method))
			{
				session.Post(url, function.GetPostData(context.GetResolver()));
			}
			else
			{
				// Not support http method
				throw new InvalidOperationException();
			}
			
			// execute tests on results
            try
            {
                function.Assert(session);
            }
            catch (Exception e) 
            {
                LOG.Error("Failed to run assertions on " + session.GetResponseText(), e);

                throw e;
            }

			// process the data
            try
            {
                function.Process(session, context);
            }
            catch (Exception e)
            {
                LOG.Error("Failed to run processor on " + session.GetResponseText(), e);

                throw e;
            }
            			
			LOG.Info("Function call: " + (function.GetResult().Success ? "SUCCESS" : "FAILED"));
			
			this.lastFunctionSucceeded = function.GetResult().Success;
		}
	}
}

