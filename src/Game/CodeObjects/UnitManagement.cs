using IRB.VirtualCPU;
using OpenCivOne.Graphics;

namespace OpenCivOne
{
	public class UnitManagement
	{
		private OpenCivOneGame parent;

		private bool Var_20f4 = false;
		private int Var_6530 = 0;
		private int Var_6534 = 0;

		public UnitManagement(OpenCivOneGame parent)
		{
			this.parent = parent;
		}

		/// <summary>
		/// Restores fortified city units that are resource drain free
		/// </summary>
		/// <param name="cityID"></param>
		public void F0_1866_0006_RestoreFortifiedCityUnits(int cityID)
		{
			//this.oCPU.Log.EnterBlock($"F0_1866_0006({cityID})");

			// function body
			if (this.parent.GameData.Cities[cityID].PlayerID == this.parent.GameData.HumanPlayerID)
			{
				this.Var_20f4 = true;

				for (int i = 0; i < 2; i++)
				{
					if (this.parent.GameData.Cities[cityID].FirstFortifiedCityUnits[i] != -1)
					{
						// Instruction address 0x1866:0x005e, size: 3
						int unitID = F0_1866_0cf5_CreateUnit(this.parent.GameData.Cities[cityID].PlayerID,
							(UnitTypeEnum)(this.parent.GameData.Cities[cityID].FirstFortifiedCityUnits[i] & 0x3f),
							this.parent.GameData.Cities[cityID].Position.X, this.parent.GameData.Cities[cityID].Position.Y);

						if (unitID != -1)
						{
							this.parent.GameData.Players[this.parent.GameData.Cities[cityID].PlayerID].Units[unitID].Status |= UnitStatusEnum.Fortified;

							if ((this.parent.GameData.Cities[cityID].FirstFortifiedCityUnits[i] & 0x40) != 0)
							{
								this.parent.GameData.Players[this.parent.GameData.Cities[cityID].PlayerID].Units[unitID].Status |= UnitStatusEnum.Veteran;
							}

							this.parent.GameData.Cities[cityID].FirstFortifiedCityUnits[i] = -1;
						}
					}
				}

				this.Var_20f4 = false;

				this.parent.GameData.Cities[cityID].Status |= CityStatusEnum.FortifiedUnitsRestored;
			}
		}

		/// <summary>
		/// Stores first fortified city units that will be resource free
		/// </summary>
		/// <param name="cityID"></param>
		public void F0_1866_00c6_StoreFortifiedCityUnits(int cityID)
		{
			//this.oCPU.Log.EnterBlock($"F0_1866_00c6({cityID})");

			// function body
			int cityPlayerID = this.parent.GameData.Cities[cityID].PlayerID;
			
			if (cityPlayerID == this.parent.GameData.HumanPlayerID)
			{
				bool oldValue = this.parent.Var_70d8;
				bool skipFirstUnit = true;
				this.parent.Var_70d8 = false;
				this.Var_20f4 = true;

				for (int i = 0; i < 128; i++)
				{
					if (this.parent.GameData.Players[cityPlayerID].Units[i].UnitType != UnitTypeEnum.None &&
						(this.parent.GameData.Players[cityPlayerID].Units[i].Position.X == this.parent.GameData.Cities[cityID].Position.X) &&
						(this.parent.GameData.Players[cityPlayerID].Units[i].Position.Y == this.parent.GameData.Cities[cityID].Position.Y) &&
						this.parent.GameData.Players[cityPlayerID].Units[i].HomeCityID == cityID &&
						(this.parent.GameData.Players[cityPlayerID].Units[i].Status & UnitStatusEnum.Fortified) != UnitStatusEnum.None)
					{
						if (!skipFirstUnit)
						{
							if (this.parent.GameData.Cities[cityID].FirstFortifiedCityUnits[0] == -1)
							{
								this.parent.GameData.Cities[cityID].FirstFortifiedCityUnits[0] = (sbyte)this.parent.GameData.Players[cityPlayerID].Units[i].UnitType;

								if ((this.parent.GameData.Players[cityPlayerID].Units[i].Status & UnitStatusEnum.Veteran) != UnitStatusEnum.None)
								{
									this.parent.GameData.Cities[cityID].FirstFortifiedCityUnits[0] |= 0x40;
								}
							}
							else
							{
								if (this.parent.GameData.Cities[cityID].FirstFortifiedCityUnits[1] != -1)
									break;

								this.parent.GameData.Cities[cityID].FirstFortifiedCityUnits[1] = (sbyte)this.parent.GameData.Players[cityPlayerID].Units[i].UnitType;

								if ((this.parent.GameData.Players[cityPlayerID].Units[i].Status & UnitStatusEnum.Veteran) != UnitStatusEnum.None)
								{
									this.parent.GameData.Cities[cityID].FirstFortifiedCityUnits[1] |= 0x40;
								}
							}

							// Instruction address 0x1866:0x013d, size: 3
							F0_1866_0f10_DeleteUnit(cityPlayerID, i);
						}
						skipFirstUnit = false;
					}
				}

				this.Var_20f4 = false;
				this.parent.Var_70d8 = oldValue;
				this.parent.GameData.Cities[cityID].ClearStatusFlags(CityStatusEnum.FortifiedUnitsRestored);
			}		
		}

