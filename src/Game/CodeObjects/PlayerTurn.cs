using IRB.VirtualCPU;
using OpenCivOne.Graphics;
using OpenCivOne.UI;
using System.Diagnostics;
using System.Text;

namespace OpenCivOne
{
	public class PlayerTurn
	{
		private OpenCivOneGame parent;

		public PlayerTurn(OpenCivOneGame parent)
		{
			this.parent = parent;
		}

		/// <summary>
		/// This function handles given Player turn. All unit movements, keyboard and mouse events
		/// </summary>
		/// <param name="playerID"></param>
		public void F0_1403_000e_PlayerTurn(int playerID)
		{
			//this.oCPU.Log.EnterBlock("'Fn1'(Cdecl, Far, Return) at 0xe");
			// Local variables
			GPoint direction;

			// function body
			this.parent.Var_6b90_CurrentPlayer = playerID;

			if (playerID == this.parent.GameData.HumanPlayerID)
			{
				this.parent.GameData.PlayerFlags = (short)(0x1 << this.parent.GameData.HumanPlayerID);

				if (this.parent.GameData.TurnCount == 20 || this.parent.GameData.TurnCount == 60)
				{
					this.parent.Help.F4_0000_02d3_ShowInstantAdvicePopup("*HELP1");
				}

				if (this.parent.GameData.TurnCount == 40 || this.parent.GameData.TurnCount == 80)
				{
					this.parent.Help.F4_0000_02d3_ShowInstantAdvicePopup("*HELP2");
				}
			}

			for (int i = 0; i < 128; i++)
			{
				if (this.parent.GameData.Players[playerID].Units[i].UnitType != UnitTypeEnum.None && this.parent.Var_df60 != 1)
				{
					this.parent.GameData.Players[playerID].Units[i].RemainingMoves =
						(short)(this.parent.GameData.Units[(int)this.parent.GameData.Players[playerID].Units[i].UnitType].MoveCount * 3);

					if (this.parent.GameData.Units[(int)this.parent.GameData.Players[playerID].Units[i].UnitType].MovementType != UnitMovementTypeEnum.Water)
					{
						if (this.parent.CityWorker.F0_1d12_6c97_PlayerHasWonder(playerID, WonderEnum.Lighthouse) ||
							this.parent.CityWorker.F0_1d12_6c97_PlayerHasWonder(playerID, WonderEnum.MagellansExpedition))
						{
							this.parent.GameData.Players[playerID].Units[i].RemainingMoves += 3;
						}
					}
				}
			}

			this.parent.RefreshScreenMap = false;

			int command;
			int maxUnitID = 127;
			bool flag2 = true;
			bool flag3 = false;
			bool flag4 = false;
			bool flag5 = false;
			bool flag6 = false;
			bool flag7 = false;

		Label17:
			int unitID = 0;
			flag2 = true;
			goto Label19;

		Label18:
			unitID++;

		Label19:
			if (unitID > 127) goto Label748;

			if (this.parent.Var_1ae0 != 0)
			{
				// Instruction address 0x1403:0x0160, size: 5
				this.parent.GameEngine.F0_1238_1b44_RedrawMapWithBlankingEffect();
			}

			if (this.parent.Var_dc48_GameEndType != 0)
			{
				unitID = 128;
				goto Label18;
			}

			if (unitID == maxUnitID)
			{
				flag4 = true;
			}

			if (this.parent.GameData.Players[playerID].Units[unitID].UnitType == UnitTypeEnum.None) goto Label18;

			if ((int)this.parent.GameData.Players[playerID].Units[unitID].UnitType > 27)
			{
				throw new Exception($"The unit has invalid UnitType of {(int)this.parent.GameData.Players[playerID].Units[unitID].UnitType}, " +
					$"unitID: {unitID}, player {this.parent.GameData.Players[playerID].Name}");

				//this.parent.GameData.Players[playerID].Units[unitID].UnitType = UnitTypeEnum.Militia;
			}

			if (!flag6)
			{
				// Instruction address 0x1403:0x01f4, size: 5
				this.parent.UnitManagement.F0_1866_01dc_UpdateUnitVisibility(
					this.parent.GameData.Players[playerID].Units[unitID].Position.X,
					this.parent.GameData.Players[playerID].Units[unitID].Position.Y,
					playerID, unitID, false);
			}

			if ((this.parent.GameData.Players[playerID].Units[unitID].Status & (UnitStatusEnum.Sentry | UnitStatusEnum.Fortified)) != UnitStatusEnum.None ||
				this.parent.GameData.Players[playerID].Units[unitID].RemainingMoves == 0) goto Label18;

			if ((this.parent.GameData.Players[playerID].Units[unitID].Status & UnitStatusEnum.Fortifying) != UnitStatusEnum.None)
			{
				this.parent.GameData.Players[playerID].Units[unitID].ClearStatusFlags(UnitStatusEnum.Fortifying);
				this.parent.GameData.Players[playerID].Units[unitID].Status |= UnitStatusEnum.Fortified;
				this.parent.GameData.Players[playerID].Units[unitID].RemainingMoves = 0;

				// Instruction address 0x1403:0x0265, size: 5
				F0_1403_3f13_DrawVisibleCellWithUnit(playerID, unitID);

				if (playerID != this.parent.GameData.HumanPlayerID)
				{
					if (this.parent.MapManagement.F0_2aea_1585_GetVisibleTerrainImprovements(this.parent.GameData.Players[playerID].Units[unitID].Position.X,
						this.parent.GameData.Players[playerID].Units[unitID].Position.Y).HasFlag(TerrainImprovementFlagsEnum.City))
					{
						int cityID = this.parent.MapManagement.GetCityByLocation(this.parent.GameData.Players[playerID].Units[unitID].Position.X,
							this.parent.GameData.Players[playerID].Units[unitID].Position.Y);

						if (this.parent.GameData.Cities[cityID].ActualSize >= 3)
						{
							this.parent.GameData.Players[playerID].Units[unitID].HomeCityID = (short)cityID;
						}
					}
				}

				if (this.parent.MapManagement.F0_2aea_1585_GetVisibleTerrainImprovements(this.parent.GameData.Players[playerID].Units[unitID].Position.X,
					this.parent.GameData.Players[playerID].Units[unitID].Position.Y).HasFlag(TerrainImprovementFlagsEnum.City))
				{
					int cityID = this.parent.MapManagement.GetCityByLocation(this.parent.GameData.Players[playerID].Units[unitID].Position.X,
						this.parent.GameData.Players[playerID].Units[unitID].Position.Y);

					if (this.parent.GameData.Players[playerID].Units[unitID].HomeCityID == cityID)
					{
						if (!this.parent.UnitManagement.F0_1866_18d0_IsEnemyUnitNear(playerID, this.parent.GameData.Players[playerID].Units[unitID].Position.X,
							this.parent.GameData.Players[playerID].Units[unitID].Position.Y))
						{
							this.parent.UnitManagement.F0_1866_00c6_StoreFortifiedCityUnits(cityID);
						}
					}
				}
				goto Label18;
			}

			if (playerID == this.parent.GameData.HumanPlayerID)
			{
				if ((this.parent.GameData.Players[playerID].Units[unitID].Status &
					(UnitStatusEnum.SettlerBuildRoadOrRail | UnitStatusEnum.SettlerBuildIrrigation | UnitStatusEnum.SettlerBuildMineOrForest)) == UnitStatusEnum.None)
				{
					if (this.parent.GameData.Players[playerID].Units[unitID].Position.X <= this.parent.Var_d4cc_MapViewX ||
						this.parent.GameData.Players[playerID].Units[unitID].Position.X >= this.parent.Var_d4cc_MapViewX + 14 ||
						this.parent.GameData.Players[playerID].Units[unitID].Position.Y <= this.parent.Var_d75e_MapViewY ||
						this.parent.GameData.Players[playerID].Units[unitID].Position.Y >= this.parent.Var_d75e_MapViewY + 10)
					{
						if (!flag4)
						{
							flag2 = false;
						}
						else
						{
							if (!this.parent.GlobalPreferences.Debug.FastMode)
								this.parent.CommonTools.WaitTimer(30);

							this.parent.MapManagement.F0_2aea_0008_DrawVisibleMap(playerID,
								this.parent.GameData.Players[playerID].Units[unitID].Position.X - 7,
								this.parent.GameData.Players[playerID].Units[unitID].Position.Y - 6);

							maxUnitID = unitID - 1;
							if (maxUnitID < 0)
								maxUnitID = 127;

							flag4 = false;

							if (this.parent.GameData.TurnCount == 0)
							{
								F0_1403_4060_DrawStatusPanel(playerID, unitID);
							}

							goto Label54;
						}

						goto Label18;
					}
				}
			}

		Label54:
			/*if ((this.parent.GameData.PlayerFlags & (0x1 << playerID)) != 0)
			{
				// Instruction address 0x1403:0x044c, size: 5
				this.parent.CommonTools.ClearKeyboardAndMouseEvents();
			}*/

			int unitMoveRetryCount = 0;

			if (flag3)
			{
				// Instruction address 0x1403:0x046b, size: 5
				this.parent.CommonTools.SetMousePositionAndIcon(0, 0, (short)this.parent.Array_d4ce[7]);
			}

			flag3 = false;
			flag7 = false;

		Label59:
			if ((this.parent.GameData.PlayerFlags & (0x1 << playerID)) == 0)
			{
				// Instruction address 0x1403:0x0496, size: 5
				command = this.parent.AIEngine.F0_25fb_0c9d_MoveUnit(playerID, unitID);

				if (command != 0)
				{
					this.parent.GameData.Players[playerID].Units[unitID].GoToDestination = OpenCivOneGame.InvalidPosition;
				}

				unitMoveRetryCount++;

				if (unitMoveRetryCount > 4)
				{
					command = ' ';
					this.parent.GameData.Players[playerID].Units[unitID].GoToDestination = OpenCivOneGame.InvalidPosition;
					this.parent.GameData.Players[playerID].Units[unitID].GoToNextDirection = -1;
				}

				goto Label151;
			}

			if (unitID >= 128 ||
				((this.parent.GameData.Players[playerID].Units[unitID].Status & 
					(UnitStatusEnum.SettlerBuildRoadOrRail | UnitStatusEnum.SettlerBuildIrrigation | UnitStatusEnum.SettlerBuildMineOrForest)) == UnitStatusEnum.None &&
				this.parent.GameData.Players[playerID].Units[unitID].GoToDestination.X == -1)) goto Label79;

			command = 'r';

			if ((this.parent.GameData.Players[playerID].Units[unitID].Status & UnitStatusEnum.SettlerBuildIrrigation) != UnitStatusEnum.None)
			{
				if (this.parent.GameData.Players[playerID].Units[unitID].UnitType != UnitTypeEnum.Settler)
				{
					command = 'm';
				}
				else
				{
					command = 'i';
				}
			}

			if ((this.parent.GameData.Players[playerID].Units[unitID].Status & UnitStatusEnum.SettlerBuildMineOrForest) != UnitStatusEnum.None)
			{
				command = 'm';

				if ((this.parent.GameData.Players[playerID].Units[unitID].Status & UnitStatusEnum.SettlerBuildIrrigation) != UnitStatusEnum.None)
				{
					command = 'f';
				}

				if ((this.parent.GameData.Players[playerID].Units[unitID].Status & UnitStatusEnum.SettlerBuildRoadOrRail) != UnitStatusEnum.None)
				{
					command = 'p';
				}
			}

			if (this.parent.GameData.Units[(int)this.parent.GameData.Players[playerID].Units[unitID].UnitType].MovementType == UnitMovementTypeEnum.Air)
			{
				command = 'h';
			}
			goto Label151;

		Label79:
			if (this.parent.RefreshScreenMap)
			{
				this.parent.MapManagement.F0_2aea_0008_DrawVisibleMap(this.parent.GameData.HumanPlayerID, this.parent.Var_d4cc_MapViewX, this.parent.Var_d75e_MapViewY);

				this.parent.RefreshScreenMap = false;
			}

			// Instruction address 0x1403:0x05d2, size: 5
			F0_1403_4060_DrawStatusPanel(playerID, unitID);

			if (this.parent.GameData.TurnCount == 0)
			{
				if (!flag6)
				{
					this.parent.Array_30b8[0] = this.parent.GameData.Players[playerID].Nation;
					this.parent.Help.F4_0000_02d3_ShowInstantAdvicePopup("*FIRSTMOVE");

					flag6 = true;
				}
			}

			if (unitID < 128 && this.parent.GameData.Players[playerID].Units[unitID].UnitType == UnitTypeEnum.Settler)
			{
				if (!flag6)
				{
					this.parent.Help.F4_0000_00af_ShowInstantAdvicePopup(playerID, unitID);

					flag6 = true;
				}
			}

			this.parent.UnitManagement.F0_1866_0ad6_ShowActiveUnitOrEndOfTurn(playerID, unitID, -1, 0);

			if (unitID < 128)
			{
				flag5 = true;
			}

			MouseEvent mouseEvent = this.parent.GetMouseEvent();
			command = -1;

			if (mouseEvent.Buttons == MouseButtonsEnum.None && this.parent.CAPI.kbhit() == 0) goto Label79;

			if (mouseEvent.Buttons == MouseButtonsEnum.None && this.parent.CAPI.kbhit() != 0)
			{
				// Instruction address 0x1403:0x0676, size: 5
				command = this.parent.CommonTools.GetNavigationKey();

				goto Label151;
			}

			int mouseX = this.parent.MapManagement.AdjustXPosition(((mouseEvent.Position.X - 80) / 16) + this.parent.Var_d4cc_MapViewX);
			int mouseY = ((mouseEvent.Position.Y - 8) / 16) + this.parent.Var_d75e_MapViewY;

			if (mouseEvent.Position.Y < 8)
			{
				// Instruction address 0x1403:0x0707, size: 5
				this.parent.Menus.F0_2c84_0000_ShowTopMenu(playerID, unitID, -1);

				// Instruction address 0x1403:0x070f, size: 5
				this.parent.CommonTools.ClearKeyboardAndMouseEvents();

				if (this.parent.Menus.Var_d4ca_MenuShortcutKey != -1)
				{
					command = this.parent.Menus.Var_d4ca_MenuShortcutKey;

					goto Label151;
				}
				goto Label722;
			}

			if (mouseEvent.Position.X < 80)
			{
				if (mouseEvent.Position.Y < 58)
				{
					//if (mouseEvent.Position.X > 0 && mouseEvent.Position.X < 79 &&
					//	mouseEvent.Position.Y > 8 && mouseEvent.Position.Y < 57)
					//{
					// X: 1-78 (78), Y: 9-56 (48)
					//this.parent.Graphics.F0_VGA_0599_DrawLine(this.parent.Var_aa_Screen0_Rectangle, 1, 9, 78, 9, 6);
					//this.parent.Graphics.F0_VGA_0599_DrawLine(this.parent.Var_aa_Screen0_Rectangle, 1, 56, 78, 56, 6);

					//this.parent.MapManagement.F0_2aea_0008_DrawVisibleMap(playerID, mouseEvent.Position.X - 1, mouseEvent.Position.Y - 9);

					this.parent.MapManagement.F0_2aea_0008_DrawVisibleMap(playerID,
						this.parent.MapManagement.AdjustXPosition(mouseEvent.Position.X - 7 + this.parent.Var_6ed6_MiniMapX),
						this.parent.CommonTools.CheckValueRange(mouseEvent.Position.Y - 14 + this.parent.Var_70ea_MiniMapY, 0, 49));
					//}
				}
				else if (mouseEvent.Position.Y < 72)
				{
					this.parent.Palace.F17_0000_07ec_DrawPalace(this.parent.GameData.HumanPlayerID, false);

					this.parent.Var_aa_Screen0_Rectangle.ScreenID = 0;

					// Instruction address 0x1403:0x07af, size: 5
					this.parent.CommonTools.StopPaletteCycleSlots();

					// Instruction address 0x1403:0x07cc, size: 5
					this.parent.DrawTools.FillRectangle(this.parent.Var_aa_Screen0_Rectangle, 0, 0, 320, 200, 0);

					// Instruction address 0x1403:0x07d8, size: 5
					this.parent.ImageTools.F0_2fa1_01a2_LoadBitmapOrPalette(-1, 0, 0, "CBACK.PIC", 1);

					// Instruction address 0x1403:0x0800, size: 5
					this.parent.Graphics.F0_VGA_07d8_DrawImage(this.parent.Var_19d4_Screen1_Rectangle, 0, 0, 320, 200, this.parent.Var_aa_Screen0_Rectangle, 0, 0);

					// Instruction address 0x1403:0x080d, size: 5
					this.parent.Segment_2459.F0_2459_0918_WaitForKeyPressOrMouseClick();

					// Instruction address 0x1403:0x0826, size: 5
					this.parent.GameEngine.F0_1238_1b44_RedrawMapWithBlankingEffect();

					// Instruction address 0x1403:0x082b, size: 5
					this.parent.CommonTools.StartPaletteCycleSlots();
				}
				else if (unitID >= 128)
				{
					goto Label755;
				}

				//this.parent.Var_db3a_MouseButton = 0;
				mouseEvent = new MouseEvent(mouseEvent.Position, MouseButtonsEnum.None);
			}

			if (mouseEvent.Buttons == MouseButtonsEnum.Right)
			{
				if (unitID < 128)
				{
					while (true)
					{
						mouseEvent = this.parent.GetMouseEvent();

						if (mouseEvent.Buttons == MouseButtonsEnum.None)
						{
							// Instruction address 0x1403:0x0882, size: 5
							mouseX = this.parent.MapManagement.AdjustXPosition((mouseEvent.Position.X - 80) / 16 + this.parent.Var_d4cc_MapViewX);
							mouseY = ((mouseEvent.Position.Y - 8) / 16) + this.parent.Var_d75e_MapViewY;

							// Instruction address 0x1403:0x08d0, size: 5
							int distance = this.parent.MapManagement.GetDistance(
								mouseX - this.parent.GameData.Players[playerID].Units[unitID].Position.X,
								mouseY - this.parent.GameData.Players[playerID].Units[unitID].Position.Y);

							if (Math.Abs(mouseX - this.parent.GameData.Players[playerID].Units[unitID].Position.X) == 79)
							{
								if (Math.Abs(mouseY - this.parent.GameData.Players[playerID].Units[unitID].Position.Y) <= 1)
								{
									distance = 1;
								}
							}

							if (distance == 1)
							{
								// The distance is one cell, we can issue a command for this movement
								Unit unit1 = this.parent.GameData.Players[playerID].Units[unitID];
								GPoint newMove = new GPoint(mouseX - unit1.Position.X, mouseY - unit1.Position.Y);

								command = -1;

								for (int i = 1; i < 9; i++)
								{
									if (this.parent.MoveDirections[i] == newMove)
									{
										command = i;
										break;
									}
								}

								if (command > 0)
								{
									goto Label151;
								}
								//this.parent.GameData.Players[playerID].Units[unitID].GoToDestination = new(mouseX, mouseY);
								//this.parent.GameData.Players[playerID].Units[unitID].GoToNextDirection = -1;

								mouseEvent = new MouseEvent(mouseEvent.Position, MouseButtonsEnum.None);
								//flag3 = false;
							}
							else
							{
								//this.parent.Var_db3a_MouseButton = 2;
								mouseEvent = new MouseEvent(mouseEvent.Position, MouseButtonsEnum.Right);

								if ((this.parent.GameData.MapVisibility[mouseX, mouseY] & (0x1 << this.parent.GameData.HumanPlayerID)) != 0 || this.parent.GlobalPreferences.Debug.DebugMode)
								{
									// Instruction address 0x1403:0x09b4, size: 5
									this.parent.Encyclopedia.DisplayEncyclopediaTopic(EncyclopediaTopicEnum.TerrainType, (int)this.parent.MapManagement.GetTerrainType(mouseX, mouseY));

									// Instruction address 0x1403:0x09c5, size: 5
									this.parent.GameEngine.F0_1238_1b44_RedrawMapWithBlankingEffect();
								}
							}
							break;
						}
					}
				}
			}
			else if (mouseEvent.Buttons == MouseButtonsEnum.Left)
			{
				if (flag3 && unitID < 128)
				{
					TerrainTypeEnum newTerrainType1 = this.parent.MapManagement.GetTerrainType(mouseX, mouseY);

					// local_36 == 2 || will never happen, because local_36 has only two values 0 and 1
					if ((this.parent.GameData.MapVisibility[mouseX, mouseY] & (0x1 << this.parent.GameData.HumanPlayerID)) != 0 &&
						((this.parent.GameData.Units[(int)this.parent.GameData.Players[playerID].Units[unitID].UnitType].MovementType == UnitMovementTypeEnum.Air && newTerrainType1 == TerrainTypeEnum.Water) ||
						(this.parent.GameData.Units[(int)this.parent.GameData.Players[playerID].Units[unitID].UnitType].MovementType == UnitMovementTypeEnum.Land && newTerrainType1 != TerrainTypeEnum.Water) ||
						(this.parent.MapManagement.F0_2aea_1585_GetVisibleTerrainImprovements(mouseX, mouseY).HasFlag(TerrainImprovementFlagsEnum.City) ||
							this.parent.GameData.Units[(int)this.parent.GameData.Players[playerID].Units[unitID].UnitType].MovementType == UnitMovementTypeEnum.Water)))
					{
						this.parent.GameData.Players[playerID].Units[unitID].GoToDestination = new(mouseX, mouseY);
						this.parent.GameData.Players[playerID].Units[unitID].GoToNextDirection = -1;
					}

					flag3 = false;

					this.parent.CommonTools.SetMousePositionAndIcon(0, 0, (short)this.parent.Array_d4ce[7]);
				}
				else
				{
					if (this.parent.MapManagement.F0_2aea_1585_GetVisibleTerrainImprovements(mouseX, mouseY).HasFlag(TerrainImprovementFlagsEnum.City) &&
						(this.parent.MapManagement.F0_2aea_1369_GetCityOwner(mouseX, mouseY) == this.parent.GameData.HumanPlayerID ||
							this.parent.GlobalPreferences.Debug.DebugMode))
					{
						int cityID = this.parent.MapManagement.GetCityByLocation(mouseX, mouseY);

						// Instruction address 0x1403:0x0b9a, size: 5
						this.parent.Segment_1ade.F0_1ade_03ea(cityID);
						// Instruction address 0x1403:0x0ba2, size: 5
						this.parent.GameEngine.F0_1238_107e_ReportHumanTaxAndPopulation();

						flag2 = false;
					}
					else
					{
						int activeUnitID1 = this.parent.MapManagement.F0_2aea_1458_GetCellActiveUnitID(mouseX, mouseY);
						int playerID1 = this.parent.MapManagement.F0_2aea_14e0_GetCellActiveUnitPlayerID(mouseX, mouseY);

						if (activeUnitID1 != -1 && this.parent.GlobalPreferences.Debug.DebugMode)
						{
							this.parent.ShowDebugDetails.ShowUnitStatus(playerID1, activeUnitID1);
						}

						if (activeUnitID1 != -1 && playerID == playerID1)
						{
							int local_3a = this.parent.UnitManagement.F0_1866_1f69_ShowUnitStackPanelList(playerID1, activeUnitID1);

							if ((this.parent.GameData.Players[playerID].Units[local_3a].Status & (UnitStatusEnum.Sentry | UnitStatusEnum.Fortified)) == UnitStatusEnum.None)
							{
								unitID = local_3a;
								maxUnitID = local_3a;
								if (maxUnitID < 0)
									maxUnitID = 127;

								flag4 = false;
								flag2 = false;

								goto Label722;
							}
						}
						else
						{
							// Instruction address 0x1403:0x0cdd, size: 5
							this.parent.MapManagement.F0_2aea_0008_DrawVisibleMap(playerID, mouseX - 7, mouseY - 6);
						}
					}
				}
			}

			// Instruction address 0x1403:0x0cfb, size: 5
			this.parent.CommonTools.ClearKeyboardEventsAndEnsureNoMouseButtonIsPressed();

		Label151:
			Unit? unit = null;
			TerrainTypeEnum terrainType = TerrainTypeEnum.Invalid;

			if (unitID < 128)
			{
				unit = this.parent.GameData.Players[playerID].Units[unitID];
				terrainType = this.parent.MapManagement.GetTerrainType(unit.Position.X, unit.Position.Y);
			}

			if (unit != null)
			{
				if (unit.GoToDestination.X != -1)
				{
					int command1 = this.parent.UnitGoTo.GetNextGoToMove(playerID, unitID);

					if (command1 == -1)
					{
						unit.GoToDestination = OpenCivOneGame.InvalidPosition;
						unit.GoToNextDirection = -1;
					}
					else
					{
						command = command1;
					}
				}
				else
				{
					unit.GoToNextDirection = -1;
				}
			}

			int nearestCityID;
			int newMoveDirection = 0;

			switch (command)
			{
				// Change government
				case -2:
					if (this.parent.MenuBoxDialog.ShowDialog(this.parent.LanguageTools.F0_2f4d_044f_GetTextFromKingSection("*REV"), 64, 80) == 1)
					{
						this.parent.GameData.Players[playerID].GovernmentType = GovernmentTypeEnum.Anarchy;

						this.parent.News.F21_0000_0000_ShowNews(-1, $"The {this.parent.GameData.Players[playerID].Nation} are\nrevolting! Citizens\ndemand new government.\n");

						this.parent.StartGameMenu.F5_0000_1af6_LoadGovernmentImage();

						// Instruction address 0x1403:0x35be, size: 5
						this.parent.GameEngine.F0_1238_1b44_RedrawMapWithBlankingEffect();
					}
					break;

				// No command
				case '\0':
					break;

				// Enter or Space
				case '\x0d':
					if (unitID >= 128) goto Label755;
					break;

				// No orders or end of turn
				case ' ':
					if (unitID >= 128) goto Label755;

					if (unit != null)
					{
						unit.RemainingMoves = 0;
					}
					break;

				// Disband
				case 'D':
					if (unit == null || unitID >= 128) break;

					unit.RemainingMoves = 0;

					// Instruction address 0x1403:0x0d90, size: 5
					this.parent.UnitManagement.F0_1866_0f10_DeleteUnit(playerID, unitID);
					break;

				// Pillage
				case 'P':
					if (unit == null || unitID >= 128) break;

					if (!this.parent.MapManagement.F0_2aea_1585_GetVisibleTerrainImprovements(unit.Position.X, unit.Position.Y).HasFlag(TerrainImprovementFlagsEnum.City) &&
						unit.UnitType != UnitTypeEnum.Fighter)
					{
						// Instruction address 0x1403:0x0df1, size: 5
						int cityID = this.parent.MapManagement.FindNearestCity(unit.Position.X, unit.Position.Y);

						if (unit.RemainingMoves != 0)
						{
							if ((short)this.parent.Segment_29f3.F0_29f3_0c9e_ConfirmAttackAction(this.parent.GameData.Cities[cityID].PlayerID) != -1)
							{
								TerrainImprovementFlagsEnum improvements = this.parent.MapManagement.F0_2aea_1585_GetVisibleTerrainImprovements(unit.Position.X, unit.Position.Y);

								if (improvements.HasFlag(TerrainImprovementFlagsEnum.Mines) || improvements.HasFlag(TerrainImprovementFlagsEnum.Irrigation))
								{
									this.parent.MapManagement.F0_2aea_16ee_ClearTerrainImprovements(unit.Position.X, unit.Position.Y, TerrainImprovementFlagsEnum.Mines | TerrainImprovementFlagsEnum.Irrigation);
								}
								else if (improvements.HasFlag(TerrainImprovementFlagsEnum.RailRoad))
								{
									this.parent.MapManagement.F0_2aea_16ee_ClearTerrainImprovements(unit.Position.X, unit.Position.Y, TerrainImprovementFlagsEnum.RailRoad | TerrainImprovementFlagsEnum.Irrigation | TerrainImprovementFlagsEnum.Mines);
								}
								else
								{
									this.parent.MapManagement.F0_2aea_16ee_ClearTerrainImprovements(unit.Position.X, unit.Position.Y, TerrainImprovementFlagsEnum.Road);
								}

								if (this.parent.GameData.Cities[cityID].PlayerID == this.parent.GameData.HumanPlayerID)
								{
									this.parent.MapManagement.F0_2aea_1601_UpdateVisibleCellStatus(unit.Position.X, unit.Position.Y);
								}

								F0_1403_3ed7_DrawVisibleCell(unit.Position.X, unit.Position.Y);
								this.parent.Diplomacy.F0_2517_0aa1_ClearDiplomacyFlags(this.parent.GameData.HumanPlayerID,
									this.parent.GameData.Cities[cityID].PlayerID, DiplomacyFlagsEnum.Peace);

								int local_54 = this.parent.GameData.Cities[cityID].PlayerID;

								if (playerID != local_54)
								{
									this.parent.AIEngine.PlayerAddUnitPolicy(local_54, unit.Position.X, unit.Position.Y, UnitRoleTypeEnum.LandAttack, 4);
								}

								unit.RemainingMoves = 0;
							}
						}
					}
					else
					{
						// Instruction address 0x1403:0x0ddd, size: 5
						this.parent.Help.F4_0000_03aa_ShowInstantWarningPopup("*PILLAGE");
					}
					break;

				// Build a city
				case 'b':
					if (unit == null || unitID >= 128) break;

					if (unit.UnitType != UnitTypeEnum.Settler)
					{
						// Instruction address 0x1403:0x19dd, size: 5
						this.parent.Help.F4_0000_03aa_ShowInstantWarningPopup("*SETTLERS");
					}
					else if (terrainType != TerrainTypeEnum.Water && unit.Position.Y >= 2 && unit.Position.Y < 48)
					{
						if (this.parent.MapManagement.F0_2aea_1585_GetVisibleTerrainImprovements(unit.Position.X, unit.Position.Y).HasFlag(TerrainImprovementFlagsEnum.City))
						{
							int cityID = this.parent.MapManagement.F0_2aea_175a_FindCityHumanPlayer(unit.Position.X, unit.Position.Y);

							if (this.parent.GameData.Cities[cityID].ActualSize < 10)
							{
								GPoint unitPosition = unit.Position;

								this.parent.GameData.Cities[cityID].ActualSize++;

								// Instruction address 0x1403:0x1a6a, size: 5
								this.parent.UnitManagement.F0_1866_0f10_DeleteUnit(playerID, unitID);

								this.parent.MapManagement.F0_2aea_11d4_DrawCellWithUnit(unitPosition.X, unitPosition.Y);

								// Instruction address 0x1403:0x1a78, size: 5
								//F0_1403_3f13_RedrawUnit(playerID, unitID);
							}
							else
							{
								// Instruction address 0x1403:0x1a87, size: 5
								this.parent.Help.F4_0000_03aa_ShowInstantWarningPopup("*ADDCITY");
							}
						}
						else
						{
							unit.RemainingMoves = 0;

							int local_3a = (short)this.parent.Overlay_20.F20_0000_0000(playerID, unit.Position.X, unit.Position.Y, 1);

							if (local_3a != -1)
							{
								// Instruction address 0x1403:0x1ad1, size: 5
								this.parent.UnitManagement.F0_1866_0f10_DeleteUnit(playerID, unitID);

								// Instruction address 0x1403:0x1adf, size: 5
								F0_1403_3f13_DrawVisibleCellWithUnit(playerID, unitID);
							}
						}
					}
					break;

				// Fortify unit or build fortress
				case 'f':
					if (unit == null || unitID >= 128) break;

					if (this.parent.GameData.Units[(int)unit.UnitType].MovementType == UnitMovementTypeEnum.Land)
					{
						if (unit.UnitType != UnitTypeEnum.Settler)
						{
							unit.Status |= UnitStatusEnum.Fortifying;
							unit.RemainingMoves = 0;

							// Instruction address 0x1403:0x12bf, size: 5
							F0_1403_3f13_DrawVisibleCellWithUnit(playerID, unitID);

							if (playerID != this.parent.GameData.HumanPlayerID &&
								this.parent.MapManagement.F0_2aea_1585_GetVisibleTerrainImprovements(unit.Position.X, unit.Position.Y).HasFlag(TerrainImprovementFlagsEnum.City))
							{
								unit.HomeCityID = (short)this.parent.MapManagement.GetCityByLocation(unit.Position.X, unit.Position.Y);
							}
						}
						else
						{
							if (this.parent.MapManagement.F0_2aea_1585_GetVisibleTerrainImprovements(unit.Position.X, unit.Position.Y).HasFlag(TerrainImprovementFlagsEnum.City) ||
								this.parent.MapManagement.F0_2aea_1585_GetVisibleTerrainImprovements(unit.Position.X, unit.Position.Y).HasFlag(TerrainImprovementFlagsEnum.Fortress) ||
								!this.parent.Segment_1ade.F0_1ade_22b5_PlayerHasTechnology(playerID, TechnologyAdvanceEnum.Construction))
							{
								unit.ClearStatusFlags(UnitStatusEnum.SettlerBuildFortress);
								unit.SpecialMoves = 0;

								this.parent.MapManagement.F0_1866_16a9_CenterMap(playerID, unit.Position.X, unit.Position.Y);
							}
							else
							{
								unit.Status |= UnitStatusEnum.SettlerBuildFortress;
								unit.RemainingMoves = 0;

								// Instruction address 0x1403:0x1388, size: 5
								F0_1403_3f13_DrawVisibleCellWithUnit(playerID, unitID);

								unit.SpecialMoves++;

								if ((this.parent.GameData.Terrains[(int)terrainType].MovementCost + 4) <= unit.SpecialMoves)
								{
									// Instruction address 0x1403:0x13bf, size: 5
									this.parent.MapManagement.F0_2aea_1653_SetTerrainImprovements(unit.Position.X, unit.Position.Y, TerrainImprovementFlagsEnum.Fortress);

									// Instruction address 0x1403:0x13db, size: 5
									this.parent.UnitManagement.F0_1866_01dc_UpdateUnitVisibility(unit.Position.X, unit.Position.Y, playerID, unitID, true);

									unit.ClearStatusFlags(UnitStatusEnum.SettlerBuildFortress);
									unit.SpecialMoves = 0;
								}
							}
						}
					}
					break;

				// Go To
				case 'g':
					if (unit == null || unitID >= 128) break;

					int mouseIconIndex = 0;

					if (flag3)
					{
						mouseIconIndex = 7;
					}
					else
					{
						mouseIconIndex = 2;
					}

					this.parent.CommonTools.SetMousePositionAndIcon(0, 0, this.parent.Array_d4ce[mouseIconIndex]);

					flag3 ^= true;
					break;

				// Set as new home city
				case 'h':
					if (unit == null || unitID >= 128) break;

					nearestCityID = this.parent.MapManagement.FindNearestCity(unit.Position);

					if (nearestCityID != -1 && this.parent.MapManagement.GetDistance(unit.Position, this.parent.GameData.Cities[nearestCityID].Position) == 0)
					{
						unit.HomeCityID = (short)nearestCityID;
					}
					else if (this.parent.GameData.Units[(int)unit.UnitType].MovementType == UnitMovementTypeEnum.Water)
					{
						// Instruction address 0x1403:0x1118, size: 5
						int nearestCityDirection = this.parent.UnitManagement.F0_1866_226d_GetNearestCityDirection(playerID, unitID);

						if (nearestCityDirection != 0)
						{
							unit.Status |= UnitStatusEnum.SettlerBuildRoadOrRail;
							MoveUnit(playerID, unit, newMoveDirection, ref unitMoveRetryCount);
							break;
						}
						else
						{
							if (playerID == this.parent.GameData.HumanPlayerID)
							{
								unit.ClearStatusFlags(UnitStatusEnum.SettlerBuildRoadOrRail);
							}
							else
							{
								// Instruction address 0x1403:0x1175, size: 5
								this.parent.UnitManagement.F0_1866_0f10_DeleteUnit(playerID, unitID);
							}
						}
					}
					break;

				// Build irrigation
				case 'i':
					if (unit == null || unitID >= 128) break;

					if (unit.UnitType != UnitTypeEnum.Settler ||
						(this.parent.GameData.DebugFlags & 0x2) == 0 ||
						this.parent.MapManagement.F0_2aea_1585_GetVisibleTerrainImprovements(unit.Position.X, unit.Position.Y).HasFlag(TerrainImprovementFlagsEnum.Irrigation))
					{
						// Instruction address 0x1403:0x15ce, size: 5
						this.parent.MapManagement.F0_1866_16a9_CenterMap(playerID, unit.Position.X, unit.Position.Y);

						if (unit.UnitType != UnitTypeEnum.Settler)
						{
							this.parent.Help.F4_0000_03aa_ShowInstantWarningPopup("*SETTLERS");
						}
						else
						{
							unit.SpecialMoves = 0;
						}

						unit.ClearStatusFlags(UnitStatusEnum.SettlerBuildIrrigation);
					}
					else
					{
						int irrigatedTerrainType = this.parent.GameData.TerrainModifications[(int)terrainType].IrrigationEffect;

						if (irrigatedTerrainType == -1)
						{
							// Instruction address 0x1403:0x165e, size: 5
							this.parent.MapManagement.F0_1866_16a9_CenterMap(playerID, unit.Position.X, unit.Position.Y);
							this.parent.Help.F4_0000_03aa_ShowInstantWarningPopup("*NOIRR");

							unit.ClearStatusFlags(UnitStatusEnum.SettlerBuildIrrigation);
						}
						else
						{
							if (irrigatedTerrainType != -2 || this.parent.MapManagement.CanIrrigateCell(unit.Position.X, unit.Position.Y))
							{
								unit.Status |= UnitStatusEnum.SettlerBuildIrrigation;

								// Instruction address 0x1403:0x16f9, size: 5
								F0_1403_3f13_DrawVisibleCellWithUnit(playerID, unitID);

								unit.RemainingMoves = 0;
								unit.GoToDestination = OpenCivOneGame.InvalidPosition;
								unit.GoToNextDirection = -1;
								unit.SpecialMoves++;

								if (this.parent.GameData.TerrainModifications[(int)terrainType].IrrigationCost <= unit.SpecialMoves)
								{
									if (irrigatedTerrainType >= 0)
									{
										// Instruction address 0x1403:0x1743, size: 5
										this.parent.MapManagement.SetTerrainType(unit.Position.X, unit.Position.Y, this.parent.PixelValuesToTerrainTypes[irrigatedTerrainType]);

										this.parent.GameData.MapVisibility[unit.Position.X, unit.Position.Y] |= 1;

										// Instruction address 0x1403:0x176a, size: 5
										this.parent.MapManagement.F0_2aea_16ee_ClearTerrainImprovements(unit.Position.X, unit.Position.Y, TerrainImprovementFlagsEnum.Irrigation | TerrainImprovementFlagsEnum.Mines);
									}
									else
									{
										// Instruction address 0x1403:0x177f, size: 5
										this.parent.MapManagement.F0_2aea_16ee_ClearTerrainImprovements(unit.Position.X, unit.Position.Y, TerrainImprovementFlagsEnum.Mines);
										// Instruction address 0x1403:0x1791, size: 5
										this.parent.MapManagement.F0_2aea_1653_SetTerrainImprovements(unit.Position.X, unit.Position.Y, TerrainImprovementFlagsEnum.Irrigation);
									}

									// Instruction address 0x1403:0x179f, size: 5
									F0_1403_3f13_DrawVisibleCellWithUnit(playerID, unitID);

									unit.ClearStatusFlags(UnitStatusEnum.SettlerBuildIrrigation);
									unit.SpecialMoves = 0;
								}
							}
							else
							{
								// Instruction address 0x1403:0x16b2, size: 5
								this.parent.MapManagement.F0_1866_16a9_CenterMap(playerID, unit.Position.X, unit.Position.Y);
								// Instruction address 0x1403:0x16be, size: 5
								this.parent.Help.F4_0000_03aa_ShowInstantWarningPopup("*NOWATER");

								unit.ClearStatusFlags(UnitStatusEnum.SettlerBuildIrrigation);
							}
						}
					}
					break;

				// Build forest or mine
				case 'm':
					if (unit == null || unitID >= 128) break;

					if (unit.UnitType != UnitTypeEnum.Settler ||
						this.parent.MapManagement.F0_2aea_1585_GetVisibleTerrainImprovements(unit.Position.X, unit.Position.Y).HasFlag(TerrainImprovementFlagsEnum.Mines))
					{
						// Instruction address 0x1403:0x17fc, size: 5
						this.parent.MapManagement.F0_1866_16a9_CenterMap(playerID, unit.Position.X, unit.Position.Y);

						if (unit.UnitType != UnitTypeEnum.Settler)
						{
							// Instruction address 0x1403:0x1822, size: 5
							this.parent.Help.F4_0000_03aa_ShowInstantWarningPopup("*SETTLERS");
						}
						else
						{
							unit.SpecialMoves = 0;
						}

						unit.ClearStatusFlags(UnitStatusEnum.SettlerBuildMineOrForest);
					}
					else
					{
						int minedTerrainType = this.parent.GameData.TerrainModifications[(int)terrainType].MiningEffect;

						if (minedTerrainType == -1)
						{
							// Instruction address 0x1403:0x188c, size: 5
							this.parent.MapManagement.F0_1866_16a9_CenterMap(playerID, unit.Position.X, unit.Position.Y);
							// Instruction address 0x1403:0x1898, size: 5
							this.parent.Help.F4_0000_03aa_ShowInstantWarningPopup("*NOMINE");

							unit.ClearStatusFlags(UnitStatusEnum.SettlerBuildMineOrForest);
						}
						else
						{
							unit.Status |= UnitStatusEnum.SettlerBuildMineOrForest;

							// Instruction address 0x1403:0x18d3, size: 5
							F0_1403_3f13_DrawVisibleCellWithUnit(playerID, unitID);

							unit.RemainingMoves = 0;
							unit.GoToDestination = OpenCivOneGame.InvalidPosition;
							unit.GoToNextDirection = -1;
							unit.SpecialMoves++;

							if (this.parent.GameData.TerrainModifications[(int)terrainType].MiningCost <= unit.SpecialMoves)
							{
								if (minedTerrainType >= 0)
								{
									unit.SpecialMoves++;

									if (unit.SpecialMoves > 5)
									{
										// Instruction address 0x1403:0x193e, size: 5
										this.parent.MapManagement.SetTerrainType(unit.Position.X, unit.Position.Y, this.parent.PixelValuesToTerrainTypes[minedTerrainType]);

										this.parent.GameData.MapVisibility[unit.Position.X, unit.Position.Y] |= 1;

										// Instruction address 0x1403:0x1965, size: 5
										this.parent.MapManagement.F0_2aea_16ee_ClearTerrainImprovements(unit.Position.X, unit.Position.Y, TerrainImprovementFlagsEnum.Mines | TerrainImprovementFlagsEnum.Irrigation);
										// Instruction address 0x1403:0x199a, size: 5
										F0_1403_3f13_DrawVisibleCellWithUnit(playerID, unitID);

										unit.ClearStatusFlags(UnitStatusEnum.SettlerBuildMineOrForest);
										unit.SpecialMoves = 0;
									}
								}
								else
								{
									// Instruction address 0x1403:0x197a, size: 5
									this.parent.MapManagement.F0_2aea_16ee_ClearTerrainImprovements(unit.Position.X, unit.Position.Y, TerrainImprovementFlagsEnum.Irrigation);
									// Instruction address 0x1403:0x198c, size: 5
									this.parent.MapManagement.F0_2aea_1653_SetTerrainImprovements(unit.Position.X, unit.Position.Y, TerrainImprovementFlagsEnum.Mines);
									// Instruction address 0x1403:0x199a, size: 5
									F0_1403_3f13_DrawVisibleCellWithUnit(playerID, unitID);

									unit.ClearStatusFlags(UnitStatusEnum.SettlerBuildMineOrForest);
									unit.SpecialMoves = 0;
								}
							}
						}
					}
					break;

				// Clear pollution
				case 'p':
					if (unit == null || unitID >= 128) break;

					if (unit.UnitType != UnitTypeEnum.Settler)
					{
						// Instruction address 0x1403:0x119e, size: 5
						this.parent.Help.F4_0000_03aa_ShowInstantWarningPopup("*SETTLERS");
					}
					else
					{
						TerrainImprovementFlagsEnum terrainImprovements = this.parent.MapManagement.F0_2aea_1585_GetVisibleTerrainImprovements(unit.Position.X, unit.Position.Y);

						if (!terrainImprovements.HasFlag(TerrainImprovementFlagsEnum.Pollution))
						{
							unit.ClearStatusFlags(UnitStatusEnum.SettlerCleanPollution);
							unit.SpecialMoves = 0;

							// Instruction address 0x1403:0x11e6, size: 5
							this.parent.MapManagement.F0_1866_16a9_CenterMap(playerID, unit.Position.X, unit.Position.Y);
						}
						else
						{
							unit.Status |= UnitStatusEnum.SettlerCleanPollution;

							// Instruction address 0x1403:0x120c, size: 5
							F0_1403_3f13_DrawVisibleCellWithUnit(playerID, unitID);

							unit.RemainingMoves = 0;
							unit.GoToDestination = OpenCivOneGame.InvalidPosition;
							unit.GoToNextDirection = -1;
							unit.SpecialMoves++;

							if (unit.SpecialMoves >= 4)
							{
								// Instruction address 0x1403:0x123e, size: 5
								this.parent.MapManagement.F0_2aea_16ee_ClearTerrainImprovements(unit.Position.X, unit.Position.Y, TerrainImprovementFlagsEnum.Pollution);
								// Instruction address 0x1403:0x124c, size: 5
								this.parent.MapManagement.F0_2aea_1601_UpdateVisibleCellStatus(unit.Position.X, unit.Position.Y);

								this.parent.GameData.PollutedSquareCount--;

								// Instruction address 0x1403:0x125e, size: 5
								F0_1403_3f13_DrawVisibleCellWithUnit(playerID, unitID);

								unit.ClearStatusFlags(UnitStatusEnum.SettlerCleanPollution);
								unit.SpecialMoves = 0;
							}
						}
					}
					break;

				// Build roads or railroads
				case 'r':
					if (unit == null || unitID >= 128) break;

					if (unit.UnitType != UnitTypeEnum.Settler)
					{
						// Instruction address 0x1403:0x141e, size: 5
						this.parent.Help.F4_0000_03aa_ShowInstantWarningPopup("*SETTLERS");
					}
					else
					{
						TerrainImprovementFlagsEnum terrainImprovements = this.parent.MapManagement.F0_2aea_1585_GetVisibleTerrainImprovements(unit.Position.X, unit.Position.Y);

						if ((terrainImprovements.HasFlag(TerrainImprovementFlagsEnum.Road) &&
							(!this.parent.Segment_1ade.F0_1ade_22b5_PlayerHasTechnology(playerID, TechnologyAdvanceEnum.Railroad) ||
							terrainImprovements.HasFlag(TerrainImprovementFlagsEnum.City))) || terrainImprovements.HasFlag(TerrainImprovementFlagsEnum.RailRoad))
						{
							unit.ClearStatusFlags(UnitStatusEnum.SettlerBuildRoadOrRail);
							unit.SpecialMoves = 0;

							// Instruction address 0x1403:0x148f, size: 5
							this.parent.MapManagement.F0_1866_16a9_CenterMap(playerID, unit.Position.X, unit.Position.Y);
						}
						else
						{
							if (terrainType != TerrainTypeEnum.River || this.parent.Segment_1ade.F0_1ade_22b5_PlayerHasTechnology(playerID, TechnologyAdvanceEnum.BridgeBuilding))
							{
								unit.Status |= UnitStatusEnum.SettlerBuildRoadOrRail;
								unit.RemainingMoves = 0;

								// Instruction address 0x1403:0x14ee, size: 5
								F0_1403_3f13_DrawVisibleCellWithUnit(playerID, unitID);

								int oldSpecialMoves = unit.SpecialMoves;

								unit.SpecialMoves++;

								if ((((terrainImprovements.HasFlag(TerrainImprovementFlagsEnum.Road)) ? 4 : 2) * this.parent.GameData.Terrains[(int)terrainType].MovementCost) <= oldSpecialMoves)
								{
									// Instruction address 0x1403:0x154b, size: 5
									this.parent.MapManagement.F0_2aea_1653_SetTerrainImprovements(
										unit.Position.X, unit.Position.Y, ((terrainImprovements.HasFlag(TerrainImprovementFlagsEnum.Road)) ? TerrainImprovementFlagsEnum.RailRoad : TerrainImprovementFlagsEnum.Road));
									// Instruction address 0x1403:0x1567, size: 5
									this.parent.UnitManagement.F0_1866_01dc_UpdateUnitVisibility(unit.Position.X, unit.Position.Y, playerID, unitID, true);

									unit.ClearStatusFlags(UnitStatusEnum.SettlerBuildRoadOrRail);
									unit.SpecialMoves = 0;
								}
							}
						}
					}
					break;

				// Sentry unit
				case 's':
					if (unit == null || unitID >= 128) break;

					unit.Status |= UnitStatusEnum.Sentry;
					unit.RemainingMoves = 0;

					// Instruction address 0x1403:0x0f64, size: 5
					F0_1403_3f13_DrawVisibleCellWithUnit(playerID, unitID);

					break;

				// Unload unit from carrier
				case 'u':
					if (unit == null || unitID >= 128) break;

					if (this.parent.GameData.Units[(int)unit.UnitType].TransportCapacity != 0 || unit.UnitType == UnitTypeEnum.Carrier)
					{
						if (unit.NextUnitID != -1)
						{
							// Instruction address 0x1403:0x0fc8, size: 5
							this.parent.UnitManagement.F0_1866_14f6_ActivateUnitOnTransport(playerID, unitID);

							flag2 = false;
							flag7 = true;
							unitID = F0_1403_4562_FindNearestUnitIDInUnitStack(playerID, unitID) - 1;
							maxUnitID = unitID;
							if (maxUnitID < 0)
								maxUnitID = 127;

							flag4 = false;
						}
					}
					break;

				// wait for orders
				case 'w':
					if (unit == null || unitID >= 128) break;

					flag7 = true;
					flag4 = true;

					break;

				// Center map on unit
				case 'c':
					if (unit == null || unitID >= 128) break;

					this.parent.MapManagement.F0_2aea_0008_DrawVisibleMap(playerID, unit.Position.X - 7, unit.Position.Y - 6);
					break;

				// Move unit Up
				case '\x001':
				case '\x4800':
					MoveUnit(playerID, unit, 1, ref unitMoveRetryCount);
					break;

				// Move unit PageUp
				case '\x002':
				case '\x4900':
					MoveUnit(playerID, unit, 2, ref unitMoveRetryCount);
					break;

				// Move unit Right
				case '\x003':
				case '\x4d00':
					MoveUnit(playerID, unit, 3, ref unitMoveRetryCount);
					break;

				// Move unit PageDown
				case '\x004':
				case '\x5100':
					MoveUnit(playerID, unit, 4, ref unitMoveRetryCount);
					break;

				// Move unit Down
				case '\x005':
				case '\x5000':
					MoveUnit(playerID, unit, 5, ref unitMoveRetryCount);
					break;

				// Move unit End
				case '\x006':
				case '\x4f00':
					MoveUnit(playerID, unit, 6, ref unitMoveRetryCount);
					break;

				// Move unit Left
				case '\x007':
				case '\x4b00':
					MoveUnit(playerID, unit, 7, ref unitMoveRetryCount);
					break;

				// Move unit Home
				case '\x008':
				case '\x4700':
					MoveUnit(playerID, unit, 8, ref unitMoveRetryCount);
					break;

				// Change tax rate
				case '+':
				case '=':
					StringBuilder taxOptions = new();

					taxOptions.Append("Select new Tax rate:\n ");

					for (int i = 0; this.parent.GameData.Players[playerID].TaxRate + this.parent.GameData.Players[playerID].ScienceTaxRate >= i; i++)
					{
						// Instruction address 0x1403:0x362a, size: 5
						taxOptions.Append($"{i * 10}% Tax, " +
							$"({(this.parent.GameData.Players[playerID].TaxRate + this.parent.GameData.Players[playerID].ScienceTaxRate - i) * 10}% Science)\n ");
					}

					this.parent.Var_2f9a_MenuBoxDefaultOptionIndex = this.parent.GameData.Players[playerID].TaxRate;

					// Instruction address 0x1403:0x368b, size: 5
					int selectedTaxRate = this.parent.MenuBoxDialog.ShowDialog(taxOptions.ToString(), 100, 80);

					if (selectedTaxRate != -1)
					{
						this.parent.GameData.Players[playerID].ScienceTaxRate += (short)(this.parent.GameData.Players[playerID].TaxRate - selectedTaxRate);
						this.parent.GameData.Players[playerID].TaxRate = (short)selectedTaxRate;

						// Instruction address 0x1403:0x36b1, size: 5
						this.parent.GameEngine.F0_1238_107e_ReportHumanTaxAndPopulation();
					}
					break;

				// Change luxury rate
				case '-':
				case '_':
					StringBuilder luxuryOptions = new();

					// Instruction address 0x1403:0x36c1, size: 5
					luxuryOptions.Append("Select new Luxuries rate:\n ");

					for (int i = 0; 10 - this.parent.GameData.Players[playerID].TaxRate >= i; i++)
					{
						luxuryOptions.Append($"{i * 10}% Luxuries, ({(10 - this.parent.GameData.Players[playerID].TaxRate - i) * 10}% Science)\n ");
					}

					this.parent.Var_2f9a_MenuBoxDefaultOptionIndex = (10 - this.parent.GameData.Players[playerID].TaxRate - this.parent.GameData.Players[playerID].ScienceTaxRate);

					int newLuxuryRate = this.parent.MenuBoxDialog.ShowDialog(luxuryOptions.ToString(), 100, 80);

					if (newLuxuryRate != -1)
					{
						this.parent.GameData.Players[playerID].ScienceTaxRate = (short)(10 - this.parent.GameData.Players[playerID].TaxRate - newLuxuryRate);
						// Instruction address 0x1403:0x37a3, size: 5
						this.parent.GameEngine.F0_1238_107e_ReportHumanTaxAndPopulation();
					}
					break;

				// Find city
				case '/':
				case '?':
					this.parent.TextBoxDialogues.F23_0000_025b_FindCityDialog();
					break;

				// Save game
				case 'S':
					// Enable Save game with zero turns
					//if (this.oParent.GameData.TurnCount != 0)
					//{
					// All Top menus are saving the current screenshot, so should we in Save game shortcut
					this.parent.Graphics.F0_VGA_07d8_DrawImage(this.parent.Var_aa_Screen0_Rectangle, 0, 0, 320, 200, this.parent.Var_19d4_Screen1_Rectangle, 0, 0);

					this.parent.LoadAndSave.ShowSaveGameDialog(false);
					//}
					break;

				// Temporarily remove units from a map
				case 't':
					this.parent.Var_dcfc = 1;

					// Instruction address 0x1403:0x37bc, size: 5
					this.parent.MapManagement.F0_2aea_0008_DrawVisibleMap(playerID, this.parent.Var_d4cc_MapViewX, this.parent.Var_d75e_MapViewY);
					// Instruction address 0x1403:0x37c4, size: 5
					this.parent.Segment_2459.F0_2459_0918_WaitForKeyPressOrMouseClick();

					this.parent.Var_dcfc = 0;

					// Instruction address 0x1403:0x37da, size: 5
					this.parent.MapManagement.F0_2aea_0008_DrawVisibleMap(playerID, this.parent.Var_d4cc_MapViewX, this.parent.Var_d75e_MapViewY);
					break;

				// Alt + Q - Quit game
				case 0x1000:
					if (this.parent.MenuBoxDialog.ShowDialog("Are you sure you\nwant to Quit?\n Keep Playing\n Yes, Quit\n", 100, 80) != 1)
					{
						this.parent.Var_dc48_GameEndType = 0;
					}
					else
					{
						if (this.parent.Var_dc48_GameEndType == 0)
						{
							this.parent.Var_dc48_GameEndType = 1;
						}
						unitID = 128;
					}
					break;

				// Alt + W - Show world menu
				case 0x1100:
					// Instruction address 0x1403:0x3a84, size: 5
					this.parent.Menus.F0_2c84_0000_ShowTopMenu(playerID, unitID, 3);

					if (this.parent.Menus.Var_d4ca_MenuShortcutKey != -1)
					{
						command = this.parent.Menus.Var_d4ca_MenuShortcutKey;
						goto Label151;
					}
					break;

				// Alt + R - Randomize leader personalities
				case 0x1300:
					for (int i = 0; i < 16; i++)
					{
						// Instruction address 0x1403:0x3805, size: 5
						this.parent.GameData.Nations[i].Mood = (short)(this.parent.CAPI.RNG.Next(3) - 1); // -1 = Friendly, 0 = Neutral, 1 = Aggressive

						// Instruction address 0x1403:0x3816, size: 5
						this.parent.GameData.Nations[i].Policy = (short)(this.parent.CAPI.RNG.Next(3) - 1); // -1 = Perfectionist, 0 = Neutral, 1 = Expansionist

						// Instruction address 0x1403:0x3827, size: 5
						this.parent.GameData.Nations[i].Ideology = (short)(this.parent.CAPI.RNG.Next(3) - 1); // -1 = Militaristic, 0 = Neutral, 1 = Civilized
					}

					// Instruction address 0x1403:0x3843, size: 5
					MessageBox.Show("Leader traits are randomized", "Game", MessageBoxIcon.Information, MessageBoxButtons.OK);
					break;

				// Alt + O - Show Order menu
				case 0x1800:
					// Instruction address 0x1403:0x3a5a, size: 5
					this.parent.Menus.F0_2c84_0000_ShowTopMenu(playerID, unitID, 1);

					if (this.parent.Menus.Var_d4ca_MenuShortcutKey != -1)
					{
						command = this.parent.Menus.Var_d4ca_MenuShortcutKey;
						goto Label151;
					}
					break;

				// Alt + A - Show Advisors menu
				case 0x1e00:
					// Instruction address 0x1403:0x3a6f, size: 5
					this.parent.Menus.F0_2c84_0000_ShowTopMenu(playerID, unitID, 2);

					if (this.parent.Menus.Var_d4ca_MenuShortcutKey != -1)
					{
						command = this.parent.Menus.Var_d4ca_MenuShortcutKey;
						goto Label151;
					}
					break;

				// Alt + D - Activate / Deactivate Debug mode
				case 0x2000:
					this.parent.GlobalPreferences.Debug.DebugMode ^= true;

					// Instruction address 0x1403:0x3506, size: 5
					this.parent.MapManagement.F0_2aea_0008_DrawVisibleMap(this.parent.GameData.HumanPlayerID, this.parent.Var_d4cc_MapViewX, this.parent.Var_d75e_MapViewY);
					break;

				// Alt + F - Activate / Deactivate Fast mode
				case 0x2001:
					this.parent.GlobalPreferences.Debug.FastMode ^= true;
					break;

				// Alt + G - Show Game menu
				case 0x2200:
					// Instruction address 0x1403:0x3a45, size: 5
					this.parent.Menus.F0_2c84_0000_ShowTopMenu(playerID, unitID, 0);

					if (this.parent.Menus.Var_d4ca_MenuShortcutKey != -1)
					{
						command = this.parent.Menus.Var_d4ca_MenuShortcutKey;
						goto Label151;
					}
					break;

				// Alt + H - Show additional Help
				case 0x2300:
					this.parent.Help.F4_0000_02d3_ShowInstantAdvicePopup("*HELP1");
					this.parent.Help.F4_0000_02d3_ShowInstantAdvicePopup("*HELP2");
					break;

				// Alt + C - Show Encyclopedia menu
				case 0x2e00:
					// Instruction address 0x1403:0x3a99, size: 5
					this.parent.Menus.F0_2c84_0000_ShowTopMenu(playerID, unitID, 4);

					if (this.parent.Menus.Var_d4ca_MenuShortcutKey != -1)
					{
						command = this.parent.Menus.Var_d4ca_MenuShortcutKey;
						goto Label151;
					}
					break;

				// Alt + V - Enable/Disable Sound
				case 0x2f00:
					this.parent.GameData.GameSettingFlags.Sound ^= true;

					// Instruction address 0x1403:0x3890, size: 5
					this.parent.MenuBoxDialog.ShowDialog($"Sounds {((this.parent.GameData.GameSettingFlags.Sound) ? "ON\n" : "OFF\n")}", 100, 80);
					break;

				// F1 - City status
				case 0x3b00:
					if (this.parent.GlobalPreferences.Debug.DebugMode)
					{
						this.parent.ShowDebugDetails.ShowMapWithStrategy();
					}
					else
					{
						this.parent.Overlay_14.F14_0000_186f_CityStatus(this.parent.GameData.HumanPlayerID);
					}
					break;

				// F2 - Military Advisor
				case 0x3c00:
					if (this.parent.GlobalPreferences.Debug.DebugMode)
					{
						this.parent.ShowDebugDetails.ShowUnitStatistics();
					}
					else
					{
						this.parent.Overlay_14.F14_0000_03ad_MilitaryReport(this.parent.GameData.HumanPlayerID);
					}
					break;

				// F3 - Intelligence Advisor
				case 0x3d00:
					if (this.parent.GlobalPreferences.Debug.DebugMode)
					{
						// Instruction address 0x1403:0x33ae, size: 5
						this.parent.Graphics.F0_VGA_06b7_DrawScreenToScreen0WithEffect(2);

						// Instruction address 0x1403:0x33b6, size: 5
						this.parent.CAPI.getch();

						// Instruction address 0x1403:0x33bf, size: 5
						this.parent.Graphics.F0_VGA_06b7_DrawScreenToScreen0WithEffect(0);
					}
					else
					{
						this.parent.Overlay_14.F14_0000_0d43_IntelligenceReport();
					}
					break;

				// F4 - Attitude Advisor
				case 0x3e00:
					this.parent.Overlay_14.F14_0000_15f4_AttitudeReport(this.parent.GameData.HumanPlayerID);
					break;

				// F5 - Trade Advisor
				case 0x3f00:
					this.parent.Overlay_14.F14_0000_07f1_TradeReport(this.parent.GameData.HumanPlayerID);
					break;

				// F6 - Science Advisor
				case 0x4000:
					this.parent.Overlay_14.F14_0000_014b_ScienceReport(this.parent.GameData.HumanPlayerID);
					break;

				// F7 - Wonders of the World
				case 0x4100:
					if (this.parent.GlobalPreferences.Debug.DebugMode)
					{
						for (int i = 1; i < 8; i++)
						{
							this.parent.ShowDebugDetails.ShowPlayerDetails(i);
						}
					}
					else
					{
						this.parent.Reports.F12_0000_080d_ShowWondersOfTheWorldReport();
					}
					break;

				// F8 - Top 5 Cities
				case 0x4200:
					if (this.parent.GlobalPreferences.Debug.DebugMode)
					{
						this.parent.Reports.F12_0000_0573_ShowPowerGraphReport();
						this.parent.Replay.F9_0000_0000();
					}
					else
					{
						this.parent.Reports.ShowTopFiveCitiesReport();
					}
					break;

				// F9 - Civilization Score
				case 0x4300:
					if (this.parent.GlobalPreferences.Debug.DebugMode)
					{
						this.parent.Reports.F12_0000_03ac_ShowContinentStatisticReport();
					}
					else
					{
						this.parent.Overlay_20.F20_0000_0ca9_ShowCivilizationScorePopup(this.parent.GameData.HumanPlayerID, true);
					}
					break;

				// F10 - World Map
				case 0x4400:
					this.parent.Reports.F12_0000_0000_ShowWorldMapReport();
					break;

				// Shift + Home - Move Map
				case 0x4737:
					direction = this.parent.MoveDirections[8];
					this.parent.Var_d4cc_MapViewX += direction.X * 4;
					this.parent.Var_d75e_MapViewY += direction.Y * 4;

					// Instruction address 0x1403:0x3506, size: 5
					this.parent.MapManagement.F0_2aea_0008_DrawVisibleMap(this.parent.GameData.HumanPlayerID, this.parent.Var_d4cc_MapViewX, this.parent.Var_d75e_MapViewY);
					break;

				// Shift + Up - Move Map
				case 0x4838:
					direction = this.parent.MoveDirections[1];
					this.parent.Var_d4cc_MapViewX += direction.X * 4;
					this.parent.Var_d75e_MapViewY += direction.Y * 4;

					// Instruction address 0x1403:0x3506, size: 5
					this.parent.MapManagement.F0_2aea_0008_DrawVisibleMap(this.parent.GameData.HumanPlayerID, this.parent.Var_d4cc_MapViewX, this.parent.Var_d75e_MapViewY);
					break;

				// Shift + PageUp - Move Map
				case 0x4939:
					direction = this.parent.MoveDirections[2];
					this.parent.Var_d4cc_MapViewX += direction.X * 4;
					this.parent.Var_d75e_MapViewY += direction.Y * 4;

					// Instruction address 0x1403:0x3506, size: 5
					this.parent.MapManagement.F0_2aea_0008_DrawVisibleMap(this.parent.GameData.HumanPlayerID, this.parent.Var_d4cc_MapViewX, this.parent.Var_d75e_MapViewY);
					break;

				// Shift + Left - Move Map
				case 0x4b34:
					direction = this.parent.MoveDirections[7];
					this.parent.Var_d4cc_MapViewX += direction.X * 4;
					this.parent.Var_d75e_MapViewY += direction.Y * 4;

					// Instruction address 0x1403:0x3506, size: 5
					this.parent.MapManagement.F0_2aea_0008_DrawVisibleMap(this.parent.GameData.HumanPlayerID, this.parent.Var_d4cc_MapViewX, this.parent.Var_d75e_MapViewY);
					break;

				// Shift + Right - Move Map
				case 0x4d36:
					direction = this.parent.MoveDirections[3];
					this.parent.Var_d4cc_MapViewX += direction.X * 4;
					this.parent.Var_d75e_MapViewY += direction.Y * 4;

					// Instruction address 0x1403:0x3506, size: 5
					this.parent.MapManagement.F0_2aea_0008_DrawVisibleMap(this.parent.GameData.HumanPlayerID, this.parent.Var_d4cc_MapViewX, this.parent.Var_d75e_MapViewY);
					break;

				// Shift + End - Move Map
				case 0x4f31:
					direction = this.parent.MoveDirections[6];
					this.parent.Var_d4cc_MapViewX += direction.X * 4;
					this.parent.Var_d75e_MapViewY += direction.Y * 4;

					// Instruction address 0x1403:0x3506, size: 5
					this.parent.MapManagement.F0_2aea_0008_DrawVisibleMap(this.parent.GameData.HumanPlayerID, this.parent.Var_d4cc_MapViewX, this.parent.Var_d75e_MapViewY);
					break;

				// Shift + Down - Move Map
				case 0x5032:
					direction = this.parent.MoveDirections[5];
					this.parent.Var_d4cc_MapViewX += direction.X * 4;
					this.parent.Var_d75e_MapViewY += direction.Y * 4;

					// Instruction address 0x1403:0x3506, size: 5
					this.parent.MapManagement.F0_2aea_0008_DrawVisibleMap(this.parent.GameData.HumanPlayerID, this.parent.Var_d4cc_MapViewX, this.parent.Var_d75e_MapViewY);
					break;

				// Shift + PageDown - Move Map
				case 0x5133:
					direction = this.parent.MoveDirections[4];
					this.parent.Var_d4cc_MapViewX += direction.X * 4;
					this.parent.Var_d75e_MapViewY += direction.Y * 4;

					// Instruction address 0x1403:0x3506, size: 5
					this.parent.MapManagement.F0_2aea_0008_DrawVisibleMap(this.parent.GameData.HumanPlayerID, this.parent.Var_d4cc_MapViewX, this.parent.Var_d75e_MapViewY);
					break;
			}

		Label722:
			if (unitID < 128 && (unitID == -1 || this.parent.GameData.Players[playerID].Units[unitID].RemainingMoves > 0) &&
				!flag7 && this.parent.Var_dc48_GameEndType == 0) goto Label59;

			if (unitID < 128)
			{
				if (unitID != -1 && this.parent.GameData.Players[playerID].Units[unitID].UnitType != UnitTypeEnum.None)
				{
					if (this.parent.GameData.Players[playerID].Units[unitID].RemainingMoves > 0)
					{
						flag2 = false;
					}
					else
					{
						if (this.parent.GameData.Units[(int)this.parent.GameData.Players[playerID].Units[unitID].UnitType].TurnsOutside != 0)
						{
							if (((int)this.parent.MapManagement.F0_2aea_1585_GetVisibleTerrainImprovements(
								this.parent.GameData.Players[playerID].Units[unitID].Position.X, this.parent.GameData.Players[playerID].Units[unitID].Position.Y) & 0x1) != 0 ||
								this.parent.UnitManagement.F0_1866_1331_CountUnitTypesInStack(playerID, unitID, UnitTypeEnum.Carrier) != 0)
							{
								this.parent.GameData.Players[playerID].Units[unitID].SpecialMoves =
									this.parent.GameData.Units[(int)this.parent.GameData.Players[playerID].Units[unitID].UnitType].TurnsOutside;
							}

							this.parent.GameData.Players[playerID].Units[unitID].SpecialMoves--;

							if (this.parent.GameData.Players[playerID].Units[unitID].SpecialMoves < 0)
							{
								// Instruction address 0x1403:0x3d5d, size: 5
								this.parent.UnitManagement.F0_1866_0f10_DeleteUnit(playerID, unitID);

								// Instruction address 0x1403:0x3d69, size: 5
								this.parent.Help.F4_0000_03aa_ShowInstantWarningPopup("*FUEL");
							}
						}

						if (this.parent.GameData.Players[playerID].Units[unitID].UnitType == UnitTypeEnum.Trireme &&
							playerID == this.parent.GameData.HumanPlayerID && this.parent.CAPI.RNG.Next(2) != 0)
						{
							for (int i = 1; i < 9; i++)
							{
								direction = this.parent.MoveDirections[i];

								GPoint newPosition = this.parent.GameData.Players[playerID].Units[unitID].Position + direction;

								if (this.parent.MapManagement.ValidateMapCoordinates(newPosition) && 
									this.parent.MapManagement.GetTerrainType(newPosition) != TerrainTypeEnum.Water)
								{
									// Instruction address 0x1403:0x3e18, size: 5
									this.parent.UnitManagement.F0_1866_0f10_DeleteUnit(playerID, unitID);
									// Instruction address 0x1403:0x3e24, size: 5
									this.parent.Help.F4_0000_03aa_ShowInstantWarningPopup("*TRIREME");
									break;
								}
							}
						}
					}
				}
			}
			else if ((this.parent.GameData.PlayerFlags & (0x1 << playerID)) != 0)
			{
				// Instruction address 0x1403:0x3e43, size: 5
				F0_1403_4060_DrawStatusPanel(playerID, unitID);
			}

			goto Label18;

		Label748:
			if (!flag2 && this.parent.Var_dc48_GameEndType == 0) goto Label17;

			if (this.parent.Var_dc48_GameEndType == 0 && (this.parent.GameData.PlayerFlags & (0x1 << playerID)) != 0 &&
				(!flag5 || this.parent.GameData.GameSettingFlags.EndOfTurn))
			{
				unitID = 128;
				flag2 = false;
				flag6 = true;
				goto Label54;
			}

		Label755:
			if (playerID == this.parent.GameData.HumanPlayerID)
			{
				// Instruction address 0x1403:0x3ec1, size: 5
				this.parent.DrawTools.F0_1238_1bb2_FillRectangleWithShadow(0, 97, 80, 103);
			}
		}

