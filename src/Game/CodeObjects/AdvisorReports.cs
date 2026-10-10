using IRB.VirtualCPU;
using System.Globalization;

namespace OpenCivOne
{
	/// <summary>
	/// The advisor reports: science, military, trade, intelligence, attitude and city status
	/// </summary>
	public class AdvisorReports
	{
		private OpenCivOneGame oParent;

		public AdvisorReports(OpenCivOneGame parent)
		{
			this.oParent = parent;
		}

		/// <summary>
		/// Saves the current screen, so the report can restore it when it closes, and draws the report header
		/// </summary>
		/// <param name="title"></param>
		/// <param name="backgroundColor"></param>
		public void ShowReportHeader(string title, int backgroundColor)
		{
			this.oParent.Graphics.F0_VGA_07d8_DrawImage(this.oParent.Var_aa_Screen0_Rectangle, 0, 0, 320, 200, this.oParent.Var_19d4_Screen1_Rectangle, 0, 0);

			DrawReportHeader(title, backgroundColor);
		}

		/// <summary>
		/// Clears the screen and draws the report title, the human player's empire and leader, and the year
		/// </summary>
		/// <param name="title"></param>
		/// <param name="backgroundColor"></param>
		public void DrawReportHeader(string title, int backgroundColor)
		{
			Player humanPlayer = this.oParent.GameData.Players[this.oParent.GameData.HumanPlayerID];

			this.oParent.DrawTools.FillRectangle(this.oParent.Var_aa_Screen0_Rectangle, 0, 0, 320, 200, (ushort)backgroundColor);

			this.oParent.DrawTools.DrawCenteredStringToScreen0(title, 160, 2, 15);

			// Despotism and Monarchy are an Empire, Communism and Republic a Kingdom, Democracy a Republic
			string empire;

			switch ((int)humanPlayer.GovernmentType / 2)
			{
				case 0:
					empire = "Empire";
					break;

				case 1:
					empire = "Kingdom";
					break;

				case 2:
					empire = "Republic";
					break;

				default:
					// The original leaves the previous text in the buffer
					empire = "";
					break;
			}

			this.oParent.DrawTools.DrawCenteredStringToScreen0($"{empire} of the {humanPlayer.Nation}", 160, 10, 15);

			this.oParent.DrawTools.DrawCenteredStringToScreen0(
				$"{this.oParent.Array_19b2_GovernmentTypeNames[(int)humanPlayer.GovernmentType]} {humanPlayer.Name}: " +
				this.oParent.GameEngine.YearToString(this.oParent.GameData.Year), 160, 18, 15);
		}

		/// <summary>
		/// Waits for a key press or a mouse click, then restores the screen saved by ShowReportHeader
		/// </summary>
		public void WaitAndCloseReport()
		{
			this.oParent.Actions.F0_2459_0918_WaitForKeyPressOrMouseClick();

			this.oParent.Graphics.F0_VGA_07d8_DrawImage(this.oParent.Var_19d4_Screen1_Rectangle, 0, 0, 320, 200, this.oParent.Var_aa_Screen0_Rectangle, 0, 0);
		}

