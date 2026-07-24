using System;
using IRB.VirtualCPU;
using OpenCivOne.Graphics;

namespace OpenCivOne
{
	public class AttackActions
	{
		private OpenCivOneGame oParent;
		private VCPU oCPU;

		public AttackActions(OpenCivOneGame parent)
		{
			this.oParent = parent;
			this.oCPU = parent.CPU;
		}

		/// <summary>
		/// Player 1, unit 1 attacks unit 2 from player 2
		/// </summary>
		/// <param name="playerID1"></param>
		/// <param name="unitID1"></param>
		/// <param name="playerID2"></param>
		/// <param name="unitID2"></param>
		/// <param name="flag"></param>
		/// <returns>Attack result</returns>
		public int F0_29f3_000e_AttackUnit(int playerID1, int unitID1, int playerID2, int unitID2, bool flag)
		{
			//this.oCPU.Log.EnterBlock($"F0_29f3_000e({playerID1}, {unitID1}, {playerID2}, {unitID2}, {flag})");

			// function body
			UnitTypeEnum unit1Type = this.oParent.GameData.Players[playerID1].Units[unitID1].UnitType;
			UnitTypeEnum unit2Type = this.oParent.GameData.Players[playerID2].Units[unitID2].UnitType;

			// Instruction address 0x29f3:0x0052, size: 5
			TerrainTypeEnum defenderTerrainType = this.oParent.MapManagement.GetTerrainType(
				this.oParent.GameData.Players[playerID2].Units[unitID2].Position.X,
				this.oParent.GameData.Players[playerID2].Units[unitID2].Position.Y);

			bool barbarianCityAttack = false;
			int attackStrength = this.oParent.GameData.Units[(int)unit1Type].AttackStrength * 8;
			int defenseStrength;

			if (this.oParent.MapManagement.F0_2aea_1585_GetVisibleTerrainImprovements(
				this.oParent.GameData.Players[playerID2].Units[unitID2].Position.X, this.oParent.GameData.Players[playerID2].Units[unitID2].Position.Y).HasFlag(TerrainImprovementFlagsEnum.Fortress))
			{
				defenseStrength = (this.oParent.GameData.Terrains[(int)defenderTerrainType].DefenseBonus * this.oParent.GameData.Units[(int)unit2Type].DefenseStrength) * 8;
			}
			else if ((this.oParent.GameData.Players[playerID2].Units[unitID2].Status & UnitStatusEnum.Fortified) != UnitStatusEnum.None)
			{
				defenseStrength = (this.oParent.GameData.Terrains[(int)defenderTerrainType].DefenseBonus * this.oParent.GameData.Units[(int)unit2Type].DefenseStrength) * 6;
			}
			else
			{
				defenseStrength = (this.oParent.GameData.Terrains[(int)defenderTerrainType].DefenseBonus * this.oParent.GameData.Units[(int)unit2Type].DefenseStrength) * 4;
			}

			if (playerID1 == 0)
			{
				if (playerID2 == this.oParent.GameData.HumanPlayerID)
				{
					attackStrength = (attackStrength * (this.oParent.GameData.DifficultyLevel + 1)) / 4;
				}
				else
				{
					attackStrength /= 2;
				}
			}

			if (this.oParent.GameData.Units[(int)unit2Type].MovementType == UnitMovementTypeEnum.Air ||
				this.oParent.GameData.Units[(int)unit2Type].MovementType == UnitMovementTypeEnum.Water)
			{
				defenseStrength = this.oParent.GameData.Units[(int)unit2Type].DefenseStrength * 8;
			}

			int attackedCityID = -1;

			if (this.oParent.MapManagement.F0_2aea_1585_GetVisibleTerrainImprovements(
				this.oParent.GameData.Players[playerID2].Units[unitID2].Position.X, 
				this.oParent.GameData.Players[playerID2].Units[unitID2].Position.Y).HasFlag(TerrainImprovementFlagsEnum.City))
			{
				// Instruction address 0x29f3:0x01a1, size: 5
				attackedCityID = this.oParent.Tools.F0_2dc4_00ba_GetCityByLocation(
					this.oParent.GameData.Players[playerID2].Units[unitID2].Position.X, this.oParent.GameData.Players[playerID2].Units[unitID2].Position.Y);

				if (this.oParent.GameData.Cities[attackedCityID].HasImprovement(ImprovementEnum.CityWalls) &&
					this.oParent.GameData.Units[(int)unit1Type].AttackStrength != 12 &&
					this.oParent.GameData.Units[(int)unit2Type].MovementType != UnitMovementTypeEnum.Air)
				{
					defenseStrength = (this.oParent.GameData.Terrains[(int)defenderTerrainType].DefenseBonus * this.oParent.GameData.Units[(int)unit2Type].DefenseStrength) * 12;
				}

				if (playerID1 == 0)
				{
					if (this.oParent.GameData.Players[playerID2].CityCount == 1)
					{
						attackStrength = 0;
					}

					if (this.oParent.GameData.Cities[attackedCityID].HasImprovement(ImprovementEnum.Palace))
					{
						attackStrength /= 2;
					}

					barbarianCityAttack = true;
				}
			}

			if ((this.oParent.GameData.Players[playerID1].Units[unitID1].Status & UnitStatusEnum.Veteran) != UnitStatusEnum.None)
			{
				attackStrength += attackStrength / 2;
			}

			if ((this.oParent.GameData.Players[playerID2].Units[unitID2].Status & UnitStatusEnum.Veteran) != UnitStatusEnum.None)
			{
				defenseStrength += defenseStrength / 2;
			}

			if (this.oParent.GameData.Players[playerID1].Units[unitID1].RemainingMoves < 3)
			{
				attackStrength = (this.oParent.GameData.Players[playerID1].Units[unitID1].RemainingMoves * attackStrength) / 3;
			}

			if (!flag)
			{
				return (attackStrength * 8) / (defenseStrength + 1);
			}

			if (playerID1 != 0)
			{
				if (this.oParent.GameData.DifficultyLevel <= 1 && playerID2 == this.oParent.GameData.HumanPlayerID)
				{
					attackStrength = attackStrength / 2;
				}

				if (this.oParent.GameData.DifficultyLevel == 0 && playerID1 == this.oParent.GameData.HumanPlayerID)
				{
					attackStrength *= 2;
				}
			}

			if (playerID1 == this.oParent.GameData.HumanPlayerID && F0_29f3_0c9e_ConfirmAttackAction(playerID2) == -1)
			{
				this.oParent.GameData.Players[playerID1].Units[unitID1].GoToDestination = OpenCivOneGame.InvalidPosition;

				return -1;
			}
		
			int local_24 = 0;

			if (this.oParent.CAPI.RNG.Next(attackStrength) > this.oParent.CAPI.RNG.Next(defenseStrength))
			{
				local_24 = 16;
			}

			if (barbarianCityAttack && local_24 != 0 && this.oParent.CAPI.RNG.Next(attackStrength) < this.oParent.CAPI.RNG.Next(defenseStrength))
			{
				local_24 = 0;
			}

			if (playerID1 == this.oParent.GameData.HumanPlayerID || playerID2 == this.oParent.GameData.HumanPlayerID ||
				(this.oParent.GameData.Players[playerID1].Units[unitID1].VisibleByPlayer & (0x1 << this.oParent.GameData.HumanPlayerID)) != 0 ||
				(this.oParent.GameData.Players[playerID2].Units[unitID2].VisibleByPlayer & (0x1 << this.oParent.GameData.HumanPlayerID)) != 0 ||
				this.oParent.Var_d806_DebugFlag)
			{
				// Instruction address 0x29f3:0x03de, size: 5
				this.oParent.UnitManagement.F0_1866_16a9_CenterMap(this.oParent.GameData.HumanPlayerID,
					this.oParent.GameData.Players[playerID2].Units[unitID2].Position.X, this.oParent.GameData.Players[playerID2].Units[unitID2].Position.Y);

				// Instruction address 0x29f3:0x0407, size: 5
				if (this.oParent.MapManagement.F0_2aea_03ba_DrawCell(this.oParent.GameData.Players[playerID1].Units[unitID1].Position.X,
					this.oParent.GameData.Players[playerID1].Units[unitID1].Position.Y))
				{
					bool local_16 = false;

					if ((local_24 == 0 && (this.oParent.GameData.Units[(int)unit2Type].AttackStrength > 4 ||
							this.oParent.GameData.Units[(int)unit2Type].DefenseStrength > 2)) ||
						this.oParent.GameData.Units[(int)unit1Type].AttackStrength > 4 ||
						this.oParent.GameData.Units[(int)unit1Type].DefenseStrength > 2)
					{
						local_16 = true;
					}

					if (local_24 != 0 && unit1Type == UnitTypeEnum.Bomber)
					{
						// Instruction address 0x29f3:0x04ae, size: 5
						this.oParent.CommonTools.PlayTune(43, 0);
					}
					else
					{
						if ((local_24 == 0 || playerID1 != this.oParent.GameData.HumanPlayerID) &&
							(local_24 != 0 || playerID2 != this.oParent.GameData.HumanPlayerID))
						{
							if (local_16)
							{
								// Instruction address 0x29f3:0x04ae, size: 5
								this.oParent.CommonTools.PlayTune(41, 0);
							}
							else
							{
								// Instruction address 0x29f3:0x04ae, size: 5
								this.oParent.CommonTools.PlayTune(39, 0);
							}
						}
						else
						{
							if (local_16)
							{
								// Instruction address 0x29f3:0x04ae, size: 5
								this.oParent.CommonTools.PlayTune(40, 0);
							}
							else
							{
								// Instruction address 0x29f3:0x04ae, size: 5
								this.oParent.CommonTools.PlayTune(38, 0);
							}
						}
					}

					// Instruction address 0x29f3:0x04bc, size: 5
					this.oParent.MapManagement.F0_2aea_0e29_DrawUnit(playerID2, unitID2);

					if (this.oParent.GameData.Players[playerID1].Units[unitID1].NextUnitID != -1)
					{
						// Instruction address 0x29f3:0x04e4, size: 5
						this.oParent.MapManagement.F0_2aea_0e29_DrawUnit(playerID1, this.oParent.GameData.Players[playerID1].Units[unitID1].NextUnitID);
					}

					// Instruction address 0x29f3:0x050a, size: 5
					this.oParent.Graphics.F0_VGA_07d8_DrawImage(this.oParent.Var_aa_Screen0_Rectangle, 80, 0, 240, 200, this.oParent.Var_19d4_Screen1_Rectangle, 80, 0);

					// Instruction address 0x29f3:0x052d, size: 5
					int newXWindow = this.oParent.MapManagement.AdjustXPosition(this.oParent.GameData.Players[playerID1].Units[unitID1].Position.X - this.oParent.Var_d4cc_MapViewX) * 16 + 80;
					int newYWindow = (this.oParent.GameData.Players[playerID1].Units[unitID1].Position.Y - this.oParent.Var_d75e_MapViewY) * 16 + 8;

					int xStep = Math.Sign(this.oParent.GameData.Players[playerID2].Units[unitID2].Position.X - this.oParent.GameData.Players[playerID1].Units[unitID1].Position.X);
					int yStep = Math.Sign(this.oParent.GameData.Players[playerID2].Units[unitID2].Position.Y - this.oParent.GameData.Players[playerID1].Units[unitID1].Position.Y);

					// Always false
					if (this.oParent.GameData.Players[playerID2].Units[unitID2].Position.X != 0 ||
						this.oParent.GameData.Players[playerID1].Units[unitID1].Position.X != 79) goto L05a3;

					xStep = -xStep;
					goto L05d9;

				L05a3:
					// Always false
					if (this.oParent.GameData.Players[playerID1].Units[unitID1].Position.X != 0 ||
						this.oParent.GameData.Players[playerID2].Units[unitID2].Position.X != 79) goto L05d9;

					xStep = -xStep;
					goto L05d9;

				L05d9:
					for (int i = 0; i < 11; i++)
					{
						int cellX = (i * xStep) + newXWindow;
						int cellY = (i * yStep) + newYWindow;

						// Instruction address 0x29f3:0x061e, size: 5
						this.oParent.Graphics.F0_VGA_0c3e_DrawBitmapToScreen(this.oParent.Var_aa_Screen0_Rectangle,
							cellX + 1, cellY + 1,
							this.oParent.Array_d4ce[64 + (int)this.oParent.GameData.Players[playerID1].Units[unitID1].UnitType + (playerID1 * 32)]);

						// Instruction address 0x29f3:0x062a, size: 5
						this.oParent.CommonTools.WaitTimer(2);

						// Instruction address 0x29f3:0x0643, size: 5
						this.oParent.Graphics.F0_VGA_07d8_DrawImage(this.oParent.Var_19d4_Screen1_Rectangle,
							cellX, cellY, 16, 16, this.oParent.Var_aa_Screen0_Rectangle, cellX, cellY);
					}

					// Instruction address 0x29f3:0x0670, size: 5
					this.oParent.MapManagement.F0_2aea_11d4_DrawCellWithUnit(
						this.oParent.GameData.Players[playerID1].Units[unitID1].Position.X, this.oParent.GameData.Players[playerID1].Units[unitID1].Position.Y);

					// Instruction address 0x29f3:0x0694, size: 5
					this.oParent.MapManagement.F0_2aea_11d4_DrawCellWithUnit(
						this.oParent.GameData.Players[playerID2].Units[unitID2].Position.X, this.oParent.GameData.Players[playerID2].Units[unitID2].Position.Y);

					// Instruction address 0x29f3:0x06b4, size: 5
					this.oParent.Graphics.F0_VGA_07d8_DrawImage(this.oParent.Var_aa_Screen0_Rectangle, 0, 0, 256, 200, this.oParent.Var_19d4_Screen1_Rectangle, 0, 0);

					for (int i = 0; i < 8; i++)
					{
						int cellX = (local_24 * xStep) + newXWindow;
						int cellY = (local_24 * yStep) + newYWindow;

						// Instruction address 0x29f3:0x06e6, size: 5
						this.oParent.Graphics.F0_VGA_0c3e_DrawBitmapToScreen(this.oParent.Var_aa_Screen0_Rectangle,
							cellX + 1, cellY + 1, this.oParent.Array_d4ce[32 + i]);

						// Instruction address 0x29f3:0x06f2, size: 5
						this.oParent.CommonTools.WaitTimer(4);

						// Instruction address 0x29f3:0x070b, size: 5
						this.oParent.Graphics.F0_VGA_07d8_DrawImage(this.oParent.Var_19d4_Screen1_Rectangle,
							cellX, cellY, 16, 16, this.oParent.Var_aa_Screen0_Rectangle, cellX, cellY);
					}
				}
			}

			this.oParent.GameData.Players[playerID1].Diplomacy[playerID2] |= DiplomacyFlagsEnum.Unknown20;
			this.oParent.GameData.Players[playerID2].Diplomacy[playerID1] |= DiplomacyFlagsEnum.Unknown20;

			if (playerID1 != 0 && playerID2 != 0)
			{
				this.oParent.GameData.PeaceTurnCount = 0;
			}

			if ((playerID2 == this.oParent.GameData.HumanPlayerID &&
				this.oParent.GameData.Players[playerID2].Diplomacy[playerID1].HasFlag(DiplomacyFlagsEnum.Peace)) ||
				this.oParent.GameData.Players[playerID1].Diplomacy[playerID2].HasFlag(DiplomacyFlagsEnum.Unknown200))
			{
				this.oParent.Var_2f9e_MessageBoxStyle = MenuBoxReportTypeEnum.DefenseMinisterReport;

				// Instruction address 0x29f3:0x07c9, size: 5
				this.oParent.Segment_1238.F0_1238_001e_ShowDialog($"Sneak attack by\n{this.oParent.GameData.Players[playerID1].Nationality} forces!\n", 100, 80);

				this.oParent.GameData.Players[playerID1].Diplomacy[playerID2] |= DiplomacyFlagsEnum.Unknown200;
				this.oParent.GameData.Players[playerID1].Diplomacy[playerID2] ^= DiplomacyFlagsEnum.Unknown200;
			}

			if (this.oParent.GameData.Players[playerID2].Diplomacy[playerID1].HasFlag(DiplomacyFlagsEnum.Peace) ||
				this.oParent.GameData.Players[playerID1].Diplomacy[playerID2].HasFlag(DiplomacyFlagsEnum.Peace))
			{
				if (playerID1 == this.oParent.GameData.HumanPlayerID || playerID2 == this.oParent.GameData.HumanPlayerID)
				{
					this.oParent.Var_2f9e_MessageBoxStyle = MenuBoxReportTypeEnum.ForeignMinisterReport;

					// Instruction address 0x29f3:0x0872, size: 5
					this.oParent.Segment_1238.F0_1238_001e_ShowDialog(
						$"{this.oParent.GameData.Players[playerID2].Nation} cancel\npeace treaty\nwith {this.oParent.GameData.Players[playerID1].Nation}.\n", 100, 80);
				}

				if (playerID1 == this.oParent.GameData.HumanPlayerID)
				{
					if (this.oParent.GameData.Players[playerID2].Diplomacy[playerID1].HasFlag(DiplomacyFlagsEnum.Peace))
					{
						this.oParent.GameData.Players[playerID2].Diplomacy[playerID1] |= DiplomacyFlagsEnum.Vendetta;
					}
				}

				// Instruction address 0x29f3:0x08a6, size: 5
				this.oParent.Segment_2517.F0_2517_0aa1_ClearDiplomacyFlags(playerID1, playerID2, DiplomacyFlagsEnum.Peace);
			}

			if (playerID1 == this.oParent.GameData.HumanPlayerID)
			{
				this.oParent.GameData.Players[playerID2].Diplomacy[playerID1] |= DiplomacyFlagsEnum.Unknown200;
				this.oParent.GameData.Players[playerID2].Diplomacy[playerID1] ^= DiplomacyFlagsEnum.Unknown200;
			}

			if (unit1Type == UnitTypeEnum.Nuclear)
			{
				// Instruction address 0x29f3:0x08ed, size: 3
				F0_29f3_0d4d_NuclearAttack(playerID1,
					this.oParent.GameData.Players[playerID2].Units[unitID2].Position.X, this.oParent.GameData.Players[playerID2].Units[unitID2].Position.Y);

				return 1;
			}

			if (local_24 != 0)
			{
				if (playerID2 == 0)
				{
					if (this.oParent.GameData.Players[playerID2].Units[unitID2].UnitType == UnitTypeEnum.Diplomat)
					{
						this.oParent.GameData.Players[playerID1].Coins += 100;

						if (playerID1 == this.oParent.GameData.HumanPlayerID)
						{
							// Instruction address 0x29f3:0x094b, size: 5
							this.oParent.Segment_1238.F0_1238_001e_ShowDialog(this.oParent.LanguageTools.F0_2f4d_044f_GetTextFromKingSection("*LEADER"), 80, 80);
						}
					}
				}

				if (attackedCityID == -1)
				{
					if (!this.oParent.MapManagement.F0_2aea_1585_GetVisibleTerrainImprovements(
						this.oParent.GameData.Players[playerID2].Units[unitID2].Position.X, this.oParent.GameData.Players[playerID2].Units[unitID2].Position.Y).HasFlag(TerrainImprovementFlagsEnum.Fortress))
					{
						int stackUnitCount = this.oParent.UnitManagement.F0_1866_1251_GetStackUnitValueSum(playerID2, unitID2, UnitStackValueTypeEnum.UnitCount);

						this.oParent.GameData.Players[playerID1].UnitsDestroyed[playerID2] += (short)stackUnitCount;

						// Instruction address 0x29f3:0x09ba, size: 5
						this.oParent.UnitManagement.F0_1866_0f10_DeleteUnitStack(playerID2, unitID2);

						if (stackUnitCount > 1)
						{
							if (playerID1 == this.oParent.GameData.HumanPlayerID || playerID2 == this.oParent.GameData.HumanPlayerID)
							{
								// Instruction address 0x29f3:0x0a19, size: 5
								this.oParent.Segment_1238.F0_1238_001e_ShowDialog($"{stackUnitCount} units destroyed.\n", 80, 80);
							}
						}
					}
					else
					{
						// Instruction address 0x29f3:0x0ad8, size: 5
						this.oParent.UnitManagement.F0_1866_0f10_DeleteUnit(playerID2, unitID2);

						this.oParent.GameData.Players[playerID1].UnitsDestroyed[playerID2]++;
					}
				}
				else
				{
					if (this.oParent.GameData.Players[playerID2].Units[unitID2].NextUnitID == -1)
					{
						for (int i = 8; i >= 0; i--)
						{
							GPoint direction = this.oParent.MoveDirections[i];

							int newX = this.oParent.GameData.Players[playerID2].Units[unitID2].Position.X + direction.X;
							int newY = this.oParent.GameData.Players[playerID2].Units[unitID2].Position.Y + direction.Y;

							// Instruction address 0x29f3:0x0a7f, size: 5
							int activeUnitID = this.oParent.MapManagement.F0_2aea_1458_GetCellActiveUnitID(newX, newY);
							int activeUnitPlayerID = this.oParent.MapManagement.F0_2aea_14e0_GetCellActiveUnitPlayerID(newX, newY);

							if (activeUnitID != -1 && activeUnitPlayerID != this.oParent.GameData.HumanPlayerID)
							{
								// Instruction address 0x29f3:0x0aa7, size: 5
								this.oParent.UnitManagement.F0_1866_1593_UnitStack(activeUnitPlayerID, activeUnitID);
							}
						}

						if (this.oParent.GameData.Cities[attackedCityID].PlayerID == this.oParent.GameData.HumanPlayerID)
						{
							// Instruction address 0x29f3:0x0aca, size: 5
							this.oParent.AIEngine.F0_25fb_3e9c_MoveTransportNearEnemyCity(attackedCityID);
						}
					}

					// Instruction address 0x29f3:0x0ad8, size: 5
					this.oParent.UnitManagement.F0_1866_0f10_DeleteUnit(playerID2, unitID2);

					this.oParent.GameData.Players[playerID1].UnitsDestroyed[playerID2]++;
				}

				if (this.oParent.CAPI.RNG.Next(2) == 0)
				{
					return 1;
				}

				this.oParent.GameData.Players[playerID1].Units[unitID1].Status |= UnitStatusEnum.Veteran;

				return 1;
			}

			this.oParent.GameData.Players[playerID2].UnitsDestroyed[playerID1]++;

			// Instruction address 0x29f3:0x0b31, size: 5
			this.oParent.UnitManagement.F0_1866_0f10_DeleteUnit(playerID1, unitID1);

			if (this.oParent.CAPI.RNG.Next(2) != 0)
			{
				this.oParent.GameData.Players[playerID2].Units[unitID2].Status |= UnitStatusEnum.Veteran;
			}
		
			return 0;
		}

