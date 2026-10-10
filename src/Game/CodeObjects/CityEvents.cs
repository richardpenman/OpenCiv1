using OpenCivOne.Graphics;

namespace OpenCivOne
{
	/// <summary>
	/// Founding cities, and random events in cities: disasters and riots
	/// </summary>
	public class CityEvents
	{
		private OpenCivOneGame oParent;

		public CityEvents(OpenCivOneGame parent)
		{
			this.oParent = parent;
		}

		/// <summary>
		/// Founds a new city for the player
		/// </summary>
		/// <param name="playerID"></param>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="citySize"></param>
		/// <returns>The ID of the new city, or -1 if there was no room for another city or the player didn't name it</returns>
		public int FoundCity(int playerID, int x, int y, int citySize)
		{
			GameData gameData = this.oParent.GameData;
			MapManagement map = this.oParent.MapManagement;

			this.oParent.AIEngine.F0_25fb_3401_PlayerClearContinentPolicies(playerID, UnitRoleTypeEnum.Settler, x, y, 4);

			int cityID = Array.FindIndex(gameData.Cities, city => city.Status == CityStatusEnum.Invalid);

			if (cityID == -1)
			{
				if (playerID == gameData.HumanPlayerID)
				{
					this.oParent.Var_2f9e_MessageBoxStyle = MenuBoxReportTypeEnum.DomesticAdvisorReport;
					this.oParent.MenuBoxDialog.ShowDialog(this.oParent.LanguageTools.F0_2f4d_044f_GetTextFromKingSection("*PMAX"), 64, 80);
				}

				return -1;
			}

			City city = gameData.Cities[cityID];
			int nameID = FindUnusedCityName(gameData.Players[playerID].NationalityID);

			city.NameID = (byte)nameID;
			gameData.CityPositions[nameID] = new GPoint(x, y);

			if ((gameData.PlayerFlags & (1 << playerID)) != 0)
			{
				if (this.oParent.TextBoxDialogues.F23_0000_0000_CityNameDialog(cityID) == 0)
				{
					return -1;
				}

				string cityName = gameData.CityNames[city.NameID];
				gameData.CityNames[city.NameID] = (cityName.Length > 12) ? cityName.Substring(0, 12) : cityName;
			}

			gameData.CityNames[city.NameID] = gameData.CityNames[city.NameID].TrimEnd(' ', '\x0');

			this.oParent.UnitManagement.F0_1866_250e_AddReplayData(1, (byte)((sbyte)playerID), city.NameID, (byte)x, (byte)y);

			map.F0_2aea_16ee_ClearTerrainImprovements(x, y, TerrainImprovementFlagsEnum.Fortress | TerrainImprovementFlagsEnum.Mines);
			map.F0_2aea_1653_SetTerrainImprovements(x, y, TerrainImprovementFlagsEnum.City | TerrainImprovementFlagsEnum.Road);

			if (gameData.TerrainModifications[(int)map.GetTerrainType(x, y)].IrrigationEffect < -1)
			{
				map.F0_2aea_1653_SetTerrainImprovements(x, y, TerrainImprovementFlagsEnum.Irrigation);
			}

			city.Status = 0;
			city.Position = new GPoint(x, y);
			city.ActualSize = (sbyte)citySize;
			city.VisibleSize = 0;
			city.WorkerFlags = 0;
			city.BaseTrade = 0;
			city.FoodCount = 0;
			city.ShieldsCount = 0;
			city.ImprovementFlags = 0;

			// The city starts building Militia, Musketeers or Riflemen
			city.CurrentProductionID = 1;

			if (this.oParent.Technology.F0_1ade_22b5_PlayerHasTechnology(playerID, TechnologyAdvanceEnum.Gunpowder))
			{
				city.CurrentProductionID = 4;
			}

			if (this.oParent.Technology.F0_1ade_22b5_PlayerHasTechnology(playerID, TechnologyAdvanceEnum.Conscription))
			{
				city.CurrentProductionID = 5;
			}

			city.FirstFortifiedCityUnits[0] = -1;
			city.FirstFortifiedCityUnits[1] = -1;

			// The player's first city gets a Palace. The original doesn't check whether the other city slots are used.
			bool firstCity = true;

			for (int i = 0; i < 128; i++)
			{
				if (gameData.Cities[i].PlayerID == playerID && i != cityID)
				{
					firstCity = false;
				}
			}

			if (firstCity)
			{
				city.ImprovementFlags = 1;
			}

			city.BaseTrade = 0;
			city.PlayerID = (short)playerID;

			for (int i = 0; i < 3; i++)
			{
				city.TradeCityIDs[i] = -1;
			}

			city.SpecialWorkerFlags = 0;

			for (int i = 1; i <= 8; i++)
			{
				GPoint direction = this.oParent.MoveDirections[i];
				TerrainTypeEnum terrainType = map.GetTerrainType(x + direction.X, y + direction.Y);

				// Next to an ocean, or to any water for the human player
				if (terrainType == TerrainTypeEnum.Water &&
					(map.F0_2aea_195d_GetMapGroupSize(x + direction.X, y + direction.Y) > 256 || playerID == gameData.HumanPlayerID))
				{
					city.Status |= CityStatusEnum.OceanAccess;
				}

				if (terrainType == TerrainTypeEnum.River || terrainType == TerrainTypeEnum.Mountains)
				{
					city.Status |= CityStatusEnum.NearRiverOrMountains;
				}
			}

			if (firstCity && playerID == gameData.HumanPlayerID)
			{
				this.oParent.Help.F4_0000_02d3_ShowInstantAdvicePopup("*FIRSTPRODUCT");
			}

			for (int i = 0; i < 45; i++)
			{
				GPoint direction = this.oParent.MoveDirections[i];
				int cellX = map.AdjustXPosition(x + direction.X);
				int cellY = y + direction.Y;

				if (i <= 20)
				{
					// The city's own squares
					map.SetPlayerLandOwnership(cellX, cellY, playerID);
				}
				else if (map.GetPlayerLandOwnership(cellX, cellY) >= 8)
				{
					map.SetPlayerLandOwnership(cellX, cellY, 8);
				}

				// A computer player's city founded next to one of the human player's cities is seen by the human player
				if (playerID != gameData.HumanPlayerID && i <= 24 &&
					(map.F0_2aea_1585_GetVisibleTerrainImprovements(cellX, cellY) & TerrainImprovementFlagsEnum.City) != 0 &&
					map.F0_2aea_1369_GetCityOwner(cellX, cellY) == gameData.HumanPlayerID)
				{
					city.VisibleSize = 1;
					map.F0_2aea_1601_UpdateVisibleCellStatus(x, y);
				}
			}

			if (playerID == gameData.HumanPlayerID)
			{
				if (gameData.GameSettingFlags.Animations && !this.oParent.GlobalPreferences.Debug.FastMode)
				{
					this.oParent.Animations.F22_0000_0eaf_FoundNewCityAnimation(cityID);
				}

				this.oParent.CityManagement.F0_1ade_03ea(cityID);
				this.oParent.GameEngine.F0_1238_1b44_RedrawMapWithBlankingEffect();
			}
			else
			{
				city.CurrentProductionID = (sbyte)(byte)this.oParent.CityManagement.F0_1ade_0421(playerID, cityID);

				// With the Apollo Program the human player sees every city
				if (this.oParent.CityWorker.F0_1d12_6c97_PlayerHasWonder(gameData.HumanPlayerID, WonderEnum.ApolloProgram))
				{
					gameData.MapVisibility[city.Position.X, city.Position.Y] |= (ushort)(1 << gameData.HumanPlayerID);
					city.VisibleSize = 1;
					map.F0_2aea_1601_UpdateVisibleCellStatus(city.Position.X, city.Position.Y);
				}
			}

			return cityID;
		}