		/// <summary>
		/// Shows Science Report window.
		/// </summary>
		/// <param name="playerID"></param>
		public void ScienceReport(short playerID)
		{
			ShowReportHeader("SCIENCE REPORT", 1);

			Player player = this.oParent.GameData.Players[playerID];
			int progressBarValue = (player.DiscoveredTechnologyCount * ((this.oParent.GameData.Year < 0) ? 1 : 2)) *
				Math.Max((oParent.GameData.DifficultyLevel * 2) + this.oParent.Var_d2de + 6, 11 - player.DiscoveredTechnologyCount);
			int progressBarSpacing = this.oParent.CommonTools.CheckValueRange(2432 / (progressBarValue + 1), 1, 64);
			int progressBarWidth = this.oParent.CommonTools.CheckValueRange(((progressBarSpacing * progressBarValue) / 8) + 8, 8, 304);
			int progressBarXStart = 160 - (progressBarWidth / 2);

			if (progressBarXStart > 40)
			{
				this.oParent.Graphics.F0_VGA_07d8_DrawImage(this.oParent.Var_19e8_Screen2_Rectangle, 280, 140, 40, 60, this.oParent.Var_aa_Screen0_Rectangle, 278, 2);
			}

			this.oParent.DrawTools.FillRectangle(this.oParent.Var_aa_Screen0_Rectangle, progressBarXStart, 25, progressBarWidth, 16, 9);

			int progress = this.oParent.CommonTools.CheckValueRange(player.ResearchProgress, 0, progressBarValue);

			for (int i = 0; i < progress; i++)
			{
				this.oParent.Graphics.F0_VGA_0c3e_DrawBitmapToScreen(this.oParent.Var_aa_Screen0_Rectangle, progressBarXStart + (progressBarSpacing * i / 8), 33,
					this.oParent.Array_d4ce[12]);
			}

			if (player.ResearchTechnologyID != -1)
			{
				string researching = $"Researching {this.oParent.GameData.TechnologyAdvances[player.ResearchTechnologyID].Name} ({progress}/{progressBarValue})";

				this.oParent.DrawTools.DrawCenteredStringToScreen0(researching, 161, 26, 0);

				this.oParent.DrawTools.DrawCenteredStringToScreen0(researching, 160, 26, 15);
			}

			int textYPos = 42;
			int column = 0;

			for (int i = 0; i < this.oParent.GameData.TechnologyAdvances.Length; i++)
			{
				if (this.oParent.Technology.F0_1ade_22b5_PlayerHasTechnology(playerID, this.oParent.GameData.TechnologyAdvances[i].Value))
				{
					this.oParent.DrawTools.DrawStringToScreen0(this.oParent.GameData.TechnologyAdvances[i].Name, ((column % 3) * 100) + 8, textYPos,
						(byte)(((this.oParent.GameData.TechnologyFirstDiscoveredBy[i] & 0x7) != playerID) ? 11 : 15));

					column++;

					if ((column % 3) == 0)
					{
						textYPos += 7;
					}

					if (textYPos > 192)
						break;
				}
			}

			WaitAndCloseReport();
		}

		/// <summary>
		/// Shows Military Report window.
		/// </summary>
		/// <param name="playerID"></param>
		public void MilitaryReport(short playerID)
		{
			Player player = this.oParent.GameData.Players[playerID];

			ShowReportHeader("MILITARY STATUS", 1);

			int rowCount = 0;
			int y = 32;

			for (int unitType = 0; unitType < 28; unitType++)
			{
				int activeCount = player.ActiveUnits[unitType];
				int inProductionCount = player.UnitsInProduction[unitType];

				if (activeCount <= 0 && inProductionCount <= 0)
					continue;

				UnitDefinition unit = this.oParent.GameData.Units[unitType];

				this.oParent.Graphics.F0_VGA_0599_DrawLine(this.oParent.Var_aa_Screen0_Rectangle, 36, y - 2, 319, y - 2, 9);

				// The unit icons alternate between two columns
				this.oParent.Graphics.F0_VGA_0c3e_DrawBitmapToScreen(this.oParent.Var_aa_Screen0_Rectangle, ((rowCount & 0x1) << 4) + 2, y - 4,
					this.oParent.Array_d4ce[64 + (playerID * 32) + unitType]);

				rowCount++;

				this.oParent.DrawTools.DrawStringToScreen0(unit.Name, 36, y, 15);
				this.oParent.DrawTools.DrawStringToScreen0(
					string.Create(CultureInfo.InvariantCulture, $"({unit.AttackStrength}/{unit.DefenseStrength}/{unit.MoveCount})"), 112, y, 11);

				if (activeCount > 0)
				{
					this.oParent.DrawTools.DrawStringToScreen0(string.Create(CultureInfo.InvariantCulture, $"{activeCount} active"), 168, y, 15);
				}

				if (inProductionCount > 0)
				{
					this.oParent.DrawTools.DrawStringToScreen0(string.Create(CultureInfo.InvariantCulture, $"{inProductionCount} in production"), 232, y, 11);
				}

				y += 9;
			}

			this.oParent.Graphics.F0_VGA_07d8_DrawImage(this.oParent.Var_19e8_Screen2_Rectangle, 160, 140, 40, 60, this.oParent.Var_aa_Screen0_Rectangle, 278, 2);

			WaitAndCloseReport();

			MilitaryLossesReport(playerID);
		}

