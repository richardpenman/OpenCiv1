using System.Text;

namespace OpenCivOne
{
	public class Menus
	{
		private OpenCivOneGame parent;

		// Local variables used exclusively inside this section
		private short Var_654a = 0;

		// public variables
		public int Var_d4ca_MenuShortcutKey = 0;

		public Menus(OpenCivOneGame parent)
		{
			this.parent = parent;
		}

		/// <summary>
		/// Draws the main menu items
		/// </summary>
		public void F0_1238_0fea_DrawMainMenu()
		{
			//this.oCPU.Log.EnterBlock("F0_1238_0fea()");

			// function body
			// Instruction address 0x1238:0x0ffd, size: 5
			this.parent.DrawTools.FillRectangle(this.parent.Var_aa_Screen0_Rectangle, 0, 0, 320, 8, 0);

			// Instruction address 0x1238:0x1015, size: 5
			this.parent.DrawTools.DrawStringToScreen0("G\x0087AME", 8, 1, 15);

			// Instruction address 0x1238:0x102d, size: 5
			this.parent.DrawTools.DrawStringToScreen0("O\x0087RDERS", 64, 1, 15);

			// Instruction address 0x1238:0x1045, size: 5
			this.parent.DrawTools.DrawStringToScreen0("A\x0087DVISORS", 128, 1, 15);

			// Instruction address 0x1238:0x105d, size: 5
			this.parent.DrawTools.DrawStringToScreen0("W\x0087ORLD", 192, 1, 15);

			// Instruction address 0x1238:0x1075, size: 5
			this.parent.DrawTools.DrawStringToScreen0("E\x0087NCYCLOP.", 240, 1, 15);
		}

		/// <summary>
		/// Shows and handles one of the five top menus: Game, Orders, Advisors, World and Encyclopedia
		/// </summary>
		/// <param name="playerID"></param>
		/// <param name="unitID"></param>
		/// <param name="menuIndex">Index of specific menu to show or -1 to select menu using current mouse X coordinate</param>
		public void F0_2c84_0000_ShowTopMenu(int playerID, int unitID, int menuIndex)
		{
			//this.oCPU.Log.EnterBlock($"F0_2c84_0000_ShowTopMenu({playerID}, {unitID}, {menuIndex})");

			// function body
			this.Var_d4ca_MenuShortcutKey = -1;
			this.Var_654a = 0;

			MouseEvent mouseEvent = this.parent.GetMouseEvent();

			if (menuIndex == -1)
			{
				// Instruction address 0x2c84:0x0026, size: 5
				menuIndex = this.parent.CommonTools.CheckValueRange(mouseEvent.Position.X / 60, 0, 4);
			}

			this.parent.Graphics.F0_VGA_07d8_DrawImage(this.parent.Var_aa_Screen0_Rectangle, 0, 0, 320, 200, this.parent.Var_19d4_Screen1_Rectangle, 0, 0);

			switch (menuIndex)
			{
				case 0:
					// Instruction address 0x2c84:0x005e, size: 3
					F0_2c84_00ad_GameMenu();
					break;

				case 1:
					// Instruction address 0x2c84:0x006a, size: 3
					F0_2c84_01d8_OrdersMenu(playerID, unitID);
					break;

				case 2:
					// Instruction address 0x2c84:0x0073, size: 3
					F0_2c84_0615_AdvisorsMenu();
					break;

				case 3:
					// Instruction address 0x2c84:0x0079, size: 3
					F0_2c84_06e4_ShowWorldMenu();
					break;

				case 4:
					// Instruction address 0x2c84:0x007f, size: 3
					F0_2c84_07af_EncyclopediaMenu();
					break;
			}

			if (this.Var_654a == 1)
			{
				// Instruction address 0x2c84:0x0094, size: 5
				this.parent.GameEngine.F0_1238_1b44_RedrawMapWithBlankingEffect();
			}

			if (this.Var_654a == 0)
			{
				this.parent.Graphics.F0_VGA_07d8_DrawImage(this.parent.Var_19d4_Screen1_Rectangle, 0, 0, 320, 200, this.parent.Var_aa_Screen0_Rectangle, 0, 0);
			}
		}

