using IRB.VirtualCPU;
using OpenCivOne.Graphics;

namespace OpenCivOne
{
	public class DrawTools
	{
		private OpenCivOneGame parent;
		private VCPU CPU;

		public DrawTools(OpenCivOneGame parent)
		{
			this.parent = parent;
			this.CPU = parent.CPU;
		}

		/// <summary>
		/// Calculates the string width
		/// </summary>
		/// <param name="text"></param>
		/// <returns></returns>
		public int GetStringWidth(string text)
		{
			int width = this.parent.Graphics.GetDrawStringSize(this.parent.Var_aa_Screen0_Rectangle.FontID, text).Width;

			this.CPU.AX.UInt16 = (ushort)((short)width);

			return width;
		}

		/// <summary>
		/// Draws a string at specified coordinates on screen selected by rectangle at 0xaa
		/// </summary>
		/// <param name="text"></param>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="frontColor"></param>
		public void F0_1182_002a_DrawString(string text, int x, int y, byte frontColor)
		{
			this.parent.Var_aa_Screen0_Rectangle.FrontColor = frontColor;

			if (this.CPU.ReadUInt16(this.CPU.DS.UInt16, 0x6b8c) != 0x0)
			{
				x *= 2;
			}

			// Instruction address 0x1182:0x0053, size: 5
			this.parent.Graphics.F0_VGA_11d7_DrawString(this.parent.Var_aa_Screen0_Rectangle, x, y, text);
		}

		/// <summary>
		/// Draws a string at specified coordinates on screen 0
		/// </summary>
		/// <param name="stringPtr"></param>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="frontColor"></param>
		public void F0_1182_005c_DrawStringToScreen0(ushort stringPtr, int x, int y, byte frontColor)
		{
			string text = this.CPU.ReadString(VCPU.ToLinearAddress(this.CPU.DS.UInt16, stringPtr));

			F0_1182_005c_DrawStringToScreen0(text, x, y, frontColor);
		}

		/// <summary>
		/// Draws a string at specified coordinates on screen 0
		/// </summary>
		/// <param name="text"></param>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="frontColor"></param>
		public void F0_1182_005c_DrawStringToScreen0(string text, int x, int y, byte frontColor)
		{
			this.parent.Var_aa_Screen0_Rectangle.Flags = 0;

			F0_1182_002a_DrawString(text, x, y, frontColor);

			this.parent.Var_aa_Screen0_Rectangle.Flags = 1;
		}

		/// <summary>
		/// Draws a string with shadow at specified coordinates on screen 0
		/// </summary>
		/// <param name="stringPtr"></param>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="frontColor"></param>
		public void F0_1182_0086_DrawStringWithShadow(ushort stringPtr, int x, int y, byte frontColor)
		{
			this.F0_1182_0086_DrawStringWithShadow(this.CPU.ReadString(VCPU.ToLinearAddress(this.CPU.DS.UInt16, stringPtr)), x, y, frontColor);
		}

		/// <summary>
		/// Draws a text with shadow at specified coordinates on screen 0
		/// </summary>
		/// <param name="text"></param>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="frontColor"></param>
		public void F0_1182_0086_DrawStringWithShadow(string text, int x, int y, byte frontColor)
		{
			F0_1182_005c_DrawStringToScreen0(text, x, y + 1, 0);
			F0_1182_005c_DrawStringToScreen0(text, x, y, frontColor);
		}

		/// <summary>
		/// Draws a string with shadow at specified coordinates on screen 0
		/// </summary>
		/// <param name="text"></param>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="frontColor"></param>
		public void F0_1182_0086_DrawStringWithShadowToScreen0(string text, int x, int y, byte frontColor)
		{
			F0_1182_005c_DrawStringToScreen0(text, x, y + 1, 0);
			F0_1182_005c_DrawStringToScreen0(text, x, y, frontColor);
		}

