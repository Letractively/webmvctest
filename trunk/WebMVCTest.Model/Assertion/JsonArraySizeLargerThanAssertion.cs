using System;
using Newtonsoft.Json.Linq;
namespace WebMVCTest.Model.Assertion
{
	public class JsonArraySizeLargerThanAssertion : AbstractAssertion
	{
		private int size;

		public JsonArraySizeLargerThanAssertion (int size)
		{
			this.size = size;
		}

		public override bool Assert (IResponse response)
		{
			if (!string.IsNullOrEmpty (response.GetResponseText()))
			{
				JArray array = JArray.Parse (response.GetResponseText());

				return array.Count > this.size;
			}
			else
			{
				return false;
			}
		}
	}
}

