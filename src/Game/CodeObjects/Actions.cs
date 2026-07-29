using IRB.VirtualCPU;
using OpenCivOne.Graphics;
using System.Text;

namespace OpenCivOne
{
	public class Actions
	{
		private OpenCivOneGame parent;

		public Actions(OpenCivOneGame parent)
		{
			this.parent = parent;
		}

		/// <summary>
		/// Player takes over the city by conquest
		/// </summary>
		/// <param name="playerID"></param>
		/// <param name="cityID"></param>
		/// <param name="flag"></param>
		public void F0_2459_0000_CityTakeover(int playerID, int cityID, bool flag)
		{
			//this.oCPU.Log.EnterBlock($"F0_2459_0000({playerID}, {cityID}, {flag})");

			// function body
			int cityPlayerID = this.parent.GameData.Cities[cityID].PlayerID;
			int cityX = this.parent.GameData.Cities[cityID].Position.X;
			int cityY = this.parent.GameData.Cities[cityID].Position.Y;

			if (playerID == this.parent.GameData.HumanPlayerID)
			{
				this.parent.GameData.Players[cityPlayerID].ContactPlayerCountdown = -2;
			}

			if (cityPlayerID != this.parent.GameData.HumanPlayerID && 
				(this.parent.GameData.Cities[cityID].HasImprovement(ImprovementEnum.Palace) || this.parent.GameData.Cities[cityID].CurrentProductionID == -1))
			{
				if (!flag && !this.parent.GameData.Cities[cityID].HasImprovement(ImprovementEnum.Palace) &&
					this.parent.GameData.Players[cityPlayerID].CityCount > 4 &&
					this.parent.GameData.Players[this.parent.GameData.HumanPlayerID].Ranking < this.parent.GameData.Players[cityPlayerID].Ranking)
				{
					if (!this.parent.Secession.F15_0000_0000_NationSecession(cityPlayerID))
					{
						this.parent.GameData.Cities[cityID].PlayerID = (short)playerID;

						int remotestCityID = this.parent.Secession.F15_0000_08ba_GetRemotestCity(cityPlayerID);

						this.parent.GameData.Cities[remotestCityID].CurrentProductionID = -1;

						this.parent.GameData.Cities[cityID].PlayerID = (short)cityPlayerID;
					}
				}
			}

			if (this.parent.GameData.Cities[cityID].HasImprovement(ImprovementEnum.Palace))
			{
				// Instruction address 0x2459:0x00d8, size: 3
				F0_2459_05ee_SpaceshipArrivalFailed(cityPlayerID);
			}

			if (cityPlayerID == this.parent.GameData.HumanPlayerID)
			{
				this.parent.GameData.Players[playerID].ContactPlayerCountdown = -1;
			}

			// Instruction address 0x2459:0x0108, size: 5
			int cityTreasury = this.parent.Tools.CheckValueRange(F0_2459_0687_GetCityTreasury(cityID), 0, this.parent.GameData.Players[cityPlayerID].Coins);

			// Instruction address 0x2459:0x011f, size: 5
			int unitID = this.parent.MapManagement.F0_2aea_1458_GetCellActiveUnitID(cityX, cityY);

			if (playerID == 0 && this.parent.GameData.Units[(int)this.parent.GameData.Players[playerID].Units[unitID].UnitType].AttackStrength <= 2 && !this.parent.Var_28bc)
			{
				// Instruction address 0x2459:0x0155, size: 5
				this.parent.Segment_1ade.F0_1ade_018e(cityID, cityX, cityY);

				this.parent.GameData.Cities[cityID].ActualSize = 0;
			}
			else
			{
				this.parent.GameData.Cities[cityID].ShieldsCount = 0;
				this.parent.GameData.Cities[cityID].StatusFlag &= 0xae;
				this.parent.GameData.Cities[cityID].ImprovementFlags0 &= 0xfbb6;

				if (!flag)
				{
					this.parent.GameData.Cities[cityID].ImprovementFlags0 &= (ushort)(0xaaaa >> this.parent.CAPI.RNG.Next(2));
					this.parent.GameData.Cities[cityID].ImprovementFlags1 &= 0;
				}

				if (!flag || this.parent.GameData.Cities[cityID].ActualSize > 1)
				{
					this.parent.GameData.Cities[cityID].ActualSize--;

					if (this.parent.GameData.Cities[cityID].ActualSize < 1)
					{
						// Instruction address 0x2459:0x01d4, size: 5
						this.parent.Segment_1ade.F0_1ade_018e(cityID, cityX, cityY);
					}
				}
			}

			this.parent.GameData.Players[cityPlayerID].Coins -= (short)cityTreasury;

			if (playerID != 0)
			{
				this.parent.GameData.Players[playerID].Coins += (short)cityTreasury;
			}

			if (this.parent.GameData.Cities[cityID].ActualSize != 0)
			{
				// Instruction address 0x2459:0x0221, size: 5
				this.parent.UnitManagement.F0_1866_250e_AddReplayData(9, (byte)playerID, this.parent.GameData.Cities[cityID].NameID, (byte)((sbyte)cityX), (byte)((sbyte)cityY));

				// Instruction address 0x2459:0x0239, size: 5
				this.parent.AIEngine.F0_25fb_3401_PlayerClearContinentPolicies(playerID, UnitRoleTypeEnum.Settler, cityX, cityY, 4);

				this.parent.GameData.Cities[cityID].StatusFlag &= 0x9b;

				for (int i = 0; i < 20; i++)
				{
					GPoint direction = this.parent.MoveDirections[i];

					// Instruction address 0x2459:0x0258, size: 5
					int newX = this.parent.MapManagement.AdjustXPosition(cityX + direction.X);
					int newY = cityY + direction.Y;

					if (this.parent.MapManagement.ValidateMapCoordinates(newX, newY))
					{
						// Instruction address 0x2459:0x027e, size: 5
						this.parent.MapManagement.SetPlayerLandOwnership(newX, newY, playerID);
					}
				}
			}

			this.parent.GameData.Cities[cityID].PlayerID = (short)playerID;

			if (playerID == this.parent.GameData.HumanPlayerID || cityPlayerID == this.parent.GameData.HumanPlayerID)
			{
				// Instruction address 0x2459:0x02b4, size: 5
				this.parent.MapManagement.F0_2aea_1601_UpdateVisibleCellStatus(cityX, cityY);

				if (cityPlayerID == this.parent.GameData.HumanPlayerID)
				{
					// Instruction address 0x2459:0x02cb, size: 5
					this.parent.UnitManagement.F0_1866_16a9_CenterMap(this.parent.GameData.HumanPlayerID, cityX, cityY);
				}

				// Instruction address 0x2459:0x02d9, size: 5
				this.parent.MapManagement.F0_2aea_03ba_DrawCell(cityX, cityY);

				// Instruction address 0x2459:0x035d, size: 5
				this.parent.CommonTools.PlayTune(this.parent.GameData.Nations[this.parent.GameData.Players[playerID].NationalityID].ShortTune, 0);

				if (this.parent.GameData.GameSettingFlags.Animations)
				{
					this.parent.CityView.F19_0000_0000_ShowCityLayout(cityID, -2,
						$"{this.parent.GameData.Players[playerID].Nation} capture\n{F0_2459_08c6_GetCityName(cityID)}. " +
						$"{cityTreasury} gold\npieces plundered.\n");

					this.parent.CityView.F19_0000_167b_InvadersAnimation(playerID);

					// Instruction address 0x2459:0x03b0, size: 5
					this.parent.Segment_1238.F0_1238_1b44();
				}
				else
				{
					this.parent.News.F21_0000_0000_ShowNews(cityID,
						$"{this.parent.GameData.Players[playerID].Nation} capture\n{F0_2459_08c6_GetCityName(cityID)}. " +
						$"{cityTreasury} gold\npieces plundered.\n");
				}

				// Instruction address 0x2459:0x03c6, size: 5
				this.parent.CommonTools.PlayTune(1, 0);

				if (playerID == this.parent.GameData.HumanPlayerID)
				{
					this.parent.GameData.Cities[cityID].VisibleSize = 0;
				}
				else
				{
					this.parent.GameData.Cities[cityID].VisibleSize = (sbyte)this.parent.GameData.Cities[cityID].ActualSize;
				}
			}
			else if (this.parent.GameData.Players[this.parent.GameData.HumanPlayerID].Diplomacy[playerID].HasFlag(DiplomacyFlagsEnum.EmbassyEstablished) ||
				this.parent.GameData.Players[this.parent.GameData.HumanPlayerID].Diplomacy[cityPlayerID].HasFlag(DiplomacyFlagsEnum.EmbassyEstablished))
			{
				this.parent.Var_2f9e_MessageBoxStyle = MenuBoxReportTypeEnum.SpiesReport;

				if (this.parent.GameData.Cities[cityID].ActualSize != 0)
				{
					// Instruction address 0x2459:0x04b4, size: 5
					this.parent.Segment_1238.F0_1238_001e_ShowDialog(
						$"Spies report:\n{this.parent.GameData.Players[playerID].Nation} capture\nthe " +
						$"{this.parent.GameData.Players[cityPlayerID].Nationality} city\nof {F0_2459_08c6_GetCityName(cityID)}.\n", 100, 80);
				}
				else
				{
					// Instruction address 0x2459:0x04b4, size: 5
					this.parent.Segment_1238.F0_1238_001e_ShowDialog(
						$"Spies report:\n{this.parent.GameData.Players[playerID].Nation} destroy\nthe " +
						$"{this.parent.GameData.Players[cityPlayerID].Nationality} city\nof {F0_2459_08c6_GetCityName(cityID)}.\n", 100, 80);
				}
			}

			// Instruction address 0x2459:0x04c3, size: 3
			F0_2459_06f2_AcquireTechnologyFromAnotherPlayer(playerID, cityPlayerID);

			// Instruction address 0x2459:0x04d2, size: 5
			this.parent.MapManagement.F0_2aea_138c_SetCityOwner(cityX, cityY, playerID);

			for (int local_10 = 0; local_10 < 128; local_10++)
			{
				if (this.parent.GameData.Players[cityPlayerID].Units[local_10].UnitType != UnitTypeEnum.None &&
					this.parent.GameData.Players[cityPlayerID].Units[local_10].HomeCityID == cityID)
				{
					if ((this.parent.GameData.Players[cityPlayerID].Units[local_10].Position.X != this.parent.GameData.Cities[cityID].Position.X) ||
						(this.parent.GameData.Players[cityPlayerID].Units[local_10].Position.Y != this.parent.GameData.Cities[cityID].Position.Y) ||
						(this.parent.GameData.Cities[cityID].ActualSize == 0))
					{
						// Instruction address 0x2459:0x0529, size: 5
						this.parent.UnitManagement.F0_1866_0f10_DeleteUnit(cityPlayerID, local_10);
					}
				}
			}
			
			if (playerID == this.parent.GameData.HumanPlayerID && this.parent.GameData.Players[this.parent.GameData.HumanPlayerID].GovernmentType < 4)
			{
				this.parent.GameData.Cities[cityID].StatusFlag |= 1;
			}

			if (this.parent.GameData.Cities[cityID].ActualSize != 0)
			{
				if (this.parent.GameData.Cities[cityID].CurrentProductionID >= 0)
				{
					this.parent.GameData.Players[cityPlayerID].UnitsInProduction[this.parent.GameData.Cities[cityID].CurrentProductionID]--;
				}

				if (playerID == this.parent.GameData.HumanPlayerID)
				{
					// Instruction address 0x2459:0x0597, size: 5
					this.parent.CityWorker.F0_1d12_0045_ProcessCityState(cityID, 1);

					// Instruction address 0x2459:0x059f, size: 5
					this.parent.Segment_1238.F0_1238_1b44();
				}
				else
				{
					this.parent.GameData.Cities[cityID].CurrentProductionID = 0;

					// Instruction address 0x2459:0x05bb, size: 5			
					this.parent.GameData.Cities[cityID].CurrentProductionID = (sbyte)((short)this.parent.Segment_1ade.F0_1ade_0421(playerID, cityID));

					if (this.parent.GameData.Cities[cityID].CurrentProductionID >= 0)
					{
						this.parent.GameData.Players[playerID].UnitsInProduction[this.parent.GameData.Cities[cityID].CurrentProductionID]++;
					}
				}
			}

			this.parent.StartGameMenu.F5_0000_0e6c_TestIfAIPlayerIsDestroyed(cityPlayerID, playerID);
		}

