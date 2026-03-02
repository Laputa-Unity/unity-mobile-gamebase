using System;
using System.Collections.Generic;
using System.IO;

namespace DldUtil
{
	public static class BigFileReader
	{
		public static bool FileHasText(string path, params string[] seekText)
		{
			if (!File.Exists(path))
			{
				return false;
			}

			FileStream fs = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
			BufferedStream bs = new BufferedStream(fs);
			StreamReader sr = new StreamReader(bs);

			while (true)
			{
				var line = sr.ReadLine();

				if (line == null)
				{
					break;
				}

				for (var seekTextIdx = 0; seekTextIdx < seekText.Length; ++seekTextIdx)
				{
					if (line.IndexOf(seekText[seekTextIdx], StringComparison.Ordinal) >= 0)
					{
						sr.Close();
						bs.Close();
						fs.Close();

						return true;
					}
				}
			}

			sr.Close();
			bs.Close();
			fs.Close();

			return false;
		}

		public static string SeekText(string path, params string[] seekText)
		{
			FileStream fs = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
			BufferedStream bs = new BufferedStream(fs);
			StreamReader sr = new StreamReader(bs);

			//long currentLine = 0;
			while (true)
			{
				//++currentLine;
				var line = sr.ReadLine();
				//Debug.LogFormat("seeking... line number {0}: {1}", currentLine, line);

				// reached end of file?
				if (line == null)
				{
					break;
				}

				for (var seekTextIdx = 0; seekTextIdx < seekText.Length; ++seekTextIdx)
				{
					if (line.IndexOf(seekText[seekTextIdx], StringComparison.Ordinal) >= 0)
					{
						return line;
					}
				}
			}

			return string.Empty;
		}

		public struct FoundText
		{
			public long LineNumber;
			public string Text;
		}

		public static string SeekFirstText(string path, params string[] seekText)
		{
			if (!File.Exists(path))
			{
				return null;
			}

			FileStream fs = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
			BufferedStream bs = new BufferedStream(fs);
			StreamReader sr = new StreamReader(bs);

			while (true)
			{
				var line = sr.ReadLine();
				//Debug.LogFormat("seeking... line number {0}: {1}", currentLine, line);

				// reached end of file?
				if (line == null)
				{
					break;
				}

				for (var seekTextIdx = 0; seekTextIdx < seekText.Length; ++seekTextIdx)
				{
					if (line.IndexOf(seekText[seekTextIdx], StringComparison.Ordinal) >= 0)
					{
						sr.Close();
						bs.Close();
						fs.Close();
						return line;
					}
				}
			}

			sr.Close();
			bs.Close();
			fs.Close();
			return null;
		}

		public static string SeekLastText(string path, params string[] seekText)
		{
			if (!File.Exists(path))
			{
				return null;
			}

			FileStream fs = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
			BufferedStream bs = new BufferedStream(fs);
			StreamReader sr = new StreamReader(bs);

			string returnValue = null;

			long currentLine = 0;
			while (true)
			{
				++currentLine;
				var line = sr.ReadLine();
				//Debug.LogFormat("seeking... line number {0}: {1}", currentLine, line);

				// reached end of file?
				if (line == null)
				{
					break;
				}

				for (var seekTextIdx = 0; seekTextIdx < seekText.Length; ++seekTextIdx)
				{
					if (line.IndexOf(seekText[seekTextIdx], StringComparison.Ordinal) >= 0)
					{
						returnValue = line;
						break;
					}
				}
			}

			sr.Close();
			bs.Close();
			fs.Close();
			return returnValue;
		}

		public static string SeekFirstTextEndingWith(string path, params string[] seekText)
		{
			if (!File.Exists(path))
			{
				return null;
			}

			FileStream fs = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
			BufferedStream bs = new BufferedStream(fs);
			StreamReader sr = new StreamReader(bs);

			while (true)
			{
				var line = sr.ReadLine();
				//Debug.LogFormat("seeking... line number {0}: {1}", currentLine, line);

				// reached end of file?
				if (line == null)
				{
					break;
				}

				for (var seekTextIdx = 0; seekTextIdx < seekText.Length; ++seekTextIdx)
				{
					if (line.EndsWith(seekText[seekTextIdx], StringComparison.Ordinal))
					{
						sr.Close();
						bs.Close();
						fs.Close();
						return line;
					}
				}
			}

			sr.Close();
			bs.Close();
			fs.Close();
			return null;
		}

		public static string SeekNextLineAfter(string path, string seekText)
		{
			if (!File.Exists(path))
			{
				return null;
			}

			FileStream fs = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
			BufferedStream bs = new BufferedStream(fs);
			StreamReader sr = new StreamReader(bs);

			bool foundLine = false;
			while (true)
			{
				var line = sr.ReadLine();
				//Debug.LogFormat("seeking... line number {0}: {1}", currentLine, line);

				// reached end of file?
				if (line == null)
				{
					break;
				}

				if (foundLine)
				{
					sr.Close();
					bs.Close();
					fs.Close();
					return line;
				}

				if (line.Contains(seekText))
				{
					foundLine = true;
				}
			}

			sr.Close();
			bs.Close();
			fs.Close();
			return null;
		}

