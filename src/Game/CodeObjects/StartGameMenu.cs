using IRB.VirtualCPU;
using OpenCivOne.Graphics;
using System.Text;

namespace OpenCivOne
{
	public class StartGameMenu
	{
		private OpenCivOneGame parent;
		private VCPU CPU;

		public StartGameMenu(OpenCivOneGame parent)
		{
			this.parent = parent;
			this.CPU = parent.CPU;
		}

		/// <summary>
		/// Init the new game data and ask for game parameters
		/// </summary>
		public void F5_0000_0000_InitNewGameData()
		{
			//this.oCPU.Log.EnterBlock("F5_0000_0000()");

			// function body
			// Instruction address 0x0000:0x0007, size: 5
			this.parent.MainCode.F0_11a8_0268_HideMouse();

			// Instruction address 0x0000:0x0024, size: 5
			this.parent.Graphics.F0_VGA_07d8_DrawImage(this.parent.Var_19d4_Screen1_Rectangle, 0, 0, 320, 200, this.parent.Var_aa_Screen0_Rectangle, 0, 0);

			// Instruction address 0x0000:0x002c, size: 5
			this.parent.MainCode.F0_11a8_0250_ShowMouse();

			this.parent.Var_aa_Screen0_Rectangle.FontID = 7;

			// Instruction address 0x0000:0x0056, size: 5
			this.parent.GameData.DifficultyLevel = (short)this.parent.MenuBoxDialog.F0_2d05_0031_ShowMenuBox(
				"Difficulty Level...\n Chieftain (easiest)\n Warlord\n Prince\n King\n Emperor (toughest)\n", 160, 35, false, false, true);

			if (this.parent.Var_1a3c_MouseAvailable && this.parent.Var_2fa2_DialogMousePressed)
			{
				// Instruction address 0x0000:0x006f, size: 5
				this.parent.MainCode.F0_11a8_0223_UpdateMouseState();

				if (this.parent.Var_db3c_MouseXPos < 135)
				{
					// Instruction address 0x0000:0x0090, size: 5
					this.parent.GameData.DifficultyLevel = (short)this.parent.GameTools.F0_2dc4_007c_CheckValueRange((this.parent.Var_db3e_MouseYPos - 12) / 35, 0, 4);
				}
			}

			if (this.parent.GameData.DifficultyLevel < -1)
			{
				this.parent.GameData.DifficultyLevel = 0;
			}

			// Instruction address 0x0000:0x00a8, size: 5
			this.parent.MainCode.F0_11a8_0268_HideMouse();

			// Instruction address 0x0000:0x00c0, size: 5
			this.parent.CommonTools.F0_1000_0bfa_FillRectangle(this.parent.Var_aa_Screen0_Rectangle, 0, 0, 150, 200, 0);

			if ((this.parent.GameData.DifficultyLevel & 0x1) != 0)
			{
				// Instruction address 0x0000:0x00fb, size: 5
				this.parent.Graphics.F0_VGA_07d8_DrawImage(this.parent.Var_19d4_Screen1_Rectangle,
					80, (35 * this.parent.GameData.DifficultyLevel) + 6, 53, 47, this.parent.Var_aa_Screen0_Rectangle, 20, 100);
			}
			else
			{
				// Instruction address 0x0000:0x00fb, size: 5
				this.parent.Graphics.F0_VGA_07d8_DrawImage(this.parent.Var_19d4_Screen1_Rectangle,
					21, (35 * this.parent.GameData.DifficultyLevel) + 6, 53, 47, this.parent.Var_aa_Screen0_Rectangle, 20, 100);
			}

			// Instruction address 0x0000:0x0121, size: 5
			this.parent.Graphics.F0_VGA_07d8_DrawImage(this.parent.Var_19d4_Screen1_Rectangle, 150, 0, 160, 200, this.parent.Var_aa_Screen0_Rectangle, 150, 0);

			// Instruction address 0x0000:0x0129, size: 5
			this.parent.MainCode.F0_11a8_0250_ShowMouse();

			StringBuilder competitionList = new();
			competitionList.Append("Level of Competition...\n ");

			for (int i = 7; i > 2; i--)
			{
				// Instruction address 0x0000:0x016b, size: 5
				competitionList.Append($"{i} Civilizations\n ");
			}

			do
			{
				// Instruction address 0x0000:0x0188, size: 5
				this.parent.GameData.AIOpponentCount = (short)this.parent.MenuBoxDialog.F0_2d05_0031_ShowMenuBox(competitionList.ToString(), 160, 35, false, false, true);
			}
			while (this.parent.GameData.AIOpponentCount == -1);

			// Instruction address 0x0000:0x01a8, size: 5
			this.parent.GameData.AIOpponentCount = (short)this.parent.GameTools.F0_2dc4_007c_CheckValueRange(6 - this.parent.GameData.AIOpponentCount, 2, 6);
			this.parent.GameData.ActiveCivilizations = (short)(0xff >> (6 - this.parent.GameData.AIOpponentCount));

			// Instruction address 0x0000:0x01c1, size: 5
			this.parent.MainCode.F0_11a8_0268_HideMouse();

			for (int i = this.parent.GameData.AIOpponentCount; i >= 0; i--)
			{
				if ((this.parent.GameData.DifficultyLevel & 0x1) != 0)
				{
					// Instruction address 0x0000:0x01d6, size: 5
					this.parent.Graphics.F0_VGA_07d8_DrawImage(this.parent.Var_19d4_Screen1_Rectangle,
						80, (35 * this.parent.GameData.DifficultyLevel) + 6, 53, 47,
						this.parent.Var_aa_Screen0_Rectangle, (i * 2) + 20, (i * 3) + 100);
				}
				else
				{
					// Instruction address 0x0000:0x01d6, size: 5
					this.parent.Graphics.F0_VGA_07d8_DrawImage(this.parent.Var_19d4_Screen1_Rectangle,
						21, (35 * this.parent.GameData.DifficultyLevel) + 6, 53, 47,
						this.parent.Var_aa_Screen0_Rectangle, (i * 2) + 20, (i * 3) + 100);
				}
			}

			this.parent.GameData.Year = -4000;
			this.parent.GameData.DebugFlags = 0xf;

			for (int i = 0; i < 128; i++)
			{
				this.parent.GameData.Cities[i].NameID = 0xff;
				this.parent.GameData.Cities[i].StatusFlag = 0xff;
			}

			for (int i = 0; i < 256; i++)
			{
				this.parent.GameData.CityPositions[i].X = -1;
			}

			for (int i = 0; i < 72; i++)
			{
				this.parent.GameData.TechnologyFirstDiscoveredBy[i] = 0;
			}

			for (int i = 0; i < 22; i++)
			{
				this.parent.GameData.WonderCityID[i] = -1;
			}

			for (int i = 0; i < 8; i++)
			{
				this.parent.GameData.Players[i].NationalityID = -1;

				F5_0000_1d1a_InitSpaceshipData(i);
			}

			for (int i = 0; i < 80; i++)
			{
				for (int j = 0; j < 50; j++)
				{
					this.parent.GameData.MapVisibility[i, j] = 0;
				}
			}

			this.parent.GameData.GameSettingFlags.InstantAdvice = false;
			this.parent.GameData.GameSettingFlags.AutoSave = true;
			this.parent.GameData.GameSettingFlags.EndOfTurn = false;
			this.parent.GameData.GameSettingFlags.Animations = true;
			this.parent.GameData.GameSettingFlags.Sound = true;
			this.parent.GameData.GameSettingFlags.EnemyMoves = true;
			this.parent.GameData.GameSettingFlags.EncyclopediaText = true;
			this.parent.GameData.GameSettingFlags.BuildPalace = true;

			if (this.parent.Var_1a30_SoundDriverType == 'N')
			{
				this.parent.GameData.GameSettingFlags.Sound = false;
			}

			// Instruction address 0x0000:0x0311, size: 5
			StringBuilder tribeList = new();

			tribeList.Append("Pick your tribe...\n ");

			for (int i = 1; i < this.parent.GameData.AIOpponentCount + 2; i++)
			{
				// Instruction address 0x0000:0x033d, size: 5
				tribeList.Append($"{this.parent.GameData.Nations[i].Nationality}\n ");
			}

			for (int i = 1; i < this.parent.GameData.AIOpponentCount + 2; i++)
			{
				// Instruction address 0x0000:0x0377, size: 5
				tribeList.Append($"{this.parent.GameData.Nations[8 + i].Nationality}\n ");
			}

			// Instruction address 0x0000:0x03ba, size: 5
			this.parent.Graphics.F0_VGA_07d8_DrawImage(this.parent.Var_19d4_Screen1_Rectangle, 150, 0, 160, 200, this.parent.Var_aa_Screen0_Rectangle, 150, 0);

			// Instruction address 0x0000:0x03c2, size: 5
			this.parent.MainCode.F0_11a8_0250_ShowMouse();

			// Instruction address 0x0000:0x03d3, size: 5
			this.parent.GameData.HumanPlayerID = (short)this.parent.MenuBoxDialog.F0_2d05_0031_ShowMenuBox(tribeList.ToString(), 160, 35, false, false, true);

			if (this.parent.GameData.HumanPlayerID <= this.parent.GameData.AIOpponentCount)
			{
				this.parent.GameData.HumanPlayerID++;
			}
			else
			{
				this.parent.GameData.HumanPlayerID = (short)((this.parent.GameData.HumanPlayerID - this.parent.GameData.AIOpponentCount) + 8);
			}

			if (this.parent.GameData.DifficultyLevel == 0)
			{
				this.parent.GameData.GameSettingFlags.InstantAdvice = true;
			}

			// Another indexing error. Value this.oParent.GameState.HumanPlayerID is equal
			// to nationality selected and not actual ID, but later in code it is forced to 0-7 by value & 7,
			// so we will use this value instead
			this.parent.GameData.Players[this.parent.GameData.HumanPlayerID & 7].ResearchTechnologyID = -1;

			// Instruction address 0x0000:0x0410, size: 5
			this.parent.GameData.NextAnthologyTurn += (short)this.parent.CAPI.RNG.Next(50);

			for (int i = 0; i < 12; i++)
			{
				this.parent.GameData.Players[this.parent.GameData.HumanPlayerID & 7].PalaceData1[i + 2] = -1;
				this.parent.GameData.Players[this.parent.GameData.HumanPlayerID & 7].PalaceData2[i] = 0;
			}

			for (int i = 3; i < 6; i++)
			{
				this.parent.GameData.Players[this.parent.GameData.HumanPlayerID & 7].PalaceData1[i + 2] = 0;
				this.parent.GameData.Players[this.parent.GameData.HumanPlayerID & 7].PalaceData1[i + 8] = 0;
			}

			this.parent.GameData.Players[this.parent.GameData.HumanPlayerID & 7].Nationality = "";

			if (this.parent.GameData.HumanPlayerID != 0)
			{
				int upperID = (this.parent.GameData.HumanPlayerID > 7) ? 1 : 0;
				this.parent.GameData.HumanPlayerID &= 7;

				if (upperID != 0)
				{
					this.parent.GameData.Players[this.parent.GameData.HumanPlayerID].NationalityID = (short)(this.parent.GameData.HumanPlayerID + 8);
				}
				else
				{
					this.parent.GameData.Players[this.parent.GameData.HumanPlayerID].NationalityID = this.parent.GameData.HumanPlayerID;
				}

				this.parent.GameData.Players[this.parent.GameData.HumanPlayerID].Name =
					this.parent.GameData.Nations[this.parent.GameData.Players[this.parent.GameData.HumanPlayerID].NationalityID].Leader;

				this.parent.GameData.Players[this.parent.GameData.HumanPlayerID].Nationality =
					this.parent.GameData.Nations[this.parent.GameData.Players[this.parent.GameData.HumanPlayerID].NationalityID].Nationality;

				if (string.IsNullOrEmpty(this.parent.GameData.Nations[this.parent.GameData.Players[this.parent.GameData.HumanPlayerID].NationalityID].Nation))
				{
					this.parent.GameData.Players[this.parent.GameData.HumanPlayerID].Nation = this.parent.GameData.Players[this.parent.GameData.HumanPlayerID].Nationality + "s";
				}
				else
				{
					this.parent.GameData.Players[this.parent.GameData.HumanPlayerID].Nation = 
						this.parent.GameData.Nations[this.parent.GameData.Players[this.parent.GameData.HumanPlayerID].NationalityID].Nation;
				}
			}
			else
			{
				// Instruction address 0x0000:0x047f, size: 5
				this.parent.GameData.HumanPlayerID = (short)(this.parent.CAPI.RNG.Next(this.parent.GameData.AIOpponentCount) + 1);

				this.parent.GameData.Players[this.parent.GameData.HumanPlayerID].Name =
					this.parent.GameData.Nations[this.parent.GameData.HumanPlayerID].Leader;

				// For example 'Zulus', 'Americans', etc.
				this.parent.GameData.Players[this.parent.GameData.HumanPlayerID].Nation =
					this.parent.GameData.Nations[this.parent.GameData.HumanPlayerID].Nation;

				// For example 'Zulu', 'American', etc.
				this.parent.GameData.Players[this.parent.GameData.HumanPlayerID].Nationality =
					this.parent.GameData.Nations[this.parent.GameData.HumanPlayerID].Nationality;

				this.parent.TextBoxDialogs.F23_0000_0173_TribeNameDialog();

				this.parent.GameData.Players[this.parent.GameData.HumanPlayerID].NationalityID = this.parent.GameData.HumanPlayerID;
			}

			// Instruction address 0x0000:0x0587, size: 5
			this.parent.DrawTools.F0_1182_00b3_DrawCenteredStringToScreen0(
				this.parent.GameData.Players[this.parent.GameData.HumanPlayerID].Nation, 46, 92, 15);

			for (int i = 0; i < 8; i++)
			{
				for (int j = 0; j < 8; j++)
				{
					this.parent.GameData.Players[i].UnitsDestroyed[j] = 0;
				}
				F5_0000_07c7(i);
			}

			//F5_0000_0fe6();

			this.parent.GameData.PlayerFlags = (short)(0x1 << this.parent.GameData.HumanPlayerID);

			this.parent.GameData.Players[this.parent.GameData.HumanPlayerID].TaxRate = 5;
			this.parent.GameData.Players[this.parent.GameData.HumanPlayerID].ScienceTaxRate = 5;

			if (this.parent.GameData.DifficultyLevel == 0)
			{
				this.parent.GameData.Players[this.parent.GameData.HumanPlayerID].Coins = 50;
			}

			this.parent.GameData.PlayerIdentityFlags = 0;

			for (int i = 0; i < 8; i++)
			{
				if (this.parent.GameData.Players[i].NationalityID >= 8)
				{
					this.parent.GameData.PlayerIdentityFlags |= (short)(0x1 << i);
				}
			}

			// Instruction address 0x0000:0x062a, size: 5
			this.parent.MainCode.F0_11a8_0268_HideMouse();

			// Instruction address 0x0000:0x0645, size: 5
			this.parent.CommonTools.F0_1000_0bfa_FillRectangle(this.parent.Var_aa_Screen0_Rectangle, 150, 0, 170, 200, 0);

			this.parent.TextBoxDialogs.F23_0000_00d6_PlayerNameDialog();

			// Instruction address 0x0000:0x0652, size: 5
			this.parent.MainCode.F0_11a8_0250_ShowMouse();

			this.parent.Var_aa_Screen0_Rectangle.FontID = 1;

			// Instruction address 0x0000:0x066e, size: 5
			this.parent.Array_30b8[3] = this.parent.GameData.Players[this.parent.GameData.HumanPlayerID].Name;

			// Instruction address 0x0000:0x0684, size: 5
			this.parent.Array_30b8[0] = this.parent.GameData.Players[this.parent.GameData.HumanPlayerID].Nation;

			// Instruction address 0x0000:0x0695, size: 5
			this.parent.CAPI.strcpy(0xba06, this.parent.LanguageTools.F0_2f4d_044f_GetTextFromKingSection("*INIT"));

			int local_2 = 0;

			for (int i = 0; i < 72; i++)
			{
				// Instruction address 0x0000:0x06bd, size: 5
				if (this.parent.Segment_1ade.F0_1ade_22b5_PlayerHasTechnology(this.parent.GameData.HumanPlayerID, (TechnologyAdvanceEnum)i))
				{
					// Instruction address 0x0000:0x06d7, size: 5
					this.parent.CAPI.strcat(0xba06, this.parent.GameData.TechnologyAdvances[i].Name);

					// Instruction address 0x0000:0x06e7, size: 5
					this.parent.CAPI.strcat(0xba06, ", ");

					local_2++;

					if (local_2 == 2)
					{
						// Instruction address 0x0000:0x0700, size: 5
						this.parent.CAPI.strcat(0xba06, "\n");
					}
				}
			}

			// Instruction address 0x0000:0x0719, size: 5
			this.parent.CAPI.strcat(0xba06, "and Roads.\n");

			// Instruction address 0x0000:0x0738, size: 5
			this.parent.CommonTools.F0_1000_0a32_PlayTune(
				this.parent.GameData.Nations[this.parent.GameData.Players[this.parent.GameData.HumanPlayerID].NationalityID].LongTune, 3);

			// Instruction address 0x0000:0x0740, size: 5
			this.parent.MainCode.F0_11a8_0268_HideMouse();

			// Instruction address 0x0000:0x0759, size: 5
			this.parent.CommonTools.F0_1000_0bfa_FillRectangle(this.parent.Var_aa_Screen0_Rectangle, 0, 0, 320, 200, 15);

			// Instruction address 0x0000:0x0761, size: 5
			this.parent.UnitManagement.F0_1866_260e();

			if ((this.parent.GameData.DifficultyLevel & 0x1) != 0)
			{
				// Instruction address 0x0000:0x0799, size: 5
				this.parent.Graphics.F0_VGA_07d8_DrawImage(this.parent.Var_19d4_Screen1_Rectangle,
					80, (35 * this.parent.GameData.DifficultyLevel) + 6, 53, 47, this.parent.Var_aa_Screen0_Rectangle, 134, 20);
			}
			else
			{
				// Instruction address 0x0000:0x0799, size: 5
				this.parent.Graphics.F0_VGA_07d8_DrawImage(this.parent.Var_19d4_Screen1_Rectangle,
					21, (35 * this.parent.GameData.DifficultyLevel) + 6, 53, 47, this.parent.Var_aa_Screen0_Rectangle, 134, 20);
			}

			// Init text fix, we don't want to cut lines here
			// Instruction address 0x0000:0x07b0, size: 5
			this.parent.DrawTools.DrawTextBlock(88, 81, this.CPU.ReadString(this.CPU.DS.UInt16, 0xba06).Replace('^', '\n'), 150, 0);

			// Instruction address 0x0000:0x07b8, size: 5
			this.parent.MainCode.F0_11a8_0250_ShowMouse();

			// Instruction address 0x0000:0x07bd, size: 5
			this.parent.Segment_2459.F0_2459_0918_WaitForKeyPressOrMouseClick();
		}

