using System.Text;

namespace OpenCivOne
{
	public class ShowDebugDetails
	{
		private OpenCivOneGame parent;

		public ShowDebugDetails(OpenCivOneGame parent)
		{
			this.parent = parent;
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
				playerList.Append($"{this.parent.GameData.Players[i].Nationality}\n ");
			}

			int selectedPlayerID = this.parent.MenuBoxDialog.ShowDialog(playerList.ToString(), 100, 80) + 1;

			this.parent.Graphics.F0_VGA_07d8_DrawImage(this.parent.Var_aa_Screen0_Rectangle, 0, 0, 320, 200, this.parent.Var_19d4_Screen1_Rectangle, 0, 0);

			this.parent.DrawTools.FillRectangle(this.parent.Var_aa_Screen0_Rectangle, 0, 0, 320, 200, 0);

			int playerBitMask = 0x1 << selectedPlayerID;

			for (int i = 0; i < 50; i++)
			{
				for (int j = 0; j < 80; j++)
				{
					if ((this.parent.GameData.MapVisibility[j, i] & playerBitMask) != 0 || this.parent.GlobalPreferences.Debug.DebugMode)
					{
						int x = j * 4;
						int y = i * 4;

						if (this.parent.MapManagement.GetTerrainType(j, i) != TerrainTypeEnum.Water)
						{
							// Instruction address 0x0000:0x00c6, size: 5
							this.parent.DrawTools.FillRectangle(this.parent.Var_aa_Screen0_Rectangle, x, y, 4, 4, 2);
						}
						else
						{
							// Instruction address 0x0000:0x00c6, size: 5
							this.parent.DrawTools.FillRectangle(this.parent.Var_aa_Screen0_Rectangle, x, y, 4, 4, 1);
						}

						if (this.parent.MapManagement.F0_2aea_1585_GetVisibleTerrainImprovements(j, i).HasFlag(TerrainImprovementFlagsEnum.City))
						{
							int cityPlayerID = this.parent.MapManagement.F0_2aea_1369_GetCityOwner(j, i);

							// Instruction address 0x0000:0x0159, size: 5
							this.parent.DrawTools.FillRectangle(this.parent.Var_aa_Screen0_Rectangle, x, y, 4, 4, this.parent.Array_1946_PlayerColours[cityPlayerID]);
						}
						else
						{
							int unitPlayerID = this.parent.MapManagement.F0_2aea_14e0_GetCellActiveUnitPlayerID(j, i);

							if (unitPlayerID != -1)
							{
								// Instruction address 0x0000:0x013f, size: 5
								this.parent.DrawTools.FillRectangle(this.parent.Var_aa_Screen0_Rectangle, x + 1, y + 1, 3, 3, 0);

								// Instruction address 0x0000:0x0159, size: 5
								this.parent.DrawTools.FillRectangle(this.parent.Var_aa_Screen0_Rectangle, x, y, 3, 3, this.parent.Array_1946_PlayerColours[unitPlayerID]);
							}
						}
					}
				}
			}

			// Instruction address 0x0000:0x01ce, size: 5
			this.parent.CAPI.getch();

			this.parent.Var_aa_Screen0_Rectangle.FontID = 2;