		/// <summary>
		/// Process spaceship failure
		/// </summary>
		/// <param name="playerID"></param>
		public void F0_2459_05ee_SpaceshipArrivalFailed(int playerID)
		{
			//this.oCPU.Log.EnterBlock($"F0_2459_05ee({playerID})");

			// function body
			if ((this.parent.GameData.SpaceshipFlags & (0x1 << (playerID + 8))) != 0)
			{
				if ((this.parent.GameData.SpaceshipFlags & (0x1 << playerID)) != 0)
				{
					// Instruction address 0x2459:0x0656, size: 5
					this.parent.Segment_1238.F0_1238_001e_ShowDialog($"{this.parent.GameData.Players[playerID].Nationality} spaceship\nreturns to Earth.\n", 100, 80);
				}
				else
				{
					// Instruction address 0x2459:0x0656, size: 5
					this.parent.Segment_1238.F0_1238_001e_ShowDialog($"{this.parent.GameData.Players[playerID].Nationality} spaceship\nconstruction cancelled.\n", 100, 80);
				}
			}

			this.parent.GameData.SpaceshipFlags |= (short)(0x1 << playerID);
			this.parent.GameData.SpaceshipFlags ^= (short)(0x1 << playerID);

			this.parent.GameData.SpaceshipFlags |= (short)(0x1 << (playerID + 8));
			this.parent.GameData.SpaceshipFlags ^= (short)(0x1 << (playerID + 8));

			this.parent.StartGameMenu.F5_0000_1d1a_InitSpaceshipData(playerID);			
		}