		/// <summary>
		/// Finds a city name that isn't used yet. Each nation has 16 city names; when they are used up the names from 224 on
		/// are tried, and then all names from the start. If every name is used, name 0 is reused.
		/// </summary>
		/// <param name="nationalityID"></param>
		/// <returns></returns>
		private int FindUnusedCityName(int nationalityID)
		{
			// The barbarians (0) have no names, nation 8 is skipped
			int nationIndex = nationalityID - 1;

			if (nationIndex >= 7)
			{
				nationIndex--;
			}

			int nameID = nationIndex << 4;
			int firstNameID = nameID;

			while (this.oParent.GameData.CityPositions[nameID].X != -1)
			{
				nameID++;

				if (nameID == firstNameID + 16)
				{
					nameID = 224;
				}

				if (nameID == 256)
				{
					nameID = 0;

					if (firstNameID == 999)
						break;

					firstNameID = 999;
				}
			}

			return nameID;
		}

		/// <summary>
		/// A random event in a city: a disaster, or a riot if the city is unhappy
		/// </summary>
		/// <param name="cityID"></param>
		public void RandomCityEvent(short cityID)
		{
			GameData gameData = this.oParent.GameData;
			City city = gameData.Cities[cityID];
			Player owner = gameData.Players[city.PlayerID];
			string cityName = this.oParent.Actions.F0_2459_08c6_GetCityName(cityID);

			// The terrain around the city, one bit for each terrain type
			int nearbyTerrain = 0;

			for (int i = 0; i <= 8; i++)
			{
				GPoint direction = this.oParent.MoveDirections[i];

				nearbyTerrain |= 1 << (int)this.oParent.MapManagement.GetTerrainType(city.Position.X + direction.X, city.Position.Y + direction.Y);
			}

			this.oParent.Array_30b8[3] = cityName;

			string message = "";

			switch (this.oParent.CAPI.RNG.Next(10))
			{
				case 0:
					// Earthquake: next to hills, destroys an improvement
					if ((nearbyTerrain & (1 << (int)TerrainTypeEnum.Hills)) != 0 && (city.ImprovementFlags0 & 0xfffe) != 0)
					{
						int improvement = DestroyRandomImprovement(city);

						message = $"Earthquake in {cityName}!\n{gameData.GetImprovementType(improvement + 1).Name} destroyed.\n";
					}
					break;

				case 1:
					// Plague: the player has Construction, but no Medicine and no Aqueduct in the city
					if (!this.oParent.Technology.F0_1ade_22b5_PlayerHasTechnology(city.PlayerID, TechnologyAdvanceEnum.Medicine) &&
						(city.ImprovementFlags0 & 0x100) == 0 &&
						this.oParent.Technology.F0_1ade_22b5_PlayerHasTechnology(city.PlayerID, gameData.GetImprovementType(9).RequiresTechnology))
					{
						message = this.oParent.LanguageTools.F0_2f4d_044f_GetTextFromKingSection("*D1");

						if (city.ActualSize != 0)
						{
							city.ActualSize -= (sbyte)(city.ActualSize / 4);
						}
					}
					break;

				case 2:
					// Flood: next to a river, the player has Masonry, no City Walls in the city
					if ((nearbyTerrain & (1 << (int)TerrainTypeEnum.River)) != 0 && (city.ImprovementFlags0 & 0x80) == 0 &&
						this.oParent.Technology.F0_1ade_22b5_PlayerHasTechnology(city.PlayerID, gameData.GetImprovementType(8).RequiresTechnology))
					{
						message = this.oParent.LanguageTools.F0_2f4d_044f_GetTextFromKingSection("*D2");

						if (city.ActualSize > 1)
						{
							city.ActualSize -= (sbyte)(city.ActualSize / 4);
						}
					}
					break;

				case 3:
					// Volcano: next to mountains, the player has Ceremonial Burial, no Temple in the city
					if ((nearbyTerrain & (1 << (int)TerrainTypeEnum.Mountains)) != 0 && (city.ImprovementFlags0 & 0x8) == 0 &&
						this.oParent.Technology.F0_1ade_22b5_PlayerHasTechnology(city.PlayerID, gameData.GetImprovementType(4).RequiresTechnology))
					{
						message = this.oParent.LanguageTools.F0_2f4d_044f_GetTextFromKingSection("*D3");

						if (city.ActualSize > 1)
						{
							city.ActualSize -= (sbyte)(city.ActualSize / 3);
						}
					}
					break;

				case 4:
					// Famine: the player has Pottery, no Granary in the city
					if (city.ActualSize != 0 && (city.ImprovementFlags0 & 0x4) == 0 &&
						this.oParent.Technology.F0_1ade_22b5_PlayerHasTechnology(city.PlayerID, gameData.GetImprovementType(3).RequiresTechnology))
					{
						message = this.oParent.LanguageTools.F0_2f4d_044f_GetTextFromKingSection("*D4");

						city.FoodCount = 0;
						city.ActualSize -= (sbyte)(city.ActualSize / 3);

						message += "Citizens demand GRANARY.\n";
					}
					break;

				case 5:
					// Fire: the player has Construction, no Aqueduct in the city, destroys an improvement
					if ((city.ImprovementFlags0 & 0xfffe) != 0 && (city.ImprovementFlags0 & 0x100) == 0 &&
						this.oParent.Technology.F0_1ade_22b5_PlayerHasTechnology(city.PlayerID, gameData.GetImprovementType(9).RequiresTechnology))
					{
						int improvement = DestroyRandomImprovement(city);

						this.oParent.Array_30b8[0] = gameData.GetImprovementType(improvement + 1).Name;
						message = this.oParent.LanguageTools.F0_2f4d_044f_GetTextFromKingSection("*D5");
					}
					break;

				case 6:
					// Pirates: next to water, no Barracks in the city
					if ((nearbyTerrain & (1 << (int)TerrainTypeEnum.Water)) != 0 && (city.ImprovementFlags0 & 0x2) == 0)
					{
						message = this.oParent.LanguageTools.F0_2f4d_044f_GetTextFromKingSection("*D6");

						city.FoodCount = 0;
						city.ShieldsCount = 0;
					}
					break;

				default:
					message = Riot(cityID, city, owner, cityName);
					break;
			}

			if (message.Length == 0)
				return;

			if ((gameData.PlayerFlags & (1 << city.PlayerID)) != 0)
			{
				this.oParent.Var_2f9e_MessageBoxStyle = MenuBoxReportTypeEnum.DomesticAdvisorReport;
				this.oParent.MenuBoxDialog.ShowDialog(message, 100, 80);
				this.oParent.PlayerTurn.F0_1403_3ed7_DrawVisibleCell(city.Position.X, city.Position.Y);
			}
			else if ((gameData.MapVisibility[city.Position.X, city.Position.Y] & (1 << gameData.HumanPlayerID)) != 0)
			{
				// The human player hears about other players' cities they can see, but only the first line
				this.oParent.Var_2f9e_MessageBoxStyle = MenuBoxReportTypeEnum.TravelersReport;
				this.oParent.MenuBoxDialog.ShowDialog(message.Substring(0, message.IndexOf('\n', StringComparison.Ordinal) + 1), 100, 64);
				this.oParent.MapManagement.F0_2aea_0008_DrawVisibleMap(gameData.HumanPlayerID, city.Position.X - 10, city.Position.Y - 6);
			}
		}

