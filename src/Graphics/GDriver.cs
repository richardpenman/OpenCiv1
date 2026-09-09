using System;
using System.IO.Compression;
using IRB.VirtualCPU;
using IRB.Collections.Generic;
using Avalonia.Media;
using OpenCivOne.Resources;

namespace OpenCivOne.Graphics
{
	public class GDriver
	{
		private OpenCivOneGame parent;
		private VCPU CPU;

		private BDictionary<int, GBitmap> screens = new BDictionary<int, GBitmap>();
		private int nextBitmapID = 0xb000;
		private BDictionary<int, GBitmap> bitmaps = new BDictionary<int, GBitmap>();
		private GFonts fonts;

		public GDriver(OpenCivOneGame parent)
		{
			this.parent = parent;
			this.CPU = parent.CPU;

			this.screens.Add(0, new GBitmap()); // Main screen

			byte[] fonts = CommonResources.Fonts_xml;

			Stream fonts1 = new GZipStream(new MemoryStream(fonts), CompressionMode.Decompress);
			//Stream fonts1 = new MemoryStream(fonts);
			this.fonts = GFonts.Deserialize(fonts1);

			fonts1.Close();
			
			//Stream writer = new GZipStream(new FileStream("Fonts.xml.gz", FileMode.Create), CompressionMode.Compress);
			//writer.Write(fonts, 0, fonts.Length);
			//writer.Flush();
			//writer.Close();
		}

		#region Properties
		public BDictionary<int, GBitmap> Screens
		{
			get { return this.screens; }
		}

		public BDictionary<int, GBitmap> Bitmaps
		{
			get { return this.bitmaps; }
		}
		#endregion

		#region Memory and Initialization functions
		public ushort F0_VGA_0492_GetFreeMemory()
		{
			// function body
			this.CPU.AX.UInt16 = (ushort)((this.CPU.Memory.FreeMemory.Size >> 4) & 0xffff);
			this.CPU.Flags.C = true;

			return this.CPU.AX.UInt16;
		}

		public ushort F0_VGA_04ae_AllocateScreen(int screenID)
		{
			// function body
			if (!this.screens.ContainsKey(screenID))
			{
				lock (VCPU.GraphicsLock)
				{
					GBitmap bitmap = new GBitmap();
					bitmap.Visible = false; // additional screens are not shown by default
					this.screens.Add(screenID, bitmap);
				}
			}

			this.CPU.AX.UInt16 = 0xa000; // return something to make underlying code happy

			return this.CPU.AX.UInt16;
		}
		#endregion

		#region Palette functions
		public void F0_VGA_010c_SetColorsByIndexArray(ushort indexArrayPtr)
		{
			// function body
			lock (VCPU.GraphicsLock)
			{
				Color[] aColors = new Color[16];

				for (int i = 0; i < 16; i++)
				{
					aColors[i] = GBitmap.Palette1[this.CPU.ReadUInt8(this.CPU.DS.UInt16, indexArrayPtr)];

					indexArrayPtr++;
				}

				// set colors to all planes, as this is what original code does
				for (int i = 0; i < this.screens.Count; i++)
				{
					this.screens[i].Value.SetPaletteFromColorArray(aColors, 0, 0, 16);
				}
			}
		}

		public void SetPaletteColor(byte index, Color color)
		{
			lock (VCPU.GraphicsLock)
			{
				// set colors to all planes, as this is what original code does
				for (int i = 0; i < this.screens.Count; i++)
				{
					this.screens[i].Value.SetPaletteColor(index, color);
				}
			}
		}

		public Color GetPaletteColor(byte index)
		{
			lock (VCPU.GraphicsLock)
			{
				// set colors to all planes, as this is what original code does
				if (this.screens.Count > 0)
				{
					return this.screens[0].Value.GetPaletteColor(index);
				}
			}

			return Color.FromUInt32(0);
		}

