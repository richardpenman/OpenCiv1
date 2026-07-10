using IRB.VirtualCPU;
using OpenCivOne.Graphics;
using System.Text;

namespace OpenCivOne
{
	public class CheckPlayerTurn
	{
		private OpenCivOneGame parent;

		public CheckPlayerTurn(OpenCivOneGame parent)
		{
			this.parent = parent;
		}

		/// <summary>
		/// This function handles given Player turn. All unit movements, keyboard and mouse events
		/// </summary>
		/// <param name="playerID"></param>
		public void F0_1403_000e_CheckPlayerTurn(int playerID)
		{
			//this.oCPU.Log.EnterBlock("'Fn1'(Cdecl, Far, Return) at 0xe");
			// Local variables
			int local_a;
			int local_e = -1;
			int local_12 = 0;
			int local_18;
			int local_1c;
			int local_24 = 0;
			int local_26;
			int local_2c = 0;
			int local_30 = 0;
			int local_3a;
			TerrainTypeEnum newTerrainType = TerrainTypeEnum.Invalid;
			TerrainImprovementFlagsEnum newImprovements;
			GPoint direction;

			// function body
			this.parent.Var_6b90 = playerID;

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
			else
			{
				this.parent.Var_6b90 = playerID;
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
			bool flag1 = false;
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
			if (unitID < 128) goto Label20;
			goto Label748;

		Label20:
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
				throw new Exception($"The unit has invalid UnitType of {(int)this.parent.GameData.Players[playerID].Units[unitID].UnitType}, "+
					$"unitID: {unitID}, player {this.parent.GameData.Players[playerID].Name}");

				this.parent.GameData.Players[playerID].Units[unitID].UnitType = UnitTypeEnum.Militia;
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
						local_3a = this.parent.GameTools.F0_2dc4_00ba_GetCityByLocation(this.parent.GameData.Players[playerID].Units[unitID].Position.X,
							this.parent.GameData.Players[playerID].Units[unitID].Position.Y);

						if (this.parent.GameData.Cities[local_3a].ActualSize >= 3)
						{
							this.parent.GameData.Players[playerID].Units[unitID].HomeCityID = (short)local_3a;
						}
					}
				}