		/// <summary>
		/// Moves the unit in the desired direction
		/// </summary>
		/// <param name="playerID"></param>
		/// <param name="unit"></param>
		/// <param name="newMoveDirection"></param>
		/// <param name="unitMoveCount"></param>
		private void MoveUnit(int playerID, Unit? unit, int newMoveDirection, ref int unitMoveCount)
		{
			if (unit != null && unit.UnitType != UnitTypeEnum.None)
			{
				GPoint direction = this.parent.MoveDirections[newMoveDirection];

				if (this.parent.MapManagement.ValidateMapCoordinates(unit.Position + direction))
				{
					// Barbarian units (excluding diplomat units)
					if (playerID == 0 && unit.UnitType != UnitTypeEnum.Diplomat)
					{
						TerrainImprovementFlagsEnum terrainImprovements = this.parent.MapManagement.F0_2aea_1585_GetVisibleTerrainImprovements(unit.Position.X, unit.Position.Y);

						if (!terrainImprovements.HasFlag(TerrainImprovementFlagsEnum.City) &&
							(terrainImprovements.HasFlag(TerrainImprovementFlagsEnum.Mines) || terrainImprovements.HasFlag(TerrainImprovementFlagsEnum.Irrigation)) &&
							unit.UnitType != UnitTypeEnum.Legion && unit.UnitType != UnitTypeEnum.Knights &&
							this.parent.GameData.Cities[this.parent.MapManagement.FindNearestCity(unit.Position.X, unit.Position.Y)].PlayerID == this.parent.GameData.HumanPlayerID)
						{
							// Instruction address 0x1403:0x1d35, size: 5
							this.parent.MapManagement.F0_2aea_16ee_ClearTerrainImprovements(unit.Position.X, unit.Position.Y, TerrainImprovementFlagsEnum.Mines | TerrainImprovementFlagsEnum.Irrigation);

							if ((this.parent.GameData.MapVisibility[unit.Position.X, unit.Position.Y] & (0x1 << this.parent.GameData.HumanPlayerID)) != 0)
							{
								// Instruction address 0x1403:0x1d68, size: 5
								this.parent.MapManagement.F0_2aea_1601_UpdateVisibleCellStatus(unit.Position.X, unit.Position.Y);
								// Instruction address 0x1403:0x1d76, size: 5
								this.parent.MapManagement.F0_2aea_11d4_DrawCellWithUnit(unit.Position.X, unit.Position.Y);
							}

							unit.RemainingMoves = 0;
							return;
						}
					}

					// current unit position
					TerrainTypeEnum currentTerrainType = this.parent.MapManagement.GetTerrainType(unit.Position);
					int unitID = unit.ID;

					// New unit position
					int newX = this.parent.MapManagement.AdjustXPosition(unit.Position.X + direction.X);
					int newY = unit.Position.Y + direction.Y;
					TerrainTypeEnum newTerrainType = this.parent.MapManagement.GetTerrainType(newX, newY);
					TerrainImprovementFlagsEnum newImprovements = this.parent.MapManagement.F0_2aea_1585_GetVisibleTerrainImprovements(newX, newY);
					int newActiveUnitID = this.parent.MapManagement.F0_2aea_1458_GetCellActiveUnitID(newX, newY);
					int newActiveUnitPlayerID = this.parent.MapManagement.F0_2aea_14e0_GetCellActiveUnitPlayerID(newX, newY);

					// Land unit movement for Diplomat or Caravan unit
					if (this.parent.GameData.Units[(int)unit.UnitType].MovementType == UnitMovementTypeEnum.Land)
					{
						// Check if there are enemy units nearby
						if (newActiveUnitID == -1 && this.parent.UnitManagement.F0_1866_1725_IsUnitNear(playerID, unit.Position.X, unit.Position.Y) &&
							this.parent.UnitManagement.F0_1866_1725_IsUnitNear(playerID, newX, newY) &&
							(unit.UnitType != UnitTypeEnum.Diplomat && unit.UnitType != UnitTypeEnum.Caravan) && currentTerrainType != TerrainTypeEnum.Water)
						{
							if (playerID == this.parent.GameData.HumanPlayerID)
							{
								// Instruction address 0x1403:0x1e9c, size: 5
								this.parent.CommonTools.PlayTune(37, 0);

								if (this.parent.GameData.GameSettingFlags.InstantAdvice)
								{
									// Instruction address 0x1403:0x1eb2, size: 5
									this.parent.Help.F4_0000_03aa_ShowInstantWarningPopup("*ZOC");
								}
							}

							unit.GoToDestination = OpenCivOneGame.InvalidPosition;

							return;
						}

						// Diplomat or Caravan unit
						if (unit.UnitType == UnitTypeEnum.Diplomat || unit.UnitType == UnitTypeEnum.Caravan)
						{
							int cityID = this.parent.MapManagement.GetCityByLocation(newX, newY);

							if (cityID != -1)
							{
								// Diplomat or Caravan unit interaction with a city

								if (currentTerrainType == TerrainTypeEnum.Water && this.parent.GameData.Cities[cityID].PlayerID != playerID)
									return;

								// Diplomat unit
								if (unit.UnitType == UnitTypeEnum.Diplomat && playerID != this.parent.GameData.Cities[cityID].PlayerID)
								{
									if (this.parent.GameData.HumanPlayerID == this.parent.GameData.Cities[cityID].PlayerID)
									{
										unit.VisibleByPlayer |= (ushort)(1 << this.parent.GameData.HumanPlayerID);

										// Instruction address 0x1403:0x1f7f, size: 5
										this.parent.MapManagement.F0_1866_16a9_CenterMap(this.parent.GameData.HumanPlayerID,
											unit.Position.X, unit.Position.Y);

										if (this.parent.GlobalPreferences.Debug.FastMode)
										{
											this.parent.CommonTools.WaitTimer(7);
										}
										else
										{
											this.parent.CommonTools.WaitTimer(30);
										}

										// Instruction address 0x1403:0x1f9c, size: 5
										this.parent.UnitManagement.F0_1866_1d55_AnimateUnitMove(playerID, unitID, newMoveDirection);
									}

									this.parent.DiplomatActions.F22_0000_0000_DiplomatActionOnCity(playerID, unitID, cityID);

									return;
								}

								// Caravan unit
								if (unit.UnitType == UnitTypeEnum.Caravan)
								{
									int distance = this.parent.MapManagement.GetDistance(newX, newY, this.parent.GameData.Cities[unit.HomeCityID].Position);

									if (this.parent.GameData.Cities[cityID].PlayerID != playerID ||
										this.parent.GameData.Cities[cityID].CurrentProductionID < -24 || distance >= 10 ||
										this.parent.MapManagement.F0_2aea_1942_GetGroupID(newX, newY) != this.parent.MapManagement.F0_2aea_1942_GetGroupID(
											this.parent.GameData.Cities[unit.HomeCityID].Position.X, this.parent.GameData.Cities[unit.HomeCityID].Position.Y))
									{
										int selectedCaravanOption = 1;

										if (this.parent.GameData.Cities[cityID].PlayerID == playerID)
										{
											StringBuilder caravanOptions = new();
											caravanOptions.Append("Will you?\n Keep moving\n Establish trade route\n");

											if (this.parent.GameData.Cities[cityID].PlayerID == playerID && this.parent.GameData.Cities[cityID].CurrentProductionID < -24)
											{
												// Instruction address 0x1403:0x209d, size: 5
												caravanOptions.Append(" Help build WONDER.\n");

												if (distance < 10)
												{
													this.parent.Var_b276_MenuBoxDisabledOptions = 2;
												}
											}

											selectedCaravanOption = this.parent.MenuBoxDialog.ShowDialog(caravanOptions.ToString(), 100, 80);
										}

										if (selectedCaravanOption == 1)
										{
											this.parent.Segment_2459.F0_2459_0948_CaravanArrivesAtDestinationCity(playerID, unitID, cityID);

											return;
										}

										if (selectedCaravanOption == 2)
										{
											this.parent.GameData.Cities[cityID].ShieldsCount += (short)(10 * this.parent.GameData.Units[(int)unit.UnitType].Cost);

											// Instruction address 0x1403:0x2149, size: 5
											this.parent.UnitManagement.F0_1866_0f10_DeleteUnit(playerID, unitID);

											return;
										}
									}
								}
							}
							else if (unit.UnitType == UnitTypeEnum.Diplomat && newActiveUnitID != -1 && playerID != newActiveUnitPlayerID)
							{
								// Diplomat interaction with the unit
								this.parent.DiplomatActions.F22_0000_0639_DiplomatInteractionWithUnit(playerID, unitID, newActiveUnitPlayerID, newActiveUnitID);

								unit.GoToDestination = OpenCivOneGame.InvalidPosition;
							}
						}
					}

					// Move land unit onto transport
					if (unit.UnitType != UnitTypeEnum.None && this.parent.GameData.Units[(int)unit.UnitType].MovementType == UnitMovementTypeEnum.Land &&
						newTerrainType == TerrainTypeEnum.Water)
					{
						if (playerID != newActiveUnitPlayerID || newActiveUnitID == -1 || 
							this.parent.UnitManagement.F0_1866_13d5_GetWaterTransportCapabilityCount(playerID, newActiveUnitID) <= 0)
							return;

						unit.Status |= UnitStatusEnum.Sentry;
						unit.ClearStatusFlags(UnitStatusEnum.Fortifying | UnitStatusEnum.Fortified);
						// !!! Why newly loaded unit has any remaining moves?
						unit.RemainingMoves = 3;
					}
					else if (newActiveUnitID != -1 && playerID != newActiveUnitPlayerID)
					{
						if (currentTerrainType == TerrainTypeEnum.Water && this.parent.GameData.Units[(int)unit.UnitType].MovementType == UnitMovementTypeEnum.Land)
						{
							if (playerID == this.parent.GameData.HumanPlayerID)
							{
								// Instruction address 0x1403:0x224c, size: 5
								this.parent.Help.F4_0000_03aa_ShowInstantWarningPopup("*AMPHIB");
							}

							unit.GoToDestination = OpenCivOneGame.InvalidPosition;

							return;
						}

						// Cases where an invalid turn is made
						if (unit.UnitType == UnitTypeEnum.Diplomat ||
							(newTerrainType != TerrainTypeEnum.Water && unit.UnitType == UnitTypeEnum.Submarine) ||
							(playerID != this.parent.GameData.HumanPlayerID && this.parent.GameData.Units[(int)unit.UnitType].AttackStrength == 0))
						{
							return;
						}

						// Case where we want to attack a air unit
						if (unit.UnitType != UnitTypeEnum.Fighter &&
							this.parent.GameData.Units[(int)this.parent.GameData.Players[newActiveUnitPlayerID].Units[newActiveUnitID].UnitType].MovementType == UnitMovementTypeEnum.Air)
						{
							if (!newImprovements.HasFlag(TerrainImprovementFlagsEnum.City))
							{
								// Only FIGHTER units may attack AIR units
								this.parent.Help.F4_0000_03aa_ShowInstantWarningPopup("*FIGHTER");

								unit.GoToDestination = OpenCivOneGame.InvalidPosition;

								return;
							}
						}

						if (unit.RemainingMoves < 3)
						{
							if (playerID == this.parent.GameData.HumanPlayerID)
							{
								if (this.parent.MenuBoxDialog.ShowDialog($"Attack at\n{unit.RemainingMoves}/3 strength?\n Cancel\n Attack\n", 100, 16) != 1)
								{
									unit.GoToDestination = OpenCivOneGame.InvalidPosition;

									return;
								}
							}
							else
							{
								// AI player will cancel the attack if strength is diminished
								if (unit.RemainingMoves < 2) return;
							}
						}

						if (newActiveUnitPlayerID == this.parent.GameData.HumanPlayerID)
						{
							// Instruction address 0x1403:0x23eb, size: 5
							this.parent.MapManagement.F0_1866_16a9_CenterMap(this.parent.GameData.HumanPlayerID, newX, newY);
						}

						newActiveUnitID = this.parent.UnitManagement.F0_1866_1122_GetBestDefenseUnitInStack(newActiveUnitPlayerID, newActiveUnitID);

						if (playerID == this.parent.GameData.HumanPlayerID || newActiveUnitPlayerID == this.parent.GameData.HumanPlayerID)
						{
							this.parent.Var_70d8 = true;
						}

						int attackResult = this.parent.Segment_29f3.F0_29f3_000e_AttackUnit(playerID, unitID, newActiveUnitPlayerID, newActiveUnitID, true);
						unit.GoToDestination = OpenCivOneGame.InvalidPosition;
						this.parent.Var_70d8 = false;

						if (attackResult == -1)
							return;

						if (unit.UnitType == UnitTypeEnum.None)
						{
							unit.RemainingMoves = 0;
							return;
						}

						unit.RemainingMoves -= 3;

						// A Bomber's turn ends after it attacks
						if (unit.RemainingMoves < 0 || unit.UnitType == UnitTypeEnum.Bomber)
						{
							unit.RemainingMoves = 0;
						}

						if (!newImprovements.HasFlag(TerrainImprovementFlagsEnum.City))
							return;

						int cityID = this.parent.MapManagement.GetCityByLocation(newX, newY);

						if (this.parent.GameData.Cities[cityID].PlayerID != this.parent.GameData.HumanPlayerID)
						{
							this.parent.GameData.Cities[cityID].Status |= CityStatusEnum.AutoProduction;
						}

						if (unit.UnitType != UnitTypeEnum.None)
						{
							if (!this.parent.GameData.Cities[cityID].HasImprovement(ImprovementEnum.CityWalls))
							{
								if (currentTerrainType != TerrainTypeEnum.Water)
								{
									if (this.parent.GameData.DifficultyLevel != 0 || newActiveUnitPlayerID != this.parent.GameData.HumanPlayerID)
									{
										this.parent.GameData.Cities[cityID].ActualSize--;
									}
								}
							}
						}

						if (this.parent.GameData.Cities[cityID].ActualSize == 0)
						{
							// Instruction address 0x1403:0x2576, size: 5
							this.parent.Segment_1ade.F0_1ade_018e(cityID, newX, newY);
							this.parent.StartGameMenu.F5_0000_0e6c_TestIfAIPlayerIsDestroyed(newActiveUnitPlayerID, playerID);

							if (playerID == this.parent.GameData.HumanPlayerID)
							{
								newActiveUnitID = this.parent.MapManagement.F0_2aea_1458_GetCellActiveUnitID(newX, newY);
								newActiveUnitPlayerID = this.parent.MapManagement.F0_2aea_14e0_GetCellActiveUnitPlayerID(newX, newY);

								if (newActiveUnitID != -1)
								{
									this.parent.GameData.Players[newActiveUnitPlayerID].Units[newActiveUnitID].VisibleByPlayer |=
										(ushort)(1 << this.parent.GameData.HumanPlayerID);
								}
							}
						}

						if (playerID == this.parent.GameData.HumanPlayerID ||
							newActiveUnitPlayerID == this.parent.GameData.HumanPlayerID ||
							this.parent.GlobalPreferences.Debug.DebugMode)
						{
							this.parent.GameData.Cities[cityID].VisibleSize = this.parent.GameData.Cities[cityID].ActualSize;

							// Instruction address 0x1403:0x2606, size: 5
							this.parent.MapManagement.F0_2aea_11d4_DrawCellWithUnit(newX, newY);
						}

						if (this.parent.MapManagement.F0_2aea_14e0_GetCellActiveUnitPlayerID(newX, newY) == -1)
						{
							if (newActiveUnitPlayerID != this.parent.GameData.HumanPlayerID)
							{
								// Instruction address 0x1403:0x2641, size: 5
								this.parent.AIEngine.F0_25fb_3459_PlayerChangeCityProductionForSameContinent(newActiveUnitPlayerID, this.parent.MapManagement.F0_2aea_1942_GetGroupID(newX, newY));
							}
						}
						return;
					}

					// Move water unit only if destination is water or a city
					if (unit.UnitType != UnitTypeEnum.None && this.parent.GameData.Units[(int)unit.UnitType].MovementType == UnitMovementTypeEnum.Water &&
						newTerrainType != TerrainTypeEnum.Water && !newImprovements.HasFlag(TerrainImprovementFlagsEnum.City))
					{
						unit.GoToDestination = OpenCivOneGame.InvalidPosition;
						return;
					}

					if (unit.UnitType != UnitTypeEnum.None && this.parent.GameData.Units[(int)unit.UnitType].MovementType != UnitMovementTypeEnum.Land &&
						newImprovements.HasFlag(TerrainImprovementFlagsEnum.City) && this.parent.MapManagement.F0_2aea_1369_GetCityOwner(newX, newY) != playerID)
					{
						if (unit.UnitType != UnitTypeEnum.Nuclear)
						{
							// Instruction address 0x1403:0x2706, size: 5
							this.parent.Help.F4_0000_03aa_ShowInstantWarningPopup("*OCCUPY");

							unit.GoToDestination = OpenCivOneGame.InvalidPosition;
						}
						else
						{
							// Instruction address 0x1403:0x271f, size: 5
							this.parent.UnitManagement.F0_1866_0f10_DeleteUnit(playerID, unitID);
							// Instruction address 0x1403:0x2730, size: 5
							this.parent.Segment_29f3.F0_29f3_0d4d_NuclearAttack(playerID, newX, newY);
						}
						return;
					}

					if (unit.RemainingMoves == 0)
					{
						return;
					}

					if (this.parent.GameData.Units[(int)unit.UnitType].MovementType == UnitMovementTypeEnum.Land)
					{
						if ((this.parent.MapManagement.F0_2aea_1585_GetVisibleTerrainImprovements(unit.Position.X, unit.Position.Y) & newImprovements).HasFlag(TerrainImprovementFlagsEnum.Road) &&
							unit.RemainingMoves != 0)
						{
							if (!this.parent.MapManagement.F0_2aea_1585_GetVisibleTerrainImprovements(unit.Position.X, unit.Position.Y).HasFlag(TerrainImprovementFlagsEnum.RailRoad) ||
								playerID != this.parent.GameData.HumanPlayerID || unit.GoToDestination.X != -1)
							{
								unit.RemainingMoves -= 1;
							}
						}
						else
						{
							if (unit.RemainingMoves != 3 &&
								this.parent.CAPI.RNG.Next(this.parent.GameData.Terrains[(int)newTerrainType].MovementCost * 3) > unit.RemainingMoves)
							{
								unit.RemainingMoves = 0;
								return;
							}

							unit.RemainingMoves -= (short)(this.parent.GameData.Terrains[(int)newTerrainType].MovementCost * 3);
						}
					}
					else
					{
						unit.RemainingMoves -= 3;
					}

					if (unit.RemainingMoves < 0)
					{
						unit.RemainingMoves = 0;
					}

					if (playerID != this.parent.GameData.HumanPlayerID)
					{
						if (unit.GoToNextDirection != -1)
						{
							if ((unit.GoToNextDirection ^ 0x4) == newMoveDirection)
							{
								if (this.parent.UnitManagement.F0_1866_1251_GetStackUnitValueSum(playerID, unitID, UnitStackValueTypeEnum.UnitCount) <= 2)
								{
									unit.GoToDestination = OpenCivOneGame.InvalidPosition;
									unit.GoToNextDirection = -1;
									unit.RemainingMoves = 0;

									return;
								}
							}
						}
						unit.GoToNextDirection = (short)newMoveDirection;
					}

					if (newActiveUnitID != -1 && playerID == newActiveUnitPlayerID &&
						playerID != this.parent.GameData.HumanPlayerID && !newImprovements.HasFlag(TerrainImprovementFlagsEnum.City))
					{
						if (this.parent.GameData.Units[(int)unit.UnitType].MovementType == UnitMovementTypeEnum.Land ||
							(this.parent.GameData.Units[(int)unit.UnitType].MovementType == UnitMovementTypeEnum.Water &&
								unit.SpecialMoves != 0))
						{
							if (((unit.RemainingMoves != 0) ? 4 : 2) <= this.parent.UnitManagement.F0_1866_1251_GetStackUnitValueSum(playerID, newActiveUnitID, UnitStackValueTypeEnum.UnitCount))
								return;
						}
					}

					// Get playerID that owns this land
					int ownerPlayerID = this.parent.MapManagement.GetPlayerLandOwnership(newX, newY);

					if (ownerPlayerID != 0)
					{
						if (ownerPlayerID < 8)
						{
							if (newTerrainType != TerrainTypeEnum.Water && currentTerrainType != TerrainTypeEnum.Water &&
								(unit.UnitType != UnitTypeEnum.Diplomat && unit.UnitType != UnitTypeEnum.Caravan) &&
								(newImprovements.HasFlag(TerrainImprovementFlagsEnum.City) || newImprovements.HasFlag(TerrainImprovementFlagsEnum.Irrigation) ||
								newImprovements.HasFlag(TerrainImprovementFlagsEnum.Mines) || newImprovements.HasFlag(TerrainImprovementFlagsEnum.Road)) && ownerPlayerID != playerID)
							{
								if ((playerID == this.parent.GameData.HumanPlayerID || ownerPlayerID == this.parent.GameData.HumanPlayerID) &&
									this.parent.GameData.Units[(int)unit.UnitType].MovementType == UnitMovementTypeEnum.Land &&
									this.parent.MapManagement.F0_2aea_14e0_GetCellActiveUnitPlayerID(newX, newY) != playerID)
								{
									if (playerID == this.parent.GameData.HumanPlayerID)
									{
										unit.GoToDestination = OpenCivOneGame.InvalidPosition;

										if ((short)this.parent.Segment_29f3.F0_29f3_0c9e_ConfirmAttackAction(ownerPlayerID) != -1)
										{
											// Instruction address 0x1403:0x2a87, size: 5
											this.parent.Diplomacy.F0_2517_0aa1_ClearDiplomacyFlags(playerID, ownerPlayerID, DiplomacyFlagsEnum.Peace);
										}
										else
										{
											return;
										}
									}
									else if (ownerPlayerID == this.parent.GameData.HumanPlayerID &&
										this.parent.GameData.Players[ownerPlayerID].Diplomacy[playerID].HasFlag(DiplomacyFlagsEnum.Peace))
									{
										unit.RemainingMoves = 0;

										return;
									}
								}
							}

							if (playerID != ownerPlayerID)
							{
								if (((int)this.parent.MapManagement.F0_2aea_1585_GetVisibleTerrainImprovements(newX, newY) & 0xe) != 0)
								{
									unit.VisibleByPlayer |= (ushort)(1 << ownerPlayerID);
								}
							}
						}
					}

					int cityPlayerID1 = this.parent.MapManagement.F0_2aea_1369_GetCityOwner(newX, newY);

					if (playerID == 0 && newTerrainType != TerrainTypeEnum.Water &&
						(this.parent.GameData.MapVisibility[unit.Position.X, unit.Position.Y] & (0x1 << this.parent.GameData.HumanPlayerID)) != 0 &&
						(unit.VisibleByPlayer & (0x1 << this.parent.GameData.HumanPlayerID)) == 0 &&
						this.parent.GameData.Cities[this.parent.MapManagement.FindNearestCity(unit.Position.X, unit.Position.Y)].PlayerID == this.parent.GameData.HumanPlayerID)
					{
						unit.VisibleByPlayer |= this.parent.GameData.MapVisibility[unit.Position.X, unit.Position.Y];
					}

					if (newImprovements.HasFlag(TerrainImprovementFlagsEnum.City) && cityPlayerID1 != playerID &&
						(this.parent.GameData.MapVisibility[newX, newY] & (0x1 << this.parent.GameData.HumanPlayerID)) != 0 &&
						(this.parent.GameData.Players[this.parent.GameData.HumanPlayerID].Diplomacy[playerID].HasFlag(DiplomacyFlagsEnum.EmbassyEstablished) ||
							this.parent.GameData.Players[this.parent.GameData.HumanPlayerID].Diplomacy[cityPlayerID1].HasFlag(DiplomacyFlagsEnum.EmbassyEstablished)))
					{
						unit.VisibleByPlayer |= (ushort)(1 << this.parent.GameData.HumanPlayerID);
					}

					if (newTerrainType == TerrainTypeEnum.Water)
					{
						if (((int)this.parent.MapManagement.F0_2aea_1585_GetVisibleTerrainImprovements(newX, newY) & 0x2) != 0)
						{
							unit.VisibleByPlayer |= (ushort)(1 << cityPlayerID1);
						}
					}

					if ((unit.VisibleByPlayer & (0x1 << this.parent.GameData.HumanPlayerID)) != 0 ||
						playerID == this.parent.GameData.HumanPlayerID || (this.parent.GlobalPreferences.Debug.DebugMode && playerID != 0))
					{
						if (!this.parent.GameData.GameSettingFlags.EnemyMoves && playerID != this.parent.GameData.HumanPlayerID)
						{
							// Instruction address 0x1403:0x2cd5, size: 5
							this.parent.MapManagement.F0_2aea_03ba_DrawCell(unit.Position.X, unit.Position.Y);
						}
						else
						{
							// Instruction address 0x1403:0x2cb3, size: 5
							this.parent.MapManagement.F0_1866_16a9_CenterMap(this.parent.GameData.HumanPlayerID, newX, newY);
							// Instruction address 0x1403:0x2cc4, size: 5
							this.parent.UnitManagement.F0_1866_1d55_AnimateUnitMove(playerID, unitID, newMoveDirection);
						}
					}

					// Remember the old stack and position, so the transported units can follow the ship
					int oldX = unit.Position.X;
					int oldY = unit.Position.Y;
					int oldNextUnitID = unit.NextUnitID;

					// Instruction address 0x1403:0x2d01, size: 5
					this.parent.MapManagement.F0_2aea_1412_SetCellActivePlayerID(oldX, oldY, playerID, unitID);

					// Instruction address 0x1403:0x2d21, size: 5
					this.parent.MapManagement.F0_2aea_13cb_SetCellPlayerID(newX, newY, playerID, unitID);

					unit.Position = new(newX, newY);
					unit.VisibleByPlayer = 0;

					if (playerID != this.parent.GameData.HumanPlayerID &&
						this.parent.GameData.Units[(int)unit.UnitType].MovementType != UnitMovementTypeEnum.Water)
					{
						unit.ClearStatusFlags(UnitStatusEnum.Sentry | UnitStatusEnum.SettlerBuildRoadOrRail | UnitStatusEnum.Fortifying |
							 UnitStatusEnum.Fortified | UnitStatusEnum.SettlerBuildIrrigation | UnitStatusEnum.SettlerBuildMineOrForest);
					}

					if (ownerPlayerID != 0 && ownerPlayerID < 8 && ownerPlayerID != playerID)
					{
						// Instruction address 0x1403:0x2d9d, size: 5
						this.parent.MapManagement.SetPlayerLandOwnership(newX, newY, 0);
					}

					if (unit.GoToDestination.X == newX && unit.GoToDestination.Y == newY)
					{
						unit.GoToDestination = OpenCivOneGame.InvalidPosition;
						unit.GoToNextDirection = -1;

						if (playerID != this.parent.GameData.HumanPlayerID)
						{
							unit.RemainingMoves = 0;
						}
					}

					if (currentTerrainType == TerrainTypeEnum.Water && newTerrainType != TerrainTypeEnum.Water &&
						this.parent.GameData.Units[(int)unit.UnitType].MovementType != UnitMovementTypeEnum.Air)
					{
						unit.RemainingMoves = 0;
					}

					if ((this.parent.GameData.Units[(int)unit.UnitType].TransportCapacity != 0 || unit.UnitType == UnitTypeEnum.Carrier) && oldNextUnitID != -1)
					{
						int nextUnitID = oldNextUnitID;
						int currentCapacity = 0;

						for (int i = 0; i < 20; i++)
						{
							int nextUnitID1 = this.parent.GameData.Players[playerID].Units[nextUnitID].NextUnitID;

							if (((this.parent.GameData.Units[(int)unit.UnitType].TransportCapacity < 1) ? UnitMovementTypeEnum.Air : UnitMovementTypeEnum.Land) ==
								this.parent.GameData.Units[(int)this.parent.GameData.Players[playerID].Units[nextUnitID].UnitType].MovementType)
							{
								if ((this.parent.GameData.Players[playerID].Units[nextUnitID].Status & UnitStatusEnum.Sentry) != UnitStatusEnum.None ||
									currentTerrainType == TerrainTypeEnum.Water ||
									(playerID != this.parent.GameData.HumanPlayerID &&
										(this.parent.GameData.Players[playerID].Units[nextUnitID].Status & (UnitStatusEnum.Fortifying | UnitStatusEnum.Fortified)) == UnitStatusEnum.None &&
									this.parent.GameData.Players[playerID].Units[nextUnitID].NextUnitID != -1))
								{
									// Instruction address 0x1403:0x2efb, size: 5
									this.parent.MapManagement.F0_2aea_1412_SetCellActivePlayerID(oldX, oldY, playerID, nextUnitID);
									// Instruction address 0x1403:0x2f0f, size: 5
									this.parent.MapManagement.F0_2aea_13cb_SetCellPlayerID(newX, newY, playerID, nextUnitID);

									this.parent.GameData.Players[playerID].Units[nextUnitID].Position = new(newX, newY);
									this.parent.GameData.Players[playerID].Units[nextUnitID].VisibleByPlayer = 0;
									this.parent.GameData.Players[playerID].Units[nextUnitID].GoToNextDirection = -1;

									if (this.parent.GameData.Units[(int)unit.UnitType].TransportCapacity != 0)
									{
										this.parent.GameData.Players[playerID].Units[nextUnitID].Status |= UnitStatusEnum.Sentry;
									}

									if (this.parent.GameData.Units[(int)this.parent.GameData.Players[playerID].Units[nextUnitID].UnitType].TurnsOutside != 0)
									{
										this.parent.GameData.Players[playerID].Units[nextUnitID].SpecialMoves =
											(short)(this.parent.GameData.Units[(int)this.parent.GameData.Players[playerID].Units[nextUnitID].UnitType].TurnsOutside - 1);
									}

									currentCapacity++;
								}
							}

							nextUnitID = nextUnitID1;

							if (nextUnitID == -1 || (this.parent.GameData.Units[(int)unit.UnitType].TransportCapacity <= currentCapacity &&
								unit.UnitType != UnitTypeEnum.Carrier && playerID != 0))
								break;
						}
					}

					if (newImprovements.HasFlag(TerrainImprovementFlagsEnum.City) && cityPlayerID1 != playerID)
					{
						unit.VisibleByPlayer |= (ushort)(1 << cityPlayerID1);

						// Instruction address 0x1403:0x303e, size: 5
						this.parent.Segment_2459.F0_2459_0000_CityTakeover(playerID, this.parent.MapManagement.GetCityByLocation(newX, newY), false);
						// Instruction address 0x1403:0x304c, size: 5
						this.parent.MapManagement.F0_2aea_1511_ActiveUnitSetFlag8(newX, newY);
					}

					// Instruction address 0x1403:0x305d, size: 5
					this.parent.MapManagement.F0_2aea_138c_SetCityOwner(newX, newY, playerID);
					// Instruction address 0x1403:0x3079, size: 5
					this.parent.UnitManagement.F0_1866_01dc_UpdateUnitVisibility(newX, newY, playerID, unitID, true);

					if (this.parent.MapManagement.F0_2aea_1894_CellHasMinorTribeHut(newX, newY, newTerrainType))
					{
						// Instruction address 0x1403:0x30a0, size: 5
						this.parent.UnitManagement.F0_1866_1931_FoundMinorTribeHut(playerID, unitID);

						this.parent.GameData.MapVisibility[newX, newY] |= 1;
					}

					if (newTerrainType == TerrainTypeEnum.Water &&
						this.parent.GameData.Units[(int)unit.UnitType].MovementType != UnitMovementTypeEnum.Air &&
						this.parent.GameData.Units[(int)unit.UnitType].MovementType != UnitMovementTypeEnum.Water)
					{
						// Instruction address 0x1403:0x30f6, size: 5
						this.parent.UnitManagement.F0_1866_1560_UpdateWaterUnitStatusInStack(playerID, unitID);
					}

					if (playerID == 0 && newTerrainType != TerrainTypeEnum.Water)
					{
						if ((this.parent.GameData.MapVisibility[newX, newY] & (0x1 << this.parent.GameData.HumanPlayerID)) != 0 &&
							(unit.VisibleByPlayer & (0x1 << this.parent.GameData.HumanPlayerID)) == 0 &&
							this.parent.GameData.Cities[this.parent.MapManagement.FindNearestCity(newX, newY)].PlayerID == this.parent.GameData.HumanPlayerID)
						{
							unit.VisibleByPlayer |= this.parent.GameData.MapVisibility[newX, newY];
						}
					}

					if ((unit.VisibleByPlayer & (0x1 << this.parent.GameData.HumanPlayerID)) != 0 ||
						playerID == this.parent.GameData.HumanPlayerID || this.parent.GlobalPreferences.Debug.DebugMode && playerID != 0)
					{
						if (playerID == this.parent.GameData.HumanPlayerID || this.parent.GameData.GameSettingFlags.EnemyMoves)
						{
							// Instruction address 0x1403:0x31e6, size: 5
							this.parent.MapManagement.F0_1866_16a9_CenterMap(this.parent.GameData.HumanPlayerID, newX, newY);
							// Instruction address 0x1403:0x31f4, size: 5
							this.parent.MapManagement.F0_2aea_0e29_DrawUnit(playerID, unitID);

							if (playerID != this.parent.GameData.HumanPlayerID)
							{
								if (this.parent.GlobalPreferences.Debug.FastMode)
								{
									this.parent.CommonTools.WaitTimer(15);
								}
								else
								{
									this.parent.CommonTools.WaitTimer(30);
								}
							}
						}
						else
						{
							if (this.parent.MapManagement.F0_2aea_0e29_DrawUnit(playerID, unitID))
							{
								if (this.parent.GlobalPreferences.Debug.FastMode)
								{
									this.parent.CommonTools.WaitTimer(5);
								}
								else
								{
									this.parent.CommonTools.WaitTimer(10);
								}
							}
						}
					}

					unitMoveCount = 0;

					if (newImprovements.HasFlag(TerrainImprovementFlagsEnum.City) && this.parent.GameData.Units[(int)unit.UnitType].TurnsOutside != 0)
					{
						unit.ClearStatusFlags(UnitStatusEnum.SettlerBuildRoadOrRail);
						unit.RemainingMoves = 0;
						unit.GoToDestination = OpenCivOneGame.InvalidPosition;
					}

					if (this.parent.GameData.Units[(int)unit.UnitType].TurnsOutside != 0)
					{
						if (this.parent.GameData.Units[(int)unit.UnitType].MovementType == UnitMovementTypeEnum.Water)
						{
							if (this.parent.UnitManagement.F0_1866_1331_CountUnitTypesInStack(playerID, unitID, UnitTypeEnum.Carrier) != 0)
							{
								unit.ClearStatusFlags(UnitStatusEnum.SettlerBuildRoadOrRail);
								unit.RemainingMoves = 0;
								unit.SpecialMoves = this.parent.GameData.Units[(int)unit.UnitType].TurnsOutside;
							}
						}
					}
				}
			}
		}

