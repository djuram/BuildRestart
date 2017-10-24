using System;

namespace BuildRestart.Log
{
	interface IExtensionLogger
	{
		void WriteStatus(string message);

		void WriteException(Exception exception);
	}
}
