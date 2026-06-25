using System.Text;

namespace OpenCivOne
{
	public class ShowDebugDetails
	{
		private OpenCivOneGame oParent;

		public ShowDebugDetails(OpenCivOneGame parent)
		{
			this.oParent = parent;
		}

		/// <summary>
		/// Shows a dialog to select the player
		/// </summary>
		public void ShowMapWithStrategy()
		{
			//this.oCPU.Log.EnterBlock("F10_0000_0000()");

			// function body
			StringBuilder playerList = new();

			playerList.Append("Which army?\n ");

			for (int i = 1; i < 8; i++)
			{
				playerList.Append($"{this.oParent.GameData.Players[i].Nationality}\n ");
			}

			int selectedPlayerID = this.oParent.Segment_1238.F0_1238_001e_ShowDialog(playerList.ToString(), 100, 80) + 1;

			this.oParent.Graphics.F0_VGA_07d8_DrawImage(this.oParent.Var_aa_Screen0_Rectangle, 0, 0, 320, 200, this.oParent.Var_19d4_Screen1_Rectangle, 0, 0);

			this.oParent.DrawTools.FillRectangle(this.oParent.Var_aa_Screen0_Rectangle, 0, 0, 320, 200, 0);

			int playerBitMask = 0x1 << selectedPlayerID;

			for (int i = 0; i < 50; i++)
			{
				for (int j = 0; j < 80; j++)
				{
					if ((this.oParent.GameData.MapVisibility[j, i] & playerBitMask) != 0 || this.oParent.Var_d806_DebugFlag)
					{
						int x = j * 4;
						int y = i * 4;

						if (this.oParent.MapManagement.GetTerrainType(j, i) != TerrainTypeEnum.Water)
						{
							// Instruction address 0x0000:0x00c6, size: 5
							this.oParent.DrawTools.FillRectangle(this.oParent.Var_aa_Screen0_Rectangle, x, y, 4, 4, 2);
						}
						else
						{
							// Instruction address 0x0000:0x00c6, size: 5
							this.oParent.DrawTools.FillRectangle(this.oParent.Var_aa_Screen0_Rectangle, x, y, 4, 4, 1);
						}

						if (this.oParent.MapManagement.F0_2aea_1585_GetVisibleTerrainImprovements(j, i).HasFlag(TerrainImprovementFlagsEnum.City))
						{
							int cityPlayerID = this.oParent.MapManagement.F0_2aea_1369_GetCityOwner(j, i);

							// Instruction address 0x0000:0x0159, size: 5
							this.oParent.DrawTools.FillRectangle(this.oParent.Var_aa_Screen0_Rectangle, x, y, 4, 4, this.oParent.Array_1946_PlayerColours[cityPlayerID]);
						}
						else
						{
							int unitPlayerID = this.oParent.MapManagement.F0_2aea_14e0_GetCellUnitPlayerID(j, i);

							if (unitPlayerID != -1)
							{
								// Instruction address 0x0000:0x013f, size: 5
								this.oParent.DrawTools.FillRectangle(this.oParent.Var_aa_Screen0_Rectangle, x + 1, y + 1, 3, 3, 0);

								// Instruction address 0x0000:0x0159, size: 5
								this.oParent.DrawTools.FillRectangle(this.oParent.Var_aa_Screen0_Rectangle, x, y, 3, 3, this.oParent.Array_1946_PlayerColours[unitPlayerID]);
							}
						}
					}
				}
			}

			// Instruction address 0x0000:0x01ce, size: 5
			this.oParent.CAPI.getch();

			this.oParent.Var_aa_Screen0_Rectangle.FontID = 2;

			for (int i = 0; i < 32; i++)
			{
				switch (this.oParent.GameData.Players[selectedPlayerID].UnitPolicies[i].UnitRoleType)
				{
					case UnitRoleTypeEnum.None:
						break;

					case UnitRoleTypeEnum.Settler:
						this.oParent.DrawTools.F0_1182_0086_DrawStringWithShadow("S",
							this.oParent.GameData.Players[selectedPlayerID].UnitPolicies[i].Position.X * 4,
							this.oParent.GameData.Players[selectedPlayerID].UnitPolicies[i].Position.Y * 4, 13);
						break;

					case UnitRoleTypeEnum.LandAttack:
						this.oParent.DrawTools.F0_1182_0086_DrawStringWithShadow("LA",
							this.oParent.GameData.Players[selectedPlayerID].UnitPolicies[i].Position.X * 4 - 2,
							this.oParent.GameData.Players[selectedPlayerID].UnitPolicies[i].Position.Y * 4, 13);
						break;

					case UnitRoleTypeEnum.Defense:
						this.oParent.DrawTools.F0_1182_0086_DrawStringWithShadow("D",
							this.oParent.GameData.Players[selectedPlayerID].UnitPolicies[i].Position.X * 4,
							this.oParent.GameData.Players[selectedPlayerID].UnitPolicies[i].Position.Y * 4, 13);
						break;

					case UnitRoleTypeEnum.SeaAttack:
						this.oParent.DrawTools.F0_1182_0086_DrawStringWithShadow("SA",
							this.oParent.GameData.Players[selectedPlayerID].UnitPolicies[i].Position.X * 4 - 2,
							this.oParent.GameData.Players[selectedPlayerID].UnitPolicies[i].Position.Y * 4, 13);
						break;

					case UnitRoleTypeEnum.AirAttack:
						this.oParent.DrawTools.F0_1182_0086_DrawStringWithShadow("AA",
							this.oParent.GameData.Players[selectedPlayerID].UnitPolicies[i].Position.X * 4 - 2,
							this.oParent.GameData.Players[selectedPlayerID].UnitPolicies[i].Position.Y * 4, 13);
						break;

					case UnitRoleTypeEnum.SeaTransport:
						this.oParent.DrawTools.F0_1182_0086_DrawStringWithShadow("ST",
							this.oParent.GameData.Players[selectedPlayerID].UnitPolicies[i].Position.X * 4 - 2,
							this.oParent.GameData.Players[selectedPlayerID].UnitPolicies[i].Position.Y * 4, 13);
						break;

					case UnitRoleTypeEnum.Neutral:
						this.oParent.DrawTools.F0_1182_0086_DrawStringWithShadow("N",
							this.oParent.GameData.Players[selectedPlayerID].UnitPolicies[i].Position.X * 4,
							this.oParent.GameData.Players[selectedPlayerID].UnitPolicies[i].Position.Y * 4, 13);
						break;
				}
			}

			for (int i = 0; i < 16; i++)
			{
				switch (this.oParent.GameData.Players[selectedPlayerID].ContinentPolicies[i].UnitRoleType)
				{
					case UnitRoleTypeEnum.None:
						break;

					case UnitRoleTypeEnum.Settler:
						// Settle?
						this.oParent.DrawTools.F0_1182_0086_DrawStringWithShadow("S",
							this.oParent.GameData.Players[selectedPlayerID].ContinentPolicies[i].Position.X * 4,
							this.oParent.GameData.Players[selectedPlayerID].ContinentPolicies[i].Position.Y * 4, 14);
						break;

					case UnitRoleTypeEnum.LandAttack:
						// Attack?
						this.oParent.DrawTools.F0_1182_0086_DrawStringWithShadow("LA",
							this.oParent.GameData.Players[selectedPlayerID].ContinentPolicies[i].Position.X * 4 - 2,
							this.oParent.GameData.Players[selectedPlayerID].ContinentPolicies[i].Position.Y * 4, 14);
						break;

					case UnitRoleTypeEnum.Defense:
						// Defend ?
						this.oParent.DrawTools.F0_1182_0086_DrawStringWithShadow("D",
							this.oParent.GameData.Players[selectedPlayerID].ContinentPolicies[i].Position.X * 4,
							this.oParent.GameData.Players[selectedPlayerID].ContinentPolicies[i].Position.Y * 4, 14);
						break;

					case UnitRoleTypeEnum.SeaAttack:
						this.oParent.DrawTools.F0_1182_0086_DrawStringWithShadow("SA",
							this.oParent.GameData.Players[selectedPlayerID].ContinentPolicies[i].Position.X * 4 - 2,
							this.oParent.GameData.Players[selectedPlayerID].ContinentPolicies[i].Position.Y * 4, 14);
						break;

					case UnitRoleTypeEnum.AirAttack:
						this.oParent.DrawTools.F0_1182_0086_DrawStringWithShadow("AA",
							this.oParent.GameData.Players[selectedPlayerID].ContinentPolicies[i].Position.X * 4 - 2,
							this.oParent.GameData.Players[selectedPlayerID].ContinentPolicies[i].Position.Y * 4, 14);
						break;

					case UnitRoleTypeEnum.SeaTransport:
						this.oParent.DrawTools.F0_1182_0086_DrawStringWithShadow("ST",
							this.oParent.GameData.Players[selectedPlayerID].ContinentPolicies[i].Position.X * 4 - 2,
							this.oParent.GameData.Players[selectedPlayerID].ContinentPolicies[i].Position.Y * 4, 14);
						break;

					case UnitRoleTypeEnum.Neutral:
						this.oParent.DrawTools.F0_1182_0086_DrawStringWithShadow("N",
							this.oParent.GameData.Players[selectedPlayerID].ContinentPolicies[i].Position.X * 4,
							this.oParent.GameData.Players[selectedPlayerID].ContinentPolicies[i].Position.Y * 4, 14);
						break;
				}
			}

			// Instruction address 0x0000:0x02e2, size: 5
			this.oParent.CAPI.getch();

			int yText = 6;

			for (int i = 1; i < 15; i++)
			{
				if (this.oParent.GameData.Players[selectedPlayerID].Continents[i].Attack != 0)
				{
					string strategyText = "";

					switch (this.oParent.GameData.Players[selectedPlayerID].Continents[i].Strategy)
					{
						case PlayerContinentStrategyEnum.Settle:
							strategyText = "S";
							break;

						case PlayerContinentStrategyEnum.Attack:
							strategyText = "A";
							break;

						case PlayerContinentStrategyEnum.Defend:
							strategyText = "D";
							break;

						case PlayerContinentStrategyEnum.Transport:
							strategyText = "T";
							break;

						default:
							throw new Exception($"Invalid Player Continent Strategy: {this.oParent.GameData.Players[selectedPlayerID].Continents[i].Strategy}");
					}

					// Instruction address 0x0000:0x0447, size: 5
					this.oParent.DrawTools.F0_1182_005c_DrawStringToScreen0(
						$"{i}/{strategyText} {this.oParent.GameData.Players[selectedPlayerID].Continents[i].Defense}/{this.oParent.GameData.Players[selectedPlayerID].Continents[i].Attack}",
						2, yText, 7);

					yText += 6;
				}
			}

			this.oParent.CAPI.getch();

			this.oParent.Var_aa_Screen0_Rectangle.FontID = 1;

			this.oParent.Graphics.F0_VGA_07d8_DrawImage(this.oParent.Var_19d4_Screen1_Rectangle, 0, 0, 320, 200, this.oParent.Var_aa_Screen0_Rectangle, 0, 0);
		}

