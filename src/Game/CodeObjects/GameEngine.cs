using Avalonia.Media;
using IRB.VirtualCPU;
using OpenCivOne.Graphics;

namespace OpenCivOne
{
	public class GameEngine
	{
		private OpenCivOneGame parent;

		private bool populationHighRange = false;
		private int nextPopulationThreshold = 1;

		public GameEngine(OpenCivOneGame parent)
		{
			this.parent = parent;
		}

		/// <summary>
		/// Handles a single game turn.
		/// Performs active player's turns, checks for disasters, victory and loss conditions.
		/// </summary>
		public void F0_1238_0092_GameTurn()
		{
			//this.oCPU.Log.EnterBlock("F0_1238_0092_GameTurn()");

			// function body
			// Instruction address 0x1238:0x009f, size: 5
			this.parent.CommonTools.StartPaletteCycleSlots();

			Player humanPlayer = this.parent.GameData.Players[this.parent.GameData.HumanPlayerID];

			int oldCoins = humanPlayer.Coins;

			this.parent.Var_d760_HumanPlayerMessageFlag = false;
			this.parent.Var_6b92 = 0;
			this.parent.GameData.MaximumTechnologyCount = 0;

			for (int i = 0; i < 8; i++)
			{
				if ((this.parent.GameData.ActiveCivilizations & (0x1 << i)) != 0)
				{
					this.parent.GameData.MaximumTechnologyCount = Math.Max(this.parent.GameData.MaximumTechnologyCount, this.parent.GameData.Players[i].DiscoveredTechnologyCount);
				}
			}

			if (this.parent.GameData.TurnCount == 0)
			{
				// Show map around starting unit
				this.parent.Var_d4cc_MapViewX = humanPlayer.Units[0].Position.X - 7;
				this.parent.Var_d75e_MapViewY = humanPlayer.Units[0].Position.Y - 6;

				// Instruction address 0x1238:0x0119, size: 3
				F0_1238_1b44_RedrawMapWithBlankingEffect();

				if (this.parent.GameData.DifficultyLevel == 0)
				{
					// Instruction address 0x1238:0x012a, size: 5
					this.parent.PlayerTurn.F0_1403_4060_DrawStatusPanel(this.parent.GameData.HumanPlayerID, 0);

					// Show UI help on first turn
					this.parent.MainIntro.F2_0000_17d9_ShowInstantAdviceElements();

					// Instruction address 0x1238:0x0138, size: 3
					F0_1238_1b44_RedrawMapWithBlankingEffect();
				}
			
				// Instruction address 0x1238:0x013f, size: 5
				this.parent.CommonTools.PlayTune(1, 0);
			}

			for (int i = 0; i < 8; i++)
			{
				Player currentPlayer = this.parent.GameData.Players[i];
				currentPlayer.Score = 0;

				if ((this.parent.GameData.ActiveCivilizations & (0x1 << i)) != 0 &&
					(i >= this.parent.GameData.HumanPlayerID || this.parent.Var_df60 == 0))
				{
					// Blink 'wait' status at the bottom left corner of the screen
					// Instruction address 0x1238:0x0164, size: 5
					this.parent.DrawTools.FillRectangle(this.parent.Var_aa_Screen0_Rectangle, 2, 192, 6, 6, 8);

					if (this.parent.Var_df60 == 0)
					{
						// Instruction address 0x1238:0x018d, size: 5
						this.parent.CityManagement.F0_1ade_0006(i);
					}

					if (currentPlayer.Coins > 30000)
					{
						currentPlayer.Coins = 30000;
					}

					// Instruction address 0x1238:0x01ab, size: 5
					this.parent.Diplomacy.F0_2517_0004_UpdatePlayerGovernmentAndDiplomacy(i);

					// Instruction address 0x1238:0x01b6, size: 5
					this.parent.AIEngine.F0_25fb_0004_RecalculateStatsAndPolicies(i);

					// Instruction address 0x1238:0x01c1, size: 5
					this.parent.AIEngine.F0_25fb_2fd7_ClearUnitPoliciesAndReassignThem((short)i);

					// Blink 'wait' status
					// Instruction address 0x1238:0x01de, size: 5
					this.parent.DrawTools.FillRectangle(this.parent.Var_aa_Screen0_Rectangle, 2, 192, 6, 6, 15);

					if (i == this.parent.GameData.HumanPlayerID)
					{
						// Instruction address 0x1238:0x01ef, size: 3
						F0_1238_107e_ReportHumanTaxAndPopulation();

						if (currentPlayer.CityCount == 0 && currentPlayer.SettlerCount == 0)
						{
							// Human player lost the game
							this.parent.MainIntro.F2_0000_152a_EndGame_PlayerLost();

							// Game state = player lost (-1)
							this.parent.Var_dc48_GameEndType = -1;
						}

						if (this.parent.GameData.Year > 0)
						{
							parent.GameData.PeaceTurnCount++;
						}
					}

					// Check if game continues
					if (this.parent.Var_dc48_GameEndType == 0)
					{
						// Instruction address 0x1238:0x022f, size: 5
						this.parent.PlayerTurn.F0_1403_000e_PlayerTurn(i);
					}

					this.parent.Var_df60 = 0;

					// Check if game should continue with the next player
					if (this.parent.Var_dc48_GameEndType != 0 && this.parent.Var_b884 == 0)
					{
						if (this.parent.Var_dc48_GameEndType == 2)
						{
							// Human player decided to retire
							// Show power graph, civilization score and hall of fame

							// Instruction address 0x1238:0x0261, size: 3
							F0_1238_08a0_EndGame();

							// Instruction address 0x1238:0x0897, size: 3
							//F0_1238_090a_ShowAdvertisementsPopup();
						}

						return;
					}
				}
			}

			F0_1238_0980_GenerateBarbarianUprising();
			F0_1238_1767_CheckGlobalWarming();
			GiveWonderToAI();

			if (this.parent.GameData.TurnCount > 50)
			{
				for (int i = 0; i < 2; i++)
				{
					// Instruction address 0x1238:0x02d3, size: 5
					int cityID = this.parent.CAPI.RNG.Next(128);

					if (this.parent.GameData.Cities[cityID].Status != CityStatusEnum.Invalid &&
						this.parent.GameData.Players[this.parent.GameData.Cities[cityID].PlayerID].CityCount > 1)
					{
						if (this.parent.GameData.Cities[cityID].ActualSize >= 5)
						{
							// Process city disasters and riots
							this.parent.CityEvents.F20_0000_0540((short)cityID);
						}
					}
				}
			}

			// Instruction address 0x1238:0x0307, size: 3
			F0_1238_0da1_RankPlayers();

			if (this.parent.Var_dc48_GameEndType == 0 && 
				humanPlayer.CityCount > 1 && humanPlayer.Coins < 100 && ((humanPlayer.Coins - oldCoins) * 10 + humanPlayer.Coins) < 0 &&
				(this.parent.GameData.PlayerFlags & (0x1 << this.parent.GameData.HumanPlayerID)) != 0)
			{
				// Warning: Funds are running low...
				this.parent.Var_2f9e_MessageBoxStyle = MenuBoxReportTypeEnum.DomesticAdvisorReport;

				// Instruction address 0x1238:0x0367, size: 3
				this.parent.MenuBoxDialog.ShowDialog(this.parent.LanguageTools.F0_2f4d_044f_GetTextFromKingSection("*TAXES"), 80, 80);
			}

			this.parent.GameData.TurnCount++;

			if (this.parent.GameData.Year < 1000)
			{
				this.parent.GameData.Year += 20;

				// handle case where after AD the year is set to 1 AD and then incremented by 20 equals 21 AD
				if (this.parent.GameData.Year == 21)
				{
					this.parent.GameData.Year = 20;
				}
			}
			else if (this.parent.GameData.Year < 1500)
			{
				this.parent.GameData.Year += 10;
			}
			else if (this.parent.GameData.Year < 1750)
			{
				this.parent.GameData.Year += 5;
			}
			else if (this.parent.GameData.Year < 1850 && (this.parent.GameData.SpaceshipFlags & 0xfe) == 0)
			{
				this.parent.GameData.Year += 2;
			}
			else
			{
				this.parent.GameData.Year += 1;
			}

			if (parent.GameData.Year == 0)
			{
				// boost research after AD event

				for (int i = 0; i < 8; i++)
				{
					this.parent.GameData.Players[i].ResearchProgress *= 2;
				}

				this.parent.GameData.Year = 1;
			}

			if ((this.parent.GameData.TurnCount % 50) == 0 && this.parent.GameData.HumanPlayerID != -1)
			{
				if (this.parent.GameData.GameSettingFlags.AutoSave)
				{
					// Game auto save
					this.parent.LoadAndSave.ShowSaveGameDialog(true);
				}

				this.parent.GameData.MaximumTechnologyCount = 0;

				for (int i = 1; i < 8; i++)
				{
					this.parent.GameData.MaximumTechnologyCount = Math.Max(this.parent.GameData.MaximumTechnologyCount, this.parent.GameData.Players[i].DiscoveredTechnologyCount);
				}

				this.parent.Var_d2de = 0;

				if (this.parent.GameData.Year >= 0)
				{
					// Instruction address 0x1238:0x0484, size: 5
					this.parent.Var_d2de = this.parent.CommonTools.CheckValueRange(
						this.parent.GameData.MaximumTechnologyCount - (this.parent.GameData.TurnCount / 9), 0, 6);
				}

				// Instruction address 0x1238:0x0493, size: 5
				int playerPopulation = this.parent.GameEngine.GetPlayerPopulationCount(this.parent.GameData.HumanPlayerID);

				// Instruction address 0x1238:0x04c8, size: 5
				this.parent.UnitManagement.F0_1866_250e_AddReplayData(11, (byte)humanPlayer.CityCount, (byte)((playerPopulation & 0xff00) >> 8), (byte)(playerPopulation & 0xff));

				int[] playerRankings = new int[4];

				Array.Fill(playerRankings, 0);

				for (int i = 1; i < 8; i++)
				{
					if ((this.parent.GameData.ActiveCivilizations & (0x1 << i)) != 0)
					{
						if ((this.parent.GameData.Players[i].Ranking & 0x1) == 0)
						{
							playerRankings[this.parent.GameData.Players[i].Ranking / 2] |= i;
						}
						else
						{
							playerRankings[this.parent.GameData.Players[i].Ranking / 2] |= i << 4;
						}
					}
				}

				// Instruction address 0x1238:0x0556, size: 5
				this.parent.UnitManagement.F0_1866_250e_AddReplayData(12, (byte)playerRankings[3], (byte)playerRankings[2], (byte)playerRankings[1], (byte)playerRankings[0]);
			}

			if (this.parent.GameData.TurnCount >= this.parent.GameData.NextPlayerHistoryRankingTurn)
			{
				// Instruction address 0x1238:0x056b, size: 5
				this.parent.GameData.NextPlayerHistoryRankingTurn = (short)(this.parent.CAPI.RNG.Next(40) + this.parent.GameData.TurnCount + 20);

				// Shows ranking report
				this.parent.Reports.F12_0000_09e2_PlayerHistoryRankingReport();
			}

			for (int i = 1; i < 22; i++)
			{
				int cityID = this.parent.GameData.WonderCityID[i];

				if (cityID != -1 && cityID < 128)
				{
					this.parent.GameData.Players[this.parent.GameData.Cities[cityID].PlayerID].Score += 25;
				}
			}

			if (this.parent.Var_3484 == -3)
			{
				this.parent.Var_3484 = -2;
			}
			else
			{
				humanPlayer.Score -= (short)(10 * this.parent.GameData.PollutedSquareCount);

				if (this.parent.Var_3484 == -2)
				{
					this.parent.Var_3484 = 0;
				}
			}

			#region For debugging, grow palace
			/*humanPlayer.PalaceBuildCount = 0;

			for (int i = 0; i < 12; i++)
			{
				humanPlayer.PalaceLevels[i] = (short)(((i > 2 && i < 6) || i > 8) ? 0 : -1);
				humanPlayer.PalaceTypes[i] = 0;
			}

			while (humanPlayer.PalaceBuildCount < 38)
			{
				this.oParent.Palace.F17_0000_0000_UpgradePalace(this.oParent.GameData.HumanPlayerID);

				humanPlayer.PalaceBuildCount++;
			}*/
			#endregion

			for (int i = 0; i < 128; i++)
			{
				if (this.parent.GameData.Cities[i].Status != CityStatusEnum.Invalid &&
					this.parent.GameData.Cities[i].PlayerID == this.parent.GameData.HumanPlayerID &&
					(this.parent.GameData.Cities[i].ImprovementFlags0 & 0x1) != 0)
				{
					if (this.parent.GameData.GameSettingFlags.BuildPalace && (this.parent.GameData.SpaceshipFlags & 0x100) == 0 &&
						(short)this.parent.CivilizationScore.F20_0000_0ca9_ShowCivilizationScorePopup(this.parent.GameData.HumanPlayerID, false) >= F0_1238_16e7_GetPalaceBuildNewScoreThreshold(humanPlayer.PalaceBuildCount + 1))
					{
						humanPlayer.PalaceBuildCount++;

						if (humanPlayer.PalaceBuildCount < 38)
						{
							// build palace user dialog
							this.parent.Palace.F17_0000_0000_UpgradePalace(this.parent.GameData.HumanPlayerID);
						}
					}
					// No need to check any further
					break;
				}
			}
		
			// Instruction address 0x1238:0x066f, size: 3
			F0_1238_107e_ReportHumanTaxAndPopulation();

			// ID of player that reached Alpha Centauri
			int playerIDSpaceVictory = -1;

			for (int i = 1; i < 8; i++)
			{
				if ((this.parent.GameData.SpaceshipFlags & (0x1 << i)) != 0 &&
					this.parent.GameData.Players[i].SpaceshipETAYear == this.parent.GameData.Year)
				{
					playerIDSpaceVictory = i;
					// no need to check any further
					break;
				}
			}

			if (this.parent.GameData.Year >= 2080 - (20 * this.parent.GameData.DifficultyLevel))
			{
				this.parent.News.F21_0000_0000_ShowNews(-1,
					$"{this.parent.Array_19b2_GovernmentTypeNames[(int)humanPlayer.GovernmentType]} {humanPlayer.Name}\nplans retirement\nin 20 years.\n");
			}

			if (playerIDSpaceVictory != -1 || this.parent.Var_b884 != 0 ||
				this.parent.GameData.Year >= 2100 - (20 * this.parent.GameData.DifficultyLevel))
			{
				if (this.parent.Var_b884 != 0)
				{
					// Conquest victory, don't care about space race
					playerIDSpaceVictory = -1;

					this.parent.News.F21_0000_0000_ShowNews(-1, "*** GAME OVER ***\nYour civilization\nhas conquered\nthe entire planet!\n");

					this.parent.Replay.F9_0000_0dde();
				}

				if (playerIDSpaceVictory != -1)
				{
					this.parent.MainIntro.F2_0000_0bd7_EndGame_SpaceVictory((short)playerIDSpaceVictory);

					// Instruction address 0x1238:0x0792, size: 3
					F0_1238_1b44_RedrawMapWithBlankingEffect();

					this.parent.News.F21_0000_0000_ShowNews(-1,
						$"*** GAME OVER ***\n{this.parent.GameData.Players[playerIDSpaceVictory].Nationality} spaceship\narrives at Alpha Centauri.\n");
				}

				if (this.parent.GameData.Year >= 2100 - (20 * this.parent.GameData.DifficultyLevel))
				{
					// Instruction address 0x1238:0x0816, size: 5
					this.parent.CommonTools.PlayTune(this.parent.GameData.Nations[humanPlayer.NationalityID].LongTune, 3);

					this.parent.News.F21_0000_0000_ShowNews(-1, $"*** GAME OVER ***\n{humanPlayer.Name} dynasty\nends after glorious\n6000 year reign!\n");

					// Instruction address 0x1238:0x082e, size: 5
					this.parent.CommonTools.PlayTune(1, 0);
				}

				if ((this.parent.GameData.SpaceshipFlags & 0x100) == 0)
				{
					// Show power graph, civilization score and hall of fame
					F0_1238_08a0_EndGame();
				}

				// Instruction address 0x1238:0x084f, size: 3
				F0_1238_1b44_RedrawMapWithBlankingEffect();

				if (this.parent.Var_b884 == 0)
				{
					// Do you want to keep playing ?
					this.parent.Var_dc48_GameEndType = 1 - this.parent.MenuBoxDialog.ShowDialog(this.parent.LanguageTools.F0_2f4d_044f_GetTextFromKingSection("*FAME"), 80, 80);
				}
				else
				{
					this.parent.Var_dc48_GameEndType = 1;
				}
			}
		}