		/// <summary>
		/// ?
		/// </summary>
		/// <param name="playerID"></param>
		/// <returns></returns>
		public int F5_0000_07c7(int playerID)
		{
			//this.oCPU.Log.EnterBlock($"F5_0000_07c7({playerID})");

			// function body
			int[] cityCount = new int[16];

			if (playerID != this.parent.GameData.HumanPlayerID || this.parent.GameData.TurnCount == 0)
			{
				this.parent.GameData.Players[playerID].SpaceshipPopulation = 0;
				this.parent.GameData.Players[playerID].CumulativeEpicRanking = 0;
				this.parent.GameData.Players[playerID].DiscoveredTechnologyCount = 0;
				this.parent.GameData.Players[playerID].ResearchProgress = 0;
				this.parent.GameData.Players[playerID].Coins = 0;
				this.parent.GameData.Players[playerID].GovernmentType = 1;
				this.parent.GameData.Players[playerID].TaxRate = 4;
				this.parent.GameData.Players[playerID].ScienceTaxRate = 4;

				for (int i = 0; i < 16; i++)
				{
					this.parent.GameData.Players[playerID].Continents[i].Strategy = 0;
					cityCount[i] = 0;

					for (int j = 1; j < 8; j++)
					{
						cityCount[i] += this.parent.GameData.Players[j].Continents[i].CityCount;
					}
				}

				this.parent.GameData.Players[playerID].CityCount = 0;
				this.parent.GameData.Players[playerID].UnitCount = 0;
				this.parent.GameData.Players[playerID].LandCount = 1;
				this.parent.GameData.Players[playerID].SettlerCount = 1;
				this.parent.GameData.Players[playerID].ContactPlayerCountdown = -1;

				for (int i = 0; i < this.parent.GameData.Players[playerID].TechnologyAcquiredFrom.Length; i++)
				{
					this.parent.GameData.Players[playerID].TechnologyAcquiredFrom[i] = -1;
				}

				for (int i = 0; i < 128; i++)
				{
					this.parent.GameData.Players[playerID].Units[i].TypeID = -1;
					this.parent.GameData.Players[playerID].Units[i].Status = 0;
				}

				this.parent.GameData.Players[playerID].Score = 0;

				if (playerID == this.parent.GameData.HumanPlayerID)
				{
					// Instruction address 0x0000:0x08f4, size: 5
					this.parent.CommonTools.F0_1000_0bfa_FillRectangle(this.parent.Var_19e8_Screen2_Rectangle, 240, 0, 80, 50, 0);
				}

				// Instruction address 0x0000:0x08ff, size: 5
				this.parent.Segment_25fb.F0_25fb_3223_ClearStrategicLocations(playerID);

				for (int i = 0; i < 8; i++)
				{
					this.parent.GameData.Players[playerID].Diplomacy[i] = 0;
					this.parent.GameData.Players[i].Diplomacy[playerID] = 0;
				}

				for (int i = 0; i < 5; i++)
				{
					this.parent.GameData.Players[playerID].DiscoveredTechnologyFlags[i] = 0;
				}

				if (this.parent.GameData.TurnCount != 0)
				{
					for (int i = 0; i < 72; i++)
					{
						// Instruction address 0x0000:0x0967, size: 5
						if (this.parent.Segment_1ade.F0_1ade_22b5_PlayerHasTechnology(this.parent.GameData.HumanPlayerID, (TechnologyAdvanceEnum)i) &&
							(this.parent.GameData.TechnologyFirstDiscoveredBy[i] & 0x7) != 0 &&
							(this.parent.GameData.TechnologyFirstDiscoveredBy[i] & 0x7) != this.parent.GameData.HumanPlayerID &&
							this.parent.CAPI.RNG.Next(2) == 0)
						{
							// Instruction address 0x0000:0x09b2, size: 5
							this.parent.Segment_1ade.F0_1ade_1d2e(playerID, (TechnologyAdvanceEnum)i, playerID);
						}
					}
				}

				if (playerID != 0 && (this.parent.GameData.ActiveCivilizations & (0x1 << playerID)) != 0)
				{
					if (playerID != this.parent.GameData.HumanPlayerID)
					{
						if (this.parent.GameData.TurnCount == 0)
						{
							bool flag;

							do
							{
								// Instruction address 0x0000:0x09f9, size: 5
								this.parent.GameData.Players[playerID].NationalityID = (short)this.parent.CAPI.RNG.Next(16);
								flag = true;

								if ((this.parent.GameData.Players[playerID].NationalityID & 7) != playerID)
								{
									flag = false;
								}

								for (int i = 0; i < 8; i++)
								{
									if (i != playerID && this.parent.GameData.Players[i].NationalityID == this.parent.GameData.Players[playerID].NationalityID)
									{
										flag = false;
									}
								}
							}
							while (!flag);
						}
						else
						{
							this.parent.GameData.Players[playerID].NationalityID ^= 8;
						}
					}

					int xStart;
					int yStart;
					int buildScore;
					int tryCount = 0;

					// pick up a suitable starting position for a first unit
				L0a5e:
					// Instruction address 0x0000:0x0a62, size: 5
					xStart = this.parent.CAPI.RNG.Next(64) + 8;
					yStart = this.parent.CAPI.RNG.Next(34) + 8;

					if (this.parent.MapManagement.GetTerrainType(xStart, yStart) == TerrainTypeEnum.Water ||
						this.parent.MapManagement.F0_2aea_1894_CellHasMinorTribeHut(xStart, yStart, this.parent.MapManagement.GetTerrainType(xStart, yStart))) goto L0a5e;

					int distance;
					int cityID = this.parent.GameTools.F0_2dc4_0102_FindNearestCity(xStart, yStart);

					if (cityID == -1)
					{
						distance = 30;
					}
					else
					{
						// Instruction address 0x0000:0x0abf, size: 5
						distance = this.parent.GameTools.F0_2dc4_0289_GetShortestDistance(xStart, yStart,
							this.parent.GameData.Cities[cityID].Position.X, this.parent.GameData.Cities[cityID].Position.Y);
					}

					if (this.parent.GameData.TurnCount == 0)
					{
						for (int j = 1; j < playerID; j++)
						{
							// Instruction address 0x0000:0x0af2, size: 5
							int newDistance = this.parent.GameTools.F0_2dc4_0289_GetShortestDistance(xStart, yStart,
								this.parent.GameData.Players[j].Units[0].Position.X, this.parent.GameData.Players[j].Units[0].Position.Y);

							if (newDistance < distance)
							{
								distance = newDistance;
							}
						}
					}

					// Instruction address 0x0000:0x0b24, size: 5
					buildScore = this.parent.MapManagement.GetBuildLocationScore(xStart, yStart);

					tryCount++;

					if (tryCount <= 2000 || (playerID == this.parent.GameData.HumanPlayerID && this.parent.GameData.TurnCount == 0))
					{
						// Instruction address 0x0000:0x0b54, size: 5
						if ((12 - (tryCount / 32)) > buildScore || distance < (10 - (tryCount / 64)) || 
							this.parent.GameData.Continents[this.parent.MapManagement.F0_2aea_1942_GetGroupID(xStart, yStart)].BuildSiteCount < (32 - (this.parent.GameData.TurnCount / 32) + (tryCount / 64)) ||
							(this.parent.GameData.Year > 0 && cityCount[this.parent.MapManagement.F0_2aea_1942_GetGroupID(xStart, yStart)] != 0)) goto L0a5e;
					}

					if (this.parent.GameData.TurnCount == 0 && this.parent.Var_d76a_EarthMap)
					{
						xStart = this.parent.Array_35da[this.parent.GameData.Players[playerID].NationalityID].X;
						yStart = this.parent.Array_35da[this.parent.GameData.Players[playerID].NationalityID].Y;
						tryCount = 0;
					}

					if (this.parent.GameData.TurnCount != 0 &&
						((this.parent.GameData.Players[playerID].NationalityID > 7 && (this.parent.GameData.PlayerIdentityFlags & (0x1 << playerID)) != 0) ||
							(this.parent.GameData.Players[playerID].NationalityID < 8 && (this.parent.GameData.PlayerIdentityFlags & (0x1 << playerID)) == 0)))
					{
						tryCount = 2000;
					}

					if (playerID != this.parent.GameData.HumanPlayerID)
					{
						this.parent.GameData.Players[playerID].ScienceTaxRate = (short)(this.parent.GameData.Nations[this.parent.GameData.Players[playerID].NationalityID].Ideology + 3);
						this.parent.GameData.Players[playerID].TaxRate = (short)(9 - this.parent.GameData.Players[playerID].ScienceTaxRate);
					}

					if ((-((50 * this.parent.GameData.DifficultyLevel) - 350) < this.parent.GameData.TurnCount || tryCount >= 2000) &&
						playerID != this.parent.GameData.HumanPlayerID)
					{
						this.parent.GameData.Players[playerID].NationalityID ^= 8;
						this.parent.GameData.ActiveCivilizations &= (short)(~(0x1 << playerID));

						if (((0x1 << this.parent.GameData.HumanPlayerID) | 0x1) == this.parent.GameData.ActiveCivilizations)
						{
							this.parent.Var_dc48_GameEndType = 1;
							this.parent.Var_b884 = 1;
						}

						return 0;
					}

					// Instruction address 0x0000:0x0d26, size: 5
					this.parent.GameData.Players[playerID].Name = this.parent.GameData.Nations[this.parent.GameData.Players[playerID].NationalityID].Leader;

					if (playerID != this.parent.GameData.HumanPlayerID || string.IsNullOrEmpty(this.parent.GameData.Players[playerID].Nationality))
					{
						// Instruction address 0x0000:0x0d53, size: 5
						this.parent.GameData.Players[playerID].Nationality = this.parent.GameData.Nations[this.parent.GameData.Players[playerID].NationalityID].Nationality;

						// Instruction address 0x0000:0x0d6a, size: 5
						this.parent.CAPI.strcpy(0xba06, this.parent.GameData.Nations[this.parent.GameData.Players[playerID].NationalityID].Nation);

						if (this.CPU.ReadUInt8(this.CPU.DS.UInt16, 0xba06) == 0)
						{
							// Instruction address 0x0000:0x0d81, size: 5
							this.parent.CAPI.strcpy(0xba06, this.parent.GameData.Players[playerID].Nationality);

							// Instruction address 0x0000:0x0d91, size: 5
							this.parent.CAPI.strcat(0xba06, "s");
						}

						// Instruction address 0x0000:0x0da6, size: 5
						this.parent.GameData.Players[playerID].Nation = this.CPU.ReadString(this.CPU.DS.UInt16, 0xba06);
					}

					// Instruction address 0x0000:0x0db7, size: 5
					this.parent.MapManagement.F0_2aea_138c_SetCityOwner(xStart, yStart, playerID);

					// Instruction address 0x0000:0x0dcb, size: 5
					this.parent.UnitManagement.F0_1866_0cf5_CreateUnit(playerID, 0, xStart, yStart);

					this.parent.GameData.Players[playerID].XStart = (short)xStart;

					for (int i = 0; i < 128; i++)
					{
						if (this.parent.GameData.Players[0].Units[i].TypeID != -1 &&
							this.parent.GameTools.F0_2dc4_0289_GetShortestDistance(xStart, yStart,
								this.parent.GameData.Players[0].Units[i].Position.X, this.parent.GameData.Players[0].Units[i].Position.Y) < 9)
						{
							// Instruction address 0x0000:0x0e18, size: 5
							this.parent.UnitManagement.F0_1866_0f10_DeleteUnit(0, i);
						}
					}

					this.parent.GameData.Players[playerID].DiscoveredTechnologyCount = 1;
				}

				for (int i = 0; i < 28; i++)
				{
					this.parent.GameData.Players[playerID].LostUnits[i] = 0;
					this.parent.GameData.Players[playerID].ActiveUnits[i] = 0;
					this.parent.GameData.Players[playerID].UnitsInProduction[i] = 0;
				}
				return 1;
			}

			return 0;
		}

