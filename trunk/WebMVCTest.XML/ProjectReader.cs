using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;
using System.Xml.Schema;
using WebMVCTest.Model;
using WebMVCTest.Model.Assertion;
using WebMVCTest.Model.Processor;
using System.Reflection;

namespace WebMVCTest.XML
{
	public class ProjectReader
	{
		private XmlTextReader reader = null;

		public ProjectReader(string fileName)
		{
			this.reader = new XmlTextReader(fileName);
		}

		public Project ReadProject()
		{
			Project project = null;
			
			while (reader.Read())
			{
				switch (reader.NodeType)
				{
					case XmlNodeType.Element:
						
						if (reader.Name.Equals("project"))
						{
							project = new Project();
						}

						else if (reader.Name.Equals("name"))
						{
							project.Name = reader.ReadElementContentAsString();
						}
						else if (reader.Name.Equals("description"))
						{
							project.Description = GetDescription();
						}
						else if (reader.Name.Equals("baseUrl"))
						{
							project.BaseUrl = reader.ReadElementContentAsString();
						}
						else if (reader.Name.Equals("context"))
						{
							ReadParams(project);
						}
						else if (reader.Name.Equals("headers"))
						{
							project.SetDefaultHeaders(ReadHeaders());
						}
						else if (reader.Name.Equals("testSet"))
						{
							if (project.TestSets == null)
							{
								project.TestSets = new List<TestSet>();
							}
							project.TestSets.Add(ReadTestSet(project));
						}
						else if (reader.Name.Equals("functions"))
						{
							if (project.Functions == null)
							{
								project.Functions = new List<Function>();
							}
						}
						else if (reader.Name.Equals("function"))
						{
							project.Functions.Add(ReadFunction(project));
						}
						else if (reader.Name.Equals("authentications"))
						{
							ReadAuthentications(project);
						}
						break;
					
					case XmlNodeType.EndElement:
						
						if (reader.Name.Equals("project"))
						{
							return project;
						}
						
						break;
				}
			}
			
			return project;
		}

		private Dictionary<string, string> ReadHeaders()
		{
			Dictionary<string, string> headers = new Dictionary<string, string>();

			while (reader.Read())
			{
				switch (reader.NodeType)
				{
					case XmlNodeType.Element:

						if (reader.Name.Equals("header"))
						{
							string key = null;
							string value = null;
							for (int i = 0; i < reader.AttributeCount; i++)
							{
								reader.MoveToAttribute(i);
								if (reader.Name.Equals("key"))
								{
									key = reader.Value;
								}
								else if (reader.Name.Equals("value"))
								{
									value = reader.Value;
								}
							}

							headers.Add(key, value);
						}

						break;

					case XmlNodeType.EndElement:

						if (reader.Name.Equals("headers"))
						{
							return headers;
						}

						break;
				}
			}

			return headers;
		}

		private void ReadAuthentications(Project project)
		{
			Authentication authentication = null;

			while (reader.Read())
			{
				switch (reader.NodeType)
				{
					case XmlNodeType.Element:

						if (reader.Name.Equals("authentication"))
						{
							authentication = new Authentication();
							
							for (int i = 0; i < reader.AttributeCount; i++)
							{
								reader.MoveToAttribute(i);
								if (reader.Name.Equals("type"))
								{
									authentication.Type = reader.Value;
								}
							}
						}
						else if (reader.Name.Equals("name"))
						{
							authentication.Name = reader.ReadElementContentAsString();
						}
						else if (reader.Name.Equals("user"))
						{
							authentication.User = reader.ReadElementContentAsString();
						}
						else if (reader.Name.Equals("password"))
						{
							authentication.Password = reader.ReadElementContentAsString();
						}
						
						break;
					
					case XmlNodeType.EndElement:
						
						if (reader.Name.Equals("authentication"))
						{
							project.AddAuthentication(authentication);
							return;
						}
						else if (reader.Name.Equals("authentications"))
						{
							return;
						}
						
						break;
				}
			}
		}

