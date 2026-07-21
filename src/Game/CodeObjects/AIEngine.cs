using OpenCivOne.Graphics;

namespace OpenCivOne
{
	public class AIEngine
	{
		private OpenCivOneGame parent;

		private int[,] MapUnitRoles = new int[20, 13];
		public int AIPlayerFlags = 0;
		public bool MakeContactWithPlayer = false;

		public AIEngine(OpenCivOneGame parent)
		{
			this.parent = parent;
		}

		/// <summary>
		/// Recalculate statistics and policies for an AI player
		/// </summary>
		/// <param name="playerID"></param>
		public void F0_25fb_0004_RecalculateStatsAndPolicies(int playerID)
		{
			//this.oCPU.Log.EnterBlock($"F0_25fb_0004({playerID})");

			// function body
			int[] unitThresholds = new int[32];

			for (int i = 0; i < 16; i++)
			{
				this.parent.GameData.Players[playerID].Continents[0].CityCount = 0;
				this.parent.GameData.Players[playerID].Continents[0].Defense = 0;
				this.parent.GameData.Players[playerID].Continents[0].Attack = 0;
			}

			for (int i = 0; i < 20; i++)
			{
				for (int j = 0; j < 13; j++)
				{
					MapUnitRoles[i, j] = 0;
				}
			}

			this.parent.GameData.Players[playerID].MilitaryPower = 0;
			this.parent.GameData.Players[playerID].SettlerCount = 0;
			this.parent.GameData.Players[playerID].UnitCount = 0;

			for (int i = 0; i < 128; i++)
			{
				if (this.parent.GameData.Players[playerID].Units[i].UnitType != UnitTypeEnum.None && this.parent.GameData.Players[playerID].Units[i].UnitType != UnitTypeEnum.Nuclear)
				{
					if (this.parent.GameData.Players[playerID].Units[i].UnitType == 0)
					{
						this.parent.GameData.Players[playerID].SettlerCount++;
					}
					else
					{
						this.parent.GameData.Players[playerID].UnitCount++;
					}

					this.parent.GameData.Players[playerID].MilitaryPower += this.parent.GameData.Units[(int)this.parent.GameData.Players[playerID].Units[i].UnitType].AttackStrength;
					this.parent.GameData.Players[playerID].MilitaryPower += this.parent.GameData.Units[(int)this.parent.GameData.Players[playerID].Units[i].UnitType].DefenseStrength;
					this.parent.GameData.Players[playerID].Units[i].Status &= 0xef;

					if (this.parent.UnitManagement.F0_1866_1750_IsUnitOrCityNear(playerID,
						this.parent.GameData.Players[playerID].Units[i].Position.X, this.parent.GameData.Players[playerID].Units[i].Position.Y))
					{
						this.parent.GameData.Players[playerID].Units[i].Status |= 0x10;
					}

					if (this.parent.MapManagement.GetTerrainType(this.parent.GameData.Players[playerID].Units[i].Position.X,
						this.parent.GameData.Players[playerID].Units[i].Position.Y) != TerrainTypeEnum.Water)
					{
						// Instruction address 0x25fb:0x0110, size: 5
						int groupID = this.parent.MapManagement.F0_2aea_1942_GetGroupID(
							this.parent.GameData.Players[playerID].Units[i].Position.X, this.parent.GameData.Players[playerID].Units[i].Position.Y);

						this.parent.GameData.Players[playerID].Continents[groupID].Attack +=
							this.parent.GameData.Units[(int)this.parent.GameData.Players[playerID].Units[i].UnitType].DefenseStrength;
						this.parent.GameData.Players[playerID].Continents[groupID].Defense +=
							this.parent.GameData.Units[(int)this.parent.GameData.Players[playerID].Units[i].UnitType].AttackStrength;
					}
					else
					{
						if (this.parent.GameData.Units[(int)this.parent.GameData.Players[playerID].Units[i].UnitType].MovementType == UnitMovementTypeEnum.Land)
						{
							this.parent.GameData.Players[playerID].Units[i].Status |= 0x10;
						}
					}

					MapUnitRoles[this.parent.GameData.Cities[i].Position.X / 4, this.parent.GameData.Cities[i].Position.Y / 4] |= 
						(0x2 << (int)this.parent.GameData.Units[(int)this.parent.GameData.Players[playerID].Units[i].UnitType].UnitRoleType);
				}
			}

			int unitThreshold = this.parent.Tools.F0_2dc4_007c_CheckValueRange(this.parent.GameData.Players[playerID].UnitCount / 8, 3, 99);
			this.parent.GameData.Players[playerID].TotalCitySize = 0;
			this.parent.GameData.Players[playerID].CityCount = 0;

			for (int i = 0; i < 28; i++)
			{
				this.parent.GameData.Players[playerID].UnitsInProduction[i] = 0;
			}

			for (int i = 0; i < 128; i++)
			{
				if (this.parent.GameData.Cities[i].StatusFlag != 0xff)
				{
					if (playerID != this.parent.GameData.Cities[i].PlayerID)
					{
						if ((this.parent.GameData.TurnCount * this.parent.GameData.DifficultyLevel) > 200 ||
							(this.parent.GameData.MapVisibility[this.parent.GameData.Cities[i].Position.X,
								this.parent.GameData.Cities[i].Position.Y] & (0x1 << playerID)) != 0 ||
							this.parent.GameData.Cities[i].PlayerID != this.parent.GameData.HumanPlayerID)
						{
							if ((this.parent.GameData.Players[playerID].Diplomacy[this.parent.GameData.Cities[i].PlayerID] & (DiplomacyFlagsEnum.Peace | DiplomacyFlagsEnum.Unknown100)) != DiplomacyFlagsEnum.Peace)
							{
								if (((i + this.parent.GameData.TurnCount) & 0x3) != 0)
								{
									if (this.parent.GameData.Cities[i].HasImprovement(ImprovementEnum.CityWalls))
									{
										// Instruction address 0x25fb:0x0264, size: 3
										PlayerAddUnitPolicy(playerID,
											this.parent.GameData.Cities[i].Position.X, this.parent.GameData.Cities[i].Position.Y,
											UnitRoleTypeEnum.LandAttack, 3);
									}
									else
									{
										// Instruction address 0x25fb:0x0264, size: 3
										PlayerAddUnitPolicy(playerID,
											this.parent.GameData.Cities[i].Position.X, this.parent.GameData.Cities[i].Position.Y,
											UnitRoleTypeEnum.LandAttack, 5);
									}
								}
							}
						}
					}
					else
					{
						this.parent.GameData.Players[playerID].CityCount++;

						this.parent.GameData.Players[playerID].Continents[this.parent.MapManagement.F0_2aea_1942_GetGroupID(
							this.parent.GameData.Cities[i].Position.X, this.parent.GameData.Cities[i].Position.Y)].CityCount++;

						this.parent.GameData.Players[playerID].TotalCitySize += this.parent.GameData.Cities[i].ActualSize;

						if (this.parent.GameData.Cities[i].CurrentProductionID >= 0)
						{
							this.parent.GameData.Players[playerID].UnitsInProduction[this.parent.GameData.Cities[i].CurrentProductionID]++;
						}

						MapUnitRoles[this.parent.GameData.Cities[i].Position.X / 4, this.parent.GameData.Cities[i].Position.Y / 4] |= 0x1;

						for (int j = 0; j < 2; j++)
						{
							if (this.parent.GameData.Cities[i].Unknown[j] != -1)
							{
								this.parent.GameData.Players[playerID].MilitaryPower +=
									this.parent.GameData.Units[this.parent.GameData.Cities[i].Unknown[j] & 0x3f].AttackStrength;
								this.parent.GameData.Players[playerID].MilitaryPower +=
									this.parent.GameData.Units[this.parent.GameData.Cities[i].Unknown[j] & 0x3f].DefenseStrength;

								int groupID = this.parent.MapManagement.F0_2aea_1942_GetGroupID(
									this.parent.GameData.Cities[i].Position.X, this.parent.GameData.Cities[i].Position.Y);

								this.parent.GameData.Players[playerID].Continents[groupID].Attack =
									this.parent.GameData.Units[this.parent.GameData.Cities[i].Unknown[j] & 0x3f].DefenseStrength;
								this.parent.GameData.Players[playerID].Continents[groupID].Defense +=
									this.parent.GameData.Units[this.parent.GameData.Cities[i].Unknown[j] & 0x3f].AttackStrength;
							}
						}
					}

					if (this.parent.MapManagement.F0_2aea_1458_GetCellActiveUnitID(
						this.parent.GameData.Cities[i].Position.X, this.parent.GameData.Cities[i].Position.Y) == -1 &&
						this.parent.GameData.Cities[i].Unknown[0] == -1)
					{
						if (this.parent.GameData.Cities[i].PlayerID == playerID)
						{
							// Instruction address 0x25fb:0x03e4, size: 3
							PlayerAddUnitPolicy(playerID,
								this.parent.GameData.Cities[i].Position.X, this.parent.GameData.Cities[i].Position.Y,
								UnitRoleTypeEnum.Defense, 4);
						}
						else
						{
							// Instruction address 0x25fb:0x03e4, size: 3
							PlayerAddUnitPolicy(playerID,
								this.parent.GameData.Cities[i].Position.X, this.parent.GameData.Cities[i].Position.Y,
								UnitRoleTypeEnum.Defense, 2);
						}
					}
				}
			}

			this.parent.GameData.Players[playerID].MapCellCount = 0;

			for (int i = 0; i < 20; i++)
			{
				for (int j = 0; j < 13; j++)
				{
					if (MapUnitRoles[i, j] != 0)
					{
						this.parent.GameData.Players[playerID].MapCellCount++;
					}
				}
			}

			// !!! Should the polar caps have no strategy? They should be unsuitable for settling
			this.parent.GameData.Players[playerID].Continents[0].Strategy = PlayerContinentStrategyEnum.Settle;
			this.parent.GameData.Players[playerID].Continents[15].Strategy = PlayerContinentStrategyEnum.Settle;

			this.AIPlayerFlags |= 0x1 << playerID;
			this.AIPlayerFlags ^= 0x1 << playerID;
			this.MakeContactWithPlayer = false;

			for (int i = 1; i < 8; i++)
			{
				if ((this.parent.GameData.Players[playerID].Diplomacy[i] & (DiplomacyFlagsEnum.Contact | DiplomacyFlagsEnum.Peace)) == DiplomacyFlagsEnum.Contact)
				{
					this.MakeContactWithPlayer = true;
				}
			}

			for (int i = 1; i < 15; i++)
			{
				int totalStrongContinents = 0;
				int totalWeakContinents = 0;
				int diplomacyFlags = 0;
				int playerCityCount = 0;
				int playerAttackStrength = 0;
				PlayerContinentStrategyEnum oldStrategy = this.parent.GameData.Players[playerID].Continents[i].Strategy;

				for (int j = 1; j < 8; j++)
				{
					if (this.parent.GameData.Players[j].Continents[i].Attack != 0)
					{
						if (this.parent.GameData.Players[playerID].Continents[i].Defense / 4 < this.parent.GameData.Players[j].Continents[i].Defense ||
							this.parent.GameData.Players[j].Continents[i].CityCount != 0)
						{
							if ((this.parent.GameData.Players[playerID].Diplomacy[j] & (DiplomacyFlagsEnum.Contact | DiplomacyFlagsEnum.Peace)) == DiplomacyFlagsEnum.Contact || 
								this.parent.GameData.Players[playerID].Diplomacy[j].HasFlag(DiplomacyFlagsEnum.Unknown100))
							{
								if (this.parent.GameData.Players[j].Continents[i].Attack >= this.parent.GameData.Players[playerID].Continents[i].Defense &&
									this.parent.GameData.Players[j].Continents[i].Defense >= this.parent.GameData.Players[playerID].Continents[i].Attack &&
									this.parent.GameData.Players[playerID].Continents[i].CityCount != 0)
								{
									totalWeakContinents++;
								}
								else
								{
									totalStrongContinents++;
								}
							}
						}
					}

					if (this.parent.GameData.Players[j].Continents[i].Attack != 0 && j == this.parent.GameData.HumanPlayerID)
					{
						if (this.parent.GameData.Players[playerID].Diplomacy[j].HasFlag(DiplomacyFlagsEnum.Peace))
						{
							if ((this.parent.GameData.Players[playerID].Continents[i].Defense / 2) + this.parent.GameData.Players[playerID].Continents[i].Attack <
								this.parent.GameData.Players[j].Continents[i].Defense)
							{
								totalWeakContinents++;
							}
						}
					}

					playerAttackStrength += this.parent.GameData.Players[j].Continents[i].Attack;
					playerCityCount += this.parent.GameData.Players[j].Continents[i].CityCount;

					if (this.parent.GameData.Players[j].Continents[i].Attack != 0)
					{
						if (this.parent.GameData.Players[playerID].Diplomacy[j].HasFlag(DiplomacyFlagsEnum.Peace))
						{
							diplomacyFlags |= 0x1;
						}
						else
						{
							diplomacyFlags |= 0x2;
						}
					}
				}

				if (this.parent.GameData.Players[0].Continents[i].CityCount != 0)
				{
					totalStrongContinents++;
				}

				if (((this.parent.GameData.Players[playerID].Continents[i].Attack + playerAttackStrength) * 2 > this.parent.GameData.Continents[i].Size ||
						(this.parent.GameData.Players[playerID].Continents[i].CityCount + playerCityCount) * 64 + 2 > this.parent.GameData.Continents[i].BuildSiteCount) &&
					this.parent.Segment_1ade.F0_1ade_22b5_PlayerHasTechnology(playerID, TechnologyAdvanceEnum.Mapmaking))
				{
					this.parent.GameData.Players[playerID].Continents[i].Strategy = PlayerContinentStrategyEnum.Transport;
				}
				else
				{
					this.parent.GameData.Players[playerID].Continents[i].Strategy = PlayerContinentStrategyEnum.Settle;
				}

				if (totalStrongContinents != 0)
				{
					this.parent.GameData.Players[playerID].Continents[i].Strategy = PlayerContinentStrategyEnum.Attack;
				}

				if (totalWeakContinents != 0)
				{
					this.parent.GameData.Players[playerID].Continents[i].Strategy = PlayerContinentStrategyEnum.Defend;
				}

				if (this.parent.GameData.Players[playerID].Continents[i].Attack == 0 &&
					this.parent.GameData.Players[playerID].Continents[i].CityCount == 0 &&
					diplomacyFlags != 1)
				{
					this.parent.GameData.Players[playerID].Continents[i].Strategy = PlayerContinentStrategyEnum.Attack;
				}

				if (this.parent.GameData.Players[playerID].Continents[i].Strategy != oldStrategy)
				{
					if ((this.parent.GameData.PlayerFlags & (0x1 << playerID)) == 0)
					{
						// Instruction address 0x25fb:0x0797, size: 3
						F0_25fb_3459_PlayerChangeCityProductionForSameContinent(playerID, i);
					}
				}

				if (this.parent.GameData.Players[playerID].Continents[i].Attack != 0 &&
					this.parent.GameData.Players[this.parent.GameData.HumanPlayerID].Continents[i].CityCount > 1)
				{
					this.AIPlayerFlags |= 0x1 << playerID;
				}
			}

			if (playerID != 0 && (this.parent.GameData.PlayerFlags & (0x1 << playerID)) == 0)
			{
				if (playerID != this.parent.GameData.HumanPlayerID &&
					this.parent.GameData.Players[playerID].SettlerCount == 0 && this.parent.GameData.Players[playerID].CityCount == 0)
				{
					this.parent.StartGameMenu.F5_0000_0e6c_TestIfAIPlayerIsDestroyed(playerID, 0);
				}

				if (((playerID + this.parent.GameData.TurnCount) & 0x7) == 0)
				{
					if (this.parent.Var_e3c2 > 0)
					{
						// Instruction address 0x25fb:0x08b9, size: 5
						this.parent.Segment_2517.F0_2517_04a1(playerID, 1);
					}
					else if (this.parent.Segment_1ade.F0_1ade_22b5_PlayerHasTechnology(playerID, TechnologyAdvanceEnum.TheRepublic) && this.parent.Var_db42 >= 0)
					{
						// Instruction address 0x25fb:0x08b9, size: 5
						this.parent.Segment_2517.F0_2517_04a1(playerID, 4);
					}
					else if (this.parent.Segment_1ade.F0_1ade_22b5_PlayerHasTechnology(playerID, TechnologyAdvanceEnum.Communism) && 
						this.parent.GameData.Players[playerID].CityCount > 10)
					{
						// Instruction address 0x25fb:0x08b9, size: 5
						this.parent.Segment_2517.F0_2517_04a1(playerID, 3);
					}
					else if (this.parent.Segment_1ade.F0_1ade_22b5_PlayerHasTechnology(playerID, TechnologyAdvanceEnum.Monarchy))
					{
						// Instruction address 0x25fb:0x08b9, size: 5
						this.parent.Segment_2517.F0_2517_04a1(playerID, 2);
					}
				}

				for (int i = 0; i < 32; i++)
				{
					unitThresholds[i] = unitThreshold;
				}

				for(int i = 0; i < 128; i++)
				{
					if (this.parent.GameData.Players[playerID].Units[i].UnitType != UnitTypeEnum.None &&
						this.parent.GameData.Players[playerID].Units[i].UnitType != UnitTypeEnum.Nuclear &&
						(this.parent.GameData.Players[playerID].Units[i].Status & 0x10) == 0)
					{
						int minimumDistance = int.MaxValue;
						int unitPolicyID = -1;

						int x = this.parent.GameData.Players[playerID].Units[i].Position.X;
						int y = this.parent.GameData.Players[playerID].Units[i].Position.Y;
						int groupID = this.parent.MapManagement.F0_2aea_1942_GetGroupID(x, y);

						if (((i + this.parent.GameData.TurnCount) & 0xf) != 0 ||

							(!this.parent.Segment_1ade.F0_1ade_22b5_PlayerHasTechnology(playerID,
							this.parent.GameData.Units[(int)this.parent.GameData.Players[playerID].Units[i].UnitType].CancelTechnology) &&
							((this.parent.GameData.Players[playerID].GovernmentType < 2 &&
							this.parent.GameData.Players[playerID].Continents[groupID].Strategy != PlayerContinentStrategyEnum.Transport) ||
							this.parent.GameData.Players[playerID].Units[i].UnitType != UnitTypeEnum.Militia)) ||

							(this.parent.UnitManagement.F0_1866_1750_IsUnitOrCityNear(playerID, x, y) ||
							(this.parent.MapManagement.F0_2aea_1585_GetVisibleTerrainImprovements(x, y).HasFlag(TerrainImprovementFlagsEnum.City) &&
							this.parent.GameData.Players[playerID].Units[i].NextUnitID == -1) ||
							this.parent.MapManagement.GetTerrainType(x, y) == TerrainTypeEnum.Water ||
							this.parent.GameData.Players[playerID].Continents[groupID].Strategy == PlayerContinentStrategyEnum.Defend))
						{
							for (int j = 0; j < 32; j++)
							{
								UnitRoleTypeEnum unitRoleType = this.parent.GameData.Players[playerID].UnitPolicies[j].UnitRoleType;
								UnitTypeEnum unitType = this.parent.GameData.Players[playerID].Units[i].UnitType;

								if (unitRoleType == UnitRoleTypeEnum.None ||
									((this.parent.GameData.Units[(int)unitType].UnitRoleType != unitRoleType ||
									this.parent.MapManagement.F0_2aea_1942_GetGroupID(this.parent.GameData.Players[playerID].UnitPolicies[j].Position.X,
										this.parent.GameData.Players[playerID].UnitPolicies[j].Position.Y) != groupID) &&
									(unitType != UnitTypeEnum.Bomber || (unitRoleType != UnitRoleTypeEnum.SeaAttack && this.parent.GameData.Units[(int)unitType].UnitRoleType != unitRoleType)))) continue;

								int distance = this.parent.Tools.F0_2dc4_0289_GetShortestDistance(x, y,
									this.parent.GameData.Players[playerID].UnitPolicies[j].Position.X, this.parent.GameData.Players[playerID].UnitPolicies[j].Position.Y);

								if (this.parent.GameData.Players[playerID].Units[i].UnitType != UnitTypeEnum.Bomber)
								{
									distance = (unitThresholds[j] * distance) / (this.parent.GameData.Players[playerID].UnitPolicies[j].Policy + 1);

									if ((this.parent.GameData.Players[playerID].Units[i].Status & 0xc) == 0)
									{
										if (distance < minimumDistance)
										{
											if (this.parent.GameData.Players[playerID].UnitPolicies[j].Policy * 4 >= distance / unitThreshold)
											{
												minimumDistance = distance;
												unitPolicyID = j;
											}
										}

										continue;
									}

									int policy = this.parent.GameData.Players[playerID].UnitPolicies[j].Policy;

									if (policy < 2 || ((policy * unitThreshold) < distance && unitThresholds[j] != unitThreshold)) continue;
								}
								else
								{
									if ((this.parent.GameData.Players[playerID].UnitPolicies[j].Policy & 0x1) == 0 ||
										((j + (this.parent.GameData.TurnCount / 2)) & 0x1) == 0) continue;

									if (this.parent.GameData.Units[(int)this.parent.GameData.Players[playerID].Units[i].UnitType].MoveCount < distance)
									{
										distance = (distance * 4) / (this.parent.GameData.Players[playerID].UnitPolicies[j].Policy + 1);
									}
									else
									{
										distance = (distance * 2) / (this.parent.GameData.Players[playerID].UnitPolicies[j].Policy + 1);
									}

									if ((this.parent.GameData.Players[playerID].Units[i].Status & 0xc) == 0)
									{
										if (distance < minimumDistance)
										{
											if (this.parent.GameData.Players[playerID].UnitPolicies[j].Policy * 4 >= distance / unitThreshold)
											{
												minimumDistance = distance;
												unitPolicyID = j;
											}
										}

										continue;
									}

									int policy = this.parent.GameData.Players[playerID].UnitPolicies[j].Policy;

									if (policy < 2 || ((policy * unitThreshold) < distance && unitThresholds[j] != unitThreshold)) continue;
								}

								if (!this.parent.MapManagement.F0_2aea_1585_GetVisibleTerrainImprovements(this.parent.GameData.Players[playerID].Units[i].Position.X,
									this.parent.GameData.Players[playerID].Units[i].Position.Y).HasFlag(TerrainImprovementFlagsEnum.City))
								{
									if (distance < minimumDistance)
									{
										if (this.parent.GameData.Players[playerID].UnitPolicies[j].Policy * 4 >= distance / unitThreshold)
										{
											minimumDistance = distance;
											unitPolicyID = j;
										}
									}
								}
							}

							if (unitPolicyID != -1)
							{
								this.parent.GameData.Players[playerID].Units[i].GoToDestination = this.parent.GameData.Players[playerID].UnitPolicies[unitPolicyID].Position;
								this.parent.GameData.Players[playerID].Units[i].Status |= 0x10;
								this.parent.GameData.Players[playerID].Units[i].Status &= 0xf0;

								unitThresholds[unitPolicyID]++;
							}
						}
						else
						{
							// Instruction address 0x25fb:0x09ed, size: 5
							this.parent.UnitManagement.F0_1866_0f10_DeleteUnit(playerID, i);
						}
					}
				}
			}
		}

