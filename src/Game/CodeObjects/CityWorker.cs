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
			int local_2;
			int local_4;
			int local_6;
			int local_8;
			int local_a;
			int local_c;
			int[] Arr_3e = new int[21]; // 0x15 - 0x3e
			int local_40;
			int local_42 = 0;
			int local_44;
			int local_46;
			int local_48;
			int local_4a;
			int local_4c;
			int local_4e;
			int local_50;
			int[] Arr_74 = new int[18]; // 0x51 - 0x74
			int[,] Arr_a6 = new int[5, 5]; // 0x75 - 0xa6
			int local_a8;
			int local_ac;
			int local_ae;
			int local_b0 = 0;
			int local_b2;
			int local_b4;
			int local_b6 = 0;
			int local_b8 = 0;
			int local_ba;
			int local_bc;
			int local_be = 0;
			int local_c0;
			int local_c2;
			int local_c6;
			int local_c8;
			int local_ca = 0;
			int local_cc = 0;
			int local_ce;
			int local_d0;
			int local_d2;
			int local_d4;
			int local_d6;
			int local_d8;
			int local_da = 0;
			int local_de;
			int local_e0;
			int local_e2;
			int local_e4;
			int local_e6;
			int local_e8;
			int local_ea;
			uint local_ee_UInt;
			int local_f0;
			int local_f4;
			int local_f6;
			int local_f8;
			int local_fa;
			int local_fc;
			int local_fe = 0;
			int local_100;
			int local_102;
			int local_106;
			int local_108 = 0;
			int local_10a = 0;

			City city = this.parent.GameData.Cities[cityID];

			if (city.StatusFlag != 0xff)
			{
				this.Var_2496 = 0;

				for (int i = 0; i < 5; i++)
				{
					for (int j = 0; j < 5; j++)
					{
						Arr_a6[i, j] = 0;
					}
				}

				local_d8 = city.Position.X;
				local_e4 = city.Position.Y;

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
					if (this.parent.GameData.Cities[i].StatusFlag != 0xff && i != city.ID)
					{
						// Instruction address 0x1d12:0x01a2, size: 5
						local_b0 = this.parent.GameTools.F0_2dc4_0289_GetShortestDistance(
							city.Position.X,
							city.Position.Y,
							this.parent.GameData.Cities[i].Position.X,
							this.parent.GameData.Cities[i].Position.Y);

						if (this.parent.GameData.Cities[i].PlayerID == city.PlayerID &&
							this.parent.GameData.Cities[i].HasImprovement(ImprovementEnum.Palace) && local_b0 < this.Var_6b30)
						{
							this.Var_6b30 = local_b0;
						}

						if (local_b0 <= 5)
						{
							for (int j = 0; j < 21; j++)
							{
								if ((this.parent.GameData.Cities[i].WorkerFlags & (uint)(1 << j)) != 0 || j == 20)
								{
									int xPos = this.parent.GameData.Cities[i].Position.X + this.parent.CityOffsets[j].X;
									int yPos = this.parent.GameData.Cities[i].Position.Y + this.parent.CityOffsets[j].Y;

									if (Math.Abs(xPos - city.Position.X) <= 2 && Math.Abs(yPos - city.Position.Y) <= 2)
									{
										Arr_a6[xPos - city.Position.X + 2, yPos - city.Position.Y + 2] = 1;
									}
								}
							}
						}
					}
				}

				Arr_a6[2, 2] = 0;
				this.Var_6548_PlayerID = city.PlayerID;
				local_c2 = this.parent.Var_d4cc_MapViewX;
				local_d0 = this.parent.Var_d75e_MapViewY;

				if (this.parent.GameData.Players[this.Var_6548_PlayerID].GovernmentType <= 1)
				{
					local_48 = 1;
				}
				else
				{
					local_48 = 2;
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
			
				local_4a = 10;

				this.parent.Var_e8b8 = (int)(city.WorkerFlags >>> 26);

				if (this.Var_6548_PlayerID != this.parent.GameData.HumanPlayerID)
				{
					local_4a = -(this.parent.GameData.DifficultyLevel * 2 - 16);

					if (this.parent.GameData.Year >= 0 &&
						this.parent.GameData.Players[this.parent.GameData.HumanPlayerID].Ranking == 7 &&
						this.parent.GameData.DifficultyLevel > 1)
					{
						local_4a -= 2;
					}

					if (flag == 0)
					{
						if ((city.StatusFlag & 0x1) == 0)
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
				local_d8 = city.Position.X;
				local_e4 = city.Position.Y;

				if (flag == 1)
				{
					// Instruction address 0x1d12:0x049a, size: 5
					this.parent.UnitManagement.F0_1866_0006(cityID);

					// Instruction address 0x1d12:0x04ba, size: 5
					this.parent.DrawTools.FillRectangle(this.parent.Var_aa_Screen0_Rectangle, 0, 0, 320, 200, 0);

					// Instruction address 0x1d12:0x04d2, size: 5
					this.parent.DrawTools.DrawRectangleAndFillWithPattern(2, 1, 208, 21, 208, 100, 16, 16);

					local_e8 = 0;

					for (int i = 1; i <= city.ActualSize; i++)
					{
						local_e8 += i;
					}

					this.parent.Var_aa_Screen0_Rectangle.FontID = 2;

					// Instruction address 0x1d12:0x056f, size: 5
					this.parent.DrawTools.F0_1182_00b3_DrawCenteredStringToScreen0($"{this.parent.Segment_2459.F0_2459_08c6_GetCityName(cityID)} " +
						$"(Pop:{this.parent.GameTools.F0_2dc4_0337_PopulationValueToString(local_e8)})", 104, 2, 15);

					// Instruction address 0x1d12:0x0587, size: 5
					this.parent.DrawTools.DrawRectangleAndFillWithPattern(127, 23, 208, 104, 208, 100, 16, 16);

					this.parent.Var_d4cc_MapViewX = city.Position.X - 5;
					this.parent.Var_d75e_MapViewY = city.Position.Y - 3;

					// Instruction address 0x1d12:0x05cd, size: 5
					this.parent.MapManagement.F0_2aea_03ba_DrawCell(city.Position.X, city.Position.Y);

					for (int i = 0; i < this.Var_6540_CityOffsetCount; i++)
					{
						local_c6 = city.Position.X + this.parent.CityOffsets[i].X;
						local_d2 = city.Position.Y + this.parent.CityOffsets[i].Y;

						if (this.Var_6548_PlayerID != this.parent.GameData.HumanPlayerID ||
							(this.parent.GameData.MapVisibility[local_c6, local_d2] & (1 << this.Var_6548_PlayerID)) != 0)
						{
							// Instruction address 0x1d12:0x0664, size: 5
							local_e8 = this.parent.MapManagement.F0_2aea_14e0_GetCellUnitPlayerID(local_c6, local_d2);

							if (local_e8 != -1 && local_e8 != this.Var_6548_PlayerID)
							{
								// Instruction address 0x1d12:0x068e, size: 5
								this.parent.MapManagement.F0_2aea_11d4_DrawCellWithUnit(local_c6, local_d2);
							}
							else
							{
								// Instruction address 0x1d12:0x06a1, size: 5
								this.parent.MapManagement.F0_2aea_03ba_DrawCell(local_c6, local_d2);

								if (Arr_a6[this.parent.CityOffsets[i].X + 2, this.parent.CityOffsets[i].Y + 2] != 0)
								{
									// Instruction address 0x1d12:0x06f6, size: 5
									this.parent.DrawTools.DrawRectangle(
										(local_c6 - this.parent.Var_d4cc_MapViewX) * 16 + 80, (local_d2 - this.parent.Var_d75e_MapViewY) * 16 + 8, 15, 15, 12);
								}
							}
						}
					}
				}

				if (flag == 0)
				{
					city.StatusFlag &= 0x7f;

					if (city.FoodCount < 0)
					{
						local_e8 = -1;

						for (int i = 0; i < 128; i++)
						{
							if (this.parent.GameData.Players[this.Var_6548_PlayerID].Units[i].UnitType == UnitTypeEnum.Settler &&
								this.parent.GameData.Players[this.Var_6548_PlayerID].Units[i].HomeCityID == cityID)
							{
								local_e8 = i;
								break;
							}
						}

						if (this.Var_6548_PlayerID == this.parent.GameData.HumanPlayerID)
						{
							// Instruction address 0x1d12:0x07dd, size: 5
							this.parent.CommonTools.PlayTune(36, 0);

							this.parent.News.F21_0000_0000_ShowNews(cityID,
								$"Food storage exhausted\nin {this.parent.Segment_2459.F0_2459_08c6_GetCityName(cityID)}!\n{((local_e8 != -1) ? "Settlers lost." : "Famine feared.")}\n");

							// Instruction address 0x1d12:0x07f4, size: 5
							this.parent.CommonTools.PlayTune(1, 0);
						}

						if (local_e8 != -1)
						{
							// Instruction address 0x1d12:0x080e, size: 5
							this.parent.UnitManagement.F0_1866_0f10_DeleteUnit(this.Var_6548_PlayerID, (short)local_e8);
						}
						else
						{
							city.ActualSize--;

							if (city.ActualSize <= 0)
							{
								// Instruction address 0x1d12:0x0843, size: 5
								this.parent.Segment_1ade.F0_1ade_018e(cityID, city.Position.X, city.Position.Y);

								this.parent.StartGameMenu.F5_0000_0e6c_TestIfAIPlayerDestroyed(this.Var_6548_PlayerID, 0);

								goto L6927;
							}
						}

						city.FoodCount = 0;

						// Instruction address 0x1d12:0x0879, size: 5
						this.parent.CheckPlayerTurn.F0_1403_3ed7(local_d8, local_e4);
					}

					if ((city.ActualSize + 1) * local_4a <= city.FoodCount)
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
						this.parent.CheckPlayerTurn.F0_1403_3ed7(local_d8, local_e4);
					}
				}
			
				local_50 = city.ActualSize + 1;

				this.Var_6546 = 0;
				this.parent.Var_deb8 = 0;
				this.parent.Var_d2f6 = 0;
				this.parent.Var_e3c6 = 0;

				for (int i = 0; i < 2; i++)
				{
					if (city.Unknown[i] != -1)
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
								if (this.parent.GameData.Players[this.Var_6548_PlayerID].GovernmentType > 1 && (this.parent.GameData.DebugFlags & 2) != 0)
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

						if (this.parent.GameData.Players[this.Var_6548_PlayerID].Units[i].UnitType == 0)
						{
							this.parent.Var_e3c6++;
						}
					}
				}

				this.Var_70e8 = 0;

				for (int i = 0; i < this.Var_6540_CityOffsetCount; i++)
				{
					Arr_3e[i] = 0;

					local_c6 = city.Position.X + this.parent.CityOffsets[i].X;
					local_d2 = city.Position.Y + this.parent.CityOffsets[i].Y;

					if (this.Var_6548_PlayerID != 0 && (this.parent.GameData.MapVisibility[local_c6, local_d2] & (1 << this.Var_6548_PlayerID)) == 0)
					{
						Arr_3e[i] = 1;
					}

					// Instruction address 0x1d12:0x0bf4, size: 5
					local_e8 = this.parent.MapManagement.F0_2aea_14e0_GetCellUnitPlayerID(local_c6, local_d2);

					if (local_e8 != -1 && local_e8 != this.Var_6548_PlayerID)
					{
						Arr_a6[this.parent.CityOffsets[i].X + 2, this.parent.CityOffsets[i].Y + 2] = 1;
						Arr_3e[i] = 1;

						if (local_e8 == this.parent.GameData.HumanPlayerID)
						{
							this.Var_70e8 = 1;
						}
					}

					if (Arr_a6[this.parent.CityOffsets[i].X + 2, this.parent.CityOffsets[i].Y + 2] != 0)
					{
						Arr_3e[i] = 1;
					}

					if (Arr_3e[i] != 0)
					{
						city.WorkerFlags &= (uint)(~(1 << i));
					}
				}

				for (int i = 0; i < 4; i++)
				{
					this.parent.Var_70da_Arr[i] = 0;
				}

				local_ee_UInt = 0;

				if (((ushort)this.parent.GameData.PlayerFlags & (1 << this.Var_6548_PlayerID)) != 0 ||
					((cityID + this.parent.GameData.TurnCount) & 0x3) != 0 ||
					(city.StatusFlag & 0x1) != 0 || this.Var_70e8 != 0)
				{
					for (int i = 0; i < this.Var_6540_CityOffsetCount; i++)
					{
						if ((city.WorkerFlags & (1 << i)) != 0)
						{
							local_50--;
						}
					}

					if (local_50 >= 0)
					{
						local_50 = city.ActualSize + 1;

						for (int i = 0; i < this.Var_6540_CityOffsetCount; i++)
						{
							if (local_50 != 0 && (city.WorkerFlags & (1 << i)) != 0)
							{
								Arr_3e[i] = 1;

								// Instruction address 0x1d12:0x0ddf, size: 5
								F0_1d12_692d_CityResources(cityID, i, flag);

								local_50--;

								local_ee_UInt |= (uint)(1 << i);
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

						local_50 = city.ActualSize + 1;
					}

					if (local_50 == this.parent.Var_e8b8 || local_50 == 0)
						goto L1292;
				}

				if (local_50 > 0 && Arr_3e[20] == 0)
				{
					Arr_3e[20] = 1;

					// Instruction address 0x1d12:0x0eb2, size: 5
					F0_1d12_692d_CityResources(cityID, 20, flag);

					local_ee_UInt |= 0x100000;
					local_50--;
				}

				while ((((city.ActualSize * 2) + (local_48 * this.parent.Var_e3c6)) > ((local_50 / 2) + this.parent.Var_70da_Arr[0]) || city.ActualSize < 3) &&
					local_50 > this.parent.Var_e8b8 && local_108 != -1)
				{
					local_108 = -1;
					local_d6 = 0;
					local_ce = 0;

					for (int i = 0; i < this.Var_6540_CityOffsetCount; i++)
					{
						if (Arr_3e[i] == 0)
						{
							local_c6 = city.Position.X + this.parent.CityOffsets[i].X;
							local_d2 = city.Position.Y + this.parent.CityOffsets[i].Y;

							// Instruction address 0x1d12:0x0f9e, size: 5
							local_cc = F0_1d12_6abc_GetCityResourceCount(this.Var_6548_PlayerID, this.Var_653e_CityID, local_c6, local_d2, CityResourceTypeEnum.Food);

							// Instruction address 0x1d12:0x0fb6, size: 5
							local_d4 = F0_1d12_6abc_GetCityResourceCount(this.Var_6548_PlayerID, this.Var_653e_CityID, local_c6, local_d2, CityResourceTypeEnum.Production) * 2;

							if (local_50 != 1 || this.parent.Var_70da_Arr[1] != 0 || local_d4 != 0)
							{
								// Instruction address 0x1d12:0x0ff0, size: 5
								local_d4 += F0_1d12_6abc_GetCityResourceCount(this.Var_6548_PlayerID, this.Var_653e_CityID, local_c6, local_d2, CityResourceTypeEnum.Trade);

								if (local_cc > local_ce || (local_cc == local_ce && local_d4 > local_d6))
								{
									local_ce = local_cc;
									local_d6 = local_d4;
									local_108 = i;
								}
							}
						}
					}

					if (local_108 != -1)
						break;

					Arr_3e[local_108] = 1;

					// Instruction address 0x1d12:0x105d, size: 5
					F0_1d12_692d_CityResources(cityID, local_108, flag);

					local_ee_UInt |= (uint)(1 << local_108);
					local_50--;
				}

				if (this.Var_6548_PlayerID != this.parent.GameData.HumanPlayerID && (city.StatusFlag & 0x1) != 0)
				{
					this.parent.Var_e8b8++;
				}

				while (local_50 > this.parent.Var_e8b8)
				{
					local_108 = -1;
					local_e2 = 0;
					local_d6 = 0;
					local_ce = 0;

					for (int i = 0; i < this.Var_6540_CityOffsetCount; i++)
					{
						if (Arr_3e[i] == 0)
						{
							local_c6 = city.Position.X + this.parent.CityOffsets[i].X;
							local_d2 = city.Position.Y + this.parent.CityOffsets[i].Y;

							// Instruction address 0x1d12:0x1173, size: 5
							local_4 = F0_1d12_6abc_GetCityResourceCount(this.Var_6548_PlayerID, this.Var_653e_CityID, local_c6, local_d2, CityResourceTypeEnum.Food);

							// Instruction address 0x1d12:0x1151, size: 5
							local_d4 = local_4 * (16 / this.parent.GameTools.F0_2dc4_007c_CheckValueRange(
								this.parent.Var_70da_Arr[0] - (city.ActualSize * 2) - (local_48 * this.parent.Var_e3c6), 1, 99));

							// Instruction address 0x1d12:0x1190, size: 5
							local_c = F0_1d12_6abc_GetCityResourceCount(this.Var_6548_PlayerID, this.Var_653e_CityID, local_c6, local_d2, CityResourceTypeEnum.Production);

							// Instruction address 0x1d12:0x11af, size: 5
							local_d4 += ((city.ActualSize * 3) / this.parent.GameTools.F0_2dc4_007c_CheckValueRange(
								this.parent.Var_70da_Arr[1] - this.parent.Var_d2f6, 1, 99)) * local_c;

							// Instruction address 0x1d12:0x11dc, size: 5
							local_46 = F0_1d12_6abc_GetCityResourceCount(this.Var_6548_PlayerID, this.Var_653e_CityID, local_c6, local_d2, CityResourceTypeEnum.Trade);

							// Instruction address 0x1d12:0x11f7, size: 5
							local_d4 += ((city.ActualSize * 2) / this.parent.GameTools.F0_2dc4_007c_CheckValueRange(this.parent.Var_70da_Arr[2], 1, 99)) * local_46;

							if (local_d4 > local_d6)
							{
								local_d6 = local_d4;
								local_108 = i;
								local_e2 = ((local_4 + local_c) * 2) + local_46;
							}
						}
					}

					if (local_108 == -1)
						break;

					Arr_3e[local_108] = 1;

					// Instruction address 0x1d12:0x1267, size: 5
					F0_1d12_692d_CityResources(cityID, local_108, flag);

					local_ee_UInt |= (uint)(1 << local_108);
					local_50--;
				}

			L1292:
				city.WorkerFlags = (uint)((uint)local_50 << 26) | local_ee_UInt;

				local_ae = -local_50;

			L12c2:
				local_d8 = city.Position.X;
				local_e4 = city.Position.Y;
				local_8 = local_50;

				for (int i = 0; i < local_8; i++)
				{
					// Instruction address 0x1d12:0x1309, size: 5
					if (F0_1d12_6da1_GetSpecialWorkerFlags(i) == 0)
					{
						// Instruction address 0x1d12:0x1321, size: 5
						F0_1d12_6d6e_SetSpecialWorkerFlags(i, 3);
					}
				}

				for (int i = local_8; i < 8; i++)
				{
					// Instruction address 0x1d12:0x134c, size: 5
					F0_1d12_6d6e_SetSpecialWorkerFlags(i, 0);
				}

				local_ac = 0;
				local_f8 = 0;
				this.parent.Var_6c98 = 1;

				if (city.HasImprovement(ImprovementEnum.Factory))
				{
					local_f8 = 2;
				}

				if (city.HasImprovement(ImprovementEnum.ManufacturingPlant))
				{
					local_f8 = 4;
				}

				if (city.HasImprovement(ImprovementEnum.PowerPlant))
				{
					local_ac = 2;
				}

				if (city.HasImprovement(ImprovementEnum.HydroPlant))
				{
					this.parent.Var_6c98 = 2;
					local_ac = 2;
				}

				if (city.HasImprovement(ImprovementEnum.NuclearPlant))
				{
					this.parent.Var_6c98 = 2;
					local_ac = 2;
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
						local_ac = 2;
					}
				}

				if (city.HasImprovement(ImprovementEnum.RecyclingCenter))
				{
					this.parent.Var_6c98 = 3;
				}

				// Instruction address 0x1d12:0x1484, size: 5
				local_ac = this.parent.GameTools.F0_2dc4_007c_CheckValueRange(local_ac, 0, local_f8);
				local_10a = this.parent.Var_70da_Arr[1];
				local_a = this.parent.Var_70da_Arr[2];

				this.parent.Var_70da_Arr[1] += ((local_f8 * this.parent.Var_70da_Arr[1]) / 4) + ((local_ac * this.parent.Var_70da_Arr[1]) / 4);

				if (flag == 0)
				{
					city.FoodCount += (short)(this.parent.Var_70da_Arr[0] - (local_48 * this.parent.Var_e3c6) - (city.ActualSize * 2));

					local_e8 = this.parent.Var_70da_Arr[1];
					local_e8 -= this.parent.Var_d2f6;

					if ((city.StatusFlag & 0x1) != 0)
					{
						local_e8 = 0;
					}

					city.ShieldsCount += (short)local_e8;

					if (city.CurrentProductionID >= 0)
					{
						if ((this.parent.GameData.Units[city.CurrentProductionID].Cost * local_4a) <= city.ShieldsCount)
						{
							if (city.CurrentProductionID == 0 && city.ActualSize == 1 && this.parent.GameData.DifficultyLevel == 0)
							{
								goto L2307;
							}
							else
							{
								city.ShieldsCount -= (short)(this.parent.GameData.Units[city.CurrentProductionID].Cost * local_4a);

								local_ba = -1;

								if (this.Var_6548_PlayerID == this.parent.GameData.HumanPlayerID || city.CurrentProductionID != (int)UnitTypeEnum.Diplomat)
								{
									// Instruction address 0x1d12:0x15fa, size: 5
									local_ba = this.parent.UnitManagement.F0_1866_0cf5_CreateUnit(this.Var_6548_PlayerID, (UnitTypeEnum)city.CurrentProductionID, city.Position.X, city.Position.Y);
								}

								if ((this.parent.GameData.TechnologyFirstDiscoveredBy[city.CurrentProductionID] & 0x8) == 0)
								{
									// Instruction address 0x1d12:0x1640, size: 5
									this.parent.UnitManagement.F0_1866_250e_AddReplayData(6, (byte)((sbyte)this.Var_6548_PlayerID), (byte)city.CurrentProductionID);

									this.parent.GameData.TechnologyFirstDiscoveredBy[city.CurrentProductionID] |= 8;
								}

								if (city.HasImprovement(ImprovementEnum.Barracks) && local_ba != -1)
								{
									this.parent.GameData.Players[this.Var_6548_PlayerID].Units[local_ba].Status |= 0x20;
								}

								if (local_ba != -1 && city.CurrentProductionID == 0 && (city.ActualSize > 1 || this.parent.GameData.Players[this.Var_6548_PlayerID].CityCount > 1))
								{
									city.ActualSize--;

									if (city.ActualSize == 0)
									{
										// Instruction address 0x1d12:0x16ff, size: 5
										this.parent.Segment_1ade.F0_1ade_018e(cityID, city.Position.X, city.Position.Y);

										// Instruction address 0x1d12:0x1727, size: 5
										this.parent.UnitManagement.F0_1866_0cf5_CreateUnit(this.Var_6548_PlayerID, (UnitTypeEnum)city.CurrentProductionID, city.Position.X, city.Position.Y);

										this.parent.StartGameMenu.F5_0000_0e6c_TestIfAIPlayerDestroyed(this.Var_6548_PlayerID, 0);

										goto L6927;
									}
								}

								if (this.Var_6548_PlayerID != this.parent.GameData.HumanPlayerID && local_ba != -1)
								{
									if (city.CurrentProductionID == 27)
									{
										local_108 = -1;
										local_c8 = -1;

										for (int i = 0; i < 128; i++)
										{
											if (this.parent.GameData.Cities[i].PlayerID != this.Var_6548_PlayerID && this.parent.GameData.Cities[i].StatusFlag != 0xff &&
												this.parent.GameData.Cities[i].PlayerID != this.parent.GameData.HumanPlayerID && this.parent.GameData.Cities[i].BaseTrade > local_c8)
											{
												local_c8 = this.parent.GameData.Cities[i].BaseTrade;
												local_108 = i;
											}
										}

										// Instruction address 0x1d12:0x180e, size: 5
										this.parent.Segment_2459.F0_2459_0948(this.Var_6548_PlayerID, local_ba, local_108);
									}

									if (city.CurrentProductionID == 26)
									{
										local_108 = -1;
										local_c8 = 32767;
										local_106 = 0;

										for (int i = 0; i < 128; i++)
										{
											if (this.parent.GameData.Cities[i].PlayerID != this.parent.GameData.HumanPlayerID || this.parent.GameData.Cities[i].StatusFlag != 0xff)
											{
												// Instruction address 0x1d12:0x1898, size: 5
												local_e8 = this.parent.GameTools.F0_2dc4_0177_FindNearestPlayerUnit(this.Var_6548_PlayerID, local_ba,
													this.parent.GameData.Cities[i].Position.X,
													this.parent.GameData.Cities[i].Position.Y);

												int unitDistance = Math.Abs(this.parent.GameTools.F0_2dc4_0289_GetShortestDistance(this.parent.GameData.Cities[i].Position, 
													this.parent.GameData.Players[this.Var_6548_PlayerID].Units[local_e8].Position) - 3);

												if ((this.parent.GameData.Cities[i].StatusFlag & 0x20) != 0)
												{
													unitDistance = 3;
												}

												if (local_c8 > unitDistance || (local_c8 == unitDistance && this.parent.CAPI.RNG.Next(++local_106) == 0))
												{
													// Instruction address 0x1d12:0x191e, size: 5
													TerrainTypeEnum terrainType = this.parent.MapManagement.GetTerrainType(
														this.parent.GameData.Players[this.Var_6548_PlayerID].Units[local_e8].Position.X,
														this.parent.GameData.Players[this.Var_6548_PlayerID].Units[local_e8].Position.Y);

													// Instruction address 0x1d12:0x1956, size: 5
													// Instruction address 0x1d12:0x1970, size: 5
													if (terrainType != TerrainTypeEnum.Water && this.parent.MapManagement.F0_2aea_1942_GetGroupID(
														this.parent.GameData.Cities[i].Position.X,	this.parent.GameData.Cities[i].Position.Y) ==
														this.parent.MapManagement.F0_2aea_1942_GetGroupID(
														this.parent.GameData.Players[this.Var_6548_PlayerID].Units[local_e8].Position.X,
														this.parent.GameData.Players[this.Var_6548_PlayerID].Units[local_e8].Position.Y))
													{
														if (local_c8 != unitDistance)
														{
															local_c8 = unitDistance;
														}
														else
														{
															local_106 = 1;
														}

														local_108 = local_e8;
														local_b0 = i;
													}
												}
											}
										}

										if (local_c8 > 3 || this.parent.GameData.Players[this.Var_6548_PlayerID].Diplomacy[this.parent.GameData.HumanPlayerID].HasFlag(DiplomacyFlagsEnum.Peace))
										{
											city.ShieldsCount += (short)(this.parent.GameData.Units[city.CurrentProductionID].Cost * local_4a);
										}
										else
										{
											// Instruction address 0x1d12:0x19f9, size: 5
											local_ba = this.parent.UnitManagement.F0_1866_0cf5_CreateUnit(
												this.Var_6548_PlayerID, UnitTypeEnum.Diplomat,
												this.parent.GameData.Players[this.Var_6548_PlayerID].Units[local_108].Position.X,
												this.parent.GameData.Players[this.Var_6548_PlayerID].Units[local_108].Position.Y);

											if (local_ba != -1)
											{
												this.parent.GameData.Players[this.Var_6548_PlayerID].Units[local_ba].GoToDestination.X = this.parent.GameData.Cities[local_b0].Position.X;
												this.parent.GameData.Players[this.Var_6548_PlayerID].Units[local_ba].GoToDestination.Y = this.parent.GameData.Cities[local_b0].Position.Y;
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

									if (local_ba != -1 && (city.CurrentProductionID == 0 || city.CurrentProductionID >= 26))
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
						if ((this.parent.GameData.GetImprovementType(-city.CurrentProductionID).Cost * local_4a) <= city.ShieldsCount)
						{
							local_e8 = -city.CurrentProductionID;

							if (local_e8 > 24)
							{
								if (this.parent.GameData.WonderCityID[local_e8 - 24] == -1)
								{
									this.parent.GameData.WonderCityID[local_e8 - 24] = (short)cityID;

									// Instruction address 0x1d12:0x1bd7, size: 5
									this.parent.UnitManagement.F0_1866_250e_AddReplayData(10, (byte)((sbyte)this.Var_6548_PlayerID), (byte)((sbyte)(local_e8 - 24)));
								}
								else
								{
									local_e8 = -1;
								}
							}
							else if (city.HasImprovement(City.BitToImprovementEnum(local_e8 - 1)))
							{
								local_e8 = -1;
							}

							if (local_e8 != -1)
							{
								if (local_e8 == 41)
								{
									this.parent.CommonTools.PlayTune(36, 0);

									this.parent.News.F21_0000_0000_ShowNews(-1,
										$"Sensors report a\nNUCLEAR WEAPONS test\nnear {this.parent.Segment_2459.F0_2459_08c6_GetCityName(cityID)}!\n");

									this.parent.CommonTools.PlayTune(1, 0);
								}

								if (local_e8 < 22)
								{
									city.AddImprovement(City.BitToImprovementEnum(local_e8 - 1));
								}

								city.ShieldsCount -= (short)(this.parent.GameData.GetImprovementType(-city.CurrentProductionID).Cost * local_4a);

								if ((this.parent.GameData.PlayerFlags & (1 << this.Var_6548_PlayerID)) != 0 || local_e8 > 24)
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
										(local_e8 <= 21 || local_e8 > 24) && (city.StatusFlag & 0x10) == 0 && local_e8 != 1)
									{
										this.parent.CityView.F19_0000_0000_ShowCityLayout(cityID, local_e8,
											$"{this.parent.Segment_2459.F0_2459_08c6_GetCityName(cityID)} builds\n{this.parent.GameData.GetImprovementType(local_e8).Name}.\n");
									}
									else
									{
										this.parent.News.F21_0000_0000_ShowNews(cityID,
											$"{this.parent.Segment_2459.F0_2459_08c6_GetCityName(cityID)} builds\n{this.parent.GameData.GetImprovementType(local_e8).Name}.\n");
									}

									city.ShieldsCount = 0;
								}
								else
								{
									if (local_e8 == 1)
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

								if (local_e8 >= 22 && local_e8 <= 24 && ((1 << this.Var_6548_PlayerID) & (ushort)this.parent.GameData.SpaceshipFlags) == 0)
								{
									if (this.Var_6548_PlayerID == this.parent.GameData.HumanPlayerID)
									{
										this.parent.Overlay_18.F18_0000_0f83(this.Var_6548_PlayerID, (ushort)((short)(local_e8 - 22)));
									}
									else
									{
										local_c8 = (short)this.parent.Overlay_18.F18_0000_0d4f(this.Var_6548_PlayerID, (short)(local_e8 - 22));

										if (local_c8 != 0 && local_e8 < 24)
										{
											city.ShieldsCount += (short)(this.parent.GameData.GetImprovementType(local_e8).Cost * local_4a);

											if (city.CurrentProductionID <= -22 && city.CurrentProductionID >= -24)
											{
												city.CurrentProductionID = (sbyte)((-local_e8) - 1);
											}

											local_c8--;
										}

										if ((this.parent.GameData.SpaceshipFlags & (1 << this.Var_6548_PlayerID)) == 0 && this.parent.GameData.AISpaceshipSuccessRate >= 40)
										{
											if (local_c8 == 0)
												goto L1feb;

											// Instruction address 0x1d12:0x1f5d, size: 5
											if (!this.parent.Segment_1ade.F0_1ade_22b5_PlayerHasTechnology(this.parent.GameData.HumanPlayerID, TechnologyAdvanceEnum.SpaceFlight))
												goto L2010;

											if (this.parent.GameData.AISpaceshipSuccessRate > 75)
												goto L1feb;

											if (this.parent.GameData.Players[this.parent.GameData.HumanPlayerID].Ranking > this.parent.GameData.Players[this.Var_6548_PlayerID].Ranking)
												goto L1feb;

											if ((this.parent.GameData.SpaceshipFlags & (1 << this.parent.GameData.HumanPlayerID)) == 0)
												goto L1fc5;

											if ((this.parent.GameData.Players[this.Var_6548_PlayerID].SpaceshipETAYear - this.parent.GameData.Year) <=
												(this.parent.GameData.Players[this.parent.GameData.HumanPlayerID].SpaceshipETAYear - this.parent.GameData.Year))
												goto L1feb;

											L1fc5:
											if ((this.parent.GameData.SpaceshipFlags & (1 << (this.parent.GameData.HumanPlayerID + 8))) != 0 &&
												this.parent.GameData.Players[this.parent.GameData.ActiveCivilizations].Coins > 1000)
												goto L1feb;

											goto L2010;

										L1feb:
											this.parent.Overlay_18.F18_0000_15c3(this.Var_6548_PlayerID);

											goto L2010;
										}
										else if (local_c8 != 0)
										{
											this.parent.Overlay_18.F18_0000_15c3(this.Var_6548_PlayerID);
										}
									}
								}

							L2010:
								if (local_e8 == 1)
								{
									for (int i = 0; i < 128; i++)
									{
										if (this.parent.GameData.Cities[i].PlayerID == this.Var_6548_PlayerID)
										{
											this.parent.GameData.Cities[i].RemoveImprovement(ImprovementEnum.Palace);
										}
									}

									if (this.Var_6548_PlayerID == this.parent.GameData.HumanPlayerID ||
										this.parent.GameData.Players[this.parent.GameData.HumanPlayerID].Diplomacy[this.Var_6548_PlayerID].HasFlag(DiplomacyFlagsEnum.Unknown40))
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

								if (local_e8 == 38)
								{
									// Instruction address 0x1d12:0x2143, size: 5
									this.parent.Segment_1ade.F0_1ade_1584(this.Var_6548_PlayerID, 0);

									// Instruction address 0x1d12:0x2153, size: 5
									this.parent.Segment_1ade.F0_1ade_1584(this.Var_6548_PlayerID, 0);
								}

								if (local_e8 == 43 && this.Var_6548_PlayerID == this.parent.GameData.HumanPlayerID)
								{
									for (int i = 0; i < 128; i++)
									{
										if (this.parent.GameData.Cities[i].StatusFlag != 0xff && this.parent.GameData.Cities[i].PlayerID != this.Var_6548_PlayerID)
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

					if ((this.Var_6548_PlayerID == this.parent.GameData.HumanPlayerID && (city.StatusFlag & 0x10) != 0) &&
						(city.ShieldsCount == 0 || (city.CurrentProductionID < 0 && local_e8 == -1)))
					{
						// Instruction address 0x1d12:0x2262, size: 5
						local_e8 = (short)this.parent.Segment_1ade.F0_1ade_0421(this.Var_6548_PlayerID, cityID);

						this.parent.Var_aa_Screen0_Rectangle.FontID = 2;

						if (local_e8 == 99)
						{
							city.StatusFlag &= 0xef;
						}
						else
						{
							this.parent.Var_b1e8 = 0;

							if (city.CurrentProductionID >= 0)
							{
								this.parent.GameData.Players[this.Var_6548_PlayerID].UnitsInProduction[city.CurrentProductionID]--;
							}

							city.CurrentProductionID = (sbyte)local_e8;

							if (city.CurrentProductionID >= 0)
							{
								this.parent.GameData.Players[this.Var_6548_PlayerID].UnitsInProduction[city.CurrentProductionID]++;
							}
						}
					}

				L2307:
					if ((this.parent.GameData.PlayerFlags & (1 << this.Var_6548_PlayerID)) == 0)
					{
						if ((city.StatusFlag & 0x10) != 0)
						{
							// Instruction address 0x1d12:0x232e, size: 5
							this.parent.AIEngine.F0_25fb_34b6_ChangeCityProduction(cityID);
						}

						local_cc = 0;

						// Instruction address 0x1d12:0x2344, size: 5
						PlayerContinentStrategyEnum local_104 = this.parent.GameData.Players[this.Var_6548_PlayerID].Continents[this.parent.MapManagement.F0_2aea_1942_GetGroupID(local_d8, local_e4)].Strategy;

						if ((local_104 == PlayerContinentStrategyEnum.Attack || local_104 == PlayerContinentStrategyEnum.Defend || local_104 == PlayerContinentStrategyEnum.Transport) && 
							local_e8 != 0 && city.CurrentProductionID >= 0 &&
							(int)this.parent.GameData.Units[city.CurrentProductionID].UnitRoleType == (int)local_104)
						{
							local_cc = this.parent.GameData.Players[this.Var_6548_PlayerID].Coins / 64;
						}

						if ((this.parent.GameData.SpaceshipFlags & (1 << (this.parent.GameData.HumanPlayerID + 8))) != 0 &&
							(-city.CurrentProductionID) >= 0x16 && (-city.CurrentProductionID) <= 0x18)
						{
							local_cc = this.parent.GameData.Players[this.Var_6548_PlayerID].Coins / 128;
						}

						if ((city.StatusFlag & 0x1) != 0 && city.CurrentProductionID < 0 && city.ShieldsCount != 0)
						{
							// Instruction address 0x1d12:0x24a7, size: 5
							local_cc = this.parent.GameTools.F0_2dc4_007c_CheckValueRange(
								(this.parent.GameData.GetImprovementType(-city.CurrentProductionID).Cost * local_4a) - city.ShieldsCount,
								0, this.parent.GameData.Players[this.Var_6548_PlayerID].Coins / 8);
						}

						// Instruction address 0x1d12:0x24bb, size: 5
						if ((this.parent.MapManagement.F0_2aea_14e0_GetCellUnitPlayerID(local_d8, local_e4) == -1 || 
							(city.StatusFlag & 0x10) != 0) && city.CurrentProductionID >= 0 && city.ShieldsCount != 0)
						{
							// Instruction address 0x1d12:0x2530, size: 5
							local_cc = this.parent.GameTools.F0_2dc4_007c_CheckValueRange(
								(this.parent.GameData.Units[city.CurrentProductionID].Cost * local_4a) - city.ShieldsCount,
								0, this.parent.GameData.Players[this.Var_6548_PlayerID].Coins / 3);
						}

						if (city.CurrentProductionID == -1 && city.ShieldsCount != 0)
						{
							// Instruction address 0x1d12:0x2591, size: 5
							local_cc = this.parent.GameTools.F0_2dc4_007c_CheckValueRange(
								(this.parent.GameData.Units[city.CurrentProductionID].Cost * local_4a) - city.ShieldsCount,
								0, this.parent.GameData.Players[this.Var_6548_PlayerID].Coins / 3);
						}

						if (city.CurrentProductionID >= 0 && city.StatusFlag == 0x19 && this.parent.GameData.Players[this.Var_6548_PlayerID].ActiveUnits[(int)UnitTypeEnum.Nuclear] == 0 && city.ShieldsCount != 0)
						{
							// Instruction address 0x1d12:0x260d, size: 5
							local_cc = this.parent.GameTools.F0_2dc4_007c_CheckValueRange(
								(this.parent.GameData.Units[city.CurrentProductionID].Cost * local_4a) - city.ShieldsCount,
								0, this.parent.GameData.Players[this.Var_6548_PlayerID].Coins / 4);
						}

						if (this.parent.GameData.Players[this.Var_6548_PlayerID].Coins > 2000)
						{
							local_cc += this.parent.GameData.Players[this.Var_6548_PlayerID].Coins / 512;
						}

						city.ShieldsCount += (short)local_cc;
						this.parent.GameData.Players[this.Var_6548_PlayerID].Coins -= (short)(local_cc * 2);
						city.StatusFlag &= 0xef;
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
			
				local_d8 = city.Position.X;
				local_e4 = city.Position.Y;

				this.parent.Var_d2e0 = 0;

				if (this.parent.GameData.Players[this.Var_6548_PlayerID].GovernmentType == 3)
				{
					this.Var_6b30 = 10;
				}

				this.parent.Var_d2e0 = ((this.parent.Var_70da_Arr[2] * this.Var_6b30) * 3) / ((this.parent.GameData.Players[this.Var_6548_PlayerID].GovernmentType * 20) + 80);

				if (city.HasImprovement(ImprovementEnum.Courthouse) || city.HasImprovement(ImprovementEnum.Palace))
				{
					this.parent.Var_d2e0 /= 2;
				}
			
				if (this.parent.GameData.Players[this.Var_6548_PlayerID].GovernmentType == 5)
				{
					this.parent.Var_d2e0 = 0;
				}

				city.BaseTrade = (sbyte)(this.parent.Var_70da_Arr[2] - this.parent.Var_d2e0);

				for (int i = 0; i < 3; i++)
				{
					local_4c = city.TradeCityIDs[i];

					if (local_4c != -1)
					{
						if (this.parent.GameData.Cities[local_4c].PlayerID == this.Var_6548_PlayerID)
						{
							this.parent.Var_70da_Arr[2] += (this.parent.GameData.Cities[local_4c].BaseTrade + this.parent.Var_70da_Arr[2] + 4) / 16;
						}
						else
						{
							this.parent.Var_70da_Arr[2] += (this.parent.GameData.Cities[local_4c].BaseTrade + this.parent.Var_70da_Arr[2] + 4) / 8;
						}
					}
				}

				this.parent.Var_d2e0 = ((this.parent.Var_70da_Arr[2] * this.Var_6b30) * 3) / ((this.parent.GameData.Players[this.Var_6548_PlayerID].GovernmentType * 20) + 80);

				if (this.parent.GameData.Players[this.Var_6548_PlayerID].GovernmentType == 5)
				{
					this.parent.Var_d2e0 = 0;
				}

				if (city.HasImprovement(ImprovementEnum.Courthouse))
				{
					this.parent.Var_d2e0 /= 2;
				}

				// Instruction address 0x1d12:0x2960, size: 5
				this.parent.Var_70da_Arr[3] = this.parent.GameTools.F0_2dc4_007c_CheckValueRange(
					((-(this.parent.GameData.Players[this.Var_6548_PlayerID].ScienceTaxRate +
						this.parent.GameData.Players[this.Var_6548_PlayerID].TaxRate - 10) *
						(this.parent.Var_70da_Arr[2] - this.parent.Var_d2e0)) + 5) / 10,
					0, this.parent.Var_70da_Arr[2]);

				// !!! Minimum value is greater than maximum value, why?
				// Instruction address 0x1d12:0x2999, size: 5
				this.parent.Var_e17a = this.parent.GameTools.F0_2dc4_007c_CheckValueRange(
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
			
				local_e8 = this.parent.Var_70e6;

				if (city.HasImprovement(ImprovementEnum.Library))
				{
					local_e8 += this.parent.Var_70e6 / 2;

					// Instruction address 0x1d12:0x2a6e, size: 5
					if (F0_1d12_6c97_PlayerHasWonder(this.Var_6548_PlayerID, WonderEnum.IsaacNewtonsCollege))
					{
						local_e8 += this.parent.Var_70e6 / 3;
					}
				}

				if (city.HasImprovement(ImprovementEnum.University))
				{
					local_e8 += this.parent.Var_70e6 / 2;

					// Instruction address 0x1d12:0x2ab2, size: 5
					if (F0_1d12_6c97_PlayerHasWonder(this.Var_6548_PlayerID, WonderEnum.IsaacNewtonsCollege))
					{
						local_e8 += this.parent.Var_70e6 / 3;
					}
				}

				// Instruction address 0x1d12:0x2ad3, size: 5
				if (F0_1d12_6cf3_GetWonderCityID((int)WonderEnum.CopernicusObservatory) == cityID)
				{
					local_e8 += local_e8;
				}
			
				this.parent.Var_70e6 = local_e8;
				this.parent.Var_70e2 = 0;

				if (this.Var_6548_PlayerID == this.parent.GameData.HumanPlayerID)
				{
					local_e8 = 14 - (parent.GameData.DifficultyLevel * 2);
					local_e8 = ((this.parent.GameData.Players[this.Var_6548_PlayerID].GovernmentType / 2) + 2) * (local_e8 / 2);
					local_a8 = (((cityID % local_e8) + this.parent.GameData.Players[this.Var_6548_PlayerID].CityCount - local_e8) / local_e8) +
						city.ActualSize + this.parent.GameData.DifficultyLevel - 6;
					this.parent.Var_70e4 = local_a8;
				}
				else
				{
					local_a8 = city.ActualSize - 3;
					this.parent.Var_70e4 = local_a8;
				}

				this.Var_6542 = 0;

				if (city.ActualSize < this.parent.Var_70e4)
				{
					this.Var_6542 = this.parent.Var_70e4 - city.ActualSize;
					this.parent.Var_70e4 = city.ActualSize;
				}
			
				// Instruction address 0x1d12:0x2bd9, size: 5
				F0_1d12_6dfe(cityID, local_8);

				if (flag == 1 && this.Var_2496 == 1)
				{
					local_42 = 116;

					// Instruction address 0x1d12:0x2c15, size: 5
					F0_1d12_6ed4_DrawResources(cityID, 100, local_42, local_8, 92);

					local_42 += 16;

					// Instruction address 0x1d12:0x2c35, size: 5
					this.parent.Graphics.F0_VGA_0599_DrawLine(this.parent.Var_aa_Screen0_Rectangle, 100, local_42 - 2, 222, local_42 - 2, 1);
				}

				this.parent.Var_70e2 = this.parent.Var_70da_Arr[3] / 2;

				// Instruction address 0x1d12:0x2c4e, size: 5
				F0_1d12_6dfe(cityID, local_8);

				if (flag == 1 && this.Var_2496 == 1 && this.parent.Var_70e2 != 0)
				{
					// Instruction address 0x1d12:0x2c8f, size: 5
					F0_1d12_6ed4_DrawResources(cityID, 100, local_42, local_8, 92);

					// Instruction address 0x1d12:0x2ca6, size: 5
					this.parent.Graphics.F0_VGA_0c3e_DrawBitmapToScreen(this.parent.Var_aa_Screen0_Rectangle,
						208, local_42 + 4, this.parent.Array_d4ce[14]);

					local_42 += 16;

					// Instruction address 0x1d12:0x2cc6, size: 5
					this.parent.Graphics.F0_VGA_0599_DrawLine(this.parent.Var_aa_Screen0_Rectangle, 100, local_42 - 2, 222, local_42 - 2, 1);
				}

				local_6 = 208;

				if (city.HasImprovement(ImprovementEnum.Colosseum))
				{
					this.parent.Var_70e4 -= 3;

					if (flag == 1 && this.Var_2496 == 1)
					{
						// Instruction address 0x1d12:0x2d13, size: 5
						F0_1d12_7045(14, (short)local_6, (short)local_42);

						local_6 -= 16;
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
							this.parent.Var_70e4 -= 6;
						}
						else
						{
							this.parent.Var_70e4 -= 4;
						}
					}

					if (flag == 1 && this.Var_2496 == 1 && city.HasImprovement(ImprovementEnum.Cathedral))
					{
						// Instruction address 0x1d12:0x2db0, size: 5
						F0_1d12_7045(11, (short)local_6, (short)local_42);

						local_6 -= 16;
					}
				}

				if (city.HasImprovement(ImprovementEnum.Temple))
				{
					// Instruction address 0x1d12:0x2dd6, size: 5
					if (this.parent.Segment_1ade.F0_1ade_22b5_PlayerHasTechnology(this.Var_6548_PlayerID, TechnologyAdvanceEnum.Mysticism))
					{
						this.parent.Var_70e4 -= 2;
					}
					else
					{
						// Instruction address 0x1d12:0x2df6, size: 5
						if (this.parent.Segment_1ade.F0_1ade_22b5_PlayerHasTechnology(this.Var_6548_PlayerID, TechnologyAdvanceEnum.CeremonialBurial))
						{
							this.parent.Var_70e4--;
						}
					}
				
					// Instruction address 0x1d12:0x2e12, size: 5
					if (F0_1d12_6c97_PlayerHasWonder(this.Var_6548_PlayerID, WonderEnum.Oracle))
					{
						// Instruction address 0x1d12:0x2e2a, size: 5
						if (this.parent.Segment_1ade.F0_1ade_22b5_PlayerHasTechnology(this.Var_6548_PlayerID, TechnologyAdvanceEnum.Mysticism))
						{
							this.parent.Var_70e4 -= 2;
						}
						else
						{
							this.parent.Var_70e4 -= 1;
						}
					}

					if (flag == 1 && this.Var_2496 == 1)
					{
						// Instruction address 0x1d12:0x2e6f, size: 5
						F0_1d12_7045(4, (short)local_6, (short)local_42);

						local_6 -= 16;
					}
				}

				// Instruction address 0x1d12:0x2e81, size: 5
				F0_1d12_6dfe(cityID, local_8);

				if (flag == 1 && this.Var_2496 == 1 && 
					(city.HasImprovement(ImprovementEnum.Colosseum) || city.HasImprovement(ImprovementEnum.Cathedral) || city.HasImprovement(ImprovementEnum.Temple)))
				{
					// Instruction address 0x1d12:0x2ecb, size: 5
					F0_1d12_6ed4_DrawResources(cityID, 100, local_42, local_8, 92);

					local_42 += 16;

					// Instruction address 0x1d12:0x2eeb, size: 5
					this.parent.Graphics.F0_VGA_0599_DrawLine(this.parent.Var_aa_Screen0_Rectangle, 100, local_42 - 2, 222, local_42 - 2, 1);
				}

				local_6 = 208;

				if (this.parent.GameData.Players[this.Var_6548_PlayerID].GovernmentType < 4)
				{
					local_2 = 0;
					local_e6 = 0;

					// Instruction address 0x1d12:0x2f1a, size: 5
					local_e8 = this.parent.MapManagement.F0_2aea_1458_GetCellActiveUnitID(local_d8, local_e4);
					local_ba = local_e8;

					while (local_ba != -1)
					{
						local_2++;

						if (local_2 >= 32)
							break;

						if (this.parent.GameData.Units[(int)this.parent.GameData.Players[this.Var_6548_PlayerID].Units[local_ba].UnitType].AttackStrength != 0)
						{
							local_e6++;

							if (flag == 1 && this.Var_2496 == 1 && local_e6 <= 3)
							{
								// Instruction address 0x1d12:0x2fbd, size: 5
								this.parent.Graphics.F0_VGA_0c3e_DrawBitmapToScreen(this.parent.Var_aa_Screen0_Rectangle,
									local_6, local_42 - 1,
									this.parent.Array_d4ce[64 + (int)this.parent.GameData.Players[this.Var_6548_PlayerID].Units[local_ba].UnitType + (this.Var_6548_PlayerID * 32)]);

								local_6 -= 2;
							}
						}

						local_ba = this.parent.GameData.Players[this.Var_6548_PlayerID].Units[local_ba].NextUnitID;

						if (local_ba == local_e8)
						{
							local_ba = -1;
						}
					}

					for (int i = 0; i < 2; i++)
					{
						if (this.parent.Var_70e4 != 0 && city.Unknown[i] != -1 && this.parent.GameData.Units[city.Unknown[i] & 0x3f].AttackStrength != 0)
						{
							local_e6++;
						}
					}

					if (local_e6 > 3)
					{
						local_e6 = 3;
					}

					// Instruction address 0x1d12:0x3079, size: 5
					this.parent.Var_70e4 -= this.parent.GameTools.F0_2dc4_007c_CheckValueRange(local_e6, 0, this.parent.Var_70e4);
				}
				else
				{
					// Instruction address 0x1d12:0x3090, size: 5
					local_e8 = F0_1d12_6c97_PlayerHasWonder(this.Var_6548_PlayerID, WonderEnum.WomensSuffrage) ? 1 : 0;

					if (this.parent.GameData.Players[this.Var_6548_PlayerID].GovernmentType == 5)
					{
						local_e8++;
					}

					if (local_e8 != 0)
					{
						this.parent.Var_70e4 += local_e8 * this.Var_6546;

						if (flag == 1 && this.Var_2496 == 1)
						{
							for (int i = 0; i < (local_e8 * this.Var_6546); i++)
							{
								// Instruction address 0x1d12:0x3117, size: 5
								this.parent.Graphics.F0_VGA_0c3e_DrawBitmapToScreen(this.parent.Var_aa_Screen0_Rectangle, local_6, local_42 + 4,
									this.parent.Array_d4ce[13]);

								local_6 -= 2;
							}
						}
					}
				}

				// Instruction address 0x1d12:0x312c, size: 5
				F0_1d12_6dfe(cityID, local_8);

				if (flag == 1 && this.Var_2496 == 1)
				{
					// Instruction address 0x1d12:0x3163, size: 5
					F0_1d12_6ed4_DrawResources(cityID, 100, local_42, local_8, 92);

					local_42 += 16;

					// Instruction address 0x1d12:0x3183, size: 5
					this.parent.Graphics.F0_VGA_0599_DrawLine(this.parent.Var_aa_Screen0_Rectangle, 100, local_42 - 2, 222, local_42 - 2, 1);
				}

				local_e8 = this.parent.Var_70e2 - this.parent.Var_70e4;

				// Instruction address 0x1d12:0x319e, size: 5
				if (F0_1d12_6c97_PlayerHasWonder(this.Var_6548_PlayerID, WonderEnum.HangingGardens))
				{
					this.parent.Var_70e2++;
				}
			
				// Instruction address 0x1d12:0x31ba, size: 5
				if (F0_1d12_6c97_PlayerHasWonder(this.Var_6548_PlayerID, WonderEnum.CureForCancer))
				{
					this.parent.Var_70e2++;
				}
			
				// Instruction address 0x1d12:0x31d2, size: 5
				if (F0_1d12_6cf3_GetWonderCityID((int)WonderEnum.ShakespearesTheatre) == cityID)
				{
					this.parent.Var_70e4 = 0;
				}
			
				// Instruction address 0x1d12:0x31f0, size: 5
				if (F0_1d12_6c97_PlayerHasWonder(this.Var_6548_PlayerID, WonderEnum.JSBachsCathedral))
				{
					// Instruction address 0x1d12:0x3229, size: 5
					// Instruction address 0x1d12:0x3217, size: 5
					if (this.parent.MapManagement.F0_2aea_1942_GetGroupID(local_d8, local_e4) == 
						this.parent.MapManagement.F0_2aea_1942_GetGroupID(
							this.parent.GameData.Cities[this.parent.GameData.WonderCityID[(int)WonderEnum.JSBachsCathedral]].Position.X,
							this.parent.GameData.Cities[this.parent.GameData.WonderCityID[(int)WonderEnum.JSBachsCathedral]].Position.Y))
					{
						this.parent.Var_70e4 -= 2;
					}
				}

				// Instruction address 0x1d12:0x3243, size: 5
				F0_1d12_6dfe(cityID, local_8);

				if (flag == 1 && this.Var_2496 == 1 && (this.parent.Var_70e2 - this.parent.Var_70e4) != local_e8)
				{
					// Instruction address 0x1d12:0x328a, size: 5
					F0_1d12_6ed4_DrawResources(cityID, 100, local_42, local_8, 92);

					// Instruction address 0x1d12:0x32a5, size: 5
					this.parent.DrawTools.F0_1182_005c_DrawStringToScreen0("WONDERS", 190, local_42 + 5, 15);

					local_42 += 16;

					// Instruction address 0x1d12:0x32c5, size: 5
					this.parent.Graphics.F0_VGA_0599_DrawLine(this.parent.Var_aa_Screen0_Rectangle, 100, local_42 - 2, 222, local_42 - 2, 1);
				}

				if ((this.parent.GameData.DebugFlags & 0x2) == 0)
				{
					this.parent.Var_70e4 = 0;
					this.parent.Var_70e2 = 0;
				}

			L32e0:
				local_4e = 0;
				this.parent.Var_deb8 = 0;
				this.parent.Var_d2f6 = 0;

				for (int i = 0; i < 2; i++)
				{
					if (city.Unknown[i] != -1)
					{
						this.parent.Var_deb8++;
					}				
				}

				if (city.ActualSize < this.parent.Var_deb8)
				{
					this.parent.Var_d2f6 = this.parent.Var_deb8 - city.ActualSize;
				}
			
				local_f6 = 8;
				local_fc = 69;
				local_fa = 100;
				local_100 = 116;

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
								if (this.parent.GameData.Players[this.Var_6548_PlayerID].GovernmentType > 1 && (this.parent.GameData.DebugFlags & 0x2) != 0)
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
								((city.StatusFlag & 0x1) != 0 && ((i + this.parent.GameData.TurnCount) & 0x7) == 0 &&
								this.Var_6548_PlayerID != this.parent.GameData.HumanPlayerID &&
								this.parent.GameData.Players[this.Var_6548_PlayerID].GovernmentType >= 4))
							{
								local_108 = -1;
								local_c8 = -1;

								for (int j = 0; j < 128; j++)
								{
									if (this.parent.GameData.Players[this.Var_6548_PlayerID].Units[j].UnitType != UnitTypeEnum.None &&
										this.parent.GameData.Players[this.Var_6548_PlayerID].Units[j].HomeCityID == cityID &&
										(this.parent.GameData.Players[this.Var_6548_PlayerID].Units[j].UnitType != UnitTypeEnum.Diplomat &&
											this.parent.GameData.Players[this.Var_6548_PlayerID].Units[j].UnitType != UnitTypeEnum.Caravan))
									{
										// Instruction address 0x1d12:0x3513, size: 5
										local_b2 = this.parent.GameTools.F0_2dc4_0289_GetShortestDistance(
											city.Position.X,
											city.Position.Y,
											this.parent.GameData.Players[this.Var_6548_PlayerID].Units[j].Position.X,
											this.parent.GameData.Players[this.Var_6548_PlayerID].Units[j].Position.Y);

										if (local_b2 > local_c8)
										{
											local_c8 = local_b2;
											local_108 = j;
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
									else if (local_c8 > 0 &&
										this.parent.GameData.Units[(int)this.parent.GameData.Players[this.Var_6548_PlayerID].Units[local_108].UnitType].MovementType == UnitMovementTypeEnum.Land &&
										this.parent.GameData.Players[this.Var_6548_PlayerID].Units[local_108].UnitType != 0)
									{
										// Instruction address 0x1d12:0x35c9, size: 5
										this.parent.UnitManagement.F0_1866_0f10_DeleteUnit(this.Var_6548_PlayerID, (short)local_108);

										city.StatusFlag &= 0xfe;
									}
								}
								else
								{
									if (this.Var_6548_PlayerID == this.parent.GameData.HumanPlayerID)
									{
										this.parent.Var_2f9e_MessageBoxStyle = MenuBoxReportTypeEnum.DefenseMinisterReport;

										// Instruction address 0x1d12:0x3657, size: 5
										this.parent.Segment_1238.F0_1238_001e_ShowDialog($"{this.parent.Segment_2459.F0_2459_08c6_GetCityName(cityID)}" +
											$" can't support\n{this.parent.GameData.Units[(int)this.parent.GameData.Players[this.Var_6548_PlayerID].Units[local_108].UnitType].Name}" +
											".\n Unit Disbanded.\n", 100, 80);
									}

									// Instruction address 0x1d12:0x3667, size: 5
									this.parent.UnitManagement.F0_1866_0f10_DeleteUnit(this.Var_6548_PlayerID, (short)local_108);

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
										local_f6 + 8, local_fc + 12, this.parent.Array_d4ce[9]);
								}
								if (this.parent.GameData.Players[this.Var_6548_PlayerID].Units[i].UnitType == 0)
								{
									// Instruction address 0x1d12:0x36f4, size: 5
									this.parent.Graphics.F0_VGA_0c3e_DrawBitmapToScreen(this.parent.Var_aa_Screen0_Rectangle,
										local_f6, local_fc + 12, this.parent.Array_d4ce[8]);

									if (this.parent.GameData.Players[this.Var_6548_PlayerID].GovernmentType >= 2)
									{
										// Instruction address 0x1d12:0x371f, size: 5
										this.parent.Graphics.F0_VGA_0c3e_DrawBitmapToScreen(this.parent.Var_aa_Screen0_Rectangle,
											local_f6 + 2, local_fc + 12, this.parent.Array_d4ce[8]);
									}
								}
								else
								{
									// Instruction address 0x1d12:0x3732, size: 5
									local_e8 = F0_1d12_6c97_PlayerHasWonder(this.Var_6548_PlayerID, WonderEnum.WomensSuffrage) ? 1 : 0;

									if (this.parent.GameData.Players[this.Var_6548_PlayerID].GovernmentType == 5)
									{
										local_e8++;
									}

									if (this.parent.GameData.Players[this.Var_6548_PlayerID].GovernmentType >= 4)
									{
										if (this.parent.GameData.Units[(int)this.parent.GameData.Players[this.Var_6548_PlayerID].Units[i].UnitType].AttackStrength != 0 &&
											local_e8 != 0 &&
											(this.parent.GameData.Units[(int)this.parent.GameData.Players[this.Var_6548_PlayerID].Units[i].UnitType].MovementType == UnitMovementTypeEnum.Air ||
												this.parent.GameData.Players[this.Var_6548_PlayerID].Units[i].Position.X != city.Position.X ||
												this.parent.GameData.Players[this.Var_6548_PlayerID].Units[i].Position.Y != city.Position.Y))
										{
											// Instruction address 0x1d12:0x381d, size: 5
											this.parent.Graphics.F0_VGA_0c3e_DrawBitmapToScreen(this.parent.Var_aa_Screen0_Rectangle,
												local_f6, local_fc + 12, this.parent.Array_d4ce[13]);

											if (local_e8 > 1)
											{
												// Instruction address 0x1d12:0x3842, size: 5
												this.parent.Graphics.F0_VGA_0c3e_DrawBitmapToScreen(this.parent.Var_aa_Screen0_Rectangle,
													local_f6 + 2, local_fc + 12, this.parent.Array_d4ce[13]);
											}
										}
									}
								}
							}

							// Instruction address 0x1d12:0x3877, size: 5
							this.parent.Graphics.F0_VGA_0c3e_DrawBitmapToScreen(this.parent.Var_aa_Screen0_Rectangle, local_f6, local_fc,
								this.parent.Array_d4ce[64 + (int)this.parent.GameData.Players[this.Var_6548_PlayerID].Units[i].UnitType + (this.Var_6548_PlayerID * 32)]);

							// Instruction address 0x1d12:0x38a3, size: 5
							F0_1d12_73ea_DrawCityRectangle(
								this.parent.GameData.Players[this.Var_6548_PlayerID].Units[i].Position.X,
								this.parent.GameData.Players[this.Var_6548_PlayerID].Units[i].Position.Y,
								7);

							local_f6 += 16;

							if (local_f6 >= 112)
							{
								local_f6 = 8;
								local_fc += 16;

								if (local_fc > 85)
								{
									local_fc -= 24;
								}
							}
						}
					}

					if (flag == 1 && local_4e < 18 && this.Var_2496 == 0 &&
						this.parent.GameData.Players[this.Var_6548_PlayerID].Units[i].UnitType != UnitTypeEnum.None &&
						this.parent.GameData.Players[this.Var_6548_PlayerID].Units[i].Position.X == local_d8 &&
						this.parent.GameData.Players[this.Var_6548_PlayerID].Units[i].Position.Y == local_e4)
					{
						// Instruction address 0x1d12:0x3969, size: 5
						this.parent.MapManagement.F0_2aea_0fb3_DrawUnitWithStatus(this.Var_6548_PlayerID, (short)i, local_fa, local_100);

						string cityName = this.parent.Segment_2459.F0_2459_08c6_GetCityName(this.parent.GameData.Players[this.Var_6548_PlayerID].Units[i].HomeCityID);

						cityName = (cityName.Length > 3) ? (cityName.Substring(0, 3) + "."): cityName;

						// Instruction address 0x1d12:0x39b4, size: 5
						this.parent.DrawTools.F0_1182_005c_DrawStringToScreen0(cityName, local_fa, local_100 + 15, 0);

						Arr_74[local_4e] = i;

						local_4e++;

						local_fa += 18;
						
						if ((local_4e % 6) == 0)
						{
							local_fa = 100;
							local_100 += 16;
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
					this.parent.Var_e3c2 += this.parent.GameTools.F0_2dc4_007c_CheckValueRange(this.parent.Var_deb8, 0, city.ActualSize);

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

					local_106 = 2;
					local_e4 = 2;
					local_40 = 0;
					local_b4 = 0;

					if (local_da == 0)
					{
						for (int i = 0; i < 21; i++)
						{
							if (this.parent.GameData.WonderCityID[i + 1] == cityID)
							{
								local_e8 = i + 24;

								// Instruction address 0x1d12:0x3b40, size: 5
								this.parent.DrawTools.F0_1182_005c_DrawStringToScreen0(
									this.parent.LanguageTools.F0_2f4d_04f7_TrimStringToWidth(this.parent.GameData.Wonders[i + 1].Name.ToUpper(), 63), 253, local_e4 + 2, 15);

								// Instruction address 0x1d12:0x3b69, size: 5
								F0_1d12_7045((short)(local_e8 + 1), (short)(((local_106 & 1) != 0) ? 213 : 233), (short)(local_e4 - 2));

								local_e4 += 6;
								local_106++;
								local_40++;
							}
						}
					}

					local_fe = 0;

					for (int i = local_da; i < 24; i++)
					{
						if (city.HasImprovement(City.BitToImprovementEnum(i)))
						{
							if (local_106 >= 16)
							{
								local_ca |= 0x2;
								local_ca &= 0x7ffffffe;
								local_fe = i;

								break;
							}
							else
							{
								local_b4++;

								// Instruction address 0x1d12:0x3bfb, size: 5
								this.parent.Graphics.F0_VGA_0c3e_DrawBitmapToScreen(this.parent.Var_aa_Screen0_Rectangle,
									309, local_e4 + 1, this.parent.Array_d4ce[11]);

								// Instruction address 0x1d12:0x3c46, size: 5
								this.parent.DrawTools.F0_1182_005c_DrawStringToScreen0(
									this.parent.LanguageTools.F0_2f4d_04f7_TrimStringToWidth(this.parent.GameData.GetImprovementType(i + 1).Name, 56), 253, local_e4 + 2, 15);

								// Instruction address 0x1d12:0x3c6f, size: 5
								F0_1d12_7045((short)(i + 1), (short)(((local_106 & 1) != 0) ? 213 : 233), (short)(local_e4 - 2));

								local_e4 += 6;
								local_106++;
							}
						}
					}

					// Instruction address 0x1d12:0x3c97, size: 5
					this.parent.Graphics.F0_VGA_0599_DrawLine(this.parent.Var_aa_Screen0_Rectangle, 231, 0, 250, 0, 0);

					// Instruction address 0x1d12:0x3cb3, size: 5
					this.parent.Graphics.F0_VGA_0599_DrawLine(this.parent.Var_aa_Screen0_Rectangle, 231, 1, 250, 1, 1);

					if ((local_ca & 2) != 0)
					{
						// Instruction address 0x1d12:0x3cdd, size: 5
						this.parent.DrawTools.DrawFilledRectangleWithCenteredText(287, 88, 315, 96, "MORE", 9);
					}

					// Instruction address 0x1d12:0x3d01, size: 5
					this.parent.Graphics.F0_VGA_009a_ReplaceColor(this.parent.Var_aa_Screen0_Rectangle, 309, 2, 8, 96, 14, 12);

					if (city.CurrentProductionID >= 0)
					{
						local_de = this.parent.GameData.Units[city.CurrentProductionID].Cost;
					}
					else
					{
						local_de = this.parent.GameData.GetImprovementType(-city.CurrentProductionID).Cost;
					}

					// Instruction address 0x1d12:0x3d7a, size: 5
					local_44 = this.parent.GameTools.F0_2dc4_007c_CheckValueRange(((local_de - 1) / 10) + 1, ((city.ShieldsCount - 1) / 100) + 1, 99);
					local_e8 = 80 / local_4a;
					local_f6 = (((local_44 * 8) - 8) / local_44) + (local_4a * local_e8);
					local_fc = (((local_de - 1) / local_44) * 8) + 8;

					// Instruction address 0x1d12:0x3ded, size: 5
					this.parent.DrawTools.FillRectangle(this.parent.Var_aa_Screen0_Rectangle, 230, 99, local_f6 + 3, local_fc + 19, 1);

					if (this.Var_6548_PlayerID == this.parent.GameData.HumanPlayerID)
					{
						// Instruction address 0x1d12:0x3e31, size: 5
						this.parent.DrawTools.DrawFilledRectangleWithCenteredText(231, 106, 263, 114, (string)(((city.StatusFlag & 0x10) != 0) ? "AUTO" : "CHANGE"), 9);

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
						this.parent.DrawTools.F0_1182_00b3_DrawCenteredStringToScreen0(
							this.parent.LanguageTools.F0_2f4d_04f7_TrimStringToWidth(this.parent.GameData.GetImprovementType(-city.CurrentProductionID).Name, 86), 274, 100, 15);
					}

					// Instruction address 0x1d12:0x3ef9, size: 5
					this.parent.DrawTools.FillRectangleWithPattern(231, 116, local_f6 + 1, local_fc + 1, 208, 100, 16, 16);

					for (int i = 0; i < city.ShieldsCount; i++)
					{
						// Instruction address 0x1d12:0x3f55, size: 5
						this.parent.Graphics.F0_VGA_0c3e_DrawBitmapToScreen(this.parent.Var_aa_Screen0_Rectangle,
							(((i % (local_4a * local_44)) * local_e8) / local_44) + 232,
							((i / (local_4a * local_44)) * 8) + 117,
							this.parent.Array_d4ce[9]);
					}

					// Instruction address 0x1d12:0x3f7b, size: 5
					if (city.ActualSize != 0)
					{
						local_e8 = this.parent.GameTools.F0_2dc4_007c_CheckValueRange(80 / city.ActualSize, 1, 8);
					}
					else
					{
						local_e8 = 1;
					}

					local_fc = 80 / local_4a;

					// Instruction address 0x1d12:0x3fb5, size: 5
					this.parent.DrawTools.FillRectangle(this.parent.Var_aa_Screen0_Rectangle, 2, 106, 91, (local_4a * local_fc) + 12, 1);

					// Instruction address 0x1d12:0x3fcd, size: 5
					this.parent.DrawTools.F0_1182_005c_DrawStringToScreen0("Food Storage", 8, 108, 15);

					// Instruction address 0x1d12:0x3ffc, size: 5
					this.parent.DrawTools.FillRectangleWithPattern(3, 115, (city.ActualSize * local_e8) + 9, (local_4a * local_fc) + 2, 208, 100, 16, 16);

					if (city.HasImprovement(ImprovementEnum.Granary))
					{
						// Instruction address 0x1d12:0x403b, size: 5
						this.parent.Graphics.F0_VGA_0599_DrawLine(this.parent.Var_aa_Screen0_Rectangle, 5, 155, (city.ActualSize * local_e8) + 9, 155, 1);
					}

					// Instruction address 0x1d12:0x406a, size: 5
					int iTemp = this.parent.GameTools.F0_2dc4_007c_CheckValueRange(city.FoodCount, 0, (city.ActualSize + 1) * local_4a);

					for (int i = 0; i < iTemp; i++)
					{
						// Instruction address 0x1d12:0x40af, size: 5
						this.parent.Graphics.F0_VGA_0c3e_DrawBitmapToScreen(this.parent.Var_aa_Screen0_Rectangle,
							((i % (city.ActualSize + 1)) * local_e8) + 4,
							((i / (city.ActualSize + 1)) * local_fc) + 116,
							this.parent.Array_d4ce[8]);
					}

					// Instruction address 0x1d12:0x40ca, size: 5
					this.parent.DrawTools.DrawRectangleAndFillWithPattern(2, 23, 124, 65, 208, 100, 16, 16);

					local_fc = 25;

					// Instruction address 0x1d12:0x40f0, size: 5
					this.parent.DrawTools.FillRectangle(this.parent.Var_aa_Screen0_Rectangle, 2, 23, 122, 9, 1);

					// Instruction address 0x1d12:0x4108, size: 5
					this.parent.DrawTools.F0_1182_005c_DrawStringToScreen0("City Resources", 8, local_fc, 15);

					local_fc += 8;

					for (int i = 0; i < 3; i++)
					{
						local_d8 = 4;

						if (i == 0)
						{
							// Instruction address 0x1d12:0x416d, size: 5
							// Instruction address 0x1d12:0x4181, size: 5
							local_e8 = this.parent.GameTools.F0_2dc4_007c_CheckValueRange(
								116 / (this.parent.GameTools.F0_2dc4_007c_CheckValueRange(this.parent.Var_70da_Arr[i], 
									(city.ActualSize * 2) + (local_48 * this.parent.Var_e3c6), 999) + 1),
								1, 8);
						}
						else
						{
							// Instruction address 0x1d12:0x41ac, size: 5
							local_e8 = this.parent.GameTools.F0_2dc4_007c_CheckValueRange(116 / (this.parent.Var_70da_Arr[i] + 1), 1, 8);
						}

						for (int j = 0; j < this.parent.Var_70da_Arr[i]; j++)
						{
							switch (i)
							{
								case 0:
									if ((city.ActualSize * 2) + (local_48 * this.parent.Var_e3c6) == j)
									{
										local_d8 += 4;
									}
									break;

								case 1:
									if (this.parent.Var_d2f6 != 0 && j == this.parent.Var_d2f6)
									{
										local_d8 += 4;
									}
									break;

								case 2:
									if ((this.parent.Var_70da_Arr[2] - this.parent.Var_d2e0) == j)
									{
										local_d8 += 2;
									}
									break;
							}

							// Instruction address 0x1d12:0x4267, size: 5
							this.parent.Graphics.F0_VGA_0c3e_DrawBitmapToScreen(this.parent.Var_aa_Screen0_Rectangle,
								local_d8, (i * 8) + local_fc, this.parent.Array_d4ce[8 + i]);

							local_d8 += local_e8;
						}
					}

					local_d8 = 8;

					// Instruction address 0x1d12:0x42a6, size: 5
					local_e8 = this.parent.GameTools.F0_2dc4_007c_CheckValueRange(
						224 / (this.parent.Var_70da_Arr[3] + this.parent.Var_70e6 + this.parent.Var_e17a + this.parent.Var_d2e0 + 2), 1, 16);

					for (int i = 0; i < this.parent.Var_70da_Arr[3]; i++)
					{
						// Instruction address 0x1d12:0x42e1, size: 5
						this.parent.Graphics.F0_VGA_0c3e_DrawBitmapToScreen(this.parent.Var_aa_Screen0_Rectangle,
							local_d8 / 2, local_fc + 24, this.parent.Array_d4ce[14]);

						local_d8 += local_e8;
					}

					if (this.parent.Var_70da_Arr[3] != 0)
					{
						local_d8 += 8;
					}

					local_f6 = local_d8;

					for (int i = 0; i < this.parent.Var_e17a; i++)
					{
						// Instruction address 0x1d12:0x433a, size: 5
						this.parent.Graphics.F0_VGA_0c3e_DrawBitmapToScreen(this.parent.Var_aa_Screen0_Rectangle,
							local_d8 / 2, local_fc + 24, this.parent.Array_d4ce[11]);

						local_d8 += local_e8;
					}

					if (this.parent.Var_e17a != 0)
					{
						local_d8 += 8;
					}

					for (int i = 0; i < this.parent.Var_70e6; i++)
					{
						// Instruction address 0x1d12:0x438b, size: 5
						this.parent.Graphics.F0_VGA_0c3e_DrawBitmapToScreen(this.parent.Var_aa_Screen0_Rectangle,
							local_d8 / 2, local_fc + 24, this.parent.Array_d4ce[12]);

						local_d8 += local_e8;
					}

					if ((city.ActualSize * 2) + (local_48 * this.parent.Var_e3c6) > this.parent.Var_70da_Arr[0])
					{
						// Instruction address 0x1d12:0x43ed, size: 5
						local_e8 = this.parent.GameTools.F0_2dc4_007c_CheckValueRange(116 / ((city.ActualSize * 2) + (local_48 * this.parent.Var_e3c6) + 1), 1, 8);

						for (int i = this.parent.Var_70da_Arr[0]; i < (city.ActualSize * 2) + (local_48 * this.parent.Var_e3c6); i++)
						{
							// Instruction address 0x1d12:0x443e, size: 5
							this.parent.Graphics.F0_VGA_0c3e_DrawBitmapToScreen(this.parent.Var_aa_Screen0_Rectangle,
								(i * local_e8) + 8, local_fc, this.parent.Array_d4ce[8]);
						}

						// Instruction address 0x1d12:0x448f, size: 5
						this.parent.Graphics.F0_VGA_009a_ReplaceColor(this.parent.Var_aa_Screen0_Rectangle,
							(local_e8 * this.parent.Var_70da_Arr[0]) + 8, local_fc,
							(((city.ActualSize * 2) + (local_48 * this.parent.Var_e3c6) - this.parent.Var_70da_Arr[0]) * local_e8) + 4,
							8, 15, 0);
					}

					if (this.parent.Var_70da_Arr[1] < this.parent.Var_d2f6)
					{
						// Instruction address 0x1d12:0x44b9, size: 5
						local_e8 = this.parent.GameTools.F0_2dc4_007c_CheckValueRange(116 / (this.parent.Var_70da_Arr[1] + 1), 1, 8);

						for (int i = this.parent.Var_70da_Arr[1]; i < this.parent.Var_d2f6; i++)
						{
							// Instruction address 0x1d12:0x44f7, size: 5
							this.parent.Graphics.F0_VGA_0c3e_DrawBitmapToScreen(this.parent.Var_aa_Screen0_Rectangle,
								(i * local_e8) + 8, local_fc + 8, this.parent.Array_d4ce[9]);
						}

						// Instruction address 0x1d12:0x4532, size: 5
						this.parent.Graphics.F0_VGA_009a_ReplaceColor(this.parent.Var_aa_Screen0_Rectangle,
							(this.parent.Var_70da_Arr[1] * local_e8) + 8, local_fc + 8,
							(this.parent.Var_d2f6 - this.parent.Var_70da_Arr[1]) * local_e8,
							8, 15, 0);
					}

					if (this.parent.Var_d2e0 != 0)
					{
						// Instruction address 0x1d12:0x455a, size: 5
						local_e8 = this.parent.GameTools.F0_2dc4_007c_CheckValueRange(116 / (this.parent.Var_70da_Arr[2] + 1), 1, 8);

						// Instruction address 0x1d12:0x4598, size: 5
						this.parent.Graphics.F0_VGA_009a_ReplaceColor(this.parent.Var_aa_Screen0_Rectangle,
							((this.parent.Var_70da_Arr[2] - this.parent.Var_d2e0) * local_e8) + 6,
							local_fc + 16, (local_e8 * this.parent.Var_d2e0) + 2,
							8, 15, 0);
					}

					// Instruction address 0x1d12:0x45b0, size: 5
					this.parent.DrawTools.FillRectangleWithPattern(8, 8, 200, 13, 208, 100, 16, 16);

					// Instruction address 0x1d12:0x45ca, size: 5
					local_f4 = F0_1d12_6ed4_DrawResources(cityID, 8, 8, local_8, 192);

					// Instruction address 0x1d12:0x45f3, size: 5
					local_e8 = this.parent.GameTools.F0_2dc4_007c_CheckValueRange((city.ActualSize * 8) + 24, 0, 128);

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
									this.parent.DrawTools.F0_1182_005c_DrawStringToScreen0($"{{{this.parent.Segment_2459.F0_2459_08c6_GetCityName(tradeCityID)}:+" +
										$"{(city.BaseTrade + this.parent.GameData.Cities[tradeCityID].BaseTrade + 4) / 8}}} ", 98, (i * 6) + 179, 10);
								}
								else
								{
									// Instruction address 0x1d12:0x4765, size: 5
									this.parent.DrawTools.F0_1182_005c_DrawStringToScreen0($"{{{this.parent.Segment_2459.F0_2459_08c6_GetCityName(tradeCityID)}:+" +
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
						local_e0 = (this.parent.Var_70da_Arr[1] / this.parent.Var_6c98) - 20 + ((city.ActualSize * this.parent.Var_b882) / 4);

						// Instruction address 0x1d12:0x47ef, size: 5
						// Instruction address 0x1d12:0x4802, size: 5
						local_fc = this.parent.GameTools.F0_2dc4_007c_CheckValueRange(
							100 / this.parent.GameTools.F0_2dc4_007c_CheckValueRange(local_e0, 1, 99), 1, 8);

						for (int i = 0; i < local_e0; i++)
						{
							// Instruction address 0x1d12:0x4847, size: 5
							this.parent.Graphics.F0_VGA_0c3e_DrawBitmapToScreen(this.parent.Var_aa_Screen0_Rectangle,
								(i * local_fc) + 98, (i & 1) + 161, this.parent.Var_b2ba);
						}
					}

					if (this.Var_6548_PlayerID == this.parent.GameData.HumanPlayerID)
					{
						local_ea = 0;

						// Instruction address 0x1d12:0x4876, size: 5
						this.parent.DrawTools.DrawFilledRectangleWithCenteredText(284, 190, 316, 198, "EXIT", 12);

						// Instruction address 0x1d12:0x4896, size: 5
						this.parent.DrawTools.DrawFilledRectangleWithCenteredText(231, 190, 272, 198, "RENAME", 9);

						local_c0 = 0;

						if (city.CurrentProductionID < 0 && city.CurrentProductionID >= -21 && city.HasImprovement(City.BitToImprovementEnum((-city.CurrentProductionID) - 1)))
						{
							local_c0 = 1;
						}

						if (city.CurrentProductionID < -21 && city.CurrentProductionID >= -24 && (this.parent.GameData.SpaceshipFlags & (1 << this.Var_6548_PlayerID)) != 0)
						{
							local_c0 = 1;
						}

						if (city.CurrentProductionID < -24)
						{
							if (this.parent.GameData.WonderCityID[Math.Abs(city.CurrentProductionID) - 24] != -1)
							{
								local_c0 = 1;
							}
						}

						if (city.CurrentProductionID >= 0 && this.parent.GameData.Players[this.Var_6548_PlayerID].UnitCount >= 127)
						{
							local_c0 = 1;
						}

						MouseEvent mouseEvent;

						while ((mouseEvent = this.parent.GetMouseEvent()).Buttons != MouseButtonsEnum.None) { }

						while ((mouseEvent = this.parent.GetMouseEvent()).Buttons == MouseButtonsEnum.None)
						{
							// Instruction address 0x1d12:0x49b5, size: 5
							if (this.parent.CAPI.kbhit() != 0 || local_b8 != 0)
								break;

							if (local_c0 != 0)
							{
								local_c0++;

								if ((local_c0 & 1) != 0)
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

						local_f0 = -1;

						// Instruction address 0x1d12:0x4a54, size: 5
						if (this.parent.CAPI.kbhit() == 0)
						{
							if (local_b8 != 0)
							{
								local_f0 = 'p';
							}
						}
						else
						{
							// Instruction address 0x1d12:0x4a61, size: 5
							local_f0 = this.parent.MenuBoxDialog.F0_2d05_0ac9_GetNavigationKey();

							// The CityWorker is not processing key presses from mouse events separately
							// We don't want to mix mouse location code when pressing a key
							// Ensure that mouse event doesn't interfere

							//this.parent.Var_db3e_MouseYPos = 0;
							//this.parent.Var_db3c_MouseXPos = 0;
							mouseEvent = new MouseEvent(new GPoint(0, 0), MouseButtonsEnum.None);
						}

						if ((mouseEvent.Position.X >= 230 && mouseEvent.Position.X < 270 &&
							mouseEvent.Position.Y >= 106 && mouseEvent.Position.Y <= 116 && mouseEvent.Buttons == MouseButtonsEnum.Right) ||
							local_f0 == 'A')
						{
							city.StatusFlag ^= 0x10;

							if ((city.StatusFlag & 0x10) != 0)
							{
								// Instruction address 0x1d12:0x4aef, size: 5
								local_102 = (short)this.parent.Segment_1ade.F0_1ade_0421(this.Var_6548_PlayerID, cityID);

								this.parent.Var_aa_Screen0_Rectangle.FontID = 2;

								if (local_102 != 99)
								{
									this.parent.Var_b1e8 = 0;

									if (city.CurrentProductionID >= 0)
									{
										this.parent.GameData.Players[this.Var_6548_PlayerID].UnitsInProduction[city.CurrentProductionID]--;
									}

									city.CurrentProductionID = (sbyte)local_102;

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

							this.parent.Var_70da_Arr[1] = local_10a;
							this.parent.Var_70da_Arr[2] = local_a;

							goto L12c2;
						}

						if (local_f0 != -1)
							goto L4be2;

						if (mouseEvent.Buttons == MouseButtonsEnum.Left) goto L4bce;
						goto L4ff5;

					L4bce:
						if (mouseEvent.Position.Y >= 106) goto L4bd8;
						goto L4ff5;

					L4bd8:
						if (mouseEvent.Position.Y <= 116) goto L4be2;
						goto L4ff5;

					L4be2:
						if (mouseEvent.Position.X < 296) goto L4bed;
						goto L4bf7;

					L4bed:
						if (local_f0 == 'b') goto L4bf7;
						goto L4e80;

					L4bf7:
						if (city.CurrentProductionID >= 0)
						{
							local_106 = this.parent.GameData.Units[city.CurrentProductionID].Cost;

							// Instruction address 0x1d12:0x4c3d, size: 5
							local_e8 = this.parent.GameTools.F0_2dc4_007c_CheckValueRange((10 * local_106) - city.ShieldsCount, 0, 999);

							local_e8 = (local_e8 * local_e8) / 20 + local_e8 * 2;
						}
						else
						{
							local_106 = this.parent.GameData.GetImprovementType(-city.CurrentProductionID).Cost;

							// Instruction address 0x1d12:0x4ca0, size: 5
							local_e8 = this.parent.GameTools.F0_2dc4_007c_CheckValueRange((10 * local_106) - city.ShieldsCount, 0, 999) * 2;

							if ((-city.CurrentProductionID) > 21)
							{
								local_e8 += local_e8;
							}
						}

						if (city.ShieldsCount == 0)
						{
							local_e8 += local_e8;
						}

						string completeProductionText = "Cost to complete\n" +
								$"{((city.CurrentProductionID >= 0) ? this.parent.GameData.Units[city.CurrentProductionID].Name :
									this.parent.GameData.GetImprovementType(-city.CurrentProductionID).Name)} is {local_e8} coins" +
								$"\nOur treasury has {this.parent.GameData.Players[this.Var_6548_PlayerID].Coins} coins.\n";

						if ((city.StatusFlag & 0x1) != 0)
						{
							completeProductionText += "CIVIL DISORDER\n";
						}
						else if (this.parent.GameData.Players[this.Var_6548_PlayerID].Coins >= local_e8)
						{
							completeProductionText += "Do you want to continue?\n Yes\n No\n";
						}

						this.parent.Var_aa_Screen0_Rectangle.FontID = 1;

						// Instruction address 0x1d12:0x4e32, size: 5
						if ((city.StatusFlag & 0x1) == 0 && this.parent.GameData.Players[this.Var_6548_PlayerID].Coins >= local_e8 &&
							this.parent.MenuBoxDialog.F0_2d05_0031_ShowMenuBox(completeProductionText, 100, 80, true, false, true) == 0)
						{
							city.ShieldsCount = (short)(local_106 * 10);

							this.parent.GameData.Players[this.Var_6548_PlayerID].Coins -= (short)local_e8;
						}

						goto L045f;

					L4e80:
						if (local_f0 != 0x1b && local_f0 != 0xd && ((mouseEvent.Position.X >= 230 && mouseEvent.Position.X < 270) || local_f0 == 'c'))
						{
							city.StatusFlag &= 0xef;

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

						if (local_f0 == 'i') goto L4f41;
						goto L4f4a;

					L4f41:
						this.Var_2496 = 0;
						goto L4faf;

					L4f4a:
						if (local_f0 == 'h') goto L4f54;
						goto L4f5d;

					L4f54:
						this.Var_2496 = 1;
						goto L4faf;

					L4f5d:
						if (local_f0 == 'm') goto L4f67;
						goto L4f70;

					L4f67:
						this.Var_2496 = 2;
						goto L4faf;

					L4f70:
						if (local_f0 == 'v') goto L4f7a;
						goto L4f83;

					L4f7a:
						this.Var_2496 = 3;
						goto L4faf;

					L4f83:
						if (local_f0 != 0x1b && local_f0 != 0xd && mouseEvent.Position.X >= 96 && mouseEvent.Position.X < 224) goto L4f98;
						goto L4ff5;

					L4f98:
						this.Var_2496 = (mouseEvent.Position.X - 96) / 32;

					L4faf:
						this.parent.Var_70da_Arr[1] = local_10a;
						this.parent.Var_70da_Arr[2] = local_a;

						if (this.Var_2496 != 3)
							goto L12c2;

						this.parent.Var_aa_Screen0_Rectangle.FontID = 1;

						this.parent.CityView.F19_0000_0000_ShowCityLayout(cityID, -1, null);

						this.parent.Var_6b64 = 1;
						this.Var_2496 = 0;

						goto L045f;

					L4ff5:
						if (local_f0 == 'a') goto L4fff;
						goto L520e;

					L4fff:
						for (local_106 = 0; local_106 < local_4e && (this.parent.GameData.Players[this.Var_6548_PlayerID].Units[Arr_74[local_106]].Status & 0xcf) == 0; local_106++)
						{
						}

						if (local_106 == local_4e)
						{
							local_106 = 0;
						}

					L504b:
						local_fa = ((local_106 % 6) * 18) + 100;
						local_100 = (local_106 / 6) * 16 + 116;

						do
						{
							// Instruction address 0x1d12:0x5088, size: 5
							this.parent.DrawTools.FillRectangleWithPattern(local_fa, local_100, 16, 16, 208, 100, 16, 16);

							// Instruction address 0x1d12:0x5094, size: 5
							this.parent.CommonTools.WaitTimer(10);

							// Instruction address 0x1d12:0x50b1, size: 5
							this.parent.MapManagement.F0_2aea_0fb3_DrawUnitWithStatus(this.Var_6548_PlayerID, (short)Arr_74[local_106], local_fa, local_100);

							// Instruction address 0x1d12:0x50bd, size: 5
							this.parent.CommonTools.WaitTimer(10);
						}
						// Instruction address 0x1d12:0x50c5, size: 5
						while (this.parent.CAPI.kbhit() == 0);

						// Instruction address 0x1d12:0x50d2, size: 5
						local_fc = this.parent.MenuBoxDialog.F0_2d05_0ac9_GetNavigationKey();

						switch (local_fc)
						{
							// Up
							case 0x4800:
								local_ea = local_106 - 6;
								break;

							// Left
							case 0x4b00:
								local_ea = local_106 - 1;
								break;

							// Right
							case 0x4d00:
								local_ea = local_106 + 1;
								break;

							// Down
							case 0x5000:
								local_ea = local_106 + 6;
								break;

							default:
								local_ea = local_106;
								break;
						}

						// Instruction address 0x1d12:0x5151, size: 5
						local_106 = this.parent.GameTools.F0_2dc4_007c_CheckValueRange(local_ea, 0, local_4e - 1);

						if (local_fc == '\n' || local_fc == ' ')
						{
							if ((this.parent.GameData.Players[this.Var_6548_PlayerID].Units[Arr_74[local_106]].Status & 0x9) != 0)
							{
								this.parent.GameData.Players[this.Var_6548_PlayerID].Units[Arr_74[local_106]].RemainingMoves =
									(short)(this.parent.GameData.Units[(int)this.parent.GameData.Players[this.Var_6548_PlayerID].Units[Arr_74[local_106]].UnitType].MoveCount * 3);
							}

							this.parent.GameData.Players[this.Var_6548_PlayerID].Units[Arr_74[local_106]].Status &= 0x30;
							this.parent.GameData.Players[this.Var_6548_PlayerID].Units[Arr_74[local_106]].GoToDestination.X = -1;
						}
					
						// Escape
						if (local_fc != 0x1b)
							goto L504b;

						this.parent.Var_70da_Arr[1] = local_10a;
						this.parent.Var_70da_Arr[2] = local_a;

						goto L12c2;

					L520e:
						if (local_f0 == 's') goto L5218;
						goto L532e;

					L5218:
						if ((city.StatusFlag & 0x80) == 0) goto L522a;
						goto L532e;

					L522a:
						local_106 = 0;

					L5230:
						// Instruction address 0x1d12:0x5258, size: 5
						this.parent.Graphics.F0_VGA_009a_ReplaceColor(this.parent.Var_aa_Screen0_Rectangle,
							252, ((local_106 + local_40) * 6) + 3, 56, 7, 9, 1);

						// Instruction address 0x1d12:0x5264, size: 5
						this.parent.CommonTools.WaitTimer(10);

						// Instruction address 0x1d12:0x5294, size: 5
						this.parent.Graphics.F0_VGA_009a_ReplaceColor(this.parent.Var_aa_Screen0_Rectangle,
							252, ((local_106 + local_40) * 6) + 3, 56, 7, 1, 9);

						// Instruction address 0x1d12:0x52a0, size: 5
						this.parent.CommonTools.WaitTimer(10);

						// Instruction address 0x1d12:0x52a8, size: 5
						if (this.parent.CAPI.kbhit() != 0) goto L52b5;
						goto L5230;

					L52b5:
						// Instruction address 0x1d12:0x52b5, size: 5
						local_fc = this.parent.MenuBoxDialog.F0_2d05_0ac9_GetNavigationKey();
						goto L52e0;

					L52c5:
						local_ea = local_106 + 1;
						goto L52f3;

					L52d1:
						local_ea = local_106 - 1;
						goto L52f3;

					L52e0:
						// Up
						if (local_fc != 0x4800) goto L52e8;
						goto L52d1;

					L52e8:
						// Down
						if (local_fc != 0x5000) goto L52f3;
						goto L52c5;

					L52f3:
						// Instruction address 0x1d12:0x5301, size: 5
						local_106 = this.parent.GameTools.F0_2dc4_007c_CheckValueRange(local_ea, 0, local_b4 - 1);

						if (local_fc == '\n') goto L5740;

						if (local_fc == ' ') goto L5740;

						if (local_fc != 0x1b) goto L5230;

					L532e:
						if (local_f0 < '1' || local_f0 > '9')
							goto L53c8;

						local_ea = local_f0 - '1';

						if (local_ea >= local_8)
							goto L045f;

						if (city.ActualSize >= 5) goto L536b;
						goto L53ad;

					L536b:
						// Instruction address 0x1d12:0x536f, size: 5
						local_e8 = F0_1d12_6da1_GetSpecialWorkerFlags(local_ea);

						if (local_e8 >= 3)
							goto L539a;

						// Instruction address 0x1d12:0x538f, size: 5
						F0_1d12_6d6e_SetSpecialWorkerFlags(local_ea, local_e8 + 1);

						goto L53aa;

					L539a:
						// Instruction address 0x1d12:0x53a2, size: 5
						F0_1d12_6d6e_SetSpecialWorkerFlags(local_ea, 1);

					L53aa:
						goto L53b5;

					L53ad:
						this.parent.MenuBoxDialog.F0_2d05_0031_ShowMenuBox("A City must have five\npopulation units to support\ntaxmen or scientists.\n", 32, 32, true, false, true);

						goto L045f;

					L53b5:
						this.parent.Var_70da_Arr[1] = local_10a;
						this.parent.Var_70da_Arr[2] = local_a;

						goto L12c2;

					L53c8:
						if (local_f0 == 'p') goto L53d2;
						goto L563f;

					L53d2:
						if (local_b8 != 0)
							goto L53ed;

						local_be = 0;
						local_b6 = 0;
						local_b8 = 1;

					L53ed:
						// Instruction address 0x1d12:0x541d, size: 5
						this.parent.Graphics.F0_VGA_07d8_DrawImage(this.parent.Var_aa_Screen0_Rectangle,
							(local_b6 * 16) + 160, (local_be * 16) + 56,
							16, 16, this.parent.Var_19d4_Screen1_Rectangle, 0, 0);

						// Instruction address 0x1d12:0x5449, size: 5
						this.parent.DrawTools.DrawRectangle((local_b6 * 16) + 160, (local_be * 16) + 56, 15, 15, 15);

						// Instruction address 0x1d12:0x5455, size: 5
						this.parent.CommonTools.WaitTimer(10);

						// Instruction address 0x1d12:0x548d, size: 5
						this.parent.Graphics.F0_VGA_07d8_DrawImage(this.parent.Var_19d4_Screen1_Rectangle, 0, 0, 16, 16, this.parent.Var_aa_Screen0_Rectangle,
							(local_b6 * 16) + 160, (local_be * 16) + 56);

						// Instruction address 0x1d12:0x5499, size: 5
						this.parent.CommonTools.WaitTimer(10);

						this.parent.Var_6b64 = 1;

						// Instruction address 0x1d12:0x54a7, size: 5
						if (this.parent.CAPI.kbhit() != 0) goto L54b4;
						goto L53ed;

					L54b4:
						local_f6 = local_b6;
						local_fc = local_be;

						// Instruction address 0x1d12:0x54c4, size: 5
						local_e8 = this.parent.MenuBoxDialog.F0_2d05_0ac9_GetNavigationKey();

						goto L551f;

					L54d4:
						local_be--;

						goto L556a;

					L54db:
						local_be--;
						local_b6++;

						goto L556a;

					L54e6:
						local_b6++;

						goto L556a;

					L54ed:
						local_b6++;
						local_be++;

						goto L556a;

					L54f8:
						local_be++;

						goto L556a;

					L54ff:
						local_b6--;
						local_be++;

						goto L556a;

					L550a:
						local_b6--;

						goto L556a;

					L5511:
						local_b6--;
						local_be--;

						goto L556a;

					L551f:
						if (local_e8 != 0x4b00) goto L5527;
						goto L550a;

					L5527:
						if (local_e8 <= 0x4b00) goto L552c;
						goto L5547;

					L552c:
						if (local_e8 != 0x4700) goto L5534;
						goto L5511;

					L5534:
						if (local_e8 != 0x4800) goto L553c;
						goto L54d4;

					L553c:
						if (local_e8 != 0x4900) goto L556a;
						goto L54db;

					L5547:
						if (local_e8 != 0x4d00) goto L554f;
						goto L54e6;

					L554f:
						if (local_e8 != 0x4f00) goto L5557;
						goto L54ff;

					L5557:
						if (local_e8 != 0x5000) goto L555f;
						goto L54f8;

					L555f:
						if (local_e8 != 0x5100) goto L556a;
						goto L54ed;

					L556a:
						if (Math.Abs(local_b6) > 2 || Math.Abs(local_be) > 2)
							goto L55f5;

						if (Math.Abs(local_b6) + Math.Abs(local_be) < 4) goto L55b6;
						goto L55f5;

					L55b6:
						if ((this.parent.GameData.MapVisibility[city.Position.X + local_b6, city.Position.Y + local_be] & (1 << this.parent.GameData.HumanPlayerID)) == 0) goto L55f5;
						goto L5605;

					L55f5:
						local_b6 = local_f6;
						local_be = local_fc;

					L5605:
						if (local_e8 == 13)
							goto L5619;

						if (local_e8 != ' ')
							goto L562c;

						L5619:
						local_f6 = local_b6;
						local_fc = local_be;

						goto L5aa6;

					L562c:
						if (local_e8 != 0x1b)
							goto L53ed;

						local_b8 = 0;

						goto L045f;

					L563f:
						if (local_f0 == 'M') goto L5649;
						goto L5671;

					L5649:
						if ((local_ca & 2) == 0)
							goto L5671;

						local_ca ^= 1;
						local_da = local_fe;
						this.parent.Var_70da_Arr[1] = local_10a;
						this.parent.Var_70da_Arr[2] = local_a;

						goto L12c2;

					L5671:
						if (local_f0 != 'r') goto L567b;
						goto L56a6;

					L567b:
						if (mouseEvent.Buttons == MouseButtonsEnum.Left) goto L5685;
						goto L56c8;

					L5685:
						if (mouseEvent.Position.X >= 231) goto L5690;
						goto L56c8;

					L5690:
						if (mouseEvent.Position.Y > 190) goto L569b;
						goto L56c8;

					L569b:
						if (mouseEvent.Position.X < 270) goto L56a6;
						goto L56c8;

					L56a6:
						// Instruction address 0x1d12:0x56a6, size: 5
						this.parent.CommonTools.EmptyKeyboardBufferAndClearMouse();

						this.parent.Var_aa_Screen0_Rectangle.FontID = 1;

						this.parent.TextBoxDialogs.F23_0000_0000_CityNameDialog(cityID);

						this.parent.Var_6b64 = 1;

						goto L045f;

					L56c8:
						if (mouseEvent.Buttons == MouseButtonsEnum.Left) goto L56d2;
						goto L5cc2;

					L56d2:
						if (mouseEvent.Position.X >= 288) goto L56dd;
						goto L5719;

					L56dd:
						if (mouseEvent.Position.Y < 96) goto L56e7;
						goto L5719;

					L56e7:
						if (mouseEvent.Position.Y > 88) goto L56f1;
						goto L5719;

					L56f1:
						if ((local_ca & 2) == 0)
							goto L5719;

						local_ca ^= 1;
						local_da = local_fe;
						this.parent.Var_70da_Arr[1] = local_10a;
						this.parent.Var_70da_Arr[2] = local_a;

						goto L12c2;

					L5719:
						if (mouseEvent.Position.X >= 300) goto L5724;
						goto L58ac;

					L5724:
						if (mouseEvent.Position.Y < 94) goto L572e;
						goto L58ac;

					L572e:
						local_106 = ((mouseEvent.Position.Y - 2) / 6) - local_40;

					L5740:
						local_bc = local_da;

						goto L574f;

					L574b:
						local_bc++;

					L574f:
						if (local_bc >= 24)
							goto L58ac;

						if (city.HasImprovement(City.BitToImprovementEnum(local_bc))) goto L5783;
						goto L574b;

					L5783:
						local_106--;

						if (local_106 != -1)
							goto L574b;

						if ((city.StatusFlag & 0x80) == 0) goto L57a5;
						goto L574b;

					L57a5:
						this.parent.Var_aa_Screen0_Rectangle.FontID = 1;

						// Instruction address 0x1d12:0x5834, size: 5
						if (this.parent.MenuBoxDialog.F0_2d05_0031_ShowMenuBox("Do you want to sell\nyour " +
							$"{this.parent.GameData.GetImprovementType(local_bc + 1).Name} for " +
							$"{(10 * this.parent.GameData.GetImprovementType(local_bc + 1).Cost)} coins?\n No.\n Yes.\n", 128, 80, true, false, true) == 1) goto L5844;
						goto L58a1;

					L5844:
						this.parent.GameData.Players[this.Var_6548_PlayerID].Coins += (short)(10 * this.parent.GameData.GetImprovementType(local_bc + 1).Cost);
						city.RemoveImprovement(City.BitToImprovementEnum(local_bc));
						city.StatusFlag |= 0x80;

						if (local_bc + 1 != 8)
							goto L58a1;

						this.parent.Var_6b64 = 1;

					L58a1:
						goto L045f;

					L58ac:
						if (mouseEvent.Position.X < 200) goto L58b7;
						goto L596b;

					L58b7:
						if (mouseEvent.Position.Y < 20) goto L58c1;
						goto L596b;

					L58c1:
						if (city.ActualSize >= 5) goto L58d3;
						goto L5953;

					L58d3:
						local_ea = (mouseEvent.Position.X - 16) / local_f4;
						local_ea -= city.ActualSize - local_50;

						if (local_ea < 0)
							goto L045f;


						if (local_ea >= local_8)
							goto L045f;

						// Instruction address 0x1d12:0x5915, size: 5
						local_e8 = F0_1d12_6da1_GetSpecialWorkerFlags(local_ea);

						if (local_e8 >= 3)
							goto L5940;

						// Instruction address 0x1d12:0x5935, size: 5
						F0_1d12_6d6e_SetSpecialWorkerFlags(local_ea, local_e8 + 1);

						goto L5950;

					L5940:
						// Instruction address 0x1d12:0x5948, size: 5
						F0_1d12_6d6e_SetSpecialWorkerFlags(local_ea, 1);

					L5950:
						goto L595b;

					L5953:
						this.parent.MenuBoxDialog.F0_2d05_0031_ShowMenuBox("A City must have five\npopulation units to support\ntaxmen or scientists.\n", 32, 32, true, false, true);

						goto L045f;

					L595b:
						this.parent.Var_70da_Arr[1] = local_10a;
						this.parent.Var_70da_Arr[2] = local_a;

						goto L12c2;

					L596b:
						if (mouseEvent.Position.X >= 100) goto L5975;
						goto L5a60;

					L5975:
						if (mouseEvent.Position.X < 320) goto L5980;
						goto L5a60;

					L5980:
						if (mouseEvent.Position.Y >= 116) goto L598a;
						goto L5a60;

					L598a:
						local_106 = (((mouseEvent.Position.Y - 116) / 16) * 6) + ((mouseEvent.Position.X - 100) / 16);

						if (local_106 >= local_4e)
							goto L5a60;

						if ((this.parent.GameData.Players[this.Var_6548_PlayerID].Units[Arr_74[local_106]].Status & 0x9) == 0)
							goto L5a18;

						this.parent.GameData.Players[this.Var_6548_PlayerID].Units[Arr_74[local_106]].RemainingMoves =
							(short)(this.parent.GameData.Units[(int)this.parent.GameData.Players[this.Var_6548_PlayerID].Units[Arr_74[local_106]].UnitType].MoveCount * 3);

					L5a18:
						this.parent.GameData.Players[this.Var_6548_PlayerID].Units[Arr_74[local_106]].Status &= 0x30;
						this.parent.GameData.Players[this.Var_6548_PlayerID].Units[Arr_74[local_106]].GoToDestination.X = -1;

						this.parent.Var_70da_Arr[1] = local_10a;
						this.parent.Var_70da_Arr[2] = local_a;

						goto L12c2;

					L5a60:
						if (mouseEvent.Position.Y >= 24) goto L5a6a;
						goto L5cc2;

					L5a6a:
						if (mouseEvent.Position.Y < 104) goto L5a74;
						goto L5cc2;

					L5a74:
						local_f6 = (mouseEvent.Position.X / 16) - 10;
						local_fc = ((mouseEvent.Position.Y - 24) / 16) - 2;

					L5aa6:
						// Instruction address 0x1d12:0x5aae, size: 5
						local_ea = F0_1d12_000a_FindCityOffset(new GPoint(local_f6, local_fc));

						if (local_ea == -1)
							goto L045f;

						if (local_ea >= 20)
							goto L045f;

						if ((this.parent.GameData.MapVisibility[city.Position.X + this.parent.CityOffsets[local_ea].X, city.Position.Y + this.parent.CityOffsets[local_ea].Y] &
							(1 << this.Var_6548_PlayerID)) != 0) goto L5b13;
						goto L045f;

					L5b13:
						this.parent.Var_70da_Arr[1] = local_10a;
						this.parent.Var_70da_Arr[2] = local_a;

						if ((city.WorkerFlags & (1 << local_ea)) != 0)
							goto L5b6e;

						if (Arr_a6[this.parent.CityOffsets[local_ea].X + 2, this.parent.CityOffsets[local_ea].Y + 2] != 0)
							goto L12c2;

					L5b6e:
						city.WorkerFlags ^= (uint)(1 << local_ea);

						if ((city.WorkerFlags & (1 << local_ea)) == 0)
							goto L5be5;

						local_ae++;

						// Instruction address 0x1d12:0x5bc9, size: 5
						F0_1d12_692d_CityResources(cityID, local_ea, flag);

						if (this.parent.Var_e8b8 != 0)
						{
							this.parent.Var_e8b8--;
							local_50--;
						}
						goto L5c78;

					L5be5:
						local_ae--;
						local_bc = 0;

						goto L5bf6;

					L5bf2:
						local_bc++;

					L5bf6:
						if (local_bc >= 3)
							goto L5c45;

						// Instruction address 0x1d12:0x5c28, size: 5
						local_cc = F0_1d12_6abc_GetCityResourceCount(this.Var_6548_PlayerID, this.Var_653e_CityID,
							city.Position.X + this.parent.CityOffsets[local_ea].X, city.Position.Y + this.parent.CityOffsets[local_ea].Y, (CityResourceTypeEnum)local_bc);

						this.parent.Var_70da_Arr[local_bc] -= local_cc;

						goto L5bf2;

					L5c45:
						// Instruction address 0x1d12:0x5c69, size: 5
						this.parent.MapManagement.F0_2aea_03ba_DrawCell(
							city.Position.X + this.parent.CityOffsets[local_ea].X,
							city.Position.Y + this.parent.CityOffsets[local_ea].Y);

						this.parent.Var_e8b8++;
						local_50++;

					L5c78:
						city.WorkerFlags &= 0x3ffffff;
						city.WorkerFlags |= (uint)((uint)local_50 << 26);

						if (local_ae > 0) goto L045f;
						goto L12c2;
					}

					// Instruction address 0x1d12:0x5cbd, size: 5
					this.parent.Segment_2459.F0_2459_0918_WaitForKeyPressOrMouseClick();

				L5cc2:
					this.parent.Var_d4cc_MapViewX = local_c2;
					this.parent.Var_d75e_MapViewY = local_d0;
				}

				if (flag != 0)
					goto L68cc;

				if (this.Var_6548_PlayerID == 0)
					goto L62d8;

				local_e8 = this.parent.Var_70e2 - this.parent.Var_70e4;

				if (local_e8 >= 0)
					goto L5f3f;

				if ((this.parent.GameData.PlayerFlags & (1 << this.Var_6548_PlayerID)) == 0)
					goto L5df3;

				// Instruction address 0x1d12:0x5d3b, size: 5
				this.parent.CommonTools.PlayTune(36, 0);

				if (this.parent.GameData.GameSettingFlags.Animations && (city.StatusFlag & 0x1) == 0 && this.parent.Var_6b92 == 0)
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

				if ((city.StatusFlag & 0x1) == 0) goto L5dd7;
				goto L5df3;

			L5dd7:
				this.parent.Var_b1e8 = 1;

				if (this.parent.GameData.GameSettingFlags.InstantAdvice) goto L5de7;
				goto L5df3;

			L5de7:
				this.parent.Help.F4_0000_02d3_ShowInstantAdvicePopup("*DISORDER");

			L5df3:
				if ((city.StatusFlag & 0x1) == 0) goto L5e05;
				goto L5eb4;

			L5e05:
				city.StatusFlag |= 1;

				if ((this.parent.GameData.PlayerFlags & (1 << this.Var_6548_PlayerID)) == 0) goto L5e24;
				goto L5ea1;

			L5e24:
				if (city.CurrentProductionID >= 0) goto L5e36;
				goto L5e54;

			L5e36:
				this.parent.GameData.Players[this.Var_6548_PlayerID].UnitsInProduction[city.CurrentProductionID]--;

			L5e54:
				// Instruction address 0x1d12:0x5e5b, size: 5
				city.CurrentProductionID = (sbyte)((short)this.parent.Segment_1ade.F0_1ade_0421(this.Var_6548_PlayerID, cityID));

				if (city.CurrentProductionID >= 0) goto L5e83;
				goto L5ea1;

			L5e83:
				this.parent.GameData.Players[this.Var_6548_PlayerID].UnitsInProduction[city.CurrentProductionID]++;

			L5ea1:
				// Instruction address 0x1d12:0x5ea9, size: 5
				this.parent.CheckPlayerTurn.F0_1403_3ed7(local_d8, local_e4);

				goto L606c;

			L5eb4:
				if (this.parent.GameData.Players[this.Var_6548_PlayerID].GovernmentType == 5)
				{
					if (this.Var_6548_PlayerID == this.parent.GameData.HumanPlayerID)
					{
						this.parent.CommonTools.PlayTune(36, 0);

						this.parent.News.F21_0000_0000_ShowNews(cityID,
							$"Discontented citizens of\n{this.parent.Segment_2459.F0_2459_08c6_GetCityName(cityID)} revolt:\nGovernment Collapses!\n");

						this.parent.GameData.Players[this.Var_6548_PlayerID].Diplomacy[0] |= DiplomacyFlagsEnum.Allied;

						this.parent.CommonTools.PlayTune(1, 0);
					}

					this.parent.Segment_2517.F0_2517_04a1(this.Var_6548_PlayerID, 0);
				}
				goto L606c;

			L5f3f:
				if (local_e8 < 0)
					goto L604f;

				if ((city.StatusFlag & 0x1) != 0) goto L5f5b;
				goto L604f;

			L5f5b:
				city.StatusFlag &= 0xfe;

				if ((this.parent.GameData.PlayerFlags & (1 << this.Var_6548_PlayerID)) != 0) goto L5f7a;
				goto L5fc2;

			L5f7a:
				this.parent.Var_2f9e_MessageBoxStyle = MenuBoxReportTypeEnum.DomesticAdvisorReport;

				// Instruction address 0x1d12:0x5fb7, size: 5
				this.parent.Segment_1238.F0_1238_001e_ShowDialog($"Order restored\nin {this.parent.Segment_2459.F0_2459_08c6_GetCityName(cityID)}.\n", 100, 80);

				goto L603f;

			L5fc2:
				if (city.CurrentProductionID >= 0) goto L5fd4;
				goto L5ff2;

			L5fd4:
				this.parent.GameData.Players[this.Var_6548_PlayerID].UnitsInProduction[city.CurrentProductionID]--;

			L5ff2:
				// Instruction address 0x1d12:0x5ff9, size: 5
				city.CurrentProductionID = (sbyte)((short)this.parent.Segment_1ade.F0_1ade_0421(this.Var_6548_PlayerID, cityID));

				if (city.CurrentProductionID >= 0) goto L6021;
				goto L603f;

			L6021:
				this.parent.GameData.Players[this.Var_6548_PlayerID].UnitsInProduction[city.CurrentProductionID]++;

			L603f:
				// Instruction address 0x1d12:0x6047, size: 5
				this.parent.CheckPlayerTurn.F0_1403_3ed7(local_d8, local_e4);

			L604f:
				if (this.parent.GameData.Players[this.Var_6548_PlayerID].GovernmentType != 0) goto L605f;
				goto L606c;

			L605f:
				this.parent.GameData.Players[this.Var_6548_PlayerID].Coins += (short)this.parent.Var_e17a;

			L606c:
				if (this.parent.Var_70e4 != 0)
					goto L6213;

				if (city.ActualSize > 2) goto L6088;
				goto L6213;

			L6088:
				if (((city.ActualSize + 1) / 2) > this.parent.Var_70e2)
					goto L6213;

				if (this.parent.GameData.Players[this.Var_6548_PlayerID].GovernmentType != 0) goto L60b4;
				goto L6213;

			L60b4:
				if ((city.StatusFlag & 0x40) == 0) goto L60c6;
				goto L61ac;

			L60c6:
				if ((this.parent.GameData.PlayerFlags & (1 << this.Var_6548_PlayerID)) != 0) goto L60d8;
				goto L619c;

			L60d8:
				if (this.parent.GameData.Players[this.Var_6548_PlayerID].GovernmentType != 0) goto L60e8;
				goto L619c;

			L60e8:
				// Instruction address 0x1d12:0x6143, size: 5
				this.parent.CommonTools.PlayTune(34, 0);

				if (this.parent.GameData.GameSettingFlags.Animations)
				{
					this.parent.CityView.F19_0000_0000_ShowCityLayout(cityID, -2,
						$"'We love the {this.parent.Array_19b2_GovernmentTypeNames[this.parent.GameData.Players[this.Var_6548_PlayerID].GovernmentType]}'\n"+
						$"day celebrated in\n{this.parent.Segment_2459.F0_2459_08c6_GetCityName(cityID)}!\n");

					this.parent.CityView.F19_0000_1ae1_LoveOurLeaderAnimation();

					// Instruction address 0x1d12:0x617d, size: 5
					this.parent.Segment_1238.F0_1238_1b44();
				}
				else
				{
					this.parent.News.F21_0000_0000_ShowNews(cityID,
						$"'We love the {this.parent.Array_19b2_GovernmentTypeNames[this.parent.GameData.Players[this.Var_6548_PlayerID].GovernmentType]}'\n" +
						$"day celebrated in\n{this.parent.Segment_2459.F0_2459_08c6_GetCityName(cityID)}!\n");
				}

				// Instruction address 0x1d12:0x6194, size: 5
				this.parent.CommonTools.PlayTune(1, 0);

			L619c:
				city.StatusFlag |= 0x40;
				goto L6210;

			L61ac:
				if (this.parent.GameData.Players[this.Var_6548_PlayerID].GovernmentType >= 4) goto L61bc;
				goto L6210;

			L61bc:
				if (((city.ActualSize * 2) + (local_48 * this.parent.Var_e3c6)) < this.parent.Var_70da_Arr[0]) goto L61df;
				goto L6210;

			L61df:
				if (city.ActualSize >= 10) goto L61f1;
				goto L6204;

			L61f1:
				if (city.HasImprovement(ImprovementEnum.Aqueduct)) goto L6204;
				goto L6210;

			L6204:
				city.ActualSize++;

			L6210:
				goto L62d8;

			L6213:
				if (local_e8 < 0)
					goto L62d8;

				if ((city.StatusFlag & 0x40) != 0) goto L622f;
				goto L62d8;

			L622f:
				if ((this.parent.GameData.PlayerFlags & (1 << this.Var_6548_PlayerID)) != 0) goto L6241;
				goto L62cb;

			L6241:
				if (this.parent.GameData.Players[this.Var_6548_PlayerID].GovernmentType != 0) goto L6251;
				goto L62cb;

			L6251:
				this.parent.CommonTools.PlayTune(35, 0);

				this.parent.News.F21_0000_0000_ShowNews(cityID,
					$"'We love the {this.parent.Array_19b2_GovernmentTypeNames[this.parent.GameData.Players[this.Var_6548_PlayerID].GovernmentType]}'\n" +
					$"celebration canceled\nin {this.parent.Segment_2459.F0_2459_08c6_GetCityName(cityID)}.\n");

				this.parent.CommonTools.PlayTune(1, 0);

			L62cb:
				city.StatusFlag &= 0xbf;

			L62d8:
				local_e8 = (short)this.parent.Var_70e6;

				if (this.Var_6548_PlayerID != this.parent.GameData.HumanPlayerID)
					goto L635d;

				if (this.parent.GameData.Players[this.parent.GameData.HumanPlayerID].ResearchTechnologyID != -1) goto L62f5;
				goto L630d;

			L62f5:
				// Instruction address 0x1d12:0x62fd, size: 5
				if (!this.parent.Segment_1ade.F0_1ade_22b5_PlayerHasTechnology(this.Var_6548_PlayerID,
					(TechnologyAdvanceEnum)this.parent.GameData.Players[this.parent.GameData.HumanPlayerID].ResearchTechnologyID))
					goto L630d;

				goto L6327;

			L630d:
				if (this.parent.GameData.Players[this.parent.GameData.HumanPlayerID].ResearchTechnologyID == -1) goto L6317;
				goto L635d;

			L6317:
				if (this.parent.GameData.Players[this.Var_6548_PlayerID].ResearchProgress != 0) goto L6327;
				goto L635d;

			L6327:
				this.parent.GameData.Players[this.parent.GameData.HumanPlayerID].ResearchTechnologyID = -1;

				if ((this.parent.GameData.DebugFlags & 0x8) != 0) goto L6337;
				goto L6351;

			L6337:
				// Instruction address 0x1d12:0x6344, size: 5
				this.parent.Segment_1ade.F0_1ade_1584(this.Var_6548_PlayerID, 0);

			L6351:
				this.parent.GameData.Players[this.Var_6548_PlayerID].ResearchProgress = 0;

			L635d:
				if (this.parent.GameData.Players[this.Var_6548_PlayerID].GovernmentType == 0)
				{
					local_e8 = 0;
				}

				this.parent.GameData.Players[this.Var_6548_PlayerID].ResearchProgress += (short)local_e8;

				if (this.Var_6548_PlayerID == this.parent.GameData.HumanPlayerID && this.parent.GameData.DifficultyLevel == 0)
				{
					if (this.parent.GameData.Players[this.parent.GameData.HumanPlayerID].ResearchTechnologyID >= 0 &&
						(this.parent.GameData.TechnologyFirstDiscoveredBy[this.parent.GameData.Players[this.parent.GameData.HumanPlayerID].ResearchTechnologyID] & 7) != 0)
					{
						this.parent.GameData.Players[this.Var_6548_PlayerID].ResearchProgress += (short)local_e8;
					}
				}

				// Instruction address 0x1d12:0x63c2, size: 5
				if (F0_1d12_6c97_PlayerHasWonder(this.Var_6548_PlayerID, WonderEnum.SETIProgram)) goto L63d2;
				goto L63e5;

			L63d2:
				this.parent.GameData.Players[this.Var_6548_PlayerID].ResearchProgress += (short)(local_e8 / 2);

			L63e5:
				if (this.Var_6548_PlayerID != this.parent.GameData.HumanPlayerID)
				{
					local_f4 = 14 - this.parent.GameData.DifficultyLevel;
				}
				else
				{
					local_f4 = (this.parent.GameData.DifficultyLevel * 2) + 6;
				}

				local_f4 += this.parent.Var_d2de;

				if (this.Var_6548_PlayerID != this.parent.GameData.HumanPlayerID)
					goto L6441;

				if ((11 - this.parent.GameData.Players[this.Var_6548_PlayerID].DiscoveredTechnologyCount) <= local_f4)
					goto L6441;

				local_f4 = 11 - this.parent.GameData.Players[this.Var_6548_PlayerID].DiscoveredTechnologyCount;

			L6441:
				if (this.Var_6548_PlayerID != this.parent.GameData.HumanPlayerID)
					goto L6457;

				if (this.parent.Var_b1e8 == 0) goto L6457;
				goto L64b8;

			L6457:
				if ((this.parent.GameData.Players[this.Var_6548_PlayerID].ResearchProgress / ((this.parent.GameData.Year < 0) ? 1 : 2)) >
					(this.parent.GameData.Players[this.Var_6548_PlayerID].DiscoveredTechnologyCount * local_f4)) goto L6488;
				goto L64b8;

			L6488:
				this.parent.GameData.Players[this.Var_6548_PlayerID].ResearchProgress = 0;

				if ((this.parent.GameData.DebugFlags & 0x8) != 0) goto L649e;
				goto L64b8;

			L649e:
				// Instruction address 0x1d12:0x64ab, size: 5
				this.parent.Segment_1ade.F0_1ade_1584(this.Var_6548_PlayerID, 0);

			L64b8:
				this.parent.GameData.Players[this.Var_6548_PlayerID].Score +=
					(short)((city.ActualSize + this.parent.Var_70e2) - this.parent.Var_70e4);

				if (this.Var_6548_PlayerID != this.parent.GameData.HumanPlayerID)
					goto L68cc;

				local_e0 = (this.parent.Var_70da_Arr[1] / this.parent.Var_6c98) - 20;
				local_e0 += (city.ActualSize * this.parent.Var_b882) / 4;

				// Instruction address 0x1d12:0x6530, size: 5
				if ((local_e0 * 2) >
					this.parent.CAPI.RNG.Next(-(((this.parent.GameData.Players[this.Var_6548_PlayerID].DiscoveredTechnologyCount * this.parent.GameData.DifficultyLevel) / 2) - 256))) goto L6545;
				goto L6640;

			L6545:
				// Instruction address 0x1d12:0x6549, size: 5
				local_e8 = this.parent.CAPI.RNG.Next(20);
				local_c6 = this.parent.CityOffsets[local_e8].X + local_d8;
				local_d2 = this.parent.CityOffsets[local_e8].Y + local_e4;

				// Instruction address 0x1d12:0x6581, size: 5
				if (!this.parent.MapManagement.F0_2aea_1585_GetVisibleTerrainImprovements(local_c6, local_d2).HasFlag(TerrainImprovementFlagsEnum.Pollution)) goto L6590;
				goto L6640;

			L6590:
				// Instruction address 0x1d12:0x6598, size: 5
				if (this.parent.MapManagement.GetTerrainType(local_c6, local_d2) != TerrainTypeEnum.Water) goto L65a8;
				goto L6640;

			L65a8:
				// Instruction address 0x1d12:0x65b0, size: 5
				if (!this.parent.MapManagement.F0_2aea_1585_GetVisibleTerrainImprovements(local_c6, local_d2).HasFlag(TerrainImprovementFlagsEnum.City)) goto L65bf;
				goto L6640;

			L65bf:
				// Instruction address 0x1d12:0x65c7, size: 5
				F0_1d12_6d33_AddPollutionToCell(local_c6, local_d2);

				// Instruction address 0x1d12:0x65e3, size: 5
				this.parent.MapManagement.F0_2aea_0008_DrawVisibleMap(this.parent.GameData.HumanPlayerID, local_c6 - 8, local_d2 - 6);

				// Instruction address 0x1d12:0x65f3, size: 5
				this.parent.MapManagement.F0_2aea_11d4_DrawCellWithUnit(local_c6, local_d2);

				this.parent.Var_2f9e_MessageBoxStyle = MenuBoxReportTypeEnum.ScienceAdvisorReport;

				// Instruction address 0x1d12:0x6638, size: 5
				this.parent.Segment_1238.F0_1238_001e_ShowDialog($"Pollution near {this.parent.Segment_2459.F0_2459_08c6_GetCityName(cityID)}.\n", 80, 64);

			L6640:
				if ((city.StatusFlag & 0x1) != 0) goto L6652;
				goto L672d;

			L6652:
				if (city.HasImprovement(ImprovementEnum.NuclearPlant)) goto L6665;
				goto L672d;

			L6665:
				// Instruction address 0x1d12:0x6669, size: 5
				if (this.parent.CAPI.RNG.Next(3) == 0) goto L6679;
				goto L672d;

			L6679:
				// Instruction address 0x1d12:0x6681, size: 5
				if (!this.parent.Segment_1ade.F0_1ade_22b5_PlayerHasTechnology(this.Var_6548_PlayerID, TechnologyAdvanceEnum.FusionPower))
					goto L6691;

				goto L672d;

			L6691:
				// Instruction address 0x1d12:0x66a5, size: 5
				this.parent.MapManagement.F0_2aea_0008_DrawVisibleMap(this.parent.GameData.HumanPlayerID, local_d8 - 8, local_e4 - 6);

				this.parent.Overlay_22.F22_0000_0967(local_d8, local_e4);

				// Instruction address 0x1d12:0x66c5, size: 5
				this.parent.Segment_29f3.F0_29f3_0ec3(local_d8, local_e4);

				city.RemoveImprovement(ImprovementEnum.NuclearPlant);

				this.parent.CommonTools.PlayTune(36, 0);

				this.parent.News.F21_0000_0000_ShowNews(cityID,
					$"Nuclear Catastrophe\nin {this.parent.Segment_2459.F0_2459_08c6_GetCityName(cityID)}!\nContamination feared!\n");

				this.parent.CommonTools.PlayTune(1, 0);

			L672d:
				local_bc = 1;
				goto L673a;

			L6736:
				local_bc++;

			L673a:
				if (local_bc >= 24)
					goto L68cc;

				if (city.HasImprovement(City.BitToImprovementEnum(local_bc))) goto L676e;
				goto L6736;

			L676e:
				if (this.parent.GameData.Players[this.Var_6548_PlayerID].GovernmentType != 0) goto L677e;
				goto L6736;

			L677e:
				this.parent.GameData.Players[this.Var_6548_PlayerID].Coins -= (short)this.parent.GameData.GetImprovementType(local_bc + 1).MaintenanceCost;

				if (local_bc != 1)
					goto L680b;

				if (this.parent.GameData.DifficultyLevel < 2)
					goto L67b3;

				this.parent.GameData.Players[this.Var_6548_PlayerID].Coins--;

			L67b3:
				if (this.parent.GameData.DifficultyLevel < 4)
					goto L67c7;

				this.parent.GameData.Players[this.Var_6548_PlayerID].Coins--;

			L67c7:
				// Instruction address 0x1d12:0x67cf, size: 5
				if (this.parent.Segment_1ade.F0_1ade_22b5_PlayerHasTechnology(this.Var_6548_PlayerID, TechnologyAdvanceEnum.Gunpowder))
					goto L67df;

				goto L67e9;

			L67df:
				this.parent.GameData.Players[this.Var_6548_PlayerID].Coins--;

			L67e9:
				// Instruction address 0x1d12:0x67f1, size: 5
				if (this.parent.Segment_1ade.F0_1ade_22b5_PlayerHasTechnology(this.Var_6548_PlayerID, TechnologyAdvanceEnum.Combustion))
					goto L6801;

				goto L680b;

			L6801:
				this.parent.GameData.Players[this.Var_6548_PlayerID].Coins--;

			L680b:
				if (this.parent.GameData.Players[this.Var_6548_PlayerID].Coins < 0) goto L681b;
				goto L6736;

			L681b:
				this.parent.GameData.Players[this.Var_6548_PlayerID].Coins = 0;
				
				city.RemoveImprovement(City.BitToImprovementEnum(local_bc));
				
				this.parent.Var_2f9e_MessageBoxStyle = MenuBoxReportTypeEnum.DomesticAdvisorReport;

				// Instruction address 0x1d12:0x68a7, size: 5
				this.parent.Segment_1238.F0_1238_001e_ShowDialog(
					$"{this.parent.Segment_2459.F0_2459_08c6_GetCityName(cityID)}\ncan't maintain\n{this.parent.GameData.GetImprovementType(local_bc + 1).Name}.\n", 100, 80);

				this.parent.GameData.Players[this.Var_6548_PlayerID].Coins += (short)(10 * this.parent.GameData.GetImprovementType(local_bc + 1).Cost);

				goto L6736;

			L68cc:
				if ((city.StatusFlag & 0x4) != 0) goto L68de;
				goto L690f;

			L68de:
				// ??? this playerID reference needs to be checked!
				// Instruction address 0x1d12:0x68f4, size: 5
				if (!this.parent.UnitManagement.F0_1866_18d0_IsEnemyUnitNear(city.PlayerID, city.Position.X, city.Position.Y)) goto L6904;
				goto L690f;

			L6904:
				// Instruction address 0x1d12:0x6907, size: 5
				this.parent.UnitManagement.F0_1866_00c6(cityID);

			L690f:
				this.parent.Var_aa_Screen0_Rectangle.FontID = 1;

				// !!! Do we really need to return a value?
				return this.parent.Var_70e2 - this.parent.Var_70e4;
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
			int local_4;
			int local_6 = this.parent.GameData.Cities[cityID].Position.X + this.parent.CityOffsets[cityOffset].X;
			int local_8;
			int local_a = this.parent.GameData.Cities[cityID].Position.Y + this.parent.CityOffsets[cityOffset].Y;
			int local_c;
			int local_e = 0;
			int local_10 = 0;

			if (flag == 1)
			{
				// Instruction address 0x1d12:0x696f, size: 5
				this.parent.MapManagement.F0_2aea_03ba_DrawCell(local_6, local_a);

				local_10 = 0;

				foreach (CityResourceTypeEnum cityResource in Enum.GetValues<CityResourceTypeEnum>())
				{
					// Instruction address 0x1d12:0x698b, size: 3
					local_10 += F0_1d12_6abc_GetCityResourceCount(this.Var_6548_PlayerID, this.Var_653e_CityID, local_6, local_a, cityResource);
				}

				if (local_10 <= 4)
				{
					local_e = 8;
				}
				else if (local_10 <= 6)
				{
					local_e = 5;
				}
				else
				{
					local_e = 3;
				}
			}
			else
			{
				this.Var_2494 = 1;

				// Instruction address 0x1d12:0x69dd, size: 5
				this.parent.MapManagement.SetPlayerLandOwnership(local_6, local_a, this.parent.GameData.Cities[cityID].PlayerID);
			}

			local_8 = 0;
			local_4 = 0;

			foreach (CityResourceTypeEnum cityResource in Enum.GetValues<CityResourceTypeEnum>())
			{
				// Instruction address 0x1d12:0x6a10, size: 3
				local_c = F0_1d12_6abc_GetCityResourceCount(this.Var_6548_PlayerID, this.Var_653e_CityID, local_6, local_a, cityResource);

				this.parent.Var_70da_Arr[(int)cityResource] += local_c;

				while (flag == 1 && local_c > 0)
				{
					// Instruction address 0x1d12:0x6a5d, size: 5
					this.parent.Graphics.F0_VGA_0c3e_DrawBitmapToScreen(this.parent.Var_aa_Screen0_Rectangle,
						161 + (this.parent.CityOffsets[cityOffset].X * 16) + local_4,
						57 + (this.parent.CityOffsets[cityOffset].Y * 16) + local_8,
						this.parent.Array_d4ce[(int)cityResource + 8]);

					if (local_4 < 8)
					{
						local_4 += local_e;
					}
					else
					{
						local_4 = 0;
						local_8 += 8;
					}

					local_c--;
				}
			}

			if (flag == 1 && local_10 == 0)
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
			if (this.parent.MapManagement.F0_2aea_1326_ValidateMapCoordinates(x, y))
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

				if (resourceCount > 2 && (this.parent.GameData.Cities[cityID].StatusFlag & 0x40) == 0)
				{
					if (this.parent.GameData.Players[playerID].GovernmentType <= 1)
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

					if ((this.parent.GameData.Cities[cityID].StatusFlag & 0x40) != 0)
					{
						if (this.parent.GameData.Players[playerID].GovernmentType >= 2)
						{
							resourceCount++;
						}
					}
					else
					{
						if (this.parent.GameData.Players[playerID].GovernmentType >= 4)
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
		private void F0_1d12_6dfe(int cityID, int size)
		{
			//this.CPU.Log.EnterBlock($"F0_1d12_6dfe({cityID}, {size})");

			// function body
			while (this.Var_6542 >= 0 && this.parent.Var_70e4 < this.Var_6542)
			{
				this.Var_6542--;
				this.parent.Var_70e4++;
			}

			// Instruction address 0x1d12:0x6e16, size: 5
			this.parent.Var_70e2 = this.parent.GameTools.F0_2dc4_007c_CheckValueRange(this.parent.Var_70e2, 0, this.parent.GameData.Cities[cityID].ActualSize);
			// Instruction address 0x1d12:0x6e98, size: 5
			this.parent.Var_70e4 = this.parent.GameTools.F0_2dc4_007c_CheckValueRange(this.parent.Var_70e4, 0, this.parent.GameData.Cities[cityID].ActualSize);

			// Instruction address 0x1d12:0x6ebb, size: 5
			while ((this.parent.Var_70e2 + this.parent.Var_70e4) > this.parent.GameTools.F0_2dc4_007c_CheckValueRange(this.parent.GameData.Cities[cityID].ActualSize - size, 0, 99))
			{
				if (this.Var_6542 > 0)
				{
					this.Var_6542--;
				}
				else
				{
					this.parent.Var_70e2--;

					// Instruction address 0x1d12:0x6e74, size: 5
					this.parent.Var_70e2 = this.parent.GameTools.F0_2dc4_007c_CheckValueRange(this.parent.Var_70e2, 0, this.parent.GameData.Cities[cityID].ActualSize);
				}

				this.parent.Var_70e4--;

				// Instruction address 0x1d12:0x6e98, size: 5
				this.parent.Var_70e4 = this.parent.GameTools.F0_2dc4_007c_CheckValueRange(this.parent.Var_70e4, 0, this.parent.GameData.Cities[cityID].ActualSize);
			}
		}

		/// <summary>
		/// ?
		/// </summary>
		/// <param name="cityID"></param>
		/// <param name="xPos"></param>
		/// <param name="yPos"></param>
		/// <param name="param4"></param>
		/// <param name="width"></param>
		/// <returns></returns>
		public int F0_1d12_6ed4_DrawResources(int cityID, int xPos, int yPos, int param4, int width)
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

			for (i = 0; i < this.parent.Var_70e2; i++)
			{
				// Instruction address 0x1d12:0x6f26, size: 5
				this.parent.Graphics.F0_VGA_0c3e_DrawBitmapToScreen(this.parent.Var_aa_Screen0_Rectangle, xPos, yPos, this.parent.Array_6e96[i & 1]);

				xPos += xSpacing;
			}

			for (i = 0; i < this.parent.GameData.Cities[cityID].ActualSize - param4 - this.parent.Var_70e2 - this.parent.Var_70e4; i++)
			{
				// Instruction address 0x1d12:0x6f66, size: 5
				this.parent.Graphics.F0_VGA_0c3e_DrawBitmapToScreen(this.parent.Var_aa_Screen0_Rectangle, xPos, yPos, this.parent.Array_6e96[2 + (i & 1)]);

				xPos += xSpacing;
			}

			if (i != 0)
			{
				xPos += 2;
			}

			for (i = 0; i < this.parent.Var_70e4; i++)
			{
				// Instruction address 0x1d12:0x6fbb, size: 5
				this.parent.Graphics.F0_VGA_0c3e_DrawBitmapToScreen(this.parent.Var_aa_Screen0_Rectangle, xPos, yPos, this.parent.Array_6e96[4 + (i & 1)]);

				if (i < this.Var_6542)
				{
					// Instruction address 0x1d12:0x6fe5, size: 5
					this.parent.Graphics.F0_VGA_009a_ReplaceColor(this.parent.Var_aa_Screen0_Rectangle, xPos, yPos, 12, 14, 5, 12);
				}

				xPos += xSpacing;
			}

			xPos += 4;

			for (i = 0; i < param4; i++)
			{
				// Instruction address 0x1d12:0x7025, size: 5
				this.parent.Graphics.F0_VGA_0c3e_DrawBitmapToScreen(this.parent.Var_aa_Screen0_Rectangle,
					xPos, yPos, this.parent.Array_6e96[5 + F0_1d12_6da1_GetSpecialWorkerFlags(i)]);

				xPos += xSpacing;
			}

			return xSpacing;
		}

		/// <summary>
		/// ?
		/// </summary>
		/// <param name="xSrc"></param>
		/// <param name="xDst"></param>
		/// <param name="yDst"></param>
		public void F0_1d12_7045(int xSrc, int xDst, int yDst)
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