		private TestSet ReadTestSet(Project project)
		{
			TestSet testSet = new TestSet(project);
			
			while (reader.Read())
			{
				switch (reader.NodeType)
				{
					case XmlNodeType.Element:
						
						if (reader.Name.Equals("name"))
						{
							testSet.Name = reader.ReadElementContentAsString();
						}
						else if (reader.Name.Equals("description"))
						{
							testSet.Description = GetDescription();
						}
						else if (reader.Name.Equals("authentication"))
						{
							string name = null;
							
							for (int i = 0; i < reader.AttributeCount; i++)
							{
								reader.MoveToAttribute(i);
								if (reader.Name.Equals("ref"))
								{
									name = reader.Value;
								}
							}
							
							testSet.SetAuthentication(project.GetAuthenticationByName(name));
						}
						else if (reader.Name.Equals("function"))
						{
							string refName = null;
							int waitInSeconds = 0;
							bool whenPreviousFunctionFailed = false;
							
							if (testSet.Functions == null)
							{
								testSet.Functions = new List<Function>();
							}
							if (reader.HasAttributes)
							{
								for (int i = 0; i < reader.AttributeCount; i++)
								{
									reader.MoveToAttribute(i);
									
									if (reader.Name.Equals("ref"))
									{
										refName = reader.Value;
									}

									else if (reader.Name.Equals("whenPreviousFunctionFailed"))
									{
										string boolVal = reader.Value;
										if (boolVal != null)
										{
											whenPreviousFunctionFailed = (boolVal.Equals("true") || boolVal.Equals("on") || boolVal.Equals("1"));
										}
									}
									else if (reader.Name.Equals("waitInSeconds"))
									{
										waitInSeconds = Int32.Parse(reader.Value);
									}
								}
								
								if (!string.IsNullOrEmpty(refName))
								{
									Function functionRef = project.GetFunctionByName(reader.Value);
									if (functionRef != null)
									{
										testSet.Functions.Add(functionRef);
									}
								}
							}
							
							if (string.IsNullOrEmpty(refName))
							{
								Function function = ReadFunction(project);
								function.SetWaitInSeconds(waitInSeconds);
								function.SetWhenPreviousFunctionFailed(whenPreviousFunctionFailed);
								
								testSet.Functions.Add(function);
							}
						}
						break;
					
					case XmlNodeType.EndElement:
						
						if (reader.Name.Equals("testSet"))
						{
							return testSet;
						}

						break;
				}
			}
			
			return testSet;
		}

		private Function ReadFunction(Project project)
		{
			Function function = new Function(project);
			
			while (reader.Read())
			{
				switch (reader.NodeType)
				{
					case XmlNodeType.Element:
						
						if (reader.Name.Equals("name"))
						{
							function.Name = reader.ReadElementContentAsString();
						}
						else if (reader.Name.Equals("description"))
						{
							function.Description = GetDescription();
						}
						else if (reader.Name.Equals("method"))
						{
							function.Method = reader.ReadElementContentAsString();
						}
						else if (reader.Name.Equals("url"))
						{
							function.SetUrl(reader.ReadElementContentAsString());
						}
						else if (reader.Name.Equals("postbody"))
						{
							function.SetPostBody(reader.ReadElementContentAsString());
						}
						else if (reader.Name.Equals("params"))
						{
							ReadParams(function);
						}
						else if (reader.Name.Equals("headers"))
						{
							function.SetHeaders(ReadHeaders());
						}
						else if (reader.Name.Equals("assertions"))
						{
							ReadAssertions(function);
						}
						else if (reader.Name.Equals("processors"))
						{
							ReadProcessors(function);
						}
						break;
					
					case XmlNodeType.EndElement:
						
						if (reader.Name.Equals("function"))
						{
							return function;
						}

						
						break;
				}
			}
			
			return function;
		}

		private void ReadParams(IKeyValueContainer keyValueStore)
		{
			while (reader.Read())
			{
				switch (reader.NodeType)
				{
					case XmlNodeType.Element:
						
						if (reader.Name.Equals("param"))
						{
							string key = null;
							string val = null;
							
							for (int i = 0; i < reader.AttributeCount; i++)
							{
								reader.MoveToAttribute(i);
								if (reader.Name.Equals("key"))
								{
									key = reader.Value;
								}

								else if (reader.Name.Equals("value"))
								{
									val = reader.Value;
								}
							}
							
							keyValueStore.AddKeyValueData(key, val);
						}

						
						break;
					
					case XmlNodeType.EndElement:
						
						if (reader.Name.Equals("params"))
						{
							return;
						}

						else if (reader.Name.Equals("context"))
						{
							return;
						}
						
						break;
				}
			}
		}

