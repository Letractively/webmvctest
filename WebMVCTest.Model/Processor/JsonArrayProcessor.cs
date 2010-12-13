using System;
using Newtonsoft.Json.Linq;
namespace WebMVCTest.Model.Processor
{
    public class JsonArrayProcessor : AbstractProcessor
    {
        private string row;

        private string column;

        public JsonArrayProcessor(string variable, string row, string column)
            : base(variable)
        {
            this.column = column;
            this.row = row;
        }

        public override void Process(IResponse response, Context context)
        {
            if (response.GetStatusCode() == 200)
            {
                JArray array = JArray.Parse(response.GetResponseText());
                string value = null;

                if (column.Equals("{ROWNUM}"))
                {
                    value = getRowNumber(array.Count, context).ToString();
                }
                else
                {
                    value = array[getRowNumber(array.Count, context)][column].ToString();
                }

                LOG.DebugFormat("Adding to context: {0}={1}", GetVariableName(), value);
                context.Add(GetVariableName(), value);
            }
        }

        private int getRowNumber(int size, Context context)
        {
            int rowNum = 0;

            if (!Int32.TryParse(row, out rowNum))
            {
                if (row.Equals("?"))
                {
                    rowNum = new Random().Next(size);
                }
                else
                {
                    if (!Int32.TryParse(context.GetResolver().Resolve(row), out rowNum))
                    {
                        LOG.ErrorFormat("Parsing of row variable '{0}' failed.", row);
                    }
                }
            }

            return rowNum;
        }
    }
}