		/// <summary>
		/// ?
		/// </summary>
		/// <param name="playerID"></param>
		/// <param name="unitID"></param>
		public void ShowUnitStatus(int playerID, int unitID)
		{
			//this.oCPU.Log.EnterBlock($"F10_0000_0477({playerID}, {unitID})");

			// function body
			StringBuilder unitStatus = new();

			unitStatus.Append($"{this.oParent.GameData.Players[playerID].Nationality}\n({unitID})/" +
				$"{this.oParent.GameData.Units[(int)this.oParent.GameData.Players[playerID].Units[unitID].UnitType].Name}\n" +
				$"{this.oParent.Segment_2459.F0_2459_08c6_GetCityName(this.oParent.GameData.Players[playerID].Units[unitID].HomeCityID)}\n" +
				$"Loc:{this.oParent.GameData.Players[playerID].Units[unitID].Position.X},{this.oParent.GameData.Players[playerID].Units[unitID].Position.Y}\n");

			if (this.oParent.GameData.Players[playerID].Units[unitID].GoToDestination.X != -1)
			{
				unitStatus.Append($"To:{this.oParent.GameData.Players[playerID].Units[unitID].GoToDestination.X},"+
					$"{this.oParent.GameData.Players[playerID].Units[unitID].GoToDestination.Y}\n");
			}
		
			int nextUnitID = this.oParent.GameData.Players[playerID].Units[unitID].NextUnitID;

			while (nextUnitID != -1 && nextUnitID != unitID && unitStatus.Length < 512)
			{
				// Instruction address 0x0000:0x066b, size: 5
				unitStatus.Append($"*{this.oParent.GameData.Units[(int)this.oParent.GameData.Players[playerID].Units[nextUnitID].UnitType].Name}\n");

				nextUnitID = this.oParent.GameData.Players[playerID].Units[nextUnitID].NextUnitID;
			}

			// Instruction address 0x0000:0x0699, size: 5
			this.oParent.Segment_1238.F0_1238_001e_ShowDialog(unitStatus.ToString(), 10, 10);
		}

