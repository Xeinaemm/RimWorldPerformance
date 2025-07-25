﻿namespace Xeinaemm;

internal static class Log
{
	[Conditional("DEBUG")]
	internal static void Message(string x, [CallerMemberName] string member = "", [CallerFilePath] string file = "", [CallerLineNumber] int line = 0)
	{
		if (Settings.EnableDebugLogging)
			Verse.Log.Message(MessageFormat($"[DEBUG] {x}", member, file, line));
	}

	internal static void Warning(string x, [CallerMemberName] string member = "", [CallerFilePath] string file = "", [CallerLineNumber] int line = 0) =>
		Verse.Log.Warning(MessageFormat($"[WARNING] {x}", member, file, line));
	internal static void Error(string x, [CallerMemberName] string member = "", [CallerFilePath] string file = "", [CallerLineNumber] int line = 0) =>
		Verse.Log.Error(MessageFormat($"[ERROR] {x}", member, file, line));

	private static string MessageFormat(string message, string memberName, string sourceFilePath, int sourceLineNumber) =>
		$"[{DateTime.Now:HH:mm:ss}] [Xeinaemm] [{Path.GetFileNameWithoutExtension(sourceFilePath)}] [{memberName}:{sourceLineNumber}] {message}";
}