using System.Text;

namespace OpenCivOne
{
	public class DiplomatActions
	{
		private OpenCivOneGame parent;

		public DiplomatActions(OpenCivOneGame parent)
		{
			this.parent = parent;
		}

		/// <summary>
		/// Determines which diplomat action to perform on a destination city
		/// </summary>
		/// <param name="diplomatPlayerID"></param>
		/// <param name="diplomatUnitID"></param>
		/// <param name="destinationCityID"></param>
		public void F22_0000_0000_DiplomatActionOnCity(int diplomatPlayerID, int diplomatUnitID, int destinationCityID)
		{
			//this.oCPU.Log.EnterBlock($"F22_0000_0000({cityID}, {playerID}, {unitID})");

			// function body
			int selectedAction;
			int cityPlayerID = this.parent.GameData.Cities[destinationCityID].PlayerID;
			bool barbariansInvolved = diplomatPlayerID == 0 || cityPlayerID == 0;

			this.parent.GameData.Players[diplomatPlayerID].Units[diplomatUnitID].GoToDestination = OpenCivOneGame.InvalidPosition;

			while (true)
			{
				if (diplomatPlayerID == this.parent.GameData.HumanPlayerID)
				{
					this.parent.Var_b276_MenuBoxDisabledOptions = 0;

					if (barbariansInvolved || this.parent.GameData.Players[diplomatPlayerID].Diplomacy[cityPlayerID].HasFlag(DiplomacyFlagsEnum.EmbassyEstablished))
					{
						this.parent.Var_b276_MenuBoxDisabledOptions = 0x1;
					}

					if (barbariansInvolved || (this.parent.GameData.Cities[destinationCityID].Status & CityStatusEnum.TechnologyStolen) != CityStatusEnum.None)
					{
						this.parent.Var_b276_MenuBoxDisabledOptions |= 0x4;
					}

					if (this.parent.GameData.Cities[destinationCityID].HasImprovement(ImprovementEnum.Palace))
					{
						this.parent.Var_b276_MenuBoxDisabledOptions |= 0x10;
					}

					if (barbariansInvolved)
					{
						this.parent.Var_b276_MenuBoxDisabledOptions |= 0x20;
					}

					// Instruction address 0x0000:0x00d2, size: 5
					selectedAction = this.parent.Segment_1238.F0_1238_001e_ShowDialog($"{this.parent.GameData.Players[diplomatPlayerID].Nationality} diplomat arrives\nin " +
						$"{this.parent.Segment_2459.F0_2459_08c6_GetCityName(destinationCityID)}, what would you like to do?\n Establish Embassy\n" +
						$" Investigate City\n Steal Technology\n Industrial Sabotage\n Incite a Revolt\n Meet with King\n", 80, 80);
				}
				else
				{
					// !!! To Do: The default AI action is to Steal Technology, but we will need to expand that later
					selectedAction = 2; // + this.oParent.CAPI.RNG.Next(2);

					if (barbariansInvolved || this.parent.GameData.Players[diplomatPlayerID].Diplomacy[cityPlayerID].HasFlag(DiplomacyFlagsEnum.Peace))
					{
						// Instruction address 0x0000:0x0141, size: 5
						this.parent.UnitManagement.F0_1866_0f10_DeleteUnit(diplomatPlayerID, diplomatUnitID);

						return;
					}
				}

				switch (selectedAction)
				{
					case 0:
						this.parent.GameData.Players[diplomatPlayerID].Diplomacy[cityPlayerID] |= DiplomacyFlagsEnum.EmbassyEstablished;

						// Instruction address 0x0000:0x0164, size: 5
						this.parent.UnitManagement.F0_1866_0f10_DeleteUnit(diplomatPlayerID, diplomatUnitID);

						this.parent.Overlay_14.F14_0000_1164(cityPlayerID);

						return;

					case 1:
						// Instruction address 0x0000:0x0133, size: 5
						this.parent.Segment_1ade.F0_1ade_03ea(destinationCityID);

						// Instruction address 0x0000:0x0141, size: 5
						this.parent.UnitManagement.F0_1866_0f10_DeleteUnit(diplomatPlayerID, diplomatUnitID);

						return;

					case 2:
						if (!barbariansInvolved && (this.parent.GameData.Cities[destinationCityID].Status & CityStatusEnum.TechnologyStolen) == CityStatusEnum.None)
						{
							if (diplomatPlayerID != this.parent.GameData.HumanPlayerID || this.parent.Segment_29f3.F0_29f3_0c9e_ConfirmAttackAction(cityPlayerID) != -1)
							{
								// Instruction address 0x0000:0x01c6, size: 5
								this.parent.Diplomacy.F0_2517_0aa1_ClearDiplomacyFlags(diplomatPlayerID, cityPlayerID, DiplomacyFlagsEnum.Peace);

								if (diplomatPlayerID != 0)
								{
									int technologyIndex = this.parent.CAPI.RNG.Next(72);

									for (int i = 0; i < 72; i++)
									{
										TechnologyAdvanceEnum selectedTechnology = (TechnologyAdvanceEnum)((technologyIndex + i) % 72);

										if (this.parent.Segment_1ade.F0_1ade_22b5_PlayerHasTechnology(cityPlayerID, selectedTechnology) &&
											!this.parent.Segment_1ade.F0_1ade_22b5_PlayerHasTechnology(diplomatPlayerID, selectedTechnology))
										{
											// Instruction address 0x0000:0x0249, size: 5
											this.parent.Segment_1ade.F0_1ade_1d2e(diplomatPlayerID, cityPlayerID, selectedTechnology);

											this.parent.Var_2f9e_MessageBoxStyle = MenuBoxReportTypeEnum.SpiesReport;

											// Instruction address 0x0000:0x02ab, size: 5
											this.parent.Segment_1238.F0_1238_001e_ShowDialog(
												$"{this.parent.GameData.Players[diplomatPlayerID].Nation} steal\n" +
												$"{this.parent.GameData.TechnologyAdvances[(int)selectedTechnology].Name}.\n", 80, 80);

											// !!! why are we creating a free City Wall in a destination city?
											this.parent.GameData.Cities[destinationCityID].Status |= CityStatusEnum.TechnologyStolen;

											// Instruction address 0x0000:0x0141, size: 5
											this.parent.UnitManagement.F0_1866_0f10_DeleteUnit(diplomatPlayerID, diplomatUnitID);

											return;
										}
									}
								}

								barbariansInvolved = true;

								if (diplomatPlayerID == this.parent.GameData.HumanPlayerID)
								{
									// Instruction address 0x0000:0x0393, size: 5
									this.parent.Segment_1238.F0_1238_001e_ShowDialog("No new technology found.", 80, 80);
								}
							}
						}
						break;

					case 3:
						if (this.parent.CAPI.RNG.Next(2) != 0 || this.parent.GameData.Cities[destinationCityID].ShieldsCount == 0)
						{
							for (int i = 0; i < 99; i++)
							{
								int improvementBit = 1 + this.parent.CAPI.RNG.Next(23);

								if (this.parent.GameData.Cities[destinationCityID].HasImprovement(City.BitToImprovementEnum(improvementBit)))
								{
									this.parent.GameData.Cities[destinationCityID].RemoveImprovement(City.BitToImprovementEnum(improvementBit));

									this.parent.Var_2f9e_MessageBoxStyle = MenuBoxReportTypeEnum.SpiesReport;

									// Instruction address 0x0000:0x0437, size: 5
									this.parent.Segment_1238.F0_1238_001e_ShowDialog(
										$"{this.parent.GameData.GetImprovementType(improvementBit + 1).Name} destroyed\n" +
										$"in {this.parent.Segment_2459.F0_2459_08c6_GetCityName(destinationCityID)}.\n", 80, 80);

									// Instruction address 0x0000:0x0141, size: 5
									this.parent.UnitManagement.F0_1866_0f10_DeleteUnit(diplomatPlayerID, diplomatUnitID);

									return;
								}
							}
						}

						this.parent.GameData.Cities[destinationCityID].ShieldsCount = 0;

						if (this.parent.GameData.Cities[destinationCityID].CurrentProductionID >= 0)
						{
							this.parent.Var_2f9e_MessageBoxStyle = MenuBoxReportTypeEnum.SpiesReport;

							// Instruction address 0x0000:0x0437, size: 5
							this.parent.Segment_1238.F0_1238_001e_ShowDialog(
								$"{this.parent.GameData.Units[this.parent.GameData.Cities[destinationCityID].CurrentProductionID].Name} production sabotaged\n" +
								$"in {this.parent.Segment_2459.F0_2459_08c6_GetCityName(destinationCityID)}.\n", 80, 80);
						}
						else
						{
							this.parent.Var_2f9e_MessageBoxStyle = MenuBoxReportTypeEnum.SpiesReport;

							// Instruction address 0x0000:0x0437, size: 5
							this.parent.Segment_1238.F0_1238_001e_ShowDialog(
								$"{this.parent.GameData.GetImprovementType(-this.parent.GameData.Cities[destinationCityID].CurrentProductionID).Name} production sabotaged\n" +
								$"in {this.parent.Segment_2459.F0_2459_08c6_GetCityName(destinationCityID)}.\n", 80, 80);
						}

						// Instruction address 0x0000:0x0141, size: 5
						this.parent.UnitManagement.F0_1866_0f10_DeleteUnit(diplomatPlayerID, diplomatUnitID);

						return;

					case 4:
						// Instruction address 0x0000:0x0459, size: 5
						int local_c = this.parent.Diplomacy.F0_2517_0d97_GetMinimumDistanceFromPalace(cityPlayerID, this.parent.GameData.Cities[destinationCityID].Position.X, this.parent.GameData.Cities[destinationCityID].Position.Y);

						if (local_c != 0)
						{
							int actionCost = ((this.parent.GameData.Players[cityPlayerID].Coins + 1000) / (local_c + 3)) * this.parent.GameData.Cities[destinationCityID].ActualSize;

							if ((this.parent.GameData.Cities[destinationCityID].Status & CityStatusEnum.CivilDisorder) != CityStatusEnum.None)
							{
								actionCost /= 2;
							}

							StringBuilder menuText = new();
							menuText.Append($"Dissidents in {this.parent.Segment_2459.F0_2459_08c6_GetCityName(destinationCityID)}\nwill revolt for {actionCost} coins.\n");

							if (actionCost <= this.parent.GameData.Players[diplomatPlayerID].Coins)
							{
								// Instruction address 0x0000:0x0517, size: 5
								menuText.Append(" Forget it\n Incite revolt\n");

								if (actionCost * 2 <= this.parent.GameData.Players[diplomatPlayerID].Coins)
								{
									if (this.parent.GameData.Players[diplomatPlayerID].Diplomacy[cityPlayerID].HasFlag(DiplomacyFlagsEnum.Peace))
									{
										// Instruction address 0x0000:0x0543, size: 5
										menuText.Append($" Subvert city for {actionCost * 2} coins\n");
									}
								}
							}

							this.parent.Var_2f9e_MessageBoxStyle = MenuBoxReportTypeEnum.SpiesReport;

							// Instruction address 0x0000:0x055d, size: 5
							selectedAction = this.parent.Segment_1238.F0_1238_001e_ShowDialog(menuText.ToString(), 100, 80);

							if (selectedAction >= 1)
							{
								if (selectedAction == 1)
								{
									if (this.parent.GameData.Players[cityPlayerID].Diplomacy[diplomatPlayerID].HasFlag(DiplomacyFlagsEnum.Peace))
									{
										// Instruction address 0x0000:0x0593, size: 5
										this.parent.Diplomacy.F0_2517_0aa1_ClearDiplomacyFlags(cityPlayerID, diplomatPlayerID, DiplomacyFlagsEnum.Peace);

										this.parent.GameData.Players[cityPlayerID].Diplomacy[diplomatPlayerID] |= DiplomacyFlagsEnum.Vendetta;
									}
								}

								this.parent.GameData.Players[diplomatPlayerID].Coins -= (short)(actionCost * selectedAction);

								F22_0000_0af5_InciteCityRevolt(destinationCityID, diplomatPlayerID);

								// Instruction address 0x0000:0x05ce, size: 5
								this.parent.MapManagement.F0_2aea_11d4_DrawCellWithUnit(this.parent.GameData.Cities[destinationCityID].Position.X, this.parent.GameData.Cities[destinationCityID].Position.Y);

								// Instruction address 0x0000:0x05dc, size: 5
								this.parent.UnitManagement.F0_1866_0f10_DeleteUnit(diplomatPlayerID, diplomatUnitID);

								// Instruction address 0x0000:0x05e4, size: 5
								this.parent.Segment_1238.F0_1238_107e();
							}
						}

						return;

					case 5:
						if (cityPlayerID != 0)
						{
							this.parent.MeetWithKing.F6_0000_0000(cityPlayerID,
								this.parent.GameData.Players[diplomatPlayerID].Units[diplomatUnitID].Position.X, this.parent.GameData.Players[diplomatPlayerID].Units[diplomatUnitID].Position.Y, 1);
						}

						this.parent.GameData.Players[diplomatPlayerID].Units[diplomatUnitID].RemainingMoves = 0;
						return;

					default:
						return;
				}
			}
		}