		/// <summary>
		/// Shows game menu and handles its options sub menu
		/// </summary>
		private void F0_2c84_00ad_GameMenu()
		{
			//this.oCPU.Log.EnterBlock("F0_2c84_00ad_GameMenu()");

			// function body
			// Enable Save game with zero turns
			if (this.parent.GameData.TurnCount == 0)
			{
				// Disable 'Save Game' option
				//this.oParent.Var_b276_MenuBoxDisabledOptions = 0x10;
			}

			// Instruction address 0x2c84:0x00f4, size: 5
			this.parent.Var_b276_MenuBoxDisabledOptions = ((this.parent.GameData.SpaceshipFlags & 0x100) != 0) ? 0 : 1 << 7;

			int selectedOption = this.parent.MenuBoxDialog.F0_2d05_0031_ShowMenuBox(
				$" Tax Rate\n Luxuries Rate\n FindCity\n Preferences\n Save Game\n REVOLUTION!\n \n View Replay\n Retire\n QUIT\n",
				16, 8, true, false, false);

			// Instruction address 0x2c84:0x00ff, size: 5
			this.parent.CommonTools.ClearKeyboardAndMouseEvents();

			switch (selectedOption)
			{
				case 0: // Tax Rate
					this.Var_d4ca_MenuShortcutKey = '=';
					break;

				case 1: // Luxuries
					this.Var_d4ca_MenuShortcutKey = '-';
					break;

				case 2: // Find City
					this.Var_d4ca_MenuShortcutKey = '?';
					break;

				// Preferences
				case 3:
					int index;

					do
					{
						// Write current flags to show as checked in options sub menu
						this.parent.Var_d7f2_MenuBoxCheckedOptions = this.parent.GameData.GameSettingFlags.Value;
						this.parent.Var_d7f2_MenuBoxCheckedOptions |= (this.parent.GlobalPreferences.Debug.DebugMode ? 1 << 9 : 0);
						this.parent.Var_d7f2_MenuBoxCheckedOptions |= (this.parent.GlobalPreferences.Debug.FastMode ? 1 << 10 : 0);
						this.parent.Var_d7f2_MenuBoxCheckedOptions |= (this.parent.GlobalPreferences.Debug.SaveEveryTurn ? 1 << 11 : 0);

						// Process options sub menu, return selected option index or -1 if selection was rejected
						index = this.parent.MenuBoxDialog.F0_2d05_0031_ShowMenuBox(
							"Game:\n Instant Advice\n AutoSave\n End of Turn\n Animations\n Sound\n Enemy Moves\n Encyclopedia Text\n Palace\n \n" +
							"\u008fDebugging (global):\n Enable debug mode\n Fast mode\n Save game every turn",
							24, 16, true, false, false);

						if (index == -1)
						{
							continue;
						}

						if (!this.parent.Var_2f9c_MenuBoxHelpRequested)
						{
							switch (index)
							{
								case 0:
								case 1:
								case 2:
								case 3:
								case 4:
								case 5:
								case 6:
								case 7:
									this.parent.GameData.GameSettingFlags.Value ^= (short)(1 << index);
									break;

								case 9:
									this.parent.GlobalPreferences.Debug.DebugMode ^= true;
									this.parent.RefreshScreenMap = true;
									break;

								case 10:
									this.parent.GlobalPreferences.Debug.FastMode ^= true;
									this.parent.RefreshScreenMap = true;
									break;

								case 11:
									this.parent.GlobalPreferences.Debug.SaveEveryTurn ^= true;
									this.parent.RefreshScreenMap = true;
									break;
							}
						}

						if (index != -1)
						{
							this.parent.Var_2f9a_MenuBoxDefaultOptionIndex = index;
						}
					}
					while (index != -1 || this.parent.Var_2f9c_MenuBoxHelpRequested);

					break;

				case 4:
					// Save Game
					this.Var_d4ca_MenuShortcutKey = 'S';
					break;

				case 5:
					// Revolution
					this.Var_d4ca_MenuShortcutKey = -2;
					break;

					// spacer
				case 6:
					break;

				case 7:
					// View replay
					if ((this.parent.GameData.SpaceshipFlags & 0x100) != 0)
					{
						this.parent.Replay.F9_0000_0000();
					}
					break;

				case 8:
					// Retire
					this.parent.Var_dc48_GameEndType = 2;
					this.Var_d4ca_MenuShortcutKey = 0x1000;
					break;

				case 9:
					// Quit
					this.parent.Var_dc48_GameEndType = 1;
					this.Var_d4ca_MenuShortcutKey = 0x1000;
					break;
			}
		}