		/// <summary>
		/// End game
		/// </summary>
		public void F0_1238_08a0_EndGame()
		{
			//this.oCPU.Log.EnterBlock("F0_1238_08a0()");

			// function body
			this.parent.Reports.F12_0000_0573_ShowPowerGraphReport();

			if (this.parent.Var_dc48_GameEndType != 2)
			{
				this.parent.Replay.F9_0000_0000();
			}

			if ((this.parent.GameData.SpaceshipFlags & 0x100) == 0)
			{
				this.parent.HallOfFame.F3_0000_065d_ShowFamousPersonsWindow((this.parent.GameData.Players[this.parent.GameData.HumanPlayerID].PalaceBuildCount / 2) - 1,
					(short)this.parent.CivilizationScore.F20_0000_0ca9_ShowCivilizationScorePopup(this.parent.GameData.HumanPlayerID, true));

				this.parent.HallOfFame.F3_0000_002b_ReadHallOfFameFromFile();

				this.parent.HallOfFame.F3_0000_0513_AddNewEntryToHallOfFame();

				this.parent.HallOfFame.F3_0000_0083_WriteHallOfFameToFile();
			}		
		}

		/// <summary>
		/// Generates barbarian uprisings in the random area in the beginning of the game.
		/// At first barbarian uprisings are near the cities and after the discovery of Automobile they are everywhere
		/// </summary>
		public void F0_1238_0980_GenerateBarbarianUprising()
		{
			//this.oCPU.Log.EnterBlock("F0_1238_0980()");

			// function body
			if ((5 - this.parent.GameData.DifficultyLevel) * 30 <= this.parent.GameData.TurnCount)
			{
				if (((this.parent.GameData.TurnCount + 1) & 0x7) == 0 &&
					(this.parent.GameData.TechnologyFirstDiscoveredBy[(int)TechnologyAdvanceEnum.Automobile] & 0x7) == 0)
				{
					int x;
					int y;

					do
					{
						x = this.parent.CAPI.RNG.Next(80);
						y = this.parent.CAPI.RNG.Next(44) + 3;
					}
					while (this.parent.MapManagement.GetTerrainType(x, y) != TerrainTypeEnum.Water ||
						this.parent.MapManagement.F0_2aea_14e0_GetCellActiveUnitPlayerID(x, y) != -1 ||
						this.parent.MapManagement.F0_2aea_195d_GetMapGroupSize(x, y) < 16);

					int barbarianUnitCount = this.parent.CommonTools.CheckValueRange((this.parent.GameData.TurnCount / 150) + 1, 1, 3);

					// Instruction address 0x1238:0x0a43, size: 5
					if (this.parent.MapManagement.GetTerrainType(x, y) == TerrainTypeEnum.Water)
					{
						this.parent.UnitManagement.F0_1866_0cf5_CreateUnit(0, ((barbarianUnitCount < 3) ? UnitTypeEnum.Sail : UnitTypeEnum.Frigate), x, y);
					}

					for (int i = 0; i <= barbarianUnitCount; i++)
					{
						UnitTypeEnum unitType;

						if ((this.parent.GameData.TechnologyFirstDiscoveredBy[(int)TechnologyAdvanceEnum.Explosives] & 0x7) == 0)
						{
							unitType = UnitTypeEnum.Legion;
						}
						else
						{
							unitType = UnitTypeEnum.Knights;
						}

						// Last barbarian unit is always a diplomat
						if (i == barbarianUnitCount)
						{
							unitType = UnitTypeEnum.Diplomat;
						}

						int unitID = this.parent.UnitManagement.F0_1866_0cf5_CreateUnit(0, unitType, x, y);

						this.parent.GameData.Players[0].Units[unitID].Status |= UnitStatusEnum.Sentry;
					}
				}

				int selectedCityID = this.parent.CAPI.RNG.Next(128);

				if (this.parent.GameData.Cities[selectedCityID].Status != CityStatusEnum.Invalid &&
					this.parent.GameData.Cities[selectedCityID].PlayerID == this.parent.GameData.HumanPlayerID &&
					this.parent.CAPI.RNG.Next(100) < this.parent.Actions.F0_2459_0687_GetCityTreasury(selectedCityID))
				{
					int x = 0;
					int y = 0;
					int retryCount;

					for (retryCount = 0; retryCount < 99; retryCount++)
					{
						x = this.parent.GameData.Cities[selectedCityID].Position.X + this.parent.CAPI.RNG.Next(13) - 6;
						y = this.parent.GameData.Cities[selectedCityID].Position.Y + this.parent.CAPI.RNG.Next(13) - 6;

						selectedCityID = this.parent.MapManagement.FindNearestCity(x, y);

						int distance = this.parent.MapManagement.GetDistance(x, y, this.parent.GameData.Cities[selectedCityID].Position);

						if (distance >= 6)
						{
							if (!this.parent.UnitManagement.F0_1866_1750_IsUnitOrCityNear(0, x, y) &&
								this.parent.MapManagement.GetTerrainType(x, y) != TerrainTypeEnum.Water &&
								this.parent.MapManagement.F0_2aea_14e0_GetCellActiveUnitPlayerID(x, y) == -1 &&
								this.parent.MapManagement.F0_2aea_1942_GetGroupID(x, y) == this.parent.MapManagement.F0_2aea_1942_GetGroupID(
									this.parent.GameData.Cities[selectedCityID].Position.X, this.parent.GameData.Cities[selectedCityID].Position.Y))
								break;
						}
					}

					if (retryCount < 99)
					{
						if (this.parent.GameData.Cities[selectedCityID].PlayerID == this.parent.GameData.HumanPlayerID &&
							(this.parent.GameData.MapVisibility[x, y] & (0x1 << this.parent.GameData.HumanPlayerID)) != 0)
						{
							// Instruction address 0x1238:0x0c4b, size: 5
							this.parent.MapManagement.F0_2aea_0008_DrawVisibleMap(this.parent.GameData.HumanPlayerID,
								this.parent.GameData.Cities[selectedCityID].Position.X - 8, this.parent.GameData.Cities[selectedCityID].Position.Y - 6);

							this.parent.Var_2f9e_MessageBoxStyle = MenuBoxReportTypeEnum.TravelersReport;

							if ((this.parent.GameData.TechnologyFirstDiscoveredBy[35] & 7) != 0)
							{
								this.parent.MenuBoxDialog.ShowDialog(
									$"Travellers report:\nGuerrilla uprising\nnear {this.parent.Actions.F0_2459_08c6_GetCityName(selectedCityID)}!\n", 100, 32);
							}
							else if ((this.parent.GameData.TechnologyFirstDiscoveredBy[68] & 7) != 0)
							{
								this.parent.MenuBoxDialog.ShowDialog(
									$"Travellers report:\nNative unrest\nnear {this.parent.Actions.F0_2459_08c6_GetCityName(selectedCityID)}!\n", 100, 32);
							}
							else
							{
								this.parent.MenuBoxDialog.ShowDialog(
									$"Travellers report:\nBarbarian uprising\nnear {this.parent.Actions.F0_2459_08c6_GetCityName(selectedCityID)}!\n", 100, 32);
							}
						}
					
						// 99 units? Isn't that a little too big, the city would actually be the size of 198 to reach that count. Is that even possible?
						int barbarianUnitCount = this.parent.CommonTools.CheckValueRange(this.parent.CAPI.RNG.Next(this.parent.GameData.Cities[selectedCityID].ActualSize / 2), 2, 99);

						for (int i = 0; i <= barbarianUnitCount; i++)
						{
							UnitTypeEnum unitType;

							if ((this.parent.GameData.TechnologyFirstDiscoveredBy[(int)TechnologyAdvanceEnum.Industrialization] & 0x7) == 0)
							{
								if ((i & 0x4) != 0)
								{
									unitType = UnitTypeEnum.Chariot;
								}
								else
								{
									unitType = UnitTypeEnum.Cavalry;
								}
							}
							else
							{
								if ((i & 0x4) == 0)
								{
									unitType = UnitTypeEnum.Musketeers;
								}
								else
								{
									unitType = UnitTypeEnum.Cannon;
								}
							}

							if (i == barbarianUnitCount)
							{
								unitType = UnitTypeEnum.Diplomat;
							}

							int unitID = this.parent.UnitManagement.F0_1866_0cf5_CreateUnit(0, unitType, x, y);

							// In case our barbarian unit count has reached capacity
							if (unitID != -1)
							{
								this.parent.GameData.Players[0].Units[unitID].VisibleByPlayer |= this.parent.GameData.MapVisibility[x, y];
							}

							if (this.parent.GameData.Cities[selectedCityID].PlayerID == this.parent.GameData.HumanPlayerID)
							{
								// Instruction address 0x1238:0x0d6a, size: 5
								this.parent.MapManagement.F0_2aea_11d4_DrawCellWithUnit(x, y);
							}
						}
					}
				}
			}
		}