		public void F0_VGA_0162_SetColorsFromColorStruct(ushort colorStructPtr)
		{
			// function body
			if (this.CPU.ReadUInt16(this.CPU.DS.UInt16, colorStructPtr) == 0x304d)
			{
				int iFrom = this.CPU.ReadUInt8(this.CPU.DS.UInt16, (ushort)(colorStructPtr + 0x4));
				int iTo = this.CPU.ReadUInt8(this.CPU.DS.UInt16, (ushort)(colorStructPtr + 0x5));
				int iCount = (iTo - iFrom) + 1;
				colorStructPtr += 6;

				lock (VCPU.GraphicsLock)
				{
					Color[] aColors = new Color[iCount];

					for (int i = 0; i < iCount; i++)
					{
						aColors[i] = GBitmap.Color18ToColor(this.CPU.ReadUInt8(this.CPU.DS.UInt16, (ushort)(colorStructPtr + (i * 3))),
							this.CPU.ReadUInt8(this.CPU.DS.UInt16, (ushort)(colorStructPtr + (i * 3) + 1)),
						this.CPU.ReadUInt8(this.CPU.DS.UInt16, (ushort)(colorStructPtr + (i * 3) + 2)));

						this.CPU.Log.WriteLine($"Setting palette index {iFrom + i}, #{aColors[i].A:x2}{aColors[i].R:x2}{aColors[i].G:x2}{aColors[i].B:x2}");
					}

					// set colors to all planes, as this is what original code does
					for (int i = 0; i < this.screens.Count; i++)
					{
						this.screens[i].Value.SetPaletteFromColorArray(aColors, iFrom, iFrom, iCount);
					}
				}
			}
		}

		public void F0_VGA_0162_SetColorsFromColorStruct(byte[] colorStruct)
		{
			// function body
			int colorStructPtr = 0;

			if (colorStruct[0] == 0x4d && colorStruct[1] == 0x30)
			{
				int iFrom = colorStruct[4];
				int iTo = colorStruct[5];
				int iCount = (iTo - iFrom) + 1;

				colorStructPtr += 6;

				lock (VCPU.GraphicsLock)
				{
					Color[] aColors = new Color[iCount];

					for (int i = 0; i < iCount; i++)
					{
						aColors[i] = GBitmap.Color18ToColor(colorStruct[colorStructPtr + (i * 3)],
							colorStruct[colorStructPtr + (i * 3) + 1], colorStruct[colorStructPtr + (i * 3) + 2]);

						//this.oCPU.Log.WriteLine($"Setting palette index {iFrom + i}, #{aColors[i].A:x2}{aColors[i].R:x2}{aColors[i].G:x2}{aColors[i].B:x2}");
					}

					// set colors to all planes, as this is what original code does
					for (int i = 0; i < this.screens.Count; i++)
					{
						this.screens[i].Value.SetPaletteFromColorArray(aColors, iFrom, iFrom, iCount);
					}
				}
			}
		}

		public void SetColorsFromColorStruct(byte[] colorStruct)
		{
			int iStructPos = 0;

			if (colorStruct[iStructPos] == 0x4d && colorStruct[iStructPos + 1] == 0x30)
			{
				int iFrom = colorStruct[iStructPos + 4];
				int iTo = colorStruct[iStructPos + 5];
				int iCount = (iTo - iFrom) + 1;
				iStructPos += 6;

				lock (VCPU.GraphicsLock)
				{
					Color[] aColors = new Color[iCount];

					for (int i = 0; i < iCount; i++)
					{
						aColors[i] = GBitmap.Color18ToColor(colorStruct[iStructPos + (i * 3)],
							colorStruct[iStructPos + (i * 3) + 1],
							colorStruct[iStructPos + (i * 3) + 2]);
					}

					// set colors to all screens, as this is what original code does
					for (int i = 0; i < this.screens.Count; i++)
					{
						this.screens[i].Value.SetPaletteFromColorArray(aColors, iFrom, iFrom, iCount);
					}
				}
			}
		}
		#endregion