		/// <summary>
		/// Shows orders menu
		/// </summary>
		/// <param name="playerID"></param>
		/// <param name="unitID"></param>
		private void F0_2c84_01d8_OrdersMenu(int playerID, int unitID)
		{
			//this.oCPU.Log.EnterBlock($"F0_2c84_01d8_OrdersMenu({playerID}, {unitID})");

			// function body
			if (unitID >= 0 && unitID < 128)
			{
				StringBuilder menuText = new();

				Unit unit = this.parent.GameData.Players[playerID].Units[unitID];

				// Instruction address 0x2c84:0x0221, size: 5
				TerrainImprovementFlagsEnum improvements = this.parent.MapManagement.F0_2aea_1585_GetVisibleTerrainImprovements(unit.Position.X, unit.Position.Y);

				// Instruction address 0x2c84:0x0232, size: 5
				TerrainTypeEnum terrainType = this.parent.MapManagement.GetTerrainType(unit.Position.X, unit.Position.Y);

				int orderCount = 0;
				char[] orders = new char[15];

				// Instruction address 0x2c84:0x0245, size: 5
				menuText.Append(" No Orders \x008fspace\n");
				orders[orderCount++] = ' ';

				// All orders are enabled by default
				this.parent.Var_b276_MenuBoxDisabledOptions = 0;

				if (unit.UnitType == UnitTypeEnum.Settler)
				{
					if (improvements.HasFlag(TerrainImprovementFlagsEnum.City))
					{
						// Instruction address 0x2c84:0x027a, size: 5
						menuText.Append(" Add to City \x008fb\n");
					}
					else
					{
						// Instruction address 0x2c84:0x027a, size: 5
						menuText.Append(" Found New City \x008fb\n");
					}

					orders[orderCount++] = 'b';

					if (!improvements.HasFlag(TerrainImprovementFlagsEnum.Road))
					{
						// Instruction address 0x2c84:0x029a, size: 5
						menuText.Append(" Build Road \x008fr\n");
						orders[orderCount++] = 'r';
					}
					else
					{
						// Instruction address 0x2c84:0x02bb, size: 5
						if (!improvements.HasFlag(TerrainImprovementFlagsEnum.RailRoad) && this.parent.Technology.F0_1ade_22b5_PlayerHasTechnology(playerID, TechnologyAdvanceEnum.Railroad))
						{
							// Instruction address 0x2c84:0x02cf, size: 5
							menuText.Append(" Build RailRoad \x008fr\n");
							orders[orderCount++] = 'r';
						}
					}

					if (!improvements.HasFlag(TerrainImprovementFlagsEnum.Irrigation))
					{
						if (this.parent.GameData.TerrainModifications[(int)terrainType].IrrigationEffect == -2)
						{
							// Instruction address 0x2c84:0x0301, size: 5
							menuText.Append(" Build Irrigation");

							// Instruction address 0x2c84:0x030f, size: 5
							if (!this.parent.MapManagement.CanIrrigateCell(unit.Position.X, unit.Position.Y))
							{
								// Disable 'Build Irrigation' option
								this.parent.Var_b276_MenuBoxDisabledOptions |= 0x1 << orderCount;
							}
						}
						else
						{
							if (this.parent.GameData.TerrainModifications[(int)terrainType].IrrigationEffect >= 0)
							{
								// Instruction address 0x2c84:0x0342, size: 5
								menuText.Append(" Change to ");

								// Instruction address 0x2c84:0x035d, size: 5
								menuText.Append(this.parent.GameData.Terrains[(int)this.parent.PixelValuesToTerrainTypes[this.parent.GameData.TerrainModifications[(int)terrainType].IrrigationEffect]].Name);
							}
						}

						if (this.parent.GameData.TerrainModifications[(int)terrainType].IrrigationEffect != -1)
						{
							// Instruction address 0x2c84:0x0386, size: 5
							menuText.Append(" \x008fi\n");
							orders[orderCount++] = 'i';
						}
					}

					if (!improvements.HasFlag(TerrainImprovementFlagsEnum.Mines))
					{
						if (this.parent.GameData.TerrainModifications[(int)terrainType].MiningEffect <= -2)
						{
							// Instruction address 0x2c84:0x03dc, size: 5
							menuText.Append(" Build Mines");
						}
						else if (this.parent.GameData.TerrainModifications[(int)terrainType].MiningEffect >= 0)
						{
							// Instruction address 0x2c84:0x03c1, size: 5
							menuText.Append(" Change to ");

							// Instruction address 0x2c84:0x03dc, size: 5
							menuText.Append(this.parent.GameData.Terrains[(int)this.parent.PixelValuesToTerrainTypes[this.parent.GameData.TerrainModifications[(int)terrainType].MiningEffect]].Name);
						}

						if (this.parent.GameData.TerrainModifications[(int)terrainType].MiningEffect != -1)
						{
							// Instruction address 0x2c84:0x0405, size: 5
							menuText.Append(" \x008fm\n");
							orders[orderCount++] = 'm';
						}
					}

					if (improvements.HasFlag(TerrainImprovementFlagsEnum.Pollution))
					{
						// Instruction address 0x2c84:0x041b, size: 5
						menuText.Append(" Clean up Pollution \x008fp\n");
						orders[orderCount++] = 'p';
					}
				}

				if (unit.UnitType == UnitTypeEnum.Settler)
				{
					// Instruction address 0x2c84:0x044c, size: 5
					menuText.Append(" Build Fortress \x008ff\n");

					// Instruction address 0x2c84:0x045b, size: 5
					if (!this.parent.Technology.F0_1ade_22b5_PlayerHasTechnology(playerID, TechnologyAdvanceEnum.Construction))
					{
						// Disable 'Build Fortress' option
						this.parent.Var_b276_MenuBoxDisabledOptions |= 0x1 << orderCount;
					}
					orders[orderCount++] = 'f';
				}
				else if (this.parent.GameData.Units[(int)unit.UnitType].MovementType == UnitMovementTypeEnum.Land)
				{
					// Instruction address 0x2c84:0x049c, size: 5
					menuText.Append(" Fortify \x008ff\n");
					orders[orderCount++] = 'f';
				}

				// Instruction address 0x2c84:0x04d5, size: 5
				menuText.Append(" Wait \x008fw\n Sentry \x008fs\n GoTo\n");

				orders[orderCount++] = 'w';
				orders[orderCount++] = 's';
				orders[orderCount++] = 'g';

				if (((ushort)improvements & (ushort)TerrainImprovementFlagsEnum.PillageMask) != 0 && 
					(unit.UnitType != UnitTypeEnum.Diplomat && unit.UnitType != UnitTypeEnum.Caravan) && 
					unit.UnitType != UnitTypeEnum.Fighter)
				{
					// Instruction address 0x2c84:0x0528, size: 5
					menuText.Append(" Pillage \x008fP\n");
					orders[orderCount++] = 'P';
				}

				if (improvements.HasFlag(TerrainImprovementFlagsEnum.City))
				{
					// Instruction address 0x2c84:0x0548, size: 5
					menuText.Append(" Home City \x008fh\n");
					orders[orderCount++] = 'h';
				}

				// Instruction address 0x2c84:0x05a4, size: 5
				if ((this.parent.GameData.Units[(int)unit.UnitType].UnitRoleType == UnitRoleTypeEnum.SeaTransport || unit.UnitType == UnitTypeEnum.Carrier) && unit.NextUnitID != -1)
				{
					// Instruction address 0x2c84:0x05a4, size: 5
					menuText.Append(" Unload \x008fu\n");
					orders[orderCount++] = 'u';
				}

				// Instruction address 0x2c84:0x05be, size: 5
				menuText.Append(" \n Disband Unit \x008fD\n");

				// Empty option line does not use hotkey
				orders[orderCount++] = '\0';
				orders[orderCount++] = 'D';

				// Instruction address 0x2c84:0x05e6, size: 5
				int selectedOrder = this.parent.MenuBoxDialog.F0_2d05_0031_ShowMenuBox(menuText.ToString(), 72, 8, true, false, false);

				if (selectedOrder < 0 || selectedOrder >= orderCount)
				{
					this.Var_d4ca_MenuShortcutKey = -1;
				}
				else
				{
					this.Var_d4ca_MenuShortcutKey = orders[selectedOrder];
				}
			}
		}