		/// <summary>
		/// Shows the units lost by the player and by the other players
		/// </summary>
		/// <param name="playerID"></param>
		public void MilitaryLossesReport(short playerID)
		{
			ShowReportHeader("MILITARY LOSSES", 4);

			int rowCount = 0;
			int y = 28;

			for (int unitType = 0; unitType < 27; unitType++)
			{
				bool anyLost = false;

				for (int i = 0; i < 8; i++)
				{
					if (this.oParent.GameData.Players[i].LostUnits[unitType] != 0)
					{
						anyLost = true;
					}
				}

				if (!anyLost)
					continue;

				this.oParent.Graphics.F0_VGA_0599_DrawLine(this.oParent.Var_aa_Screen0_Rectangle, 36, y - 1, 319, y - 1, 8);
				this.oParent.Graphics.F0_VGA_0c3e_DrawBitmapToScreen(this.oParent.Var_aa_Screen0_Rectangle, ((rowCount & 1) << 4) + 2, y - 4,
					this.oParent.Array_d4ce[64 + (playerID * 32) + unitType]);

				rowCount++;

				this.oParent.DrawTools.DrawStringToScreen0(this.oParent.GameData.Units[unitType].Name, 36, y, 15);

				int lost = this.oParent.GameData.Players[playerID].LostUnits[unitType];

				if (lost != 0)
				{
					this.oParent.DrawTools.DrawStringToScreen0(lost.ToString(CultureInfo.InvariantCulture), 104, y, 12);
				}

				// The other players' losses, one column each in their colors
				int x = 104;

				for (int otherPlayerID = 1; otherPlayerID < 8; otherPlayerID++)
				{
					if (otherPlayerID == playerID)
						continue;

					x += 28;
					lost = this.oParent.GameData.Players[otherPlayerID].LostUnits[unitType];

					if (lost != 0)
					{
						this.oParent.DrawTools.DrawStringToScreen0(lost.ToString(CultureInfo.InvariantCulture), x, y,
							this.oParent.Array_1946_PlayerColours[otherPlayerID]);
					}
				}

				y += 7;
			}

			this.oParent.DrawTools.DrawRectangle(100, 25, 18, y - 25, this.oParent.Array_1946_PlayerColours[this.oParent.GameData.HumanPlayerID]);

			WaitAndCloseReport();
		}

