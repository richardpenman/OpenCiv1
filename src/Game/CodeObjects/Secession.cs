using Avalonia.Media.TextFormatting.Unicode;
using IRB.VirtualCPU;

namespace OpenCivOne
{
	public class Secession
	{
		private OpenCivOneGame parent;

		public Secession(OpenCivOneGame parent)
		{
			this.parent = parent;
		}

		/// <summary>
		/// When prerequisites are fulfilled the nation will go into Secession (split in half)
		/// The new player is AI player
		/// </summary>
		/// <param name="playerID"></param>
		/// <returns>true is successful, otherwise false</returns>
		public bool F15_0000_0000_NationSecession(int playerID)
		{
			//this.oCPU.Log.EnterBlock($"F15_0000_0000({playerID})");

			// function body
			int[] groupPopulationSizes = new int[16];

			int newPlayerID = -1;

			for (int i = 1; i < 8; i++)
			{
				if (this.parent.GameData.Players[i].UnitCount == 0 && this.parent.GameData.Players[i].CityCount == 0)
				{
					newPlayerID = i;
					break;
				}
			}
		
			if (newPlayerID == -1)
			{
				return false;
			}

			for (int i = 0; i < 16; i++)
			{
				groupPopulationSizes[i] = 0;
			}

			int oldMainCityGroupID = -1;
			int oldMainCityID = -1;
			int oldMainCityX = -1;
			int oldMainCityY = -1;

			for (int i = 0; i < 128; i++)
			{
				if (this.parent.GameData.Cities[i].StatusFlag != 0xff && this.parent.GameData.Cities[i].PlayerID == playerID)
				{
					// Instruction address 0x0000:0x0086, size: 5
					int groupID = this.parent.MapManagement.F0_2aea_1942_GetGroupID(
						this.parent.GameData.Cities[i].Position.X, this.parent.GameData.Cities[i].Position.Y);

					groupPopulationSizes[groupID] += this.parent.GameData.Cities[i].ActualSize;

					if (this.parent.GameData.Cities[i].HasImprovement(ImprovementEnum.Palace))
					{
						if (oldMainCityGroupID == -1 || this.parent.GameData.Players[playerID].XStart == this.parent.GameData.Cities[i].Position.X)
						{
							// Instruction address 0x0000:0x00cc, size: 5
							oldMainCityGroupID = this.parent.MapManagement.F0_2aea_1942_GetGroupID(
								this.parent.GameData.Cities[i].Position.X, this.parent.GameData.Cities[i].Position.Y);

							oldMainCityX = this.parent.GameData.Cities[i].Position.X;
							oldMainCityY = this.parent.GameData.Cities[i].Position.Y;
							oldMainCityID = i;
						}
					}
				}
			}

			if (oldMainCityGroupID == -1)
			{
				return false;
			}
		
			this.parent.GameData.ActiveCivilizations |= (short)(0x1 << newPlayerID);
			this.parent.GameData.ActiveCivilizations ^= (short)(0x1 << newPlayerID);

			if (!this.parent.StartGameMenu.F5_0000_07c7_InitPlayerData(newPlayerID))
			{
				return false;
			}

			this.parent.GameData.ActiveCivilizations |= (short)(0x1 << newPlayerID);

			int nationalityID = this.parent.GameData.Players[newPlayerID].NationalityID;

			if ((newPlayerID > this.parent.GameData.AIOpponentCount + 1) || (nationalityID & ((newPlayerID != 7) ? 1 : 0)) != 0 || nationalityID > 15)
			{
				// Instruction address 0x0000:0x0173, size: 5
				if (this.parent.CAPI.RNG.Next(2) != 0)
				{
					this.parent.GameData.Players[newPlayerID].NationalityID = (short)newPlayerID;
				}
				else
				{
					this.parent.GameData.Players[newPlayerID].NationalityID = (short)(newPlayerID + 8);
				}

				if (this.parent.GameData.Players[newPlayerID].NationalityID >= 8)
				{
					this.parent.GameData.PlayerIdentityFlags |= (short)(0x1 << newPlayerID);
				}
			}
			else
			{
				this.parent.GameData.Players[newPlayerID].NationalityID ^= 8;
			}

			// Instruction address 0x0000:0x01bd, size: 5
			this.parent.GameData.Players[newPlayerID].Name = this.parent.GameData.Nations[this.parent.GameData.Players[newPlayerID].NationalityID].Leader;

			// Instruction address 0x0000:0x01d4, size: 5
			this.parent.GameData.Players[newPlayerID].Nationality = this.parent.GameData.Nations[this.parent.GameData.Players[newPlayerID].NationalityID].Nationality;

			// Instruction address 0x0000:0x01eb, size: 5
			this.parent.GameData.Players[newPlayerID].Nation = this.parent.GameData.Nations[this.parent.GameData.Players[newPlayerID].NationalityID].Nation;

			// Instruction address 0x0000:0x0241, size: 5
			this.parent.Array_30b8[0] = this.parent.GameData.Players[playerID].Nationality;

			// Instruction address 0x0000:0x0251, size: 5
			this.parent.Array_30b8[1] = this.parent.GameData.Players[newPlayerID].Nationality;

			this.parent.Var_2f9e_MessageBoxStyle = MenuBoxReportTypeEnum.ForeignMinisterReport;

			// Instruction address 0x0000:0x0281, size: 5
			this.parent.Segment_1238.F0_1238_001e_ShowDialog(
				this.parent.LanguageTools.F0_2f4d_044f_GetTextFromKingSection(((playerID == this.parent.GameData.HumanPlayerID) ? "*SCHISM" : "*ESCHISM")), 80, 80);

			int newTreasury = this.parent.GameData.Players[playerID].Coins / 2;
			this.parent.GameData.Players[newPlayerID].Coins = (short)newTreasury;
			this.parent.GameData.Players[playerID].Coins -= (short)newTreasury;

			int newMilitaryPower = this.parent.GameData.Players[playerID].MilitaryPower / 2;
			this.parent.GameData.Players[newPlayerID].MilitaryPower = (short)newMilitaryPower;
			this.parent.GameData.Players[playerID].MilitaryPower -= (short)newMilitaryPower;

			this.parent.GameData.Players[newPlayerID].ResearchProgress = this.parent.GameData.Players[playerID].ResearchProgress;
			this.parent.GameData.Players[newPlayerID].DiscoveredTechnologyCount = this.parent.GameData.Players[playerID].DiscoveredTechnologyCount;
			this.parent.GameData.Players[newPlayerID].GovernmentType = this.parent.GameData.Players[playerID].GovernmentType;

			for (int i = 0; i < 5; i++)
			{
				this.parent.GameData.Players[playerID].DiscoveredTechnologyFlags[i] = this.parent.GameData.Players[playerID].DiscoveredTechnologyFlags[i];
			}

			this.parent.GameData.Players[newPlayerID].ContactPlayerCountdown = (short)Math.Max(this.parent.GameData.TurnCount - 8, 0);
			this.parent.GameData.Players[playerID].Diplomacy[newPlayerID] |= DiplomacyFlagsEnum.Contact;
			this.parent.GameData.Players[playerID].Diplomacy[newPlayerID] |= DiplomacyFlagsEnum.Contact | DiplomacyFlagsEnum.Vendetta;

			int oldMainCityGroupSize = groupPopulationSizes[oldMainCityGroupID];
			int otherGroupSizeSum = 0;

			for (int i = 0; i < 16; i++)
			{
				if (groupPopulationSizes[i] != 0)
				{
					if (otherGroupSizeSum <= oldMainCityGroupSize && i != oldMainCityGroupID)
					{
						otherGroupSizeSum += groupPopulationSizes[i];

						groupPopulationSizes[i] = 2;
					}
					else
					{
						if (i != oldMainCityGroupID)
						{
							oldMainCityGroupSize += groupPopulationSizes[i];
						}

						groupPopulationSizes[i] = 1;
					}
				}
			}

			int cityIDNewPlayer = -1;

			if (otherGroupSizeSum * 2 > oldMainCityGroupSize && otherGroupSizeSum <= oldMainCityGroupSize)
			{
				for (int i = 0; i < 128; i++)
				{
					if (this.parent.GameData.Cities[i].StatusFlag != 0xff && this.parent.GameData.Cities[i].PlayerID == playerID)
					{
						// Instruction address 0x0000:0x03ce, size: 5
						int groupID = this.parent.MapManagement.F0_2aea_1942_GetGroupID(
							this.parent.GameData.Cities[i].Position.X, this.parent.GameData.Cities[i].Position.Y);

						if (groupPopulationSizes[groupID] == 2)
						{
							F15_0000_087f_SetCityOwner(i, newPlayerID);
						}
					}
				}
			}
			else
			{
				// this.oCPU.ReadInt16(this.oCPU.DS.UInt16, 0xdc6a) is always zero
				for (int i = 0; (i * 3) < this.parent.GameData.Players[playerID].TotalCitySize;)
				{
					int maximumDistance = 1;
					cityIDNewPlayer = -1;

					for (int j = 0; j < 128; j++)
					{
						if (this.parent.GameData.Cities[j].StatusFlag != 0xff && this.parent.GameData.Cities[j].PlayerID == playerID)
						{
							// Instruction address 0x0000:0x0431, size: 5
							int distance = this.parent.Tools.F0_2dc4_0289_GetShortestDistance(
								this.parent.GameData.Cities[j].Position, oldMainCityX, oldMainCityY);

							if (distance > maximumDistance)
							{
								maximumDistance = distance;
								cityIDNewPlayer = j;
							}
						}
					}

					if (cityIDNewPlayer != -1)
					{
						i += this.parent.GameData.Cities[cityIDNewPlayer].ActualSize;

						F15_0000_087f_SetCityOwner(cityIDNewPlayer, newPlayerID);
					}
					else
					{
						break;
					}
				}
			}

			this.parent.GameData.Players[playerID].TotalCitySize -= this.parent.GameData.Players[newPlayerID].TotalCitySize;

			for (int i = 0; i < 128; i++)
			{
				if (this.parent.GameData.Players[playerID].Units[i].UnitType != UnitTypeEnum.None)
				{
					UnitTypeEnum unitType = this.parent.GameData.Players[playerID].Units[i].UnitType;

					if (this.parent.MapManagement.F0_2aea_1585_GetVisibleTerrainImprovements(
						this.parent.GameData.Players[playerID].Units[i].Position.X, this.parent.GameData.Players[playerID].Units[i].Position.Y).HasFlag(TerrainImprovementFlagsEnum.City))
					{
						// Instruction address 0x0000:0x0780, size: 5
						int cityID = this.parent.Tools.F0_2dc4_00ba_GetCityByLocation(
							this.parent.GameData.Players[playerID].Units[i].Position.X, this.parent.GameData.Players[playerID].Units[i].Position.Y);

						unitType = this.parent.GameData.Players[playerID].Units[i].UnitType;

						if (this.parent.GameData.Cities[this.parent.GameData.Players[playerID].Units[i].HomeCityID].PlayerID == this.parent.GameData.Cities[cityID].PlayerID)
						{
							if (cityIDNewPlayer != -1 && this.parent.GameData.Cities[cityIDNewPlayer].PlayerID == newPlayerID)
							{
								// Instruction address 0x0000:0x04cd, size: 5
								this.parent.UnitManagement.F0_1866_0f10_DeleteUnit(playerID, i);

								// Instruction address 0x0000:0x04f1, size: 5
								this.parent.MapManagement.F0_2aea_1511_ActiveUnitSetFlag8(
									this.parent.GameData.Players[playerID].Units[i].Position.X, this.parent.GameData.Players[playerID].Units[i].Position.Y);

								// Instruction address 0x0000:0x050b, size: 5
								int newUnitID = this.parent.UnitManagement.F0_1866_0cf5_CreateUnit(
									newPlayerID, unitType,
									this.parent.GameData.Players[playerID].Units[i].Position.X, this.parent.GameData.Players[playerID].Units[i].Position.Y);

								this.parent.GameData.Players[newPlayerID].Units[newUnitID].Status = this.parent.GameData.Players[playerID].Units[i].Status;
								this.parent.GameData.Players[newPlayerID].Units[newUnitID].ClearStatusFlags(UnitStatusEnum.Sentry);
							}
						}
						else
						{
							// Instruction address 0x0000:0x07b8, size: 5
							this.parent.UnitManagement.F0_1866_0f10_DeleteUnit(playerID, i);

							// Instruction address 0x0000:0x07cc, size: 5
							this.parent.MapManagement.F0_2aea_1511_ActiveUnitSetFlag8(
								this.parent.GameData.Players[playerID].Units[i].Position.X, this.parent.GameData.Players[playerID].Units[i].Position.Y);

							// Instruction address 0x0000:0x07e7, size: 5
							int newUnitID = this.parent.UnitManagement.F0_1866_0cf5_CreateUnit(
								this.parent.GameData.Cities[cityID].PlayerID, this.parent.GameData.Players[playerID].Units[i].UnitType,
								this.parent.GameData.Players[playerID].Units[i].Position.X, this.parent.GameData.Players[playerID].Units[i].Position.Y);
						}
					}
					else
					{
						int unitHomeCityID = this.parent.GameData.Cities[this.parent.GameData.Players[playerID].Units[i].HomeCityID].PlayerID;
						int nextUnitID = this.parent.GameData.Players[playerID].Units[i].NextUnitID;

						if (unitHomeCityID == newPlayerID)
						{
							// Instruction address 0x0000:0x056c, size: 5
							this.parent.UnitManagement.F0_1866_0f10_DeleteUnit(playerID, i);

							// Instruction address 0x0000:0x0583, size: 5
							this.parent.MapManagement.F0_2aea_138c_SetCityOwner(
								this.parent.GameData.Players[playerID].Units[i].Position.X, this.parent.GameData.Players[playerID].Units[i].Position.Y, newPlayerID);

							// Instruction address 0x0000:0x0597, size: 5
							this.parent.MapManagement.F0_2aea_1511_ActiveUnitSetFlag8(
								this.parent.GameData.Players[playerID].Units[i].Position.X, this.parent.GameData.Players[playerID].Units[i].Position.Y);

							// Instruction address 0x0000:0x05b1, size: 5
							int newUnitID = this.parent.UnitManagement.F0_1866_0cf5_CreateUnit(newPlayerID, unitType,
								this.parent.GameData.Players[playerID].Units[i].Position.X, this.parent.GameData.Players[playerID].Units[i].Position.Y);

							this.parent.GameData.Players[newPlayerID].Units[newUnitID].Status = this.parent.GameData.Players[playerID].Units[i].Status;
							this.parent.GameData.Players[newPlayerID].Units[newUnitID].ClearStatusFlags(UnitStatusEnum.Sentry);
							this.parent.GameData.Players[newPlayerID].Units[newUnitID].HomeCityID = this.parent.GameData.Players[playerID].Units[i].HomeCityID;

							while (nextUnitID != -1 && nextUnitID != i)
							{
								int newNextUnitID = this.parent.GameData.Players[playerID].Units[nextUnitID].NextUnitID;
								unitType = this.parent.GameData.Players[playerID].Units[nextUnitID].UnitType;

								// Instruction address 0x0000:0x061a, size: 5
								this.parent.UnitManagement.F0_1866_0f10_DeleteUnit(playerID, nextUnitID);

								// Instruction address 0x0000:0x063e, size: 5
								this.parent.MapManagement.F0_2aea_138c_SetCityOwner(
									this.parent.GameData.Players[playerID].Units[i].Position.X, this.parent.GameData.Players[playerID].Units[i].Position.Y, newPlayerID);

								if (unitType != UnitTypeEnum.None)
								{
									// Instruction address 0x0000:0x065b, size: 5
									this.parent.MapManagement.F0_2aea_1511_ActiveUnitSetFlag8(
										this.parent.GameData.Players[playerID].Units[i].Position.X, this.parent.GameData.Players[playerID].Units[i].Position.Y);

									// Instruction address 0x0000:0x0678, size: 5
									newUnitID = this.parent.UnitManagement.F0_1866_0cf5_CreateUnit(
										newPlayerID, unitType,
										this.parent.GameData.Players[playerID].Units[i].Position.X, this.parent.GameData.Players[playerID].Units[i].Position.Y);

									this.parent.GameData.Players[newPlayerID].Units[newUnitID].Status = this.parent.GameData.Players[playerID].Units[i].Status;
									this.parent.GameData.Players[newPlayerID].Units[newUnitID].ClearStatusFlags(UnitStatusEnum.Sentry);

									if (this.parent.GameData.Cities[this.parent.GameData.Players[playerID].Units[nextUnitID].HomeCityID].PlayerID != unitHomeCityID)
									{
										this.parent.GameData.Players[newPlayerID].Units[newUnitID].HomeCityID = this.parent.GameData.Players[playerID].Units[i].HomeCityID;
									}
								}

								nextUnitID = newNextUnitID;
							}
						}
						else
						{
							while (nextUnitID != -1)
							{
								if (nextUnitID == i)
									break;

								if (this.parent.GameData.Cities[this.parent.GameData.Players[playerID].Units[nextUnitID].HomeCityID].PlayerID != unitHomeCityID)
								{
									this.parent.GameData.Players[playerID].Units[nextUnitID].HomeCityID = this.parent.GameData.Players[playerID].Units[i].HomeCityID;
								}

								nextUnitID = this.parent.GameData.Players[playerID].Units[nextUnitID].NextUnitID;
							}
						}
					}
				}
			}

			int remotestCityID = F15_0000_08ba_GetRemotestCity(newPlayerID);

			this.parent.GameData.Cities[remotestCityID].AddImprovement(ImprovementEnum.Palace);

			this.parent.GameData.Players[newPlayerID].XStart = (short)this.parent.GameData.Cities[remotestCityID].Position.X;

			if (playerID == this.parent.GameData.HumanPlayerID)
			{
				// Instruction address 0x0000:0x082f, size: 5
				this.parent.MapManagement.F0_2aea_0008_DrawVisibleMap(this.parent.GameData.HumanPlayerID, this.parent.Var_d4cc_MapViewX, this.parent.Var_d75e_MapViewY);
			}
			else
			{
				this.parent.GameData.Cities[oldMainCityID].PlayerID = -1;

				remotestCityID = F15_0000_08ba_GetRemotestCity(playerID);

				this.parent.GameData.Cities[remotestCityID].AddImprovement(ImprovementEnum.Palace);

				this.parent.GameData.Players[playerID].XStart = (short)this.parent.GameData.Cities[remotestCityID].Position.X;

				this.parent.GameData.Cities[oldMainCityID].PlayerID = (short)playerID;
			}

			return true;
		}

