using System;
using log4net;
namespace WebMVCTest.Model.Processor
{
	public abstract class AbstractProcessor
	{
        protected static ILog LOG = LogManager.GetLogger(typeof(AbstractProcessor));

		private string variable;

		public AbstractProcessor (string variable)
		{
			this.variable = variable;
		}

		protected string GetVariableName ()
		{
			return this.variable;
		}

		public abstract void Process (IResponse response, Context context);
	}
}