		/// <summary>
		/// Determines the next move for AI unit
		/// </summary>
		/// <param name="playerID"></param>
		/// <param name="unitID"></param>
		/// <returns></returns>
		public int F0_25fb_0c9d_MoveUnit(int playerID, int unitID)
		{
			//this.oCPU.Log.EnterBlock($"F0_25fb_0c9d({playerID}, {unitID})");

			// function body
			int playerIDBit = (0x1 << playerID);

			if (playerID == 0)
			{
				// Instruction address 0x25fb:0x0cc6, size: 3
				return F0_25fb_362d_MoveBarbarianUnit(playerID, unitID);
			}
			else
			{
				Unit unit = this.parent.GameData.Players[playerID].Units[unitID];

				if (unit.UnitType == UnitTypeEnum.Bomber && unit.SpecialMoves <= 0)
				{
					return 'h';
				}

				if (unit.UnitType != UnitTypeEnum.Fighter || unit.GoToDestination.X == -1)
				{
					if ((this.parent.GameData.Units[(int)unit.UnitType].MoveCount / 2) * 3 >= unit.RemainingMoves)
					{
						return 'h';
					}

					int minimumDistance = int.MaxValue;
					int selectedUnitID = -1;

					for (int i = 0; i < 128; i++)
					{
						if (this.parent.GameData.Players[this.parent.GameData.HumanPlayerID].Units[i].UnitType == UnitTypeEnum.Bomber &&
							(this.parent.GameData.Players[this.parent.GameData.HumanPlayerID].Units[i].VisibleByPlayer & playerIDBit) != 0)
						{
							// Instruction address 0x25fb:0x0d96, size: 5
							int distance = this.parent.Tools.F0_2dc4_0289_GetShortestDistance(
								unit.Position, this.parent.GameData.Players[this.parent.GameData.HumanPlayerID].Units[i].Position);

							if (distance < minimumDistance)
							{
								minimumDistance = distance;
								selectedUnitID = i;
							}
						}
					}

					if (selectedUnitID != -1 && minimumDistance > 1)
					{
						if (this.parent.GameData.Units[(int)unit.UnitType].MoveCount > minimumDistance)
						{
							unit.GoToDestination = this.parent.GameData.Players[this.parent.GameData.HumanPlayerID].Units[selectedUnitID].Position;
						}
						else
						{
							minimumDistance = int.MaxValue;
							int selectedCityID = -1;

							for (int i = 0; i < 128; i++)
							{
								if (this.parent.GameData.Cities[i].StatusFlag != 0xff)
								{
									if (this.parent.GameData.Cities[i].PlayerID == playerID)
									{
										// Instruction address 0x25fb:0x0e55, size: 5
										if (this.parent.GameData.Units[(int)unit.UnitType].MoveCount >=
											this.parent.Tools.F0_2dc4_0289_GetShortestDistance(
												unit.Position, this.parent.GameData.Cities[i].Position))
										{
											// Instruction address 0x25fb:0x0ec1, size: 5
											int distance = this.parent.Tools.F0_2dc4_0289_GetShortestDistance(
												this.parent.GameData.Players[this.parent.GameData.HumanPlayerID].Units[selectedUnitID].Position,
												this.parent.GameData.Cities[i].Position);

											if (distance < minimumDistance)
											{
												minimumDistance = distance;
												selectedCityID = i;
											}
										}
									}
								}
							}

							if (selectedCityID == -1)
							{
								return ' ';
							}
						
							unit.GoToDestination = this.parent.GameData.Cities[selectedCityID].Position;
						}
					}
				}
				
				int unitX = unit.Position.X;
				int unitY = unit.Position.Y;
				UnitTypeEnum unitType = unit.UnitType;
				UnitRoleTypeEnum unitRoleType = this.parent.GameData.Units[(int)unit.UnitType].UnitRoleType;

				bool isUnitNear = this.parent.UnitManagement.F0_1866_1725_IsUnitNear(playerID, unit.Position.X, unit.Position.Y);

				int nearestCityID = this.parent.Tools.F0_2dc4_0102_FindNearestCity(unitX, unitY);
				int nearestCityDistance = (nearestCityID != -1) ? 
					this.parent.Tools.F0_2dc4_0289_GetShortestDistance(unitX, unitY, this.parent.GameData.Cities[nearestCityID].Position) : int.MaxValue;

				int nearestUnitID = this.parent.Tools.F0_2dc4_0177_FindNearestPlayerUnit(playerID, unitID, unitX, unitY);
				int nearestUnitDistance = (nearestUnitID == -1) ? int.MaxValue : this.parent.Tools.F0_2dc4_0289_GetShortestDistance(unitX, unitY, this.parent.GameData.Players[playerID].Units[nearestUnitID].Position);

				TerrainTypeEnum terrainType = this.parent.MapManagement.GetTerrainType(unitX, unitY);
				int groupID = this.parent.MapManagement.F0_2aea_1942_GetGroupID(unitX, unitY);
				PlayerContinentStrategyEnum continentStrategy = this.parent.GameData.Players[playerID].Continents[groupID].Strategy;
				GPoint direction;

				if (unitType == UnitTypeEnum.Nuclear)
				{
					int maximumAttackScore = -1;

					for(int i = 0; i < 128; i++)
					{
						int attackScore = -1;

						if (this.parent.GameData.Cities[i].StatusFlag != 0xff &&
							!this.parent.GameData.Cities[i].HasImprovement(ImprovementEnum.SDIDefense))
						{
							int militaryPower = this.parent.GameData.Players[playerID].MilitaryPower;
							int playerID1 = this.parent.GameData.Cities[i].PlayerID;

							if (militaryPower * 3 < this.parent.GameData.Players[playerID1].MilitaryPower * 2 ||
								this.parent.GameData.Players[playerID].Diplomacy[playerID1].HasFlag(DiplomacyFlagsEnum.Vendetta) ||
								(this.parent.GameData.Players[playerID1].ActiveUnits[(int)UnitTypeEnum.Nuclear] == 0 &&
								militaryPower < this.parent.GameData.Players[playerID1].MilitaryPower * 2))
							{
								if ((this.parent.GameData.Players[playerID].Diplomacy[playerID1] & (DiplomacyFlagsEnum.Peace | DiplomacyFlagsEnum.Unknown80)) == DiplomacyFlagsEnum.Unknown80 &&
									this.parent.GameData.Cities[i].ActualSize > 4)
								{
									attackScore = 0;

									for (int j = 0; j < 9; j++)
									{
										direction = this.parent.MoveDirections[j];

										int newX = this.parent.MapManagement.AdjustXPosition(this.parent.GameData.Cities[i].Position.X + direction.X);
										int newY = this.parent.GameData.Cities[i].Position.Y + direction.Y;

										if (this.parent.MapManagement.F0_2aea_1326_ValidateMapCoordinates(newX, newY))
										{
											// Instruction address 0x25fb:0x1044, size: 5
											int cellOwnerPlayerID = this.parent.MapManagement.F0_2aea_14e0_GetCellUnitPlayerID(newX, newY);

											if (cellOwnerPlayerID != -1)
											{
												if ((this.parent.GameData.Players[playerID].Diplomacy[cellOwnerPlayerID] & (DiplomacyFlagsEnum.Peace | DiplomacyFlagsEnum.Unknown80)) != DiplomacyFlagsEnum.Unknown80)
												{
													if (cellOwnerPlayerID == playerID)
													{
														attackScore -= 2;
													}
													else
													{
														attackScore -= 99;
													}
												}
												else
												{
													attackScore++;
												}
											}
										}
									}

									attackScore += this.parent.GameData.Cities[i].ActualSize / 2;
								}
							}
						}

						if (attackScore > maximumAttackScore)
						{
							bool playerCityNearby = false;

							for (int j = 0; j < 128; j++)
							{
								if (this.parent.GameData.Cities[j].StatusFlag != 0xff && this.parent.GameData.Cities[j].PlayerID == playerID ||
									this.parent.Tools.F0_2dc4_0289_GetShortestDistance(this.parent.GameData.Cities[i].Position,
										this.parent.GameData.Cities[j].Position) < 17)
								{
									playerCityNearby = true;
									break;
								}
							}

							if (playerCityNearby)
							{
								maximumAttackScore = attackScore;
								nearestCityID = i;
							}
						}
					}

					if (maximumAttackScore < 10)
					{
						return ' ';
					}

					if (this.parent.GameData.Players[playerID].ActiveUnits[(int)UnitTypeEnum.Nuclear] < 2)
					{
						return ' ';
					}

					unit.GoToDestination = this.parent.GameData.Cities[nearestCityID].Position;

					for (int i = 1; i < 9; i++)
					{
						direction = this.parent.MoveDirections[i];

						int newX = this.parent.MapManagement.AdjustXPosition(unit.GoToDestination.X + direction.X);
						int newY = unit.GoToDestination.Y + direction.Y;

						if (this.parent.MapManagement.F0_2aea_1326_ValidateMapCoordinates(newX, newY) &&
							this.parent.MapManagement.F0_2aea_14e0_GetCellUnitPlayerID(newX, newY) == -1)
						{
							// Instruction address 0x25fb:0x1253, size: 5
							this.parent.MapManagement.F0_2aea_1412_SetCellActivePlayerID(unitX, unitY, playerID, unitID);

							unit.Position =new(newX, newY);

							// Instruction address 0x25fb:0x1275, size: 5
							this.parent.MapManagement.F0_2aea_13cb_SetCellPlayerID(newX, newY, playerID, unitID);

							if (this.parent.GameData.Cities[nearestCityID].PlayerID == this.parent.GameData.HumanPlayerID)
							{
								this.parent.GameData.Players[playerID].ContactPlayerCountdown = -2;
							}

							return (i ^ 0x4);
						}
					}

					return ' ';
				}

				if (unitType == UnitTypeEnum.Diplomat)
				{
					if (unit.GoToDestination.X == -1)
					{
						if (this.parent.GameData.Cities[nearestCityID].PlayerID != playerID)
						{
							unit.GoToDestination = this.parent.GameData.Cities[nearestCityID].Position;

							return '\x0';
						}

						// Instruction address 0x25fb:0x12ff, size: 5
						this.parent.UnitManagement.F0_1866_0f10_DeleteUnit(playerID, unitID);

						return ' ';
					}

					return '\x0';
				}

				if (unitRoleType == UnitRoleTypeEnum.Settler)
				{
					// Instruction address 0x25fb:0x131b, size: 3
					F0_25fb_3401_PlayerClearContinentPolicies(playerID, UnitRoleTypeEnum.Settler, unitX, unitY, 0);
				}
			
				// Instruction address 0x25fb:0x132f, size: 5
				int ownerPlayerID = this.parent.MapManagement.GetPlayerLandOwnership(unitX, unitY);

				if (ownerPlayerID != 0)
				{
					if (unitRoleType == UnitRoleTypeEnum.None || unitRoleType == UnitRoleTypeEnum.Settler ||
						unitRoleType == UnitRoleTypeEnum.LandAttack || unitRoleType == UnitRoleTypeEnum.Defense)
					{
						int newUnitDirection = 0;

						for (int i = 1; i < 9; i++)
						{
							direction = this.parent.MoveDirections[i];

							int newX = this.parent.MapManagement.AdjustXPosition(unitX + direction.X);
							int newY = unitY + direction.Y;

							if (this.parent.MapManagement.F0_2aea_1326_ValidateMapCoordinates(newX, newY))
							{
								// Instruction address 0x25fb:0x138d, size: 5
								int newOwnerPlayerID = this.parent.MapManagement.GetPlayerLandOwnership(newX, newY);

								if (newOwnerPlayerID > ownerPlayerID)
								{
									ownerPlayerID = newOwnerPlayerID;
									newUnitDirection = i;
								}

								if (this.parent.MapManagement.F0_2aea_1894_CellHasMinorTribeHut(newX, newY, this.parent.MapManagement.GetTerrainType(newX, newY)) &&
									unitRoleType != UnitRoleTypeEnum.Settler)
								{
									return i;
								}
							}
						}

						if (this.parent.GameData.TurnCount == 0 && this.parent.Var_d76a_EarthMap)
						{
							ownerPlayerID = 15;
							newUnitDirection = 0;
						}

						if (ownerPlayerID > 9 && (15 - nearestCityDistance) <= ownerPlayerID && !isUnitNear)
						{
							if (unitRoleType == UnitRoleTypeEnum.Settler)
							{
								if (newUnitDirection != 0)
								{
									return newUnitDirection;
								}

								return 'b';
							}

							// Instruction address 0x25fb:0x142e, size: 3
							AddPlayerContinentPolicy(playerID, unitX, unitY, UnitRoleTypeEnum.Settler, 2);
						}
					}
				}

				// Instruction address 0x25fb:0x148e, size: 5
				int buildScore = this.parent.MapManagement.GetBuildLocationScore(unitX, unitY);

				if (unitRoleType == UnitRoleTypeEnum.Settler &&
					this.parent.GameData.DifficultyLevel != 0 &&
					this.parent.GameData.HumanPlayerID == ((nearestCityID >= 0 && nearestCityID < 128) ? this.parent.GameData.Cities[nearestCityID].PlayerID : -1) &&
					!isUnitNear &&
					nearestCityDistance > 1 &&
					playerID != ((nearestCityID >= 0 && nearestCityID < 128) ? this.parent.GameData.Cities[nearestCityID].PlayerID : -1) &&
					this.parent.GameData.Players[playerID].DiscoveredTechnologyCount < this.parent.GameData.Players[this.parent.GameData.HumanPlayerID].DiscoveredTechnologyCount &&
					buildScore > 8 &&
					(14 - nearestCityDistance) <= buildScore)
				{
					return 'b';
				}

				if (this.parent.MapManagement.F0_2aea_1585_GetVisibleTerrainImprovements(unitX, unitY).HasFlag(TerrainImprovementFlagsEnum.City))
				{
					if (unitRoleType == UnitRoleTypeEnum.Defense)
					{
						if (unit.NextUnitID != -1)
						{
							UnitTypeEnum unitType1 = unit.UnitType;

							unit.UnitType = UnitTypeEnum.Diplomat;

							// Instruction address 0x25fb:0x150c, size: 5
							int selectedUnitID = this.parent.UnitManagement.F0_1866_1089_GetStackStrongestDefenseUnit(playerID, unitID);

							unit.UnitType = unitType1;

							if (selectedUnitID != -1)
							{
								if (this.parent.GameData.Units[(int)this.parent.GameData.Players[playerID].Units[selectedUnitID].UnitType].DefenseStrength <
									this.parent.GameData.Units[(int)unit.UnitType].DefenseStrength)
								{
									return 'f';
								}
							}

							int settleCoefficient = this.parent.UnitManagement.F0_1866_1750_IsUnitOrCityNear(playerID, unitX, unitY) ? 3 : 4;

							if (this.parent.GameData.Cities[nearestCityID].HasImprovement(ImprovementEnum.Palace))
							{
								settleCoefficient--;
							}
							else
							{
								if (continentStrategy == PlayerContinentStrategyEnum.Settle)
								{
									settleCoefficient++;
								}
							}

							// Instruction address 0x25fb:0x158c, size: 5
							int sameUnitRoleCount = this.parent.UnitManagement.F0_1866_1380_GetStackUnitCount(playerID, unitID, UnitRoleTypeEnum.Defense);

							if ((this.parent.GameData.Cities[nearestCityID].ActualSize / settleCoefficient) + 1 > sameUnitRoleCount)
							{
								// Instruction address 0x25fb:0x15bf, size: 3
								PlayerAddUnitPolicy(playerID, unitX, unitY, UnitRoleTypeEnum.Defense, 2);
							}

							if ((this.parent.GameData.Cities[nearestCityID].ActualSize / settleCoefficient) + 1 >= sameUnitRoleCount)
							{
								return 'f';
							}
						
							short tempUnitStatus = unit.Status;

							unit.Status |= 8;

							if (this.parent.UnitManagement.F0_1866_1089_GetStackStrongestDefenseUnit(playerID, unitID) == unitID &&
								(this.parent.GameData.Cities[nearestCityID].ActualSize / settleCoefficient) + 1 > this.parent.UnitManagement.F0_1866_1251_GetStackUnitValueSum(playerID, unitID, UnitStackValueTypeEnum.DefenseUnitCount))
							{
								unit.Status = tempUnitStatus;

								return 'f';
							}

							unit.Status = tempUnitStatus;

							if (((unitID + this.parent.GameData.TurnCount) & 0x7) == 0)
							{
								unit.Status &= 0xf2;
							}
						}
						else
						{
							return 'f';
						}
					}
					else if (nearestUnitDistance > 0 && unitRoleType != UnitRoleTypeEnum.Settler)
					{
						// Instruction address 0x25fb:0x16ad, size: 3
						PlayerAddUnitPolicy(playerID, unitX, unitY, UnitRoleTypeEnum.Defense, 4);

						return ' ';
					}
				}

				if (unitRoleType == UnitRoleTypeEnum.SeaTransport || unitRoleType == UnitRoleTypeEnum.SeaAttack)
				{
					int newUnitDirection = 0;
					int unitRoleBits = 0;
					int unitCount = 0;

					int nextUnitID = unit.NextUnitID;

					while (nextUnitID != -1 && nextUnitID != unitID)
					{
						if ((this.parent.GameData.Players[playerID].Units[nextUnitID].Status & 0x8) == 0)
						{
							unitRoleBits |= (0x1 << (int)this.parent.GameData.Units[(int)this.parent.GameData.Players[playerID].Units[nextUnitID].UnitType].UnitRoleType);

							if (this.parent.GameData.Units[(int)this.parent.GameData.Players[playerID].Units[nextUnitID].UnitType].MovementType == UnitMovementTypeEnum.Land)
							{
								unitCount++;
							}
						}
						else
						{
							if (continentStrategy == PlayerContinentStrategyEnum.Transport &&
								(nearestCityID >= 0 && nearestCityID < 128 && this.parent.GameData.Cities[nearestCityID].ActualSize / 5 < (newUnitDirection++)))
							{
								this.parent.GameData.Players[playerID].Units[nextUnitID].Status &= 0xf7;

								unitCount++;
							}
						}

						nextUnitID = this.parent.GameData.Players[playerID].Units[nextUnitID].NextUnitID;
					}

					if ((((nearestCityDistance != 0) ? 1 : 3) > unitCount || unitRoleBits == 1) && unitRoleType == UnitRoleTypeEnum.SeaTransport)
					{
						int minimumDistance = 999;
						PlayerContinentStrategyEnum currentContinentStrategy = PlayerContinentStrategyEnum.Settle;

						for (int i = 0; i < 128; i++)
						{
							if (this.parent.GameData.Cities[i].StatusFlag != 0xff && this.parent.GameData.Cities[i].PlayerID == playerID &&
								(this.parent.GameData.Cities[i].StatusFlag & 0x2) != 0)
							{
								// Instruction address 0x25fb:0x179d, size: 5
								int distance = this.parent.Tools.F0_2dc4_0289_GetShortestDistance(unitX, unitY,
									this.parent.GameData.Cities[i].Position.X, this.parent.GameData.Cities[i].Position.Y);

								// Instruction address 0x25fb:0x17b4, size: 5
								int cityGroupID = this.parent.MapManagement.F0_2aea_1942_GetGroupID(this.parent.GameData.Cities[i].Position.X,
									this.parent.GameData.Cities[i].Position.Y);

								if (this.parent.GameData.Players[playerID].Continents[cityGroupID].Attack >= 16)
								{
									// Instruction address 0x25fb:0x1814, size: 5
									int activeUnitID = this.parent.MapManagement.F0_2aea_1458_GetCellActiveUnitID(
										this.parent.GameData.Cities[i].Position.X, this.parent.GameData.Cities[i].Position.Y);

									if (activeUnitID != -1 &&
										((distance < 1) ? 1 : 0) < this.parent.UnitManagement.F0_1866_1380_GetStackUnitCount(playerID, activeUnitID, UnitRoleTypeEnum.SeaTransport))
									{
										distance += 16;
									}

									if (this.parent.GameData.Players[playerID].Continents[cityGroupID].Strategy != PlayerContinentStrategyEnum.Transport)
									{
										distance += 16;
									}
									else
									{
										distance -= (this.parent.GameData.Players[playerID].Continents[cityGroupID].Defense / 4);
									}

									if (distance < minimumDistance)
									{
										minimumDistance = distance;

										unit.GoToDestination = this.parent.GameData.Cities[i].Position;

										currentContinentStrategy = this.parent.GameData.Players[playerID].Continents[cityGroupID].Strategy;
									}
								}
							}
						}

						int newContinentPolicy = ((currentContinentStrategy == PlayerContinentStrategyEnum.Transport) ? 4 : 2);

						if (newContinentPolicy * 3 >= minimumDistance)
						{
							if (currentContinentStrategy != PlayerContinentStrategyEnum.Settle)
							{
								// Instruction address 0x25fb:0x191c, size: 3
								PlayerAddUnitPolicy(playerID, unit.GoToDestination.X, unit.GoToDestination.Y, UnitRoleTypeEnum.Settler, (short)newContinentPolicy);
							}

							if (currentContinentStrategy != PlayerContinentStrategyEnum.Defend)
							{
								// Instruction address 0x25fb:0x194f, size: 3
								PlayerAddUnitPolicy(playerID, unit.GoToDestination.X, unit.GoToDestination.Y, UnitRoleTypeEnum.Defense, (short)newContinentPolicy);
							}

							if (currentContinentStrategy != PlayerContinentStrategyEnum.Attack)
							{
								// Instruction address 0x25fb:0x1985, size: 3
								PlayerAddUnitPolicy(playerID, unit.GoToDestination.X, unit.GoToDestination.Y, UnitRoleTypeEnum.LandAttack, (short)newContinentPolicy);
							}
						}
					}
					else
					{
						if (unitRoleType == UnitRoleTypeEnum.SeaTransport)
						{
							for (int i = 1; i < 9; i++)
							{
								direction = this.parent.MoveDirections[i];

								// Instruction address 0x25fb:0x1a7b, size: 5
								int newX = this.parent.MapManagement.AdjustXPosition(unitX + direction.X);
								int newY = unitY + direction.Y;

								if (this.parent.MapManagement.F0_2aea_1326_ValidateMapCoordinates(newX, newY) &&
									this.parent.MapManagement.GetTerrainType(newX, newY) != TerrainTypeEnum.Water)
								{
									// Instruction address 0x25fb:0x1a94, size: 5
									int unitPlayerID = this.parent.MapManagement.F0_2aea_14e0_GetCellUnitPlayerID(newX, newY);

									if ((unitPlayerID == -1 || unitPlayerID == playerID) &&
										(unitPlayerID != playerID ||
										this.parent.UnitManagement.F0_1866_1251_GetStackUnitValueSum(playerID, this.parent.MapManagement.F0_2aea_1458_GetCellActiveUnitID(newX, newY), UnitStackValueTypeEnum.UnitCount) < 2))
									{
										if (!this.parent.GameData.Players[playerID].Diplomacy[this.parent.GameData.HumanPlayerID].HasFlag(DiplomacyFlagsEnum.Peace) ||
											this.parent.MapManagement.F0_2aea_1369_GetCityOwner(newX, newY) != this.parent.GameData.HumanPlayerID ||
											(this.parent.MapManagement.F0_2aea_1585_GetVisibleTerrainImprovements(newX, newY) &
											(TerrainImprovementFlagsEnum.City | TerrainImprovementFlagsEnum.Irrigation | TerrainImprovementFlagsEnum.Mines | TerrainImprovementFlagsEnum.Road)) == TerrainImprovementFlagsEnum.None)
										{
											int newGroupID = this.parent.MapManagement.F0_2aea_1942_GetGroupID(newX, newY);
											bool chooseNewDirection = false;

											if ((this.parent.GameData.Players[playerID].Continents[newGroupID].Attack < 16 &&
												this.parent.GameData.Players[playerID].Continents[newGroupID].Strategy != PlayerContinentStrategyEnum.Transport) ||
												(((0x1 << (int)this.parent.GameData.Players[playerID].Continents[newGroupID].Strategy) & unitRoleBits) & 0x6) != 0)
											{
												chooseNewDirection = true;
											}

											if (unit.GoToDestination.X != -1 && this.parent.MapManagement.F0_2aea_1942_GetGroupID(unit.GoToDestination.X, unit.GoToDestination.Y) == newGroupID)
											{
												chooseNewDirection = true;
											}

											if (chooseNewDirection && this.parent.GameData.Continents[newGroupID].BuildSiteCount > 4 && newY > 1 && newY < 48)
											{
												if (unit.RemainingMoves > 3)
												{
													// Instruction address 0x25fb:0x1c0f, size: 3
													int newDirection = F0_25fb_3521_UnitChooseNewDirection(playerID, unitX, unitY);

													if (newDirection > 0)
													{
														return newDirection;
													}
												}

												if (this.parent.GameData.Players[playerID].Continents[newGroupID].Strategy == PlayerContinentStrategyEnum.Attack)
												{
													// Instruction address 0x25fb:0x19c4, size: 3
													PlayerAddUnitPolicy(playerID, unitX, unitY, UnitRoleTypeEnum.SeaAttack, 5);
												}

												unit.RemainingMoves = 0;
												unit.GoToDestination = OpenCivOneGame.InvalidPosition;

												return 'u';
											}

											if ((this.parent.GameData.TurnCount & 0x3) == 0)
											{
												unit.GoToDestination = OpenCivOneGame.InvalidPosition;
											}
											break;
										}
									}
								}
							}

							if (unit.RemainingMoves < 4)
							{
								int newX = (unit.Position.X + unit.GoToDestination.X) / 2;
								int newY = (unit.Position.Y + unit.GoToDestination.Y) / 2;

								if (unit.GoToDestination.X != -1 && this.parent.MapManagement.GetTerrainType(newX, newY) == TerrainTypeEnum.Water)
								{
									// Instruction address 0x25fb:0x1c31, size: 3
									PlayerAddUnitPolicy(playerID, newX, newY, UnitRoleTypeEnum.SeaAttack, 3);
								}
							}
						}

						if (!isUnitNear || unitRoleType == UnitRoleTypeEnum.SeaTransport)
						{
							if (unit.GoToDestination.X == -1 && ((unitRoleBits & 0x2) != 0 || (unitRoleBits & 0x1) == 0) && unit.UnitType != UnitTypeEnum.Trireme)
							{
								int minimumDistance = 999;

								for (int i = (this.parent.GameData.TurnCount & 0x7); i < 128; i += 8)
								{
									int cityPlayerID = this.parent.GameData.Cities[i].PlayerID;

									if (this.parent.GameData.Cities[i].StatusFlag != 0xff && cityPlayerID != playerID &&
										(this.parent.GameData.Players[playerID].Diplomacy[cityPlayerID] & (DiplomacyFlagsEnum.Peace | DiplomacyFlagsEnum.Unknown100)) != DiplomacyFlagsEnum.Peace)
									{
										if (this.parent.GameData.Players[this.parent.GameData.HumanPlayerID].Ranking < 7 || cityPlayerID == this.parent.GameData.HumanPlayerID)
										{
											// Instruction address 0x25fb:0x1d02, size: 5
											int currentDistance = this.parent.Tools.F0_2dc4_0289_GetShortestDistance(unitX, unitY,
												this.parent.GameData.Cities[i].Position.X, this.parent.GameData.Cities[i].Position.Y);

											if (currentDistance < minimumDistance)
											{
												minimumDistance = currentDistance;
												unit.GoToDestination = this.parent.GameData.Cities[i].Position;
											}
										}
									}
								}
							}

							if (unit.GoToDestination.X == -1 && unit.UnitType != UnitTypeEnum.Trireme)
							{
								int minimumDistance = 999;
								int selectedGroupID = -1;

								if (this.parent.MapManagement.GetTerrainType(unitX, unitY) != TerrainTypeEnum.Water)
								{
									selectedGroupID = groupID;
								}

								for (int i = 0; i < 128; i++)
								{
									if (this.parent.GameData.Cities[i].StatusFlag != 0xff && this.parent.GameData.Cities[i].PlayerID == playerID)
									{
										// Instruction address 0x25fb:0x1dcc, size: 5
										int currentDistance = this.parent.Tools.F0_2dc4_0289_GetShortestDistance(unitX, unitY,
											this.parent.GameData.Cities[i].Position.X, this.parent.GameData.Cities[i].Position.Y);

										if (currentDistance >= 8)
										{
											// Instruction address 0x25fb:0x1df3, size: 5
											int newGroupID = this.parent.MapManagement.F0_2aea_1942_GetGroupID(
												this.parent.GameData.Cities[i].Position.X, this.parent.GameData.Cities[i].Position.Y);

											if (newGroupID != selectedGroupID && this.parent.GameData.Players[playerID].Continents[newGroupID].Strategy != PlayerContinentStrategyEnum.Transport)
											{
												if ((((0x1 << (int)this.parent.GameData.Players[playerID].Continents[newGroupID].Strategy) & unitRoleBits) & 0x7) == 0)
												{
													currentDistance *= 2;
												}

												if (currentDistance < minimumDistance)
												{
													minimumDistance = currentDistance;
													unit.GoToDestination = this.parent.GameData.Cities[i].Position;
												}
											}
										}
									}
								}
							}

							if (unit.GoToDestination.X == -1)
							{
								for (int i = 2; i < 24; i++)
								{
									direction = this.parent.MoveDirections[this.parent.CAPI.RNG.Next(9)];

									// Instruction address 0x25fb:0x1ed9, size: 5
									int newX = this.parent.MapManagement.AdjustXPosition((direction.X * i) + unit.Position.X);
									int newY = (direction.Y * i) + unit.Position.Y;

									if (newY > 2 && newY < 47 && this.parent.MapManagement.GetTerrainType(newX, newY) != TerrainTypeEnum.Water &&
										this.parent.GameData.Players[playerID].Continents[this.parent.MapManagement.F0_2aea_1942_GetGroupID(newX, newY)].CityCount == 0)
									{
										unit.GoToDestination =new(newX, newY);
										break;
									}
								}
							}
						}
					}
				}

				if (this.parent.GameData.Units[(int)unit.UnitType].MoveCount < 2 && nearestCityDistance < 4 &&
					this.parent.GameData.Cities[nearestCityID].PlayerID == this.parent.GameData.HumanPlayerID &&
					!this.parent.GameData.Players[this.parent.GameData.HumanPlayerID].Diplomacy[playerID].HasFlag(DiplomacyFlagsEnum.Peace) &&
					(this.parent.MapManagement.F0_2aea_1585_GetVisibleTerrainImprovements(unitX, unitY) & (TerrainImprovementFlagsEnum.Irrigation | TerrainImprovementFlagsEnum.Mines)) != TerrainImprovementFlagsEnum.None &&
					this.parent.GameData.Cities[nearestCityID].PlayerID != playerID)
				{
					return 'P';
				}

				if (unitRoleType == UnitRoleTypeEnum.Settler)
				{
					if (terrainType != TerrainTypeEnum.Water)
					{
						if (nearestUnitDistance > 1)
						{
							// Instruction address 0x25fb:0x1fd9, size: 3
							PlayerAddUnitPolicy(playerID, unitX, unitY, UnitRoleTypeEnum.Defense, 2);
						}

						if ((!this.parent.Segment_1ade.F0_1ade_22b5_PlayerHasTechnology(playerID, TechnologyAdvanceEnum.Monarchy) ?
							this.parent.GameData.TerrainModifications[(int)terrainType].AICanImproveBeforeMonarchy :
							this.parent.GameData.TerrainModifications[(int)terrainType].AICanImproveAfterMonarchy) ||
							continentStrategy == PlayerContinentStrategyEnum.Transport)
						{
							if (nearestCityDistance > 0 && nearestCityDistance <= 2 && this.parent.GameData.Cities[nearestCityID].PlayerID == playerID)
							{
								if (this.parent.GameData.Cities[nearestCityID].ActualSize >= 3 || terrainType != TerrainTypeEnum.Hills ||
									this.parent.MapManagement.F0_2aea_1836_CellHasSpecialResource(unitX, unitY))
								{
									if ((this.parent.GameData.DebugFlags & 0x2) != 0)
									{
										// Instruction address 0x25fb:0x2070, size: 5
										TerrainImprovementFlagsEnum preferredImprovement = this.parent.PlayerTurn.F0_1403_3f68_GetPreferredImprovement(unitX, unitY);

										if (preferredImprovement == TerrainImprovementFlagsEnum.Irrigation)
										{
											unit.GoToDestination = OpenCivOneGame.InvalidPosition;

											return 'i';
										}

										if (preferredImprovement == TerrainImprovementFlagsEnum.Mines)
										{
											unit.GoToDestination = OpenCivOneGame.InvalidPosition;

											return 'm';
										}

										// Instruction address 0x25fb:0x2088, size: 5
										TerrainImprovementFlagsEnum terrainImprovements = this.parent.MapManagement.F0_2aea_1585_GetVisibleTerrainImprovements(unitX, unitY);

										if ((terrainImprovements & (TerrainImprovementFlagsEnum.Irrigation | TerrainImprovementFlagsEnum.Mines)) != TerrainImprovementFlagsEnum.None ||
											(terrainImprovements & TerrainImprovementFlagsEnum.Road) != TerrainImprovementFlagsEnum.Road &&
											(terrainType == TerrainTypeEnum.Invalid || terrainType == TerrainTypeEnum.Desert ||
												terrainType == TerrainTypeEnum.Plains || terrainType == TerrainTypeEnum.Grassland))
										{
											unit.GoToDestination = OpenCivOneGame.InvalidPosition;

											return 'r';
										}

										// !!! F0_1d12_6abc_GetCityResourceCount was using uninitialized playerID and cityID from CityWorker class, it now makes sense to use referenced player and unit ID
										if ((terrainImprovements & (TerrainImprovementFlagsEnum.Road | TerrainImprovementFlagsEnum.RailRoad)) != (TerrainImprovementFlagsEnum.Road | TerrainImprovementFlagsEnum.RailRoad) &&
											this.parent.Segment_1ade.F0_1ade_22b5_PlayerHasTechnology(playerID, TechnologyAdvanceEnum.Railroad) &&
											(((terrainImprovements & TerrainImprovementFlagsEnum.Road) == TerrainImprovementFlagsEnum.Road) ? 1 : 2) <=
											this.parent.CityWorker.F0_1d12_6abc_GetCityResourceCount(playerID, unit.HomeCityID, unitX, unitY, CityResourceTypeEnum.Production))
										{
											unit.GoToDestination = OpenCivOneGame.InvalidPosition;

											return 'r';
										}
									}
								}
							}
						}

						if ((this.parent.MapManagement.F0_2aea_1585_GetVisibleTerrainImprovements(unitX, unitY) & TerrainImprovementFlagsEnum.Pollution) == TerrainImprovementFlagsEnum.Pollution)
						{
							return 'p';
						}

						if (unit.GoToDestination.X != -1 &&
							((this.parent.GameData.Nations[this.parent.GameData.Players[playerID].NationalityID].Policy + 1) * this.parent.GameData.Continents[groupID].BuildSiteCount) >
							(this.parent.GameData.Players[playerID].Continents[groupID].CityCount * 16))
						{
							return '\x0';
						}

						if (nearestCityDistance <= 1 && this.parent.GameData.Cities[nearestCityID].PlayerID == playerID && (this.parent.GameData.DebugFlags & 0x2) != 0)
						{
							for (int i = 1; i < 9; i++)
							{
								direction = this.parent.MoveDirections[((i + this.parent.GameData.TurnCount) & 0x7) + 1];

								int newX = this.parent.MapManagement.AdjustXPosition(direction.X + unitX);
								int newY = direction.Y + unitY;

								if (this.parent.MapManagement.F0_2aea_1326_ValidateMapCoordinates(newX, newY))
								{
									int activeUnitID = this.parent.MapManagement.F0_2aea_1458_GetCellActiveUnitID(newX, newY);

									if (activeUnitID == -1 || (playerID == this.parent.Var_d7f0 && this.parent.UnitManagement.F0_1866_1331_CountUnitTypesInStack(playerID, activeUnitID, UnitTypeEnum.Settler) == 0))
									{
										if ((this.parent.Segment_1ade.F0_1ade_22b5_PlayerHasTechnology(playerID, TechnologyAdvanceEnum.Monarchy) ?
											this.parent.GameData.TerrainModifications[(int)this.parent.MapManagement.GetTerrainType(newX, newY)].AICanImproveAfterMonarchy :
												this.parent.GameData.TerrainModifications[(int)this.parent.MapManagement.GetTerrainType(newX, newY)].AICanImproveBeforeMonarchy) &&
											this.parent.PlayerTurn.F0_1403_3f68_GetPreferredImprovement(newX, newY) != 0)
										{
											if (this.parent.GameData.Cities[nearestCityID].ActualSize >= 3 ||
												this.parent.MapManagement.GetTerrainType(newX, newY) != TerrainTypeEnum.Hills ||
												this.parent.MapManagement.F0_2aea_1836_CellHasSpecialResource(newX, newY))
											{
												if (this.parent.Tools.F0_2dc4_0289_GetShortestDistance(newX - this.parent.GameData.Cities[nearestCityID].Position.X,
													newY - this.parent.GameData.Cities[nearestCityID].Position.Y) < 3)
												{
													unit.GoToDestination = new(newX, newY);

													return '\x0';
												}
											}
										}
									}
								}
							}
						}

						if ((this.parent.MapManagement.F0_2aea_1585_GetVisibleTerrainImprovements(unitX, unitY) & TerrainImprovementFlagsEnum.Road) != TerrainImprovementFlagsEnum.Road &&
							terrainType != TerrainTypeEnum.River || this.parent.Segment_1ade.F0_1ade_22b5_PlayerHasTechnology(playerID, TechnologyAdvanceEnum.BridgeBuilding))
						{
							if (((nearestCityID < 128) ? this.parent.GameData.Cities[nearestCityID].PlayerID : -1) == playerID)
							{
								if (nearestCityDistance > 2 || (terrainType != TerrainTypeEnum.Plains && terrainType != TerrainTypeEnum.Grassland))
								{
									bool neighbourCellHasRoad = false;
									int roadCellCount = 0;
									int roadCellBits = 0;

									for (int i = 1; i < 9; i++)
									{
										direction = this.parent.MoveDirections[i];

										int newX = this.parent.MapManagement.AdjustXPosition(unitX + direction.X);
										int newY = unitY + direction.Y;

										if (this.parent.MapManagement.F0_2aea_1326_ValidateMapCoordinates(newX, newY) &&
											(this.parent.MapManagement.F0_2aea_1585_GetVisibleTerrainImprovements(newX, newY) & TerrainImprovementFlagsEnum.Road) == TerrainImprovementFlagsEnum.Road)
										{
											roadCellBits |= (0x1 << (i - 1));

											roadCellCount++;

											if ((this.parent.MapManagement.F0_2aea_1585_GetVisibleTerrainImprovements(newX + direction.X, newY + direction.Y) & TerrainImprovementFlagsEnum.Road) == TerrainImprovementFlagsEnum.Road)
											{
												neighbourCellHasRoad = true;
											}
										}
									}

									roadCellBits &= roadCellBits >> 4;

									if (roadCellCount < 4)
									{
										if (roadCellBits == 0)
										{
											if (nearestCityDistance == 1 && (terrainType == TerrainTypeEnum.Invalid || terrainType == TerrainTypeEnum.Desert ||
												terrainType == TerrainTypeEnum.Plains || terrainType == TerrainTypeEnum.Grassland))
											{
												return 'r';
											}
										}
										else
										{
											return 'r';
										}
									}

									if (roadCellCount == 1 && neighbourCellHasRoad && this.parent.GameData.Terrains[(int)terrainType].MovementCost == 1)
									{
										return 'r';
									}
								}
								else
								{
									return 'r';
								}
							}
						}
					}
					else
					{
						if (unit.GoToDestination.X != -1)
						{
							return '\x0';
						}

						// Instruction address 0x25fb:0x243c, size: 5
						direction = this.parent.MoveDirections[this.parent.CAPI.RNG.Next(20) + 1];

						int newX = this.parent.MapManagement.AdjustXPosition(unit.Position.X + direction.X);
						int newY = unit.Position.Y + direction.Y;

						if (this.parent.MapManagement.F0_2aea_1326_ValidateMapCoordinates(newX, newY))
						{
							newX = (unit.Position.X / 4) + direction.X;
							newY = (unit.Position.Y / 4) + direction.Y;

							// !!! Illegal memory access this.oCPU.ReadUInt16(this.oCPU.SS.Word, (ushort)(this.oCPU.BP.Word - 0x26)) == -1
							if ((MapUnitRoles[newX, newY] & 0x3) == 0)
							{
								unit.GoToDestination = new(unit.Position.X + (direction.X * 4), unit.Position.Y + (direction.Y * 4));

								if (this.parent.MapManagement.F0_2aea_1326_ValidateMapCoordinates(unit.GoToDestination.X, unit.GoToDestination.Y) &&
									this.parent.GameData.Terrains[(int)this.parent.MapManagement.GetTerrainType(unit.GoToDestination.X, unit.GoToDestination.Y)].Production != 0 &&
									this.parent.MapManagement.F0_2aea_1942_GetGroupID(unit.GoToDestination.X, unit.GoToDestination.Y) == groupID)
								{
									return '\x0';
								}

								unit.GoToDestination = OpenCivOneGame.InvalidPosition;
							}
						}
					}
				}

				if (unit.GoToDestination.X != -1)
				{
					if (unitRoleType != UnitRoleTypeEnum.SeaAttack || !this.parent.UnitManagement.F0_1866_18d0_IsEnemyUnitNear(playerID, unitX, unitY))
					{
						return '\x0';
					}
				}

				if (unitRoleType == UnitRoleTypeEnum.Defense && terrainType != TerrainTypeEnum.Water)
				{
					if (nearestUnitDistance > 1 && nearestCityDistance < 4 && this.parent.GameData.Cities[nearestCityID].PlayerID == playerID)
					{
						return 'f';
					}
				
					if (isUnitNear && nearestUnitDistance != 0)
					{
						return 'f';
					}
				}

				if (unitRoleType == UnitRoleTypeEnum.Settler &&
					this.parent.GameData.Players[playerID].Continents[groupID].CityCount > this.parent.GameData.Continents[groupID].BuildSiteCount / 8)
				{
					if (nearestCityDistance == 0 && this.parent.GameData.Cities[nearestCityID].ActualSize < 10)
					{
						return 'b';
					}
				
					if (this.parent.GameData.Cities[nearestCityID].PlayerID == playerID && this.parent.GameData.Cities[nearestCityID].ActualSize < 10)
					{
						unit.GoToDestination = this.parent.GameData.Cities[nearestCityID].Position;

						return '\x0';
					}
				}

				int maximumScore = -999;
				int newUnitDirection1 = 0;
				bool unitOrCityNotNear = false;

				if (this.parent.UnitManagement.F0_1866_1750_IsUnitOrCityNear(playerID, unitX, unitY))
				{
					unit.GoToNextDirection = -1;
				}
				else
				{
					unitOrCityNotNear = true;
				}

				if (unitType == UnitTypeEnum.Militia && continentStrategy == PlayerContinentStrategyEnum.Attack)
				{
					unitRoleType = UnitRoleTypeEnum.LandAttack;
				}

				bool moveToOtherPlayerCity = false;

				for (int i = 1; i < 9; i++)
				{
					direction = this.parent.MoveDirections[i];

					int newX = this.parent.MapManagement.AdjustXPosition(unitX + direction.X);
					int newY = unitY + direction.Y;

					if (this.parent.MapManagement.F0_2aea_1326_ValidateMapCoordinates(newX, newY))
					{
						int newCityPlayerID = this.parent.MapManagement.F0_2aea_1369_GetCityOwner(newX, newY);
						TerrainTypeEnum newTerrainType = this.parent.MapManagement.GetTerrainType(newX, newY);

						if (newTerrainType != TerrainTypeEnum.Water || this.parent.GameData.Units[(int)unit.UnitType].MovementType != UnitMovementTypeEnum.Land)
						{
							// Instruction address 0x25fb:0x2705, size: 5
							int selectedActiveUnitID = this.parent.MapManagement.F0_2aea_1458_GetCellActiveUnitID(newX, newY);

							if (selectedActiveUnitID != -1 && this.parent.GameData.Players[this.parent.Var_d7f0].Units[selectedActiveUnitID].UnitType == UnitTypeEnum.Diplomat)
							{
								int activeUnitID = selectedActiveUnitID;

								do
								{
									activeUnitID = this.parent.GameData.Players[this.parent.Var_d7f0].Units[activeUnitID].NextUnitID;
								}
								while ((activeUnitID != -1 && activeUnitID != selectedActiveUnitID) && this.parent.GameData.Players[this.parent.Var_d7f0].Units[activeUnitID].UnitType == UnitTypeEnum.Diplomat);

								if (activeUnitID != -1)
								{
									selectedActiveUnitID = activeUnitID;
								}
							}

							if ((!isUnitNear || this.parent.GameData.Units[(int)unit.UnitType].MovementType != UnitMovementTypeEnum.Land ||
								selectedActiveUnitID != -1 || !this.parent.UnitManagement.F0_1866_1725_IsUnitNear(playerID, newX, newY) ||
								unit.UnitType == UnitTypeEnum.Diplomat || unit.UnitType == UnitTypeEnum.Caravan || terrainType == TerrainTypeEnum.Water) &&
								(terrainType != TerrainTypeEnum.Water || this.parent.GameData.Units[(int)unit.UnitType].MovementType != UnitMovementTypeEnum.Land ||
								selectedActiveUnitID == -1 || newCityPlayerID == playerID))
							{
								bool flag = false;

								if (this.parent.GameData.Units[(int)unit.UnitType].MovementType != UnitMovementTypeEnum.Water)
								{
									if (selectedActiveUnitID == -1 || newCityPlayerID != playerID || this.parent.UnitManagement.F0_1866_1251_GetStackUnitValueSum(playerID, selectedActiveUnitID, UnitStackValueTypeEnum.UnitCount) < 2)
									{
										flag = true;
									}
								}
								else if (newTerrainType != TerrainTypeEnum.Water)
								{
									if (selectedActiveUnitID != -1 && newCityPlayerID != playerID && unitType != UnitTypeEnum.Submarine && terrainType == TerrainTypeEnum.Water)
									{
										flag = true;
									}
								}
								else if (selectedActiveUnitID == -1 || newCityPlayerID != playerID || this.parent.UnitManagement.F0_1866_1251_GetStackUnitValueSum(playerID, selectedActiveUnitID, UnitStackValueTypeEnum.UnitCount) < 2)
								{
									flag = true;
								}

								if (flag)
								{
									int actionScore = 0;

									if (unitRoleType != UnitRoleTypeEnum.Settler)
									{
										if (unit.VisibleByPlayer != 0 || terrainType == TerrainTypeEnum.Water)
										{
											actionScore = this.parent.CAPI.RNG.Next(5);

											if (selectedActiveUnitID != -1 && newCityPlayerID == playerID)
											{
												if (unitRoleType == UnitRoleTypeEnum.LandAttack)
												{
													actionScore += (this.parent.UnitManagement.F0_1866_1251_GetStackUnitValueSum(playerID, selectedActiveUnitID, UnitStackValueTypeEnum.DefenseStrength) * 4) / (this.parent.UnitManagement.F0_1866_1251_GetStackUnitValueSum(playerID, selectedActiveUnitID, UnitStackValueTypeEnum.UnitCount) + 1);
												}

												if (unitRoleType == UnitRoleTypeEnum.Settler)
												{
													actionScore += (this.parent.GameData.Terrains[(int)newTerrainType].DefenseBonus + this.parent.UnitManagement.F0_1866_1251_GetStackUnitValueSum(playerID, selectedActiveUnitID, UnitStackValueTypeEnum.DefenseStrength)) * 2;
												}

												if (unitRoleType == UnitRoleTypeEnum.Defense)
												{
													actionScore += (this.parent.UnitManagement.F0_1866_1251_GetStackUnitValueSum(playerID, selectedActiveUnitID, UnitStackValueTypeEnum.AttackStrength) * 2) / (this.parent.UnitManagement.F0_1866_1251_GetStackUnitValueSum(playerID, selectedActiveUnitID, UnitStackValueTypeEnum.DefenseStrength) + 1);
												}
											}
											else
											{
												actionScore += this.parent.GameData.Terrains[(int)newTerrainType].DefenseBonus * 4;
											}
										}
										else
										{
											if (unitRoleType != UnitRoleTypeEnum.LandAttack)
											{
												actionScore = this.parent.CAPI.RNG.Next(3) - this.parent.GameData.Terrains[(int)newTerrainType].DefenseBonus;
											}
											else
											{
												actionScore = this.parent.CAPI.RNG.Next(3) - (this.parent.GameData.Terrains[(int)newTerrainType].MovementCost * 2);
											}
										}

										if (this.parent.GameData.Units[(int)unit.UnitType].MovementType == UnitMovementTypeEnum.Air)
										{
											actionScore = this.parent.CAPI.RNG.Next(3);
										}
									}
									else
									{
										actionScore = 0;

										if (terrainType == TerrainTypeEnum.Water && this.parent.UnitManagement.F0_1866_1750_IsUnitOrCityNear(playerID, newX, newY))
											continue;
									}

									if (unit.GoToNextDirection != -1)
									{
										int local_a = Math.Abs(unit.GoToNextDirection - i);

										if (local_a > 4)
										{
											local_a = 8 - local_a;
										}

										actionScore -= (local_a * local_a) * 2;
									}

									bool otherPlayerCityIsNear = false;
									bool checkMovementFlag = false;

									if (selectedActiveUnitID == -1)
									{
										if ((this.parent.MapManagement.F0_2aea_1585_GetVisibleTerrainImprovements(newX, newY) & TerrainImprovementFlagsEnum.City) != TerrainImprovementFlagsEnum.None &&
											newCityPlayerID != playerID)
										{
											actionScore = 999;
										}

										if (this.parent.MapManagement.F0_2aea_1894_CellHasMinorTribeHut(newX, newY, newTerrainType))
										{
											actionScore += 20;
										}
									}
									else if (newCityPlayerID != playerID)
									{
										otherPlayerCityIsNear = true;

										if (unitRoleType != UnitRoleTypeEnum.Settler)
										{
											if (this.parent.GameData.Players[playerID].Diplomacy[newCityPlayerID].HasFlag(DiplomacyFlagsEnum.Peace))
											{
												if (unitRoleType == UnitRoleTypeEnum.LandAttack)
												{
													if (continentStrategy == PlayerContinentStrategyEnum.Attack)
													{
														if (this.parent.GameData.Units[(int)unit.UnitType].MovementType == UnitMovementTypeEnum.Land &&
															(this.parent.GameData.Players[newCityPlayerID].Units[selectedActiveUnitID].Status & 0x8) != 0 &&
															(this.parent.GameData.Players[playerID].Diplomacy[this.parent.GameData.HumanPlayerID] & (DiplomacyFlagsEnum.Contact | DiplomacyFlagsEnum.Peace)) == DiplomacyFlagsEnum.Contact &&
															(this.parent.GameData.Players[newCityPlayerID].Diplomacy[this.parent.GameData.HumanPlayerID] & (DiplomacyFlagsEnum.Contact | DiplomacyFlagsEnum.Peace)) != DiplomacyFlagsEnum.Contact &&
															this.parent.GameData.Players[this.parent.GameData.HumanPlayerID].Continents[groupID].CityCount != 0)
														{
															if (this.parent.UnitManagement.F0_1866_1251_GetStackUnitValueSum(newCityPlayerID, selectedActiveUnitID, UnitStackValueTypeEnum.UnitCount) < 2)
															{
																if (this.parent.CAPI.RNG.Next(8) == 0 || this.parent.GameData.Cities[nearestCityID].PlayerID == this.parent.GameData.HumanPlayerID)
																{
																	if ((this.parent.MapManagement.F0_2aea_1585_GetVisibleTerrainImprovements(newX, newY) & TerrainImprovementFlagsEnum.City) == TerrainImprovementFlagsEnum.None)
																	{
																		this.parent.Overlay_22.F22_0000_0639_DiplomatInteraction(newCityPlayerID, selectedActiveUnitID, playerID);
																	}
																}
															}
														}
													}
												}
											}
											else
											{
												// Instruction address 0x25fb:0x2a72, size: 5
												selectedActiveUnitID = this.parent.UnitManagement.F0_1866_1122(newCityPlayerID, selectedActiveUnitID);

												if (this.parent.GameData.Units[(int)this.parent.GameData.Players[newCityPlayerID].Units[selectedActiveUnitID].UnitType].MovementType != UnitMovementTypeEnum.Air ||
													unit.UnitType == UnitTypeEnum.Fighter ||
													(this.parent.MapManagement.F0_2aea_1585_GetVisibleTerrainImprovements(newX, newY) & TerrainImprovementFlagsEnum.City) == TerrainImprovementFlagsEnum.City)
												{
													// Instruction address 0x25fb:0x2ad8, size: 5
													int local_5a = this.parent.Segment_29f3.F0_29f3_000e_AttackUnit(playerID, unitID, newCityPlayerID, selectedActiveUnitID, false);

													// Instruction address 0x25fb:0x2aec, size: 5
													local_5a = (this.parent.UnitManagement.F0_1866_1251_GetStackUnitValueSum(newCityPlayerID, selectedActiveUnitID, UnitStackValueTypeEnum.Cost) + 1) * local_5a;

													local_5a /= this.parent.GameData.Units[(int)unit.UnitType].Cost;

													if ((this.parent.MapManagement.F0_2aea_1585_GetVisibleTerrainImprovements(newX, newY) & TerrainImprovementFlagsEnum.City) == TerrainImprovementFlagsEnum.City)
													{
														local_5a *= 3;
													}

													if (unitRoleType == UnitRoleTypeEnum.LandAttack && continentStrategy == PlayerContinentStrategyEnum.Attack)
													{
														local_5a = local_5a * 3;
													}

													if (unitRoleType == UnitRoleTypeEnum.LandAttack &&
														(this.parent.GameData.Units[(int)unit.UnitType].AttackStrength * 2) < this.parent.UnitManagement.F0_1866_1251_GetStackUnitValueSum(playerID, unitID, UnitStackValueTypeEnum.AttackStrength))
													{
														local_5a *= 2;
													}

													if (((playerID != 0) ? 6 : 12) > local_5a)
													{
														actionScore -= 999;

														if (unitRoleType == UnitRoleTypeEnum.LandAttack)
														{
															if (continentStrategy == PlayerContinentStrategyEnum.Attack)
															{
																if (this.parent.GameData.Units[(int)unit.UnitType].MovementType == UnitMovementTypeEnum.Land &&
																	(this.parent.GameData.Players[newCityPlayerID].Units[selectedActiveUnitID].Status & 0x28) != 0 &&
																	this.parent.GameData.Terrains[(int)newTerrainType].DefenseBonus >= 4)
																{
																	if (this.parent.UnitManagement.F0_1866_1251_GetStackUnitValueSum(newCityPlayerID, selectedActiveUnitID, UnitStackValueTypeEnum.UnitCount) < 2)
																	{
																		if (this.parent.CAPI.RNG.Next(4) == 0)
																		{
																			if ((this.parent.MapManagement.F0_2aea_1585_GetVisibleTerrainImprovements(newX, newY) & TerrainImprovementFlagsEnum.City) == TerrainImprovementFlagsEnum.None)
																			{
																				this.parent.Overlay_22.F22_0000_0639_DiplomatInteraction(newCityPlayerID, selectedActiveUnitID, playerID);
																			}
																		}
																	}
																}
															}
														}
													}
													else
													{
														actionScore += local_5a * 4;
													}
													checkMovementFlag = true;
												}
											}
										}
									}
									else
									{
										actionScore -= this.parent.GameData.Units[(int)unit.UnitType].DefenseStrength;
										checkMovementFlag = true;
									}

									if (!checkMovementFlag)
										continue;

									if (unitOrCityNotNear)
									{
										direction = this.parent.MoveDirections[i];

										newX = unitX + (direction.X * 4);
										newY = unitY + (direction.Y * 4);

										if (this.parent.MapManagement.F0_2aea_1326_ValidateMapCoordinates(newX, newY) &&
											MapUnitRoles[newX / 4, newY / 4] == 0 &&
											this.parent.MapManagement.GetTerrainType(newX, newY) != TerrainTypeEnum.Water)
										{
											actionScore += 8;
										}

										for (int j = 1; j < 9; j++)
										{
											direction = this.parent.MoveDirections[j];

											int local_e = this.parent.MapManagement.AdjustXPosition(newX + direction.X);
											int local_16 = direction.Y + newY;

											if (this.parent.MapManagement.F0_2aea_1326_ValidateMapCoordinates(local_e, local_16))
											{
												if ((this.parent.GameData.MapVisibility[local_e, local_16] & playerIDBit) == 0)
												{
													if (this.parent.MapManagement.GetTerrainType(local_e, local_16) != TerrainTypeEnum.Water ||
														this.parent.GameData.Units[(int)unit.UnitType].MovementType == UnitMovementTypeEnum.Water)
													{
														actionScore += 2;
													}
												}

												if (this.parent.MapManagement.F0_2aea_1458_GetCellActiveUnitID(local_e, local_16) != -1)
												{
													actionScore -= 2;
												}

												if (unitRoleType == UnitRoleTypeEnum.Settler)
												{
													actionScore += this.parent.GameData.Terrains[(int)this.parent.MapManagement.GetTerrainType(local_e, local_16)].Food;
												}
											}
										}
									}

									if (otherPlayerCityIsNear)
									{
										if (unit.RemainingMoves < 3)
										{
											actionScore = (this.parent.UnitManagement.F0_1866_1251_GetStackUnitValueSum(playerID, unitID, UnitStackValueTypeEnum.DefenseStrength) * actionScore) /
												this.parent.Tools.F0_2dc4_007c_CheckValueRange(this.parent.UnitManagement.F0_1866_1251_GetStackUnitValueSum(playerID, unitID, UnitStackValueTypeEnum.AttackStrength), 1, 99);
										}
									}

									if (actionScore > maximumScore)
									{
										maximumScore = actionScore;
										newUnitDirection1 = i;
										moveToOtherPlayerCity = otherPlayerCityIsNear;
									}
								}
							}
						}
					}
				}

				if (moveToOtherPlayerCity)
				{
					if (unit.RemainingMoves < 3)
					{
						newUnitDirection1 = 0;
					}
				}

				unit.GoToNextDirection = (short)newUnitDirection1;

				return newUnitDirection1;
			}
		}