		/// <summary>
		/// Draws a centered string by rectangle defined at 0xaa on screen 0
		/// </summary>
		/// <param name="stringPtr"></param>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="frontColor"></param>
		public void F0_1182_00b3_DrawCenteredStringToScreen0(ushort stringPtr, int x, int y, byte frontColor)
		{
			string text = this.CPU.ReadString(VCPU.ToLinearAddress(this.CPU.DS.UInt16, stringPtr));

			F0_1182_00b3_DrawCenteredStringToScreen0(text, x, y, frontColor);
		}

		/// <summary>
		/// Draws a centered string by rectangle defined at 0xaa on screen 0
		/// </summary>
		/// <param name="text"></param>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="frontColor"></param>
		public void F0_1182_00b3_DrawCenteredStringToScreen0(string text, int x, int y, byte frontColor)
		{
			x -= GetStringWidth(text) / 2;

			this.parent.Var_aa_Screen0_Rectangle.Flags = 0;

			F0_1182_002a_DrawString(text, x, y, frontColor);

			this.parent.Var_aa_Screen0_Rectangle.Flags = 1;
		}

		/// <summary>
		/// Draws a centered string with shadow by rectangle defined at 0xaa on screen 0
		/// </summary>
		/// <param name="text"></param>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="frontColor"></param>
		public void F0_1182_00b3_DrawCenteredStringWithShadowToScreen0(string text, int x, int y, byte frontColor)
		{
			x -= GetStringWidth(text) / 2;

			this.parent.Var_aa_Screen0_Rectangle.Flags = 0;

			F0_1182_002a_DrawString(text, x + 1, y + 1, 0);
			F0_1182_002a_DrawString(text, x, y, frontColor);

			this.parent.Var_aa_Screen0_Rectangle.Flags = 1;
		}

		/// <summary>
		/// Draws rectangle filled with solid color and centered text message
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="x1"></param>
		/// <param name="y1"></param>
		/// <param name="stringPtr"></param>
		/// <param name="mode"></param>
		public void DrawFilledRectangleWithCenteredText(int x, int y, int x1, int y1, string text, ushort mode)
		{
			//this.CPU.Log.EnterBlock($"F0_1d12_71bf({xPos}, {yPos}, {xPos1}, {yPos1}, 0x{stringPtr:x4}, {mode})");

			// function body
			// Instruction address 0x1d12:0x71e8, size: 5
			this.parent.DrawTools.FillRectangle(this.parent.Var_aa_Screen0_Rectangle,
				x, y, x1 - x, y1 - y, mode);

			// Instruction address 0x1d12:0x720e, size: 5
			this.parent.Graphics.F0_VGA_0599_DrawLine(this.parent.Var_aa_Screen0_Rectangle,
				x, y, x1, y, (ushort)((mode < 8) ? (mode + 8) : 7));

			// Instruction address 0x1d12:0x7234, size: 5
			this.parent.Graphics.F0_VGA_0599_DrawLine(this.parent.Var_aa_Screen0_Rectangle,
				x, y, x, y1, (ushort)((mode < 8) ? (mode + 8) : 7));

			// Instruction address 0x1d12:0x725c, size: 5
			this.parent.Graphics.F0_VGA_0599_DrawLine(this.parent.Var_aa_Screen0_Rectangle,
				x + 1, y1, x1, y1, (ushort)((mode < 8) ? 8 : (mode - 8)));

			// Instruction address 0x1d12:0x7282, size: 5
			this.parent.Graphics.F0_VGA_0599_DrawLine(this.parent.Var_aa_Screen0_Rectangle,
				x1, y, x1, y1, (ushort)((mode < 0x8) ? 8 : (mode - 8)));

			// Instruction address 0x1d12:0x72ae, size: 5
			this.parent.DrawTools.F0_1182_00b3_DrawCenteredStringToScreen0(text,
				((x + x1) / 2) + 1, ((y + y1) / 2) - 2, (byte)(mode ^ 8));
		}