		/// <summary>
		/// This function tests if a AI player has no more cities. 
		/// If the city count is zero the player is marked as destroyed and all units that belong to the AI player are deleted.
		/// The function doesn't take into account settlers which could respawn new cities.
		/// </summary>
		/// <param name="playerID">AI player to test</param>
		/// <param name="playerID1">The player that destroyed the last city</param>
		public void F5_0000_0e6c_TestIfAIPlayerDestroyed(int playerID, int playerID1)
		{
			//this.oCPU.Log.EnterBlock($"F5_0000_0e6c({playerID}, {playerID1})");

			// function body
			if (playerID != 0 && playerID != this.parent.GameData.HumanPlayerID)
			{
				int cityCount = 0;

				for (int i = 0; i < 128; i++)
				{
					if (this.parent.GameData.Cities[i].StatusFlag != 0xff && this.parent.GameData.Cities[i].PlayerID == playerID)
					{
						cityCount++;
					}
				}

				if (cityCount == 0)
				{
					// Instruction address 0x0000:0x0ec1, size: 5
					this.parent.CAPI.strcpy(0xba06, this.parent.GameData.Players[playerID].Nationality);

					// Instruction address 0x0000:0x0ed1, size: 5
					this.parent.CAPI.strcat(0xba06, "\ncivilization\ndestroyed\nby ");

					// Instruction address 0x0000:0x0ee6, size: 5
					this.parent.CAPI.strcat(0xba06, this.parent.GameData.Players[playerID1].Nation);

					// Instruction address 0x0000:0x0ef6, size: 5
					this.parent.CAPI.strcat(0xba06, "!\n");

					if ((this.parent.GameData.Players[this.parent.GameData.HumanPlayerID].Diplomacy[playerID] & 0x40) != 0)
					{
						this.parent.Var_2f9e_MessageBoxStyle = MenuBoxReportTypeEnum.ForeignMinisterReport;
					}
					else
					{
						this.parent.Var_2f9e_MessageBoxStyle = MenuBoxReportTypeEnum.TravelersReport;
					}


					if (playerID1 == this.parent.GameData.HumanPlayerID)
					{
						this.parent.Var_2f9e_MessageBoxStyle = MenuBoxReportTypeEnum.DefenseMinisterReport;
					}

					// Instruction address 0x0000:0x0f32, size: 5
					this.parent.Segment_1238.F0_1238_001e_ShowDialog(0xba06, 100, 80);

					// Instruction address 0x0000:0x0f3d, size: 5
					this.parent.Segment_2459.F0_2459_05ee(playerID);

					// Instruction address 0x0000:0x0f53, size: 5
					this.parent.UnitManagement.F0_1866_250e_AddReplayData(13, (byte)playerID, (byte)playerID1);

					for (int i = 0; i < 128; i++)
					{
						if (this.parent.GameData.Players[playerID].Units[i].TypeID != -1)
						{
							// Instruction address 0x0000:0x0f7d, size: 5
							this.parent.UnitManagement.F0_1866_0f10_DeleteUnit(playerID, i);
						}
					}

					ushort visibilityMask = (ushort)(~(0x1 << playerID));

					for (int j = 0; j < 80; j++)
					{
						for (int k = 0; k < 50; k++)
						{
							this.parent.GameData.MapVisibility[j, k] &= visibilityMask;
						}
					}

					F5_0000_07c7(playerID);
				}
			}
		}

