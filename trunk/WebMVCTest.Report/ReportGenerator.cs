using System;
using WebMVCTest.Model;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;
using System.Text;
using WebMVCTest.Engine;

namespace WebMVCTest.Report
{
	public class ReportGenerator
	{
		private static ReportGenerator current = new ReportGenerator();

		private Settings settings;

		public static ReportGenerator GetInstance()
		{
			return current;
		}

		public void CreatePDF(Project project)
		{
			Document document = new Document(PageSize.A4);
			PdfWriter.GetInstance(document, new FileStream(project.GetSaveName() + ".pdf", FileMode.Create));
			
			document.Open();
			document.Add(CreateProject(project));
			
			foreach (TestSet testSet in project.TestSets)
			{
				if (testSet.Executed)
				{
					document.Add(CreateTestSet(testSet));
				}
			}
			
			document.Close();
		}

		private int GetDefaultFontSize()
		{
			return 8;
		}

		private Font GetNormalFont()
		{
			return FontFactory.GetFont(FontFactory.HELVETICA, GetDefaultFontSize(), Font.NORMAL);
		}

		private Font GetHeaderFont()
		{
			return FontFactory.GetFont(FontFactory.HELVETICA, GetDefaultFontSize(), Font.BOLD);
		}

		private void StyleDefaultCell(PdfPCell cell)
		{
		}

		private void AddHeaderCell(PdfPTable table, string text)
		{
			table.AddCell(new Phrase(text, GetHeaderFont()));
		}

		private void AddNormalCell(PdfPTable table, string text)
		{
			table.AddCell(new Phrase(text, GetNormalFont()));
		}

		private PdfPTable CreateProject(Project project)
		{
			PdfPTable table = new PdfPTable(new float[] { 2, 8 });
			StyleDefaultCell(table.DefaultCell);
			table.SpacingAfter = 20;
			table.WidthPercentage = 100f;
			AddHeaderCell(table, "Project");
			AddNormalCell(table, project.Name);
			AddHeaderCell(table, "Base URL");
			AddNormalCell(table, project.GetBaseUrl());
			AddHeaderCell(table, "Description");
			AddNormalCell(table, project.Description);
			
			return table;
		}

		private PdfPTable CreateTestSet(TestSet testSet)
		{
			PdfPTable table = new PdfPTable(new float[] { 2, 8 });
			StyleDefaultCell(table.DefaultCell);
			table.SpacingAfter = 5;
			table.WidthPercentage = 100f;
			AddHeaderCell(table, "TestSet");
			AddNormalCell(table, testSet.Name);
			
			if (!string.IsNullOrEmpty(testSet.Description))
			{
				AddHeaderCell(table, "Description");
				AddNormalCell(table, testSet.Description);
			}
			
			int success = 0;
			int total = 0;
			
			foreach (Function function in testSet.Functions)
			{
				if (!string.IsNullOrEmpty(function.Name))
				{
					AddHeaderCell(table, "Name");
					AddNormalCell(table, function.Name);
				}
				if (!string.IsNullOrEmpty(function.Description))
				{
					AddHeaderCell(table, "Description");
					AddNormalCell(table, function.Description);
				}
				
				AddHeaderCell(table, "Call");
				AddNormalCell(table, function.Method + " " + function.GetResult().ExecutedUrl);
				
				AddHeaderCell(table, "Result");
				
				if (function.GetResult().Executed)
				{
					total++;

					StringBuilder text = new StringBuilder();
					
					if (function.GetResult().Success)
					{
						text.Append("SUCCESS");
					}
					else
					{
						text.Append("FAILED (");
						text.Append(function.GetResult().StatusCode);
						text.Append(" ");
						text.Append(function.GetResult().StatusDescription);
						text.Append(")");
					}

					Phrase phrase = new Phrase(text.ToString(), GetNormalFont());
					PdfPCell cell = new PdfPCell(phrase);

					if (function.GetResult().Success)
					{
						cell.BackgroundColor = new BaseColor(0, 255, 0);
					}
					else
					{
						cell.BackgroundColor = new BaseColor(255, 0, 0);
					}
					table.AddCell(cell);
					
					// count the successes
					if (function.GetResult().Success)
					{
						success++;
					}
					else
					{
						if (this.settings.ShowDebug)
						{
							AddHeaderCell(table, "Debuging output");
							AddNormalCell(table, function.GetResult().ResponseText);
						}
					}
				}
				else
				{
					Phrase phrase = new Phrase("Skipped", GetNormalFont());
					PdfPCell cell = new PdfPCell(phrase);
					cell.BackgroundColor = new BaseColor(150, 150, 150);
					table.AddCell(cell);
				}
				
				AddHeaderCell(table, "Execution time");
				AddNormalCell(table, function.GetResult().ExecutionTime.ToString());
			}
			
			// add summary
			decimal percentage = (((decimal)success / (decimal)total) * 100);
			AddHeaderCell(table, "Total score");
			AddNormalCell(table, string.Format("{1} total, {0} successful ({2:0.00}%)", success, total, percentage));
			
			return table;
		}

		public void SetSettings(Settings settings)
		{
			this.settings = settings;
		}
	}
}