		/// <summary>
		/// Updates visibility of the terrain in unit proximity
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="playerID"></param>
		/// <param name="unitID"></param>
		/// <param name="flag"></param>
		public void F0_1866_01dc_UpdateUnitVisibility(int x, int y, int playerID, int unitID, bool flag)
		{
			//this.oCPU.Log.EnterBlock($"F0_1866_01dc({x}, {y}, {playerID}, {unitID}, {flag})");

			// function body
			TerrainTypeEnum terrainType = this.parent.MapManagement.GetTerrainType(x, y);
			// Instruction address 0x1866:0x02b5, size: 5
			TerrainImprovementFlagsEnum visibleTerrainImprovements = this.parent.MapManagement.F0_2aea_1585_GetVisibleTerrainImprovements(x, y);

			if (playerID == this.parent.GameData.HumanPlayerID)
			{
				for (int i = 0; i < 25; i++)
				{
					GPoint direction = this.parent.MoveDirections[i];

					// Instruction address 0x1866:0x0212, size: 5
					int newX = this.parent.MapManagement.AdjustXPosition(x + direction.X);
					int newY = y + direction.Y;

					if (!this.parent.MapManagement.ValidateMapCoordinates(newX, newY))
						continue;

					if(i < 9 || (this.parent.GameData.Units[(int)this.parent.GameData.Players[playerID].Units[unitID].UnitType].SightRange & 0x2) != 0)
					{
						// Instruction address 0x1866:0x026c, size: 5
						this.parent.GameData.MapVisibility[newX, newY] |= (ushort)(1 << playerID);

						// Instruction address 0x1866:0x029b, size: 5
						this.parent.MapManagement.F0_2aea_1601_UpdateVisibleCellStatus(newX, newY);
					}
				}
			}

			for (int i = 1; i < 9; i++)
			{
				GPoint direction = this.parent.MoveDirections[i];

				// Instruction address 0x1866:0x0397, size: 5
				int newX = this.parent.MapManagement.AdjustXPosition(x + direction.X);
				int newY = y + direction.Y;

				// Instruction address 0x1866:0x03b0, size: 5
				if (!this.parent.MapManagement.ValidateMapCoordinates(newX, newY))
					continue;

				if (playerID != 0)
				{
					this.parent.GameData.MapVisibility[newX, newY] |= (ushort)(1 << playerID);
				}

				// Instruction address 0x1866:0x03e5, size: 5
				TerrainImprovementFlagsEnum newVisibleTerrainImprovements = this.parent.MapManagement.F0_2aea_1585_GetVisibleTerrainImprovements(newX, newY);

				if (newVisibleTerrainImprovements.HasFlag(TerrainImprovementFlagsEnum.City))
				{
					// Instruction address 0x1866:0x03ff, size: 5
					if (this.parent.MapManagement.F0_2aea_1369_GetCityOwner(newX, newY) != playerID)
					{
						// Instruction address 0x1866:0x0415, size: 5
						int cityOwnerPlayerID = this.parent.MapManagement.F0_2aea_1369_GetCityOwner(newX, newY);

						// Instruction address 0x1866:0x0426, size: 5
						int cityID = this.parent.Tools.GetCityByLocation(newX, newY);

						// Instruction address 0x1866:0x0433, size: 3
						F0_1866_0006_RestoreFortifiedCityUnits(cityID);

						if (playerID == this.parent.GameData.HumanPlayerID)
						{
							this.parent.GameData.Cities[cityID].VisibleSize = this.parent.GameData.Cities[cityID].ActualSize;
						}

						if (this.parent.GameData.Units[(int)this.parent.GameData.Players[playerID].Units[unitID].UnitType].MovementType == UnitMovementTypeEnum.Land)
						{
							this.parent.GameData.Players[playerID].Units[unitID].VisibleByPlayer |= (ushort)(1 << cityOwnerPlayerID);

							if (!this.parent.GameData.Players[playerID].Diplomacy[cityOwnerPlayerID].HasFlag(DiplomacyFlagsEnum.Peace))
							{
								// Instruction address 0x1866:0x049f, size: 5
								this.parent.AIEngine.PlayerAddUnitPolicy(cityOwnerPlayerID, newX, newY, UnitRoleTypeEnum.LandAttack, 4);

								// Instruction address 0x1866:0x04b5, size: 5
								this.parent.AIEngine.PlayerAddUnitPolicy(cityOwnerPlayerID, newX, newY, UnitRoleTypeEnum.Defense, 2);
							}
						}
					}
				}

				// Instruction address 0x1866:0x04c3, size: 5
				int activeUnitID = this.parent.MapManagement.F0_2aea_1458_GetCellActiveUnitID(newX, newY);
				int activeUnitPlayerID = this.parent.MapManagement.F0_2aea_14e0_GetCellActiveUnitPlayerID(newX, newY);

				if (activeUnitID != -1 && playerID != activeUnitPlayerID)
				{
					if (!visibleTerrainImprovements.HasFlag(TerrainImprovementFlagsEnum.City))
					{
						this.parent.GameData.Players[playerID].Units[unitID].VisibleByPlayer |= (ushort)(1 << activeUnitPlayerID);
					}

					// Instruction address 0x1866:0x0513, size: 3
					F0_1866_14a2_UnitStackUpdateSentryFlag(activeUnitPlayerID, activeUnitID);

					// Instruction address 0x1866:0x051f, size: 5
					if (this.parent.MapManagement.GetTerrainType(newX, newY) == TerrainTypeEnum.Water || terrainType != TerrainTypeEnum.Water)
					{
						this.parent.GameData.Players[activeUnitPlayerID].Units[activeUnitID].GoToDestination = OpenCivOneGame.InvalidPosition;
					}

					// Instruction address 0x1866:0x054e, size: 5
					if (this.parent.MapManagement.GetTerrainType(newX, newY) != TerrainTypeEnum.Water ||
						terrainType == TerrainTypeEnum.Water ||
						this.parent.GameData.Units[(int)this.parent.GameData.Players[playerID].Units[unitID].UnitType].MovementType == UnitMovementTypeEnum.Air)
					{
						this.parent.GameData.Players[playerID].Units[unitID].GoToDestination = OpenCivOneGame.InvalidPosition;
					}

					if (!newVisibleTerrainImprovements.HasFlag(TerrainImprovementFlagsEnum.City))
					{
						this.parent.GameData.Players[activeUnitPlayerID].Units[activeUnitID].VisibleByPlayer |= (ushort)(1 << playerID);
					}

					if (this.parent.GameData.Units[(int)this.parent.GameData.Players[playerID].Units[unitID].UnitType].MovementType == UnitMovementTypeEnum.Land &&
						this.parent.MapManagement.GetTerrainType(x, y) != TerrainTypeEnum.Water &&
						this.parent.MapManagement.GetTerrainType(newX, newY) != TerrainTypeEnum.Water &&
						this.parent.GameData.Units[(int)this.parent.GameData.Players[activeUnitPlayerID].Units[activeUnitID].UnitType].MovementType != UnitMovementTypeEnum.Air)
					{
						// Instruction address 0x1866:0x0635, size: 5
						this.parent.Diplomacy.F0_2517_0737_UpdateDiplomacyStatus(playerID, activeUnitPlayerID, x, y);

						if (!this.parent.GameData.Players[playerID].Diplomacy[activeUnitPlayerID].HasFlag(DiplomacyFlagsEnum.Peace))
						{
							int local_6 = 1;

							if (activeUnitPlayerID != 0 &&
								this.parent.GameData.Players[activeUnitPlayerID].Units[activeUnitID].UnitType != UnitTypeEnum.Diplomat &&
								((this.parent.GameData.Players[activeUnitPlayerID].Units[activeUnitID].Status & UnitStatusEnum.Fortified) == UnitStatusEnum.None ||
									this.parent.GameData.Units[(int)this.parent.GameData.Players[activeUnitPlayerID].Units[activeUnitID].UnitType].UnitRoleType != UnitRoleTypeEnum.Defense))
							{
								local_6 = 3;
							}

							// Instruction address 0x1866:0x069b, size: 5
							this.parent.AIEngine.PlayerAddUnitPolicy(playerID, newX, newY, UnitRoleTypeEnum.LandAttack, (short)local_6);
						}

						if (this.parent.GameData.Players[playerID].Units[unitID].UnitType != UnitTypeEnum.Diplomat)
						{
							if (playerID == 0)
							{
								// Instruction address 0x1866:0x02da, size: 5
								this.parent.AIEngine.PlayerAddUnitPolicy(activeUnitPlayerID, x, y, UnitRoleTypeEnum.Defense, 1);
							}
							else
							{
								// Instruction address 0x1866:0x02da, size: 5
								this.parent.AIEngine.PlayerAddUnitPolicy(activeUnitPlayerID, x, y, UnitRoleTypeEnum.Defense, 2);
							}
						}
					}

					// Instruction address 0x1866:0x02e8, size: 5
					if (this.parent.MapManagement.GetTerrainType(newX, newY) == TerrainTypeEnum.Water &&
						activeUnitPlayerID != 0 &&
						!this.parent.GameData.Players[playerID].Diplomacy[activeUnitPlayerID].HasFlag(DiplomacyFlagsEnum.Peace))
					{
						// Instruction address 0x1866:0x0321, size: 5
						this.parent.AIEngine.PlayerAddUnitPolicy(playerID, newX, newY, UnitRoleTypeEnum.SeaAttack, 2);
					}
				}

				if (flag && (playerID == this.parent.GameData.HumanPlayerID || this.parent.Var_d806_DebugFlag))
				{
					// Instruction address 0x1866:0x0344, size: 5
					this.parent.MapManagement.F0_2aea_11d4_DrawCellWithUnit(newX, newY);

					// Instruction address 0x1866:0x0358, size: 3
					if (this.parent.MapManagement.GetTerrainType(newX, newY) == TerrainTypeEnum.Water)
					{
						// Instruction address 0x1866:0x0376, size: 5
						this.parent.MapManagement.SetMiniMapCell(newX, newY, 1);
					}
					else
					{
						// Instruction address 0x1866:0x0376, size: 5
						this.parent.MapManagement.SetMiniMapCell(newX, newY, 2);
					}
				}
			}

			for (int i = 9; i < 25; i++)
			{
				GPoint direction = this.parent.MoveDirections[i];

				// Instruction address 0x1866:0x08d2, size: 5
				int newX = this.parent.MapManagement.AdjustXPosition(x + direction.X);
				int newY = y + direction.Y;

				// Instruction address 0x1866:0x08eb, size: 5
				if (this.parent.MapManagement.ValidateMapCoordinates(newX, newY))
				{
					// Instruction address 0x1866:0x08fd, size: 5
					int activeUnitID = this.parent.MapManagement.F0_2aea_1458_GetCellActiveUnitID(newX, newY);
					int activeUnitPlayerID = this.parent.MapManagement.F0_2aea_14e0_GetCellActiveUnitPlayerID(newX, newY);

					// Instruction address 0x1866:0x090e, size: 5
					TerrainImprovementFlagsEnum newVisibleTerrainImprovements = this.parent.MapManagement.F0_2aea_1585_GetVisibleTerrainImprovements(newX, newY);

					if ((this.parent.GameData.Units[(int)this.parent.GameData.Players[playerID].Units[unitID].UnitType].SightRange & 0x2) != 0 &&
						(this.parent.GameData.Units[(int)this.parent.GameData.Players[playerID].Units[unitID].UnitType].MovementType != UnitMovementTypeEnum.Water ||
						this.parent.MapManagement.GetTerrainType(newX, newY) == TerrainTypeEnum.Water))
					{
						if (playerID != 0)
						{
							this.parent.GameData.MapVisibility[newX, newY] |= (ushort)(1 << playerID);
						}

						if (activeUnitID != -1 && playerID != activeUnitPlayerID &&
							this.parent.GameData.Players[activeUnitPlayerID].Units[activeUnitID].UnitType != UnitTypeEnum.Submarine)
						{
							if (!newVisibleTerrainImprovements.HasFlag(TerrainImprovementFlagsEnum.City))
							{
								this.parent.GameData.Players[activeUnitPlayerID].Units[activeUnitID].VisibleByPlayer |= (ushort)(1 << playerID);
							}

							// Instruction address 0x1866:0x09b8, size: 5
							if (this.parent.MapManagement.GetTerrainType(newX, newY) == TerrainTypeEnum.Water && activeUnitPlayerID != 0 &&
								!this.parent.GameData.Players[playerID].Diplomacy[activeUnitPlayerID].HasFlag(DiplomacyFlagsEnum.Peace))
							{
								// Instruction address 0x1866:0x09f1, size: 5
								this.parent.AIEngine.PlayerAddUnitPolicy(playerID, newX, newY, UnitRoleTypeEnum.SeaAttack, 2);
							}
						}

						if (flag && (playerID == this.parent.GameData.HumanPlayerID || this.parent.Var_d806_DebugFlag))
						{
							// Instruction address 0x1866:0x0a1a, size: 5
							this.parent.MapManagement.F0_2aea_11d4_DrawCellWithUnit(newX, newY);

							// Instruction address 0x1866:0x0a2e, size: 3
							if (this.parent.MapManagement.GetTerrainType(newX, newY) == TerrainTypeEnum.Water)
							{
								// Instruction address 0x1866:0x06e2, size: 5
								this.parent.MapManagement.SetMiniMapCell(newX, newY, 1);
							}
							else
							{
								// Instruction address 0x1866:0x06e2, size: 5
								this.parent.MapManagement.SetMiniMapCell(newX, newY, 2);
							}
						}
					}

					if (activeUnitID != -1)
					{
						if (playerID != activeUnitPlayerID &&
							this.parent.GameData.Players[playerID].Units[unitID].UnitType != UnitTypeEnum.Submarine)
						{
							if ((this.parent.GameData.Units[(int)this.parent.GameData.Players[activeUnitPlayerID].Units[activeUnitID].UnitType].SightRange & 0x2) != 0 &&
								(this.parent.GameData.Units[(int)this.parent.GameData.Players[activeUnitPlayerID].Units[activeUnitID].UnitType].MovementType != UnitMovementTypeEnum.Water ||
									this.parent.MapManagement.GetTerrainType(x, y) == TerrainTypeEnum.Water))
							{
								this.parent.GameData.Players[activeUnitPlayerID].Units[activeUnitID].ClearStatusFlags(UnitStatusEnum.Sentry);
								this.parent.GameData.Players[activeUnitPlayerID].Units[activeUnitID].GoToDestination = OpenCivOneGame.InvalidPosition;

								if (!visibleTerrainImprovements.HasFlag(TerrainImprovementFlagsEnum.City))
								{
									this.parent.GameData.Players[playerID].Units[unitID].VisibleByPlayer |= (ushort)(1 << activeUnitPlayerID);
								}

								// Instruction address 0x1866:0x07a3, size: 5
								if (this.parent.MapManagement.GetTerrainType(newX, newY) == TerrainTypeEnum.Water && activeUnitPlayerID != 0 &&
									!this.parent.GameData.Players[playerID].Diplomacy[activeUnitPlayerID].HasFlag(DiplomacyFlagsEnum.Peace))
								{
									// Instruction address 0x1866:0x07e6, size: 5
									this.parent.AIEngine.PlayerAddUnitPolicy(activeUnitPlayerID, x, y, UnitRoleTypeEnum.SeaAttack, 2);
								}
							}
							else if (activeUnitPlayerID == this.parent.GameData.HumanPlayerID &&
								this.parent.MapManagement.F0_2aea_1585_GetVisibleTerrainImprovements(newX, newY).HasFlag(TerrainImprovementFlagsEnum.City) &&
								(this.parent.GameData.MapVisibility[x, y] & (0x1 << this.parent.GameData.HumanPlayerID)) != 0)
							{
								this.parent.GameData.Players[playerID].Units[unitID].VisibleByPlayer |= (ushort)(1 << activeUnitPlayerID);

								// Instruction address 0x1866:0x084f, size: 5
								this.parent.MapManagement.F0_2aea_11d4_DrawCellWithUnit(x, y);
							}
						}
					}

					if (newVisibleTerrainImprovements.HasFlag(TerrainImprovementFlagsEnum.City) &&
						this.parent.MapManagement.F0_2aea_1369_GetCityOwner(newX, newY) == this.parent.GameData.HumanPlayerID &&
						(this.parent.GameData.MapVisibility[x, y] & (0x1 << this.parent.GameData.HumanPlayerID)) != 0)
					{
						this.parent.GameData.Players[playerID].Units[unitID].VisibleByPlayer |= (ushort)(1 << this.parent.GameData.HumanPlayerID);

						// Instruction address 0x1866:0x08b1, size: 5
						this.parent.MapManagement.F0_2aea_11d4_DrawCellWithUnit(x, y);
					}
				}
			}

			if (flag && playerID == this.parent.GameData.HumanPlayerID)
			{
				// Instruction address 0x1866:0x0a59, size: 5
				this.parent.MapManagement.F0_2aea_11d4_DrawCellWithUnit(x, y);

				for (int i = 9; i < 49; i++)
				{
					GPoint direction = this.parent.MoveDirections[i];

					// Instruction address 0x1866:0x0a73, size: 5
					int newX = this.parent.MapManagement.AdjustXPosition(x + direction.X);
					int newY = y + direction.Y;

					// Instruction address 0x1866:0x0a8c, size: 5
					if (this.parent.MapManagement.ValidateMapCoordinates(newX, newY) &&
						(this.parent.GameData.MapVisibility[newX, newY] & (0x1 << playerID)) != 0)
					{
						// Instruction address 0x1866:0x0abf, size: 5
						this.parent.MapManagement.F0_2aea_11d4_DrawCellWithUnit(newX, newY);
					}
				}
			}
		}