		/// <summary>
		/// For a selected player clear all unit policies and reassign them according to continent policy
		/// </summary>
		/// <param name="playerID"></param>
		public void F0_25fb_2fd7_ClearUnitPoliciesAndReassignThem(short playerID)
		{
			//this.oCPU.Log.EnterBlock($"F0_25fb_2fd7({playerID})");

			// function body
			for (int i = 0; i < 32; i++)
			{
				this.parent.GameData.Players[playerID].UnitPolicies[i].UnitRoleType = UnitRoleTypeEnum.None;
				this.parent.GameData.Players[playerID].UnitPolicies[i].Policy = 0;
			}

			for (int i = 0; i < 16; i++)
			{
				if (this.parent.GameData.Players[playerID].ContinentPolicies[i].UnitRoleType != UnitRoleTypeEnum.None &&
					(int)this.parent.GameData.Players[playerID].ContinentPolicies[i].UnitRoleType <= (int)UnitRoleTypeEnum.Defense)
				{
					// Instruction address 0x25fb:0x3039, size: 3
					PlayerAddUnitPolicy(playerID,
						this.parent.GameData.Players[playerID].ContinentPolicies[i].Position.X,
						this.parent.GameData.Players[playerID].ContinentPolicies[i].Position.Y,
						this.parent.GameData.Players[playerID].ContinentPolicies[i].UnitRoleType,
						(short)this.parent.GameData.Players[playerID].ContinentPolicies[i].Policy);
				}
			}
		}