		/// <summary>
		/// Draws the visible cell
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		public void F0_1403_3ed7_DrawVisibleCell(int x, int y)
		{
			//this.oCPU.Log.EnterBlock("'Fn2'(Cdecl, Far, Return) at 0x3ed7");

			// function body
			if ((this.parent.GameData.MapVisibility[x, y] & (1 << this.parent.GameData.HumanPlayerID)) != 0 || this.parent.GlobalPreferences.Debug.DebugMode)
			{
				// Instruction address 0x1403:0x3f09, size: 5
				this.parent.MapManagement.F0_2aea_11d4_DrawCellWithUnit(x, y);
			}
		}

		/// <summary>
		/// Redraws unit if unit is visible to the human player
		/// </summary>
		/// <param name="playerID"></param>
		/// <param name="unitID"></param>
		private void F0_1403_3f13_DrawVisibleCellWithUnit(int playerID, int unitID)
		{
			//this.oCPU.Log.EnterBlock("'Fn3'(Cdecl, Far, Return) at 0x3f13");

			// function body
			if (playerID == this.parent.GameData.HumanPlayerID ||
				(this.parent.GameData.Players[playerID].Units[unitID].VisibleByPlayer & (0x1 << this.parent.GameData.HumanPlayerID)) != 0)
			{
				// Instruction address 0x1403:0x3f5d, size: 5
				this.parent.MapManagement.F0_2aea_11d4_DrawCellWithUnit(
					this.parent.GameData.Players[playerID].Units[unitID].Position.X,
					this.parent.GameData.Players[playerID].Units[unitID].Position.Y);
			}
		}

