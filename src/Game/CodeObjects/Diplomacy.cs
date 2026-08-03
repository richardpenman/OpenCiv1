using Avalonia.Media.TextFormatting.Unicode;
using IRB.VirtualCPU;
using System.Text;

namespace OpenCivOne
{
	public class Diplomacy
	{
		private OpenCivOneGame parent;
		private VCPU oCPU;

		public Diplomacy(OpenCivOneGame parent)
		{
			this.parent = parent;
			this.oCPU = parent.CPU;
		}

		/// <summary>
		/// This function updates player government and diplomacy each turn
		/// </summary>
		/// <param name="playerID"></param>
		public void F0_2517_0004_UpdatePlayerGovernmentAndDiplomacy(int playerID)
		{
			//this.oCPU.Log.EnterBlock($"F0_2517_0004({playerID})");

			// function body
			if (this.parent.GameData.Players[playerID].GovernmentType == GovernmentTypeEnum.Anarchy)
			{
				if (playerID == this.parent.GameData.HumanPlayerID && (this.parent.GameData.TurnCount & 0x3) == 0)
				{
					if (this.parent.GameData.Players[playerID].Diplomacy[0].HasFlag(DiplomacyFlagsEnum.Allied))
					{
						this.parent.GameData.Players[playerID].Diplomacy[0] |= DiplomacyFlagsEnum.Allied;
						this.parent.GameData.Players[playerID].Diplomacy[0] ^= DiplomacyFlagsEnum.Allied;
					}
					else
					{
						// Instruction address 0x2517:0x0044, size: 3
						F0_2517_04a1_ChangeCurrentGovernment(playerID, GovernmentTypeEnum.Despotism);
					}
				}
				else
				{
					if (this.parent.CityWorker.F0_1d12_6c97_PlayerHasWonder(playerID, WonderEnum.Pyramids) || (this.parent.GameData.TurnCount & 0x3) == 0)
					{
						// Instruction address 0x2517:0x006e, size: 3
						F0_2517_04a1_ChangeCurrentGovernment(playerID, GovernmentTypeEnum.Despotism);

						if (playerID == this.parent.GameData.HumanPlayerID)
						{
							this.parent.GameData.Players[playerID].Diplomacy[0] |= DiplomacyFlagsEnum.Allied;
							this.parent.GameData.Players[playerID].Diplomacy[0] ^= DiplomacyFlagsEnum.Allied;
						}
					}
				}
			}

			if (playerID != this.parent.GameData.HumanPlayerID && this.parent.GameData.Players[playerID].Diplomacy[0].HasFlag(DiplomacyFlagsEnum.Allied))
			{
				// Instruction address 0x2517:0x00ab, size: 3
				F0_2517_04a1_ChangeCurrentGovernment(playerID, this.parent.GameData.Players[playerID].GovernmentType);
			}

			for (int i = 1; i < 8; i++)
			{
				if (this.parent.GameData.Players[playerID].Diplomacy[i].HasFlag(DiplomacyFlagsEnum.PeaceTreaty) && this.parent.CAPI.RNG.Next(3) == 0)
				{
					this.parent.GameData.Players[playerID].Diplomacy[i] |= DiplomacyFlagsEnum.PeaceTreaty | DiplomacyFlagsEnum.Peace;
					this.parent.GameData.Players[playerID].Diplomacy[i] ^= DiplomacyFlagsEnum.PeaceTreaty | DiplomacyFlagsEnum.Peace;
					this.parent.GameData.Players[playerID].Diplomacy[i] |= DiplomacyFlagsEnum.WarDeclared;
				}

				if ((this.parent.GameData.TurnCount & 0xf) == 0)
				{
					if (!this.parent.GameData.Players[playerID].Diplomacy[i].HasFlag(DiplomacyFlagsEnum.FormalContact))
					{
						if (this.parent.GameData.Players[playerID].Diplomacy[i].HasFlag(DiplomacyFlagsEnum.Contact) &&
							!this.parent.GameData.Players[playerID].Diplomacy[i].HasFlag(DiplomacyFlagsEnum.Peace))
						{
							if (this.parent.GameData.Players[this.parent.GameData.HumanPlayerID].Diplomacy[playerID].HasFlag(DiplomacyFlagsEnum.EmbassyEstablished) ||
								this.parent.GameData.Players[this.parent.GameData.HumanPlayerID].Diplomacy[i].HasFlag(DiplomacyFlagsEnum.EmbassyEstablished))
							{
								if (playerID != this.parent.GameData.HumanPlayerID && i != this.parent.GameData.HumanPlayerID)
								{
									this.parent.Var_2f9e_MessageBoxStyle = MenuBoxReportTypeEnum.DiplomatsReport;

									// Instruction address 0x2517:0x01ba, size: 5
									this.parent.Segment_1238.F0_1238_001e_ShowDialog($"The war between\nthe {this.parent.GameData.Players[playerID].Nation} and the\n" +
										$"{this.parent.GameData.Players[i].Nation} has ended.\n", 100, 80);
								}
							}
						}

						this.parent.GameData.Players[playerID].Diplomacy[i] |= DiplomacyFlagsEnum.Contact;
						this.parent.GameData.Players[playerID].Diplomacy[i] ^= DiplomacyFlagsEnum.Contact;
						this.parent.GameData.Players[i].Diplomacy[playerID] |= DiplomacyFlagsEnum.Contact;
						this.parent.GameData.Players[i].Diplomacy[playerID] ^= DiplomacyFlagsEnum.Contact;
					}

					this.parent.GameData.Players[playerID].Diplomacy[i] |= DiplomacyFlagsEnum.FormalContact;
					this.parent.GameData.Players[playerID].Diplomacy[i] ^= DiplomacyFlagsEnum.FormalContact;
					this.parent.GameData.Players[i].Diplomacy[playerID] |= DiplomacyFlagsEnum.FormalContact;
					this.parent.GameData.Players[i].Diplomacy[playerID] ^= DiplomacyFlagsEnum.FormalContact;
				}
			}

			if (playerID != 0 && (this.parent.GameData.TurnCount & 0x1f) == playerID * 4 && playerID != this.parent.GameData.HumanPlayerID)
			{
				for (int i = 1; i < 8; i++)
				{
					if (this.parent.GameData.Players[playerID].Diplomacy[i].HasFlag(DiplomacyFlagsEnum.EverFought) &&
						this.parent.GameData.Players[this.parent.GameData.HumanPlayerID].Diplomacy[i].HasFlag(DiplomacyFlagsEnum.Peace) &&
						!this.parent.GameData.Players[playerID].Diplomacy[this.parent.GameData.HumanPlayerID].HasFlag(DiplomacyFlagsEnum.EverFought))
					{
						if (this.parent.GameData.Players[playerID].MilitaryPower * 2 < this.parent.GameData.Players[i].MilitaryPower &&
							this.parent.MeetWithKing.F6_0000_16cd(playerID, -1, 0) == 0)
						{
							// Instruction address 0x2517:0x0355, size: 5
							int helpAmount = this.parent.Tools.CheckValueRange(this.parent.GameData.Players[playerID].Coins / 100, 1, 10) * 50;

							// Instruction address 0x2517:0x036d, size: 5
							this.parent.Array_30b8[1] = this.parent.GameData.Players[i].Nation;

							// Instruction address 0x2517:0x03a2, size: 5
							this.parent.Array_30b8[2] = $"{helpAmount}";

							this.parent.Var_3936 = 1;
							this.parent.MeetWithKing.Var_d76c = 12;

							if (this.parent.MeetWithKing.F6_0000_251d_ShowInlineDialog(this.parent.LanguageTools.F0_2f4d_044f_GetTextFromKingSection("*HELPME"), 36, 139) == 1)
							{
								this.parent.GameData.Players[this.parent.GameData.HumanPlayerID].Coins -= (short)helpAmount;
								this.parent.GameData.Players[playerID].Coins += (short)helpAmount;

								F0_2517_0a30_SetDiplomacyFlags(playerID, this.parent.GameData.HumanPlayerID, DiplomacyFlagsEnum.Peace);
								F0_2517_0aa1_ClearDiplomacyFlags(i, this.parent.GameData.HumanPlayerID, DiplomacyFlagsEnum.Peace);

								this.parent.GameData.Players[i].Diplomacy[this.parent.GameData.HumanPlayerID] |= DiplomacyFlagsEnum.Vendetta;
							}

							this.parent.MeetWithKing.F6_0000_16ac();
						}

						if (this.parent.GameData.Players[i].MilitaryPower > this.parent.GameData.Players[playerID].MilitaryPower &&
							this.parent.GameData.Players[playerID].MilitaryPower * 2 >= this.parent.GameData.Players[i].MilitaryPower &&
							this.parent.MeetWithKing.F6_0000_16cd(playerID, -1, 0) == 0)
						{
							// Instruction address 0x2517:0x0475, size: 5
							this.parent.Array_30b8[1] = this.parent.GameData.Players[i].Nation;

							if (!this.parent.GameData.Players[playerID].Diplomacy[this.parent.GameData.HumanPlayerID].HasFlag(DiplomacyFlagsEnum.Peace))
							{
								// Instruction address 0x2517:0x0248, size: 5
								this.parent.Array_30b8[3] = "agree to a";
							}
							else
							{
								// Instruction address 0x2517:0x0248, size: 5
								this.parent.Array_30b8[3] = "extend our";
							}

							this.parent.Var_3936 = 1;
							this.parent.MeetWithKing.Var_d76c = 12;

							if (this.parent.MeetWithKing.F6_0000_251d_ShowInlineDialog(this.parent.LanguageTools.F0_2f4d_044f_GetTextFromKingSection("*CRUSADE"), 36, 139) == 1)
							{
								// Instruction address 0x2517:0x028d, size: 3
								F0_2517_0a30_SetDiplomacyFlags(playerID, this.parent.GameData.HumanPlayerID, DiplomacyFlagsEnum.Peace);

								// Instruction address 0x2517:0x029f, size: 3
								F0_2517_0aa1_ClearDiplomacyFlags(i, this.parent.GameData.HumanPlayerID, DiplomacyFlagsEnum.Peace);

								this.parent.GameData.Players[i].Diplomacy[this.parent.GameData.HumanPlayerID] |= DiplomacyFlagsEnum.Vendetta;

								this.parent.GameData.Players[playerID].ContactPlayerCountdown = (short)(parent.GameData.TurnCount + 16);
							}

							this.parent.MeetWithKing.F6_0000_16ac();
						}
					}

					this.parent.GameData.Players[playerID].Diplomacy[i] |= DiplomacyFlagsEnum.EverFought;
					this.parent.GameData.Players[playerID].Diplomacy[i] ^= DiplomacyFlagsEnum.EverFought;
				}
			}
		}

