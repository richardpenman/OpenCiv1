using IRB.Collections.Generic;
using OpenCivOne.UI;
using System.Text;
using System.Text.RegularExpressions;

namespace OpenCivOne
{
	public class LanguageTools
	{
		private OpenCivOneGame parent;

		private BDictionary<string, List<string>> languagePackItems = new();

		public LanguageTools(OpenCivOneGame parent)
		{
			this.parent = parent;

			/*string[] textSections = {"BLURB0", "BLURB1", "BLURB2", "BLURB3", "BLURB4", "ERROR", "HELP", "KING", "PRODUCE"};

			for (int i = 0; i < textSections.Length; i++)
			{
				try
				{
					ParseSectionFile(textSections[i]);
				}
				catch { }
			}*/
		}

		public void ParseSectionFile(string section)
		{
			FileStream inputStream;
			StreamReader textReader;
			Regex rxKey = new Regex(@"^\s*\*([A-Z0-9' \.]+)\s*$");
			Match match;
			string? line;

			inputStream = new FileStream($"{this.parent.ResourcePath}{section}.TXT", FileMode.Open);
			inputStream.Seek(0x212, SeekOrigin.Begin);
			textReader = new StreamReader(inputStream, Encoding.ASCII);

			line = textReader.ReadLine();

			while (!textReader.EndOfStream)
			{
				List<string> contentItems = new List<string>();
				StringBuilder contentBuilder = new StringBuilder();
				List<string> keys = new List<string>();

				if (line != null && (match = rxKey.Match(line)).Success)
				{
					if (match.Groups[1].Value.Equals("END", StringComparison.CurrentCultureIgnoreCase))
						break;

					while (line != null && (match = rxKey.Match(line)).Success)
					{
						keys.Add(match.Groups[1].Value.Replace(' ', '_').Replace("'", "").Replace(".", "").ToUpper());

						line = textReader.ReadLine();
					}

					while (line != null && !(match = rxKey.Match(line)).Success)
					{
						line = line.Trim();

						if (line.Length > 0)
						{
							if (line.StartsWith('_') || line.StartsWith('-'))
							{
								contentItems.Add(contentBuilder.ToString().TrimEnd(' ', '^').Replace("  ", " "));
								contentBuilder.Clear();
							}

							if (line.EndsWith('^'))
							{
								contentBuilder.Append(line.TrimEnd(' ', '^'));
								contentBuilder.Append("^");
							}
							else
							{
								contentBuilder.Append(line);
								contentBuilder.Append(" ");
							}
						}

						line = textReader.ReadLine();
					}

					if (contentBuilder.Length > 0)
					{
						contentItems.Add(contentBuilder.ToString().TrimEnd(' ', '^').Replace("  ", " "));
					}

					for (int i = 0; i < keys.Count; i++)
					{
						languagePackItems.Add($"{section}_{keys[i]}", contentItems);
					}
				}
				else
				{
					line = textReader.ReadLine();
				}
			}

			textReader.Close();
		}

		/// <summary>
		/// Adjusts the given text block to specified width in characters
		/// </summary>
		/// <param name="maxLength">Maximum length of the one line of text in characters</param>
		public string F0_2f4d_0000_AdjustTextBlockWidth(string text, int maxLength)
		{
			//this.oCPU.Log.EnterBlock($"F0_2f4d_0000({length})");

			// function body
			int previousDelimiter = 0;
			int currentTextLength = 0;
			List<char> newText = new();

			for (int i = 0; i < text.Length; i++)
			{
				char ch = text[i];
				char nextCh = (i + 1 < text.Length) ? text[i + 1] : '\0';

				if (ch == ' ' || ch == '^' || ch == '\n')
				{
					if ((ch == '^' || ch == '\n') && (nextCh == ' ' || nextCh=='_'))
					{
						// the case where an option is presented
						newText.Add('\n');
						newText.Add(nextCh);
						currentTextLength = 1;
						previousDelimiter = 0;
						i++;
					}
					else if (newText.Count > 0 && currentTextLength > 0)
					{
						if (newText[newText.Count - 1] != ' ')
						{
							newText.Add(' ');
							currentTextLength++;
						}

						previousDelimiter = newText.Count - 1;
					}
					else
					{
						previousDelimiter = 0;
					}
				}
				else
				{
					newText.Add(ch);
					currentTextLength++;

					if (currentTextLength > maxLength && previousDelimiter > 0)
					{
						newText[previousDelimiter] = '\n';
						currentTextLength = i - previousDelimiter;
						previousDelimiter = 0;
					}
				}
			}

			if (currentTextLength > 0)
			{
				while (newText.Count > 0 && newText[newText.Count - 1] == ' ')
				{
					newText.RemoveAt(newText.Count - 1);
				}
				newText.Add('\n');
			}

			return new(newText.ToArray());
		}