		/// <summary>
		/// Get preferred improvement for this cell
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		public TerrainImprovementFlagsEnum F0_1403_3f68_GetPreferredImprovement(int x, int y)
		{
			//this.oCPU.Log.EnterBlock("'Fn4'(Cdecl, Far, Return) at 0x3f68");
			// function body
			if ((this.parent.MapManagement.F0_2aea_1585_GetVisibleTerrainImprovements(x, y) & 
				(TerrainImprovementFlagsEnum.City | TerrainImprovementFlagsEnum.Irrigation | TerrainImprovementFlagsEnum.Mines)) == TerrainImprovementFlagsEnum.None)
			{
				// Instruction address 0x1403:0x3f8a, size: 5
				TerrainTypeEnum terrainType = this.parent.MapManagement.GetTerrainType(x, y);

				if (this.parent.GameData.TerrainModifications[(int)terrainType].MiningEffect > -3)
				{
					if (this.parent.GameData.TerrainModifications[(int)terrainType].IrrigationEffect == -2 && this.parent.MapManagement.CanIrrigateCell(x, y))
					{
						// Irrigation
						return TerrainImprovementFlagsEnum.Irrigation;
					}
				}
				else
				{
					// Mine
					return TerrainImprovementFlagsEnum.Mines;
				}
			}

			// None
			return TerrainImprovementFlagsEnum.None;
		}