		/// <summary>
		/// This function blinks the active unit or 'End Of Turn', it also allows cities to be clicked and managed
		/// The mouse state that is captured by this function needs to be preserved for the calling function to process
		/// </summary>
		/// <param name="playerID"></param>
		/// <param name="unitID"></param>
		/// <param name="xOffset"></param>
		/// <param name="yOffset"></param>
		public void F0_1866_0ad6_ShowActiveUnitOrEndOfTurn(int playerID, int unitID, int xOffset, int yOffset)
		{
			//this.oCPU.Log.EnterBlock($"F0_1866_0ad6({playerID}, {unitID}, {xOffset}, {yOffset})");

			// function body
			int xScreen = -1;
			int yScreen = 0;

			if (xOffset != -1)
			{
				// Instruction address 0x1866:0x0af0, size: 5
				xScreen = this.parent.MapManagement.AdjustXPosition(xOffset - this.parent.Var_d4cc_MapViewX) * 16 + 80;
				yScreen = (yOffset - this.parent.Var_d75e_MapViewY) * 16 + 8;

				if (xScreen < 80 || xScreen >= 320 || yScreen < 8 || yScreen > 192)
				{
					xScreen = -1;
				}				
			}

			int local_e = (this.parent.Var_70ea_MiniMapY < 0) ? 0 : this.parent.Var_70ea_MiniMapY;
			int local_a = local_e - this.parent.Var_70ea_MiniMapY;
			
			// Instruction address 0x1866:0x0b5b, size: 5
			int newX = this.parent.MapManagement.AdjustXPosition(this.parent.GameData.Players[playerID].Units[unitID].Position.X - this.parent.Var_6ed6_MiniMapX);
			int newY = (unitID < 127) ? (this.parent.GameData.Players[playerID].Units[unitID].Position.Y - local_e + local_a) : (-1 - local_e + local_a);

			//MouseEvent mouseState;

			do
			{
				if (unitID >= 128)
				{
					// Instruction address 0x1866:0x0bd9, size: 5
					this.parent.DrawTools.DrawStringToScreen0("End of Turn", 4, 124, 7);
				}
				else
				{
					// Instruction address 0x1866:0x0b9d, size: 5
					this.parent.MapManagement.F0_2aea_03ba_DrawCell(this.parent.GameData.Players[playerID].Units[unitID].Position.X,
						this.parent.GameData.Players[playerID].Units[unitID].Position.Y);

					if (newY >= 0 && newY < 50)
					{
						// Instruction address 0x1866:0x0bbf, size: 5
						this.parent.Graphics.F0_VGA_0550_SetPixel(this.parent.Var_aa_Screen0_Rectangle.ScreenID, newX, newY + 8, 15);
					}
				}

				if (xScreen != -1)
				{
					// Instruction address 0x1866:0x0bf3, size: 5
					this.parent.DrawTools.DrawRectangle(xScreen, yScreen, 15, 15, 15);
				}

				// Instruction address 0x1866:0x0c00, size: 5
				if (this.parent.CAPI.kbhit() != 0 || (this.parent.HasNewMouseEvent() && this.parent.PeekMouseEvent().Buttons != MouseButtonsEnum.None))
				{
					// Instruction address 0x1866:0x0c12, size: 5
					//this.parent.CommonTools.WaitTimer(1);
				}
				else
				{
					// Instruction address 0x1866:0x0c12, size: 5
					this.parent.CommonTools.WaitTimer(10);
				}

				if (unitID >= 128)
				{
					// Instruction address 0x1866:0x0c95, size: 5
					this.parent.DrawTools.DrawStringToScreen0("End of Turn", 4, 124, 0);
				}
				else
				{
					// Instruction address 0x1866:0x0c2c, size: 5
					this.parent.MapManagement.F0_2aea_0e29_DrawUnit(playerID, unitID);

					if (newY >= 0 && newY < 50)
					{
						// Instruction address 0x1866:0x0c5c, size: 5
						if (this.parent.MapManagement.GetTerrainType(this.parent.GameData.Players[playerID].Units[unitID].Position.X,
							this.parent.GameData.Players[playerID].Units[unitID].Position.Y) == TerrainTypeEnum.Water)
						{
							// Instruction address 0x1866:0x0c7c, size: 5
							this.parent.Graphics.F0_VGA_0550_SetPixel(this.parent.Var_aa_Screen0_Rectangle.ScreenID, newX, newY + 8, 1);
						}
						else
						{
							// Instruction address 0x1866:0x0c7c, size: 5
							this.parent.Graphics.F0_VGA_0550_SetPixel(this.parent.Var_aa_Screen0_Rectangle.ScreenID, newX, newY + 8, 2);
						}
					}
				}

				if (xScreen != -1)
				{
					// Instruction address 0x1866:0x0cb1, size: 5
					this.parent.DrawTools.DrawRectangle(xScreen, yScreen, 15, 15, 0);
				}

				// Instruction address 0x1866:0x0cbe, size: 5
				if (this.parent.CAPI.kbhit() != 0 || (this.parent.HasNewMouseEvent() && this.parent.PeekMouseEvent().Buttons != MouseButtonsEnum.None))
				{
					// Instruction address 0x1866:0x0cd0, size: 5
					//this.parent.CommonTools.WaitTimer(1);
				}
				else
				{
					// Instruction address 0x1866:0x0cd0, size: 5
					this.parent.CommonTools.WaitTimer(10);
				}

				/*mouseState = this.parent.PeekMouseEvent();

				if (mouseState.Buttons == MouseButtonsEnum.None)
				{
					// We can safely consume this mouse event
					mouseState = this.parent.GetMouseEvent();
				}*/
			}
			while (this.parent.CAPI.kbhit() == 0 && !this.parent.HasNewMouseEvent());
		}

		/// <summary>
		/// Creates unit of specified type.
		/// </summary>
		/// <param name="playerID"></param>
		/// <param name="unitType"></param>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns>ID of newly created unit or -1 if unit can not be created</returns>
		public int F0_1866_0cf5_CreateUnit(int playerID, UnitTypeEnum unitType, int x, int y)
		{
			//this.oCPU.Log.EnterBlock($"F0_1866_0cf5_CreateUnit({playerID}, {unitTypeID}, {xPos}, {yPos})");

			// function body
			Player player = this.parent.GameData.Players[playerID];
			int unitID;

			// Find free unit ID
			for (unitID = 0; unitID < 127 && player.Units[unitID].UnitType != UnitTypeEnum.None; unitID++)
			{ }

			if (unitID >= 127)
			{
				if (playerID == this.parent.GameData.HumanPlayerID && !this.parent.Var_d760_HumanPlayerMessageFlag)
				{
					this.parent.Var_2f9e_MessageBoxStyle = MenuBoxReportTypeEnum.DefenseMinisterReport;

					// Instruction address 0x1866:0x0efa, size: 5
					this.parent.MenuBoxDialog.ShowDialog(this.parent.LanguageTools.F0_2f4d_044f_GetTextFromKingSection("*UMAX"), 80, 64);

					// Show message to the human player only once per turn
					this.parent.Var_d760_HumanPlayerMessageFlag = true;
				}
			}
			else
			{
				Unit unit = player.Units[unitID];
				unit.Position = OpenCivOneGame.InvalidPosition;
				unit.NextUnitID = -1;

				// Instruction address 0x1866:0x0d39, size: 5
				this.parent.MapManagement.F0_2aea_138c_SetCityOwner(x, y, playerID);

				// Instruction address 0x1866:0x0d4d, size: 5
				this.parent.MapManagement.F0_2aea_13cb_SetCellPlayerID(x, y, playerID, unitID);

				this.parent.GameData.MapVisibility[x, y] |= (ushort)(1 << playerID);

				unit.Status = 0;
				unit.Position = new GPoint(x, y);
				unit.UnitType = unitType;
				unit.VisibleByPlayer = (ushort)(1 << playerID);
				unit.GoToDestination = OpenCivOneGame.InvalidPosition;
				unit.GoToNextDirection = -1;
				unit.SpecialMoves = this.parent.GameData.Units[(int)unit.UnitType].TurnsOutside;

				// Instruction address 0x1866:0x0db6, size: 5
				unit.HomeCityID = (short)this.parent.Tools.FindNearestCity(x, y);

				short cityOwnerID = (short)((unit.HomeCityID < 128 && unit.HomeCityID != -1) ? this.parent.GameData.Cities[unit.HomeCityID].PlayerID : -1);

				if (cityOwnerID != playerID)
				{
					unit.HomeCityID = -1;
				}

				if (unit.SpecialMoves != 0)
				{
					unit.SpecialMoves--;
				}

				player.ActiveUnits[(int)unitType]++;

				if (!this.Var_20f4)
				{
					if (this.parent.Var_d806_DebugFlag || playerID == this.parent.GameData.HumanPlayerID ||
						(unit.VisibleByPlayer & (1 << this.parent.GameData.HumanPlayerID)) != 0)
					{
						// Instruction address 0x1866:0x0e5c, size: 5
						this.parent.MapManagement.F0_2aea_11d4_DrawCellWithUnit(unit.Position.X, unit.Position.Y);
					}

					// Instruction address 0x1866:0x0e77, size: 3
					F0_1866_01dc_UpdateUnitVisibility(x, y, playerID, unitID, true);
				}

				if (playerID == this.parent.GameData.HumanPlayerID && 
					this.parent.GameData.TurnCount != 0 && !this.Var_20f4)
				{
					if (player.UnitCount == 0)
					{
						this.parent.Help.F4_0000_02d3_ShowInstantAdvicePopup("*FIRSTUNIT1");
					}

					if (player.UnitCount == 1)
					{
						this.parent.Help.F4_0000_02d3_ShowInstantAdvicePopup("*FIRSTUNIT2");
					}
				}

				return unitID;
			}

			return -1;
		}

		/// <summary>
		/// Deletes stack of units counting it as destroyed
		/// </summary>
		/// <param name="playerID"></param>
		/// <param name="unitID"></param>
		public void F0_1866_0f10_DeleteUnitStack(int playerID, int unitID)
		{
			int oldUnitID = unitID;

			for (int i = 0; i < 10 && oldUnitID != -1; i++)
			{
				int newUnitID = this.parent.GameData.Players[playerID].Units[oldUnitID].NextUnitID;

				F0_1866_0f10_DeleteUnit(playerID, oldUnitID);

				oldUnitID = newUnitID;

				if (oldUnitID == unitID)
					break;
			}
		}

		/// <summary>
		/// Deletes unit counting it as destroyed
		/// </summary>
		/// <param name="playerID"></param>
		/// <param name="unitID"></param>
		public void F0_1866_0f10_DeleteUnit(int playerID, int unitID)
		{
			//this.oCPU.Log.EnterBlock($"F0_1866_0f10_DeleteUnit({playerID}, {unitID})");

			// function body
			Player player = this.parent.GameData.Players[playerID];
			Unit unit = player.Units[unitID];

			if (unit.UnitType != UnitTypeEnum.None)
			{
				if (this.parent.Var_70d8)
				{
					player.LostUnits[(int)unit.UnitType]++;
				}
	
				if (this.parent.GameData.Units[(int)unit.UnitType].TransportCapacity != 0 && unit.NextUnitID != -1 && 
					this.parent.MapManagement.GetTerrainType(unit.Position.X, unit.Position.Y) == TerrainTypeEnum.Water)
				{
					// Delete land units aboard the ship

					// Instruction address 0x1866:0x0f94, size: 3
					F0_1866_1610_DeleteLandUnitsInStack(playerID, unitID);
				}
	
				if (unit.UnitType == UnitTypeEnum.Carrier && unit.NextUnitID != -1 && 
					this.parent.MapManagement.GetTerrainType(unit.Position.X, unit.Position.Y) == TerrainTypeEnum.Water)
				{
					// Delete air unit(s) along with aircraft carrier

					// Instruction address 0x1866:0x0fe0, size: 3
					F0_1866_1676_DeleteAirUnitInStack(playerID, unitID);
				}
	
				if (unit.UnitType != UnitTypeEnum.None)
				{
					player.ActiveUnits[(int)unit.UnitType]--;
				}
	
				unit.UnitType = UnitTypeEnum.None;
				unit.RemainingMoves = 0;
	
				// Instruction address 0x1866:0x1027, size: 5
				this.parent.MapManagement.F0_2aea_1412_SetCellActivePlayerID(unit.Position.X, unit.Position.Y, playerID, unitID);
	
				if (!this.Var_20f4 && this.parent.Var_3936 == -1)
				{
					if (this.parent.Var_d806_DebugFlag || playerID == this.parent.GameData.HumanPlayerID ||
						(unit.VisibleByPlayer & (1 << this.parent.GameData.HumanPlayerID)) != 0)
					{	
						// Instruction address 0x1866:0x107d, size: 5
						this.parent.MapManagement.F0_2aea_11d4_DrawCellWithUnit(unit.Position.X, unit.Position.Y);
					}
				}
			}			
		}