		/// <summary>
		/// Rank players and trigger contact with Human player (if it has lowest ranking)
		/// </summary>
		public void F0_1238_0da1_RankPlayers()
		{
			// this.CPU.Log.EnterBlock("F0_1238_0da1()");

			// function body
			int[] rankingScores = new int[8];

			for (int i = 1; i < 8; i++)
			{
				Player player = this.parent.GameData.Players[i];

				rankingScores[i] = player.Coins / 32 + player.TotalCitySize * 8 + player.DiscoveredTechnologyCount;

				for (int j = 0; j < 28; j++)
				{
					rankingScores[i] += this.parent.GameData.Units[j].Cost * player.ActiveUnits[j];
				}

				if ((this.parent.GameData.ActiveCivilizations & (0x1 << i)) == 0)
				{
					rankingScores[i] = 0;
				}

				if (this.parent.GameData.TurnCount < 600)
				{
					this.parent.GameData.ScoreGraphData[((this.parent.GameData.TurnCount / 4) % 150) * 8 + i] = (byte)this.parent.CommonTools.CheckValueRange(rankingScores[i] / 8, 0, 255);
				}
			
				int peaceFlags = 0;

				for (int j = 0; j < 8; j++)
				{
					if ((player.Diplomacy[j] & (DiplomacyFlagsEnum.Contact | DiplomacyFlagsEnum.Peace)) == DiplomacyFlagsEnum.Contact)
					{
						peaceFlags |= (0x1 << j);
					}
				}

				if (this.parent.GameData.TurnCount < 600)
				{
					this.parent.GameData.PeaceGraphData[((this.parent.GameData.TurnCount / 4) % 150) * 8 + i] = (byte)peaceFlags;
				}
			}

			for (int i = 1; i < 8; i++)
			{
				int selectedPlayerID = 0;
				int highestScore = -1;

				for (int j = 1; j < 8; j++)
				{
					if (highestScore < rankingScores[j])
					{
						selectedPlayerID = j;
						highestScore = rankingScores[j];
					}
				}

				if (selectedPlayerID < this.parent.GameData.Players.Length)
				{
					this.parent.GameData.Players[selectedPlayerID].Ranking = (short)(8 - i);
				}

				rankingScores[selectedPlayerID] = -1;
			}
			
			this.parent.Var_8078 = 0;

			// If the human player is ranked as lowest trigger contact with other players
			if (this.parent.GameData.Players[this.parent.GameData.HumanPlayerID].Ranking == 7 &&
				this.parent.GameData.Players[this.parent.GameData.HumanPlayerID].CityCount > 4 &&
				this.parent.GameData.Players[this.parent.GameData.HumanPlayerID].ActiveUnits[(int)UnitTypeEnum.Nuclear] == 0 &&
				this.parent.GameData.TurnCount > 200)
			{
				this.parent.Var_8078 = 1;

				for (int i = 1; i < 8; i++)
				{
					if (i != this.parent.GameData.HumanPlayerID)
					{
						if (this.parent.GameData.Players[i].CityCount > 1)
						{
							if (!this.parent.GameData.Players[i].Diplomacy[this.parent.GameData.HumanPlayerID].HasFlag(DiplomacyFlagsEnum.Peace))
							{
								this.parent.GameData.Players[i].Diplomacy[this.parent.GameData.HumanPlayerID] |= DiplomacyFlagsEnum.Contact;
							}
							else
							{
								this.parent.GameData.Players[i].Diplomacy[this.parent.GameData.HumanPlayerID] |= DiplomacyFlagsEnum.PeaceTreaty;
							}
						}
					}
				}
			}
		}