		/// <summary>
		/// Changes current government to new government
		/// </summary>
		/// <param name="playerID"></param>
		/// <param name="newGovernmentType"></param>
		public void F0_2517_04a1_ChangeCurrentGovernment(int playerID, GovernmentTypeEnum newGovernmentType)
		{
			//this.oCPU.Log.EnterBlock($"F0_2517_04a1({playerID}, {governmentType})");

			// function body
			List<GovernmentTypeEnum> governments = new();
			GovernmentTypeEnum currentGovernmentType = this.parent.GameData.Players[playerID].GovernmentType;

			if ((this.parent.GameData.PlayerFlags & (0x1 << playerID)) != 0)
			{
				if (newGovernmentType == GovernmentTypeEnum.Anarchy)
				{
					if (currentGovernmentType != newGovernmentType)
					{
						this.parent.GameData.Players[playerID].GovernmentType = newGovernmentType;

						this.parent.StartGameMenu.F5_0000_1af6_LoadGovernmentImage();

						// Instruction address 0x2517:0x05dc, size: 5
						this.parent.Segment_1238.F0_1238_1b44();

						this.parent.StartGameMenu.F5_0000_1ba2_ChangeGovernment();
					}
				}
				else
				{
					int local_14 = 0;

					do
					{
						StringBuilder governmentMenu = new();
						// Instruction address 0x2517:0x04ce, size: 5
						governmentMenu.Append("Select type of government:\n");

						for (int i = 1; i < 6; i++)
						{
							if (!this.parent.CityWorker.F0_1d12_6c97_PlayerHasWonder(playerID, WonderEnum.Pyramids))
							{
								switch (i)
								{
									case 2:
										if (this.parent.Segment_1ade.F0_1ade_22b5_PlayerHasTechnology(playerID, TechnologyAdvanceEnum.Monarchy))
										{
											governments.Add((GovernmentTypeEnum)i);
											governmentMenu.Append($" {this.parent.Array_1966_GovernmentNames[i]}\n");
										}
										break;

									case 3:
										if (this.parent.Segment_1ade.F0_1ade_22b5_PlayerHasTechnology(playerID, TechnologyAdvanceEnum.Communism))
										{
											governments.Add((GovernmentTypeEnum)i);
											governmentMenu.Append($" {this.parent.Array_1966_GovernmentNames[i]}\n");
										}
										break;

									case 4:
										if (this.parent.Segment_1ade.F0_1ade_22b5_PlayerHasTechnology(playerID, TechnologyAdvanceEnum.TheRepublic))
										{
											governments.Add((GovernmentTypeEnum)i);
											governmentMenu.Append($" {this.parent.Array_1966_GovernmentNames[i]}\n");
										}
										break;

									case 5:
										if (this.parent.Segment_1ade.F0_1ade_22b5_PlayerHasTechnology(playerID, TechnologyAdvanceEnum.Democracy))
										{
											governments.Add((GovernmentTypeEnum)i);
											governmentMenu.Append($" {this.parent.Array_1966_GovernmentNames[i]}\n");
										}
										break;
								}
							}
							else
							{
								governments.Add((GovernmentTypeEnum)i);
								governmentMenu.Append($" {this.parent.Array_1966_GovernmentNames[i]}\n");
							}
						}

						if (governments.Count == 0)
						{
							governments.Add(GovernmentTypeEnum.Despotism);
							governmentMenu.Append($" {this.parent.Array_1966_GovernmentNames[1]}\n");
						}

						// Instruction address 0x2517:0x05ab, size: 5
						local_14 = this.parent.Segment_1238.F0_1238_001e_ShowDialog(governmentMenu.ToString(), 100, 64);
					}
					while (local_14 < 0);

					this.parent.GameData.Players[playerID].GovernmentType = governments[local_14];

					this.parent.StartGameMenu.F5_0000_1af6_LoadGovernmentImage();

					// Instruction address 0x2517:0x05dc, size: 5
					this.parent.Segment_1238.F0_1238_1b44();

					this.parent.StartGameMenu.F5_0000_1ba2_ChangeGovernment();
				}
			}
			else
			{
				if (this.parent.GameData.Players[this.parent.GameData.HumanPlayerID].Diplomacy[playerID].HasFlag(DiplomacyFlagsEnum.EmbassyEstablished) &&
					(this.parent.GameData.Players[playerID].GovernmentType != newGovernmentType ||
					this.parent.GameData.Players[playerID].Diplomacy[0].HasFlag(DiplomacyFlagsEnum.Allied)))
				{
					if (this.parent.GameData.Players[playerID].Diplomacy[0].HasFlag(DiplomacyFlagsEnum.Allied))
					{
						this.parent.GameData.Players[playerID].Diplomacy[0] |= DiplomacyFlagsEnum.Allied;
						this.parent.GameData.Players[playerID].Diplomacy[0] ^= DiplomacyFlagsEnum.Allied;

						this.parent.Var_2f9e_MessageBoxStyle = MenuBoxReportTypeEnum.DiplomatsReport;

						// Instruction address 0x2517:0x06ce, size: 5
						this.parent.Segment_1238.F0_1238_001e_ShowDialog($"The {this.parent.GameData.Players[playerID].Nationality} government\nchanged " +
							$"to {this.parent.Array_1966_GovernmentNames[(int)newGovernmentType]}!\n", 80, 64);
					}
					else
					{
						this.parent.GameData.Players[playerID].Diplomacy[0] |= DiplomacyFlagsEnum.Allied;

						this.parent.Var_2f9e_MessageBoxStyle = MenuBoxReportTypeEnum.DiplomatsReport;

						// Instruction address 0x2517:0x06ce, size: 5
						this.parent.Segment_1238.F0_1238_001e_ShowDialog(
							$"The {this.parent.GameData.Players[playerID].Nationality} government\nhas been overthrown!\n", 80, 64);
					}
				}

				this.parent.GameData.Players[playerID].GovernmentType = newGovernmentType;

				this.parent.GameData.Players[playerID].TaxRate = (short)(4 - ((int)newGovernmentType / 2));
			}

			if (currentGovernmentType != this.parent.GameData.Players[playerID].GovernmentType)
			{
				// Instruction address 0x2517:0x070a, size: 5
				this.parent.UnitManagement.F0_1866_250e_AddReplayData(8, (byte)playerID, (byte)this.parent.GameData.Players[playerID].GovernmentType);

				for (int i = 0; i < 8; i++)
				{
					this.parent.GameData.Players[playerID].Diplomacy[i] |= DiplomacyFlagsEnum.Vendetta;
					this.parent.GameData.Players[playerID].Diplomacy[i] ^= DiplomacyFlagsEnum.Vendetta;
				}
			}
		}