		/// <summary>
		/// Enumerates the stack of units and gets best defense unit
		/// This function searches for additional unit(s) that are on the same location, but not in a stack!
		/// </summary>
		/// <param name="playerID"></param>
		/// <param name="unitID"></param>
		/// <returns></returns>
		public int F0_1866_1089_GetBestDefenseUnitInStack(int playerID, int unitID)
		{
			//this.oCPU.Log.EnterBlock($"F0_1866_1089({playerID}, {unitID})");

			// function body
			if (this.parent.GameData.Players[playerID].Units[unitID].NextUnitID == -1)
			{
				return unitID;
			}

			this.Var_6530 = unitID;
			this.Var_6534 = -1;

			for (int i = 0; i < 128; i++)
			{
				if (this.parent.GameData.Players[playerID].Units[i].UnitType != UnitTypeEnum.None &&
					(this.parent.GameData.Players[playerID].Units[unitID].Position.X == this.parent.GameData.Players[playerID].Units[i].Position.X) &&
					(this.parent.GameData.Players[playerID].Units[unitID].Position.Y == this.parent.GameData.Players[playerID].Units[i].Position.Y))
				{
					// Instruction address 0x1866:0x1113, size: 3
					F0_1866_1169_GetBestDefenseUnitInStack(playerID, i);
					break;
				}
			}

			return this.Var_6530;
		}

		/// <summary>
		/// Enumerates the stack of units and gets best defense unit
		/// </summary>
		/// <param name="playerID"></param>
		/// <param name="unitID"></param>
		/// <returns></returns>
		public int F0_1866_1122_GetBestDefenseUnitInStack(int playerID, int unitID)
		{
			//this.oCPU.Log.EnterBlock($"F0_1866_1122({playerID}, {unitID})");

			// function body			
			if (this.parent.GameData.Players[playerID].Units[unitID].NextUnitID == -1)
			{
				return unitID;
			}

			this.Var_6530 = unitID;
			this.Var_6534 = -1;

			// Instruction address 0x1866:0x115d, size: 3
			F0_1866_1169_GetBestDefenseUnitInStack(playerID, unitID);

			return this.Var_6530;
		}

		/// <summary>
		/// Enumerates the stack of units and gets best defense unit
		/// </summary>
		/// <param name="playerID"></param>
		/// <param name="unitID"></param>
		private void F0_1866_1169_GetBestDefenseUnitInStack(int playerID, int unitID)
		{
			int oldUnitID = unitID;

			for (int i = 0; i < 10 && oldUnitID != -1; i++)
			{
				int newUnitID = this.parent.GameData.Players[playerID].Units[oldUnitID].NextUnitID;

				UnitTypeEnum unitType = this.parent.GameData.Players[playerID].Units[oldUnitID].UnitType;

				// Instruction address 0x1866:0x1195, size: 5
				TerrainTypeEnum terrainType = this.parent.MapManagement.GetTerrainType(this.parent.GameData.Players[playerID].Units[oldUnitID].Position.X,
					this.parent.GameData.Players[playerID].Units[oldUnitID].Position.Y);

				if (terrainType != TerrainTypeEnum.Water || this.parent.GameData.Units[(int)unitType].MovementType == UnitMovementTypeEnum.Water)
				{
					int defenseStrength;

					if (this.parent.GameData.Units[(int)unitType].MovementType == UnitMovementTypeEnum.Land)
					{

						if ((this.parent.GameData.Players[playerID].Units[oldUnitID].Status & UnitStatusEnum.Fortified) != UnitStatusEnum.None)
						{
							defenseStrength = this.parent.GameData.Units[(int)unitType].DefenseStrength * 3 * this.parent.GameData.Terrains[(int)terrainType].DefenseBonus * 8;
						}
						else
						{
							defenseStrength = this.parent.GameData.Units[(int)unitType].DefenseStrength * 2 * this.parent.GameData.Terrains[(int)terrainType].DefenseBonus * 8;
						}
					}
					else
					{
						defenseStrength = this.parent.GameData.Units[(int)unitType].DefenseStrength * 16;
					}

					if ((this.parent.GameData.Players[playerID].Units[oldUnitID].Status & UnitStatusEnum.Veteran) != UnitStatusEnum.None)
					{
						defenseStrength += defenseStrength / 2;
					}

					if (defenseStrength > this.Var_6534)
					{
						this.Var_6534 = defenseStrength;
						this.Var_6530 = oldUnitID;
					}
				}


				oldUnitID = newUnitID;

				if (oldUnitID == unitID)
					break;
			}
		}

		/// <summary>
		/// Get unit value coefficient according to value type in a stack
		/// </summary>
		/// <param name="playerID"></param>
		/// <param name="unitID"></param>
		/// <param name="valueType"></param>
		/// <returns></returns>
		public int F0_1866_1251_GetStackUnitValueSum(int playerID, int unitID, UnitStackValueTypeEnum valueType)
		{
			//this.oCPU.Log.EnterBlock($"F0_1866_1251({playerID}, {unitID}, {param3})");

			// function body
			int oldUnitID = unitID;
			int value = 0;

			for (int i = 0; i < 10 && oldUnitID != -1; i++)
			{
				int newUnitID = this.parent.GameData.Players[playerID].Units[oldUnitID].NextUnitID;

				value += F0_1866_1280_GetUnitValue(playerID, oldUnitID, unitID, valueType);

				oldUnitID = newUnitID;

				if (oldUnitID == unitID)
					break;
			}

			return value;
		}

		/// <summary>
		/// Gets the unit value according to valueType provided
		/// </summary>
		/// <param name="playerID"></param>
		/// <param name="unitID"></param>
		private int F0_1866_1280_GetUnitValue(int playerID, int unitID, int mainUnitID, UnitStackValueTypeEnum valueType)
		{
			//this.oCPU.Log.EnterBlock($"F0_1866_1280({playerID}, {unitID})");

			// function body
			int value = 0;

			switch (valueType)
			{
				case UnitStackValueTypeEnum.Cost:
					value += this.parent.GameData.Units[(int)this.parent.GameData.Players[playerID].Units[unitID].UnitType].Cost;
					break;

				case UnitStackValueTypeEnum.DefenseStrength:
					value += this.parent.GameData.Units[(int)this.parent.GameData.Players[playerID].Units[unitID].UnitType].DefenseStrength;
					break;

				case UnitStackValueTypeEnum.UnitCount:
					value++;
					break;

				case UnitStackValueTypeEnum.AttackStrength:
					value += this.parent.GameData.Units[(int)this.parent.GameData.Players[playerID].Units[unitID].UnitType].AttackStrength;
					break;

					// !!! To Check: This makes no sense, why would unitID and mainUnitID matter at all?
				case UnitStackValueTypeEnum.DefenseUnitCount:
					if (unitID < mainUnitID && 
						this.parent.GameData.Units[(int)this.parent.GameData.Players[playerID].Units[unitID].UnitType].UnitRoleType == UnitRoleTypeEnum.Defense)
					{
						value++;
					}
					break;
			}

			return value;
		}

		/// <summary>
		/// Counts the number of units in a stack according to unitType
		/// </summary>
		/// <param name="playerID"></param>
		/// <param name="unitID"></param>
		/// <param name="unitType"></param>
		/// <returns></returns>
		public int F0_1866_1331_CountUnitTypesInStack(int playerID, int unitID, UnitTypeEnum unitType)
		{
			//this.oCPU.Log.EnterBlock($"F0_1866_1331({playerID}, {unitID}, {param3})");

			// function body			
			int oldUnitID = unitID;
			int count = 0;

			for (int i = 0; i < 10 && oldUnitID != -1; i++)
			{
				int newUnitID = this.parent.GameData.Players[playerID].Units[oldUnitID].NextUnitID;

				if (this.parent.GameData.Players[playerID].Units[oldUnitID].UnitType == unitType)
				{
					count++;
				}

				oldUnitID = newUnitID;

				if (oldUnitID == unitID)
					break;
			}

			return count;
		}

		/// <summary>
		/// Count units in a stack that have same unit role
		/// </summary>
		/// <param name="playerID"></param>
		/// <param name="unitID"></param>
		/// <param name="unitRole"></param>
		/// <returns></returns>
		public int F0_1866_1380_GetStackUnitCount(int playerID, int unitID, UnitRoleTypeEnum unitRole)
		{
			//this.oCPU.Log.EnterBlock($"F0_1866_1380({playerID}, {unitID}, {aiRole})");

			// function body
			int oldUnitID = unitID;
			int count = 0;

			// just in case we get invalid playerID or unitID
			if (playerID >= 0 && unitID >= 0)
			{
				for (int i = 0; i < 10 && oldUnitID != -1; i++)
				{
					int newUnitID = this.parent.GameData.Players[playerID].Units[oldUnitID].NextUnitID;

					if (this.parent.GameData.Units[(int)this.parent.GameData.Players[playerID].Units[oldUnitID].UnitType].UnitRoleType == unitRole)
					{
						count++;
					}

					oldUnitID = newUnitID;

					if (oldUnitID == unitID)
						break;
				}
			}

			return count;
		}

		/// <summary>
		/// Counts if water transport units can sustain embarked land units
		/// </summary>
		/// <param name="playerID"></param>
		/// <param name="unitID"></param>
		/// <returns></returns>
		public int F0_1866_13d5_GetWaterTransportCapabilityCount(int playerID, int unitID)
		{
			//this.oCPU.Log.EnterBlock($"F0_1866_13d5({playerID}, {unitID})");

			// function body
			int oldUnitID = unitID;
			int count = 0;

			for (int i = 0; i < 10 && oldUnitID != -1; i++)
			{
				if (this.parent.GameData.Units[(int)this.parent.GameData.Players[playerID].Units[oldUnitID].UnitType].MovementType == UnitMovementTypeEnum.Land)
				{
					count--;
				}

				if (this.parent.GameData.Units[(int)this.parent.GameData.Players[playerID].Units[oldUnitID].UnitType].UnitRoleType == UnitRoleTypeEnum.SeaTransport)
				{
					count += this.parent.GameData.Units[(int)this.parent.GameData.Players[playerID].Units[oldUnitID].UnitType].TransportCapacity;
				}

				oldUnitID = this.parent.GameData.Players[playerID].Units[oldUnitID].NextUnitID;

				if (oldUnitID == unitID)
					break;
			}

			return count;
		}

		/// <summary>
		/// Update sentry flags on a transported units
		/// </summary>
		/// <param name="playerID"></param>
		/// <param name="unitID"></param>
		private void F0_1866_14a2_UnitStackUpdateSentryFlag(int playerID, int unitID)
		{
			int oldUnitID = unitID;

			for (int i = 0; i < 10 && oldUnitID != -1; i++)
			{
				int newUnitID = this.parent.GameData.Players[playerID].Units[oldUnitID].NextUnitID;

				if (this.parent.MapManagement.GetTerrainType(this.parent.GameData.Players[playerID].Units[oldUnitID].Position.X,
						this.parent.GameData.Players[playerID].Units[oldUnitID].Position.Y) != TerrainTypeEnum.Water)
				{
					this.parent.GameData.Players[playerID].Units[oldUnitID].ClearStatusFlags(UnitStatusEnum.Sentry);
				}
				else
				{
					if (this.parent.GameData.Units[(int)this.parent.GameData.Players[playerID].Units[oldUnitID].UnitType].MovementType != UnitMovementTypeEnum.Land)
					{
						this.parent.GameData.Players[playerID].Units[oldUnitID].ClearStatusFlags(UnitStatusEnum.Sentry);
					}
				}

				oldUnitID = newUnitID;

				if (oldUnitID == unitID)
					break;
			}
		}