		/// <summary>
		/// Report tax and population values for human player
		/// </summary>
		public void F0_1238_107e_ReportHumanTaxAndPopulation()
		{
			//this.CPU.Log.EnterBlock("F0_1238_107e()");

			// function body			
			// Instruction address 0x1238:0x109f, size: 3
			this.parent.DrawTools.F0_1238_1bb2_FillRectangleWithShadow(0, 58, 80, 39);

			// Instruction address 0x1238:0x10ba, size: 5
			this.parent.DrawTools.FillRectangle(this.parent.Var_aa_Screen0_Rectangle, 3, 60, 74, 11, 11);

			if (this.parent.GameData.PeaceTurnCount > 1)
			{
				// Instruction address 0x1238:0x10f3, size: 5
				this.parent.DrawTools.FillRectangle(this.parent.Var_aa_Screen0_Rectangle, 3, 60, this.parent.CommonTools.CheckValueRange(this.parent.GameData.PeaceTurnCount * 2, 0, 60), 2, 15);
			}

			// Instruction address 0x1238:0x110c, size: 3
			F0_1238_14a3_DrawMiniPalace(this.parent.GameData.HumanPlayerID, this.parent.GameData.Players[this.parent.GameData.HumanPlayerID].PalaceBuildCount, 8, 58);

			int populationCount = this.parent.GameEngine.GetPlayerPopulationCount(this.parent.GameData.HumanPlayerID);

			if (populationCount != 0)
			{
				// Instruction address 0x1238:0x1196, size: 5
				this.parent.DrawTools.DrawStringToScreen0($"{this.parent.GameEngine.PopulationValueToString(populationCount)}#", 2, 73, 0);
			}

			// Instruction address 0x1238:0x11b6, size: 5
			this.parent.DrawTools.DrawStringToScreen0(YearToString(this.parent.GameData.Year), 2, 81, 0);

			int researchValue;

			if (this.parent.GameData.Year < 0)
			{
				researchValue = (this.parent.GameData.Players[this.parent.GameData.HumanPlayerID].ResearchProgress * 2) /
					((this.parent.GameData.DifficultyLevel * 2 + this.parent.Var_d2de + 6) *
						this.parent.GameData.Players[this.parent.GameData.HumanPlayerID].DiscoveredTechnologyCount + 1);
			}
			else
			{
				researchValue = (this.parent.GameData.Players[this.parent.GameData.HumanPlayerID].ResearchProgress * 2) /
					((this.parent.GameData.DifficultyLevel * 2 + this.parent.Var_d2de + 6) *
						this.parent.GameData.Players[this.parent.GameData.HumanPlayerID].DiscoveredTechnologyCount * 2 + 1);
			}

			// Instruction address 0x1238:0x1229, size: 5
			this.parent.Graphics.F0_VGA_07d8_DrawImage(this.parent.Var_19e8_Screen2_Rectangle,
				(this.parent.CommonTools.CheckValueRange(researchValue, 0, 3) * 8) + 160, 120, 8, 8, this.parent.Var_aa_Screen0_Rectangle, 48, 80);

			if (this.parent.GameData.PollutionEffectLevel != 0)
			{
				// Instruction address 0x1238:0x1280, size: 5
				this.parent.Graphics.F0_VGA_07d8_DrawImage(this.parent.Var_19e8_Screen2_Rectangle,
					(this.parent.CommonTools.CheckValueRange(this.parent.GameData.PollutionEffectLevel / 4, 0, 3) * 8) + 160, 128, 8, 8, this.parent.Var_aa_Screen0_Rectangle, 56, 80);
			}

			// Instruction address 0x1238:0x1382, size: 5
			this.parent.DrawTools.DrawStringToScreen0($"{this.parent.GameData.Players[this.parent.GameData.HumanPlayerID].Coins}$ " +
				$"{10 - this.parent.GameData.Players[this.parent.GameData.HumanPlayerID].TaxRate + this.parent.GameData.Players[this.parent.GameData.HumanPlayerID].ScienceTaxRate}(" +
				$"{this.parent.GameData.Players[this.parent.GameData.HumanPlayerID].TaxRate}/{this.parent.GameData.Players[this.parent.GameData.HumanPlayerID].ScienceTaxRate})", 2, 89, 0);

			int oldPopulationThreshold = this.nextPopulationThreshold;

			if (populationCount < 100)
			{
				if (this.nextPopulationThreshold * 10 < populationCount)
				{
					this.nextPopulationThreshold = populationCount / 10;
				}
			}
			else
			{
				if (!this.populationHighRange || this.nextPopulationThreshold * 100 < populationCount)
				{
					this.nextPopulationThreshold = populationCount / 100;
					this.populationHighRange = true;
				}
			}

			if (oldPopulationThreshold < this.nextPopulationThreshold && this.parent.Var_3484 >= -1)
			{
				this.parent.Var_2f9e_MessageBoxStyle = MenuBoxReportTypeEnum.DomesticAdvisorReport;

				// Instruction address 0x1238:0x1498, size: 3
				this.parent.MenuBoxDialog.ShowDialog($"The population of the\n{this.parent.GameData.Players[this.parent.GameData.HumanPlayerID].Nationality} " +
					$"empire now\nexceeds {this.parent.GameEngine.PopulationValueToString(populationCount)} citizens.\n", 100, 80);
			}
		}