		/// <summary>
		/// Shows Trade Report window.
		/// </summary>
		/// <param name="playerID"></param>
		public void TradeReport(short playerID)
		{
			// The Palace costs more to maintain on higher difficulty levels and with Gunpowder and Combustion
			int palaceMaintenance = this.oParent.GameData.DifficultyLevel / 2;

			if (this.oParent.Technology.F0_1ade_22b5_PlayerHasTechnology(playerID, TechnologyAdvanceEnum.Gunpowder))
			{
				palaceMaintenance++;
			}

			if (this.oParent.Technology.F0_1ade_22b5_PlayerHasTechnology(playerID, TechnologyAdvanceEnum.Combustion))
			{
				palaceMaintenance++;
			}

			ShowReportHeader("TRADE REPORT", 2);

			this.oParent.Graphics.F0_VGA_07d8_DrawImage(this.oParent.Var_19e8_Screen2_Rectangle, 200, 140, 40, 60, this.oParent.Var_aa_Screen0_Rectangle, 278, 2);

			this.oParent.DrawTools.DrawStringToScreen0("City Trade", 8, 32, 15);

			// The totals are 16-bit, as in the original
			short totalIncome = 0;
			short totalScience = 0;

			// The maintenance cost of each improvement, indexed by the improvement's bit in the city's improvement flags
			int[] maintenance = new int[24];

			int y = 40;

			for (int cityID = 0; cityID < 128; cityID++)
			{
				City city = this.oParent.GameData.Cities[cityID];

				if (city.Status == CityStatusEnum.Invalid || city.PlayerID != playerID)
					continue;

				if (y >= 180)
				{
					// Next page
					this.oParent.Actions.F0_2459_0918_WaitForKeyPressOrMouseClick();

					DrawReportHeader("TRADE REPORT", 2);

					y = 40;
				}

				this.oParent.CityWorker.F0_1d12_0045_ProcessCityState(cityID, -1);

				string cityName = this.oParent.LanguageTools.F0_2f4d_04f7_TrimStringToWidth(this.oParent.Actions.F0_2459_08c6_GetCityName(cityID), 62);

				if (y < 180)
				{
					this.oParent.DrawTools.DrawStringWithShadow(cityName, 16, y, 15);
				}

				string trade;

				if ((city.Status & CityStatusEnum.CivilDisorder) != CityStatusEnum.None)
				{
					trade = " \x008c(Disorder)";
				}
				else
				{
					trade = string.Create(CultureInfo.InvariantCulture,
						$" {this.oParent.Var_70da_Arr[3]}\\/{this.oParent.Var_e17a}$/{this.oParent.Var_70e6}~");

					totalIncome = (short)(totalIncome + this.oParent.Var_e17a);
					totalScience = (short)(totalScience + this.oParent.Var_70e6);
				}

				if (y < 180)
				{
					this.oParent.DrawTools.DrawStringToScreen0(trade, 80, y, 10);
					y += 8;
				}

				uint improvements = ((uint)city.ImprovementFlags1 << 16) | city.ImprovementFlags0;

				for (int i = 1; i < 24; i++)
				{
					if ((improvements & (1u << i)) != 0)
					{
						maintenance[i] += this.oParent.GameData.GetImprovementType(i + 1).MaintenanceCost;

						if (i == 1)
						{
							maintenance[i] += palaceMaintenance;
						}
					}
				}
			}

			this.oParent.DrawTools.DrawStringToScreen0(string.Create(CultureInfo.InvariantCulture, $"Total Income: {totalIncome}$"), 8, y + 4, 10);

			if (this.oParent.CityWorker.F0_1d12_6c97_PlayerHasWonder(this.oParent.GameData.HumanPlayerID, WonderEnum.SETIProgram))
			{
				totalScience = (short)(totalScience + (totalScience / 2));
			}

			if (totalScience != 0 && y < 180)
			{
				int yearFactor = (this.oParent.GameData.Year < 0) ? 1 : ((this.oParent.GameData.Year < 1750) ? 2 : 3);

				// The original works in 16 bits: the cost of the next discovery is truncated before it is divided
				short discoveryCost = (short)(yearFactor * this.oParent.GameData.Players[this.oParent.GameData.HumanPlayerID].DiscoveredTechnologyCount *
					((this.oParent.GameData.DifficultyLevel * 2) + 6));
				int turns = discoveryCost / totalScience;

				this.oParent.DrawTools.DrawStringToScreen0(string.Create(CultureInfo.InvariantCulture, $"Discoveries: {turns} turns"), 8, y + 12, 10);
			}

			this.oParent.DrawTools.DrawStringToScreen0("Maintenance Costs", 160, 32, 15);

			y = 40;
			short totalCost = 0;

			for (int i = 0; i < 24; i++)
			{
				int cost = maintenance[i];

				if (cost == 0)
					continue;

				// The number of improvements of this type
				int count = (i == 1) ? (short)cost / (short)palaceMaintenance : cost / this.oParent.GameData.GetImprovementType(i + 1).MaintenanceCost;

				if (y < 180)
				{
					this.oParent.DrawTools.DrawStringToScreen0(
						string.Create(CultureInfo.InvariantCulture, $"{count} {this.oParent.GameData.GetImprovementType(i + 1).Name}, {cost}$"), 160, y, 14);

					y += 8;
				}

				totalCost = (short)(totalCost + cost);
			}

			this.oParent.DrawTools.DrawStringToScreen0(string.Create(CultureInfo.InvariantCulture, $"Total Cost: {totalCost}$"), 160, y + 4, 14);

			WaitAndCloseReport();
		}