			for (int i = 0; i < 32; i++)
			{
				switch (this.parent.GameData.Players[selectedPlayerID].UnitPolicies[i].UnitRoleType)
				{
					case UnitRoleTypeEnum.None:
						break;

					case UnitRoleTypeEnum.Settler:
						this.parent.DrawTools.DrawStringWithShadow("S",
							this.parent.GameData.Players[selectedPlayerID].UnitPolicies[i].Position.X * 4,
							this.parent.GameData.Players[selectedPlayerID].UnitPolicies[i].Position.Y * 4, 13);
						break;

					case UnitRoleTypeEnum.LandAttack:
						this.parent.DrawTools.DrawStringWithShadow("LA",
							this.parent.GameData.Players[selectedPlayerID].UnitPolicies[i].Position.X * 4 - 2,
							this.parent.GameData.Players[selectedPlayerID].UnitPolicies[i].Position.Y * 4, 13);
						break;

					case UnitRoleTypeEnum.Defense:
						this.parent.DrawTools.DrawStringWithShadow("D",
							this.parent.GameData.Players[selectedPlayerID].UnitPolicies[i].Position.X * 4,
							this.parent.GameData.Players[selectedPlayerID].UnitPolicies[i].Position.Y * 4, 13);
						break;

					case UnitRoleTypeEnum.SeaAttack:
						this.parent.DrawTools.DrawStringWithShadow("SA",
							this.parent.GameData.Players[selectedPlayerID].UnitPolicies[i].Position.X * 4 - 2,
							this.parent.GameData.Players[selectedPlayerID].UnitPolicies[i].Position.Y * 4, 13);
						break;

					case UnitRoleTypeEnum.AirAttack:
						this.parent.DrawTools.DrawStringWithShadow("AA",
							this.parent.GameData.Players[selectedPlayerID].UnitPolicies[i].Position.X * 4 - 2,
							this.parent.GameData.Players[selectedPlayerID].UnitPolicies[i].Position.Y * 4, 13);
						break;

					case UnitRoleTypeEnum.SeaTransport:
						this.parent.DrawTools.DrawStringWithShadow("ST",
							this.parent.GameData.Players[selectedPlayerID].UnitPolicies[i].Position.X * 4 - 2,
							this.parent.GameData.Players[selectedPlayerID].UnitPolicies[i].Position.Y * 4, 13);
						break;

					case UnitRoleTypeEnum.Neutral:
						this.parent.DrawTools.DrawStringWithShadow("N",
							this.parent.GameData.Players[selectedPlayerID].UnitPolicies[i].Position.X * 4,
							this.parent.GameData.Players[selectedPlayerID].UnitPolicies[i].Position.Y * 4, 13);
						break;
				}
			}

			for (int i = 0; i < 16; i++)
			{
				switch (this.parent.GameData.Players[selectedPlayerID].ContinentPolicies[i].UnitRoleType)
				{
					case UnitRoleTypeEnum.None:
						break;

					case UnitRoleTypeEnum.Settler:
						// Settle?
						this.parent.DrawTools.DrawStringWithShadow("S",
							this.parent.GameData.Players[selectedPlayerID].ContinentPolicies[i].Position.X * 4,
							this.parent.GameData.Players[selectedPlayerID].ContinentPolicies[i].Position.Y * 4, 14);
						break;

					case UnitRoleTypeEnum.LandAttack:
						// Attack?
						this.parent.DrawTools.DrawStringWithShadow("LA",
							this.parent.GameData.Players[selectedPlayerID].ContinentPolicies[i].Position.X * 4 - 2,
							this.parent.GameData.Players[selectedPlayerID].ContinentPolicies[i].Position.Y * 4, 14);
						break;

					case UnitRoleTypeEnum.Defense:
						// Defend ?
						this.parent.DrawTools.DrawStringWithShadow("D",
							this.parent.GameData.Players[selectedPlayerID].ContinentPolicies[i].Position.X * 4,
							this.parent.GameData.Players[selectedPlayerID].ContinentPolicies[i].Position.Y * 4, 14);
						break;

					case UnitRoleTypeEnum.SeaAttack:
						this.parent.DrawTools.DrawStringWithShadow("SA",
							this.parent.GameData.Players[selectedPlayerID].ContinentPolicies[i].Position.X * 4 - 2,
							this.parent.GameData.Players[selectedPlayerID].ContinentPolicies[i].Position.Y * 4, 14);
						break;

					case UnitRoleTypeEnum.AirAttack:
						this.parent.DrawTools.DrawStringWithShadow("AA",
							this.parent.GameData.Players[selectedPlayerID].ContinentPolicies[i].Position.X * 4 - 2,
							this.parent.GameData.Players[selectedPlayerID].ContinentPolicies[i].Position.Y * 4, 14);
						break;

					case UnitRoleTypeEnum.SeaTransport:
						this.parent.DrawTools.DrawStringWithShadow("ST",
							this.parent.GameData.Players[selectedPlayerID].ContinentPolicies[i].Position.X * 4 - 2,
							this.parent.GameData.Players[selectedPlayerID].ContinentPolicies[i].Position.Y * 4, 14);
						break;

					case UnitRoleTypeEnum.Neutral:
						this.parent.DrawTools.DrawStringWithShadow("N",
							this.parent.GameData.Players[selectedPlayerID].ContinentPolicies[i].Position.X * 4,
							this.parent.GameData.Players[selectedPlayerID].ContinentPolicies[i].Position.Y * 4, 14);
						break;
				}
			}