		/// <summary>
		/// Add new player unit policy
		/// </summary>
		/// <param name="playerID"></param>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="unitRoleType"></param>
		/// <param name="policy"></param>
		public void PlayerAddUnitPolicy(int playerID, int x, int y, UnitRoleTypeEnum unitRoleType, short policy)
		{
			//this.oCPU.Log.EnterBlock($"F0_25fb_304d({playerID}, {x}, {y}, {active}, {policy})");

			// function body
			for (int i = 0; i < 32; i++)
			{
				if (this.parent.GameData.Players[playerID].UnitPolicies[i].Position.X == x &&
					this.parent.GameData.Players[playerID].UnitPolicies[i].Position.Y == y &&
					this.parent.GameData.Players[playerID].UnitPolicies[i].UnitRoleType == unitRoleType &&
					this.parent.GameData.Players[playerID].UnitPolicies[i].Policy <= policy)
				{
					return;
				}
			}

			if (playerID == this.parent.Var_6b90_CurrentPlayer && playerID != this.parent.GameData.HumanPlayerID && 
				(unitRoleType == UnitRoleTypeEnum.SeaAttack || unitRoleType == UnitRoleTypeEnum.AirAttack))
			{
				for (int i = 0; i < 128; i++)
				{
					if (this.parent.GameData.Players[playerID].Units[i].UnitType != UnitTypeEnum.None && this.parent.GameData.Players[playerID].Units[i].RemainingMoves != 0 &&
						this.parent.GameData.Units[(int)this.parent.GameData.Players[playerID].Units[i].UnitType].UnitRoleType == unitRoleType &&
						this.parent.MapManagement.F0_2aea_1942_GetGroupID(this.parent.GameData.Players[playerID].Units[i].Position.X,
							this.parent.GameData.Players[playerID].Units[i].Position.Y) == this.parent.MapManagement.F0_2aea_1942_GetGroupID(x, y))
					{
						// Instruction address 0x25fb:0x313c, size: 5
						int distance = this.parent.Tools.F0_2dc4_0289_GetShortestDistance(this.parent.GameData.Players[playerID].Units[i].Position.X,
							this.parent.GameData.Players[playerID].Units[i].Position.Y, x, y);

						if (this.parent.GameData.Players[playerID].Units[i].RemainingMoves >= distance * 2)
						{
							this.parent.GameData.Players[playerID].Units[i].GoToDestination = new(x, y);
							this.parent.GameData.Players[playerID].Units[i].Status |= 0x10;
							this.parent.GameData.Players[playerID].Units[i].Status &= 0xf0;
							break;
						}
					}
				}
			}

			for (int i = 0; i < 32; i++)
			{
				if (this.parent.GameData.Players[playerID].UnitPolicies[i].UnitRoleType == UnitRoleTypeEnum.None ||
					this.parent.GameData.Players[playerID].UnitPolicies[i].Policy < policy)
				{
					for (int j = 30; j >= i; j--)
					{
						if (this.parent.GameData.Players[playerID].UnitPolicies[j].Policy != 0)
						{
							this.parent.GameData.Players[playerID].UnitPolicies[j + 1].Position =
								this.parent.GameData.Players[playerID].UnitPolicies[j].Position;

							this.parent.GameData.Players[playerID].UnitPolicies[j + 1].UnitRoleType =
								this.parent.GameData.Players[playerID].UnitPolicies[j].UnitRoleType;

							this.parent.GameData.Players[playerID].UnitPolicies[j + 1].Policy =
								this.parent.GameData.Players[playerID].UnitPolicies[j].Policy;
						}
					}

					this.parent.GameData.Players[playerID].UnitPolicies[i].Position = new(x, y);
					this.parent.GameData.Players[playerID].UnitPolicies[i].UnitRoleType = unitRoleType;
					this.parent.GameData.Players[playerID].UnitPolicies[i].Policy = policy;

					break;
				}
			}
		}

