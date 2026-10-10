using IRB.VirtualCPU;
using OpenCivOne.Graphics;

namespace OpenCivOne
{
	/// <summary>
	/// Which technologies players have, and giving technologies to players
	/// </summary>
	public class Technology
	{
		private OpenCivOneGame oParent;
		private VCPU oCPU;

		public Technology(OpenCivOneGame parent)
		{
			this.oParent = parent;
			this.oCPU = parent.CPU;
		}

		/// <summary>
		/// Player 1 ans technology that player 2 doesn't have
		/// </summary>
		/// <param name="playerID1"></param>
		/// <param name="playerID2"></param>
		/// <param name="technology"></param>
		public void F0_1ade_1d2e(int playerID1, int playerID2, TechnologyAdvanceEnum technology)
		{
			this.oCPU.Log.EnterBlock($"F0_1ade_1d2e({playerID1}, {technology}, {playerID2})");

			// function body
			this.oCPU.PUSH_UInt16(this.oCPU.BP.UInt16);
			this.oCPU.BP.UInt16 = this.oCPU.SP.UInt16;
			this.oCPU.SP.UInt16 = this.oCPU.SUB_UInt16(this.oCPU.SP.UInt16, 0x8);
			this.oCPU.PUSH_UInt16(this.oCPU.DI.UInt16);
			this.oCPU.PUSH_UInt16(this.oCPU.SI.UInt16);

			// Instruction address 0x1ade:0x1d3d, size: 3
			this.oCPU.AX.UInt16 = (ushort)(F0_1ade_22b5_PlayerHasTechnology(playerID1, technology) ? 1 : 0);
			this.oCPU.AX.UInt16 = this.oCPU.OR_UInt16(this.oCPU.AX.UInt16, this.oCPU.AX.UInt16);
			if (this.oCPU.Flags.E)
				goto L1d4a;

			goto L22af;

		L1d4a:
			if (technology == TechnologyAdvanceEnum.FutureTechnology5) goto L1d9e;

			this.oCPU.AX.UInt16 = (ushort)technology;
			this.oCPU.CWD(this.oCPU.AX, this.oCPU.DX);
			this.oCPU.CX.UInt16 = 0x10;
			this.oCPU.IDIV_UInt16(this.oCPU.AX, this.oCPU.DX, this.oCPU.CX.UInt16);
			this.oCPU.CX.UInt16 = this.oCPU.DX.UInt16;
			this.oCPU.AX.UInt16 = 0x1;
			this.oCPU.AX.UInt16 = this.oCPU.SHL_UInt16(this.oCPU.AX.UInt16, this.oCPU.CX.LowUInt8);
			this.oCPU.CX.UInt16 = this.oCPU.AX.UInt16;

			this.oCPU.AX.UInt16 = 0xa;
			this.oCPU.IMUL_UInt16(this.oCPU.AX, this.oCPU.DX, (ushort)((short)playerID1));
			this.oCPU.BX.UInt16 = this.oCPU.AX.UInt16;

			this.oCPU.AX.UInt16 = (ushort)technology;
			this.oCPU.SI.UInt16 = this.oCPU.CX.UInt16;
			this.oCPU.CWD(this.oCPU.AX, this.oCPU.DX);
			this.oCPU.AX.UInt16 = this.oCPU.XOR_UInt16(this.oCPU.AX.UInt16, this.oCPU.DX.UInt16);
			this.oCPU.AX.UInt16 = this.oCPU.SUB_UInt16(this.oCPU.AX.UInt16, this.oCPU.DX.UInt16);
			this.oCPU.CX.UInt16 = 0x4;
			this.oCPU.AX.UInt16 = this.oCPU.SAR_UInt16(this.oCPU.AX.UInt16, this.oCPU.CX.LowUInt8);
			this.oCPU.AX.UInt16 = this.oCPU.XOR_UInt16(this.oCPU.AX.UInt16, this.oCPU.DX.UInt16);
			this.oCPU.AX.UInt16 = this.oCPU.SUB_UInt16(this.oCPU.AX.UInt16, this.oCPU.DX.UInt16);
			this.oCPU.DI.UInt16 = this.oCPU.AX.UInt16;
			this.oCPU.DI.UInt16 = this.oCPU.SHL_UInt16(this.oCPU.DI.UInt16, 0x1);

			this.oParent.GameData.Players[playerID1].DiscoveredTechnologyFlags[this.oCPU.AX.UInt16] |= this.oCPU.SI.UInt16;
			this.oParent.GameData.Players[playerID1].TechnologyAcquiredFrom[(int)technology] = (short)playerID2;

		L1d9e:
			if (this.oParent.GameData.TurnCount == 0)
				goto L22af;

			this.oParent.GameData.Players[playerID1].DiscoveredTechnologyCount++;

			if (playerID1 == this.oParent.GameData.HumanPlayerID) goto L1dce;

			this.oCPU.SI.Int16 = (short)playerID1;
			this.oCPU.SI.UInt16 = this.oCPU.SHL_UInt16(this.oCPU.SI.UInt16, 0x1);

			this.oCPU.BX.UInt16 = (ushort)this.oParent.GameData.HumanPlayerID;
			this.oCPU.CX.LowUInt8 = 0x4;
			this.oCPU.BX.UInt16 = this.oCPU.SHL_UInt16(this.oCPU.BX.UInt16, this.oCPU.CX.LowUInt8);

			if (!this.oParent.GameData.Players[this.oParent.GameData.HumanPlayerID].Diplomacy[playerID1].HasFlag(DiplomacyFlagsEnum.EmbassyEstablished))
				goto L1f47;

		L1dce:
			this.oCPU.WriteUInt8(this.oCPU.DS.UInt16, 0xba06, 0x0);

			// Instruction address 0x1ade:0x1de0, size: 5
			this.oParent.CAPI.strcat(0xba06, this.oParent.GameData.Players[playerID1].Nation);

			if (playerID1 == playerID2 || playerID2 == 0)
			{
				// Instruction address 0x1ade:0x1e01, size: 5
				this.oParent.CAPI.strcat(0xba06, " develop\n");
			}
			else
			{
				// Instruction address 0x1ade:0x1e01, size: 5
				this.oParent.CAPI.strcat(0xba06, " acquire\n");
			}

			// Instruction address 0x1ade:0x1e17, size: 5
			this.oParent.CAPI.strcat(0xba06, this.oParent.GameData.TechnologyAdvances[(int)technology].Name);

			if (playerID1 == playerID2) goto L1e5c;
			this.oCPU.AX.UInt16 = this.oCPU.OR_UInt16(this.oCPU.AX.UInt16, this.oCPU.AX.UInt16);
			if (this.oCPU.Flags.E) goto L1e5c;

			// Instruction address 0x1ade:0x1e33, size: 5
			this.oParent.CAPI.strcat(0xba06, " from\nthe ");

			if (playerID2 != -2) goto L1e47;

			// Instruction address 0x1ade:0x1e54, size: 5
			this.oParent.CAPI.strcat(0xba06, "Great Library");
			goto L1e5c;

		L1e47:
			// Instruction address 0x1ade:0x1e54, size: 5
			this.oParent.CAPI.strcat(0xba06, this.oParent.GameData.Players[playerID2].Nation);

		L1e5c:
			// Instruction address 0x1ade:0x1e64, size: 5
			this.oParent.CAPI.strcat(0xba06, "!\n ");

			if (playerID1 == this.oParent.GameData.HumanPlayerID)
				goto L1ef9;

			this.oCPU.WriteUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x2), 0x0);