		/// <summary>
		/// Perform diplomat action on a Unit (only one unit allowed)
		/// !!! To Do: Allow a unit from a stack to be bribed.
		/// </summary>
		/// <param name="diplomatPlayerID">The Player ID that is performing the diplomat action</param>
		/// <param name="destinationPlayerID">The Player ID of the player on which the action is being performed</param>
		/// <param name="destinationUnitID">The Unit ID of the player on which the action is being performed</param>
		public void F22_0000_0639_DiplomatInteractionWithUnit(int diplomatPlayerID,int diplomatUnitID, int destinationPlayerID, int destinationUnitID)
		{
			// this.oCPU.Log.EnterBlock($"F22_0000_0639({playerID}, {unitID}, {playerID1})");

			// function body
			if (this.parent.UnitManagement.F0_1866_1251_GetStackUnitValueSum(destinationPlayerID, destinationUnitID, UnitStackValueTypeEnum.UnitCount) <= 1)
			{
				// Instruction address 0x0000:0x067a, size: 5
				int local_8 = this.parent.Diplomacy.F0_2517_0d97_GetMinimumDistanceFromPalace(destinationPlayerID,
					this.parent.GameData.Players[destinationPlayerID].Units[destinationUnitID].Position.X, this.parent.GameData.Players[destinationPlayerID].Units[destinationUnitID].Position.Y);

				int actionCost = ((this.parent.GameData.Players[destinationPlayerID].Coins + 750) / (local_8 + 2)) *
					this.parent.GameData.Units[(int)this.parent.GameData.Players[destinationPlayerID].Units[destinationUnitID].UnitType].Cost;

				if (this.parent.GameData.Players[destinationPlayerID].Units[destinationUnitID].UnitType != UnitTypeEnum.Settler)
				{
					actionCost /= 2;
				}

				if (this.parent.GameData.Players[diplomatPlayerID].Coins >= actionCost)
				{
					if (diplomatPlayerID == this.parent.GameData.HumanPlayerID)
					{
						// Instruction address 0x0000:0x06e3, size: 5
						this.parent.CAPI.strcpy(0xba06, $"{this.parent.GameData.Players[destinationPlayerID].Nationality} " +
							$"{this.parent.GameData.Units[(int)this.parent.GameData.Players[destinationPlayerID].Units[destinationUnitID].UnitType].Name}\n" +
							$"will desert for {actionCost} coins.\nOur treasury currently contains {this.parent.GameData.Players[diplomatPlayerID].Coins} coins.\n");

						if (this.parent.GameData.Players[diplomatPlayerID].Coins >= actionCost)
						{
							// Instruction address 0x0000:0x07a8, size: 5
							this.parent.CAPI.strcat(0xba06, " Cancel action\n Bribe unit\n");
						}

						if (this.parent.Segment_1238.F0_1238_001e_ShowDialog(0xba06, 100, 80) != 1)
							return;
					}

					if (destinationPlayerID == this.parent.GameData.HumanPlayerID)
					{
						// Instruction address 0x0000:0x0806, size: 5
						this.parent.MapManagement.F0_2aea_0008_DrawVisibleMap(destinationPlayerID,
							this.parent.GameData.Players[destinationPlayerID].Units[destinationUnitID].Position.X - 8, this.parent.GameData.Players[destinationPlayerID].Units[destinationUnitID].Position.Y - 6);

						this.parent.Var_2f9e_MessageBoxStyle = MenuBoxReportTypeEnum.SpiesReport;

						// Instruction address 0x0000:0x0890, size: 5
						this.parent.Segment_1238.F0_1238_001e_ShowDialog(
							$"{this.parent.GameData.Players[destinationPlayerID].Nationality} " +
							$"{this.parent.GameData.Units[(int)this.parent.GameData.Players[destinationPlayerID].Units[destinationUnitID].UnitType].Name} unit\nbribed by " +
							$"{this.parent.GameData.Players[diplomatPlayerID].Nation}!\n", 80, 40);
					}

					UnitTypeEnum unitType = this.parent.GameData.Players[destinationPlayerID].Units[destinationUnitID].UnitType;

					// Instruction address 0x0000:0x08b6, size: 5
					this.parent.UnitManagement.F0_1866_0f10_DeleteUnit(destinationPlayerID, destinationUnitID);

					this.parent.GameData.Players[destinationPlayerID].ActiveUnits[(int)unitType]--;

					// Instruction address 0x0000:0x08e1, size: 5
					int newUnitID = this.parent.UnitManagement.F0_1866_0cf5_CreateUnit(diplomatPlayerID, unitType,
						this.parent.GameData.Players[destinationPlayerID].Units[destinationUnitID].Position.X, this.parent.GameData.Players[destinationPlayerID].Units[destinationUnitID].Position.Y);

					if (newUnitID != -1)
					{
						this.parent.GameData.Players[diplomatPlayerID].ActiveUnits[(int)unitType]++;

						this.parent.GameData.Players[diplomatPlayerID].Units[newUnitID].VisibleByPlayer |= (ushort)(1 << destinationPlayerID);
					}

					this.parent.GameData.Players[diplomatPlayerID].Coins -= (short)actionCost;

					if (this.parent.Var_d806_DebugFlag || destinationPlayerID == this.parent.GameData.HumanPlayerID)
					{
						// Instruction address 0x0000:0x0948, size: 5
						this.parent.MapManagement.F0_2aea_11d4_DrawCellWithUnit(this.parent.GameData.Players[destinationPlayerID].Units[destinationUnitID].Position.X,
							this.parent.GameData.Players[destinationPlayerID].Units[destinationUnitID].Position.Y);

						// Instruction address 0x0000:0x0950, size: 5
						this.parent.Segment_1238.F0_1238_107e();

						// Instruction address 0x0000:0x0959, size: 5
						this.parent.CommonTools.WaitTimer(30);
					}
				}
			}
			return;
		}