		/// <summary>
		/// Updates player diplomacy relations and notifies of diplomacy changes if embassy established
		/// </summary>
		/// <param name="playerID1"></param>
		/// <param name="playerID2"></param>
		/// <param name="x"></param>
		/// <param name="y"></param>
		public void F0_2517_0737_UpdateDiplomacyStatus(int playerID1, int playerID2, int x, int y)
		{
			//this.oCPU.Log.EnterBlock($"F0_2517_0737({playerID1}, {playerID2}, {x}, {y})");

			// function body
			if (playerID1 != 0 && playerID2 != 0)
			{
				if (playerID1 == this.parent.GameData.HumanPlayerID)
				{
					if (!this.parent.GameData.Players[playerID1].Diplomacy[playerID2].HasFlag(DiplomacyFlagsEnum.Contact) ||
						this.parent.GameData.TurnCount - this.parent.GameData.Players[playerID2].ContactPlayerCountdown >= 16)
					{
						this.parent.MeetWithKing.F6_0000_0000(playerID2, x, y, 0);
					}
				}

				if (playerID2 == this.parent.GameData.HumanPlayerID)
				{
					if (!this.parent.GameData.Players[playerID1].Diplomacy[playerID2].HasFlag(DiplomacyFlagsEnum.Contact) ||
						this.parent.GameData.TurnCount - this.parent.GameData.Players[playerID1].ContactPlayerCountdown >= 16)
					{
						this.parent.MeetWithKing.F6_0000_0000(playerID1, x, y, 0);
					}
				}

				this.parent.GameData.Players[playerID1].Diplomacy[playerID2] |= DiplomacyFlagsEnum.Contact | DiplomacyFlagsEnum.FormalContact;
				this.parent.GameData.Players[playerID2].Diplomacy[playerID1] |= DiplomacyFlagsEnum.Contact | DiplomacyFlagsEnum.FormalContact;

				if (playerID1 != this.parent.GameData.HumanPlayerID && playerID2 != this.parent.GameData.HumanPlayerID)
				{
					if (((playerID1 + playerID2 + this.parent.GameData.TurnCount) & 0x3) == 0 ||
						!this.parent.GameData.Players[playerID1].Diplomacy[playerID2].HasFlag(DiplomacyFlagsEnum.Contact))
					{
						if (!this.parent.GameData.Players[playerID1].Diplomacy[playerID2].HasFlag(DiplomacyFlagsEnum.Vendetta) &&
							!this.parent.GameData.Players[playerID2].Diplomacy[playerID1].HasFlag(DiplomacyFlagsEnum.Vendetta))
						{
							if (!F0_2517_0b1d_DetermineWhoIsStrongest(playerID1, playerID2) ||
								this.parent.CityWorker.F0_1d12_6c97_PlayerHasWonder(playerID2, WonderEnum.GreatWall) ||
								this.parent.GameData.Players[playerID1].GovernmentType == GovernmentTypeEnum.Republic ||
								this.parent.GameData.Players[playerID1].GovernmentType == GovernmentTypeEnum.Democracy)
							{
								if (!F0_2517_0b1d_DetermineWhoIsStrongest(playerID2, playerID1) ||
									this.parent.CityWorker.F0_1d12_6c97_PlayerHasWonder(playerID1, WonderEnum.GreatWall) ||
									this.parent.GameData.Players[playerID2].GovernmentType == GovernmentTypeEnum.Republic ||
									this.parent.GameData.Players[playerID2].GovernmentType == GovernmentTypeEnum.Democracy)
								{
									// Instruction address 0x2517:0x08b8, size: 3
									F0_2517_0cef_CanExchangeTechnologies(playerID1, playerID2);

									if (!this.parent.GameData.Players[playerID1].Diplomacy[playerID2].HasFlag(DiplomacyFlagsEnum.Peace))
									{
										if (this.parent.GameData.Players[this.parent.GameData.HumanPlayerID].Diplomacy[playerID1].HasFlag(DiplomacyFlagsEnum.EmbassyEstablished) ||
											this.parent.GameData.Players[this.parent.GameData.HumanPlayerID].Diplomacy[playerID2].HasFlag(DiplomacyFlagsEnum.EmbassyEstablished))
										{
											this.parent.Var_2f9e_MessageBoxStyle = MenuBoxReportTypeEnum.DiplomatsReport;

											// Instruction address 0x2517:0x094d, size: 5
											this.parent.Segment_1238.F0_1238_001e_ShowDialog($"{this.parent.GameData.Players[playerID1].Nation} " +
												$"and\n{this.parent.GameData.Players[playerID2].Nation} sign\na peace treaty.\n", 100, 80);
										}

										// Instruction address 0x2517:0x0960, size: 3
										F0_2517_0a30_SetDiplomacyFlags(playerID1, playerID2, DiplomacyFlagsEnum.Peace);
									}

									return;
								}
							}

							if (this.parent.GameData.Players[playerID1].Diplomacy[playerID2].HasFlag(DiplomacyFlagsEnum.Peace) &&
								this.parent.GameData.Players[playerID1].Diplomacy[playerID2].HasFlag(DiplomacyFlagsEnum.Contact))
							{
								if (this.parent.GameData.Players[this.parent.GameData.HumanPlayerID].Diplomacy[playerID1].HasFlag(DiplomacyFlagsEnum.EmbassyEstablished) ||
									this.parent.GameData.Players[this.parent.GameData.HumanPlayerID].Diplomacy[playerID2].HasFlag(DiplomacyFlagsEnum.EmbassyEstablished))
								{
									if (this.parent.GameData.Players[playerID1].Diplomacy[playerID2].HasFlag(DiplomacyFlagsEnum.Peace))
									{
										this.parent.Var_2f9e_MessageBoxStyle = MenuBoxReportTypeEnum.DiplomatsReport;

										// Instruction address 0x2517:0x0a11, size: 5
										this.parent.Segment_1238.F0_1238_001e_ShowDialog(
											$"{this.parent.GameData.Players[playerID1].Nation} and\n{this.parent.GameData.Players[playerID2].Nation} cancel\npeace treaty.\n", 100, 80);
									}
									else
									{
										this.parent.Var_2f9e_MessageBoxStyle = MenuBoxReportTypeEnum.DiplomatsReport;

										// Instruction address 0x2517:0x0a11, size: 5
										this.parent.Segment_1238.F0_1238_001e_ShowDialog(
											$"{this.parent.GameData.Players[playerID1].Nation} and\n{this.parent.GameData.Players[playerID2].Nation} declare\nwar on each other.\n", 100, 80);
									}
								}
							
								// Instruction address 0x2517:0x0a24, size: 3
								F0_2517_0aa1_ClearDiplomacyFlags(playerID1, playerID2, DiplomacyFlagsEnum.Peace);
							}
						}
					}
				}
			}
		}

