using System;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using System.Reflection;

namespace WebMVCTest.XML
{
	public class ProjectValidator
	{
		public ProjectValidator ()
		{
		}

		public bool IsValid (string xmlFile)
		{
			XmlReaderSettings settings = new XmlReaderSettings ();
			settings.ValidationType = ValidationType.Schema;
			XmlSchemaSet schemas = new XmlSchemaSet ();
			settings.Schemas = schemas;
			
			StreamReader schemaStream = new StreamReader (Assembly.GetExecutingAssembly ().GetManifestResourceStream ("WebMVCTest.XML.WebMVCTest-0.1.xsd"));
			
			XmlSchema schema = XmlSchema.Read (schemaStream, new ValidationEventHandler (ValidationError));
			
			schemas.Add (schema);
			
			settings.ValidationEventHandler += new ValidationEventHandler (ValidationError);
			
			XmlReader validator = XmlReader.Create (xmlFile, settings);
			while (validator.Read ())
				;
			
			return true;
		}

		private void ValidationError (object sender, ValidationEventArgs arguments)
		{
			throw arguments.Exception;
		}
	}
}