		/// <summary>
		/// Clear all continent policies for a player
		/// </summary>
		/// <param name="playerID"></param>
		public void ClearPlayerContinentPolicies(int playerID)
		{
			for (int i = 0; i < 16; i++)
			{
				this.parent.GameData.Players[playerID].ContinentPolicies[i].UnitRoleType = UnitRoleTypeEnum.None;
				this.parent.GameData.Players[playerID].ContinentPolicies[i].Policy = 0;
			}
		}

		/// <summary>
		/// Adds new player continent policy
		/// </summary>
		/// <param name="playerID"></param>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="unitRoleType"></param>
		/// <param name="policy"></param>
		public void AddPlayerContinentPolicy(int playerID, int x, int y, UnitRoleTypeEnum unitRoleType, sbyte policy)
		{
			//this.oCPU.Log.EnterBlock($"F0_25fb_3251_AddStrategicLocation({playerID}, {x}, {y}, {unitRoleType}, {policy})");

			// function body
			for (int i = 0; i < 16; i++)
			{
				if (this.parent.GameData.Players[playerID].ContinentPolicies[i].Position.X == x &&
					this.parent.GameData.Players[playerID].ContinentPolicies[i].Position.Y == y &&
					this.parent.GameData.Players[playerID].ContinentPolicies[i].UnitRoleType == unitRoleType &&
					this.parent.GameData.Players[playerID].ContinentPolicies[i].Policy <= policy)
				{
					return;
				}
			}

			for (int i = 0; i < 16; i++)
			{
				if (this.parent.GameData.Players[playerID].ContinentPolicies[i].UnitRoleType == UnitRoleTypeEnum.None ||
					this.parent.GameData.Players[playerID].ContinentPolicies[i].Policy < policy)
				{
					for (int j = 14; j >= i; j--)
					{
						if (this.parent.GameData.Players[playerID].ContinentPolicies[j].Policy != 0)
						{
							this.parent.GameData.Players[playerID].ContinentPolicies[j + 1].Position =
								this.parent.GameData.Players[playerID].ContinentPolicies[j].Position;

							this.parent.GameData.Players[playerID].ContinentPolicies[j + 1].UnitRoleType =
								this.parent.GameData.Players[playerID].ContinentPolicies[j].UnitRoleType;

							this.parent.GameData.Players[playerID].ContinentPolicies[j + 1].Policy =
								this.parent.GameData.Players[playerID].ContinentPolicies[j].Policy;
						}
					}

					this.parent.GameData.Players[playerID].ContinentPolicies[i].Position = new GPoint(x, y);
					this.parent.GameData.Players[playerID].ContinentPolicies[i].UnitRoleType = unitRoleType;
					this.parent.GameData.Players[playerID].ContinentPolicies[i].Policy = policy;
					break;
				}
			}
		}