		/// <summary>
		/// Draws a text in a block of maximum width
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="maxWidth"></param>
		/// <param name="frontColor"></param>
		/// <returns>The new Y position</returns>
		public int DrawTextBlock(int x, int y, string text, int maxWidth, byte frontColor)
		{
			//this.oCPU.Log.EnterBlock($"F0_2f4d_0088_DrawTextBlock({maxWidth}, {x}, {y}, {frontColor})");

			// function body
			int newY = y;
			int lastSeparatorPosition = -1;
			int currentPosition = 0;

			// clean out text of unnecessary characters
			text = text.Replace('\r', ' ').Replace("^\n", "\r").Replace('\n', ' ').Replace('\r', '\n').Replace('^', '\n');

			while (text.IndexOf("  ") >= 0)
			{
				text = text.Replace("  ", " ");
			}

			// we want to adamantly end our text with a line separator
			if (!text.EndsWith('\n'))
			{
				text += '\n';
			}

			// process text block
			int textLength = text.Length;

			while (currentPosition < textLength && newY < 193)
			{
				int separatorPosition = text.IndexOfAny([' ', '\n'], lastSeparatorPosition + 1);

				if (separatorPosition >= 0)
				{
					char separatorChar = text[separatorPosition];
					string textPart = text.Substring(currentPosition, separatorPosition - currentPosition);
					GSize textPartSize = this.parent.Graphics.GetDrawStringSize(this.parent.Var_aa_Screen0_Rectangle.FontID, textPart);

					if (textPartSize.Width > maxWidth)
					{
						textPart = text.Substring(currentPosition, lastSeparatorPosition - currentPosition);

						// Instruction address 0x2f4d:0x013b, size: 5
						this.parent.DrawTools.F0_1182_005c_DrawStringToScreen0(textPart, x, newY, frontColor);

						currentPosition = lastSeparatorPosition + 1;
						lastSeparatorPosition = separatorPosition;
						newY += textPartSize.Height;
					}
					else if (separatorChar == '\n')
					{
						// Instruction address 0x2f4d:0x013b, size: 5
						this.parent.DrawTools.F0_1182_005c_DrawStringToScreen0(textPart, x, newY, frontColor);

						currentPosition = separatorPosition + 1;
						lastSeparatorPosition = separatorPosition;
						newY += textPartSize.Height;
					}
					else
					{
						lastSeparatorPosition = separatorPosition;
					}
				}
				else
				{
					string textPart = text.Substring(currentPosition, textLength - currentPosition).Trim(' ', '\n');
					GSize textPartSize = this.parent.Graphics.GetDrawStringSize(this.parent.Var_aa_Screen0_Rectangle.FontID, textPart);

					// Instruction address 0x2f4d:0x013b, size: 5
					this.parent.DrawTools.F0_1182_005c_DrawStringToScreen0(textPart, x, newY, frontColor);

					currentPosition = textLength;
					lastSeparatorPosition = textLength;
					newY += textPartSize.Height;
				}
			}

			return newY;
		}

		/// <summary>
		/// Calculates the height of the text block
		/// </summary>
		/// <param name="text"></param>
		/// <param name="y"></param>
		/// <param name="maxWidth"></param>
		/// <returns></returns>
		public int GetTextBlockHeight(string text, int y, int maxWidth)
		{
			int newY = y;
			int lastSeparatorPosition = -1;
			int currentPosition = 0;

			// clean out text of unnecessary characters
			text = text.Replace('\r', ' ').Replace("^\n", "\r").Replace('\n', ' ').Replace('\r', '\n').Replace('^', '\n');

			while (text.IndexOf("  ") >= 0)
			{
				text = text.Replace("  ", " ");
			}

			// we want to adamantly end our text with a line separator
			if (!text.EndsWith('\n'))
			{
				text += '\n';
			}

			// process text block
			int textLength = text.Length;

			while (currentPosition < textLength && newY < 193)
			{
				int separatorPosition = text.IndexOfAny([' ', '\n'], lastSeparatorPosition + 1);

				if (separatorPosition >= 0)
				{
					char separatorChar = text[separatorPosition];
					string textPart = text.Substring(currentPosition, separatorPosition - currentPosition);
					GSize textPartSize = this.parent.Graphics.GetDrawStringSize(this.parent.Var_aa_Screen0_Rectangle.FontID, textPart);

					if (textPartSize.Width > maxWidth)
					{
						textPart = text.Substring(currentPosition, lastSeparatorPosition - currentPosition);

						currentPosition = lastSeparatorPosition + 1;
						lastSeparatorPosition = separatorPosition;
						newY += textPartSize.Height;
					}
					else if (separatorChar == '\n')
					{
						currentPosition = separatorPosition + 1;
						lastSeparatorPosition = separatorPosition;
						newY += textPartSize.Height;
					}
					else
					{
						lastSeparatorPosition = separatorPosition;
					}
				}
				else
				{
					string textPart = text.Substring(currentPosition, textLength - currentPosition);
					GSize textPartSize = this.parent.Graphics.GetDrawStringSize(this.parent.Var_aa_Screen0_Rectangle.FontID, textPart);

					currentPosition = textLength;
					lastSeparatorPosition = textLength;
					newY += textPartSize.Height;
				}
			}

			return newY - y;
		}

