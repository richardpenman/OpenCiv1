using IRB.VirtualCPU;
using OpenCiv1.Graphics;

namespace OpenCiv1
{
	public class Help
	{
		private OpenCiv1Game oParent;
		private VCPU oCPU;

		public Help(OpenCiv1Game parent)
		{
			this.oParent = parent;
			this.oCPU = parent.CPU;
		}

		/// <summary>
		/// ?
		/// </summary>
		/// <param name="stringPtr"></param>
		public void F4_0000_0000(string key)
		{
			//this.oCPU.Log.EnterBlock($"F4_0000_0000(0x{key:x4})");

			// function body
			string content = this.oParent.LanguageTools.F0_2f4d_01ad_GetTextBySectionAndKey("HELP", "*" + key.ToUpper());

			if (!string.IsNullOrEmpty(content))
			{
				// Instruction address 0x0000:0x0047, size: 5
				this.oParent.MainCode.F0_11a8_0268_HideMouse();

				this.oParent.Graphics.F0_VGA_07d8_DrawImage(this.oParent.Var_aa_Screen0_Rectangle, 0, 0, 320, 200, this.oParent.Var_19d4_Screen1_Rectangle, 0, 0);

				// Instruction address 0x0000:0x0065, size: 5
				this.oParent.CommonTools.F0_1000_0bfa_FillRectangle(this.oParent.Var_aa_Screen0_Rectangle, 0, 0, 320, 200, 2);

				// Instruction address 0x0000:0x007c, size: 5
				this.oParent.DrawStringTools.F0_1182_00b3_DrawCenteredStringToScreen0(key, 160, 192, 0);

				// Instruction address 0x0000:0x008e, size: 5
				this.oParent.LanguageTools.F0_2f4d_0088_DrawTextBlock(32, 32, content, 256, 15);

				// Instruction address 0x0000:0x0096, size: 5
				this.oParent.Segment_2459.F0_2459_0918_WaitForKeyPressOrMouseClick();

				this.oParent.Graphics.F0_VGA_07d8_DrawImage(this.oParent.Var_19d4_Screen1_Rectangle, 0, 0, 320, 200, this.oParent.Var_aa_Screen0_Rectangle, 0, 0);

				// Instruction address 0x0000:0x00a0, size: 5
				this.oParent.MainCode.F0_11a8_0250_ShowMouse();
			}
		
			this.oParent.Var_2f9c_MenuBoxHelpRequested = false;
		}

		/// <summary>
		/// ?
		/// </summary>
		/// <param name="playerID"></param>
		/// <param name="unitID"></param>
		public void F4_0000_00af(int playerID, int unitID)
		{
			this.oCPU.Log.EnterBlock($"F4_0000_00af({playerID}, {unitID})");

			// function body
			this.oCPU.PUSH_UInt16(this.oCPU.BP.UInt16);
			this.oCPU.BP.UInt16 = this.oCPU.SP.UInt16;
			this.oCPU.SP.UInt16 = this.oCPU.SUB_UInt16(this.oCPU.SP.UInt16, 0xc);
			this.oCPU.PUSH_UInt16(this.oCPU.DI.UInt16);
			this.oCPU.PUSH_UInt16(this.oCPU.SI.UInt16);
			
			if (unitID < 128) goto L00c1;
			goto L02cd;

		L00c1:
			this.oCPU.AX.UInt16 = 0x600;
			this.oCPU.IMUL_UInt16(this.oCPU.AX, this.oCPU.DX, (ushort)((short)playerID));
			this.oCPU.SI.UInt16 = this.oCPU.AX.UInt16;
			this.oCPU.AX.UInt16 = 0xc;
			this.oCPU.IMUL_UInt16(this.oCPU.AX, this.oCPU.DX, (ushort)((short)unitID));
			this.oCPU.SI.UInt16 = this.oCPU.ADD_UInt16(this.oCPU.SI.UInt16, this.oCPU.AX.UInt16);

			// Instruction address 0x0000:0x00dd, size: 5
			this.oCPU.AX.Int16 = (short)this.oParent.MapManagement.GetTerrainType(this.oParent.GameData.Players[playerID].Units[unitID].Position.X, 
				this.oParent.GameData.Players[playerID].Units[unitID].Position.Y);

			this.oCPU.WriteUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0xc), this.oCPU.AX.UInt16);