			// Instruction address 0x0000:0x02e2, size: 5
			this.parent.CAPI.getch();

			int yText = 6;

			for (int i = 1; i < 15; i++)
			{
				if (this.parent.GameData.Players[selectedPlayerID].Continents[i].Attack != 0)
				{
					string strategyText = "";

					switch (this.parent.GameData.Players[selectedPlayerID].Continents[i].Strategy)
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
							throw new Exception($"Invalid Player Continent Strategy: {this.parent.GameData.Players[selectedPlayerID].Continents[i].Strategy}");
					}

					// Instruction address 0x0000:0x0447, size: 5
					this.parent.DrawTools.DrawStringToScreen0(
						$"{i}/{strategyText} {this.parent.GameData.Players[selectedPlayerID].Continents[i].Defense}/{this.parent.GameData.Players[selectedPlayerID].Continents[i].Attack}",
						2, yText, 7);

					yText += 6;
				}
			}

			this.parent.CAPI.getch();

			this.parent.Var_aa_Screen0_Rectangle.FontID = 1;

			this.parent.Graphics.F0_VGA_07d8_DrawImage(this.parent.Var_19d4_Screen1_Rectangle, 0, 0, 320, 200, this.parent.Var_aa_Screen0_Rectangle, 0, 0);
		}

		/// <summary>
		/// Print the status of the unit
		/// </summary>
		/// <param name="playerID"></param>
		/// <param name="unitID"></param>
		public void ShowUnitStatus(int playerID, int unitID)
		{
			//this.oCPU.Log.EnterBlock($"F10_0000_0477({playerID}, {unitID})");

			// function body
			StringBuilder unitStatus = new();

			unitStatus.Append($"{this.parent.GameData.Players[playerID].Nationality}\n({unitID})/" +
				$"{this.parent.GameData.Units[(int)this.parent.GameData.Players[playerID].Units[unitID].UnitType].Name}\n" +
				$"{this.parent.Segment_2459.F0_2459_08c6_GetCityName(this.parent.GameData.Players[playerID].Units[unitID].HomeCityID)}\n" +
				$"Loc:{this.parent.GameData.Players[playerID].Units[unitID].Position.X},{this.parent.GameData.Players[playerID].Units[unitID].Position.Y}\n");

			if (this.parent.GameData.Players[playerID].Units[unitID].GoToDestination.X != -1)
			{
				unitStatus.Append($"To:{this.parent.GameData.Players[playerID].Units[unitID].GoToDestination.X},"+
					$"{this.parent.GameData.Players[playerID].Units[unitID].GoToDestination.Y}\n");
			}
		
			int nextUnitID = this.parent.GameData.Players[playerID].Units[unitID].NextUnitID;

			while (nextUnitID != -1 && nextUnitID != unitID && unitStatus.Length < 512)
			{
				// Instruction address 0x0000:0x066b, size: 5
				unitStatus.Append($"*{this.parent.GameData.Units[(int)this.parent.GameData.Players[playerID].Units[nextUnitID].UnitType].Name}\n");

				nextUnitID = this.parent.GameData.Players[playerID].Units[nextUnitID].NextUnitID;
			}

			// Instruction address 0x0000:0x0699, size: 5
			this.parent.MenuBoxDialog.ShowDialog(unitStatus.ToString(), 10, 10);
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
			this.parent.Graphics.F0_VGA_07d8_DrawImage(this.parent.Var_aa_Screen0_Rectangle, 0, 0, 320, 200, this.parent.Var_19d4_Screen1_Rectangle, 0, 0);

			// Instruction address 0x0000:0x0040, size: 5
			this.parent.DrawTools.FillRectangle(this.parent.Var_aa_Screen0_Rectangle, 0, 0, 320, 200, 1);

			// Instruction address 0x0000:0x005d, size: 5
			this.parent.DrawTools.DrawStringToScreen0(this.parent.GameData.Players[playerID].Nationality, 128, 4, 15);

			// Instruction address 0x0000:0x0078, size: 5
			this.parent.Graphics.F0_VGA_0c3e_DrawBitmapToScreen(this.parent.Var_aa_Screen0_Rectangle, 16, 4, this.parent.Array_d4ce[64 + (playerID * 32)]);

			// Instruction address 0x0000:0x00b6, size: 5
			this.parent.DrawTools.DrawStringToScreen0($"Government: {this.parent.Array_1966_GovernmentNames[(int)this.parent.GameData.Players[playerID].GovernmentType]}", 16, 24, 15);

			// Instruction address 0x0000:0x00e9, size: 5
			this.parent.DrawTools.DrawStringToScreen0(
				$"Population: {this.parent.GameEngine.PopulationValueToString(this.parent.GameEngine.GetPlayerPopulationCount(playerID))}", 16, 32, 15);

			// Instruction address 0x0000:0x0194, size: 5
			this.parent.DrawTools.DrawStringToScreen0($"Treasury: {this.parent.GameData.Players[playerID].Coins} coins, " +
				$"Taxes/Science: {this.parent.GameData.Players[playerID].TaxRate}/{this.parent.GameData.Players[playerID].ScienceTaxRate}", 16, 40, 15);

			// Instruction address 0x0000:0x01ac, size: 5
			this.parent.DrawTools.DrawStringToScreen0(
				$"{this.parent.GameData.Players[playerID].UnitCount} units / " +
				$"{this.parent.GameData.Players[playerID].CityCount} cities / " +
				$"{this.parent.GameData.Players[playerID].SettlerCount} settlers / " +
				$"{this.parent.GameData.Players[playerID].MapCellCount} land", 16, 48, 15);

			int yOffset = 4;

			for (int i = 1; i < 15; i++)
			{
				if (this.parent.GameData.Players[playerID].Continents[i].Attack != 0)
				{
					switch (this.parent.GameData.Players[playerID].Continents[i].Strategy)
					{
						case PlayerContinentStrategyEnum.Settle:
							this.parent.DrawTools.DrawStringToScreen0($"Settle/{i}", 256, yOffset, 7);
							yOffset += 8;
							break;

						case PlayerContinentStrategyEnum.Attack:
							this.parent.DrawTools.DrawStringToScreen0($"Attack/{i}", 256, yOffset, 7);
							yOffset += 8;
							break;

						case PlayerContinentStrategyEnum.Defend:
							this.parent.DrawTools.DrawStringToScreen0($"Defend/{i}", 256, yOffset, 7);
							yOffset += 8;
							break;

						case PlayerContinentStrategyEnum.Transport:
							this.parent.DrawTools.DrawStringToScreen0($"Transport/{i}", 256, yOffset, 7);
							yOffset += 8;
							break;

						default:
							throw new Exception($"Invalid Player Continent Strategy: {this.parent.GameData.Players[playerID].Continents[i].Strategy}");
					}
				}
			}

			yOffset = 24;

			for (int i = 1; i < 8; i++)
			{
				if (i != playerID && (this.parent.GameData.Players[playerID].Diplomacy[i] & (DiplomacyFlagsEnum.Contact | DiplomacyFlagsEnum.Peace | DiplomacyFlagsEnum.Vendetta)) != DiplomacyFlagsEnum.None)
				{
					StringBuilder diplomacyText = new();

					diplomacyText.Append(this.parent.GameData.Players[i].Nationality);

					if ((this.parent.GameData.Players[playerID].Diplomacy[i] & (DiplomacyFlagsEnum.Contact | DiplomacyFlagsEnum.Peace | DiplomacyFlagsEnum.Vendetta)) == DiplomacyFlagsEnum.Contact)
					{
						diplomacyText.Append(": Contact");
					}

					if (this.parent.GameData.Players[playerID].Diplomacy[i].HasFlag(DiplomacyFlagsEnum.Peace))
					{
						if (!this.parent.GameData.Players[playerID].Diplomacy[i].HasFlag(DiplomacyFlagsEnum.Allied))
						{
							diplomacyText.Append(": Peace");
						}
						else
						{
							diplomacyText.Append(": Allied");
						}
					}

					if (this.parent.GameData.Players[playerID].Diplomacy[i].HasFlag(DiplomacyFlagsEnum.Vendetta))
					{
						diplomacyText.Append(": Vendetta");
					}

					this.parent.DrawTools.DrawStringToScreen0(diplomacyText.ToString(), 192, yOffset, 7);
					yOffset += 8;
				}
			}

			yOffset = 60;
			int column = 0;

			// Instruction address 0x0000:0x047e, size: 5
			this.parent.DrawTools.DrawStringToScreen0("Technologies:", 16, yOffset, 15);
			yOffset += 8;

			for (int i = 0; i < 72 && yOffset < 193; i++)
			{
				// Instruction address 0x0000:0x04e2, size: 5
				if (this.parent.Segment_1ade.F0_1ade_22b5_PlayerHasTechnology(playerID, (TechnologyAdvanceEnum)i))
				{
					if ((this.parent.GameData.TechnologyFirstDiscoveredBy[i] & 0x7) != playerID)
					{
						this.parent.DrawTools.DrawStringToScreen0(this.parent.GameData.TechnologyAdvances[i].Name, 8 + ((column % 3) * 100), yOffset, 7);
					}
					else
					{
						this.parent.DrawTools.DrawStringToScreen0(this.parent.GameData.TechnologyAdvances[i].Name, 8 + ((column % 3) * 100), yOffset, 15);
					}

					column++;

					if ((column % 3) == 0)
					{
						yOffset += 8;
					}
				}
			}

			// Instruction address 0x0000:0x0525, size: 5
			this.parent.Segment_2459.F0_2459_0918_WaitForKeyPressOrMouseClick();

			// Instruction address 0x0000:0x0542, size: 5
			this.parent.Graphics.F0_VGA_07d8_DrawImage(this.parent.Var_19d4_Screen1_Rectangle, 0, 0, 320, 200, this.parent.Var_aa_Screen0_Rectangle, 0, 0);
		}

		/// <summary>
		/// Print unit statistics for all players
		/// </summary>
		public void ShowUnitStatistics()
		{
			//this.oCPU.Log.EnterBlock("F13_0000_0554()");

			// function body
			this.parent.Graphics.F0_VGA_07d8_DrawImage(this.parent.Var_aa_Screen0_Rectangle, 0, 0, 320, 200, this.parent.Var_19d4_Screen1_Rectangle, 0, 0);

			this.parent.DrawTools.FillRectangle(this.parent.Var_aa_Screen0_Rectangle, 0, 0, 320, 200, 1);

			int xOffset = 0;
			int yOffset = 18;

			for (int i = 0; i < 28; i++)
			{
				bool playerHasUnits = false;

				for (int j = 1; j < 8; j++)
				{
					if (this.parent.GameData.Players[j].ActiveUnits[i] != 0 || this.parent.GameData.Players[j].UnitsInProduction[i] != 0)
					{
						playerHasUnits = true;
					}
				}

				if (playerHasUnits)
				{
					this.parent.Graphics.F0_VGA_0599_DrawLine(this.parent.Var_aa_Screen0_Rectangle, 36, yOffset - 1, 319, yOffset - 1, 9);

					this.parent.Graphics.F0_VGA_0c3e_DrawBitmapToScreen(
						this.parent.Var_aa_Screen0_Rectangle, ((xOffset & 1) * 16) + 2, yOffset - 4, this.parent.Array_d4ce[96 + i]);

					xOffset++;

					this.parent.DrawTools.DrawStringToScreen0(this.parent.GameData.Units[i].Name, 36, yOffset, 15);

					yOffset += 8;
				}
			}

			for (int i = 0; i < 8; i++)
			{
				this.parent.DrawTools.DrawStringToScreen0($"{this.parent.GameData.Players[i].Ranking}", (i * 30) + 84, 1, 15);

				this.parent.Graphics.F0_VGA_0c3e_DrawBitmapToScreen(this.parent.Var_aa_Screen0_Rectangle,
					(30 * i) + 90, 2, this.parent.Array_d4ce[64 + (i * 32)]);

				yOffset = 18;

				for (int j = 0; j < 28; j++)
				{
					bool anyPlayerHasUnits = false;

					for (int k = 1; k < 8; k++)
					{
						if (this.parent.GameData.Players[k].ActiveUnits[j] != 0 || this.parent.GameData.Players[k].UnitsInProduction[j] != 0)
						{
							anyPlayerHasUnits = true;
						}
					}

					if (anyPlayerHasUnits)
					{
						if (this.parent.GameData.Players[i].ActiveUnits[j] != 0 || this.parent.GameData.Players[i].UnitsInProduction[j] != 0)
						{
							// Instruction address 0x0000:0x074c, size: 5
							this.parent.DrawTools.DrawStringToScreen0(
								$"{this.parent.GameData.Players[i].ActiveUnits[j]}/{this.parent.GameData.Players[i].UnitsInProduction[j]}",
								(i * 30) + 88, yOffset, 15);
						}

						yOffset += 8;
					}
				}
			}

			this.parent.Segment_2459.F0_2459_0918_WaitForKeyPressOrMouseClick();

			// Show destroyed unit statistics
			this.parent.DrawTools.FillRectangle(this.parent.Var_aa_Screen0_Rectangle, 0, 0, 320, 200, 4);

			for (int i = 1; i < 8; i++)
			{
				this.parent.DrawTools.DrawStringWithShadowToScreen0(
					this.parent.GameData.Players[i].Nationality, i * 40, 8, this.parent.Array_1946_PlayerColours[i]);

				this.parent.Graphics.F0_VGA_0c3e_DrawBitmapToScreen(this.parent.Var_aa_Screen0_Rectangle, i * 40, 16, this.parent.Array_d4ce[64 + (i * 32)]);

				this.parent.DrawTools.DrawStringWithShadowToScreen0(
					this.parent.GameData.Players[i].Nationality, 4, i * 12 + 32, this.parent.Array_1946_PlayerColours[i]);
			}

			for (int i = 1; i < 8; i++)
			{
				for (int j = 1; j < 8; j++)
				{
					if (i != j)
					{
						if (this.parent.GlobalPreferences.Debug.DebugMode || i == this.parent.GameData.HumanPlayerID || j == this.parent.GameData.HumanPlayerID)
						{
							// Instruction address 0x0000:0x0976, size: 5
							this.parent.DrawTools.DrawStringToScreen0(
								$"{this.parent.GameData.Players[i].UnitsDestroyed[j]}/{this.parent.GameData.Players[j].UnitsDestroyed[i]}",
								i * 40, (j * 12) + 32, this.parent.Array_1946_PlayerColours[i]);
						}
					}
				}
			}

			this.parent.Segment_2459.F0_2459_0918_WaitForKeyPressOrMouseClick();

			this.parent.Graphics.F0_VGA_07d8_DrawImage(this.parent.Var_19d4_Screen1_Rectangle, 0, 0, 320, 200, this.parent.Var_aa_Screen0_Rectangle, 0, 0);
		}
	}
}