		/// <summary>
		/// Calculate distributed treasury for a specific city (city worth)
		/// </summary>
		/// <param name="cityID"></param>
		/// <returns></returns>
		public int F0_2459_0687_GetCityTreasury(int cityID)
		{
			//this.oCPU.Log.EnterBlock($"F0_2459_0687({cityID})");

			// function body
			int playerID = this.parent.GameData.Cities[cityID].PlayerID;

			return (this.parent.GameData.Players[playerID].Coins * this.parent.GameData.Cities[cityID].ActualSize) / (this.parent.GameData.Players[playerID].TotalCitySize + 1);
		}

		/// <summary>
		/// Choose what technology to take from another player
		/// </summary>
		/// <param name="playerID1"></param>
		/// <param name="playerID2"></param>
		public void F0_2459_06f2_AcquireTechnologyFromAnotherPlayer(int playerID1, int playerID2)
		{
			//this.oCPU.Log.EnterBlock($"F0_2459_06f2({playerID1}, {playerID2})");

			// function body			
			if (playerID1 != 0 && playerID2 != 0)
			{
				int latestTechnology = -1;
				int[] undiscoveredTechnologies = new int[72];
				StringBuilder menuText = new();

				// Instruction address 0x2459:0x0719, size: 5
				menuText.Append("Select one...\n ");

				int maximumTechnologyGrade = -1;
				int j = 0;

				for (int i = 71; i >= 0; i--, j++)
				{
					if (!this.parent.Segment_1ade.F0_1ade_22b5_PlayerHasTechnology(playerID1, (TechnologyAdvanceEnum)i) &&
						this.parent.Segment_1ade.F0_1ade_22b5_PlayerHasTechnology(playerID2, (TechnologyAdvanceEnum)i))
					{
						// Instruction address 0x2459:0x0762, size: 5
						menuText.Append($"{this.parent.GameData.TechnologyAdvances[i].Name}\n ");

						undiscoveredTechnologies[j] = i;

						if (this.parent.Segment_1ade.F0_1ade_2317_GradeTechnology(playerID1, (TechnologyAdvanceEnum)i) > maximumTechnologyGrade)
						{
							maximumTechnologyGrade = this.parent.Segment_1ade.F0_1ade_2317_GradeTechnology(playerID1, (TechnologyAdvanceEnum)i);
							latestTechnology = i;
						}
					}
				}

				if (j != 0)
				{
					if (playerID1 == this.parent.GameData.HumanPlayerID)
					{
						int selectedTechnology = -1;

						while (selectedTechnology == -1)
						{
							if (this.parent.Var_3934 == -1)
							{
								// Instruction address 0x2459:0x07e6, size: 5
								selectedTechnology = this.parent.Segment_1238.F0_1238_001e_ShowDialog(menuText.ToString(), 80, 32);
							}
							else
							{
								selectedTechnology = this.parent.MeetWithKing.F6_0000_251d_ShowInlineDialog(menuText.ToString(), 36, 139);
							}
						}

						// Instruction address 0x2459:0x0818, size: 5
						this.parent.Segment_1ade.F0_1ade_1d2e(playerID1, (TechnologyAdvanceEnum)undiscoveredTechnologies[selectedTechnology], playerID2);
					}
					else
					{
						if (playerID2 == this.parent.GameData.HumanPlayerID)
						{
							if (this.parent.Var_3934 == -1)
							{
								// Instruction address 0x2459:0x0886, size: 5
								this.parent.Segment_1238.F0_1238_001e_ShowDialog(
									$"{this.parent.GameData.Players[playerID1].Nation} take\n{this.parent.GameData.TechnologyAdvances[latestTechnology].Name}.\n", 80, 80);
							}
							else
							{
								this.parent.MeetWithKing.F6_0000_251d_ShowInlineDialog(
									$"{this.parent.GameData.Players[playerID1].Nation} take\n{this.parent.GameData.TechnologyAdvances[latestTechnology].Name}.\n", 36, 139);
							}
						}

						// Instruction address 0x2459:0x08aa, size: 5
						this.parent.Segment_1ade.F0_1ade_1d2e(playerID1, (TechnologyAdvanceEnum)latestTechnology, playerID2);

						// Instruction address 0x2459:0x08b9, size: 5
						this.parent.AIEngine.F0_25fb_3459_PlayerChangeCityProductionForSameContinent(playerID1, -1);
					}
				}
			}
		}

