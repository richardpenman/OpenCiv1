using Avalonia.Media;
using IRB.VirtualCPU;
using OpenCivOne.Graphics;

namespace OpenCivOne
{
	public class MainIntro
	{
		private OpenCivOneGame parent;
		private VCPU CPU;

		public MainIntro(OpenCivOneGame parent)
		{
			this.parent = parent;
			this.CPU = parent.CPU;
		}

		/// <summary>
		/// Shows and animates main game intro
		/// </summary>
		public void F2_0000_0000_ShowMainGameIntro()
		{
			//this.oCPU.Log.EnterBlock("F2_0000_0000()");

			// function body
			byte[] introPalette;
			this.parent.Var_deba = 8;
			this.parent.Var_aa_Screen0_Rectangle.FontID = 1;

			for (int i = 0; i < 3; i++)
			{
				// Instruction address 0x0000:0x01a1, size: 5
				this.parent.Graphics.F0_VGA_04ae_AllocateScreen(i);
			}

			// Instruction address 0x0000:0x01be, size: 5
			this.parent.CommonTools.InitSoundEngine();

			// Instruction address 0x0000:0x01c3, size: 5
			this.parent.CommonTools.StartGameMainTimer();

			// Instruction address 0x0000:0x01c8, size: 5
			this.parent.GameTools.F0_2dc4_0042_Randomize();

			// Instruction address 0x0000:0x01e0, size: 5
			this.parent.DrawTools.FillRectangle(this.parent.Var_aa_Screen0_Rectangle, 0, 0, 320, 200, 0);

			// Instruction address 0x0000:0x0223, size: 5
			this.parent.ImageTools.F0_2fa1_01a2_LoadBitmapOrPalette(1, 0, 0, "LOGO.PIC", out introPalette);

			// Instruction address 0x0000:0x023e, size: 5
			int logoBitmapID = this.parent.Graphics.F0_VGA_0b85_ScreenToBitmap(1, 0, 64, 320, 80);

			// Instruction address 0x0000:0x0256, size: 5
			this.parent.ImageTools.F0_2fa1_01a2_LoadBitmapOrPalette(1, 0, 0, "BIRTH0.PIC", 1);

			// Instruction address 0x0000:0x027c, size: 5
			//this.oParent.Graphics.F0_VGA_07d8_DrawImage(this.oParent.Var_19d4_Screen1_Rectangle, 0, 76, 320, 24, this.oParent.Var_19d4_Screen1_Rectangle, 0, 176);

			// Instruction address 0x0000:0x028c, size: 5
			this.parent.ImageTools.F0_2fa1_01a2_LoadBitmapOrPalette(2, 0, 0, "BIRTH1.PIC", 1);

			//this.oParent.ImageTools.F0_2fa1_01a2_LoadBitmapOrPalette(-1, 0, 0, "LOGO.PIC", 1);
			this.parent.Graphics.F0_VGA_0162_SetColorsFromColorStruct(introPalette);

			// Instruction address 0x0000:0x02f4, size: 5
			this.parent.CommonTools.AddPaletteCycleSlot(1, 14, 146, 152);

			this.parent.CommonTools.StartPaletteCycleSlot(1);

			this.parent.GameData.GameSettingFlags.Sound = true;

			MouseEvent mouseEvent = this.parent.GetMouseEvent();

			// Instruction address 0x0000:0x0329, size: 5
			using (StreamReader reader = new($"{this.parent.ResourcePath}CREDITS.TXT"))
			{
				// Instruction address 0x0000:0x0334, size: 5
				this.parent.CommonTools.ResetWaitTimer();

				// Instruction address 0x0000:0x033d, size: 5
				this.parent.CommonTools.PlayTune(3, 0);

				this.parent.Var_aa_Screen0_Rectangle.FontID = 5;
				string? introText = "";

				for (int i = 0; i < 320; i++)
				{
					switch (i)
					{
						case 10:
						case 70:
						case 120:
						case 170:
						case 190:
						case 210:
						case 230:
						case 250:
						case 270:
						case 290:
						case 310:
							introText = reader.ReadLine();
							break;

						case 50:
						case 160:
							introText = "";
							break;
					}

					GSize introTextSize = new(0, 0);

					if (!string.IsNullOrEmpty(introText))
					{
						introTextSize = this.parent.Graphics.GetDrawStringSize(this.parent.Var_aa_Screen0_Rectangle.FontID, introText);
					}

					this.parent.DrawTools.FillRectangle(this.parent.Var_aa_Screen0_Rectangle, 0, 0, 320 - i, 200, 0);
					this.parent.Graphics.F0_VGA_07d8_DrawImage(this.parent.Var_19d4_Screen1_Rectangle, 0, 0, i, 200, this.parent.Var_aa_Screen0_Rectangle, 320 - i, 0);

					if (!string.IsNullOrEmpty(introText))
					{
						int x = (320 - introTextSize.Width) / 2;
						int y = (200 - introTextSize.Height) / 2;

						// Instruction address 0x0000:0x0362, size: 5
						this.parent.DrawTools.F0_1182_005c_DrawStringToScreen0(introText, x, y + 2, 252);

						// Instruction address 0x0000:0x0386, size: 5
						this.parent.DrawTools.F0_1182_005c_DrawStringToScreen0(introText, x, y, 248);

						// Instruction address 0x0000:0x03aa, size: 5
						this.parent.DrawTools.F0_1182_005c_DrawStringToScreen0(introText, x, y + 1, 250);
					}

					while ((21 * i) / 4 > this.parent.CommonTools.SoundEngineTimer() && this.parent.CAPI.kbhit() == 0) ;

					mouseEvent = this.parent.GetMouseEvent();

					// Instruction address 0x0000:0x04c1, size: 5
					if (this.parent.CAPI.kbhit() != 0 || mouseEvent.Buttons != MouseButtonsEnum.None)
					{
						break;
					}
				}

				if (this.parent.CAPI.kbhit() == 0  && mouseEvent.Buttons == MouseButtonsEnum.None)
				{
					introText = "";

					for (int i = 0; i < 320; i++)
					{
						switch (i)
						{
							case 10:
							case 30:
							case 64:
							case 84:
							case 130:
							case 150:
							case 170:
							case 190:
							case 210:
							case 230:
							case 250:
							case 270:
							case 290:
								introText = reader.ReadLine();
								break;

							case 50:
							case 116:
							case 310:
								introText = "";
								break;
						}

						GSize introTextSize = new(0, 0);

						if (!string.IsNullOrEmpty(introText))
						{
							introTextSize = this.parent.Graphics.GetDrawStringSize(this.parent.Var_aa_Screen0_Rectangle.FontID, introText);
						}

						this.parent.Graphics.F0_VGA_07d8_DrawImage(this.parent.Var_19d4_Screen1_Rectangle, i, 0, 320 - i, 200, this.parent.Var_aa_Screen0_Rectangle, 0, 0);
						this.parent.Graphics.F0_VGA_07d8_DrawImage(this.parent.Var_19e8_Screen2_Rectangle, 0, 0, i, 200, this.parent.Var_aa_Screen0_Rectangle, 320 - i, 0);

						if (!string.IsNullOrEmpty(introText))
						{
							int x = (320 - introTextSize.Width) / 2;
							int y = (200 - introTextSize.Height) / 2;

							// Instruction address 0x0000:0x0362, size: 5
							this.parent.DrawTools.F0_1182_005c_DrawStringToScreen0(introText, x, y + 2, 252);

							// Instruction address 0x0000:0x0386, size: 5
							this.parent.DrawTools.F0_1182_005c_DrawStringToScreen0(introText, x, y, 248);

							// Instruction address 0x0000:0x03aa, size: 5
							this.parent.DrawTools.F0_1182_005c_DrawStringToScreen0(introText, x, y + 1, 250);
						}

						while ((21 * i) / 4 + 1680 > this.parent.CommonTools.SoundEngineTimer() && this.parent.CAPI.kbhit() == 0) ;

						mouseEvent = this.parent.GetMouseEvent();

						if (this.parent.CAPI.kbhit() != 0 || mouseEvent.Buttons != MouseButtonsEnum.None)
						{
							break;
						}
					}
				}
			}

			this.parent.Var_6b32_SelectedGameType = -2;

			if (this.parent.CAPI.kbhit() != 0 || mouseEvent.Buttons != MouseButtonsEnum.None)
			{
				this.parent.Var_6b32_SelectedGameType = -1;

				if (this.parent.CAPI.kbhit() != 0)
				{
					// Instruction address 0x0000:0x09ec, size: 5
					switch (this.parent.MenuBoxDialog.F0_2d05_0ac9_GetNavigationKey())
					{
						case 'N':
						case 'n':
							this.parent.Var_6b32_SelectedGameType = 0;
							break;

						case 'C':
						case 'c':
							this.parent.Var_6b32_SelectedGameType = 3;
							break;

						case 'E':
						case 'e':
							this.parent.Var_6b32_SelectedGameType = 2;
							break;

						case 'L':
						case 'l':
							this.parent.Var_6b32_SelectedGameType = 1;
							break;
					}
				}
			}

			if (this.parent.Var_6b32_SelectedGameType == -2)
			{
				// Instruction address 0x0000:0x0a19, size: 5
				while (this.parent.CommonTools.SoundEngineTimer() < 3463 && this.parent.CAPI.kbhit() == 0 && mouseEvent.Buttons == MouseButtonsEnum.None) { }

				// !!! Assuming that passing tune 0 stops all music
				this.parent.CommonTools.PlayTune(0, 0);

				// Instruction address 0x0000:0x0a6b, size: 5
				this.parent.Graphics.F0_VGA_07d8_DrawImage(this.parent.Var_19e8_Screen2_Rectangle, 0, 0, 320, 200, this.parent.Var_19d4_Screen1_Rectangle, 0, 0);

				// Instruction address 0x0000:0x0a81, size: 5
				this.parent.Graphics.F0_VGA_0c3e_DrawBitmapToScreen(this.parent.Var_19d4_Screen1_Rectangle, 0, 64, logoBitmapID);

				// Instruction address 0x0000:0x0aa0, size: 5
				this.parent.CommonTools.AddPaletteCycleSlot(4, 10, 224, 239);

				// Instruction address 0x0000:0x0aac, size: 5
				this.parent.CommonTools.StartPaletteCycleSlot(4);

				for (int i = 0; i < 320; i += 4)
				{
					// Instruction address 0x0000:0x0ad7, size: 5
					this.parent.Graphics.F0_VGA_07d8_DrawImage(this.parent.Var_19d4_Screen1_Rectangle, i, 64, 4, 80, this.parent.Var_aa_Screen0_Rectangle, i, 64);

					// Instruction address 0x0000:0x0ae3, size: 5
					this.parent.CommonTools.WaitTimer(1);
				}

				for (int i = 0; i < 100 && this.parent.CAPI.kbhit() == 0; i++)
				{
					// Instruction address 0x0000:0x0b01, size: 5
					this.parent.CommonTools.WaitTimer(5);
				}

				// Instruction address 0x0000:0x0b1b, size: 5
				this.parent.CommonTools.EmptyKeyboardBufferAndClearMouse();

				this.parent.Var_6b32_SelectedGameType = -1;
			}

			for (int i = 1; i < 5; i++)
			{
				// Instruction address 0x0000:0x0b40, size: 5
				this.parent.CommonTools.StopPaletteCycleSlot(i);
			}

			// Instruction address 0x0000:0x0b65, size: 5
			this.parent.DrawTools.FillRectangle(this.parent.Var_19d4_Screen1_Rectangle, 0, 0, 320, 200, 0);

			// Instruction address 0x0000:0x0b7b, size: 5
			this.parent.Graphics.F0_VGA_0c3e_DrawBitmapToScreen(this.parent.Var_19d4_Screen1_Rectangle, 0, 64, logoBitmapID);

#if DEBUG
			this.parent.Graphics.F0_VGA_06b7_DrawScreenToMainScreen(1);
#else
			this.oParent.Graphics.F0_VGA_06b7_DrawScreenToMainScreenWithEffect(1);
#endif

			// Instruction address 0x0000:0x0bab, size: 5
			this.parent.Graphics.F0_VGA_07d8_DrawImage(this.parent.Var_19d4_Screen1_Rectangle, 0, 0, 320, 200, this.parent.Var_aa_Screen0_Rectangle, 0, 0);

			// Instruction address 0x0000:0x0bb6, size: 5
			this.parent.CommonTools.F0_1000_0846(0);

			this.parent.Var_aa_Screen0_Rectangle.FontID = 1;

			// Instruction address 0x0000:0x0bca, size: 5
			this.parent.GameTools.F0_2dc4_0523_FreeResource(logoBitmapID, "ShowMainIntro");
		}