		/// <summary>
		/// Draws a rectangle
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="width"></param>
		/// <param name="height"></param>
		/// <param name="mode"></param>
		public void DrawRectangle(int x, int y, int width, int height, ushort mode)
		{
			// function body
			// Instruction address 0x2d05:0x0a1d, size: 5
			this.parent.Graphics.F0_VGA_0599_DrawLine(this.parent.Var_aa_Screen0_Rectangle, x, y, x + width, y, mode);

			// Instruction address 0x2d05:0x0a34, size: 5
			this.parent.Graphics.F0_VGA_0599_DrawLine(this.parent.Var_aa_Screen0_Rectangle, x, y + height, x + width, y + height, mode);

			// Instruction address 0x2d05:0x0a45, size: 5
			this.parent.Graphics.F0_VGA_0599_DrawLine(this.parent.Var_aa_Screen0_Rectangle, x + width, y, x + width, y + height, mode);

			// Instruction address 0x2d05:0x0a5a, size: 5
			this.parent.Graphics.F0_VGA_0599_DrawLine(this.parent.Var_aa_Screen0_Rectangle, x, y, x, y + height, mode);
		}

		/// <summary>
		/// Draws a shadow rectangle
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="width"></param>
		/// <param name="height"></param>
		/// <param name="mode"></param>
		/// <param name="mode1"></param>
		public void DrawShadowRectangle(int x, int y, int width, int height, ushort mode, ushort mode1)
		{
			// function body
			// Instruction address 0x2d05:0x0a7e, size: 5
			this.parent.Graphics.F0_VGA_0599_DrawLine(this.parent.Var_aa_Screen0_Rectangle, x, y, x + width, y, mode1);

			// Instruction address 0x2d05:0x0a95, size: 5
			this.parent.Graphics.F0_VGA_0599_DrawLine(this.parent.Var_aa_Screen0_Rectangle, x, y + height, x + width, y + height, mode);

			// Instruction address 0x2d05:0x0aa6, size: 5
			this.parent.Graphics.F0_VGA_0599_DrawLine(this.parent.Var_aa_Screen0_Rectangle, x + width, y, x + width, y + height, mode1);

			// Instruction address 0x2d05:0x0abd, size: 5
			this.parent.Graphics.F0_VGA_0599_DrawLine(this.parent.Var_aa_Screen0_Rectangle, x, y + 1, x, y + height, mode);
		}

		/// <summary>
		/// Fills the rectangle with color
		/// </summary>
		/// <param name="rectPtr"></param>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="width"></param>
		/// <param name="height"></param>
		/// <param name="mode"></param>
		public void FillRectangle(CRectangle rect, int x, int y, int width, int height, ushort mode)
		{
			//this.oCPU.Log.EnterBlock($"F0_1000_0bfa_FillRectangle({rect}, {x}, {y}, {width}, {height}, 0x{mode:x4})");

			// function body
			if (width > 0 && height > 0)
			{
				this.parent.Graphics.F0_VGA_040a_FillRectangle(rect.ScreenID,
					new GRectangle(rect.Left + x, rect.Top + y, width, height), (byte)(mode & 0xff), (byte)((mode & 0xff00) >> 8));
			}
		}