		/// <summary>
		/// Returns the name of the city
		/// </summary>
		/// <param name="cityID"></param>
		/// <returns>City name as string</returns>
		public string F0_2459_08c6_GetCityName(int cityID)
		{
			//this.oCPU.Log.EnterBlock($"F0_2459_08c6_GetCityName({cityID})");

			// function body
			if (cityID >= 0 && cityID < 128)
			{
				return this.parent.GameData.CityNames[this.parent.GameData.Cities[cityID].NameID];
			}

			return "NONE";
		}

		/// <summary>
		/// Waits for Key press or Mouse click
		/// </summary>
		public int F0_2459_0918_WaitForKeyPressOrMouseClick()
		{
			//this.oCPU.Log.EnterBlock("F0_2459_0918_WaitForKeyPressOrMouseClick()");

			// function body
			// Instruction address 0x2459:0x0918, size: 5
			this.parent.CommonTools.ClearKeyboardAndMouseEvents();

			MouseEvent mouseEvent;

			while ((mouseEvent = this.parent.GetMouseEvent()).Buttons == MouseButtonsEnum.None && this.parent.CAPI.kbhit() == 0) { }

			if (mouseEvent.Buttons == MouseButtonsEnum.None && this.parent.CAPI.getch() == 0)
			{
				// Instruction address 0x2459:0x0942, size: 5
				return this.parent.CAPI.getch();
			}

			return -1;
		}