		/// <summary>
		/// End Game animation when the player wins the game by Space Victory
		/// </summary>
		/// <param name="playerID"></param>
		public void F2_0000_0bd7_EndGame_SpaceVictory(int playerID)
		{
			//this.oCPU.Log.EnterBlock($"F2_0000_0bd7({playerID})");

			// function body
			int[] bitmaps = new int[24];

			this.parent.ImageTools.F0_2fa1_01a2_LoadBitmapOrPalette(1, 0, 0, "PLANET2.PIC", 0);

			// Load animation Alpha Centauri bitmaps
			for (int i = 0; i < 24; i++)
			{
				bitmaps[i] = this.parent.Graphics.F0_VGA_0b85_ScreenToBitmap(1, ((i % 6) * 50) + 1, ((i / 6) * 50) + 1, 49, 49);
			}

			this.parent.GameTools.F0_2dc4_065f_StopPaletteCycleSlots();
			this.parent.DrawTools.FillRectangle(this.parent.Var_aa_Screen0_Rectangle, 0, 0, 320, 200, 236);
			this.parent.ImageTools.F0_2fa1_01a2_LoadBitmapOrPalette(1, 0, 0, "PLANET1.PIC", 1);
			this.parent.Graphics.F0_VGA_07d8_DrawImage(this.parent.Var_19d4_Screen1_Rectangle, 0, 24, 320, 176, this.parent.Var_19d4_Screen1_Rectangle, 0, 0);
			this.parent.Graphics.F0_VGA_07d8_DrawImage(this.parent.Var_19d4_Screen1_Rectangle, 0, 32, 320, 24, this.parent.Var_19d4_Screen1_Rectangle, 0, 176);

			this.parent.DrawTools.FillRectangle(this.parent.Var_aa_Screen0_Rectangle, 0, 0, 320, 200, 236);
			this.parent.DrawTools.FillRectangle(this.parent.Var_aa_Screen0_Rectangle, 0, 12, 320, 176, 236);

			// Load spaceship text narrative
			this.parent.Array_30b8[0] = $"{this.parent.Array_19b2_GovernmentTypeNames[this.parent.GameData.Players[this.parent.GameData.HumanPlayerID].GovernmentType]} " +
				$"{this.parent.GameData.Players[this.parent.GameData.HumanPlayerID].Name}";
			this.parent.Array_30b8[1] = this.parent.GameData.Players[playerID].Nationality;
			this.parent.Array_30b8[2] = $"{this.parent.GameData.Players[playerID].SpaceshipPopulation}";
			this.parent.Array_30b8[3] = $"{this.parent.GameData.Players[playerID].SpaceshipLaunchYear}";
			this.parent.Array_30b8[4] = $"{this.parent.GameData.Year}";

			string textContent = this.parent.LanguageTools.F0_2f4d_0471_ReplaceKeywords(this.parent.LanguageTools.F0_2f4d_01ad_GetTextBySectionAndKey("KING",
				((playerID == this.parent.GameData.HumanPlayerID) ? "*SPACE" : "*SPACE2")));

			string[] content = textContent.Replace("\n", "").Split('^');

			this.parent.Var_aa_Screen0_Rectangle.FontID = 6;

			// Animate starship travel to Alpha Centauri
			int contentIndex = 0;
			int bitmapID = 0;
			int local_242 = -10;

			this.parent.CommonTools.PlayTune(3, 0);

			for (int i = 0; i < 320; i++)
			{
				this.parent.CommonTools.ResetWaitTimer();

				if (i < 320)
				{
					this.parent.DrawTools.FillRectangle(this.parent.Var_19d4_Screen1_Rectangle, 0, 176, 320 - i, 24, 236);
				}

				if (i != 0)
				{
					this.parent.Graphics.F0_VGA_07d8_DrawImage(this.parent.Var_19d4_Screen1_Rectangle, 0, 20, i, 24,
						this.parent.Var_19d4_Screen1_Rectangle, 320 - i, 176);
				}

				if ((i % 22) == 0 && contentIndex < content.Length)
				{
					contentIndex++;
				}

				int oldScreenID = this.parent.Var_aa_Screen0_Rectangle.ScreenID;

				this.parent.Var_aa_Screen0_Rectangle.ScreenID = 1;

				this.parent.DrawTools.F0_1182_00b3_DrawCenteredStringToScreen0(content[contentIndex], 160, 184, 56);
				this.parent.DrawTools.F0_1182_00b3_DrawCenteredStringToScreen0(content[contentIndex], 160, 183, 60);

				this.parent.Var_aa_Screen0_Rectangle.ScreenID = oldScreenID;

				bitmapID++;

				if (bitmapID >= 23)
				{
					bitmapID = 0;
				}

				this.parent.Graphics.F0_VGA_0c3e_DrawBitmapToScreen(this.parent.Var_19d4_Screen1_Rectangle, 135, 51, bitmaps[bitmapID]);

				if (i < 320)
				{
					this.parent.DrawTools.FillRectangle(this.parent.Var_aa_Screen0_Rectangle, 0, 12, 319, 20, 236);
				}

				if (i != 0)
				{
					this.parent.Graphics.F0_VGA_07d8_DrawImage(this.parent.Var_19d4_Screen1_Rectangle, 0, 0, i, 20,
						this.parent.Var_aa_Screen0_Rectangle, 320 - i, 12);
				}

				this.parent.Graphics.F0_VGA_07d8_DrawImage(this.parent.Var_19d4_Screen1_Rectangle, 0, 176, 320, 24, this.parent.Var_aa_Screen0_Rectangle, 0, 32);

				if (i < 320)
				{
					this.parent.DrawTools.FillRectangle(this.parent.Var_aa_Screen0_Rectangle, 0, 56, 320 - i, 132, 236);
				}

				if (i != 0)
				{
					this.parent.Graphics.F0_VGA_07d8_DrawImage(this.parent.Var_19d4_Screen1_Rectangle, 0, 44, i, 132,
						this.parent.Var_aa_Screen0_Rectangle, 320 - i, 56);
				}

				if ((i & 0x1) != 0)
				{
					local_242++;

					this.parent.Graphics.F0_VGA_0550_SetPixel(this.parent.Var_aa_Screen0_Rectangle.ScreenID, local_242 - i, 92, 15);
				}

				local_242++;

				if (this.parent.Var_5c_TickCount > 12 && (i & 0x1) == 0)
				{
					i++;

					this.CPU.DoEvents();
				}

				while (this.parent.Var_5c_TickCount < 8)
				{
					this.CPU.DoEvents();
				}
			}

			for (int i = 0; i < 24; i++)
			{
				this.parent.GameTools.F0_2dc4_0523_FreeResource(bitmaps[i], "EndGame_SpaceVictory");
			}

			// Show static narrative text animation
			this.parent.Var_aa_Screen0_Rectangle.ScreenID = 0;

			this.parent.CommonTools.F0_1000_0846(0);

			if (playerID == this.parent.GameData.HumanPlayerID)
			{
				this.parent.ImageTools.F0_2fa1_01a2_LoadBitmapOrPalette(1, 0, 0, "SPACEST.PIC", 0);

				this.parent.CommonTools.PlayTune(1, 0);

				this.parent.CommonTools.TransformPaletteToColor(5, Color.FromRgb(0, 0, 0));

				this.parent.DrawTools.FillRectangle(this.parent.Var_aa_Screen0_Rectangle, 0, 0, 320, 200, 0);

				this.parent.ImageTools.F0_2fa1_01a2_LoadBitmapOrPalette(-1, 0, 0, "SPACEST.PIC", 1);

				this.parent.CommonTools.PlayTune(34, 0);

				this.parent.Graphics.F0_VGA_07d8_DrawImage(this.parent.Var_19d4_Screen1_Rectangle, 0, 0, 320, 200, this.parent.Var_aa_Screen0_Rectangle, 0, 0);

				this.parent.CommonTools.AddPaletteCycleSlot(1, 15, 16, 31);
				this.parent.CommonTools.AddPaletteCycleSlot(2, 100, 32, 34);
				this.parent.CommonTools.AddPaletteCycleSlot(3, 42, 35, 40);
				this.parent.CommonTools.AddPaletteCycleSlot(4, 300, 41, 47);
				this.parent.CommonTools.AddPaletteCycleSlot(5, 60, 48, 53);
				this.parent.CommonTools.AddPaletteCycleSlot(6, 37, 54, 63);

				for (int i = 1; i < 7; i++)
				{
					this.parent.CommonTools.StartPaletteCycleSlot(i);
				}
			}
			else
			{
				this.parent.Palace.F17_0000_07ec(1);

				this.parent.Graphics.F0_VGA_07d8_DrawImage(this.parent.Var_19d4_Screen1_Rectangle, 0, 0, 320, 200, this.parent.Var_aa_Screen0_Rectangle, 0, 0);
			}

			for (; contentIndex < content.Length; contentIndex++)
			{
				this.parent.Graphics.F0_VGA_07d8_DrawImage(this.parent.Var_19d4_Screen1_Rectangle, 0, 0, 320, 32, this.parent.Var_aa_Screen0_Rectangle, 0, 0);

				this.parent.DrawTools.F0_1182_00b3_DrawCenteredStringToScreen0(content[contentIndex], 160, 5, 11);
				this.parent.DrawTools.F0_1182_00b3_DrawCenteredStringToScreen0(content[contentIndex], 160, 4, 15);

				this.parent.Graphics.F0_VGA_0550_SetPixel(this.parent.Var_aa_Screen0_Rectangle.ScreenID, 280, 32, 15);

				this.parent.CommonTools.WaitTimer(80);

				this.parent.Graphics.F0_VGA_0550_SetPixel(this.parent.Var_aa_Screen0_Rectangle.ScreenID, 280, 32, 0);

				this.parent.CommonTools.WaitTimer(80);
			}

			this.parent.Segment_2459.F0_2459_0918_WaitForKeyPressOrMouseClick();

			if (playerID == this.parent.GameData.HumanPlayerID)
			{
				for (int i = 1; i < 7; i++)
				{
					this.parent.CommonTools.StopPaletteCycleSlot(i);
				}
			}

			this.parent.Var_aa_Screen0_Rectangle.ScreenID = 0;

			this.parent.CommonTools.PlayTune(1, 0);

			this.parent.Var_aa_Screen0_Rectangle.FontID = 1;

			this.parent.DrawTools.FillRectangle(this.parent.Var_aa_Screen0_Rectangle, 0, 0, 320, 200, 0);

			this.parent.Segment_1238.F0_1238_1beb();

			this.parent.GameTools.F0_2dc4_05dd_AddPaletteCycleSlots();
		}