		/// <summary>
		/// Shows Intelligence Report window.
		/// </summary>
		public void IntelligenceReport()
		{
			int humanPlayerID = this.oParent.GameData.HumanPlayerID;
			Player humanPlayer = this.oParent.GameData.Players[humanPlayerID];

			ShowReportHeader("INTELLIGENCE REPORT", 1);

			bool reportShown = false;
			int rowCount = 0;

			// The player shown on each row. A row without an embassy is counted but not stored, the original left whatever
			// was on the stack.
			int[] rowPlayerIDs = new int[8];

			for (int playerID = 1; playerID < 8; playerID++)
			{
				if ((this.oParent.GameData.ActiveCivilizations & (1 << playerID)) == 0)
					continue;

				Player player = this.oParent.GameData.Players[playerID];
				int y = (rowCount * 24) + 32;

				this.oParent.Graphics.F0_VGA_0599_DrawLine(this.oParent.Var_aa_Screen0_Rectangle, 4, y - 2, 316, y - 2, 9);

				if (playerID != humanPlayerID && !humanPlayer.Diplomacy[playerID].HasFlag(DiplomacyFlagsEnum.EmbassyEstablished))
				{
					this.oParent.DrawTools.DrawStringToScreen0("No embassy established.", 96, y, this.oParent.Array_1946_PlayerColours[playerID]);

					rowCount++;
					continue;
				}

				// The original also looks for the player's capital here, but doesn't use it

				this.oParent.DrawTools.DrawStringWithShadow($"{player.Nation}: {player.Name}", 8, y, 15);

				string status = string.Create(CultureInfo.InvariantCulture,
					$"{this.oParent.Array_1966_GovernmentNames[(int)player.GovernmentType]}, {player.Coins}$");

				if (this.oParent.GameData.DifficultyLevel <= 1)
				{
					status += string.Create(CultureInfo.InvariantCulture, $", {player.UnitCount} Units.");
				}

				this.oParent.DrawTools.DrawStringToScreen0(status, 160, y, this.oParent.Array_1946_PlayerColours[playerID]);

				y += 8;

				// Relations with the other players
				int x = 16;

				for (int otherPlayerID = 1; otherPlayerID < 8; otherPlayerID++)
				{
					string nation = this.oParent.GameData.Players[otherPlayerID].Nation;
					DiplomacyFlagsEnum diplomacy = player.Diplomacy[otherPlayerID];

					if (x > 240)
					{
						x = 16;
						y += 8;
					}

					if (diplomacy.HasFlag(DiplomacyFlagsEnum.Peace))
					{
						nation += diplomacy.HasFlag(DiplomacyFlagsEnum.Allied) ? ":\x008f ALLIED" : ":\x008f PEACE";

						this.oParent.DrawTools.DrawStringToScreen0(nation, x, y, 7);
						x += 96;
					}

					if ((diplomacy & (DiplomacyFlagsEnum.Contact | DiplomacyFlagsEnum.Peace)) == DiplomacyFlagsEnum.Contact)
					{
						nation += ":\x008c WAR";

						this.oParent.DrawTools.DrawStringToScreen0(nation, x, y, 7);
						x += 96;
					}
				}

				rowPlayerIDs[rowCount] = playerID;
				rowCount++;

				if (playerID != humanPlayerID)
				{
					// Button to show the player's details
					this.oParent.DrawTools.DrawFilledRectangleWithCenteredText(281, (rowCount * 24) + 20, 318, (rowCount * 24) + 29,
						string.Create(CultureInfo.InvariantCulture, $"INFO{rowCount}"), this.oParent.Array_1946_PlayerColours[playerID]);
				}
			}

			bool close;

			do
			{
				MouseEvent mouseEvent;

				do
				{
					mouseEvent = this.oParent.GetMouseEvent();
				}
				while (mouseEvent.Buttons == MouseButtonsEnum.None && this.oParent.CAPI.kbhit() == 0);

				close = true;

				// A row is chosen with the keys 1 to 7, or by clicking an INFO button
				int row = -1;
				bool rowChosen = false;

				if (this.oParent.CAPI.kbhit() != 0)
				{
					int key = this.oParent.CommonTools.GetNavigationKey();

					if (key >= '1' && key <= '7')
					{
						row = key - '1';
						rowChosen = true;
					}
				}

				if (!rowChosen && mouseEvent.Position.X > 281)
				{
					row = (mouseEvent.Position.Y - 28) / 24;
					rowChosen = true;
				}

				if (rowChosen && row >= 0 && row < rowCount)
				{
					int playerID = rowPlayerIDs[row];

					if (playerID != humanPlayerID && humanPlayer.Diplomacy[playerID].HasFlag(DiplomacyFlagsEnum.EmbassyEstablished))
					{
						PlayerIntelligenceReport(playerID);

						reportShown = true;
						close = false;
					}
				}

				this.oParent.CommonTools.ClearKeyboardAndMouseEvents();
			}
			while (!close);

			if (reportShown)
			{
				this.oParent.GameEngine.F0_1238_1b44_RedrawMapWithBlankingEffect();
			}
			else
			{
				this.oParent.Graphics.F0_VGA_07d8_DrawImage(this.oParent.Var_19d4_Screen1_Rectangle, 0, 0, 320, 200, this.oParent.Var_aa_Screen0_Rectangle, 0, 0);
			}
		}

