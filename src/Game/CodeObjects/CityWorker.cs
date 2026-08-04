using IRB.VirtualCPU;
using OpenCivOne.Graphics;

namespace OpenCivOne
{
	public class CityWorker
	{
		private OpenCivOneGame parent;

		// Local variables used exclusively inside this section

		private int Var_2494 = 0;
		private int Var_2496 = 0;

		// 0x652e - after this offset the default values are set to 0

		public int Var_653e_CityID = 0;
		private int Var_6540_CityOffsetCount = 21;
		private int Var_6542 = 0;
		private int Var_6546 = 0;
		public short Var_6548_PlayerID = 0;
		private int Var_6b30 = 0;
		private int Var_70e8 = 0;
		//private ushort Var_deb6 = 0;
		private GPoint[] cityMapOffsets = [new(0, -1), new(2, 0), new(0, 1), new(1, 2)];

		public CityWorker(OpenCivOneGame parent)
		{
			this.parent = parent;
		}

		/// <summary>
		/// ?
		/// </summary>
		/// <param name="cityID"></param>
		/// <param name="flag"></param>
		public int F0_1d12_0045_ProcessCityState(int cityID, int flag)
		{
			//this.CPU.Log.EnterBlock($"F0_1d12_0045({cityID}, {flag})");

			// function body
			bool flag_b8 = false;

			int outsideUnitIndex;

			int improvementIndex;
			bool hasNextImprovementPage = false;
			int currentImprovementIndex = 0;
			int previousImprovementIndex = 0;

			int bestCityCellIndex = 0;
			int newCityWindowUnitX = 0;
			int newCityWindowUnitY = 0;

			int[] cityFreeArea = new int[21];
			City city = this.parent.GameData.Cities[cityID];
			bool[,] cityArea = new bool[5, 5];

			if (city.Status != CityStatusEnum.Invalid)
			{
				this.Var_2496 = 0;

				for (int i = 0; i < 5; i++)
				{
					for (int j = 0; j < 5; j++)
					{
						cityArea[i, j] = false;
					}
				}

				int cityX = city.Position.X;
				int cityY = city.Position.Y;

				this.Var_653e_CityID = cityID;

				if (city.HasImprovement(ImprovementEnum.Palace))
				{
					this.Var_6b30 = 0;
				}
				else
				{
					this.Var_6b30 = 32;
				}

				if (city.ShieldsCount < 0 || city.ShieldsCount > 999)
				{
					city.ShieldsCount = 0;
				}

				for (int i = 0; i < 128; i++)
				{
					if (this.parent.GameData.Cities[i].Status != CityStatusEnum.Invalid && i != city.ID)
					{
						// Instruction address 0x1d12:0x01a2, size: 5
						int intercityDistance = this.parent.MapManagement.GetDistance(city.Position, this.parent.GameData.Cities[i].Position);

						if (this.parent.GameData.Cities[i].PlayerID == city.PlayerID &&
							this.parent.GameData.Cities[i].HasImprovement(ImprovementEnum.Palace) && intercityDistance < this.Var_6b30)
						{
							this.Var_6b30 = intercityDistance;
						}

						if (intercityDistance < 6)
						{
							for (int j = 0; j < 21; j++)
							{
								if ((this.parent.GameData.Cities[i].WorkerFlags & (uint)(1 << j)) != 0 || j == 20)
								{
									int xPos = this.parent.GameData.Cities[i].Position.X + this.parent.CityOffsets[j].X;
									int yPos = this.parent.GameData.Cities[i].Position.Y + this.parent.CityOffsets[j].Y;

									if (Math.Abs(xPos - city.Position.X) <= 2 && Math.Abs(yPos - city.Position.Y) <= 2)
									{
										cityArea[xPos - city.Position.X + 2, yPos - city.Position.Y + 2] = true;
									}
								}
							}
						}
					}
				}

				cityArea[2, 2] = false;
				this.Var_6548_PlayerID = city.PlayerID;
				int mapViewX = this.parent.Var_d4cc_MapViewX;
				int mapViewY = this.parent.Var_d75e_MapViewY;

				int productionRatio;

				if (this.parent.GameData.Players[this.Var_6548_PlayerID].GovernmentType == GovernmentTypeEnum.Anarchy ||
					this.parent.GameData.Players[this.Var_6548_PlayerID].GovernmentType == GovernmentTypeEnum.Despotism)
				{
					productionRatio = 1;
				}
				else
				{
					productionRatio = 2;
				}

				this.parent.Var_b882 = 0;

				if (!city.HasImprovement(ImprovementEnum.MassTransit))
				{
					// Instruction address 0x1d12:0x035e, size: 5
					if (this.parent.Segment_1ade.F0_1ade_22b5_PlayerHasTechnology(this.Var_6548_PlayerID, TechnologyAdvanceEnum.Industrialization))
					{
						this.parent.Var_b882++;
					}

					// Instruction address 0x1d12:0x037a, size: 5
					if (this.parent.Segment_1ade.F0_1ade_22b5_PlayerHasTechnology(this.Var_6548_PlayerID, TechnologyAdvanceEnum.Automobile))
					{
						this.parent.Var_b882++;
					}

					// Instruction address 0x1d12:0x0396, size: 5
					if (this.parent.Segment_1ade.F0_1ade_22b5_PlayerHasTechnology(this.Var_6548_PlayerID, TechnologyAdvanceEnum.MassProduction))
					{
						this.parent.Var_b882++;
					}

					// Instruction address 0x1d12:0x03b2, size: 5
					if (this.parent.Segment_1ade.F0_1ade_22b5_PlayerHasTechnology(this.Var_6548_PlayerID, TechnologyAdvanceEnum.Plastics))
					{
						this.parent.Var_b882++;
					}
				}
			
				int difficultyProductionRatio = 10;

				this.parent.Var_e8b8 = (int)(city.WorkerFlags >>> 26);

				if (this.Var_6548_PlayerID != this.parent.GameData.HumanPlayerID)
				{
					difficultyProductionRatio = 16 - this.parent.GameData.DifficultyLevel * 2;

					if (this.parent.GameData.Year >= 0 &&
						this.parent.GameData.Players[this.parent.GameData.HumanPlayerID].Ranking == 7 &&
						this.parent.GameData.DifficultyLevel > 1)
					{
						difficultyProductionRatio -= 2;
					}

					if (flag == 0)
					{
						if ((city.Status & CityStatusEnum.CivilDisorder) == CityStatusEnum.None)
						{
							this.parent.Var_e8b8 = 0;
						}
						else
						{
							city.WorkerFlags = 0;
						}
					}
				}

			L045f:
				cityX = city.Position.X;
				cityY = city.Position.Y;

				if (flag == 1)
				{
					// Instruction address 0x1d12:0x049a, size: 5
					this.parent.UnitManagement.F0_1866_0006_RestoreFortifiedCityUnits(cityID);

					// Instruction address 0x1d12:0x04ba, size: 5
					this.parent.DrawTools.FillRectangle(this.parent.Var_aa_Screen0_Rectangle, 0, 0, 320, 200, 0);

					// Instruction address 0x1d12:0x04d2, size: 5
					this.parent.DrawTools.DrawRectangleAndFillWithPattern(2, 1, 208, 21, 208, 100, 16, 16);

					int cityPopulation = 0;

					for (int i = 1; i <= city.ActualSize; i++)
					{
						cityPopulation += i;
					}

					this.parent.Var_aa_Screen0_Rectangle.FontID = 2;

					// Instruction address 0x1d12:0x056f, size: 5
					this.parent.DrawTools.DrawCenteredStringToScreen0($"{this.parent.Segment_2459.F0_2459_08c6_GetCityName(cityID)} " +
						$"(Pop:{this.parent.Tools.PopulationValueToString(cityPopulation)})", 104, 2, 15);

					// Instruction address 0x1d12:0x0587, size: 5
					this.parent.DrawTools.DrawRectangleAndFillWithPattern(127, 23, 208, 104, 208, 100, 16, 16);

					this.parent.Var_d4cc_MapViewX = city.Position.X - 5;
					this.parent.Var_d75e_MapViewY = city.Position.Y - 3;

					// Instruction address 0x1d12:0x05cd, size: 5
					this.parent.MapManagement.F0_2aea_03ba_DrawCell(city.Position.X, city.Position.Y);

					for (int i = 0; i < this.Var_6540_CityOffsetCount; i++)
					{
						int newX = city.Position.X + this.parent.CityOffsets[i].X;
						int newY = city.Position.Y + this.parent.CityOffsets[i].Y;

						if (this.Var_6548_PlayerID != this.parent.GameData.HumanPlayerID ||
							(this.parent.GameData.MapVisibility[newX, newY] & (1 << this.Var_6548_PlayerID)) != 0)
						{
							// Instruction address 0x1d12:0x0664, size: 5
							cityPopulation = this.parent.MapManagement.F0_2aea_14e0_GetCellActiveUnitPlayerID(newX, newY);

							if (cityPopulation != -1 && cityPopulation != this.Var_6548_PlayerID)
							{
								// Instruction address 0x1d12:0x068e, size: 5
								this.parent.MapManagement.F0_2aea_11d4_DrawCellWithUnit(newX, newY);
							}
							else
							{
								// Instruction address 0x1d12:0x06a1, size: 5
								this.parent.MapManagement.F0_2aea_03ba_DrawCell(newX, newY);

								if (cityArea[this.parent.CityOffsets[i].X + 2, this.parent.CityOffsets[i].Y + 2])
								{
									// Instruction address 0x1d12:0x06f6, size: 5
									this.parent.DrawTools.DrawRectangle(
										(newX - this.parent.Var_d4cc_MapViewX) * 16 + 80, (newY - this.parent.Var_d75e_MapViewY) * 16 + 8, 15, 15, 12);
								}
							}
						}
					}
				}

				if (flag == 0)
				{
					// !!! This is to prevent corruption, we should remove this
					city.Status &= (CityStatusEnum.CivilDisorder | CityStatusEnum.OceanAccess | CityStatusEnum.FortifiedUnitsRestored | CityStatusEnum.NearRiverOrMountains |
						 CityStatusEnum.AutoProduction | CityStatusEnum.TechnologyStolen | CityStatusEnum.Celebrating);

					if (city.FoodCount < 0)
					{
						int lostUnitID = -1;

						for (int i = 0; i < 128; i++)
						{
							if (this.parent.GameData.Players[this.Var_6548_PlayerID].Units[i].UnitType == UnitTypeEnum.Settler &&
								this.parent.GameData.Players[this.Var_6548_PlayerID].Units[i].HomeCityID == cityID)
							{
								lostUnitID = i;
								break;
							}
						}

						if (this.Var_6548_PlayerID == this.parent.GameData.HumanPlayerID)
						{
							// Instruction address 0x1d12:0x07dd, size: 5
							this.parent.CommonTools.PlayTune(36, 0);

							this.parent.News.F21_0000_0000_ShowNews(cityID,
								$"Food storage exhausted\nin {this.parent.Segment_2459.F0_2459_08c6_GetCityName(cityID)}!\n{((lostUnitID != -1) ? "Settlers lost." : "Famine feared.")}\n");

							// Instruction address 0x1d12:0x07f4, size: 5
							this.parent.CommonTools.PlayTune(1, 0);
						}

						if (lostUnitID != -1)
						{
							// Instruction address 0x1d12:0x080e, size: 5
							this.parent.UnitManagement.F0_1866_0f10_DeleteUnit(this.Var_6548_PlayerID, (short)lostUnitID);
						}
						else
						{
							city.ActualSize--;

							if (city.ActualSize <= 0)
							{
								// Instruction address 0x1d12:0x0843, size: 5
								this.parent.Segment_1ade.F0_1ade_018e(cityID, city.Position.X, city.Position.Y);

								this.parent.StartGameMenu.F5_0000_0e6c_TestIfAIPlayerIsDestroyed(this.Var_6548_PlayerID, 0);

								goto L6927;
							}
						}

						city.FoodCount = 0;

						// Instruction address 0x1d12:0x0879, size: 5
						this.parent.PlayerTurn.F0_1403_3ed7_DrawVisibleCell(cityX, cityY);
					}

					if ((city.ActualSize + 1) * difficultyProductionRatio <= city.FoodCount)
					{
						city.ActualSize++;

						if (city.HasImprovement(ImprovementEnum.Granary))
						{
							city.FoodCount = (short)((5 * city.ActualSize) + 5);
						}
						else
						{
							city.FoodCount = 0;
						}

						if (city.ActualSize > 10 && !city.HasImprovement(ImprovementEnum.Aqueduct) && (this.parent.GameData.DebugFlags & 0x4) != 0)
						{
							city.ActualSize--;

							if (this.Var_6548_PlayerID == this.parent.GameData.HumanPlayerID)
							{
								this.parent.Var_2f9e_MessageBoxStyle = MenuBoxReportTypeEnum.DomesticAdvisorReport;

								// Instruction address 0x1d12:0x094d, size: 5
								this.parent.Segment_1238.F0_1238_001e_ShowDialog(
									$"{this.parent.Segment_2459.F0_2459_08c6_GetCityName(cityID)} requires an AQUEDUCT\nfor further growth.\n", 100, 80);
							}
						}

						// Instruction address 0x1d12:0x095d, size: 5
						this.parent.PlayerTurn.F0_1403_3ed7_DrawVisibleCell(cityX, cityY);
					}
				}
			
				int currentWorkingCitizenCount = city.ActualSize + 1;

				this.Var_6546 = 0;
				this.parent.Var_deb8 = 0;
				this.parent.Var_d2f6 = 0;
				this.parent.Var_e3c6 = 0;

				for (int i = 0; i < 2; i++)
				{
					if (city.FirstFortifiedCityUnits[i] != -1)
					{
						this.parent.Var_deb8++;
					}
				}

				if (city.ActualSize < this.parent.Var_deb8)
				{
					this.parent.Var_d2f6 = this.parent.Var_deb8 - city.ActualSize;
				}

				for (int i = 0; i < 128; i++)
				{
					if (this.parent.GameData.Players[this.Var_6548_PlayerID].Units[i].UnitType != UnitTypeEnum.None &&
						this.parent.GameData.Players[this.Var_6548_PlayerID].Units[i].HomeCityID == cityID)
					{
						if (this.parent.GameData.Players[this.Var_6548_PlayerID].Units[i].UnitType != UnitTypeEnum.Diplomat && 
							this.parent.GameData.Players[this.Var_6548_PlayerID].Units[i].UnitType != UnitTypeEnum.Caravan)
						{
							this.parent.Var_deb8++;

							if (city.ActualSize >= this.parent.Var_deb8)
							{
								if ((this.parent.GameData.Players[this.Var_6548_PlayerID].GovernmentType == GovernmentTypeEnum.Monarchy ||
									this.parent.GameData.Players[this.Var_6548_PlayerID].GovernmentType == GovernmentTypeEnum.Communist ||
									this.parent.GameData.Players[this.Var_6548_PlayerID].GovernmentType == GovernmentTypeEnum.Republic ||
									this.parent.GameData.Players[this.Var_6548_PlayerID].GovernmentType == GovernmentTypeEnum.Democracy) && (this.parent.GameData.DebugFlags & 2) != 0)
								{
									this.parent.Var_d2f6++;
								}
							}
							else
							{
								this.parent.Var_d2f6++;
							}

							if (this.parent.GameData.Units[(int)this.parent.GameData.Players[this.Var_6548_PlayerID].Units[i].UnitType].AttackStrength != 0)
							{
								if (this.parent.GameData.Units[(int)this.parent.GameData.Players[this.Var_6548_PlayerID].Units[i].UnitType].MovementType == UnitMovementTypeEnum.Air)
								{
									this.Var_6546++;
								}
								else
								{
									if (this.parent.GameData.Players[this.Var_6548_PlayerID].Units[i].Position.X != city.Position.X ||
										this.parent.GameData.Players[this.Var_6548_PlayerID].Units[i].Position.Y != city.Position.Y)
									{
										this.Var_6546++;
									}
								}
							}
						}

						if (this.parent.GameData.Players[this.Var_6548_PlayerID].Units[i].UnitType == UnitTypeEnum.Settler)
						{
							this.parent.Var_e3c6++;
						}
					}
				}

				this.Var_70e8 = 0;

				for (int i = 0; i < this.Var_6540_CityOffsetCount; i++)
				{
					cityFreeArea[i] = 0;

					int newX = city.Position.X + this.parent.CityOffsets[i].X;
					int newY = city.Position.Y + this.parent.CityOffsets[i].Y;

					if (this.Var_6548_PlayerID != 0 && (this.parent.GameData.MapVisibility[newX, newY] & (1 << this.Var_6548_PlayerID)) == 0)
					{
						cityFreeArea[i] = 1;
					}

					// Instruction address 0x1d12:0x0bf4, size: 5
					int activeUnitPlayerID = this.parent.MapManagement.F0_2aea_14e0_GetCellActiveUnitPlayerID(newX, newY);

					if (activeUnitPlayerID != -1 && activeUnitPlayerID != this.Var_6548_PlayerID)
					{
						cityArea[this.parent.CityOffsets[i].X + 2, this.parent.CityOffsets[i].Y + 2] = true;
						cityFreeArea[i] = 1;

						if (activeUnitPlayerID == this.parent.GameData.HumanPlayerID)
						{
							this.Var_70e8 = 1;
						}
					}

					if (cityArea[this.parent.CityOffsets[i].X + 2, this.parent.CityOffsets[i].Y + 2])
					{
						cityFreeArea[i] = 1;
					}

					if (cityFreeArea[i] != 0)
					{
						city.WorkerFlags &= (uint)(~(1 << i));
					}
				}

				for (int i = 0; i < 4; i++)
				{
					this.parent.Var_70da_Arr[i] = 0;
				}

				uint newWorkerFlags = 0;

				if (((ushort)this.parent.GameData.PlayerFlags & (1 << this.Var_6548_PlayerID)) != 0 ||
					((cityID + this.parent.GameData.TurnCount) & 0x3) != 0 ||
					(city.Status & CityStatusEnum.CivilDisorder) != CityStatusEnum.None || this.Var_70e8 != 0)
				{
					for (int i = 0; i < this.Var_6540_CityOffsetCount; i++)
					{
						if ((city.WorkerFlags & (1 << i)) != 0)
						{
							currentWorkingCitizenCount--;
						}
					}

					if (currentWorkingCitizenCount >= 0)
					{
						currentWorkingCitizenCount = city.ActualSize + 1;

						for (int i = 0; i < this.Var_6540_CityOffsetCount; i++)
						{
							if (currentWorkingCitizenCount != 0 && (city.WorkerFlags & (1 << i)) != 0)
							{
								cityFreeArea[i] = 1;

								// Instruction address 0x1d12:0x0ddf, size: 5
								F0_1d12_692d_CityResources(cityID, i, flag);

								currentWorkingCitizenCount--;

								newWorkerFlags |= (uint)(1 << i);
							}
						}
					}
					else
					{
						if (flag == 0 && this.Var_6548_PlayerID == this.parent.GameData.HumanPlayerID)
						{
							this.parent.Var_2f9e_MessageBoxStyle = MenuBoxReportTypeEnum.DomesticAdvisorReport;

							// Instruction address 0x1d12:0x0e5b, size: 5
							this.parent.Segment_1238.F0_1238_001e_ShowDialog($"Population decrease\nin {this.parent.Segment_2459.F0_2459_08c6_GetCityName(cityID)}.\n", 100, 80);

							this.parent.Var_b1e8 = 1;
						}

						currentWorkingCitizenCount = city.ActualSize + 1;
					}

					if (currentWorkingCitizenCount == this.parent.Var_e8b8 || currentWorkingCitizenCount == 0)
						goto L1292;
				}

				if (currentWorkingCitizenCount > 0 && cityFreeArea[20] == 0)
				{
					cityFreeArea[20] = 1;

					// Instruction address 0x1d12:0x0eb2, size: 5
					F0_1d12_692d_CityResources(cityID, 20, flag);

					newWorkerFlags |= 0x100000;
					currentWorkingCitizenCount--;
				}

				while ((((city.ActualSize * 2) + (productionRatio * this.parent.Var_e3c6)) > ((currentWorkingCitizenCount / 2) + this.parent.Var_70da_Arr[0]) || city.ActualSize < 3) &&
					currentWorkingCitizenCount > this.parent.Var_e8b8 && bestCityCellIndex != -1)
				{
					bestCityCellIndex = -1;
					int maximumCityProduction = 0;
					int maximumCellFood = 0;

					for (int i = 0; i < this.Var_6540_CityOffsetCount; i++)
					{
						if (cityFreeArea[i] == 0)
						{
							int newX = city.Position.X + this.parent.CityOffsets[i].X;
							int newY = city.Position.Y + this.parent.CityOffsets[i].Y;

							// Instruction address 0x1d12:0x0f9e, size: 5
							int cityFood = F0_1d12_6abc_GetCityResourceCount(this.Var_6548_PlayerID, this.Var_653e_CityID, newX, newY, CityResourceTypeEnum.Food);

							// Instruction address 0x1d12:0x0fb6, size: 5
							int cityProduction = F0_1d12_6abc_GetCityResourceCount(this.Var_6548_PlayerID, this.Var_653e_CityID, newX, newY, CityResourceTypeEnum.Production) * 2;

							if (currentWorkingCitizenCount != 1 || this.parent.Var_70da_Arr[1] != 0 || cityProduction != 0)
							{
								// Instruction address 0x1d12:0x0ff0, size: 5
								cityProduction += F0_1d12_6abc_GetCityResourceCount(this.Var_6548_PlayerID, this.Var_653e_CityID, newX, newY, CityResourceTypeEnum.Trade);

								if (cityFood > maximumCellFood || (cityFood == maximumCellFood && cityProduction > maximumCityProduction))
								{
									maximumCellFood = cityFood;
									maximumCityProduction = cityProduction;
									bestCityCellIndex = i;
								}
							}
						}
					}

					if (bestCityCellIndex != -1)
						break;

					cityFreeArea[bestCityCellIndex] = 1;

					// Instruction address 0x1d12:0x105d, size: 5
					F0_1d12_692d_CityResources(cityID, bestCityCellIndex, flag);

					newWorkerFlags |= (uint)(1 << bestCityCellIndex);
					currentWorkingCitizenCount--;
				}

				if (this.Var_6548_PlayerID != this.parent.GameData.HumanPlayerID && (city.Status & CityStatusEnum.CivilDisorder) != CityStatusEnum.None)
				{
					this.parent.Var_e8b8++;
				}

				while (currentWorkingCitizenCount > this.parent.Var_e8b8)
				{
					bestCityCellIndex = -1;
					int local_d6 = 0;

					for (int i = 0; i < this.Var_6540_CityOffsetCount; i++)
					{
						if (cityFreeArea[i] == 0)
						{
							int newX = city.Position.X + this.parent.CityOffsets[i].X;
							int newY = city.Position.Y + this.parent.CityOffsets[i].Y;

							// Instruction address 0x1d12:0x1151, size: 5
							int cityFood = F0_1d12_6abc_GetCityResourceCount(this.Var_6548_PlayerID, this.Var_653e_CityID, newX, newY, CityResourceTypeEnum.Food) * 
								(16 / this.parent.Tools.CheckValueRange(this.parent.Var_70da_Arr[0] - (city.ActualSize * 2) - (productionRatio * this.parent.Var_e3c6), 1, 99));

							// Instruction address 0x1d12:0x1190, size: 5
							int resourceCount = F0_1d12_6abc_GetCityResourceCount(this.Var_6548_PlayerID, this.Var_653e_CityID, newX, newY, CityResourceTypeEnum.Production);

							// Instruction address 0x1d12:0x11af, size: 5
							cityFood += ((city.ActualSize * 3) / this.parent.Tools.CheckValueRange(
								this.parent.Var_70da_Arr[1] - this.parent.Var_d2f6, 1, 99)) * resourceCount;

							// Instruction address 0x1d12:0x11dc, size: 5
							int tradeResources = F0_1d12_6abc_GetCityResourceCount(this.Var_6548_PlayerID, this.Var_653e_CityID, newX, newY, CityResourceTypeEnum.Trade);

							// Instruction address 0x1d12:0x11f7, size: 5
							cityFood += ((city.ActualSize * 2) / this.parent.Tools.CheckValueRange(this.parent.Var_70da_Arr[2], 1, 99)) * tradeResources;

							if (cityFood > local_d6)
							{
								local_d6 = cityFood;
								bestCityCellIndex = i;
							}
						}
					}

					if (bestCityCellIndex == -1)
						break;

					cityFreeArea[bestCityCellIndex] = 1;

					// Instruction address 0x1d12:0x1267, size: 5
					F0_1d12_692d_CityResources(cityID, bestCityCellIndex, flag);

					newWorkerFlags |= (uint)(1 << bestCityCellIndex);
					currentWorkingCitizenCount--;
				}

			L1292:
				city.WorkerFlags = (uint)((uint)currentWorkingCitizenCount << 26) | newWorkerFlags;

				int nonworkingCitizenCount = -currentWorkingCitizenCount;

			L12c2:
				cityX = city.Position.X;
				cityY = city.Position.Y;
				int workingCitizenCount = currentWorkingCitizenCount;

				for (int i = 0; i < workingCitizenCount; i++)
				{
					// Instruction address 0x1d12:0x1309, size: 5
					if (F0_1d12_6da1_GetSpecialWorkerFlags(i) == 0)
					{
						// Instruction address 0x1d12:0x1321, size: 5
						F0_1d12_6d6e_SetSpecialWorkerFlags(i, 3);
					}
				}

				for (int i = workingCitizenCount; i < 8; i++)
				{
					// Instruction address 0x1d12:0x134c, size: 5
					F0_1d12_6d6e_SetSpecialWorkerFlags(i, 0);
				}

				int cityPowerProductionRatio = 0;
				int cityFactoryRatio = 0;
				this.parent.Var_6c98 = 1;

				if (city.HasImprovement(ImprovementEnum.Factory))
				{
					cityFactoryRatio = 2;
				}

				if (city.HasImprovement(ImprovementEnum.ManufacturingPlant))
				{
					cityFactoryRatio = 4;
				}

				if (city.HasImprovement(ImprovementEnum.PowerPlant))
				{
					cityPowerProductionRatio = 2;
				}

				if (city.HasImprovement(ImprovementEnum.HydroPlant))
				{
					this.parent.Var_6c98 = 2;
					cityPowerProductionRatio = 2;
				}

				if (city.HasImprovement(ImprovementEnum.NuclearPlant))
				{
					this.parent.Var_6c98 = 2;
					cityPowerProductionRatio = 2;
				}

				// Instruction address 0x1d12:0x13f9, size: 5
				if (F0_1d12_6c97_PlayerHasWonder(this.Var_6548_PlayerID, WonderEnum.HooverDam))
				{
					// Instruction address 0x1d12:0x1428, size: 5
					// Instruction address 0x1d12:0x1442, size: 5
					if (this.parent.MapManagement.F0_2aea_1942_GetGroupID(city.Position.X, city.Position.Y) ==
						this.parent.MapManagement.F0_2aea_1942_GetGroupID(
						this.parent.GameData.Cities[this.parent.GameData.WonderCityID[(int)WonderEnum.HooverDam]].Position.X,
						this.parent.GameData.Cities[this.parent.GameData.WonderCityID[(int)WonderEnum.HooverDam]].Position.Y))
					{
						this.parent.Var_6c98 = 2;
						cityPowerProductionRatio = 2;
					}
				}

				if (city.HasImprovement(ImprovementEnum.RecyclingCenter))
				{
					this.parent.Var_6c98 = 3;
				}

				// Instruction address 0x1d12:0x1484, size: 5
				cityPowerProductionRatio = this.parent.Tools.CheckValueRange(cityPowerProductionRatio, 0, cityFactoryRatio);
				int temp1 = this.parent.Var_70da_Arr[1];
				int temp2 = this.parent.Var_70da_Arr[2];

				this.parent.Var_70da_Arr[1] += ((cityFactoryRatio * this.parent.Var_70da_Arr[1]) / 4) + ((cityPowerProductionRatio * this.parent.Var_70da_Arr[1]) / 4);

				if (flag == 0)
				{
					city.FoodCount += (short)(this.parent.Var_70da_Arr[0] - (productionRatio * this.parent.Var_e3c6) - (city.ActualSize * 2));

					int newShields = this.parent.Var_70da_Arr[1];
					newShields -= this.parent.Var_d2f6;

					if ((city.Status & CityStatusEnum.CivilDisorder) != CityStatusEnum.None)
					{
						newShields = 0;
					}

					city.ShieldsCount += (short)newShields;

					if (city.CurrentProductionID >= 0)
					{
						if ((this.parent.GameData.Units[city.CurrentProductionID].Cost * difficultyProductionRatio) <= city.ShieldsCount)
						{
							if (city.CurrentProductionID == 0 && city.ActualSize == 1 && this.parent.GameData.DifficultyLevel == 0)
							{
								goto L2307;
							}
							else
							{
								city.ShieldsCount -= (short)(this.parent.GameData.Units[city.CurrentProductionID].Cost * difficultyProductionRatio);

								int newUnitID = -1;

								if (this.Var_6548_PlayerID == this.parent.GameData.HumanPlayerID || city.CurrentProductionID != (int)UnitTypeEnum.Diplomat)
								{
									// Instruction address 0x1d12:0x15fa, size: 5
									newUnitID = this.parent.UnitManagement.F0_1866_0cf5_CreateUnit(this.Var_6548_PlayerID, (UnitTypeEnum)city.CurrentProductionID, 
										city.Position.X, city.Position.Y);
								}

								if ((this.parent.GameData.TechnologyFirstDiscoveredBy[city.CurrentProductionID] & 0x8) == 0)
								{
									// Instruction address 0x1d12:0x1640, size: 5
									this.parent.UnitManagement.F0_1866_250e_AddReplayData(6, (byte)((sbyte)this.Var_6548_PlayerID), (byte)city.CurrentProductionID);

									this.parent.GameData.TechnologyFirstDiscoveredBy[city.CurrentProductionID] |= 8;
								}

								if (city.HasImprovement(ImprovementEnum.Barracks) && newUnitID != -1)
								{
									this.parent.GameData.Players[this.Var_6548_PlayerID].Units[newUnitID].Status |= UnitStatusEnum.Veteran;
								}

								if (newUnitID != -1 && city.CurrentProductionID == 0 && (city.ActualSize > 1 || this.parent.GameData.Players[this.Var_6548_PlayerID].CityCount > 1))
								{
									city.ActualSize--;

									if (city.ActualSize == 0)
									{
										// Instruction address 0x1d12:0x16ff, size: 5
										this.parent.Segment_1ade.F0_1ade_018e(cityID, city.Position.X, city.Position.Y);

										// Instruction address 0x1d12:0x1727, size: 5
										this.parent.UnitManagement.F0_1866_0cf5_CreateUnit(this.Var_6548_PlayerID, (UnitTypeEnum)city.CurrentProductionID, city.Position.X, city.Position.Y);

										this.parent.StartGameMenu.F5_0000_0e6c_TestIfAIPlayerIsDestroyed(this.Var_6548_PlayerID, 0);

										goto L6927;
									}
								}

								if (this.Var_6548_PlayerID != this.parent.GameData.HumanPlayerID && newUnitID != -1)
								{
									if (city.CurrentProductionID == 27)
									{
										int selectedTradeCityID = -1;
										int maximumBaseTrade = -1;

										for (int i = 0; i < 128; i++)
										{
											if (this.parent.GameData.Cities[i].PlayerID != this.Var_6548_PlayerID && this.parent.GameData.Cities[i].Status != CityStatusEnum.Invalid &&
												this.parent.GameData.Cities[i].PlayerID != this.parent.GameData.HumanPlayerID && this.parent.GameData.Cities[i].BaseTrade > maximumBaseTrade)
											{
												maximumBaseTrade = this.parent.GameData.Cities[i].BaseTrade;
												selectedTradeCityID = i;
											}
										}

										// Instruction address 0x1d12:0x180e, size: 5
										this.parent.Segment_2459.F0_2459_0948_CaravanArrivesAtDestinationCity(this.Var_6548_PlayerID, newUnitID, selectedTradeCityID);
									}

									if (city.CurrentProductionID == 26)
									{
										int selectedUnitID = -1;
										int minimumUnitDistance = int.MaxValue;
										int selectionVariance = 0;
										int selectedCityID = 0;

										for (int i = 0; i < 128; i++)
										{
											if (this.parent.GameData.Cities[i].PlayerID != this.parent.GameData.HumanPlayerID || this.parent.GameData.Cities[i].Status != CityStatusEnum.Invalid)
											{
												// Instruction address 0x1d12:0x1898, size: 5
												int nearestUnitID = this.parent.Tools.FindNearestPlayerUnit(this.Var_6548_PlayerID, newUnitID,
													this.parent.GameData.Cities[i].Position.X, this.parent.GameData.Cities[i].Position.Y);

												int unitDistance = Math.Abs(this.parent.MapManagement.GetDistance(this.parent.GameData.Cities[i].Position, 
													this.parent.GameData.Players[this.Var_6548_PlayerID].Units[nearestUnitID].Position) - 3);

												if ((this.parent.GameData.Cities[i].Status & CityStatusEnum.TechnologyStolen) != CityStatusEnum.None)
												{
													unitDistance = 3;
												}

												if (minimumUnitDistance > unitDistance || (minimumUnitDistance == unitDistance && this.parent.CAPI.RNG.Next(++selectionVariance) == 0))
												{
													// Instruction address 0x1d12:0x191e, size: 5
													TerrainTypeEnum terrainType = this.parent.MapManagement.GetTerrainType(
														this.parent.GameData.Players[this.Var_6548_PlayerID].Units[nearestUnitID].Position.X,
														this.parent.GameData.Players[this.Var_6548_PlayerID].Units[nearestUnitID].Position.Y);

													// Instruction address 0x1d12:0x1956, size: 5
													// Instruction address 0x1d12:0x1970, size: 5
													if (terrainType != TerrainTypeEnum.Water && this.parent.MapManagement.F0_2aea_1942_GetGroupID(
														this.parent.GameData.Cities[i].Position.X,	this.parent.GameData.Cities[i].Position.Y) ==
														this.parent.MapManagement.F0_2aea_1942_GetGroupID(
														this.parent.GameData.Players[this.Var_6548_PlayerID].Units[nearestUnitID].Position.X,
														this.parent.GameData.Players[this.Var_6548_PlayerID].Units[nearestUnitID].Position.Y))
													{
														if (minimumUnitDistance != unitDistance)
														{
															minimumUnitDistance = unitDistance;
														}
														else
														{
															selectionVariance = 1;
														}

														selectedUnitID = nearestUnitID;
														selectedCityID = i;
													}
												}
											}
										}

										if (minimumUnitDistance > 3 || 
											this.parent.GameData.Players[this.Var_6548_PlayerID].Diplomacy[this.parent.GameData.HumanPlayerID].HasFlag(DiplomacyFlagsEnum.Peace))
										{
											city.ShieldsCount += (short)(this.parent.GameData.Units[city.CurrentProductionID].Cost * difficultyProductionRatio);
										}
										else
										{
											// Instruction address 0x1d12:0x19f9, size: 5
											newUnitID = this.parent.UnitManagement.F0_1866_0cf5_CreateUnit(
												this.Var_6548_PlayerID, UnitTypeEnum.Diplomat,
												this.parent.GameData.Players[this.Var_6548_PlayerID].Units[selectedUnitID].Position.X,
												this.parent.GameData.Players[this.Var_6548_PlayerID].Units[selectedUnitID].Position.Y);

											if (newUnitID != -1)
											{
												this.parent.GameData.Players[this.Var_6548_PlayerID].Units[newUnitID].GoToDestination = 
													this.parent.GameData.Cities[selectedCityID].Position;
											}
										}
									}

									if (city.CurrentProductionID == 25 && this.parent.GameData.Players[this.Var_6548_PlayerID].ActiveUnits[(int)UnitTypeEnum.Nuclear] == 1)
									{
										this.parent.GameData.Players[this.parent.GameData.HumanPlayerID].ContactPlayerCountdown = -1;
									}

									// Instruction address 0x1d12:0x1aab, size: 5
									this.parent.AIEngine.F0_25fb_34b6_ChangeCityProduction(cityID);
								}
								else
								{
									city.ShieldsCount = 0;

									if (newUnitID != -1 && (city.CurrentProductionID == 0 || city.CurrentProductionID >= 26))
									{
										this.parent.Var_b1e8 = 1;
										this.parent.Var_2f9e_MessageBoxStyle = MenuBoxReportTypeEnum.DefenseMinisterReport;

										// Instruction address 0x1d12:0x1b58, size: 5
										this.parent.Segment_1238.F0_1238_001e_ShowDialog($"{this.parent.Segment_2459.F0_2459_08c6_GetCityName(cityID)} builds " +
											$"{this.parent.GameData.Units[city.CurrentProductionID].Name}.\n", 80, 80);
									}
								}
							}
						}
					}
					else
					{
						if ((this.parent.GameData.GetImprovementType(-city.CurrentProductionID).Cost * difficultyProductionRatio) <= city.ShieldsCount)
						{
							newShields = -city.CurrentProductionID;

							if (newShields > 24)
							{
								if (this.parent.GameData.WonderCityID[newShields - 24] == -1)
								{
									this.parent.GameData.WonderCityID[newShields - 24] = (short)cityID;

									// Instruction address 0x1d12:0x1bd7, size: 5
									this.parent.UnitManagement.F0_1866_250e_AddReplayData(10, (byte)((sbyte)this.Var_6548_PlayerID), (byte)((sbyte)(newShields - 24)));
								}
								else
								{
									newShields = -1;
								}
							}
							else if (city.HasImprovement(City.BitToImprovementEnum(newShields - 1)))
							{
								newShields = -1;
							}

							if (newShields != -1)
							{
								if (newShields == 41)
								{
									this.parent.CommonTools.PlayTune(36, 0);

									this.parent.News.F21_0000_0000_ShowNews(-1,
										$"Sensors report a\nNUCLEAR WEAPONS test\nnear {this.parent.Segment_2459.F0_2459_08c6_GetCityName(cityID)}!\n");

									this.parent.CommonTools.PlayTune(1, 0);
								}

								if (newShields < 22)
								{
									city.AddImprovement(City.BitToImprovementEnum(newShields - 1));
								}

								city.ShieldsCount -= (short)(this.parent.GameData.GetImprovementType(-city.CurrentProductionID).Cost * difficultyProductionRatio);

								if ((this.parent.GameData.PlayerFlags & (1 << this.Var_6548_PlayerID)) != 0 || newShields > 24)
								{
									if ((this.parent.GameData.PlayerFlags & (1 << this.Var_6548_PlayerID)) == 0)
									{
										this.parent.GameData.MapVisibility[city.Position.X, city.Position.Y] = 0xffff;
									}
									else
									{
										this.parent.Var_b1e8 = 1;
									}

									// Instruction address 0x1d12:0x1dbe, size: 5
									if (this.Var_6548_PlayerID == this.parent.GameData.HumanPlayerID && 
										this.parent.GameData.GameSettingFlags.Animations && 
										(newShields <= 21 || newShields > 24) && (city.Status & CityStatusEnum.AutoProduction) == CityStatusEnum.None && newShields != 1)
									{
										this.parent.CityView.F19_0000_0000_ShowCityLayout(cityID, newShields,
											$"{this.parent.Segment_2459.F0_2459_08c6_GetCityName(cityID)} builds\n{this.parent.GameData.GetImprovementType(newShields).Name}.\n");
									}
									else
									{
										this.parent.News.F21_0000_0000_ShowNews(cityID,
											$"{this.parent.Segment_2459.F0_2459_08c6_GetCityName(cityID)} builds\n{this.parent.GameData.GetImprovementType(newShields).Name}.\n");
									}

									city.ShieldsCount = 0;
								}
								else
								{
									if (newShields == 1)
									{
										city.CurrentProductionID = -2;
									}

									// Instruction address 0x1d12:0x1e1a, size: 5
									city.CurrentProductionID = (sbyte)((short)this.parent.Segment_1ade.F0_1ade_0421(this.Var_6548_PlayerID, cityID));

									if (city.CurrentProductionID >= 0)
									{
										this.parent.GameData.Players[this.Var_6548_PlayerID].UnitsInProduction[city.CurrentProductionID]++;
									}
								}

								if (newShields >= 22 && newShields <= 24 && ((1 << this.Var_6548_PlayerID) & (ushort)this.parent.GameData.SpaceshipFlags) == 0)
								{
									if (this.Var_6548_PlayerID == this.parent.GameData.HumanPlayerID)
									{
										this.parent.Overlay_18.F18_0000_0f83(this.Var_6548_PlayerID, (ushort)((short)(newShields - 22)));
									}
									else
									{
										int local_c8 = (short)this.parent.Overlay_18.F18_0000_0d4f(this.Var_6548_PlayerID, (short)(newShields - 22));

										if (local_c8 != 0 && newShields < 24)
										{
											city.ShieldsCount += (short)(this.parent.GameData.GetImprovementType(newShields).Cost * difficultyProductionRatio);

											if (city.CurrentProductionID <= -22 && city.CurrentProductionID >= -24)
											{
												city.CurrentProductionID = (sbyte)((-newShields) - 1);
											}

											local_c8--;
										}

										if ((this.parent.GameData.SpaceshipFlags & (1 << this.Var_6548_PlayerID)) == 0 && this.parent.GameData.AISpaceshipSuccessRate >= 40)
										{
											if (local_c8 == 0)
											{
												this.parent.Overlay_18.F18_0000_15c3(this.Var_6548_PlayerID);
											}
											else if (this.parent.Segment_1ade.F0_1ade_22b5_PlayerHasTechnology(this.parent.GameData.HumanPlayerID, TechnologyAdvanceEnum.SpaceFlight))
											{
												if (this.parent.GameData.AISpaceshipSuccessRate > 75)
												{
													this.parent.Overlay_18.F18_0000_15c3(this.Var_6548_PlayerID);
												}
												else if (this.parent.GameData.Players[this.parent.GameData.HumanPlayerID].Ranking > this.parent.GameData.Players[this.Var_6548_PlayerID].Ranking)
												{
													this.parent.Overlay_18.F18_0000_15c3(this.Var_6548_PlayerID);
												}
												else if ((this.parent.GameData.SpaceshipFlags & (1 << this.parent.GameData.HumanPlayerID)) != 0 &&
													(this.parent.GameData.Players[this.Var_6548_PlayerID].SpaceshipETAYear - this.parent.GameData.Year) <=
														(this.parent.GameData.Players[this.parent.GameData.HumanPlayerID].SpaceshipETAYear - this.parent.GameData.Year))
												{
													this.parent.Overlay_18.F18_0000_15c3(this.Var_6548_PlayerID);
												}
												else if ((this.parent.GameData.SpaceshipFlags & (1 << (this.parent.GameData.HumanPlayerID + 8))) != 0 &&
													this.parent.GameData.Players[this.parent.GameData.ActiveCivilizations].Coins > 1000)
												{
													this.parent.Overlay_18.F18_0000_15c3(this.Var_6548_PlayerID);
												}
											}
										}
										else if (local_c8 != 0)
										{
											this.parent.Overlay_18.F18_0000_15c3(this.Var_6548_PlayerID);
										}
									}
								}

								if (newShields == 1)
								{
									for (int i = 0; i < 128; i++)
									{
										if (this.parent.GameData.Cities[i].PlayerID == this.Var_6548_PlayerID)
										{
											this.parent.GameData.Cities[i].RemoveImprovement(ImprovementEnum.Palace);
										}
									}

									if (this.Var_6548_PlayerID == this.parent.GameData.HumanPlayerID ||
										this.parent.GameData.Players[this.parent.GameData.HumanPlayerID].Diplomacy[this.Var_6548_PlayerID].HasFlag(DiplomacyFlagsEnum.EmbassyEstablished))
									{
										if (this.Var_6548_PlayerID != this.parent.GameData.HumanPlayerID)
										{
											this.parent.Var_2f9e_MessageBoxStyle = MenuBoxReportTypeEnum.DiplomatsReport;

											// Instruction address 0x1d12:0x20f8, size: 5
											this.parent.Segment_1238.F0_1238_001e_ShowDialog($"Diplomats report:\n" +
												$"{this.parent.GameData.Players[this.Var_6548_PlayerID].Nationality} capital\nmoved to " +
												$"{this.parent.Segment_2459.F0_2459_08c6_GetCityName(cityID)}.\n", 80, 80);
										}
										else
										{
											this.parent.Var_2f9e_MessageBoxStyle = MenuBoxReportTypeEnum.ForeignMinisterReport;

											// Instruction address 0x1d12:0x20f8, size: 5
											this.parent.Segment_1238.F0_1238_001e_ShowDialog($"{this.parent.GameData.Players[this.Var_6548_PlayerID].Nationality}" +
												$" capital\nmoved to {this.parent.Segment_2459.F0_2459_08c6_GetCityName(cityID)}.\n", 80, 80);
										}

										this.parent.GameData.Players[this.Var_6548_PlayerID].XStart = (short)city.Position.X;

										city.AddImprovement(ImprovementEnum.Palace);
									}
								}

								if (newShields == 38)
								{
									// Instruction address 0x1d12:0x2143, size: 5
									this.parent.Segment_1ade.F0_1ade_1584(this.Var_6548_PlayerID, 0);

									// Instruction address 0x1d12:0x2153, size: 5
									this.parent.Segment_1ade.F0_1ade_1584(this.Var_6548_PlayerID, 0);
								}

								if (newShields == 43 && this.Var_6548_PlayerID == this.parent.GameData.HumanPlayerID)
								{
									for (int i = 0; i < 128; i++)
									{
										if (this.parent.GameData.Cities[i].Status != CityStatusEnum.Invalid && this.parent.GameData.Cities[i].PlayerID != this.Var_6548_PlayerID)
										{
											this.parent.GameData.Cities[i].VisibleSize = this.parent.GameData.Cities[i].ActualSize;

											this.parent.GameData.MapVisibility[this.parent.GameData.Cities[i].Position.X,
												this.parent.GameData.Cities[i].Position.Y] |= (ushort)(1 << this.Var_6548_PlayerID);

											// Instruction address 0x1d12:0x2204, size: 5
											this.parent.MapManagement.F0_2aea_1601_UpdateVisibleCellStatus(
												this.parent.GameData.Cities[i].Position.X, this.parent.GameData.Cities[i].Position.Y);
										}
									}
								}
							}
						}
					}

					if ((this.Var_6548_PlayerID == this.parent.GameData.HumanPlayerID && (city.Status & CityStatusEnum.AutoProduction) != CityStatusEnum.None) &&
						(city.ShieldsCount == 0 || (city.CurrentProductionID < 0 && newShields == -1)))
					{
						// Instruction address 0x1d12:0x2262, size: 5
						newShields = (short)this.parent.Segment_1ade.F0_1ade_0421(this.Var_6548_PlayerID, cityID);

						this.parent.Var_aa_Screen0_Rectangle.FontID = 2;

						if (newShields == 99)
						{
							city.ClearStatusFlags(CityStatusEnum.AutoProduction);
						}
						else
						{
							this.parent.Var_b1e8 = 0;

							if (city.CurrentProductionID >= 0)
							{
								this.parent.GameData.Players[this.Var_6548_PlayerID].UnitsInProduction[city.CurrentProductionID]--;
							}

							city.CurrentProductionID = (sbyte)newShields;

							if (city.CurrentProductionID >= 0)
							{
								this.parent.GameData.Players[this.Var_6548_PlayerID].UnitsInProduction[city.CurrentProductionID]++;
							}
						}
					}

				L2307:
					if ((this.parent.GameData.PlayerFlags & (1 << this.Var_6548_PlayerID)) == 0)
					{
						if ((city.Status & CityStatusEnum.AutoProduction) != CityStatusEnum.None)
						{
							// Instruction address 0x1d12:0x232e, size: 5
							this.parent.AIEngine.F0_25fb_34b6_ChangeCityProduction(cityID);
						}

						int cityShieldIncome = 0;

						// Instruction address 0x1d12:0x2344, size: 5
						PlayerContinentStrategyEnum local_104 = this.parent.GameData.Players[this.Var_6548_PlayerID].Continents[this.parent.MapManagement.F0_2aea_1942_GetGroupID(cityX, cityY)].Strategy;

						if ((local_104 == PlayerContinentStrategyEnum.Attack || local_104 == PlayerContinentStrategyEnum.Defend || local_104 == PlayerContinentStrategyEnum.Transport) && 
							newShields != 0 && city.CurrentProductionID >= 0 &&
							(int)this.parent.GameData.Units[city.CurrentProductionID].UnitRoleType == (int)local_104)
						{
							cityShieldIncome = this.parent.GameData.Players[this.Var_6548_PlayerID].Coins / 64;
						}

						if ((this.parent.GameData.SpaceshipFlags & (1 << (this.parent.GameData.HumanPlayerID + 8))) != 0 &&
							(-city.CurrentProductionID) >= 0x16 && (-city.CurrentProductionID) <= 0x18)
						{
							cityShieldIncome = this.parent.GameData.Players[this.Var_6548_PlayerID].Coins / 128;
						}

						if ((city.Status & CityStatusEnum.CivilDisorder) != CityStatusEnum.None && city.CurrentProductionID < 0 && city.ShieldsCount != 0)
						{
							// Instruction address 0x1d12:0x24a7, size: 5
							cityShieldIncome = this.parent.Tools.CheckValueRange(
								(this.parent.GameData.GetImprovementType(-city.CurrentProductionID).Cost * difficultyProductionRatio) - city.ShieldsCount,
								0, this.parent.GameData.Players[this.Var_6548_PlayerID].Coins / 8);
						}

						// Instruction address 0x1d12:0x24bb, size: 5
						if ((this.parent.MapManagement.F0_2aea_14e0_GetCellActiveUnitPlayerID(cityX, cityY) == -1 || 
							(city.Status & CityStatusEnum.AutoProduction) != CityStatusEnum.None) && city.CurrentProductionID >= 0 && city.ShieldsCount != 0)
						{
							// Instruction address 0x1d12:0x2530, size: 5
							cityShieldIncome = this.parent.Tools.CheckValueRange(
								(this.parent.GameData.Units[city.CurrentProductionID].Cost * difficultyProductionRatio) - city.ShieldsCount,
								0, this.parent.GameData.Players[this.Var_6548_PlayerID].Coins / 3);
						}

						if (city.CurrentProductionID == -1 && city.ShieldsCount != 0)
						{
							// Instruction address 0x1d12:0x2591, size: 5
							cityShieldIncome = this.parent.Tools.CheckValueRange(
								(this.parent.GameData.Units[city.CurrentProductionID].Cost * difficultyProductionRatio) - city.ShieldsCount,
								0, this.parent.GameData.Players[this.Var_6548_PlayerID].Coins / 3);
						}

						if (city.CurrentProductionID >= 0 && city.Status == (CityStatusEnum.CivilDisorder | CityStatusEnum.NearRiverOrMountains | CityStatusEnum.AutoProduction) && 
							this.parent.GameData.Players[this.Var_6548_PlayerID].ActiveUnits[(int)UnitTypeEnum.Nuclear] == 0 && city.ShieldsCount != 0)
						{
							// Instruction address 0x1d12:0x260d, size: 5
							cityShieldIncome = this.parent.Tools.CheckValueRange(
								(this.parent.GameData.Units[city.CurrentProductionID].Cost * difficultyProductionRatio) - city.ShieldsCount,
								0, this.parent.GameData.Players[this.Var_6548_PlayerID].Coins / 4);
						}

						if (this.parent.GameData.Players[this.Var_6548_PlayerID].Coins > 2000)
						{
							cityShieldIncome += this.parent.GameData.Players[this.Var_6548_PlayerID].Coins / 512;
						}

						city.ShieldsCount += (short)cityShieldIncome;
						this.parent.GameData.Players[this.Var_6548_PlayerID].Coins -= (short)(cityShieldIncome * 2);
						city.ClearStatusFlags(CityStatusEnum.AutoProduction);
					}
				}

				if (flag == 1)
				{
					// Instruction address 0x1d12:0x2697, size: 5
					this.parent.DrawTools.DrawRectangleAndFillWithPattern(2, 67, 124, 104, 208, 100, 16, 16);

					// Instruction address 0x1d12:0x26af, size: 5
					this.parent.DrawTools.DrawRectangleAndFillWithPattern(95, 106, 227, 197, 208, 100, 16, 16);

					// Instruction address 0x1d12:0x26cf, size: 5
					this.parent.DrawTools.DrawFilledRectangleWithCenteredText(95, 106, 128, 114, "INFO", 9);

					// Instruction address 0x1d12:0x26ef, size: 5
					this.parent.DrawTools.DrawFilledRectangleWithCenteredText(129, 106, 160, 114, "HAPPY", 9);

					// Instruction address 0x1d12:0x270f, size: 5
					this.parent.DrawTools.DrawFilledRectangleWithCenteredText(161, 106, 193, 114, "MAP", 9);

					// Instruction address 0x1d12:0x272f, size: 5
					this.parent.DrawTools.DrawFilledRectangleWithCenteredText(194, 106, 226, 114, "VIEW", 9);

					// Instruction address 0x1d12:0x275a, size: 5
					this.parent.Graphics.F0_VGA_009a_ReplaceColor(this.parent.Var_aa_Screen0_Rectangle, (33 * this.Var_2496) + 96, 107, 32, 7, 9, 15);

					if (this.Var_2496 == 2)
					{
						// Instruction address 0x1d12:0x2768, size: 5
						F0_1d12_72b7_DrawCityWorldMap();
					}
				}
			
				cityX = city.Position.X;
				cityY = city.Position.Y;

				this.parent.Var_d2e0 = 0;

				if (this.parent.GameData.Players[this.Var_6548_PlayerID].GovernmentType == GovernmentTypeEnum.Communist)
				{
					this.Var_6b30 = 10;
				}

				this.parent.Var_d2e0 = ((this.parent.Var_70da_Arr[2] * this.Var_6b30) * 3) / (((int)this.parent.GameData.Players[this.Var_6548_PlayerID].GovernmentType * 20) + 80);

				if (city.HasImprovement(ImprovementEnum.Courthouse) || city.HasImprovement(ImprovementEnum.Palace))
				{
					this.parent.Var_d2e0 /= 2;
				}
			
				if (this.parent.GameData.Players[this.Var_6548_PlayerID].GovernmentType == GovernmentTypeEnum.Democracy)
				{
					this.parent.Var_d2e0 = 0;
				}

				city.BaseTrade = (sbyte)(this.parent.Var_70da_Arr[2] - this.parent.Var_d2e0);

				for (int i = 0; i < 3; i++)
				{
					int tradeCityID = city.TradeCityIDs[i];

					if (tradeCityID != -1)
					{
						if (this.parent.GameData.Cities[tradeCityID].PlayerID == this.Var_6548_PlayerID)
						{
							this.parent.Var_70da_Arr[2] += (this.parent.GameData.Cities[tradeCityID].BaseTrade + this.parent.Var_70da_Arr[2] + 4) / 16;
						}
						else
						{
							this.parent.Var_70da_Arr[2] += (this.parent.GameData.Cities[tradeCityID].BaseTrade + this.parent.Var_70da_Arr[2] + 4) / 8;
						}
					}
				}

				this.parent.Var_d2e0 = ((this.parent.Var_70da_Arr[2] * this.Var_6b30) * 3) / (((int)this.parent.GameData.Players[this.Var_6548_PlayerID].GovernmentType * 20) + 80);

				if (this.parent.GameData.Players[this.Var_6548_PlayerID].GovernmentType == GovernmentTypeEnum.Democracy)
				{
					this.parent.Var_d2e0 = 0;
				}

				if (city.HasImprovement(ImprovementEnum.Courthouse))
				{
					this.parent.Var_d2e0 /= 2;
				}

				// Instruction address 0x1d12:0x2960, size: 5
				this.parent.Var_70da_Arr[3] = this.parent.Tools.CheckValueRange(
					((-(this.parent.GameData.Players[this.Var_6548_PlayerID].ScienceTaxRate +
						this.parent.GameData.Players[this.Var_6548_PlayerID].TaxRate - 10) *
						(this.parent.Var_70da_Arr[2] - this.parent.Var_d2e0)) + 5) / 10,
					0, this.parent.Var_70da_Arr[2]);

				// !!! Minimum value is greater than maximum value, why?
				// Instruction address 0x1d12:0x2999, size: 5
				this.parent.Var_e17a = this.parent.Tools.CheckValueRange(
					((this.parent.GameData.Players[this.Var_6548_PlayerID].TaxRate * (this.parent.Var_70da_Arr[2] - this.parent.Var_d2e0)) + 5) / 10,
					0, this.parent.Var_70da_Arr[2] - this.parent.Var_70da_Arr[3] - this.parent.Var_d2e0);

				this.parent.Var_70e6 = this.parent.Var_70da_Arr[2] - this.parent.Var_70da_Arr[3] - this.parent.Var_e17a - this.parent.Var_d2e0;

				// Instruction address 0x1d12:0x29ba, size: 5
				this.parent.Var_e17a += F0_1d12_6dcc_GetWorkerCountByType(1) * 2;

				// Instruction address 0x1d12:0x29cc, size: 5
				this.parent.Var_70e6 += F0_1d12_6dcc_GetWorkerCountByType(2) * 2;

				// Instruction address 0x1d12:0x29de, size: 5
				this.parent.Var_70da_Arr[3] += F0_1d12_6dcc_GetWorkerCountByType(3) * 2;

				if (city.HasImprovement(ImprovementEnum.MarketPlace))
				{
					this.parent.Var_70da_Arr[3] += this.parent.Var_70da_Arr[3] / 2;
					this.parent.Var_e17a += this.parent.Var_e17a / 2;
				}

				if (city.HasImprovement(ImprovementEnum.Bank))
				{
					this.parent.Var_70da_Arr[3] += this.parent.Var_70da_Arr[3] / 2;
					this.parent.Var_e17a += this.parent.Var_e17a / 2;
				}
			
				int unhappyCitizenRatio = this.parent.Var_70e6;

				if (city.HasImprovement(ImprovementEnum.Library))
				{
					unhappyCitizenRatio += this.parent.Var_70e6 / 2;

					// Instruction address 0x1d12:0x2a6e, size: 5
					if (F0_1d12_6c97_PlayerHasWonder(this.Var_6548_PlayerID, WonderEnum.IsaacNewtonsCollege))
					{
						unhappyCitizenRatio += this.parent.Var_70e6 / 3;
					}
				}

				if (city.HasImprovement(ImprovementEnum.University))
				{
					unhappyCitizenRatio += this.parent.Var_70e6 / 2;

					// Instruction address 0x1d12:0x2ab2, size: 5
					if (F0_1d12_6c97_PlayerHasWonder(this.Var_6548_PlayerID, WonderEnum.IsaacNewtonsCollege))
					{
						unhappyCitizenRatio += this.parent.Var_70e6 / 3;
					}
				}

				// Instruction address 0x1d12:0x2ad3, size: 5
				if (F0_1d12_6cf3_GetWonderCityID((int)WonderEnum.CopernicusObservatory) == cityID)
				{
					unhappyCitizenRatio += unhappyCitizenRatio;
				}
			
				this.parent.Var_70e6 = unhappyCitizenRatio;
				this.parent.Var_70e2_CityHappyCitizenCount = 0;

				if (this.Var_6548_PlayerID == this.parent.GameData.HumanPlayerID)
				{
					unhappyCitizenRatio = 14 - (parent.GameData.DifficultyLevel * 2);
					unhappyCitizenRatio = (((int)this.parent.GameData.Players[this.Var_6548_PlayerID].GovernmentType / 2) + 2) * (unhappyCitizenRatio / 2);
					this.parent.Var_70e4_CityUnhappyCitizenCount = (((cityID % unhappyCitizenRatio) + this.parent.GameData.Players[this.Var_6548_PlayerID].CityCount - unhappyCitizenRatio) / unhappyCitizenRatio) +
						city.ActualSize + this.parent.GameData.DifficultyLevel - 6;
				}
				else
				{
					this.parent.Var_70e4_CityUnhappyCitizenCount = city.ActualSize - 3;
				}

				this.Var_6542 = 0;

				if (city.ActualSize < this.parent.Var_70e4_CityUnhappyCitizenCount)
				{
					this.Var_6542 = this.parent.Var_70e4_CityUnhappyCitizenCount - city.ActualSize;
					this.parent.Var_70e4_CityUnhappyCitizenCount = city.ActualSize;
				}
			
				// Instruction address 0x1d12:0x2bd9, size: 5
				F0_1d12_6dfe_AdjustHappyUnhappyCitizens(cityID, workingCitizenCount);

				int resourcesY = 0;

				if (flag == 1 && this.Var_2496 == 1)
				{
					resourcesY = 116;

					// Instruction address 0x1d12:0x2c15, size: 5
					F0_1d12_6ed4_DrawResources(cityID, 100, resourcesY, workingCitizenCount, 92);

					resourcesY += 16;

					// Instruction address 0x1d12:0x2c35, size: 5
					this.parent.Graphics.F0_VGA_0599_DrawLine(this.parent.Var_aa_Screen0_Rectangle, 100, resourcesY - 2, 222, resourcesY - 2, 1);
				}

				this.parent.Var_70e2_CityHappyCitizenCount = this.parent.Var_70da_Arr[3] / 2;

				// Instruction address 0x1d12:0x2c4e, size: 5
				F0_1d12_6dfe_AdjustHappyUnhappyCitizens(cityID, workingCitizenCount);

				if (flag == 1 && this.Var_2496 == 1 && this.parent.Var_70e2_CityHappyCitizenCount != 0)
				{
					// Instruction address 0x1d12:0x2c8f, size: 5
					F0_1d12_6ed4_DrawResources(cityID, 100, resourcesY, workingCitizenCount, 92);

					// Instruction address 0x1d12:0x2ca6, size: 5
					this.parent.Graphics.F0_VGA_0c3e_DrawBitmapToScreen(this.parent.Var_aa_Screen0_Rectangle,
						208, resourcesY + 4, this.parent.Array_d4ce[14]);

					resourcesY += 16;

					// Instruction address 0x1d12:0x2cc6, size: 5
					this.parent.Graphics.F0_VGA_0599_DrawLine(this.parent.Var_aa_Screen0_Rectangle, 100, resourcesY - 2, 222, resourcesY - 2, 1);
				}

				int xPosition = 208;

				if (city.HasImprovement(ImprovementEnum.Colosseum))
				{
					this.parent.Var_70e4_CityUnhappyCitizenCount -= 3;

					if (flag == 1 && this.Var_2496 == 1)
					{
						// Instruction address 0x1d12:0x2d13, size: 5
						F0_1d12_7045_DrawWonderOrImprovement(14, (short)xPosition, (short)resourcesY);

						xPosition -= 16;
					}
				}

				// Instruction address 0x1d12:0x2d27, size: 5
				if (this.parent.Segment_1ade.F0_1ade_22b5_PlayerHasTechnology(this.Var_6548_PlayerID, TechnologyAdvanceEnum.Religion))
				{
					if (city.HasImprovement(ImprovementEnum.Cathedral))
					{
						// Instruction address 0x1d12:0x2d52, size: 5
						if (F0_1d12_6c97_PlayerHasWonder(this.Var_6548_PlayerID, WonderEnum.MichelangelosChapel))
						{
							this.parent.Var_70e4_CityUnhappyCitizenCount -= 6;
						}
						else
						{
							this.parent.Var_70e4_CityUnhappyCitizenCount -= 4;
						}
					}

					if (flag == 1 && this.Var_2496 == 1 && city.HasImprovement(ImprovementEnum.Cathedral))
					{
						// Instruction address 0x1d12:0x2db0, size: 5
						F0_1d12_7045_DrawWonderOrImprovement(11, (short)xPosition, (short)resourcesY);

						xPosition -= 16;
					}
				}

				if (city.HasImprovement(ImprovementEnum.Temple))
				{
					// Instruction address 0x1d12:0x2dd6, size: 5
					if (this.parent.Segment_1ade.F0_1ade_22b5_PlayerHasTechnology(this.Var_6548_PlayerID, TechnologyAdvanceEnum.Mysticism))
					{
						this.parent.Var_70e4_CityUnhappyCitizenCount -= 2;
					}
					else
					{
						// Instruction address 0x1d12:0x2df6, size: 5
						if (this.parent.Segment_1ade.F0_1ade_22b5_PlayerHasTechnology(this.Var_6548_PlayerID, TechnologyAdvanceEnum.CeremonialBurial))
						{
							this.parent.Var_70e4_CityUnhappyCitizenCount--;
						}
					}
				
					// Instruction address 0x1d12:0x2e12, size: 5
					if (F0_1d12_6c97_PlayerHasWonder(this.Var_6548_PlayerID, WonderEnum.Oracle))
					{
						// Instruction address 0x1d12:0x2e2a, size: 5
						if (this.parent.Segment_1ade.F0_1ade_22b5_PlayerHasTechnology(this.Var_6548_PlayerID, TechnologyAdvanceEnum.Mysticism))
						{
							this.parent.Var_70e4_CityUnhappyCitizenCount -= 2;
						}
						else
						{
							this.parent.Var_70e4_CityUnhappyCitizenCount -= 1;
						}
					}

					if (flag == 1 && this.Var_2496 == 1)
					{
						// Instruction address 0x1d12:0x2e6f, size: 5
						F0_1d12_7045_DrawWonderOrImprovement(4, (short)xPosition, (short)resourcesY);

						xPosition -= 16;
					}
				}

				// Instruction address 0x1d12:0x2e81, size: 5
				F0_1d12_6dfe_AdjustHappyUnhappyCitizens(cityID, workingCitizenCount);

				if (flag == 1 && this.Var_2496 == 1 && 
					(city.HasImprovement(ImprovementEnum.Colosseum) || city.HasImprovement(ImprovementEnum.Cathedral) || city.HasImprovement(ImprovementEnum.Temple)))
				{
					// Instruction address 0x1d12:0x2ecb, size: 5
					F0_1d12_6ed4_DrawResources(cityID, 100, resourcesY, workingCitizenCount, 92);

					resourcesY += 16;

					// Instruction address 0x1d12:0x2eeb, size: 5
					this.parent.Graphics.F0_VGA_0599_DrawLine(this.parent.Var_aa_Screen0_Rectangle, 100, resourcesY - 2, 222, resourcesY - 2, 1);
				}

				xPosition = 208;

				if (this.parent.GameData.Players[this.Var_6548_PlayerID].GovernmentType == GovernmentTypeEnum.Anarchy ||
					this.parent.GameData.Players[this.Var_6548_PlayerID].GovernmentType == GovernmentTypeEnum.Despotism ||
					this.parent.GameData.Players[this.Var_6548_PlayerID].GovernmentType == GovernmentTypeEnum.Monarchy ||
					this.parent.GameData.Players[this.Var_6548_PlayerID].GovernmentType == GovernmentTypeEnum.Communist)
				{
					int attackUnitCount = 0;
					int activeUnitID = this.parent.MapManagement.F0_2aea_1458_GetCellActiveUnitID(cityX, cityY);
					int nextActiveUnitID = activeUnitID;

					for (int i = 0; i < 32; i++)
					{
						if (nextActiveUnitID == -1)
							break;

						if (this.parent.GameData.Units[(int)this.parent.GameData.Players[this.Var_6548_PlayerID].Units[nextActiveUnitID].UnitType].AttackStrength != 0)
						{
							attackUnitCount++;

							if (flag == 1 && this.Var_2496 == 1 && attackUnitCount <= 3)
							{
								// Instruction address 0x1d12:0x2fbd, size: 5
								this.parent.Graphics.F0_VGA_0c3e_DrawBitmapToScreen(this.parent.Var_aa_Screen0_Rectangle,
									xPosition, resourcesY - 1,
									this.parent.Array_d4ce[64 + (int)this.parent.GameData.Players[this.Var_6548_PlayerID].Units[nextActiveUnitID].UnitType + (this.Var_6548_PlayerID * 32)]);

								xPosition -= 2;
							}
						}

						nextActiveUnitID = this.parent.GameData.Players[this.Var_6548_PlayerID].Units[nextActiveUnitID].NextUnitID;

						if (nextActiveUnitID == activeUnitID)
						{
							nextActiveUnitID = -1;
						}
					}

					for (int i = 0; i < 2; i++)
					{
						if (this.parent.Var_70e4_CityUnhappyCitizenCount != 0 && city.FirstFortifiedCityUnits[i] != -1 && this.parent.GameData.Units[city.FirstFortifiedCityUnits[i] & 0x3f].AttackStrength != 0)
						{
							attackUnitCount++;
						}
					}

					if (attackUnitCount > 3)
					{
						attackUnitCount = 3;
					}

					// Instruction address 0x1d12:0x3079, size: 5
					this.parent.Var_70e4_CityUnhappyCitizenCount -= this.parent.Tools.CheckValueRange(attackUnitCount, 0, this.parent.Var_70e4_CityUnhappyCitizenCount);
				}
				else
				{
					// Instruction address 0x1d12:0x3090, size: 5
					int wonderCount = F0_1d12_6c97_PlayerHasWonder(this.Var_6548_PlayerID, WonderEnum.WomensSuffrage) ? 1 : 0;

					if (this.parent.GameData.Players[this.Var_6548_PlayerID].GovernmentType == GovernmentTypeEnum.Democracy)
					{
						wonderCount++;
					}

					if (wonderCount != 0)
					{
						this.parent.Var_70e4_CityUnhappyCitizenCount += wonderCount * this.Var_6546;

						if (flag == 1 && this.Var_2496 == 1)
						{
							for (int i = 0; i < (wonderCount * this.Var_6546); i++)
							{
								// Instruction address 0x1d12:0x3117, size: 5
								this.parent.Graphics.F0_VGA_0c3e_DrawBitmapToScreen(this.parent.Var_aa_Screen0_Rectangle, xPosition, resourcesY + 4,
									this.parent.Array_d4ce[13]);

								xPosition -= 2;
							}
						}
					}
				}

				// Instruction address 0x1d12:0x312c, size: 5
				F0_1d12_6dfe_AdjustHappyUnhappyCitizens(cityID, workingCitizenCount);

				if (flag == 1 && this.Var_2496 == 1)
				{
					// Instruction address 0x1d12:0x3163, size: 5
					F0_1d12_6ed4_DrawResources(cityID, 100, resourcesY, workingCitizenCount, 92);

					resourcesY += 16;

					// Instruction address 0x1d12:0x3183, size: 5
					this.parent.Graphics.F0_VGA_0599_DrawLine(this.parent.Var_aa_Screen0_Rectangle, 100, resourcesY - 2, 222, resourcesY - 2, 1);
				}

				int cityPopulationHappinessDifference = this.parent.Var_70e2_CityHappyCitizenCount - this.parent.Var_70e4_CityUnhappyCitizenCount;

				// Instruction address 0x1d12:0x319e, size: 5
				if (F0_1d12_6c97_PlayerHasWonder(this.Var_6548_PlayerID, WonderEnum.HangingGardens))
				{
					this.parent.Var_70e2_CityHappyCitizenCount++;
				}
			
				// Instruction address 0x1d12:0x31ba, size: 5
				if (F0_1d12_6c97_PlayerHasWonder(this.Var_6548_PlayerID, WonderEnum.CureForCancer))
				{
					this.parent.Var_70e2_CityHappyCitizenCount++;
				}
			
				// Instruction address 0x1d12:0x31d2, size: 5
				if (F0_1d12_6cf3_GetWonderCityID((int)WonderEnum.ShakespearesTheatre) == cityID)
				{
					this.parent.Var_70e4_CityUnhappyCitizenCount = 0;
				}
			
				// Instruction address 0x1d12:0x31f0, size: 5
				if (F0_1d12_6c97_PlayerHasWonder(this.Var_6548_PlayerID, WonderEnum.JSBachsCathedral))
				{
					// Instruction address 0x1d12:0x3229, size: 5
					// Instruction address 0x1d12:0x3217, size: 5
					if (this.parent.MapManagement.F0_2aea_1942_GetGroupID(cityX, cityY) == 
						this.parent.MapManagement.F0_2aea_1942_GetGroupID(
							this.parent.GameData.Cities[this.parent.GameData.WonderCityID[(int)WonderEnum.JSBachsCathedral]].Position.X,
							this.parent.GameData.Cities[this.parent.GameData.WonderCityID[(int)WonderEnum.JSBachsCathedral]].Position.Y))
					{
						this.parent.Var_70e4_CityUnhappyCitizenCount -= 2;
					}
				}

				// Instruction address 0x1d12:0x3243, size: 5
				F0_1d12_6dfe_AdjustHappyUnhappyCitizens(cityID, workingCitizenCount);

				if (flag == 1 && this.Var_2496 == 1 && (this.parent.Var_70e2_CityHappyCitizenCount - this.parent.Var_70e4_CityUnhappyCitizenCount) != cityPopulationHappinessDifference)
				{
					// Instruction address 0x1d12:0x328a, size: 5
					F0_1d12_6ed4_DrawResources(cityID, 100, resourcesY, workingCitizenCount, 92);

					// Instruction address 0x1d12:0x32a5, size: 5
					this.parent.DrawTools.DrawStringToScreen0("WONDERS", 190, resourcesY + 5, 15);

					resourcesY += 16;

					// Instruction address 0x1d12:0x32c5, size: 5
					this.parent.Graphics.F0_VGA_0599_DrawLine(this.parent.Var_aa_Screen0_Rectangle, 100, resourcesY - 2, 222, resourcesY - 2, 1);
				}

				if ((this.parent.GameData.DebugFlags & 0x2) == 0)
				{
					this.parent.Var_70e4_CityUnhappyCitizenCount = 0;
					this.parent.Var_70e2_CityHappyCitizenCount = 0;
				}

			L32e0:
				int cityWindowUnitCount = 0;
				int[] cityWindowUnitIDs = new int[18];
				this.parent.Var_deb8 = 0;
				this.parent.Var_d2f6 = 0;

				for (int i = 0; i < 2; i++)
				{
					if (city.FirstFortifiedCityUnits[i] != -1)
					{
						this.parent.Var_deb8++;
					}				
				}

				if (city.ActualSize < this.parent.Var_deb8)
				{
					this.parent.Var_d2f6 = this.parent.Var_deb8 - city.ActualSize;
				}
			
				int cityWindowX = 8;
				int cityWindowY = 69;
				int cityUnitX = 100;
				int cityUnitY = 116;

				for (int i = 0; i < 128; i++)
				{
					if (this.parent.GameData.Players[this.Var_6548_PlayerID].Units[i].UnitType != UnitTypeEnum.None &&
						this.parent.GameData.Players[this.Var_6548_PlayerID].Units[i].HomeCityID == cityID)
					{
						if (this.parent.GameData.Players[this.Var_6548_PlayerID].Units[i].UnitType != UnitTypeEnum.Diplomat &&
							this.parent.GameData.Players[this.Var_6548_PlayerID].Units[i].UnitType != UnitTypeEnum.Caravan)
						{
							this.parent.Var_deb8++;

							if (city.ActualSize >= this.parent.Var_deb8)
							{
								if ((this.parent.GameData.Players[this.Var_6548_PlayerID].GovernmentType == GovernmentTypeEnum.Monarchy ||
									this.parent.GameData.Players[this.Var_6548_PlayerID].GovernmentType == GovernmentTypeEnum.Communist ||
									this.parent.GameData.Players[this.Var_6548_PlayerID].GovernmentType == GovernmentTypeEnum.Republic ||
									this.parent.GameData.Players[this.Var_6548_PlayerID].GovernmentType == GovernmentTypeEnum.Democracy) && (this.parent.GameData.DebugFlags & 0x2) != 0)
								{
									this.parent.Var_d2f6++;
								}
							}
							else
							{
								this.parent.Var_d2f6++;
							}
						}

						if (flag == 0)
						{
							if (this.parent.Var_70da_Arr[1] < this.parent.Var_d2f6 ||
								((city.Status & CityStatusEnum.CivilDisorder) != CityStatusEnum.None && ((i + this.parent.GameData.TurnCount) & 0x7) == 0 &&
								this.Var_6548_PlayerID != this.parent.GameData.HumanPlayerID &&
								(this.parent.GameData.Players[this.Var_6548_PlayerID].GovernmentType == GovernmentTypeEnum.Republic ||
									this.parent.GameData.Players[this.Var_6548_PlayerID].GovernmentType == GovernmentTypeEnum.Democracy)))
							{
								int selectedUnit = -1;
								int maximumDistance = -1;

								for (int j = 0; j < 128; j++)
								{
									if (this.parent.GameData.Players[this.Var_6548_PlayerID].Units[j].UnitType != UnitTypeEnum.None &&
										this.parent.GameData.Players[this.Var_6548_PlayerID].Units[j].HomeCityID == cityID &&
										(this.parent.GameData.Players[this.Var_6548_PlayerID].Units[j].UnitType != UnitTypeEnum.Diplomat &&
											this.parent.GameData.Players[this.Var_6548_PlayerID].Units[j].UnitType != UnitTypeEnum.Caravan))
									{
										// Instruction address 0x1d12:0x3513, size: 5
										int distance = this.parent.MapManagement.GetDistance(city.Position, this.parent.GameData.Players[this.Var_6548_PlayerID].Units[j].Position);

										if (distance > maximumDistance)
										{
											maximumDistance = distance;
											selectedUnit = j;
										}
									}
								}

								if (this.parent.Var_70da_Arr[1] >= this.parent.Var_d2f6)
								{
									if (this.parent.Var_8078 != 0 &&
										this.parent.GameData.Players[this.Var_6548_PlayerID].DiscoveredTechnologyCount <
										this.parent.GameData.Players[this.parent.GameData.HumanPlayerID].DiscoveredTechnologyCount)
									{
										this.parent.Var_db42 = -999; // 0xfc19
									}
									else if (maximumDistance > 0 &&
										this.parent.GameData.Units[(int)this.parent.GameData.Players[this.Var_6548_PlayerID].Units[selectedUnit].UnitType].MovementType == UnitMovementTypeEnum.Land &&
										this.parent.GameData.Players[this.Var_6548_PlayerID].Units[selectedUnit].UnitType != UnitTypeEnum.Settler)
									{
										// Instruction address 0x1d12:0x35c9, size: 5
										this.parent.UnitManagement.F0_1866_0f10_DeleteUnit(this.Var_6548_PlayerID, (short)selectedUnit);

										city.ClearStatusFlags(CityStatusEnum.CivilDisorder);
									}
								}
								else
								{
									if (this.Var_6548_PlayerID == this.parent.GameData.HumanPlayerID)
									{
										this.parent.Var_2f9e_MessageBoxStyle = MenuBoxReportTypeEnum.DefenseMinisterReport;

										// Instruction address 0x1d12:0x3657, size: 5
										this.parent.Segment_1238.F0_1238_001e_ShowDialog($"{this.parent.Segment_2459.F0_2459_08c6_GetCityName(cityID)}" +
											$" can't support\n{this.parent.GameData.Units[(int)this.parent.GameData.Players[this.Var_6548_PlayerID].Units[selectedUnit].UnitType].Name}" +
											".\n Unit Disbanded.\n", 100, 80);
									}

									// Instruction address 0x1d12:0x3667, size: 5
									this.parent.UnitManagement.F0_1866_0f10_DeleteUnit(this.Var_6548_PlayerID, (short)selectedUnit);

									goto L32e0;
								}
							}
						}

						if (flag == 1)
						{
							if (this.parent.GameData.Players[this.Var_6548_PlayerID].Units[i].UnitType != UnitTypeEnum.Diplomat &&
								this.parent.GameData.Players[this.Var_6548_PlayerID].Units[i].UnitType != UnitTypeEnum.Caravan)
							{
								if (this.parent.Var_d2f6 != 0)
								{
									// Instruction address 0x1d12:0x36c0, size: 5
									this.parent.Graphics.F0_VGA_0c3e_DrawBitmapToScreen(this.parent.Var_aa_Screen0_Rectangle,
										cityWindowX + 8, cityWindowY + 12, this.parent.Array_d4ce[9]);
								}
								if (this.parent.GameData.Players[this.Var_6548_PlayerID].Units[i].UnitType == UnitTypeEnum.Settler)
								{
									// Instruction address 0x1d12:0x36f4, size: 5
									this.parent.Graphics.F0_VGA_0c3e_DrawBitmapToScreen(this.parent.Var_aa_Screen0_Rectangle,
										cityWindowX, cityWindowY + 12, this.parent.Array_d4ce[8]);

									if (this.parent.GameData.Players[this.Var_6548_PlayerID].GovernmentType == GovernmentTypeEnum.Monarchy ||
										this.parent.GameData.Players[this.Var_6548_PlayerID].GovernmentType == GovernmentTypeEnum.Communist ||
										this.parent.GameData.Players[this.Var_6548_PlayerID].GovernmentType == GovernmentTypeEnum.Republic ||
										this.parent.GameData.Players[this.Var_6548_PlayerID].GovernmentType == GovernmentTypeEnum.Democracy)
									{
										// Instruction address 0x1d12:0x371f, size: 5
										this.parent.Graphics.F0_VGA_0c3e_DrawBitmapToScreen(this.parent.Var_aa_Screen0_Rectangle,
											cityWindowX + 2, cityWindowY + 12, this.parent.Array_d4ce[8]);
									}
								}
								else
								{
									// Instruction address 0x1d12:0x3732, size: 5
									int womensSuffrageEffectScore = F0_1d12_6c97_PlayerHasWonder(this.Var_6548_PlayerID, WonderEnum.WomensSuffrage) ? 1 : 0;

									if (this.parent.GameData.Players[this.Var_6548_PlayerID].GovernmentType == GovernmentTypeEnum.Democracy)
									{
										womensSuffrageEffectScore++;
									}

									if (this.parent.GameData.Players[this.Var_6548_PlayerID].GovernmentType == GovernmentTypeEnum.Republic ||
										this.parent.GameData.Players[this.Var_6548_PlayerID].GovernmentType == GovernmentTypeEnum.Democracy)
									{
										if (this.parent.GameData.Units[(int)this.parent.GameData.Players[this.Var_6548_PlayerID].Units[i].UnitType].AttackStrength != 0 &&
											womensSuffrageEffectScore != 0 &&
											(this.parent.GameData.Units[(int)this.parent.GameData.Players[this.Var_6548_PlayerID].Units[i].UnitType].MovementType == UnitMovementTypeEnum.Air ||
												this.parent.GameData.Players[this.Var_6548_PlayerID].Units[i].Position.X != city.Position.X ||
												this.parent.GameData.Players[this.Var_6548_PlayerID].Units[i].Position.Y != city.Position.Y))
										{
											// Instruction address 0x1d12:0x381d, size: 5
											this.parent.Graphics.F0_VGA_0c3e_DrawBitmapToScreen(this.parent.Var_aa_Screen0_Rectangle,
												cityWindowX, cityWindowY + 12, this.parent.Array_d4ce[13]);

											if (womensSuffrageEffectScore > 1)
											{
												// Instruction address 0x1d12:0x3842, size: 5
												this.parent.Graphics.F0_VGA_0c3e_DrawBitmapToScreen(this.parent.Var_aa_Screen0_Rectangle,
													cityWindowX + 2, cityWindowY + 12, this.parent.Array_d4ce[13]);
											}
										}
									}
								}
							}

							// Instruction address 0x1d12:0x3877, size: 5
							this.parent.Graphics.F0_VGA_0c3e_DrawBitmapToScreen(this.parent.Var_aa_Screen0_Rectangle, cityWindowX, cityWindowY,
								this.parent.Array_d4ce[64 + (int)this.parent.GameData.Players[this.Var_6548_PlayerID].Units[i].UnitType + (this.Var_6548_PlayerID * 32)]);

							// Instruction address 0x1d12:0x38a3, size: 5
							F0_1d12_73ea_DrawCityRectangle(
								this.parent.GameData.Players[this.Var_6548_PlayerID].Units[i].Position.X,
								this.parent.GameData.Players[this.Var_6548_PlayerID].Units[i].Position.Y,
								7);

							cityWindowX += 16;

							if (cityWindowX >= 112)
							{
								cityWindowX = 8;
								cityWindowY += 16;

								if (cityWindowY > 85)
								{
									cityWindowY -= 24;
								}
							}
						}
					}

					if (flag == 1 && cityWindowUnitCount < 18 && this.Var_2496 == 0 &&
						this.parent.GameData.Players[this.Var_6548_PlayerID].Units[i].UnitType != UnitTypeEnum.None &&
						this.parent.GameData.Players[this.Var_6548_PlayerID].Units[i].Position.X == cityX &&
						this.parent.GameData.Players[this.Var_6548_PlayerID].Units[i].Position.Y == cityY)
					{
						// Instruction address 0x1d12:0x3969, size: 5
						this.parent.MapManagement.F0_2aea_0fb3_DrawUnitWithStatus(this.Var_6548_PlayerID, (short)i, cityUnitX, cityUnitY);

						string cityName = this.parent.Segment_2459.F0_2459_08c6_GetCityName(this.parent.GameData.Players[this.Var_6548_PlayerID].Units[i].HomeCityID);

						cityName = (cityName.Length > 3) ? (cityName.Substring(0, 3) + "."): cityName;

						// Instruction address 0x1d12:0x39b4, size: 5
						this.parent.DrawTools.DrawStringToScreen0(cityName, cityUnitX, cityUnitY + 15, 0);

						cityWindowUnitIDs[cityWindowUnitCount] = i;

						cityWindowUnitCount++;

						cityUnitX += 18;
						
						if ((cityWindowUnitCount % 6) == 0)
						{
							cityUnitX = 100;
							cityUnitY += 16;
						}
					}
				}

				if (flag == 0)
				{
					if (this.parent.Var_70da_Arr[1] < this.parent.Var_deb8)
					{
						this.parent.Var_e3c2 += (this.parent.Var_deb8 - this.parent.Var_70da_Arr[1]) * 5;
					}

					// Instruction address 0x1d12:0x3a2c, size: 5
					this.parent.Var_e3c2 += this.parent.Tools.CheckValueRange(this.parent.Var_deb8, 0, city.ActualSize);

					if (city.HasImprovement(ImprovementEnum.MarketPlace))
					{
						this.parent.Var_db42 -= (5 - this.parent.GameData.Nations[this.parent.GameData.Players[this.Var_6548_PlayerID].NationalityID].Ideology) * this.Var_6546;
					}
					else
					{
						this.parent.Var_db42 -= (7 - this.parent.GameData.Nations[this.parent.GameData.Players[this.Var_6548_PlayerID].NationalityID].Ideology) * this.Var_6546;
					}
				}

				if (flag == 1)
				{
					this.parent.Var_aa_Screen0_Rectangle.FontID = 2;

					// Instruction address 0x1d12:0x3a9f, size: 5
					this.parent.DrawTools.DrawRectangleAndFillWithPattern(211, 1, 317, 97, 208, 100, 16, 16);

					int improvementX = 2;
					cityY = 2;
					int wonderCount = 0;

					if (previousImprovementIndex == 0)
					{
						for (int i = 0; i < 21; i++)
						{
							if (this.parent.GameData.WonderCityID[i + 1] == cityID)
							{
								// Instruction address 0x1d12:0x3b40, size: 5
								this.parent.DrawTools.DrawStringToScreen0(
									this.parent.LanguageTools.F0_2f4d_04f7_TrimStringToWidth(this.parent.GameData.Wonders[i + 1].Name.ToUpper(), 63), 253, cityY + 2, 15);

								// Instruction address 0x1d12:0x3b69, size: 5
								F0_1d12_7045_DrawWonderOrImprovement(i + 25, ((improvementX & 1) != 0) ? 213 : 233, cityY - 2);

								cityY += 6;
								improvementX++;
								wonderCount++;
							}
						}
					}

					int shownImprovementCount = 0;
					currentImprovementIndex = 0;

					for (int i = previousImprovementIndex; i < 24; i++)
					{
						if (city.HasImprovement(City.BitToImprovementEnum(i)))
						{
							if (improvementX >= 16)
							{
								hasNextImprovementPage = true;
								currentImprovementIndex = i;

								break;
							}
							else
							{
								shownImprovementCount++;

								// Instruction address 0x1d12:0x3bfb, size: 5
								this.parent.Graphics.F0_VGA_0c3e_DrawBitmapToScreen(this.parent.Var_aa_Screen0_Rectangle,
									309, cityY + 1, this.parent.Array_d4ce[11]);

								// Instruction address 0x1d12:0x3c46, size: 5
								this.parent.DrawTools.DrawStringToScreen0(
									this.parent.LanguageTools.F0_2f4d_04f7_TrimStringToWidth(this.parent.GameData.GetImprovementType(i + 1).Name, 56), 253, cityY + 2, 15);

								// Instruction address 0x1d12:0x3c6f, size: 5
								F0_1d12_7045_DrawWonderOrImprovement((short)(i + 1), (short)(((improvementX & 1) != 0) ? 213 : 233), (short)(cityY - 2));

								cityY += 6;
								improvementX++;
							}
						}
					}

					// Instruction address 0x1d12:0x3c97, size: 5
					this.parent.Graphics.F0_VGA_0599_DrawLine(this.parent.Var_aa_Screen0_Rectangle, 231, 0, 250, 0, 0);

					// Instruction address 0x1d12:0x3cb3, size: 5
					this.parent.Graphics.F0_VGA_0599_DrawLine(this.parent.Var_aa_Screen0_Rectangle, 231, 1, 250, 1, 1);

					if (hasNextImprovementPage)
					{
						// Instruction address 0x1d12:0x3cdd, size: 5
						this.parent.DrawTools.DrawFilledRectangleWithCenteredText(287, 88, 315, 96, "MORE", 9);
					}

					// Instruction address 0x1d12:0x3d01, size: 5
					this.parent.Graphics.F0_VGA_009a_ReplaceColor(this.parent.Var_aa_Screen0_Rectangle, 309, 2, 8, 96, 14, 12);

					int productionCost;

					if (city.CurrentProductionID >= 0)
					{
						productionCost = this.parent.GameData.Units[city.CurrentProductionID].Cost;
					}
					else
					{
						productionCost = this.parent.GameData.GetImprovementType(-city.CurrentProductionID).Cost;
					}

					// Instruction address 0x1d12:0x3d7a, size: 5
					int productionSpacing = this.parent.Tools.CheckValueRange(((productionCost - 1) / 10) + 1, ((city.ShieldsCount - 1) / 100) + 1, 99);
					int foodStorage = 80 / difficultyProductionRatio;
					cityWindowX = (((productionSpacing * 8) - 8) / productionSpacing) + (difficultyProductionRatio * foodStorage);
					cityWindowY = (((productionCost - 1) / productionSpacing) * 8) + 8;

					// Instruction address 0x1d12:0x3ded, size: 5
					this.parent.DrawTools.FillRectangle(this.parent.Var_aa_Screen0_Rectangle, 230, 99, cityWindowX + 3, cityWindowY + 19, 1);

					if (this.Var_6548_PlayerID == this.parent.GameData.HumanPlayerID)
					{
						// Instruction address 0x1d12:0x3e31, size: 5
						this.parent.DrawTools.DrawFilledRectangleWithCenteredText(231, 106, 263, 114, (string)(((city.Status & CityStatusEnum.AutoProduction) != CityStatusEnum.None) ? "AUTO" : "CHANGE"), 9);

						// Instruction address 0x1d12:0x3e51, size: 5
						this.parent.DrawTools.DrawFilledRectangleWithCenteredText(294, 106, 311, 114, "BUY", 9);
					}

					if (city.CurrentProductionID >= 0)
					{
						// Instruction address 0x1d12:0x3e8e, size: 5
						this.parent.Graphics.F0_VGA_0c3e_DrawBitmapToScreen(this.parent.Var_aa_Screen0_Rectangle, 264, 100,
							this.parent.Array_d4ce[64 + city.CurrentProductionID + (this.Var_6548_PlayerID * 32)]);
					}
					else
					{
						// Instruction address 0x1d12:0x3edd, size: 5
						this.parent.DrawTools.DrawCenteredStringToScreen0(
							this.parent.LanguageTools.F0_2f4d_04f7_TrimStringToWidth(this.parent.GameData.GetImprovementType(-city.CurrentProductionID).Name, 86), 274, 100, 15);
					}

					// Instruction address 0x1d12:0x3ef9, size: 5
					this.parent.DrawTools.FillRectangleWithPattern(231, 116, cityWindowX + 1, cityWindowY + 1, 208, 100, 16, 16);

					for (int i = 0; i < city.ShieldsCount; i++)
					{
						// Instruction address 0x1d12:0x3f55, size: 5
						this.parent.Graphics.F0_VGA_0c3e_DrawBitmapToScreen(this.parent.Var_aa_Screen0_Rectangle,
							(((i % (difficultyProductionRatio * productionSpacing)) * foodStorage) / productionSpacing) + 232,
							((i / (difficultyProductionRatio * productionSpacing)) * 8) + 117,
							this.parent.Array_d4ce[9]);
					}

					// Instruction address 0x1d12:0x3f7b, size: 5
					if (city.ActualSize != 0)
					{
						foodStorage = this.parent.Tools.CheckValueRange(80 / city.ActualSize, 1, 8);
					}
					else
					{
						foodStorage = 1;
					}

					cityWindowY = 80 / difficultyProductionRatio;

					// Instruction address 0x1d12:0x3fb5, size: 5
					this.parent.DrawTools.FillRectangle(this.parent.Var_aa_Screen0_Rectangle, 2, 106, 91, (difficultyProductionRatio * cityWindowY) + 12, 1);

					// Instruction address 0x1d12:0x3fcd, size: 5
					this.parent.DrawTools.DrawStringToScreen0("Food Storage", 8, 108, 15);

					// Instruction address 0x1d12:0x3ffc, size: 5
					this.parent.DrawTools.FillRectangleWithPattern(3, 115, (city.ActualSize * foodStorage) + 9, (difficultyProductionRatio * cityWindowY) + 2, 208, 100, 16, 16);

					if (city.HasImprovement(ImprovementEnum.Granary))
					{
						// Instruction address 0x1d12:0x403b, size: 5
						this.parent.Graphics.F0_VGA_0599_DrawLine(this.parent.Var_aa_Screen0_Rectangle, 5, 155, (city.ActualSize * foodStorage) + 9, 155, 1);
					}

					// Instruction address 0x1d12:0x406a, size: 5
					int iTemp = this.parent.Tools.CheckValueRange(city.FoodCount, 0, (city.ActualSize + 1) * difficultyProductionRatio);

					for (int i = 0; i < iTemp; i++)
					{
						// Instruction address 0x1d12:0x40af, size: 5
						this.parent.Graphics.F0_VGA_0c3e_DrawBitmapToScreen(this.parent.Var_aa_Screen0_Rectangle,
							((i % (city.ActualSize + 1)) * foodStorage) + 4,
							((i / (city.ActualSize + 1)) * cityWindowY) + 116,
							this.parent.Array_d4ce[8]);
					}

					// Instruction address 0x1d12:0x40ca, size: 5
					this.parent.DrawTools.DrawRectangleAndFillWithPattern(2, 23, 124, 65, 208, 100, 16, 16);

					cityWindowY = 25;

					// Instruction address 0x1d12:0x40f0, size: 5
					this.parent.DrawTools.FillRectangle(this.parent.Var_aa_Screen0_Rectangle, 2, 23, 122, 9, 1);

					// Instruction address 0x1d12:0x4108, size: 5
					this.parent.DrawTools.DrawStringToScreen0("City Resources", 8, cityWindowY, 15);

					cityWindowY += 8;
					int xStep;

					for (int i = 0; i < 3; i++)
					{
						cityX = 4;

						if (i == 0)
						{
							// Instruction address 0x1d12:0x416d, size: 5
							// Instruction address 0x1d12:0x4181, size: 5
							xStep = this.parent.Tools.CheckValueRange(
								116 / (this.parent.Tools.CheckValueRange(this.parent.Var_70da_Arr[i], 
									(city.ActualSize * 2) + (productionRatio * this.parent.Var_e3c6), 999) + 1), 1, 8);
						}
						else
						{
							// Instruction address 0x1d12:0x41ac, size: 5
							xStep = this.parent.Tools.CheckValueRange(116 / (this.parent.Var_70da_Arr[i] + 1), 1, 8);
						}

						for (int j = 0; j < this.parent.Var_70da_Arr[i]; j++)
						{
							switch (i)
							{
								case 0:
									if ((city.ActualSize * 2) + (productionRatio * this.parent.Var_e3c6) == j)
									{
										cityX += 4;
									}
									break;

								case 1:
									if (this.parent.Var_d2f6 != 0 && j == this.parent.Var_d2f6)
									{
										cityX += 4;
									}
									break;

								case 2:
									if ((this.parent.Var_70da_Arr[2] - this.parent.Var_d2e0) == j)
									{
										cityX += 2;
									}
									break;
							}

							// Instruction address 0x1d12:0x4267, size: 5
							this.parent.Graphics.F0_VGA_0c3e_DrawBitmapToScreen(this.parent.Var_aa_Screen0_Rectangle,
								cityX, (i * 8) + cityWindowY, this.parent.Array_d4ce[8 + i]);

							cityX += xStep;
						}
					}

					cityX = 8;

					// Instruction address 0x1d12:0x42a6, size: 5
					xStep = this.parent.Tools.CheckValueRange(
						224 / (this.parent.Var_70da_Arr[3] + this.parent.Var_70e6 + this.parent.Var_e17a + this.parent.Var_d2e0 + 2), 1, 16);

					for (int i = 0; i < this.parent.Var_70da_Arr[3]; i++)
					{
						// Instruction address 0x1d12:0x42e1, size: 5
						this.parent.Graphics.F0_VGA_0c3e_DrawBitmapToScreen(this.parent.Var_aa_Screen0_Rectangle,
							cityX / 2, cityWindowY + 24, this.parent.Array_d4ce[14]);

						cityX += xStep;
					}

					if (this.parent.Var_70da_Arr[3] != 0)
					{
						cityX += 8;
					}

					cityWindowX = cityX;

					for (int i = 0; i < this.parent.Var_e17a; i++)
					{
						// Instruction address 0x1d12:0x433a, size: 5
						this.parent.Graphics.F0_VGA_0c3e_DrawBitmapToScreen(this.parent.Var_aa_Screen0_Rectangle,
							cityX / 2, cityWindowY + 24, this.parent.Array_d4ce[11]);

						cityX += xStep;
					}

					if (this.parent.Var_e17a != 0)
					{
						cityX += 8;
					}

					for (int i = 0; i < this.parent.Var_70e6; i++)
					{
						// Instruction address 0x1d12:0x438b, size: 5
						this.parent.Graphics.F0_VGA_0c3e_DrawBitmapToScreen(this.parent.Var_aa_Screen0_Rectangle,
							cityX / 2, cityWindowY + 24, this.parent.Array_d4ce[12]);

						cityX += xStep;
					}

					if ((city.ActualSize * 2) + (productionRatio * this.parent.Var_e3c6) > this.parent.Var_70da_Arr[0])
					{
						// Instruction address 0x1d12:0x43ed, size: 5
						xStep = this.parent.Tools.CheckValueRange(116 / ((city.ActualSize * 2) + (productionRatio * this.parent.Var_e3c6) + 1), 1, 8);

						for (int i = this.parent.Var_70da_Arr[0]; i < (city.ActualSize * 2) + (productionRatio * this.parent.Var_e3c6); i++)
						{
							// Instruction address 0x1d12:0x443e, size: 5
							this.parent.Graphics.F0_VGA_0c3e_DrawBitmapToScreen(this.parent.Var_aa_Screen0_Rectangle,
								(i * xStep) + 8, cityWindowY, this.parent.Array_d4ce[8]);
						}

						// Instruction address 0x1d12:0x448f, size: 5
						this.parent.Graphics.F0_VGA_009a_ReplaceColor(this.parent.Var_aa_Screen0_Rectangle,
							(xStep * this.parent.Var_70da_Arr[0]) + 8, cityWindowY,
							(((city.ActualSize * 2) + (productionRatio * this.parent.Var_e3c6) - this.parent.Var_70da_Arr[0]) * xStep) + 4,
							8, 15, 0);
					}

					if (this.parent.Var_70da_Arr[1] < this.parent.Var_d2f6)
					{
						// Instruction address 0x1d12:0x44b9, size: 5
						xStep = this.parent.Tools.CheckValueRange(116 / (this.parent.Var_70da_Arr[1] + 1), 1, 8);

						for (int i = this.parent.Var_70da_Arr[1]; i < this.parent.Var_d2f6; i++)
						{
							// Instruction address 0x1d12:0x44f7, size: 5
							this.parent.Graphics.F0_VGA_0c3e_DrawBitmapToScreen(this.parent.Var_aa_Screen0_Rectangle,
								(i * xStep) + 8, cityWindowY + 8, this.parent.Array_d4ce[9]);
						}

						// Instruction address 0x1d12:0x4532, size: 5
						this.parent.Graphics.F0_VGA_009a_ReplaceColor(this.parent.Var_aa_Screen0_Rectangle,
							(this.parent.Var_70da_Arr[1] * xStep) + 8, cityWindowY + 8,
							(this.parent.Var_d2f6 - this.parent.Var_70da_Arr[1]) * xStep,
							8, 15, 0);
					}

					if (this.parent.Var_d2e0 != 0)
					{
						// Instruction address 0x1d12:0x455a, size: 5
						xStep = this.parent.Tools.CheckValueRange(116 / (this.parent.Var_70da_Arr[2] + 1), 1, 8);

						// Instruction address 0x1d12:0x4598, size: 5
						this.parent.Graphics.F0_VGA_009a_ReplaceColor(this.parent.Var_aa_Screen0_Rectangle,
							((this.parent.Var_70da_Arr[2] - this.parent.Var_d2e0) * xStep) + 6,
							cityWindowY + 16, (xStep * this.parent.Var_d2e0) + 2,
							8, 15, 0);
					}

					// Instruction address 0x1d12:0x45b0, size: 5
					this.parent.DrawTools.FillRectangleWithPattern(8, 8, 200, 13, 208, 100, 16, 16);

					// Instruction address 0x1d12:0x45ca, size: 5
					int resourceXSpacing = F0_1d12_6ed4_DrawResources(cityID, 8, 8, workingCitizenCount, 192);

					for (int i = 0; i < 3; i++)
					{
						int tradeCityID = city.TradeCityIDs[i];

						if (tradeCityID != -1)
						{
							if (this.Var_2496 == 0)
							{
								if (this.parent.GameData.Cities[tradeCityID].PlayerID != this.Var_6548_PlayerID)
								{
									// Instruction address 0x1d12:0x4765, size: 5
									this.parent.DrawTools.DrawStringToScreen0($"{{{this.parent.Segment_2459.F0_2459_08c6_GetCityName(tradeCityID)}:+" +
										$"{(city.BaseTrade + this.parent.GameData.Cities[tradeCityID].BaseTrade + 4) / 8}}} ", 98, (i * 6) + 179, 10);
								}
								else
								{
									// Instruction address 0x1d12:0x4765, size: 5
									this.parent.DrawTools.DrawStringToScreen0($"{{{this.parent.Segment_2459.F0_2459_08c6_GetCityName(tradeCityID)}:+" +
										$"{((city.BaseTrade + this.parent.GameData.Cities[tradeCityID].BaseTrade + 4) / 16)}}} ", 98, (i * 6) + 179, 10);
								}
							}
							else if (this.Var_2496 == 2)
							{
								// Instruction address 0x1d12:0x473c, size: 5
								F0_1d12_73ea_DrawCityRectangle(this.parent.GameData.Cities[tradeCityID].Position.X,
									this.parent.GameData.Cities[tradeCityID].Position.Y, 10);
							}
						}
					}

					if (this.Var_2496 == 2)
					{
						// Instruction address 0x1d12:0x4795, size: 5
						F0_1d12_73ea_DrawCityRectangle(this.parent.GameData.Cities[this.Var_653e_CityID].Position.X,
							this.parent.GameData.Cities[this.Var_653e_CityID].Position.Y, 15);
					}

					if (this.Var_2496 == 0)
					{
						int local_e0 = (this.parent.Var_70da_Arr[1] / this.parent.Var_6c98) - 20 + ((city.ActualSize * this.parent.Var_b882) / 4);

						// Instruction address 0x1d12:0x47ef, size: 5
						// Instruction address 0x1d12:0x4802, size: 5
						cityWindowY = this.parent.Tools.CheckValueRange(100 / this.parent.Tools.CheckValueRange(local_e0, 1, 99), 1, 8);

						for (int i = 0; i < local_e0; i++)
						{
							// Instruction address 0x1d12:0x4847, size: 5
							this.parent.Graphics.F0_VGA_0c3e_DrawBitmapToScreen(this.parent.Var_aa_Screen0_Rectangle,
								(i * cityWindowY) + 98, (i & 1) + 161, this.parent.Var_b2ba);
						}
					}

					if (this.Var_6548_PlayerID == this.parent.GameData.HumanPlayerID)
					{
						outsideUnitIndex = 0;

						// Instruction address 0x1d12:0x4876, size: 5
						this.parent.DrawTools.DrawFilledRectangleWithCenteredText(284, 190, 316, 198, "EXIT", 12);

						// Instruction address 0x1d12:0x4896, size: 5
						this.parent.DrawTools.DrawFilledRectangleWithCenteredText(231, 190, 272, 198, "RENAME", 9);

						int buttonSelected = 0;

						if (city.CurrentProductionID < 0 && city.CurrentProductionID >= -21 && city.HasImprovement(City.BitToImprovementEnum((-city.CurrentProductionID) - 1)))
						{
							buttonSelected = 1;
						}

						if (city.CurrentProductionID < -21 && city.CurrentProductionID >= -24 && (this.parent.GameData.SpaceshipFlags & (1 << this.Var_6548_PlayerID)) != 0)
						{
							buttonSelected = 1;
						}

						if (city.CurrentProductionID < -24)
						{
							if (this.parent.GameData.WonderCityID[Math.Abs(city.CurrentProductionID) - 24] != -1)
							{
								buttonSelected = 1;
							}
						}

						if (city.CurrentProductionID >= 0 && this.parent.GameData.Players[this.Var_6548_PlayerID].UnitCount >= 127)
						{
							buttonSelected = 1;
						}

						MouseEvent mouseEvent;

						while ((mouseEvent = this.parent.GetMouseEvent()).Buttons != MouseButtonsEnum.None) { }

						while ((mouseEvent = this.parent.GetMouseEvent()).Buttons == MouseButtonsEnum.None)
						{
							// Instruction address 0x1d12:0x49b5, size: 5
							if (this.parent.CAPI.kbhit() != 0 || flag_b8)
								break;

							if (buttonSelected != 0)
							{
								buttonSelected++;

								if ((buttonSelected & 1) != 0)
								{
									// Instruction address 0x1d12:0x4a0b, size: 5
									this.parent.Graphics.F0_VGA_009a_ReplaceColor(this.parent.Var_aa_Screen0_Rectangle, 231, 106, 32, 8, 14, 9);
								}
								else
								{
									// Instruction address 0x1d12:0x4a32, size: 5
									this.parent.Graphics.F0_VGA_009a_ReplaceColor(this.parent.Var_aa_Screen0_Rectangle, 231, 106, 32, 8, 9, 14);
								}

								// Instruction address 0x1d12:0x4a43, size: 5
								this.parent.CommonTools.WaitTimer(10);
							}
						}

						int pressedKey = -1;

						// Instruction address 0x1d12:0x4a54, size: 5
						if (this.parent.CAPI.kbhit() == 0)
						{
							if (flag_b8)
							{
								pressedKey = 'p';
							}
						}
						else
						{
							// Instruction address 0x1d12:0x4a61, size: 5
							pressedKey = this.parent.MenuBoxDialog.F0_2d05_0ac9_GetNavigationKey();

							// The CityWorker is not processing key presses from mouse events separately
							// We don't want to mix mouse location code when pressing a key
							// Ensure that mouse event doesn't interfere

							//this.parent.Var_db3e_MouseYPos = 0;
							//this.parent.Var_db3c_MouseXPos = 0;
							mouseEvent = new MouseEvent(new GPoint(0, 0), MouseButtonsEnum.None);
						}

						if ((mouseEvent.Position.X >= 230 && mouseEvent.Position.X < 270 &&
							mouseEvent.Position.Y >= 106 && mouseEvent.Position.Y <= 116 && mouseEvent.Buttons == MouseButtonsEnum.Right) ||
							pressedKey == 'A')
						{
							city.Status ^= CityStatusEnum.AutoProduction;

							if ((city.Status & CityStatusEnum.AutoProduction) != CityStatusEnum.None)
							{
								// Instruction address 0x1d12:0x4aef, size: 5
								int newProductionID = (short)this.parent.Segment_1ade.F0_1ade_0421(this.Var_6548_PlayerID, cityID);

								this.parent.Var_aa_Screen0_Rectangle.FontID = 2;

								if (newProductionID != 99)
								{
									this.parent.Var_b1e8 = 0;

									if (city.CurrentProductionID >= 0)
									{
										this.parent.GameData.Players[this.Var_6548_PlayerID].UnitsInProduction[city.CurrentProductionID]--;
									}

									city.CurrentProductionID = (sbyte)newProductionID;

									if (city.CurrentProductionID >= 0)
									{
										this.parent.GameData.Players[this.Var_6548_PlayerID].UnitsInProduction[city.CurrentProductionID]++;
									}

									// Instruction address 0x1d12:0x4b9c, size: 5
									this.parent.DrawTools.FillRectangle(this.parent.Var_aa_Screen0_Rectangle, 230, 99, 90, 100, 0);
								}
								else
								{
									goto L045f;
								}
							}

							this.parent.Var_70da_Arr[1] = temp1;
							this.parent.Var_70da_Arr[2] = temp2;

							goto L12c2;
						}

						if (pressedKey != -1 || (mouseEvent.Buttons == MouseButtonsEnum.Left && mouseEvent.Position.Y >= 106 && mouseEvent.Position.Y <= 116))
						{
							if (mouseEvent.Position.X >= 296 || pressedKey == 'b')
							{
								int costToComplete;
								int itemShieldCost;

								if (city.CurrentProductionID >= 0)
								{
									itemShieldCost = this.parent.GameData.Units[city.CurrentProductionID].Cost;

									// Instruction address 0x1d12:0x4c3d, size: 5
									costToComplete = this.parent.Tools.CheckValueRange((10 * itemShieldCost) - city.ShieldsCount, 0, 999);

									costToComplete = (costToComplete * costToComplete) / 20 + costToComplete * 2;
								}
								else
								{
									itemShieldCost = this.parent.GameData.GetImprovementType(-city.CurrentProductionID).Cost;

									// Instruction address 0x1d12:0x4ca0, size: 5
									costToComplete = this.parent.Tools.CheckValueRange((10 * itemShieldCost) - city.ShieldsCount, 0, 999) * 2;

									if ((-city.CurrentProductionID) > 21)
									{
										costToComplete += costToComplete;
									}
								}

								if (city.ShieldsCount == 0)
								{
									costToComplete += costToComplete;
								}

								string completeProductionText = "Cost to complete\n" +
										$"{((city.CurrentProductionID >= 0) ? this.parent.GameData.Units[city.CurrentProductionID].Name :
											this.parent.GameData.GetImprovementType(-city.CurrentProductionID).Name)} is {costToComplete} coins" +
										$"\nOur treasury has {this.parent.GameData.Players[this.Var_6548_PlayerID].Coins} coins.\n";

								if ((city.Status & CityStatusEnum.CivilDisorder) != CityStatusEnum.None)
								{
									completeProductionText += "CIVIL DISORDER\n";
								}
								else if (this.parent.GameData.Players[this.Var_6548_PlayerID].Coins >= costToComplete)
								{
									completeProductionText += "Do you want to continue?\n Yes\n No\n";
								}

								this.parent.Var_aa_Screen0_Rectangle.FontID = 1;

								// Instruction address 0x1d12:0x4e32, size: 5
								if ((city.Status & CityStatusEnum.CivilDisorder) == CityStatusEnum.None && this.parent.GameData.Players[this.Var_6548_PlayerID].Coins >= costToComplete &&
									this.parent.MenuBoxDialog.F0_2d05_0031_ShowMenuBox(completeProductionText, 100, 80, true, false, true) == 0)
								{
									city.ShieldsCount = (short)(itemShieldCost * 10);

									this.parent.GameData.Players[this.Var_6548_PlayerID].Coins -= (short)costToComplete;
								}

								goto L045f;
							}

							if (pressedKey != 0x1b && pressedKey != 0xd && ((mouseEvent.Position.X >= 230 && mouseEvent.Position.X < 270) || pressedKey == 'c'))
							{
								city.ClearStatusFlags(CityStatusEnum.AutoProduction);

								if (city.CurrentProductionID >= 0)
								{
									this.parent.GameData.Players[this.Var_6548_PlayerID].UnitsInProduction[city.CurrentProductionID]--;
								}

								// Instruction address 0x1d12:0x4ee9, size: 5
								city.CurrentProductionID = (sbyte)((short)this.parent.Segment_1ade.F0_1ade_0421(this.Var_6548_PlayerID, cityID));

								if (city.CurrentProductionID >= 0)
								{
									this.parent.GameData.Players[this.Var_6548_PlayerID].UnitsInProduction[city.CurrentProductionID]++;
								}

								goto L045f;
							}

							if (pressedKey == 'i')
							{
								this.Var_2496 = 0;
								this.parent.Var_70da_Arr[1] = temp1;
								this.parent.Var_70da_Arr[2] = temp2;

								if (this.Var_2496 != 3)
									goto L12c2;

								this.parent.Var_aa_Screen0_Rectangle.FontID = 1;

								this.parent.CityView.F19_0000_0000_ShowCityLayout(cityID, -1, null);

								this.parent.Var_6b64 = 1;
								this.Var_2496 = 0;

								goto L045f;
							}

							if (pressedKey == 'h')
							{
								this.Var_2496 = 1;
								this.parent.Var_70da_Arr[1] = temp1;
								this.parent.Var_70da_Arr[2] = temp2;

								if (this.Var_2496 != 3)
									goto L12c2;

								this.parent.Var_aa_Screen0_Rectangle.FontID = 1;

								this.parent.CityView.F19_0000_0000_ShowCityLayout(cityID, -1, null);

								this.parent.Var_6b64 = 1;
								this.Var_2496 = 0;

								goto L045f;
							}

							if (pressedKey == 'm')
							{
								this.Var_2496 = 2;
								this.parent.Var_70da_Arr[1] = temp1;
								this.parent.Var_70da_Arr[2] = temp2;

								if (this.Var_2496 != 3)
									goto L12c2;

								this.parent.Var_aa_Screen0_Rectangle.FontID = 1;

								this.parent.CityView.F19_0000_0000_ShowCityLayout(cityID, -1, null);

								this.parent.Var_6b64 = 1;
								this.Var_2496 = 0;

								goto L045f;
							}

							if (pressedKey == 'v')
							{
								this.Var_2496 = 3;
								this.parent.Var_70da_Arr[1] = temp1;
								this.parent.Var_70da_Arr[2] = temp2;

								if (this.Var_2496 != 3)
									goto L12c2;

								this.parent.Var_aa_Screen0_Rectangle.FontID = 1;

								this.parent.CityView.F19_0000_0000_ShowCityLayout(cityID, -1, null);

								this.parent.Var_6b64 = 1;
								this.Var_2496 = 0;

								goto L045f;
							}

							if (pressedKey != 0x1b && pressedKey != 0xd && mouseEvent.Position.X >= 96 && mouseEvent.Position.X < 224)
							{
								this.Var_2496 = (mouseEvent.Position.X - 96) / 32;
								this.parent.Var_70da_Arr[1] = temp1;
								this.parent.Var_70da_Arr[2] = temp2;

								if (this.Var_2496 != 3)
									goto L12c2;

								this.parent.Var_aa_Screen0_Rectangle.FontID = 1;

								this.parent.CityView.F19_0000_0000_ShowCityLayout(cityID, -1, null);

								this.parent.Var_6b64 = 1;
								this.Var_2496 = 0;

								goto L045f;
							}
						}

						if (pressedKey == 'a')
						{
							int outsideFreeUnitCount;

							for (outsideFreeUnitCount = 0; outsideFreeUnitCount < cityWindowUnitCount && (this.parent.GameData.Players[this.Var_6548_PlayerID].Units[cityWindowUnitIDs[outsideFreeUnitCount]].Status &
								(UnitStatusEnum.Sentry | UnitStatusEnum.SettlerBuildRoadOrRail | UnitStatusEnum.Fortifying | UnitStatusEnum.Fortified |
									UnitStatusEnum.SettlerBuildIrrigation | UnitStatusEnum.SettlerBuildMineOrForest)) == UnitStatusEnum.None; outsideFreeUnitCount++)
							{ }

							if (outsideFreeUnitCount == cityWindowUnitCount)
							{
								outsideFreeUnitCount = 0;
							}

							do
							{
								int patternX = ((outsideFreeUnitCount % 6) * 18) + 100;
								int patternY = (outsideFreeUnitCount / 6) * 16 + 116;

								do
								{
									// Instruction address 0x1d12:0x5088, size: 5
									this.parent.DrawTools.FillRectangleWithPattern(patternX, patternY, 16, 16, 208, 100, 16, 16);

									// Instruction address 0x1d12:0x5094, size: 5
									this.parent.CommonTools.WaitTimer(10);

									// Instruction address 0x1d12:0x50b1, size: 5
									this.parent.MapManagement.F0_2aea_0fb3_DrawUnitWithStatus(this.Var_6548_PlayerID, (short)cityWindowUnitIDs[outsideFreeUnitCount], patternX, patternY);

									// Instruction address 0x1d12:0x50bd, size: 5
									this.parent.CommonTools.WaitTimer(10);
								}
								// Instruction address 0x1d12:0x50c5, size: 5
								while (this.parent.CAPI.kbhit() == 0);

								// Instruction address 0x1d12:0x50d2, size: 5
								int pressedKey1 = this.parent.MenuBoxDialog.F0_2d05_0ac9_GetNavigationKey();

								switch (pressedKey1)
								{
									// Up
									case 0x4800:
										outsideUnitIndex = outsideFreeUnitCount - 6;
										break;

									// Left
									case 0x4b00:
										outsideUnitIndex = outsideFreeUnitCount - 1;
										break;

									// Right
									case 0x4d00:
										outsideUnitIndex = outsideFreeUnitCount + 1;
										break;

									// Down
									case 0x5000:
										outsideUnitIndex = outsideFreeUnitCount + 6;
										break;

									default:
										outsideUnitIndex = outsideFreeUnitCount;
										break;
								}

								// Instruction address 0x1d12:0x5151, size: 5
								outsideFreeUnitCount = this.parent.Tools.CheckValueRange(outsideUnitIndex, 0, cityWindowUnitCount - 1);

								if (pressedKey1 == '\n' || pressedKey1 == ' ')
								{
									if ((this.parent.GameData.Players[this.Var_6548_PlayerID].Units[cityWindowUnitIDs[outsideFreeUnitCount]].Status & (UnitStatusEnum.Sentry | UnitStatusEnum.Fortified)) != UnitStatusEnum.None)
									{
										this.parent.GameData.Players[this.Var_6548_PlayerID].Units[cityWindowUnitIDs[outsideFreeUnitCount]].RemainingMoves =
											(short)(this.parent.GameData.Units[(int)this.parent.GameData.Players[this.Var_6548_PlayerID].Units[cityWindowUnitIDs[outsideFreeUnitCount]].UnitType].MoveCount * 3);
									}

									this.parent.GameData.Players[this.Var_6548_PlayerID].Units[cityWindowUnitIDs[outsideFreeUnitCount]].ClearStatusFlags(
										UnitStatusEnum.Sentry | UnitStatusEnum.SettlerBuildRoadOrRail | UnitStatusEnum.Fortifying | UnitStatusEnum.Fortified |
											UnitStatusEnum.SettlerBuildIrrigation | UnitStatusEnum.SettlerBuildMineOrForest);
									this.parent.GameData.Players[this.Var_6548_PlayerID].Units[cityWindowUnitIDs[outsideFreeUnitCount]].GoToDestination = OpenCivOneGame.InvalidPosition;
								}
							}
							while (cityWindowY != 0x1b); // Escape

							this.parent.Var_70da_Arr[1] = temp1;
							this.parent.Var_70da_Arr[2] = temp2;

							goto L12c2;
						}

						if (pressedKey == 's' && (city.Status & CityStatusEnum.ImprovementSoldThisTurn) == CityStatusEnum.None)
						{
							improvementIndex = 0;

							do
							{
								do
								{
									// Instruction address 0x1d12:0x5258, size: 5
									this.parent.Graphics.F0_VGA_009a_ReplaceColor(this.parent.Var_aa_Screen0_Rectangle,
										252, ((improvementIndex + wonderCount) * 6) + 3, 56, 7, 9, 1);

									// Instruction address 0x1d12:0x5264, size: 5
									this.parent.CommonTools.WaitTimer(10);

									// Instruction address 0x1d12:0x5294, size: 5
									this.parent.Graphics.F0_VGA_009a_ReplaceColor(this.parent.Var_aa_Screen0_Rectangle,
										252, ((improvementIndex + wonderCount) * 6) + 3, 56, 7, 1, 9);

									// Instruction address 0x1d12:0x52a0, size: 5
									this.parent.CommonTools.WaitTimer(10);
								}
								while (this.parent.CAPI.kbhit() == 0);

								// Instruction address 0x1d12:0x52b5, size: 5
								cityWindowY = this.parent.MenuBoxDialog.F0_2d05_0ac9_GetNavigationKey();

								switch (cityWindowY)
								{
									// Up
									case 0x4800:
										outsideUnitIndex = improvementIndex - 1;
										break;

									// Down
									case 0x5000:
										outsideUnitIndex = improvementIndex + 1;
										break;
								}

								// Instruction address 0x1d12:0x5301, size: 5
								improvementIndex = this.parent.Tools.CheckValueRange(outsideUnitIndex, 0, shownImprovementCount - 1);

								if (cityWindowY == '\n' || cityWindowY == ' ') goto L5740;
							}
							while (cityWindowY != 0x1b);
						}

						if (pressedKey >= '1' && pressedKey <= '9')
						{
							int flagIndex = pressedKey - '1';

							if (flagIndex < workingCitizenCount)
							{
								if (city.ActualSize > 4)
								{
									// Instruction address 0x1d12:0x536f, size: 5
									int workerFlags = F0_1d12_6da1_GetSpecialWorkerFlags(flagIndex);

									if (workerFlags < 3)
									{
										// Instruction address 0x1d12:0x538f, size: 5
										F0_1d12_6d6e_SetSpecialWorkerFlags(flagIndex, workerFlags + 1);
									}
									else
									{
										// Instruction address 0x1d12:0x53a2, size: 5
										F0_1d12_6d6e_SetSpecialWorkerFlags(flagIndex, 1);
									}

									this.parent.Var_70da_Arr[1] = temp1;
									this.parent.Var_70da_Arr[2] = temp2;

									goto L12c2;
								}

								this.parent.MenuBoxDialog.F0_2d05_0031_ShowMenuBox("A City must have five\npopulation units to support\ntaxmen or scientists.\n", 32, 32, true, false, true);
							}
							goto L045f;
						}

						if (pressedKey == 'p')
						{
							if (!flag_b8)
							{
								newCityWindowUnitX = 0;
								newCityWindowUnitY = 0;
								flag_b8 = true;
							}

							int keyPressed1;

							do
							{
								do
								{
									// Instruction address 0x1d12:0x541d, size: 5
									this.parent.Graphics.F0_VGA_07d8_DrawImage(this.parent.Var_aa_Screen0_Rectangle,
										(newCityWindowUnitX * 16) + 160, (newCityWindowUnitY * 16) + 56,
										16, 16, this.parent.Var_19d4_Screen1_Rectangle, 0, 0);

									// Instruction address 0x1d12:0x5449, size: 5
									this.parent.DrawTools.DrawRectangle((newCityWindowUnitX * 16) + 160, (newCityWindowUnitY * 16) + 56, 15, 15, 15);

									// Instruction address 0x1d12:0x5455, size: 5
									this.parent.CommonTools.WaitTimer(10);

									// Instruction address 0x1d12:0x548d, size: 5
									this.parent.Graphics.F0_VGA_07d8_DrawImage(this.parent.Var_19d4_Screen1_Rectangle, 0, 0, 16, 16, this.parent.Var_aa_Screen0_Rectangle,
										(newCityWindowUnitX * 16) + 160, (newCityWindowUnitY * 16) + 56);

									// Instruction address 0x1d12:0x5499, size: 5
									this.parent.CommonTools.WaitTimer(10);

									this.parent.Var_6b64 = 1;
								}
								while (this.parent.CAPI.kbhit() == 0);

								cityWindowX = newCityWindowUnitX;
								cityWindowY = newCityWindowUnitY;

								// Instruction address 0x1d12:0x54c4, size: 5
								keyPressed1 = this.parent.MenuBoxDialog.F0_2d05_0ac9_GetNavigationKey();

								switch (keyPressed1)
								{
									case 0x4b00:
										newCityWindowUnitX--;
										break;

									case 0x4700:
										newCityWindowUnitX--;
										newCityWindowUnitY--;
										break;

									case 0x4800:
										newCityWindowUnitY--;
										break;

									case 0x4900:
										newCityWindowUnitY--;
										newCityWindowUnitX++;
										break;

									case 0x4d00:
										newCityWindowUnitX++;
										break;

									case 0x4f00:
										newCityWindowUnitX--;
										newCityWindowUnitY++;
										break;

									case 0x5000:
										newCityWindowUnitY++;
										break;

									case 0x5100:
										newCityWindowUnitX++;
										newCityWindowUnitY++;
										break;
								}

								if (Math.Abs(newCityWindowUnitX) > 2 || Math.Abs(newCityWindowUnitY) > 2 || Math.Abs(newCityWindowUnitX) + Math.Abs(newCityWindowUnitY) >= 4 ||
									(this.parent.GameData.MapVisibility[city.Position.X + newCityWindowUnitX, city.Position.Y + newCityWindowUnitY] & (1 << this.parent.GameData.HumanPlayerID)) == 0)
								{
									newCityWindowUnitX = cityWindowX;
									newCityWindowUnitY = cityWindowY;
								}

								if (keyPressed1 == 13 || keyPressed1 == ' ')
								{
									cityWindowX = newCityWindowUnitX;
									cityWindowY = newCityWindowUnitY;

									goto L5aa6;
								}
							}
							while (keyPressed1 != 0x1b);

							flag_b8 = false;

							goto L045f;
						}

						if (pressedKey == 'M')
						{
							if (hasNextImprovementPage)
							{
								previousImprovementIndex = currentImprovementIndex;
								this.parent.Var_70da_Arr[1] = temp1;
								this.parent.Var_70da_Arr[2] = temp2;

								goto L12c2;
							}
						}

						if (pressedKey == 'r' ||
							(mouseEvent.Buttons == MouseButtonsEnum.Left && mouseEvent.Position.X >= 231 && mouseEvent.Position.Y > 190 && mouseEvent.Position.X < 270))
						{
							// Instruction address 0x1d12:0x56a6, size: 5
							this.parent.CommonTools.ClearKeyboardAndMouseEvents();

							this.parent.Var_aa_Screen0_Rectangle.FontID = 1;

							this.parent.TextBoxDialogues.F23_0000_0000_CityNameDialog(cityID);

							this.parent.Var_6b64 = 1;

							goto L045f;
						}

						if (mouseEvent.Buttons == MouseButtonsEnum.Left)
						{
							if (mouseEvent.Position.X >= 288 && mouseEvent.Position.Y < 96 && mouseEvent.Position.Y > 88)
							{
								if (hasNextImprovementPage)
								{
									previousImprovementIndex = currentImprovementIndex;
									this.parent.Var_70da_Arr[1] = temp1;
									this.parent.Var_70da_Arr[2] = temp2;

									goto L12c2;
								}
							}

							if (mouseEvent.Position.X >= 300 && mouseEvent.Position.Y < 94) goto L572e;
							goto L58ac;
						}
						goto L5cc2;

					L572e:
						improvementIndex = ((mouseEvent.Position.Y - 2) / 6) - wonderCount;

					L5740:
						for (int i = previousImprovementIndex; i < 24; i++)
						{
							if (city.HasImprovement(City.BitToImprovementEnum(i)))
							{
								improvementIndex--;

								if (improvementIndex == -1)
								{
									if ((city.Status & CityStatusEnum.ImprovementSoldThisTurn) == CityStatusEnum.None)
									{
										this.parent.Var_aa_Screen0_Rectangle.FontID = 1;

										// Instruction address 0x1d12:0x5834, size: 5
										if (this.parent.MenuBoxDialog.F0_2d05_0031_ShowMenuBox("Do you want to sell\nyour " +
											$"{this.parent.GameData.GetImprovementType(i + 1).Name} for " +
											$"{(10 * this.parent.GameData.GetImprovementType(i + 1).Cost)} coins?\n No.\n Yes.\n", 128, 80, true, false, true) == 1)
										{
											this.parent.GameData.Players[this.Var_6548_PlayerID].Coins += (short)(10 * this.parent.GameData.GetImprovementType(i + 1).Cost);
											city.RemoveImprovement(City.BitToImprovementEnum(i));
											city.Status |= CityStatusEnum.ImprovementSoldThisTurn;

											if (i + 1 == 8)
											{
												this.parent.Var_6b64 = 1;
											}
										}

										goto L045f;
									}
								}
							}
						}

					L58ac:
						if (mouseEvent.Position.X < 200 && mouseEvent.Position.Y < 20)
						{
							if (city.ActualSize < 5)
							{
								this.parent.MenuBoxDialog.F0_2d05_0031_ShowMenuBox("A City must have five\npopulation units to support\ntaxmen or scientists.\n", 32, 32, true, false, true);

								goto L045f;
							}
							else
							{
								int workerIndex = (mouseEvent.Position.X - 16) / resourceXSpacing;
								workerIndex -= city.ActualSize - currentWorkingCitizenCount;

								if (workerIndex < 0 || workerIndex >= workingCitizenCount) goto L045f;

								// Instruction address 0x1d12:0x5915, size: 5
								int workerFlags = F0_1d12_6da1_GetSpecialWorkerFlags(workerIndex);

								if (workerFlags < 3)
								{
									// Instruction address 0x1d12:0x5935, size: 5
									F0_1d12_6d6e_SetSpecialWorkerFlags(workerIndex, workerFlags + 1);
								}
								else
								{
									// Instruction address 0x1d12:0x5948, size: 5
									F0_1d12_6d6e_SetSpecialWorkerFlags(workerIndex, 1);
								}

								this.parent.Var_70da_Arr[1] = temp1;
								this.parent.Var_70da_Arr[2] = temp2;

								goto L12c2;
							}
						}

						if (mouseEvent.Position.X >= 100 && mouseEvent.Position.X < 320 && mouseEvent.Position.Y >= 116)
						{
							int selectedUnit = (((mouseEvent.Position.Y - 116) / 16) * 6) + ((mouseEvent.Position.X - 100) / 16);

							if (selectedUnit < cityWindowUnitCount)
							{
								if ((this.parent.GameData.Players[this.Var_6548_PlayerID].Units[cityWindowUnitIDs[selectedUnit]].Status & (UnitStatusEnum.Sentry | UnitStatusEnum.Fortified)) != UnitStatusEnum.None)
								{
									this.parent.GameData.Players[this.Var_6548_PlayerID].Units[cityWindowUnitIDs[selectedUnit]].RemainingMoves =
										(short)(this.parent.GameData.Units[(int)this.parent.GameData.Players[this.Var_6548_PlayerID].Units[cityWindowUnitIDs[selectedUnit]].UnitType].MoveCount * 3);
								}

								this.parent.GameData.Players[this.Var_6548_PlayerID].Units[cityWindowUnitIDs[selectedUnit]].ClearStatusFlags(UnitStatusEnum.Sentry |
									UnitStatusEnum.SettlerBuildRoadOrRail | UnitStatusEnum.Fortifying | UnitStatusEnum.Fortified | UnitStatusEnum.SettlerBuildIrrigation |
									UnitStatusEnum.SettlerBuildMineOrForest);
								this.parent.GameData.Players[this.Var_6548_PlayerID].Units[cityWindowUnitIDs[selectedUnit]].GoToDestination = OpenCivOneGame.InvalidPosition;

								this.parent.Var_70da_Arr[1] = temp1;
								this.parent.Var_70da_Arr[2] = temp2;

								goto L12c2;
							}
						}

						if (mouseEvent.Position.Y >= 24 && mouseEvent.Position.Y < 104) goto L5a74;
						goto L5cc2;

					L5a74:
						cityWindowX = (mouseEvent.Position.X / 16) - 10;
						cityWindowY = ((mouseEvent.Position.Y - 24) / 16) - 2;

					L5aa6:
						// Instruction address 0x1d12:0x5aae, size: 5
						outsideUnitIndex = F0_1d12_000a_FindCityOffset(new GPoint(cityWindowX, cityWindowY));

						if (outsideUnitIndex != -1 && outsideUnitIndex < 20)
						{
							if ((this.parent.GameData.MapVisibility[city.Position.X + this.parent.CityOffsets[outsideUnitIndex].X, city.Position.Y + this.parent.CityOffsets[outsideUnitIndex].Y] &
								(1 << this.Var_6548_PlayerID)) != 0)
							{
								this.parent.Var_70da_Arr[1] = temp1;
								this.parent.Var_70da_Arr[2] = temp2;

								if ((city.WorkerFlags & (1 << outsideUnitIndex)) != 0 ||
									!cityArea[this.parent.CityOffsets[outsideUnitIndex].X + 2, this.parent.CityOffsets[outsideUnitIndex].Y + 2])
								{
									city.WorkerFlags ^= (uint)(1 << outsideUnitIndex);

									if ((city.WorkerFlags & (1 << outsideUnitIndex)) != 0)
									{
										nonworkingCitizenCount++;

										// Instruction address 0x1d12:0x5bc9, size: 5
										F0_1d12_692d_CityResources(cityID, outsideUnitIndex, flag);

										if (this.parent.Var_e8b8 != 0)
										{
											this.parent.Var_e8b8--;
											currentWorkingCitizenCount--;
										}
									}
									else
									{
										nonworkingCitizenCount--;

										for (int i = 0; i < 3; i++)
										{
											this.parent.Var_70da_Arr[i] -= F0_1d12_6abc_GetCityResourceCount(this.Var_6548_PlayerID, this.Var_653e_CityID,
												city.Position.X + this.parent.CityOffsets[outsideUnitIndex].X, city.Position.Y + this.parent.CityOffsets[outsideUnitIndex].Y, (CityResourceTypeEnum)i);
										}

										// Instruction address 0x1d12:0x5c69, size: 5
										this.parent.MapManagement.F0_2aea_03ba_DrawCell(
											city.Position.X + this.parent.CityOffsets[outsideUnitIndex].X,
											city.Position.Y + this.parent.CityOffsets[outsideUnitIndex].Y);

										this.parent.Var_e8b8++;
										currentWorkingCitizenCount++;
									}

									city.WorkerFlags &= 0x3ffffff;
									city.WorkerFlags |= (uint)((uint)currentWorkingCitizenCount << 26);

									if (nonworkingCitizenCount > 0) goto L045f;
								}
								goto L12c2;
							}
						}
						goto L045f;
					}

					// Instruction address 0x1d12:0x5cbd, size: 5
					this.parent.Segment_2459.F0_2459_0918_WaitForKeyPressOrMouseClick();

				L5cc2:
					this.parent.Var_d4cc_MapViewX = mapViewX;
					this.parent.Var_d75e_MapViewY = mapViewY;
				}