		/// <summary>
		/// Draws unit status panel or 'End Of Turn' message
		/// </summary>
		/// <param name="playerID"></param>
		/// <param name="unitID"></param>
		public void F0_1403_4060_DrawStatusPanel(int playerID, int unitID)
		{
			//this.oCPU.Log.EnterBlock("'Fn6'(Cdecl, Far, Return) at 0x4060");

			// function body
			// Instruction address 0x1403:0x407c, size: 5
			this.parent.DrawTools.F0_1238_1bb2_FillRectangleWithShadow(0, 97, 80, 103);

			if (unitID == 128)
			{
				// Instruction address 0x1403:0x409a, size: 5
				this.parent.DrawTools.DrawStringToScreen0("End of Turn", 4, 124, 0);
				// Instruction address 0x1403:0x40b1, size: 5
				this.parent.DrawTools.DrawStringToScreen0("Press Enter", 4, 136, 0);
				// Instruction address 0x1403:0x44f5, size: 5
				this.parent.DrawTools.DrawStringToScreen0("to continue", 4, 144, 0);
			}
			else
			{
				if (this.parent.GameData.Players[playerID].Units[unitID].UnitType != UnitTypeEnum.None)
				{
					int y = 99;

					// Instruction address 0x1403:0x40f3, size: 5
					this.parent.DrawTools.DrawStringToScreen0(this.parent.GameData.Players[playerID].Nationality, 4, 99, 0);

					y += 8;

					// Instruction address 0x1403:0x4133, size: 5
					this.parent.DrawTools.DrawStringToScreen0(this.parent.GameData.Units[(int)this.parent.GameData.Players[playerID].Units[unitID].UnitType].Name, 
						4, y, 0);

					y += 8;

					if ((this.parent.GameData.Players[playerID].Units[unitID].Status & UnitStatusEnum.Veteran) != UnitStatusEnum.None)
					{
						// Instruction address 0x1403:0x4154, size: 5
						this.parent.DrawTools.DrawStringToScreen0("Veteran", 8, y, 0);

						y += 8;
					}

					int Local_e = this.parent.CommonTools.CheckValueRange(this.parent.GameData.Players[playerID].Units[unitID].RemainingMoves / 3, 0, 99);
					int Local_e1 = Local_e + ((this.parent.GameData.Units[(int)this.parent.GameData.Players[playerID].Units[unitID].UnitType].TurnsOutside != 0) ?
					(this.parent.GameData.Units[(int)this.parent.GameData.Players[playerID].Units[unitID].UnitType].MoveCount *
							this.parent.GameData.Players[playerID].Units[unitID].SpecialMoves) : 0);
					int Local_c = this.parent.GameData.Players[playerID].Units[unitID].RemainingMoves % 3;

					// Instruction address 0x1403:0x4285, size: 5
					this.parent.DrawTools.DrawStringToScreen0($"Moves: {Local_e}{((Local_c != 0) ? $".{Local_c}" : "")}" +
						$"{((Local_e != Local_e1) ? $"({Local_e1})" : "")}", 4, y, 0);

					y += 8;

					// Instruction address 0x1403:0x42c2, size: 5
					this.parent.DrawTools.DrawStringToScreen0(this.parent.Segment_2459.F0_2459_08c6_GetCityName(this.parent.GameData.Players[playerID].Units[unitID].HomeCityID), 4, y, 0);

					y += 8;

					// Instruction address 0x1403:0x4325, size: 5
					this.parent.DrawTools.DrawStringToScreen0($"({this.parent.GameData.Terrains[(int)this.parent.MapManagement.GetTerrainType(
						this.parent.GameData.Players[playerID].Units[unitID].Position.X,
						this.parent.GameData.Players[playerID].Units[unitID].Position.Y)].Name})", 4, y, 0);