		/// <summary>
		/// ?
		/// </summary>
		/// <returns></returns>
		/*private int F5_0000_0fe6()
		{
			//this.oCPU.Log.EnterBlock("F5_0000_0fe6()");

			// function body
			int[] local_1c = new int[8];
			int nearestPlayers = 0;
			int local_24 = 0;

			for (int i = 1; i < 8; i++)
			{
				if (this.oParent.GameData.Players[i].Units[0].TypeID == -1)
					break;

				int local_6 = 0;
				int shortestDistance = 999;

				for (int j = 1; j < 8; j++)
				{
					if (j != i && this.oParent.GameData.Players[j].Units[0].TypeID != -1 &&
						this.oParent.MapManagement.F0_2aea_1942_GetGroupID(this.oParent.GameData.Players[i].Units[0].Position.X,
						this.oParent.GameData.Players[i].Units[0].Position.Y) == this.oParent.MapManagement.F0_2aea_1942_GetGroupID(
						this.oParent.GameData.Players[j].Units[0].Position.X,
						this.oParent.GameData.Players[j].Units[0].Position.Y))
					{
						// Instruction address 0x0000:0x12ae, size: 5
						int distance = this.oParent.Segment_2dc4.F0_2dc4_0289_GetShortestDistance(
							this.oParent.GameData.Players[i].Units[0].Position.X, this.oParent.GameData.Players[i].Units[0].Position.Y,
							this.oParent.GameData.Players[j].Units[0].Position.X, this.oParent.GameData.Players[j].Units[0].Position.Y);

						if (distance < shortestDistance)
						{
							shortestDistance = distance;
						}
					}
				}

				if (shortestDistance >= 10 && shortestDistance != 999)
				{
					nearestPlayers |= (0x1 << i);
				}

				// Instruction address 0x0000:0x12f3, size: 5
				this.oParent.CAPI.strcpy(0xba06, "Nearest civilization: ");

				if (shortestDistance < 10)
				{
					// Instruction address 0x0000:0x130c, size: 5
					this.oParent.CAPI.strcat(0xba06, "close");
				}
				else if (shortestDistance < 20)
				{
					// Instruction address 0x0000:0x1077, size: 5
					this.oParent.CAPI.strcat(0xba06, "moderate");

					local_6++;
				}
				else if (shortestDistance != 999)
				{
					// Instruction address 0x0000:0x1093, size: 5
					this.oParent.CAPI.strcat(0xba06, "far away");

					local_6 += 2;
				}
				else
				{
					// Instruction address 0x0000:0x10a9, size: 5
					this.oParent.CAPI.strcat(0xba06, "isolated");

					local_6 += 4;
				}
				int waterCellCount = 0;
				int grasslandCellCount = 0;
				int riverCellCount = 0;

				for (int k = 0; k < 9; k++)
				{
					GPoint direction = this.oParent.MoveDirections[k];

					int newX = this.oParent.MapManagement.AdjustXPosition(this.oParent.GameData.Players[i].Units[0].Position.X + direction.X);
					int newY = this.oParent.GameData.Players[i].Units[0].Position.Y + direction.Y;

					if (this.oParent.MapManagement.F0_2aea_1326_ValidateMapCoordinates(newX, newY))
					{
						// Instruction address 0x0000:0x10ee, size: 5
						switch (this.oParent.MapManagement.GetTerrainType(newX, newY))
						{
							case TerrainTypeEnum.Water:
								waterCellCount++;
								break;

							case TerrainTypeEnum.Grassland:
								grasslandCellCount++;
								break;

							case TerrainTypeEnum.River:
								riverCellCount++;
								break;
						}
					}
				}

				if (waterCellCount != 0)
				{
					// Instruction address 0x0000:0x112f, size: 5
					this.oParent.CAPI.strcpy(0xba06, "Coastal ");
				}
				else
				{
					// Instruction address 0x0000:0x112f, size: 5
					this.oParent.CAPI.strcpy(0xba06, "Inland ");
				}

				if (riverCellCount > 1)
				{
					// Instruction address 0x0000:0x1145, size: 5
					this.oParent.CAPI.strcat(0xba06, "river valley on a ");

					local_6 += 2;
				}
				else if (grasslandCellCount >= 3)
				{
					// Instruction address 0x0000:0x1161, size: 5
					this.oParent.CAPI.strcat(0xba06, "grasslands on a ");

					local_6++;
				}
				else
				{
					// Instruction address 0x0000:0x1176, size: 5
					this.oParent.CAPI.strcat(0xba06, "plains on a ");
				}

				// Instruction address 0x0000:0x119a, size: 5
				switch (this.oParent.MapManagement.F0_2aea_195d_GetMapGroupSize(
					this.oParent.GameData.Players[i].Units[0].Position.X,
					this.oParent.GameData.Players[i].Units[0].Position.Y) / 50)
				{
					case 0:
						// Instruction address 0x0000:0x11e2, size: 5
						this.oParent.CAPI.strcat(0xba06, "small island.");
						break;

					case 1:
						// Instruction address 0x0000:0x11e2, size: 5
						this.oParent.CAPI.strcat(0xba06, "large island.");
						break;

					case 2:
						// Instruction address 0x0000:0x1325, size: 5
						this.oParent.CAPI.strcat(0xba06, "small continent.");
						local_6++;
						break;

					case 3:
						// Instruction address 0x0000:0x1325, size: 5
						this.oParent.CAPI.strcat(0xba06, "medium continent.");
						local_6++;
						break;

					default:
						// Instruction address 0x0000:0x11cc, size: 5
						this.oParent.CAPI.strcat(0xba06, "large continent.");
						local_6 += 2;
						break;
				}

				if (local_6 > local_24)
				{
					local_24 = local_6;
				}

				local_1c[i] = local_6;
			}

			int local_5c = -1;

			for (int i = 1; i < 8; i++)
			{
				if (this.oParent.GameData.Players[i].Units[0].TypeID != -1)
				{
					// Instruction address 0x0000:0x13fb, size: 5
					this.oParent.CAPI.strcpy(0xba06, "Skills: ");

					if (local_24 - local_1c[i] > 3)
					{
						local_1c[i] += 3;

						// Instruction address 0x0000:0x142c, size: 5
						this.oParent.UnitManagement.F0_1866_0cf5_CreateUnit(i, 0,
							this.oParent.GameData.Players[i].Units[0].Position.X, this.oParent.GameData.Players[i].Units[0].Position.Y);

						// Instruction address 0x0000:0x143c, size: 5
						this.oParent.CAPI.strcat(0xba06, "Settlers+");
					}

					int local_48 = 0;

					while (local_24 - local_1c[i] > local_48)
					{
						if (local_48 != 0)
						{
							// Instruction address 0x0000:0x1353, size: 5
							this.oParent.CAPI.strcat(0xba06, "+");
						}

						// Instruction address 0x0000:0x1361, size: 5
						// Instruction address 0x0000:0x1376, size: 5
						this.oParent.CAPI.strcat(0xba06, this.oParent.GameData.Technologies[(short)this.oParent.Segment_1ade.F0_1ade_1584(i, 0)].Name);

						local_48++;
					}

					if (local_1c[i] == local_24)
					{
						// Instruction address 0x0000:0x13a6, size: 5
						this.oParent.CAPI.strcat(0xba06, "NONE");

						if (local_5c == -1 || (nearestPlayers & (0x1 << i)) != 0)
						{
							local_5c = i;
						}
					}
				}
			}

			return local_5c;
		}*/