		/// <summary>
		/// Adds unit to new or existing stack
		/// !!! To Do: Check if stack overflows!
		/// </summary>
		/// <param name="playerID"></param>
		/// <param name="unitID"></param>
		/// <param name="nextUnitID"></param>
		public void F0_29f3_0b66_AddUnitToStack(int playerID, int unitID, int nextUnitID)
		{
			//this.oCPU.Log.EnterBlock($"F0_29f3_0b66({playerID}, {unitID}, {nextUnitID})");

			// function body
			if (this.oParent.GameData.Players[playerID].Units[nextUnitID].NextUnitID == -1)
			{
				this.oParent.GameData.Players[playerID].Units[nextUnitID].NextUnitID = (short)unitID;
				this.oParent.GameData.Players[playerID].Units[unitID].NextUnitID = (short)nextUnitID;
			}
			else
			{
				this.oParent.GameData.Players[playerID].Units[unitID].NextUnitID = this.oParent.GameData.Players[playerID].Units[nextUnitID].NextUnitID;
				this.oParent.GameData.Players[playerID].Units[nextUnitID].NextUnitID = (short)unitID;
			}
		}

		/// <summary>
		/// Remove unit from stack
		/// </summary>
		/// <param name="playerID"></param>
		/// <param name="unitID"></param>
		public void F0_29f3_0bc9_RemoveUnitFromStack(int playerID, int unitID)
		{
			//this.oCPU.Log.EnterBlock($"F0_29f3_0bc9({playerID}, {unitID})");

			// function body
			short nextUnitID = this.oParent.GameData.Players[playerID].Units[unitID].NextUnitID;

			if (this.oParent.GameData.Players[playerID].Units[unitID].NextUnitID != -1)
			{
				if (this.oParent.GameData.Players[playerID].Units[nextUnitID].NextUnitID == unitID)
				{
					this.oParent.GameData.Players[playerID].Units[nextUnitID].NextUnitID = -1;
				}
				else
				{
					int count = 0;

					do
					{
						nextUnitID = this.oParent.GameData.Players[playerID].Units[nextUnitID].NextUnitID;
						count++;
					} while (nextUnitID != -1 && count < 32 && this.oParent.GameData.Players[playerID].Units[nextUnitID].NextUnitID != unitID);

					if (count < 32 && nextUnitID != -1)
					{
						this.oParent.GameData.Players[playerID].Units[nextUnitID].NextUnitID = this.oParent.GameData.Players[playerID].Units[unitID].NextUnitID;
					}
				}

				this.oParent.GameData.Players[playerID].Units[unitID].NextUnitID = -1;
			}
		}

