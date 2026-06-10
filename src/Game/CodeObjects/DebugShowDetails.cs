using System.Text;

namespace OpenCivOne
{
	public class DebugShowDetails
	{
		private OpenCivOneGame oParent;

		public DebugShowDetails(OpenCivOneGame parent)
		{
			this.oParent = parent;
		}

		/// <summary>
		/// Shows a dialog to select the player
		/// </summary>
		public void F10_0000_0000_DebugShowMapWithStrategy()
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

			this.oParent.MainCode.F0_11a8_0268_HideMouse();

			this.oParent.CommonTools.F0_1000_0bfa_FillRectangle(this.oParent.Var_aa_Screen0_Rectangle, 0, 0, 320, 200, 0);

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
							this.oParent.CommonTools.F0_1000_0bfa_FillRectangle(this.oParent.Var_aa_Screen0_Rectangle, x, y, 4, 4, 2);
						}
						else
						{
							// Instruction address 0x0000:0x00c6, size: 5
							this.oParent.CommonTools.F0_1000_0bfa_FillRectangle(this.oParent.Var_aa_Screen0_Rectangle, x, y, 4, 4, 1);
						}

						if (this.oParent.MapManagement.F0_2aea_1585_GetVisibleTerrainImprovements(j, i).HasFlag(TerrainImprovementFlagsEnum.City))
						{
							int cityPlayerID = this.oParent.MapManagement.F0_2aea_1369_GetCityOwner(j, i);

							// Instruction address 0x0000:0x0159, size: 5
							this.oParent.CommonTools.F0_1000_0bfa_FillRectangle(this.oParent.Var_aa_Screen0_Rectangle, x, y, 4, 4, this.oParent.Array_1946_PlayerColours[cityPlayerID]);
						}
						else
						{
							int unitPlayerID = this.oParent.MapManagement.F0_2aea_14e0_GetCellUnitPlayerID(j, i);

							if (unitPlayerID != -1)
							{
								// Instruction address 0x0000:0x013f, size: 5
								this.oParent.CommonTools.F0_1000_0bfa_FillRectangle(this.oParent.Var_aa_Screen0_Rectangle, x + 1, y + 1, 3, 3, 0);

								// Instruction address 0x0000:0x0159, size: 5
								this.oParent.CommonTools.F0_1000_0bfa_FillRectangle(this.oParent.Var_aa_Screen0_Rectangle, x, y, 3, 3, this.oParent.Array_1946_PlayerColours[unitPlayerID]);
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
						case 0:
							// Settle ?
							strategyText = "S";
							break;

						case 1:
							// Attack ?
							strategyText = "A";
							break;

						case 2:
							// Defend ?
							strategyText = "D";
							break;

						// !!! What about values 3 and 4 ?
						case 3:
							strategyText = "V3";
							break;

						case 4:
							strategyText = "V4";
							break;

						case 5:
							// Trade or Transport ?
							strategyText = "T";
							break;
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

			this.oParent.MainCode.F0_11a8_0250_ShowMouse();

			this.oParent.Graphics.F0_VGA_07d8_DrawImage(this.oParent.Var_19d4_Screen1_Rectangle, 0, 0, 320, 200, this.oParent.Var_aa_Screen0_Rectangle, 0, 0);
		}

		/// <summary>
		/// ?
		/// </summary>
		/// <param name="playerID"></param>
		/// <param name="unitID"></param>
		public void F10_0000_0477_DebugShowUnitStatus(int playerID, int unitID)
		{
			//this.oCPU.Log.EnterBlock($"F10_0000_0477({playerID}, {unitID})");

			// function body
			StringBuilder unitStatus = new();

			unitStatus.Append($"{this.oParent.GameData.Players[playerID].Nationality}\n({unitID})/" +
				$"{this.oParent.GameData.Units[this.oParent.GameData.Players[playerID].Units[unitID].TypeID].Name}\n" +
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
				unitStatus.Append($"*{this.oParent.GameData.Units[this.oParent.GameData.Players[playerID].Units[nextUnitID].TypeID].Name}\n");

				nextUnitID = this.oParent.GameData.Players[playerID].Units[nextUnitID].NextUnitID;
			}

			// Instruction address 0x0000:0x0699, size: 5
			this.oParent.Segment_1238.F0_1238_001e_ShowDialog(unitStatus.ToString(), 10, 10);
		}
	}
}
