using IRB.VirtualCPU;

namespace OpenCivOne
{
	public class News
	{
		private OpenCivOneGame oParent;
		private VCPU oCPU;

		public News(OpenCivOneGame parent)
		{
			this.oParent = parent;
			this.oCPU = parent.CPU;
		}

		/// <summary>
		/// ?
		/// </summary>
		/// <param name="cityID"></param>
		public void F21_0000_0000_ShowNews(int cityID, string newsText)
		{
			//this.oCPU.Log.EnterBlock($"F21_0000_0000({cityID})");

			// function body
			int newCityID = cityID;

			if (cityID < 0)
			{
				for (int i = 0; i < 128; i++)
				{
					if (this.oParent.GameData.Cities[i].StatusFlag != 0xff &&
						this.oParent.GameData.Cities[i].PlayerID == this.oParent.GameData.HumanPlayerID)
					{
						if (cityID == -1 || (this.oParent.GameData.Cities[i].ImprovementFlags0 & 0x1) != 0)
						{
							newCityID = i;
						}
					}
				}
			}

			this.oParent.MainCode.F0_11a8_0268_HideMouse();

			if (cityID != -2)
			{
				this.oParent.Graphics.F0_VGA_07d8_DrawImage(this.oParent.Var_aa_Screen0_Rectangle, 0, 0, 320, 200, this.oParent.Var_19d4_Screen1_Rectangle, 0, 0);
			}

			this.oParent.CommonTools.F0_1000_0bfa_FillRectangle(this.oParent.Var_aa_Screen0_Rectangle, 0, 0, 320, 100, 15);

			this.oParent.Var_aa_Screen0_Rectangle.FontID = 4;

			this.oParent.Graphics.F0_VGA_0599_DrawLine(this.oParent.Var_aa_Screen0_Rectangle, 1, 1, 318, 1, 0);

			this.oParent.CommonTools.F0_1000_0bfa_FillRectangle(this.oParent.Var_aa_Screen0_Rectangle, 8, 3, 304, 20, 15);

			this.oParent.Var_aa_Screen0_Rectangle.BackColor = 15;

			this.oParent.Graphics.F0_VGA_0599_DrawLine(this.oParent.Var_aa_Screen0_Rectangle, 0, 35, 319, 35, 0);
			this.oParent.Graphics.F0_VGA_0599_DrawLine(this.oParent.Var_aa_Screen0_Rectangle, 1, 1, 1, 35, 0);
			this.oParent.Graphics.F0_VGA_0599_DrawLine(this.oParent.Var_aa_Screen0_Rectangle, 318, 1, 318, 35, 0);
			this.oParent.Graphics.F0_VGA_0599_DrawLine(this.oParent.Var_aa_Screen0_Rectangle, 0, 97, 319, 97, 0);

			if (cityID != -2)
			{
				for (int i = 0; i < 16; i++)
				{
					this.oParent.CommonTools.F0_1000_0797_DrawBitmapToScreen(this.oParent.Var_aa_Screen0_Rectangle,
						20 * i, 100 - this.oParent.CAPI.RNG.Next(2), this.oParent.Var_df0c);
				}
			}

			this.oParent.DrawTools.DrawTextBlock(16, 40, newsText, 288, 0);

			this.oParent.Var_aa_Screen0_Rectangle.FontID = 5;

			string newsTitle = "The Daily News";
			int titleWidth = this.oParent.DrawTools.F0_1182_00ef_GetStringWidth(newsTitle);

			if (newCityID >= 0)
			{
				switch (this.oParent.CAPI.RNG.Next(4))
				{
					case 0:
						newsTitle = $"{this.oParent.Segment_2459.F0_2459_08c6_GetCityName(newCityID)} Weekly";
						break;

					case 1:
						newsTitle = $"{this.oParent.Segment_2459.F0_2459_08c6_GetCityName(newCityID)} Today";
						break;

					case 2:
						newsTitle = $"The {this.oParent.Segment_2459.F0_2459_08c6_GetCityName(newCityID)} Times";
						break;

					case 3:
						newsTitle = $"The {this.oParent.Segment_2459.F0_2459_08c6_GetCityName(newCityID)} Tribune";
						break;
				}

				titleWidth = this.oParent.DrawTools.F0_1182_00ef_GetStringWidth(newsTitle);

				if (titleWidth > 300)
				{
					newsTitle = $"{this.oParent.Segment_2459.F0_2459_08c6_GetCityName(newCityID)} News";

					titleWidth = this.oParent.DrawTools.F0_1182_00ef_GetStringWidth(newsTitle);
				}
			}

			if (titleWidth < 200)
			{
				this.oParent.DrawTools.F0_1182_002a_DrawString(",-.", 8, 11, 0);
				this.oParent.DrawTools.F0_1182_002a_DrawString(",-.", 268, 11, 0);
			}

			this.oParent.DrawTools.F0_1182_00b3_DrawCenteredStringToScreen0(newsTitle, 160, 11, 0);

			string newsExtra = (this.oParent.CAPI.RNG.Next(2) != 0) ? "EXTRA!" : "FLASH";

			this.oParent.Var_aa_Screen0_Rectangle.FontID = 3;

			this.oParent.DrawTools.F0_1182_002a_DrawString(newsExtra, 6, 3, 0);
			this.oParent.DrawTools.F0_1182_002a_DrawString(newsExtra, 272, 3, 0);

			this.oParent.Var_aa_Screen0_Rectangle.FontID = 1;

			this.oParent.DrawTools.F0_1182_005c_DrawStringToScreen0("10 cents", 272, 28, 0);
			this.oParent.DrawTools.F0_1182_005c_DrawStringToScreen0($"January 1, {this.oParent.Segment_1238.F0_1238_1720_GetCurrentYearAsString()}", 8, 28, 0);

			this.oParent.Var_aa_Screen0_Rectangle.FontID = 2;

			string newsContent = this.oParent.LanguageTools.F0_2f4d_01ad_GetTextBySectionAndKey("KING", $"*NEWS{(char)('A' + this.oParent.CAPI.RNG.Next(14))}");

			this.oParent.DrawTools.F0_1182_00b3_DrawCenteredStringToScreen0(newsContent, 160, 3, 0);

			this.oParent.Var_aa_Screen0_Rectangle.FontID = 1;

			if (cityID != -2)
			{
				this.oParent.MenuBoxDialog.F0_2d05_0031_ShowMenuBox("Press any key to continue.", 80, 128, true, false, true);

				this.oParent.Graphics.F0_VGA_07d8_DrawImage(this.oParent.Var_19d4_Screen1_Rectangle, 0, 0, 320, 200, this.oParent.Var_aa_Screen0_Rectangle, 0, 0);
			}

			this.oParent.MainCode.F0_11a8_0250_ShowMouse();
		}
	}
}