		/// <summary>
		/// Sets the new city owner
		/// </summary>
		/// <param name="cityID"></param>
		/// <param name="playerID"></param>
		public void F15_0000_087f_SetCityOwner(int cityID, int playerID)
		{
			//this.oCPU.Log.EnterBlock($"F15_0000_087f_TransferCityToAnotherPlayer({cityID}, {playerID})");

			// function body
			City city = this.parent.GameData.Cities[cityID];

			this.parent.GameData.Players[city.PlayerID].TotalCitySize -= city.ActualSize;

			city.PlayerID = (short)playerID;

			// Instruction address 0x0000:0x08a1, size: 5
			this.parent.MapManagement.F0_2aea_138c_SetCityOwner(city.Position.X, city.Position.Y, playerID);

			this.parent.GameData.Players[playerID].TotalCitySize += city.ActualSize;
		}

		/// <summary>
		/// Gets the city that's most remotest from other cities (for the selected player)
		/// </summary>
		/// <param name="playerID"></param>
		/// <returns></returns>
		public int F15_0000_08ba_GetRemotestCity(int playerID)
		{
			//this.oCPU.Log.EnterBlock($"F15_0000_08ba({playerID})");

			// function body
			int selectedCity = -1;
			int minimumDistance = int.MaxValue;

			for (int i = 0; i < 128; i++)
			{
				if (this.parent.GameData.Cities[i].PlayerID == playerID && this.parent.GameData.Cities[i].StatusFlag != 0xff)
				{
					int distanceSum = 0;

					for (int j = 0; j < 128; j++)
					{
						if (this.parent.GameData.Cities[j].PlayerID == playerID && this.parent.GameData.Cities[j].StatusFlag != 0xff)
						{
							// Instruction address 0x0000:0x0905, size: 5
							distanceSum += this.parent.Tools.F0_2dc4_0289_GetShortestDistance(this.parent.GameData.Cities[i].Position, this.parent.GameData.Cities[j].Position);
						}
					}

					if (distanceSum < minimumDistance)
					{
						minimumDistance = distanceSum;
						selectedCity = i;
					}
				}
			}

			return selectedCity;
		}
	}
}