		/// <summary>
		/// Activate unit on a transport
		/// </summary>
		/// <param name="playerID"></param>
		/// <param name="unitID"></param>
		public void F0_1866_14f6_ActivateUnitOnTransport(int playerID, int unitID)
		{
			int oldUnitID = unitID;

			for (int i = 0; i < 10 && oldUnitID != -1; i++)
			{
				int newUnitID = this.parent.GameData.Players[playerID].Units[oldUnitID].NextUnitID;

				if ((this.parent.GameData.Players[playerID].Units[oldUnitID].Status & UnitStatusEnum.Sentry) != UnitStatusEnum.None)
				{
					this.parent.GameData.Players[playerID].Units[oldUnitID].ClearStatusFlags(UnitStatusEnum.Sentry);

					this.parent.GameData.Players[playerID].Units[oldUnitID].RemainingMoves =
						(short)(this.parent.GameData.Units[(int)this.parent.GameData.Players[playerID].Units[oldUnitID].UnitType].MoveCount * 3);

					if (this.parent.GameData.Units[(int)this.parent.GameData.Players[playerID].Units[oldUnitID].UnitType].TurnsOutside != 0)
					{
						this.parent.GameData.Players[playerID].Units[oldUnitID].SpecialMoves =
							(short)(this.parent.GameData.Units[(int)this.parent.GameData.Players[playerID].Units[oldUnitID].UnitType].TurnsOutside - 1);
					}
				}

				this.parent.GameData.Players[playerID].Units[oldUnitID].GoToDestination = OpenCivOneGame.InvalidPosition;
				this.parent.GameData.Players[playerID].Units[oldUnitID].GoToNextDirection = -1;

				oldUnitID = newUnitID;

				if (oldUnitID == unitID)
					break;
			}
		}

		/// <summary>
		/// Update water unit statuses in a stack
		/// </summary>
		/// <param name="playerID"></param>
		/// <param name="unitID"></param>
		public void F0_1866_1560_UpdateWaterUnitStatusInStack(int playerID, int unitID)
		{
			int oldUnitID = unitID;

			for (int i = 0; i < 10 && oldUnitID != -1; i++)
			{
				int newUnitID = this.parent.GameData.Players[playerID].Units[oldUnitID].NextUnitID;

				if (this.parent.GameData.Units[(int)this.parent.GameData.Players[playerID].Units[oldUnitID].UnitType].MovementType == UnitMovementTypeEnum.Water)
				{
					// Instruction address 0x1866:0x158a, size: 3
					F0_1866_1593_UpdateUnitStatuses(playerID, oldUnitID);
				}

				oldUnitID = newUnitID;

				if (oldUnitID == unitID)
					break;
			}
		}

		/// <summary>
		/// Update all unit statuses in a stack
		/// </summary>
		/// <param name="playerID"></param>
		/// <param name="unitID"></param>
		public void F0_1866_1593_UpdateUnitStatusInStack(int playerID, int unitID)
		{
			int oldUnitID = unitID;

			for (int i = 0; i < 10 && oldUnitID != -1; i++)
			{
				int newUnitID = this.parent.GameData.Players[playerID].Units[oldUnitID].NextUnitID;

				F0_1866_1593_UpdateUnitStatuses(playerID, oldUnitID);

				oldUnitID = newUnitID;

				if (oldUnitID == unitID)
					break;
			}
		}

		/// <summary>
		/// ?
		/// </summary>
		/// <param name="playerID"></param>
		/// <param name="oldUnitID"></param>
		private void F0_1866_1593_UpdateUnitStatuses(int playerID, int oldUnitID)
		{
			//this.oCPU.Log.EnterBlock($"F0_1866_1593({playerID}, {unitID})");

			// function body
			if ((this.parent.GameData.Players[playerID].Units[oldUnitID].Status & (UnitStatusEnum.SettlerBuildRoadOrRail |
					UnitStatusEnum.SettlerBuildIrrigation | UnitStatusEnum.SettlerBuildMineOrForest)) != UnitStatusEnum.None)
			{
				this.parent.GameData.Players[playerID].Units[oldUnitID].SpecialMoves = 0;
			}

			if ((this.parent.GameData.Players[playerID].Units[oldUnitID].Status & (UnitStatusEnum.Sentry | UnitStatusEnum.SettlerBuildRoadOrRail |
				 UnitStatusEnum.Fortified | UnitStatusEnum.SettlerBuildIrrigation | UnitStatusEnum.SettlerBuildMineOrForest)) != UnitStatusEnum.None)
			{
				this.parent.GameData.Players[playerID].Units[oldUnitID].RemainingMoves =
					(short)(this.parent.GameData.Units[(int)this.parent.GameData.Players[playerID].Units[oldUnitID].UnitType].MoveCount * 3);
			}

			this.parent.GameData.Players[playerID].Units[oldUnitID].ClearStatusFlags(UnitStatusEnum.Sentry | UnitStatusEnum.SettlerBuildRoadOrRail |
				 UnitStatusEnum.Fortifying | UnitStatusEnum.Fortified | UnitStatusEnum.SettlerBuildIrrigation | UnitStatusEnum.SettlerBuildMineOrForest);

			this.parent.GameData.Players[playerID].Units[oldUnitID].GoToDestination = OpenCivOneGame.InvalidPosition;

			if (playerID == this.parent.GameData.HumanPlayerID)
			{
				// Instruction address 0x1866:0x1605, size: 5
				this.parent.MapManagement.F0_2aea_0e29_DrawUnit(playerID, oldUnitID);
			}
		}

		/// <summary>
		/// Deletes all land units in a stack, leaving the transport carrying them
		/// </summary>
		/// <param name="playerID"></param>
		/// <param name="unitID"></param>
		public void F0_1866_1610_DeleteLandUnitsInStack(int playerID, int unitID)
		{
			int oldUnitID = unitID;

			for (int i = 0; i < 10 && oldUnitID != -1; i++)
			{
				int newUnitID = this.parent.GameData.Players[playerID].Units[oldUnitID].NextUnitID;

				if (this.parent.GameData.Players[playerID].Units[oldUnitID].UnitType != UnitTypeEnum.None &&
					this.parent.GameData.Units[(int)this.parent.GameData.Players[playerID].Units[oldUnitID].UnitType].MovementType == UnitMovementTypeEnum.Land)
				{
					// Instruction address 0x1866:0x163a, size: 3
					F0_1866_0f10_DeleteUnit(playerID, oldUnitID);
				}

				oldUnitID = newUnitID;

				if (oldUnitID == unitID)
					break;
			}
		}

		/// <summary>
		/// Deletes all units in a stack that do not move on land
		/// </summary>
		/// <param name="playerID"></param>
		/// <param name="unitID"></param>
		public void F0_1866_1643_DeleteNonLandUnitsInStack(int playerID, int unitID)
		{
			int oldUnitID = unitID;

			for (int i = 0; i < 10 && oldUnitID != -1; i++)
			{
				int newUnitID = this.parent.GameData.Players[playerID].Units[oldUnitID].NextUnitID;

				if (this.parent.GameData.Players[playerID].Units[oldUnitID].UnitType != UnitTypeEnum.None &&
					this.parent.GameData.Units[(int)this.parent.GameData.Players[playerID].Units[oldUnitID].UnitType].MovementType != UnitMovementTypeEnum.Land)
				{
					// Instruction address 0x1866:0x166d, size: 3
					F0_1866_0f10_DeleteUnit(playerID, oldUnitID);
				}

				oldUnitID = newUnitID;

				if (oldUnitID == unitID)
					break;
			}
		}

		/// <summary>
		/// Deletes all air units in a stack
		/// </summary>
		/// <param name="playerID"></param>
		/// <param name="unitID"></param>
		private void F0_1866_1676_DeleteAirUnitInStack(int playerID, int unitID)
		{
			int oldUnitID = unitID;

			for (int i = 0; i < 10 && oldUnitID != -1; i++)
			{
				int newUnitID = this.parent.GameData.Players[playerID].Units[oldUnitID].NextUnitID;

				if (this.parent.GameData.Players[playerID].Units[oldUnitID].UnitType != UnitTypeEnum.None && 
					this.parent.GameData.Units[(int)this.parent.GameData.Players[playerID].Units[oldUnitID].UnitType].MovementType == UnitMovementTypeEnum.Air)
				{
					// Instruction address 0x1866:0x16a0, size: 3
					F0_1866_0f10_DeleteUnit(playerID, oldUnitID);
				}

				oldUnitID = newUnitID;

				if (oldUnitID == unitID)
					break;
			}
		}

		/// <summary>
		/// Center map according to given position
		/// </summary>
		/// <param name="playerID"></param>
		/// <param name="x"></param>
		/// <param name="y"></param>
		public void F0_1866_16a9_CenterMap(int playerID, int x, int y)
		{
			//this.oCPU.Log.EnterBlock($"F0_1866_16a9({playerID}, {x}, {y})");

			// function body
			bool flag = false;

			if (x < 16 && this.parent.Var_d4cc_MapViewX > 64)
			{
				x += 80;
			}

			if (x < this.parent.Var_d4cc_MapViewX + 2 || x > this.parent.Var_d4cc_MapViewX + 13 ||
				y < this.parent.Var_d75e_MapViewY + 2 || y > this.parent.Var_d75e_MapViewY + 9)
			{
				flag = true;
			}

			if (flag)
			{
				// Instruction address 0x1866:0x1719, size: 5
				this.parent.MapManagement.F0_2aea_0008_DrawVisibleMap(playerID, this.parent.MapManagement.AdjustXPosition(x - 8), y - 6);
			}
		}

		/// <summary>
		/// This function tests if unit is near and updates visibility if player is human player
		/// This should be reworked as the function should update visibility for any player taking into account the visibility range
		/// </summary>
		/// <param name="playerID"></param>
		/// <param name="x"></param>
		/// <param name="y"></param>
		public bool F0_1866_1725_IsUnitNear(int playerID, int x, int y)
		{
			//this.oCPU.Log.EnterBlock($"F0_1866_1725({playerID}, {x}, {y})");

			// function body
			if (!this.parent.MapManagement.F0_2aea_1585_GetVisibleTerrainImprovements(x, y).HasFlag(TerrainImprovementFlagsEnum.City))
			{
				// Instruction address 0x1866:0x1748, size: 3
				return F0_1866_1750_IsUnitOrCityNear(playerID, x, y);
			}

			return false;
		}