		/// <summary>
		/// Sets the diplomacy flags for both players involved
		/// </summary>
		/// <param name="playerID1"></param>
		/// <param name="playerID2"></param>
		/// <param name="flags"></param>
		public void F0_2517_0a30_SetDiplomacyFlags(int playerID1, int playerID2, DiplomacyFlagsEnum flags)
		{
			//this.oCPU.Log.EnterBlock($"F0_2517_0a30_SetDiplomacyFlags({playerID1}, {playerID2}, {flags})");

			// function body
			if (flags == DiplomacyFlagsEnum.Peace &&
				(this.parent.GameData.Players[playerID1].Diplomacy[playerID2] & flags) == DiplomacyFlagsEnum.None)
			{
				// Instruction address 0x2517:0x0a5e, size: 5
				this.parent.UnitManagement.F0_1866_250e_AddReplayData(3, (byte)(((ushort)playerID1 << 4) | (ushort)playerID2));
			}

			this.parent.GameData.Players[playerID1].Diplomacy[playerID2] |= flags;
			this.parent.GameData.Players[playerID2].Diplomacy[playerID1] |= flags;

			if (flags == DiplomacyFlagsEnum.Peace)
			{
				this.parent.GameData.Players[playerID1].Diplomacy[playerID2] |= DiplomacyFlagsEnum.Peace;
				this.parent.GameData.Players[playerID1].Diplomacy[playerID2] ^= DiplomacyFlagsEnum.Peace;

				this.parent.GameData.Players[playerID2].Diplomacy[playerID1] |= DiplomacyFlagsEnum.Peace;
				this.parent.GameData.Players[playerID2].Diplomacy[playerID1] ^= DiplomacyFlagsEnum.Peace;
			}
		}