		private void ReadAssertions(Function function)
		{
			while (reader.Read())
			{
				switch (reader.NodeType)
				{
					case XmlNodeType.Element:
						
						if (reader.Name.Equals("responseTextContains"))
						{
							reader.MoveToFirstAttribute();
							reader.ReadAttributeValue();
							function.AddAssertion(new ResponseTextContainsAssertion(reader.Value));
						}

						else if (reader.Name.Equals("responseTextDoesNotContain"))
						{
							reader.MoveToFirstAttribute();
							reader.ReadAttributeValue();
							function.AddAssertion(new ResponseTextDoesNotContainAssertion(reader.Value));
						}
						else if (reader.Name.Equals("responseTextLargerThan"))
						{
							reader.MoveToFirstAttribute();
							reader.ReadAttributeValue();
							function.AddAssertion(new ResponseTextLargerThanAssertion(Int32.Parse(reader.Value)));
						}
						else if (reader.Name.Equals("responseCodeEquals"))
						{
							reader.MoveToFirstAttribute();
							reader.ReadAttributeValue();
							function.AddAssertion(new ResponseCodeEqualsAssertion(Int32.Parse(reader.Value)));
						}
						else if (reader.Name.Equals("notNull"))
						{
							function.AddAssertion(new NotNullAssertion());
						}
						else if (reader.Name.Equals("jsonArrayValueEquals"))
						{
							int row = 0;
							string column = null;
							string value = null;
							
							for (int i = 0; i < reader.AttributeCount; i++)
							{
								reader.MoveToAttribute(i);
								if (reader.Name.Equals("row"))
								{
									row = Int32.Parse(reader.Value);
								}


								else if (reader.Name.Equals("column"))
								{
									column = reader.Value;
								}
								else if (reader.Name.Equals("value"))
								{
									value = reader.Value;
								}
							}
							
							function.AddAssertion(new JsonArrayValueEqualsAssertion(row, column, value));
						}
						else if (reader.Name.Equals("jsonObjectValueEquals"))
						{
							string column = null;
							string value = null;
							
							for (int i = 0; i < reader.AttributeCount; i++)
							{
								reader.MoveToAttribute(i);
								if (reader.Name.Equals("column"))
								{
									column = reader.Value;
								}

								else if (reader.Name.Equals("value"))
								{
									value = reader.Value;
								}
							}
							
							function.AddAssertion(new JsonObjectValueEqualsAssertion(column, value));
						}
						else if (reader.Name.Equals("jsonArraySizeLargerThan"))
						{
							int size = 0;
							
							for (int i = 0; i < reader.AttributeCount; i++)
							{
								reader.MoveToAttribute(i);
								if (reader.Name.Equals("size"))
								{
									size = Int32.Parse(reader.Value);
								}
							}
							function.AddAssertion(new JsonArraySizeLargerThanAssertion(size));
						}
						break;
					
					case XmlNodeType.EndElement:
						
						if (reader.Name.Equals("assertions"))
						{
							return;
						}

						break;
				}
			}
		}

		private void ReadProcessors(Function function)
		{
			while (reader.Read())
			{
				switch (reader.NodeType)
				{
					case XmlNodeType.Element:
						
						if (reader.Name.Equals("jsonArray"))
						{
							string row = null;
							string column = null;
							string varName = null;
							
							for (int i = 0; i < reader.AttributeCount; i++)
							{
								reader.MoveToAttribute(i);
								if (reader.Name.Equals("row"))
								{
									row = reader.Value;
								}


								else if (reader.Name.Equals("column"))
								{
									column = reader.Value;
								}
								else if (reader.Name.Equals("var"))
								{
									varName = reader.Value;
								}
							}
							
							function.AddProcessor(new JsonArrayProcessor(varName, row, column));
						}

						else if (reader.Name.Equals("jsonObject"))
						{
							string column = null;
							string varName = null;
							
							for (int i = 0; i < reader.AttributeCount; i++)
							{
								reader.MoveToAttribute(i);
								if (reader.Name.Equals("column"))
								{
									column = reader.Value;
								}


								else if (reader.Name.Equals("var"))
								{
									varName = reader.Value;
								}
							}
							
							function.AddProcessor(new JsonObjectProcessor(varName, column));
						}
						break;
					
					case XmlNodeType.EndElement:
						
						if (reader.Name.Equals("processors"))
						{
							return;
						}

						break;
				}
			}
		}

		private string GetDescription()
		{
			string desc = reader.ReadElementContentAsString();
			
			desc = desc.Replace("\n", "");
			desc = desc.Replace("\r", "");
			desc = desc.Replace("\t", "");
			desc = desc.Trim();
			
			return desc;
		}

		public void Close()
		{
			this.reader.Close();
			this.reader = null;
		}
	}
}