					y += 8;

					TerrainImprovementFlagsEnum terraintImprovements = this.parent.MapManagement.F0_2aea_1585_GetVisibleTerrainImprovements(
						this.parent.GameData.Players[playerID].Units[unitID].Position.X, this.parent.GameData.Players[playerID].Units[unitID].Position.Y);

					if (terraintImprovements.HasFlag(TerrainImprovementFlagsEnum.RailRoad))
					{
						// Instruction address 0x1403:0x4371, size: 5
						this.parent.DrawTools.DrawStringToScreen0("(RailRoad)", 4, y, 0);

						y += 8;
					}
					else
					{
						if (terraintImprovements.HasFlag(TerrainImprovementFlagsEnum.Road))
						{
							// Instruction address 0x1403:0x4371, size: 5
							this.parent.DrawTools.DrawStringToScreen0("(Road)", 4, y, 0);

							y += 8;
						}
					}

					if (terraintImprovements.HasFlag(TerrainImprovementFlagsEnum.Irrigation))
					{
						// Instruction address 0x1403:0x43a6, size: 5
						this.parent.DrawTools.DrawStringToScreen0("(Irrigation)", 4, y, 0);

						y += 8;
					}
					else
					{
						if (terraintImprovements.HasFlag(TerrainImprovementFlagsEnum.Mines))
						{
							// Instruction address 0x1403:0x43a6, size: 5
							this.parent.DrawTools.DrawStringToScreen0("(Mining)", 4, y, 0);

							y += 8;
						}
					}