		/// <summary>
		/// End Game animation when the player lost the game
		/// </summary>
		public void F2_0000_152a_EndGame_PlayerLost()
		{
			//this.oCPU.Log.EnterBlock("F2_0000_152a()");

			// function body
			// Instruction address 0x0000:0x1530, size: 5
			this.parent.GameTools.F0_2dc4_065f_StopPaletteCycleSlots();

			// Instruction address 0x0000:0x155a, size: 5
			this.parent.DrawTools.FillRectangle(this.parent.Var_aa_Screen0_Rectangle, 0, 0, 320, 200, 0);

			// Instruction address 0x0000:0x1566, size: 5
			this.parent.CommonTools.PlayTune(35, 0);

			// Instruction address 0x0000:0x1576, size: 5
			this.parent.ImageTools.F0_2fa1_01a2_LoadBitmapOrPalette(1, 0, 0, "ARCH.PIC", 1);

			// Instruction address 0x0000:0x15a7, size: 5
			this.parent.Graphics.F0_VGA_07d8_DrawImage(this.parent.Var_19d4_Screen1_Rectangle, 0, 0, 320, 200, this.parent.Var_aa_Screen0_Rectangle, 0, 0);

			// Instruction address 0x0000:0x15d9, size: 5
			this.parent.Array_30b8[0] = this.parent.GameData.Players[this.parent.GameData.HumanPlayerID].Name.ToUpper();

			// Instruction address 0x0000:0x15e9, size: 5
			this.parent.Array_30b8[3] = "Irrigation";

			for (int i = 0; i < 72; i++)
			{
				if (this.parent.Segment_1ade.F0_1ade_22b5_PlayerHasTechnology(this.parent.GameData.HumanPlayerID, (TechnologyAdvanceEnum)i))
				{
					// Instruction address 0x0000:0x1617, size: 5
					this.parent.Array_30b8[3] = this.parent.GameData.TechnologyAdvances[i].Name;
				}
			}

			// Instruction address 0x0000:0x1645, size: 5
			string gameEndText = this.parent.LanguageTools.F0_2f4d_0471_ReplaceKeywords(this.parent.LanguageTools.F0_2f4d_01ad_GetTextBySectionAndKey("KING", "*ARCH")).Replace("\n", "");

			string[] gameEndBlock = gameEndText.Split('^');

			this.parent.Var_aa_Screen0_Rectangle.FontID = 6;

			for (int i = 0; i < gameEndBlock.Length; i++)
			{
				string textLine = gameEndBlock[i];

				// Instruction address 0x0000:0x16c3, size: 5
				this.parent.Graphics.F0_VGA_07d8_DrawImage(this.parent.Var_19d4_Screen1_Rectangle, 0, 6, 320, 20, this.parent.Var_aa_Screen0_Rectangle, 0, 6);

				// Instruction address 0x0000:0x16e5, size: 5
				this.parent.DrawTools.F0_1182_00b3_DrawCenteredStringToScreen0(textLine, 160, 7, 15);

				// Instruction address 0x0000:0x1700, size: 5
				this.parent.DrawTools.F0_1182_00b3_DrawCenteredStringToScreen0(textLine, 160, 9, 13);

				// Instruction address 0x0000:0x173b, size: 5
				this.parent.DrawTools.F0_1182_00b3_DrawCenteredStringToScreen0(textLine, 160, 8, 14);

				if (this.parent.CAPI.kbhit() != 0)
					break;

				if (i < gameEndBlock.Length - 1)
					this.parent.CommonTools.WaitTimer(200);
			}

			// Instruction address 0x0000:0x1771, size: 5
			this.parent.CommonTools.PlayTune(1, 0);

			// Instruction address 0x0000:0x178c, size: 5
			this.parent.CommonTools.PlayTune(
				this.parent.GameData.Nations[this.parent.GameData.Players[this.parent.GameData.HumanPlayerID].NationalityID].ShortTune, 0);

			// Instruction address 0x0000:0x1798, size: 5
			this.parent.CommonTools.WaitTimer(300);

			// Instruction address 0x0000:0x17a4, size: 5
			this.parent.CommonTools.PlayTune(1, 0);

			// Instruction address 0x0000:0x17bf, size: 5
			this.parent.DrawTools.FillRectangle(this.parent.Var_aa_Screen0_Rectangle, 0, 0, 320, 200, 0);

			// Instruction address 0x0000:0x17c7, size: 5
			this.parent.Segment_1238.F0_1238_1beb();

			this.parent.Var_aa_Screen0_Rectangle.FontID = 1;
		}