		/// <summary>
		/// Shows the intelligence report about a player: leader, capital, government, treasury, military, foreign affairs
		/// and technologies
		/// </summary>
		/// <param name="playerID"></param>
		public void PlayerIntelligenceReport(int playerID)
		{
			Player player = this.oParent.GameData.Players[playerID];
			NationDefinition nation = this.oParent.GameData.Nations[player.NationalityID];

			ShowReportHeader("INTELLIGENCE REPORT", 1);

			int y = 32;

			this.oParent.DrawTools.DrawStringWithShadow($"Subject: the {player.Nation}", 16, 32, 15);
			y += 12;

			this.oParent.DrawTools.DrawStringToScreen0($"Leader:\x008f {this.oParent.Array_19b2_GovernmentTypeNames[(int)player.GovernmentType]} {player.Name}", 16, y, 9);
			y += 8;

			if (nation.Mood == 1)
			{
				this.oParent.DrawTools.DrawStringToScreen0("Aggressive", 24, y, 7);
				y += 8;
			}

			if (nation.Mood == -1)
			{
				this.oParent.DrawTools.DrawStringToScreen0("Friendly", 24, y, 7);
				y += 8;
			}

			if (nation.Policy == 1)
			{
				this.oParent.DrawTools.DrawStringToScreen0("Expansionistic", 24, y, 7);
				y += 8;
			}

			if (nation.Policy == -1)
			{
				this.oParent.DrawTools.DrawStringToScreen0("Perfectionist", 24, y, 7);
				y += 8;
			}

			if (nation.Ideology == 1)
			{
				this.oParent.DrawTools.DrawStringToScreen0("Civilized", 24, y, 7);
				y += 8;
			}

			if (nation.Ideology == -1)
			{
				this.oParent.DrawTools.DrawStringToScreen0("Militaristic", 24, y, 7);
				y += 8;
			}

			y += 4;

			// The capital is the last city of the player with a Palace, or none
			int capitalCityID = -1;

			for (int cityID = 0; cityID < 128; cityID++)
			{
				City city = this.oParent.GameData.Cities[cityID];

				if (city.PlayerID == playerID && city.Status != CityStatusEnum.Invalid && (city.ImprovementFlags0 & 0x1) != 0)
				{
					capitalCityID = cityID;
				}
			}

			this.oParent.DrawTools.DrawStringToScreen0($"Capital:\x008f {this.oParent.Actions.F0_2459_08c6_GetCityName(capitalCityID)}", 16, y, 9);
			y += 8;

			this.oParent.DrawTools.DrawStringToScreen0($"Government:\x008f {this.oParent.Array_1966_GovernmentNames[(int)player.GovernmentType]}", 16, y, 9);
			y += 8;

			this.oParent.DrawTools.DrawStringToScreen0(string.Create(CultureInfo.InvariantCulture, $"Treasury:\x008f {player.Coins}$"), 16, y, 9);
			y += 8;

			if (this.oParent.GameData.DifficultyLevel <= 1)
			{
				this.oParent.DrawTools.DrawStringToScreen0(string.Create(CultureInfo.InvariantCulture, $"Military:\x008f {player.UnitCount} Units."), 16, y, 9);
				y += 12;
			}

			this.oParent.DrawTools.DrawStringToScreen0("Foreign Affairs:", 16, y, 9);
			y += 8;

			string relation = "";

			for (int otherPlayerID = 1; otherPlayerID < 8; otherPlayerID++)
			{
				DiplomacyFlagsEnum diplomacy = player.Diplomacy[otherPlayerID];

				if (diplomacy.HasFlag(DiplomacyFlagsEnum.Peace))
				{
					relation = "At\x008f PEACE";
				}

				if ((diplomacy & (DiplomacyFlagsEnum.Contact | DiplomacyFlagsEnum.Peace)) == DiplomacyFlagsEnum.Contact)
				{
					relation = "At\x008c WAR";
				}

				if (diplomacy.HasFlag(DiplomacyFlagsEnum.Contact))
				{
					this.oParent.DrawTools.DrawStringToScreen0($"{relation}\x0087 with the {this.oParent.GameData.Players[otherPlayerID].Nation}.", 24, y, 7);
					y += 8;
				}
			}

			y += 4;

			this.oParent.DrawTools.DrawStringToScreen0("Technologies:", 16, y, 9);
			y += 8;

			// From the most advanced technology down, until the page is full
			for (int technology = 71; technology >= 0; technology--)
			{
				if (this.oParent.Technology.F0_1ade_22b5_PlayerHasTechnology(playerID, (TechnologyAdvanceEnum)technology))
				{
					this.oParent.DrawTools.DrawStringToScreen0(this.oParent.GameData.TechnologyAdvances[technology].Name, 24, y, 7);
					y += 8;

					if (y >= 192)
						break;
				}
			}

			WaitAndCloseReport();
		}