					if (terraintImprovements.HasFlag(TerrainImprovementFlagsEnum.Pollution))
					{
						// Instruction address 0x1403:0x43c6, size: 5
						this.parent.DrawTools.DrawStringToScreen0("(Pollution)", 4, y, 0);

						y += 8;
					}

					y += 4;

					int nextUnitID = this.parent.GameData.Players[playerID].Units[unitID].NextUnitID;
					int x = 8;

					if (this.parent.MapManagement.F0_2aea_1585_GetVisibleTerrainImprovements(this.parent.GameData.Players[playerID].Units[unitID].Position.X,
						this.parent.GameData.Players[playerID].Units[unitID].Position.Y).HasFlag(TerrainImprovementFlagsEnum.City))
					{
						int cityID = this.parent.MapManagement.GetCityByLocation(
							this.parent.GameData.Players[playerID].Units[unitID].Position.X,
							this.parent.GameData.Players[playerID].Units[unitID].Position.Y);

						for (int i = 0; i < 2; i++)
						{
							int unknownValue = this.parent.GameData.Cities[cityID].FirstFortifiedCityUnits[i];

							if (unknownValue != -1)
							{
								this.parent.GameData.Players[playerID].Units[127].UnitType = (UnitTypeEnum)(unknownValue & 0x3f);
								this.parent.GameData.Players[playerID].Units[127].Status = UnitStatusEnum.Fortified;
								this.parent.GameData.Players[playerID].Units[127].GoToDestination = OpenCivOneGame.InvalidPosition;

								// Instruction address 0x1403:0x4468, size: 5
								this.parent.MapManagement.F0_2aea_0fb3_DrawUnitWithStatus(playerID, 127, x, y);

								this.parent.GameData.Players[playerID].Units[127].UnitType = UnitTypeEnum.None;

								x += 16;
							}
						}
					}