		/// <summary>
		/// Draws mini palace on main status board, and on scoring page
		/// </summary>
		/// <param name="playerID"></param>
		/// <param name="palaceBuildCount"></param>
		/// <param name="x"></param>
		/// <param name="y"></param>
		public void F0_1238_14a3_DrawMiniPalace(int playerID, int palaceBuildCount, int x, int y)
		{
			//this.CPU.Log.EnterBlock($"F0_1238_14a3({playerID}, {palaceLevel}, {x}, {y})");

			// function body
			int[] palaceLevels = new int[9];
			int[] palaceTypes = new int[9];

			// Instruction address 0x1238:0x14c5, size: 5
			this.parent.Graphics.F0_VGA_0599_DrawLine(this.parent.Var_aa_Screen0_Rectangle, x - 5, y + 13, x + 68, y + 13, 2);

			if (this.parent.GameData.Players[playerID].CityCount != 0)
			{
				for (int i = 0; i < 9; i++)
				{
					palaceLevels[i] = -1;
				}

				for (int i = 3; i < 6; i++)
				{
					palaceLevels[i] = 0;
				}

				if (playerID == this.parent.GameData.HumanPlayerID)
				{
					for (int i = 0; i < 9; i++)
					{
						palaceLevels[i] = this.parent.GameData.Players[this.parent.GameData.HumanPlayerID].PalaceLevels[i];
						palaceTypes[i] = this.parent.GameData.Players[this.parent.GameData.HumanPlayerID].PalaceTypes[i];
					}
				}
				else
				{
					int miniPalaceLevel = this.parent.CommonTools.CheckValueRange(palaceBuildCount + 3, 3, 36);
					int miniPalaceThreshold = miniPalaceLevel / 5;
					int randomValue = playerID * this.parent.GameData.RandomSeed;

					for (int i = 0; i < miniPalaceLevel; i++)
					{
						int palaceLevel = 0;
						int palaceSection = 0;

						for (int j = 0; j < 999; j++)
						{
							bool palaceSectionFinished = true;

							randomValue = randomValue * 5 + 1;
							palaceSection = randomValue % 9;
							palaceLevel = palaceLevels[palaceSection] + 1;

							if (palaceSection < 4 && (palaceLevels[palaceSection + 1] < palaceLevel || palaceLevels[7 - palaceSection] == -1))
							{
								palaceSectionFinished = false;
							}

							if (palaceSection > 4 && (palaceLevels[palaceSection - 1] < palaceLevel || palaceLevels[9 - palaceSection] == -1))
							{
								palaceSectionFinished = false;
							}

							if (palaceLevel > miniPalaceThreshold || palaceLevel > 3)
							{
								palaceSectionFinished = false;
							}

							if (palaceSectionFinished)
								break;
						}

						palaceLevels[palaceSection] = palaceLevel;
						palaceTypes[palaceSection] = playerID % 3;
					}
				}

				for (int i = 0; i < 9; i++)
				{
					if (palaceLevels[i] > 0)
					{
						int palaceSection = 2; // Center section

						if (i < 4)
						{
							palaceSection = 1; // inner piece

							if (i == 1 || palaceLevels[i - 1] == -1)
							{
								palaceSection = 0; // Left edge
							}
						}

						if (i > 4)
						{
							palaceSection = 3; // inner piece

							if (i == 7 || palaceLevels[i + 1] == -1)
							{
								palaceSection = 4; // right edge
							}
						}

						// Instruction address 0x1238:0x16cd, size: 5
						this.parent.Graphics.F0_VGA_0c3e_DrawBitmapToScreen(this.parent.Var_aa_Screen0_Rectangle,
							(i * 7) + x, y - 1,
							this.parent.Array_d21c[palaceTypes[i], this.parent.CommonTools.CheckValueRange(palaceLevels[i] - 1, 0, 3) * 5 + palaceSection]);
					}
				}
			}
		}