		/// <summary>
		/// Fills a rectangle with default pattern
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="width"></param>
		/// <param name="height"></param>
		public void FillRectangleWithPattern(int x, int y, int width, int height)
		{
			FillRectangleWithPattern(x, y, width, height, 288, 120, 32, 16);
		}

		/// <summary>
		/// Fills a rectangle with custom pattern
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="width"></param>
		/// <param name="height"></param>
		public void FillRectangleWithPattern(int x, int y, int width, int height, int patternX, int patternY, int patternWidth, int patternHeight)
		{
			// function body
			int currentY = y;
			int currentHeight = height;

			while (currentHeight > 0)
			{
				int cellHeight = Math.Min(currentHeight, patternHeight);
				int currentX = x;
				int currentWidth = width;

				while (currentWidth > 0)
				{
					int cellWidth = Math.Min(currentWidth, patternWidth);

					// Instruction address 0x2dc4:0x0435, size: 5
					this.parent.Graphics.F0_VGA_07d8_DrawImage(this.parent.Var_19e8_Screen2_Rectangle,
						patternX, patternY, cellWidth, cellHeight, this.parent.Var_aa_Screen0_Rectangle, currentX, currentY);

					currentX += cellWidth;
					currentWidth -= cellWidth;
				}

				currentY += cellHeight;
				currentHeight -= cellHeight;
			}
		}

		/// <summary>
		/// Draws rectangle and fills it with default pattern
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="x1"></param>
		/// <param name="y1"></param>
		public void DrawRectangleAndFillWithPattern(int x, int y, int x1, int y1)
		{
			//this.CPU.Log.EnterBlock($"F0_1d12_70cb_FillRectangleWithPattern({xPos1}, {yPos1}, {xPos2}, {yPos2})");

			// function body			
			int iWidth = x1 - x;
			int iHeight = y1 - y;

			// Instruction address 0x1d12:0x70ee, size: 3
			FillRectangleWithPattern(x, y, iWidth, iHeight);

			// Instruction address 0x1d12:0x7104, size: 5
			DrawRectangle(x, y, iWidth, iHeight, 1);
		}

		/// <summary>
		/// Draws rectangle and fills it with custom pattern
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="x1"></param>
		/// <param name="y1"></param>
		public void DrawRectangleAndFillWithPattern(int x, int y, int x1, int y1, int patternX, int patternY, int patternWidth, int patternHeight)
		{
			//this.CPU.Log.EnterBlock($"F0_1d12_70cb_FillRectangleWithPattern({xPos1}, {yPos1}, {xPos2}, {yPos2})");

			// function body			
			int iWidth = x1 - x;
			int iHeight = y1 - y;

			// Instruction address 0x1d12:0x70ee, size: 3
			FillRectangleWithPattern(x, y, iWidth, iHeight, patternX, patternY, patternWidth, patternHeight);

			// Instruction address 0x1d12:0x7104, size: 5
			DrawRectangle(x, y, iWidth, iHeight, 1);
		}

		/// <summary>
		/// Fills the rectangle and draws a double shadow around the rectangle
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="width"></param>
		/// <param name="height"></param>
		/// <param name="mode"></param>
		public void FillRectangleWithDoubleShadow(int x, int y, int width, int height, ushort mode)
		{
			// function body
			if (mode == 7 && this.parent.Var_2f98_PatternAvailable)
			{
				// Instruction address 0x2d05:0x098c, size: 5
				this.parent.DrawTools.FillRectangleWithPattern(x, y, width, height);
			}
			else
			{
				// Instruction address 0x2d05:0x09b1, size: 5
				this.parent.DrawTools.FillRectangle(this.parent.Var_aa_Screen0_Rectangle, x, y, width, height, mode);
			}

			this.parent.Var_aa_Screen0_Rectangle.BackColor = (byte)mode;

			// Instruction address 0x2d05:0x09e1, size: 3
			DrawShadowRectangle(x - 1, y - 1, width + 1, height + 1, 15, 8);

			// Instruction address 0x2d05:0x09fd, size: 3
			DrawRectangle(x - 2, y - 2, width + 3, height + 3, 0);
		}
	}
}
