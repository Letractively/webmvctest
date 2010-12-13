using System;
using System.Collections.Generic;
namespace WebMVCTest.Model
{
	public class Project
	{
		public string Name { get; set; }

		public string Description { get; set; }

		public string BaseUrl { get; set; }

		public List<TestSet> TestSets { get; set; }

		public List<Function> Functions { get; set; }

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
	}
}