		/// <summary>
		/// Confirms attack action against leader with whom there is a peace treaty
		/// </summary>
		/// <param name="playerID">The player which is attacked</param>
		/// <returns></returns>
		public int F0_29f3_0c9e_ConfirmAttackAction(int playerID)
		{
			//this.oCPU.Log.EnterBlock($"F0_29f3_0c9e({playerID})");

			// function body
			if (this.oParent.GameData.Players[this.oParent.GameData.HumanPlayerID].Diplomacy[playerID].HasFlag(DiplomacyFlagsEnum.Peace))
			{
				this.oParent.Var_2f9e_MessageBoxStyle = MenuBoxReportTypeEnum.ForeignMinisterReport;

				if (this.oParent.Segment_1238.F0_1238_001e_ShowDialog(
					$"We have signed a\npeace treaty with\nthe {this.oParent.GameData.Players[playerID].Nation}!\n Cancel action.\n Break treaty.\n", 80, 80) != 1)
				{
					return -1;
				}

				if (this.oParent.GameData.Players[this.oParent.GameData.HumanPlayerID].GovernmentType > 3)
				{
					this.oParent.Var_2f9e_MessageBoxStyle = MenuBoxReportTypeEnum.DomesticAdvisorReport;

					// Instruction address 0x29f3:0x0d39, size: 5
					this.oParent.Segment_1238.F0_1238_001e_ShowDialog("Overruled by\nthe Senate.\nAction canceled.\n", 100, 80);

					return -1;
				}

				return 1;
			}

			return 0;
		}