		public static string SeekFirstTextWithPrefixSuffix(string path, string prefixSeekText, string suffixSeekText)
		{
			if (!File.Exists(path))
			{
				return null;
			}

			FileStream fs = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
			BufferedStream bs = new BufferedStream(fs);
			StreamReader sr = new StreamReader(bs);

			while (true)
			{
				var line = sr.ReadLine();
				//Debug.LogFormat("seeking... line number {0}: {1}", currentLine, line);

				// reached end of file?
				if (line == null)
				{
					break;
				}

				int prefixIdx = line.IndexOf(prefixSeekText, StringComparison.Ordinal);
				int suffixIdx = line.IndexOf(suffixSeekText, StringComparison.Ordinal);
				if (prefixIdx >= 0 && suffixIdx >= 0 && suffixIdx > prefixIdx)
				{
					sr.Close();
					bs.Close();
					fs.Close();
					return line;
				}
			}

			sr.Close();
			bs.Close();
			fs.Close();
			return null;
		}

		public static (long, long) GetLineNumberTextWithPrevLines(string path,
			string seekTextA, string prevLineSeekTextA, string prev2LineSeekTextA,
			string seekTextB, string prevLineSeekTextB, string prev2LineSeekTextB)
		{
			if (!File.Exists(path))
			{
				return (-1, -1);
			}

			long firstFoundLineA = -1;
			long lastFoundLineA = -1;
			long firstFoundLineB = -1;
			long lastFoundLineB = -1;

			FileStream fs = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
			BufferedStream bs = new BufferedStream(fs);
			StreamReader sr = new StreamReader(bs);

			long currentLine = 0;
			string prevLine = null;
			string prev2Line = null;

			bool encounteredBlankLine = false;

			while (true)
			{
				++currentLine;
				string line = sr.ReadLine();

				if (string.IsNullOrWhiteSpace(line))
				{
					encounteredBlankLine = true;
				}

				// reached end of file?
				if (line == null)
				{
					break;
				}

				if (line.IndexOf(seekTextA, StringComparison.Ordinal) >= 0 &&
				    prevLine != null && prevLine.IndexOf(prevLineSeekTextA, StringComparison.Ordinal) >= 0 &&
				    prev2Line != null && prev2Line.IndexOf(prev2LineSeekTextA, StringComparison.Ordinal) >= 0)
				{
					if (encounteredBlankLine)
					{
						firstFoundLineA = -1;
						encounteredBlankLine = false;
					}
					else if (firstFoundLineA == -1)
					{
						firstFoundLineA = lastFoundLineA;
					}
					lastFoundLineA = currentLine;
				}
				else if (line.IndexOf(seekTextB, StringComparison.Ordinal) >= 0 &&
				         prevLine != null && prevLine.IndexOf(prevLineSeekTextB, StringComparison.Ordinal) >= 0 &&
				         prev2Line != null && prev2Line.IndexOf(prev2LineSeekTextB, StringComparison.Ordinal) >= 0)
				{
					if (encounteredBlankLine)
					{
						firstFoundLineB = -1;
						encounteredBlankLine = false;
					}
					else if (firstFoundLineB == -1)
					{
						firstFoundLineB = lastFoundLineB;
					}
					lastFoundLineB = currentLine;
				}

				prev2Line = prevLine;
				prevLine = line;
			}

			sr.Close();
			bs.Close();
			fs.Close();
			return (firstFoundLineA == -1 ? lastFoundLineA : firstFoundLineA,
				firstFoundLineB == -1 ? lastFoundLineB : firstFoundLineB);
		}

		public static List<FoundText> SeekAllText(string path, params string[] seekText)
		{
			if (!File.Exists(path))
			{
				return null;
			}

			FileStream fs = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
			BufferedStream bs = new BufferedStream(fs);
			StreamReader sr = new StreamReader(bs);

			List<FoundText> returnValue = new List<FoundText>();

			long currentLine = 0;
			while (true)
			{
				++currentLine;
				var line = sr.ReadLine();
				//Debug.LogFormat("seeking... line number {0}: {1}", currentLine, line);

				// reached end of file?
				if (line == null)
				{
					break;
				}

				for (var seekTextIdx = 0; seekTextIdx < seekText.Length; ++seekTextIdx)
				{
					if (line.IndexOf(seekText[seekTextIdx], StringComparison.Ordinal) >= 0)
					{
						FoundText newFoundText;
						newFoundText.LineNumber = currentLine;
						newFoundText.Text = line;
						returnValue.Add(newFoundText);
						break;
					}
				}
			}

			sr.Close();
			bs.Close();
			fs.Close();
			return returnValue;
		}