		/// <summary>
		/// Clear player continent policy where unitRoleType matches and distance is greater than given maximum
		/// </summary>
		/// <param name="playerID"></param>
		/// <param name="unitRoleType"></param>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="maximumDistance"></param>
		public void F0_25fb_3401_PlayerClearContinentPolicies(int playerID, UnitRoleTypeEnum unitRoleType, int x, int y, int maximumDistance)
		{
			//this.oCPU.Log.EnterBlock($"F0_25fb_3401({playerID}, {unitRoleType}, {x}, {y}, {maximumDistance})");

			// function body
			for (int i = 0; i < 16; i++)
			{
				if (this.parent.GameData.Players[playerID].ContinentPolicies[i].UnitRoleType == unitRoleType)
				{
					// Instruction address 0x25fb:0x3433, size: 5
					if (this.parent.Tools.F0_2dc4_0289_GetShortestDistance(x, y, this.parent.GameData.Players[playerID].ContinentPolicies[i].Position) <= maximumDistance)
					{
						this.parent.GameData.Players[playerID].ContinentPolicies[i].UnitRoleType = UnitRoleTypeEnum.None;
					}
				}
			}
		}

		/// <summary>
		/// Change player city production if the continent (groupID) matches
		/// </summary>
		/// <param name="playerID"></param>
		/// <param name="groupID"></param>
		public void F0_25fb_3459_PlayerChangeCityProductionForSameContinent(int playerID, int groupID)
		{
			//this.oCPU.Log.EnterBlock($"F0_25fb_3459({playerID}, {groupID})");

			// function body
			for (int i = 0; i < 128; i++)
			{
				if (this.parent.GameData.Cities[i].PlayerID == playerID && this.parent.GameData.Cities[i].StatusFlag != 0xff)
				{
					if (groupID == -1 ||
						this.parent.MapManagement.F0_2aea_1942_GetGroupID(this.parent.GameData.Cities[i].Position.X, this.parent.GameData.Cities[i].Position.Y) == groupID)
					{
						// Instruction address 0x25fb:0x34a1, size: 3
						F0_25fb_34b6_ChangeCityProduction(i);
					}
				}
			}
		}

