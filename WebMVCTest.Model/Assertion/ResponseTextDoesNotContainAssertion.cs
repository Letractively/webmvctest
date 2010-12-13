using System;
namespace WebMVCTest.Model.Assertion
{
	public class ResponseTextDoesNotContainAssertion : AbstractAssertion
	{
		private string test;

        public ResponseTextDoesNotContainAssertion(String test)
		{
			this.test = test;
		}

		public override bool Assert (IResponse response)
		{
			if (!string.IsNullOrEmpty (response.GetResponseText()))
			{
				return !response.GetResponseText().Contains(test);
			}
			else
			{
				return false;
			}
		}
	}
}

