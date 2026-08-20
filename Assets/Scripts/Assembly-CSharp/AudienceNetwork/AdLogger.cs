namespace AudienceNetwork
{
	internal static class AdLogger
	{
		private enum AdLogLevel
		{
			None = 0,
			Notification = 1,
			Error = 2,
			Warning = 3,
			Log = 4,
			Debug = 5,
			Verbose = 6
		}

		private static AdLogLevel logLevel = AdLogLevel.Log;

		private static readonly string logPrefix = "Audience Network Unity ";

		internal static void Log(string message)
		{
			AdLogLevel adLogLevel = AdLogLevel.Log;
			if (logLevel >= adLogLevel)
			{
				DConsole.Log(logPrefix + LevelAsString(adLogLevel) + message);
			}
		}

		internal static void LogWarning(string message)
		{
			AdLogLevel adLogLevel = AdLogLevel.Warning;
			if (logLevel >= adLogLevel)
			{
				DConsole.LogWarning(logPrefix + LevelAsString(adLogLevel) + message);
			}
		}

		internal static void LogError(string message)
		{
			AdLogLevel adLogLevel = AdLogLevel.Error;
			if (logLevel >= adLogLevel)
			{
				DConsole.LogError(logPrefix + LevelAsString(adLogLevel) + message);
			}
		}

		private static string LevelAsString(AdLogLevel logLevel)
		{
			switch (logLevel)
			{
			case AdLogLevel.Error:
				return "<error>: ";
			case AdLogLevel.Warning:
				return "<warn>: ";
			case AdLogLevel.Log:
				return "<log>: ";
			case AdLogLevel.Debug:
				return "<debug>: ";
			case AdLogLevel.Verbose:
				return "<verbose>: ";
			default:
				return "";
			}
		}
	}
}