		/// <summary>
		/// This function tests if unit or city is near and updates visibility if player is human player
		/// This should be reworked as the function should update visibility for any player taking into account the visibility range
		/// </summary>
		/// <param name="playerID"></param>
		/// <param name="x"></param>
		/// <param name="y"></param>
		public bool F0_1866_1750_IsUnitOrCityNear(int playerID, int x, int y)
		{
			//this.oCPU.Log.EnterBlock($"F0_1866_1750({playerID}, {x}, {y})");

			// function body
			TerrainTypeEnum terrainType = this.parent.MapManagement.GetTerrainType(x, y);

			for(int i = 1; i < 9; i++)
			{
				GPoint direction = this.parent.MoveDirections[i];

				int newX = this.parent.MapManagement.AdjustXPosition(x + direction.X);
				int newY = y + direction.Y;

				if (this.parent.MapManagement.ValidateMapCoordinates(newX, newY) &&
					((this.parent.MapManagement.GetTerrainType(newX, newY) == TerrainTypeEnum.Water) ? 1 : 0) == ((terrainType == TerrainTypeEnum.Water) ? 1 : 0))
				{
					// Instruction address 0x1866:0x17d1, size: 5
					int unitPlayerID = this.parent.MapManagement.F0_2aea_14e0_GetCellActiveUnitPlayerID(newX, newY);

					if (unitPlayerID != -1 && unitPlayerID != playerID)
					{
						if (playerID == this.parent.GameData.HumanPlayerID)
						{
							ushort humanPlayerBitmask = (ushort)(0x1 << this.parent.GameData.HumanPlayerID);
							int unitID = this.parent.MapManagement.F0_2aea_1458_GetCellActiveUnitID(newX, newY);

							if ((this.parent.GameData.Players[unitPlayerID].Units[unitID].VisibleByPlayer & humanPlayerBitmask) == 0)
							{
								this.parent.GameData.Players[unitPlayerID].Units[unitID].VisibleByPlayer |= humanPlayerBitmask;

								this.parent.GameData.MapVisibility[newX, newY] |= humanPlayerBitmask;

								// Instruction address 0x1866:0x184e, size: 5
								this.parent.MapManagement.F0_2aea_11d4_DrawCellWithUnit(newX, newY);
							}

							// Instruction address 0x1866:0x185c, size: 5
							if (this.parent.MapManagement.F0_2aea_1585_GetVisibleTerrainImprovements(newX, newY).HasFlag(TerrainImprovementFlagsEnum.City))
							{
								// Instruction address 0x1866:0x186e, size: 5
								int cityID = this.parent.Tools.GetCityByLocation(newX, newY);

								this.parent.GameData.Cities[cityID].VisibleSize = this.parent.GameData.Cities[cityID].ActualSize;

								// Instruction address 0x1866:0x188f, size: 5
								this.parent.MapManagement.F0_2aea_1601_UpdateVisibleCellStatus(newX, newY);

								this.parent.GameData.MapVisibility[newX, newY] |= humanPlayerBitmask;

								// Instruction address 0x1866:0x18bb, size: 5
								this.parent.MapManagement.F0_2aea_11d4_DrawCellWithUnit(newX, newY);
							}
						}

						return true;
					}
				}
			}

			return false;
		}

		public bool IsEnemyUnitNear(int playerID, int x, int y, int distance)
		{
			ushort playerBitmask = (ushort)(0x1 << playerID);
			GPoint[] moveDirections = this.parent.MoveDirections;
			MapManagement mapManagement = this.parent.MapManagement;

			if (distance != 0)
			{
				for (int i = -distance; i < distance; i++)
				{
					for (int j = -distance; j < distance; j++)
					{
						if (i == 0 && j == 0)
							continue;

						int newX = x + i;
						int newY = y + j;
						int unitPlayerID = mapManagement.F0_2aea_14e0_GetCellActiveUnitPlayerID(newX, newY);
						int unitID = mapManagement.F0_2aea_1458_GetCellActiveUnitID(newX, newY);

						if (unitPlayerID != -1 && unitID != -1 && unitPlayerID != playerID &&
							(this.parent.GameData.Players[unitPlayerID].Units[unitID].VisibleByPlayer & playerBitmask) != 0)
						{
							return true;
						}
					}
				}
			}

			return false;
		}

		public bool IsEnemyCityNear(int playerID, int x, int y, int distance)
		{
			ushort playerBitmask = (ushort)(0x1 << playerID);
			GPoint[] moveDirections = this.parent.MoveDirections;
			MapManagement mapManagement = this.parent.MapManagement;

			if (distance != 0)
			{
				for (int i = -distance; i < distance; i++)
				{
					for (int j = -distance; j < distance; j++)
					{
						if (i == 0 && j == 0)
							continue;

						int newX = this.parent.MapManagement.AdjustXPosition(x + i);
						int newY = y + j;
						int cityID;

						if ((this.parent.GameData.MapVisibility[newX, newY] & playerBitmask) != 0 &&
							mapManagement.F0_2aea_1585_GetVisibleTerrainImprovements(newX, newY).HasFlag(TerrainImprovementFlagsEnum.City) &&
							(cityID = this.parent.Tools.GetCityByLocation(newX, newY)) != -1 &&
							this.parent.GameData.Cities[cityID].PlayerID != playerID)
						{
							return true;
						}
					}
				}
			}

			return false;
		}

		/// <summary>
		/// Check is enemy unit is near
		/// </summary>
		/// <param name="playerID"></param>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		public bool F0_1866_18d0_IsEnemyUnitNear(int playerID, int x, int y)
		{
			//this.oCPU.Log.EnterBlock($"F0_1866_18d0({playerID}, {x}, {y})");

			// function body
			for (int i = 1; i < 9; i++)
			{
				GPoint direction = this.parent.MoveDirections[i];

				// Instruction address 0x1866:0x18f4, size: 5
				int newX = this.parent.MapManagement.AdjustXPosition(x + direction.X);
				int newY = y + direction.Y;

				if (this.parent.MapManagement.ValidateMapCoordinates(newX, newY))
				{
					// Instruction address 0x1866:0x190d, size: 5
					int unitPlayerID = this.parent.MapManagement.F0_2aea_14e0_GetCellActiveUnitPlayerID(newX, newY);

					if (unitPlayerID != -1 && unitPlayerID != playerID)
					{
						return true;
					}
				}
			}
		
			return false;
		}

		/// <summary>
		/// Activate minor tribal hut
		/// </summary>
		/// <param name="playerID"></param>
		/// <param name="unitID"></param>
		public void F0_1866_1931_FoundMinorTribeHut(int playerID, int unitID)
		{
			//this.oCPU.Log.EnterBlock($"F0_1866_1931({playerID}, {unitID})");

			// function body
			if (playerID != 0 &&
				this.parent.GameData.Units[(int)this.parent.GameData.Players[playerID].Units[unitID].UnitType].MovementType != UnitMovementTypeEnum.Air)
			{
				// Instruction address 0x1866:0x1980, size: 5
				int nearestCityID = this.parent.Tools.FindNearestCity(this.parent.GameData.Players[playerID].Units[unitID].Position);

				// Instruction address 0x1866:0x19a9, size: 5
				int nearestCityDistance = (nearestCityID < 0 || nearestCityID > 127) ? int.MaxValue: this.parent.MapManagement.GetDistance(
					this.parent.GameData.Players[playerID].Units[unitID].Position, this.parent.GameData.Cities[nearestCityID].Position);

				// Instruction address 0x1866:0x19b8, size: 5
				switch (this.parent.CAPI.RNG.Next(4))
				{
					case 0:
						if (nearestCityDistance > 3)
						{
							// Instruction address 0x1866:0x1a0b, size: 3
							int playerID1 = this.parent.MapManagement.GetPlayerLandOwnership(
								this.parent.GameData.Players[playerID].Units[unitID].Position.X,
								this.parent.GameData.Players[playerID].Units[unitID].Position.Y) & 0x7;

							if (playerID1 > 6)
							{
								F0_1866_1931_TribeHutFoundAdvancedTribe(playerID, unitID);
							}
							else
							{
								F0_1866_1931_TribeHutFoundMetalDeposits(playerID, unitID);
							}
						}
						else
						{
							F0_1866_1931_TribeHutFoundSkilledMercenaries(playerID, unitID);
						}
						break;

					case 1:
						if (this.parent.GameData.TurnCount == 0 || this.parent.GameData.Year > 1000)
						{
							F0_1866_1931_TribeHutFoundMetalDeposits(playerID, unitID);
						}
						else
						{
							F0_1866_1931_TribeHutFoundAncientWisdom(playerID, unitID);
						}
						break;

					case 2:
						F0_1866_1931_TribeHutFoundMetalDeposits(playerID, unitID);
						break;

					case 3:
						if (nearestCityDistance < 4 || this.parent.GameData.Players[playerID].CityCount == 0)
						{
							F0_1866_1931_TribeHutFoundSkilledMercenaries(playerID, unitID);
						}
						else
						{
							F0_1866_1931_TribeHutFoundBarbarians(playerID, unitID);
						}
						break;

					case 4:
						F0_1866_1931_TribeHutFoundSkilledMercenaries(playerID, unitID);
						break;

					default:
						break;
				}
			}
		}

		private void F0_1866_1931_TribeHutFoundMetalDeposits(int playerID, int unitID)
		{
			if (playerID == this.parent.GameData.HumanPlayerID)
			{
				// Instruction address 0x1866:0x1ab2, size: 5
				this.parent.MenuBoxDialog.ShowDialog("You have discovered\nvaluable metal deposits\nworth 50 coins\n", 100, 80);
			}

			this.parent.GameData.Players[playerID].Coins += 50;
		}

		private void F0_1866_1931_TribeHutFoundAdvancedTribe(int playerID, int unitID)
		{
			if (playerID == this.parent.GameData.HumanPlayerID)
			{
				// Instruction address 0x1866:0x1a49, size: 5
				this.parent.MenuBoxDialog.ShowDialog("You have discovered\nan advanced tribe.\n", 100, 80);
			}

			this.parent.Overlay_20.F20_0000_0000(playerID,
				this.parent.GameData.Players[playerID].Units[unitID].Position.X, this.parent.GameData.Players[playerID].Units[unitID].Position.Y, 1);

			if (playerID == this.parent.GameData.HumanPlayerID)
			{
				// Instruction address 0x1866:0x1d47, size: 5
				this.parent.MapManagement.F0_2aea_11d4_DrawCellWithUnit(this.parent.GameData.Players[playerID].Units[unitID].Position.X,
					this.parent.GameData.Players[playerID].Units[unitID].Position.Y);
			}
		}

		private void F0_1866_1931_TribeHutFoundSkilledMercenaries(int playerID, int unitID)
		{
			if (playerID == this.parent.GameData.HumanPlayerID)
			{
				// Instruction address 0x1866:0x1bbe, size: 5
				this.parent.MenuBoxDialog.ShowDialog("You have discovered\na friendly tribe of\nskilled mercenaries.\n", 100, 80);
			}

			// Instruction address 0x1866:0x1d2d, size: 3
			F0_1866_0cf5_CreateUnit(playerID, (this.parent.CAPI.RNG.Next(2) != 0) ? UnitTypeEnum.Legion : UnitTypeEnum.Cavalry,
				this.parent.GameData.Players[playerID].Units[unitID].Position.X,
				this.parent.GameData.Players[playerID].Units[unitID].Position.Y);

			if (playerID == this.parent.GameData.HumanPlayerID)
			{
				// Instruction address 0x1866:0x1d47, size: 5
				this.parent.MapManagement.F0_2aea_11d4_DrawCellWithUnit(this.parent.GameData.Players[playerID].Units[unitID].Position.X,
					this.parent.GameData.Players[playerID].Units[unitID].Position.Y);
			}
		}

		private void F0_1866_1931_TribeHutFoundAncientWisdom(int playerID, int unitID)
		{
			if (playerID == this.parent.GameData.HumanPlayerID)
			{
				// Instruction address 0x1866:0x1aeb, size: 5
				this.parent.MenuBoxDialog.ShowDialog("You have discovered\nscrolls of ancient wisdom.\n", 100, 80);
			}

			// Compile a list of yet undiscovered technologies that have all of the prerequisites
			List<TechnologyAdvanceEnum> undiscoveredTech = new();

			foreach (TechnologyAdvanceEnum technology in Enum.GetValues(typeof(TechnologyAdvanceEnum)))
			{
				if (technology != TechnologyAdvanceEnum.None && technology != TechnologyAdvanceEnum.NewFutureTechnology &&
					!this.parent.Segment_1ade.F0_1ade_22b5_PlayerHasTechnology(playerID, technology) &&
					this.parent.Segment_1ade.F0_1ade_22b5_PlayerHasTechnology(playerID, this.parent.GameData.TechnologyAdvances[(int)technology].RequiresTechnologyAdvance1) &&
					this.parent.Segment_1ade.F0_1ade_22b5_PlayerHasTechnology(playerID, this.parent.GameData.TechnologyAdvances[(int)technology].RequiresTechnologyAdvance2))
				{
					undiscoveredTech.Add(technology);
				}
			}

			if (undiscoveredTech.Count > 0)
			{
				int index = this.parent.CAPI.RNG.Next(undiscoveredTech.Count);

				this.parent.Segment_1ade.F0_1ade_1d2e(playerID, 0, undiscoveredTech[index]);
			}
			else
			{
				F0_1866_1931_TribeHutFoundMetalDeposits(playerID, unitID);
			}
		}

