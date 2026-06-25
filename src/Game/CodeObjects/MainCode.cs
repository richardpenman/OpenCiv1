using Avalonia.Media;
using IRB.VirtualCPU;

namespace OpenCivOne
{
	public class MainCode
	{
		private OpenCivOneGame parent;

		//private int Var_652e_MouseHideCountTemp = 0;
		//private int Var_deea_MouseHideCount = 0;

		public MainCode(OpenCivOneGame parent)
		{
			this.parent = parent;
		}

		/// <summary>
		/// Main game entry
		/// </summary>
		public void F0_11a8_0008_Main()
		{
			// this.oCPU.Log.EnterBlock("F0_11a8_0008_Main()");

			// function body

			// Main menu selection
			// 'N' - No sound, 'A' - Sound blaster, 'R' - Roland MIDI board
			this.parent.Var_1a30_SoundDriverType = 'N';

			this.parent.MainIntro.F2_0000_0000_ShowMainGameIntro();

			// Instruction address 0x11a8:0x010b, size: 5
			this.parent.Var_6e92_MouseIconHandle = this.parent.Graphics.LoadIcon("TORCH.PIC");

			// Instruction address 0x11a8:0x0139, size: 5
			this.parent.CommonTools.SetMousePositionAndIcon(0, 0, this.parent.Var_6e92_MouseIconHandle);
			
			// Game type, load, etc. menu
			// And then after menu Intro
			// Instruction address 0x11a8:0x0146, size: 3
			F0_11a8_0486_LogoAndMainGameMenu();

			// Start Game menu, level, tribe, name
			// Instruction address 0x11a8:0x014a, size: 3
			F0_11a8_087c_NewGameMenu();

			if (this.parent.Var_6b32_SelectedGameType == 1)
			{
				this.parent.Var_d4cc_MapViewX = this.parent.GameData.Players[this.parent.GameData.HumanPlayerID].XStart - 7;
				this.parent.Var_d75e_MapViewY = 19;

				// Instruction address 0x11a8:0x016a, size: 5
				this.parent.Segment_1238.F0_1238_1b44();
			}

			this.parent.Var_dc48_GameEndType = 0;

			while (this.parent.Var_dc48_GameEndType == 0)
			{
				// Instruction address 0x11a8:0x0175, size: 5
				this.parent.Segment_1238.F0_1238_0092_GameTurn();
			}
		
			// Instruction address 0x11a8:0x0191, size: 3
			this.parent.CommonTools.PlayTune(0, 0);

			// Instruction address 0x11a8:0x0197, size: 5
			this.parent.CommonTools.CloseSoundEngine();

			// Instruction address 0x11a8:0x019c, size: 5
			this.parent.CommonTools.StopGameMainTimer();

			this.parent.GameTools.F0_2dc4_0523_FreeResource(this.parent.Var_6e92_MouseIconHandle, "Mouse icon");
		}