		/// <summary>
		/// Gets the next score threshold to build the palace
		/// </summary>
		/// <param name="palaceLevel"></param>
		public int F0_1238_16e7_GetPalaceBuildNewScoreThreshold(int palaceLevel)
		{
			//this.CPU.Log.EnterBlock($"F0_1238_16e7({palaceLevel})");

			// function body
			int thresholdValue = 0;

			for (int i = 0; i <= palaceLevel; i++)
			{
				thresholdValue += i * (7 - this.parent.GameData.DifficultyLevel);
			}

			return thresholdValue / 2 + 1;
		}

		/// <summary>
		/// Converts year to String
		/// </summary>
		public string YearToString(int year)
		{
			return $"{Math.Abs(year)} {((year < 0) ? "BC" : "AD")}";
		}

		/// <summary>
		/// Convert the turn count to current year
		/// </summary>
		/// <param name="turnCount"></param>
		/// <returns></returns>
		public int TurnCountToYear(int turnCount)
		{
			//this.oCPU.Log.EnterBlock($"F9_0000_097c({turnCount})");
			int year;

			// function body
			if (turnCount > 250)
			{
				turnCount -= 250;

				if (turnCount > 50)
				{
					turnCount -= 50;

					if (turnCount > 50)
					{
						turnCount -= 50;

						if (turnCount > 50)
						{
							turnCount -= 50;

							year = turnCount + 1850;
						}
						else
						{
							year = (turnCount * 2) + 1750;
						}
					}
					else
					{
						year = (turnCount * 5) + 1500;
					}
				}
				else
				{
					year = (turnCount * 10) + 1000;
				}
			}
			else
			{
				year = (turnCount * 20) - 4000;
			}

			return year;
		}