		/// <summary>
		/// Shows advisors menu
		/// </summary>
		private void F0_2c84_0615_AdvisorsMenu()
		{
			//this.oCPU.Log.EnterBlock("F0_2c84_0615_AdvisorsMenu()");

			// function body
			// Instruction address 0x2c84:0x0647, size: 5
			int selectedOption = this.parent.MenuBoxDialog.F0_2d05_0031_ShowMenuBox(" City Status (F1)\n Military Advisor (F2)\n Intelligence Advisor (F3)\n" +
				" Attitude Advisor (F4)\n Trade Advisor (F5)\n Science Advisor (F6)\n", 112, 8, true, false, false);

			this.parent.Graphics.F0_VGA_07d8_DrawImage(this.parent.Var_19d4_Screen1_Rectangle, 0, 0, 320, 200, this.parent.Var_aa_Screen0_Rectangle, 0, 0);

			Var_654a = -1;

			switch (selectedOption)
			{
				case 0: // City Status
					this.parent.AdvisorReports.CityStatus(this.parent.GameData.HumanPlayerID);
					break;

				case 1: // Military Advisor
					this.parent.AdvisorReports.MilitaryReport(this.parent.GameData.HumanPlayerID);
					break;

				case 2: // Intelligence Advisor
					this.parent.AdvisorReports.IntelligenceReport();
					break;

				case 3: // Attitude Advisor
					this.parent.AdvisorReports.AttitudeReport(this.parent.GameData.HumanPlayerID);
					break;

				case 4: // Trade Advisor
					this.parent.AdvisorReports.TradeReport(this.parent.GameData.HumanPlayerID);
					break;

				case 5: // Science Advisor
					this.parent.AdvisorReports.ScienceReport(this.parent.GameData.HumanPlayerID);
					break;
			}
		}