		#region Drawing functions
		public void F0_VGA_06b7_DrawScreenToScreen0WithEffect(int screenID)
		{
			// function body
			// this is either random fade in image, or a scrolling from right to left
			if (screenID != 0)
			{
				if (this.screens.ContainsKey(screenID))
				{
					RandomMT19937 oRNG = new RandomMT19937();
					GBitmap screen0 = this.screens.GetValueByKey(0);
					GBitmap newScreen = this.screens.GetValueByKey(screenID);
					bool[,] aPixels = new bool[newScreen.Width, newScreen.Height];

					for (int i = 0; i < newScreen.Width; i++)
					{
						for (int j = 0; j < newScreen.Height; j++)
						{
							aPixels[i, j] = false;
						}
					}

					int iCount = 0;
					int iPixelCount = newScreen.Width * newScreen.Height;

					while (iCount < iPixelCount)
					{
						// draw it in batches, because of the slow timer
						lock (VCPU.GraphicsLock)
						{
							for (int k = 0; k < 80 && iCount < iPixelCount; k++)
							{
								int xPos = oRNG.Next(newScreen.Width);
								int yPos = oRNG.Next(newScreen.Height);

								while (aPixels[xPos, yPos])
								{
									xPos = oRNG.Next(newScreen.Width);
									yPos = oRNG.Next(newScreen.Height);
								}

								aPixels[xPos, yPos] = true;
								iCount++;
								screen0.SetPixel(xPos, yPos, newScreen.GetPixel(xPos, yPos));
							}
						}

						if (this.parent.CAPI.kbhit() != 0 || this.parent.PeekMouseEvent().Buttons != MouseButtonsEnum.None)
						{
							screen0.DrawBitmap(newScreen);
							break;
						}

						Thread.Sleep(1);
					}
				}
				else
				{
					throw new Exception($"The screen {screenID} is not allocated");
				}
			}
		}

		public void F0_VGA_06b7_DrawScreenToMainScreen(int screenID)
		{
			// function body
			if (screenID != 0)
			{
				if (this.screens.ContainsKey(screenID))
				{
					GBitmap screen0 = this.screens.GetValueByKey(0);
					GBitmap newScreen = this.screens.GetValueByKey(screenID);

					lock (VCPU.GraphicsLock)
					{
						screen0.DrawBitmap(newScreen);
					}
				}
				else
				{
					throw new Exception($"The screen {screenID} is not allocated");
				}
			}
		}

		public void F0_VGA_07d8_DrawImage(CRectangle srcRect, int xFrom, int yFrom, int width, int height, CRectangle dstRect, int xTo, int yTo)
		{
			int iXOffsetFrom = srcRect.Left + xFrom;
			int iYOffsetFrom = srcRect.Top + yFrom;
			int iXOffsetTo = dstRect.Left + xTo;
			int iYOffsetTo = dstRect.Top + yTo;

			// function body
			if (this.screens.ContainsKey(srcRect.ScreenID))
			{
				GBitmap srcBitmap = this.screens.GetValueByKey(srcRect.ScreenID);

				if (this.screens.ContainsKey(dstRect.ScreenID))
				{
					lock (VCPU.GraphicsLock)
					{
						GBitmap destBitmap = this.screens.GetValueByKey(dstRect.ScreenID);

						destBitmap.DrawBitmap(iXOffsetTo, iYOffsetTo, srcBitmap, new GRectangle(iXOffsetFrom, iYOffsetFrom, width, height), false);
					}
				}
				else
				{
					throw new Exception($"The screen {dstRect.ScreenID} is not allocated");
				}
			}
			else
			{
				throw new Exception($"The screen {srcRect.ScreenID} is not allocated");
			}
		}

		public int BitmapToBitmap(GBitmap bitmap, int x, int y, int width, int height)
		{
			int newBitmapID = -1;

			// function body
			lock (VCPU.GraphicsLock)
			{
				GBitmap newBitmap = new GBitmap(width, height);
				GRectangle rect = new GRectangle(x, y, width, height);
				newBitmapID = this.nextBitmapID;

				bitmap.CopyPalette(newBitmap);

				newBitmap.DrawBitmap(0, 0, bitmap, rect, false);

				this.bitmaps.Add(newBitmapID, newBitmap);

				this.nextBitmapID++;
			}

			return newBitmapID;
		}