		/// <summary>
		/// Check global warming and pollution
		/// </summary>
		public void F0_1238_1767_CheckGlobalWarming()
		{
			//this.CPU.Log.EnterBlock("F0_1238_1767()");

			// function body
			this.parent.GameData.PollutionEffectLevel = (short)this.parent.CommonTools.CheckValueRange(
				Math.Sign(this.parent.GameData.PollutedSquareCount * 2 - this.parent.GameData.GlobalWarmingCount * 4 - this.parent.GameData.PollutionEffectLevel) +
					this.parent.GameData.PollutionEffectLevel, 0, 99);

			if (this.parent.GameData.PollutionEffectLevel == 12 && this.parent.GameData.PollutedSquareCount > 6)
			{
				this.parent.News.F21_0000_0000_ShowNews(-1, "Scientists alarmed by\nrising temperatures.\nGlobal Warming feared!\n");
			}

			if (this.parent.GameData.PollutionEffectLevel > 16)
			{
				F7_0000_1be3_IcecapMeltEvent(this.parent.GameData.GlobalWarmingCount);

				this.parent.GameData.GlobalWarmingCount++;
				this.parent.GameData.PollutionEffectLevel = 0;
			}
		}

		/// <summary>
		/// Gives a wonder to any AI by chance
		/// </summary>
		public void GiveWonderToAI()
		{
			for (int i = 0; i <= this.parent.GameData.DifficultyLevel; i++)
			{
				int wonderID = this.parent.CAPI.RNG.Next(21) + 1;

				if (this.parent.GameData.DifficultyLevel != 0 ||
					this.parent.Technology.F0_1ade_22b5_PlayerHasTechnology(this.parent.GameData.HumanPlayerID,
						this.parent.GameData.GetImprovementType(wonderID).RequiresTechnology))
				{
					if (this.parent.GameData.DifficultyLevel < 2)
					{
						for (int j = 0; j < 128; j++)
						{
							if (this.parent.GameData.Cities[j].Status != CityStatusEnum.Invalid &&
								this.parent.GameData.Cities[j].PlayerID == this.parent.GameData.HumanPlayerID &&
								this.parent.GameData.Cities[j].CurrentProductionID == -(wonderID + 24))
							{
								wonderID = 17;
							}
						}
					}

					// Instruction address 0x1238:0x1928, size: 5
					int cityID = this.parent.CAPI.RNG.Next(128);

					if (this.parent.GameData.Cities[cityID].Status != CityStatusEnum.Invalid)
					{
						int playerID = this.parent.GameData.Cities[cityID].PlayerID;

						if (playerID != -1 && playerID != this.parent.GameData.HumanPlayerID &&
							!this.parent.GameData.Players[playerID].Diplomacy[0].HasFlag(DiplomacyFlagsEnum.Allied) &&
							(wonderID != 17 || this.parent.Technology.F0_1ade_22b5_PlayerHasTechnology(playerID, TechnologyAdvanceEnum.Rocketry)))
						{
							if (i == 0 &&
								this.parent.Technology.F0_1ade_22b5_PlayerHasTechnology(playerID, TechnologyAdvanceEnum.Rocketry) &&
								!this.parent.Technology.F0_1ade_22b5_PlayerHasTechnology(this.parent.GameData.HumanPlayerID, TechnologyAdvanceEnum.Rocketry) &&
								this.parent.GameData.WonderCityID[17] == -1)
							{
								wonderID = 17;
							}

							if ((wonderID != 17 ||
									!this.parent.Technology.F0_1ade_22b5_PlayerHasTechnology(this.parent.GameData.HumanPlayerID, TechnologyAdvanceEnum.Rocketry) ||
									this.parent.GameData.Players[playerID].MilitaryPower <= this.parent.GameData.Players[this.parent.GameData.HumanPlayerID].MilitaryPower) &&
								this.parent.GameData.WonderCityID[wonderID] == -1 &&
								this.parent.GameData.Cities[cityID].ActualSize > this.parent.GameData.Wonders[wonderID].Cost / 10 &&
								this.parent.Technology.F0_1ade_22b5_PlayerHasTechnology(playerID, this.parent.GameData.Wonders[wonderID].RequiresTechnology))
							{
								this.parent.GameData.WonderCityID[wonderID] = (short)cityID;

								this.parent.GameData.Cities[cityID].ShieldsCount = 0;

								// Instruction address 0x1238:0x1a6f, size: 5
								this.parent.UnitManagement.F0_1866_250e_AddReplayData(10, (byte)((sbyte)playerID), (byte)((sbyte)wonderID));

								this.parent.GameData.Players[playerID].Coins -= (short)this.parent.CommonTools.CheckValueRange(this.parent.GameData.Players[playerID].Coins / 3, 0,
									(10 * this.parent.GameData.Wonders[wonderID].Cost) - this.parent.GameData.Cities[cityID].ShieldsCount);

								// Instruction address 0x1238:0x1b1e, size: 5
								this.parent.CommonTools.PlayTune(this.parent.GameData.Nations[this.parent.GameData.Players[playerID].NationalityID].ShortTune, 0);

								if (!this.parent.GameData.Players[this.parent.GameData.HumanPlayerID].Diplomacy[playerID].HasFlag(DiplomacyFlagsEnum.EmbassyEstablished))
								{
									this.parent.Var_2f9e_MessageBoxStyle = MenuBoxReportTypeEnum.TravelersReport;
								}
								else
								{
									this.parent.Var_2f9e_MessageBoxStyle = MenuBoxReportTypeEnum.ForeignMinisterReport;
								}

								// Instruction address 0x1238:0x1802, size: 3
								this.parent.MenuBoxDialog.ShowDialog($"{this.parent.Actions.F0_2459_08c6_GetCityName(cityID)} ({this.parent.GameData.Players[playerID].Nationality})\nbuilds " +
									$"{this.parent.GameData.Wonders[wonderID].Name}.\n", 100, 80);

								this.parent.Encyclopedia.DisplayEncyclopediaTopic(EncyclopediaTopicEnum.CityImprovement, wonderID + 24);

								// Instruction address 0x1238:0x181c, size: 3
								F0_1238_1b44_RedrawMapWithBlankingEffect();

								// Instruction address 0x1238:0x1823, size: 5
								this.parent.CommonTools.PlayTune(1, 0);

								if (wonderID == 14)
								{
									// Instruction address 0x1238:0x1837, size: 5
									this.parent.CityManagement.F0_1ade_1584(playerID, 0);

									// Instruction address 0x1238:0x1845, size: 5
									this.parent.CityManagement.F0_1ade_1584(playerID, 0);
								}

								for (int j = 0; j < 128; j++)
								{
									if (this.parent.GameData.Cities[j].Status != CityStatusEnum.Invalid &&
										this.parent.GameData.Cities[j].PlayerID == this.parent.GameData.HumanPlayerID &&
										this.parent.GameData.Cities[j].CurrentProductionID == -(wonderID + 24))
									{
										// Instruction address 0x1238:0x1893, size: 5
										this.parent.CityManagement.F0_1ade_03ea(j);
									}
								}

								break;
							}
						}
					}
				}
			}
		}

