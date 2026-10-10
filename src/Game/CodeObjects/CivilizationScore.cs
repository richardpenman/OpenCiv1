using IRB.VirtualCPU;
using OpenCivOne.Graphics;

namespace OpenCivOne
{
	/// <summary>
	/// The civilization score screen
	/// </summary>
	public class CivilizationScore
	{
		private OpenCivOneGame oParent;
		private VCPU oCPU;

		public CivilizationScore(OpenCivOneGame parent)
		{
			this.oParent = parent;
			this.oCPU = parent.CPU;
		}

		/// <summary>
		/// Shows Civilization Score window.
		/// </summary>
		/// <param name="playerID"></param>
		/// <param name="flag"></param>
		/// <returns></returns>
		public ushort F20_0000_0ca9_ShowCivilizationScorePopup(short playerID, bool flag)
		{
			this.oCPU.Log.EnterBlock($"F20_0000_0ca9_ShowCivilizationScorePopup({playerID}, {flag})");

			// function body
			this.oCPU.PUSH_UInt16(this.oCPU.BP.UInt16);
			this.oCPU.BP.UInt16 = this.oCPU.SP.UInt16;
			this.oCPU.SP.UInt16 = this.oCPU.SUB_UInt16(this.oCPU.SP.UInt16, 0x1a);
			this.oCPU.PUSH_UInt16(this.oCPU.SI.UInt16);
			this.oCPU.WriteUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x2), 0x0);
			
			if (!flag) goto L0cc3;

			this.oParent.Var_aa_Screen0_Rectangle.ScreenID = 1;

		L0cc3:
			if (!flag) goto L0cd9;

			this.oParent.AdvisorReports.ShowReportHeader("CIVILIZATION SCORE", 3);

		L0cd9:
			this.oParent.GameData.Players[0].Score = 0;
			
			if (!flag) goto L0d3e;

			this.oCPU.TEST_UInt16((ushort)this.oParent.GameData.SpaceshipFlags, 0x100);
			if (this.oCPU.Flags.E) goto L0d3e;

			// Instruction address 0x0000:0x0cfd, size: 5
			this.oParent.DrawTools.DrawCenteredStringToScreen0("SCORING COMPLETED", 160, 100, 15);

			this.oParent.Var_aa_Screen0_Rectangle.ScreenID = 0;

			// Instruction address 0x0000:0x0d22, size: 5
			this.oParent.Graphics.F0_VGA_07d8_DrawImage(this.oParent.Var_19d4_Screen1_Rectangle, 0, 0, 320, 200, this.oParent.Var_aa_Screen0_Rectangle, 0, 0);

			// Instruction address 0x0000:0x0d2a, size: 5
			this.oParent.Actions.F0_2459_0918_WaitForKeyPressOrMouseClick();

			// Instruction address 0x0000:0x0d34, size: 5
			this.oParent.GameEngine.F0_1238_1b44_RedrawMapWithBlankingEffect();

			this.oCPU.AX.UInt16 = 0;
			goto L13f3;

