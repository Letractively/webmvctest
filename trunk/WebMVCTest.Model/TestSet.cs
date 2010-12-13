using System;
using System.Collections.Generic;

namespace WebMVCTest.Model
{
	public class TestSet
	{
		public string Name { get; set; }

		public string Description { get; set; }

		public List<Function> Functions { get; set; }

		private Context context = new Context ();

        public bool Executed { get; set; }

		public Context GetContext ()
		{
			return this.context;
		}

		public void Clear ()
		{
			this.context.Clear ();
		}

		public Function GetFunctionByName (string name)
		{
			foreach (Function function in this.Functions)
			{
				if (name.Equals (function.Name))
				{
					return function.Copy();
				}
			}
			
			return null;
		}
	}
}

