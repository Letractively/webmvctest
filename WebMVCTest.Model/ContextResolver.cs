using System;
using System.Text.RegularExpressions;
namespace WebMVCTest.Model
{
    public class ContextResolver : IResolver
    {
        private Context context;

        public ContextResolver(Context context)
        {
            this.context = context;
        }

        public string Resolve(string data)
        {
            if (data != null)
            {
                Regex regexp = new Regex("\\{[a-zA-Z0-9.-_]*}");

                MatchCollection matches = regexp.Matches(data);

                foreach (Match match in matches)
                {
                    string reference = match.Value;

                    if (reference != null)
                    {

                        // remove the { and }
                        string key = reference.Substring(1);
                        key = key.Substring(0, key.Length - 1);

                        if (!this.context.Contains(key)) 
                        {
                            throw new Exception(string.Format("'{0}' not found in current context.", key));
                        }

                        data = data.Replace(reference, this.context.Get(key));
                    }
                }
            }

            return data;
        }
    }
}

