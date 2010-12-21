using System;
using System.Collections.Generic;

namespace WebMVCTest.Model
{
	public class TestSet
	{
        private Authentication authentication;

		public string Name { get; set; }

		public string Description { get; set; }

		public List<Function> Functions { get; set; }

        private Context context = null;

        public bool Executed { get; set; }

        private Project project;

        public TestSet(Project project)
        {
            this.project = project;
        }

        public Authentication GetAuthentication()
        {
            return this.authentication;
        }

		public Context GetContext ()
		{
            if (this.context == null) 
            {
                this.context = this.project.GetInitialContext();
            }

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

        public void SetAuthentication(Authentication authentication)
        {
            this.authentication = authentication;
        }
    }
}

