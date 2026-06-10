using System;
using IRB.VirtualCPU;
using OpenCivOne.Graphics;

namespace OpenCivOne
{
	/// <summary>
	/// Image loading functions - RLE and LZW compression was used
	/// </summary>
	public class ImageTools
	{
		private OpenCivOneGame parent;
		private VCPU CPU;

		public ImageTools(OpenCivOneGame parent)
		{
			this.parent = parent;
			this.CPU = parent.CPU;
		}

		#region New Image loading functions
		public void F0_2fa1_01a2_LoadBitmapOrPalette(int screenID, int x, int y, ushort filenamePtr, ushort palettePtr)
		{
			byte[] palette;

			F0_2fa1_01a2_LoadBitmapOrPalette(screenID, x, y, this.CPU.ReadString(this.CPU.DS.UInt16, filenamePtr), out palette);

			if (palette != null)
			{
				ushort startPtr;

				switch (palettePtr)
				{
					case 0:
						startPtr = 0xba08;
						for (int i = 0; i < palette.Length; i++)
						{
							this.CPU.WriteUInt8(this.CPU.DS.UInt16, startPtr++, palette[i]);
						}
						break;
					case 1:
						startPtr = 0xba06;
						for (int i = 0; i < palette.Length; i++)
						{
							this.CPU.WriteUInt8(this.CPU.DS.UInt16, startPtr++, palette[i]);
						}
						break;

					default:
						startPtr = palettePtr;
						for (int i = 0; i < palette.Length; i++)
						{
							this.CPU.WriteUInt8(this.CPU.DS.UInt16, startPtr++, palette[i]);
						}
						break;
				}

				if (palettePtr == 1 || palettePtr == 0xba06)
					this.parent.Graphics.SetColorsFromColorStruct(palette);
			}
		}

		public void F0_2fa1_01a2_LoadBitmapOrPalette(int screenID, int x, int y, string filename, ushort palettePtr)
		{
			byte[] palette;

			F0_2fa1_01a2_LoadBitmapOrPalette(screenID, x, y, filename, out palette);

			if (palette != null)
			{
				ushort startPtr;

				switch (palettePtr)
				{
					case 0:
						startPtr = 0xba08;
						for (int i = 0; i < palette.Length; i++)
						{
							this.CPU.WriteUInt8(this.CPU.DS.UInt16, startPtr++, palette[i]);
						}
						break;
					case 1:
						startPtr = 0xba06;
						for (int i = 0; i < palette.Length; i++)
						{
							this.CPU.WriteUInt8(this.CPU.DS.UInt16, startPtr++, palette[i]);
						}
						break;

					default:
						startPtr = palettePtr;
						for (int i = 0; i < palette.Length; i++)
						{
							this.CPU.WriteUInt8(this.CPU.DS.UInt16, startPtr++, palette[i]);
						}
						break;
				}

				if (palettePtr == 1 || palettePtr == 0xba06)
					this.parent.Graphics.SetColorsFromColorStruct(palette);
			}
		}

		/// <summary>
		/// Loads and .pic or .map Image from given filename with optional palette data
		/// </summary>
		/// <param name="screenID"></param>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="filenamePtr"></param>
		/// <param name="palettePtr"></param>
		/// <exception cref="Exception"></exception>
		public void F0_2fa1_01a2_LoadBitmapOrPalette(int screenID, int x, int y, string filename, out byte[] palette)
		{
			string filename1 = this.parent.ResourcePath + CAPI.GetDOSFileName(filename);

			//this.CPU.Log.EnterBlock($"F0_2fa1_01a2_LoadBitmapOrPalette(0x{screenID:x4}, 0x{xPos:x4}, 0x{yPos:x4}, '{filename1}')");

			if (screenID >= 0 && (Path.GetExtension(filename1).Equals(".pic", StringComparison.InvariantCultureIgnoreCase) ||
				Path.GetExtension(filename1).Equals(".map", StringComparison.InvariantCultureIgnoreCase)))
			{
				if (this.parent.Graphics.Screens.ContainsKey(screenID))
				{
					this.parent.Graphics.Screens.GetValueByKey(screenID).LoadPIC(filename1, x, y, out palette);
				}
				else
				{
					throw new Exception($"The page {screenID} is not allocated");
				}
			}
			else
			{
				GBitmap.ReadPaletteFromPICFile(filename1, out palette);
			}
		}

		/// <summary>
		/// Loads an Bitmap (Icon) from given image Filename
		/// </summary>
		/// <param name="filenamePtr"></param>
		public int F0_2fa1_044c_LoadIcon(ushort filenamePtr)
		{
			return this.parent.Graphics.LoadIcon(CAPI.GetDOSFileName(this.CPU.ReadString(VCPU.ToLinearAddress(this.CPU.DS.UInt16, filenamePtr))));
		}
		#endregion
	}
}