		/// <summary>
		/// Shows Attitude Report window.
		/// </summary>
		/// <param name="playerID"></param>
		public void AttitudeReport(short playerID)
		{
			ShowReportHeader("ATTITUDE SURVEY", 9);

			int y = 32;
			int unhappyCount = 0;
			int happyCount = 0;
			int citizenCount = 0;
			int resourcesXOffset = 0;

			for (int cityID = 0; cityID < 128; cityID++)
			{
				City city = this.oParent.GameData.Cities[cityID];

				if (city.Status == CityStatusEnum.Invalid || city.PlayerID != playerID)
					continue;

				if (y > 190)
				{
					// Next page
					this.oParent.Actions.F0_2459_0918_WaitForKeyPressOrMouseClick();

					DrawReportHeader("ATTITUDE SURVEY", 9);

					y = 32;
				}

				this.oParent.CityWorker.F0_1d12_0045_ProcessCityState(cityID, -1);

				this.oParent.DrawTools.DrawStringToScreen0($"{this.oParent.Actions.F0_2459_08c6_GetCityName(cityID)}:", 16, y, 15);

				// Draw the citizens, shifted a little on each row
				this.oParent.CityWorker.F0_1d12_6ed4_DrawResources(cityID, (resourcesXOffset & 0x7) + 80, y - 3, this.oParent.Var_e8b8, 124);

				// The improvements that make citizens content
				int x = 212;

				this.oParent.DrawTools.FillRectangle(this.oParent.Var_aa_Screen0_Rectangle, 212, y - 1, 90, 10, 11);

				for (int i = 0; i < 24; i++)
				{
					// The original shifts in 16 bits, so bits 16 and above are never set
					int bit = (i < 16) ? (1 << i) : 0;

					if ((city.ImprovementFlags0 & bit & 0x2618) != 0)
					{
						this.oParent.CityWorker.F0_1d12_7045_DrawWonderOrImprovement(i + 1, x, y - 1);
						x += 18;
					}
				}

				citizenCount += (sbyte)city.ActualSize;
				happyCount += (short)this.oParent.Var_70e2_CityHappyCitizenCount;
				unhappyCount += (short)this.oParent.Var_70e4_CityUnhappyCitizenCount;
				resourcesXOffset += 4;
				y += 10;
			}

			if (citizenCount == 0)
			{
				citizenCount++;
			}

			this.oParent.DrawTools.DrawStringToScreen0(
				$"Population: {this.oParent.GameEngine.PopulationValueToString(this.oParent.GameEngine.GetPlayerPopulationCount(playerID))}" +
				$" Happy:{Percentage(happyCount, citizenCount)}" +
				$"% Content:{Percentage(citizenCount - happyCount - unhappyCount, citizenCount)}" +
				$"% Unhappy:{Percentage(unhappyCount, citizenCount)}%", 16, y + 8, 15);

			WaitAndCloseReport();
		}

