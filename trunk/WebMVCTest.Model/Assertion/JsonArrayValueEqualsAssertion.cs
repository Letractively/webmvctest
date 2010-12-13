using System;
using Newtonsoft.Json.Linq;
namespace WebMVCTest.Model.Assertion
{
	public class JsonArrayValueEqualsAssertion : AbstractAssertion
	{
		private int row;

		private string column;

		private string value;

		public JsonArrayValueEqualsAssertion (int row, string column, string value)
		{
			this.row = row;
			this.column = column;
			this.value = value;
		}

		public override bool Assert (IResponse response)
		{
			if (!string.IsNullOrEmpty (response.GetResponseText()))
			{
				JArray array = JArray.Parse (response.GetResponseText());

				return this.value.Equals (array[row][column].ToString ());
			}
			else
			{
				return false;
			}
		}
	}
}