		/// <summary>
		/// Clears the diplomacy flags for both players involved
		/// </summary>
		/// <param name="playerID1"></param>
		/// <param name="playerID2"></param>
		/// <param name="flags"></param>
		public void F0_2517_0aa1_ClearDiplomacyFlags(int playerID1, int playerID2, DiplomacyFlagsEnum flags)
		{
			//this.oCPU.Log.EnterBlock($"F0_2517_0aa1_ClearDiplomacyFlags({playerID1}, {playerID2}, {flags})");

			// function body
			if (flags == DiplomacyFlagsEnum.Peace &&
				(this.parent.GameData.Players[playerID1].Diplomacy[playerID2] & flags) != DiplomacyFlagsEnum.None)
			{
				// Instruction address 0x2517:0x0ad2, size: 5
				this.parent.UnitManagement.F0_1866_250e_AddReplayData(2, (byte)((short)((playerID1 << 4) | playerID2)));
			}

			this.parent.GameData.Players[playerID1].Diplomacy[playerID2] |= flags;
			this.parent.GameData.Players[playerID1].Diplomacy[playerID2] ^= flags;

			this.parent.GameData.Players[playerID2].Diplomacy[playerID1] |= flags;
			this.parent.GameData.Players[playerID2].Diplomacy[playerID1] ^= flags;

			if (flags.HasFlag(DiplomacyFlagsEnum.Peace))
			{
				this.parent.GameData.Players[playerID1].Diplomacy[playerID2] |= DiplomacyFlagsEnum.Allied;
				this.parent.GameData.Players[playerID1].Diplomacy[playerID2] ^= DiplomacyFlagsEnum.Allied;

				this.parent.GameData.Players[playerID2].Diplomacy[playerID1] |= DiplomacyFlagsEnum.Allied;
				this.parent.GameData.Players[playerID2].Diplomacy[playerID1] ^= DiplomacyFlagsEnum.Allied;
			}
		}