		L1e7c:
			this.oCPU.AX.UInt16 = 0x1e;
			this.oCPU.IMUL_UInt16(this.oCPU.AX, this.oCPU.DX, this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x2)));
			this.oCPU.SI.UInt16 = this.oCPU.AX.UInt16;

			if (this.oParent.GameData.GetImprovementType(this.oCPU.ReadInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x2))).RequiresTechnology != technology)
				goto L1eaf;

			// Instruction address 0x1ade:0x1e97, size: 5
			this.oParent.CAPI.strcat(0xba06,
				this.oParent.GameData.GetImprovementType(this.oCPU.ReadInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x2))).Name);

			// Instruction address 0x1ade:0x1ea7, size: 5
			this.oParent.CAPI.strcat(0xba06, "\n ");

		L1eaf:
			this.oCPU.WriteUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x2), this.oCPU.INC_UInt16(this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x2))));
			this.oCPU.CMP_UInt16(this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x2)), 0x18);
			if (this.oCPU.Flags.L) goto L1e7c;
			this.oCPU.WriteUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x2), 0x0);

		L1ebd:
			this.oCPU.AX.UInt16 = 0x22;
			this.oCPU.IMUL_UInt16(this.oCPU.AX, this.oCPU.DX, this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x2)));
			this.oCPU.SI.UInt16 = this.oCPU.AX.UInt16;

			if (this.oParent.GameData.Units[this.oCPU.ReadInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x2))].RequiredTechnology == (TechnologyAdvanceEnum)technology)
			{
				// Instruction address 0x1ade:0x1ed8, size: 5
				this.oParent.CAPI.strcat(0xba06,
					this.oParent.GameData.Units[this.oCPU.ReadInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x2))].Name);

				// Instruction address 0x1ade:0x1ee8, size: 5
				this.oParent.CAPI.strcat(0xba06, "\n ");
			}

			this.oCPU.WriteUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x2), 
				this.oCPU.INC_UInt16(this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x2))));

			this.oCPU.CMP_UInt16(this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x2)), 0x1c);
			if (this.oCPU.Flags.L) goto L1ebd;

		L1ef9:
			if (playerID1 != this.oParent.GameData.HumanPlayerID) goto L1f0a;

			if (playerID2 != -2)
				goto L2013;

		L1f0a:
			if (this.oParent.Var_3934 == -1) goto L1f14;
			goto L2013;

		L1f14:
			if (playerID1 == this.oParent.GameData.HumanPlayerID) goto L1f2a;

			this.oParent.Var_2f9e_MessageBoxStyle = MenuBoxReportTypeEnum.SpiesReport;

			goto L1f30;

		L1f2a:
			this.oParent.Var_2f9e_MessageBoxStyle = MenuBoxReportTypeEnum.ScienceAdvisorReport;

		L1f30:
			// Instruction address 0x1ade:0x1f3c, size: 5
			this.oParent.MenuBoxDialog.ShowDialog(this.oCPU.ReadString(this.oCPU.DS.UInt16, 0xba06), 100, 80);

			goto L2013;

		L1f47:
			if ((this.oParent.GameData.TechnologyFirstDiscoveredBy[(int)technology] & 0x7) == 0) goto L1f5b;
			goto L2013;

		L1f5b:
			// Instruction address 0x1ade:0x1f63, size: 3
			this.oCPU.AX.UInt16 = (ushort)(F0_1ade_22b5_PlayerHasTechnology(playerID1, TechnologyAdvanceEnum.Navigation) ? 1 : 0);
			this.oCPU.AX.UInt16 = this.oCPU.OR_UInt16(this.oCPU.AX.UInt16, this.oCPU.AX.UInt16);
			if (this.oCPU.Flags.NE)
				goto L1f70;

			goto L2013;

		L1f70:
			// Instruction address 0x1ade:0x1f78, size: 3
			this.oCPU.AX.UInt16 = (ushort)(F0_1ade_22b5_PlayerHasTechnology(this.oParent.GameData.HumanPlayerID, technology) ? 1 : 0);
			this.oCPU.AX.UInt16 = this.oCPU.OR_UInt16(this.oCPU.AX.UInt16, this.oCPU.AX.UInt16);
			if (this.oCPU.Flags.E)
				goto L1f85;

			goto L2013;

		L1f85:
			// Instruction address 0x1ade:0x1f89, size: 5
			this.oCPU.AX.UInt16 = (ushort)(this.oParent.CAPI.RNG.Next(3));

			this.oCPU.AX.UInt16 = this.oCPU.OR_UInt16(this.oCPU.AX.UInt16, this.oCPU.AX.UInt16);
			if (this.oCPU.Flags.NE) goto L2013;

			this.oParent.MeetWithKing.F6_0000_16cd(playerID1, -1, 0);
			
			this.oCPU.AX.UInt16 = this.oCPU.INC_UInt16(this.oCPU.AX.UInt16);
			if (this.oCPU.Flags.E) goto L2013;

			this.oParent.Var_3934 = 0;
			this.oCPU.WriteUInt16(this.oCPU.DS.UInt16, 0xd804, 0);

			// Instruction address 0x1ade:0x1fc4, size: 5
			this.oParent.Array_30b8[0] = this.oParent.GameData.Players[playerID1].Nation;

			// Instruction address 0x1ade:0x1fda, size: 5
			this.oParent.Array_30b8[3] = this.oParent.GameData.TechnologyAdvances[(int)technology].Name;

			// Instruction address 0x1ade:0x1fe6, size: 5
			this.oParent.CAPI.strcpy(0xba06, this.oParent.LanguageTools.F0_2f4d_044f_GetTextFromKingSection("*GAPE"));

			this.oParent.Var_3936 = 2;
			this.oParent.MeetWithKing.Var_d76c = 12;

			this.oParent.MeetWithKing.F6_0000_251d_ShowInlineDialog(0xba06, 36, 139);

			this.oParent.MeetWithKing.F6_0000_16ac();

		L2013:
			if (playerID1 != this.oParent.GameData.HumanPlayerID) goto L2065;

			if (technology == TechnologyAdvanceEnum.FutureTechnology5) goto L202f;

			this.oParent.Encyclopedia.DisplayEncyclopediaTopic(EncyclopediaTopicEnum.TechnologyAdvance, (int)technology);

		L202f:
			if (technology != TechnologyAdvanceEnum.Invention) goto L203f;

			this.oParent.StartGameMenu.F5_0000_1af6_LoadGovernmentImage();

			// Instruction address 0x1ade:0x203a, size: 5
			this.oParent.GameEngine.F0_1238_1b44_RedrawMapWithBlankingEffect();

		L203f:
			if (this.oParent.Var_3934 == -1) goto L2049;
			goto L20d9;

		L2049:
			// Instruction address 0x1ade:0x2049, size: 5
			this.oParent.GameEngine.F0_1238_1b44_RedrawMapWithBlankingEffect();
			
			if (technology != TechnologyAdvanceEnum.Monarchy)
				goto L20d9;

			this.oParent.Help.F4_0000_02d3_ShowInstantAdvicePopup("*MONARCHY");
			
			goto L20d9;

		L2065:
			if (this.oParent.GameData.DifficultyLevel != 0 || technology != TechnologyAdvanceEnum.Writing) goto L20d9;

			this.oCPU.SI.Int16 = (short)playerID1;
			this.oCPU.SI.UInt16 = this.oCPU.SHL_UInt16(this.oCPU.SI.UInt16, 0x1);

			this.oCPU.DI.Int16 = (short)this.oParent.GameData.HumanPlayerID;
			this.oCPU.CX.LowUInt8 = 0x4;
			this.oCPU.DI.UInt16 = this.oCPU.SHL_UInt16(this.oCPU.DI.UInt16, this.oCPU.CX.LowUInt8);
			this.oCPU.DI.UInt16 = this.oCPU.ADD_UInt16(this.oCPU.DI.UInt16, this.oCPU.SI.UInt16);

			// !!! Exchange of Ambassadors, rule is never invoked!
			// This piece of code is always skipped, as the result is always 0 or 1, and bit 0x40 is always 0!
			//
			// this.oCPU.CMPWord(this.oCPU.ReadUInt16(this.oCPU.DS.Word, this.oCPU.DI.Word), 0x1);
			// this.oCPU.AX.Word = this.oCPU.SBBWord(this.oCPU.AX.Word, this.oCPU.AX.Word);
			// this.oCPU.AX.Word = this.oCPU.NEGWord(this.oCPU.AX.Word);
			// this.oCPU.TESTByte(this.oCPU.AX.Low, 0x40);
			if (((this.oParent.GameData.Players[this.oParent.GameData.HumanPlayerID].Diplomacy[playerID1] == DiplomacyFlagsEnum.None ? 1 : 0) & 0x40) == 0)
				goto L20d9;

			if (this.oParent.Var_3934 != -1) goto L20d9;

			this.oParent.GameData.Players[this.oParent.GameData.HumanPlayerID].Diplomacy[playerID1] |= DiplomacyFlagsEnum.EmbassyEstablished;

			// Instruction address 0x1ade:0x20a2, size: 5
			this.oParent.CAPI.strcpy(0xba06, this.oParent.GameData.Players[playerID1].Nation);

			// Instruction address 0x1ade:0x20b2, size: 5
			this.oParent.CAPI.strcat(0xba06, " agree to\nan exchange of ambassadors.\n");

			this.oParent.Var_2f9e_MessageBoxStyle = MenuBoxReportTypeEnum.DiplomatsReport;

			// Instruction address 0x1ade:0x20cc, size: 5
			this.oParent.MenuBoxDialog.ShowDialog(this.oCPU.ReadString(this.oCPU.DS.UInt16, 0xba06), 80, 100);

			this.oParent.AdvisorReports.IntelligenceReport();

		L20d9:
			if (technology != TechnologyAdvanceEnum.Gunpowder && technology != TechnologyAdvanceEnum.Combustion)
				goto L2174;

			for (int i = 0; i < 128; i++)
			{
				if (this.oParent.GameData.Cities[i].Status != CityStatusEnum.Invalid &&
					this.oParent.GameData.Cities[i].PlayerID == playerID1)
				{
					this.oParent.GameData.Cities[i].ImprovementFlags0 &= 0xfffd;
				}
			}

			if (playerID1 != this.oParent.GameData.HumanPlayerID)
				goto L2174;

			if (this.oParent.Var_3934 != -1) goto L2174;

			// Instruction address 0x1ade:0x212c, size: 5
			this.oParent.CAPI.strcpy(0xba06, "Development of ");

			// Instruction address 0x1ade:0x2142, size: 5
			this.oParent.CAPI.strcat(0xba06, this.oParent.GameData.TechnologyAdvances[(int)technology].Name);

			// Instruction address 0x1ade:0x2152, size: 5
			this.oParent.CAPI.strcat(0xba06, "\nmakes existing Barracks\nobsolete.\n");

			this.oParent.Var_2f9e_MessageBoxStyle = MenuBoxReportTypeEnum.DefenseMinisterReport;

			// Instruction address 0x1ade:0x216c, size: 5
			this.oParent.MenuBoxDialog.ShowDialog(this.oCPU.ReadString(this.oCPU.DS.UInt16, 0xba06), 80, 100);

		L2174:
			this.oCPU.TEST_UInt8((byte)this.oParent.GameData.TechnologyFirstDiscoveredBy[(int)technology], 0x7);
			if (this.oCPU.Flags.E) goto L2188;
			goto L2234;

		L2188:
			this.oParent.GameData.TechnologyFirstDiscoveredBy[(int)technology] = (short)playerID1;

			// Instruction address 0x1ade:0x21a1, size: 5
			this.oParent.UnitManagement.F0_1866_250e_AddReplayData(5, (byte)playerID1, (byte)technology);

			for (int i = 1; i < 22; i++)
			{
				if (this.oParent.GameData.WonderCityID[i] != -1 &&
					this.oParent.GameData.Wonders[i].ObsoletesAfterTechnology == (TechnologyAdvanceEnum)technology)
				{
					if (this.oParent.GameData.Cities[this.oParent.GameData.WonderCityID[i]].PlayerID == this.oParent.GameData.HumanPlayerID)
					{
						this.oParent.News.F21_0000_0000_ShowNews(-1,
							$"{this.oParent.GameData.TechnologyAdvances[(int)technology].Name} cancels\n" +
							$"the effect of\n{this.oParent.GameData.Wonders[i].Name}.\n");
					}
				}
			}

		L2234:
			this.oCPU.AX.UInt16 = (ushort)this.oParent.GameData.WonderCityID[5];
			this.oCPU.WriteUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x8), this.oCPU.AX.UInt16);
			this.oCPU.CMP_UInt16(this.oCPU.AX.UInt16, 0xffff);
			if (this.oCPU.Flags.E) goto L22af;

			this.oCPU.AX.UInt16 = (ushort)this.oParent.GameData.Cities[this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x8))].PlayerID;
			this.oCPU.WriteUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x8), this.oCPU.AX.UInt16);

			// Instruction address 0x1ade:0x2254, size: 3
			this.oCPU.AX.UInt16 = (ushort)(F0_1ade_22b5_PlayerHasTechnology((short)this.oCPU.AX.UInt16, technology) ? 1 : 0);
			this.oCPU.AX.UInt16 = this.oCPU.OR_UInt16(this.oCPU.AX.UInt16, this.oCPU.AX.UInt16);
			if (this.oCPU.Flags.NE)
				goto L22af;

			// Instruction address 0x1ade:0x2265, size: 5
			this.oCPU.AX.UInt16 = (ushort)(this.oParent.CityWorker.F0_1d12_6c97_PlayerHasWonder(this.oCPU.ReadInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x8)), WonderEnum.GreatLibrary) ? 1 : 0);
			
			this.oCPU.AX.UInt16 = this.oCPU.OR_UInt16(this.oCPU.AX.UInt16, this.oCPU.AX.UInt16);
			if (this.oCPU.Flags.E)
				goto L22af;

			int iCount = 0;

			for (int i = 1; i < 8; i++)
			{
				// Instruction address 0x1ade:0x2282, size: 3
				if (F0_1ade_22b5_PlayerHasTechnology(i, technology))
				{
					iCount++;
				}
			}

			if (iCount >= 2)
			{
				// Instruction address 0x1ade:0x22a9, size: 3
				F0_1ade_1d2e(this.oCPU.ReadInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x8)), -2, technology);
			}

		L22af:
			this.oCPU.SI.UInt16 = this.oCPU.POP_UInt16();
			this.oCPU.DI.UInt16 = this.oCPU.POP_UInt16();
			this.oCPU.SP.UInt16 = this.oCPU.BP.UInt16;
			this.oCPU.BP.UInt16 = this.oCPU.POP_UInt16();
			// Far return
			this.oCPU.Log.ExitBlock("F0_1ade_1d2e");
		}

		/// <summary>
		/// ?
		/// </summary>
		/// <param name="playerID"></param>
		/// <param name="technology"></param>
		/// <returns></returns>
		public bool F0_1ade_22b5_PlayerHasTechnology(int playerID, TechnologyAdvanceEnum technology)
		{
			// Everyone has TechnologyEnum.None (-1) technology,
			// PlayerID == 0 (Barbarians can't have any technology)
			// TechnologyID can be in the range [0-70]
			// DiscoveredTechnologyFlags holds the bit if technology is discovered

			return (technology == TechnologyAdvanceEnum.None ||
				(playerID != 0 && technology != TechnologyAdvanceEnum.None && technology != TechnologyAdvanceEnum.NewFutureTechnology &&
					(this.oParent.GameData.Players[playerID].DiscoveredTechnologyFlags[(int)technology >> 4] & (1 << ((int)technology & 0xf))) != 0)) ? true : false;

			//return (ushort)((technologyID == -1 || (technologyID >= 0 && (technologyID >= 71 || playerID == 0 ||
			//	(this.oParent.CivState.Players[playerID].DiscoveredTechnologyFlags[technologyID >> 4] & (1 << (technologyID & 0xf))) == 0))) ? 0 : 1);
		}

		private static int[] technologyGradeCoefficient1 = { 5, 4, 4, 4, 3, 6, 4, 6, 6, 6, 4, 3, 4, 3, 3, 3, 3, 6, 5, 3, 4, 3, 3, 4, 4, 5, 4, 8, 3, 3, 3, 4, 5, 4, 8, 5, 5, 6, 4, 5, 4, 6, 6, 2, 5, 4, 6, 4, 6, 4, 3, 3, 4, 4, 3, 4, 5, 2, 8, 2, 4, 2, 5, 5, 6, 4, 3, 0, 0, 0, 0, 0};
		private static int[] technologyGradeCoefficient2 = { 0, 1, 1, 0, 2, 1, 0, -1, -1, -1, 0, 0, 0, 1, -2, 0, 1, -1, -2, 0, 0, 0, 1, -1, 1, 0, 0, -2, 1, 2, 2, -1, -1, -2, -2, -1, -1, -1, -1, -1, 1, 0, 1, 0, 1, 0, -2, 0, -2, 0, 0, 1, -1, 1, 1, 0, -1, 1, -1, 2, 1, 1, -2, -1, -1, -1, 0, 0, 0, 0, 0, 0};

		/// <summary>
		/// Grade technology in according with player ideology
		/// </summary>
		/// <param name="playerID"></param>
		/// <param name="technologyID"></param>
		public int F0_1ade_2317_GradeTechnology(int playerID, TechnologyAdvanceEnum technology)
		{
			//this.oCPU.Log.EnterBlock($"F0_1ade_2317({playerID}, {TechnologyID})");

			// function body
			//this.oCPU.CX.Int16 = this.oCPU.ReadInt8(this.oCPU.DS.UInt16, (ushort)(0xb0a + (technologyID * 2)));
			//this.oCPU.BX.Int16 = this.oCPU.ReadInt8(this.oCPU.DS.UInt16, (ushort)(0xb0b + (technologyID * 2)));

			return (this.oParent.GameData.Nations[this.oParent.GameData.Players[playerID].NationalityID].Ideology * technologyGradeCoefficient2[(int)technology]) + 
				technologyGradeCoefficient1[(int)technology];
		}
	}
}