		private string[] caravanGoodsNames = { "Silk", "Silver", "Wine", "Copper", "Gems", "Dye", "Salt", "Spice" };

		/// <summary>
		/// Processes Caravan unit arrival at another city
		/// </summary>
		/// <param name="playerID"></param>
		/// <param name="unitID"></param>
		/// <param name="cityID"></param>
		/// <returns></returns>
		public void F0_2459_0948_CaravanArrivesAtDestinationCity(int playerID, int unitID, int cityID)
		{
			//this.oCPU.Log.EnterBlock($"F0_2459_0948({playerID}, {unitID}, {cityID})");

			// function body
			int homeCityID = this.parent.GameData.Players[playerID].Units[unitID].HomeCityID;

			// Instruction address 0x2459:0x098e, size: 5
			int caravanGoodsWorth = this.parent.MapManagement.GetDistance(this.parent.GameData.Cities[cityID].Position, 
				this.parent.GameData.Cities[homeCityID].Position) + 10;

			caravanGoodsWorth *= this.parent.GameData.Cities[cityID].BaseTrade + this.parent.GameData.Cities[homeCityID].BaseTrade;
			caravanGoodsWorth /= 24;

			// Instruction address 0x2459:0x09c4, size: 5
			int homeCityGroupID = this.parent.MapManagement.F0_2aea_1942_GetGroupID(this.parent.GameData.Cities[homeCityID].Position);

			// Instruction address 0x2459:0x09db, size: 5
			int cityGroupID = this.parent.MapManagement.F0_2aea_1942_GetGroupID(this.parent.GameData.Cities[cityID].Position);

			if (cityGroupID == homeCityGroupID)
			{
				caravanGoodsWorth /= 2;
			}

			if (this.parent.GameData.Cities[cityID].PlayerID == playerID)
			{
				caravanGoodsWorth /= 2;
			}

			// Instruction address 0x2459:0x0a24, size: 5
			if (this.parent.Segment_1ade.F0_1ade_22b5_PlayerHasTechnology(this.parent.GameData.Cities[cityID].PlayerID, TechnologyAdvanceEnum.Railroad))
			{
				caravanGoodsWorth -= caravanGoodsWorth / 3;
			}

			// Instruction address 0x2459:0x0a4e, size: 5
			if (this.parent.Segment_1ade.F0_1ade_22b5_PlayerHasTechnology(this.parent.GameData.Cities[cityID].PlayerID, TechnologyAdvanceEnum.Flight))
			{
				caravanGoodsWorth -= caravanGoodsWorth / 3;
			}

			this.parent.GameData.Players[playerID].Coins += (short)caravanGoodsWorth;
			this.parent.GameData.Players[playerID].ResearchProgress += (short)caravanGoodsWorth;

			if (playerID == this.parent.GameData.HumanPlayerID)
			{
				// Instruction address 0x2459:0x0b16, size: 5
				this.parent.Segment_1238.F0_1238_001e_ShowDialog(
					$"{caravanGoodsNames[unitID & 0x7]} caravan from {F0_2459_08c6_GetCityName(homeCityID)}\narrives in {F0_2459_08c6_GetCityName(cityID)}\n" +
					$"Trade route established\nThe goods sold for {caravanGoodsWorth} coins.\n", 80, 80);

				// Instruction address 0x2459:0x0b1e, size: 5
				this.parent.Segment_1238.F0_1238_107e();
			}

			// Instruction address 0x2459:0x0b29, size: 5
			this.parent.UnitManagement.F0_1866_0f10_DeleteUnit(playerID, unitID);

			int totalTradeWorth = this.parent.GameData.Cities[cityID].BaseTrade;

			if (this.parent.GameData.Cities[cityID].PlayerID != playerID)
			{
				totalTradeWorth *= 2;
			}

			int maximumTradeCityID = -1;

			for (int i = 0; i < 3; i++)
			{
				int tradeCityID = this.parent.GameData.Cities[homeCityID].TradeCityIDs[i];

				if (tradeCityID != cityID)
				{
					int currentTradeWorth = -1;

					if (tradeCityID != -1)
					{
						currentTradeWorth = this.parent.GameData.Cities[tradeCityID].BaseTrade;

						if (this.parent.GameData.Cities[tradeCityID].PlayerID != playerID)
						{
							currentTradeWorth *= 2;
						}
					}

					if (currentTradeWorth < totalTradeWorth)
					{
						totalTradeWorth = currentTradeWorth;
						maximumTradeCityID = i;
					}
				}
			}

			if (maximumTradeCityID != -1)
			{
				this.parent.GameData.Cities[homeCityID].TradeCityIDs[maximumTradeCityID] = (sbyte)cityID;
			}
		}
	}
}