		/// <summary>
		/// Attack with nuclear weapon
		/// </summary>
		/// <param name="playerID"></param>
		/// <param name="x"></param>
		/// <param name="y"></param>
		public void F0_29f3_0d4d_NuclearAttack(int playerID, int x, int y)
		{
			//this.oCPU.Log.EnterBlock($"F0_29f3_0d4d({playerID}, {x}, {y})");

			// function body			
			if (playerID != this.oParent.GameData.HumanPlayerID)
			{
				this.oParent.Var_2f9e_MessageBoxStyle = MenuBoxReportTypeEnum.DefenseMinisterReport;

				// Instruction address 0x29f3:0x0d93, size: 5
				this.oParent.Segment_1238.F0_1238_001e_ShowDialog($"{this.oParent.GameData.Players[playerID].Nation} use\nnuclear weapons!\n", 100, 80);
			}

			this.oParent.Overlay_22.F22_0000_0967(x, y);

			for (int i = 8; i >= 0; i--)
			{
				GPoint direction = this.oParent.MoveDirections[i];

				int newX = this.oParent.MapManagement.AdjustXPosition(x + direction.X);
				int newY = y + direction.Y;

				if (this.oParent.MapManagement.ValidateMapCoordinates(newX, newY))
				{
					// Instruction address 0x29f3:0x0dcb, size: 5
					int activeUnitID = this.oParent.MapManagement.F0_2aea_1458_GetCellActiveUnitID(newX, newY);
					int activeUnitPlayerID = this.oParent.MapManagement.F0_2aea_14e0_GetCellActiveUnitPlayerID(newX, newY);

					if (this.oParent.MapManagement.F0_2aea_1585_GetVisibleTerrainImprovements(newX, newY).HasFlag(TerrainImprovementFlagsEnum.City))
					{
						// Instruction address 0x29f3:0x0dee, size: 5
						int cityID = this.oParent.Tools.F0_2dc4_00ba_GetCityByLocation(newX, newY);

						if (this.oParent.GameData.Cities[cityID].HasImprovement(ImprovementEnum.SDIDefense))
						{
							// Instruction address 0x29f3:0x0e40, size: 5
							this.oParent.Segment_1238.F0_1238_001e_ShowDialog($"SDI protects {this.oParent.Segment_2459.F0_2459_08c6_GetCityName(cityID)}.\n", 80, 100);

							activeUnitID = -1;
						}
					}

					if (activeUnitID != -1)
					{
						// Instruction address 0x29f3:0x0e5e, size: 5
						int unitCount = this.oParent.UnitManagement.F0_1866_1251_GetStackUnitValueSum(activeUnitPlayerID, activeUnitID, UnitStackValueTypeEnum.UnitCount);

						this.oParent.GameData.Players[playerID].UnitsDestroyed[activeUnitPlayerID] += (short)unitCount;

						// Instruction address 0x29f3:0x0e89, size: 5
						this.oParent.UnitManagement.F0_1866_0f10_DeleteUnitStack(activeUnitPlayerID, activeUnitID);

						if (playerID != activeUnitPlayerID)
						{
							this.oParent.GameData.Players[activeUnitPlayerID].Diplomacy[playerID] |= DiplomacyFlagsEnum.Vendetta | DiplomacyFlagsEnum.Unknown80;
						}
					}
				}
			}

			// Instruction address 0x29f3:0x0eb8, size: 3
			F0_29f3_0ec3_AddEffectsOfNuclearAttackOrCatastrophe(x, y);
		}