				if (flag == 0)
				{
					if (this.Var_6548_PlayerID != 0)
					{
						int totalCityHappiness = this.parent.Var_70e2_CityHappyCitizenCount - this.parent.Var_70e4_CityUnhappyCitizenCount;

						if (totalCityHappiness < 0)
						{
							if ((this.parent.GameData.PlayerFlags & (1 << this.Var_6548_PlayerID)) != 0)
							{
								// Instruction address 0x1d12:0x5d3b, size: 5
								this.parent.CommonTools.PlayTune(36, 0);

								if (this.parent.GameData.GameSettingFlags.Animations && (city.Status & CityStatusEnum.CivilDisorder) == CityStatusEnum.None && this.parent.Var_6b92 == 0)
								{
									this.parent.CityView.F19_0000_0000_ShowCityLayout(cityID, -2,
										$"Civil Disorder in\n{this.parent.Segment_2459.F0_2459_08c6_GetCityName(cityID)}! Mayor\nflees in panic.\n");

									this.parent.CityView.F19_0000_18c1_CivilDisorderAnimation();

									// Instruction address 0x1d12:0x5d91, size: 5
									this.parent.Segment_1238.F0_1238_1b44();

									this.parent.Var_6b92 = 1;
								}
								else
								{
									this.parent.Var_2f9e_MessageBoxStyle = MenuBoxReportTypeEnum.DomesticAdvisorReport;

									// Instruction address 0x1d12:0x5db1, size: 5
									this.parent.Segment_1238.F0_1238_001e_ShowDialog($"Civil Disorder in\n{this.parent.Segment_2459.F0_2459_08c6_GetCityName(cityID)}! Mayor\nflees in panic.\n", 100, 80);
								}

								// Instruction address 0x1d12:0x5dbd, size: 5
								this.parent.CommonTools.PlayTune(1, 0);

								if ((city.Status & CityStatusEnum.CivilDisorder) == CityStatusEnum.None)
								{
									this.parent.Var_b1e8 = 1;

									if (this.parent.GameData.GameSettingFlags.InstantAdvice)
									{
										this.parent.Help.F4_0000_02d3_ShowInstantAdvicePopup("*DISORDER");
									}
								}
							}

							if ((city.Status & CityStatusEnum.CivilDisorder) == CityStatusEnum.None)
							{
								city.Status |= CityStatusEnum.CivilDisorder;

								if ((this.parent.GameData.PlayerFlags & (1 << this.Var_6548_PlayerID)) == 0)
								{
									if (city.CurrentProductionID >= 0)
									{
										this.parent.GameData.Players[this.Var_6548_PlayerID].UnitsInProduction[city.CurrentProductionID]--;
									}

									// Instruction address 0x1d12:0x5e5b, size: 5
									city.CurrentProductionID = (sbyte)((short)this.parent.Segment_1ade.F0_1ade_0421(this.Var_6548_PlayerID, cityID));

									if (city.CurrentProductionID >= 0)
									{
										this.parent.GameData.Players[this.Var_6548_PlayerID].UnitsInProduction[city.CurrentProductionID]++;
									}
								}

								// Instruction address 0x1d12:0x5ea9, size: 5
								this.parent.PlayerTurn.F0_1403_3ed7_DrawVisibleCell(cityX, cityY);
							}
							else
							{
								if (this.parent.GameData.Players[this.Var_6548_PlayerID].GovernmentType == GovernmentTypeEnum.Democracy)
								{
									if (this.Var_6548_PlayerID == this.parent.GameData.HumanPlayerID)
									{
										this.parent.CommonTools.PlayTune(36, 0);

										this.parent.News.F21_0000_0000_ShowNews(cityID,
											$"Discontented citizens of\n{this.parent.Segment_2459.F0_2459_08c6_GetCityName(cityID)} revolt:\nGovernment Collapses!\n");

										this.parent.GameData.Players[this.Var_6548_PlayerID].Diplomacy[0] |= DiplomacyFlagsEnum.Allied;

										this.parent.CommonTools.PlayTune(1, 0);
									}

									this.parent.Diplomacy.F0_2517_04a1_ChangeCurrentGovernment(this.Var_6548_PlayerID, GovernmentTypeEnum.Anarchy);
								}
							}
						}
						else
						{
							if (totalCityHappiness >= 0 && (city.Status & CityStatusEnum.CivilDisorder) != CityStatusEnum.None)
							{
								city.ClearStatusFlags(CityStatusEnum.CivilDisorder);

								if ((this.parent.GameData.PlayerFlags & (1 << this.Var_6548_PlayerID)) != 0)
								{
									this.parent.Var_2f9e_MessageBoxStyle = MenuBoxReportTypeEnum.DomesticAdvisorReport;

									// Instruction address 0x1d12:0x5fb7, size: 5
									this.parent.Segment_1238.F0_1238_001e_ShowDialog($"Order restored\nin {this.parent.Segment_2459.F0_2459_08c6_GetCityName(cityID)}.\n", 100, 80);
								}
								else
								{
									if (city.CurrentProductionID >= 0)
									{
										this.parent.GameData.Players[this.Var_6548_PlayerID].UnitsInProduction[city.CurrentProductionID]--;
									}

									// Instruction address 0x1d12:0x5ff9, size: 5
									city.CurrentProductionID = (sbyte)((short)this.parent.Segment_1ade.F0_1ade_0421(this.Var_6548_PlayerID, cityID));

									if (city.CurrentProductionID >= 0)
									{
										this.parent.GameData.Players[this.Var_6548_PlayerID].UnitsInProduction[city.CurrentProductionID]++;
									}
								}

								// Instruction address 0x1d12:0x6047, size: 5
								this.parent.PlayerTurn.F0_1403_3ed7_DrawVisibleCell(cityX, cityY);
							}

							if (this.parent.GameData.Players[this.Var_6548_PlayerID].GovernmentType != GovernmentTypeEnum.Anarchy)
							{
								this.parent.GameData.Players[this.Var_6548_PlayerID].Coins += (short)this.parent.Var_e17a;
							}
						}

						if (this.parent.Var_70e4_CityUnhappyCitizenCount == 0 && city.ActualSize > 2 &&
							((city.ActualSize + 1) / 2) <= this.parent.Var_70e2_CityHappyCitizenCount &&
							this.parent.GameData.Players[this.Var_6548_PlayerID].GovernmentType != GovernmentTypeEnum.Anarchy)
						{
							if ((city.Status & CityStatusEnum.Celebrating) == CityStatusEnum.None)
							{
								if ((this.parent.GameData.PlayerFlags & (1 << this.Var_6548_PlayerID)) != 0)
								{
									if (this.parent.GameData.Players[this.Var_6548_PlayerID].GovernmentType != GovernmentTypeEnum.Anarchy)
									{
										// Instruction address 0x1d12:0x6143, size: 5
										this.parent.CommonTools.PlayTune(34, 0);

										if (this.parent.GameData.GameSettingFlags.Animations)
										{
											this.parent.CityView.F19_0000_0000_ShowCityLayout(cityID, -2,
												$"'We love the {this.parent.Array_19b2_GovernmentTypeNames[(int)this.parent.GameData.Players[this.Var_6548_PlayerID].GovernmentType]}'\n" +
												$"day celebrated in\n{this.parent.Segment_2459.F0_2459_08c6_GetCityName(cityID)}!\n");

											this.parent.CityView.F19_0000_1ae1_LoveOurLeaderAnimation();

											// Instruction address 0x1d12:0x617d, size: 5
											this.parent.Segment_1238.F0_1238_1b44();
										}
										else
										{
											this.parent.News.F21_0000_0000_ShowNews(cityID,
												$"'We love the {this.parent.Array_19b2_GovernmentTypeNames[(int)this.parent.GameData.Players[this.Var_6548_PlayerID].GovernmentType]}'\n" +
												$"day celebrated in\n{this.parent.Segment_2459.F0_2459_08c6_GetCityName(cityID)}!\n");
										}

										// Instruction address 0x1d12:0x6194, size: 5
										this.parent.CommonTools.PlayTune(1, 0);
									}
								}
								city.Status |= CityStatusEnum.Celebrating;
							}
							else if (this.parent.GameData.Players[this.Var_6548_PlayerID].GovernmentType == GovernmentTypeEnum.Republic ||
								this.parent.GameData.Players[this.Var_6548_PlayerID].GovernmentType == GovernmentTypeEnum.Democracy)
							{
								if (((city.ActualSize * 2) + (productionRatio * this.parent.Var_e3c6)) < this.parent.Var_70da_Arr[0])
								{
									if (city.ActualSize < 10 || city.HasImprovement(ImprovementEnum.Aqueduct))
									{
										city.ActualSize++;
									}
								}
							}
						}
						else if (totalCityHappiness >= 0 && (city.Status & CityStatusEnum.Celebrating) != CityStatusEnum.None)
						{
							if ((this.parent.GameData.PlayerFlags & (1 << this.Var_6548_PlayerID)) != 0)
							{
								if (this.parent.GameData.Players[this.Var_6548_PlayerID].GovernmentType != GovernmentTypeEnum.Anarchy)
								{
									this.parent.CommonTools.PlayTune(35, 0);

									this.parent.News.F21_0000_0000_ShowNews(cityID,
										$"'We love the {this.parent.Array_19b2_GovernmentTypeNames[(int)this.parent.GameData.Players[this.Var_6548_PlayerID].GovernmentType]}'\n" +
										$"celebration canceled\nin {this.parent.Segment_2459.F0_2459_08c6_GetCityName(cityID)}.\n");

									this.parent.CommonTools.PlayTune(1, 0);
								}
							}

							city.ClearStatusFlags(CityStatusEnum.Celebrating);
						}
					}
				
					int newResearchPoints = (short)this.parent.Var_70e6;

					if (this.Var_6548_PlayerID == this.parent.GameData.HumanPlayerID)
					{

						if ((this.parent.GameData.Players[this.parent.GameData.HumanPlayerID].ResearchTechnologyID != -1 &&
							this.parent.Segment_1ade.F0_1ade_22b5_PlayerHasTechnology(this.Var_6548_PlayerID,
								(TechnologyAdvanceEnum)this.parent.GameData.Players[this.parent.GameData.HumanPlayerID].ResearchTechnologyID)) ||
							(this.parent.GameData.Players[this.parent.GameData.HumanPlayerID].ResearchTechnologyID == -1 &&
							this.parent.GameData.Players[this.Var_6548_PlayerID].ResearchProgress != 0))
						{
							this.parent.GameData.Players[this.parent.GameData.HumanPlayerID].ResearchTechnologyID = -1;

							if ((this.parent.GameData.DebugFlags & 0x8) != 0)
							{
								// Instruction address 0x1d12:0x6344, size: 5
								this.parent.Segment_1ade.F0_1ade_1584(this.Var_6548_PlayerID, 0);
							}

							this.parent.GameData.Players[this.Var_6548_PlayerID].ResearchProgress = 0;
						}
					}

					if (this.parent.GameData.Players[this.Var_6548_PlayerID].GovernmentType == GovernmentTypeEnum.Anarchy)
					{
						newResearchPoints = 0;
					}

					this.parent.GameData.Players[this.Var_6548_PlayerID].ResearchProgress += (short)newResearchPoints;

					if (this.Var_6548_PlayerID == this.parent.GameData.HumanPlayerID && this.parent.GameData.DifficultyLevel == 0)
					{
						if (this.parent.GameData.Players[this.parent.GameData.HumanPlayerID].ResearchTechnologyID >= 0 &&
							(this.parent.GameData.TechnologyFirstDiscoveredBy[this.parent.GameData.Players[this.parent.GameData.HumanPlayerID].ResearchTechnologyID] & 7) != 0)
						{
							this.parent.GameData.Players[this.Var_6548_PlayerID].ResearchProgress += (short)newResearchPoints;
						}
					}

					// Instruction address 0x1d12:0x63c2, size: 5
					if (F0_1d12_6c97_PlayerHasWonder(this.Var_6548_PlayerID, WonderEnum.SETIProgram))
					{
						this.parent.GameData.Players[this.Var_6548_PlayerID].ResearchProgress += (short)(newResearchPoints / 2);
					}

					int technologyDifficultyRatio;

					if (this.Var_6548_PlayerID != this.parent.GameData.HumanPlayerID)
					{
						technologyDifficultyRatio = 14 - this.parent.GameData.DifficultyLevel;
					}
					else
					{
						technologyDifficultyRatio = (this.parent.GameData.DifficultyLevel * 2) + 6;
					}

					technologyDifficultyRatio += this.parent.Var_d2de;

					if (this.Var_6548_PlayerID == this.parent.GameData.HumanPlayerID &&
						(11 - this.parent.GameData.Players[this.Var_6548_PlayerID].DiscoveredTechnologyCount) > technologyDifficultyRatio)
					{
						technologyDifficultyRatio = 11 - this.parent.GameData.Players[this.Var_6548_PlayerID].DiscoveredTechnologyCount;
					}

					if (this.Var_6548_PlayerID != this.parent.GameData.HumanPlayerID || this.parent.Var_b1e8 == 0)
					{
						if ((this.parent.GameData.Players[this.Var_6548_PlayerID].ResearchProgress / ((this.parent.GameData.Year < 0) ? 1 : 2)) >
							(this.parent.GameData.Players[this.Var_6548_PlayerID].DiscoveredTechnologyCount * technologyDifficultyRatio))
						{
							this.parent.GameData.Players[this.Var_6548_PlayerID].ResearchProgress = 0;

							if ((this.parent.GameData.DebugFlags & 0x8) != 0)
							{
								// Instruction address 0x1d12:0x64ab, size: 5
								this.parent.Segment_1ade.F0_1ade_1584(this.Var_6548_PlayerID, 0);
							}
						}
					}

					this.parent.GameData.Players[this.Var_6548_PlayerID].Score +=
						(short)((city.ActualSize + this.parent.Var_70e2_CityHappyCitizenCount) - this.parent.Var_70e4_CityUnhappyCitizenCount);

					if (this.Var_6548_PlayerID == this.parent.GameData.HumanPlayerID)
					{
						int local_e0 = (this.parent.Var_70da_Arr[1] / this.parent.Var_6c98) - 20;
						local_e0 += (city.ActualSize * this.parent.Var_b882) / 4;

						// Instruction address 0x1d12:0x6530, size: 5
						if ((local_e0 * 2) >
							this.parent.CAPI.RNG.Next(256 - ((this.parent.GameData.Players[this.Var_6548_PlayerID].DiscoveredTechnologyCount * this.parent.GameData.DifficultyLevel) / 2)))
						{
							// Instruction address 0x1d12:0x6549, size: 5
							int cityOffsetIndex = this.parent.CAPI.RNG.Next(20);
							int newX = this.parent.CityOffsets[cityOffsetIndex].X + cityX;
							int newY = this.parent.CityOffsets[cityOffsetIndex].Y + cityY;

							// Instruction address 0x1d12:0x6581, size: 5
							if (!this.parent.MapManagement.F0_2aea_1585_GetVisibleTerrainImprovements(newX, newY).HasFlag(TerrainImprovementFlagsEnum.Pollution) &&
								this.parent.MapManagement.GetTerrainType(newX, newY) != TerrainTypeEnum.Water &&
								!this.parent.MapManagement.F0_2aea_1585_GetVisibleTerrainImprovements(newX, newY).HasFlag(TerrainImprovementFlagsEnum.City))
							{
								// Instruction address 0x1d12:0x65c7, size: 5
								F0_1d12_6d33_AddPollutionToCell(newX, newY);

								// Instruction address 0x1d12:0x65e3, size: 5
								this.parent.MapManagement.F0_2aea_0008_DrawVisibleMap(this.parent.GameData.HumanPlayerID, newX - 8, newY - 6);

								// Instruction address 0x1d12:0x65f3, size: 5
								this.parent.MapManagement.F0_2aea_11d4_DrawCellWithUnit(newX, newY);

								this.parent.Var_2f9e_MessageBoxStyle = MenuBoxReportTypeEnum.ScienceAdvisorReport;

								// Instruction address 0x1d12:0x6638, size: 5
								this.parent.Segment_1238.F0_1238_001e_ShowDialog($"Pollution near {this.parent.Segment_2459.F0_2459_08c6_GetCityName(cityID)}.\n", 80, 64);
							}
						}

						if ((city.Status & CityStatusEnum.CivilDisorder) != CityStatusEnum.None && city.HasImprovement(ImprovementEnum.NuclearPlant) &&
							!this.parent.Segment_1ade.F0_1ade_22b5_PlayerHasTechnology(this.Var_6548_PlayerID, TechnologyAdvanceEnum.FusionPower) && this.parent.CAPI.RNG.Next(3) == 0)
						{
							// Instruction address 0x1d12:0x66a5, size: 5
							this.parent.MapManagement.F0_2aea_0008_DrawVisibleMap(this.parent.GameData.HumanPlayerID, cityX - 8, cityY - 6);

							this.parent.Animations.F22_0000_0967_NukeAnimation(cityX, cityY);

							// Instruction address 0x1d12:0x66c5, size: 5
							this.parent.Segment_29f3.F0_29f3_0ec3_AddEffectsOfNuclearAttackOrCatastrophe(cityX, cityY);

							city.RemoveImprovement(ImprovementEnum.NuclearPlant);

							this.parent.CommonTools.PlayTune(36, 0);

							this.parent.News.F21_0000_0000_ShowNews(cityID,
								$"Nuclear Catastrophe\nin {this.parent.Segment_2459.F0_2459_08c6_GetCityName(cityID)}!\nContamination feared!\n");

							this.parent.CommonTools.PlayTune(1, 0);
						}

						for (int i = 1; i < 24; i++)
						{
							if (city.HasImprovement(City.BitToImprovementEnum(i)) && this.parent.GameData.Players[this.Var_6548_PlayerID].GovernmentType != GovernmentTypeEnum.Anarchy)
							{
								this.parent.GameData.Players[this.Var_6548_PlayerID].Coins -= (short)this.parent.GameData.GetImprovementType(i + 1).MaintenanceCost;

								if (i == 1)
								{
									if (this.parent.GameData.DifficultyLevel >= 2)
									{
										this.parent.GameData.Players[this.Var_6548_PlayerID].Coins--;
									}

									if (this.parent.GameData.DifficultyLevel >= 4)
									{
										this.parent.GameData.Players[this.Var_6548_PlayerID].Coins--;
									}

									// Instruction address 0x1d12:0x67cf, size: 5
									if (this.parent.Segment_1ade.F0_1ade_22b5_PlayerHasTechnology(this.Var_6548_PlayerID, TechnologyAdvanceEnum.Gunpowder))
									{
										this.parent.GameData.Players[this.Var_6548_PlayerID].Coins--;
									}

									// Instruction address 0x1d12:0x67f1, size: 5
									if (this.parent.Segment_1ade.F0_1ade_22b5_PlayerHasTechnology(this.Var_6548_PlayerID, TechnologyAdvanceEnum.Combustion))
									{
										this.parent.GameData.Players[this.Var_6548_PlayerID].Coins--;
									}
								}

								if (this.parent.GameData.Players[this.Var_6548_PlayerID].Coins < 0)
								{
									this.parent.GameData.Players[this.Var_6548_PlayerID].Coins = 0;

									city.RemoveImprovement(City.BitToImprovementEnum(i));

									this.parent.Var_2f9e_MessageBoxStyle = MenuBoxReportTypeEnum.DomesticAdvisorReport;

									// Instruction address 0x1d12:0x68a7, size: 5
									this.parent.Segment_1238.F0_1238_001e_ShowDialog(
										$"{this.parent.Segment_2459.F0_2459_08c6_GetCityName(cityID)}\ncan't maintain\n{this.parent.GameData.GetImprovementType(i + 1).Name}.\n", 100, 80);

									this.parent.GameData.Players[this.Var_6548_PlayerID].Coins += (short)(10 * this.parent.GameData.GetImprovementType(i + 1).Cost);
								}
							}
						}
					}
				}
			
