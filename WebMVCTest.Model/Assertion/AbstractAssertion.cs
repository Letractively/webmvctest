using System;
using log4net;

namespace WebMVCTest.Model.Assertion
{
	public abstract class AbstractAssertion
	{
		protected static ILog LOG = LogManager.GetLogger(typeof(AbstractAssertion));
		
		public abstract bool Assert(IResponse response);
	}
}