		/// <summary>
		/// Riots, corruption or a scandal in an unhappy city. The city may defect to the player with the most prosperous
		/// city nearby.
		/// </summary>
		/// <returns>The message for the player, or an empty string</returns>
		private string Riot(short cityID, City city, Player owner, string cityName)
		{
			GameData gameData = this.oParent.GameData;

			this.oParent.CityWorker.F0_1d12_0045_ProcessCityState(cityID, -1);

			if (this.oParent.Var_70e2_CityHappyCitizenCount >= this.oParent.Var_70e4_CityUnhappyCitizenCount)
				return "";

			string message;

			switch (this.oParent.CAPI.RNG.Next(3))
			{
				case 0:
					message = "Rioting in ";
					break;

				case 1:
					message = "Corruption in ";
					break;

				default:
					message = "Scandal in ";
					break;
			}

			message += $"{cityName}!\nCitizens demand ";

			if ((city.ImprovementFlags0 & 0x8) == 0)
			{
				message += "TEMPLE\n";
			}
			else if ((city.ImprovementFlags0 & 0x40) == 0)
			{
				message += "COURTHOUSE\n";
			}
			else if ((city.ImprovementFlags0 & 0x10) == 0)
			{
				message += "MARKETPLACE\n";
			}
			else if (this.oParent.Technology.F0_1ade_22b5_PlayerHasTechnology(city.PlayerID, TechnologyAdvanceEnum.Religion) &&
				!city.HasImprovement(ImprovementEnum.Cathedral))
			{
				message += "CATHEDRAL\n";
			}
			else
			{
				message += "lower taxes.\n";
			}

			if ((gameData.PlayerFlags & (1 << city.PlayerID)) != 0)
			{
				this.oParent.MenuBoxDialog.ShowDialog(message, 100, 80);
				this.oParent.PlayerTurn.F0_1403_3ed7_DrawVisibleCell(city.Position.X, city.Position.Y);
				message = "";
			}

			city.FoodCount = 0;
			city.ShieldsCount = 0;

			// A city without a Palace, of a player with more than 3 cities, may defect
			if (this.oParent.Var_70e2_CityHappyCitizenCount > this.oParent.Var_70e4_CityUnhappyCitizenCount ||
				(city.ImprovementFlags0 & 0x1) != 0 || owner.CityCount <= 3)
				return message;

			// The most prosperous city nearby. The original only looks at the cities before this one, and stops at the first
			// unused city slot.
			int bestScore = 4;
			int bestCityID = -1;

			for (int otherCityID = 0; otherCityID < 128 && otherCityID != cityID &&
				gameData.Cities[otherCityID].Status != CityStatusEnum.Invalid; otherCityID++)
			{
				short distance = (short)this.oParent.MapManagement.GetDistance(city.Position, gameData.Cities[otherCityID].Position);

				this.oParent.CityWorker.F0_1d12_0045_ProcessCityState(otherCityID, -1);

				int score = (short)((this.oParent.Var_70e2_CityHappyCitizenCount - this.oParent.Var_70e4_CityUnhappyCitizenCount) << 5) / distance;

				if (score > bestScore)
				{
					bestScore = score;
					bestCityID = otherCityID;
				}
			}

			if (bestCityID == -1 || gameData.Cities[bestCityID].PlayerID == city.PlayerID)
				return message;

			if ((gameData.MapVisibility[city.Position.X, city.Position.Y] & (1 << gameData.HumanPlayerID)) != 0)
			{
				this.oParent.MenuBoxDialog.ShowDialog(
					$"Residents of {cityName} admire\nthe prosperity of {this.oParent.Actions.F0_2459_08c6_GetCityName(bestCityID)}.\n", 80, 80);
			}

			this.oParent.DiplomatActions.F22_0000_0af5_InciteCityRevolt(cityID, gameData.Cities[bestCityID].PlayerID);

			return "";
		}

		/// <summary>
		/// Destroys a random improvement in the city, other than the Palace
		/// </summary>
		/// <returns>The bit of the destroyed improvement in the city's improvement flags</returns>
		private int DestroyRandomImprovement(City city)
		{
			int improvement;

			// The original makes the 32-bit mask with a 16-bit shift, sign extended into the high word. So it only picks
			// improvements 1 to 15, and picking 15 also clears all the improvements from 16 on.
			do
			{
				do
				{
					improvement = this.oParent.CAPI.RNG.Next(24);
				}
				while (improvement == 0);
			}
			while ((city.ImprovementFlags & SignExtendedMask((ushort)(1 << improvement))) == 0);

			city.ImprovementFlags &= SignExtendedMask((ushort)~(1 << improvement));

			return improvement;
		}

		/// <summary>
		/// Extends a 16-bit value to 32 bits with its sign, as the x86 CWD instruction does
		/// </summary>
		private static uint SignExtendedMask(ushort value)
		{
			return ((value & 0x8000) != 0) ? (0xffff0000 | value) : value;
		}
	}
}
