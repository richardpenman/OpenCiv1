using System;
using IRB.VirtualCPU;

namespace OpenCivOne
{
	public class Reports
	{
		private OpenCivOneGame parent;
		private VCPU oCPU;

		public Reports(OpenCivOneGame parent)
		{
			this.parent = parent;
			this.oCPU = parent.CPU;
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
						(this.parent.MapManagement.F0_2aea_1326_ValidateMapCoordinates(0, i + yMedian) && (this.parent.GameData.MapVisibility[j, i] & playerMask) != 0))
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
							int cityID = this.parent.Tools.F0_2dc4_00ba_GetCityByLocation(j, i);

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
							int unitPlayerID = this.parent.MapManagement.F0_2aea_14e0_GetCellUnitPlayerID(j, i);

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
		public void F12_0000_03ac_ShowContinentStatsReport()
		{
			//this.oCPU.Log.EnterBlock("F12_0000_03ac()");

			// function body
			this.parent.Graphics.F0_VGA_07d8_DrawImage(this.parent.Var_aa_Screen0_Rectangle, 0, 0, 320, 200, this.parent.Var_19d4_Screen1_Rectangle, 0, 0);

			// Instruction address 0x0000:0x03d2, size: 5
			this.parent.DrawTools.FillRectangle(this.parent.Var_aa_Screen0_Rectangle, 0, 0, 320, 200, 4);

			for (int i = 1; i < 8; i++)
			{
				// Instruction address 0x0000:0x03f9, size: 5
				this.parent.DrawTools.F0_1182_0086_DrawStringWithShadowToScreen0(
					this.parent.GameData.Players[i].Nationality, i * 40, 8, this.parent.Array_1946_PlayerColours[i]);

				// Instruction address 0x0000:0x0411, size: 5
				this.parent.Graphics.F0_VGA_0c3e_DrawBitmapToScreen(this.parent.Var_aa_Screen0_Rectangle, i * 40, 16, this.parent.Array_d4ce[64 + (i * 32)]);
			}

			for (int i = 1; i < 15; i++)
			{
				int y = 24 + i * 8;

				// Instruction address 0x0000:0x054e, size: 5
				this.parent.DrawTools.F0_1182_005c_DrawStringToScreen0($"{this.parent.GameData.Continents[i].Size}", 8, y, 0);

				for (int j = 1; j < 8; j++)
				{
					if (this.parent.GameData.Players[j].Continents[i].Attack != 0)
					{
						// Instruction address 0x0000:0x04f4, size: 5
						this.parent.DrawTools.F0_1182_005c_DrawStringToScreen0(
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
					this.parent.DrawTools.F0_1182_005c_DrawStringToScreen0(
						this.parent.Segment_1238.GetYearAsString(this.parent.Segment_1238.TurnCountToYear(i)), i / 2, 194, 15);
				}
			}

			this.parent.Var_aa_Screen0_Rectangle.FontID = 1;

			int maximumScoreValue = 50;

			for (int i = 1; i < 9; i++)
			{
				if ((this.parent.GameData.ActiveCivilizations & (0x1 << i)) != 0 || i <= this.parent.GameData.AIOpponentCount + 1)
				{
					// Instruction address 0x0000:0x070b, size: 5
					this.parent.DrawTools.F0_1182_005c_DrawStringToScreen0(
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
			this.parent.DrawTools.F0_1182_0086_DrawStringWithShadowToScreen0("CIVILIZATION POWERGraph", 100, 2, 15);

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
						this.parent.DrawTools.F0_1182_0086_DrawStringWithShadowToScreen0("The Wonders of the World", 100, 12, 15);

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
					this.parent.CityWorker.F0_1d12_7045(i + 24, 16, y + 3);

					// Instruction address 0x0000:0x08f3, size: 5
					this.parent.DrawTools.F0_1182_005c_DrawStringToScreen0(this.parent.LanguageTools.F0_2f4d_04f7_TrimStringToWidth(wonderText, 270), 40, y + 5, 15);

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
			this.parent.DrawTools.F0_1182_00b3_DrawCenteredStringToScreen0($"{historicalNames[this.parent.CAPI.RNG.Next(4)]} completes his epic history on", 160, 4, 15);

			// Instruction address 0x0000:0x0aac, size: 5
			this.parent.DrawTools.F0_1182_00b3_DrawCenteredStringToScreen0($"'The {reportTypes[reportType]} Leaders in the World'.", 160, 12, 15);

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
						playerGrades[this.parent.GameData.Cities[i].PlayerID] += this.parent.GameData.Cities[i].ActualSize + this.parent.Var_70e2 - this.parent.Var_70e4;
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
							this.parent.GameData.Players[this.parent.GameData.HumanPlayerID].Diplomacy[selectedPlayerID].HasFlag(DiplomacyFlagsEnum.Unknown40))
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
							this.parent.DrawTools.F0_1182_005c_DrawStringToScreen0(
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
			this.oCPU.Log.EnterBlock($"F12_0000_0d6d_ShowsDemographicsPopup({playerID})");

			// function body
			this.oCPU.PUSH_UInt16(this.oCPU.BP.UInt16);
			this.oCPU.BP.UInt16 = this.oCPU.SP.UInt16;
			this.oCPU.SP.UInt16 = this.oCPU.SUB_UInt16(this.oCPU.SP.UInt16, 0xba);
			this.oCPU.PUSH_UInt16(this.oCPU.DI.UInt16);
			this.oCPU.PUSH_UInt16(this.oCPU.SI.UInt16);

			this.parent.Graphics.F0_VGA_07d8_DrawImage(this.parent.Var_aa_Screen0_Rectangle, 0, 0, 320, 200, this.parent.Var_19d4_Screen1_Rectangle, 0, 0);

			// Instruction address 0x0000:0x0d94, size: 5
			this.parent.DrawTools.FillRectangle(this.parent.Var_aa_Screen0_Rectangle, 0, 0, 320, 200, 1);

			// Instruction address 0x0000:0x0da9, size: 5
			this.parent.CAPI.strcpy(0xba06, this.parent.GameData.Players[playerID].Nationality);

			// Instruction address 0x0000:0x0db9, size: 5
			this.parent.CAPI.strcat(0xba06, " Demographics");

			// Instruction address 0x0000:0x0dd1, size: 5
			this.parent.DrawTools.F0_1182_00b3_DrawCenteredStringToScreen0(0xba06, 160, 4, 15);

			this.oCPU.WriteUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x56), 0x0);

		L0dde:
			this.oCPU.SI.UInt16 = this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x56));
			this.oCPU.SI.UInt16 = this.oCPU.SHL_UInt16(this.oCPU.SI.UInt16, 0x1);
			this.oCPU.SI.UInt16 = this.oCPU.ADD_UInt16(this.oCPU.SI.UInt16, this.oCPU.BP.UInt16);
			this.oCPU.WriteUInt16(this.oCPU.DS.UInt16, (ushort)(this.oCPU.SI.UInt16 - 0x98), 0x1);
			this.oCPU.AX.UInt16 = 0;
			this.oCPU.WriteUInt16(this.oCPU.DS.UInt16, (ushort)(this.oCPU.SI.UInt16 - 0x40), this.oCPU.AX.UInt16);
			this.oCPU.WriteUInt16(this.oCPU.DS.UInt16, (ushort)(this.oCPU.SI.UInt16 - 0xb8), this.oCPU.AX.UInt16);
			this.oCPU.WriteUInt16(this.oCPU.DS.UInt16, (ushort)(this.oCPU.SI.UInt16 - 0x78), this.oCPU.AX.UInt16);
			this.oCPU.WriteUInt16(this.oCPU.DS.UInt16, (ushort)(this.oCPU.SI.UInt16 - 0x52), this.oCPU.AX.UInt16);
			this.oCPU.WriteUInt16(this.oCPU.DS.UInt16, (ushort)(this.oCPU.SI.UInt16 - 0x10), this.oCPU.AX.UInt16);
			this.oCPU.WriteUInt16(this.oCPU.DS.UInt16, (ushort)(this.oCPU.SI.UInt16 - 0x88), this.oCPU.AX.UInt16);
			this.oCPU.WriteUInt16(this.oCPU.DS.UInt16, (ushort)(this.oCPU.SI.UInt16 - 0x20), this.oCPU.AX.UInt16);
			this.oCPU.WriteUInt16(this.oCPU.DS.UInt16, (ushort)(this.oCPU.SI.UInt16 - 0x30), this.oCPU.AX.UInt16);
			this.oCPU.WriteUInt16(this.oCPU.DS.UInt16, (ushort)(this.oCPU.SI.UInt16 - 0x66), this.oCPU.AX.UInt16);

			this.oCPU.WriteUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x56), this.oCPU.INC_UInt16(this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x56))));
			this.oCPU.CMP_UInt16(this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x56)), 0x8);
			if (this.oCPU.Flags.L) goto L0dde;

			this.oCPU.WriteUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x42), this.oCPU.AX.UInt16);
			goto L0e4f;

		L0e18:
			this.oCPU.WriteUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x54), this.oCPU.INC_UInt16(this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x54))));

		L0e1b:
			this.oCPU.CMP_UInt16(this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x54)), 0x30);
			if (this.oCPU.Flags.GE) goto L0e4c;

			// Instruction address 0x0000:0x0e27, size: 5
			this.oCPU.AX.Int16 = (short)this.parent.MapManagement.GetTerrainType(
				this.oCPU.ReadInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x42)),
				this.oCPU.ReadInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x54)));

			this.oCPU.CMP_UInt16(this.oCPU.AX.UInt16, 0xa);
			if (this.oCPU.Flags.E) goto L0e18;

			// Instruction address 0x0000:0x0e3a, size: 5
			this.oCPU.AX.Int16 = (short)this.parent.MapManagement.F0_2aea_1369_GetCityOwner(
				this.oCPU.ReadInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x42)),
				this.oCPU.ReadInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x54)));

			this.oCPU.SI.UInt16 = this.oCPU.AX.UInt16;
			this.oCPU.SI.UInt16 = this.oCPU.SHL_UInt16(this.oCPU.SI.UInt16, 0x1);
			this.oCPU.WriteUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 + this.oCPU.SI.UInt16 - 0xb8), 
				this.oCPU.INC_UInt16(this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 + this.oCPU.SI.UInt16 - 0xb8))));
			goto L0e18;

		L0e4c:
			this.oCPU.WriteUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x42), this.oCPU.INC_UInt16(this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x42))));

		L0e4f:
			this.oCPU.CMP_UInt16(this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x42)), 0x50);
			if (this.oCPU.Flags.GE) goto L0e5c;
			this.oCPU.WriteUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x54), 0x2);
			goto L0e1b;

		L0e5c:
			this.oCPU.WriteUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x56), 0x0);
			goto L0f84;

		L0e64:
			// Instruction address 0x0000:0x0e6b, size: 5
			this.parent.CityWorker.F0_1d12_0045_ProcessCityState(this.oCPU.ReadInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x56)), -1);

			this.oCPU.AX.UInt16 = 0x1c;
			this.oCPU.IMUL_UInt16(this.oCPU.AX, this.oCPU.DX, this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x56)));
			this.oCPU.SI.UInt16 = this.oCPU.AX.UInt16;
			this.oCPU.DI.UInt16 = (ushort)this.parent.GameData.Cities[this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x56))].PlayerID;
			this.oCPU.DI.UInt16 = this.oCPU.SHL_UInt16(this.oCPU.DI.UInt16, 0x1);
			this.oCPU.DI.UInt16 = this.oCPU.ADD_UInt16(this.oCPU.DI.UInt16, this.oCPU.BP.UInt16);
			this.oCPU.AX.LowUInt8 = (byte)this.parent.GameData.Cities[this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x56))].ActualSize;
			this.oCPU.CBW(this.oCPU.AX);
			this.oCPU.WriteUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0xba), this.oCPU.AX.UInt16);
			this.oCPU.WriteUInt16(this.oCPU.DS.UInt16, (ushort)(this.oCPU.DI.UInt16 - 0x98), this.oCPU.ADD_UInt16(this.oCPU.ReadUInt16(this.oCPU.DS.UInt16, (ushort)(this.oCPU.DI.UInt16 - 0x98)), this.oCPU.AX.UInt16));
			this.oCPU.AX.UInt16 = this.oCPU.ADD_UInt16(this.oCPU.AX.UInt16, (ushort)((short)this.parent.Var_70e2));
			this.oCPU.AX.UInt16 = this.oCPU.SUB_UInt16(this.oCPU.AX.UInt16, (ushort)((short)this.parent.Var_70e4));
			this.oCPU.WriteUInt16(this.oCPU.DS.UInt16, (ushort)(this.oCPU.DI.UInt16 - 0x20), this.oCPU.ADD_UInt16(this.oCPU.ReadUInt16(this.oCPU.DS.UInt16, (ushort)(this.oCPU.DI.UInt16 - 0x20)), this.oCPU.AX.UInt16));
			this.oCPU.AX.UInt16 = (ushort)((short)this.parent.Var_70da_Arr[3]);
			this.oCPU.AX.UInt16 = this.oCPU.SHL_UInt16(this.oCPU.AX.UInt16, 0x1);
			this.oCPU.AX.UInt16 = this.oCPU.ADD_UInt16(this.oCPU.AX.UInt16, (ushort)((short)this.parent.Var_e17a));
			this.oCPU.WriteUInt16(this.oCPU.DS.UInt16, (ushort)(this.oCPU.DI.UInt16 - 0x78), this.oCPU.ADD_UInt16(this.oCPU.ReadUInt16(this.oCPU.DS.UInt16, (ushort)(this.oCPU.DI.UInt16 - 0x78)), this.oCPU.AX.UInt16));
			this.oCPU.AX.UInt16 = (ushort)((short)this.parent.Var_70da_Arr[0]);
			this.oCPU.CX.UInt16 = this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0xba));
			this.oCPU.CX.UInt16 = this.oCPU.SHL_UInt16(this.oCPU.CX.UInt16, 0x1);
			this.oCPU.AX.UInt16 = this.oCPU.SUB_UInt16(this.oCPU.AX.UInt16, this.oCPU.CX.UInt16);
			this.oCPU.WriteUInt16(this.oCPU.DS.UInt16, (ushort)(this.oCPU.DI.UInt16 - 0x40), this.oCPU.ADD_UInt16(this.oCPU.ReadUInt16(this.oCPU.DS.UInt16, (ushort)(this.oCPU.DI.UInt16 - 0x40)), this.oCPU.AX.UInt16));
			this.oCPU.AX.UInt16 = (ushort)((short)this.parent.Var_70da_Arr[0]);
			this.oCPU.AX.UInt16 = this.oCPU.ADD_UInt16(this.oCPU.AX.UInt16, (ushort)((short)this.parent.Var_70da_Arr[1]));
			this.oCPU.AX.UInt16 = this.oCPU.ADD_UInt16(this.oCPU.AX.UInt16, (ushort)((short)this.parent.Var_70da_Arr[2]));
			this.oCPU.WriteUInt16(this.oCPU.DS.UInt16, (ushort)(this.oCPU.DI.UInt16 - 0x30), this.oCPU.ADD_UInt16(this.oCPU.ReadUInt16(this.oCPU.DS.UInt16, (ushort)(this.oCPU.DI.UInt16 - 0x30)), this.oCPU.AX.UInt16));
			this.oCPU.AX.UInt16 = (ushort)((short)this.parent.Var_70da_Arr[1]);
			this.oCPU.WriteUInt16(this.oCPU.DS.UInt16, (ushort)(this.oCPU.DI.UInt16 - 0x66), this.oCPU.ADD_UInt16(this.oCPU.ReadUInt16(this.oCPU.DS.UInt16, (ushort)(this.oCPU.DI.UInt16 - 0x66)), this.oCPU.AX.UInt16));
			this.oCPU.CWD(this.oCPU.AX, this.oCPU.DX);
			this.oCPU.CX.UInt16 = (ushort)((short)this.parent.Var_6c98);
			this.oCPU.IDIV_UInt16(this.oCPU.AX, this.oCPU.DX, this.oCPU.CX.UInt16);
			this.oCPU.AX.UInt16 = this.oCPU.SUB_UInt16(this.oCPU.AX.UInt16, 0x14);
			this.oCPU.WriteUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x68), this.oCPU.AX.UInt16);
			this.oCPU.AX.UInt16 = this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0xba));
			this.oCPU.IMUL_UInt16(this.oCPU.AX, this.oCPU.DX, (ushort)((short)this.parent.Var_b882));
			this.oCPU.CWD(this.oCPU.AX, this.oCPU.DX);
			this.oCPU.AX.UInt16 = this.oCPU.XOR_UInt16(this.oCPU.AX.UInt16, this.oCPU.DX.UInt16);
			this.oCPU.AX.UInt16 = this.oCPU.SUB_UInt16(this.oCPU.AX.UInt16, this.oCPU.DX.UInt16);
			this.oCPU.CX.UInt16 = 0x2;
			this.oCPU.AX.UInt16 = this.oCPU.SAR_UInt16(this.oCPU.AX.UInt16, this.oCPU.CX.LowUInt8);
			this.oCPU.AX.UInt16 = this.oCPU.XOR_UInt16(this.oCPU.AX.UInt16, this.oCPU.DX.UInt16);
			this.oCPU.AX.UInt16 = this.oCPU.SUB_UInt16(this.oCPU.AX.UInt16, this.oCPU.DX.UInt16);
			this.oCPU.WriteUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x68), this.oCPU.ADD_UInt16(this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x68)), this.oCPU.AX.UInt16));
			this.oCPU.CMP_UInt16(this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x68)), 0x0);
			if (this.oCPU.Flags.LE) goto L0eff;
			this.oCPU.AX.UInt16 = this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x68));
			this.oCPU.WriteUInt16(this.oCPU.DS.UInt16, (ushort)(this.oCPU.DI.UInt16 - 0x88), this.oCPU.ADD_UInt16(this.oCPU.ReadUInt16(this.oCPU.DS.UInt16, (ushort)(this.oCPU.DI.UInt16 - 0x88)), this.oCPU.AX.UInt16));

		L0eff:
			this.oCPU.AX.UInt16 = 28;
			this.oCPU.IMUL_UInt16(this.oCPU.AX, this.oCPU.DX, this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x56)));
			this.oCPU.SI.UInt16 = this.oCPU.AX.UInt16;
			this.oCPU.TEST_UInt16(this.parent.GameData.Cities[this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x56))].ImprovementFlags0, 0x4);
			if (this.oCPU.Flags.E) goto L0f1f;
			this.oCPU.DI.UInt16 = (ushort)this.parent.GameData.Cities[this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x56))].PlayerID;
			this.oCPU.DI.UInt16 = this.oCPU.SHL_UInt16(this.oCPU.DI.UInt16, 0x1);
			this.oCPU.AX.LowUInt8 = (byte)this.parent.GameData.Cities[this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x56))].ActualSize;
			this.oCPU.CBW(this.oCPU.AX);
			this.oCPU.WriteUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 + this.oCPU.DI.UInt16 - 0x10), 
				this.oCPU.ADD_UInt16(this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 + this.oCPU.DI.UInt16 - 0x10)), this.oCPU.AX.UInt16));

		L0f1f:
			this.oCPU.AX.UInt16 = 28;
			this.oCPU.IMUL_UInt16(this.oCPU.AX, this.oCPU.DX, this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x56)));
			this.oCPU.SI.UInt16 = this.oCPU.AX.UInt16;
			this.oCPU.TEST_UInt16(this.parent.GameData.Cities[this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x56))].ImprovementFlags0, 0x100);
			if (this.oCPU.Flags.E) goto L0f40;
			this.oCPU.DI.UInt16 = (ushort)this.parent.GameData.Cities[this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x56))].PlayerID;
			this.oCPU.DI.UInt16 = this.oCPU.SHL_UInt16(this.oCPU.DI.UInt16, 0x1);
			this.oCPU.AX.LowUInt8 = (byte)this.parent.GameData.Cities[this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x56))].ActualSize;
			this.oCPU.CBW(this.oCPU.AX);
			this.oCPU.WriteUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 + this.oCPU.DI.UInt16 - 0x10), 
				this.oCPU.ADD_UInt16(this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 + this.oCPU.DI.UInt16 - 0x10)), this.oCPU.AX.UInt16));

		L0f40:
			this.oCPU.AX.UInt16 = 28;
			this.oCPU.IMUL_UInt16(this.oCPU.AX, this.oCPU.DX, this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x56)));
			this.oCPU.SI.UInt16 = this.oCPU.AX.UInt16;
			this.oCPU.TEST_UInt16(this.parent.GameData.Cities[this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x56))].ImprovementFlags0, 0x20);
			if (this.oCPU.Flags.E) goto L0f60;
			this.oCPU.DI.UInt16 = (ushort)this.parent.GameData.Cities[this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x56))].PlayerID;
			this.oCPU.DI.UInt16 = this.oCPU.SHL_UInt16(this.oCPU.DI.UInt16, 0x1);
			this.oCPU.AX.LowUInt8 = (byte)this.parent.GameData.Cities[this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x56))].ActualSize;
			this.oCPU.CBW(this.oCPU.AX);
			this.oCPU.WriteUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 + this.oCPU.DI.UInt16 - 0x52), 
				this.oCPU.ADD_UInt16(this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 + this.oCPU.DI.UInt16 - 0x52)), this.oCPU.AX.UInt16));

		L0f60:
			this.oCPU.AX.UInt16 = 28;
			this.oCPU.IMUL_UInt16(this.oCPU.AX, this.oCPU.DX, this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x56)));
			this.oCPU.SI.UInt16 = this.oCPU.AX.UInt16;
			this.oCPU.TEST_UInt16(this.parent.GameData.Cities[this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x56))].ImprovementFlags0, 0x800);
			if (this.oCPU.Flags.E) goto L0f81;
			this.oCPU.DI.UInt16 = (ushort)this.parent.GameData.Cities[this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x56))].PlayerID;
			this.oCPU.DI.UInt16 = this.oCPU.SHL_UInt16(this.oCPU.DI.UInt16, 0x1);
			this.oCPU.AX.LowUInt8 = (byte)this.parent.GameData.Cities[this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x56))].ActualSize;
			this.oCPU.CBW(this.oCPU.AX);
			this.oCPU.WriteUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 + this.oCPU.DI.UInt16 - 0x52), 
				this.oCPU.ADD_UInt16(this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 + this.oCPU.DI.UInt16 - 0x52)), this.oCPU.AX.UInt16));

		L0f81:
			this.oCPU.WriteUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x56), this.oCPU.INC_UInt16(this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x56))));

		L0f84:
			this.oCPU.CMP_UInt16(this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x56)), 0x80);
			if (this.oCPU.Flags.GE) goto L0fa6;
			this.oCPU.AX.UInt16 = 0x1c;
			this.oCPU.IMUL_UInt16(this.oCPU.AX, this.oCPU.DX, this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x56)));
			this.oCPU.SI.UInt16 = this.oCPU.AX.UInt16;
			if (this.parent.GameData.Cities[this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x56))].PlayerID == -1) goto L0f81;
			this.oCPU.CMP_UInt8(this.parent.GameData.Cities[this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x56))].StatusFlag, 0xff);
			if (this.oCPU.Flags.E) goto L0fa4;
			goto L0e64;

		L0fa4:
			goto L0f81;

		L0fa6:
			this.oCPU.WriteUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x54), 0x18);

			// Instruction address 0x0000:0x0fbb, size: 5
			this.parent.DrawTools.F0_1182_0086_DrawStringWithShadowToScreen0("Approval Rating:", 8, 24, 15);

			this.oCPU.WriteUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x56), 0x1);

		L0fc8:
			this.oCPU.SI.UInt16 = this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x56));
			this.oCPU.SI.UInt16 = this.oCPU.SHL_UInt16(this.oCPU.SI.UInt16, 0x1);
			this.oCPU.SI.UInt16 = this.oCPU.ADD_UInt16(this.oCPU.SI.UInt16, this.oCPU.BP.UInt16);
			this.oCPU.AX.UInt16 = 0x32;
			this.oCPU.IMUL_UInt16(this.oCPU.AX, this.oCPU.DX, this.oCPU.ReadUInt16(this.oCPU.DS.UInt16, (ushort)(this.oCPU.SI.UInt16 - 0x20)));
			this.oCPU.CWD(this.oCPU.AX, this.oCPU.DX);
			this.oCPU.CX.UInt16 = this.oCPU.ReadUInt16(this.oCPU.DS.UInt16, (ushort)(this.oCPU.SI.UInt16 - 0x98));
			this.oCPU.IDIV_UInt16(this.oCPU.AX, this.oCPU.DX, this.oCPU.CX.UInt16);
			this.oCPU.WriteUInt16(this.oCPU.DS.UInt16, (ushort)(this.oCPU.SI.UInt16 - 0xa8), this.oCPU.AX.UInt16);
			this.oCPU.AX.UInt16 = (ushort)playerID;
			this.oCPU.CMP_UInt16(this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x56)), this.oCPU.AX.UInt16);
			if (this.oCPU.Flags.NE) goto L1035;
			this.oCPU.WriteUInt8(this.oCPU.DS.UInt16, 0xba06, 0x0);

			// Instruction address 0x0000:0x1006, size: 5
			this.parent.CAPI.strcat(0xba06,
				this.parent.CAPI.itoa((short)this.oCPU.ReadUInt16(this.oCPU.DS.UInt16, (ushort)(this.oCPU.SI.UInt16 - 0xa8)), 10));

			// Instruction address 0x0000:0x1016, size: 5
			this.parent.CAPI.strcat(0xba06, "%");

			// Instruction address 0x0000:0x102d, size: 5
			this.parent.DrawTools.F0_1182_005c_DrawStringToScreen0(0xba06, 104, this.oCPU.ReadInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x54)), 15);

		L1035:
			this.oCPU.WriteUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x56), this.oCPU.INC_UInt16(this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x56))));
			this.oCPU.CMP_UInt16(this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x56)), 0x8);
			if (this.oCPU.Flags.L) goto L0fc8;

			F12_0000_1968((ushort)(this.oCPU.BP.UInt16 - 0xa8), this.oCPU.ReadInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x54)), "%");

			this.oCPU.WriteUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x54), 
				this.oCPU.ADD_UInt16(this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x54)), 0xc));

			// Instruction address 0x0000:0x1064, size: 5
			this.parent.DrawTools.F0_1182_0086_DrawStringWithShadowToScreen0("Population:", 8, this.oCPU.ReadInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x54)), 15);

			this.oCPU.WriteUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x56), 0x1);

		L1071:
			this.oCPU.WriteUInt8(this.oCPU.DS.UInt16, 0xba06, 0x0);

			// Instruction address 0x0000:0x1079, size: 5
			this.oCPU.AX.Int16 = (short)this.parent.Tools.F0_2dc4_02cd_GetPlayerTotalPopulationCount(
				this.oCPU.ReadInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x56)));

			this.parent.CAPI.strcat(0xba06, this.parent.Tools.F0_2dc4_0337_PopulationValueToString(this.oCPU.AX.Int16));

			this.oCPU.SI.UInt16 = this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x56));
			this.oCPU.SI.UInt16 = this.oCPU.SHL_UInt16(this.oCPU.SI.UInt16, 0x1);
			this.oCPU.WriteUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 + this.oCPU.SI.UInt16 - 0xa8), this.oCPU.AX.UInt16);
			this.oCPU.AX.UInt16 = (ushort)playerID;
			this.oCPU.CMP_UInt16(this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x56)), this.oCPU.AX.UInt16);
			if (this.oCPU.Flags.NE) goto L10a9;

			// Instruction address 0x0000:0x10a1, size: 5
			this.parent.DrawTools.F0_1182_005c_DrawStringToScreen0(0xba06, 104, this.oCPU.ReadInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x54)), 15);

		L10a9:
			this.oCPU.WriteUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x56), this.oCPU.INC_UInt16(this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x56))));
			this.oCPU.CMP_UInt16(this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x56)), 0x8);
			if (this.oCPU.Flags.L) goto L1071;

			F12_0000_1968((ushort)(this.oCPU.BP.UInt16 - 0xa8), this.oCPU.ReadInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x54)), "0000");

			this.oCPU.WriteUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x54), 
				this.oCPU.ADD_UInt16(this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x54)), 0xc));

			// Instruction address 0x0000:0x10d8, size: 5
			this.parent.DrawTools.F0_1182_0086_DrawStringWithShadowToScreen0("GNP:", 8, this.oCPU.ReadInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x54)), 15);

			this.oCPU.WriteUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x56), 0x1);

		L10e5:
			this.oCPU.WriteUInt8(this.oCPU.DS.UInt16, 0xba06, 0x0);
			this.oCPU.SI.UInt16 = this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x56));
			this.oCPU.SI.UInt16 = this.oCPU.SHL_UInt16(this.oCPU.SI.UInt16, 0x1);
			this.oCPU.SI.UInt16 = this.oCPU.ADD_UInt16(this.oCPU.SI.UInt16, this.oCPU.BP.UInt16);
			this.oCPU.AX.UInt16 = this.oCPU.ReadUInt16(this.oCPU.DS.UInt16, (ushort)(this.oCPU.SI.UInt16 - 0x78));
			this.oCPU.WriteUInt16(this.oCPU.DS.UInt16, (ushort)(this.oCPU.SI.UInt16 - 0xa8), this.oCPU.AX.UInt16);
			this.oCPU.WriteUInt8(this.oCPU.DS.UInt16, 0xba06, 0x0);

			// Instruction address 0x0000:0x1116, size: 5
			this.parent.CAPI.strcat(0xba06,
				this.parent.CAPI.itoa((short)this.oCPU.ReadUInt16(this.oCPU.DS.UInt16, (ushort)(this.oCPU.SI.UInt16 - 0xa8)), 10));

			// Instruction address 0x0000:0x1126, size: 5
			this.parent.CAPI.strcat(0xba06, " million $");

			this.oCPU.AX.UInt16 = (ushort)playerID;
			this.oCPU.CMP_UInt16(this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x56)), this.oCPU.AX.UInt16);
			if (this.oCPU.Flags.NE) goto L114d;

			// Instruction address 0x0000:0x1145, size: 5
			this.parent.DrawTools.F0_1182_005c_DrawStringToScreen0(0xba06, 104, this.oCPU.ReadInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x54)), 15);

		L114d:
			this.oCPU.WriteUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x56), this.oCPU.INC_UInt16(this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x56))));
			this.oCPU.CMP_UInt16(this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x56)), 0x8);
			if (this.oCPU.Flags.L) goto L10e5;

			F12_0000_1968((ushort)(this.oCPU.BP.UInt16 - 0xa8), this.oCPU.ReadInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x54)), "M$");

			this.oCPU.WriteUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x54), 
				this.oCPU.ADD_UInt16(this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x54)), 0xc));

			// Instruction address 0x0000:0x117c, size: 5
			this.parent.DrawTools.F0_1182_0086_DrawStringWithShadowToScreen0("Mfg. Goods:", 8, this.oCPU.ReadInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x54)), 15);

			this.oCPU.WriteUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x56), 0x1);

		L1189:
			this.oCPU.SI.UInt16 = this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x56));
			this.oCPU.SI.UInt16 = this.oCPU.SHL_UInt16(this.oCPU.SI.UInt16, 0x1);
			this.oCPU.SI.UInt16 = this.oCPU.ADD_UInt16(this.oCPU.SI.UInt16, this.oCPU.BP.UInt16);
			this.oCPU.AX.UInt16 = this.oCPU.ReadUInt16(this.oCPU.DS.UInt16, (ushort)(this.oCPU.SI.UInt16 - 0x66));
			this.oCPU.WriteUInt16(this.oCPU.DS.UInt16, (ushort)(this.oCPU.SI.UInt16 - 0xa8), this.oCPU.AX.UInt16);
			this.oCPU.AX.UInt16 = (ushort)playerID;
			this.oCPU.CMP_UInt16(this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x56)), this.oCPU.AX.UInt16);
			if (this.oCPU.Flags.NE) goto L11ec;
			this.oCPU.WriteUInt8(this.oCPU.DS.UInt16, 0xba06, 0x0);

			// Instruction address 0x0000:0x11bd, size: 5
			this.parent.CAPI.strcat(0xba06,
				this.parent.CAPI.itoa((short)this.oCPU.ReadUInt16(this.oCPU.DS.UInt16, (ushort)(this.oCPU.SI.UInt16 - 0xa8)), 10));

			// Instruction address 0x0000:0x11cd, size: 5
			this.parent.CAPI.strcat(0xba06, " Mtons");

			// Instruction address 0x0000:0x11e4, size: 5
			this.parent.DrawTools.F0_1182_005c_DrawStringToScreen0(0xba06, 104, this.oCPU.ReadInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x54)), 15);

		L11ec:
			this.oCPU.WriteUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x56), 
				this.oCPU.INC_UInt16(this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x56))));
			this.oCPU.CMP_UInt16(this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x56)), 0x8);
			if (this.oCPU.Flags.L) goto L1189;

			F12_0000_1968((ushort)(this.oCPU.BP.UInt16 - 0xa8), this.oCPU.ReadInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x54)), " Mtons");

			this.oCPU.WriteUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x54), 
				this.oCPU.ADD_UInt16(this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x54)), 0xc));

			// Instruction address 0x0000:0x121b, size: 5
			this.parent.DrawTools.F0_1182_0086_DrawStringWithShadowToScreen0("Land Area:", 8, this.oCPU.ReadInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x54)), 15);

			this.oCPU.WriteUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x56), 0x1);

		L1228:
			this.oCPU.WriteUInt8(this.oCPU.DS.UInt16, 0xba06, 0x0);
			this.oCPU.SI.UInt16 = this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x56));
			this.oCPU.SI.UInt16 = this.oCPU.SHL_UInt16(this.oCPU.SI.UInt16, 0x1);
			this.oCPU.SI.UInt16 = this.oCPU.ADD_UInt16(this.oCPU.SI.UInt16, this.oCPU.BP.UInt16);
			this.oCPU.AX.UInt16 = this.oCPU.ReadUInt16(this.oCPU.DS.UInt16, (ushort)(this.oCPU.SI.UInt16 - 0xb8));
			this.oCPU.WriteUInt16(this.oCPU.DS.UInt16, (ushort)(this.oCPU.SI.UInt16 - 0xa8), this.oCPU.AX.UInt16);
			this.oCPU.WriteUInt8(this.oCPU.DS.UInt16, 0xba06, 0x0);

			// Instruction address 0x0000:0x125a, size: 5
			this.parent.CAPI.strcat(0xba06,
				this.parent.CAPI.itoa((short)this.oCPU.ReadUInt16(this.oCPU.DS.UInt16, (ushort)(this.oCPU.SI.UInt16 - 0xa8)), 10));

			// Instruction address 0x0000:0x126a, size: 5
			this.parent.CAPI.strcat(0xba06, ",000 sq.miles");

			this.oCPU.AX.UInt16 = (ushort)playerID;
			this.oCPU.CMP_UInt16(this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x56)), this.oCPU.AX.UInt16);
			if (this.oCPU.Flags.NE) goto L1291;

			// Instruction address 0x0000:0x1289, size: 5
			this.parent.DrawTools.F0_1182_005c_DrawStringToScreen0(0xba06, 104, this.oCPU.ReadInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x54)), 15);

		L1291:
			this.oCPU.WriteUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x56), 
				this.oCPU.INC_UInt16(this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x56))));
			this.oCPU.CMP_UInt16(this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x56)), 0x8);
			if (this.oCPU.Flags.L) goto L1228;

			F12_0000_1968((ushort)(this.oCPU.BP.UInt16 - 0xa8), this.oCPU.ReadInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x54)), ",000");

			this.oCPU.WriteUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x54), 
				this.oCPU.ADD_UInt16(this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x54)), 0xc));

			// Instruction address 0x0000:0x12c0, size: 5
			this.parent.DrawTools.F0_1182_0086_DrawStringWithShadowToScreen0("Literacy:", 8, this.oCPU.ReadInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x54)), 15);

			this.oCPU.WriteUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x56), 0x1);

		L12cd:
			this.oCPU.SI.UInt16 = this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x56));
			this.oCPU.SI.UInt16 = this.oCPU.SHL_UInt16(this.oCPU.SI.UInt16, 0x1);
			this.oCPU.SI.UInt16 = this.oCPU.ADD_UInt16(this.oCPU.SI.UInt16, this.oCPU.BP.UInt16);
			this.oCPU.AX.UInt16 = this.oCPU.ReadUInt16(this.oCPU.DS.UInt16, (ushort)(this.oCPU.SI.UInt16 - 0x98));
			this.oCPU.AX.UInt16 = this.oCPU.SHL_UInt16(this.oCPU.AX.UInt16, 0x1);
			this.oCPU.AX.UInt16 = this.oCPU.ADD_UInt16(this.oCPU.AX.UInt16, this.oCPU.ReadUInt16(this.oCPU.DS.UInt16, (ushort)(this.oCPU.SI.UInt16 - 0x52)));
			this.oCPU.AX.UInt16 = this.oCPU.SHL_UInt16(this.oCPU.AX.UInt16, 0x1);
			this.oCPU.CWD(this.oCPU.AX, this.oCPU.DX);
			this.oCPU.CX.UInt16 = this.oCPU.ReadUInt16(this.oCPU.DS.UInt16, (ushort)(this.oCPU.SI.UInt16 - 0x98));
			this.oCPU.IDIV_UInt16(this.oCPU.AX, this.oCPU.DX, this.oCPU.CX.UInt16);
			this.oCPU.WriteUInt16(this.oCPU.DS.UInt16, (ushort)(this.oCPU.SI.UInt16 - 0xa8), this.oCPU.AX.UInt16);

			// Instruction address 0x0000:0x12f0, size: 5
			this.oCPU.AX.UInt16 = (ushort)(this.parent.Segment_1ade.F0_1ade_22b5_PlayerHasTechnology(this.oCPU.ReadInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x56)),
				TechnologyAdvanceEnum.Alphabet) ? 1 : 0);
			this.oCPU.AX.UInt16 = this.oCPU.OR_UInt16(this.oCPU.AX.UInt16, this.oCPU.AX.UInt16);
			if (this.oCPU.Flags.NE)
			{
				this.oCPU.WriteUInt16(this.oCPU.DS.UInt16, (ushort)(this.oCPU.SI.UInt16 - 0xa8), this.oCPU.SHL_UInt16(this.oCPU.ReadUInt16(this.oCPU.DS.UInt16, (ushort)(this.oCPU.SI.UInt16 - 0xa8)), 0x1));
			}

			// Instruction address 0x0000:0x1307, size: 5
			this.oCPU.AX.UInt16 = (ushort)(this.parent.Segment_1ade.F0_1ade_22b5_PlayerHasTechnology(this.oCPU.ReadInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x56)),
				TechnologyAdvanceEnum.Writing) ? 1 : 0);
			this.oCPU.AX.UInt16 = this.oCPU.OR_UInt16(this.oCPU.AX.UInt16, this.oCPU.AX.UInt16);
			if (this.oCPU.Flags.NE)
			{
				this.oCPU.SI.UInt16 = this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x56));
				this.oCPU.SI.UInt16 = this.oCPU.SHL_UInt16(this.oCPU.SI.UInt16, 0x1);
				this.oCPU.WriteUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 + this.oCPU.SI.UInt16 - 0xa8), 
					this.oCPU.SHL_UInt16(this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 + this.oCPU.SI.UInt16 - 0xa8)), 0x1));
			}

			// Instruction address 0x0000:0x1323, size: 5
			this.oCPU.AX.UInt16 = (ushort)(this.parent.Segment_1ade.F0_1ade_22b5_PlayerHasTechnology(this.oCPU.ReadInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x56)),
				TechnologyAdvanceEnum.Literacy) ? 1 : 0);
			this.oCPU.AX.UInt16 = this.oCPU.OR_UInt16(this.oCPU.AX.UInt16, this.oCPU.AX.UInt16);
			if (this.oCPU.Flags.NE)
			{
				this.oCPU.SI.UInt16 = this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x56));
				this.oCPU.SI.UInt16 = this.oCPU.SHL_UInt16(this.oCPU.SI.UInt16, 0x1);
				this.oCPU.WriteUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 + this.oCPU.SI.UInt16 - 0xa8), 
					this.oCPU.SHL_UInt16(this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 + this.oCPU.SI.UInt16 - 0xa8)), 0x1));
			}

			// Instruction address 0x0000:0x133f, size: 5
			this.oCPU.AX.UInt16 = (ushort)(this.parent.Segment_1ade.F0_1ade_22b5_PlayerHasTechnology(this.oCPU.ReadInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x56)),
				TechnologyAdvanceEnum.University) ? 1 : 0);
			this.oCPU.AX.UInt16 = this.oCPU.OR_UInt16(this.oCPU.AX.UInt16, this.oCPU.AX.UInt16);
			if (this.oCPU.Flags.NE)
			{
				this.oCPU.SI.UInt16 = this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x56));
				this.oCPU.SI.UInt16 = this.oCPU.SHL_UInt16(this.oCPU.SI.UInt16, 0x1);
				this.oCPU.WriteUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 + this.oCPU.SI.UInt16 - 0xa8), 
					this.oCPU.SHL_UInt16(this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 + this.oCPU.SI.UInt16 - 0xa8)), 0x1));
			}
		
			this.oCPU.SI.UInt16 = (ushort)((this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x56)) << 1) + 
				this.oCPU.BP.UInt16 - 0xa8);
			// Instruction address 0x0000:0x1368, size: 5
			this.oCPU.AX.Int16 = (short)this.parent.Tools.F0_2dc4_007c_CheckValueRange(this.oCPU.ReadInt16(this.oCPU.DS.UInt16, this.oCPU.SI.UInt16), 0, 100);

			this.oCPU.WriteUInt16(this.oCPU.DS.UInt16, this.oCPU.SI.UInt16, this.oCPU.AX.UInt16);
			this.oCPU.AX.UInt16 = (ushort)playerID;
			this.oCPU.CMP_UInt16(this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x56)), this.oCPU.AX.UInt16);
			if (this.oCPU.Flags.NE) goto L13c5;
			this.oCPU.WriteUInt8(this.oCPU.DS.UInt16, 0xba06, 0x0);

			// Instruction address 0x0000:0x1396, size: 5
			this.parent.CAPI.strcat(0xba06,
				this.parent.CAPI.itoa((short)this.oCPU.ReadUInt16(this.oCPU.DS.UInt16, this.oCPU.SI.UInt16), 10));

			// Instruction address 0x0000:0x13a6, size: 5
			this.parent.CAPI.strcat(0xba06, "%");

			// Instruction address 0x0000:0x13bd, size: 5
			this.parent.DrawTools.F0_1182_005c_DrawStringToScreen0(0xba06, 104, this.oCPU.ReadInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x54)), 15);

		L13c5:
			this.oCPU.WriteUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x56), 
				this.oCPU.INC_UInt16(this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x56))));
			this.oCPU.CMP_UInt16(this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x56)), 0x8);
			if (this.oCPU.Flags.GE) goto L13d1;
			goto L12cd;

		L13d1:
			F12_0000_1968((ushort)(this.oCPU.BP.UInt16 - 0xa8), this.oCPU.ReadInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x54)), "%");

			this.oCPU.WriteUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x54), 
				this.oCPU.ADD_UInt16(this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x54)), 0xc));

			// Instruction address 0x0000:0x13f7, size: 5
			this.parent.DrawTools.F0_1182_0086_DrawStringWithShadowToScreen0("Disease:", 8, this.oCPU.ReadInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x54)), 15);

			this.oCPU.WriteUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x56), 0x1);

		L1404:
			this.oCPU.SI.UInt16 = this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x56));
			this.oCPU.SI.UInt16 = this.oCPU.SHL_UInt16(this.oCPU.SI.UInt16, 0x1);
			this.oCPU.SI.UInt16 = this.oCPU.ADD_UInt16(this.oCPU.SI.UInt16, this.oCPU.BP.UInt16);
			this.oCPU.AX.UInt16 = 0x32;
			this.oCPU.IMUL_UInt16(this.oCPU.AX, this.oCPU.DX, this.oCPU.ReadUInt16(this.oCPU.DS.UInt16, (ushort)(this.oCPU.SI.UInt16 - 0x98)));
			this.oCPU.CWD(this.oCPU.AX, this.oCPU.DX);
			this.oCPU.CX.UInt16 = this.oCPU.ReadUInt16(this.oCPU.DS.UInt16, (ushort)(this.oCPU.SI.UInt16 - 0x98));
			this.oCPU.CX.UInt16 = this.oCPU.ADD_UInt16(this.oCPU.CX.UInt16, this.oCPU.ReadUInt16(this.oCPU.DS.UInt16, (ushort)(this.oCPU.SI.UInt16 - 0x10)));
			this.oCPU.IDIV_UInt16(this.oCPU.AX, this.oCPU.DX, this.oCPU.CX.UInt16);
			this.oCPU.WriteUInt16(this.oCPU.DS.UInt16, (ushort)(this.oCPU.SI.UInt16 - 0xa8), this.oCPU.AX.UInt16);

			// Instruction address 0x0000:0x1427, size: 5
			this.oCPU.AX.UInt16 = (ushort)(this.parent.Segment_1ade.F0_1ade_22b5_PlayerHasTechnology(this.oCPU.ReadInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x56)),
				TechnologyAdvanceEnum.Medicine) ? 1 : 0);
			this.oCPU.AX.UInt16 = this.oCPU.OR_UInt16(this.oCPU.AX.UInt16, this.oCPU.AX.UInt16);
			if (this.oCPU.Flags.NE)
			{
				this.oCPU.CX.UInt16 = 0x2;
				this.oCPU.AX.UInt16 = this.oCPU.ReadUInt16(this.oCPU.DS.UInt16, (ushort)(this.oCPU.SI.UInt16 - 0xa8));
				this.oCPU.CWD(this.oCPU.AX, this.oCPU.DX);
				this.oCPU.IDIV_UInt16(this.oCPU.AX, this.oCPU.DX, this.oCPU.CX.UInt16);
				this.oCPU.WriteUInt16(this.oCPU.DS.UInt16, (ushort)(this.oCPU.SI.UInt16 - 0xa8), this.oCPU.AX.UInt16);
			}
		
			this.oCPU.SI.UInt16 = this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x56));
			this.oCPU.SI.UInt16 = this.oCPU.SHL_UInt16(this.oCPU.SI.UInt16, 0x1);
			this.oCPU.SI.UInt16 = this.oCPU.ADD_UInt16(this.oCPU.SI.UInt16, this.oCPU.BP.UInt16);
			this.oCPU.AX.UInt16 = 0x5dc;
			this.oCPU.CWD(this.oCPU.AX, this.oCPU.DX);
			this.oCPU.CX.UInt16 = this.oCPU.ReadUInt16(this.oCPU.DS.UInt16, (ushort)(this.oCPU.SI.UInt16 - 0xa8));
			this.oCPU.CX.UInt16 = this.oCPU.ADD_UInt16(this.oCPU.CX.UInt16, 0x14);
			this.oCPU.IDIV_UInt16(this.oCPU.AX, this.oCPU.DX, this.oCPU.CX.UInt16);
			this.oCPU.WriteUInt16(this.oCPU.DS.UInt16, (ushort)(this.oCPU.SI.UInt16 - 0x10), this.oCPU.AX.UInt16);
			this.oCPU.AX.UInt16 = (ushort)playerID;
			this.oCPU.CMP_UInt16(this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x56)), this.oCPU.AX.UInt16);
			if (this.oCPU.Flags.NE) goto L14ad;
			this.oCPU.WriteUInt8(this.oCPU.DS.UInt16, 0xba06, 0x0);

			// Instruction address 0x0000:0x147e, size: 5
			this.parent.CAPI.strcat(0xba06,
				this.parent.CAPI.itoa((short)this.oCPU.ReadUInt16(this.oCPU.DS.UInt16, (ushort)(this.oCPU.SI.UInt16 - 0xa8)), 10));

			// Instruction address 0x0000:0x148e, size: 5
			this.parent.CAPI.strcat(0xba06, "%");

			// Instruction address 0x0000:0x14a5, size: 5
			this.parent.DrawTools.F0_1182_005c_DrawStringToScreen0(0xba06, 104, this.oCPU.ReadInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x54)), 15);

		L14ad:
			this.oCPU.WriteUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x56), this.oCPU.INC_UInt16(this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x56))));
			this.oCPU.CMP_UInt16(this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x56)), 0x8);
			if (this.oCPU.Flags.GE) goto L14b9;
			goto L1404;

		L14b9:
			F12_0000_1968((ushort)(this.oCPU.BP.UInt16 - 0xa8), (short)(-this.oCPU.ReadInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x54))), "%");

			this.oCPU.WriteUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x54), 
				this.oCPU.ADD_UInt16(this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x54)), 0xc));

			// Instruction address 0x0000:0x14e2, size: 5
			this.parent.DrawTools.F0_1182_0086_DrawStringWithShadowToScreen0("Pollution:", 8, this.oCPU.ReadInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x54)), 15);

			this.oCPU.WriteUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x56), 0x1);

		L14ef:
			this.oCPU.SI.UInt16 = this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x56));
			this.oCPU.SI.UInt16 = this.oCPU.SHL_UInt16(this.oCPU.SI.UInt16, 0x1);
			this.oCPU.SI.UInt16 = this.oCPU.ADD_UInt16(this.oCPU.SI.UInt16, this.oCPU.BP.UInt16);
			this.oCPU.AX.UInt16 = this.oCPU.ReadUInt16(this.oCPU.DS.UInt16, (ushort)(this.oCPU.SI.UInt16 - 0x88));
			this.oCPU.WriteUInt16(this.oCPU.DS.UInt16, (ushort)(this.oCPU.SI.UInt16 - 0xa8), this.oCPU.AX.UInt16);
			this.oCPU.AX.UInt16 = 0xa;
			this.oCPU.IMUL_UInt16(this.oCPU.AX, this.oCPU.DX, this.oCPU.ReadUInt16(this.oCPU.DS.UInt16, (ushort)(this.oCPU.SI.UInt16 - 0x88)));
			this.oCPU.CWD(this.oCPU.AX, this.oCPU.DX);
			this.oCPU.CX.UInt16 = this.oCPU.ReadUInt16(this.oCPU.DS.UInt16, (ushort)(this.oCPU.SI.UInt16 - 0x98));
			this.oCPU.IDIV_UInt16(this.oCPU.AX, this.oCPU.DX, this.oCPU.CX.UInt16);
			this.oCPU.WriteUInt16(this.oCPU.DS.UInt16, (ushort)(this.oCPU.SI.UInt16 - 0x10), this.oCPU.SUB_UInt16(this.oCPU.ReadUInt16(this.oCPU.DS.UInt16, (ushort)(this.oCPU.SI.UInt16 - 0x10)), this.oCPU.AX.UInt16));
			this.oCPU.AX.UInt16 = (ushort)playerID;
			this.oCPU.CMP_UInt16(this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x56)), this.oCPU.AX.UInt16);
			if (this.oCPU.Flags.NE) goto L1564;
			this.oCPU.WriteUInt8(this.oCPU.DS.UInt16, 0xba06, 0x0);

			// Instruction address 0x0000:0x1535, size: 5
			this.parent.CAPI.strcat(0xba06,
				this.parent.CAPI.itoa((short)this.oCPU.ReadUInt16(this.oCPU.DS.UInt16, (ushort)(this.oCPU.SI.UInt16 - 0xa8)), 10));

			// Instruction address 0x0000:0x1545, size: 5
			this.parent.CAPI.strcat(0xba06, "0 tons/year");

			// Instruction address 0x0000:0x155c, size: 5
			this.parent.DrawTools.F0_1182_005c_DrawStringToScreen0(0xba06, 104, this.oCPU.ReadInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x54)), 15);

		L1564:
			this.oCPU.WriteUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x56), this.oCPU.INC_UInt16(this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x56))));
			this.oCPU.CMP_UInt16(this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x56)), 0x8);
			if (this.oCPU.Flags.L) goto L14ef;

			F12_0000_1968((ushort)(this.oCPU.BP.UInt16 - 0xa8), (short)(-this.oCPU.ReadInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x54))), "0 tons");

			this.oCPU.WriteUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x54), 
				this.oCPU.ADD_UInt16(this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x54)), 0xc));

			// Instruction address 0x0000:0x1596, size: 5
			this.parent.DrawTools.F0_1182_0086_DrawStringWithShadowToScreen0("Life Expectancy:", 8, this.oCPU.ReadInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x54)), 15);

			this.oCPU.WriteUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x56), 0x1);

		L15a3:
			this.oCPU.SI.UInt16 = (ushort)((this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x56)) << 1) +
				this.oCPU.BP.UInt16);

			// Instruction address 0x0000:0x15b5, size: 5
			this.oCPU.AX.Int16 = (short)this.parent.Tools.F0_2dc4_007c_CheckValueRange(
				this.oCPU.ReadInt16(this.oCPU.DS.UInt16, (ushort)(this.oCPU.SI.UInt16 - 0x10)), 20, 99);

			this.oCPU.WriteUInt16(this.oCPU.DS.UInt16, (ushort)(this.oCPU.SI.UInt16 - 0xa8), this.oCPU.AX.UInt16);
			this.oCPU.AX.UInt16 = (ushort)playerID;
			this.oCPU.CMP_UInt16(this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x56)), this.oCPU.AX.UInt16);
			if (this.oCPU.Flags.NE) goto L1616;
			this.oCPU.WriteUInt8(this.oCPU.DS.UInt16, 0xba06, 0x0);

			// Instruction address 0x0000:0x15e7, size: 5
			this.parent.CAPI.strcat(0xba06,
				this.parent.CAPI.itoa((short)this.oCPU.ReadUInt16(this.oCPU.DS.UInt16, (ushort)(this.oCPU.SI.UInt16 - 0xa8)), 10));

			// Instruction address 0x0000:0x15f7, size: 5
			this.parent.CAPI.strcat(0xba06, " years");

			// Instruction address 0x0000:0x160e, size: 5
			this.parent.DrawTools.F0_1182_005c_DrawStringToScreen0(0xba06, 104, this.oCPU.ReadInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x54)), 15);

		L1616:
			this.oCPU.WriteUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x56), this.oCPU.INC_UInt16(this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x56))));
			this.oCPU.CMP_UInt16(this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x56)), 0x8);
			if (this.oCPU.Flags.L) goto L15a3;

			F12_0000_1968((ushort)(this.oCPU.BP.UInt16 - 0xa8), this.oCPU.ReadInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x54)), " years");

			this.oCPU.WriteUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x54), 
				this.oCPU.ADD_UInt16(this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x54)), 0xc));

			// Instruction address 0x0000:0x1645, size: 5
			this.parent.DrawTools.F0_1182_0086_DrawStringWithShadowToScreen0("Family Size:", 8, this.oCPU.ReadInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x54)), 15);

			this.oCPU.WriteUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x56), 0x1);

		L1652:
			this.oCPU.SI.UInt16 = this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x56));
			this.oCPU.SI.UInt16 = this.oCPU.SHL_UInt16(this.oCPU.SI.UInt16, 0x1);
			this.oCPU.SI.UInt16 = this.oCPU.ADD_UInt16(this.oCPU.SI.UInt16, this.oCPU.BP.UInt16);
			this.oCPU.AX.UInt16 = 0x28;
			this.oCPU.IMUL_UInt16(this.oCPU.AX, this.oCPU.DX, this.oCPU.ReadUInt16(this.oCPU.DS.UInt16, (ushort)(this.oCPU.SI.UInt16 - 0x40)));
			this.oCPU.CWD(this.oCPU.AX, this.oCPU.DX);
			this.oCPU.CX.UInt16 = this.oCPU.ReadUInt16(this.oCPU.DS.UInt16, (ushort)(this.oCPU.SI.UInt16 - 0x98));
			this.oCPU.IDIV_UInt16(this.oCPU.AX, this.oCPU.DX, this.oCPU.CX.UInt16);
			this.oCPU.AX.UInt16 = this.oCPU.ADD_UInt16(this.oCPU.AX.UInt16, 0x14);
			this.oCPU.WriteUInt16(this.oCPU.DS.UInt16, (ushort)(this.oCPU.SI.UInt16 - 0xa8), this.oCPU.AX.UInt16);
			this.oCPU.AX.UInt16 = (ushort)playerID;
			this.oCPU.CMP_UInt16(this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x56)), this.oCPU.AX.UInt16);
			if (this.oCPU.Flags.E) goto L1678;
			goto L1704;

		L1678:
			this.oCPU.WriteUInt8(this.oCPU.DS.UInt16, 0xba06, 0x0);

			// Instruction address 0x0000:0x169d, size: 5
			this.parent.CAPI.strcat(0xba06,
				this.parent.CAPI.itoa((short)((short)this.oCPU.ReadUInt16(this.oCPU.DS.UInt16, (ushort)(this.oCPU.SI.UInt16 - 0xa8)) / 10), 10));

			// Instruction address 0x0000:0x16ad, size: 5
			this.parent.CAPI.strcat(0xba06, ".");

			// Instruction address 0x0000:0x16d5, size: 5
			this.parent.CAPI.strcat(0xba06,
				this.parent.CAPI.itoa((short)((short)this.oCPU.ReadUInt16(this.oCPU.DS.UInt16, (ushort)(this.oCPU.SI.UInt16 - 0xa8)) / 10), 10));

			// Instruction address 0x0000:0x16e5, size: 5
			this.parent.CAPI.strcat(0xba06, " children");

			// Instruction address 0x0000:0x16fc, size: 5
			this.parent.DrawTools.F0_1182_005c_DrawStringToScreen0(0xba06, 104, this.oCPU.ReadInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x54)), 15);

		L1704:
			this.oCPU.SI.UInt16 = this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x56));
			this.oCPU.SI.UInt16 = this.oCPU.SHL_UInt16(this.oCPU.SI.UInt16, 0x1);
			this.oCPU.CX.UInt16 = 0xa;
			this.oCPU.AX.UInt16 = this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 + this.oCPU.SI.UInt16 - 0xa8));
			this.oCPU.CWD(this.oCPU.AX, this.oCPU.DX);
			this.oCPU.IDIV_UInt16(this.oCPU.AX, this.oCPU.DX, this.oCPU.CX.UInt16);
			this.oCPU.WriteUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 + this.oCPU.SI.UInt16 - 0xa8), this.oCPU.AX.UInt16);
			this.oCPU.WriteUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x56), this.oCPU.INC_UInt16(this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x56))));
			this.oCPU.CMP_UInt16(this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x56)), 0x8);
			if (this.oCPU.Flags.GE) goto L1723;
			goto L1652;

		L1723:
			F12_0000_1968((ushort)(this.oCPU.BP.UInt16 - 0xa8), this.oCPU.ReadInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x54)), "");

			this.oCPU.WriteUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x54), 
				this.oCPU.ADD_UInt16(this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x54)), 0xc));

			// Instruction address 0x0000:0x1749, size: 5
			this.parent.DrawTools.F0_1182_0086_DrawStringWithShadowToScreen0("Military Service:", 8, this.oCPU.ReadInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x54)), 15);

			this.oCPU.WriteUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x56), 0x1);

		L1756:
			this.oCPU.SI.UInt16 = this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x56));
			this.oCPU.SI.UInt16 = this.oCPU.SHL_UInt16(this.oCPU.SI.UInt16, 0x1);
			this.oCPU.AX.UInt16 = 0xa;
			this.oCPU.IMUL_UInt16(this.oCPU.AX, this.oCPU.DX, 
				(ushort)this.parent.GameData.Players[this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x56))].UnitCount);
			this.oCPU.CWD(this.oCPU.AX, this.oCPU.DX);
			this.oCPU.CX.UInt16 = this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 + this.oCPU.SI.UInt16 - 0x98));
			this.oCPU.IDIV_UInt16(this.oCPU.AX, this.oCPU.DX, this.oCPU.CX.UInt16);
			this.oCPU.WriteUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 + this.oCPU.SI.UInt16 - 0xa8), this.oCPU.AX.UInt16);
			this.oCPU.AX.UInt16 = (ushort)playerID;
			this.oCPU.CMP_UInt16(this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x56)), this.oCPU.AX.UInt16);
			if (this.oCPU.Flags.NE) goto L17c2;
			this.oCPU.WriteUInt8(this.oCPU.DS.UInt16, 0xba06, 0x0);

			// Instruction address 0x0000:0x1793, size: 5
			this.parent.CAPI.strcat(0xba06,
				this.parent.CAPI.itoa((short)this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 + this.oCPU.SI.UInt16 - 0xa8)), 10));

			// Instruction address 0x0000:0x17a3, size: 5
			this.parent.CAPI.strcat(0xba06, " years");

			// Instruction address 0x0000:0x17ba, size: 5
			this.parent.DrawTools.F0_1182_005c_DrawStringToScreen0(0xba06, 104, this.oCPU.ReadInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x54)), 15);

		L17c2:
			this.oCPU.WriteUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x56), this.oCPU.INC_UInt16(this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x56))));
			this.oCPU.CMP_UInt16(this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x56)), 0x8);
			if (this.oCPU.Flags.L) goto L1756;

			F12_0000_1968((ushort)(this.oCPU.BP.UInt16 - 0xa8), this.oCPU.ReadInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x54)), " years");

			this.oCPU.WriteUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x54), 
				this.oCPU.ADD_UInt16(this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x54)), 0xc));

			// Instruction address 0x0000:0x17f1, size: 5
			this.parent.DrawTools.F0_1182_0086_DrawStringWithShadowToScreen0("Annual Income:", 8, this.oCPU.ReadInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x54)), 15);

			this.oCPU.WriteUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x56), 0x1);
			goto L1874;

		L1800:
			this.oCPU.SI.UInt16 = this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x56));
			this.oCPU.SI.UInt16 = this.oCPU.SHL_UInt16(this.oCPU.SI.UInt16, 0x1);
			this.oCPU.AX.UInt16 = this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 + this.oCPU.SI.UInt16 - 0x78));
			this.oCPU.CWD(this.oCPU.AX, this.oCPU.DX);
			this.oCPU.CX.UInt16 = this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 + this.oCPU.SI.UInt16 - 0x98));
			this.oCPU.IDIV_UInt16(this.oCPU.AX, this.oCPU.DX, this.oCPU.CX.UInt16);
			this.oCPU.IMUL_UInt16(this.oCPU.AX, this.oCPU.DX, 
				(ushort)this.parent.GameData.Players[this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x56))].DiscoveredTechnologyCount);

		L1813:
			this.oCPU.WriteUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 + this.oCPU.SI.UInt16 - 0xa8), this.oCPU.AX.UInt16);
			this.oCPU.AX.UInt16 = (ushort)playerID;
			this.oCPU.CMP_UInt16(this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x56)), this.oCPU.AX.UInt16);
			if (this.oCPU.Flags.NE) goto L1871;
			this.oCPU.WriteUInt8(this.oCPU.DS.UInt16, 0xba06, 0x0);

			// Instruction address 0x0000:0x1842, size: 5
			this.parent.CAPI.strcat(0xba06,
				this.parent.CAPI.itoa((short)this.oCPU.ReadUInt16(this.oCPU.SS.UInt16,
					(ushort)((this.oCPU.BP.UInt16 + (this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x56)) << 1) - 0xa8) & 0xffff)), 10));

			// Instruction address 0x0000:0x1852, size: 5
			this.parent.CAPI.strcat(0xba06, "$ per capita");

			// Instruction address 0x0000:0x1869, size: 5
			this.parent.DrawTools.F0_1182_005c_DrawStringToScreen0(0xba06, 104, this.oCPU.ReadInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x54)), 15);

		L1871:
			this.oCPU.WriteUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x56), 
				this.oCPU.INC_UInt16(this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x56))));

		L1874:
			this.oCPU.CMP_UInt16(this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x56)), 0x8);
			if (this.oCPU.Flags.GE) goto L18a3;
			this.oCPU.SI.UInt16 = this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x56));
			this.oCPU.SI.UInt16 = this.oCPU.SHL_UInt16(this.oCPU.SI.UInt16, 0x1);
			this.oCPU.AX.UInt16 = 0x7530;
			this.oCPU.CWD(this.oCPU.AX, this.oCPU.DX);
			this.oCPU.CX.UInt16 = (ushort)(this.parent.GameData.Players[this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x56))].DiscoveredTechnologyCount + 1);
			this.oCPU.IDIV_UInt16(this.oCPU.AX, this.oCPU.DX, this.oCPU.CX.UInt16);
			this.oCPU.CMP_UInt16(this.oCPU.AX.UInt16, this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 + this.oCPU.SI.UInt16 - 0x78)));
			if (this.oCPU.Flags.G) goto L1892;
			goto L1800;

		L1892:
			this.oCPU.AX.UInt16 = this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 + this.oCPU.SI.UInt16 - 0x78));
			this.oCPU.IMUL_UInt16(this.oCPU.AX, this.oCPU.DX, 
				(ushort)this.parent.GameData.Players[this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x56))].DiscoveredTechnologyCount);
			this.oCPU.CWD(this.oCPU.AX, this.oCPU.DX);
			this.oCPU.CX.UInt16 = this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 + this.oCPU.SI.UInt16 - 0x98));
			this.oCPU.IDIV_UInt16(this.oCPU.AX, this.oCPU.DX, this.oCPU.CX.UInt16);
			goto L1813;

		L18a3:
			F12_0000_1968((ushort)(this.oCPU.BP.UInt16 - 0xa8), this.oCPU.ReadInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x54)), "$");

			this.oCPU.WriteUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x54), 
				this.oCPU.ADD_UInt16(this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x54)), 0xc));

			// Instruction address 0x0000:0x18c9, size: 5
			this.parent.DrawTools.F0_1182_0086_DrawStringWithShadowToScreen0("Productivity:", 8, this.oCPU.ReadInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x54)), 15);

			this.oCPU.WriteUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x56), 0x1);

		L18d6:
			this.oCPU.SI.UInt16 = this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x56));
			this.oCPU.SI.UInt16 = this.oCPU.SHL_UInt16(this.oCPU.SI.UInt16, 0x1);
			this.oCPU.SI.UInt16 = this.oCPU.ADD_UInt16(this.oCPU.SI.UInt16, this.oCPU.BP.UInt16);
			this.oCPU.AX.UInt16 = 0xa;
			this.oCPU.IMUL_UInt16(this.oCPU.AX, this.oCPU.DX, this.oCPU.ReadUInt16(this.oCPU.DS.UInt16, (ushort)(this.oCPU.SI.UInt16 - 0x30)));
			this.oCPU.CWD(this.oCPU.AX, this.oCPU.DX);
			this.oCPU.CX.UInt16 = this.oCPU.ReadUInt16(this.oCPU.DS.UInt16, (ushort)(this.oCPU.SI.UInt16 - 0x98));
			this.oCPU.IDIV_UInt16(this.oCPU.AX, this.oCPU.DX, this.oCPU.CX.UInt16);
			this.oCPU.WriteUInt16(this.oCPU.DS.UInt16, (ushort)(this.oCPU.SI.UInt16 - 0xa8), this.oCPU.AX.UInt16);
			this.oCPU.AX.UInt16 = (ushort)playerID;
			this.oCPU.CMP_UInt16(this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x56)), this.oCPU.AX.UInt16);
			if (this.oCPU.Flags.NE) goto L1933;
			this.oCPU.WriteUInt8(this.oCPU.DS.UInt16, 0xba06, 0x0);

			// Instruction address 0x0000:0x1914, size: 5
			this.parent.CAPI.strcat(0xba06,
				this.parent.CAPI.itoa((short)this.oCPU.ReadUInt16(this.oCPU.DS.UInt16, (ushort)(this.oCPU.SI.UInt16 - 0xa8)), 10));

			// Instruction address 0x0000:0x192b, size: 5
			this.parent.DrawTools.F0_1182_005c_DrawStringToScreen0(0xba06, 104, this.oCPU.ReadInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x54)), 15);

		L1933:
			this.oCPU.WriteUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x56), this.oCPU.INC_UInt16(this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x56))));
			this.oCPU.CMP_UInt16(this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x56)), 0x8);
			if (this.oCPU.Flags.L) goto L18d6;

			F12_0000_1968((ushort)(this.oCPU.BP.UInt16 - 0xa8), this.oCPU.ReadInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x54)), "");

			this.oCPU.WriteUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x54), 
				this.oCPU.ADD_UInt16(this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x54)), 0xc));

			// Instruction address 0x0000:0x1953, size: 5
			this.parent.Segment_2459.F0_2459_0918_WaitForKeyPressOrMouseClick();

			this.parent.Graphics.F0_VGA_07d8_DrawImage(this.parent.Var_19d4_Screen1_Rectangle, 0, 0, 320, 200, this.parent.Var_aa_Screen0_Rectangle, 0, 0);

			this.oCPU.SI.UInt16 = this.oCPU.POP_UInt16();
			this.oCPU.DI.UInt16 = this.oCPU.POP_UInt16();
			this.oCPU.SP.UInt16 = this.oCPU.BP.UInt16;
			this.oCPU.BP.UInt16 = this.oCPU.POP_UInt16();
			// Far return
			this.oCPU.Log.ExitBlock("F12_0000_0d6d_ShowsDemographicsPopup");
		}

		/// <summary>
		/// Demographics report helper
		/// </summary>
		/// <param name="arrayPtr"></param>
		/// <param name="y"></param>
		/// <param name="text"></param>
		private void F12_0000_1968(ushort arrayPtr, int y, string text)
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
						this.oCPU.WriteInt16(this.oCPU.DS.UInt16, (ushort)(arrayPtr + (i * 2)),
							(short)(-this.oCPU.ReadInt16(this.oCPU.DS.UInt16, (ushort)(arrayPtr + (i * 2)))));
					}

					if (this.oCPU.ReadInt16(this.oCPU.DS.UInt16, (ushort)(arrayPtr + (i * 2))) > this.oCPU.ReadInt16(this.oCPU.DS.UInt16, (ushort)(arrayPtr + (this.parent.GameData.HumanPlayerID * 2))))
					{
						humanPlayerRank++;
					}

					if (maximumValue < this.oCPU.ReadInt16(this.oCPU.DS.UInt16, (ushort)(arrayPtr + (i * 2))))
					{
						maximumValue = this.oCPU.ReadInt16(this.oCPU.DS.UInt16, (ushort)(arrayPtr + (i * 2)));
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
					this.parent.DrawTools.F0_1182_0086_DrawStringWithShadow($"{humanPlayerRank}st", 192, Math.Abs(y), 15);
					break;

				case 2:
					// Instruction address 0x0000:0x1a84, size: 5
					this.parent.DrawTools.F0_1182_0086_DrawStringWithShadow($"{humanPlayerRank}nd", 192, Math.Abs(y), 15);
					break;

				case 3:
					// Instruction address 0x0000:0x1a84, size: 5
					this.parent.DrawTools.F0_1182_0086_DrawStringWithShadow($"{humanPlayerRank}rd", 192, Math.Abs(y), 15);
					break;

				default:
					// Instruction address 0x0000:0x1a84, size: 5
					this.parent.DrawTools.F0_1182_0086_DrawStringWithShadow($"{humanPlayerRank}th", 192, Math.Abs(y), 15);
					break;
			}

			if (selectedPlayerID != this.parent.GameData.HumanPlayerID)
			{
				if (this.parent.GameData.Players[this.parent.GameData.HumanPlayerID].Diplomacy[selectedPlayerID].HasFlag(DiplomacyFlagsEnum.Unknown40))
				{
					string stats = $"({this.parent.GameData.Players[selectedPlayerID].Nation}: {Math.Abs(maximumValue)}{text})";

					// Instruction address 0x0000:0x1b4a, size: 5
					this.parent.DrawTools.F0_1182_005c_DrawStringToScreen0(
						stats, 318 - this.parent.DrawTools.GetStringWidth(stats), Math.Abs(y), this.parent.Array_1946_PlayerColours[selectedPlayerID]);
				}
			}
		}
	}
}
