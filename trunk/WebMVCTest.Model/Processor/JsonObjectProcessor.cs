using System;
using Newtonsoft.Json.Linq;
namespace WebMVCTest.Model.Processor
{
    public class JsonObjectProcessor : AbstractProcessor
    {
        private string column;

        public JsonObjectProcessor(string variable, string column)
            : base(variable)
        {
            this.column = column;
        }

        public override void Process(IResponse response, Context context)
        {
            if (response.GetStatusCode() == 200)
            {
                if (!string.IsNullOrEmpty(response.GetResponseText()))
                {
                    JObject obj = JObject.Parse(response.GetResponseText());

                    string value = obj[column].ToString();

                    LOG.DebugFormat("Adding to context: {0}={1}", GetVariableName(), value);
                    context.Add(GetVariableName(), value);
                }
            }
        }
    }
}

