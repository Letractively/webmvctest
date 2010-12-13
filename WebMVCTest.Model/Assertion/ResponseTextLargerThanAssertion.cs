using System;
namespace WebMVCTest.Model.Assertion
{
	public class ResponseTextLargerThanAssertion : AbstractAssertion
	{
		private int minimalLength;

		public ResponseTextLargerThanAssertion (int minimalLength)
		{
			this.minimalLength = minimalLength;
		}

		public override bool Assert (IResponse response)
		{
			if (!string.IsNullOrEmpty (response.GetResponseText()))
			{
				return response.GetResponseText().Length > this.minimalLength;
			}
			else
			{
				return false;
			}
		}
	}
}