		/// <summary>
		/// This function loads all game bitmaps
		/// </summary>
		public void F5_0000_1455_LoadBitmaps()
		{
			//this.oCPU.Log.EnterBlock("F5_0000_1455()");

			// function body
			// Instruction address 0x0000:0x147c, size: 5
			this.parent.ImageTools.F0_2fa1_01a2_LoadBitmapOrPalette(1, 0, 0, "SP299.PIC", 0);

			// Instruction address 0x0000:0x14a4, size: 5
			this.parent.Graphics.F0_VGA_07d8_DrawImage(this.parent.Var_19d4_Screen1_Rectangle, 160, 50, 160, 150, this.parent.Var_19e8_Screen2_Rectangle, 160, 50);

			for (int i = 0; i < 3; i++)
			{
				// Instruction address 0x0000:0x14cb, size: 5
				this.parent.Array_df62[i] = this.parent.Graphics.F0_VGA_0b85_ScreenToBitmap(1, (42 * i) + 160, 142, 41, 58);
			}

			// Instruction address 0x0000:0x14f7, size: 5
			this.parent.ImageTools.F0_2fa1_01a2_LoadBitmapOrPalette(1, 0, 0, "TER257.PIC", 0);

			for (int i = 0; i < 10; i++)
			{
				for (int j = 0; j < 16; j++)
				{
					if ((i >= 6 || this.parent.Var_1a3e != 0) && j != 0)
					{
						this.parent.Array_b886[i, j] = this.parent.Array_b886[i, j - 1];
					}
					else
					{
						// Instruction address 0x0000:0x154f, size: 5
						this.parent.Array_b886[i, j] = this.parent.Graphics.F0_VGA_0b85_ScreenToBitmap(1, j * 16, i * 16, 16, 16);
					}
				}
			}

			for (int i = 0; i < 8; i++)
			{
				for (int j = 0; j < 4; j++)
				{
					if (j < 2)
					{
						// Instruction address 0x0000:0x15a4, size: 5
						this.parent.Array_d294[i, j] = this.parent.Graphics.F0_VGA_0b85_ScreenToBitmap(1, (i * 16) + ((j & 1) * 8), 176, 8, 8);
					}
					else
					{
						// Instruction address 0x0000:0x15a4, size: 5
						this.parent.Array_d294[i, j] = this.parent.Graphics.F0_VGA_0b85_ScreenToBitmap(1, (i * 16) - ((j & 1) * 8) + 8, 184, 8, 8);
					}
				}
			}

			for (int i = 0; i < 4; i++)
			{
				// Instruction address 0x0000:0x1624, size: 5
				this.parent.Array_d2d4[i] = this.parent.Graphics.F0_VGA_0b85_ScreenToBitmap(1, (i * 16) + 128, 176, 16, 16);
			}

			// Instruction address 0x0000:0x16c2, size: 5
			this.parent.ImageTools.F0_2fa1_01a2_LoadBitmapOrPalette(1, 0, 0, "SP257.PIC", 0);

			byte[] palette;

			// Instruction address 0x0000:0x16d2, size: 5
			this.parent.ImageTools.F0_2fa1_01a2_LoadBitmapOrPalette(-1, 0, 0, "SP256.PAL", out palette); // 0xbdee

			if (palette.Length > 53)
			{
				for (int i = 0; i < 48; i++)
				{
					this.parent.Var_6b34[i] = palette[i + 6]; // 0xbdf4 (0xbdee + 6)
				}
			}

			for (int i = 0; i < 8; i++)
			{
				// Instruction address 0x0000:0x1714, size: 5
				this.parent.Array_b27a[i] = this.parent.Graphics.F0_VGA_0b85_ScreenToBitmap(1, i * 16, 48, 16, 16);

				// Instruction address 0x0000:0x1732, size: 5
				this.parent.Array_b29a[i] = this.parent.Graphics.F0_VGA_0b85_ScreenToBitmap(1, i * 16 + 128, 96, 16, 16);
			}

			// Instruction address 0x0000:0x1758, size: 5
			this.parent.Var_b2ba = this.parent.Graphics.F0_VGA_0b85_ScreenToBitmap(1, 48, 32, 16, 16);

			for (int i = 1; i < 16; i++)
			{
				// Instruction address 0x0000:0x1782, size: 5
				this.parent.Array_6e00[i - 1] = this.parent.Graphics.F0_VGA_0b85_ScreenToBitmap(1, i * 16, 64, 16, 16);

				// Instruction address 0x0000:0x179c, size: 5
				this.parent.Array_6e00[i + 15] = this.parent.Graphics.F0_VGA_0b85_ScreenToBitmap(1, i * 16, 80, 16, 16);
			}

			for (int i = 0; i < 20; i++)
			{
				// Instruction address 0x0000:0x17d7, size: 5
				this.parent.Array_d21c[0, i] = this.parent.Graphics.F0_VGA_0b85_ScreenToBitmap(1, 7 * i + 160, 0, 7, 15); // 0xd21c

				// Instruction address 0x0000:0x17f4, size: 5
				this.parent.Array_d21c[1, i] = this.parent.Graphics.F0_VGA_0b85_ScreenToBitmap(1, 7 * i + 160, 16, 7, 15); // 0xd244

				// Instruction address 0x0000:0x1811, size: 5
				this.parent.Array_d21c[2,i] = this.parent.Graphics.F0_VGA_0b85_ScreenToBitmap(1, 7 * i + 160, 32, 7, 15); // 0xd26c
			}

			for (int i = 0; i < 4; i++)
			{
				// Instruction address 0x0000:0x1843, size: 5
				this.parent.Array_7eec[i] = this.parent.Graphics.F0_VGA_0b85_ScreenToBitmap(1, i * 16 + 80, 128, 16, 16);
			}

			for (int i = 0; i < 7; i++)
			{
				// Instruction address 0x0000:0x1877, size: 5
				this.parent.Array_d4ce[i] = this.parent.Graphics.F0_VGA_0b85_ScreenToBitmap(1, i * 16, 32, 16, 16);
			}

			this.parent.Array_d4ce[7] = this.parent.Var_6e92_MouseIconHandle;

			for (int i = 0; i < 8; i++)
			{
				// Instruction address 0x0000:0x18bf, size: 5
				this.parent.Array_d4ce[8 + i] = this.parent.Graphics.F0_VGA_0b85_ScreenToBitmap(1, (i & 3) * 8 + 129, (i & 4) * 2 + 33, 7, 7);
			}

			for (int i = 0; i < 9; i++)
			{
				// Instruction address 0x0000:0x18f6, size: 5
				this.parent.Array_6e96[i] = this.parent.Graphics.F0_VGA_0b85_ScreenToBitmap(1, i * 8, 128, 8, 16);
			}

			for (int i = 0; i < 16; i++)
			{
				// Instruction address 0x0000:0x192b, size: 5
				this.parent.Array_d4ce[16 + i] = this.parent.Graphics.F0_VGA_0b85_ScreenToBitmap(1, (i * 16) + 1, 113, 15, 15);
			}

			for (int i = 0; i < 8; i++)
			{
				// Instruction address 0x0000:0x1960, size: 5
				this.parent.Array_d4ce[32 + i] = this.parent.Graphics.F0_VGA_0b85_ScreenToBitmap(1, (i * 16) + 1, 97, 15, 15);
			}

			// Instruction address 0x0000:0x198b, size: 5
			this.parent.Var_b880 = this.parent.Graphics.F0_VGA_0b85_ScreenToBitmap(1, 153, 41, 7, 7);
			
			// Instruction address 0x0000:0x19aa, size: 5
			this.parent.Var_df0c = this.parent.Graphics.F0_VGA_0b85_ScreenToBitmap(1, 176, 128, 20, 9);

			for (int i = 0; i < 8; i++)
			{
				// Instruction address 0x0000:0x1a32, size: 5
				this.parent.Graphics.F0_VGA_07d8_DrawImage(this.parent.Var_19d4_Screen1_Rectangle, 0, 160, 320, 32, this.parent.Var_19d4_Screen1_Rectangle, 0, 128);

				if (this.parent.Array_1946[i] == 15)
				{
					// Instruction address 0x0000:0x1a61, size: 5
					this.parent.Graphics.F0_VGA_009a_ReplaceColor(this.parent.Var_19d4_Screen1_Rectangle, 0, 128, 320, 32, 15, 11);
				}

				if (this.parent.Array_1946[i] == 7)
				{
					// Instruction address 0x0000:0x1a90, size: 5
					this.parent.Graphics.F0_VGA_009a_ReplaceColor(this.parent.Var_19d4_Screen1_Rectangle, 0, 128, 320, 32, 7, 3);
				}

				// Instruction address 0x0000:0x1ab8, size: 5
				this.parent.Graphics.F0_VGA_009a_ReplaceColor(this.parent.Var_19d4_Screen1_Rectangle, 0, 128, 320, 32, 10, this.parent.Array_1946[i]);

				// Instruction address 0x0000:0x1adb, size: 5
				this.parent.Graphics.F0_VGA_009a_ReplaceColor(this.parent.Var_19d4_Screen1_Rectangle, 0, 128, 320, 32, 2, this.parent.Array_1956[i]);

				for (int j = 0; j < 28; j++)
				{
					// Instruction address 0x0000:0x19ee, size: 5
					this.parent.Array_d4ce[64 + (i * 32 + j)] = this.parent.Graphics.F0_VGA_0b85_ScreenToBitmap(1, (j % 20) * 16 + 1, ((j / 20) * 16) + 129, 15, 15);
				}
			}
		}