		private void F0_1866_1931_TribeHutFoundBarbarians(int playerID, int unitID)
		{
			if (playerID == this.parent.GameData.HumanPlayerID)
			{
				// Instruction address 0x1866:0x1c23, size: 5
				this.parent.MenuBoxDialog.ShowDialog("You have unleashed\na horde of barbarians!\n", 100, 80);
			}

			for (int i = 1; i < 9;)
			{
				GPoint direction = this.parent.MoveDirections[i];

				int newX = this.parent.GameData.Players[playerID].Units[unitID].Position.X + direction.X;
				int newY = this.parent.GameData.Players[playerID].Units[unitID].Position.Y + direction.Y;

				// Instruction address 0x1866:0x1ccd, size: 5
				if (this.parent.MapManagement.F0_2aea_14e0_GetCellActiveUnitPlayerID(newX, newY) == -1 &&
					!this.parent.MapManagement.F0_2aea_1585_GetVisibleTerrainImprovements(newX, newY).HasFlag(TerrainImprovementFlagsEnum.City))
				{
					// Instruction address 0x1866:0x1cf0, size: 5
					if (this.parent.MapManagement.GetTerrainType(newX, newY) != TerrainTypeEnum.Water)
					{
						// Instruction address 0x1866:0x1c43, size: 3
						int newUnitID = F0_1866_0cf5_CreateUnit(0, 
							(this.parent.GameData.Terrains[(int)this.parent.MapManagement.GetTerrainType(newX, newY)].MovementCost < 3) ? UnitTypeEnum.Cavalry : UnitTypeEnum.Legion, 
							newX, newY);

						// In case our unit count has reached capacity
						if (newUnitID != -1)
						{
							this.parent.GameData.Players[0].Units[newUnitID].VisibleByPlayer |= (ushort)(1 << playerID);
						}

						if (playerID == this.parent.GameData.HumanPlayerID)
						{
							// Instruction address 0x1866:0x1c69, size: 5
							this.parent.MapManagement.F0_2aea_11d4_DrawCellWithUnit(newX, newY);
						}
					}
				}
				// Instruction address 0x1866:0x1c86, size: 5
				i += this.parent.Tools.CheckValueRange(4 - this.parent.GameData.Players[playerID].CityCount, 1, 4);
			}
		}

		/// <summary>
		/// Animates moving of a unit
		/// </summary>
		/// <param name="playerID"></param>
		/// <param name="unitID"></param>
		/// <param name="moveDirection"></param>
		public void F0_1866_1d55_AnimateUnitMove(int playerID, int unitID, int moveDirection)
		{
			//this.oCPU.Log.EnterBlock($"F0_1866_1d55({playerID}, {unitID}, {moveDirection})");

			// function body
			GPoint direction = this.parent.MoveDirections[moveDirection];

			// Instruction address 0x1866:0x1dae, size: 5
			if (this.parent.MapManagement.F0_2aea_03ba_DrawCell(this.parent.GameData.Players[playerID].Units[unitID].Position.X,
					this.parent.GameData.Players[playerID].Units[unitID].Position.Y) &&
				this.parent.PlayerTurn.F0_1403_4508_ValidateMapCoordinates(this.parent.GameData.Players[playerID].Units[unitID].Position.X,
					this.parent.GameData.Players[playerID].Units[unitID].Position.Y) &&
				this.parent.PlayerTurn.F0_1403_4508_ValidateMapCoordinates(this.parent.GameData.Players[playerID].Units[unitID].Position.X + direction.X,
					this.parent.GameData.Players[playerID].Units[unitID].Position.Y + direction.Y))
			{
				if (this.parent.GameData.Players[playerID].Units[unitID].NextUnitID != -1 &&
					(this.parent.MapManagement.GetTerrainType(this.parent.GameData.Players[playerID].Units[unitID].Position.X,
						this.parent.GameData.Players[playerID].Units[unitID].Position.Y) != TerrainTypeEnum.Water ||
					this.parent.GameData.Units[(int)this.parent.GameData.Players[playerID].Units[this.parent.GameData.Players[playerID].Units[unitID].NextUnitID].UnitType].MovementType == UnitMovementTypeEnum.Water))
				{
					// Instruction address 0x1866:0x1e59, size: 5
					this.parent.MapManagement.F0_2aea_0e29_DrawUnit(playerID, this.parent.GameData.Players[playerID].Units[unitID].NextUnitID);
				}

				// Instruction address 0x1866:0x1e7f, size: 5
				this.parent.Graphics.F0_VGA_07d8_DrawImage(this.parent.Var_aa_Screen0_Rectangle, 80, 0, 240, 200, this.parent.Var_19d4_Screen1_Rectangle, 80, 0);

				// Instruction address 0x1866:0x1ea2, size: 5
				int mapX = this.parent.MapManagement.AdjustXPosition(this.parent.GameData.Players[playerID].Units[unitID].Position.X - this.parent.Var_d4cc_MapViewX) * 16 + 80;
				int mapY = (this.parent.GameData.Players[playerID].Units[unitID].Position.Y - this.parent.Var_d75e_MapViewY) * 16 + 8;

				for (int i = 0; i < 17; i++)
				{
					// Instruction address 0x1866:0x1f0a, size: 5
					this.parent.Graphics.F0_VGA_0c3e_DrawBitmapToScreen(this.parent.Var_aa_Screen0_Rectangle,
						(direction.X * i) + mapX + 1, (direction.Y * i) + mapY + 1,
						this.parent.Array_d4ce[64 + (int)this.parent.GameData.Players[playerID].Units[unitID].UnitType + (playerID * 32)]);

					// Instruction address 0x1866:0x1f16, size: 5
					this.parent.CommonTools.WaitTimer(1);

					int newX = direction.X * i + mapX;
					int newY = direction.Y * i + mapY;

					// Instruction address 0x1866:0x1f4a, size: 5
					this.parent.Graphics.F0_VGA_07d8_DrawImage(this.parent.Var_19d4_Screen1_Rectangle, newX, newY, 16, 16, this.parent.Var_aa_Screen0_Rectangle, newX, newY);
				}
			}
		}

		/// <summary>
		/// Shows unit stack panel list
		/// </summary>
		/// <param name="playerID"></param>
		/// <param name="unitID"></param>
		public int F0_1866_1f69_ShowUnitStackPanelList(int playerID, int unitID)
		{
			//this.oCPU.Log.EnterBlock($"F0_1866_1f69({playerID}, {unitID})");

			// function body
			this.parent.Graphics.F0_VGA_07d8_DrawImage(this.parent.Var_aa_Screen0_Rectangle, 0, 0, 320, 200, this.parent.Var_19d4_Screen1_Rectangle, 0, 0);

			// Instruction address 0x1866:0x1f81, size: 3
			int selectedUnitID = F0_1866_1089_GetBestDefenseUnitInStack(playerID, unitID);
			int[] unitIDs = new int[12];
			int nextUnitID = unitID;
			int unitCount = 0;

			do
			{
				unitIDs[unitCount] = nextUnitID;
				nextUnitID = this.parent.GameData.Players[playerID].Units[nextUnitID].NextUnitID;

				unitCount++;
			}
			while (unitCount < 12 && nextUnitID != -1 && nextUnitID != unitID);

			int yTop = 96 - unitCount * 8;

		L1fe2:
			// Instruction address 0x1866:0x1ffc, size: 5
			this.parent.DrawTools.FillRectangleWithDoubleShadow(100, yTop, 120, (unitCount * 16) + 5, 3);

			unitCount = 0;
			nextUnitID = unitID;
			int unitYPos = yTop + 5;

			do
			{
				// Instruction address 0x1866:0x2025, size: 5
				this.parent.MapManagement.F0_2aea_0fb3_DrawUnitWithStatus(playerID, nextUnitID, 106, unitYPos);

				// Instruction address 0x1866:0x2079, size: 5
				this.parent.DrawTools.DrawStringToScreen0($"{this.parent.GameData.Units[(int)this.parent.GameData.Players[playerID].Units[nextUnitID].UnitType].Name}" +
					$"{(((this.parent.GameData.Players[playerID].Units[nextUnitID].Status & UnitStatusEnum.Veteran) != UnitStatusEnum.None) ? " (V)" : "")}", 128, unitYPos, 15);

				// Instruction address 0x1866:0x20b7, size: 5
				this.parent.DrawTools.DrawStringToScreen0(this.parent.Segment_2459.F0_2459_08c6_GetCityName(this.parent.GameData.Players[playerID].Units[nextUnitID].HomeCityID), 
					128, unitYPos + 8, 14);

				nextUnitID = this.parent.GameData.Players[playerID].Units[nextUnitID].NextUnitID;

				unitYPos += 16;
				unitCount++;
			}
			while (unitCount < 12 && nextUnitID != -1 && nextUnitID != unitID);

			// Instruction address 0x1866:0x20e9, size: 5
			this.parent.CommonTools.ClearKeyboardAndMouseEvents();

			MouseEvent mouseEvent = this.parent.GetMouseEvent();

			while ((mouseEvent = this.parent.GetMouseEvent()).Buttons == MouseButtonsEnum.None && this.parent.CAPI.kbhit() == 0) { }

			if (mouseEvent.Buttons == MouseButtonsEnum.Left && mouseEvent.Position.X > 100 && mouseEvent.Position.X < 220)
			{
				nextUnitID = (mouseEvent.Position.Y - yTop - 5) / 16;

				if (nextUnitID >= 0 && nextUnitID < unitCount)
				{
					selectedUnitID = unitIDs[nextUnitID];

					if ((this.parent.GameData.Players[playerID].Units[selectedUnitID].Status & (UnitStatusEnum.SettlerBuildRoadOrRail | 
						UnitStatusEnum.SettlerBuildIrrigation | UnitStatusEnum.SettlerBuildMineOrForest)) != UnitStatusEnum.None)
					{
						this.parent.GameData.Players[playerID].Units[unitID].RemainingMoves =
							(short)(this.parent.GameData.Units[(int)this.parent.GameData.Players[playerID].Units[selectedUnitID].UnitType].MoveCount * 3);
					}

					this.parent.GameData.Players[playerID].Units[selectedUnitID].ClearStatusFlags(UnitStatusEnum.Sentry | UnitStatusEnum.SettlerBuildRoadOrRail |
						 UnitStatusEnum.Fortifying | UnitStatusEnum.Fortified | UnitStatusEnum.SettlerBuildIrrigation | UnitStatusEnum.SettlerBuildMineOrForest);
					this.parent.GameData.Players[playerID].Units[selectedUnitID].GoToDestination = OpenCivOneGame.InvalidPosition;

					// !!! This looks like a closed loop
					if (unitCount > 1) goto L1fe2;
				}
			}

			this.parent.Graphics.F0_VGA_07d8_DrawImage(this.parent.Var_19d4_Screen1_Rectangle, 0, 0, 320, 200, this.parent.Var_aa_Screen0_Rectangle, 0, 0);

			// Instruction address 0x1866:0x225f, size: 5
			this.parent.CommonTools.ClearKeyboardAndMouseEvents();

			return selectedUnitID;
		}

