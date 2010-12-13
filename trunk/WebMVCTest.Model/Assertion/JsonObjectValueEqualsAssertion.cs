using System;
using Newtonsoft.Json.Linq;
namespace WebMVCTest.Model.Assertion
{
	public class JsonObjectValueEqualsAssertion : AbstractAssertion
	{
		private string column;

		private string value;

		public JsonObjectValueEqualsAssertion (string column, string value)
		{
			this.column = column;
			this.value = value;
		}

		public override bool Assert (IResponse response)
		{
			if (!string.IsNullOrEmpty (response.GetResponseText()))
			{
				JObject obj = JObject.Parse (response.GetResponseText());

				return this.value.Equals (obj[column].ToString ());
			}
			else
			{
				return false;
			}
		}
	}
}