		/// <summary>
		/// ?
		/// </summary>
		public void F5_0000_1af6_LoadGovernmentImage()
		{
			//this.oCPU.Log.EnterBlock("F5_0000_1af6()");

			// function body
			// Instruction address 0x0000:0x1b17, size: 5
			int governmentTypeID = this.parent.GameData.Players[this.parent.GameData.HumanPlayerID].GovernmentType / 2;
			bool ancientGovernment = true;

			if (this.parent.GameData.Players[this.parent.GameData.HumanPlayerID].GovernmentType == 3)
			{
				governmentTypeID = 3;
			}
			else
			{
				// Instruction address 0x0000:0x1b48, size: 5
				if (this.parent.Segment_1ade.F0_1ade_22b5_PlayerHasTechnology(parent.GameData.HumanPlayerID, TechnologyAdvanceEnum.Invention))
				{
					ancientGovernment = false;
				}
			}

			// Instruction address 0x0000:0x1b60, size: 5
			this.parent.ImageTools.F0_2fa1_01a2_LoadBitmapOrPalette(1, 0, 0, $"GOVT{governmentTypeID}{(ancientGovernment ? 'A' : 'M')}.PIC", 0);

			// Instruction address 0x0000:0x1b84, size: 5
			this.parent.Graphics.F0_VGA_07d8_DrawImage(this.parent.Var_19d4_Screen1_Rectangle, 0, 0, 160, 60, this.parent.Var_19e8_Screen2_Rectangle, 160, 140);
		}

