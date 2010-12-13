using System;
namespace WebMVCTest.Model.Assertion
{
	public class ResponseTextContainsAssertion : AbstractAssertion
	{
		private string test;

		public ResponseTextContainsAssertion (String test)
		{
			this.test = test;
		}

		public override bool Assert (IResponse response)
		{
			if (!string.IsNullOrEmpty (response.GetResponseText()))
			{
				return response.GetResponseText().Contains(test);
			}
			else
			{
				return false;
			}
		}
	}
}

