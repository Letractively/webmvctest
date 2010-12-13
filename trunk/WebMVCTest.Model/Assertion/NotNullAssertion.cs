using System;
namespace WebMVCTest.Model.Assertion
{
	public class NotNullAssertion : AbstractAssertion
	{
		public override bool Assert (IResponse response)
		{
			return response.GetResponseText() != null;
		}
	}
}