		/// <summary>
		/// Get nearest city direction to the city owned by playerID
		/// </summary>
		/// <param name="playerID"></param>
		/// <param name="unitID"></param>
		public int F0_1866_226d_GetNearestCityDirection(int playerID, int unitID)
		{
			// this.oCPU.Log.EnterBlock($"F0_1866_226d({playerID}, {unitID})");

			// function body
			int distance;
			int minDistance = 999;
			int x = 0;
			int y = 0;

			for (int i = 0; i < 128; i++)
			{
				if (this.parent.GameData.Cities[i].Status != CityStatusEnum.Invalid && this.parent.GameData.Cities[i].PlayerID == playerID)
				{
					int xDistance = Math.Abs(this.parent.GameData.Players[playerID].Units[unitID].Position.X - this.parent.GameData.Cities[i].Position.X);

					if (xDistance > 40)
					{
						xDistance = 80 - xDistance;
					}
				
					int yDistance = Math.Abs(this.parent.GameData.Players[playerID].Units[unitID].Position.Y - this.parent.GameData.Cities[i].Position.Y);

					if (xDistance < yDistance)
					{
						distance = yDistance;
					}
					else
					{
						distance = xDistance;
					}

					if (distance < minDistance)
					{
						minDistance = distance;
						x = this.parent.GameData.Cities[i].Position.X;
						y = this.parent.GameData.Cities[i].Position.Y;
					}
				}
			}

			for (int i = 0; i < 128; i++)
			{
				if (this.parent.GameData.Players[playerID].Units[i].UnitType == UnitTypeEnum.Carrier)
				{
					// Instruction address 0x1866:0x239d, size: 5
					int xDistance = Math.Abs(this.parent.GameData.Players[playerID].Units[unitID].Position.X - this.parent.GameData.Players[playerID].Units[i].Position.X);

					if (xDistance > 40)
					{
						xDistance = 80 - xDistance;
					}

					int yDistance = Math.Abs(this.parent.GameData.Players[playerID].Units[unitID].Position.Y - this.parent.GameData.Players[playerID].Units[i].Position.Y);

					if (xDistance < yDistance)
					{
						distance = yDistance;
					}
					else
					{
						distance = xDistance;
					}

					if (distance < minDistance)
					{
						minDistance = distance;
						x = this.parent.GameData.Players[playerID].Units[i].Position.X;
						y = this.parent.GameData.Players[playerID].Units[i].Position.Y;
					}
				}
			}

			int selectedDirection = 0;

			// Instruction address 0x1866:0x244c, size: 5
			minDistance = this.parent.MapManagement.GetDistance(
				this.parent.GameData.Players[playerID].Units[unitID].Position.X - x,
				this.parent.GameData.Players[playerID].Units[unitID].Position.Y - y);

			for (int i = 1; i < 9; i++)
			{
				GPoint direction = this.parent.MoveDirections[i];

				int newX = this.parent.GameData.Players[playerID].Units[unitID].Position.X + direction.X;
				int newY = this.parent.GameData.Players[playerID].Units[unitID].Position.Y + direction.Y;

				if (this.parent.MapManagement.ValidateMapCoordinates(newX, newY))
				{
					// Instruction address 0x1866:0x248f, size: 5
					int newPlayerID = this.parent.MapManagement.F0_2aea_14e0_GetCellActiveUnitPlayerID(newX, newY);

					if (newPlayerID != -1 && newPlayerID != playerID)
					{
						// Instruction address 0x1866:0x24ad, size: 5
						if (!this.parent.MapManagement.F0_2aea_1585_GetVisibleTerrainImprovements(newX, newY).HasFlag(TerrainImprovementFlagsEnum.City) ||
							this.parent.MapManagement.F0_2aea_1369_GetCityOwner(newX, newY) == playerID)
						{
							// Instruction address 0x1866:0x24da, size: 5
							distance = this.parent.MapManagement.GetDistance(newX - x, newY - y);

							if (distance < minDistance)
							{
								minDistance = distance;
								selectedDirection = i;
							}
						}
					}
				}
			}

			return selectedDirection;
		}

		/// <summary>
		/// Draws random food icons
		/// !!! Why?
		/// </summary>
		public void F0_1866_260e_DrawRandomFoodIcons()
		{
			//this.oCPU.Log.EnterBlock("F0_1866_260e()");

			// function body
			// Instruction address 0x1866:0x261a, size: 5
			int x = this.parent.CAPI.RNG.Next(2) * 32 + 192;
			int y = 120;

			// Instruction address 0x1866:0x2649, size: 5
			this.parent.Graphics.F0_VGA_07d8_DrawImage(this.parent.Var_19e8_Screen2_Rectangle, x, 120, 8, 8, this.parent.Var_aa_Screen0_Rectangle, 0, 0);

			// Instruction address 0x1866:0x2670, size: 5
			this.parent.Graphics.F0_VGA_07d8_DrawImage(this.parent.Var_19e8_Screen2_Rectangle, x + 8, 120, 8, 8, this.parent.Var_aa_Screen0_Rectangle, 312, 0);

			// Instruction address 0x1866:0x2698, size: 5
			this.parent.Graphics.F0_VGA_07d8_DrawImage(this.parent.Var_19e8_Screen2_Rectangle, x + 24, 120, 8, 8, this.parent.Var_aa_Screen0_Rectangle, 312, 192);

			// Instruction address 0x1866:0x26bf, size: 5
			this.parent.Graphics.F0_VGA_07d8_DrawImage(this.parent.Var_19e8_Screen2_Rectangle, x + 16, 120, 8, 8, this.parent.Var_aa_Screen0_Rectangle, 0, 192);

			for (int i = 1; i < 24; i++)
			{
				// Instruction address 0x1866:0x26f2, size: 5
				this.parent.Graphics.F0_VGA_07d8_DrawImage(this.parent.Var_19e8_Screen2_Rectangle, x + 24, y + 8, 8, 8, this.parent.Var_aa_Screen0_Rectangle, 0, i * 8);

				// Instruction address 0x1866:0x2714, size: 5
				this.parent.Graphics.F0_VGA_07d8_DrawImage(this.parent.Var_19e8_Screen2_Rectangle, x + 8, y + 8, 8, 8, this.parent.Var_aa_Screen0_Rectangle, 312, i * 8);
			}

			for (int i = 1; i < 39; i++)
			{
				// Instruction address 0x1866:0x274c, size: 5
				this.parent.Graphics.F0_VGA_07d8_DrawImage(this.parent.Var_19e8_Screen2_Rectangle, x, y + 8, 8, 8, this.parent.Var_aa_Screen0_Rectangle, i * 8, 0);

				// Instruction address 0x1866:0x276e, size: 5
				this.parent.Graphics.F0_VGA_07d8_DrawImage(this.parent.Var_19e8_Screen2_Rectangle, x + 16, y + 8, 8, 8, this.parent.Var_aa_Screen0_Rectangle, i * 8, 192);
			}
		}

		/// <summary>
		/// ?
		/// </summary>
		/// <param name="replayID"></param>
		/// <param name="data1"></param>
		public void F0_1866_250e_AddReplayData(ushort replayID, byte data1)
		{
			//this.oCPU.Log.EnterBlock($"F0_1866_250e_AddReplayData({replayID}, {data1})");

			// function body
			if (this.parent.GameData.ReplayDataLength + 3 < 0x1000)
			{
				ushort usTemp = (ushort)((replayID << 12) | ((ushort)this.parent.GameData.TurnCount & 0xfff));

				this.parent.GameData.ReplayData[this.parent.GameData.ReplayDataLength] = (byte)((usTemp & 0xff00) >> 8);
				this.parent.GameData.ReplayDataLength++;

				this.parent.GameData.ReplayData[this.parent.GameData.ReplayDataLength] = (byte)(usTemp & 0xff);
				this.parent.GameData.ReplayDataLength++;

				this.parent.GameData.ReplayData[this.parent.GameData.ReplayDataLength] = data1;
				this.parent.GameData.ReplayDataLength++;
			}
		}

		/// <summary>
		/// ?
		/// </summary>
		/// <param name="replayID"></param>
		/// <param name="data1"></param>
		/// <param name="data2"></param>
		public void F0_1866_250e_AddReplayData(ushort replayID, byte data1, byte data2)
		{
			//this.oCPU.Log.EnterBlock($"F0_1866_250e_AddReplayData({replayID}, {data1}, {data2})");

			// function body
			if (this.parent.GameData.ReplayDataLength + 4 < 0x1000)
			{
				ushort usTemp = (ushort)((replayID << 12) | ((ushort)this.parent.GameData.TurnCount & 0xfff));

				this.parent.GameData.ReplayData[this.parent.GameData.ReplayDataLength] = (byte)((usTemp & 0xff00) >> 8);
				this.parent.GameData.ReplayDataLength++;

				this.parent.GameData.ReplayData[this.parent.GameData.ReplayDataLength] = (byte)(usTemp & 0xff);
				this.parent.GameData.ReplayDataLength++;

				this.parent.GameData.ReplayData[this.parent.GameData.ReplayDataLength] = data1;
				this.parent.GameData.ReplayDataLength++;
				this.parent.GameData.ReplayData[this.parent.GameData.ReplayDataLength] = data2;
				this.parent.GameData.ReplayDataLength++;
			}
		}

		/// <summary>
		/// ?
		/// </summary>
		/// <param name="replayID"></param>
		/// <param name="data1"></param>
		/// <param name="data2"></param>
		/// <param name="data3"></param>
		public void F0_1866_250e_AddReplayData(ushort replayID, byte data1, byte data2, byte data3)
		{
			//this.oCPU.Log.EnterBlock($"F0_1866_250e_AddReplayData({replayID}, {data1}, {data2}, {data3})");

			// function body
			if (this.parent.GameData.ReplayDataLength + 5 < 0x1000)
			{
				ushort usTemp = (ushort)((replayID << 12) | ((ushort)this.parent.GameData.TurnCount & 0xfff));

				this.parent.GameData.ReplayData[this.parent.GameData.ReplayDataLength] = (byte)((usTemp & 0xff00) >> 8);
				this.parent.GameData.ReplayDataLength++;

				this.parent.GameData.ReplayData[this.parent.GameData.ReplayDataLength] = (byte)(usTemp & 0xff);
				this.parent.GameData.ReplayDataLength++;

				this.parent.GameData.ReplayData[this.parent.GameData.ReplayDataLength] = data1;
				this.parent.GameData.ReplayDataLength++;
				this.parent.GameData.ReplayData[this.parent.GameData.ReplayDataLength] = data2;
				this.parent.GameData.ReplayDataLength++;
				this.parent.GameData.ReplayData[this.parent.GameData.ReplayDataLength] = data3;
				this.parent.GameData.ReplayDataLength++;
			}
		}

		/// <summary>
		/// ?
		/// </summary>
		/// <param name="replayID"></param>
		/// <param name="data1"></param>
		/// <param name="data2"></param>
		/// <param name="data3"></param>
		/// <param name="data4"></param>
		public void F0_1866_250e_AddReplayData(ushort replayID, byte data1, byte data2, byte data3, byte data4)
		{
			//this.oCPU.Log.EnterBlock($"F0_1866_250e_AddReplayData({replayID}, {data1}, {data2}, {data3}, {data4})");

			// function body
			if (this.parent.GameData.ReplayDataLength + 6 < 0x1000)
			{
				ushort usTemp = (ushort)((replayID << 12) | ((ushort)this.parent.GameData.TurnCount & 0xfff));

				this.parent.GameData.ReplayData[this.parent.GameData.ReplayDataLength] = (byte)((usTemp & 0xff00) >> 8);
				this.parent.GameData.ReplayDataLength++;

				this.parent.GameData.ReplayData[this.parent.GameData.ReplayDataLength] = (byte)(usTemp & 0xff);
				this.parent.GameData.ReplayDataLength++;

				this.parent.GameData.ReplayData[this.parent.GameData.ReplayDataLength] = data1;
				this.parent.GameData.ReplayDataLength++;
				this.parent.GameData.ReplayData[this.parent.GameData.ReplayDataLength] = data2;
				this.parent.GameData.ReplayDataLength++;
				this.parent.GameData.ReplayData[this.parent.GameData.ReplayDataLength] = data3;
				this.parent.GameData.ReplayDataLength++;
				this.parent.GameData.ReplayData[this.parent.GameData.ReplayDataLength] = data4;
				this.parent.GameData.ReplayDataLength++;
			}
		}
	}
}