			this.oCPU.AX.UInt16 = 0x13;
			this.oCPU.IMUL_UInt16(this.oCPU.AX, this.oCPU.DX, this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0xc)));
			this.oCPU.DI.UInt16 = this.oCPU.AX.UInt16;

			this.oCPU.AX.LowUInt8 = (byte)this.oParent.GameData.Terrains[this.oCPU.ReadInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0xc))].Production;
			this.oCPU.CBW(this.oCPU.AX);
			this.oCPU.CX.UInt16 = this.oCPU.AX.UInt16;
			this.oCPU.AX.LowUInt8 = (byte)this.oParent.GameData.Terrains[this.oCPU.ReadInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0xc))].Food;
			this.oCPU.CBW(this.oCPU.AX);
			this.oCPU.AX.UInt16 = this.oCPU.ADD_UInt16(this.oCPU.AX.UInt16, this.oCPU.CX.UInt16);
			this.oCPU.CMP_UInt16(this.oCPU.AX.UInt16, 0x2);
			if (this.oCPU.Flags.L) goto L0108;
			this.oCPU.AX.UInt16 = 0x1;
			goto L010a;

		L0108:
			this.oCPU.AX.UInt16 = 0;

		L010a:
			this.oCPU.WriteUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x2), this.oCPU.AX.UInt16);

			// Instruction address 0x0000:0x0120, size: 5
			this.oCPU.AX.Int16 = (short)this.oParent.MapManagement.GetPlayerLandOwnership(this.oParent.GameData.Players[playerID].Units[unitID].Position.X,
				this.oParent.GameData.Players[playerID].Units[unitID].Position.Y);

			this.oCPU.WriteUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x6), this.oCPU.AX.UInt16);
			this.oCPU.CMP_UInt16(this.oCPU.AX.UInt16, 0xa);
			if (this.oCPU.Flags.GE) goto L0135;
			this.oCPU.WriteUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x2), 0x0);

		L0135:
			this.oCPU.WriteUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x8), 0x1);

		L013a:
			this.oCPU.SI.UInt16 = this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x8));
			this.oCPU.SI.UInt16 = this.oCPU.SHL_UInt16(this.oCPU.SI.UInt16, 0x1);

			GPoint direction = this.oParent.MoveDirections[this.oCPU.ReadInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x8))];

			this.oCPU.AX.UInt16 = 0x600;
			this.oCPU.IMUL_UInt16(this.oCPU.AX, this.oCPU.DX, (ushort)((short)playerID));
			this.oCPU.DI.UInt16 = this.oCPU.AX.UInt16;
			this.oCPU.AX.UInt16 = 0xc;
			this.oCPU.IMUL_UInt16(this.oCPU.AX, this.oCPU.DX, (ushort)((short)unitID));
			this.oCPU.DI.UInt16 = this.oCPU.ADD_UInt16(this.oCPU.DI.UInt16, this.oCPU.AX.UInt16);

			// Instruction address 0x0000:0x016c, size: 5
			this.oCPU.AX.Int16 = (short)this.oParent.MapManagement.GetPlayerLandOwnership(this.oParent.GameData.Players[playerID].Units[unitID].Position.X + direction.X,
				this.oParent.GameData.Players[playerID].Units[unitID].Position.Y + direction.Y);

			this.oCPU.CMP_UInt16(this.oCPU.AX.UInt16, this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x6)));
			if (this.oCPU.Flags.LE) goto L017e;
			this.oCPU.WriteUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x2), 0x0);

		L017e:
			this.oCPU.WriteUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x8), this.oCPU.INC_UInt16(this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x8))));
			this.oCPU.CMP_UInt16(this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x8)), 0x8);
			if (this.oCPU.Flags.LE) goto L013a;
			this.oCPU.CMP_UInt16(this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x2)), 0x0);
			if (this.oCPU.Flags.E) goto L01c3;

			this.oCPU.AX.UInt16 = 0x600;
			this.oCPU.IMUL_UInt16(this.oCPU.AX, this.oCPU.DX, (ushort)((short)playerID));
			this.oCPU.SI.UInt16 = this.oCPU.AX.UInt16;
			this.oCPU.AX.UInt16 = 0xc;
			this.oCPU.IMUL_UInt16(this.oCPU.AX, this.oCPU.DX, (ushort)((short)unitID));
			this.oCPU.SI.UInt16 = this.oCPU.ADD_UInt16(this.oCPU.SI.UInt16, this.oCPU.AX.UInt16);

			// Instruction address 0x0000:0x01b5, size: 5
			this.oParent.MapManagement.F0_2aea_0008_DrawVisibleMap(this.oParent.GameData.HumanPlayerID,
				this.oParent.GameData.Players[playerID].Units[unitID].Position.X - 7, this.oParent.GameData.Players[playerID].Units[unitID].Position.Y - 6);

			F4_0000_02d3_ShowInstantAdvicePopup("*BUILDCITY");

			goto L02cd;

		L01c3:
			this.oCPU.AX.UInt16 = 0x600;
			this.oCPU.IMUL_UInt16(this.oCPU.AX, this.oCPU.DX, (ushort)((short)playerID));
			this.oCPU.SI.UInt16 = this.oCPU.AX.UInt16;
			this.oCPU.AX.UInt16 = 0xc;
			this.oCPU.IMUL_UInt16(this.oCPU.AX, this.oCPU.DX, (ushort)((short)unitID));
			this.oCPU.SI.UInt16 = this.oCPU.ADD_UInt16(this.oCPU.SI.UInt16, this.oCPU.AX.UInt16);

			// Instruction address 0x0000:0x01df, size: 5
			this.oParent.Segment_2dc4.F0_2dc4_0102(this.oParent.GameData.Players[playerID].Units[unitID].Position.X, this.oParent.GameData.Players[playerID].Units[unitID].Position.Y);

			this.oCPU.WriteUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x4), this.oCPU.AX.UInt16);
			this.oCPU.CMP_UInt16(this.oCPU.AX.UInt16, 0xffff);
			if (this.oCPU.Flags.NE) goto L01f2;
			goto L02cd;

		L01f2:
			this.oCPU.AX.UInt16 = 0x1c;
			this.oCPU.IMUL_UInt16(this.oCPU.AX, this.oCPU.DX, this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x4)));
			this.oCPU.BX.UInt16 = this.oCPU.AX.UInt16;
			this.oCPU.AX.UInt16 = (ushort)this.oParent.GameData.Cities[this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x4))].PlayerID;
			this.oCPU.CMP_UInt16(this.oCPU.AX.UInt16, (ushort)((short)playerID));
			if (this.oCPU.Flags.E) goto L0207;
			goto L02cd;

		L0207:
			if (this.oParent.Var_6c9a == 1) goto L0211;
			goto L02cd;

		L0211:
			// Instruction address 0x0000:0x021d, size: 5
			this.oCPU.AX.Int16 = (short)this.oParent.MapManagement.F0_2aea_1585_GetVisibleTerrainImprovements(this.oParent.GameData.Players[playerID].Units[unitID].Position.X, 
				this.oParent.GameData.Players[playerID].Units[unitID].Position.Y);

			this.oCPU.WriteUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0xa), this.oCPU.AX.UInt16);
			this.oCPU.WriteUInt8(this.oCPU.DS.UInt16, 0xba06, 0x0);

			// Instruction address 0x0000:0x0230, size: 5
			this.oParent.CAPI.strcat(0xba06, this.oParent.Segment_2459.F0_2459_08c6_GetCityName(this.oCPU.ReadInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x4))));

			// Instruction address 0x0000:0x0240, size: 5
			this.oParent.Array_30b8[0] = this.oCPU.ReadString(VCPU.ToLinearAddress(this.oCPU.DS.UInt16, 0xba06));

			this.oCPU.CMP_UInt16(this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0xc)), 0x4);
			if (this.oCPU.Flags.NE) goto L025f;
			this.oCPU.TEST_UInt8(this.oCPU.ReadUInt8(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0xa)), 0x6);
			if (this.oCPU.Flags.NE) goto L025f;

			F4_0000_02d3_ShowInstantAdvicePopup("*MINING");

		L025f:
			this.oCPU.CMP_UInt16(this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0xc)), 0x1);
			if (this.oCPU.Flags.NE) goto L0298;

			this.oCPU.AX.UInt16 = 0x600;
			this.oCPU.IMUL_UInt16(this.oCPU.AX, this.oCPU.DX, (ushort)((short)playerID));
			this.oCPU.SI.UInt16 = this.oCPU.AX.UInt16;
			this.oCPU.AX.UInt16 = 0xc;
			this.oCPU.IMUL_UInt16(this.oCPU.AX, this.oCPU.DX, (ushort)((short)unitID));
			this.oCPU.SI.UInt16 = this.oCPU.ADD_UInt16(this.oCPU.SI.UInt16, this.oCPU.AX.UInt16);

			// Instruction address 0x0000:0x0281, size: 5
			this.oCPU.AX.UInt16 = (ushort)(this.oParent.CheckPlayerTurn.F0_1403_3fd0_CanIrrigateCell(this.oParent.GameData.Players[playerID].Units[unitID].Position.X, this.oParent.GameData.Players[playerID].Units[unitID].Position.Y) ? 1 : 0);

			this.oCPU.AX.UInt16 = this.oCPU.OR_UInt16(this.oCPU.AX.UInt16, this.oCPU.AX.UInt16);
			if (this.oCPU.Flags.E) goto L0298;
			this.oCPU.TEST_UInt8(this.oCPU.ReadUInt8(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0xa)), 0x6);
			if (this.oCPU.Flags.NE) goto L0298;

			F4_0000_02d3_ShowInstantAdvicePopup("*IRRIGATE");

			goto L02cd;

		L0298:
			this.oCPU.AX.UInt16 = 0x13;
			this.oCPU.IMUL_UInt16(this.oCPU.AX, this.oCPU.DX, this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0xc)));
			this.oCPU.BX.UInt16 = this.oCPU.AX.UInt16;

			if (this.oParent.GameData.Terrains[this.oCPU.ReadInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0xc))].MovementCost != 1)
				goto L02cd;

			this.oCPU.TEST_UInt8(this.oCPU.ReadUInt8(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0xa)), 0x8);
			if (this.oCPU.Flags.NE) goto L02cd;
			this.oCPU.CMP_UInt16(this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0xc)), 0xb);
			if (this.oCPU.Flags.E) goto L02cd;
			this.oCPU.AX.UInt16 = 0x38;
			this.oCPU.IMUL_UInt16(this.oCPU.AX, this.oCPU.DX, (ushort)((short)playerID));
			this.oCPU.BX.UInt16 = this.oCPU.AX.UInt16;
			
			if (this.oParent.GameData.Players[playerID].ActiveUnits[0] >= 2)
				goto L02cd;

			F4_0000_02d3_ShowInstantAdvicePopup("*ROAD");

		L02cd:
			this.oCPU.SI.UInt16 = this.oCPU.POP_UInt16();
			this.oCPU.DI.UInt16 = this.oCPU.POP_UInt16();
			this.oCPU.SP.UInt16 = this.oCPU.BP.UInt16;
			this.oCPU.BP.UInt16 = this.oCPU.POP_UInt16();
			// Far return
			this.oCPU.Log.ExitBlock("F4_0000_00af");
		}

		/// <summary>
		/// Shows instant advice message.
		/// </summary>
		/// <param name="key"></param>
		public void F4_0000_02d3_ShowInstantAdvicePopup(string key)
		{
			//this.oCPU.Log.EnterBlock($"F4_0000_02d3_ShowInstantAdvicePopup(0x{stringPtr:x4})");

			// function body
			if (this.oParent.GameData.GameSettingFlags.InstantAdvice && this.oParent.GameData.Year < 0)
			{
				// Instruction address 0x0000:0x02fa, size: 5
				string content = this.oParent.LanguageTools.F0_2f4d_0471_ReplaceKeywords(this.oParent.LanguageTools.F0_2f4d_01ad_GetTextBySectionAndKey("HELP", key));

				// Instruction address 0x0000:0x030a, size: 5
				this.oParent.MainCode.F0_11a8_0268_HideMouse();

				this.oParent.Graphics.F0_VGA_07d8_DrawImage(this.oParent.Var_aa_Screen0_Rectangle, 0, 0, 320, 200, this.oParent.Var_19d4_Screen1_Rectangle, 0, 0);

				int windowHeight = this.oParent.LanguageTools.GetTextBlockHeight(content, 25, 256) + 4;

				// Instruction address 0x0000:0x0334, size: 5
				this.oParent.CommonTools.F0_1000_0bfa_FillRectangle(this.oParent.Var_aa_Screen0_Rectangle, 56, 16, 208, windowHeight, 2);

				// Instruction address 0x0000:0x034d, size: 5
				this.oParent.MenuBoxDialog.F0_2d05_0a05_DrawRectangle(56, 16, 208, windowHeight, 10);

				this.oParent.Var_aa_Screen0_Rectangle.FontID = 2;

				// Instruction address 0x0000:0x036d, size: 5
				this.oParent.DrawStringTools.F0_1182_00b3_DrawCenteredStringToScreen0("--- Civilization Note ---", 160, 19, 0);

				// Instruction address 0x0000:0x0385, size: 5
				this.oParent.LanguageTools.F0_2f4d_0088_DrawTextBlock(64, 25, content, 256, 15);

				this.oParent.Var_aa_Screen0_Rectangle.FontID = 1;

				// Instruction address 0x0000:0x0396, size: 5
				this.oParent.Segment_2459.F0_2459_0918_WaitForKeyPressOrMouseClick();

				this.oParent.Graphics.F0_VGA_07d8_DrawImage(this.oParent.Var_19d4_Screen1_Rectangle, 0, 0, 320, 200, this.oParent.Var_aa_Screen0_Rectangle, 0, 0);

				// Instruction address 0x0000:0x03a0, size: 5
				this.oParent.MainCode.F0_11a8_0250_ShowMouse();
			}
		}

		/// <summary>
		/// Shows civilization note warning message
		/// </summary>
		/// <param name="stringPtr"></param>
		public void F4_0000_03aa_ShowInstantWarningPopup(string key)
		{
			//this.oCPU.Log.EnterBlock($"F4_0000_03aa_ShowInstantWarningPopup(0x{stringPtr:x4})");

			// function body			
			if (this.oParent.Var_6b90 == this.oParent.GameData.HumanPlayerID)
			{
				// Instruction address 0x0000:0x03c9, size: 5
				string content = this.oParent.LanguageTools.F0_2f4d_0471_ReplaceKeywords(this.oParent.LanguageTools.F0_2f4d_01ad_GetTextBySectionAndKey("ERROR", key));

				// Instruction address 0x0000:0x03d9, size: 5
				this.oParent.MainCode.F0_11a8_0268_HideMouse();

				this.oParent.Graphics.F0_VGA_07d8_DrawImage(this.oParent.Var_aa_Screen0_Rectangle, 0, 0, 320, 200, this.oParent.Var_19d4_Screen1_Rectangle, 0, 0);

				int windowHeight = this.oParent.LanguageTools.GetTextBlockHeight(content, 25, 256) + 4;

				// Instruction address 0x0000:0x0403, size: 5
				this.oParent.CommonTools.F0_1000_0bfa_FillRectangle(this.oParent.Var_aa_Screen0_Rectangle, 56, 16, 208, windowHeight, 4);

				// Instruction address 0x0000:0x041c, size: 5
				this.oParent.MenuBoxDialog.F0_2d05_0a05_DrawRectangle(56, 16, 208, windowHeight, 12);

				this.oParent.Var_aa_Screen0_Rectangle.FontID = 2;

				// Instruction address 0x0000:0x043c, size: 5
				this.oParent.DrawStringTools.F0_1182_00b3_DrawCenteredStringToScreen0("--- Civilization Note ---", 160, 19, 0);

				// Instruction address 0x0000:0x0454, size: 5
				this.oParent.LanguageTools.F0_2f4d_0088_DrawTextBlock(64, 25, content, 256, 15);

				this.oParent.Var_aa_Screen0_Rectangle.FontID = 1;

				// Instruction address 0x0000:0x0465, size: 5
				this.oParent.Segment_2459.F0_2459_0918_WaitForKeyPressOrMouseClick();

				this.oParent.Graphics.F0_VGA_07d8_DrawImage(this.oParent.Var_19d4_Screen1_Rectangle, 0, 0, 320, 200, this.oParent.Var_aa_Screen0_Rectangle, 0, 0);

				// Instruction address 0x0000:0x046f, size: 5
				this.oParent.MainCode.F0_11a8_0250_ShowMouse();
			}
		}
	}
}
