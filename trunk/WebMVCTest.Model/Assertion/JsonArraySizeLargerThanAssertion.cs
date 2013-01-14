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
				try {
					JArray array = JArray.Parse (response.GetResponseText());

					return array.Count > this.size;
				}
				catch (Exception e) 
				{
					LOG.Debug("Response can not be parsed as JSON.", e);
					return false;
				}
			}
			else
			{
				return false;
			}
		}
	}
}