		public static IEnumerable<string> ReadFile(string path, params string[] seekText)
		{
			return ReadFile(path, true, false, seekText);
		}

		public static IEnumerable<string> ReadFile(string path, bool startAfterSeekedText, params string[] seekText)
		{
			return ReadFile(path, startAfterSeekedText, false, seekText);
		}

		public static IEnumerable<string> ReadFile(string path, bool startAfterSeekedText, bool usePreviousIfFound, params string[] seekText)
		{
			if (!File.Exists(path))
			{
				yield break;
			}

			var fs = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
			var bs = new BufferedStream(fs);
			var sr = new StreamReader(bs);

			string line;

			bool seekTextRequested = (seekText != null) && (seekText.Length > 0) && !string.IsNullOrEmpty(seekText[0]);

			if (seekTextRequested)
			{
				long seekTextFoundAtLine = -1;
				long prevSeekTextFoundAtLine = -1;
				bool foundBlankLineSinceLastSeekTextFound = false;

				long currentLine = 0;
				while (true)
				{
					++currentLine;
					line = sr.ReadLine();
					//Debug.LogFormat("seeking... line number {0}: {1}", currentLine, line);
					if (string.IsNullOrWhiteSpace(line))
					{
						foundBlankLineSinceLastSeekTextFound = true;
					}

					// This loop just continues until we reach the last line.
					// That ensures that whatever seekTextFoundAtLine we have, it's the last one.
					if (line == null)
					{
						// reached end of file
						break;
					}

					var atLeastOneSeekTextFound = false;
					for (var seekTextIdx = 0; seekTextIdx < seekText.Length; ++seekTextIdx)
					{
						if (line.IndexOf(seekText[seekTextIdx], StringComparison.Ordinal) >= 0)
						{
							atLeastOneSeekTextFound = true;
							break;
						}
					}

					// if seekText not found yet, continue search
					if (!atLeastOneSeekTextFound)
					{
						continue;
					}

					if (!foundBlankLineSinceLastSeekTextFound)
					{
						prevSeekTextFoundAtLine = seekTextFoundAtLine;
					}
					else
					{
						prevSeekTextFoundAtLine = -1;
						foundBlankLineSinceLastSeekTextFound = false;
					}

					seekTextFoundAtLine = currentLine;

					//Debug.Log("seeking: " + line);
					//Debug.LogFormat("seekText found at line number {0}: {1}", currentLine, line);
				}
				//Debug.Log("done seeking");

				if (prevSeekTextFoundAtLine != -1 && usePreviousIfFound)
				{
					seekTextFoundAtLine = prevSeekTextFoundAtLine;
				}

				if (seekTextFoundAtLine != -1)
				{
					fs.Seek(0, SeekOrigin.Begin);

					currentLine = 0;
					while (true)
					{
						++currentLine;
						line = sr.ReadLine();

						if (line == null)
						{
							break;
						}

						if (startAfterSeekedText && currentLine <= seekTextFoundAtLine)
						{
							continue;
						}

						if (!startAfterSeekedText && currentLine < seekTextFoundAtLine)
						{
							continue;
						}

						//Debug.Log("seeked: " + line);

						yield return line;
					}
				}
			}
			else
			{
				while (true)
				{
					line = sr.ReadLine();

					if (line == null)
					{
						break;
					}

					yield return line;
				}
			}

			sr.Close();
			bs.Close();
			fs.Close();
		}

		public static IEnumerable<string> ReadFile(string path, long startAtLineNumber)
		{
			if (!File.Exists(path))
			{
				yield break;
			}

			var fs = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
			var bs = new BufferedStream(fs);
			var sr = new StreamReader(bs);

			long currentLine = 0;
			while (true)
			{
				string line = sr.ReadLine();
				++currentLine;

				if (line == null)
				{
					break;
				}

				if (currentLine >= startAtLineNumber)
				{
					yield return line;
				}
			}

			sr.Close();
			bs.Close();
			fs.Close();
		}

		public static IEnumerable<string> ReadFile(string path)
		{
			var fs = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
			var bs = new BufferedStream(fs);
			var sr = new StreamReader(bs);

			while (true)
			{
				var line = sr.ReadLine();

				if (line == null)
				{
					break;
				}

				yield return line;
			}

			sr.Close();
			bs.Close();
			fs.Close();
		}

		public static IEnumerable<FoundText> ReadFileWithLine(string path)
		{
			var fs = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
			var bs = new BufferedStream(fs);
			var sr = new StreamReader(bs);

			long currentLineNumber = 0;
			while (true)
			{
				++currentLineNumber;
				var line = sr.ReadLine();

				if (line == null)
				{
					break;
				}

				FoundText text;
				text.Text = line;
				text.LineNumber = currentLineNumber;
				yield return text;
			}

			sr.Close();
			bs.Close();
			fs.Close();
		}
	}
}