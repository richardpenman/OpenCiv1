using IRB.VirtualCPU;
using OpenCivOne.Graphics;
using System.Diagnostics;

namespace OpenCivOne
{
	public class MenuBoxDialog
	{
		private OpenCivOneGame parent;
		private VCPU CPU;

		private string[] Array_2fa6 = { "Spies report:", "Diplomats report:", "Travelers report:", "Defense Minister reports:", "Domestic Advisor reports:", "Foreign Minister reports:", "Science Advisor reports:" };

		public MenuBoxDialog(OpenCivOneGame parent)
		{
			this.parent = parent;
			this.CPU = parent.CPU;
		}

		public int F0_2d05_0031_ShowMenuBox(ushort stringPtr, int x, int y, bool windowFrame, bool helpOption, bool emptyKeyboardAndMouse)
		{
			return F0_2d05_0031_ShowMenuBox(this.CPU.ReadString(this.CPU.DS.UInt16, stringPtr), x, y, windowFrame, helpOption, emptyKeyboardAndMouse);
		}

		/// <summary>
		/// Shows customized MenuBox
		/// </summary>
		/// <param name="stringPtr"></param>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="windowFrame"></param>
		/// <param name="helpOption"></param>
		/// <param name="emptyKeyboardAndMouse"></param>
		/// <returns></returns>
		public int F0_2d05_0031_ShowMenuBox(string menuString, int x, int y, bool windowFrame, bool helpOption, bool emptyKeyboardAndMouse)
		{
			//this.oCPU.Log.EnterBlock($"F0_2d05_0031_ShowMenuBox(0x{stringPtr:x4}, {x}, {y}, {emptyKeyboardAndMouse})");

			if (windowFrame && this.parent.Var_2f9e_MessageBoxStyle != MenuBoxReportTypeEnum.None)
			{
				// Instruction address 0x2d05:0x0621, size: 5
				menuString = $"{Array_2fa6[(int)this.parent.Var_2f9e_MessageBoxStyle]}\n{menuString}";
			}

			// We want to remove empty entries, because they shouldn't be in the list
			string[] menuItems = menuString.TrimEnd().Split("\n" , StringSplitOptions.RemoveEmptyEntries);

			#region Print debug data
			/*Debug.WriteLine($"\"{menuString}\"");

			Debug.Write("{");
			for (int i = 0; i < menuItems.Length; i++)
			{
				if (i > 0)
					Debug.Write(", ");

				Debug.Write($"\"{menuItems[i]}\"");
			}
			Debug.WriteLine("}");//*/
			#endregion

			this.parent.Var_2fa2_DialogMousePressed = false;
			this.parent.Var_2f9c_MenuBoxHelpRequested = false;

			// Determine maximum line width and height
			int lineHeight = this.parent.Graphics.F0_VGA_11ae_GetTextHeight(this.parent.Var_aa_Screen0_Rectangle.FontID);

			if (lineHeight == 9)
			{
				lineHeight--;
			}

			if (this.parent.Var_3936 != -1)
			{
				lineHeight = 8;
			}

			List<int> optionIndexes = new();
			List<char> optionChars = new();
			int selectedOptionIndex = 0;
			int maxContentWidth = this.parent.ScreenSize.Width - (windowFrame ? 8 : 0) - x - ((windowFrame && this.parent.Var_2f9e_MessageBoxStyle != MenuBoxReportTypeEnum.None) ? 44 : 0);
			int maxContentHeight = this.parent.ScreenSize.Height - (windowFrame ? 8 : 0) - y;
			int maxLineWidth = 0;
			int maxLineCount = (maxContentHeight - (helpOption && windowFrame ? 8 : 0)) / lineHeight;

			// limit the number of available lines to window height
			if (menuItems.Length > maxLineCount)
			{
				Array.Resize(ref menuItems, maxLineCount);
			}

			// Detect default checked char, it can be a simple space or a bullet, but not checkmark '^'
			char defaultCheckedChar = '\x0';

			for (int i = 0; i < menuItems.Length; i++)
			{
				string menuItem = menuItems[i];

				if (defaultCheckedChar == '\x0' && (menuItem.StartsWith(' ') || menuItem.StartsWith('_')))
				{
					defaultCheckedChar = menuItem[0];
					break;
				}
			}

			for (int i = 0; i < menuItems.Length; i++)
			{
				string menuItem = menuItems[i];
				int itemWidth = this.parent.Graphics.GetDrawStringSize(this.parent.Var_aa_Screen0_Rectangle.FontID, menuItem).Width;

				if (optionIndexes.Count > 0 && !menuItem.StartsWith(defaultCheckedChar) && menuItem.StartsWith(' '))
				{
					menuItem = defaultCheckedChar + menuItem;

					// treat this as exception for now
					throw new Exception($"The menu items are not properly formed, the offending menu text: '{menuString}'");
				}

				// Trim the item to the available content size
				while (itemWidth > maxContentWidth)
				{
					menuItem = menuItem.Substring(0, menuItem.Length - 1);
					itemWidth = this.parent.Graphics.GetDrawStringSize(this.parent.Var_aa_Screen0_Rectangle.FontID, menuItem + "...").Width;

					if (itemWidth <= maxContentWidth)
					{
						menuItem += "...";
						menuItems[i] = menuItem;
					}
				}

				if (itemWidth > maxLineWidth)
				{
					maxLineWidth = itemWidth;
				}

				// detect options and select check marks if necessary
				if (menuItem.StartsWith(defaultCheckedChar))
				{
					optionIndexes.Add(i);

					// test if this option is checked
					if ((this.parent.Var_d7f2_MenuBoxCheckedOptions & (0x1 << optionIndexes.Count - 1)) != 0)
					{
						// Current default is to pass space as first character and then amend it to a check mark (if selected)
						menuItems[i] = '^' + menuItem.Substring(1);
					}

					// These are the shortcut characters which can be selected with the keyboard
					// !!! To do: What if the shortcut characters have a duplicate?
					if (menuItem.Length > 1)
					{
						optionChars.Add(char.ToLower(menuItem[1]));
					}
				}
			}

			int contentLeft = x + (windowFrame ? 4 : 0) + ((windowFrame && this.parent.Var_2f9e_MessageBoxStyle != MenuBoxReportTypeEnum.None) ? 44 : 0);
			int contentTop = y + (windowFrame ? 4 : 0);
			int contentWidth = maxLineWidth;
			int contentHeight = Math.Max((menuItems.Length * lineHeight) + (helpOption && windowFrame ? 8 : 0), ((windowFrame && this.parent.Var_2f9e_MessageBoxStyle != MenuBoxReportTypeEnum.None) ? 60 : 0));

			int windowWidth = contentWidth + (windowFrame ? 8 : 0) + ((windowFrame && this.parent.Var_2f9e_MessageBoxStyle != MenuBoxReportTypeEnum.None) ? 44 : 0);
			int windowHeight = contentHeight + (windowFrame ? 8 : 0);

			// Adjust default selected option (if selected)
			if (this.parent.Var_2f9a_MenuBoxDefaultOptionIndex != -1)
			{
				selectedOptionIndex = Math.Min(optionIndexes.Count - 1, this.parent.Var_2f9a_MenuBoxDefaultOptionIndex);
			}

			// Determine colors
			int textColor;
			int backgroundColor = 7;
			int highlightColor = 22;

			if (y == 139)
			{
				highlightColor = 8;
			}

			if (!windowFrame && y != 139)
			{
				// in this case we want for a background color to be as color on the screen
				backgroundColor = this.parent.Graphics.F0_VGA_038c_GetPixel(0, x, y);
				highlightColor = -1;
			}

			textColor = (backgroundColor == 15) ? 3 : 15;

			if (windowFrame)
			{
				this.parent.DrawTools.FillRectangleWithDoubleShadow(x, y, windowWidth, windowHeight, 7);

				if (this.parent.Var_2f9e_MessageBoxStyle != MenuBoxReportTypeEnum.None)
				{
					// This is dialog image being drawn

					if (this.parent.Var_2f9e_MessageBoxStyle == MenuBoxReportTypeEnum.DefenseMinisterReport ||
						this.parent.Var_2f9e_MessageBoxStyle == MenuBoxReportTypeEnum.DomesticAdvisorReport ||
						this.parent.Var_2f9e_MessageBoxStyle == MenuBoxReportTypeEnum.ForeignMinisterReport ||
						this.parent.Var_2f9e_MessageBoxStyle == MenuBoxReportTypeEnum.ScienceAdvisorReport)
					{
						this.parent.Graphics.F0_VGA_07d8_DrawImage(this.parent.Var_19e8_Screen2_Rectangle,
							40 + (40 * (int)this.parent.Var_2f9e_MessageBoxStyle), 140, 40, 60,
							this.parent.Var_aa_Screen0_Rectangle, contentLeft - 44, contentTop);
					}
					else
					{
						// Instruction address 0x2d05:0x06c8, size: 5
						this.parent.Graphics.F0_VGA_0c3e_DrawBitmapToScreen(this.parent.Var_aa_Screen0_Rectangle, 
							contentLeft - 44, contentTop, this.parent.Array_df62[(int)this.parent.Var_2f9e_MessageBoxStyle]);
					}
				}
			}

			if (windowFrame && helpOption)
			{
				int fontID = this.parent.Var_aa_Screen0_Rectangle.FontID;
				this.parent.Var_aa_Screen0_Rectangle.FontID = 2;

				string helpText = "(HELP AVAILABLE)";
				GSize helpSize = this.parent.Graphics.GetDrawStringSize(this.parent.Var_aa_Screen0_Rectangle.FontID, helpText);

				// Instruction address 0x2d05:0x0787, size: 5
				this.parent.DrawTools.F0_1182_005c_DrawStringToScreen0(helpText, contentLeft + contentWidth - helpSize.Width, contentTop + contentHeight - helpSize.Height, 10);

				this.parent.Var_aa_Screen0_Rectangle.FontID = fontID;
			}

			this.parent.Var_aa_Screen0_Rectangle.FrontColor = (byte)textColor;

			int optionIndex = -1;

			for (int i = 0; i < menuItems.Length; i++)
			{
				string menuItem = menuItems[i];

				if (optionIndexes.Count > 0)
				{
					if (optionIndex == -1 && i == optionIndexes[0])
					{
						optionIndex = 0;
					}
					else if (optionIndex != -1)
					{
						if (optionIndex < optionIndexes.Count)
						{
							optionIndex++;
						}
						else
						{
							throw new Exception("Run out of options. This should never happen.");
						}
					}
				}

				if (optionIndex == -1)
				{
					if (lineHeight > 9)
					{
						// Instruction address 0x2d05:0x08c6, size: 5
						this.parent.DrawTools.F0_1182_0086_DrawStringWithShadowToScreen0(menuItem, contentLeft, contentTop + (i * lineHeight), (byte)textColor);
					}
					else
					{
						// Instruction address 0x2d05:0x08c6, size: 5
						this.parent.DrawTools.F0_1182_005c_DrawStringToScreen0(menuItem, contentLeft, contentTop + (i * lineHeight), (byte)textColor);
					}
				}
				else
				{
					if ((this.parent.Var_b276_MenuBoxDisabledOptions & (0x1 << optionIndex)) != 0)
					{
						// Instruction address 0x2d05:0x0834, size: 5
						this.parent.DrawTools.F0_1182_005c_DrawStringToScreen0(menuItem, contentLeft, contentTop + (i * lineHeight), 4);
					}
					else
					{
						// Instruction address 0x2d05:0x0834, size: 5
						this.parent.DrawTools.F0_1182_005c_DrawStringToScreen0(menuItem, contentLeft, contentTop + (i * lineHeight), 0);
					}
				}
			}

			if (emptyKeyboardAndMouse)
			{
				// Instruction address 0x2d05:0x003e, size: 5
				this.parent.CommonTools.EmptyKeyboardBufferAndClearMouse();
			}

			if (this.parent.Var_db38 == 0)
			{
				MouseEvent mouseEvent = this.parent.GetMouseEvent();

				int oldMouseX = mouseEvent.Position.X;
				int oldMouseY = mouseEvent.Position.Y;
				int oldSelectedOptionIndex = -1;
				bool mouseActive = false;
				bool exitLoop = false;

				while (true)
				{
					if (this.parent.Var_3936 != -1)
					{
						this.parent.MeetWithKing.F6_0000_1b33();
					}

					//this.parent.Var_db3a_MouseButton = 0;

					mouseEvent = this.parent.GetMouseEvent();

					if (mouseEvent.Position.X >= contentLeft && mouseEvent.Position.X <= contentLeft + contentWidth &&
						mouseEvent.Position.Y >= contentTop && mouseEvent.Position.Y <= contentTop + contentHeight)
					{
						if (mouseEvent.Position.X != oldMouseX && mouseEvent.Position.Y != oldMouseY)
						{
							int selectedLine = ((mouseEvent.Position.Y - contentTop) / lineHeight);

							for (int i = 0; i < optionIndexes.Count; i++)
							{
								if (selectedLine == optionIndexes[i])
								{
									selectedOptionIndex = i;
									mouseActive = true;
									break;
								}
							}

							oldMouseX = mouseEvent.Position.X;
							oldMouseY = mouseEvent.Position.Y;
						}
					}
					else if (mouseActive && mouseEvent.Buttons != MouseButtonsEnum.None)
					{
						selectedOptionIndex = -1;
						exitLoop = true;
					}

					if (!mouseActive && mouseEvent.Buttons == MouseButtonsEnum.None)
					{
						mouseActive = true;
					}
					else if (mouseActive)
					{
						if (mouseEvent.Buttons == MouseButtonsEnum.Left)
						{
							this.parent.Var_2fa2_DialogMousePressed = true;

							if (selectedOptionIndex == oldSelectedOptionIndex)
							{
								exitLoop = true;
							}
						}
						else if (mouseEvent.Buttons == MouseButtonsEnum.Right)
						{
							if (helpOption && selectedOptionIndex == oldSelectedOptionIndex)
							{
								this.parent.Var_2f9c_MenuBoxHelpRequested = true;
								exitLoop = true;
							}
						}
					}

					// Instruction address 0x2d05:0x01db, size: 5
					if (this.parent.CAPI.kbhit() != 0)
					{
						// Instruction address 0x2d05:0x01e5, size: 3
						int pressedKey = F0_2d05_0ac9_GetNavigationKey();

						switch (pressedKey)
						{
							case 0xd:
							case 0x20:
								if ((this.parent.Var_b276_MenuBoxDisabledOptions & (0x1 << selectedOptionIndex)) == 0)
								{
									exitLoop = true;
								}
								break;

							case 0x1b:
								selectedOptionIndex = -1;
								exitLoop = true;
								break;

							case 0x2300:
								this.parent.Var_2f9c_MenuBoxHelpRequested = true;
								exitLoop = true;
								break;

							case 0x2f00:
								this.parent.GameData.GameSettingFlags.Sound ^= true;

								if (!this.parent.GameData.GameSettingFlags.Sound)
								{
									// Instruction address 0x2d05:0x03ad, size: 5
									this.parent.CommonTools.PlayTune(1, 0);
								}
								break;

							case 0x4800:
								if (selectedOptionIndex == -1 && optionIndexes.Count > 0)
								{
									selectedOptionIndex = 0;
								}
								else if (selectedOptionIndex > 0)
								{
									selectedOptionIndex--;
								}
								break;

							case 0x5000:
								if (selectedOptionIndex == -1 && optionIndexes.Count > 0)
								{
									selectedOptionIndex = 0;
								}
								else if (selectedOptionIndex < optionIndexes.Count - 1)
								{
									selectedOptionIndex++;
								}
								break;

							default:
								char ch = char.ToLower((char)pressedKey);

								for (int i = 0; i < optionChars.Count; i++)
								{
									int index = (selectedOptionIndex + 1 + i) % optionIndexes.Count;

									if (optionChars[index] != '\0' && optionChars[index] == ch)
									{
										selectedOptionIndex = index;
										break;
									}
								}
								break;
						}
					}

					if (selectedOptionIndex < 0 || selectedOptionIndex >= optionIndexes.Count)
					{
						selectedOptionIndex = -1;
					}

					if (selectedOptionIndex != oldSelectedOptionIndex)
					{
						if (oldSelectedOptionIndex != -1)
						{
							int oldLineIndex = optionIndexes[oldSelectedOptionIndex];

							// Instruction address 0x2d05:0x027c, size: 5
							this.parent.Graphics.F0_VGA_009a_ReplaceColor(this.parent.Var_aa_Screen0_Rectangle,
								contentLeft, contentTop + (oldLineIndex * lineHeight) - 1, maxLineWidth, lineHeight, 11, (byte)backgroundColor);

							if (highlightColor != -1)
							{
								// Instruction address 0x2d05:0x02b2, size: 5
								this.parent.Graphics.F0_VGA_009a_ReplaceColor(this.parent.Var_aa_Screen0_Rectangle,
									contentLeft, contentTop + (oldLineIndex * lineHeight) - 1, maxLineWidth, lineHeight, 3, (byte)highlightColor);
							}
						}

						if (selectedOptionIndex != -1)
						{
							int lineIndex = optionIndexes[selectedOptionIndex];

							// Instruction address 0x2d05:0x02ee, size: 5
							this.parent.Graphics.F0_VGA_009a_ReplaceColor(this.parent.Var_aa_Screen0_Rectangle,
								contentLeft, contentTop + (lineIndex * lineHeight) - 1, maxLineWidth, lineHeight, (byte)backgroundColor, 11);

							if (highlightColor != -1)
							{
								// Instruction address 0x2d05:0x0324, size: 5
								this.parent.Graphics.F0_VGA_009a_ReplaceColor(this.parent.Var_aa_Screen0_Rectangle,
									contentLeft, contentTop + (lineIndex * lineHeight) - 1, maxLineWidth, lineHeight, (byte)highlightColor, 3);
							}
						}

						oldSelectedOptionIndex = selectedOptionIndex;
					}

					if (exitLoop)
					{
						if (selectedOptionIndex != -1)
						{
							// Instruction address 0x2d05:0x0441, size: 5
							this.parent.CommonTools.WaitTimer(20);
						}
						else
						{
							this.parent.Var_2f9c_MenuBoxHelpRequested = false;
						}

						break;
					}
				}

				this.parent.Var_2f9e_MessageBoxStyle = MenuBoxReportTypeEnum.None;
				this.parent.Var_2f9a_MenuBoxDefaultOptionIndex = -1;
				this.parent.Var_b276_MenuBoxDisabledOptions = 0;
				this.parent.Var_d7f2_MenuBoxCheckedOptions = 0;

				return selectedOptionIndex;
			}

			this.parent.Var_db38 = 0;

			return -1;
		}

		/// <summary>
		/// Get navigation key
		/// </summary>
		/// <returns></returns>
		public int F0_2d05_0ac9_GetNavigationKey()
		{
			//this.oCPU.Log.EnterBlock("F0_2d05_0ac9_GetNavigationKey()");

			// function body
			// Instruction address 0x2d05:0x0acf, size: 5
			return this.parent.CAPI.getch();
		}
	}
}