		/// <summary>
		/// Fades main screen to black, draws the map and fades colors from black to main palette colors
		/// </summary>
		public void F0_1238_1b44_RedrawMapWithBlankingEffect()
		{
			//this.oCPU.Log.EnterBlock("F0_1238_1b44()");

			// function body
			// Instruction address 0x1238:0x1b48, size: 5
			this.parent.CommonTools.StopPaletteCycleSlots();

			// Instruction address 0x1238:0x1b66, size: 5
			this.parent.CommonTools.TransformPaletteToColor(5, Color.FromRgb(0, 0, 0));

			// Instruction address 0x1238:0x1b7f, size: 5
			this.parent.MapManagement.F0_2aea_0008_DrawVisibleMap(this.parent.GameData.HumanPlayerID, this.parent.Var_d4cc_MapViewX, this.parent.Var_d75e_MapViewY);

			// Instruction address 0x1238:0x1b97, size: 3
			this.parent.DrawTools.F0_1238_1bb2_FillRectangleWithShadow(0, 97, 80, 103);

			// Instruction address 0x1238:0x1ba3, size: 3
			this.parent.CommonTools.LoadAndApplyDefaultColorPalette();

			// Instruction address 0x1238:0x1bac, size: 5
			this.parent.CommonTools.StartPaletteCycleSlots();
		}

		/// <summary>
		/// Calculates the approximate palace level to be built from score
		/// </summary>
		/// <param name="score"></param>
		public int F0_1238_1c98_GetPalaceLevelFromScore(int score)
		{
			// function body
			int level = 0;

			do
			{
				level += 5;
				score -= level;
			} while (score > 0);

			level /= 5;
			level--;

			return level;
		}

		/// <summary>
		/// Global warming causes Icecap melt event
		/// </summary>
		/// <param name="globalWarmingCount"></param>
		public void F7_0000_1be3_IcecapMeltEvent(short globalWarmingCount)
		{
			//this.CPU.Log.EnterBlock($"F7_0000_1be3({globalWarmingCount})");

			// function body
			this.parent.News.F21_0000_0000_ShowNews(-1, "Global temperature\nrises! Icecaps melt.\nSevere Drought.\n");

			for (int i = 0; i < 80; i++)
			{
				for (int j = 0; j < 50; j++)
				{
					// Instruction address 0x0000:0x1cbe, size: 5
					TerrainTypeEnum terrainType = this.parent.MapManagement.GetTerrainType(i, j);

					if (terrainType == TerrainTypeEnum.Desert || terrainType == TerrainTypeEnum.Plains || terrainType == TerrainTypeEnum.Grassland || terrainType == TerrainTypeEnum.Forest)
					{
						int waterCount = 0;

						for (int k = 1; k < 9; k++)
						{
							GPoint direction = this.parent.MoveDirections[k];
							int newX = i + direction.X;
							int newY = j + direction.Y;

							if (this.parent.MapManagement.ValidateMapCoordinates(newX, newY) && this.parent.MapManagement.GetTerrainType(newX, newY) == TerrainTypeEnum.Water)
							{
								waterCount++;
							}
						}

						if (7 - globalWarmingCount <= waterCount)
						{
							this.parent.GameData.MapVisibility[i, j] |= 1;

							if (terrainType != TerrainTypeEnum.Forest)
							{
								// Instruction address 0x0000:0x1c18, size: 5
								this.parent.MapManagement.SetTerrainType(i, j, TerrainTypeEnum.Swamp);
							}
							else
							{
								// Instruction address 0x0000:0x1c18, size: 5
								this.parent.MapManagement.SetTerrainType(i, j, TerrainTypeEnum.Jungle);
							}

							// Instruction address 0x0000:0x1c2a, size: 5
							this.parent.MapManagement.F0_2aea_16ee_ClearTerrainImprovements(i, j, TerrainImprovementFlagsEnum.Mines | TerrainImprovementFlagsEnum.Irrigation);

							if ((this.parent.GameData.MapVisibility[i, j] & (0x1 << this.parent.GameData.HumanPlayerID)) != 0)
							{
								// Instruction address 0x0000:0x1ca4, size: 5
								this.parent.MapManagement.F0_2aea_11d4_DrawCellWithUnit(i, j);
							}
						}
						else if (((i * 11 + j * 13) & 0x7) == globalWarmingCount)
						{
							this.parent.GameData.MapVisibility[i, j] |= 1;

							if (terrainType == TerrainTypeEnum.Desert || terrainType == TerrainTypeEnum.Plains)
							{
								// Instruction address 0x0000:0x1c82, size: 5
								this.parent.MapManagement.SetTerrainType(i, j, TerrainTypeEnum.Desert);
							}
							else
							{
								// Instruction address 0x0000:0x1c82, size: 5
								this.parent.MapManagement.SetTerrainType(i, j, TerrainTypeEnum.Plains);
							}

							if ((this.parent.GameData.MapVisibility[i, j] & (0x1 << this.parent.GameData.HumanPlayerID)) != 0)
							{
								// Instruction address 0x0000:0x1ca4, size: 5
								this.parent.MapManagement.F0_2aea_11d4_DrawCellWithUnit(i, j);
							}
						}
					}
				}
			}
		}

		/// <summary>
		/// Get a sum of total population count for a given player
		/// </summary>
		/// <param name="playerID"></param>
		/// <returns></returns>
		public int GetPlayerPopulationCount(int playerID)
		{
			//this.oCPU.Log.EnterBlock($"F0_2dc4_02cd({playerID})");

			// function body
			int population = 0;

			for (int i = 0; i < 128; i++)
			{
				if (this.parent.GameData.Cities[i].PlayerID == playerID && this.parent.GameData.Cities[i].Status != CityStatusEnum.Invalid)
				{
					for (int j = 1; j <= this.parent.GameData.Cities[i].ActualSize; j++)
					{
						population += j;
					}
				}
			}

			return population;
		}

		/// <summary>
		/// Converts population value to string
		/// </summary>
		/// <param name="value"></param>
		public string PopulationValueToString(int value)
		{
			// function body
			return $"{(value * 1000):###,###,##0}";
		}
	}
}