		/// <summary>
		/// Add effects of nuclear attack or nuclear catastrophe to affected surrounding cells
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		public void F0_29f3_0ec3_AddEffectsOfNuclearAttackOrCatastrophe(int x, int y)
		{
			//this.oCPU.Log.EnterBlock($"F0_29f3_0ec3({xPos}, {yPos})");

			// function body
			// !!! To Do: Effects of nuclear attack should exceed more than one cell in each direction
			for (int i = 0; i < 9; i++)
			{
				GPoint direction = this.oParent.MoveDirections[i];

				// Instruction address 0x29f3:0x0f26, size: 5
				int newX = this.oParent.MapManagement.AdjustXPosition(x + direction.X);
				int newY = y + direction.Y;

				if (this.oParent.MapManagement.ValidateMapCoordinates(newX, newY))
				{
					if (this.oParent.MapManagement.F0_2aea_1585_GetVisibleTerrainImprovements(newX, newY).HasFlag(TerrainImprovementFlagsEnum.City))
					{
						// Instruction address 0x29f3:0x0f51, size: 5
						int cityID = this.oParent.Tools.F0_2dc4_00ba_GetCityByLocation(newX, newY);

						this.oParent.GameData.Cities[cityID].ActualSize -= (sbyte)(this.oParent.GameData.Cities[cityID].ActualSize / 2);
					}
					else
					{
						if (this.oParent.MapManagement.GetTerrainType(newX, newY) != TerrainTypeEnum.Water && this.oParent.CAPI.RNG.Next(3) != 0)
						{
							// Instruction address 0x29f3:0x0efa, size: 5
							this.oParent.CityWorker.F0_1d12_6d33_AddPollutionToCell(newX, newY);
						}
					}

					// Instruction address 0x29f3:0x0f08, size: 5
					this.oParent.MapManagement.F0_2aea_11d4_DrawCellWithUnit(newX, newY);
				}
			}
		}
	}
}