		/// <summary>
		/// Change unit production in a city
		/// </summary>
		/// <param name="cityID"></param>
		public void F0_25fb_34b6_ChangeCityProduction(int cityID)
		{
			//this.oCPU.Log.EnterBlock($"F0_25fb_34b6({cityID})");

			// function body
			int playerID = this.parent.GameData.Cities[cityID].PlayerID;

			if (this.parent.GameData.Cities[cityID].CurrentProductionID >= 0)
			{
				this.parent.GameData.Players[playerID].UnitsInProduction[this.parent.GameData.Cities[cityID].CurrentProductionID]--;
			}
		
			// Instruction address 0x25fb:0x34fc, size: 5
			int productionID = (short)this.parent.Segment_1ade.F0_1ade_0421(playerID, cityID);
			
			this.parent.GameData.Cities[cityID].CurrentProductionID = (sbyte)productionID;

			if (productionID >= 0)
			{
				this.parent.GameData.Players[playerID].UnitsInProduction[this.parent.GameData.Cities[cityID].CurrentProductionID]++;
			}
		}

		/// <summary>
		/// Choose a new unit direction with most available moves for this and next move
		/// </summary>
		/// <param name="playerID"></param>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		public int F0_25fb_3521_UnitChooseNewDirection(int playerID, int x, int y)
		{
			//this.oCPU.Log.EnterBlock($"F0_25fb_3521({playerID}, {x}, {y})");

			// function body
			int directionIndex = -1;
			int directionCount = -1;
			GPoint direction;

			for (int i = 0; i < 9; i++)
			{
				int newDirectionCount = 0;

				direction = this.parent.MoveDirections[i];

				int newX = x + direction.X;
				int newY = y + direction.Y;

				// Instruction address 0x25fb:0x35f8, size: 5
				if (this.parent.MapManagement.F0_2aea_1326_ValidateMapCoordinates(newX, newY) && 
					this.parent.MapManagement.GetTerrainType(newX, newY) == TerrainTypeEnum.Water &&
					(i == 0 || this.parent.MapManagement.F0_2aea_14e0_GetCellUnitPlayerID(newX, newY) == -1))
				{
					for (int j = 1; j < 9; j++)
					{
						direction = this.parent.MoveDirections[j];

						int newX1 = newX + direction.X;
						int newY1 = newY + direction.Y;

						if (this.parent.MapManagement.F0_2aea_1326_ValidateMapCoordinates(newX1, newY1) &&
							this.parent.MapManagement.GetTerrainType(newX1, newY1) != TerrainTypeEnum.Water &&
							this.parent.MapManagement.F0_2aea_14e0_GetCellUnitPlayerID(newX1, newY1) == -1)
						{
							newDirectionCount++;

							int ownerPlayerID = this.parent.MapManagement.GetPlayerLandOwnership(newX1, newY1);

							// Check if the owner of this cell is the player with which we have a peace treaty
							if (ownerPlayerID != -1 && ownerPlayerID != playerID && 
								this.parent.GameData.Players[playerID].Diplomacy[ownerPlayerID].HasFlag(DiplomacyFlagsEnum.Peace))
							{
								newDirectionCount--;
							}
						}
					}

					if (newDirectionCount > directionCount)
					{
						directionCount = newDirectionCount;
						directionIndex = i;
					}
				}
			}

			return directionIndex;
		}