		/// <summary>
		/// Prints debug details for all players
		/// </summary>
		/// <param name="playerID"></param>
		public void ShowPlayerDetails(int playerID)
		{
			//this.oCPU.Log.EnterBlock($"F13_0000_0000({playerID})");

			// function body
			// Instruction address 0x0000:0x0024, size: 5
			this.oParent.Graphics.F0_VGA_07d8_DrawImage(this.oParent.Var_aa_Screen0_Rectangle, 0, 0, 320, 200, this.oParent.Var_19d4_Screen1_Rectangle, 0, 0);

			// Instruction address 0x0000:0x0040, size: 5
			this.oParent.DrawTools.FillRectangle(this.oParent.Var_aa_Screen0_Rectangle, 0, 0, 320, 200, 1);

			// Instruction address 0x0000:0x005d, size: 5
			this.oParent.DrawTools.F0_1182_005c_DrawStringToScreen0(this.oParent.GameData.Players[playerID].Nationality, 128, 4, 15);

			// Instruction address 0x0000:0x0078, size: 5
			this.oParent.Graphics.F0_VGA_0c3e_DrawBitmapToScreen(this.oParent.Var_aa_Screen0_Rectangle, 16, 4, this.oParent.Array_d4ce[64 + (playerID * 32)]);

			// Instruction address 0x0000:0x00b6, size: 5
			this.oParent.DrawTools.F0_1182_005c_DrawStringToScreen0($"Government: {this.oParent.Array_1966[this.oParent.GameData.Players[playerID].GovernmentType]}", 16, 24, 15);

			// Instruction address 0x0000:0x00e9, size: 5
			this.oParent.DrawTools.F0_1182_005c_DrawStringToScreen0(
				$"Population: {this.oParent.GameTools.F0_2dc4_0337_PopulationValueToString(this.oParent.GameTools.F0_2dc4_02cd_GetPlayerTotalPopulationCount(playerID))}", 16, 32, 15);

			// Instruction address 0x0000:0x0194, size: 5
			this.oParent.DrawTools.F0_1182_005c_DrawStringToScreen0($"Treasury: {this.oParent.GameData.Players[playerID].Coins} coins, " +
				$"Taxes/Science: {this.oParent.GameData.Players[playerID].TaxRate}/{this.oParent.GameData.Players[playerID].ScienceTaxRate}", 16, 40, 15);

			// Instruction address 0x0000:0x01ac, size: 5
			this.oParent.DrawTools.F0_1182_005c_DrawStringToScreen0(
				$"{this.oParent.GameData.Players[playerID].UnitCount} units / " +
				$"{this.oParent.GameData.Players[playerID].CityCount} cities / " +
				$"{this.oParent.GameData.Players[playerID].SettlerCount} settlers / " +
				$"{this.oParent.GameData.Players[playerID].MapCellCount} land", 16, 48, 15);

			int yOffset = 4;

			for (int i = 1; i < 15; i++)
			{
				if (this.oParent.GameData.Players[playerID].Continents[i].Attack != 0)
				{
					switch (this.oParent.GameData.Players[playerID].Continents[i].Strategy)
					{
						case PlayerContinentStrategyEnum.Settle:
							this.oParent.DrawTools.F0_1182_005c_DrawStringToScreen0($"Settle/{i}", 256, yOffset, 7);
							yOffset += 8;
							break;

						case PlayerContinentStrategyEnum.Attack:
							this.oParent.DrawTools.F0_1182_005c_DrawStringToScreen0($"Attack/{i}", 256, yOffset, 7);
							yOffset += 8;
							break;

						case PlayerContinentStrategyEnum.Defend:
							this.oParent.DrawTools.F0_1182_005c_DrawStringToScreen0($"Defend/{i}", 256, yOffset, 7);
							yOffset += 8;
							break;

						case PlayerContinentStrategyEnum.Transport:
							this.oParent.DrawTools.F0_1182_005c_DrawStringToScreen0($"Transport/{i}", 256, yOffset, 7);
							yOffset += 8;
							break;

						default:
							throw new Exception($"Invalid Player Continent Strategy: {this.oParent.GameData.Players[playerID].Continents[i].Strategy}");
					}
				}
			}

			yOffset = 24;

			for (int i = 1; i < 8; i++)
			{
				if (i != playerID && (this.oParent.GameData.Players[playerID].Diplomacy[i] & (DiplomacyFlagsEnum.Contact | DiplomacyFlagsEnum.Peace | DiplomacyFlagsEnum.Vendetta)) != DiplomacyFlagsEnum.None)
				{
					StringBuilder diplomacyText = new();

					diplomacyText.Append(this.oParent.GameData.Players[i].Nationality);

					if ((this.oParent.GameData.Players[playerID].Diplomacy[i] & (DiplomacyFlagsEnum.Contact | DiplomacyFlagsEnum.Peace | DiplomacyFlagsEnum.Vendetta)) == DiplomacyFlagsEnum.Contact)
					{
						diplomacyText.Append(": Contact");
					}

					if (this.oParent.GameData.Players[playerID].Diplomacy[i].HasFlag(DiplomacyFlagsEnum.Peace))
					{
						if (!this.oParent.GameData.Players[playerID].Diplomacy[i].HasFlag(DiplomacyFlagsEnum.Allied))
						{
							diplomacyText.Append(": Peace");
						}
						else
						{
							diplomacyText.Append(": Allied");
						}
					}

					if (this.oParent.GameData.Players[playerID].Diplomacy[i].HasFlag(DiplomacyFlagsEnum.Vendetta))
					{
						diplomacyText.Append(": Vendetta");
					}

					this.oParent.DrawTools.F0_1182_005c_DrawStringToScreen0(diplomacyText.ToString(), 192, yOffset, 7);
					yOffset += 8;
				}
			}

			yOffset = 60;
			int column = 0;

			// Instruction address 0x0000:0x047e, size: 5
			this.oParent.DrawTools.F0_1182_005c_DrawStringToScreen0("Technologies:", 16, yOffset, 15);
			yOffset += 8;

			for (int i = 0; i < 72 && yOffset < 193; i++)
			{
				// Instruction address 0x0000:0x04e2, size: 5
				if (this.oParent.Segment_1ade.F0_1ade_22b5_PlayerHasTechnology(playerID, (TechnologyAdvanceEnum)i))
				{
					if ((this.oParent.GameData.TechnologyFirstDiscoveredBy[i] & 0x7) != playerID)
					{
						this.oParent.DrawTools.F0_1182_005c_DrawStringToScreen0(this.oParent.GameData.TechnologyAdvances[i].Name, 8 + ((column % 3) * 100), yOffset, 7);
					}
					else
					{
						this.oParent.DrawTools.F0_1182_005c_DrawStringToScreen0(this.oParent.GameData.TechnologyAdvances[i].Name, 8 + ((column % 3) * 100), yOffset, 15);
					}

					column++;

					if ((column % 3) == 0)
					{
						yOffset += 8;
					}
				}
			}

			// Instruction address 0x0000:0x0525, size: 5
			this.oParent.Segment_2459.F0_2459_0918_WaitForKeyPressOrMouseClick();

			// Instruction address 0x0000:0x0542, size: 5
			this.oParent.Graphics.F0_VGA_07d8_DrawImage(this.oParent.Var_19d4_Screen1_Rectangle, 0, 0, 320, 200, this.oParent.Var_aa_Screen0_Rectangle, 0, 0);
		}

