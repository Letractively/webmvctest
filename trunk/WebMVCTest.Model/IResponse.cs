using System;
namespace WebMVCTest.Model
{
	public interface IResponse
	{
		string GetResponseText();

		string GetStatusDescription();

		int GetStatusCode();

        TimeSpan? GetExecutionTime();

        bool IsTimedOut();
	}
}