		/// <summary>
		/// Determines next move for barbarian unit
		/// </summary>
		/// <param name="playerID"></param>
		/// <param name="unitID"></param>
		/// <returns></returns>
		public int F0_25fb_362d_MoveBarbarianUnit(int playerID, int unitID)
		{
			//this.oCPU.Log.EnterBlock($"F0_25fb_362d({playerID}, {unitID})");

			// function body
			Unit unit = this.parent.GameData.Players[playerID].Units[unitID];
			int unitX = unit.Position.X;
			int unitY = unit.Position.Y;
			int nearestCityID;
			int nearestCityDistance;

			if (unitY < 2 || unitY >= 48)
			{
				// Instruction address 0x25fb:0x365a, size: 5
				this.parent.UnitManagement.F0_1866_0f10_DeleteUnit(playerID, unitID);

				return ' ';
			}

			if (this.parent.GameData.Units[(int)unit.UnitType].UnitRoleType == UnitRoleTypeEnum.SeaTransport)
			{
				if (unit.NextUnitID == -1)
				{
					// Instruction address 0x25fb:0x36cb, size: 5
					this.parent.UnitManagement.F0_1866_0f10_DeleteUnit(playerID, unitID);

					return ' ';
				}

				// Instruction address 0x25fb:0x36df, size: 5
				nearestCityID = this.parent.Tools.F0_2dc4_0102_FindNearestCity(unitX, unitY);
				nearestCityDistance = this.parent.Tools.F0_2dc4_0289_GetShortestDistance(unit.Position, this.parent.GameData.Cities[nearestCityID].Position);

				for (int i = 1; i < 9; i++)
				{
					GPoint direction = this.parent.MoveDirections[i];

					int newX = this.parent.MapManagement.AdjustXPosition(unitX + direction.X);
					int newY = unitY + direction.Y;

					if (nearestCityDistance <= 8 && this.parent.MapManagement.GetTerrainType(newX, newY) != TerrainTypeEnum.Water &&
						!this.parent.MapManagement.F0_2aea_1585_GetVisibleTerrainImprovements(newX, newY).HasFlag(TerrainImprovementFlagsEnum.City) &&
						this.parent.MapManagement.F0_2aea_14e0_GetCellUnitPlayerID(newX, newY) == -1 && newY > 1 && newY < 48)
					{
						if ((this.parent.GameData.MapVisibility[newX, newY] & (0x1 << this.parent.GameData.HumanPlayerID)) != 0 &&
							this.parent.GameData.Cities[nearestCityID].PlayerID == this.parent.GameData.HumanPlayerID &&
							this.parent.MapManagement.F0_2aea_1942_GetGroupID(newX, newY) == this.parent.MapManagement.F0_2aea_1942_GetGroupID(
								this.parent.GameData.Cities[nearestCityID].Position.X, this.parent.GameData.Cities[nearestCityID].Position.Y))
						{
							unit.VisibleByPlayer |= (ushort)(1 << this.parent.GameData.HumanPlayerID);

							// Instruction address 0x25fb:0x3813, size: 5
							this.parent.UnitManagement.F0_1866_16a9_CenterMap(this.parent.GameData.HumanPlayerID, newX, newY);

							this.parent.Var_2f9e_MessageBoxStyle = MenuBoxReportTypeEnum.DefenseMinisterReport;

							// Instruction address 0x25fb:0x3867, size: 5
							this.parent.Segment_1238.F0_1238_001e_ShowDialog("Barbarian raiding party\nlands near " +
								$"{this.parent.Segment_2459.F0_2459_08c6_GetCityName(this.parent.Tools.F0_2dc4_0102_FindNearestCity(newX, newY))}!\nCitizens are alarmed.\n", 100, 32);
						}

						unit.VisibleByPlayer |= this.parent.GameData.MapVisibility[newX, newY];

						unit.RemainingMoves = 0;

						return 'u';
					}

					if (this.parent.MapManagement.F0_2aea_14e0_GetCellUnitPlayerID(newX, newY) > 0)
					{
						return i;
					}
				}

				if (unit.GoToDestination.X == -1)
				{
					int maximumCityProfit = 0;
					int cityID = -1;

					for (int i = 0; i < 128; i++)
					{
						if (this.parent.GameData.Cities[i].StatusFlag != 0xff)
						{
							int currentCityProfit = (this.parent.Segment_2459.F0_2459_0687_GetCityTreasury(i) + 50) / (this.parent.Tools.F0_2dc4_0289_GetShortestDistance(unitX, unitY,
								this.parent.GameData.Cities[i].Position.X, this.parent.GameData.Cities[i].Position.Y) + 1);

							if (currentCityProfit > maximumCityProfit)
							{
								maximumCityProfit = currentCityProfit;
								cityID = i;
							}
						}
					}

					if (maximumCityProfit == 0)
					{
						// Instruction address 0x25fb:0x36cb, size: 5
						this.parent.UnitManagement.F0_1866_0f10_DeleteUnit(playerID, unitID);

						return ' ';
					}

					unit.GoToDestination = this.parent.GameData.Cities[cityID].Position;

					return '\x0';
				}
			}

			nearestCityID = this.parent.Tools.F0_2dc4_0102_FindNearestCity(unitX, unitY);
			nearestCityDistance = this.parent.Tools.F0_2dc4_0289_GetShortestDistance(unit.Position, this.parent.GameData.Cities[nearestCityID].Position);
			// The algorithm for distance has been contained within FindNearestCity and FindNearestPlayerUnit, so there was a confusion
			// Perhaps we don't need this at all
			int nearestDistance = nearestCityDistance;
			int nearestPlayerID = this.parent.GameData.Cities[nearestCityID].PlayerID;

			// For a case where barbarians capture a City
			if (nearestCityDistance == 0 && (unit.NextUnitID == -1 || this.parent.GameData.Units[(int)unit.UnitType].UnitRoleType == UnitRoleTypeEnum.Defense))
			{
				return 'f';
			}

			int xStep = -2;
			int yStep = -2;

			if (((unitID + this.parent.GameData.TurnCount) & 0x3) == 0)
			{
				if (this.parent.MapManagement.F0_2aea_1942_GetGroupID(unitX, unitY) != this.parent.MapManagement.F0_2aea_1942_GetGroupID(
					this.parent.GameData.Cities[nearestCityID].Position.X, this.parent.GameData.Cities[nearestCityID].Position.Y))
				{
					// Instruction address 0x25fb:0x36cb, size: 5
					this.parent.UnitManagement.F0_1866_0f10_DeleteUnit(playerID, unitID);

					return ' ';
				}

				if (nearestCityDistance > 8)
				{
					// Instruction address 0x25fb:0x36cb, size: 5
					this.parent.UnitManagement.F0_1866_0f10_DeleteUnit(playerID, unitID);

					return ' ';
				}
			}

			if (nearestCityID != -1 &&
				this.parent.MapManagement.F0_2aea_1942_GetGroupID(unitX, unitY) == this.parent.MapManagement.F0_2aea_1942_GetGroupID(
					this.parent.GameData.Cities[nearestCityID].Position.X, this.parent.GameData.Cities[nearestCityID].Position.Y) &&
				(this.parent.GameData.PlayerFlags & (0x1 << this.parent.GameData.Cities[nearestCityID].PlayerID)) != 0)
			{
				yStep = Math.Sign(this.parent.GameData.Cities[nearestCityID].Position.X - unitX);
				xStep = Math.Sign(this.parent.GameData.Cities[nearestCityID].Position.Y - unitY);
			}

			if (playerID == nearestPlayerID ||
				(this.parent.GameData.Players[nearestPlayerID].CityCount < 2 && this.parent.GameData.Players[nearestPlayerID].Coins < 100))
			{
				xStep = -2;
				yStep = -2;
			}

			if (yStep != -2)
			{
				unit.GoToDestination = this.parent.GameData.Cities[nearestCityID].Position;
				unit.GoToNextDirection = (short)this.parent.UnitGoTo.GetNextGoToMove(playerID, unitID);
			}
		
			unit.GoToDestination = OpenCivOneGame.InvalidPosition;

			if (unit.UnitType == UnitTypeEnum.Diplomat)
			{
				TerrainTypeEnum terrainType = this.parent.MapManagement.GetTerrainType(unitX, unitY);

				if (unit.NextUnitID != -1 && terrainType != TerrainTypeEnum.Water &&
					this.parent.GameData.Players[playerID].Units[unit.NextUnitID].UnitType != UnitTypeEnum.Diplomat)
				{
					return ' ';
				}

				if (terrainType == TerrainTypeEnum.Water)
				{
					if (nearestCityDistance < 3)
					{
						// Instruction address 0x25fb:0x36cb, size: 5
						this.parent.UnitManagement.F0_1866_0f10_DeleteUnit(playerID, unitID);

						return ' ';
					}
				}
				else
				{
					// Instruction address 0x25fb:0x3b55, size: 5
					int playerUnitID = this.parent.Tools.F0_2dc4_0177_FindNearestPlayerUnit(playerID, unitID, unitX, unitY);
					if (playerUnitID != -1)
					{
						int nearestUnitDistance = this.parent.Tools.F0_2dc4_0289_GetShortestDistance(unit.Position,
							this.parent.GameData.Players[playerID].Units[playerUnitID].Position);
						nearestDistance = nearestUnitDistance;

						if (nearestUnitDistance < 4 && this.parent.GameData.Players[playerID].Units[playerUnitID].UnitType != UnitTypeEnum.Diplomat)
						{
							unit.GoToDestination = this.parent.GameData.Players[playerID].Units[playerUnitID].Position;

							return '\x0';
						}
					}

					if ((this.parent.GameData.TurnCount & 0x7) + 4 <= nearestCityDistance)
					{
						// Instruction address 0x25fb:0x36cb, size: 5
						this.parent.UnitManagement.F0_1866_0f10_DeleteUnit(playerID, unitID);

						return ' ';
					}
				}
			}

			int newDirection = 0;
			int bestScore = -999;

			for(int i = 1; i < 9; i++)
			{
				GPoint direction = this.parent.MoveDirections[i];

				int newX = this.parent.MapManagement.AdjustXPosition(unitX + direction.X);
				int newY = unitY + direction.Y;
				TerrainTypeEnum terrainType = this.parent.MapManagement.GetTerrainType(newX, newY);

				if (((this.parent.GameData.Units[(int)unit.UnitType].MovementType == UnitMovementTypeEnum.Water) ? 1 : 0) == 
					((terrainType == TerrainTypeEnum.Water) ? 1 : 0))
				{
					if (this.parent.MapManagement.F0_2aea_1326_ValidateMapCoordinates(newX, newY))
					{
						int activeUnitID = this.parent.MapManagement.F0_2aea_1458_GetCellActiveUnitID(newX, newY);

						if (unit.UnitType != UnitTypeEnum.Diplomat)
						{
							if (!this.parent.MapManagement.F0_2aea_1585_GetVisibleTerrainImprovements(newX, newY).HasFlag(TerrainImprovementFlagsEnum.City) ||
								this.parent.MapManagement.F0_2aea_1369_GetCityOwner(newX, newY) == playerID)
							{
								// Instruction address 0x25fb:0x3d09, size: 5
								int attackScore = this.parent.CAPI.RNG.Next(6);

								if ((this.parent.MapManagement.F0_2aea_1585_GetVisibleTerrainImprovements(newX, newY) & (TerrainImprovementFlagsEnum.Irrigation | TerrainImprovementFlagsEnum.Mines)) != TerrainImprovementFlagsEnum.None)
								{
									attackScore += 6;
								}

								if (this.parent.MapManagement.F0_2aea_1369_GetCityOwner(newX, newY) != playerID)
								{
									attackScore += 4;
								}

								bool flag = false;

								if (activeUnitID == -1)
								{
									flag = true;
								}
								else if (playerID != this.parent.Var_d7f0)
								{
									if (this.parent.MapManagement.GetTerrainType(unitX, unitY) != TerrainTypeEnum.Water)
									{
										if (this.parent.GameData.Players[this.parent.Var_d7f0].Units[activeUnitID].UnitType != UnitTypeEnum.Diplomat)
										{
											attackScore += 99;
										}
										flag = true;
									}
								}
								else if (this.parent.MapManagement.GetTerrainType(unitX, unitY) == TerrainTypeEnum.Water)
								{
									attackScore -= 20;
									flag = true;
								}

								if (flag)
								{
									if (!this.parent.UnitManagement.F0_1866_1725_IsUnitNear(playerID, unitX, unitY) ||
										this.parent.GameData.Units[(int)unit.UnitType].MovementType != UnitMovementTypeEnum.Land ||
										activeUnitID != -1 || !this.parent.UnitManagement.F0_1866_1725_IsUnitNear(playerID, newX, newY))
									{
										if (unit.GoToNextDirection == i)
										{
											attackScore += 6;
										}

										direction = this.parent.MoveDirections[i];

										if (direction.X == yStep)
										{
											attackScore += 2;
										}

										if (direction.Y == xStep)
										{
											attackScore += 2;
										}

										if (attackScore > bestScore)
										{
											bestScore = attackScore;
											newDirection = i;
										}
									}
								}
							}
							else if (this.parent.MapManagement.GetTerrainType(unitX, unitY) != TerrainTypeEnum.Water)
							{
								return i;
							}
						}
						else
						{
							int attackScore = 0;

							if (activeUnitID != -1)
							{
								attackScore += ((playerID == this.parent.Var_d7f0) ? 99 : -99);
							}

							attackScore += this.parent.GameData.Terrains[(int)terrainType].MovementCost + this.parent.CAPI.RNG.Next(4) + (nearestDistance * 4);

							if (attackScore > bestScore)
							{
								bestScore = attackScore;
								newDirection = i;
							}
						}
					}
				}
			}

			if (this.parent.MapManagement.F0_2aea_1585_GetVisibleTerrainImprovements(unitX, unitY).HasFlag(TerrainImprovementFlagsEnum.City) &&
				unit.NextUnitID == -1 && bestScore < 99)
			{
				return ' ';
			}
		
			unit.GoToNextDirection = (short)newDirection;

			return newDirection;
		}

		/// <summary>
		/// Move transport near enemy city
		/// </summary>
		/// <param name="cityID"></param>
		public void F0_25fb_3e9c_MoveTransportNearEnemyCity(int cityID)
		{
			//this.oCPU.Log.EnterBlock($"F0_25fb_3e9c({cityID})");

			// function body
			int cityX = this.parent.GameData.Cities[cityID].Position.X;
			int cityY = this.parent.GameData.Cities[cityID].Position.Y;

			for (int i = 1; i < 8; i++)
			{
				if (i != this.parent.GameData.HumanPlayerID && !this.parent.GameData.Players[i].Diplomacy[this.parent.GameData.HumanPlayerID].HasFlag(DiplomacyFlagsEnum.Peace))
				{
					for (int j = 0; j < 128; j++)
					{
						if (this.parent.GameData.Players[i].Units[j].UnitType != UnitTypeEnum.None &&
							this.parent.GameData.Units[(int)this.parent.GameData.Players[i].Units[j].UnitType].TransportCapacity != 0 &&
							this.parent.GameData.Players[i].Units[j].NextUnitID != -1)
						{
							if (this.parent.MapManagement.GetTerrainType(this.parent.GameData.Players[i].Units[j].Position.X,
								this.parent.GameData.Players[i].Units[j].Position.Y) == TerrainTypeEnum.Water)
							{
								if (this.parent.GameData.Units[(int)this.parent.GameData.Players[i].Units[j].UnitType].MoveCount * 3 >
									this.parent.Tools.F0_2dc4_0289_GetShortestDistance(this.parent.GameData.Players[i].Units[j].Position, cityX, cityY))
								{
									this.parent.GameData.Players[i].Units[j].GoToDestination = new(cityX, cityY);
								}
							}
						}
					}
				}
			}
		}
	}
}
