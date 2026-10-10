using Avalonia.Media;
using IRB.VirtualCPU;

namespace OpenCivOne
{
	public class Animations
	{
		private OpenCivOneGame parent;
		private VCPU CPU;
		public Animations(OpenCivOneGame parent)
		{
			this.parent = parent;
			this.CPU = parent.CPU;
		}

		/// <summary>
		/// Draws a nuke animation on a Map
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		public void F22_0000_0967_NukeAnimation(int x, int y)
		{
			//this.oCPU.Log.EnterBlock($"F22_0000_0967({xPos}, {yPos})");

			// function body
			// Instruction address 0x0000:0x098d, size: 5
			byte[] palette;
			int[] bitmaps = new int[28];

			this.parent.ImageTools.F0_2fa1_01a2_LoadBitmapOrPalette(1, 0, 0, "NUKE1.PIC", out palette);

			for (int i = 0; i < 28; i++)
			{
				// Instruction address 0x0000:0x09ca, size: 5
				bitmaps[i] = this.parent.Graphics.F0_VGA_0b85_ScreenToBitmap(1, ((i % 7) * 45) + 1, ((i / 7) * 45) + 1, 44, 44);
			}

			x = this.parent.CommonTools.CheckValueRange(this.parent.MapManagement.AdjustXPosition(x - this.parent.Var_d4cc_MapViewX) * 16 + 66, 80, 276);
			y = this.parent.CommonTools.CheckValueRange((y - this.parent.Var_d75e_MapViewY) * 16 - 6, 8, 156);

			// Instruction address 0x0000:0x0a41, size: 5
			this.parent.CommonTools.PlayTune(42, 0);

			// Instruction address 0x0000:0x0a62, size: 5
			this.parent.Graphics.F0_VGA_07d8_DrawImage(this.parent.Var_aa_Screen0_Rectangle, x, y, 44, 44, this.parent.Var_19d4_Screen1_Rectangle, x, y);

			for (int i = 0; i < 35; i++)
			{
				if (i >= 28)
				{
					// Instruction address 0x0000:0x0a86, size: 5
					this.parent.Graphics.F0_VGA_0c3e_DrawBitmapToScreen(this.parent.Var_aa_Screen0_Rectangle, x, y, bitmaps[i - 7]);
				}
				else
				{
					// Instruction address 0x0000:0x0a86, size: 5
					this.parent.Graphics.F0_VGA_0c3e_DrawBitmapToScreen(this.parent.Var_aa_Screen0_Rectangle, x, y, bitmaps[i]);
				}

				if (this.parent.GlobalPreferences.Debug.FastMode)
				{
					this.parent.CommonTools.WaitTimer(1);
				}
				else
				{
					this.parent.CommonTools.WaitTimer(5);
				}

				// Instruction address 0x0000:0x0ab3, size: 5
				this.parent.Graphics.F0_VGA_07d8_DrawImage(this.parent.Var_19d4_Screen1_Rectangle, x, y, 44, 44, this.parent.Var_aa_Screen0_Rectangle, x, y);
			}

			// Instruction address 0x0000:0x0ad2, size: 5
			for (int i = 0; i < 28; i++)
			{
				this.parent.Graphics.FreeResource(bitmaps[i], "NukeAnimation");
			}

			// Instruction address 0x0000:0x0aeb, size: 5
			this.parent.CommonTools.LoadAndApplyDefaultColorPalette();
		}