		L0d3e:
			this.oCPU.WriteUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x8), 0x1);
			this.oCPU.WriteUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x12), 0x1a);
			this.oCPU.WriteUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0xc), 0x1a);
			this.oCPU.WriteUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0xe), 0x3e7);
			this.oCPU.AX.UInt16 = 0;
			this.oCPU.WriteUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x6), this.oCPU.AX.UInt16);
			this.oCPU.WriteUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x4), this.oCPU.AX.UInt16);
			this.oCPU.WriteUInt16(this.oCPU.DS.UInt16, 0xb25c, 0x3e7);
			this.oCPU.WriteUInt16(this.oCPU.DS.UInt16, 0xb2bc, 0x1a);
			this.oCPU.AX.UInt16 = (ushort)this.oParent.GameData.Players[playerID].TaxRate;
			this.oCPU.WriteUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0xa), this.oCPU.AX.UInt16);
			this.oParent.GameData.Players[playerID].TaxRate = (short)(6 - this.oParent.GameData.Players[playerID].ScienceTaxRate);
			this.oCPU.WriteUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x10), 0x0);
			goto L0e3c;

		L0d85:
			F20_0000_13f8((ushort)(this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x14)) & 1));
			
			this.oCPU.WriteUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x14), 
				this.oCPU.INC_UInt16(this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x14))));

		L0d96:
			this.oCPU.AX.UInt16 = (ushort)((short)this.oParent.Var_70e2_CityHappyCitizenCount);
			this.oCPU.CMP_UInt16(this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x14)), this.oCPU.AX.UInt16);
			if (this.oCPU.Flags.L) goto L0d85;
			this.oCPU.WriteUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x14), 0x0);
			goto L0db8;

		L0da5:
			F20_0000_13f8((ushort)((this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x14)) & 1) + 2));
			
			this.oCPU.WriteUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x14), 
				this.oCPU.INC_UInt16(this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x14))));

		L0db8:
			this.oCPU.AX.UInt16 = 0x1c;
			this.oCPU.IMUL_UInt16(this.oCPU.AX, this.oCPU.DX, this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x10)));
			this.oCPU.BX.UInt16 = this.oCPU.AX.UInt16;
			this.oCPU.AX.LowUInt8 = (byte)this.oParent.GameData.Cities[this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x10))].ActualSize;
			this.oCPU.CBW(this.oCPU.AX);
			this.oCPU.AX.UInt16 = this.oCPU.SUB_UInt16(this.oCPU.AX.UInt16, (ushort)((short)this.oParent.Var_70e2_CityHappyCitizenCount));
			this.oCPU.AX.UInt16 = this.oCPU.SUB_UInt16(this.oCPU.AX.UInt16, (ushort)((short)this.oParent.Var_70e4_CityUnhappyCitizenCount));
			this.oCPU.AX.UInt16 = this.oCPU.SUB_UInt16(this.oCPU.AX.UInt16, (ushort)((short)this.oParent.Var_e8b8));
			this.oCPU.CMP_UInt16(this.oCPU.AX.UInt16, this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x14)));
			if (this.oCPU.Flags.G) goto L0da5;
			this.oCPU.WriteUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x14), 0x0);
			goto L0df1;

		L0ddd:
			F20_0000_13f8((ushort)((this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x14)) & 1) + 4));
			
			this.oCPU.WriteUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x14), 
				this.oCPU.INC_UInt16(this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x14))));

		L0df1:
			this.oCPU.AX.UInt16 = (ushort)((short)this.oParent.Var_70e4_CityUnhappyCitizenCount);
			this.oCPU.CMP_UInt16(this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x14)), this.oCPU.AX.UInt16);
			if (this.oCPU.Flags.L) goto L0ddd;
			this.oCPU.WriteUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x14), 0x0);
			goto L0e19;

		L0e00:
			// Instruction address 0x0000:0x0e03, size: 5
			this.oCPU.AX.Int16 = (short)this.oParent.CityWorker.F0_1d12_6da1_GetSpecialWorkerFlags(this.oCPU.ReadInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x14)));

			F20_0000_13f8((ushort)(this.oCPU.AX.UInt16 + 5));
			
			this.oCPU.WriteUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x14), 
				this.oCPU.INC_UInt16(this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x14))));

		L0e19:
			this.oCPU.AX.UInt16 = (ushort)((short)this.oParent.Var_e8b8);
			this.oCPU.CMP_UInt16(this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x14)), this.oCPU.AX.UInt16);
			if (this.oCPU.Flags.L) goto L0e00;

		L0e21:
			this.oCPU.AX.UInt16 = 0x1c;
			this.oCPU.IMUL_UInt16(this.oCPU.AX, this.oCPU.DX, this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x10)));
			this.oCPU.BX.UInt16 = this.oCPU.AX.UInt16;
			this.oCPU.AX.LowUInt8 = (byte)this.oParent.GameData.Cities[this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x10))].ActualSize;
			this.oCPU.CBW(this.oCPU.AX);
			this.oCPU.AX.UInt16 = this.oCPU.ADD_UInt16(this.oCPU.AX.UInt16, (ushort)((short)this.oParent.Var_70e2_CityHappyCitizenCount));
			this.oCPU.AX.UInt16 = this.oCPU.SUB_UInt16(this.oCPU.AX.UInt16, (ushort)((short)this.oParent.Var_70e4_CityUnhappyCitizenCount));
			this.oCPU.WriteUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x4), this.oCPU.ADD_UInt16(this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x4)), this.oCPU.AX.UInt16));

		L0e39:
			this.oCPU.WriteUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x10), 
				this.oCPU.INC_UInt16(this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x10))));

		L0e3c:
			this.oCPU.CMP_UInt16(this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x10)), 0x80);
			if (this.oCPU.Flags.GE) goto L0e79;

			this.oCPU.AX.UInt16 = 0x1c;
			this.oCPU.IMUL_UInt16(this.oCPU.AX, this.oCPU.DX, this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x10)));
			this.oCPU.SI.UInt16 = this.oCPU.AX.UInt16;
			
			if (this.oParent.GameData.Cities[this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x10))].Status == CityStatusEnum.Invalid) goto L0e39;
			
			if (this.oParent.GameData.Cities[this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x10))].PlayerID != playerID)
				goto L0e39;

			// Instruction address 0x0000:0x0e63, size: 5
			this.oParent.CityWorker.F0_1d12_0045_ProcessCityState(this.oCPU.ReadInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x10)), -1);

			if (!flag) goto L0e21;
			this.oCPU.WriteUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x14), 0x0);
			goto L0d96;

		L0e79:
			this.oCPU.SI.UInt16 = (ushort)playerID;
			this.oCPU.SI.UInt16 = this.oCPU.SHL_UInt16(this.oCPU.SI.UInt16, 0x1);

			this.oCPU.AX.UInt16 = this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0xa));
			this.oParent.GameData.Players[playerID].TaxRate = (short)this.oCPU.AX.UInt16;

			this.oParent.GameData.Players[this.oParent.GameData.HumanPlayerID].TotalCitizenCount = this.oCPU.ReadInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x4));

			// Instruction address 0x0000:0x0e93, size: 5
			this.oParent.CAPI.strcpy(0xba06, this.oParent.GameData.Players[playerID].Nationality);

			// Instruction address 0x0000:0x0ea3, size: 5
			this.oParent.CAPI.strcat(0xba06, " Citizens (");

			// Instruction address 0x0000:0x0ec4, size: 5
			this.oParent.CAPI.strcat(0xba06,
				this.oParent.CAPI.itoa(this.oParent.GameData.Players[this.oParent.GameData.HumanPlayerID].TotalCitizenCount, 10));

			// Instruction address 0x0000:0x0ed4, size: 5
			this.oParent.CAPI.strcat(0xba06, ")");

			if (!flag) goto L0ef9;

			// Instruction address 0x0000:0x0ef1, size: 5
			this.oParent.DrawTools.DrawStringWithShadow(0xba06, 16, this.oCPU.ReadInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0xc)), 15);

		L0ef9:
			this.oCPU.AX.UInt16 = this.oCPU.ReadUInt16(this.oCPU.DS.UInt16, 0xb2bc);
			this.oCPU.AX.UInt16 = this.oCPU.ADD_UInt16(this.oCPU.AX.UInt16, 0x10);
			this.oCPU.WriteUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x12), this.oCPU.AX.UInt16);
			this.oCPU.WriteUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0xc), this.oCPU.AX.UInt16);
			this.oCPU.WriteUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0xe), 0x3e7);

			this.oParent.GameData.Players[this.oParent.GameData.HumanPlayerID].AchievementScore = 0;
			this.oCPU.WriteUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x10), 0x1);

		L0f15:
			this.oCPU.SI.UInt16 = (ushort)this.oParent.GameData.WonderCityID[this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x10))];
			this.oCPU.CMP_UInt16(this.oCPU.SI.UInt16, 0xffff);
			if (this.oCPU.Flags.NE) goto L0f26;
			goto L0fb9;

		L0f26:
			this.oCPU.CMP_UInt16(this.oCPU.SI.UInt16, 0x80);
			if (this.oCPU.Flags.NE) goto L0f2f;
			goto L0fb9;

		L0f2f:
			this.oCPU.AX.UInt16 = 0x1c;
			this.oCPU.IMUL_UInt16(this.oCPU.AX, this.oCPU.DX, this.oCPU.SI.UInt16);
			this.oCPU.BX.UInt16 = this.oCPU.AX.UInt16;
			
			if (this.oParent.GameData.Cities[this.oCPU.SI.UInt16].PlayerID != playerID) goto L0fb9;

			this.oCPU.WriteUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0xe), 
				this.oCPU.ADD_UInt16(this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0xe)), 0x98));
			this.oCPU.CMP_UInt16(this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0xe)), 0x12c);
			if (this.oCPU.Flags.LE) goto L0f6a;

			// Instruction address 0x0000:0x0f5a, size: 5
			this.oCPU.AX.Int16 = (short)this.oParent.CommonTools.CheckValueRange(
				this.oCPU.ReadInt16(this.oCPU.DS.UInt16, 0x5116) + 2, 8, 99);

			this.oCPU.WriteUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x12), this.oCPU.ADD_UInt16(this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x12)), this.oCPU.AX.UInt16));
			this.oCPU.WriteUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0xe), 0x10);

		L0f6a:
			this.oCPU.AX.UInt16 = this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x10));
			this.oCPU.AX.UInt16 = this.oCPU.ADD_UInt16(this.oCPU.AX.UInt16, 0x18);
			this.oCPU.WriteUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x14), this.oCPU.AX.UInt16);

			if (!flag) goto L0fb4;

			this.oCPU.CMP_UInt16(this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x12)), 0xb4);
			if (this.oCPU.Flags.GE) goto L0fb4;

			// Instruction address 0x0000:0x0f8c, size: 5
			this.oParent.CityWorker.F0_1d12_7045_DrawWonderOrImprovement(this.oCPU.ReadInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x14)),
				this.oCPU.ReadInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0xe)),
				(short)(this.oCPU.ReadInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x12)) - 2));

			// Instruction address 0x0000:0x0fac, size: 5
			this.oParent.DrawTools.DrawStringToScreen0(
				this.oParent.GameData.Wonders[this.oCPU.ReadInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x10))].Name,
				this.oCPU.ReadInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0xe)) + 20,
				this.oCPU.ReadInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x12)),
				11);

		L0fb4:
			this.oParent.GameData.Players[this.oParent.GameData.HumanPlayerID].AchievementScore += 20;

		L0fb9:
			if (this.oParent.GameData.WonderCityID[this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x10))] != -1)
				goto L0fca;
			this.oCPU.WriteUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x8), 0x0);

		L0fca:
			this.oCPU.WriteUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x10), 
				this.oCPU.INC_UInt16(this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x10))));
			this.oCPU.CMP_UInt16(this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x10)), 0x15);
			if (this.oCPU.Flags.G) goto L0fd6;
			goto L0f15;

		L0fd6:
			// Instruction address 0x0000:0x0fe3, size: 5
			this.oParent.CAPI.strcpy(0xba06, this.oParent.GameData.Players[playerID].Nationality);

			// Instruction address 0x0000:0x0ff3, size: 5
			this.oParent.CAPI.strcat(0xba06, " Achievements (");

			// Instruction address 0x0000:0x1014, size: 5
			this.oParent.CAPI.strcat(0xba06,
				this.oParent.CAPI.itoa(this.oParent.GameData.Players[this.oParent.GameData.HumanPlayerID].AchievementScore, 10));

			// Instruction address 0x0000:0x1024, size: 5
			this.oParent.CAPI.strcat(0xba06, ")");

			if (!flag) goto L1049;

			// Instruction address 0x0000:0x1041, size: 5
			this.oParent.DrawTools.DrawStringWithShadow(0xba06, 16, this.oCPU.ReadInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0xc)), 15);

		L1049:
			this.oParent.GameData.Players[0].Score = (short)(this.oParent.GameData.Players[this.oParent.GameData.HumanPlayerID].AchievementScore +
				this.oParent.GameData.Players[this.oParent.GameData.HumanPlayerID].TotalCitizenCount);

			this.oCPU.WriteUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x12), this.oCPU.ADD_UInt16(this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x12)), 0xa));
			this.oCPU.SI.UInt16 = (ushort)this.oParent.GameData.Players[oParent.GameData.HumanPlayerID].SpaceshipPopulation;
			this.oCPU.SI.UInt16 = this.oCPU.OR_UInt16(this.oCPU.SI.UInt16, this.oCPU.SI.UInt16);
			if (this.oCPU.Flags.NE) goto L1068;
			goto L1118;

		L1068:
			if (this.oParent.GameData.Players[this.oParent.GameData.HumanPlayerID].SpaceshipSuccessRate != 0) goto L1072;
			goto L1118;

		L1072:
			this.oCPU.AX.UInt16 = (ushort)this.oParent.GameData.Players[this.oParent.GameData.HumanPlayerID].SpaceshipSuccessRate;
			this.oCPU.IMUL_UInt16(this.oCPU.AX, this.oCPU.DX, this.oCPU.SI.UInt16);
			this.oCPU.CWD(this.oCPU.AX, this.oCPU.DX);
			this.oCPU.AX.UInt16 = this.oCPU.SUB_UInt16(this.oCPU.AX.UInt16, this.oCPU.DX.UInt16);
			this.oCPU.AX.UInt16 = this.oCPU.SAR_UInt16(this.oCPU.AX.UInt16, 0x1);
			this.oCPU.WriteUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0xe), this.oCPU.AX.UInt16);
			this.oCPU.AX.UInt16 = this.oCPU.OR_UInt16(this.oCPU.AX.UInt16, this.oCPU.AX.UInt16);
			if (this.oCPU.Flags.GE) goto L1088;
			this.oCPU.WriteUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0xe), 0x0);

		L1088:
			// Instruction address 0x0000:0x1090, size: 5
			this.oParent.CAPI.strcpy(0xba06, "SpaceShip: (");

			// Instruction address 0x0000:0x10b0, size: 5
			this.oParent.CAPI.strcat(0xba06,
				this.oParent.CAPI.itoa((short)this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0xe)), 10));
			
			if (this.oParent.GameData.Players[oParent.GameData.HumanPlayerID].SpaceshipETAYear != oParent.GameData.Year) goto L10d0;
			this.oCPU.AX.UInt16 = this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0xe));
			this.oParent.GameData.Players[0].Score += (short)this.oCPU.AX.UInt16;
			goto L10e0;

		L10d0:
			// Instruction address 0x0000:0x10d8, size: 5
			this.oParent.CAPI.strcat(0xba06, " possible");

		L10e0:
			// Instruction address 0x0000:0x10e8, size: 5
			this.oParent.CAPI.strcat(0xba06, ")");

			if (!flag) goto L1114;

			this.oCPU.CMP_UInt16(this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x12)), 0xb8);
			if (this.oCPU.Flags.GE) goto L1114;

			// Instruction address 0x0000:0x110c, size: 5
			this.oParent.DrawTools.DrawStringWithShadow(0xba06, 16, this.oCPU.ReadInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x12)), 15);

		L1114:
			this.oCPU.WriteUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x12), this.oCPU.ADD_UInt16(this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x12)), 0xa));

		L1118:
			this.oCPU.WriteUInt8(this.oCPU.DS.UInt16, 0xba06, 0x0);
			if (this.oParent.GameData.PollutedSquareCount == 0) goto L1174;

			// Instruction address 0x0000:0x112c, size: 5
			this.oParent.CAPI.strcpy(0xba06, "Pollution: (");

			// Instruction address 0x0000:0x1151, size: 5
			this.oParent.CAPI.strcat(0xba06,
				this.oParent.CAPI.itoa((short)(-10 * this.oParent.GameData.PollutedSquareCount), 10));

			// Instruction address 0x0000:0x1161, size: 5
			this.oParent.CAPI.strcat(0xba06, ")   ");

			this.oCPU.AX.UInt16 = 0xa;
			this.oCPU.IMUL_UInt16(this.oCPU.AX, this.oCPU.DX, (ushort)this.oParent.GameData.PollutedSquareCount);
			this.oParent.GameData.Players[0].Score -= (short)this.oCPU.AX.UInt16;

		L1174:
			if (oParent.GameData.Year <= 0) goto L11e3;
			if (this.oParent.GameData.PeaceTurnCount == 0) goto L11e3;

			// Instruction address 0x0000:0x1191, size: 5
			this.oCPU.AX.Int16 = (short)this.oParent.CommonTools.CheckValueRange(this.oParent.GameData.PeaceTurnCount * 3, 0, 100);

			this.oCPU.WriteUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x16), this.oCPU.AX.UInt16);

			// Instruction address 0x0000:0x11a4, size: 5
			this.oParent.CAPI.strcat(0xba06, "Peace: (+");

			// Instruction address 0x0000:0x11c4, size: 5
			this.oParent.CAPI.strcat(0xba06,
				this.oParent.CAPI.itoa((short)this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x16)), 10));

			// Instruction address 0x0000:0x11d4, size: 5
			this.oParent.CAPI.strcat(0xba06, ") ");

			this.oCPU.AX.UInt16 = this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x16));
			this.oParent.GameData.Players[0].Score += (short)this.oCPU.AX.UInt16;

		L11e3:
			if (this.oParent.GameData.Players[this.oParent.GameData.HumanPlayerID].FutureTechnologyCount == 0) goto L123a;

			// Instruction address 0x0000:0x11f2, size: 5
			this.oParent.CAPI.strcat(0xba06, "Future Tech: (+");

			// Instruction address 0x0000:0x1217, size: 5
			this.oParent.CAPI.strcat(0xba06,
				this.oParent.CAPI.itoa((short)(5 * this.oParent.GameData.Players[this.oParent.GameData.HumanPlayerID].FutureTechnologyCount), 10));

			// Instruction address 0x0000:0x1227, size: 5
			this.oParent.CAPI.strcat(0xba06, ")");

			this.oCPU.AX.UInt16 = 0x5;
			this.oCPU.IMUL_UInt16(this.oCPU.AX, this.oCPU.DX, (ushort)this.oParent.GameData.Players[this.oParent.GameData.HumanPlayerID].FutureTechnologyCount);
			this.oParent.GameData.Players[0].Score += (short)this.oCPU.AX.UInt16;

		L123a:
			this.oCPU.CMP_UInt8(this.oCPU.ReadUInt8(this.oCPU.DS.UInt16, 0xba06), 0x0);
			if (this.oCPU.Flags.E) goto L1269;
			
			if (!flag) goto L1269;

			this.oCPU.CMP_UInt16(this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x12)), 0xb8);
			if (this.oCPU.Flags.GE) goto L1269;

			// Instruction address 0x0000:0x125d, size: 5
			this.oParent.DrawTools.DrawStringWithShadow(0xba06, 16, this.oCPU.ReadInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x12)), 15);

			this.oCPU.WriteUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x12), 
				this.oCPU.ADD_UInt16(this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x12)), 0xa));

		L1269:
			this.oCPU.WriteUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x18), 0xffff);
			
			if (this.oParent.Var_b884 == 0) goto L128e;

			this.oCPU.AX.UInt16 = 0x64;
			this.oCPU.IMUL_UInt16(this.oCPU.AX, this.oCPU.DX, (ushort)this.oParent.GameData.AIOpponentCount);
			this.oCPU.CX.UInt16 = 0x226;
			this.oCPU.CX.UInt16 = this.oCPU.SUB_UInt16(this.oCPU.CX.UInt16, (ushort)this.oParent.GameData.TurnCount);
			this.oCPU.CX.UInt16 = this.oCPU.SHL_UInt16(this.oCPU.CX.UInt16, 0x1);
			this.oCPU.CX.UInt16 = this.oCPU.ADD_UInt16(this.oCPU.CX.UInt16, this.oCPU.AX.UInt16);
			this.oCPU.CX.UInt16 = this.oCPU.ADD_UInt16(this.oCPU.CX.UInt16, 0x190);
			this.oCPU.WriteUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x18), this.oCPU.CX.UInt16);

		L128e:
			if (this.oParent.GameData.Players[0].Score < 0)
			{
				this.oParent.GameData.Players[0].Score = 0;
			}

			this.oCPU.AX.UInt16 = (ushort)this.oParent.GameData.Players[0].Score;
			this.oCPU.CMP_UInt16(this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x18)), this.oCPU.AX.UInt16);
			if (this.oCPU.Flags.GE) goto L12d6;

			// Instruction address 0x0000:0x12ab, size: 5
			this.oParent.CAPI.strcpy(0xba06, "Total Score: ");

			// Instruction address 0x0000:0x12cc, size: 5
			this.oParent.CAPI.strcat(0xba06,
				this.oParent.CAPI.itoa(this.oParent.GameData.Players[0].Score, 10));
			goto L130c;

		L12d6:
			// Instruction address 0x0000:0x12de, size: 5
			this.oParent.CAPI.strcpy(0xba06, "Bonus Score: ");

			// Instruction address 0x0000:0x12fe, size: 5
			this.oParent.CAPI.strcat(0xba06,
				this.oParent.CAPI.itoa((short)this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x18)), 10));

			this.oCPU.AX.UInt16 = this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x18));
			this.oParent.GameData.Players[0].Score = (short)this.oCPU.AX.UInt16;

		L130c:
			if (!flag) goto L1330;

			this.oCPU.CMP_UInt16(this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x12)), 0xba);
			if (this.oCPU.Flags.GE) goto L1330;

			// Instruction address 0x0000:0x1328, size: 5
			this.oParent.DrawTools.DrawStringWithShadow(0xba06, 16, this.oCPU.ReadInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x12)), 15);

		L1330:
			this.oCPU.CMP_UInt16(this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x12)), 0xba);
			if (this.oCPU.Flags.L) goto L1348;
			this.oCPU.WriteUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x2), 0x1);
			this.oCPU.WriteUInt16(this.oCPU.DS.UInt16, 0x5116, this.oCPU.DEC_UInt16(this.oCPU.ReadUInt16(this.oCPU.DS.UInt16, 0x5116)));

			goto L0cc3;

		L1348:
			this.oParent.GameData.Players[this.oParent.GameData.HumanPlayerID].TotalPlayerScore = this.oParent.GameData.Players[0].Score;

			if (!flag)
				goto L13f0;

			// Instruction address 0x0000:0x136f, size: 5
			this.oParent.DrawTools.FillRectangle(this.oParent.Var_aa_Screen0_Rectangle, 35, 192, 250, 7, 8);

			if (this.oParent.GameData.Players[0].Score > 0)
			{
				// Instruction address 0x0000:0x139f, size: 5
				this.oCPU.AX.Int16 = (short)this.oParent.CommonTools.CheckValueRange(this.oParent.GameData.Players[0].Score / 4, 0, 250);

				// Instruction address 0x0000:0x13b4, size: 5
				this.oParent.DrawTools.FillRectangle(this.oParent.Var_aa_Screen0_Rectangle, 35, 192, (short)this.oCPU.AX.UInt16, 7, 11);
			}

			this.oParent.Var_aa_Screen0_Rectangle.ScreenID = 0;

			// Instruction address 0x0000:0x13d9, size: 5
			this.oParent.Graphics.F0_VGA_07d8_DrawImage(this.oParent.Var_19d4_Screen1_Rectangle, 0, 0, 320, 200, this.oParent.Var_aa_Screen0_Rectangle, 0, 0);

			// Instruction address 0x0000:0x13e1, size: 5
			this.oParent.Actions.F0_2459_0918_WaitForKeyPressOrMouseClick();

			// Instruction address 0x0000:0x13eb, size: 5
			this.oParent.GameEngine.F0_1238_1b44_RedrawMapWithBlankingEffect();

		L13f0:
			this.oCPU.AX.UInt16 = (ushort)this.oParent.GameData.Players[0].Score;

		L13f3:
			this.oCPU.SI.UInt16 = this.oCPU.POP_UInt16();
			this.oCPU.SP.UInt16 = this.oCPU.BP.UInt16;
			this.oCPU.BP.UInt16 = this.oCPU.POP_UInt16();

			// Far return
			this.oCPU.Log.ExitBlock("F20_0000_0ca9_ShowCivilizationScorePopup");

			return this.oCPU.AX.UInt16;
		}

		/// <summary>
		/// ?
		/// </summary>
		/// <param name="param1"></param>
		public void F20_0000_13f8(ushort param1)
		{
			this.oCPU.Log.EnterBlock($"F20_0000_13f8({param1})");

			// function body
			this.oCPU.PUSH_UInt16(this.oCPU.BP.UInt16);
			this.oCPU.BP.UInt16 = this.oCPU.SP.UInt16;
			this.oCPU.WriteUInt16(this.oCPU.DS.UInt16, 0xb25c, this.oCPU.ADD_UInt16(this.oCPU.ReadUInt16(this.oCPU.DS.UInt16, 0xb25c), 0x8));
			this.oCPU.CMP_UInt16(this.oCPU.ReadUInt16(this.oCPU.DS.UInt16, 0xb25c), 0x124);
			if (this.oCPU.Flags.LE) goto L141d;
			this.oCPU.AX.UInt16 = this.oCPU.ReadUInt16(this.oCPU.DS.UInt16, 0x5116);
			this.oCPU.WriteUInt16(this.oCPU.DS.UInt16, 0xb2bc, this.oCPU.ADD_UInt16(this.oCPU.ReadUInt16(this.oCPU.DS.UInt16, 0xb2bc), this.oCPU.AX.UInt16));
			this.oCPU.AX.UInt16 = this.oCPU.ReadUInt16(this.oCPU.DS.UInt16, 0xb2bc);
			this.oCPU.AX.UInt16 = this.oCPU.SAR_UInt16(this.oCPU.AX.UInt16, 0x1);
			this.oCPU.AX.UInt16 = this.oCPU.AND_UInt16(this.oCPU.AX.UInt16, 0x4);
			this.oCPU.AX.UInt16 = this.oCPU.ADD_UInt16(this.oCPU.AX.UInt16, 0x10);
			this.oCPU.WriteUInt16(this.oCPU.DS.UInt16, 0xb25c, this.oCPU.AX.UInt16);

		L141d:
			this.oCPU.CMP_UInt16(this.oCPU.ReadUInt16(this.oCPU.DS.UInt16, 0xb2bc), 0xa0);
			if (this.oCPU.Flags.G) goto L1442;

			// Instruction address 0x0000:0x143a, size: 5
			this.oParent.Graphics.F0_VGA_0c3e_DrawBitmapToScreen(
				this.oParent.Var_aa_Screen0_Rectangle,
				this.oCPU.ReadInt16(this.oCPU.DS.UInt16, 0xb25c),
				this.oCPU.ReadInt16(this.oCPU.DS.UInt16, 0xb2bc),
				this.oParent.Array_6e96[param1]);

		L1442:
			this.oCPU.BP.UInt16 = this.oCPU.POP_UInt16();
			// Far return
			this.oCPU.Log.ExitBlock("F20_0000_13f8");
		}
	}
}