		/// <summary>
		/// ?
		/// </summary>
		public void F5_0000_1ba2_ChangeGovernment()
		{
			//this.oCPU.Log.EnterBlock("F5_0000_1ba2()");

			// function body
			if (this.parent.GameData.Players[this.parent.GameData.HumanPlayerID].GovernmentType != 0)
			{
				// Instruction address 0x0000:0x1bb9, size: 5
				this.parent.Graphics.F0_VGA_07d8_DrawImage(this.parent.Var_aa_Screen0_Rectangle, 0, 0, 320, 200, this.parent.Var_19d4_Screen1_Rectangle, 0, 0);

				this.parent.News.F21_0000_0000_ShowNews(-2,
					$"{this.parent.GameData.Players[this.parent.GameData.HumanPlayerID].Nationality} government\nchanged to " +
					$"{this.parent.Array_1966[this.parent.GameData.Players[this.parent.GameData.HumanPlayerID].GovernmentType]}!\n");

				// Instruction address 0x0000:0x1c38, size: 5
				this.parent.CommonTools.F0_1000_0bfa_FillRectangle(this.parent.Var_aa_Screen0_Rectangle, 0, 100, 320, 100, 15);

				this.parent.Var_aa_Screen0_Rectangle.FontID = 6;

				// Instruction address 0x0000:0x1c58, size: 5
				this.parent.DrawTools.F0_1182_00b3_DrawCenteredStringToScreen0("New Cabinet:", 160, 102, 0);

				this.parent.Var_aa_Screen0_Rectangle.FontID = 2;

				for (int i = 0; i < 4; i++)
				{
					// Instruction address 0x0000:0x1cbb, size: 5
					this.parent.Graphics.F0_VGA_07d8_DrawImage(this.parent.Var_19e8_Screen2_Rectangle, (40 * i) + 160, 140, 40, 60,
						this.parent.Var_aa_Screen0_Rectangle, (80 * i) + 20, 118);

					// Instruction address 0x0000:0x1cd0, size: 5
					this.parent.CAPI.strcpy(0xba06, this.parent.Array_2fac[i]);

					// Instruction address 0x0000:0x1cdc, size: 5
					this.CPU.WriteUInt8(this.CPU.DS.UInt16, (ushort)(0xba06 + this.parent.CAPI.strlen(0xba06) - 1), 0x0);

					// Instruction address 0x0000:0x1c7c, size: 5
					this.parent.DrawTools.F0_1182_00b3_DrawCenteredStringToScreen0(0xba06, (80 * i) + 40, ((i & 1) != 0) ? 186 : 180, 0);
				}

				this.parent.Var_aa_Screen0_Rectangle.FontID = 1;

				// Instruction address 0x0000:0x1d06, size: 5
				this.parent.Graphics.F0_VGA_07d8_DrawImage(this.parent.Var_19d4_Screen1_Rectangle, 0, 0, 320, 200, this.parent.Var_aa_Screen0_Rectangle, 0, 0);

				// Instruction address 0x0000:0x1d0b, size: 5
				this.parent.Segment_2459.F0_2459_0918_WaitForKeyPressOrMouseClick();
			}
		}

		/// <summary>
		/// ?
		/// </summary>
		/// <param name="playerID"></param>
		public void F5_0000_1d1a_InitSpaceshipData(int playerID)
		{
			//this.oCPU.Log.EnterBlock($"F5_0000_1d1a_InitSpaceshipData({playerID})");

			// function body
			this.parent.GameData.Players[playerID].SpaceshipETAYear = -1;

			for (int i = 0; i < 15; i++)
			{
				for (int j = 0; j < 12; j++)
				{
					this.parent.GameData.Players[playerID].SpaceshipData[(12 * i) + j] = -1;
				}
			}
		}
	}
}
