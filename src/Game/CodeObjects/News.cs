namespace OpenCivOne
{
	public class News
	{
		private OpenCivOneGame parent;

		public News(OpenCivOneGame parent)
		{
			this.parent = parent;
		}

		/// <summary>
		/// Shows the News
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
					if (this.parent.GameData.Cities[i].StatusFlag != 0xff &&
						this.parent.GameData.Cities[i].PlayerID == this.parent.GameData.HumanPlayerID)
					{
						if (cityID == -1 || (this.parent.GameData.Cities[i].ImprovementFlags0 & 0x1) != 0)
						{
							newCityID = i;
						}
					}
				}
			}

			if (cityID != -2)
			{
				this.parent.Graphics.F0_VGA_07d8_DrawImage(this.parent.Var_aa_Screen0_Rectangle, 0, 0, 320, 200, this.parent.Var_19d4_Screen1_Rectangle, 0, 0);
			}

			this.parent.DrawTools.FillRectangle(this.parent.Var_aa_Screen0_Rectangle, 0, 0, 320, 100, 15);

			this.parent.Var_aa_Screen0_Rectangle.FontID = 4;

			this.parent.Graphics.F0_VGA_0599_DrawLine(this.parent.Var_aa_Screen0_Rectangle, 1, 1, 318, 1, 0);

			this.parent.DrawTools.FillRectangle(this.parent.Var_aa_Screen0_Rectangle, 8, 3, 304, 20, 15);

			this.parent.Var_aa_Screen0_Rectangle.BackColor = 15;

			this.parent.Graphics.F0_VGA_0599_DrawLine(this.parent.Var_aa_Screen0_Rectangle, 0, 35, 319, 35, 0);
			this.parent.Graphics.F0_VGA_0599_DrawLine(this.parent.Var_aa_Screen0_Rectangle, 1, 1, 1, 35, 0);
			this.parent.Graphics.F0_VGA_0599_DrawLine(this.parent.Var_aa_Screen0_Rectangle, 318, 1, 318, 35, 0);
			this.parent.Graphics.F0_VGA_0599_DrawLine(this.parent.Var_aa_Screen0_Rectangle, 0, 97, 319, 97, 0);

			if (cityID != -2)
			{
				for (int i = 0; i < 16; i++)
				{
					this.parent.Graphics.F0_VGA_0c3e_DrawBitmapToScreen(this.parent.Var_aa_Screen0_Rectangle,
						20 * i, 100 - this.parent.CAPI.RNG.Next(2), this.parent.Var_df0c);
				}
			}

			this.parent.DrawTools.DrawTextBlock(16, 40, newsText, 288, 0);

			this.parent.Var_aa_Screen0_Rectangle.FontID = 5;

			string newsTitle = "The Daily News";
			int titleWidth = this.parent.DrawTools.GetStringWidth(newsTitle);

			if (newCityID >= 0)
			{
				switch (this.parent.CAPI.RNG.Next(4))
				{
					case 0:
						newsTitle = $"{this.parent.Segment_2459.F0_2459_08c6_GetCityName(newCityID)} Weekly";
						break;

					case 1:
						newsTitle = $"{this.parent.Segment_2459.F0_2459_08c6_GetCityName(newCityID)} Today";
						break;

					case 2:
						newsTitle = $"The {this.parent.Segment_2459.F0_2459_08c6_GetCityName(newCityID)} Times";
						break;

					case 3:
						newsTitle = $"The {this.parent.Segment_2459.F0_2459_08c6_GetCityName(newCityID)} Tribune";
						break;
				}

				titleWidth = this.parent.DrawTools.GetStringWidth(newsTitle);

				if (titleWidth > 300)
				{
					newsTitle = $"{this.parent.Segment_2459.F0_2459_08c6_GetCityName(newCityID)} News";

					titleWidth = this.parent.DrawTools.GetStringWidth(newsTitle);
				}
			}

			if (titleWidth < 200)
			{
				this.parent.DrawTools.F0_1182_002a_DrawString(",-.", 8, 11, 0);
				this.parent.DrawTools.F0_1182_002a_DrawString(",-.", 268, 11, 0);
			}

			this.parent.DrawTools.F0_1182_00b3_DrawCenteredStringToScreen0(newsTitle, 160, 11, 0);

			string newsExtra = (this.parent.CAPI.RNG.Next(2) != 0) ? "EXTRA!" : "FLASH";

			this.parent.Var_aa_Screen0_Rectangle.FontID = 3;

			this.parent.DrawTools.F0_1182_002a_DrawString(newsExtra, 6, 3, 0);
			this.parent.DrawTools.F0_1182_002a_DrawString(newsExtra, 272, 3, 0);

			this.parent.Var_aa_Screen0_Rectangle.FontID = 1;

			this.parent.DrawTools.F0_1182_005c_DrawStringToScreen0("10 cents", 272, 28, 0);
			this.parent.DrawTools.F0_1182_005c_DrawStringToScreen0($"January 1, {this.parent.Segment_1238.F0_1238_1720_GetCurrentYearAsString()}", 8, 28, 0);

			this.parent.Var_aa_Screen0_Rectangle.FontID = 2;

			string newsContent = this.parent.LanguageTools.F0_2f4d_01ad_GetTextBySectionAndKey("KING", $"*NEWS{(char)('A' + this.parent.CAPI.RNG.Next(14))}");

			this.parent.DrawTools.F0_1182_00b3_DrawCenteredStringToScreen0(newsContent, 160, 3, 0);

			this.parent.Var_aa_Screen0_Rectangle.FontID = 1;

			if (cityID != -2)
			{
				this.parent.MenuBoxDialog.F0_2d05_0031_ShowMenuBox("Press any key to continue.", 80, 128, true, false, true);

				this.parent.Graphics.F0_VGA_07d8_DrawImage(this.parent.Var_19d4_Screen1_Rectangle, 0, 0, 320, 200, this.parent.Var_aa_Screen0_Rectangle, 0, 0);
			}
		}
	}
}