		/// <summary>
		/// Incite revolt or subvert a city
		/// </summary>
		/// <param name="destinationCityID"></param>
		/// <param name="actionPlayerID"></param>
		public void F22_0000_0af5_InciteCityRevolt(int destinationCityID, int actionPlayerID)
		{
			//this.oCPU.Log.EnterBlock($"F22_0000_0af5({cityID}, {playerID})");

			// function body
			int cityPlayerID = this.parent.GameData.Cities[destinationCityID].PlayerID;
			int cityX = this.parent.GameData.Cities[destinationCityID].Position.X;
			int cityY = this.parent.GameData.Cities[destinationCityID].Position.Y;

			if (actionPlayerID == -1)
			{
				int selectedCityID = -1;
				int bestCityScore = 0;

				for (int i = 0; i < 128 && i != destinationCityID && this.parent.GameData.Cities[i].Status != CityStatusEnum.Invalid &&
					this.parent.GameData.Cities[i].PlayerID != cityPlayerID; i++)
				{
					// Instruction address 0x0000:0x0b67, size: 5
					int cityDistance = this.parent.MapManagement.GetDistance(this.parent.GameData.Cities[destinationCityID].Position, this.parent.GameData.Cities[i].Position);

					// Instruction address 0x0000:0x0b79, size: 5
					this.parent.CityWorker.F0_1d12_0045_ProcessCityState(i, -1);

					int cityScore = ((this.parent.Var_70e2_CityHappyCitizenCount - this.parent.Var_70e4_CityUnhappyCitizenCount) * 32) / cityDistance;

					if (cityScore > bestCityScore)
					{
						bestCityScore = cityScore;
						selectedCityID = i;
					}
				}

				if (selectedCityID != -1 && bestCityScore > 0)
				{
					actionPlayerID = this.parent.GameData.Cities[selectedCityID].PlayerID;
				}
			}

			if (actionPlayerID != -1)
			{
				this.parent.GameData.MapVisibility[cityX, cityY] |= (ushort)(1 << actionPlayerID);

				for (int i = 0; i < 20; i++)
				{
					int newX = this.parent.MapManagement.AdjustXPosition(this.parent.CityOffsets[i].X + cityX);
					int newY = this.parent.CityOffsets[i].Y + cityY;

					if (this.parent.MapManagement.ValidateMapCoordinates(newX, newY))
					{
						this.parent.GameData.MapVisibility[newX, newY] |= (ushort)(1 << actionPlayerID);
					}
				}

				if (cityPlayerID == this.parent.GameData.HumanPlayerID || actionPlayerID == this.parent.GameData.HumanPlayerID ||
					this.parent.GameData.Players[this.parent.GameData.HumanPlayerID].Diplomacy[cityPlayerID].HasFlag(DiplomacyFlagsEnum.EmbassyEstablished) ||
					this.parent.GameData.Players[this.parent.GameData.HumanPlayerID].Diplomacy[actionPlayerID].HasFlag(DiplomacyFlagsEnum.EmbassyEstablished))
				{
					if ((this.parent.GameData.MapVisibility[cityX, cityY] & (0x1 << this.parent.GameData.HumanPlayerID)) != 0)
					{
						// Instruction address 0x0000:0x0cb6, size: 5
						this.parent.MapManagement.F0_2aea_0008_DrawVisibleMap(this.parent.GameData.HumanPlayerID, cityX - 8, cityY - 5);
					}

					if (actionPlayerID == 0)
					{
						// Instruction address 0x0000:0x0d45, size: 5
						this.parent.Segment_1238.F0_1238_001e_ShowDialog(
							$"{this.parent.GameData.Players[cityPlayerID].Nation} declare\nindependence in\n" +
							$"{this.parent.Segment_2459.F0_2459_08c6_GetCityName(destinationCityID)}\n", 100, 32);
					}
					else
					{
						// Instruction address 0x0000:0x0d45, size: 5
						this.parent.Segment_1238.F0_1238_001e_ShowDialog(
							$"{this.parent.GameData.Players[cityPlayerID].Nation} rebel!\nCivil War in\n" +
							$"{this.parent.Segment_2459.F0_2459_08c6_GetCityName(destinationCityID)}.\n" +
							$"{this.parent.GameData.Players[actionPlayerID].Nationality} influence\nsuspected.\n", 100, 32);
					}
				}

				for (int i = 0; i < 128; i++)
				{
					if (this.parent.GameData.Players[cityPlayerID].Units[i].UnitType != UnitTypeEnum.None)
					{
						// Instruction address 0x0000:0x0d74, size: 5
						int unitDistance = this.parent.MapManagement.GetDistance(cityX, cityY, this.parent.GameData.Players[cityPlayerID].Units[i].Position);

						if (unitDistance <= 1 && (unitDistance == 0 ||
							!this.parent.MapManagement.F0_2aea_1585_GetVisibleTerrainImprovements(this.parent.GameData.Players[cityPlayerID].Units[i].Position.X,
								this.parent.GameData.Players[cityPlayerID].Units[i].Position.Y).HasFlag(TerrainImprovementFlagsEnum.City)))
						{
							UnitTypeEnum unitType = this.parent.GameData.Players[cityPlayerID].Units[i].UnitType;

							// Instruction address 0x0000:0x0dce, size: 5
							this.parent.UnitManagement.F0_1866_0f10_DeleteUnit(cityPlayerID, i);

							// Instruction address 0x0000:0x0de5, size: 5
							this.parent.MapManagement.F0_2aea_138c_SetCityOwner(this.parent.GameData.Players[cityPlayerID].Units[i].Position.X,
								this.parent.GameData.Players[cityPlayerID].Units[i].Position.Y,
								actionPlayerID);

							// Instruction address 0x0000:0x0df9, size: 5
							this.parent.MapManagement.F0_2aea_1511_ActiveUnitSetFlag8(
								this.parent.GameData.Players[cityPlayerID].Units[i].Position.X,
								this.parent.GameData.Players[cityPlayerID].Units[i].Position.Y);

							// Instruction address 0x0000:0x0e13, size: 5
							int newUnitID = this.parent.UnitManagement.F0_1866_0cf5_CreateUnit(actionPlayerID, unitType,
								this.parent.GameData.Players[cityPlayerID].Units[i].Position.X, this.parent.GameData.Players[cityPlayerID].Units[i].Position.Y);

							if (newUnitID != -1)
							{
								this.parent.GameData.Players[actionPlayerID].Units[newUnitID].VisibleByPlayer |= (ushort)(1 << cityPlayerID);
								this.parent.GameData.Players[actionPlayerID].Units[newUnitID].HomeCityID = (short)destinationCityID;
							}
						}
					}
				}

				int gameSettings = this.parent.GameData.GameSettingFlags.Value;
				this.parent.Var_28bc = true;

				if (this.parent.Var_deba < 2)
				{
					//this.oParent.CivState.GameSettingFlags.Value &= 0x7ff7;
					this.parent.GameData.GameSettingFlags.Animations = false;
				}

				// Instruction address 0x0000:0x0e76, size: 5
				this.parent.Segment_2459.F0_2459_0000_CityTakeover(actionPlayerID, destinationCityID, true);

				this.parent.Var_28bc = false;
				this.parent.GameData.GameSettingFlags.Value = gameSettings;

				// Instruction address 0x0000:0x0ea1, size: 5
				this.parent.MapManagement.F0_2aea_138c_SetCityOwner(this.parent.GameData.Cities[destinationCityID].Position.X,
					this.parent.GameData.Cities[destinationCityID].Position.Y, actionPlayerID);
			}
		}
	}
}
