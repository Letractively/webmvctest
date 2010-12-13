using System;
namespace WebMVCTest.Model.Assertion
{
	public class ResponseCodeEqualsAssertion : AbstractAssertion
	{
		private int statusCode;

		public ResponseCodeEqualsAssertion (int statusCode)
		{
			this.statusCode = statusCode;
		}

		public override bool Assert (IResponse response)
		{
			return this.statusCode.Equals(response.GetStatusCode());
		}
	}
}