		/// <summary>
		/// Shows world menu
		/// </summary>
		private void F0_2c84_06e4_ShowWorldMenu()
		{
			//this.oCPU.Log.EnterBlock("F0_2c84_06e4_ShowWorldMenu()");

			// function body
			if ((this.parent.GameData.SpaceshipFlags & 0xfe00) == 0)
			{
				// Disable 'SpaceShips' option
				this.parent.Var_b276_MenuBoxDisabledOptions = 0x20;
			}

			// Instruction address 0x2c84:0x0724, size: 5
			int selectedOption = this.parent.MenuBoxDialog.F0_2d05_0031_ShowMenuBox(" Wonders of the World (F7)\n Top 5 Cities (F8)\n" +
				" Civilization Score (F9)\n World Map (F10)\n Demographics\n SpaceShips\n", 144, 8, true, false, false);

			this.parent.Graphics.F0_VGA_07d8_DrawImage(this.parent.Var_19d4_Screen1_Rectangle, 0, 0, 320, 200, this.parent.Var_aa_Screen0_Rectangle, 0, 0);

			this.Var_654a = -1;

			switch (selectedOption)
			{
				case 0: // Wonders of the World
					this.parent.Reports.F12_0000_080d_ShowWondersOfTheWorldReport();
					break;

				case 1: // Top 5 Cities
					this.parent.Reports.ShowTopFiveCitiesReport();
					break;

				case 2: // Civilization Score
					this.parent.CivilizationScore.ShowCivilizationScore(this.parent.GameData.HumanPlayerID, true);
					break;

				case 3: // World Map
					this.parent.Reports.F12_0000_0000_ShowWorldMapReport();
					break;

				case 4: // Demographics
					this.parent.Reports.F12_0000_0d6d_ShowsDemographicsReport(this.parent.GameData.HumanPlayerID);
					break;

				case 5: // SpaceShips
					this.parent.Spaceship.F18_0000_1527_ShowSpaceshipNationDialog();
					break;
			}
		}

		/// <summary>
		/// Shows Encyclopedia menu
		/// </summary>
		private void F0_2c84_07af_EncyclopediaMenu()
		{
			//this.oCPU.Log.EnterBlock("F0_2c84_07af_EncyclopediaMenu()");

			// function body
			// Instruction address 0x2c84:0x07e1, size: 5
			int selectedOption = this.parent.MenuBoxDialog.F0_2d05_0031_ShowMenuBox(" Complete\n Technology Advances\n City Improvements\n" +
				" Unit Types\n Terrain Types\n Miscellaneous\n", 182, 8, true, false, false);

			if (selectedOption < 0)
			{
				this.Var_654a = 1;
			}
			else
			{
				this.parent.Encyclopedia.ShowEncyclopediaByTopic((EncyclopediaTopicEnum)selectedOption);
				this.Var_654a = -1;
			}
		}
	}
}