		/// <summary>
		/// Shows Instant Advice elements on the screen
		/// </summary>
		public void F2_0000_17d9_ShowInstantAdviceElements()
		{
			//this.oCPU.Log.EnterBlock("F2_0000_17d9()");

			// function body
			int startX = this.parent.Var_d4cc_MapViewX + 7;
			int startY = this.parent.Var_d75e_MapViewY + 6;

			for (int i = 1; i < 9; i++)
			{
				GPoint direction = this.parent.MoveDirections[i];

				int x = this.parent.MapManagement.AdjustXPosition(startX + direction.X);
				int y = startY + direction.Y;

				TerrainTypeEnum terrainType = this.parent.MapManagement.GetTerrainType(x, y);
				string adviceText = "";

				if (this.parent.MapManagement.F0_2aea_1894_CellHasMinorTribeHut(x, y, terrainType))
				{
					adviceText = "Village\n";
				}
				else if (!this.parent.MapManagement.F0_2aea_1836_CellHasSpecialResource(x, y))
				{
					adviceText = this.parent.GameData.Terrains[(int)terrainType].Name;
				}
				else
				{
					adviceText = this.parent.GameData.Terrains[12 + (int)terrainType].Name;
				}

				direction = this.parent.MoveDirections[i];

				this.parent.Graphics.F0_VGA_0599_DrawLine(this.parent.Var_aa_Screen0_Rectangle,
					200 + (direction.X * 16), 112 + (direction.Y * 16), 170 + (direction.X * 64), 100 + (direction.Y * 48) + 6, 15);
				this.parent.Var_db38 = 1;
				this.parent.MenuBoxDialog.F0_2d05_0031_ShowMenuBox(adviceText, 170 + (direction.X * 64), 100 + (direction.Y * 48), true, false, true);
			}

			this.parent.Graphics.F0_VGA_0599_DrawLine(this.parent.Var_aa_Screen0_Rectangle, 160, 6, 200, 16 + 6, 15);
			this.parent.Var_db38 = 1;
			this.parent.MenuBoxDialog.F0_2d05_0031_ShowMenuBox("Menu Bar\n", 200, 16, true, false, true);

			this.parent.Graphics.F0_VGA_0599_DrawLine(this.parent.Var_aa_Screen0_Rectangle, 48, 32, 88, 24 + 6, 15);
			this.parent.Var_db38 = 1;
			this.parent.MenuBoxDialog.F0_2d05_0031_ShowMenuBox("Map Window\n", 88, 24, true, false, true);

			this.parent.Graphics.F0_VGA_0599_DrawLine(this.parent.Var_aa_Screen0_Rectangle, 40, 128, 88, 170 + 6, 15);
			this.parent.Var_db38 = 1;
			this.parent.MenuBoxDialog.F0_2d05_0031_ShowMenuBox("Active Unit\n", 88, 170, true, false, true);

			this.parent.Segment_2459.F0_2459_0918_WaitForKeyPressOrMouseClick();
		}
	}
}