		/// <summary>
		/// Trims (deletes) characters from the end of a string to a specified width in pixels
		/// </summary>
		/// <param name="text">The text to trim</param>
		/// <param name="maxWidth">Maximum width of text in pixels</param>
		public string F0_2f4d_04f7_TrimStringToWidth(string text, int maxWidth)
		{
			//this.oCPU.Log.EnterBlock($"F0_2f4d_04f7_TrimStringToWidth(0x{text:x4}, {maxWidth})");

			// function body
			while (text.Length > 1 && this.parent.DrawTools.GetStringWidth(text) > maxWidth)
			{
				if (text[text.Length - 2] != ' ')
				{
					text = text.Substring(0, text.Length - 2) + '.';
				}
				else
				{
					text = text.Substring(0, text.Length - 1);
				}
			}

			return text;
		}

		/// <summary>
		/// Gets the Language item from Language pack specified by its section and key
		/// </summary>
		/// <param name="section">The Language item section</param>
		/// <param name="topic">The topic</param>
		/// <returns>The string from the Language pack</returns>
		public string F0_2f4d_01ad_GetTextBySectionAndKey(string section, string topic)
		{
			//this.oCPU.Log.EnterBlock($"F0_2f4d_01ad(0x{filename:x4}, 0x{key:x4})");

			// function body
			// !!! This hashing thing doesn't function correctly
			/*string resourcePath = $"{this.oParent.ResourcePath}{section}.TXT";
			int hashValue = 0;

			for (int i = 0; i < key.Length; i++)
			{
				hashValue += (int)key[i] * i;
			}
			//hashValue &= 0xffff;

			int itemFilePosition;

			using (FileStream fileStream = new(resourcePath, FileMode.Open, FileAccess.Read))
			{
				fileStream.Seek((((hashValue >> 5) + hashValue) & 0xff) * 2, SeekOrigin.Begin);

				itemFilePosition = GameLoadAndSave.ReadUInt16(fileStream);
			}//*/

			int lineCount = 0;

			using (FileStream inputStream = new FileStream($"{this.parent.ResourcePath}{section.ToUpper()}.TXT", FileMode.Open))
			{
				inputStream.Seek(0x212, SeekOrigin.Begin);

				using (StreamReader textReader = new StreamReader(inputStream, Encoding.ASCII))
				{
					StringBuilder itemText = new StringBuilder();

					while (true)
					{
						string? line = textReader.ReadLine();

						// Instruction address 0x2f4d:0x0310, size: 5
						if (line != null && line.Equals(topic, StringComparison.CurrentCultureIgnoreCase))
						{
							line = textReader.ReadLine();

							while (line != null && line.StartsWith('*'))
							{
								line = textReader.ReadLine();
							}

							while (line != null && !line.StartsWith('*'))
							{
								line = line.Trim();

								if (line.Length > 0)
								{
									itemText.Append(line.TrimEnd());
									itemText.Append('\n');
								}

								line = textReader.ReadLine();

								lineCount++;
							}

							string item = itemText.ToString().TrimEnd(' ', '^', '\n');

							while (item.IndexOf("  ") >= 0)
							{
								item = item.Replace("  ", " ");
							}

							return item;
						}
						else if (line == null)
						{
							// The Language item is not found, report the error via MessageBox for now
							MessageBox.Show($"The language item from section '{section}', key '{topic}' is not found in the Language pack!", "Error", MessageBoxIcon.Error);
							return "";
						}
					}
				}
			}
		}

		/// <summary>
		/// Gets the Language item in king section from Language pack, replaces keywords, and adjusts the the width of the text to 80 characters
		/// </summary>
		/// <param name="topic">The topic</param>
		/// <returns>Adjusted Language item string</returns>
		public string F0_2f4d_044f_GetTextFromKingSection(string topic)
		{
			return F0_2f4d_044f_GetTextFromKingSection(topic, 80);
		}

		/// <summary>
		/// Gets the Language item in king section from Language pack, replaces keywords, and adjusts the the width of the text
		/// </summary>
		/// <param name="topic">The topic</param>
		/// <param name="maxWidth">Maximum width of the text in characters</param>
		/// <returns>Adjusted Language item string</returns>
		public string F0_2f4d_044f_GetTextFromKingSection(string topic, int maxWidth)
		{
			//this.oCPU.Log.EnterBlock($"F0_2f4d_044f(0x{stringPtr:x4})");

			// function body
			return F0_2f4d_0000_AdjustTextBlockWidth(F0_2f4d_0471_ReplaceKeywords(F0_2f4d_01ad_GetTextBySectionAndKey("KING", topic)), maxWidth);
		}

		private static readonly string[] Array_30ae = { "$US", "$THEM", "$BUCKS", "$RPLC1", "$RPLC2" };

		/// <summary>
		/// Replaces keywords in a string with their value
		/// </summary>
		public string F0_2f4d_0471_ReplaceKeywords(string text)
		{
			//this.oCPU.Log.EnterBlock("F0_2f4d_0471_ReplaceKeywords()");

			// function body
			for (int i = 0; i < Array_30ae.Length; i++)
			{
				text = text.Replace(Array_30ae[i], this.parent.Array_30b8[i]);
			}

			return text;
		}
	}
}
