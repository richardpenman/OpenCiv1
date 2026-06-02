using IRB.VirtualCPU;
using OpenCiv1.Graphics;

namespace OpenCiv1
{
	public class DrawTools
	{
		private OpenCiv1Game parent;
		private VCPU CPU;

		public DrawTools(OpenCiv1Game parent)
		{
			this.parent = parent;
			this.CPU = parent.CPU;
		}

		/// <summary>
		/// Draws a string at specified coordinates on screen selected by rectangle at 0xaa
		/// </summary>
		/// <param name="stringPtr"></param>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="frontColor"></param>
		public void F0_1182_002a_DrawString(ushort stringPtr, int x, int y, byte frontColor)
		{
			string text = this.CPU.ReadString(VCPU.ToLinearAddress(this.CPU.DS.UInt16, stringPtr));

			F0_1182_002a_DrawString(text, x, y, frontColor);
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
			F0_1182_005c_DrawStringToScreen0(stringPtr, x, y + 1, 0);
			F0_1182_005c_DrawStringToScreen0(stringPtr, x, y, frontColor);
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
			x -= F0_1182_00ef_GetStringWidth(text) / 2;

			this.parent.Var_aa_Screen0_Rectangle.Flags = 0;

			F0_1182_002a_DrawString(text, x, y, frontColor);

			this.parent.Var_aa_Screen0_Rectangle.Flags = 1;
		}

		/// <summary>
		/// Draws a centered string with shadow by rectangle defined at 0xaa on screen 0
		/// </summary>
		/// <param name="stringPtr"></param>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="frontColor"></param>
		public void F0_1182_00b3_DrawCenteredStringWithShadowToScreen0(ushort stringPtr, int x, int y, byte frontColor)
		{
			string text = this.CPU.ReadString(VCPU.ToLinearAddress(this.CPU.DS.UInt16, stringPtr));

			F0_1182_00b3_DrawCenteredStringWithShadowToScreen0(text, x, y, frontColor);
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
			x -= F0_1182_00ef_GetStringWidth(text) / 2;

			this.parent.Var_aa_Screen0_Rectangle.Flags = 0;

			F0_1182_002a_DrawString(text, x + 1, y + 1, 0);
			F0_1182_002a_DrawString(text, x, y, frontColor);

			this.parent.Var_aa_Screen0_Rectangle.Flags = 1;
		}

		/// <summary>
		/// Calculates the string width
		/// </summary>
		/// <param name="stringPtr"></param>
		/// <returns></returns>
		public int F0_1182_00ef_GetStringWidth(ushort stringPtr)
		{
			string text = this.CPU.ReadString(VCPU.ToLinearAddress(this.CPU.DS.UInt16, stringPtr));

			return F0_1182_00ef_GetStringWidth(text);
		}

		/// <summary>
		/// Calculates the string width
		/// </summary>
		/// <param name="text"></param>
		/// <returns></returns>
		public int F0_1182_00ef_GetStringWidth(string text)
		{
			int width = this.parent.Graphics.GetDrawStringSize(this.parent.Var_aa_Screen0_Rectangle.FontID, text).Width;

			this.CPU.AX.UInt16 = (ushort)((short)width);

			return width;
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
		/// Fills a rectangle with default pattern
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="width"></param>
		/// <param name="height"></param>
		public void FillRectangleWithPattern(int x, int y, int width, int height)
		{
			// function body
			int currentY = y;
			int currentHeight = height;

			while (currentHeight > 0)
			{
				int cellHeight = Math.Min(currentHeight, 16);
				int currentX = x;
				int CurrentWidth = width;

				while (CurrentWidth > 0)
				{
					int iCellWidth = Math.Min(CurrentWidth, 32);

					// Instruction address 0x2dc4:0x0435, size: 5
					this.parent.Graphics.F0_VGA_07d8_DrawImage(this.parent.Var_19e8_Screen2_Rectangle,
						288, 120, iCellWidth, cellHeight, this.parent.Var_aa_Screen0_Rectangle, currentX, currentY);

					currentX += iCellWidth;
					CurrentWidth -= iCellWidth;
				}

				currentY += cellHeight;
				currentHeight -= cellHeight;
			}
		}
	}
}
