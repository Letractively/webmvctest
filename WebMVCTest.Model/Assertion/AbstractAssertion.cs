using System;
namespace WebMVCTest.Model.Assertion
{
	public abstract class AbstractAssertion
	{
		public abstract bool Assert(IResponse response);
	}
}