		/// <summary>
		/// Returns value as a percentage of total
		/// </summary>
		/// <param name="value"></param>
		/// <param name="total"></param>
		public string Percentage(int value, int total)
		{
			int percentage;

			if ((short)value < 300)
			{
				percentage = (100 * (short)value) / (short)total;
			}
			else
			{
				percentage = (10 * (short)value) / ((short)total / 10);
			}

			return percentage.ToString(CultureInfo.InvariantCulture);
		}

		/// <summary>
		/// Shows City Status window.
		/// </summary>
		/// <param name="playerID"></param>
		public void CityStatus(short playerID)
		{
			ShowReportHeader("CITY STATUS", 8);

			int y = 32;

			for (int cityID = 0; cityID < 128; cityID++)
			{
				City city = this.oParent.GameData.Cities[cityID];

				if (city.Status == CityStatusEnum.Invalid || city.PlayerID != playerID)
					continue;

				if (y >= 192)
				{
					// Next page
					this.oParent.Actions.F0_2459_0918_WaitForKeyPressOrMouseClick();

					DrawReportHeader("CITY STATUS", 8);

					y = 32;
				}

				this.oParent.CityWorker.F0_1d12_0045_ProcessCityState(cityID, -1);

				this.oParent.DrawTools.DrawStringToScreen0(this.oParent.Actions.F0_2459_08c6_GetCityName(cityID), 8, y, 15);

				// Size, food, shields and trade
				string text = string.Create(CultureInfo.InvariantCulture,
					$"{city.ActualSize}-{this.oParent.Var_70da_Arr[0]}{{ {this.oParent.Var_70da_Arr[1]}| {this.oParent.Var_70da_Arr[2]}}}");

				this.oParent.DrawTools.DrawStringToScreen0(text, 80, y, 15);

				if (city.CurrentProductionID < 0)
				{
					ImprovementDefinition improvement = this.oParent.GameData.GetImprovementType(-city.CurrentProductionID);

					text = this.oParent.LanguageTools.F0_2f4d_04f7_TrimStringToWidth(improvement.Name, 96) +
						string.Create(CultureInfo.InvariantCulture, $"\x0087 ({city.ShieldsCount}/{10 * improvement.Cost})");
				}
				else if (city.CurrentProductionID < 0x40)
				{
					UnitDefinition unit = this.oParent.GameData.Units[city.CurrentProductionID];

					text = unit.Name + string.Create(CultureInfo.InvariantCulture, $"\x0087 ({city.ShieldsCount}/{10 * unit.Cost})");
				}

				// For any other production the original draws the size text again
				this.oParent.DrawTools.DrawStringToScreen0(text, 172, y, 15);

				y += 8;
			}

			WaitAndCloseReport();
		}
	}
}