				if (this.parent.MapManagement.F0_2aea_1585_GetVisibleTerrainImprovements(this.parent.GameData.Players[playerID].Units[unitID].Position.X,
					this.parent.GameData.Players[playerID].Units[unitID].Position.Y).HasFlag(TerrainImprovementFlagsEnum.City))
				{
					local_3a = this.parent.GameTools.F0_2dc4_00ba_GetCityByLocation(this.parent.GameData.Players[playerID].Units[unitID].Position.X,
						this.parent.GameData.Players[playerID].Units[unitID].Position.Y);

					if (this.parent.GameData.Players[playerID].Units[unitID].HomeCityID == local_3a)
					{
						if (!this.parent.UnitManagement.F0_1866_18d0_IsEnemyUnitNear(playerID, this.parent.GameData.Players[playerID].Units[unitID].Position.X,
							this.parent.GameData.Players[playerID].Units[unitID].Position.Y))
						{
							this.parent.UnitManagement.F0_1866_00c6(local_3a);
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
				this.parent.CommonTools.EmptyKeyboardBufferAndClearMouse();
			}

			local_a = 0;

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
				goto Label152;
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

			this.parent.UnitManagement.F0_1866_0ad6_ShowActiveUnitOrEndOfTurn(playerID, unitID, local_e, local_12);

			if (unitID < 128)
			{
				flag5 = true;
			}

			MouseEvent mouseEvent = this.parent.GetMouseEvent();

			if (mouseEvent.Buttons == MouseButtonsEnum.None && this.parent.CAPI.kbhit() == 0) goto Label79;

			if (mouseEvent.Buttons == MouseButtonsEnum.None && this.parent.CAPI.kbhit() != 0)
			{
				// Instruction address 0x1403:0x0676, size: 5
				command = this.parent.MenuBoxDialog.F0_2d05_0ac9_GetNavigationKey();

				if (command == 0xd && flag1)
				{
					local_1c = local_e;
					local_26 = local_12;
					//this.parent.Var_db3c_MouseXPos = 80;
					//this.parent.Var_db3e_MouseYPos = 8;
					//this.parent.Var_db3a_MouseButton = 1;
					mouseEvent = new MouseEvent(new GPoint(80, 8), MouseButtonsEnum.Left);

					goto Label114;
				}
				goto Label151;
			}

			local_1c = this.parent.MapManagement.AdjustXPosition(((mouseEvent.Position.X - 80) / 16) + this.parent.Var_d4cc_MapViewX);
			local_26 = ((mouseEvent.Position.Y - 8) / 16) + this.parent.Var_d75e_MapViewY;

			if (mouseEvent.Position.Y < 8)
			{
				// Instruction address 0x1403:0x0707, size: 5
				this.parent.GameMenus.F0_2c84_0000_ShowTopMenu(playerID, unitID, -1);

				// Instruction address 0x1403:0x070f, size: 5
				this.parent.CommonTools.EmptyKeyboardBufferAndClearMouse();

				if (this.parent.GameMenus.Var_d4ca_MenuShortcutKey != -1)
				{
					command = this.parent.GameMenus.Var_d4ca_MenuShortcutKey;

					if (unitID < 128) goto Label152;
					goto Label674;
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
							this.parent.GameTools.F0_2dc4_007c_CheckValueRange(mouseEvent.Position.Y - 14 + this.parent.Var_70ea_MiniMapY, 0, 49));
					//}
				}
				else if (mouseEvent.Position.Y < 72)
				{
					this.parent.Palace.F17_0000_07ec(0);

					this.parent.Var_aa_Screen0_Rectangle.ScreenID = 0;

					// Instruction address 0x1403:0x07af, size: 5
					this.parent.GameTools.F0_2dc4_065f_StopPaletteCycleSlots();

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
					this.parent.GameTools.F0_2dc4_0626_StartPaletteCycleSlots();
				}
				else if (unitID >= 128)
				{
					goto Label755;
				}

				//this.parent.Var_db3a_MouseButton = 0;
				mouseEvent = new MouseEvent(mouseEvent.Position, MouseButtonsEnum.None);
			}

			if (mouseEvent.Buttons == MouseButtonsEnum.Right && unitID < 128)
			{
				while (true)
				{
					mouseEvent = this.parent.GetMouseEvent();

					if (mouseEvent.Buttons == MouseButtonsEnum.None)
					{
						// Instruction address 0x1403:0x0882, size: 5
						local_1c = this.parent.MapManagement.AdjustXPosition((mouseEvent.Position.X - 80) / 16 + this.parent.Var_d4cc_MapViewX);
						local_26 = ((mouseEvent.Position.Y - 8) / 16) + this.parent.Var_d75e_MapViewY;

						// Instruction address 0x1403:0x08d0, size: 5
						local_18 = this.parent.GameTools.F0_2dc4_0289_GetShortestDistance(
							local_1c - this.parent.GameData.Players[playerID].Units[unitID].Position.X,
							local_26 - this.parent.GameData.Players[playerID].Units[unitID].Position.Y);

						if (Math.Abs(local_1c - this.parent.GameData.Players[playerID].Units[unitID].Position.X) == 79)
						{
							if (Math.Abs(local_26 - this.parent.GameData.Players[playerID].Units[unitID].Position.Y) <= 1)
							{
								local_18 = 1;
							}
						}

						if (local_18 == 1)
						{
							this.parent.GameData.Players[playerID].Units[unitID].GoToDestination = new(local_1c, local_26);
							this.parent.GameData.Players[playerID].Units[unitID].GoToNextDirection = -1;
							//this.parent.Var_db3a_MouseButton = 0;
							mouseEvent = new MouseEvent(mouseEvent.Position, MouseButtonsEnum.None);
							flag3 = false;

							// Instruction address 0x1403:0x095b, size: 5
							this.parent.CommonTools.SetMousePositionAndIcon(0, 0, (short)this.parent.Array_d4ce[7]);
						}
						else
						{
							//this.parent.Var_db3a_MouseButton = 2;
							mouseEvent = new MouseEvent(mouseEvent.Position, MouseButtonsEnum.Right);
						}
						break;
					}
				}
			}

			if (mouseEvent.Buttons == MouseButtonsEnum.Right)
			{
				if ((this.parent.GameData.MapVisibility[local_1c, local_26] & (0x1 << this.parent.GameData.HumanPlayerID)) != 0 ||
					this.parent.Var_d806_DebugFlag)
				{
					// Instruction address 0x1403:0x09b4, size: 5
					this.parent.Encyclopedia.F8_0000_062a_DisplayEncyclopediaTopic(EncyclopediaTopicEnum.TerrainType, (int)this.parent.MapManagement.GetTerrainType(local_1c, local_26));

					// Instruction address 0x1403:0x09c5, size: 5
					this.parent.Segment_1238.F0_1238_1b44();
				}
			}

		Label114:
			if (mouseEvent.Buttons == MouseButtonsEnum.Left)
			{
				if (flag3 && unitID < 128)
				{
					newTerrainType = this.parent.MapManagement.GetTerrainType(local_1c, local_26);

					// local_36 == 2 || will never happen, because local_36 has only two values 0 and 1
					if ((this.parent.GameData.MapVisibility[local_1c, local_26] & (0x1 << this.parent.GameData.HumanPlayerID)) != 0 &&
						((this.parent.GameData.Units[(int)this.parent.GameData.Players[playerID].Units[unitID].UnitType].MovementType == UnitMovementTypeEnum.Air && newTerrainType == TerrainTypeEnum.Water) ||
						(this.parent.GameData.Units[(int)this.parent.GameData.Players[playerID].Units[unitID].UnitType].MovementType == UnitMovementTypeEnum.Land && newTerrainType != TerrainTypeEnum.Water) ||
						(this.parent.MapManagement.F0_2aea_1585_GetVisibleTerrainImprovements(local_1c, local_26).HasFlag(TerrainImprovementFlagsEnum.City) ||
							this.parent.GameData.Units[(int)this.parent.GameData.Players[playerID].Units[unitID].UnitType].MovementType == UnitMovementTypeEnum.Water)))
					{
						this.parent.GameData.Players[playerID].Units[unitID].GoToDestination = new(local_1c, local_26);
						this.parent.GameData.Players[playerID].Units[unitID].GoToNextDirection = -1;
					}

					flag3 = false;

					this.parent.CommonTools.SetMousePositionAndIcon(0, 0, (short)this.parent.Array_d4ce[7]);

					if (flag1)
					{
						flag1 = false;

						if ((this.parent.GameData.MapVisibility[local_e, local_12] & (0x1 << playerID)) != 0)
						{
							this.parent.MapManagement.F0_2aea_11d4_DrawCellWithUnit(local_e, local_12);
						}

						local_e = -1;
					}
				}
				else
				{
					if (this.parent.MapManagement.F0_2aea_1585_GetVisibleTerrainImprovements(local_1c, local_26).HasFlag(TerrainImprovementFlagsEnum.City) &&
						(this.parent.MapManagement.F0_2aea_1369_GetCityOwner(local_1c, local_26) == this.parent.GameData.HumanPlayerID ||
							this.parent.Var_d806_DebugFlag))
					{
						int cityID = this.parent.GameTools.F0_2dc4_00ba_GetCityByLocation(local_1c, local_26);

						// Instruction address 0x1403:0x0b9a, size: 5
						this.parent.Segment_1ade.F0_1ade_03ea(cityID);
						// Instruction address 0x1403:0x0ba2, size: 5
						this.parent.Segment_1238.F0_1238_107e();

						flag2 = false;

						if (flag1)
						{
							flag1 = false;

							if ((this.parent.GameData.MapVisibility[local_e, local_12] & (0x1 << playerID)) != 0)
							{
								this.parent.MapManagement.F0_2aea_11d4_DrawCellWithUnit(local_e, local_12);
							}

							local_e = -1;
						}
					}
					else
					{
						int activeUnitID = this.parent.MapManagement.F0_2aea_1458_GetCellActiveUnitID(local_1c, local_26);

						if (this.parent.Var_d806_DebugFlag && activeUnitID != -1)
						{
							this.parent.ShowDebugDetails.ShowUnitStatus(this.parent.Var_d7f0, activeUnitID);
						}

						if (activeUnitID != -1 && playerID == this.parent.Var_d7f0)
						{
							local_3a = this.parent.UnitManagement.F0_1866_1f69(this.parent.Var_d7f0, activeUnitID);

							if ((this.parent.GameData.Players[playerID].Units[local_3a].Status & 0x9) == 0)
							{
								unitID = local_3a;
								maxUnitID = local_3a;
								if (maxUnitID < 0)
									maxUnitID = 127;

								flag4 = false;
								flag2 = false;

								if (flag1)
								{
									flag1 = false;

									if ((this.parent.GameData.MapVisibility[local_e, local_12] & (0x1 << playerID)) != 0)
									{
										// Instruction address 0x1403:0x0cb9, size: 5
										this.parent.MapManagement.F0_2aea_11d4_DrawCellWithUnit(local_e, local_12);
									}

									local_e = -1;
								}
								goto Label722;
							}
						}
						else
						{
							// Instruction address 0x1403:0x0cdd, size: 5
							this.parent.MapManagement.F0_2aea_0008_DrawVisibleMap(playerID, local_1c - 7, local_26 - 6);
						}
					}
				}
			}

			command = -1;

			// Instruction address 0x1403:0x0cfb, size: 5
			this.parent.CommonTools.EmptyKeyboardBufferAndClearMouse();

		Label151:
			if (unitID < 128) goto Label152;
			goto Label286;

		Label152:
			if (this.parent.GameData.Players[playerID].Units[unitID].GoToDestination.X != -1)
			{
				int command1 = this.parent.UnitGoTo.GetNextGoToMove(playerID, unitID);

				if (command1 == -1)
				{
					this.parent.GameData.Players[playerID].Units[unitID].GoToDestination = OpenCivOneGame.InvalidPosition;
					this.parent.GameData.Players[playerID].Units[unitID].GoToNextDirection = -1;
				}
				else
				{
					command = command1;
				}
			}
			else
			{
				this.parent.GameData.Players[playerID].Units[unitID].GoToNextDirection = -1;
			}

			if (unitID < 128)
			{
				local_2c = this.parent.GameData.Players[playerID].Units[unitID].Position.X;
				local_30 = this.parent.GameData.Players[playerID].Units[unitID].Position.Y;
			}
			else
			{
				local_2c = 0;
				local_30 = 0;
			}

			int nearestCityID;
			int distanceToObject;

			switch ((char)command)
			{
				case ' ':
					this.parent.GameData.Players[playerID].Units[unitID].RemainingMoves = 0;
					break;

				case 'D':
					this.parent.GameData.Players[playerID].Units[unitID].RemainingMoves = 0;

					// Instruction address 0x1403:0x0d90, size: 5
					this.parent.UnitManagement.F0_1866_0f10_DeleteUnit(playerID, unitID);
					break;

				case 'P':
					if (!this.parent.MapManagement.F0_2aea_1585_GetVisibleTerrainImprovements(local_2c, local_30).HasFlag(TerrainImprovementFlagsEnum.City) &&
						this.parent.GameData.Players[playerID].Units[unitID].UnitType != UnitTypeEnum.Fighter)
					{
						// Instruction address 0x1403:0x0df1, size: 5
						int cityID = this.parent.GameTools.F0_2dc4_0102_FindNearestCity(local_2c, local_30);

						if (this.parent.GameData.Players[playerID].Units[unitID].RemainingMoves != 0)
						{
							if ((short)this.parent.Segment_29f3.F0_29f3_0c9e(this.parent.GameData.Cities[cityID].PlayerID) != -1)
							{
								TerrainImprovementFlagsEnum improvements = this.parent.MapManagement.F0_2aea_1585_GetVisibleTerrainImprovements(local_2c, local_30);

								if (improvements.HasFlag(TerrainImprovementFlagsEnum.Mines) || improvements.HasFlag(TerrainImprovementFlagsEnum.Irrigation))
								{
									this.parent.MapManagement.F0_2aea_16ee_ClearTerrainImprovements(local_2c, local_30, TerrainImprovementFlagsEnum.Mines | TerrainImprovementFlagsEnum.Irrigation);
								}
								else if (improvements.HasFlag(TerrainImprovementFlagsEnum.RailRoad))
								{
									this.parent.MapManagement.F0_2aea_16ee_ClearTerrainImprovements(local_2c, local_30, TerrainImprovementFlagsEnum.RailRoad | TerrainImprovementFlagsEnum.Irrigation | TerrainImprovementFlagsEnum.Mines);
								}
								else
								{
									this.parent.MapManagement.F0_2aea_16ee_ClearTerrainImprovements(local_2c, local_30, TerrainImprovementFlagsEnum.Road);
								}

								if (this.parent.GameData.Cities[cityID].PlayerID == this.parent.GameData.HumanPlayerID)
								{
									this.parent.MapManagement.F0_2aea_1601_UpdateVisibleCellStatus(local_2c, local_30);
								}

								F0_1403_3ed7(local_2c, local_30);
								this.parent.Segment_2517.F0_2517_0aa1_ClearDiplomacyFlags(this.parent.GameData.HumanPlayerID,
									this.parent.GameData.Cities[cityID].PlayerID, DiplomacyFlagsEnum.Peace);

								int local_54 = this.parent.GameData.Cities[cityID].PlayerID;

								if (playerID != local_54)
								{
									this.parent.AIEngine.PlayerAddUnitPolicy(local_54, local_2c, local_30, UnitRoleTypeEnum.LandAttack, 4);
								}

								this.parent.GameData.Players[playerID].Units[unitID].RemainingMoves = 0;
							}
						}
					}
					else
					{
						// Instruction address 0x1403:0x0ddd, size: 5
						this.parent.Help.F4_0000_03aa_ShowInstantWarningPopup("*PILLAGE");
					}
					break;

				case 'b':
					if (this.parent.GameData.Players[playerID].Units[unitID].UnitType != 0)
					{
						// Instruction address 0x1403:0x19dd, size: 5
						this.parent.Help.F4_0000_03aa_ShowInstantWarningPopup("*SETTLERS");
					}
					else if (this.parent.MapManagement.GetTerrainType(local_2c, local_30) != TerrainTypeEnum.Water && local_30 >= 2 && local_30 < 48)
					{
						if (this.parent.MapManagement.F0_2aea_1585_GetVisibleTerrainImprovements(local_2c, local_30).HasFlag(TerrainImprovementFlagsEnum.City))
						{
							int cityID = this.parent.MapManagement.F0_2aea_175a_FindCityHumanPlayer(local_2c, local_30);

							if (this.parent.GameData.Cities[cityID].ActualSize < 10)
							{
								this.parent.GameData.Cities[cityID].ActualSize++;
								this.parent.GameData.Players[playerID].Units[unitID].RemainingMoves = 0;

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
							this.parent.GameData.Players[playerID].Units[unitID].RemainingMoves = 0;

							local_3a = (short)this.parent.Overlay_20.F20_0000_0000(playerID, local_2c, local_30, 1);

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

				case 'f':
					if (this.parent.GameData.Units[(int)this.parent.GameData.Players[playerID].Units[unitID].UnitType].MovementType == UnitMovementTypeEnum.Land)
					{
						if (this.parent.GameData.Players[playerID].Units[unitID].UnitType != 0)
						{
							this.parent.GameData.Players[playerID].Units[unitID].Status |= 0x4;
							this.parent.GameData.Players[playerID].Units[unitID].RemainingMoves = 0;

							// Instruction address 0x1403:0x12bf, size: 5
							F0_1403_3f13(playerID, unitID);

							if (playerID != this.parent.GameData.HumanPlayerID &&
								this.parent.MapManagement.F0_2aea_1585_GetVisibleTerrainImprovements(local_2c, local_30).HasFlag(TerrainImprovementFlagsEnum.City))
							{
								this.parent.GameData.Players[playerID].Units[unitID].HomeCityID = (short)this.parent.GameTools.F0_2dc4_00ba_GetCityByLocation(local_2c, local_30);
							}
						}
						else
						{
							if (this.parent.MapManagement.F0_2aea_1585_GetVisibleTerrainImprovements(local_2c, local_30).HasFlag(TerrainImprovementFlagsEnum.City) ||
								this.parent.MapManagement.F0_2aea_1585_GetVisibleTerrainImprovements(local_2c, local_30).HasFlag(TerrainImprovementFlagsEnum.Fortress) ||
								!this.parent.Segment_1ade.F0_1ade_22b5_PlayerHasTechnology(playerID, TechnologyAdvanceEnum.Construction))
							{
								this.parent.GameData.Players[playerID].Units[unitID].Status &= 0x3f;
								this.parent.GameData.Players[playerID].Units[unitID].SpecialMoves = 0;

								this.parent.UnitManagement.F0_1866_16a9(playerID, local_2c, local_30);
							}
							else
							{
								this.parent.GameData.Players[playerID].Units[unitID].Status |= 0xc0;
								this.parent.GameData.Players[playerID].Units[unitID].RemainingMoves = 0;

								// Instruction address 0x1403:0x1388, size: 5
								F0_1403_3f13(playerID, unitID);

								this.parent.GameData.Players[playerID].Units[unitID].SpecialMoves++;
								newTerrainType = this.parent.MapManagement.GetTerrainType(local_2c, local_30);

								if ((this.parent.GameData.Terrains[(int)newTerrainType].MovementCost + 4) <= this.parent.GameData.Players[playerID].Units[unitID].SpecialMoves)
								{
									// Instruction address 0x1403:0x13bf, size: 5
									this.parent.MapManagement.F0_2aea_1653_SetTerrainImprovements(local_2c, local_30, TerrainImprovementFlagsEnum.Fortress);

									// Instruction address 0x1403:0x13db, size: 5
									this.parent.UnitManagement.F0_1866_01dc(local_2c, local_30, playerID, unitID, true);

									this.parent.GameData.Players[playerID].Units[unitID].Status &= 0x3f;
									this.parent.GameData.Players[playerID].Units[unitID].SpecialMoves = 0;
								}
							}
						}
					}
					break;

				case 'g':
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

				case 'h':
					nearestCityID = this.parent.GameTools.F0_2dc4_0102_FindNearestCity(local_2c, local_30);

					if (nearestCityID != -1 &&
						(distanceToObject = this.parent.GameTools.F0_2dc4_0289_GetShortestDistance(local_2c, local_30, this.parent.GameData.Cities[nearestCityID].Position)) == 0)
					{
						this.parent.GameData.Players[playerID].Units[unitID].HomeCityID = (short)nearestCityID;
					}
					else if (this.parent.GameData.Units[(int)this.parent.GameData.Players[playerID].Units[unitID].UnitType].MovementType == UnitMovementTypeEnum.Water)
					{
						// Instruction address 0x1403:0x1118, size: 5
						local_24 = this.parent.UnitManagement.F0_1866_226d(playerID, unitID);

						if (local_24 != 0)
						{
							this.parent.GameData.Players[playerID].Units[unitID].Status |= 2;
							goto Label300;
						}
						else
						{
							if (playerID == this.parent.GameData.HumanPlayerID)
							{
								this.parent.GameData.Players[playerID].Units[unitID].Status &= 0xfd;
							}
							else
							{
								this.parent.GameData.Players[playerID].Units[unitID].RemainingMoves = 0;

								// Instruction address 0x1403:0x1175, size: 5
								this.parent.UnitManagement.F0_1866_0f10_DeleteUnit(playerID, unitID);
							}
						}
					}
					break;

				case 'i':
					if (this.parent.GameData.Players[playerID].Units[unitID].UnitType != 0 ||
						(this.parent.GameData.DebugFlags & 0x2) == 0 ||
						this.parent.MapManagement.F0_2aea_1585_GetVisibleTerrainImprovements(local_2c, local_30).HasFlag(TerrainImprovementFlagsEnum.Irrigation))
					{
						// Instruction address 0x1403:0x15ce, size: 5
						this.parent.UnitManagement.F0_1866_16a9(playerID, local_2c, local_30);

						if (this.parent.GameData.Players[playerID].Units[unitID].UnitType != 0)
						{
							this.parent.Help.F4_0000_03aa_ShowInstantWarningPopup("*SETTLERS");
						}
						else
						{
							this.parent.GameData.Players[playerID].Units[unitID].SpecialMoves = 0;
						}

						this.parent.GameData.Players[playerID].Units[unitID].Status &= 0xbf;
					}
					else
					{
						newTerrainType = this.parent.MapManagement.GetTerrainType(local_2c, local_30);
						local_3a = this.parent.GameData.TerrainModifications[(int)newTerrainType].IrrigationEffect;

						if (local_3a == -1)
						{
							// Instruction address 0x1403:0x165e, size: 5
							this.parent.UnitManagement.F0_1866_16a9(playerID, local_2c, local_30);
							this.parent.Help.F4_0000_03aa_ShowInstantWarningPopup("*NOIRR");

							this.parent.GameData.Players[playerID].Units[unitID].Status &= 0xbf;
						}
						else
						{
							if (local_3a != -2 || this.parent.MapManagement.CanIrrigateCell(local_2c, local_30))
							{
								this.parent.GameData.Players[playerID].Units[unitID].Status |= 0x40;

								// Instruction address 0x1403:0x16f9, size: 5
								F0_1403_3f13(playerID, unitID);

								this.parent.GameData.Players[playerID].Units[unitID].RemainingMoves = 0;
								this.parent.GameData.Players[playerID].Units[unitID].GoToDestination = OpenCivOneGame.InvalidPosition;
								this.parent.GameData.Players[playerID].Units[unitID].GoToNextDirection = -1;
								this.parent.GameData.Players[playerID].Units[unitID].SpecialMoves++;

								if (this.parent.GameData.TerrainModifications[(int)newTerrainType].IrrigationCost <= this.parent.GameData.Players[playerID].Units[unitID].SpecialMoves)
								{
									if (local_3a >= 0)
									{
										// Instruction address 0x1403:0x1743, size: 5
										this.parent.MapManagement.SetTerrainType(local_2c, local_30, this.parent.PixelValuesToTerrainTypes[local_3a]);

										this.parent.GameData.MapVisibility[local_2c, local_30] |= 1;

										// Instruction address 0x1403:0x176a, size: 5
										this.parent.MapManagement.F0_2aea_16ee_ClearTerrainImprovements(local_2c, local_30, TerrainImprovementFlagsEnum.Irrigation | TerrainImprovementFlagsEnum.Mines);
									}
									else
									{
										// Instruction address 0x1403:0x177f, size: 5
										this.parent.MapManagement.F0_2aea_16ee_ClearTerrainImprovements(local_2c, local_30, TerrainImprovementFlagsEnum.Mines);
										// Instruction address 0x1403:0x1791, size: 5
										this.parent.MapManagement.F0_2aea_1653_SetTerrainImprovements(local_2c, local_30, TerrainImprovementFlagsEnum.Irrigation);
									}

									// Instruction address 0x1403:0x179f, size: 5
									F0_1403_3f13(playerID, unitID);

									this.parent.GameData.Players[playerID].Units[unitID].Status &= 0xbf;
									this.parent.GameData.Players[playerID].Units[unitID].SpecialMoves = 0;
								}
							}
							else
							{
								// Instruction address 0x1403:0x16b2, size: 5
								this.parent.UnitManagement.F0_1866_16a9(playerID, local_2c, local_30);
								// Instruction address 0x1403:0x16be, size: 5
								this.parent.Help.F4_0000_03aa_ShowInstantWarningPopup("*NOWATER");

								this.parent.GameData.Players[playerID].Units[unitID].Status &= 0xbf;
							}
						}
					}
					break;

				case 'm':
					if (this.parent.GameData.Players[playerID].Units[unitID].UnitType != 0 ||
						this.parent.MapManagement.F0_2aea_1585_GetVisibleTerrainImprovements(local_2c, local_30).HasFlag(TerrainImprovementFlagsEnum.Mines))
					{
						// Instruction address 0x1403:0x17fc, size: 5
						this.parent.UnitManagement.F0_1866_16a9(playerID, local_2c, local_30);

						if (this.parent.GameData.Players[playerID].Units[unitID].UnitType != 0)
						{
							// Instruction address 0x1403:0x1822, size: 5
							this.parent.Help.F4_0000_03aa_ShowInstantWarningPopup("*SETTLERS");
						}
						else
						{
							this.parent.GameData.Players[playerID].Units[unitID].SpecialMoves = 0;
						}

						this.parent.GameData.Players[playerID].Units[unitID].Status &= 0x7f;
					}
					else
					{
						newTerrainType = this.parent.MapManagement.GetTerrainType(local_2c, local_30);
						local_3a = this.parent.GameData.TerrainModifications[(int)newTerrainType].MiningEffect;

						if (local_3a == -1)
						{
							// Instruction address 0x1403:0x188c, size: 5
							this.parent.UnitManagement.F0_1866_16a9(playerID, local_2c, local_30);
							// Instruction address 0x1403:0x1898, size: 5
							this.parent.Help.F4_0000_03aa_ShowInstantWarningPopup("*NOMINE");

							this.parent.GameData.Players[playerID].Units[unitID].Status &= 0x7f;
						}
						else
						{
							this.parent.GameData.Players[playerID].Units[unitID].Status |= 0x80;

							// Instruction address 0x1403:0x18d3, size: 5
							F0_1403_3f13(playerID, unitID);

							this.parent.GameData.Players[playerID].Units[unitID].RemainingMoves = 0;
							this.parent.GameData.Players[playerID].Units[unitID].GoToDestination = OpenCivOneGame.InvalidPosition;
							this.parent.GameData.Players[playerID].Units[unitID].GoToNextDirection = -1;
							this.parent.GameData.Players[playerID].Units[unitID].SpecialMoves++;

							if (this.parent.GameData.TerrainModifications[(int)newTerrainType].MiningCost <= this.parent.GameData.Players[playerID].Units[unitID].SpecialMoves)
							{
								if (local_3a >= 0)
								{
									this.parent.GameData.Players[playerID].Units[unitID].SpecialMoves++;

									if (this.parent.GameData.Players[playerID].Units[unitID].SpecialMoves > 5)
									{
										// Instruction address 0x1403:0x193e, size: 5
										this.parent.MapManagement.SetTerrainType(local_2c, local_30, this.parent.PixelValuesToTerrainTypes[local_3a]);

										this.parent.GameData.MapVisibility[local_2c, local_30] |= 1;

										// Instruction address 0x1403:0x1965, size: 5
										this.parent.MapManagement.F0_2aea_16ee_ClearTerrainImprovements(local_2c, local_30, TerrainImprovementFlagsEnum.Mines | TerrainImprovementFlagsEnum.Irrigation);
										// Instruction address 0x1403:0x199a, size: 5
										F0_1403_3f13(playerID, unitID);

										this.parent.GameData.Players[playerID].Units[unitID].Status &= 0x7f;
										this.parent.GameData.Players[playerID].Units[unitID].SpecialMoves = 0;
									}
								}
								else
								{
									// Instruction address 0x1403:0x197a, size: 5
									this.parent.MapManagement.F0_2aea_16ee_ClearTerrainImprovements(local_2c, local_30, TerrainImprovementFlagsEnum.Irrigation);
									// Instruction address 0x1403:0x198c, size: 5
									this.parent.MapManagement.F0_2aea_1653_SetTerrainImprovements(local_2c, local_30, TerrainImprovementFlagsEnum.Mines);
									// Instruction address 0x1403:0x199a, size: 5
									F0_1403_3f13(playerID, unitID);

									this.parent.GameData.Players[playerID].Units[unitID].Status &= 0x7f;
									this.parent.GameData.Players[playerID].Units[unitID].SpecialMoves = 0;
								}
							}
						}
					}
					break;

				case 'p':
					if (this.parent.GameData.Players[playerID].Units[unitID].UnitType != 0)
					{
						// Instruction address 0x1403:0x119e, size: 5
						this.parent.Help.F4_0000_03aa_ShowInstantWarningPopup("*SETTLERS");
					}
					else
					{
						newImprovements = this.parent.MapManagement.F0_2aea_1585_GetVisibleTerrainImprovements(local_2c, local_30);

						if (!newImprovements.HasFlag(TerrainImprovementFlagsEnum.Pollution))
						{
							this.parent.GameData.Players[playerID].Units[unitID].Status &= 0x7d;
							this.parent.GameData.Players[playerID].Units[unitID].SpecialMoves = 0;

							// Instruction address 0x1403:0x11e6, size: 5
							this.parent.UnitManagement.F0_1866_16a9(playerID, local_2c, local_30);
						}
						else
						{
							this.parent.GameData.Players[playerID].Units[unitID].Status |= 0x82;

							// Instruction address 0x1403:0x120c, size: 5
							F0_1403_3f13(playerID, unitID);

							this.parent.GameData.Players[playerID].Units[unitID].RemainingMoves = 0;
							this.parent.GameData.Players[playerID].Units[unitID].GoToDestination = OpenCivOneGame.InvalidPosition;
							this.parent.GameData.Players[playerID].Units[unitID].GoToNextDirection = -1;
							this.parent.GameData.Players[playerID].Units[unitID].SpecialMoves++;

							if (this.parent.GameData.Players[playerID].Units[unitID].SpecialMoves >= 4)
							{
								// Instruction address 0x1403:0x123e, size: 5
								this.parent.MapManagement.F0_2aea_16ee_ClearTerrainImprovements(local_2c, local_30, TerrainImprovementFlagsEnum.Pollution);
								// Instruction address 0x1403:0x124c, size: 5
								this.parent.MapManagement.F0_2aea_1601_UpdateVisibleCellStatus(local_2c, local_30);

								this.parent.GameData.PollutedSquareCount--;

								// Instruction address 0x1403:0x125e, size: 5
								F0_1403_3f13(playerID, unitID);

								this.parent.GameData.Players[playerID].Units[unitID].Status &= 0x7d;
								this.parent.GameData.Players[playerID].Units[unitID].SpecialMoves = 0;
							}
						}
					}
					break;

				case 'r':
					if (this.parent.GameData.Players[playerID].Units[unitID].UnitType != 0)
					{
						// Instruction address 0x1403:0x141e, size: 5
						this.parent.Help.F4_0000_03aa_ShowInstantWarningPopup("*SETTLERS");
					}
					else
					{
						newImprovements = this.parent.MapManagement.F0_2aea_1585_GetVisibleTerrainImprovements(local_2c, local_30);

						if ((newImprovements.HasFlag(TerrainImprovementFlagsEnum.Road) &&
							(!this.parent.Segment_1ade.F0_1ade_22b5_PlayerHasTechnology(playerID, TechnologyAdvanceEnum.Railroad) || 
							newImprovements.HasFlag(TerrainImprovementFlagsEnum.City))) || newImprovements.HasFlag(TerrainImprovementFlagsEnum.RailRoad))
						{
							this.parent.GameData.Players[playerID].Units[unitID].Status &= 0xfd;
							this.parent.GameData.Players[playerID].Units[unitID].SpecialMoves = 0;

							// Instruction address 0x1403:0x148f, size: 5
							this.parent.UnitManagement.F0_1866_16a9(playerID, local_2c, local_30);
						}
						else
						{
							newTerrainType = this.parent.MapManagement.GetTerrainType(local_2c, local_30);

							if (newTerrainType != TerrainTypeEnum.River || this.parent.Segment_1ade.F0_1ade_22b5_PlayerHasTechnology(playerID, TechnologyAdvanceEnum.BridgeBuilding))
							{
								this.parent.GameData.Players[playerID].Units[unitID].Status |= 2;
								this.parent.GameData.Players[playerID].Units[unitID].RemainingMoves = 0;

								// Instruction address 0x1403:0x14ee, size: 5
								F0_1403_3f13(playerID, unitID);

								int local_58 = this.parent.GameData.Players[playerID].Units[unitID].SpecialMoves;

								this.parent.GameData.Players[playerID].Units[unitID].SpecialMoves++;

								if ((((newImprovements.HasFlag(TerrainImprovementFlagsEnum.Road)) ? 4 : 2) * this.parent.GameData.Terrains[(int)newTerrainType].MovementCost) <= local_58)
								{
									// Instruction address 0x1403:0x154b, size: 5
									this.parent.MapManagement.F0_2aea_1653_SetTerrainImprovements(
										local_2c, local_30, ((newImprovements.HasFlag(TerrainImprovementFlagsEnum.Road)) ? TerrainImprovementFlagsEnum.RailRoad : TerrainImprovementFlagsEnum.Road));
									// Instruction address 0x1403:0x1567, size: 5
									this.parent.UnitManagement.F0_1866_01dc(local_2c, local_30, playerID, unitID, true);

									this.parent.GameData.Players[playerID].Units[unitID].Status &= 0xfd;
									this.parent.GameData.Players[playerID].Units[unitID].SpecialMoves = 0;
								}
							}
						}
					}
					break;

				case 's':
					this.parent.GameData.Players[playerID].Units[unitID].Status |= 1;
					this.parent.GameData.Players[playerID].Units[unitID].RemainingMoves = 0;

					// Instruction address 0x1403:0x0f64, size: 5
					F0_1403_3f13(playerID, unitID);

					break;

				case 'u':
					if (this.parent.GameData.Units[(int)this.parent.GameData.Players[playerID].Units[unitID].UnitType].TransportCapacity != 0 ||
						this.parent.GameData.Players[playerID].Units[unitID].UnitType == UnitTypeEnum.Carrier)
					{
						if (this.parent.GameData.Players[playerID].Units[unitID].NextUnitID != -1)
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

				case 'w':
					flag7 = true;
					flag4 = true;

					break;
			}

		Label286:
			if (unitID < 128 || flag1)
			{
				switch ((char)command)
				{
					case '\0':
						break;

					case 'c':
						if (flag1)
						{
							this.parent.MapManagement.F0_2aea_0008_DrawVisibleMap(playerID, local_e - 7, local_12 - 6);
						}
						else
						{
							this.parent.MapManagement.F0_2aea_0008_DrawVisibleMap(playerID, local_2c - 7, local_30 - 6);
						}
						break;

					// Up
					case '\x001':
					case '\x4800':
						local_24 = 1;
						goto Label300;

					// PageUp
					case '\x002':
					case '\x4900':
						local_24 = 2;
						goto Label300;

					// Right
					case '\x003':
					case '\x4d00':
						local_24 = 3;
						goto Label300;

					// PageDown
					case '\x004':
					case '\x5100':
						local_24 = 4;
						goto Label300;

					// Down
					case '\x005':
					case '\x5000':
						local_24 = 5;
						goto Label300;

					// End
					case '\x006':
					case '\x4f00':
						local_24 = 6;
						goto Label300;

					// Left
					case '\x007':
					case '\x4b00':
						local_24 = 7;
						goto Label300;

					// Home
					case '\x008':
					case '\x4700':
						local_24 = 8;
						goto Label300;
				}
			}

			goto Label674;

		Label300:
			#region Move Unit
			if (flag1)
			{
				if ((this.parent.GameData.MapVisibility[local_e, local_12] & (0x1 << playerID)) != 0)
				{
					// Instruction address 0x1403:0x1c43, size: 5
					this.parent.MapManagement.F0_2aea_11d4_DrawCellWithUnit(local_e, local_12);
				}

				local_e = this.parent.MapManagement.AdjustXPosition(this.parent.MoveDirections[local_24].X + local_e);
				local_12 = this.parent.GameTools.F0_2dc4_007c_CheckValueRange(this.parent.MoveDirections[local_24].Y + local_12, 0, 49);

				if (!F0_1403_4508(local_e, local_12))
				{
					// Instruction address 0x1403:0x1ca5, size: 5
					this.parent.MapManagement.F0_2aea_0008_DrawVisibleMap(playerID, local_e - 8, local_12 - 7);
				}
				goto Label674;
			}

			if (playerID == 0 && this.parent.GameData.Players[playerID].Units[unitID].UnitType != UnitTypeEnum.Diplomat)
			{
				newImprovements = this.parent.MapManagement.F0_2aea_1585_GetVisibleTerrainImprovements(local_2c, local_30);

				if (!newImprovements.HasFlag(TerrainImprovementFlagsEnum.City) && 
					(newImprovements.HasFlag(TerrainImprovementFlagsEnum.Mines) || newImprovements.HasFlag(TerrainImprovementFlagsEnum.Irrigation)) &&
					this.parent.GameData.Players[playerID].Units[unitID].UnitType != UnitTypeEnum.Legion &&
					this.parent.GameData.Players[playerID].Units[unitID].UnitType != UnitTypeEnum.Knights &&
					this.parent.GameData.Cities[this.parent.GameTools.F0_2dc4_0102_FindNearestCity(local_2c, local_30)].PlayerID == this.parent.GameData.HumanPlayerID)
				{
					// Instruction address 0x1403:0x1d35, size: 5
					this.parent.MapManagement.F0_2aea_16ee_ClearTerrainImprovements(local_2c, local_30, TerrainImprovementFlagsEnum.Mines | TerrainImprovementFlagsEnum.Irrigation);

					if ((this.parent.GameData.MapVisibility[local_2c, local_30] & (0x1 << this.parent.GameData.HumanPlayerID)) != 0)
					{
						// Instruction address 0x1403:0x1d68, size: 5
						this.parent.MapManagement.F0_2aea_1601_UpdateVisibleCellStatus(local_2c, local_30);
						// Instruction address 0x1403:0x1d76, size: 5
						this.parent.MapManagement.F0_2aea_11d4_DrawCellWithUnit(local_2c, local_30);
					}

					this.parent.GameData.Players[playerID].Units[unitID].RemainingMoves = 0;
					goto Label674;
				}
			}

			local_1c = this.parent.MapManagement.AdjustXPosition(this.parent.MoveDirections[local_24].X + local_2c);
			local_26 = this.parent.GameTools.F0_2dc4_007c_CheckValueRange(this.parent.MoveDirections[local_24].Y + local_30, 0, 49);
			TerrainTypeEnum terrainType = this.parent.MapManagement.GetTerrainType(local_2c, local_30);
			newTerrainType = this.parent.MapManagement.GetTerrainType(local_1c, local_26);
			newImprovements = this.parent.MapManagement.F0_2aea_1585_GetVisibleTerrainImprovements(local_1c, local_26);
			local_18 = this.parent.MapManagement.F0_2aea_1458_GetCellActiveUnitID(local_1c, local_26);

			if (this.parent.GameData.Players[playerID].Units[unitID].UnitType != UnitTypeEnum.None &&
				this.parent.GameData.Units[(int)this.parent.GameData.Players[playerID].Units[unitID].UnitType].MovementType == UnitMovementTypeEnum.Land)
			{
				if (local_18 == -1 && this.parent.UnitManagement.F0_1866_1725_IsUnitNear(playerID, local_2c, local_30) &&
					this.parent.UnitManagement.F0_1866_1725_IsUnitNear(playerID, local_1c, local_26) &&
					(this.parent.GameData.Players[playerID].Units[unitID].UnitType != UnitTypeEnum.Diplomat && 
						this.parent.GameData.Players[playerID].Units[unitID].UnitType != UnitTypeEnum.Caravan) && 
					terrainType != TerrainTypeEnum.Water)
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

					this.parent.GameData.Players[playerID].Units[unitID].GoToDestination = OpenCivOneGame.InvalidPosition;

					goto Label674;
				}

				if (this.parent.GameData.Players[playerID].Units[unitID].UnitType == UnitTypeEnum.Diplomat || 
					this.parent.GameData.Players[playerID].Units[unitID].UnitType == UnitTypeEnum.Caravan)
				{
					int cityID = this.parent.GameTools.F0_2dc4_00ba_GetCityByLocation(local_1c, local_26);

					if (cityID != -1)
					{
						if (this.parent.GameData.Cities[cityID].PlayerID != playerID && terrainType == TerrainTypeEnum.Water) goto Label674;

						if (this.parent.GameData.Players[playerID].Units[unitID].UnitType == UnitTypeEnum.Diplomat &&
							playerID != this.parent.GameData.Cities[cityID].PlayerID)
						{
							if (this.parent.GameData.HumanPlayerID == this.parent.GameData.Cities[cityID].PlayerID)
							{
								this.parent.GameData.Players[playerID].Units[unitID].VisibleByPlayer |=
									(ushort)(1 << this.parent.GameData.HumanPlayerID);

								// Instruction address 0x1403:0x1f7f, size: 5
								this.parent.UnitManagement.F0_1866_16a9(this.parent.GameData.HumanPlayerID,
									this.parent.GameData.Players[playerID].Units[unitID].Position.X,
									this.parent.GameData.Players[playerID].Units[unitID].Position.Y);
								// Instruction address 0x1403:0x1f8b, size: 5
								this.parent.CommonTools.WaitTimer(30);
								// Instruction address 0x1403:0x1f9c, size: 5
								this.parent.UnitManagement.F0_1866_1d55(playerID, unitID, local_24);
							}

							this.parent.Overlay_22.F22_0000_0000(cityID, playerID, unitID);

							goto Label674;
						}

						if (this.parent.GameData.Players[playerID].Units[unitID].UnitType == UnitTypeEnum.Caravan)
						{
							local_3a = this.parent.GameTools.F0_2dc4_0289_GetShortestDistance(local_1c,
								this.parent.GameData.Cities[this.parent.GameData.Players[playerID].Units[unitID].HomeCityID].Position.X,
								local_26,
								this.parent.GameData.Cities[this.parent.GameData.Players[playerID].Units[unitID].HomeCityID].Position.Y);

							if (this.parent.GameData.Cities[cityID].PlayerID != playerID ||
								this.parent.GameData.Cities[cityID].CurrentProductionID < -24 || local_3a >= 10 ||
								this.parent.MapManagement.F0_2aea_1942_GetGroupID(local_1c, local_26) != this.parent.MapManagement.F0_2aea_1942_GetGroupID(
									this.parent.GameData.Cities[this.parent.GameData.Players[playerID].Units[unitID].HomeCityID].Position.X,
									this.parent.GameData.Cities[this.parent.GameData.Players[playerID].Units[unitID].HomeCityID].Position.Y))
							{
								if (this.parent.GameData.Cities[cityID].PlayerID != playerID)
								{
									local_3a = 1;
								}
								else
								{
									StringBuilder caravanOptions = new();
									caravanOptions.Append("Will you?\n Keep moving\n Establish trade route\n");

									if (this.parent.GameData.Cities[cityID].PlayerID == playerID && this.parent.GameData.Cities[cityID].CurrentProductionID < -24)
									{
										// Instruction address 0x1403:0x209d, size: 5
										caravanOptions.Append(" Help build WONDER.\n");

										if (local_3a < 10)
										{
											this.parent.Var_b276_MenuBoxDisabledOptions = 2;
										}
									}

									local_3a = this.parent.Segment_1238.F0_1238_001e_ShowDialog(caravanOptions.ToString(), 100, 80);
								}

								if (local_3a == 1)
								{
									if (this.parent.Segment_2459.F0_2459_0948(playerID, unitID, cityID) != 0) goto Label674;
								}

								if (local_3a == 2)
								{
									this.parent.GameData.Cities[cityID].ShieldsCount +=
										(short)(10 * this.parent.GameData.Units[(int)this.parent.GameData.Players[playerID].Units[unitID].UnitType].Cost);

									// Instruction address 0x1403:0x2149, size: 5
									this.parent.UnitManagement.F0_1866_0f10_DeleteUnit(playerID, unitID);

									goto Label674;
								}
							}
						}
					}
					else
					{
						if (this.parent.GameData.Players[playerID].Units[unitID].UnitType == UnitTypeEnum.Diplomat &&
							local_18 != -1 && playerID != this.parent.Var_d20a)
						{
							this.parent.Overlay_22.F22_0000_0639(this.parent.Var_d20a, local_18, playerID);

							this.parent.GameData.Players[playerID].Units[unitID].GoToDestination = OpenCivOneGame.InvalidPosition;
						}
					}
				}
			}

			if (newTerrainType == TerrainTypeEnum.Water && this.parent.GameData.Players[playerID].Units[unitID].UnitType != UnitTypeEnum.None &&
				this.parent.GameData.Units[(int)this.parent.GameData.Players[playerID].Units[unitID].UnitType].MovementType == UnitMovementTypeEnum.Land)
			{
				if (playerID != this.parent.Var_d20a || local_18 == -1 || this.parent.UnitManagement.F0_1866_13d5(playerID, local_18) <= 0) goto Label674;

				this.parent.GameData.Players[playerID].Units[unitID].Status |= 1;
				this.parent.GameData.Players[playerID].Units[unitID].Status &= 0xf3;
				this.parent.GameData.Players[playerID].Units[unitID].RemainingMoves = 3;
			}
			else if (local_18 != -1 && playerID != this.parent.Var_d20a)
			{
				if (terrainType == TerrainTypeEnum.Water &&
					this.parent.GameData.Units[(int)this.parent.GameData.Players[playerID].Units[unitID].UnitType].MovementType == UnitMovementTypeEnum.Land)
				{
					// Instruction address 0x1403:0x224c, size: 5
					this.parent.Help.F4_0000_03aa_ShowInstantWarningPopup("*AMPHIB");

					this.parent.GameData.Players[playerID].Units[unitID].GoToDestination = OpenCivOneGame.InvalidPosition;

					goto Label674;
				}

				if (newTerrainType != TerrainTypeEnum.Water && this.parent.GameData.Players[playerID].Units[unitID].UnitType == UnitTypeEnum.Submarine) goto Label674;

				if (playerID != this.parent.GameData.HumanPlayerID &&
					this.parent.GameData.Units[(int)this.parent.GameData.Players[playerID].Units[unitID].UnitType].AttackStrength == 0) goto Label674;

				if (this.parent.GameData.Players[playerID].Units[unitID].UnitType == UnitTypeEnum.Diplomat) goto Label674;

				if (this.parent.GameData.Units[(int)this.parent.GameData.Players[this.parent.Var_d20a].Units[local_18].UnitType].MovementType == UnitMovementTypeEnum.Water)
				{
					if (this.parent.GameData.Players[playerID].Units[unitID].UnitType != UnitTypeEnum.Fighter)
					{
						if (!newImprovements.HasFlag(TerrainImprovementFlagsEnum.City))
						{
							// Instruction address 0x1403:0x2319, size: 5
							this.parent.Help.F4_0000_03aa_ShowInstantWarningPopup("*FIGHTER");

							this.parent.GameData.Players[playerID].Units[unitID].GoToDestination = OpenCivOneGame.InvalidPosition;

							goto Label674;
						}
					}
				}

				if (this.parent.GameData.Players[playerID].Units[unitID].RemainingMoves < 3)
				{
					if (playerID == this.parent.GameData.HumanPlayerID)
					{
						if (this.parent.Segment_1238.F0_1238_001e_ShowDialog(
							$"Attack at\n{this.parent.GameData.Players[playerID].Units[unitID].RemainingMoves}/3 strength?\n Cancel\n Attack\n", 100, 16) != 1)
						{
							this.parent.GameData.Players[playerID].Units[unitID].GoToDestination = OpenCivOneGame.InvalidPosition;

							goto Label674;
						}
					}
					else
					{
						if (this.parent.GameData.Players[playerID].Units[unitID].RemainingMoves < 2) goto Label674;
					}
				}

				if (this.parent.Var_d20a == this.parent.GameData.HumanPlayerID)
				{
					// Instruction address 0x1403:0x23eb, size: 5
					this.parent.UnitManagement.F0_1866_16a9(this.parent.GameData.HumanPlayerID, local_1c, local_26);
				}

				local_18 = this.parent.UnitManagement.F0_1866_1122(this.parent.Var_d20a, local_18);

				if (playerID == this.parent.GameData.HumanPlayerID ||
					this.parent.Var_d20a == this.parent.GameData.HumanPlayerID)
				{
					this.parent.Var_70d8 = true;
				}

				local_3a = this.parent.Segment_29f3.F0_29f3_000e(playerID, unitID, this.parent.Var_d20a, local_18, true);
				this.parent.GameData.Players[playerID].Units[unitID].GoToDestination = OpenCivOneGame.InvalidPosition;
				this.parent.Var_70d8 = false;

				if (local_3a == -1) goto Label674;

				if (this.parent.GameData.Players[playerID].Units[unitID].UnitType == UnitTypeEnum.None)
				{
					this.parent.GameData.Players[playerID].Units[unitID].RemainingMoves = 0;
					goto Label722;
				}

				this.parent.GameData.Players[playerID].Units[unitID].RemainingMoves -= 3;

				if (this.parent.GameData.Players[playerID].Units[unitID].RemainingMoves < 0 || this.parent.GameData.Players[playerID].Units[unitID].RemainingMoves == 15)
				{
					this.parent.GameData.Players[playerID].Units[unitID].RemainingMoves = 0;
				}

				if (!newImprovements.HasFlag(TerrainImprovementFlagsEnum.City)) goto Label674;

				int cityID = this.parent.GameTools.F0_2dc4_00ba_GetCityByLocation(local_1c, local_26);

				if (this.parent.GameData.Cities[cityID].PlayerID != this.parent.GameData.HumanPlayerID)
				{
					this.parent.GameData.Cities[cityID].StatusFlag |= 0x10;
				}

				if (this.parent.GameData.Players[playerID].Units[unitID].UnitType != UnitTypeEnum.None)
				{
					if ((this.parent.GameData.Cities[cityID].ImprovementFlags0 & 0x80) == 0)
					{
						if (terrainType != TerrainTypeEnum.Water)
						{
							if (this.parent.GameData.DifficultyLevel != 0 ||
								this.parent.Var_d20a != this.parent.GameData.HumanPlayerID)
							{
								this.parent.GameData.Cities[cityID].ActualSize--;
							}
						}
					}
				}

				if (this.parent.GameData.Cities[cityID].ActualSize == 0)
				{
					// Instruction address 0x1403:0x2576, size: 5
					this.parent.Segment_1ade.F0_1ade_018e(cityID, local_1c, local_26);
					this.parent.StartGameMenu.F5_0000_0e6c_TestIfAIPlayerDestroyed(this.parent.Var_d20a, playerID);

					if (playerID == this.parent.GameData.HumanPlayerID)
					{
						local_18 = this.parent.MapManagement.F0_2aea_1458_GetCellActiveUnitID(local_1c, local_26);

						if (local_18 != -1)
						{
							this.parent.GameData.Players[this.parent.Var_d20a].Units[local_18].VisibleByPlayer |=
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
					this.parent.MapManagement.F0_2aea_11d4_DrawCellWithUnit(local_1c, local_26);
				}

				if (this.parent.MapManagement.F0_2aea_14e0_GetCellUnitPlayerID(local_1c, local_26) == -1)
				{
					if (this.parent.Var_d20a != this.parent.GameData.HumanPlayerID)
					{
						// Instruction address 0x1403:0x2641, size: 5
						this.parent.AIEngine.F0_25fb_3459_PlayerChangeCityProductionForSameContinent(this.parent.Var_d20a, this.parent.MapManagement.F0_2aea_1942_GetGroupID(local_1c, local_26));
					}
				}
				goto Label674;
			}

			if (newTerrainType != TerrainTypeEnum.Water && !newImprovements.HasFlag(TerrainImprovementFlagsEnum.City) &&
				this.parent.GameData.Players[playerID].Units[unitID].UnitType != UnitTypeEnum.None &&
				this.parent.GameData.Units[(int)this.parent.GameData.Players[playerID].Units[unitID].UnitType].MovementType == UnitMovementTypeEnum.Air)
			{
				this.parent.GameData.Players[playerID].Units[unitID].GoToDestination = OpenCivOneGame.InvalidPosition;
				goto Label674;
			}

			if (this.parent.GameData.Players[playerID].Units[unitID].UnitType != UnitTypeEnum.None &&
				this.parent.GameData.Units[(int)this.parent.GameData.Players[playerID].Units[unitID].UnitType].MovementType != UnitMovementTypeEnum.Land &&
				newImprovements.HasFlag(TerrainImprovementFlagsEnum.City) && this.parent.MapManagement.F0_2aea_1369_GetCityOwner(local_1c, local_26) != playerID)
			{
				if (this.parent.GameData.Players[playerID].Units[unitID].UnitType != UnitTypeEnum.Nuclear)
				{
					// Instruction address 0x1403:0x2706, size: 5
					this.parent.Help.F4_0000_03aa_ShowInstantWarningPopup("*OCCUPY");

					this.parent.GameData.Players[playerID].Units[unitID].GoToDestination = OpenCivOneGame.InvalidPosition;
				}
				else
				{
					// Instruction address 0x1403:0x271f, size: 5
					this.parent.UnitManagement.F0_1866_0f10_DeleteUnit(playerID, unitID);
					// Instruction address 0x1403:0x2730, size: 5
					this.parent.Segment_29f3.F0_29f3_0d4d(playerID, local_1c, local_26);
				}
				goto Label674;
			}

			if (this.parent.GameData.Players[playerID].Units[unitID].RemainingMoves == 0)
			{
				goto Label674;
			}

			if (this.parent.GameData.Units[(int)this.parent.GameData.Players[playerID].Units[unitID].UnitType].MovementType == UnitMovementTypeEnum.Land)
			{
				if ((this.parent.MapManagement.F0_2aea_1585_GetVisibleTerrainImprovements(local_2c, local_30) & newImprovements).HasFlag(TerrainImprovementFlagsEnum.Road) &&
					this.parent.GameData.Players[playerID].Units[unitID].RemainingMoves != 0)
				{
					if (!this.parent.MapManagement.F0_2aea_1585_GetVisibleTerrainImprovements(local_2c, local_30).HasFlag(TerrainImprovementFlagsEnum.RailRoad) ||
						playerID != this.parent.GameData.HumanPlayerID ||
						this.parent.GameData.Players[playerID].Units[unitID].GoToDestination.X != -1)
					{
						this.parent.GameData.Players[playerID].Units[unitID].RemainingMoves -= 1;
					}
				}
				else
				{
					if (this.parent.GameData.Players[playerID].Units[unitID].RemainingMoves != 3 &&
						this.parent.CAPI.RNG.Next(this.parent.GameData.Terrains[(int)newTerrainType].MovementCost * 3) > this.parent.GameData.Players[playerID].Units[unitID].RemainingMoves)
					{
						this.parent.GameData.Players[playerID].Units[unitID].RemainingMoves = 0;
						goto Label674;
					}

					this.parent.GameData.Players[playerID].Units[unitID].RemainingMoves -= (short)(this.parent.GameData.Terrains[(int)newTerrainType].MovementCost * 3);
				}
			}
			else
			{
				this.parent.GameData.Players[playerID].Units[unitID].RemainingMoves -= 3;
			}

			if (this.parent.GameData.Players[playerID].Units[unitID].RemainingMoves < 0)
			{
				this.parent.GameData.Players[playerID].Units[unitID].RemainingMoves = 0;
			}

			if (playerID != this.parent.GameData.HumanPlayerID)
			{
				if (this.parent.GameData.Players[playerID].Units[unitID].GoToNextDirection != -1)
				{
					if ((this.parent.GameData.Players[playerID].Units[unitID].GoToNextDirection ^ 0x4) == local_24)
					{
						if (this.parent.UnitManagement.F0_1866_1251_GetStackUnitValueCoefficient(playerID, unitID, 2) <= 2)
						{
							this.parent.GameData.Players[playerID].Units[unitID].GoToDestination = OpenCivOneGame.InvalidPosition;
							this.parent.GameData.Players[playerID].Units[unitID].GoToNextDirection = -1;
							this.parent.GameData.Players[playerID].Units[unitID].RemainingMoves = 0;

							goto Label674;
						}
					}
				}
				this.parent.GameData.Players[playerID].Units[unitID].GoToNextDirection = (short)local_24;
			}

			if (local_18 != -1 &&
				playerID == this.parent.Var_d20a && playerID != this.parent.GameData.HumanPlayerID && !newImprovements.HasFlag(TerrainImprovementFlagsEnum.City))
			{
				if (this.parent.GameData.Units[(int)this.parent.GameData.Players[playerID].Units[unitID].UnitType].MovementType == UnitMovementTypeEnum.Land ||
					(this.parent.GameData.Units[(int)this.parent.GameData.Players[playerID].Units[unitID].UnitType].MovementType == UnitMovementTypeEnum.Water &&
						this.parent.GameData.Players[playerID].Units[unitID].SpecialMoves != 0))
				{
					if (((this.parent.GameData.Players[playerID].Units[unitID].RemainingMoves != 0) ? 4 : 2) <= this.parent.UnitManagement.F0_1866_1251_GetStackUnitValueCoefficient(playerID, local_18, 2))
						goto Label674;
				}
			}

			// Get playerID that owns this land
			int ownerPlayerID = this.parent.MapManagement.GetPlayerLandOwnership(local_1c, local_26);

			if (ownerPlayerID != 0)
			{
				if (ownerPlayerID < 8)
				{
					if (newTerrainType != TerrainTypeEnum.Water && terrainType != TerrainTypeEnum.Water &&
						(this.parent.GameData.Players[playerID].Units[unitID].UnitType != UnitTypeEnum.Diplomat && 
						this.parent.GameData.Players[playerID].Units[unitID].UnitType != UnitTypeEnum.Caravan) &&
						(newImprovements.HasFlag(TerrainImprovementFlagsEnum.City) || newImprovements.HasFlag(TerrainImprovementFlagsEnum.Irrigation) ||
						newImprovements.HasFlag(TerrainImprovementFlagsEnum.Mines) || newImprovements.HasFlag(TerrainImprovementFlagsEnum.Road)) && ownerPlayerID != playerID)
					{
						if ((playerID == this.parent.GameData.HumanPlayerID || ownerPlayerID == this.parent.GameData.HumanPlayerID) &&
							this.parent.GameData.Units[(int)this.parent.GameData.Players[playerID].Units[unitID].UnitType].MovementType == UnitMovementTypeEnum.Land &&
							this.parent.MapManagement.F0_2aea_14e0_GetCellUnitPlayerID(local_1c, local_26) != playerID)
						{
							if (playerID == this.parent.GameData.HumanPlayerID)
							{
								this.parent.GameData.Players[playerID].Units[unitID].GoToDestination = OpenCivOneGame.InvalidPosition;

								if ((short)this.parent.Segment_29f3.F0_29f3_0c9e(ownerPlayerID) != -1)
								{
									// Instruction address 0x1403:0x2a87, size: 5
									this.parent.Segment_2517.F0_2517_0aa1_ClearDiplomacyFlags(playerID, ownerPlayerID, DiplomacyFlagsEnum.Peace);
								}
								else
								{
									goto Label674;
								}
							}
							else if (ownerPlayerID == this.parent.GameData.HumanPlayerID &&
								this.parent.GameData.Players[ownerPlayerID].Diplomacy[playerID].HasFlag(DiplomacyFlagsEnum.Peace))
							{
								this.parent.GameData.Players[playerID].Units[unitID].RemainingMoves = 0;

								goto Label674;
							}
						}
					}

					if (playerID != ownerPlayerID)
					{
						if (((int)this.parent.MapManagement.F0_2aea_1585_GetVisibleTerrainImprovements(local_1c, local_26) & 0xe) != 0)
						{
							this.parent.GameData.Players[playerID].Units[unitID].VisibleByPlayer |= (ushort)(1 << ownerPlayerID);
						}
					}
				}
			}

			int cityPlayerID1 = this.parent.MapManagement.F0_2aea_1369_GetCityOwner(local_1c, local_26);

			if (playerID == 0 && newTerrainType != TerrainTypeEnum.Water &&
				(this.parent.GameData.MapVisibility[local_2c, local_30] & (0x1 << this.parent.GameData.HumanPlayerID)) != 0 &&
				(this.parent.GameData.Players[playerID].Units[unitID].VisibleByPlayer & (0x1 << this.parent.GameData.HumanPlayerID)) == 0 &&
				this.parent.GameData.Cities[this.parent.GameTools.F0_2dc4_0102_FindNearestCity(local_2c, local_30)].PlayerID == this.parent.GameData.HumanPlayerID)
			{
				this.parent.GameData.Players[playerID].Units[unitID].VisibleByPlayer |= this.parent.GameData.MapVisibility[local_2c, local_30];
			}

			if (newImprovements.HasFlag(TerrainImprovementFlagsEnum.City) && cityPlayerID1 != playerID &&
				(this.parent.GameData.MapVisibility[local_1c, local_26] & (0x1 << this.parent.GameData.HumanPlayerID)) != 0 &&
				(this.parent.GameData.Players[this.parent.GameData.HumanPlayerID].Diplomacy[playerID].HasFlag(DiplomacyFlagsEnum.Unknown40) ||
					this.parent.GameData.Players[this.parent.GameData.HumanPlayerID].Diplomacy[cityPlayerID1].HasFlag(DiplomacyFlagsEnum.Unknown40)))
			{
				this.parent.GameData.Players[playerID].Units[unitID].VisibleByPlayer |= (ushort)(1 << this.parent.GameData.HumanPlayerID);
			}

			if (newTerrainType == TerrainTypeEnum.Water)
			{
				if (((int)this.parent.MapManagement.F0_2aea_1585_GetVisibleTerrainImprovements(local_1c, local_26) & 0x2) != 0)
				{
					this.parent.GameData.Players[playerID].Units[unitID].VisibleByPlayer |= (ushort)(1 << cityPlayerID1);
				}
			}

			if ((this.parent.GameData.Players[playerID].Units[unitID].VisibleByPlayer & (0x1 << this.parent.GameData.HumanPlayerID)) != 0 ||
				playerID == this.parent.GameData.HumanPlayerID || (this.parent.Var_d806_DebugFlag && playerID != 0))
			{
				if (!this.parent.GameData.GameSettingFlags.EnemyMoves && playerID != this.parent.GameData.HumanPlayerID)
				{
					// Instruction address 0x1403:0x2cd5, size: 5
					this.parent.MapManagement.F0_2aea_03ba_DrawCell(local_2c, local_30);
				}
				else
				{
					// Instruction address 0x1403:0x2cb3, size: 5
					this.parent.UnitManagement.F0_1866_16a9(this.parent.GameData.HumanPlayerID, local_1c, local_26);
					// Instruction address 0x1403:0x2cc4, size: 5
					this.parent.UnitManagement.F0_1866_1d55(playerID, unitID, local_24);
				}
			}

			// Instruction address 0x1403:0x2d01, size: 5
			this.parent.MapManagement.F0_2aea_1412_SetCellActivePlayerID(local_2c, local_30, playerID, unitID);

			// Instruction address 0x1403:0x2d21, size: 5
			this.parent.MapManagement.F0_2aea_13cb_SetCellPlayerID(local_1c, local_26, playerID, unitID);

			this.parent.GameData.Players[playerID].Units[unitID].Position = new(local_1c, local_26);
			this.parent.GameData.Players[playerID].Units[unitID].VisibleByPlayer = 0;

			if (playerID != this.parent.GameData.HumanPlayerID &&
				this.parent.GameData.Units[(int)this.parent.GameData.Players[playerID].Units[unitID].UnitType].MovementType != UnitMovementTypeEnum.Water)
			{
				this.parent.GameData.Players[playerID].Units[unitID].Status &= 0x30;
			}

			if (ownerPlayerID != 0 && ownerPlayerID < 8 && ownerPlayerID != playerID)
			{
				// Instruction address 0x1403:0x2d9d, size: 5
				this.parent.MapManagement.SetPlayerLandOwnership(local_1c, local_26, 0);
			}

			if (this.parent.GameData.Players[playerID].Units[unitID].GoToDestination.X == local_1c &&
				this.parent.GameData.Players[playerID].Units[unitID].GoToDestination.Y == local_26)
			{
				this.parent.GameData.Players[playerID].Units[unitID].GoToDestination = OpenCivOneGame.InvalidPosition;
				this.parent.GameData.Players[playerID].Units[unitID].GoToNextDirection = -1;

				if (playerID != this.parent.GameData.HumanPlayerID)
				{
					this.parent.GameData.Players[playerID].Units[unitID].RemainingMoves = 0;
				}
			}

			if (terrainType == TerrainTypeEnum.Water && newTerrainType != TerrainTypeEnum.Water &&
				this.parent.GameData.Units[(int)this.parent.GameData.Players[playerID].Units[unitID].UnitType].MovementType != UnitMovementTypeEnum.Water)
			{
				this.parent.GameData.Players[playerID].Units[unitID].RemainingMoves = 0;
			}

			int nextUnitID = this.parent.GameData.Players[playerID].Units[unitID].NextUnitID;

			if (nextUnitID != -1 && this.parent.GameData.Units[(int)this.parent.GameData.Players[playerID].Units[unitID].UnitType].TransportCapacity == 0 && 
				this.parent.GameData.Players[playerID].Units[unitID].UnitType == UnitTypeEnum.Carrier)
			{
				int neededCapacity = 0;

				for (int i = 0; i < 20; i++)
				{
					int nextUnitID1 = this.parent.GameData.Players[playerID].Units[nextUnitID].NextUnitID;

					if (((this.parent.GameData.Units[(int)this.parent.GameData.Players[playerID].Units[unitID].UnitType].TransportCapacity < 1) ? UnitMovementTypeEnum.Water : UnitMovementTypeEnum.Land) == 
						this.parent.GameData.Units[(int)this.parent.GameData.Players[playerID].Units[nextUnitID].UnitType].MovementType)
					{
						if (((this.parent.GameData.Players[playerID].Units[nextUnitID].Status & 0x1) == 0 && terrainType == TerrainTypeEnum.Water) ||
							(playerID != this.parent.GameData.HumanPlayerID &&
							(this.parent.GameData.Players[playerID].Units[nextUnitID].Status & 0xc) == 0 &&
							this.parent.GameData.Players[playerID].Units[nextUnitID].NextUnitID != -1))
						{
							// Instruction address 0x1403:0x2efb, size: 5
							this.parent.MapManagement.F0_2aea_1412_SetCellActivePlayerID(local_2c, local_30, playerID, nextUnitID);
							// Instruction address 0x1403:0x2f0f, size: 5
							this.parent.MapManagement.F0_2aea_13cb_SetCellPlayerID(local_1c, local_26, playerID, nextUnitID);

							this.parent.GameData.Players[playerID].Units[nextUnitID].Position = new(local_1c, local_26);
							this.parent.GameData.Players[playerID].Units[nextUnitID].VisibleByPlayer = 0;
							this.parent.GameData.Players[playerID].Units[nextUnitID].GoToNextDirection = -1;

							if (this.parent.GameData.Units[(int)this.parent.GameData.Players[playerID].Units[unitID].UnitType].TransportCapacity != 0)
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

					if (nextUnitID == -1 ||
						(this.parent.GameData.Units[(int)this.parent.GameData.Players[playerID].Units[unitID].UnitType].TransportCapacity <= neededCapacity && 
						this.parent.GameData.Players[playerID].Units[unitID].UnitType != UnitTypeEnum.Carrier && playerID != 0))
						break;
				}
			}

			if (newImprovements.HasFlag(TerrainImprovementFlagsEnum.City) && cityPlayerID1 != playerID)
			{
				this.parent.GameData.Players[playerID].Units[unitID].VisibleByPlayer |= (ushort)(1 << cityPlayerID1);

				// Instruction address 0x1403:0x303e, size: 5
				this.parent.Segment_2459.F0_2459_0000(playerID, this.parent.GameTools.F0_2dc4_00ba_GetCityByLocation(local_1c, local_26), 0);
				// Instruction address 0x1403:0x304c, size: 5
				this.parent.MapManagement.F0_2aea_1511_ActiveUnitSetFlag8(local_1c, local_26);
			}

			// Instruction address 0x1403:0x305d, size: 5
			this.parent.MapManagement.F0_2aea_138c_SetCityOwner(local_1c, local_26, playerID);
			// Instruction address 0x1403:0x3079, size: 5
			this.parent.UnitManagement.F0_1866_01dc(local_1c, local_26, playerID, unitID, true);

			if (this.parent.MapManagement.F0_2aea_1894_CellHasMinorTribeHut(local_1c, local_26, newTerrainType))
			{
				// Instruction address 0x1403:0x30a0, size: 5
				this.parent.UnitManagement.F0_1866_1931_FoundMinorTribeHut(playerID, unitID);

				this.parent.GameData.MapVisibility[local_1c, local_26] |= 1;
			}

			if (newTerrainType == TerrainTypeEnum.Water &&
				this.parent.GameData.Units[(int)this.parent.GameData.Players[playerID].Units[unitID].UnitType].MovementType != UnitMovementTypeEnum.Air &&
				this.parent.GameData.Units[(int)this.parent.GameData.Players[playerID].Units[unitID].UnitType].MovementType != UnitMovementTypeEnum.Water)
			{
				// Instruction address 0x1403:0x30f6, size: 5
				this.parent.UnitManagement.F0_1866_1560_UnitStack(playerID, unitID);
			}

			if (playerID == 0 && newTerrainType != TerrainTypeEnum.Water)
			{
				if ((this.parent.GameData.MapVisibility[local_1c, local_26] & (0x1 << this.parent.GameData.HumanPlayerID)) != 0 &&
					(this.parent.GameData.Players[playerID].Units[unitID].VisibleByPlayer & (0x1 << this.parent.GameData.HumanPlayerID)) == 0 &&
					this.parent.GameData.Cities[this.parent.GameTools.F0_2dc4_0102_FindNearestCity(local_1c, local_26)].PlayerID == this.parent.GameData.HumanPlayerID)
				{
					this.parent.GameData.Players[playerID].Units[unitID].VisibleByPlayer |= this.parent.GameData.MapVisibility[local_1c, local_26];
				}
			}

			if ((this.parent.GameData.Players[playerID].Units[unitID].VisibleByPlayer & (0x1 << this.parent.GameData.HumanPlayerID)) != 0 ||
				playerID == this.parent.GameData.HumanPlayerID || this.parent.Var_d806_DebugFlag && playerID != 0)
			{
				if (playerID == this.parent.GameData.HumanPlayerID ||
					this.parent.GameData.GameSettingFlags.EnemyMoves)
				{
					// Instruction address 0x1403:0x31e6, size: 5
					this.parent.UnitManagement.F0_1866_16a9(this.parent.GameData.HumanPlayerID, local_1c, local_26);
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

			if (newImprovements.HasFlag(TerrainImprovementFlagsEnum.City) &&
				this.parent.GameData.Units[(int)this.parent.GameData.Players[playerID].Units[unitID].UnitType].TurnsOutside != 0)
			{
				this.parent.GameData.Players[playerID].Units[unitID].Status &= 0xfd;
				this.parent.GameData.Players[playerID].Units[unitID].RemainingMoves = 0;
				this.parent.GameData.Players[playerID].Units[unitID].GoToDestination = OpenCivOneGame.InvalidPosition;
			}

			if (this.parent.GameData.Units[(int)this.parent.GameData.Players[playerID].Units[unitID].UnitType].TurnsOutside != 0)
			{
				if (this.parent.GameData.Units[(int)this.parent.GameData.Players[playerID].Units[unitID].UnitType].MovementType == UnitMovementTypeEnum.Water)
				{
					if (this.parent.UnitManagement.F0_1866_1331(playerID, unitID, UnitTypeEnum.Carrier) != 0)
					{
						this.parent.GameData.Players[playerID].Units[unitID].Status &= 0xfd;
						this.parent.GameData.Players[playerID].Units[unitID].RemainingMoves = 0;
						this.parent.GameData.Players[playerID].Units[unitID].SpecialMoves =
							this.parent.GameData.Units[(int)this.parent.GameData.Players[playerID].Units[unitID].UnitType].TurnsOutside;
					}
				}
			}
			goto Label674;
		#endregion

		Label674:
			switch (command)
			{
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

				// Enter
				case 0xd:
				case ' ':
					if (unitID >= 128) goto Label755;
					break;

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
					local_3a = this.parent.Segment_1238.F0_1238_001e_ShowDialog(taxOptions.ToString(), 100, 80);

					if (local_3a != -1)
					{
						this.parent.GameData.Players[playerID].ScienceTaxRate += (short)(this.parent.GameData.Players[playerID].TaxRate - local_3a);
						this.parent.GameData.Players[playerID].TaxRate = (short)local_3a;

						// Instruction address 0x1403:0x36b1, size: 5
						this.parent.Segment_1238.F0_1238_107e();
					}
					break;

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

					local_3a = this.parent.Segment_1238.F0_1238_001e_ShowDialog(luxuryOptions.ToString(), 100, 80);

					if (local_3a != -1)
					{
						this.parent.GameData.Players[playerID].ScienceTaxRate = (short)(10 - this.parent.GameData.Players[playerID].TaxRate + local_3a);
						// Instruction address 0x1403:0x37a3, size: 5
						this.parent.Segment_1238.F0_1238_107e();
					}
					break;

				case '/':
				case '?':
					this.parent.TextBoxDialogs.F23_0000_025b_FindCityDialog();
					break;

				case 'S':
					// Enable Save game with zero turns
					//if (this.oParent.GameData.TurnCount != 0)
					//{
					// All Top menus are saving the current screenshot, so should we in Save game shortcut
					this.parent.Graphics.F0_VGA_07d8_DrawImage(this.parent.Var_aa_Screen0_Rectangle, 0, 0, 320, 200, this.parent.Var_19d4_Screen1_Rectangle, 0, 0);

					this.parent.GameLoadAndSave.F11_0000_036a_SaveGameDialog(false);
					//}
					break;

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

				// Alt + Q
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

				// Alt + W
				case 0x1100:
					// Instruction address 0x1403:0x3a84, size: 5
					this.parent.GameMenus.F0_2c84_0000_ShowTopMenu(playerID, unitID, 3);

					if (this.parent.GameMenus.Var_d4ca_MenuShortcutKey != -1)
					{
						command = this.parent.GameMenus.Var_d4ca_MenuShortcutKey;
						goto Label152;
					}
					break;

				// Alt + R
				case 0x1300:
					for (int i = 0; i < 16; i++)
					{
						// Instruction address 0x1403:0x3805, size: 5
						this.parent.GameData.Nations[i].Mood = (short)(this.parent.CAPI.RNG.Next(3) - 1); // -1 = Friendly, 0 = Neutral, 1 = Aggressive

						// Instruction address 0x1403:0x3816, size: 5
						this.parent.GameData.Nations[i].Policy = (short)(this.parent.CAPI.RNG.Next(3) - 1); // -1 = Perfectionist, 0 = Neutral, 1 = Expansionistic

						// Instruction address 0x1403:0x3827, size: 5
						this.parent.GameData.Nations[i].Ideology = (short)(this.parent.CAPI.RNG.Next(3) - 1); // -1 = Militaristic, 0 = Neutral, 1 = Civilized
					}

					// Instruction address 0x1403:0x3843, size: 5
					this.parent.Segment_1238.F0_1238_001e_ShowDialog(0x2003, 100, 80);
					break;

				// Alt + O
				case 0x1800:
					// Instruction address 0x1403:0x3a5a, size: 5
					this.parent.GameMenus.F0_2c84_0000_ShowTopMenu(playerID, unitID, 1);

					if (this.parent.GameMenus.Var_d4ca_MenuShortcutKey != -1)
					{
						command = this.parent.GameMenus.Var_d4ca_MenuShortcutKey;
						goto Label152;
					}
					break;

				// Alt + A
				case 0x1e00:
					// Instruction address 0x1403:0x3a6f, size: 5
					this.parent.GameMenus.F0_2c84_0000_ShowTopMenu(playerID, unitID, 2);

					if (this.parent.GameMenus.Var_d4ca_MenuShortcutKey != -1)
					{
						command = this.parent.GameMenus.Var_d4ca_MenuShortcutKey;
						goto Label152;
					}
					break;

				// Alt + D
				case 0x2000:
					this.parent.Var_d806_DebugFlag = !this.parent.Var_d806_DebugFlag;

					// Instruction address 0x1403:0x3506, size: 5
					this.parent.MapManagement.F0_2aea_0008_DrawVisibleMap(this.parent.GameData.HumanPlayerID, this.parent.Var_d4cc_MapViewX, this.parent.Var_d75e_MapViewY);
					break;

				// Alt + G
				case 0x2200:
					// Instruction address 0x1403:0x3a45, size: 5
					this.parent.GameMenus.F0_2c84_0000_ShowTopMenu(playerID, unitID, 0);

					if (this.parent.GameMenus.Var_d4ca_MenuShortcutKey != -1)
					{
						command = this.parent.GameMenus.Var_d4ca_MenuShortcutKey;
						goto Label152;
					}
					break;

				// Alt + H
				case 0x2300:
					this.parent.Help.F4_0000_02d3_ShowInstantAdvicePopup("*HELP1");
					this.parent.Help.F4_0000_02d3_ShowInstantAdvicePopup("*HELP2");
					break;

				// Alt + C
				case 0x2e00:
					// Instruction address 0x1403:0x3a99, size: 5
					this.parent.GameMenus.F0_2c84_0000_ShowTopMenu(playerID, unitID, 4);

					if (this.parent.GameMenus.Var_d4ca_MenuShortcutKey != -1)
					{
						command = this.parent.GameMenus.Var_d4ca_MenuShortcutKey;
						goto Label152;
					}
					break;

				// Alt + V
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
						this.parent.GameStatistics.F12_0000_080d_ShowWondersOfTheWorldReport();
					}
					break;

				// F8 - Top 5 Cities
				case 0x4200:
					if (this.parent.Var_d806_DebugFlag)
					{
						this.parent.GameStatistics.F12_0000_0573_ShowPowerGraphReport();
						this.parent.GameReplay.F9_0000_0000();
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
						this.parent.GameStatistics.F12_0000_03ac_ShowContinentStatsReport();
					}
					else
					{
						this.parent.Overlay_20.F20_0000_0ca9_ShowCivilizationScorePopup(this.parent.GameData.HumanPlayerID, true);
					}
					break;

				// F10 - World Map
				case 0x4400:
					this.parent.GameStatistics.F12_0000_0000_ShowWorldMapReport();
					break;

				// Shift + Home
				case 0x4737:
					local_24 = 8;

					direction = this.parent.MoveDirections[local_24];
					this.parent.Var_d4cc_MapViewX += direction.X * 4;
					this.parent.Var_d75e_MapViewY += direction.Y * 4;

					// Instruction address 0x1403:0x3506, size: 5
					this.parent.MapManagement.F0_2aea_0008_DrawVisibleMap(this.parent.GameData.HumanPlayerID, this.parent.Var_d4cc_MapViewX, this.parent.Var_d75e_MapViewY);
					break;

				// Shift + Up
				case 0x4838:
					local_24 = 1;

					direction = this.parent.MoveDirections[local_24];
					this.parent.Var_d4cc_MapViewX += direction.X * 4;
					this.parent.Var_d75e_MapViewY += direction.Y * 4;

					// Instruction address 0x1403:0x3506, size: 5
					this.parent.MapManagement.F0_2aea_0008_DrawVisibleMap(this.parent.GameData.HumanPlayerID, this.parent.Var_d4cc_MapViewX, this.parent.Var_d75e_MapViewY);
					break;

				// Shift + PageUp
				case 0x4939:
					local_24 = 2;

					direction = this.parent.MoveDirections[local_24];
					this.parent.Var_d4cc_MapViewX += direction.X * 4;
					this.parent.Var_d75e_MapViewY += direction.Y * 4;

					// Instruction address 0x1403:0x3506, size: 5
					this.parent.MapManagement.F0_2aea_0008_DrawVisibleMap(this.parent.GameData.HumanPlayerID, this.parent.Var_d4cc_MapViewX, this.parent.Var_d75e_MapViewY);
					break;

				// Shift + Left
				case 0x4b34:
					local_24 = 7;

					direction = this.parent.MoveDirections[local_24];
					this.parent.Var_d4cc_MapViewX += direction.X * 4;
					this.parent.Var_d75e_MapViewY += direction.Y * 4;

					// Instruction address 0x1403:0x3506, size: 5
					this.parent.MapManagement.F0_2aea_0008_DrawVisibleMap(this.parent.GameData.HumanPlayerID, this.parent.Var_d4cc_MapViewX, this.parent.Var_d75e_MapViewY);
					break;

				// Shift + Right
				case 0x4d36:
					local_24 = 3;

					direction = this.parent.MoveDirections[local_24];
					this.parent.Var_d4cc_MapViewX += direction.X * 4;
					this.parent.Var_d75e_MapViewY += direction.Y * 4;

					// Instruction address 0x1403:0x3506, size: 5
					this.parent.MapManagement.F0_2aea_0008_DrawVisibleMap(this.parent.GameData.HumanPlayerID, this.parent.Var_d4cc_MapViewX, this.parent.Var_d75e_MapViewY);
					break;

				// Shift + End
				case 0x4f31:
					local_24 = 6;

					direction = this.parent.MoveDirections[local_24];
					this.parent.Var_d4cc_MapViewX += direction.X * 4;
					this.parent.Var_d75e_MapViewY += direction.Y * 4;

					// Instruction address 0x1403:0x3506, size: 5
					this.parent.MapManagement.F0_2aea_0008_DrawVisibleMap(this.parent.GameData.HumanPlayerID, this.parent.Var_d4cc_MapViewX, this.parent.Var_d75e_MapViewY);
					break;

				// Shift + Down
				case 0x5032:
					local_24 = 5;

					direction = this.parent.MoveDirections[local_24];
					this.parent.Var_d4cc_MapViewX += direction.X * 4;
					this.parent.Var_d75e_MapViewY += direction.Y * 4;

					// Instruction address 0x1403:0x3506, size: 5
					this.parent.MapManagement.F0_2aea_0008_DrawVisibleMap(this.parent.GameData.HumanPlayerID, this.parent.Var_d4cc_MapViewX, this.parent.Var_d75e_MapViewY);
					break;

				// Shift + PageDown
				case 0x5133:
					local_24 = 4;

					direction = this.parent.MoveDirections[local_24];
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
								this.parent.UnitManagement.F0_1866_1331(playerID, unitID, UnitTypeEnum.Carrier) != 0)
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

					int Local_e = this.parent.GameTools.F0_2dc4_007c_CheckValueRange(this.parent.GameData.Players[playerID].Units[unitID].RemainingMoves / 3, 0, 99);
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
						Local_2 = this.parent.GameTools.F0_2dc4_00ba_GetCityByLocation(
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
		/// ?
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		public bool F0_1403_4508(int x, int y)
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