		public int F0_VGA_0b85_ScreenToBitmap(int screenID, int x, int y, int width, int height)
		{
			int newBitmapID = -1;

			// function body
			if (this.screens.ContainsKey(screenID))
			{
				lock (VCPU.GraphicsLock)
				{
					GBitmap screen = this.screens.GetValueByKey(screenID);
					GBitmap bitmap = new GBitmap(width, height);
					GRectangle rect = new GRectangle(x, y, width, height);
					newBitmapID = this.nextBitmapID;

					screen.CopyPalette(bitmap);

					bitmap.DrawBitmap(0, 0, screen, rect, false);

					this.bitmaps.Add(newBitmapID, bitmap);

					//bitmap.Bitmap.SaveToPIC($"Bitmaps{Path.DirectorySeparatorChar}Image_{this.iNextBitmapID:x4}.png", ImageFormat.Png);

					this.CPU.AX.UInt16 = (ushort)((short)newBitmapID);

					this.nextBitmapID++;
				}
			}
			else
			{
				throw new Exception($"The screen {screenID} is not allocated");
			}

			return newBitmapID;
		}

		public void F0_VGA_0c3e_DrawBitmapToScreen(CRectangle rect, int x, int y, int bitmapID)
		{
			// function body
			if (this.screens.ContainsKey(rect.ScreenID))
			{
				if (this.bitmaps.ContainsKey(bitmapID))
				{
					lock (VCPU.GraphicsLock)
					{
						GBitmap screen = this.screens.GetValueByKey(rect.ScreenID);
						GBitmap bitmap = this.bitmaps.GetValueByKey(bitmapID);

						screen.DrawBitmap(rect.Left + x, rect.Top + y, bitmap, true);
					}
				}
				else
				{
					this.CPU.Log.WriteLine($"The bitmap 0x{bitmapID:x4} is not allocated");
				}
			}
			else
			{
				throw new Exception($"The screen {rect.ScreenID} is not allocated");
			}
		}

		public void DrawBitmapToScreen(GBitmap bitmap, int xFrom, int yFrom, int width, int height, CRectangle dstRect, int xTo, int yTo)
		{
			// function body
			if (this.screens.ContainsKey(dstRect.ScreenID))
			{
				lock (VCPU.GraphicsLock)
				{
					GBitmap screen = this.screens.GetValueByKey(dstRect.ScreenID);

					screen.DrawBitmap(dstRect.Left + xTo, dstRect.Top + yTo, 
						bitmap, new GRectangle(xFrom, yFrom, width, height), false);
				}
			}
			else
			{
				throw new Exception($"The screen {dstRect.ScreenID} is not allocated");
			}
		}

		public int LoadIcon(string filename)
		{
			byte[] aTemp;
			GBitmap? bitmap = GBitmap.FromPICFile($"{this.parent.ResourcePath}{filename}", out aTemp, true);
			int bitmapID = -1;

			if (bitmap != null)
			{
				this.bitmaps.Add(this.nextBitmapID, bitmap);

				bitmapID = this.nextBitmapID;
				this.nextBitmapID++;
			}

			return bitmapID;
		}

		public byte F0_VGA_038c_GetPixel(int screenID, int x, int y)
		{
			// function body
			if (this.screens.ContainsKey(screenID))
			{
				lock (VCPU.GraphicsLock)
				{
					this.CPU.AX.UInt16 = this.screens.GetValueByKey(screenID).GetPixel(x, y);
				}
			}
			else
			{
				throw new Exception($"The screen {screenID} is not allocated");
			}

			return this.CPU.AX.LowUInt8;
		}

		public void F0_VGA_0550_SetPixel(int screenID, int x, int y, byte frontColor)
		{
			F0_VGA_0550_SetPixel(screenID, x, y, frontColor, 0);
		}

		public void F0_VGA_0550_SetPixel(int screenID, int x, int y, byte frontColor, byte pixelMode)
		{
			// function body
			if (this.screens.ContainsKey(screenID))
			{
				lock (VCPU.GraphicsLock)
				{
					GBitmap screen = this.screens.GetValueByKey(screenID);

					screen.SetPixel(x, y, frontColor, (PixelWriteModeEnum)pixelMode);
				}
			}
			else
			{
				throw new Exception($"The screen {screenID} is not allocated");
			}
		}

