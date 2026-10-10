using OpenCivOne.Graphics;

namespace OpenCivOne
{
	public class AttackActions
	{
		private OpenCivOneGame parent;

		public AttackActions(OpenCivOneGame parent)
		{
			this.parent = parent;
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
			UnitTypeEnum unit1Type = this.parent.GameData.Players[playerID1].Units[unitID1].UnitType;
			UnitTypeEnum unit2Type = this.parent.GameData.Players[playerID2].Units[unitID2].UnitType;

			// Instruction address 0x29f3:0x0052, size: 5
			TerrainTypeEnum defenderTerrainType = this.parent.MapManagement.GetTerrainType(
				this.parent.GameData.Players[playerID2].Units[unitID2].Position.X,
				this.parent.GameData.Players[playerID2].Units[unitID2].Position.Y);

			bool barbarianCityAttack = false;
			int attackStrength = this.parent.GameData.Units[(int)unit1Type].AttackStrength * 8;
			int defenseStrength;

			if (this.parent.MapManagement.F0_2aea_1585_GetVisibleTerrainImprovements(
				this.parent.GameData.Players[playerID2].Units[unitID2].Position.X, this.parent.GameData.Players[playerID2].Units[unitID2].Position.Y).HasFlag(TerrainImprovementFlagsEnum.Fortress))
			{
				defenseStrength = (this.parent.GameData.Terrains[(int)defenderTerrainType].DefenseBonus * this.parent.GameData.Units[(int)unit2Type].DefenseStrength) * 8;
			}
			else if ((this.parent.GameData.Players[playerID2].Units[unitID2].Status & UnitStatusEnum.Fortified) != UnitStatusEnum.None)
			{
				defenseStrength = (this.parent.GameData.Terrains[(int)defenderTerrainType].DefenseBonus * this.parent.GameData.Units[(int)unit2Type].DefenseStrength) * 6;
			}
			else
			{
				defenseStrength = (this.parent.GameData.Terrains[(int)defenderTerrainType].DefenseBonus * this.parent.GameData.Units[(int)unit2Type].DefenseStrength) * 4;
			}

			if (playerID1 == 0)
			{
				if (playerID2 == this.parent.GameData.HumanPlayerID)
				{
					attackStrength = (attackStrength * (this.parent.GameData.DifficultyLevel + 1)) / 4;
				}
				else
				{
					attackStrength /= 2;
				}
			}

			if (this.parent.GameData.Units[(int)unit2Type].MovementType == UnitMovementTypeEnum.Air ||
				this.parent.GameData.Units[(int)unit2Type].MovementType == UnitMovementTypeEnum.Water)
			{
				defenseStrength = this.parent.GameData.Units[(int)unit2Type].DefenseStrength * 8;
			}

			int attackedCityID = -1;

			if (this.parent.MapManagement.F0_2aea_1585_GetVisibleTerrainImprovements(
				this.parent.GameData.Players[playerID2].Units[unitID2].Position.X, 
				this.parent.GameData.Players[playerID2].Units[unitID2].Position.Y).HasFlag(TerrainImprovementFlagsEnum.City))
			{
				// Instruction address 0x29f3:0x01a1, size: 5
				attackedCityID = this.parent.MapManagement.GetCityByLocation(
					this.parent.GameData.Players[playerID2].Units[unitID2].Position.X, this.parent.GameData.Players[playerID2].Units[unitID2].Position.Y);

				if (this.parent.GameData.Cities[attackedCityID].HasImprovement(ImprovementEnum.CityWalls) &&
					this.parent.GameData.Units[(int)unit1Type].AttackStrength != 12 &&
					this.parent.GameData.Units[(int)unit2Type].MovementType != UnitMovementTypeEnum.Air)
				{
					defenseStrength = (this.parent.GameData.Terrains[(int)defenderTerrainType].DefenseBonus * this.parent.GameData.Units[(int)unit2Type].DefenseStrength) * 12;
				}

				if (playerID1 == 0)
				{
					if (this.parent.GameData.Players[playerID2].CityCount == 1)
					{
						attackStrength = 0;
					}

					if (this.parent.GameData.Cities[attackedCityID].HasImprovement(ImprovementEnum.Palace))
					{
						attackStrength /= 2;
					}

					barbarianCityAttack = true;
				}
			}

			if ((this.parent.GameData.Players[playerID1].Units[unitID1].Status & UnitStatusEnum.Veteran) != UnitStatusEnum.None)
			{
				attackStrength += attackStrength / 2;
			}

			if ((this.parent.GameData.Players[playerID2].Units[unitID2].Status & UnitStatusEnum.Veteran) != UnitStatusEnum.None)
			{
				defenseStrength += defenseStrength / 2;
			}

			if (this.parent.GameData.Players[playerID1].Units[unitID1].RemainingMoves < 3)
			{
				attackStrength = (this.parent.GameData.Players[playerID1].Units[unitID1].RemainingMoves * attackStrength) / 3;
			}

			if (!flag)
			{
				return (attackStrength * 8) / (defenseStrength + 1);
			}

			if (playerID1 != 0)
			{
				if (this.parent.GameData.DifficultyLevel <= 1 && playerID2 == this.parent.GameData.HumanPlayerID)
				{
					attackStrength = attackStrength / 2;
				}

				if (this.parent.GameData.DifficultyLevel == 0 && playerID1 == this.parent.GameData.HumanPlayerID)
				{
					attackStrength *= 2;
				}
			}

			if (playerID1 == this.parent.GameData.HumanPlayerID && F0_29f3_0c9e_ConfirmAttackAction(playerID2) == -1)
			{
				this.parent.GameData.Players[playerID1].Units[unitID1].GoToDestination = OpenCivOneGame.InvalidPosition;

				return -1;
			}
		
			int local_24 = 0;

			if (this.parent.CAPI.RNG.Next(attackStrength) > this.parent.CAPI.RNG.Next(defenseStrength))
			{
				local_24 = 16;
			}

			if (barbarianCityAttack && local_24 != 0 && this.parent.CAPI.RNG.Next(attackStrength) < this.parent.CAPI.RNG.Next(defenseStrength))
			{
				local_24 = 0;
			}

			if (playerID1 == this.parent.GameData.HumanPlayerID || playerID2 == this.parent.GameData.HumanPlayerID ||
				(this.parent.GameData.Players[playerID1].Units[unitID1].VisibleByPlayer & (0x1 << this.parent.GameData.HumanPlayerID)) != 0 ||
				(this.parent.GameData.Players[playerID2].Units[unitID2].VisibleByPlayer & (0x1 << this.parent.GameData.HumanPlayerID)) != 0 ||
				this.parent.GlobalPreferences.Debug.DebugMode)
			{
				// Instruction address 0x29f3:0x03de, size: 5
				this.parent.MapManagement.F0_1866_16a9_CenterMap(this.parent.GameData.HumanPlayerID,
					this.parent.GameData.Players[playerID2].Units[unitID2].Position.X, this.parent.GameData.Players[playerID2].Units[unitID2].Position.Y);

				// Instruction address 0x29f3:0x0407, size: 5
				if (this.parent.MapManagement.F0_2aea_03ba_DrawCell(this.parent.GameData.Players[playerID1].Units[unitID1].Position.X,
					this.parent.GameData.Players[playerID1].Units[unitID1].Position.Y))
				{
					bool local_16 = false;

					if ((local_24 == 0 && (this.parent.GameData.Units[(int)unit2Type].AttackStrength > 4 ||
							this.parent.GameData.Units[(int)unit2Type].DefenseStrength > 2)) ||
						this.parent.GameData.Units[(int)unit1Type].AttackStrength > 4 ||
						this.parent.GameData.Units[(int)unit1Type].DefenseStrength > 2)
					{
						local_16 = true;
					}

					if (local_24 != 0 && unit1Type == UnitTypeEnum.Bomber)
					{
						// Instruction address 0x29f3:0x04ae, size: 5
						this.parent.CommonTools.PlayTune(43, 0);
					}
					else
					{
						if ((local_24 == 0 || playerID1 != this.parent.GameData.HumanPlayerID) &&
							(local_24 != 0 || playerID2 != this.parent.GameData.HumanPlayerID))
						{
							if (local_16)
							{
								// Instruction address 0x29f3:0x04ae, size: 5
								this.parent.CommonTools.PlayTune(41, 0);
							}
							else
							{
								// Instruction address 0x29f3:0x04ae, size: 5
								this.parent.CommonTools.PlayTune(39, 0);
							}
						}
						else
						{
							if (local_16)
							{
								// Instruction address 0x29f3:0x04ae, size: 5
								this.parent.CommonTools.PlayTune(40, 0);
							}
							else
							{
								// Instruction address 0x29f3:0x04ae, size: 5
								this.parent.CommonTools.PlayTune(38, 0);
							}
						}
					}

					// Instruction address 0x29f3:0x04bc, size: 5
					this.parent.MapManagement.F0_2aea_0e29_DrawUnit(playerID2, unitID2);

					if (this.parent.GameData.Players[playerID1].Units[unitID1].NextUnitID != -1)
					{
						// Instruction address 0x29f3:0x04e4, size: 5
						this.parent.MapManagement.F0_2aea_0e29_DrawUnit(playerID1, this.parent.GameData.Players[playerID1].Units[unitID1].NextUnitID);
					}

					// Instruction address 0x29f3:0x050a, size: 5
					this.parent.Graphics.F0_VGA_07d8_DrawImage(this.parent.Var_aa_Screen0_Rectangle, 80, 0, 240, 200, this.parent.Var_19d4_Screen1_Rectangle, 80, 0);

					// Instruction address 0x29f3:0x052d, size: 5
					int newXWindow = this.parent.MapManagement.AdjustXPosition(this.parent.GameData.Players[playerID1].Units[unitID1].Position.X - this.parent.Var_d4cc_MapViewX) * 16 + 80;
					int newYWindow = (this.parent.GameData.Players[playerID1].Units[unitID1].Position.Y - this.parent.Var_d75e_MapViewY) * 16 + 8;

					int xStep = Math.Sign(this.parent.GameData.Players[playerID2].Units[unitID2].Position.X - this.parent.GameData.Players[playerID1].Units[unitID1].Position.X);
					int yStep = Math.Sign(this.parent.GameData.Players[playerID2].Units[unitID2].Position.Y - this.parent.GameData.Players[playerID1].Units[unitID1].Position.Y);

					if ((this.parent.GameData.Players[playerID1].Units[unitID1].Position.X == 0 &&
						this.parent.GameData.Players[playerID2].Units[unitID2].Position.X == 79) ||
						(this.parent.GameData.Players[playerID2].Units[unitID2].Position.X == 0 &&
						this.parent.GameData.Players[playerID1].Units[unitID1].Position.X == 79))
					{
						xStep = -xStep;
					}

					if (this.parent.GlobalPreferences.Debug.FastMode)
					{
						for (int i = 0; i < 11; i += 2)
						{
							int cellX = newXWindow + (i * xStep);
							int cellY = newYWindow + (i * yStep);

							// Instruction address 0x29f3:0x061e, size: 5
							this.parent.Graphics.F0_VGA_0c3e_DrawBitmapToScreen(this.parent.Var_aa_Screen0_Rectangle,
								cellX + 1, cellY + 1,
								this.parent.Array_d4ce[64 + (int)this.parent.GameData.Players[playerID1].Units[unitID1].UnitType + (playerID1 * 32)]);

							// Instruction address 0x29f3:0x062a, size: 5
							this.parent.CommonTools.WaitTimer(1);

							// Instruction address 0x29f3:0x0643, size: 5
							this.parent.Graphics.F0_VGA_07d8_DrawImage(this.parent.Var_19d4_Screen1_Rectangle,
								cellX, cellY, 16, 16, this.parent.Var_aa_Screen0_Rectangle, cellX, cellY);
						}
					}
					else
					{
						for (int i = 0; i < 11; i++)
						{
							int cellX = newXWindow + (i * xStep);
							int cellY = newYWindow + (i * yStep);

							// Instruction address 0x29f3:0x061e, size: 5
							this.parent.Graphics.F0_VGA_0c3e_DrawBitmapToScreen(this.parent.Var_aa_Screen0_Rectangle,
								cellX + 1, cellY + 1,
								this.parent.Array_d4ce[64 + (int)this.parent.GameData.Players[playerID1].Units[unitID1].UnitType + (playerID1 * 32)]);

							// Instruction address 0x29f3:0x062a, size: 5
							this.parent.CommonTools.WaitTimer(2);

							// Instruction address 0x29f3:0x0643, size: 5
							this.parent.Graphics.F0_VGA_07d8_DrawImage(this.parent.Var_19d4_Screen1_Rectangle,
								cellX, cellY, 16, 16, this.parent.Var_aa_Screen0_Rectangle, cellX, cellY);
						}
					}

					// Instruction address 0x29f3:0x0670, size: 5
					this.parent.MapManagement.F0_2aea_11d4_DrawCellWithUnit(
						this.parent.GameData.Players[playerID1].Units[unitID1].Position.X, this.parent.GameData.Players[playerID1].Units[unitID1].Position.Y);

					// Instruction address 0x29f3:0x0694, size: 5
					this.parent.MapManagement.F0_2aea_11d4_DrawCellWithUnit(
						this.parent.GameData.Players[playerID2].Units[unitID2].Position.X, this.parent.GameData.Players[playerID2].Units[unitID2].Position.Y);

					// Instruction address 0x29f3:0x06b4, size: 5
					this.parent.Graphics.F0_VGA_07d8_DrawImage(this.parent.Var_aa_Screen0_Rectangle, 0, 0, 256, 200, this.parent.Var_19d4_Screen1_Rectangle, 0, 0);

					if (this.parent.GlobalPreferences.Debug.FastMode)
					{
						for (int i = 0; i < 8; i += 2)
						{
							int cellX = newXWindow + (i * xStep);
							int cellY = newYWindow + (i * yStep);

							// Instruction address 0x29f3:0x06e6, size: 5
							this.parent.Graphics.F0_VGA_0c3e_DrawBitmapToScreen(this.parent.Var_aa_Screen0_Rectangle,
								cellX + 1, cellY + 1, this.parent.Array_d4ce[32 + i]);

							// Instruction address 0x29f3:0x06f2, size: 5
							this.parent.CommonTools.WaitTimer(1);

							// Instruction address 0x29f3:0x070b, size: 5
							this.parent.Graphics.F0_VGA_07d8_DrawImage(this.parent.Var_19d4_Screen1_Rectangle,
								cellX, cellY, 16, 16, this.parent.Var_aa_Screen0_Rectangle, cellX, cellY);
						}
					}
					else
					{
						for (int i = 0; i < 8; i++)
						{
							int cellX = newXWindow + (i * xStep);
							int cellY = newYWindow + (i * yStep);

							// Instruction address 0x29f3:0x06e6, size: 5
							this.parent.Graphics.F0_VGA_0c3e_DrawBitmapToScreen(this.parent.Var_aa_Screen0_Rectangle,
								cellX + 1, cellY + 1, this.parent.Array_d4ce[32 + i]);

							// Instruction address 0x29f3:0x06f2, size: 5
							this.parent.CommonTools.WaitTimer(4);

							// Instruction address 0x29f3:0x070b, size: 5
							this.parent.Graphics.F0_VGA_07d8_DrawImage(this.parent.Var_19d4_Screen1_Rectangle,
								cellX, cellY, 16, 16, this.parent.Var_aa_Screen0_Rectangle, cellX, cellY);
						}
					}
				}
			}

			this.parent.GameData.Players[playerID1].Diplomacy[playerID2] |= DiplomacyFlagsEnum.EverFought;
			this.parent.GameData.Players[playerID2].Diplomacy[playerID1] |= DiplomacyFlagsEnum.EverFought;

			if (playerID1 != 0 && playerID2 != 0)
			{
				this.parent.GameData.PeaceTurnCount = 0;
			}

			if ((playerID2 == this.parent.GameData.HumanPlayerID &&
				this.parent.GameData.Players[playerID2].Diplomacy[playerID1].HasFlag(DiplomacyFlagsEnum.Peace)) ||
				this.parent.GameData.Players[playerID1].Diplomacy[playerID2].HasFlag(DiplomacyFlagsEnum.WarDeclared))
			{
				this.parent.Var_2f9e_MessageBoxStyle = MenuBoxReportTypeEnum.DefenseMinisterReport;

				// Instruction address 0x29f3:0x07c9, size: 5
				this.parent.MenuBoxDialog.ShowDialog($"Sneak attack by\n{this.parent.GameData.Players[playerID1].Nationality} forces!\n", 100, 80);

				this.parent.GameData.Players[playerID1].Diplomacy[playerID2] |= DiplomacyFlagsEnum.WarDeclared;
				this.parent.GameData.Players[playerID1].Diplomacy[playerID2] ^= DiplomacyFlagsEnum.WarDeclared;
			}

			if (this.parent.GameData.Players[playerID2].Diplomacy[playerID1].HasFlag(DiplomacyFlagsEnum.Peace) ||
				this.parent.GameData.Players[playerID1].Diplomacy[playerID2].HasFlag(DiplomacyFlagsEnum.Peace))
			{
				if (playerID1 == this.parent.GameData.HumanPlayerID || playerID2 == this.parent.GameData.HumanPlayerID)
				{
					this.parent.Var_2f9e_MessageBoxStyle = MenuBoxReportTypeEnum.ForeignMinisterReport;

					// Instruction address 0x29f3:0x0872, size: 5
					this.parent.MenuBoxDialog.ShowDialog(
						$"{this.parent.GameData.Players[playerID2].Nation} cancel\npeace treaty\nwith {this.parent.GameData.Players[playerID1].Nation}.\n", 100, 80);
				}

				if (playerID1 == this.parent.GameData.HumanPlayerID)
				{
					if (this.parent.GameData.Players[playerID2].Diplomacy[playerID1].HasFlag(DiplomacyFlagsEnum.Peace))
					{
						this.parent.GameData.Players[playerID2].Diplomacy[playerID1] |= DiplomacyFlagsEnum.Vendetta;
					}
				}

				// Instruction address 0x29f3:0x08a6, size: 5
				this.parent.Diplomacy.F0_2517_0aa1_ClearDiplomacyFlags(playerID1, playerID2, DiplomacyFlagsEnum.Peace);
			}

			if (playerID1 == this.parent.GameData.HumanPlayerID)
			{
				this.parent.GameData.Players[playerID2].Diplomacy[playerID1] |= DiplomacyFlagsEnum.WarDeclared;
				this.parent.GameData.Players[playerID2].Diplomacy[playerID1] ^= DiplomacyFlagsEnum.WarDeclared;
			}

			if (unit1Type == UnitTypeEnum.Nuclear)
			{
				// Instruction address 0x29f3:0x08ed, size: 3
				F0_29f3_0d4d_NuclearAttack(playerID1,
					this.parent.GameData.Players[playerID2].Units[unitID2].Position.X, this.parent.GameData.Players[playerID2].Units[unitID2].Position.Y);

				return 1;
			}

			if (local_24 != 0)
			{
				if (playerID2 == 0)
				{
					if (this.parent.GameData.Players[playerID2].Units[unitID2].UnitType == UnitTypeEnum.Diplomat)
					{
						this.parent.GameData.Players[playerID1].Coins += 100;

						if (playerID1 == this.parent.GameData.HumanPlayerID)
						{
							// Instruction address 0x29f3:0x094b, size: 5
							this.parent.MenuBoxDialog.ShowDialog(this.parent.LanguageTools.F0_2f4d_044f_GetTextFromKingSection("*LEADER"), 80, 80);
						}
					}
				}

				if (attackedCityID == -1)
				{
					if (!this.parent.MapManagement.F0_2aea_1585_GetVisibleTerrainImprovements(
						this.parent.GameData.Players[playerID2].Units[unitID2].Position.X, this.parent.GameData.Players[playerID2].Units[unitID2].Position.Y).HasFlag(TerrainImprovementFlagsEnum.Fortress))
					{
						int stackUnitCount = this.parent.UnitManagement.F0_1866_1251_GetStackUnitValueSum(playerID2, unitID2, UnitStackValueTypeEnum.UnitCount);

						this.parent.GameData.Players[playerID1].UnitsDestroyed[playerID2] += (short)stackUnitCount;

						// Instruction address 0x29f3:0x09ba, size: 5
						this.parent.UnitManagement.F0_1866_0f10_DeleteUnitStack(playerID2, unitID2);

						if (stackUnitCount > 1)
						{
							if (playerID1 == this.parent.GameData.HumanPlayerID || playerID2 == this.parent.GameData.HumanPlayerID)
							{
								// Instruction address 0x29f3:0x0a19, size: 5
								this.parent.MenuBoxDialog.ShowDialog($"{stackUnitCount} units destroyed.\n", 80, 80);
							}
						}
					}
					else
					{
						// Instruction address 0x29f3:0x0ad8, size: 5
						this.parent.UnitManagement.F0_1866_0f10_DeleteUnit(playerID2, unitID2);

						this.parent.GameData.Players[playerID1].UnitsDestroyed[playerID2]++;
					}
				}
				else
				{
					if (this.parent.GameData.Players[playerID2].Units[unitID2].NextUnitID == -1)
					{
						for (int i = 8; i >= 0; i--)
						{
							GPoint direction = this.parent.MoveDirections[i];

							int newX = this.parent.GameData.Players[playerID2].Units[unitID2].Position.X + direction.X;
							int newY = this.parent.GameData.Players[playerID2].Units[unitID2].Position.Y + direction.Y;

							// Instruction address 0x29f3:0x0a7f, size: 5
							int activeUnitID = this.parent.MapManagement.F0_2aea_1458_GetCellActiveUnitID(newX, newY);
							int activeUnitPlayerID = this.parent.MapManagement.F0_2aea_14e0_GetCellActiveUnitPlayerID(newX, newY);

							if (activeUnitID != -1 && activeUnitPlayerID != this.parent.GameData.HumanPlayerID)
							{
								// Instruction address 0x29f3:0x0aa7, size: 5
								this.parent.UnitManagement.F0_1866_1593_UpdateUnitStatusInStack(activeUnitPlayerID, activeUnitID);
							}
						}

						if (this.parent.GameData.Cities[attackedCityID].PlayerID == this.parent.GameData.HumanPlayerID)
						{
							// Instruction address 0x29f3:0x0aca, size: 5
							this.parent.AIEngine.F0_25fb_3e9c_MoveTransportNearEnemyCity(attackedCityID);
						}
					}

					// Instruction address 0x29f3:0x0ad8, size: 5
					this.parent.UnitManagement.F0_1866_0f10_DeleteUnit(playerID2, unitID2);

					this.parent.GameData.Players[playerID1].UnitsDestroyed[playerID2]++;
				}

				if (this.parent.CAPI.RNG.Next(2) == 0)
				{
					return 1;
				}

				this.parent.GameData.Players[playerID1].Units[unitID1].Status |= UnitStatusEnum.Veteran;

				return 1;
			}

			this.parent.GameData.Players[playerID2].UnitsDestroyed[playerID1]++;

			// Instruction address 0x29f3:0x0b31, size: 5
			this.parent.UnitManagement.F0_1866_0f10_DeleteUnit(playerID1, unitID1);

			if (this.parent.CAPI.RNG.Next(2) != 0)
			{
				this.parent.GameData.Players[playerID2].Units[unitID2].Status |= UnitStatusEnum.Veteran;
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
			if (this.parent.GameData.Players[playerID].Units[nextUnitID].NextUnitID == -1)
			{
				this.parent.GameData.Players[playerID].Units[nextUnitID].NextUnitID = (short)unitID;
				this.parent.GameData.Players[playerID].Units[unitID].NextUnitID = (short)nextUnitID;
			}
			else
			{
				this.parent.GameData.Players[playerID].Units[unitID].NextUnitID = this.parent.GameData.Players[playerID].Units[nextUnitID].NextUnitID;
				this.parent.GameData.Players[playerID].Units[nextUnitID].NextUnitID = (short)unitID;
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
			short nextUnitID = this.parent.GameData.Players[playerID].Units[unitID].NextUnitID;

			if (this.parent.GameData.Players[playerID].Units[unitID].NextUnitID != -1)
			{
				if (this.parent.GameData.Players[playerID].Units[nextUnitID].NextUnitID == unitID)
				{
					this.parent.GameData.Players[playerID].Units[nextUnitID].NextUnitID = -1;
				}
				else
				{
					// The original gives up after 32 steps, so removing a unit from a stack of more than 33 units left
					// the previous unit pointing at it. A player has at most 128 units, so search the whole stack.
					const int maximumStackSize = 128;
					int count = 0;

					do
					{
						nextUnitID = this.parent.GameData.Players[playerID].Units[nextUnitID].NextUnitID;
						count++;
					} while (nextUnitID != -1 && count < maximumStackSize && this.parent.GameData.Players[playerID].Units[nextUnitID].NextUnitID != unitID);

					if (count < maximumStackSize && nextUnitID != -1)
					{
						this.parent.GameData.Players[playerID].Units[nextUnitID].NextUnitID = this.parent.GameData.Players[playerID].Units[unitID].NextUnitID;
					}
				}

				this.parent.GameData.Players[playerID].Units[unitID].NextUnitID = -1;
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
			if (this.parent.GameData.Players[this.parent.GameData.HumanPlayerID].Diplomacy[playerID].HasFlag(DiplomacyFlagsEnum.Peace))
			{
				this.parent.Var_2f9e_MessageBoxStyle = MenuBoxReportTypeEnum.ForeignMinisterReport;

				if (this.parent.MenuBoxDialog.ShowDialog(
					$"We have signed a\npeace treaty with\nthe {this.parent.GameData.Players[playerID].Nation}!\n Cancel action.\n Break treaty.\n", 80, 80) != 1)
				{
					return -1;
				}

				if (this.parent.GameData.Players[this.parent.GameData.HumanPlayerID].GovernmentType == GovernmentTypeEnum.Republic ||
					this.parent.GameData.Players[this.parent.GameData.HumanPlayerID].GovernmentType == GovernmentTypeEnum.Democracy)
				{
					this.parent.Var_2f9e_MessageBoxStyle = MenuBoxReportTypeEnum.DomesticAdvisorReport;

					// Instruction address 0x29f3:0x0d39, size: 5
					this.parent.MenuBoxDialog.ShowDialog("Overruled by\nthe Senate.\nAction canceled.\n", 100, 80);

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
			if (playerID != this.parent.GameData.HumanPlayerID)
			{
				this.parent.Var_2f9e_MessageBoxStyle = MenuBoxReportTypeEnum.DefenseMinisterReport;

				// Instruction address 0x29f3:0x0d93, size: 5
				this.parent.MenuBoxDialog.ShowDialog($"{this.parent.GameData.Players[playerID].Nation} use\nnuclear weapons!\n", 100, 80);
			}

			this.parent.Animations.F22_0000_0967_NukeAnimation(x, y);

			for (int i = 8; i >= 0; i--)
			{
				GPoint direction = this.parent.MoveDirections[i];

				int newX = this.parent.MapManagement.AdjustXPosition(x + direction.X);
				int newY = y + direction.Y;

				if (this.parent.MapManagement.ValidateMapCoordinates(newX, newY))
				{
					// Instruction address 0x29f3:0x0dcb, size: 5
					int activeUnitID = this.parent.MapManagement.F0_2aea_1458_GetCellActiveUnitID(newX, newY);
					int activeUnitPlayerID = this.parent.MapManagement.F0_2aea_14e0_GetCellActiveUnitPlayerID(newX, newY);

					if (this.parent.MapManagement.F0_2aea_1585_GetVisibleTerrainImprovements(newX, newY).HasFlag(TerrainImprovementFlagsEnum.City))
					{
						// Instruction address 0x29f3:0x0dee, size: 5
						int cityID = this.parent.MapManagement.GetCityByLocation(newX, newY);

						if (this.parent.GameData.Cities[cityID].HasImprovement(ImprovementEnum.SDIDefense))
						{
							// Instruction address 0x29f3:0x0e40, size: 5
							this.parent.MenuBoxDialog.ShowDialog($"SDI protects {this.parent.Actions.F0_2459_08c6_GetCityName(cityID)}.\n", 80, 100);

							activeUnitID = -1;
						}
					}

					if (activeUnitID != -1)
					{
						// Instruction address 0x29f3:0x0e5e, size: 5
						int unitCount = this.parent.UnitManagement.F0_1866_1251_GetStackUnitValueSum(activeUnitPlayerID, activeUnitID, UnitStackValueTypeEnum.UnitCount);

						this.parent.GameData.Players[playerID].UnitsDestroyed[activeUnitPlayerID] += (short)unitCount;

						// Instruction address 0x29f3:0x0e89, size: 5
						this.parent.UnitManagement.F0_1866_0f10_DeleteUnitStack(activeUnitPlayerID, activeUnitID);

						if (playerID != activeUnitPlayerID)
						{
							this.parent.GameData.Players[activeUnitPlayerID].Diplomacy[playerID] |= DiplomacyFlagsEnum.Vendetta | DiplomacyFlagsEnum.TributeDemanded;
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
				GPoint direction = this.parent.MoveDirections[i];

				// Instruction address 0x29f3:0x0f26, size: 5
				int newX = this.parent.MapManagement.AdjustXPosition(x + direction.X);
				int newY = y + direction.Y;

				if (this.parent.MapManagement.ValidateMapCoordinates(newX, newY))
				{
					if (this.parent.MapManagement.F0_2aea_1585_GetVisibleTerrainImprovements(newX, newY).HasFlag(TerrainImprovementFlagsEnum.City))
					{
						// Instruction address 0x29f3:0x0f51, size: 5
						int cityID = this.parent.MapManagement.GetCityByLocation(newX, newY);

						this.parent.GameData.Cities[cityID].ActualSize -= (sbyte)(this.parent.GameData.Cities[cityID].ActualSize / 2);
					}
					else
					{
						if (this.parent.MapManagement.GetTerrainType(newX, newY) != TerrainTypeEnum.Water && this.parent.CAPI.RNG.Next(3) != 0)
						{
							// Instruction address 0x29f3:0x0efa, size: 5
							this.parent.CityWorker.F0_1d12_6d33_AddPollutionToCell(newX, newY);
						}
					}

					// Instruction address 0x29f3:0x0f08, size: 5
					this.parent.MapManagement.F0_2aea_11d4_DrawCellWithUnit(newX, newY);
				}
			}
		}
	}
}
