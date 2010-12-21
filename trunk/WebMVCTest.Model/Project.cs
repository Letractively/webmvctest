using System;
using System.Collections.Generic;
namespace WebMVCTest.Model
{
    public class Project : IKeyValueContainer
	{
		public string Name { get; set; }

		public string Description { get; set; }

		public string BaseUrl { get; set; }

        private List<Authentication> authentications = new List<Authentication>();

		public List<TestSet> TestSets { get; set; }

		public List<Function> Functions { get; set; }

        private Context initialContext = new Context();

        public void AddAuthentication(Authentication authentication)
        {
            this.authentications.Add(authentication);
        }

        /// <summary>
        /// Add a key value pair to the initial context
        /// </summary>
        /// <param name="key">key of the variable</param>
        /// <param name="value">value ofthe variable</param>
        public void AddKeyValueData(string key, string value)
        {
            this.initialContext.Add(key, value);
        }

		public TestSet GetTestSetByName(string name)
		{
			if (this.TestSets != null)
			{
				foreach (TestSet testSet in this.TestSets)
				{
					if (name.Equals(testSet.Name))
					{
						return testSet;
					}
				}
			}
			
			return null;
		}

		public string GetSaveName()
		{
			return this.Name.Replace(" ", "_");
		}

		public Function GetFunctionByName(string name)
		{
			if (this.Functions != null)
			{
				foreach (Function function in this.Functions)
				{
					if (name.Equals(function.Name))
					{
                        return function.Copy();
					}
				}
			}
			
			foreach (TestSet testSet in this.TestSets)
			{
				Function function = testSet.GetFunctionByName(name);
				
				if (function != null)
				{
                    // the function is allready a copy!
                    return function;
				}
			}
			
			return null;
		}

        public Authentication GetAuthenticationByName(string name)
        {
            foreach (Authentication authentication in this.authentications)
            {
                if (name.Equals(authentication.Name))
                {
                    return authentication;
                }
            }

            return null;
        }

        public Context GetInitialContext()
        {
            return this.initialContext.Copy();
        }
    }
}