		public void F0_VGA_0599_DrawLine(CRectangle rect, int x1, int y1, int x2, int y2, ushort mode)
		{
			// function body
			if (this.screens.ContainsKey(rect.ScreenID))
			{
				lock (VCPU.GraphicsLock)
				{
					GBitmap screen = this.screens.GetValueByKey(rect.ScreenID);

					x1 += rect.Left;
					y1 += rect.Top;
					x2 += rect.Left;
					y2 += rect.Top;

					screen.DrawLine(x1, y1, x2, y2, (byte)(mode & 0xff), (PixelWriteModeEnum)((mode & 0xff00) >> 8));
				}
			}
			else
			{
				throw new Exception($"The screen {rect.ScreenID} is not allocated");
			}
		}

		public void F0_VGA_040a_FillRectangle(int screenID, GRectangle rect, byte frontColor, byte pixelMode)
		{
			// function body
			if (this.screens.ContainsKey(screenID))
			{
				lock (VCPU.GraphicsLock)
				{
					this.screens.GetValueByKey(screenID).FillRectangle(rect, frontColor, (PixelWriteModeEnum)pixelMode);
				}
			}
			else
			{
				throw new Exception($"The screen {screenID} is not allocated");
			}
		}

		public void F0_VGA_009a_ReplaceColor(CRectangle rect, int x, int y, int width, int height, byte oldColor, byte newColor)
		{
			// function body
			lock (VCPU.GraphicsLock)
			{
				int iLeft = rect.Left + x;
				int iTop = rect.Top + y;

				if (this.screens.ContainsKey(rect.ScreenID))
				{
					this.screens.GetValueByKey(rect.ScreenID).ReplaceColor(new GRectangle(iLeft, iTop, width, height), oldColor, newColor);
				}
				else
				{
					throw new Exception($"The screen {rect.ScreenID} is not allocated");
				}
			}
		}
		#endregion

		#region Fonts
		public int F0_VGA_115d_GetCharWidth(int fontID, byte ch)
		{
			// function body
			int iValue = GetDrawStringSize(fontID, new string((char)ch, 1)).Width;
			this.CPU.AX.UInt16 = (ushort)((short)iValue);

			return iValue;
		}

		public int F0_VGA_115d_GetCharWidth(int fontID, char ch)
		{
			// function body
			int iValue = GetDrawStringSize(fontID, new string(ch, 1)).Width;
			this.CPU.AX.UInt16 = (ushort)((short)iValue);

			return iValue;
		}

		public GSize GetDrawStringSize(int fontID, string text)
		{
			int iWidth = 0;
			int iHeight = 0;

			if (!this.fonts.ContainsKey(fontID))
			{
				fontID = 1; // default font
			}

			GFont font = this.fonts.GetValueByKey(fontID);

			for (int i = 0; i < text.Length; i++)
			{
				char ch = text[i];
				GFontChar fontCh;

				//if (i > 0)
				iWidth += font.CharacterWidthSpacing;

				if (font.Characters.ContainsKey(ch))
				{
					fontCh = font.Characters.GetValueByKey(ch);
				}
				else
				{
					// unknown char, use '?'
					fontCh = font.Characters.GetValueByKey('?');
				}
				iWidth += fontCh.Width;
				iHeight = Math.Max(iHeight, fontCh.Height + font.LineSpacing);
			}

			return new GSize(iWidth, iHeight);
		}

		public int F0_VGA_11ae_GetTextHeight(int fontID)
		{
			// function body
			int iValue = GetDrawStringSize(fontID, "?").Height;
			this.CPU.AX.UInt16 = (ushort)((short)iValue);

			return iValue;
		}

		public void F0_VGA_11d7_DrawString(CRectangle rect, int x, int y, string text)
		{
			if (!this.fonts.ContainsKey(rect.FontID))
			{
				rect.FontID = 1; // default font
			}

			// function body
			GFont font = this.fonts.GetValueByKey(rect.FontID);

			if (this.screens.ContainsKey(rect.ScreenID))
			{
				lock (VCPU.GraphicsLock)
				{
					GBitmap screen = this.screens.GetValueByKey(rect.ScreenID);
					GRectangle rect1 = new GRectangle(rect.Left + x, rect.Top + y, rect.Width, rect.Height);

					screen.DrawString(text, font, rect1, rect.FrontColor, rect.BackColor, (PixelWriteModeEnum)rect.PixelMode);
				}
			}
			else
			{
				throw new Exception($"The screen {rect.ScreenID} is not allocated");
			}
		}
		#endregion
	}
}
