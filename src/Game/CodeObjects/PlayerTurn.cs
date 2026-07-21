using IRB.VirtualCPU;
using OpenCivOne.Graphics;
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
				this.parent.Segment_1238.F0_1238_1b44();
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
				this.parent.UnitManagement.F0_1866_01dc(
					this.parent.GameData.Players[playerID].Units[unitID].Position.X,
					this.parent.GameData.Players[playerID].Units[unitID].Position.Y,
					playerID, unitID, false);
			}

			if ((this.parent.GameData.Players[playerID].Units[unitID].Status & 0x9) != 0 ||
				this.parent.GameData.Players[playerID].Units[unitID].RemainingMoves == 0) goto Label18;

			if ((this.parent.GameData.Players[playerID].Units[unitID].Status & 0x4) != 0)
			{
				this.parent.GameData.Players[playerID].Units[unitID].Status &= 0xfb;
				this.parent.GameData.Players[playerID].Units[unitID].Status |= 0x8;
				this.parent.GameData.Players[playerID].Units[unitID].RemainingMoves = 0;

				// Instruction address 0x1403:0x0265, size: 5
				F0_1403_3f13(playerID, unitID);

				if (playerID != this.parent.GameData.HumanPlayerID)
				{
					if (this.parent.MapManagement.F0_2aea_1585_GetVisibleTerrainImprovements(this.parent.GameData.Players[playerID].Units[unitID].Position.X,
						this.parent.GameData.Players[playerID].Units[unitID].Position.Y).HasFlag(TerrainImprovementFlagsEnum.City))
					{
						int cityID = this.parent.Tools.F0_2dc4_00ba_GetCityByLocation(this.parent.GameData.Players[playerID].Units[unitID].Position.X,
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
					int cityID = this.parent.Tools.F0_2dc4_00ba_GetCityByLocation(this.parent.GameData.Players[playerID].Units[unitID].Position.X,
						this.parent.GameData.Players[playerID].Units[unitID].Position.Y);

					if (this.parent.GameData.Players[playerID].Units[unitID].HomeCityID == cityID)
					{
						if (!this.parent.UnitManagement.F0_1866_18d0_IsEnemyUnitNear(playerID, this.parent.GameData.Players[playerID].Units[unitID].Position.X,
							this.parent.GameData.Players[playerID].Units[unitID].Position.Y))
						{
							this.parent.UnitManagement.F0_1866_00c6(cityID);
						}
					}
				}
				goto Label18;
			}

			if (playerID == this.parent.GameData.HumanPlayerID)
			{
				if ((this.parent.GameData.Players[playerID].Units[unitID].Status & 0xc2) == 0)
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
							// Instruction address 0x1403:0x03dd, size: 5
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
								F0_1403_4060(playerID, unitID);
							}

							goto Label54;
						}

						goto Label18;
					}
				}
			}

		Label54:
			if ((this.parent.GameData.PlayerFlags & (0x1 << playerID)) != 0)
			{
				// Instruction address 0x1403:0x044c, size: 5
				this.parent.CommonTools.ClearKeyboardAndMouseEvents();
			}

			int local_a = 0;

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

				local_a++;

				if (local_a > 4)
				{
					command = ' ';
					this.parent.GameData.Players[playerID].Units[unitID].GoToDestination = OpenCivOneGame.InvalidPosition;
					this.parent.GameData.Players[playerID].Units[unitID].GoToNextDirection = -1;
				}
				goto Label151;
			}

			if (unitID >= 128 ||
				((this.parent.GameData.Players[playerID].Units[unitID].Status & 0xc2) == 0 &&
					this.parent.GameData.Players[playerID].Units[unitID].GoToDestination.X == -1)) goto Label79;

			command = 'r';

			if ((this.parent.GameData.Players[playerID].Units[unitID].Status & 0x40) != 0)
			{
				if (this.parent.GameData.Players[playerID].Units[unitID].UnitType != 0)
				{
					command = 'm';
				}
				else
				{
					command = 'i';
				}
			}

			if ((this.parent.GameData.Players[playerID].Units[unitID].Status & 0x80) != 0)
			{
				command = 'm';

				if ((this.parent.GameData.Players[playerID].Units[unitID].Status & 0x40) != 0)
				{
					command = 'f';
				}

				if ((this.parent.GameData.Players[playerID].Units[unitID].Status & 0x2) != 0)
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
			// Instruction address 0x1403:0x05d2, size: 5
			F0_1403_4060(playerID, unitID);

			if (this.parent.GameData.TurnCount == 0)
			{
				if (!flag6)
				{
					this.parent.Array_30b8[0] = this.parent.GameData.Players[playerID].Nation;
					this.parent.Help.F4_0000_02d3_ShowInstantAdvicePopup("*FIRSTMOVE");

					flag6 = true;
				}
			}

			if (unitID < 128 && this.parent.GameData.Players[playerID].Units[unitID].UnitType == 0)
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
				command = this.parent.MenuBoxDialog.F0_2d05_0ac9_GetNavigationKey();

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
						this.parent.Tools.F0_2dc4_007c_CheckValueRange(mouseEvent.Position.Y - 14 + this.parent.Var_70ea_MiniMapY, 0, 49));
					//}
				}
				else if (mouseEvent.Position.Y < 72)
				{
					this.parent.Palace.F17_0000_07ec(0);

					this.parent.Var_aa_Screen0_Rectangle.ScreenID = 0;

					// Instruction address 0x1403:0x07af, size: 5
					this.parent.Tools.F0_2dc4_065f_StopPaletteCycleSlots();

					// Instruction address 0x1403:0x07cc, size: 5
					this.parent.DrawTools.FillRectangle(this.parent.Var_aa_Screen0_Rectangle, 0, 0, 320, 200, 0);

					// Instruction address 0x1403:0x07d8, size: 5
					this.parent.ImageTools.F0_2fa1_01a2_LoadBitmapOrPalette(-1, 0, 0, "CBACK.PIC", 1);

					// Instruction address 0x1403:0x0800, size: 5
					this.parent.Graphics.F0_VGA_07d8_DrawImage(this.parent.Var_19d4_Screen1_Rectangle, 0, 0, 320, 200, this.parent.Var_aa_Screen0_Rectangle, 0, 0);

					// Instruction address 0x1403:0x080d, size: 5
					this.parent.Segment_2459.F0_2459_0918_WaitForKeyPressOrMouseClick();

					// Instruction address 0x1403:0x0826, size: 5
					this.parent.Segment_1238.F0_1238_1b44();

					// Instruction address 0x1403:0x082b, size: 5
					this.parent.Tools.F0_2dc4_0626_StartPaletteCycleSlots();
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
							int distance = this.parent.Tools.F0_2dc4_0289_GetShortestDistance(
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

								if ((this.parent.GameData.MapVisibility[mouseX, mouseY] & (0x1 << this.parent.GameData.HumanPlayerID)) != 0 || this.parent.Var_d806_DebugFlag)
								{
									// Instruction address 0x1403:0x09b4, size: 5
									this.parent.Encyclopedia.F8_0000_062a_DisplayEncyclopediaTopic(EncyclopediaTopicEnum.TerrainType, (int)this.parent.MapManagement.GetTerrainType(mouseX, mouseY));

									// Instruction address 0x1403:0x09c5, size: 5
									this.parent.Segment_1238.F0_1238_1b44();
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
							this.parent.Var_d806_DebugFlag))
					{
						int cityID = this.parent.Tools.F0_2dc4_00ba_GetCityByLocation(mouseX, mouseY);

						// Instruction address 0x1403:0x0b9a, size: 5
						this.parent.Segment_1ade.F0_1ade_03ea(cityID);
						// Instruction address 0x1403:0x0ba2, size: 5
						this.parent.Segment_1238.F0_1238_107e();

						flag2 = false;
					}
					else
					{
						int activeUnitID1 = this.parent.MapManagement.F0_2aea_1458_GetCellActiveUnitID(mouseX, mouseY);

						if (this.parent.Var_d806_DebugFlag && activeUnitID1 != -1)
						{
							this.parent.ShowDebugDetails.ShowUnitStatus(this.parent.Var_d7f0, activeUnitID1);
						}

						if (activeUnitID1 != -1 && playerID == this.parent.Var_d7f0)
						{
							int local_3a = this.parent.UnitManagement.F0_1866_1f69(this.parent.Var_d7f0, activeUnitID1);

							if ((this.parent.GameData.Players[playerID].Units[local_3a].Status & 0x9) == 0)
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
			int distanceToObject;
			int newMoveDirection = 0;

			switch (command)
			{
				// Change government
				case -2:
					if (this.parent.Segment_1238.F0_1238_001e_ShowDialog(this.parent.LanguageTools.F0_2f4d_044f_GetTextFromKingSection("*REV"), 64, 80) == 1)
					{
						this.parent.GameData.Players[playerID].GovernmentType = 0;

						this.parent.News.F21_0000_0000_ShowNews(-1, $"The {this.parent.GameData.Players[playerID].Nation} are\nrevolting! Citizens\ndemand new government.\n");

						this.parent.StartGameMenu.F5_0000_1af6_LoadGovernmentImage();

						// Instruction address 0x1403:0x35be, size: 5
						this.parent.Segment_1238.F0_1238_1b44();
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
						int cityID = this.parent.Tools.F0_2dc4_0102_FindNearestCity(unit.Position.X, unit.Position.Y);

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

								F0_1403_3ed7(unit.Position.X, unit.Position.Y);
								this.parent.Segment_2517.F0_2517_0aa1_ClearDiplomacyFlags(this.parent.GameData.HumanPlayerID,
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

					if (unit.UnitType != 0)
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
								this.parent.GameData.Cities[cityID].ActualSize++;
								unit.RemainingMoves = 0;

								// Instruction address 0x1403:0x1a6a, size: 5
								this.parent.UnitManagement.F0_1866_0f10_DeleteUnit(playerID, unitID);

								// Instruction address 0x1403:0x1a78, size: 5
								F0_1403_3f13(playerID, unitID);
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
								F0_1403_3f13(playerID, unitID);
							}
						}
					}
					break;

				// Build fortress
				case 'f':
					if (unit == null || unitID >= 128) break;

					if (this.parent.GameData.Units[(int)unit.UnitType].MovementType == UnitMovementTypeEnum.Land)
					{
						if (unit.UnitType != 0)
						{
							unit.Status |= 0x4;
							unit.RemainingMoves = 0;

							// Instruction address 0x1403:0x12bf, size: 5
							F0_1403_3f13(playerID, unitID);

							if (playerID != this.parent.GameData.HumanPlayerID &&
								this.parent.MapManagement.F0_2aea_1585_GetVisibleTerrainImprovements(unit.Position.X, unit.Position.Y).HasFlag(TerrainImprovementFlagsEnum.City))
							{
								unit.HomeCityID = (short)this.parent.Tools.F0_2dc4_00ba_GetCityByLocation(unit.Position.X, unit.Position.Y);
							}
						}
						else
						{
							if (this.parent.MapManagement.F0_2aea_1585_GetVisibleTerrainImprovements(unit.Position.X, unit.Position.Y).HasFlag(TerrainImprovementFlagsEnum.City) ||
								this.parent.MapManagement.F0_2aea_1585_GetVisibleTerrainImprovements(unit.Position.X, unit.Position.Y).HasFlag(TerrainImprovementFlagsEnum.Fortress) ||
								!this.parent.Segment_1ade.F0_1ade_22b5_PlayerHasTechnology(playerID, TechnologyAdvanceEnum.Construction))
							{
								unit.Status &= 0x3f;
								unit.SpecialMoves = 0;

								this.parent.UnitManagement.F0_1866_16a9_CenterMap(playerID, unit.Position.X, unit.Position.Y);
							}
							else
							{
								unit.Status |= 0xc0;
								unit.RemainingMoves = 0;

								// Instruction address 0x1403:0x1388, size: 5
								F0_1403_3f13(playerID, unitID);

								unit.SpecialMoves++;

								if ((this.parent.GameData.Terrains[(int)terrainType].MovementCost + 4) <= unit.SpecialMoves)
								{
									// Instruction address 0x1403:0x13bf, size: 5
									this.parent.MapManagement.F0_2aea_1653_SetTerrainImprovements(unit.Position.X, unit.Position.Y, TerrainImprovementFlagsEnum.Fortress);

									// Instruction address 0x1403:0x13db, size: 5
									this.parent.UnitManagement.F0_1866_01dc(unit.Position.X, unit.Position.Y, playerID, unitID, true);

									unit.Status &= 0x3f;
									unit.SpecialMoves = 0;
								}
							}
						}
					}
					break;

				// Go To
				case 'g':
					if (unit == null || unitID >= 128) break;

					int local_56 = 0;

					if (flag3)
					{
						local_56 = 7;
					}
					else
					{
						local_56 = 2;
					}

					this.parent.CommonTools.SetMousePositionAndIcon(0, 0, this.parent.Array_d4ce[local_56]);

					flag3 ^= true;
					break;

				// Set as new home city
				case 'h':
					if (unit == null || unitID >= 128) break;

					nearestCityID = this.parent.Tools.F0_2dc4_0102_FindNearestCity(unit.Position.X, unit.Position.Y);

					if (nearestCityID != -1 &&
						(distanceToObject = this.parent.Tools.F0_2dc4_0289_GetShortestDistance(unit.Position.X, unit.Position.Y, this.parent.GameData.Cities[nearestCityID].Position)) == 0)
					{
						unit.HomeCityID = (short)nearestCityID;
					}
					else if (this.parent.GameData.Units[(int)unit.UnitType].MovementType == UnitMovementTypeEnum.Water)
					{
						// Instruction address 0x1403:0x1118, size: 5
						int local_24 = this.parent.UnitManagement.F0_1866_226d(playerID, unitID);

						if (local_24 != 0)
						{
							unit.Status |= 2;
							goto Label300;
						}
						else
						{
							if (playerID == this.parent.GameData.HumanPlayerID)
							{
								unit.Status &= 0xfd;
							}
							else
							{
								unit.RemainingMoves = 0;

								// Instruction address 0x1403:0x1175, size: 5
								this.parent.UnitManagement.F0_1866_0f10_DeleteUnit(playerID, unitID);
							}
						}
					}
					break;

				// Build irrigation
				case 'i':
					if (unit == null || unitID >= 128) break;

					if (unit.UnitType != 0 ||
						(this.parent.GameData.DebugFlags & 0x2) == 0 ||
						this.parent.MapManagement.F0_2aea_1585_GetVisibleTerrainImprovements(unit.Position.X, unit.Position.Y).HasFlag(TerrainImprovementFlagsEnum.Irrigation))
					{
						// Instruction address 0x1403:0x15ce, size: 5
						this.parent.UnitManagement.F0_1866_16a9_CenterMap(playerID, unit.Position.X, unit.Position.Y);

						if (unit.UnitType != 0)
						{
							this.parent.Help.F4_0000_03aa_ShowInstantWarningPopup("*SETTLERS");
						}
						else
						{
							unit.SpecialMoves = 0;
						}

						unit.Status &= 0xbf;
					}
					else
					{
						int irrigatedTerrainType = this.parent.GameData.TerrainModifications[(int)terrainType].IrrigationEffect;

						if (irrigatedTerrainType == -1)
						{
							// Instruction address 0x1403:0x165e, size: 5
							this.parent.UnitManagement.F0_1866_16a9_CenterMap(playerID, unit.Position.X, unit.Position.Y);
							this.parent.Help.F4_0000_03aa_ShowInstantWarningPopup("*NOIRR");

							unit.Status &= 0xbf;
						}
						else
						{
							if (irrigatedTerrainType != -2 || this.parent.MapManagement.CanIrrigateCell(unit.Position.X, unit.Position.Y))
							{
								unit.Status |= 0x40;

								// Instruction address 0x1403:0x16f9, size: 5
								F0_1403_3f13(playerID, unitID);

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
									F0_1403_3f13(playerID, unitID);

									unit.Status &= 0xbf;
									unit.SpecialMoves = 0;
								}
							}
							else
							{
								// Instruction address 0x1403:0x16b2, size: 5
								this.parent.UnitManagement.F0_1866_16a9_CenterMap(playerID, unit.Position.X, unit.Position.Y);
								// Instruction address 0x1403:0x16be, size: 5
								this.parent.Help.F4_0000_03aa_ShowInstantWarningPopup("*NOWATER");

								unit.Status &= 0xbf;
							}
						}
					}
					break;

				// Build forest or mine
				case 'm':
					if (unit == null || unitID >= 128) break;

					if (unit.UnitType != 0 ||
						this.parent.MapManagement.F0_2aea_1585_GetVisibleTerrainImprovements(unit.Position.X, unit.Position.Y).HasFlag(TerrainImprovementFlagsEnum.Mines))
					{
						// Instruction address 0x1403:0x17fc, size: 5
						this.parent.UnitManagement.F0_1866_16a9_CenterMap(playerID, unit.Position.X, unit.Position.Y);

						if (unit.UnitType != 0)
						{
							// Instruction address 0x1403:0x1822, size: 5
							this.parent.Help.F4_0000_03aa_ShowInstantWarningPopup("*SETTLERS");
						}
						else
						{
							unit.SpecialMoves = 0;
						}

						unit.Status &= 0x7f;
					}
					else
					{
						int minedTerrainType = this.parent.GameData.TerrainModifications[(int)terrainType].MiningEffect;

						if (minedTerrainType == -1)
						{
							// Instruction address 0x1403:0x188c, size: 5
							this.parent.UnitManagement.F0_1866_16a9_CenterMap(playerID, unit.Position.X, unit.Position.Y);
							// Instruction address 0x1403:0x1898, size: 5
							this.parent.Help.F4_0000_03aa_ShowInstantWarningPopup("*NOMINE");

							unit.Status &= 0x7f;
						}
						else
						{
							unit.Status |= 0x80;

							// Instruction address 0x1403:0x18d3, size: 5
							F0_1403_3f13(playerID, unitID);

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
										F0_1403_3f13(playerID, unitID);

										unit.Status &= 0x7f;
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
									F0_1403_3f13(playerID, unitID);

									unit.Status &= 0x7f;
									unit.SpecialMoves = 0;
								}
							}
						}
					}
					break;

				// Clear pollution
				case 'p':
					if (unit == null || unitID >= 128) break;

					if (unit.UnitType != 0)
					{
						// Instruction address 0x1403:0x119e, size: 5
						this.parent.Help.F4_0000_03aa_ShowInstantWarningPopup("*SETTLERS");
					}
					else
					{
						TerrainImprovementFlagsEnum terrainImprovements = this.parent.MapManagement.F0_2aea_1585_GetVisibleTerrainImprovements(unit.Position.X, unit.Position.Y);

						if (!terrainImprovements.HasFlag(TerrainImprovementFlagsEnum.Pollution))
						{
							unit.Status &= 0x7d;
							unit.SpecialMoves = 0;

							// Instruction address 0x1403:0x11e6, size: 5
							this.parent.UnitManagement.F0_1866_16a9_CenterMap(playerID, unit.Position.X, unit.Position.Y);
						}
						else
						{
							unit.Status |= 0x82;

							// Instruction address 0x1403:0x120c, size: 5
							F0_1403_3f13(playerID, unitID);

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
								F0_1403_3f13(playerID, unitID);

								unit.Status &= 0x7d;
								unit.SpecialMoves = 0;
							}
						}
					}
					break;

				// Build roads or railroads
				case 'r':
					if (unit == null || unitID >= 128) break;

					if (unit.UnitType != 0)
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
							unit.Status &= 0xfd;
							unit.SpecialMoves = 0;

							// Instruction address 0x1403:0x148f, size: 5
							this.parent.UnitManagement.F0_1866_16a9_CenterMap(playerID, unit.Position.X, unit.Position.Y);
						}
						else
						{
							if (terrainType != TerrainTypeEnum.River || this.parent.Segment_1ade.F0_1ade_22b5_PlayerHasTechnology(playerID, TechnologyAdvanceEnum.BridgeBuilding))
							{
								unit.Status |= 2;
								unit.RemainingMoves = 0;

								// Instruction address 0x1403:0x14ee, size: 5
								F0_1403_3f13(playerID, unitID);

								int local_58 = unit.SpecialMoves;

								unit.SpecialMoves++;

								if ((((terrainImprovements.HasFlag(TerrainImprovementFlagsEnum.Road)) ? 4 : 2) * this.parent.GameData.Terrains[(int)terrainType].MovementCost) <= local_58)
								{
									// Instruction address 0x1403:0x154b, size: 5
									this.parent.MapManagement.F0_2aea_1653_SetTerrainImprovements(
										unit.Position.X, unit.Position.Y, ((terrainImprovements.HasFlag(TerrainImprovementFlagsEnum.Road)) ? TerrainImprovementFlagsEnum.RailRoad : TerrainImprovementFlagsEnum.Road));
									// Instruction address 0x1403:0x1567, size: 5
									this.parent.UnitManagement.F0_1866_01dc(unit.Position.X, unit.Position.Y, playerID, unitID, true);

									unit.Status &= 0xfd;
									unit.SpecialMoves = 0;
								}
							}
						}
					}
					break;

				// Sentry unit
				case 's':
					if (unit == null || unitID >= 128) break;

					unit.Status |= 1;
					unit.RemainingMoves = 0;

					// Instruction address 0x1403:0x0f64, size: 5
					F0_1403_3f13(playerID, unitID);

					break;

				// Unload unit from carrier
				case 'u':
					if (unit == null || unitID >= 128) break;

					if (this.parent.GameData.Units[(int)unit.UnitType].TransportCapacity != 0 ||
						unit.UnitType == UnitTypeEnum.Carrier)
					{
						if (unit.NextUnitID != -1)
						{
							// Instruction address 0x1403:0x0fc8, size: 5
							this.parent.UnitManagement.F0_1866_14f6_UnitStack(playerID, unitID);

							flag2 = false;
							flag7 = true;
							unitID = F0_1403_4562(playerID, unitID) - 1;
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
					if (unit == null || unitID >= 128) break;

					newMoveDirection = 1;
					goto Label300;

				// Move unit PageUp
				case '\x002':
				case '\x4900':
					if (unit == null || unitID >= 128) break;

					newMoveDirection = 2;
					goto Label300;

				// Move unit Right
				case '\x003':
				case '\x4d00':
					if (unit == null || unitID >= 128) break;

					newMoveDirection = 3;
					goto Label300;

				// Move unit PageDown
				case '\x004':
				case '\x5100':
					if (unit == null || unitID >= 128) break;

					newMoveDirection = 4;
					goto Label300;

				// Move unit Down
				case '\x005':
				case '\x5000':
					if (unit == null || unitID >= 128) break;

					newMoveDirection = 5;
					goto Label300;

				// Move unit End
				case '\x006':
				case '\x4f00':
					if (unit == null || unitID >= 128) break;

					newMoveDirection = 6;
					goto Label300;

				// Move unit Left
				case '\x007':
				case '\x4b00':
					if (unit == null || unitID >= 128) break;

					newMoveDirection = 7;
					goto Label300;

				// Move unit Home
				case '\x008':
				case '\x4700':
					if (unit == null || unitID >= 128) break;

					newMoveDirection = 8;
					goto Label300;

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
					int selectedTaxRate = this.parent.Segment_1238.F0_1238_001e_ShowDialog(taxOptions.ToString(), 100, 80);

					if (selectedTaxRate != -1)
					{
						this.parent.GameData.Players[playerID].ScienceTaxRate += (short)(this.parent.GameData.Players[playerID].TaxRate - selectedTaxRate);
						this.parent.GameData.Players[playerID].TaxRate = (short)selectedTaxRate;

						// Instruction address 0x1403:0x36b1, size: 5
						this.parent.Segment_1238.F0_1238_107e();
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
						luxuryOptions.Append($"{i * 10}% Luxuries, ({(10 - i + this.parent.GameData.Players[playerID].TaxRate) * 10}% Science)\n ");
					}

					this.parent.Var_2f9a_MenuBoxDefaultOptionIndex = (10 - this.parent.GameData.Players[playerID].TaxRate + this.parent.GameData.Players[playerID].ScienceTaxRate);

					int newLuxuryRate = this.parent.Segment_1238.F0_1238_001e_ShowDialog(luxuryOptions.ToString(), 100, 80);

					if (newLuxuryRate != -1)
					{
						this.parent.GameData.Players[playerID].ScienceTaxRate = (short)(10 - this.parent.GameData.Players[playerID].TaxRate + newLuxuryRate);
						// Instruction address 0x1403:0x37a3, size: 5
						this.parent.Segment_1238.F0_1238_107e();
					}
					break;

				// Find city
				case '/':
				case '?':
					this.parent.TextBoxDialogs.F23_0000_025b_FindCityDialog();
					break;

				// Save game
				case 'S':
					// Enable Save game with zero turns
					//if (this.oParent.GameData.TurnCount != 0)
					//{
					// All Top menus are saving the current screenshot, so should we in Save game shortcut
					this.parent.Graphics.F0_VGA_07d8_DrawImage(this.parent.Var_aa_Screen0_Rectangle, 0, 0, 320, 200, this.parent.Var_19d4_Screen1_Rectangle, 0, 0);

					this.parent.LoadAndSave.F11_0000_036a_SaveGameDialog(false);
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
					if (this.parent.Segment_1238.F0_1238_001e_ShowDialog("Are you sure you\nwant to Quit?\n Keep Playing\n Yes, Quit\n", 100, 80) != 1)
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
					this.parent.Segment_1238.F0_1238_001e_ShowDialog(0x2003, 100, 80);
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

				// Alt + D - Activate / Deactivate debug mode
				case 0x2000:
					this.parent.Var_d806_DebugFlag = !this.parent.Var_d806_DebugFlag;

					// Instruction address 0x1403:0x3506, size: 5
					this.parent.MapManagement.F0_2aea_0008_DrawVisibleMap(this.parent.GameData.HumanPlayerID, this.parent.Var_d4cc_MapViewX, this.parent.Var_d75e_MapViewY);
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
					this.parent.Segment_1238.F0_1238_001e_ShowDialog($"Sounds {((this.parent.GameData.GameSettingFlags.Sound) ? "ON\n" : "OFF\n")}", 100, 80);
					break;

				// F1 - City status
				case 0x3b00:
					if (this.parent.Var_d806_DebugFlag)
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
					if (this.parent.Var_d806_DebugFlag)
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
					if (this.parent.Var_d806_DebugFlag)
					{
						// Instruction address 0x1403:0x33ae, size: 5
						this.parent.CommonTools.F0_1000_0846(2);

						// Instruction address 0x1403:0x33b6, size: 5
						this.parent.CAPI.getch();

						// Instruction address 0x1403:0x33bf, size: 5
						this.parent.CommonTools.F0_1000_0846(0);
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
					if (this.parent.Var_d806_DebugFlag)
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
					if (this.parent.Var_d806_DebugFlag)
					{
						this.parent.Reports.F12_0000_0573_ShowPowerGraphReport();
						this.parent.Replay.F9_0000_0000();
					}
					else
					{
						this.parent.HallOfFame.F3_0000_09ac_ShowTopFiveCitiesPopup();
					}
					break;

				// F9 - Civilization Score
				case 0x4300:
					if (this.parent.Var_d806_DebugFlag)
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
			goto Label722;

		Label300:
			#region Move Unit
			if (unit != null && unit.UnitType != UnitTypeEnum.None)
			{
				if (playerID == 0 && unit.UnitType != UnitTypeEnum.Diplomat)
				{
					TerrainImprovementFlagsEnum terrainImprovements = this.parent.MapManagement.F0_2aea_1585_GetVisibleTerrainImprovements(unit.Position.X, unit.Position.Y);

					if (!terrainImprovements.HasFlag(TerrainImprovementFlagsEnum.City) &&
						(terrainImprovements.HasFlag(TerrainImprovementFlagsEnum.Mines) || terrainImprovements.HasFlag(TerrainImprovementFlagsEnum.Irrigation)) &&
						unit.UnitType != UnitTypeEnum.Legion && unit.UnitType != UnitTypeEnum.Knights &&
						this.parent.GameData.Cities[this.parent.Tools.F0_2dc4_0102_FindNearestCity(unit.Position.X, unit.Position.Y)].PlayerID == this.parent.GameData.HumanPlayerID)
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
						goto Label722;
					}
				}

				int newX = this.parent.MapManagement.AdjustXPosition(unit.Position.X + this.parent.MoveDirections[newMoveDirection].X);
				int newY = this.parent.Tools.F0_2dc4_007c_CheckValueRange(unit.Position.Y + this.parent.MoveDirections[newMoveDirection].Y, 0, 49);
				TerrainTypeEnum newTerrainType = this.parent.MapManagement.GetTerrainType(newX, newY);
				TerrainImprovementFlagsEnum newImprovements = this.parent.MapManagement.F0_2aea_1585_GetVisibleTerrainImprovements(newX, newY);
				int activeUnitID = this.parent.MapManagement.F0_2aea_1458_GetCellActiveUnitID(newX, newY);

				if (this.parent.GameData.Units[(int)unit.UnitType].MovementType == UnitMovementTypeEnum.Land)
				{
					if (activeUnitID == -1 && this.parent.UnitManagement.F0_1866_1725_IsUnitNear(playerID, unit.Position.X, unit.Position.Y) &&
						this.parent.UnitManagement.F0_1866_1725_IsUnitNear(playerID, newX, newY) &&
						(unit.UnitType != UnitTypeEnum.Diplomat && unit.UnitType != UnitTypeEnum.Caravan) && terrainType != TerrainTypeEnum.Water)
					{
						if (playerID == this.parent.GameData.HumanPlayerID)
						{
							// Instruction address 0x1403:0x1e9c, size: 5
							this.parent.CommonTools.PlayTune(37, 0);
						}

						if (this.parent.GameData.GameSettingFlags.InstantAdvice)
						{
							// Instruction address 0x1403:0x1eb2, size: 5
							this.parent.Help.F4_0000_03aa_ShowInstantWarningPopup("*ZOC");
						}

						unit.GoToDestination = OpenCivOneGame.InvalidPosition;

						goto Label722;
					}

					if (unit.UnitType == UnitTypeEnum.Diplomat || unit.UnitType == UnitTypeEnum.Caravan)
					{
						int cityID = this.parent.Tools.F0_2dc4_00ba_GetCityByLocation(newX, newY);

						if (cityID != -1)
						{
							if (this.parent.GameData.Cities[cityID].PlayerID != playerID && terrainType == TerrainTypeEnum.Water) goto Label722;

							if (unit.UnitType == UnitTypeEnum.Diplomat && playerID != this.parent.GameData.Cities[cityID].PlayerID)
							{
								if (this.parent.GameData.HumanPlayerID == this.parent.GameData.Cities[cityID].PlayerID)
								{
									unit.VisibleByPlayer |= (ushort)(1 << this.parent.GameData.HumanPlayerID);

									// Instruction address 0x1403:0x1f7f, size: 5
									this.parent.UnitManagement.F0_1866_16a9_CenterMap(this.parent.GameData.HumanPlayerID,
										unit.Position.X, unit.Position.Y);
									// Instruction address 0x1403:0x1f8b, size: 5
									this.parent.CommonTools.WaitTimer(30);
									// Instruction address 0x1403:0x1f9c, size: 5
									this.parent.UnitManagement.F0_1866_1d55_AnimateUnitMove(playerID, unitID, newMoveDirection);
								}

								this.parent.Overlay_22.F22_0000_0000_DiplomatAction(cityID, playerID, unitID);

								goto Label722;
							}

							if (unit.UnitType == UnitTypeEnum.Caravan)
							{
								int distance = this.parent.Tools.F0_2dc4_0289_GetShortestDistance(
									newX, this.parent.GameData.Cities[unit.HomeCityID].Position.X,
									newY, this.parent.GameData.Cities[unit.HomeCityID].Position.Y);

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

										selectedCaravanOption = this.parent.Segment_1238.F0_1238_001e_ShowDialog(caravanOptions.ToString(), 100, 80);
									}

									if (selectedCaravanOption == 1)
									{
										this.parent.Segment_2459.F0_2459_0948_CaravanArrivesAtDestinationCity(playerID, unitID, cityID);

										goto Label722;
									}

									if (selectedCaravanOption == 2)
									{
										this.parent.GameData.Cities[cityID].ShieldsCount += (short)(10 * this.parent.GameData.Units[(int)unit.UnitType].Cost);

										// Instruction address 0x1403:0x2149, size: 5
										this.parent.UnitManagement.F0_1866_0f10_DeleteUnit(playerID, unitID);

										goto Label722;
									}
								}
							}
						}
						else
						{
							if (unit.UnitType == UnitTypeEnum.Diplomat && activeUnitID != -1 && playerID != this.parent.Var_d20a)
							{
								this.parent.Overlay_22.F22_0000_0639_DiplomatInteraction(this.parent.Var_d20a, activeUnitID, playerID);

								unit.GoToDestination = OpenCivOneGame.InvalidPosition;
							}
						}
					}
				}

				if (unit.UnitType != UnitTypeEnum.None && this.parent.GameData.Units[(int)unit.UnitType].MovementType == UnitMovementTypeEnum.Land && 
					newTerrainType == TerrainTypeEnum.Water)
				{
					if (playerID != this.parent.Var_d20a || activeUnitID == -1 || this.parent.UnitManagement.F0_1866_13d5(playerID, activeUnitID) <= 0) goto Label722;

					unit.Status |= 1;
					unit.Status &= 0xf3;
					unit.RemainingMoves = 3;
				}
				else if (activeUnitID != -1 && playerID != this.parent.Var_d20a)
				{
					if (terrainType == TerrainTypeEnum.Water && this.parent.GameData.Units[(int)unit.UnitType].MovementType == UnitMovementTypeEnum.Land)
					{
						// Instruction address 0x1403:0x224c, size: 5
						this.parent.Help.F4_0000_03aa_ShowInstantWarningPopup("*AMPHIB");

						unit.GoToDestination = OpenCivOneGame.InvalidPosition;

						goto Label722;
					}

					if (newTerrainType != TerrainTypeEnum.Water && unit.UnitType == UnitTypeEnum.Submarine) goto Label722;

					if (playerID != this.parent.GameData.HumanPlayerID && this.parent.GameData.Units[(int)unit.UnitType].AttackStrength == 0) goto Label722;

					if (unit.UnitType == UnitTypeEnum.Diplomat) goto Label722;

					if (this.parent.GameData.Units[(int)this.parent.GameData.Players[this.parent.Var_d20a].Units[activeUnitID].UnitType].MovementType == UnitMovementTypeEnum.Water)
					{
						if (unit.UnitType != UnitTypeEnum.Fighter)
						{
							if (!newImprovements.HasFlag(TerrainImprovementFlagsEnum.City))
							{
								// Instruction address 0x1403:0x2319, size: 5
								this.parent.Help.F4_0000_03aa_ShowInstantWarningPopup("*FIGHTER");

								unit.GoToDestination = OpenCivOneGame.InvalidPosition;

								goto Label722;
							}
						}
					}

					if (unit.RemainingMoves < 3)
					{
						if (playerID == this.parent.GameData.HumanPlayerID)
						{
							if (this.parent.Segment_1238.F0_1238_001e_ShowDialog($"Attack at\n{unit.RemainingMoves}/3 strength?\n Cancel\n Attack\n", 100, 16) != 1)
							{
								unit.GoToDestination = OpenCivOneGame.InvalidPosition;

								goto Label722;
							}
						}
						else
						{
							if (unit.RemainingMoves < 2) goto Label722;
						}
					}

					if (this.parent.Var_d20a == this.parent.GameData.HumanPlayerID)
					{
						// Instruction address 0x1403:0x23eb, size: 5
						this.parent.UnitManagement.F0_1866_16a9_CenterMap(this.parent.GameData.HumanPlayerID, newX, newY);
					}

					activeUnitID = this.parent.UnitManagement.F0_1866_1122(this.parent.Var_d20a, activeUnitID);

					if (playerID == this.parent.GameData.HumanPlayerID || this.parent.Var_d20a == this.parent.GameData.HumanPlayerID)
					{
						this.parent.Var_70d8 = true;
					}

					// Attack unit?
					int local_3a = this.parent.Segment_29f3.F0_29f3_000e_AttackUnit(playerID, unitID, this.parent.Var_d20a, activeUnitID, true);
					unit.GoToDestination = OpenCivOneGame.InvalidPosition;
					this.parent.Var_70d8 = false;

					if (local_3a == -1) goto Label722;

					if (unit.UnitType == UnitTypeEnum.None)
					{
						unit.RemainingMoves = 0;
						goto Label722;
					}

					unit.RemainingMoves -= 3;

					if (unit.RemainingMoves < 0 || unit.RemainingMoves == 15)
					{
						unit.RemainingMoves = 0;
					}

					if (!newImprovements.HasFlag(TerrainImprovementFlagsEnum.City)) goto Label722;

					int cityID = this.parent.Tools.F0_2dc4_00ba_GetCityByLocation(newX, newY);

					if (this.parent.GameData.Cities[cityID].PlayerID != this.parent.GameData.HumanPlayerID)
					{
						this.parent.GameData.Cities[cityID].StatusFlag |= 0x10;
					}

					if (unit.UnitType != UnitTypeEnum.None)
					{
						if (!this.parent.GameData.Cities[cityID].HasImprovement(ImprovementEnum.CityWalls))
						{
							if (terrainType != TerrainTypeEnum.Water)
							{
								if (this.parent.GameData.DifficultyLevel != 0 || this.parent.Var_d20a != this.parent.GameData.HumanPlayerID)
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
						this.parent.StartGameMenu.F5_0000_0e6c_TestIfAIPlayerIsDestroyed(this.parent.Var_d20a, playerID);

						if (playerID == this.parent.GameData.HumanPlayerID)
						{
							activeUnitID = this.parent.MapManagement.F0_2aea_1458_GetCellActiveUnitID(newX, newY);

							if (activeUnitID != -1)
							{
								this.parent.GameData.Players[this.parent.Var_d20a].Units[activeUnitID].VisibleByPlayer |=
									(ushort)(1 << this.parent.GameData.HumanPlayerID);
							}
						}
					}

					if (playerID == this.parent.GameData.HumanPlayerID ||
						this.parent.Var_d20a == this.parent.GameData.HumanPlayerID ||
						this.parent.Var_d806_DebugFlag)
					{
						this.parent.GameData.Cities[cityID].VisibleSize = this.parent.GameData.Cities[cityID].ActualSize;

						// Instruction address 0x1403:0x2606, size: 5
						this.parent.MapManagement.F0_2aea_11d4_DrawCellWithUnit(newX, newY);
					}

					if (this.parent.MapManagement.F0_2aea_14e0_GetCellUnitPlayerID(newX, newY) == -1)
					{
						if (this.parent.Var_d20a != this.parent.GameData.HumanPlayerID)
						{
							// Instruction address 0x1403:0x2641, size: 5
							this.parent.AIEngine.F0_25fb_3459_PlayerChangeCityProductionForSameContinent(this.parent.Var_d20a, this.parent.MapManagement.F0_2aea_1942_GetGroupID(newX, newY));
						}
					}
					goto Label722;
				}

				if (unit.UnitType != UnitTypeEnum.None && this.parent.GameData.Units[(int)unit.UnitType].MovementType == UnitMovementTypeEnum.Air &&
					newTerrainType != TerrainTypeEnum.Water && !newImprovements.HasFlag(TerrainImprovementFlagsEnum.City))
				{
					unit.GoToDestination = OpenCivOneGame.InvalidPosition;
					goto Label722;
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
					goto Label722;
				}

				if (unit.RemainingMoves == 0)
				{
					goto Label722;
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
							goto Label722;
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

								goto Label722;
							}
						}
					}
					unit.GoToNextDirection = (short)newMoveDirection;
				}

				if (activeUnitID != -1 && playerID == this.parent.Var_d20a &&
					playerID != this.parent.GameData.HumanPlayerID && !newImprovements.HasFlag(TerrainImprovementFlagsEnum.City))
				{
					if (this.parent.GameData.Units[(int)unit.UnitType].MovementType == UnitMovementTypeEnum.Land ||
						(this.parent.GameData.Units[(int)unit.UnitType].MovementType == UnitMovementTypeEnum.Water &&
							unit.SpecialMoves != 0))
					{
						if (((unit.RemainingMoves != 0) ? 4 : 2) <= this.parent.UnitManagement.F0_1866_1251_GetStackUnitValueSum(playerID, activeUnitID, UnitStackValueTypeEnum.UnitCount))
							goto Label722;
					}
				}

				// Get playerID that owns this land
				int ownerPlayerID = this.parent.MapManagement.GetPlayerLandOwnership(newX, newY);

				if (ownerPlayerID != 0)
				{
					if (ownerPlayerID < 8)
					{
						if (newTerrainType != TerrainTypeEnum.Water && terrainType != TerrainTypeEnum.Water &&
							(unit.UnitType != UnitTypeEnum.Diplomat && unit.UnitType != UnitTypeEnum.Caravan) &&
							(newImprovements.HasFlag(TerrainImprovementFlagsEnum.City) || newImprovements.HasFlag(TerrainImprovementFlagsEnum.Irrigation) ||
							newImprovements.HasFlag(TerrainImprovementFlagsEnum.Mines) || newImprovements.HasFlag(TerrainImprovementFlagsEnum.Road)) && ownerPlayerID != playerID)
						{
							if ((playerID == this.parent.GameData.HumanPlayerID || ownerPlayerID == this.parent.GameData.HumanPlayerID) &&
								this.parent.GameData.Units[(int)unit.UnitType].MovementType == UnitMovementTypeEnum.Land &&
								this.parent.MapManagement.F0_2aea_14e0_GetCellUnitPlayerID(newX, newY) != playerID)
							{
								if (playerID == this.parent.GameData.HumanPlayerID)
								{
									unit.GoToDestination = OpenCivOneGame.InvalidPosition;

									if ((short)this.parent.Segment_29f3.F0_29f3_0c9e_ConfirmAttackAction(ownerPlayerID) != -1)
									{
										// Instruction address 0x1403:0x2a87, size: 5
										this.parent.Segment_2517.F0_2517_0aa1_ClearDiplomacyFlags(playerID, ownerPlayerID, DiplomacyFlagsEnum.Peace);
									}
									else
									{
										goto Label722;
									}
								}
								else if (ownerPlayerID == this.parent.GameData.HumanPlayerID &&
									this.parent.GameData.Players[ownerPlayerID].Diplomacy[playerID].HasFlag(DiplomacyFlagsEnum.Peace))
								{
									unit.RemainingMoves = 0;

									goto Label722;
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
					this.parent.GameData.Cities[this.parent.Tools.F0_2dc4_0102_FindNearestCity(unit.Position.X, unit.Position.Y)].PlayerID == this.parent.GameData.HumanPlayerID)
				{
					unit.VisibleByPlayer |= this.parent.GameData.MapVisibility[unit.Position.X, unit.Position.Y];
				}

				if (newImprovements.HasFlag(TerrainImprovementFlagsEnum.City) && cityPlayerID1 != playerID &&
					(this.parent.GameData.MapVisibility[newX, newY] & (0x1 << this.parent.GameData.HumanPlayerID)) != 0 &&
					(this.parent.GameData.Players[this.parent.GameData.HumanPlayerID].Diplomacy[playerID].HasFlag(DiplomacyFlagsEnum.Unknown40) ||
						this.parent.GameData.Players[this.parent.GameData.HumanPlayerID].Diplomacy[cityPlayerID1].HasFlag(DiplomacyFlagsEnum.Unknown40)))
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
					playerID == this.parent.GameData.HumanPlayerID || (this.parent.Var_d806_DebugFlag && playerID != 0))
				{
					if (!this.parent.GameData.GameSettingFlags.EnemyMoves && playerID != this.parent.GameData.HumanPlayerID)
					{
						// Instruction address 0x1403:0x2cd5, size: 5
						this.parent.MapManagement.F0_2aea_03ba_DrawCell(unit.Position.X, unit.Position.Y);
					}
					else
					{
						// Instruction address 0x1403:0x2cb3, size: 5
						this.parent.UnitManagement.F0_1866_16a9_CenterMap(this.parent.GameData.HumanPlayerID, newX, newY);
						// Instruction address 0x1403:0x2cc4, size: 5
						this.parent.UnitManagement.F0_1866_1d55_AnimateUnitMove(playerID, unitID, newMoveDirection);
					}
				}

				// Instruction address 0x1403:0x2d01, size: 5
				this.parent.MapManagement.F0_2aea_1412_SetCellActivePlayerID(unit.Position.X, unit.Position.Y, playerID, unitID);

				// Instruction address 0x1403:0x2d21, size: 5
				this.parent.MapManagement.F0_2aea_13cb_SetCellPlayerID(newX, newY, playerID, unitID);

				unit.Position = new(newX, newY);
				unit.VisibleByPlayer = 0;

				if (playerID != this.parent.GameData.HumanPlayerID &&
					this.parent.GameData.Units[(int)unit.UnitType].MovementType != UnitMovementTypeEnum.Water)
				{
					unit.Status &= 0x30;
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

				if (terrainType == TerrainTypeEnum.Water && newTerrainType != TerrainTypeEnum.Water &&
					this.parent.GameData.Units[(int)unit.UnitType].MovementType != UnitMovementTypeEnum.Water)
				{
					unit.RemainingMoves = 0;
				}

				int nextUnitID = unit.NextUnitID;

				if (nextUnitID != -1 && this.parent.GameData.Units[(int)unit.UnitType].TransportCapacity == 0 &&
					unit.UnitType == UnitTypeEnum.Carrier)
				{
					int neededCapacity = 0;

					for (int i = 0; i < 20; i++)
					{
						int nextUnitID1 = this.parent.GameData.Players[playerID].Units[nextUnitID].NextUnitID;

						if (((this.parent.GameData.Units[(int)unit.UnitType].TransportCapacity < 1) ? UnitMovementTypeEnum.Water : UnitMovementTypeEnum.Land) ==
							this.parent.GameData.Units[(int)this.parent.GameData.Players[playerID].Units[nextUnitID].UnitType].MovementType)
						{
							if (((this.parent.GameData.Players[playerID].Units[nextUnitID].Status & 0x1) == 0 && terrainType == TerrainTypeEnum.Water) ||
								(playerID != this.parent.GameData.HumanPlayerID &&
								(this.parent.GameData.Players[playerID].Units[nextUnitID].Status & 0xc) == 0 &&
								this.parent.GameData.Players[playerID].Units[nextUnitID].NextUnitID != -1))
							{
								// Instruction address 0x1403:0x2efb, size: 5
								this.parent.MapManagement.F0_2aea_1412_SetCellActivePlayerID(unit.Position.X, unit.Position.Y, playerID, nextUnitID);
								// Instruction address 0x1403:0x2f0f, size: 5
								this.parent.MapManagement.F0_2aea_13cb_SetCellPlayerID(newX, newY, playerID, nextUnitID);

								this.parent.GameData.Players[playerID].Units[nextUnitID].Position = new(newX, newY);
								this.parent.GameData.Players[playerID].Units[nextUnitID].VisibleByPlayer = 0;
								this.parent.GameData.Players[playerID].Units[nextUnitID].GoToNextDirection = -1;

								if (this.parent.GameData.Units[(int)unit.UnitType].TransportCapacity != 0)
								{
									this.parent.GameData.Players[playerID].Units[nextUnitID].Status |= 0x1;
								}

								if (this.parent.GameData.Units[(int)this.parent.GameData.Players[playerID].Units[nextUnitID].UnitType].TurnsOutside != 0)
								{
									this.parent.GameData.Players[playerID].Units[nextUnitID].SpecialMoves =
										(short)(this.parent.GameData.Units[(int)this.parent.GameData.Players[playerID].Units[nextUnitID].UnitType].TurnsOutside - 1);
								}

								neededCapacity++;
							}
						}

						nextUnitID = nextUnitID1;

						if (nextUnitID == -1 || (this.parent.GameData.Units[(int)unit.UnitType].TransportCapacity <= neededCapacity &&
							unit.UnitType != UnitTypeEnum.Carrier && playerID != 0))
							break;
					}
				}

				if (newImprovements.HasFlag(TerrainImprovementFlagsEnum.City) && cityPlayerID1 != playerID)
				{
					unit.VisibleByPlayer |= (ushort)(1 << cityPlayerID1);

					// Instruction address 0x1403:0x303e, size: 5
					this.parent.Segment_2459.F0_2459_0000_CityTakeover(playerID, this.parent.Tools.F0_2dc4_00ba_GetCityByLocation(newX, newY), false);
					// Instruction address 0x1403:0x304c, size: 5
					this.parent.MapManagement.F0_2aea_1511_ActiveUnitSetFlag8(newX, newY);
				}

				// Instruction address 0x1403:0x305d, size: 5
				this.parent.MapManagement.F0_2aea_138c_SetCityOwner(newX, newY, playerID);
				// Instruction address 0x1403:0x3079, size: 5
				this.parent.UnitManagement.F0_1866_01dc(newX, newY, playerID, unitID, true);

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
					this.parent.UnitManagement.F0_1866_1560_UnitStack(playerID, unitID);
				}

				if (playerID == 0 && newTerrainType != TerrainTypeEnum.Water)
				{
					if ((this.parent.GameData.MapVisibility[newX, newY] & (0x1 << this.parent.GameData.HumanPlayerID)) != 0 &&
						(unit.VisibleByPlayer & (0x1 << this.parent.GameData.HumanPlayerID)) == 0 &&
						this.parent.GameData.Cities[this.parent.Tools.F0_2dc4_0102_FindNearestCity(newX, newY)].PlayerID == this.parent.GameData.HumanPlayerID)
					{
						unit.VisibleByPlayer |= this.parent.GameData.MapVisibility[newX, newY];
					}
				}

				if ((unit.VisibleByPlayer & (0x1 << this.parent.GameData.HumanPlayerID)) != 0 ||
					playerID == this.parent.GameData.HumanPlayerID || this.parent.Var_d806_DebugFlag && playerID != 0)
				{
					if (playerID == this.parent.GameData.HumanPlayerID || this.parent.GameData.GameSettingFlags.EnemyMoves)
					{
						// Instruction address 0x1403:0x31e6, size: 5
						this.parent.UnitManagement.F0_1866_16a9_CenterMap(this.parent.GameData.HumanPlayerID, newX, newY);
						// Instruction address 0x1403:0x31f4, size: 5
						this.parent.MapManagement.F0_2aea_0e29_DrawUnit(playerID, unitID);

						if (playerID != this.parent.GameData.HumanPlayerID)
						{
							// Instruction address 0x1403:0x3215, size: 5
							this.parent.CommonTools.WaitTimer(30);
						}
					}
					else
					{
						if (this.parent.MapManagement.F0_2aea_0e29_DrawUnit(playerID, unitID))
						{
							// Instruction address 0x1403:0x323a, size: 5
							this.parent.CommonTools.WaitTimer(10);
						}
					}
				}

				local_a = 0;

				if (newImprovements.HasFlag(TerrainImprovementFlagsEnum.City) && this.parent.GameData.Units[(int)unit.UnitType].TurnsOutside != 0)
				{
					unit.Status &= 0xfd;
					unit.RemainingMoves = 0;
					unit.GoToDestination = OpenCivOneGame.InvalidPosition;
				}

				if (this.parent.GameData.Units[(int)unit.UnitType].TurnsOutside != 0)
				{
					if (this.parent.GameData.Units[(int)unit.UnitType].MovementType == UnitMovementTypeEnum.Water)
					{
						if (this.parent.UnitManagement.F0_1866_1331_CountUnitTypesInStack(playerID, unitID, UnitTypeEnum.Carrier) != 0)
						{
							unit.Status &= 0xfd;
							unit.RemainingMoves = 0;
							unit.SpecialMoves = this.parent.GameData.Units[(int)unit.UnitType].TurnsOutside;
						}
					}
				}
			}
		#endregion

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
							bool flag8 = false;

							for (int i = 1; i < 9; i++)
							{
								direction = this.parent.MoveDirections[i];

								if (this.parent.MapManagement.GetTerrainType(
									this.parent.GameData.Players[playerID].Units[unitID].Position.X + direction.X,
									this.parent.GameData.Players[playerID].Units[unitID].Position.Y + direction.Y) != TerrainTypeEnum.Water)
								{
									flag8 = true;
									break;
								}
							}

							if (!flag8)
							{
								// Instruction address 0x1403:0x3e18, size: 5
								this.parent.UnitManagement.F0_1866_0f10_DeleteUnit(playerID, unitID);
								// Instruction address 0x1403:0x3e24, size: 5
								this.parent.Help.F4_0000_03aa_ShowInstantWarningPopup("*TRIREME");
							}
						}
					}
				}
			}
			else if ((this.parent.GameData.PlayerFlags & (0x1 << playerID)) != 0)
			{
				// Instruction address 0x1403:0x3e43, size: 5
				F0_1403_4060(playerID, unitID);
			}

			goto Label18;

		Label748:
			if (!flag2 && this.parent.Var_dc48_GameEndType == 0) goto Label17;

			if (this.parent.Var_dc48_GameEndType == 0 &&
				(this.parent.GameData.PlayerFlags & (0x1 << playerID)) != 0 &&
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
				this.parent.Segment_1238.F0_1238_1bb2_FillRectangleWithShadow(0, 97, 80, 103);
			}
		}

		/// <summary>
		/// ?
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		public void F0_1403_3ed7(int x, int y)
		{
			// This function is referenced 7 time(s)
			// Standard C frame
			//this.oCPU.Log.EnterBlock("'Fn2'(Cdecl, Far, Return) at 0x3ed7");

			// function body
			if ((this.parent.GameData.MapVisibility[x, y] & (1 << this.parent.GameData.HumanPlayerID)) != 0 || this.parent.Var_d806_DebugFlag)
			{
				// Instruction address 0x1403:0x3f09, size: 5
				this.parent.MapManagement.F0_2aea_11d4_DrawCellWithUnit(x, y);
			}
		}

		/// <summary>
		/// ?
		/// </summary>
		/// <param name="playerID"></param>
		/// <param name="unitID"></param>
		private void F0_1403_3f13(int playerID, int unitID)
		{
			// This function is referenced 13 time(s)
			// Standard C frame
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
			// This function is referenced 2 time(s)
			// Standard C frame
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
		/// ?
		/// </summary>
		/// <param name="playerID"></param>
		/// <param name="unitID"></param>
		public void F0_1403_4060(int playerID, int unitID)
		{
			// This function is referenced 4 time(s)
			// Standard C frame
			//this.oCPU.Log.EnterBlock("'Fn6'(Cdecl, Far, Return) at 0x4060");

			// Local variables
			int Local_2;
			int Local_4;
			int Local_6;
			int Local_8;
			int Local_a;
			int Local_10;
			int Local_12;

			// function body
			// Instruction address 0x1403:0x407c, size: 5
			this.parent.Segment_1238.F0_1238_1bb2_FillRectangleWithShadow(0, 97, 80, 103);

			if (unitID == 128)
			{
				// Instruction address 0x1403:0x409a, size: 5
				this.parent.DrawTools.F0_1182_005c_DrawStringToScreen0("End of Turn", 4, 124, 0);
				// Instruction address 0x1403:0x40b1, size: 5
				this.parent.DrawTools.F0_1182_005c_DrawStringToScreen0("Press Enter", 4, 136, 0);
				// Instruction address 0x1403:0x44f5, size: 5
				this.parent.DrawTools.F0_1182_005c_DrawStringToScreen0("to continue", 4, 144, 0);
			}
			else
			{
				if (this.parent.GameData.Players[playerID].Units[unitID].UnitType != UnitTypeEnum.None)
				{
					Local_a = 99;

					// Instruction address 0x1403:0x40f3, size: 5
					this.parent.DrawTools.F0_1182_005c_DrawStringToScreen0(this.parent.GameData.Players[playerID].Nationality, 4, 99, 0);

					Local_a += 8;

					// Instruction address 0x1403:0x4133, size: 5
					this.parent.DrawTools.F0_1182_005c_DrawStringToScreen0(this.parent.GameData.Units[(int)this.parent.GameData.Players[playerID].Units[unitID].UnitType].Name, 
						4, Local_a, 0);

					Local_a += 8;

					if ((this.parent.GameData.Players[playerID].Units[unitID].Status & 0x20) != 0)
					{
						// Instruction address 0x1403:0x4154, size: 5
						this.parent.DrawTools.F0_1182_005c_DrawStringToScreen0("Veteran", 8, Local_a, 0);

						Local_a += 8;
					}

					int Local_e = this.parent.Tools.F0_2dc4_007c_CheckValueRange(this.parent.GameData.Players[playerID].Units[unitID].RemainingMoves / 3, 0, 99);
					int Local_e1 = Local_e + ((this.parent.GameData.Units[(int)this.parent.GameData.Players[playerID].Units[unitID].UnitType].TurnsOutside != 0) ?
					(this.parent.GameData.Units[(int)this.parent.GameData.Players[playerID].Units[unitID].UnitType].MoveCount *
							this.parent.GameData.Players[playerID].Units[unitID].SpecialMoves) : 0);
					int Local_c = this.parent.GameData.Players[playerID].Units[unitID].RemainingMoves % 3;

					// Instruction address 0x1403:0x4285, size: 5
					this.parent.DrawTools.F0_1182_005c_DrawStringToScreen0($"Moves: {Local_e}{((Local_c != 0) ? $".{Local_c}" : "")}" +
						$"{((Local_e != Local_e1) ? $"({Local_e1})" : "")}", 4, Local_a, 0);

					Local_a += 8;

					// Instruction address 0x1403:0x42c2, size: 5
					this.parent.DrawTools.F0_1182_005c_DrawStringToScreen0(this.parent.Segment_2459.F0_2459_08c6_GetCityName(this.parent.GameData.Players[playerID].Units[unitID].HomeCityID), 4, Local_a, 0);

					Local_a += 8;

					// Instruction address 0x1403:0x4325, size: 5
					this.parent.DrawTools.F0_1182_005c_DrawStringToScreen0($"({this.parent.GameData.Terrains[(int)this.parent.MapManagement.GetTerrainType(
						this.parent.GameData.Players[playerID].Units[unitID].Position.X,
						this.parent.GameData.Players[playerID].Units[unitID].Position.Y)].Name})", 4, Local_a, 0);

					Local_a += 8;

					Local_10 = (int)this.parent.MapManagement.F0_2aea_1585_GetVisibleTerrainImprovements(
						this.parent.GameData.Players[playerID].Units[unitID].Position.X, this.parent.GameData.Players[playerID].Units[unitID].Position.Y);

					if ((Local_10 & 0x10) != 0)
					{
						// Instruction address 0x1403:0x4371, size: 5
						this.parent.DrawTools.F0_1182_005c_DrawStringToScreen0("(RailRoad)", 4, Local_a, 0);

						Local_a += 8;
					}
					else
					{
						if ((Local_10 & 0x8) != 0)
						{
							// Instruction address 0x1403:0x4371, size: 5
							this.parent.DrawTools.F0_1182_005c_DrawStringToScreen0("(Road)", 4, Local_a, 0);

							Local_a += 8;
						}
					}

					if ((Local_10 & 0x2) != 0)
					{
						// Instruction address 0x1403:0x43a6, size: 5
						this.parent.DrawTools.F0_1182_005c_DrawStringToScreen0("(Irrigation)", 4, Local_a, 0);

						Local_a += 8;
					}
					else
					{
						if ((Local_10 & 0x4) != 0)
						{
							// Instruction address 0x1403:0x43a6, size: 5
							this.parent.DrawTools.F0_1182_005c_DrawStringToScreen0("(Mining)", 4, Local_a, 0);

							Local_a += 8;
						}
					}

					if ((Local_10 & 0x40) != 0)
					{
						// Instruction address 0x1403:0x43c6, size: 5
						this.parent.DrawTools.F0_1182_005c_DrawStringToScreen0("(Pollution)", 4, Local_a, 0);

						Local_a += 8;
					}

					Local_a += 4;
					Local_4 = this.parent.GameData.Players[playerID].Units[unitID].NextUnitID;
					Local_8 = 8;

					if (((int)this.parent.MapManagement.F0_2aea_1585_GetVisibleTerrainImprovements(
						this.parent.GameData.Players[playerID].Units[unitID].Position.X,
						this.parent.GameData.Players[playerID].Units[unitID].Position.Y) & 0x1) != 0)
					{
						Local_2 = this.parent.Tools.F0_2dc4_00ba_GetCityByLocation(
							this.parent.GameData.Players[playerID].Units[unitID].Position.X,
							this.parent.GameData.Players[playerID].Units[unitID].Position.Y);

						for (Local_6 = 0; Local_6 < 2; Local_6++)
						{
							Local_12 = this.parent.GameData.Cities[Local_2].Unknown[Local_6];

							if (Local_12 != -1)
							{
								this.parent.GameData.Players[playerID].Units[127].UnitType = (UnitTypeEnum)(Local_12 & 0x3f);
								this.parent.GameData.Players[playerID].Units[127].Status = 8;
								this.parent.GameData.Players[playerID].Units[127].GoToDestination = OpenCivOneGame.InvalidPosition;

								// Instruction address 0x1403:0x4468, size: 5
								this.parent.MapManagement.F0_2aea_0fb3_DrawUnitWithStatus(playerID, 127, Local_8, Local_a);

								this.parent.GameData.Players[playerID].Units[127].UnitType = UnitTypeEnum.None;

								Local_8 += 16;
							}
						}
					}

					while (Local_4 != -1)
					{
						if (Local_4 == unitID || Local_a >= 184)
							break;

						// Instruction address 0x1403:0x449f, size: 5
						this.parent.MapManagement.F0_2aea_0fb3_DrawUnitWithStatus(playerID, Local_4, Local_8, Local_a);

						Local_8 += 16;

						if (Local_8 > 64)
						{
							Local_8 = 8;
							Local_a += 16;
						}

						Local_4 = this.parent.GameData.Players[playerID].Units[Local_4].NextUnitID;
					}

					if (Local_4 != -1 && Local_4 != unitID)
					{
						// Instruction address 0x1403:0x44f5, size: 5
						this.parent.DrawTools.F0_1182_005c_DrawStringToScreen0("+", 74, 192, 0);
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
			// This function is referenced 4 time(s)
			// Standard C frame
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
		/// ?
		/// </summary>
		/// <param name="playerID"></param>
		/// <param name="unitID"></param>
		/// <returns></returns>
		private int F0_1403_4562(int playerID, int unitID)
		{
			// This function is referenced 1 time(s)
			// Standard C frame
			//this.oCPU.Log.EnterBlock("'Fn9'(Cdecl, Far, Return) at 0x4562");

			// Local variables
			int Local_2;
			int Local_4;
			int Local_6;
			int Local_8;
			int Local_a;

			// function body
			Local_6 = this.parent.GameData.Players[playerID].Units[unitID].NextUnitID;

			if (Local_6 == -1)
			{
				return unitID;
			}
			else
			{
				Local_2 = unitID;
				Local_8 = 999;

				for (Local_a = 0; Local_a < 16; Local_a++)
				{
					if (this.parent.GameData.Units[(int)this.parent.GameData.Players[playerID].Units[Local_6].UnitType].MovementType != UnitMovementTypeEnum.Air)
					{
						if (Local_6 > unitID)
						{
							Local_4 = Local_6 - unitID;
						}
						else
						{
							Local_4 = (Local_6 - unitID) + 128;
						}

						if (Local_4 < Local_8)
						{
							Local_8 = Local_4;
							Local_2 = Local_6;
						}
					}

					Local_6 = this.parent.GameData.Players[playerID].Units[Local_6].NextUnitID;

					if (Local_6 == unitID)
						break;
				}

				return Local_2;
			}
		}
	}
}