		/// <summary>
		/// Print unit statistics for all players
		/// </summary>
		public void ShowUnitStatistics()
		{
			//this.oCPU.Log.EnterBlock("F13_0000_0554()");

			// function body
			this.oParent.Graphics.F0_VGA_07d8_DrawImage(this.oParent.Var_aa_Screen0_Rectangle, 0, 0, 320, 200, this.oParent.Var_19d4_Screen1_Rectangle, 0, 0);

			this.oParent.DrawTools.FillRectangle(this.oParent.Var_aa_Screen0_Rectangle, 0, 0, 320, 200, 1);

			int xOffset = 0;
			int yOffset = 18;

			for (int i = 0; i < 28; i++)
			{
				bool playerHasUnits = false;

				for (int j = 1; j < 8; j++)
				{
					if (this.oParent.GameData.Players[j].ActiveUnits[i] != 0 || this.oParent.GameData.Players[j].UnitsInProduction[i] != 0)
					{
						playerHasUnits = true;
					}
				}

				if (playerHasUnits)
				{
					this.oParent.Graphics.F0_VGA_0599_DrawLine(this.oParent.Var_aa_Screen0_Rectangle, 36, yOffset - 1, 319, yOffset - 1, 9);

					this.oParent.Graphics.F0_VGA_0c3e_DrawBitmapToScreen(
						this.oParent.Var_aa_Screen0_Rectangle, ((xOffset & 1) * 16) + 2, yOffset - 4, this.oParent.Array_d4ce[96 + i]);

					xOffset++;

					this.oParent.DrawTools.F0_1182_005c_DrawStringToScreen0(this.oParent.GameData.Units[i].Name, 36, yOffset, 15);

					yOffset += 8;
				}
			}

			for (int i = 0; i < 8; i++)
			{
				this.oParent.DrawTools.F0_1182_005c_DrawStringToScreen0($"{this.oParent.GameData.Players[i].Ranking}", (i * 30) + 84, 1, 15);

				this.oParent.Graphics.F0_VGA_0c3e_DrawBitmapToScreen(this.oParent.Var_aa_Screen0_Rectangle,
					(30 * i) + 90, 2, this.oParent.Array_d4ce[64 + (i * 32)]);

				yOffset = 18;

				for (int j = 0; j < 28; j++)
				{
					bool anyPlayerHasUnits = false;

					for (int k = 1; k < 8; k++)
					{
						if (this.oParent.GameData.Players[k].ActiveUnits[j] != 0 || this.oParent.GameData.Players[k].UnitsInProduction[j] != 0)
						{
							anyPlayerHasUnits = true;
						}
					}

					if (anyPlayerHasUnits)
					{
						if (this.oParent.GameData.Players[i].ActiveUnits[j] != 0 || this.oParent.GameData.Players[i].UnitsInProduction[j] != 0)
						{
							// Instruction address 0x0000:0x074c, size: 5
							this.oParent.DrawTools.F0_1182_005c_DrawStringToScreen0(
								$"{this.oParent.GameData.Players[i].ActiveUnits[j]}/{this.oParent.GameData.Players[i].UnitsInProduction[j]}",
								(i * 30) + 88, yOffset, 15);
						}

						yOffset += 8;
					}
				}
			}

			this.oParent.Segment_2459.F0_2459_0918_WaitForKeyPressOrMouseClick();

			// Show destroyed unit statistics
			this.oParent.DrawTools.FillRectangle(this.oParent.Var_aa_Screen0_Rectangle, 0, 0, 320, 200, 4);

			for (int i = 1; i < 8; i++)
			{
				this.oParent.DrawTools.F0_1182_0086_DrawStringWithShadowToScreen0(
					this.oParent.GameData.Players[i].Nationality, i * 40, 8, this.oParent.Array_1946_PlayerColours[i]);

				this.oParent.Graphics.F0_VGA_0c3e_DrawBitmapToScreen(this.oParent.Var_aa_Screen0_Rectangle, i * 40, 16, this.oParent.Array_d4ce[64 + (i * 32)]);

				this.oParent.DrawTools.F0_1182_0086_DrawStringWithShadowToScreen0(
					this.oParent.GameData.Players[i].Nationality, 4, i * 12 + 32, this.oParent.Array_1946_PlayerColours[i]);
			}

			for (int i = 1; i < 8; i++)
			{
				for (int j = 1; j < 8; j++)
				{
					if (i != j)
					{
						if (this.oParent.Var_d806_DebugFlag || i == this.oParent.GameData.HumanPlayerID || j == this.oParent.GameData.HumanPlayerID)
						{
							// Instruction address 0x0000:0x0976, size: 5
							this.oParent.DrawTools.F0_1182_005c_DrawStringToScreen0(
								$"{this.oParent.GameData.Players[i].UnitsDestroyed[j]}/{this.oParent.GameData.Players[j].UnitsDestroyed[i]}",
								i * 40, (j * 12) + 32, this.oParent.Array_1946_PlayerColours[i]);
						}
					}
				}
			}

			this.oParent.Segment_2459.F0_2459_0918_WaitForKeyPressOrMouseClick();

			this.oParent.Graphics.F0_VGA_07d8_DrawImage(this.oParent.Var_19d4_Screen1_Rectangle, 0, 0, 320, 200, this.oParent.Var_aa_Screen0_Rectangle, 0, 0);
		}
	}
}