		/// <summary>
		/// ?
		/// </summary>
		public void F0_11a8_0486_LogoAndMainGameMenu()
		{
			//this.oCPU.Log.EnterBlock("F0_11a8_0486_LogoAndMainGameMenu()");

			// function body
			this.parent.Var_d76a_EarthMap = false;

			while (this.parent.Var_6b32_SelectedGameType == -1)
			{
				// Instruction address 0x11a8:0x04b0, size: 5
				this.parent.Var_6b32_SelectedGameType = this.parent.MenuBoxDialog.F0_2d05_0031_ShowMenuBox(" Start a New Game\n Load a Saved Game\n EARTH\n Customize World\n View Hall of Fame\n", 100, 140, true, false, true);

				int timerValue = 0;
				do
				{
					// Instruction address 0x11a8:0x04c2, size: 5
					timerValue = this.parent.CommonTools.SoundEngineTimer();

					if (timerValue <= 5269) break;

					timerValue -= 5269;
					timerValue %= 288;
				}
				while (timerValue != 0);

				// Instruction address 0x11a8:0x04e1, size: 3
				this.parent.CommonTools.PlayTune(1, 0);

				switch (this.parent.Var_6b32_SelectedGameType)
				{
					case 0:
						this.parent.Var_7ef6_PlanetLandMass = 1;
						this.parent.Var_7ef8_PlanetTemperature = 1;
						this.parent.Var_7efa_PlanetClimate = 1;
						this.parent.Var_7efc_PlanetAge = 1;

						// Intro...
						this.parent.MapInitAndIntro.F7_0000_0012_GenerateMap();

						this.parent.Var_aa_Screen0_Rectangle.ScreenID = 0;

						// Instruction address 0x11a8:0x053e, size: 5
						this.parent.CommonTools.F0_1000_0846(0);

						// Instruction address 0x11a8:0x054e, size: 5
						this.parent.ImageTools.F0_2fa1_01a2_LoadBitmapOrPalette(1, 0, 0, "SP299.PIC", 0);

						// Instruction address 0x11a8:0x0576, size: 5
						this.parent.Graphics.F0_VGA_07d8_DrawImage(this.parent.Var_19d4_Screen1_Rectangle, 160, 50, 160, 150, this.parent.Var_19e8_Screen2_Rectangle, 160, 50);
						break;

					case 1:
						if (this.parent.GameLoadAndSave.F11_0000_0000_LoadGameDialog() == -1)
						{
							this.parent.Var_6b32_SelectedGameType = -1;
						}
						else
						{
							// Instruction address 0x11a8:0x07e4, size: 5
							this.parent.DrawTools.FillRectangle(this.parent.Var_aa_Screen0_Rectangle, 0, 0, 320, 200, 7);
						}
						break;

					case 2:
						this.parent.Var_d76a_EarthMap = true;

						this.parent.MapInitAndIntro.F7_0000_0012_GenerateMap();

						this.parent.Var_aa_Screen0_Rectangle.ScreenID = 0;

						// Instruction address 0x11a8:0x07af, size: 5
						this.parent.CommonTools.F0_1000_0846(0);

						// Instruction address 0x11a8:0x054e, size: 5
						this.parent.ImageTools.F0_2fa1_01a2_LoadBitmapOrPalette(1, 0, 0, "SP299.PIC", 0);

						// Instruction address 0x11a8:0x0576, size: 5
						this.parent.Graphics.F0_VGA_07d8_DrawImage(this.parent.Var_19d4_Screen1_Rectangle, 160, 50, 160, 150, this.parent.Var_19e8_Screen2_Rectangle, 160, 50);

						break;

					case 3:
						// Instruction address 0x11a8:0x05b6, size: 5
						this.parent.CommonTools.TransformPaletteToColor(5, Color.FromRgb(0, 0, 0));

						// Instruction address 0x11a8:0x05d1, size: 5
						this.parent.DrawTools.FillRectangle(this.parent.Var_aa_Screen0_Rectangle, 0, 0, 320, 200, 0);

						// Instruction address 0x11a8:0x05e1, size: 5
						this.parent.ImageTools.F0_2fa1_01a2_LoadBitmapOrPalette(1, 0, 0, "CUSTOM.PIC", 1);

						// Instruction address 0x11a8:0x0611, size: 5
						this.parent.Graphics.F0_VGA_07d8_DrawImage(this.parent.Var_19d4_Screen1_Rectangle, 0, 0, 320, 200, this.parent.Var_aa_Screen0_Rectangle, 0, 0);

						this.parent.Var_2f9a_MenuBoxDefaultOptionIndex = 1;
						this.parent.Var_7ef6_PlanetLandMass = -1;

						while (this.parent.Var_7ef6_PlanetLandMass == -1)
						{
							// Instruction address 0x11a8:0x064f, size: 5
							this.parent.Var_7ef6_PlanetLandMass = this.parent.MenuBoxDialog.F0_2d05_0031_ShowMenuBox("LAND MASS:\n Small\n Normal\n Large\n", 200, 1, false, true, true);

							if (this.parent.Var_7ef6_PlanetLandMass != -1 && this.parent.Var_2f9c_MenuBoxHelpRequested)
							{
								this.parent.Help.F4_0000_0000_DisplayHelpContent("Land");

								this.parent.Var_2f9a_MenuBoxDefaultOptionIndex = this.parent.Var_7ef6_PlanetLandMass;
								this.parent.Var_7ef6_PlanetLandMass = -1;
							}
						}

						this.parent.Var_2f9a_MenuBoxDefaultOptionIndex = 1;
						this.parent.Var_7ef8_PlanetTemperature = -1;

						while (this.parent.Var_7ef8_PlanetTemperature == -1)
						{
							// Instruction address 0x11a8:0x06ac, size: 5
							this.parent.Var_7ef8_PlanetTemperature = this.parent.MenuBoxDialog.F0_2d05_0031_ShowMenuBox("TEMPERATURE:\n Cool\n Temperate\n Warm\n", 200, 51, false, true, true);

							if (this.parent.Var_7ef8_PlanetTemperature != -1 && this.parent.Var_2f9c_MenuBoxHelpRequested)
							{
								this.parent.Help.F4_0000_0000_DisplayHelpContent("Temperature");

								this.parent.Var_2f9a_MenuBoxDefaultOptionIndex = this.parent.Var_7ef8_PlanetTemperature;
								this.parent.Var_7ef8_PlanetTemperature = -1;
							}
						}

						this.parent.Var_2f9a_MenuBoxDefaultOptionIndex = 1;
						this.parent.Var_7efa_PlanetClimate = -1;

						while (this.parent.Var_7efa_PlanetClimate == -1)
						{
							// Instruction address 0x11a8:0x0709, size: 5
							this.parent.Var_7efa_PlanetClimate = this.parent.MenuBoxDialog.F0_2d05_0031_ShowMenuBox("CLIMATE:\n Arid\n Normal\n Wet\n", 200, 101, false, true, true);

							if (this.parent.Var_7efa_PlanetClimate != -1 && this.parent.Var_2f9c_MenuBoxHelpRequested)
							{
								this.parent.Help.F4_0000_0000_DisplayHelpContent("Climate");

								this.parent.Var_2f9a_MenuBoxDefaultOptionIndex = this.parent.Var_7efa_PlanetClimate;
								this.parent.Var_7efa_PlanetClimate = -1;
							}
						}

						this.parent.Var_2f9a_MenuBoxDefaultOptionIndex = 1;
						this.parent.Var_7efc_PlanetAge = -1;

						while (this.parent.Var_7efc_PlanetAge == -1)
						{
							// Instruction address 0x11a8:0x0766, size: 5
							this.parent.Var_7efc_PlanetAge = this.parent.MenuBoxDialog.F0_2d05_0031_ShowMenuBox("AGE:\n 3 billion years\n 4 billion years\n 5 billion years\n", 200, 151, false, true, true);

							if (this.parent.Var_7efc_PlanetAge != -1 && this.parent.Var_2f9c_MenuBoxHelpRequested)
							{
								this.parent.Help.F4_0000_0000_DisplayHelpContent("Age");

								this.parent.Var_2f9a_MenuBoxDefaultOptionIndex = this.parent.Var_7efc_PlanetAge;
								this.parent.Var_7efc_PlanetAge = -1;
							}
						}

						// Intro...
						this.parent.MapInitAndIntro.F7_0000_0012_GenerateMap();

						this.parent.Var_aa_Screen0_Rectangle.ScreenID = 0;

						// Instruction address 0x11a8:0x053e, size: 5
						this.parent.CommonTools.F0_1000_0846(0);

						// Instruction address 0x11a8:0x054e, size: 5
						this.parent.ImageTools.F0_2fa1_01a2_LoadBitmapOrPalette(1, 0, 0, "SP299.PIC", 0);

						// Instruction address 0x11a8:0x0576, size: 5
						this.parent.Graphics.F0_VGA_07d8_DrawImage(this.parent.Var_19d4_Screen1_Rectangle, 160, 50, 160, 150, this.parent.Var_19e8_Screen2_Rectangle, 160, 50);

						break;

					case 4:
						this.parent.HallOfFame.F3_0000_002b();

						this.parent.HallOfFame.F3_0000_00d7(-1);

						this.parent.Var_6b32_SelectedGameType = -1;
						break;

					default:
						this.parent.Var_6b32_SelectedGameType = -1;
						break;
				}
			}

			this.parent.StartGameMenu.F5_0000_1455_LoadBitmaps();

			this.parent.Var_2f98_PatternAvailable = true;

			if (this.parent.Var_6b32_SelectedGameType == 0 || this.parent.Var_6b32_SelectedGameType == 2 || this.parent.Var_6b32_SelectedGameType == 3)
			{
				// Instruction address 0x11a8:0x0840, size: 5
				this.parent.CommonTools.TransformPaletteToColor(5, Color.FromRgb(0, 0, 0));
			}

			// Instruction address 0x11a8:0x085b, size: 5
			this.parent.DrawTools.FillRectangle(this.parent.Var_aa_Screen0_Rectangle, 0, 0, 320, 200, 0);

			// Instruction address 0x11a8:0x0867, size: 5
			this.parent.ImageTools.F0_2fa1_01a2_LoadBitmapOrPalette(-1, 0, 0, "SP257.PIC", 1);

			// Instruction address 0x11a8:0x086f, size: 5
			this.parent.GameTools.F0_2dc4_05dd_AddPaletteCycleSlots();
		}

		/// <summary>
		/// ?
		/// </summary>
		public void F0_11a8_087c_NewGameMenu()
		{
			//this.oCPU.Log.EnterBlock("F0_11a8_087c_NewGameMenu()");

			// function body
			// Instruction address 0x11a8:0x0891, size: 5
			this.parent.CommonTools.SetMousePositionAndIcon(0, 0, this.parent.Array_d4ce[7]);

			if (this.parent.GameData.TurnCount == 0)
			{
				// Instruction address 0x11a8:0x08a8, size: 5
				this.parent.ImageTools.F0_2fa1_01a2_LoadBitmapOrPalette(1, 0, 0, "DIFFS.PIC", 1);

				this.parent.StartGameMenu.F5_0000_0000_InitNewGameData();
			}
		
			this.parent.StartGameMenu.F5_0000_1af6_LoadGovernmentImage();
		}
	}
}