		/// <summary>
		/// Determine player strengths
		/// !!! This function is a little strange as it also tests for human player
		/// </summary>
		/// <param name="playerID1"></param>
		/// <param name="playerID2"></param>
		/// <returns>true if playerID1 is strongest than playerID2</returns>
		public bool F0_2517_0b1d_DetermineWhoIsStrongest(int playerID1, int playerID2)
		{
			//this.oCPU.Log.EnterBlock($"F0_2517_0b1d({playerID1}, {playerID2})");

			// function body
			if ((this.parent.GameData.DebugFlags & 0x1) != 0)
			{
				if ((this.parent.GameData.Players[playerID1].Diplomacy[this.parent.GameData.HumanPlayerID] & (DiplomacyFlagsEnum.Contact | DiplomacyFlagsEnum.Peace)) != DiplomacyFlagsEnum.Contact ||
					this.parent.GameData.Players[playerID1].MilitaryPower >= this.parent.GameData.Players[this.parent.GameData.HumanPlayerID].MilitaryPower ||
					this.parent.GameData.Players[playerID1].Diplomacy[this.parent.GameData.HumanPlayerID].HasFlag(DiplomacyFlagsEnum.Allied))
				{
					if ((this.parent.GameData.Players[playerID2].Diplomacy[this.parent.GameData.HumanPlayerID] & (DiplomacyFlagsEnum.Contact | DiplomacyFlagsEnum.Peace)) != DiplomacyFlagsEnum.Contact ||
						this.parent.GameData.Players[playerID2].MilitaryPower >= this.parent.GameData.Players[this.parent.GameData.HumanPlayerID].MilitaryPower ||
						this.parent.GameData.Players[playerID2].Diplomacy[this.parent.GameData.HumanPlayerID].HasFlag(DiplomacyFlagsEnum.Allied))
					{
						if (this.parent.GameData.Players[playerID1].ActiveUnits[(int)UnitTypeEnum.Nuclear] >= this.parent.GameData.Players[playerID2].ActiveUnits[(int)UnitTypeEnum.Nuclear])
						{
							int peaceScore = 0;

							for (int i = 1; i < 8; i++)
							{
								if (i != playerID1 && i != playerID2)
								{
									if ((this.parent.GameData.Players[playerID1].Diplomacy[i] & (DiplomacyFlagsEnum.Contact | DiplomacyFlagsEnum.Peace)) == DiplomacyFlagsEnum.Contact)
									{
										peaceScore++;
									}
								}
							}

							if (this.parent.GameData.Players[playerID1].Diplomacy[playerID2].HasFlag(DiplomacyFlagsEnum.Peace))
							{
								peaceScore++;
							}

							peaceScore -= this.parent.GameData.Nations[this.parent.GameData.Players[playerID1].NationalityID].Mood;
							peaceScore += 4;

							int totalContinentCityCount = 0;
							int totalContinentDefense = 0;
							int totalContinentAttackRatio = 1;

							for (int i = 1; i < 16; i++)
							{
								if (i == 15)
								{
									if (totalContinentCityCount == 0 || (totalContinentDefense * 4) / totalContinentAttackRatio >= peaceScore)
									{
										return true;
									}
									break;
								}

								if (this.parent.GameData.Players[playerID1].Continents[i].CityCount != 0 &&
									(this.parent.GameData.Players[playerID1].Continents[i].Defense / 2) + this.parent.GameData.Players[playerID1].Continents[i].Attack <
									this.parent.GameData.Players[playerID2].Continents[i].Defense)
								{
									break;
								}

								if (this.parent.GameData.Players[playerID1].Continents[i].Defense != 0)
								{
									int continentAttack = this.parent.GameData.Players[playerID2].Continents[i].Attack;

									if (continentAttack != 0)
									{
										totalContinentCityCount += this.parent.GameData.Players[playerID1].Continents[i].CityCount;
										totalContinentDefense += this.parent.GameData.Players[playerID1].Continents[i].Defense;
										totalContinentAttackRatio += this.parent.GameData.Players[playerID2].Continents[i].Defense / 2 + continentAttack;
									}
								}
							}
						}
					}
				}

				return false;
			}

			return true;
		}