		/// <summary>
		/// Found new city animation
		/// </summary>
		/// <param name="cityID"></param>
		public void F22_0000_0eaf_FoundNewCityAnimation(int cityID)
		{
			//this.oCPU.Log.EnterBlock($"F22_0000_0eaf({cityID})");

			// function body
			// Instruction address 0x0000:0x0ecd, size: 5
			this.parent.CommonTools.StopPaletteCycleSlots();

			// Instruction address 0x0000:0x0ee4, size: 5
			this.parent.ImageTools.F0_2fa1_01a2_LoadBitmapOrPalette(1, 0, 0, "SETTLERS.PIC", 0);

			int[] bitmaps = new int[4];

			for (int i = 0; i < 4; i++)
			{
				// Instruction address 0x0000:0x0f07, size: 5
				bitmaps[i] = this.parent.Graphics.F0_VGA_0b85_ScreenToBitmap(1, 1, (i * 16) + 1, 47, 15);
			}

			// Instruction address 0x0000:0x0f2d, size: 5
			this.parent.ImageTools.F0_2fa1_01a2_LoadBitmapOrPalette(1, 0, 0, "HILL.PIC", 0);

			// Instruction address 0x0000:0x0f48, size: 5
			this.parent.DrawTools.FillRectangle(this.parent.Var_aa_Screen0_Rectangle, 0, 0, 320, 200, 0);

			// Instruction address 0x0000:0x0f54, size: 5
			this.parent.ImageTools.F0_2fa1_01a2_LoadBitmapOrPalette(-1, 0, 0, "HILL.PIC", 1);

			// Instruction address 0x0000:0x0f74, size: 5
			this.parent.Graphics.F0_VGA_07d8_DrawImage(this.parent.Var_19d4_Screen1_Rectangle, 0, 0, 320, 200, this.parent.Var_aa_Screen0_Rectangle, 0, 0);

			this.parent.Var_aa_Screen0_Rectangle.FontID = 6;

			// Instruction address 0x0000:0x0fc9, size: 5
			this.parent.DrawTools.DrawCenteredStringToScreen0($"{this.parent.Actions.F0_2459_08c6_GetCityName(cityID)} founded in", 160, 4, 0);
			this.parent.DrawTools.DrawCenteredStringToScreen0($"the year {this.parent.GameEngine.YearToString(this.parent.GameData.Year)}.", 160, 20, 0);

			this.parent.Var_aa_Screen0_Rectangle.FontID = 1;

			// Instruction address 0x0000:0x0ff5, size: 5
			this.parent.Graphics.F0_VGA_07d8_DrawImage(this.parent.Var_19d4_Screen1_Rectangle, 0, 120, 320, 40, this.parent.Var_19d4_Screen1_Rectangle, 0, 0);

			// Instruction address 0x0000:0x1002, size: 5
			this.parent.CommonTools.ClearKeyboardAndMouseEvents();

			for (int i = 80; i < 120; i++)
			{
				// Instruction address 0x0000:0x101a, size: 5
				this.parent.CommonTools.ResetWaitTimer();

				// Instruction address 0x0000:0x103a, size: 5
				this.parent.Graphics.F0_VGA_07d8_DrawImage(this.parent.Var_19d4_Screen1_Rectangle, 0, 0, 320, 40, this.parent.Var_19d4_Screen1_Rectangle, 0, 120);

				for (int j = 0; j < 4; j++)
				{
					// Instruction address 0x0000:0x1070, size: 5
					this.parent.Graphics.F0_VGA_0c3e_DrawBitmapToScreen(this.parent.Var_19d4_Screen1_Rectangle,
						(13 * j) + i, ((10 * j) & 0xf) + 120, bitmaps[(j + i) % 4]);
				}

				// Instruction address 0x0000:0x109f, size: 5
				this.parent.Graphics.F0_VGA_07d8_DrawImage(this.parent.Var_19d4_Screen1_Rectangle, 0, 120, 320, 40, this.parent.Var_aa_Screen0_Rectangle, 0, 120);

				while (this.parent.Var_5c_TickCount < 10)
				{
					this.CPU.DoEvents();
				}

				// Instruction address 0x0000:0x10b9, size: 5
				if (this.parent.CAPI.kbhit() != 0 || this.parent.GetMouseEvent().Buttons != MouseButtonsEnum.None) break;
			}

			for (int i = 0; i < 4; i++)
			{
				// Instruction address 0x0000:0x10d3, size: 5
				this.parent.Graphics.FreeResource(bitmaps[i], "FoundNewCity");
			}

			// Instruction address 0x0000:0x10fc, size: 5
			this.parent.CommonTools.TransformPaletteToColor(10, Color.FromRgb(0, 0, 0));

			// Instruction address 0x0000:0x1109, size: 5
			if (this.parent.CAPI.kbhit() == 0)
			{
				this.parent.CityView.F19_0000_0000_ShowCityLayout(cityID, -3, null);
			}
			else
			{
				// Instruction address 0x0000:0x1136, size: 5
				this.parent.DrawTools.FillRectangle(this.parent.Var_aa_Screen0_Rectangle, 0, 0, 320, 200, 0);

				// Instruction address 0x0000:0x113e, size: 5
				this.parent.CommonTools.LoadAndApplyDefaultColorPalette();
			}

			// Instruction address 0x0000:0x1148, size: 5
			this.parent.CommonTools.ClearKeyboardAndMouseEvents();
		}
	}
}
