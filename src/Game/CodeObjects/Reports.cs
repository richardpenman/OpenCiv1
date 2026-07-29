namespace OpenCivOne
{
	public class Reports
	{
		private OpenCivOneGame parent;

		public Reports(OpenCivOneGame parent)
		{
			this.parent = parent;
		}

		/// <summary>
		/// Shows World Map report
		/// </summary>
		public void F12_0000_0000_ShowWorldMapReport()
		{
			//this.oCPU.Log.EnterBlock($"F12_0000_0000_ShowWorldMapPopup({flag})");

			// function body
			// Instruction address 0x0000:0x0025, size: 5
			this.parent.Graphics.F0_VGA_07d8_DrawImage(this.parent.Var_aa_Screen0_Rectangle, 0, 0, 320, 200, this.parent.Var_19d4_Screen1_Rectangle, 0, 0);

			// Instruction address 0x0000:0x0040, size: 5
			this.parent.DrawTools.FillRectangle(this.parent.Var_aa_Screen0_Rectangle, 0, 0, 320, 200, 0);

			int playerID = this.parent.GameData.HumanPlayerID;
			int xOffset = 40 - this.parent.GameData.Players[this.parent.GameData.HumanPlayerID].XStart;
			int yMedian;
			int yMinimum = int.MaxValue;
			int playerMask = 0x1 << this.parent.GameData.HumanPlayerID;
			int yMaximum = 0;

			if (this.parent.Var_d806_DebugFlag)
			{
				yMedian = 0;
				xOffset = 0;
			}
			else
			{
				for (int i = 0; i < 128; i++)
				{
					if (this.parent.GameData.Players[playerID].Units[i].UnitType != UnitTypeEnum.None)
					{
						if (yMinimum > this.parent.GameData.Players[playerID].Units[i].Position.Y)
						{
							yMinimum = this.parent.GameData.Players[playerID].Units[i].Position.Y;
						}

						if (yMaximum < this.parent.GameData.Players[playerID].Units[i].Position.Y)
						{
							yMaximum = this.parent.GameData.Players[playerID].Units[i].Position.Y;
						}
					}
				}

				for (int i = 0; i < 128; i++)
				{
					if (this.parent.GameData.Cities[i].PlayerID == this.parent.GameData.HumanPlayerID)
					{
						if (yMinimum > this.parent.GameData.Cities[i].Position.Y)
						{
							yMinimum = this.parent.GameData.Cities[i].Position.Y;
						}

						if (yMaximum < this.parent.GameData.Cities[i].Position.Y)
						{
							yMaximum = this.parent.GameData.Cities[i].Position.Y;
						}
					}
				}

				for (int i = 0; i < 80; i++)
				{
					if ((this.parent.GameData.MapVisibility[i, 0] & playerMask) != 0 || (this.parent.GameData.MapVisibility[i, 1] & playerMask) != 0)
					{
						yMinimum = 0;
						yMaximum = 49;
					}
				}

				yMedian = 25 - ((yMaximum + yMinimum) / 2);
			}

			for (int i = 0; i < 50; i++)
			{
				for (int j = 0; j < 80; j++)
				{
					if (this.parent.Var_d806_DebugFlag || 
						(this.parent.MapManagement.ValidateMapCoordinates(0, i + yMedian) && (this.parent.GameData.MapVisibility[j, i] & playerMask) != 0))
					{
						int cellX = this.parent.MapManagement.AdjustXPosition(j + xOffset) * 4;
						int cellY = (i + yMedian) * 4;
						TerrainTypeEnum terrainType = this.parent.MapManagement.GetTerrainType(j, i);

						// Instruction address 0x0000:0x0359, size: 5
						this.parent.Graphics.F0_VGA_07d8_DrawImage(this.parent.Var_19e8_Screen2_Rectangle,
							160 + ((int)terrainType * 4), 111 + (((j + i) & 1) * 4), 4, 4,
							this.parent.Var_aa_Screen0_Rectangle, cellX, cellY);

						// Instruction address 0x0000:0x01ae, size: 5
						if (this.parent.MapManagement.F0_2aea_1585_GetVisibleTerrainImprovements(j, i).HasFlag(TerrainImprovementFlagsEnum.City))
						{
							// Instruction address 0x0000:0x01c0, size: 5
							int cityID = this.parent.Tools.GetCityByLocation(j, i);

							if (this.parent.Var_d806_DebugFlag ||
								this.parent.GameData.Cities[cityID].VisibleSize != 0 || 
								this.parent.GameData.Cities[cityID].PlayerID == this.parent.GameData.HumanPlayerID)
							{
								// Instruction address 0x0000:0x01f5, size: 5
								int cityPlayerID = this.parent.MapManagement.F0_2aea_1369_GetCityOwner(j, i);

								// Instruction address 0x0000:0x02a2, size: 5
								this.parent.DrawTools.FillRectangle(this.parent.Var_aa_Screen0_Rectangle,
									cellX, cellY, 4, 4, this.parent.Array_1946_PlayerColours[cityPlayerID]);
							}
						}
						else
						{
							// Instruction address 0x0000:0x0215, size: 5
							int unitPlayerID = this.parent.MapManagement.F0_2aea_14e0_GetCellActiveUnitPlayerID(j, i);

							if (unitPlayerID != -1)
							{
								// Instruction address 0x0000:0x023d, size: 5
								int unitID = this.parent.MapManagement.F0_2aea_1458_GetCellActiveUnitID(j, i);

								if (this.parent.Var_d806_DebugFlag || 
									unitPlayerID == this.parent.GameData.HumanPlayerID ||
									(this.parent.GameData.Players[unitPlayerID].Units[unitID].VisibleByPlayer & (0x1 << this.parent.GameData.HumanPlayerID)) != 0)
								{
									// Instruction address 0x0000:0x0288, size: 5
									this.parent.DrawTools.FillRectangle(this.parent.Var_aa_Screen0_Rectangle, cellX + 1, cellY + 1, 3, 3, 0);

									// Instruction address 0x0000:0x02a2, size: 5
									this.parent.DrawTools.FillRectangle(this.parent.Var_aa_Screen0_Rectangle, cellX, cellY, 3, 3, 
										this.parent.Array_1946_PlayerColours[unitPlayerID]);
								}
							}
						}
					}
				}
			}

			// Instruction address 0x0000:0x037c, size: 5
			this.parent.Segment_2459.F0_2459_0918_WaitForKeyPressOrMouseClick();

			// Instruction address 0x0000:0x0399, size: 5
			this.parent.Graphics.F0_VGA_07d8_DrawImage(this.parent.Var_19d4_Screen1_Rectangle, 0, 0, 320, 200, this.parent.Var_aa_Screen0_Rectangle, 0, 0);
		}

		/// <summary>
		/// Shows Continent statistics report
		/// </summary>
		public void F12_0000_03ac_ShowContinentStatisticReport()
		{
			//this.oCPU.Log.EnterBlock("F12_0000_03ac()");

			// function body
			this.parent.Graphics.F0_VGA_07d8_DrawImage(this.parent.Var_aa_Screen0_Rectangle, 0, 0, 320, 200, this.parent.Var_19d4_Screen1_Rectangle, 0, 0);

			// Instruction address 0x0000:0x03d2, size: 5
			this.parent.DrawTools.FillRectangle(this.parent.Var_aa_Screen0_Rectangle, 0, 0, 320, 200, 4);

			for (int i = 1; i < 8; i++)
			{
				// Instruction address 0x0000:0x03f9, size: 5
				this.parent.DrawTools.DrawStringWithShadowToScreen0(
					this.parent.GameData.Players[i].Nationality, i * 40, 8, this.parent.Array_1946_PlayerColours[i]);

				// Instruction address 0x0000:0x0411, size: 5
				this.parent.Graphics.F0_VGA_0c3e_DrawBitmapToScreen(this.parent.Var_aa_Screen0_Rectangle, i * 40, 16, this.parent.Array_d4ce[64 + (i * 32)]);
			}

			for (int i = 1; i < 15; i++)
			{
				int y = 24 + i * 8;

				// Instruction address 0x0000:0x054e, size: 5
				this.parent.DrawTools.DrawStringToScreen0($"{this.parent.GameData.Continents[i].Size}", 8, y, 0);

				for (int j = 1; j < 8; j++)
				{
					if (this.parent.GameData.Players[j].Continents[i].Attack != 0)
					{
						// Instruction address 0x0000:0x04f4, size: 5
						this.parent.DrawTools.DrawStringToScreen0(
							$"{this.parent.GameData.Players[j].Continents[i].Defense}/{this.parent.GameData.Players[j].Continents[i].Attack}/" +
								$"{this.parent.GameData.Players[j].Continents[i].CityCount}",
							j * 40, y, this.parent.Array_1946_PlayerColours[j]);
					}
				}
			}

			// Instruction address 0x0000:0x055e, size: 5
			this.parent.Segment_2459.F0_2459_0918_WaitForKeyPressOrMouseClick();

			this.parent.Graphics.F0_VGA_07d8_DrawImage(this.parent.Var_19d4_Screen1_Rectangle, 0, 0, 320, 200, this.parent.Var_aa_Screen0_Rectangle, 0, 0);
		}

		/// <summary>
		/// Shows Power/Score Graph report
		/// </summary>
		public void F12_0000_0573_ShowPowerGraphReport()
		{
			//this.oCPU.Log.EnterBlock("F12_0000_0573()");

			// function body
			this.parent.Graphics.F0_VGA_07d8_DrawImage(this.parent.Var_aa_Screen0_Rectangle, 0, 0, 320, 200, this.parent.Var_19d4_Screen1_Rectangle, 0, 0);

			// Instruction address 0x0000:0x0599, size: 5
			this.parent.DrawTools.FillRectangle(this.parent.Var_aa_Screen0_Rectangle, 0, 0, 320, 200, 8);

			// Instruction address 0x0000:0x05b9, size: 5
			this.parent.DrawTools.DrawRectangle(4, 9, 311, 183, 0);

			this.parent.Var_aa_Screen0_Rectangle.FontID = 2;

			for (int i = 0; i <= this.parent.GameData.TurnCount && i <= 600; i += 50)
			{
				// Instruction address 0x0000:0x05ee, size: 5
				this.parent.Graphics.F0_VGA_0599_DrawLine(this.parent.Var_aa_Screen0_Rectangle, i / 2 + 4, 9, i / 2 + 4, 192, 0);

				if ((i / 100) == 0)
				{
					// Instruction address 0x0000:0x0667, size: 5
					this.parent.DrawTools.DrawStringToScreen0(
						this.parent.Segment_1238.YearToString(this.parent.Segment_1238.TurnCountToYear(i)), i / 2, 194, 15);
				}
			}

			this.parent.Var_aa_Screen0_Rectangle.FontID = 1;

			int maximumScoreValue = 50;

			for (int i = 1; i < 9; i++)
			{
				if ((this.parent.GameData.ActiveCivilizations & (0x1 << i)) != 0 || i <= this.parent.GameData.AIOpponentCount + 1)
				{
					// Instruction address 0x0000:0x070b, size: 5
					this.parent.DrawTools.DrawStringToScreen0(
						this.parent.GameData.Players[i].Nationality, 8, (i * 8) + 4, this.parent.Array_1946_PlayerColours[i]);

					for (int j = 0; this.parent.GameData.TurnCount / 4 > j; j++)
					{
						if (maximumScoreValue < this.parent.GameData.ScoreGraphData[j * 8 + i])
						{
							maximumScoreValue = this.parent.GameData.ScoreGraphData[j * 8 + i];
						}
					}
				}
			}

			// Instruction address 0x0000:0x072b, size: 5
			this.parent.DrawTools.DrawStringWithShadowToScreen0("CIVILIZATION POWERGraph", 100, 2, 15);

			for (int i = 1; i < 8; i++)
			{
				if ((this.parent.GameData.ActiveCivilizations & (0x1 << i)) != 0 || i <= this.parent.GameData.AIOpponentCount + 1)
				{
					int startX = 0;
					int startY = 0;

					for (int j = 0; this.parent.GameData.TurnCount / 4 > j; j++)
					{
						if (j < 150)
						{
							int endX = j * 2;
							int endY = (180 * this.parent.GameData.ScoreGraphData[j * 8 + i]) / maximumScoreValue;

							// Instruction address 0x0000:0x0785, size: 5
							this.parent.Graphics.F0_VGA_0599_DrawLine(this.parent.Var_aa_Screen0_Rectangle,
								startX + 4, 192 - startY, endX + 4, 192 - endY, this.parent.Array_1946_PlayerColours[i]);

							startX = endX;
							startY = endY;
						}
					}
				}
			}

			// Instruction address 0x0000:0x07ea, size: 5
			this.parent.Segment_2459.F0_2459_0918_WaitForKeyPressOrMouseClick();

			this.parent.Graphics.F0_VGA_07d8_DrawImage(this.parent.Var_19d4_Screen1_Rectangle, 0, 0, 320, 200, this.parent.Var_aa_Screen0_Rectangle, 0, 0);
		}

		/// <summary>
		/// Shows Wonders of the World report
		/// </summary>
		public void F12_0000_080d_ShowWondersOfTheWorldReport()
		{
			//this.oCPU.Log.EnterBlock("F12_0000_080d_ShowWondersOfTheWorldPopup()");

			// function body
			this.parent.Graphics.F0_VGA_07d8_DrawImage(this.parent.Var_aa_Screen0_Rectangle, 0, 0, 320, 200, this.parent.Var_19d4_Screen1_Rectangle, 0, 0);

			int y = 999;

			for(int i = 1; i < 22; i++)
			{
				if (this.parent.GameData.WonderCityID[i] != -1)
				{
					if (y > 180)
					{
						if (y != 999)
						{
							// Instruction address 0x0000:0x0925, size: 5
							this.parent.Segment_2459.F0_2459_0918_WaitForKeyPressOrMouseClick();
						}

						// Instruction address 0x0000:0x093e, size: 5
						this.parent.DrawTools.FillRectangle(this.parent.Var_aa_Screen0_Rectangle, 0, 0, 320, 200, 3);

						// Instruction address 0x0000:0x0956, size: 5
						this.parent.DrawTools.DrawStringWithShadowToScreen0("The Wonders of the World", 100, 12, 15);

						y = 32;
					}

					int wonderCityID = this.parent.GameData.WonderCityID[i];
					int wonderPlayerID = (wonderCityID < 0 || wonderCityID > 127) ? -1 : this.parent.GameData.Cities[wonderCityID].PlayerID;
					string wonderText;

					if (this.parent.GameData.WonderCityID[i] != 128)
					{
						/*if (i < 8)
						{
							wonderText = $"The {this.oParent.GameData.Wonders[i].Name} of {this.oParent.Segment_2459.F0_2459_08c6_GetCityName(wonderCityID)}. "+
								$"({this.oParent.GameData.Players[wonderPlayerID].Nationality})";
						}
						else
						{*/
							wonderText = $"{this.parent.GameData.Wonders[i].Name} in {this.parent.Segment_2459.F0_2459_08c6_GetCityName(wonderCityID)}. "+
								$"({this.parent.GameData.Players[wonderPlayerID].Nationality})";
						//}
					}
					else
					{
						wonderText = $"{((i < 8) ? "The " : "")}{this.parent.GameData.Wonders[i].Name} (Destroyed)";

						wonderPlayerID = 0;
					}

					// Instruction address 0x0000:0x08a9, size: 5
					this.parent.DrawTools.DrawRectangle(8, y, 303, 15, this.parent.Array_1946_PlayerColours[wonderPlayerID]);

					// Instruction address 0x0000:0x08c8, size: 5
					this.parent.CityWorker.F0_1d12_7045_DrawWonderOrImprovement(i + 24, 16, y + 3);

					// Instruction address 0x0000:0x08f3, size: 5
					this.parent.DrawTools.DrawStringToScreen0(this.parent.LanguageTools.F0_2f4d_04f7_TrimStringToWidth(wonderText, 270), 40, y + 5, 15);

					y += 24;
				}
			}

			// Instruction address 0x0000:0x09cf, size: 5
			this.parent.Segment_2459.F0_2459_0918_WaitForKeyPressOrMouseClick();

			this.parent.Graphics.F0_VGA_07d8_DrawImage(this.parent.Var_19d4_Screen1_Rectangle, 0, 0, 320, 200, this.parent.Var_aa_Screen0_Rectangle, 0, 0);
		}

		private string[] historicalNames = { "Herodotus", "Pliny", "Toynbee", "Gibbon" };
		private string[] reportTypes = { "Wealthiest", "Most Powerful", "Most Advanced", "Happiest", "Largest" };
		private string[] reportGrades = { "Glorious", "Great", "Fine", "Mediocre", "Puny", "Pathetic", "Hopeless" };

		/// <summary>
		/// Creates player history ranking report of random type
		/// </summary>
		public void F12_0000_09e2_PlayerHistoryRankingReport()
		{
			//this.oCPU.Log.EnterBlock("F12_0000_09e2()");

			// function body
			this.parent.Graphics.F0_VGA_07d8_DrawImage(this.parent.Var_aa_Screen0_Rectangle, 0, 0, 320, 200, this.parent.Var_19d4_Screen1_Rectangle, 0, 0);

			// Instruction address 0x0000:0x0a17, size: 5
			this.parent.DrawTools.FillRectangle(this.parent.Var_aa_Screen0_Rectangle, 0, 0, 320, 200, 3);

			// Instruction address 0x0000:0x09f8, size: 5
			int reportType = this.parent.CAPI.RNG.Next(5);

			// Instruction address 0x0000:0x0a5f, size: 5
			this.parent.DrawTools.DrawCenteredStringToScreen0($"{historicalNames[this.parent.CAPI.RNG.Next(4)]} completes his epic history on", 160, 4, 15);

			// Instruction address 0x0000:0x0aac, size: 5
			this.parent.DrawTools.DrawCenteredStringToScreen0($"'The {reportTypes[reportType]} Leaders in the World'.", 160, 12, 15);

			int[] playerGrades = new int[8];

			for (int i = 1; i < 8; i++)
			{
				playerGrades[i] = 0;
			}

			for (int i = 0; i < 128; i++)
			{
				if (this.parent.GameData.Cities[i].PlayerID != -1 && this.parent.GameData.Cities[i].StatusFlag != 0xff)
				{
					// Instruction address 0x0000:0x0aee, size: 5
					this.parent.CityWorker.F0_1d12_0045_ProcessCityState(i, -1);

					if (reportType == 3)
					{
						playerGrades[this.parent.GameData.Cities[i].PlayerID] += this.parent.GameData.Cities[i].ActualSize + this.parent.Var_70e2_CityHappyCitizenCount - this.parent.Var_70e4_CityUnhappyCitizenCount;
					}

					if (reportType == 4)
					{
						playerGrades[this.parent.GameData.Cities[i].PlayerID] += this.parent.GameData.Cities[i].ActualSize;
					}
				}
			}

			for (int i = 0; i < 8; i++)
			{
				switch (reportType)
				{
					case 0:
						playerGrades[i] = this.parent.GameData.Players[i].Coins;
						break;

					case 1:
						playerGrades[i] = this.parent.GameData.Players[i].MilitaryPower;
						break;

					case 2:
						playerGrades[i] = 0;

						for (int j = 0; j < 72; j++)
						{
							if (this.parent.Segment_1ade.F0_1ade_22b5_PlayerHasTechnology(i, (TechnologyAdvanceEnum)j))
							{
								playerGrades[i]++;
							}
						}
						break;
				}
			}

			int y = 32;
			int rank = 0;

			for (int i = 1; i < 8; i++)
			{
				if ((this.parent.GameData.ActiveCivilizations & (0x1 << i)) != 0)
				{
					int maximumGrade = 0;
					int selectedPlayerID = -1;

					for (int j = 1; j < 8; j++)
					{
						if (playerGrades[j] >= maximumGrade)
						{
							if ((this.parent.GameData.ActiveCivilizations & (0x1 << j)) != 0)
							{
								maximumGrade = playerGrades[j];
								selectedPlayerID = j;
							}
						}
					}

					if (selectedPlayerID != -1)
					{
						if (this.parent.Var_d806_DebugFlag || this.parent.GameData.DifficultyLevel == 0 || selectedPlayerID == this.parent.GameData.HumanPlayerID ||
							this.parent.GameData.Players[this.parent.GameData.HumanPlayerID].Diplomacy[selectedPlayerID].HasFlag(DiplomacyFlagsEnum.EmbassyEstablished))
						{
							// Instruction address 0x0000:0x0ca5, size: 5
							this.parent.CommonTools.WaitTimer(60);

							// Instruction address 0x0000:0x0ccd, size: 5
							this.parent.Segment_1238.F0_1238_14a3(selectedPlayerID,
								this.parent.Segment_1238.F0_1238_1c98_GetPalaceLevel(this.parent.GameData.Players[selectedPlayerID].Score),
								128, y - 13);

							// Instruction address 0x0000:0x0ce8, size: 5
							this.parent.DrawTools.DrawRectangle(8, y, 303, 15, this.parent.Array_1946_PlayerColours[selectedPlayerID]);

							// Instruction address 0x0000:0x0d03, size: 5
							this.parent.DrawTools.DrawStringToScreen0(
								$"{(rank + 1)}. The {reportGrades[rank]} {this.parent.GameData.Players[selectedPlayerID].Name} " +
									$"of the {this.parent.GameData.Players[selectedPlayerID].Nation}.", 40, y + 5, 15);

							y += 24;
						}

						this.parent.GameData.Players[selectedPlayerID].HistoryRankingScore += (short)(7 - rank);

						rank++;

						playerGrades[selectedPlayerID] = -1;
					}
				}
			}

			this.parent.GameData.Players[0].HistoryRankingScore++;

			// Instruction address 0x0000:0x0d53, size: 5
			this.parent.CommonTools.ClearKeyboardAndMouseEvents();

			// Instruction address 0x0000:0x0d58, size: 5
			this.parent.Segment_2459.F0_2459_0918_WaitForKeyPressOrMouseClick();

			this.parent.Graphics.F0_VGA_07d8_DrawImage(this.parent.Var_19d4_Screen1_Rectangle, 0, 0, 320, 200, this.parent.Var_aa_Screen0_Rectangle, 0, 0);
		}

		/// <summary>
		/// Shows Demographics report
		/// </summary>
		/// <param name="playerID"></param>
		public void F12_0000_0d6d_ShowsDemographicsReport(short playerID)
		{
			//this.oCPU.Log.EnterBlock($"F12_0000_0d6d_ShowsDemographicsPopup({playerID})");

			// function body
			this.parent.Graphics.F0_VGA_07d8_DrawImage(this.parent.Var_aa_Screen0_Rectangle, 0, 0, 320, 200, this.parent.Var_19d4_Screen1_Rectangle, 0, 0);

			// Instruction address 0x0000:0x0d94, size: 5
			this.parent.DrawTools.FillRectangle(this.parent.Var_aa_Screen0_Rectangle, 0, 0, 320, 200, 1);

			// Instruction address 0x0000:0x0dd1, size: 5
			this.parent.DrawTools.DrawCenteredStringToScreen0($"{this.parent.GameData.Players[playerID].Nationality} Demographics", 160, 4, 15);

			int[] cityGrowthCounts = new int[8];
			int[] happyValues = new int[8];
			int[] productivityValues = new int[8];
			int[] populationGrowthValues = new int[8];
			int[] educationCounts = new int[8];
			int[] productionValues = new int[8];
			int[] incomeValues = new int[8];
			int[] pollutionValues = new int[8];
			int[] citySizes = new int[8];
			int[] cityCounts = new int[8];

			int[] rankData = new int[8];

			for (int i = 0; i < 8; i++)
			{
				citySizes[i] = 1;
				populationGrowthValues[i] = 0;
				cityCounts[i] = 0;
				incomeValues[i] = 0;
				educationCounts[i] = 0;
				cityGrowthCounts[i] = 0;
				pollutionValues[i] = 0;
				happyValues[i] = 0;
				productivityValues[i] = 0;
				productionValues[i] = 0;
			}

			for (int i = 0; i < 80; i++)
			{
				for (int j = 2; j < 48; j++)
				{
					if (this.parent.MapManagement.GetTerrainType(i, j) != TerrainTypeEnum.Water)
					{
						int cityPlayerID = this.parent.MapManagement.F0_2aea_1369_GetCityOwner(i, j);

						if (cityPlayerID != -1)
						{
							cityCounts[cityPlayerID]++;
						}
					}
				}
			}

			for (int i = 0; i < 128; i++)
			{
				City city = this.parent.GameData.Cities[i];

				if (city.PlayerID != -1 && city.StatusFlag != 0xff)
				{
					// !!! Variables Var_6c98, Var_b882, Var_70e2, Var_70e4 and Var_70da_Arr
					// are processed by F0_1d12_0045_ProcessCityState, but we still don't know it's meaning
					this.parent.CityWorker.F0_1d12_0045_ProcessCityState(i, -1);

					int cityPlayerID = city.PlayerID;
					int actualCitySize = city.ActualSize;

					citySizes[cityPlayerID] += actualCitySize;
					happyValues[cityPlayerID] += actualCitySize + this.parent.Var_70e2_CityHappyCitizenCount - this.parent.Var_70e4_CityUnhappyCitizenCount;
					incomeValues[cityPlayerID] += (this.parent.Var_70da_Arr[3] * 2) + this.parent.Var_e17a;
					populationGrowthValues[cityPlayerID] += this.parent.Var_70da_Arr[0] - (actualCitySize * 2);
					productivityValues[cityPlayerID] += this.parent.Var_70da_Arr[0] + this.parent.Var_70da_Arr[1] + this.parent.Var_70da_Arr[2];
					productionValues[cityPlayerID] += this.parent.Var_70da_Arr[1];

					int local_68 = (this.parent.Var_70da_Arr[1] / this.parent.Var_6c98) - 20 + (actualCitySize * this.parent.Var_b882) / 4;

					if (local_68 > 0)
					{
						pollutionValues[cityPlayerID] += local_68;
					}

					if (this.parent.GameData.Cities[i].HasImprovement(ImprovementEnum.Granary))
					{
						cityGrowthCounts[cityPlayerID] += actualCitySize;
					}
				
					if (this.parent.GameData.Cities[i].HasImprovement(ImprovementEnum.Aqueduct))
					{						
						cityGrowthCounts[cityPlayerID] += actualCitySize;
					}
				
					if (this.parent.GameData.Cities[i].HasImprovement(ImprovementEnum.Library))
					{						
						educationCounts[cityPlayerID] += actualCitySize;
					}
				
					if (this.parent.GameData.Cities[i].HasImprovement(ImprovementEnum.University))
					{						
						educationCounts[cityPlayerID] += actualCitySize;
					}
				}
			}

			int yPosition = 24;

			// Instruction address 0x0000:0x0fbb, size: 5
			this.parent.DrawTools.DrawStringWithShadowToScreen0("Approval Rating:", 8, 24, 15);

			for (int i = 1; i < 8; i++)
			{
				rankData[i] = (50 * happyValues[i]) / citySizes[i];

				if (i == playerID)
				{
					// Instruction address 0x0000:0x102d, size: 5
					this.parent.DrawTools.DrawStringToScreen0($"{rankData[i]}%", 104, yPosition, 15);
				}
			}

			PrintPlayerRank(rankData, yPosition, "%");

			yPosition += 12;

			// Instruction address 0x0000:0x1064, size: 5
			this.parent.DrawTools.DrawStringWithShadowToScreen0("Population:", 8, yPosition, 15);

			for (int i = 1; i < 8; i++)
			{
				int populationCount = this.parent.Tools.GetPlayerPopulationCount(i);

				rankData[i] = populationCount;

				if (i == playerID)
				{
					// Instruction address 0x0000:0x10a1, size: 5
					this.parent.DrawTools.DrawStringToScreen0($"{this.parent.Tools.PopulationValueToString(populationCount)}", 104, yPosition, 15);
				}
			}

			PrintPlayerRank(rankData, yPosition, "0000");

			yPosition += 12;

			// !!! What does GNP mean, find the correct explanation and be descriptive
			// Instruction address 0x0000:0x10d8, size: 5
			this.parent.DrawTools.DrawStringWithShadowToScreen0("GNP:", 8, yPosition, 15);

			for (int i = 1; i < 8; i++)
			{
				rankData[i] = incomeValues[i];

				if (i == playerID)
				{
					// Instruction address 0x0000:0x1145, size: 5
					this.parent.DrawTools.DrawStringToScreen0($"{rankData[i]} million $", 104, yPosition, 15);
				}
			}

			PrintPlayerRank(rankData, yPosition, "M$");

			yPosition += 12;

			// Instruction address 0x0000:0x117c, size: 5
			this.parent.DrawTools.DrawStringWithShadowToScreen0("Mfg. Goods:", 8, yPosition, 15);

			for (int i = 1; i < 8; i++)
			{
				rankData[i] = productionValues[i];

				if (i == playerID)
				{
					// Instruction address 0x0000:0x11e4, size: 5
					this.parent.DrawTools.DrawStringToScreen0($"{rankData[i]} Mtons", 104, yPosition, 15);
				}
			}

			PrintPlayerRank(rankData, yPosition, " Mtons");

			yPosition += 12;

			// Instruction address 0x0000:0x121b, size: 5
			this.parent.DrawTools.DrawStringWithShadowToScreen0("Land Area:", 8, yPosition, 15);

			for (int i = 1; i < 8; i++)
			{
				rankData[i] = cityCounts[i];

				if (i == playerID)
				{
					// Instruction address 0x0000:0x1289, size: 5
					this.parent.DrawTools.DrawStringToScreen0($"{rankData[i]},000 sq.miles", 104, yPosition, 15);
				}
			}

			PrintPlayerRank(rankData, yPosition, ",000");

			yPosition += 12;

			// Instruction address 0x0000:0x12c0, size: 5
			this.parent.DrawTools.DrawStringWithShadowToScreen0("Literacy:", 8, yPosition, 15);

			for (int i = 1; i < 8; i++)
			{
				rankData[i] = ((citySizes[i] * 2 + educationCounts[i]) * 2) / citySizes[i];

				if (this.parent.Segment_1ade.F0_1ade_22b5_PlayerHasTechnology(i, TechnologyAdvanceEnum.Alphabet))
				{
					rankData[i] *= 2;
				}

				if (this.parent.Segment_1ade.F0_1ade_22b5_PlayerHasTechnology(i, TechnologyAdvanceEnum.Writing))
				{
					rankData[i] *= 2;
				}

				if (this.parent.Segment_1ade.F0_1ade_22b5_PlayerHasTechnology(i, TechnologyAdvanceEnum.Literacy))
				{
					rankData[i] *= 2;
				}

				if (this.parent.Segment_1ade.F0_1ade_22b5_PlayerHasTechnology(i, TechnologyAdvanceEnum.University))
				{
					rankData[i] *= 2;
				}

				rankData[i] = this.parent.Tools.CheckValueRange(rankData[i], 0, 100);

				if (i == playerID)
				{
					// Instruction address 0x0000:0x13bd, size: 5
					this.parent.DrawTools.DrawStringToScreen0($"{rankData[i]}%", 104, yPosition, 15);
				}
			}

			PrintPlayerRank(rankData, yPosition, "%");

			yPosition += 12;

			// Instruction address 0x0000:0x13f7, size: 5
			this.parent.DrawTools.DrawStringWithShadowToScreen0("Disease:", 8, yPosition, 15);

			for (int i = 1; i < 8; i++)
			{
				rankData[i] = (50 * citySizes[i]) / (citySizes[i] + cityGrowthCounts[i]);

				if (this.parent.Segment_1ade.F0_1ade_22b5_PlayerHasTechnology(i, TechnologyAdvanceEnum.Medicine))
				{
					rankData[i] /= 2;
				}

				cityGrowthCounts[i] = 1500 / (rankData[i] + 20);

				if (i == playerID)
				{
					// Instruction address 0x0000:0x14a5, size: 5
					this.parent.DrawTools.DrawStringToScreen0($"{rankData[i]}%", 104, yPosition, 15);
				}
			}

			PrintPlayerRank(rankData, -yPosition, "%");

			yPosition += 12;

			// Instruction address 0x0000:0x14e2, size: 5
			this.parent.DrawTools.DrawStringWithShadowToScreen0("Pollution:", 8, yPosition, 15);

			for (int i = 1; i < 8; i++)
			{
				rankData[i] = pollutionValues[i];
				cityGrowthCounts[i] -= (10 * pollutionValues[i]) / citySizes[i];

				if (i == playerID)
				{
					// Instruction address 0x0000:0x155c, size: 5
					this.parent.DrawTools.DrawStringToScreen0($"{rankData[i]}0 tons/year", 104, yPosition, 15);
				}
			}

			PrintPlayerRank(rankData, -yPosition, "0 tons");

			yPosition += 12;

			// Instruction address 0x0000:0x1596, size: 5
			this.parent.DrawTools.DrawStringWithShadowToScreen0("Life Expectancy:", 8, yPosition, 15);

			for (int i = 1; i < 8; i++)
			{
				rankData[i] = this.parent.Tools.CheckValueRange(cityGrowthCounts[i], 20, 99);

				if (i == playerID)
				{
					// Instruction address 0x0000:0x160e, size: 5
					this.parent.DrawTools.DrawStringToScreen0($"{rankData[i]} years", 104, yPosition, 15);
				}
			}

			PrintPlayerRank(rankData, yPosition, " years");

			yPosition += 12;

			// Instruction address 0x0000:0x1645, size: 5
			this.parent.DrawTools.DrawStringWithShadowToScreen0("Family Size:", 8, yPosition, 15);

			for (int i = 1; i < 8; i++)
			{
				rankData[i] = ((40 * populationGrowthValues[i]) / citySizes[i]) + 20;

				if (i == playerID)
				{
					// Instruction address 0x0000:0x16fc, size: 5
					this.parent.DrawTools.DrawStringToScreen0($"{rankData[i] / 10}.{rankData[i] % 10} children", 104, yPosition, 15);
				}

				rankData[i] /= 10;
			}

			PrintPlayerRank(rankData, yPosition, "");

			yPosition += 12;

			// Instruction address 0x0000:0x1749, size: 5
			this.parent.DrawTools.DrawStringWithShadowToScreen0("Military Service:", 8, yPosition, 15);

			for (int i = 1; i < 8; i++)
			{
				rankData[i] = (this.parent.GameData.Players[i].UnitCount * 10) / citySizes[i];

				if (i == playerID)
				{
					// Instruction address 0x0000:0x17ba, size: 5
					this.parent.DrawTools.DrawStringToScreen0($"{rankData[i]} years", 104, yPosition, 15);
				}
			}

			PrintPlayerRank(rankData, yPosition, " years");

			yPosition += 12;

			// Instruction address 0x0000:0x17f1, size: 5
			this.parent.DrawTools.DrawStringWithShadowToScreen0("Annual Income:", 8, yPosition, 15);

			for (int i = 1; i < 8; i++)
			{
				if (30000 / (this.parent.GameData.Players[i].DiscoveredTechnologyCount + 1) <= incomeValues[i])
				{
					rankData[i] = (incomeValues[i] / citySizes[i]) * this.parent.GameData.Players[i].DiscoveredTechnologyCount;
				}
				else
				{
					rankData[i] = (incomeValues[i] * this.parent.GameData.Players[i].DiscoveredTechnologyCount) / citySizes[i];
				}

				if (i == playerID)
				{
					// Instruction address 0x0000:0x1869, size: 5
					this.parent.DrawTools.DrawStringToScreen0($"{rankData[i]}$ per capita", 104, yPosition, 15);
				}
			}

			PrintPlayerRank(rankData, yPosition, "$");

			yPosition += 12;

			// Instruction address 0x0000:0x18c9, size: 5
			this.parent.DrawTools.DrawStringWithShadowToScreen0("Productivity:", 8, yPosition, 15);

			for (int i = 1; i < 8; i++)
			{
				rankData[i] = (productivityValues[i] * 10) / citySizes[i];

				if (i == playerID)
				{
					// Instruction address 0x0000:0x192b, size: 5
					this.parent.DrawTools.DrawStringToScreen0($"{rankData[i]}", 104, yPosition, 15);
				}
			}

			PrintPlayerRank(rankData, yPosition, "");

			// Instruction address 0x0000:0x1953, size: 5
			this.parent.Segment_2459.F0_2459_0918_WaitForKeyPressOrMouseClick();

			this.parent.Graphics.F0_VGA_07d8_DrawImage(this.parent.Var_19d4_Screen1_Rectangle, 0, 0, 320, 200, this.parent.Var_aa_Screen0_Rectangle, 0, 0);
		}

		/// <summary>
		/// Demographics report helper
		/// </summary>
		/// <param name="arrayPtr"></param>
		/// <param name="y"></param>
		/// <param name="text"></param>
		private void PrintPlayerRank(int[] rankData, int y, string text)
		{
			//this.oCPU.Log.EnterBlock($"F12_0000_1968({param1}, {yPos}, 0x{stringPtr:x4})");

			// function body
			int humanPlayerRank = 1;
			int maximumValue = -9999;
			int selectedPlayerID = this.parent.GameData.HumanPlayerID;

			for (int i = 1; i < 8; i++)
			{
				if ((this.parent.GameData.ActiveCivilizations & (0x1 << i)) != 0)
				{
					if (y < 0)
					{
						rankData[i] = -rankData[i];
					}

					if (rankData[i] > rankData[this.parent.GameData.HumanPlayerID])
					{
						humanPlayerRank++;
					}

					if (maximumValue < rankData[i])
					{
						maximumValue = rankData[i];
						selectedPlayerID = i;
					}
				}
			}

			// Instruction address 0x0000:0x1a64, size: 5
			this.parent.Graphics.F0_VGA_0599_DrawLine(this.parent.Var_aa_Screen0_Rectangle, 4, Math.Abs(y) - 3, 316, Math.Abs(y) - 3, 9);

			switch (humanPlayerRank)
			{
				case 1:
					// Instruction address 0x0000:0x1a84, size: 5
					this.parent.DrawTools.DrawStringWithShadow($"{humanPlayerRank}st", 192, Math.Abs(y), 15);
					break;

				case 2:
					// Instruction address 0x0000:0x1a84, size: 5
					this.parent.DrawTools.DrawStringWithShadow($"{humanPlayerRank}nd", 192, Math.Abs(y), 15);
					break;

				case 3:
					// Instruction address 0x0000:0x1a84, size: 5
					this.parent.DrawTools.DrawStringWithShadow($"{humanPlayerRank}rd", 192, Math.Abs(y), 15);
					break;

				default:
					// Instruction address 0x0000:0x1a84, size: 5
					this.parent.DrawTools.DrawStringWithShadow($"{humanPlayerRank}th", 192, Math.Abs(y), 15);
					break;
			}

			if (selectedPlayerID != this.parent.GameData.HumanPlayerID)
			{
				if (this.parent.GameData.Players[this.parent.GameData.HumanPlayerID].Diplomacy[selectedPlayerID].HasFlag(DiplomacyFlagsEnum.EmbassyEstablished))
				{
					string stats = $"({this.parent.GameData.Players[selectedPlayerID].Nation}: {Math.Abs(maximumValue)}{text})";

					// Instruction address 0x0000:0x1b4a, size: 5
					this.parent.DrawTools.DrawStringToScreen0(
						stats, 318 - this.parent.DrawTools.GetStringWidth(stats), Math.Abs(y), this.parent.Array_1946_PlayerColours[selectedPlayerID]);
				}
			}
		}

		/// <summary>
		/// Shows Top Five Cities in the World window
		/// </summary>
		public void ShowTopFiveCitiesReport()
		{
			//this.oCPU.Log.EnterBlock("F3_0000_09ac_ShowTopFiveCitiesPopup()");

			// function body
			int[] cityIDs = new int[5];
			int[] cityScores = new int[5];

			for (int i = 0; i < 5; i++)
			{
				cityIDs[i] = -1;
				cityScores[i] = -1;
			}

			for (int i = 0; i < 128; i++)
			{
				if (this.parent.GameData.Cities[i].StatusFlag != 0xff)
				{
					// Instruction address 0x0000:0x0a29, size: 5
					this.parent.CityWorker.F0_1d12_0045_ProcessCityState(i, -1);

					int cityScore = this.parent.GameData.Cities[i].ActualSize + this.parent.Var_70e2_CityHappyCitizenCount - this.parent.Var_70e4_CityUnhappyCitizenCount;

					for (int j = 1; j < 22; j++)
					{
						if (this.parent.GameData.WonderCityID[j] == i)
						{
							cityScore += 10;
						}
					}

					for (int j = 0; j < 5; j++)
					{
						if (cityScore > cityScores[j])
						{
							for (int k = 3; k > j; k--)
							{
								cityScores[k + 1] = cityScores[k];
								cityIDs[k + 1] = cityIDs[k];
							}

							cityScores[j] = cityScore;
							cityIDs[j] = i;
							break;
						}
					}
				}
			}

			this.parent.Graphics.F0_VGA_07d8_DrawImage(this.parent.Var_aa_Screen0_Rectangle, 0, 0, 320, 200, this.parent.Var_19d4_Screen1_Rectangle, 0, 0);

			// Instruction address 0x0000:0x0aac, size: 5
			this.parent.DrawTools.FillRectangle(this.parent.Var_aa_Screen0_Rectangle, 0, 0, 320, 200, 3);

			// Instruction address 0x0000:0x0ac4, size: 5
			this.parent.DrawTools.DrawStringWithShadowToScreen0("The Top Five Cities in the World", 80, 12, 15);

			int y = 32;

			for (int i = 0; i < 5; i++)
			{
				if (cityScores[i] != -1)
				{
					int cityID = cityIDs[i];

					// Instruction address 0x0000:0x0b8e, size: 5
					this.parent.DrawTools.DrawCenteredStringToScreen0(
						$"{i + 1}. {this.parent.Segment_2459.F0_2459_08c6_GetCityName(cityID)} " +
						$"({this.parent.GameData.Players[this.parent.GameData.Cities[cityID].PlayerID].Nationality})", 160, y + 3,
						this.parent.Array_1946_PlayerColours[this.parent.GameData.Cities[cityID].PlayerID]);

					// Instruction address 0x0000:0x0b9d, size: 5
					this.parent.CityWorker.F0_1d12_0045_ProcessCityState(cityID, -1);

					int wonderX = 48;

					// Instruction address 0x0000:0x0bc0, size: 5
					this.parent.CityWorker.F0_1d12_6ed4_DrawResources(cityID, 48, y + 10, this.parent.Var_e8b8, 160);

					// Instruction address 0x0000:0x0bd9, size: 5
					wonderX += this.parent.Tools.CheckValueRange(this.parent.GameData.Cities[cityID].ActualSize * 8, 0, 160) + 8;

					for (int j = 1; j < 22; j++)
					{
						if (this.parent.GameData.WonderCityID[j] == cityID)
						{
							if (wonderX < 293)
							{
								// Instruction address 0x0000:0x0c12, size: 5
								this.parent.CityWorker.F0_1d12_7045_DrawWonderOrImprovement(j + 24, wonderX, y + 12);
							}

							wonderX += 19;
						}
					}

					// Instruction address 0x0000:0x0c4b, size: 5
					this.parent.DrawTools.DrawRectangle(8, y, 303, 25, this.parent.Array_1946_PlayerColours[this.parent.GameData.Cities[cityID].PlayerID]);

					y += 32;
				}
			}

			// Instruction address 0x0000:0x0c63, size: 5
			this.parent.Segment_2459.F0_2459_0918_WaitForKeyPressOrMouseClick();

			this.parent.Graphics.F0_VGA_07d8_DrawImage(this.parent.Var_19d4_Screen1_Rectangle, 0, 0, 320, 200, this.parent.Var_aa_Screen0_Rectangle, 0, 0);
		}
	}
}