		/// <summary>
		/// Determine if both players can exchange technology
		/// </summary>
		/// <param name="playerID1"></param>
		/// <param name="playerID2"></param>
		private void F0_2517_0cef_CanExchangeTechnologies(int playerID1, int playerID2)
		{
			//this.oCPU.Log.EnterBlock($"F0_2517_0cef({playerID1}, {playerID2})");

			// function body
			int missingTechnologyPlayer1 = -1;
			int missingTechnologyPlayer2 = -1;

			for (int i = 0; i < 72; i++)
			{
				if (!this.parent.Segment_1ade.F0_1ade_22b5_PlayerHasTechnology(playerID1, (TechnologyAdvanceEnum)i) &&
					this.parent.Segment_1ade.F0_1ade_22b5_PlayerHasTechnology(playerID2, (TechnologyAdvanceEnum)i))
				{
					missingTechnologyPlayer1 = i;
				}

				if (this.parent.Segment_1ade.F0_1ade_22b5_PlayerHasTechnology(playerID1, (TechnologyAdvanceEnum)i) &&
					!this.parent.Segment_1ade.F0_1ade_22b5_PlayerHasTechnology(playerID2, (TechnologyAdvanceEnum)i))
				{
					missingTechnologyPlayer2 = i;
				}
			}

			if (missingTechnologyPlayer1 != -1 && missingTechnologyPlayer2 != -1)
			{
				// Instruction address 0x2517:0x0d73, size: 5
				this.parent.Segment_1ade.F0_1ade_1d2e(playerID1, playerID2, (TechnologyAdvanceEnum)missingTechnologyPlayer1);

				// Instruction address 0x2517:0x0d84, size: 5
				this.parent.Segment_1ade.F0_1ade_1d2e(playerID2, playerID1, (TechnologyAdvanceEnum)missingTechnologyPlayer2);
			}
		}

		/// <summary>
		/// Gets minimum distance from palace
		/// </summary>
		/// <param name="playerID"></param>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns>minimum distance</returns>
		public int F0_2517_0d97_GetMinimumDistanceFromPalace(int playerID, int x, int y)
		{
			//this.oCPU.Log.EnterBlock($"F0_2517_0d97({playerID}, {x}, {y})");

			// function body
			int minimumDistance = 16;

			for (int i = 0; i < 128; i++)
			{
				if (this.parent.GameData.Cities[i].Status != CityStatusEnum.Invalid && this.parent.GameData.Cities[i].PlayerID == playerID &&
					this.parent.GameData.Cities[i].HasImprovement(ImprovementEnum.Palace))
				{
					// Instruction address 0x2517:0x0dda, size: 5
					int distance = this.parent.MapManagement.GetDistance(this.parent.GameData.Cities[i].Position, x, y);

					if (distance < minimumDistance)
					{
						minimumDistance = distance;
					}
				}
			}

			return minimumDistance;
		}
	}
}