				// !!! this playerID reference needs to be checked!
				if ((city.Status & CityStatusEnum.FortifiedUnitsRestored) != CityStatusEnum.None && !this.parent.UnitManagement.F0_1866_18d0_IsEnemyUnitNear(city.PlayerID, city.Position.X, city.Position.Y))
				{
					// Instruction address 0x1d12:0x6907, size: 5
					this.parent.UnitManagement.F0_1866_00c6_StoreFortifiedCityUnits(cityID);
				}

				this.parent.Var_aa_Screen0_Rectangle.FontID = 1;

				// !!! Do we really need to return a value?
				return this.parent.Var_70e2_CityHappyCitizenCount - this.parent.Var_70e4_CityUnhappyCitizenCount;
			}

		L6927:
			return 0;
		}

		/// <summary>
		/// Finds Offset index for a give Point
		/// </summary>
		/// <param name="xPos"></param>
		/// <param name="yPos"></param>
		/// <returns></returns>
		private int F0_1d12_000a_FindCityOffset(GPoint point)
		{
			// function body
			for (int i = 0; i < 21; i++)
			{
				if (this.parent.CityOffsets[i].Equals(point))
				{
					return i;
				}
			}

			return -1;
		}

		/// <summary>
		/// Gets and draws (if flag=1) city resources
		/// </summary>
		/// <param name="cityID"></param>
		/// <param name="cityOffset"></param>
		/// <param name="flag"></param>
		private void F0_1d12_692d_CityResources(int cityID, int cityOffset, int flag)
		{
			//this.CPU.Log.EnterBlock($"F0_1d12_692d_CityResources({cityID}, {cityOffset}, {flag})");

			// function body
			int newX = this.parent.GameData.Cities[cityID].Position.X + this.parent.CityOffsets[cityOffset].X;
			int newY = this.parent.GameData.Cities[cityID].Position.Y + this.parent.CityOffsets[cityOffset].Y;
			int xStep = 0;
			int totalResources = 0;

			if (flag == 1)
			{
				// Instruction address 0x1d12:0x696f, size: 5
				this.parent.MapManagement.F0_2aea_03ba_DrawCell(newX, newY);

				totalResources = 0;

				foreach (CityResourceTypeEnum cityResourceType in Enum.GetValues<CityResourceTypeEnum>())
				{
					// Instruction address 0x1d12:0x698b, size: 3
					totalResources += F0_1d12_6abc_GetCityResourceCount(this.Var_6548_PlayerID, this.Var_653e_CityID, newX, newY, cityResourceType);
				}

				if (totalResources < 5)
				{
					xStep = 8;
				}
				else if (totalResources < 7)
				{
					xStep = 5;
				}
				else
				{
					xStep = 3;
				}
			}
			else
			{
				this.Var_2494 = 1;

				// Instruction address 0x1d12:0x69dd, size: 5
				this.parent.MapManagement.SetPlayerLandOwnership(newX, newY, this.parent.GameData.Cities[cityID].PlayerID);
			}

			int xOffset = 0;
			int yOffset = 0;

			foreach (CityResourceTypeEnum cityResourceType in Enum.GetValues<CityResourceTypeEnum>())
			{
				// Instruction address 0x1d12:0x6a10, size: 3
				int resourceCount = F0_1d12_6abc_GetCityResourceCount(this.Var_6548_PlayerID, this.Var_653e_CityID, newX, newY, cityResourceType);

				this.parent.Var_70da_Arr[(int)cityResourceType] += resourceCount;

				while (flag == 1 && resourceCount > 0)
				{
					// Instruction address 0x1d12:0x6a5d, size: 5
					this.parent.Graphics.F0_VGA_0c3e_DrawBitmapToScreen(this.parent.Var_aa_Screen0_Rectangle,
						161 + (this.parent.CityOffsets[cityOffset].X * 16) + xOffset,
						57 + (this.parent.CityOffsets[cityOffset].Y * 16) + yOffset,
						this.parent.Array_d4ce[(int)cityResourceType + 8]);

					if (xOffset < 8)
					{
						xOffset += xStep;
					}
					else
					{
						xOffset = 0;
						yOffset += 8;
					}

					resourceCount--;
				}
			}

			if (flag == 1 && totalResources == 0)
			{
				// Instruction address 0x1d12:0x6aa8, size: 5
				this.parent.Graphics.F0_VGA_0c3e_DrawBitmapToScreen(this.parent.Var_aa_Screen0_Rectangle,
					165 + (this.parent.CityOffsets[cityOffset].X * 16),
					61 + (this.parent.CityOffsets[cityOffset].Y * 16),
					this.parent.Array_d4ce[13]);
			}

			this.Var_2494 = 0;
		}

		/// <summary>
		/// Gets City resource count by resource type
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="resourceType"></param>
		/// <returns></returns>
		public int F0_1d12_6abc_GetCityResourceCount(int playerID, int cityID, int x, int y, CityResourceTypeEnum resourceType)
		{
			//this.oCPU.Log.EnterBlock($"F0_1d12_6abc_GetCityResourceCount({playerID}, {cityID}, {x}, {y}, {resourceType})");

			// function body
			// Instruction address 0x1d12:0x6ac9, size: 5
			if (this.parent.MapManagement.ValidateMapCoordinates(x, y))
			{
				// Instruction address 0x1d12:0x6ade, size: 5
				TerrainTypeEnum terrainType = this.parent.MapManagement.GetTerrainType(x, y);
				int terrainIndex = (int)terrainType;

				// Instruction address 0x1d12:0x6af0, size: 5
				if (this.parent.MapManagement.F0_2aea_1836_CellHasSpecialResource(x, y))
				{
					// Access addon terrains
					terrainIndex += 12;
				}

				int resourceCount = 0;

				switch (resourceType)
				{
					case CityResourceTypeEnum.Food:
						resourceCount = this.parent.GameData.Terrains[terrainIndex].Food;
						break;

					case CityResourceTypeEnum.Production:
						resourceCount = this.parent.GameData.Terrains[terrainIndex].Production;
						break;

					case CityResourceTypeEnum.Trade:
						resourceCount = this.parent.GameData.Terrains[terrainIndex].Trade;
						break;

					default:
						throw new Exception("Unknown terrain field");
				}

				// Instruction address 0x1d12:0x6b26, size: 5
				TerrainImprovementFlagsEnum improvements = this.parent.MapManagement.F0_2aea_1585_GetVisibleTerrainImprovements(x, y);

				if ((this.parent.GameData.DebugFlags & 0x2) == 0)
				{
					improvements = (terrainType == TerrainTypeEnum.Desert || terrainType == TerrainTypeEnum.Plains ||
						terrainType == TerrainTypeEnum.Grassland) ? TerrainImprovementFlagsEnum.Irrigation : TerrainImprovementFlagsEnum.Mines;

					improvements |= (terrainType != TerrainTypeEnum.Plains) ? TerrainImprovementFlagsEnum.Road : TerrainImprovementFlagsEnum.None;
				}

				if (terrainType != TerrainTypeEnum.Water)
				{
					if (resourceType == CityResourceTypeEnum.Food && improvements.HasFlag(TerrainImprovementFlagsEnum.Irrigation))
					{
						resourceCount += -1 - this.parent.GameData.TerrainModifications[(int)terrainType].IrrigationEffect;
					}

					if (resourceType == CityResourceTypeEnum.Production && improvements.HasFlag(TerrainImprovementFlagsEnum.Mines))
					{
						resourceCount += -1 - this.parent.GameData.TerrainModifications[(int)terrainType].MiningEffect;
					}

					if (resourceType == CityResourceTypeEnum.Trade &&
						improvements.HasFlag(TerrainImprovementFlagsEnum.Road) &&
						(terrainType == TerrainTypeEnum.Desert || terrainType == TerrainTypeEnum.Plains || terrainType == TerrainTypeEnum.Grassland))
					{
						resourceCount++;
					}
				}

				if (resourceType == CityResourceTypeEnum.Production &&
					(terrainType == TerrainTypeEnum.Grassland || terrainType == TerrainTypeEnum.River) &&
					(((x * 7) + (y * 11)) & 0x2) != 0)
				{
					resourceCount = 0;
				}

				// Instruction address 0x1d12:0x6be7, size: 3
				if (resourceCount != 0 && resourceType == CityResourceTypeEnum.Trade && F0_1d12_6cf3_GetWonderCityID((int)WonderEnum.Colossus) == cityID)
				{
					resourceCount++;
				}

				if (improvements.HasFlag(TerrainImprovementFlagsEnum.RailRoad))
				{
					resourceCount += resourceCount / 2;
				}

				if (resourceCount > 2 && (this.parent.GameData.Cities[cityID].Status & CityStatusEnum.Celebrating) == CityStatusEnum.None)
				{
					if (this.parent.GameData.Players[playerID].GovernmentType == GovernmentTypeEnum.Anarchy ||
						this.parent.GameData.Players[playerID].GovernmentType == GovernmentTypeEnum.Despotism)
					{
						resourceCount--;
					}

					if (this.Var_2494 != 0)
					{
						this.parent.Var_e3c2 -= 2;
					}
				}

				if (resourceCount != 0 && resourceType == CityResourceTypeEnum.Trade)
				{
					if (this.Var_2494 != 0)
					{
						this.parent.Var_db42++;
					}

					if ((this.parent.GameData.Cities[cityID].Status & CityStatusEnum.Celebrating) != CityStatusEnum.None)
					{
						if (this.parent.GameData.Players[playerID].GovernmentType == GovernmentTypeEnum.Monarchy ||
							this.parent.GameData.Players[playerID].GovernmentType == GovernmentTypeEnum.Communist ||
							this.parent.GameData.Players[playerID].GovernmentType == GovernmentTypeEnum.Republic ||
							this.parent.GameData.Players[playerID].GovernmentType == GovernmentTypeEnum.Democracy)
						{
							resourceCount++;
						}
					}
					else
					{
						if (this.parent.GameData.Players[playerID].GovernmentType == GovernmentTypeEnum.Republic ||
							this.parent.GameData.Players[playerID].GovernmentType == GovernmentTypeEnum.Democracy)
						{
							resourceCount++;
						}
					}
				}

				if (improvements.HasFlag(TerrainImprovementFlagsEnum.Pollution))
				{
					resourceCount = (resourceCount + 1) / 2;
				}

				if (resourceCount < 0)
				{
					resourceCount = 0;
				}

				return resourceCount;
			}

			return 0;
		}

		/// <summary>
		/// Tests if player has built a Wonder and if the Wonder is not obsolete
		/// </summary>
		/// <param name="playerID"></param>
		/// <param name="wonderType"></param>
		/// <returns></returns>
		public bool F0_1d12_6c97_PlayerHasWonder(int playerID, WonderEnum wonderType)
		{
			//this.oCPU.Log.EnterBlock($"F0_1d12_6c97_PlayerHasWonder({playerID}, {wonderType})");

			// function body
			int i;

			for (i = 1; i < 8; i++)
			{
				// Instruction address 0x1d12:0x6cb9, size: 5
				ImprovementDefinition wonder = this.parent.GameData.Wonders[(int)wonderType];

				if (wonder.ObsoletesAfterTechnology != TechnologyAdvanceEnum.None &&
					this.parent.Segment_1ade.F0_1ade_22b5_PlayerHasTechnology(i, wonder.ObsoletesAfterTechnology))
				{
					break;
				}
			}

			if (i < 8 || this.parent.GameData.WonderCityID[(int)wonderType] == -1 ||
				this.parent.GameData.Cities[this.parent.GameData.WonderCityID[(int)wonderType]].PlayerID != playerID)
			{
				return false;
			}
			else
			{
				return true;
			}
		}

		/// <summary>
		/// Returns a CityID that has a Wonder, and only if the Wonder is not obsolete
		/// </summary>
		/// <param name="wonderID"></param>
		/// <returns></returns>
		private int F0_1d12_6cf3_GetWonderCityID(int wonderID)
		{
			//this.CPU.Log.EnterBlock($"F0_1d12_6cf3_GetWonderCityID({wonderID})");

			// function body
			int i;

			for (i = 1; i < 8; i++)
			{
				// Instruction address 0x1d12:0x6d15, size: 5
				if (this.parent.GameData.Wonders[wonderID].ObsoletesAfterTechnology != TechnologyAdvanceEnum.None &&
					this.parent.Segment_1ade.F0_1ade_22b5_PlayerHasTechnology(i, this.parent.GameData.Wonders[wonderID].ObsoletesAfterTechnology))
				{
					break;
				}
			}

			if (i < 8)
			{
				return -1;
			}
			else
			{
				return this.parent.GameData.WonderCityID[wonderID];
			}
		}

		/// <summary>
		/// ?
		/// </summary>
		/// <param name="xPos"></param>
		/// <param name="yPos"></param>
		public void F0_1d12_6d33_AddPollutionToCell(int xPos, int yPos)
		{
			//this.CPU.Log.EnterBlock($"F0_1d12_6d33({xPos}, {yPos})");

			// function body
			// Instruction address 0x1d12:0x6d3c, size: 5
			if (!this.parent.MapManagement.F0_2aea_1585_GetVisibleTerrainImprovements(xPos, yPos).HasFlag(TerrainImprovementFlagsEnum.Pollution))
			{
				// Instruction address 0x1d12:0x6d52, size: 5
				this.parent.MapManagement.F0_2aea_1653_SetTerrainImprovements(xPos, yPos, TerrainImprovementFlagsEnum.Pollution);

				// Instruction address 0x1d12:0x6d60, size: 5
				this.parent.MapManagement.F0_2aea_1601_UpdateVisibleCellStatus(xPos, yPos);

				this.parent.GameData.PollutedSquareCount++;
			}		
		}

		/// <summary>
		/// ?
		/// </summary>
		/// <param name="position"></param>
		/// <param name="flag"></param>
		public void F0_1d12_6d6e_SetSpecialWorkerFlags(int position, int flag)
		{
			//this.CPU.Log.EnterBlock($"F0_1d12_6d6e_SetSpecialWorkerFlags({position}, {flag})");

			// function body
			if (position < 8)
			{
				this.parent.GameData.Cities[this.Var_653e_CityID].SpecialWorkerFlags &= (ushort)(~(0x3 << (position * 2)));

				this.parent.GameData.Cities[this.Var_653e_CityID].SpecialWorkerFlags |= (ushort)((flag & 0x3) << (position * 2));
			}		
		}

		/// <summary>
		/// ?
		/// </summary>
		/// <param name="position"></param>
		/// <returns></returns>
		public int F0_1d12_6da1_GetSpecialWorkerFlags(int position)
		{
			//this.CPU.Log.EnterBlock($"F0_1d12_6da1_GetSpecialWorkerFlags({position})");

			// function body
			if (position < 8)
			{
				return ((this.parent.GameData.Cities[this.Var_653e_CityID].SpecialWorkerFlags >> (position * 2)) & 0x3);
			}
			else
			{
				return 1;
			}		
		}

		/// <summary>
		/// ?
		/// </summary>
		/// <param name="workerType"></param>
		/// <returns></returns>
		private int F0_1d12_6dcc_GetWorkerCountByType(int workerType)
		{
			//this.CPU.Log.EnterBlock($"F0_1d12_6dcc_GetWorkerCountByType({workerType})");

			// function body
			int workerCount = 0;

			for (int i = 0; i < 8; i++)
			{
				// Instruction address 0x1d12:0x6de0, size: 3
				if (F0_1d12_6da1_GetSpecialWorkerFlags(i) == workerType)
				{
					workerCount++;
				}
			}

			return workerCount;
		}

		/// <summary>
		/// ?
		/// </summary>
		/// <param name="cityID"></param>
		/// <param name="param2"></param>
		private void F0_1d12_6dfe_AdjustHappyUnhappyCitizens(int cityID, int size)
		{
			//this.CPU.Log.EnterBlock($"F0_1d12_6dfe({cityID}, {size})");

			// function body
			while (this.Var_6542 >= 0 && this.parent.Var_70e4_CityUnhappyCitizenCount < this.Var_6542)
			{
				this.Var_6542--;
				this.parent.Var_70e4_CityUnhappyCitizenCount++;
			}

			// Instruction address 0x1d12:0x6e16, size: 5
			this.parent.Var_70e2_CityHappyCitizenCount = this.parent.Tools.CheckValueRange(this.parent.Var_70e2_CityHappyCitizenCount, 0, this.parent.GameData.Cities[cityID].ActualSize);
			// Instruction address 0x1d12:0x6e98, size: 5
			this.parent.Var_70e4_CityUnhappyCitizenCount = this.parent.Tools.CheckValueRange(this.parent.Var_70e4_CityUnhappyCitizenCount, 0, this.parent.GameData.Cities[cityID].ActualSize);

			// Instruction address 0x1d12:0x6ebb, size: 5
			while ((this.parent.Var_70e2_CityHappyCitizenCount + this.parent.Var_70e4_CityUnhappyCitizenCount) > this.parent.Tools.CheckValueRange(this.parent.GameData.Cities[cityID].ActualSize - size, 0, 99))
			{
				if (this.Var_6542 > 0)
				{
					this.Var_6542--;
				}
				else
				{
					this.parent.Var_70e2_CityHappyCitizenCount--;

					// Instruction address 0x1d12:0x6e74, size: 5
					this.parent.Var_70e2_CityHappyCitizenCount = this.parent.Tools.CheckValueRange(this.parent.Var_70e2_CityHappyCitizenCount, 0, this.parent.GameData.Cities[cityID].ActualSize);
				}

				this.parent.Var_70e4_CityUnhappyCitizenCount--;

				// Instruction address 0x1d12:0x6e98, size: 5
				this.parent.Var_70e4_CityUnhappyCitizenCount = this.parent.Tools.CheckValueRange(this.parent.Var_70e4_CityUnhappyCitizenCount, 0, this.parent.GameData.Cities[cityID].ActualSize);
			}
		}

		/// <summary>
		/// Draws city resources
		/// </summary>
		/// <param name="cityID"></param>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="param4"></param>
		/// <param name="width"></param>
		/// <returns></returns>
		public int F0_1d12_6ed4_DrawResources(int cityID, int x, int y, int param4, int width)
		{
			//this.CPU.Log.EnterBlock($"F0_1d12_6ed4_DrawResources({cityID}, {xPos}, {yPos}, {param4}, {width})");

			// function body
			int citySize = this.parent.GameData.Cities[cityID].ActualSize;
			int i;
			int xSpacing;
			
			if (citySize * 7 > width)
			{
				xSpacing = width / citySize;
			}
			else
			{
				xSpacing = 7;
			}

			for (i = 0; i < this.parent.Var_70e2_CityHappyCitizenCount; i++)
			{
				// Instruction address 0x1d12:0x6f26, size: 5
				this.parent.Graphics.F0_VGA_0c3e_DrawBitmapToScreen(this.parent.Var_aa_Screen0_Rectangle, x, y, this.parent.Array_6e96[i & 1]);

				x += xSpacing;
			}

			for (i = 0; i < this.parent.GameData.Cities[cityID].ActualSize - param4 - this.parent.Var_70e2_CityHappyCitizenCount - this.parent.Var_70e4_CityUnhappyCitizenCount; i++)
			{
				// Instruction address 0x1d12:0x6f66, size: 5
				this.parent.Graphics.F0_VGA_0c3e_DrawBitmapToScreen(this.parent.Var_aa_Screen0_Rectangle, x, y, this.parent.Array_6e96[2 + (i & 1)]);

				x += xSpacing;
			}

			if (i != 0)
			{
				x += 2;
			}

			for (i = 0; i < this.parent.Var_70e4_CityUnhappyCitizenCount; i++)
			{
				// Instruction address 0x1d12:0x6fbb, size: 5
				this.parent.Graphics.F0_VGA_0c3e_DrawBitmapToScreen(this.parent.Var_aa_Screen0_Rectangle, x, y, this.parent.Array_6e96[4 + (i & 1)]);

				if (i < this.Var_6542)
				{
					// Instruction address 0x1d12:0x6fe5, size: 5
					this.parent.Graphics.F0_VGA_009a_ReplaceColor(this.parent.Var_aa_Screen0_Rectangle, x, y, 12, 14, 5, 12);
				}

				x += xSpacing;
			}

			x += 4;

			for (i = 0; i < param4; i++)
			{
				// Instruction address 0x1d12:0x7025, size: 5
				this.parent.Graphics.F0_VGA_0c3e_DrawBitmapToScreen(this.parent.Var_aa_Screen0_Rectangle,
					x, y, this.parent.Array_6e96[5 + F0_1d12_6da1_GetSpecialWorkerFlags(i)]);

				x += xSpacing;
			}

			return xSpacing;
		}

		/// <summary>
		/// ?
		/// </summary>
		/// <param name="xSrc"></param>
		/// <param name="xDst"></param>
		/// <param name="yDst"></param>
		public void F0_1d12_7045_DrawWonderOrImprovement(int xSrc, int xDst, int yDst)
		{
			//this.CPU.Log.EnterBlock($"F0_1d12_7045({xPosSrc}, {xPosDst}, {yPosDst})");

			// function body
			if (xSrc < 22 || xSrc > 24)
			{
				xSrc--;

				if (xSrc >= 24)
				{
					xSrc -= 3;
				}

				if (xSrc >= 40)
				{
					// Instruction address 0x1d12:0x70c1, size: 5
					this.parent.Graphics.F0_VGA_07d8_DrawImage(this.parent.Var_19e8_Screen2_Rectangle, ((xSrc & 1) * 19) + 161, 100, 18, 10,
						this.parent.Var_aa_Screen0_Rectangle, xDst, yDst);
				}
				else
				{
					// Instruction address 0x1d12:0x70c1, size: 5
					this.parent.Graphics.F0_VGA_07d8_DrawImage(this.parent.Var_19e8_Screen2_Rectangle, ((xSrc / 5) * 19) + 161, ((xSrc % 5) * 10) + 50, 18, 10,
						this.parent.Var_aa_Screen0_Rectangle, xDst, yDst);
				}
			}
		}

		/// <summary>
		/// ?
		/// </summary>
		private void F0_1d12_72b7_DrawCityWorldMap()
		{
			//this.CPU.Log.EnterBlock("F0_1d12_72b7()");

			// function body
			// Instruction address 0x1d12:0x72d4, size: 5
			this.parent.DrawTools.FillRectangle(this.parent.Var_aa_Screen0_Rectangle, 101, 117, 120, 75, 0);

			for (int yPos = 0; yPos < 50; yPos++)
			{
				for (int xPos = 0; xPos < 80; xPos++)
				{
					// Instruction address 0x1d12:0x731c, size: 5
					int mapXPos = this.parent.MapManagement.AdjustXPosition(this.parent.GameData.Players[this.parent.GameData.HumanPlayerID].XStart + xPos - 40);
					int mapYPos = yPos;

					if ((this.parent.GameData.MapVisibility[mapXPos, mapYPos] & (1 << this.parent.GameData.HumanPlayerID)) != 0)
					{
						int offsetIndex = ((yPos & 1) * 2) + (xPos & 1);

						int rectXPos = 101 + ((xPos / 2) * 3) + cityMapOffsets[offsetIndex].X;
						int rectYPos = 118 + ((yPos / 2) * 3) + cityMapOffsets[offsetIndex].Y;

						// Instruction address 0x1d12:0x73a3, size: 5
						if (this.parent.MapManagement.GetTerrainType(mapXPos, mapYPos) != TerrainTypeEnum.Water)
						{
							// Instruction address 0x1d12:0x72f7, size: 5
							this.parent.DrawTools.FillRectangle(this.parent.Var_aa_Screen0_Rectangle, rectXPos, rectYPos, 2, 2, 2);
						}
						else
						{
							// Instruction address 0x1d12:0x72f7, size: 5
							this.parent.DrawTools.FillRectangle(this.parent.Var_aa_Screen0_Rectangle, rectXPos, rectYPos, 2, 2, 1);
						}
					}
				}
			}

			// Instruction address 0x1d12:0x73e1, size: 5
			this.parent.DrawTools.DrawRectangle(100, 117, 121, 75, 9);
		}

		/// <summary>
		/// Draws a rectangle that represents a city (2x2 pixels)
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="mode"></param>
		private void F0_1d12_73ea_DrawCityRectangle(int x, int y, ushort mode)
		{
			//this.CPU.Log.EnterBlock($"F0_1d12_73ea({xPos}, {yPos}, {mode})");

			// function body
			if (this.Var_2496 == 2)
			{
				// Instruction address 0x1d12:0x7405, size: 5
				x = this.parent.MapManagement.AdjustXPosition(
					x - this.parent.GameData.Players[this.parent.GameData.HumanPlayerID].XStart + 40);

				// Instruction address 0x1d12:0x7444, size: 5
				this.parent.DrawTools.FillRectangle(this.parent.Var_aa_Screen0_Rectangle,
					((x * 3) / 2) + 101, ((y * 3) / 2) + 118, 2, 2, mode);
			}
		}
	}
}