					while (nextUnitID != -1)
					{
						if (nextUnitID == unitID || y >= 184)
							break;

						// Instruction address 0x1403:0x449f, size: 5
						this.parent.MapManagement.F0_2aea_0fb3_DrawUnitWithStatus(playerID, nextUnitID, x, y);

						x += 16;

						if (x > 64)
						{
							x = 8;
							y += 16;
						}

						nextUnitID = this.parent.GameData.Players[playerID].Units[nextUnitID].NextUnitID;
					}

					if (nextUnitID != -1 && nextUnitID != unitID)
					{
						// Instruction address 0x1403:0x44f5, size: 5
						this.parent.DrawTools.DrawStringToScreen0("+", 74, 192, 0);
					}
				}
			}
		}

		/// <summary>
		/// Test is new map coordinates are valid
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		public bool F0_1403_4508_ValidateMapCoordinates(int x, int y)
		{
			//this.oCPU.Log.EnterBlock("'Fn7'(Cdecl, Far, Return) at 0x4508");
			// function body
			if (x < 16 && this.parent.Var_d4cc_MapViewX >= 65)
			{
				x += 80;
			}

			return (x >= this.parent.Var_d4cc_MapViewX && x < this.parent.Var_d4cc_MapViewX + 15 &&
				y >= this.parent.Var_d75e_MapViewY && y < this.parent.Var_d75e_MapViewY + 12);
		}

		/// <summary>
		/// Finds the nearest unit by its ID in a unit stack
		/// !!! Can't think why would that be important
		/// </summary>
		/// <param name="playerID"></param>
		/// <param name="unitID"></param>
		/// <returns></returns>
		private int F0_1403_4562_FindNearestUnitIDInUnitStack(int playerID, int unitID)
		{
			//this.oCPU.Log.EnterBlock("'Fn9'(Cdecl, Far, Return) at 0x4562");

			// Local variables
			int selectedUnitID;

			// function body
			int nextUnitID = this.parent.GameData.Players[playerID].Units[unitID].NextUnitID;

			if (nextUnitID == -1)
			{
				return unitID;
			}
			else
			{
				selectedUnitID = unitID;
				int smallestIDRange = 999;

				for (int i = 0; i < 16; i++)
				{
					// An empty slot in the stack chain has UnitType None (-1); Units[-1] throws in C#
					if (this.parent.GameData.Players[playerID].Units[nextUnitID].UnitType != UnitTypeEnum.None &&
						this.parent.GameData.Units[(int)this.parent.GameData.Players[playerID].Units[nextUnitID].UnitType].MovementType != UnitMovementTypeEnum.Air)
					{
						int idRange;

						if (nextUnitID > unitID)
						{
							idRange = nextUnitID - unitID;
						}
						else
						{
							idRange = (nextUnitID - unitID) + 128;
						}

						if (idRange < smallestIDRange)
						{
							smallestIDRange = idRange;
							selectedUnitID = nextUnitID;
						}
					}

					nextUnitID = this.parent.GameData.Players[playerID].Units[nextUnitID].NextUnitID;

					if (nextUnitID == unitID)
						break;
				}

				return selectedUnitID;
			}
		}
	}
}
