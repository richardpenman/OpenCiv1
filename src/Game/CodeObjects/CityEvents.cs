using IRB.VirtualCPU;
using OpenCivOne.Graphics;

namespace OpenCivOne
{
	/// <summary>
	/// Founding cities, and random events in cities: disasters and riots
	/// </summary>
	public class CityEvents
	{
		private OpenCivOneGame oParent;
		private VCPU oCPU;

		public CityEvents(OpenCivOneGame parent)
		{
			this.oParent = parent;
			this.oCPU = parent.CPU;
		}

		/// <summary>
		/// ?
		/// </summary>
		/// <param name="playerID"></param>
		/// <param name="moveDirection"></param>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="citySize"></param>
		/// <returns></returns>
		public ushort F20_0000_0000(int playerID, int x, int y, int citySize)
		{
			this.oCPU.Log.EnterBlock($"F20_0000_0000({playerID}, {x}, {y}, {citySize})");

			// function body
			this.oCPU.PUSH_UInt16(this.oCPU.BP.UInt16);
			this.oCPU.BP.UInt16 = this.oCPU.SP.UInt16;
			this.oCPU.SP.UInt16 = this.oCPU.SUB_UInt16(this.oCPU.SP.UInt16, 0x14);
			this.oCPU.PUSH_UInt16(this.oCPU.SI.UInt16);

			// Instruction address 0x0000:0x0017, size: 5
			
			this.oParent.AIEngine.F0_25fb_3401_PlayerClearContinentPolicies(playerID, UnitRoleTypeEnum.Settler, x, y, 4);

			this.oCPU.WriteUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x8), 0x0);
			goto L0029;

		L0026:
			this.oCPU.WriteUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x8), this.oCPU.INC_UInt16(this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x8))));

		L0029:
			this.oCPU.CMP_UInt16(this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x8)), 0x80);
			if (this.oCPU.Flags.L) goto L0033;
			goto L0502;

		L0033:
			this.oCPU.AX.UInt16 = 0x1c;
			this.oCPU.IMUL_UInt16(this.oCPU.AX, this.oCPU.DX, this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x8)));
			this.oCPU.BX.UInt16 = this.oCPU.AX.UInt16;

			if (this.oParent.GameData.Cities[this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x8))].Status != CityStatusEnum.Invalid) goto L0026;

			this.oCPU.AX.UInt16 = (ushort)this.oParent.GameData.Players[playerID].NationalityID;
			this.oCPU.AX.UInt16 = this.oCPU.DEC_UInt16(this.oCPU.AX.UInt16);
			this.oCPU.WriteUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x10), this.oCPU.AX.UInt16);
			this.oCPU.CMP_UInt16(this.oCPU.AX.UInt16, 0x7);
			if (this.oCPU.Flags.L) goto L0057;
			this.oCPU.WriteUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x10), this.oCPU.DEC_UInt16(this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x10))));

		L0057:
			this.oCPU.CX.LowUInt8 = 0x4;
			this.oCPU.WriteUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x10), this.oCPU.SHL_UInt16(this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x10)), this.oCPU.CX.LowUInt8));
			this.oCPU.AX.UInt16 = this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x10));
			this.oCPU.WriteUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x2), this.oCPU.AX.UInt16);

		L0062:
			this.oCPU.WriteUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0xa), 0x1);

			if (this.oParent.GameData.CityPositions[this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x10))].X == -1)
				goto L00ab;

			this.oCPU.WriteUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0xa), 0x0);
			this.oCPU.AX.UInt16 = this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x2));
			this.oCPU.AX.UInt16 = this.oCPU.ADD_UInt16(this.oCPU.AX.UInt16, 0x10);
			this.oCPU.WriteUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x10), 
				this.oCPU.INC_UInt16(this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x10))));
			this.oCPU.CMP_UInt16(this.oCPU.AX.UInt16, this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x10)));
			if (this.oCPU.Flags.NE) goto L008e;
			this.oCPU.WriteUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x10), 0xe0);

		L008e:
			this.oCPU.CMP_UInt16(this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x10)), 0x100);
			if (this.oCPU.Flags.NE) goto L00ab;
			this.oCPU.WriteUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x10), 0x0);
			this.oCPU.CMP_UInt16(this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x2)), 0x3e7);
			if (this.oCPU.Flags.NE) goto L00a6;
			this.oCPU.WriteUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0xa), 0x1);

		L00a6:
			this.oCPU.WriteUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x2), 0x3e7);

		L00ab:
			this.oCPU.CMP_UInt16(this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0xa)), 0x0);
			if (this.oCPU.Flags.E) goto L0062;
			this.oCPU.AX.UInt16 = 0x1c;
			this.oCPU.IMUL_UInt16(this.oCPU.AX, this.oCPU.DX, this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x8)));
			this.oCPU.BX.UInt16 = this.oCPU.AX.UInt16;
			this.oCPU.AX.LowUInt8 = this.oCPU.ReadUInt8(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x10));
			this.oParent.GameData.Cities[this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x8))].NameID = this.oCPU.AX.LowUInt8;

			this.oParent.GameData.CityPositions[this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x10))] = new GPoint(x, y);

			this.oCPU.AX.UInt16 = 0x1;
			this.oCPU.CX.LowInt8 = (sbyte)playerID;
			this.oCPU.AX.UInt16 = this.oCPU.SHL_UInt16(this.oCPU.AX.UInt16, this.oCPU.CX.LowUInt8);
			this.oCPU.TEST_UInt16(this.oCPU.AX.UInt16, (ushort)this.oParent.GameData.PlayerFlags);
			if (this.oCPU.Flags.E) goto L0118;

			this.oCPU.AX.Int16 = (short)this.oParent.TextBoxDialogues.F23_0000_0000_CityNameDialog(this.oCPU.ReadInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x8)));
			
			this.oCPU.AX.UInt16 = this.oCPU.OR_UInt16(this.oCPU.AX.UInt16, this.oCPU.AX.UInt16);
			if (this.oCPU.Flags.NE) goto L00fe;

		L00f8:
			this.oCPU.AX.UInt16 = 0xffff;
			goto L053b;

		L00fe:
			byte cityNameID = this.oParent.GameData.Cities[this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x8))].NameID;
			string cityName = this.oParent.GameData.CityNames[cityNameID];
			this.oParent.GameData.CityNames[cityNameID] = (cityName.Length > 12) ? cityName.Substring(0, 12) : cityName;

		L0118:
			cityNameID = this.oParent.GameData.Cities[this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x8))].NameID;
			this.oParent.GameData.CityNames[cityNameID] = this.oParent.GameData.CityNames[cityNameID].TrimEnd(' ', '\x0');

			// Instruction address 0x0000:0x016f, size: 5
			this.oParent.UnitManagement.F0_1866_250e_AddReplayData(1, (byte)((sbyte)playerID),
				this.oParent.GameData.Cities[this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x8))].NameID,
				(byte)x, (byte)y);

			// Instruction address 0x0000:0x0181, size: 5
			this.oParent.MapManagement.F0_2aea_16ee_ClearTerrainImprovements(x, y, TerrainImprovementFlagsEnum.Fortress | TerrainImprovementFlagsEnum.Mines);

			// Instruction address 0x0000:0x0193, size: 5
			this.oParent.MapManagement.F0_2aea_1653_SetTerrainImprovements(x, y, TerrainImprovementFlagsEnum.City | TerrainImprovementFlagsEnum.Road);

			// Instruction address 0x0000:0x01a1, size: 5
			this.oCPU.AX.Int16 = (short)this.oParent.MapManagement.GetTerrainType(x, y);

			if (this.oParent.GameData.TerrainModifications[this.oCPU.AX.UInt16].IrrigationEffect < -1)
			{
				// Instruction address 0x0000:0x01c1, size: 5
				this.oParent.MapManagement.F0_2aea_1653_SetTerrainImprovements(x, y, TerrainImprovementFlagsEnum.Irrigation);
			}
		
			this.oCPU.AX.UInt16 = 0x1c;
			this.oCPU.IMUL_UInt16(this.oCPU.AX, this.oCPU.DX, this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x8)));
			this.oCPU.SI.UInt16 = this.oCPU.AX.UInt16;
			this.oParent.GameData.Cities[this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x8))].Status = 0;
			this.oParent.GameData.Cities[this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x8))].Position = new GPoint(x, y);
			this.oParent.GameData.Cities[this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x8))].ActualSize = (sbyte)citySize;
			this.oParent.GameData.Cities[this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x8))].VisibleSize = 0;
			this.oParent.GameData.Cities[this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x8))].WorkerFlags = 0;
			this.oParent.GameData.Cities[this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x8))].BaseTrade = 0;
			this.oParent.GameData.Cities[this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x8))].FoodCount = 0;
			this.oParent.GameData.Cities[this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x8))].ShieldsCount = 0;
			this.oParent.GameData.Cities[this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x8))].ImprovementFlags = 0;
			this.oParent.GameData.Cities[this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x8))].CurrentProductionID = 1;

			// Instruction address 0x0000:0x021d, size: 5
			this.oCPU.AX.UInt16 = (ushort)(this.oParent.Technology.F0_1ade_22b5_PlayerHasTechnology(playerID, TechnologyAdvanceEnum.Gunpowder) ? 1 : 0);
			this.oCPU.AX.UInt16 = this.oCPU.OR_UInt16(this.oCPU.AX.UInt16, this.oCPU.AX.UInt16);
			if (this.oCPU.Flags.NE)
			{
				this.oParent.GameData.Cities[this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x8))].CurrentProductionID = 4;
			}

			// Instruction address 0x0000:0x0235, size: 5
			this.oCPU.AX.UInt16 = (ushort)(this.oParent.Technology.F0_1ade_22b5_PlayerHasTechnology(playerID, TechnologyAdvanceEnum.Conscription) ? 1 : 0);
			this.oCPU.AX.UInt16 = this.oCPU.OR_UInt16(this.oCPU.AX.UInt16, this.oCPU.AX.UInt16);
			if (this.oCPU.Flags.NE)
			{
				this.oCPU.AX.UInt16 = 0x1c;
				this.oCPU.IMUL_UInt16(this.oCPU.AX, this.oCPU.DX, this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x8)));
				this.oCPU.BX.UInt16 = this.oCPU.AX.UInt16;
				this.oParent.GameData.Cities[this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x8))].CurrentProductionID = 5;
			}
		
			this.oCPU.AX.UInt16 = 0x1c;
			this.oCPU.IMUL_UInt16(this.oCPU.AX, this.oCPU.DX, this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x8)));
			this.oCPU.SI.UInt16 = this.oCPU.AX.UInt16;
			this.oParent.GameData.Cities[this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x8))].FirstFortifiedCityUnits[0] = -1;
			this.oParent.GameData.Cities[this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x8))].FirstFortifiedCityUnits[1] = -1;
			this.oCPU.WriteUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x6), 0x1);
			this.oCPU.WriteUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0xc), 0x0);

		L026a:
			this.oCPU.AX.UInt16 = 0x1c;
			this.oCPU.IMUL_UInt16(this.oCPU.AX, this.oCPU.DX, this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0xc)));
			this.oCPU.BX.UInt16 = this.oCPU.AX.UInt16;

			if (this.oParent.GameData.Cities[this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0xc))].PlayerID != playerID) goto L0289;

			this.oCPU.AX.UInt16 = this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x8));
			this.oCPU.CMP_UInt16(this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0xc)), this.oCPU.AX.UInt16);
			if (this.oCPU.Flags.E) goto L0289;
			this.oCPU.WriteUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x6), 0x0);

		L0289:
			this.oCPU.WriteUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0xc), this.oCPU.INC_UInt16(this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0xc))));
			this.oCPU.CMP_UInt16(this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0xc)), 0x80);
			if (this.oCPU.Flags.L) goto L026a;
			this.oCPU.CMP_UInt16(this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x6)), 0x0);
			if (this.oCPU.Flags.E) goto L02ad;
			this.oCPU.AX.UInt16 = 0x1c;
			this.oCPU.IMUL_UInt16(this.oCPU.AX, this.oCPU.DX, this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x8)));
			this.oCPU.BX.UInt16 = this.oCPU.AX.UInt16;
			this.oParent.GameData.Cities[this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x8))].ImprovementFlags = 1;

		L02ad:
			this.oCPU.AX.UInt16 = 0x1c;
			this.oCPU.IMUL_UInt16(this.oCPU.AX, this.oCPU.DX, this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x8)));
			this.oCPU.SI.UInt16 = this.oCPU.AX.UInt16;
			this.oParent.GameData.Cities[this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x8))].BaseTrade = 0;
			this.oParent.GameData.Cities[this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x8))].PlayerID = (short)playerID;
			this.oCPU.WriteUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0xc), 0x0);

		L02c6:
			this.oCPU.AX.UInt16 = 0x1c;
			this.oCPU.IMUL_UInt16(this.oCPU.AX, this.oCPU.DX, this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x8)));
			this.oCPU.SI.UInt16 = this.oCPU.AX.UInt16;

			this.oCPU.BX.UInt16 = this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0xc));

			this.oParent.GameData.Cities[this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x8))].TradeCityIDs[this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0xc))] = -1;
			
			this.oCPU.WriteUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0xc), 
				this.oCPU.INC_UInt16(this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0xc))));

			this.oCPU.CMP_UInt16(this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0xc)), 0x3);
			if (this.oCPU.Flags.L) goto L02c6;

			this.oCPU.AX.UInt16 = 0x1c;
			this.oCPU.IMUL_UInt16(this.oCPU.AX, this.oCPU.DX, this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x8)));
			this.oCPU.BX.UInt16 = this.oCPU.AX.UInt16;

			this.oParent.GameData.Cities[this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x8))].SpecialWorkerFlags = 0;
			this.oCPU.WriteUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0xc), 0x1);

		L02f2:
			this.oCPU.SI.UInt16 = this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0xc));
			this.oCPU.SI.UInt16 = this.oCPU.SHL_UInt16(this.oCPU.SI.UInt16, 0x1);

			GPoint direction = this.oParent.MoveDirections[this.oCPU.ReadInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0xc))];

			// Instruction address 0x0000:0x0307, size: 5
			this.oCPU.AX.Int16 = (short)this.oParent.MapManagement.GetTerrainType(x + direction.X, y + direction.Y);

			this.oCPU.WriteUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0xe), this.oCPU.AX.UInt16);
			this.oCPU.CMP_UInt16(this.oCPU.AX.UInt16, 0xa);
			if (this.oCPU.Flags.NE) goto L0349;

			// Instruction address 0x0000:0x0327, size: 5
			this.oCPU.AX.Int16 = (short)this.oParent.MapManagement.F0_2aea_195d_GetMapGroupSize(x + direction.X, y + direction.Y);

			this.oCPU.CMP_UInt16(this.oCPU.AX.UInt16, 0x100);
			if (this.oCPU.Flags.G) goto L033c;

			if (playerID != this.oParent.GameData.HumanPlayerID)
				goto L0349;

		L033c:
			this.oCPU.AX.UInt16 = 0x1c;
			this.oCPU.IMUL_UInt16(this.oCPU.AX, this.oCPU.DX, this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x8)));
			this.oCPU.BX.UInt16 = this.oCPU.AX.UInt16;
			this.oParent.GameData.Cities[this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x8))].Status |= CityStatusEnum.OceanAccess;

		L0349:
			this.oCPU.CMP_UInt16(this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0xe)), 0xb);
			if (this.oCPU.Flags.E) goto L0355;
			this.oCPU.CMP_UInt16(this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0xe)), 0x5);
			if (this.oCPU.Flags.NE) goto L0362;

		L0355:
			this.oCPU.AX.UInt16 = 0x1c;
			this.oCPU.IMUL_UInt16(this.oCPU.AX, this.oCPU.DX, this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x8)));
			this.oCPU.BX.UInt16 = this.oCPU.AX.UInt16;
			this.oParent.GameData.Cities[this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x8))].Status |= CityStatusEnum.NearRiverOrMountains;

		L0362:
			this.oCPU.WriteUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0xc), this.oCPU.INC_UInt16(this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0xc))));
			this.oCPU.CMP_UInt16(this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0xc)), 0x8);
			if (this.oCPU.Flags.LE) goto L02f2;
			this.oCPU.CMP_UInt16(this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x6)), 0x0);
			if (this.oCPU.Flags.E) goto L0385;

			if (playerID != this.oParent.GameData.HumanPlayerID) goto L0385;

			this.oParent.Help.F4_0000_02d3_ShowInstantAdvicePopup("*FIRSTPRODUCT");

		L0385:
			this.oCPU.WriteUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0xc), 0x0);
			goto L0411;

		L038d:
			// Instruction address 0x0000:0x039b, size: 5
			this.oCPU.AX.Int16 = (short)this.oParent.MapManagement.GetPlayerLandOwnership(this.oCPU.ReadInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x2)),
				this.oCPU.ReadInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x4)));

			this.oCPU.WriteUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x10), this.oCPU.AX.UInt16);
			this.oCPU.CMP_UInt16(this.oCPU.AX.UInt16, 0x8);
			if (this.oCPU.Flags.L) goto L03bf;

			// Instruction address 0x0000:0x03b7, size: 5
			this.oParent.MapManagement.SetPlayerLandOwnership(
				this.oCPU.ReadInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x2)),
				this.oCPU.ReadInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x4)), 8);

		L03bf:
			if (playerID == this.oParent.GameData.HumanPlayerID) goto L040e;

			this.oCPU.CMP_UInt16(this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0xc)), 0x18);
			if (this.oCPU.Flags.G) goto L040e;

			// Instruction address 0x0000:0x03d3, size: 5
			this.oCPU.AX.Int16 = (short)this.oParent.MapManagement.F0_2aea_1585_GetVisibleTerrainImprovements(
				this.oCPU.ReadInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x2)),
				this.oCPU.ReadInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x4)));

			this.oCPU.TEST_UInt8(this.oCPU.AX.LowUInt8, 0x1);
			if (this.oCPU.Flags.E) goto L040e;

			// Instruction address 0x0000:0x03e5, size: 5
			this.oCPU.AX.Int16 = (short)this.oParent.MapManagement.F0_2aea_1369_GetCityOwner(
				this.oCPU.ReadInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x2)),
				this.oCPU.ReadInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x4)));

			this.oCPU.CMP_UInt16(this.oCPU.AX.UInt16, (ushort)this.oParent.GameData.HumanPlayerID);
			if (this.oCPU.Flags.NE) goto L040e;
			this.oCPU.AX.UInt16 = 0x1c;
			this.oCPU.IMUL_UInt16(this.oCPU.AX, this.oCPU.DX, this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x8)));
			this.oCPU.BX.UInt16 = this.oCPU.AX.UInt16;
			this.oParent.GameData.Cities[this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x8))].VisibleSize = 1;
			
			// Instruction address 0x0000:0x0406, size: 5
			this.oParent.MapManagement.F0_2aea_1601_UpdateVisibleCellStatus(x, y);

		L040e:
			this.oCPU.WriteUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0xc), this.oCPU.INC_UInt16(this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0xc))));

		L0411:
			this.oCPU.CMP_UInt16(this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0xc)), 0x2d);
			if (this.oCPU.Flags.GE) goto L0456;
			
			this.oCPU.SI.UInt16 = this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0xc));
			this.oCPU.SI.UInt16 = this.oCPU.SHL_UInt16(this.oCPU.SI.UInt16, 0x1);

			direction = this.oParent.MoveDirections[this.oCPU.ReadInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0xc))];

			// Instruction address 0x0000:0x0424, size: 5
			this.oCPU.AX.Int16 = (short)this.oParent.MapManagement.AdjustXPosition(x + direction.X);
			this.oCPU.WriteUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x2), this.oCPU.AX.UInt16);

			this.oCPU.WriteUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x4), (ushort)((short)(y + direction.Y)));

			this.oCPU.CMP_UInt16(this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0xc)), 0x14);
			if (this.oCPU.Flags.LE) goto L0442;
			goto L038d;

		L0442:
			// Instruction address 0x0000:0x03b7, size: 5
			this.oParent.MapManagement.SetPlayerLandOwnership(
				this.oCPU.ReadInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x2)),
				this.oCPU.ReadInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x4)), playerID);

			goto L03bf;

		L0456:
			if (playerID != this.oParent.GameData.HumanPlayerID) goto L0499;

			if (!this.oParent.GameData.GameSettingFlags.Animations || this.oParent.GlobalPreferences.Debug.FastMode) goto L0484;
			
			this.oParent.Animations.F22_0000_0eaf_FoundNewCityAnimation(this.oCPU.ReadInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x8)));

		L0484:
			// Instruction address 0x0000:0x048a, size: 5
			this.oParent.CityManagement.F0_1ade_03ea(this.oCPU.ReadInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x8)));

			// Instruction address 0x0000:0x0492, size: 5
			this.oParent.GameEngine.F0_1238_1b44_RedrawMapWithBlankingEffect();

			goto L04fd;

		L0499:
			this.oCPU.AX.UInt16 = 0x1c;
			this.oCPU.IMUL_UInt16(this.oCPU.AX, this.oCPU.DX, this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x8)));
			this.oCPU.SI.UInt16 = this.oCPU.AX.UInt16;

			// Instruction address 0x0000:0x04a7, size: 5
			this.oParent.CityManagement.F0_1ade_0421(playerID,
				this.oCPU.ReadInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x8)));
			
			this.oParent.GameData.Cities[this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x8))].CurrentProductionID = (sbyte)this.oCPU.AX.LowUInt8;

			// Instruction address 0x0000:0x04bb, size: 5
			this.oCPU.AX.UInt16 = (ushort)(this.oParent.CityWorker.F0_1d12_6c97_PlayerHasWonder(this.oParent.GameData.HumanPlayerID, WonderEnum.ApolloProgram) ? 1 : 0);
			
			this.oCPU.AX.UInt16 = this.oCPU.OR_UInt16(this.oCPU.AX.UInt16, this.oCPU.AX.UInt16);
			if (this.oCPU.Flags.E) goto L04fd;

			this.oParent.GameData.MapVisibility[this.oParent.GameData.Cities[this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x8))].Position.X,
				this.oParent.GameData.Cities[this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x8))].Position.Y] |= (ushort)(1 << this.oParent.GameData.HumanPlayerID);

			this.oParent.GameData.Cities[this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x8))].VisibleSize = 1;

			// Instruction address 0x0000:0x04f5, size: 5
			this.oParent.MapManagement.F0_2aea_1601_UpdateVisibleCellStatus(
				this.oParent.GameData.Cities[this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x8))].Position.X,
				this.oParent.GameData.Cities[this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x8))].Position.Y);

		L04fd:
			this.oCPU.AX.UInt16 = this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x8));
			goto L053b;

		L0502:
			if (playerID != this.oParent.GameData.HumanPlayerID)
				goto L00f8;

			this.oParent.Var_2f9e_MessageBoxStyle = MenuBoxReportTypeEnum.DomesticAdvisorReport;

			// Instruction address 0x0000:0x051c, size: 5
			this.oParent.CAPI.strcpy(0xba06, this.oParent.LanguageTools.F0_2f4d_044f_GetTextFromKingSection("*PMAX"));

			// Instruction address 0x0000:0x0530, size: 5
			this.oParent.MenuBoxDialog.ShowDialog(this.oCPU.ReadString(this.oCPU.DS.UInt16, 0xba06), 64, 80);

			goto L00f8;

		L053b:
			this.oCPU.SI.UInt16 = this.oCPU.POP_UInt16();
			this.oCPU.SP.UInt16 = this.oCPU.BP.UInt16;
			this.oCPU.BP.UInt16 = this.oCPU.POP_UInt16();

			// Far return
			this.oCPU.Log.ExitBlock("F20_0000_0000");

			return this.oCPU.AX.UInt16;
		}

		/// <summary>
		/// ?
		/// </summary>
		/// <param name="cityID"></param>
		public void F20_0000_0540(short cityID)
		{
			this.oCPU.Log.EnterBlock($"F20_0000_0540({cityID})");

			// function body
			this.oCPU.PUSH_UInt16(this.oCPU.BP.UInt16);
			this.oCPU.BP.UInt16 = this.oCPU.SP.UInt16;
			this.oCPU.SP.UInt16 = this.oCPU.SUB_UInt16(this.oCPU.SP.UInt16, 0x10);
			this.oCPU.PUSH_UInt16(this.oCPU.DI.UInt16);
			this.oCPU.PUSH_UInt16(this.oCPU.SI.UInt16);
			this.oCPU.WriteUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0xc), 0x0);
			this.oCPU.WriteUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0xa), 0x0);

		L0552:
			this.oCPU.SI.UInt16 = this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0xa));
			this.oCPU.SI.UInt16 = this.oCPU.SHL_UInt16(this.oCPU.SI.UInt16, 0x1);

			GPoint direction = this.oParent.MoveDirections[this.oCPU.ReadInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0xa))];

			this.oCPU.AX.UInt16 = 0x1c;
			this.oCPU.IMUL_UInt16(this.oCPU.AX, this.oCPU.DX, (ushort)cityID);
			this.oCPU.DI.UInt16 = this.oCPU.AX.UInt16;

			// Instruction address 0x0000:0x0575, size: 5
			this.oCPU.AX.Int16 = (short)this.oParent.MapManagement.GetTerrainType(
				this.oParent.GameData.Cities[cityID].Position.X + direction.X,
				this.oParent.GameData.Cities[cityID].Position.Y + direction.Y);

			this.oCPU.CX.UInt16 = this.oCPU.AX.UInt16;
			this.oCPU.AX.UInt16 = 0x1;
			this.oCPU.AX.UInt16 = this.oCPU.SHL_UInt16(this.oCPU.AX.UInt16, this.oCPU.CX.LowUInt8);
			this.oCPU.WriteUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0xc), this.oCPU.OR_UInt16(this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0xc)), this.oCPU.AX.UInt16));
			this.oCPU.WriteUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0xa), this.oCPU.INC_UInt16(this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0xa))));
			this.oCPU.CMP_UInt16(this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0xa)), 0x8);
			if (this.oCPU.Flags.LE) goto L0552;
			this.oCPU.WriteUInt8(this.oCPU.DS.UInt16, 0xba06, 0x0);

			// Instruction address 0x0000:0x0598, size: 5
			this.oParent.CAPI.strcat(0xba06, this.oParent.Actions.F0_2459_08c6_GetCityName(cityID));

			// Instruction address 0x0000:0x05a8, size: 5
			this.oParent.Array_30b8[3] = this.oCPU.ReadString(this.oCPU.DS.UInt16, 0xba06);

			this.oCPU.WriteUInt8(this.oCPU.DS.UInt16, 0xba06, 0x0);

			// Instruction address 0x0000:0x05b9, size: 5
			this.oCPU.AX.UInt16 = (ushort)(this.oParent.CAPI.RNG.Next(10));

			this.oCPU.CMP_UInt16(this.oCPU.AX.UInt16, 0x9);
			if (this.oCPU.Flags.BE) goto L05c9;
			goto L0bc6;

		L05c9:
			switch(this.oCPU.AX.UInt16)
			{
				case 0:
					goto L05d1;
				case 1:
					goto L0698;
				case 2:
					goto L0703;
				case 3:
					goto L0750;
				case 4:
					goto L07a4;
				case 5:
					goto L07fe;
				case 6:
					goto L08b1;
				case 7:
					goto L08e8;
				case 8:
					goto L08e8;
				case 9:
					goto L08e8;
			}

		L05d1:
			this.oCPU.TEST_UInt8(this.oCPU.ReadUInt8(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0xc)), 0x10);
			if (this.oCPU.Flags.NE) goto L05da;
			goto L0bc6;

		L05da:
			this.oCPU.AX.UInt16 = 0x1c;
			this.oCPU.IMUL_UInt16(this.oCPU.AX, this.oCPU.DX, (ushort)cityID);
			this.oCPU.BX.UInt16 = this.oCPU.AX.UInt16;
			this.oCPU.TEST_UInt16(this.oParent.GameData.Cities[cityID].ImprovementFlags0, 0xfffe);
			if (this.oCPU.Flags.NE) goto L05ed;
			goto L0bc6;

		L05ed:
			// Instruction address 0x0000:0x05f1, size: 5
			this.oCPU.AX.UInt16 = (ushort)(this.oParent.CAPI.RNG.Next(24));

			this.oCPU.WriteUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x10), this.oCPU.AX.UInt16);
			this.oCPU.AX.UInt16 = this.oCPU.INC_UInt16(this.oCPU.AX.UInt16);
			this.oCPU.CMP_UInt16(this.oCPU.AX.UInt16, 0x1);
			if (this.oCPU.Flags.E) goto L05ed;
			this.oCPU.AX.UInt16 = 0x1c;
			this.oCPU.IMUL_UInt16(this.oCPU.AX, this.oCPU.DX, (ushort)cityID);
			this.oCPU.BX.UInt16 = this.oCPU.AX.UInt16;
			this.oCPU.AX.UInt16 = this.oParent.GameData.Cities[cityID].ImprovementFlags0;
			this.oCPU.DX.UInt16 = this.oParent.GameData.Cities[cityID].ImprovementFlags1;
			this.oCPU.CX.UInt16 = this.oCPU.AX.UInt16;
			this.oCPU.AX.UInt16 = 0x1;
			this.oCPU.BX.UInt16 = this.oCPU.CX.UInt16;
			this.oCPU.CX.LowUInt8 = this.oCPU.ReadUInt8(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x10));
			this.oCPU.AX.UInt16 = this.oCPU.SHL_UInt16(this.oCPU.AX.UInt16, this.oCPU.CX.LowUInt8);
			this.oCPU.CX.UInt16 = this.oCPU.DX.UInt16;
			this.oCPU.CWD(this.oCPU.AX, this.oCPU.DX);
			this.oCPU.BX.UInt16 = this.oCPU.AND_UInt16(this.oCPU.BX.UInt16, this.oCPU.AX.UInt16);
			this.oCPU.CX.UInt16 = this.oCPU.AND_UInt16(this.oCPU.CX.UInt16, this.oCPU.DX.UInt16);
			this.oCPU.CX.UInt16 = this.oCPU.OR_UInt16(this.oCPU.CX.UInt16, this.oCPU.BX.UInt16);
			if (this.oCPU.Flags.E) goto L05ed;
			this.oCPU.AX.UInt16 = 0x1c;
			this.oCPU.IMUL_UInt16(this.oCPU.AX, this.oCPU.DX, (ushort)cityID);
			this.oCPU.BX.UInt16 = this.oCPU.AX.UInt16;
			this.oCPU.AX.UInt16 = 0x1;
			this.oCPU.CX.LowUInt8 = this.oCPU.ReadUInt8(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x10));
			this.oCPU.AX.UInt16 = this.oCPU.SHL_UInt16(this.oCPU.AX.UInt16, this.oCPU.CX.LowUInt8);
			this.oCPU.AX.UInt16 = this.oCPU.NOT_UInt16(this.oCPU.AX.UInt16);
			this.oCPU.CWD(this.oCPU.AX, this.oCPU.DX);
			this.oParent.GameData.Cities[cityID].ImprovementFlags0 &= this.oCPU.AX.UInt16;
			this.oParent.GameData.Cities[cityID].ImprovementFlags1 &= this.oCPU.DX.UInt16;

			// Instruction address 0x0000:0x064c, size: 5
			this.oParent.CAPI.strcpy(0xba06, "Earthquake in ");

			// Instruction address 0x0000:0x0657, size: 5
			this.oParent.CAPI.strcat(0xba06, this.oParent.Actions.F0_2459_08c6_GetCityName(cityID));

			// Instruction address 0x0000:0x0667, size: 5
			this.oParent.CAPI.strcat(0xba06, "!\n");

			// Instruction address 0x0000:0x067d, size: 5
			this.oParent.CAPI.strcat(0xba06,
				this.oParent.GameData.GetImprovementType(this.oCPU.ReadInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x10)) + 1).Name);

			// Instruction address 0x0000:0x068d, size: 5
			this.oParent.CAPI.strcat(0xba06, " destroyed.\n");

			goto L0bc6;

		L0698:
			this.oCPU.AX.UInt16 = 0x1c;
			this.oCPU.IMUL_UInt16(this.oCPU.AX, this.oCPU.DX, (ushort)cityID);
			this.oCPU.SI.UInt16 = this.oCPU.AX.UInt16;

			// Instruction address 0x0000:0x06aa, size: 5
			this.oCPU.AX.UInt16 = (ushort)(this.oParent.Technology.F0_1ade_22b5_PlayerHasTechnology(this.oParent.GameData.Cities[cityID].PlayerID, TechnologyAdvanceEnum.Medicine) ? 1 : 0);
			this.oCPU.AX.UInt16 = this.oCPU.OR_UInt16(this.oCPU.AX.UInt16, this.oCPU.AX.UInt16);
			if (this.oCPU.Flags.E)
				goto L06b9;

			goto L0bc6;

		L06b9:
			this.oCPU.TEST_UInt16(this.oParent.GameData.Cities[cityID].ImprovementFlags0, 0x100);
			if (this.oCPU.Flags.E) goto L06c4;
			goto L0bc6;

		L06c4:
			// Instruction address 0x0000:0x06ce, size: 5
			this.oCPU.AX.UInt16 = (ushort)(this.oParent.Technology.F0_1ade_22b5_PlayerHasTechnology(this.oParent.GameData.Cities[cityID].PlayerID,
				(TechnologyAdvanceEnum)this.oCPU.ReadInt16(this.oCPU.DS.UInt16, 0xcc4)) ? 1 : 0);
			this.oCPU.AX.UInt16 = this.oCPU.OR_UInt16(this.oCPU.AX.UInt16, this.oCPU.AX.UInt16);
			if (this.oCPU.Flags.NE)
				goto L06dd;

			goto L0bc6;

		L06dd:
			// Instruction address 0x0000:0x06e1, size: 5
			this.oParent.CAPI.strcpy(0xba06, this.oParent.LanguageTools.F0_2f4d_044f_GetTextFromKingSection("*D1"));

			this.oCPU.CMP_UInt8((byte)this.oParent.GameData.Cities[cityID].ActualSize, 0x0);
			if (this.oCPU.Flags.NE) goto L06f3;
			goto L0bc6;

		L06f3:
			this.oCPU.AX.LowUInt8 = (byte)this.oParent.GameData.Cities[cityID].ActualSize;
			this.oCPU.CBW(this.oCPU.AX);
			this.oCPU.CX.LowUInt8 = 0x4;

		L06fa:
			this.oCPU.IDIV_UInt8(this.oCPU.AX, this.oCPU.CX.LowUInt8);
			this.oParent.GameData.Cities[cityID].ActualSize -= (sbyte)this.oCPU.AX.LowUInt8;
			goto L0bc6;

		L0703:
			this.oCPU.TEST_UInt16(this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0xc)), 0x800);
			if (this.oCPU.Flags.NE) goto L070d;
			goto L0bc6;

		L070d:
			this.oCPU.AX.UInt16 = 0x1c;
			this.oCPU.IMUL_UInt16(this.oCPU.AX, this.oCPU.DX, (ushort)cityID);
			this.oCPU.SI.UInt16 = this.oCPU.AX.UInt16;
			this.oCPU.TEST_UInt16(this.oParent.GameData.Cities[cityID].ImprovementFlags0, 0x80);
			if (this.oCPU.Flags.E) goto L071f;
			goto L0bc6;

		L071f:
			// Instruction address 0x0000:0x0729, size: 5
			this.oCPU.AX.UInt16 = (ushort)(this.oParent.Technology.F0_1ade_22b5_PlayerHasTechnology(this.oParent.GameData.Cities[cityID].PlayerID,
				(TechnologyAdvanceEnum)this.oCPU.ReadInt16(this.oCPU.DS.UInt16, 0xca6)) ? 1 : 0);
			this.oCPU.AX.UInt16 = this.oCPU.OR_UInt16(this.oCPU.AX.UInt16, this.oCPU.AX.UInt16);
			if (this.oCPU.Flags.NE)
				goto L0738;

			goto L0bc6;

		L0738:
			// Instruction address 0x0000:0x073c, size: 5
			this.oParent.CAPI.strcpy(0xba06, this.oParent.LanguageTools.F0_2f4d_044f_GetTextFromKingSection("*D2"));

			if (this.oParent.GameData.Cities[cityID].ActualSize > 1)
				goto L06f3;

			goto L0bc6;

		L0750:
			this.oCPU.TEST_UInt8(this.oCPU.ReadUInt8(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0xc)), 0x20);
			if (this.oCPU.Flags.NE) goto L0759;
			goto L0bc6;

		L0759:
			this.oCPU.AX.UInt16 = 0x1c;
			this.oCPU.IMUL_UInt16(this.oCPU.AX, this.oCPU.DX, (ushort)cityID);
			this.oCPU.SI.UInt16 = this.oCPU.AX.UInt16;
			this.oCPU.TEST_UInt16(this.oParent.GameData.Cities[cityID].ImprovementFlags0, 0x8);
			if (this.oCPU.Flags.E) goto L076b;
			goto L0bc6;

		L076b:
			// Instruction address 0x0000:0x0775, size: 5
			this.oCPU.AX.UInt16 = (ushort)(this.oParent.Technology.F0_1ade_22b5_PlayerHasTechnology(this.oParent.GameData.Cities[cityID].PlayerID,
				(TechnologyAdvanceEnum)this.oCPU.ReadInt16(this.oCPU.DS.UInt16, 0xc2e)) ? 1 : 0);
			this.oCPU.AX.UInt16 = this.oCPU.OR_UInt16(this.oCPU.AX.UInt16, this.oCPU.AX.UInt16);
			if (this.oCPU.Flags.NE)
				goto L0784;

			goto L0bc6;

		L0784:
			// Instruction address 0x0000:0x0788, size: 5
			this.oParent.CAPI.strcpy(0xba06, this.oParent.LanguageTools.F0_2f4d_044f_GetTextFromKingSection("*D3"));

			if (this.oParent.GameData.Cities[cityID].ActualSize > 1)
				goto L079a;

			goto L0bc6;

		L079a:
			this.oCPU.AX.LowUInt8 = (byte)this.oParent.GameData.Cities[cityID].ActualSize;
			this.oCPU.CBW(this.oCPU.AX);
			this.oCPU.CX.LowUInt8 = 0x3;
			goto L06fa;

		L07a4:
			this.oCPU.AX.UInt16 = 0x1c;
			this.oCPU.IMUL_UInt16(this.oCPU.AX, this.oCPU.DX, (ushort)cityID);
			this.oCPU.SI.UInt16 = this.oCPU.AX.UInt16;
			this.oCPU.CMP_UInt8((byte)this.oParent.GameData.Cities[cityID].ActualSize, 0x0);
			if (this.oCPU.Flags.NE) goto L07b6;
			goto L0bc6;

		L07b6:
			this.oCPU.TEST_UInt16(this.oParent.GameData.Cities[cityID].ImprovementFlags0, 0x4);
			if (this.oCPU.Flags.E) goto L07c0;
			goto L0bc6;

		L07c0:
			// Instruction address 0x0000:0x07ca, size: 5
			this.oCPU.AX.UInt16 = (ushort)(this.oParent.Technology.F0_1ade_22b5_PlayerHasTechnology(this.oParent.GameData.Cities[cityID].PlayerID,
				(TechnologyAdvanceEnum)this.oCPU.ReadInt16(this.oCPU.DS.UInt16, 0xc10)) ? 1 : 0);
			this.oCPU.AX.UInt16 = this.oCPU.OR_UInt16(this.oCPU.AX.UInt16, this.oCPU.AX.UInt16);
			if (this.oCPU.Flags.NE)
				goto L07d9;

			goto L0bc6;

		L07d9:
			// Instruction address 0x0000:0x07dd, size: 5
			this.oParent.CAPI.strcpy(0xba06, this.oParent.LanguageTools.F0_2f4d_044f_GetTextFromKingSection("*D4"));

			this.oParent.GameData.Cities[cityID].FoodCount = 0;
			this.oCPU.AX.LowUInt8 = (byte)this.oParent.GameData.Cities[cityID].ActualSize;
			this.oCPU.CBW(this.oCPU.AX);
			this.oCPU.CX.LowUInt8 = 0x3;
			this.oCPU.IDIV_UInt8(this.oCPU.AX, this.oCPU.CX.LowUInt8);
			this.oParent.GameData.Cities[cityID].ActualSize -= (sbyte)this.oCPU.AX.LowUInt8;

			// Instruction address 0x0000:0x068d, size: 5
			this.oParent.CAPI.strcat(0xba06, "Citizens demand GRANARY.\n");

			goto L0bc6;

		L07fe:
			this.oCPU.AX.UInt16 = 0x1c;
			this.oCPU.IMUL_UInt16(this.oCPU.AX, this.oCPU.DX, (ushort)cityID);
			this.oCPU.SI.UInt16 = this.oCPU.AX.UInt16;
			this.oCPU.TEST_UInt16(this.oParent.GameData.Cities[cityID].ImprovementFlags0, 0xfffe);
			if (this.oCPU.Flags.NE) goto L0811;
			goto L0bc6;

		L0811:
			this.oCPU.TEST_UInt16(this.oParent.GameData.Cities[cityID].ImprovementFlags0, 0x100);
			if (this.oCPU.Flags.E) goto L081c;
			goto L0bc6;

		L081c:
			// Instruction address 0x0000:0x0826, size: 5
			this.oCPU.AX.UInt16 = (ushort)(this.oParent.Technology.F0_1ade_22b5_PlayerHasTechnology(this.oParent.GameData.Cities[cityID].PlayerID,
				(TechnologyAdvanceEnum)this.oCPU.ReadInt16(this.oCPU.DS.UInt16, 0xcc4)) ? 1 : 0);
			this.oCPU.AX.UInt16 = this.oCPU.OR_UInt16(this.oCPU.AX.UInt16, this.oCPU.AX.UInt16);
			if (this.oCPU.Flags.NE)
				goto L0835;

			goto L0bc6;

		L0835:
			// Instruction address 0x0000:0x0839, size: 5
			this.oCPU.AX.UInt16 = (ushort)(this.oParent.CAPI.RNG.Next(24));

			this.oCPU.WriteUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x10), this.oCPU.AX.UInt16);
			this.oCPU.AX.UInt16 = this.oCPU.INC_UInt16(this.oCPU.AX.UInt16);
			this.oCPU.CMP_UInt16(this.oCPU.AX.UInt16, 0x1);
			if (this.oCPU.Flags.E) goto L0835;
			this.oCPU.AX.UInt16 = 0x1c;
			this.oCPU.IMUL_UInt16(this.oCPU.AX, this.oCPU.DX, (ushort)cityID);
			this.oCPU.BX.UInt16 = this.oCPU.AX.UInt16;
			this.oCPU.AX.UInt16 = this.oParent.GameData.Cities[cityID].ImprovementFlags0;
			this.oCPU.DX.UInt16 = this.oParent.GameData.Cities[cityID].ImprovementFlags1;
			this.oCPU.CX.UInt16 = this.oCPU.AX.UInt16;
			this.oCPU.AX.UInt16 = 0x1;
			this.oCPU.BX.UInt16 = this.oCPU.CX.UInt16;
			this.oCPU.CX.LowUInt8 = this.oCPU.ReadUInt8(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x10));
			this.oCPU.AX.UInt16 = this.oCPU.SHL_UInt16(this.oCPU.AX.UInt16, this.oCPU.CX.LowUInt8);
			this.oCPU.CX.UInt16 = this.oCPU.DX.UInt16;
			this.oCPU.CWD(this.oCPU.AX, this.oCPU.DX);
			this.oCPU.BX.UInt16 = this.oCPU.AND_UInt16(this.oCPU.BX.UInt16, this.oCPU.AX.UInt16);
			this.oCPU.CX.UInt16 = this.oCPU.AND_UInt16(this.oCPU.CX.UInt16, this.oCPU.DX.UInt16);
			this.oCPU.CX.UInt16 = this.oCPU.OR_UInt16(this.oCPU.CX.UInt16, this.oCPU.BX.UInt16);
			if (this.oCPU.Flags.E) goto L0835;
			this.oCPU.AX.UInt16 = 0x1c;
			this.oCPU.IMUL_UInt16(this.oCPU.AX, this.oCPU.DX, (ushort)cityID);
			this.oCPU.BX.UInt16 = this.oCPU.AX.UInt16;
			this.oCPU.AX.UInt16 = 0x1;
			this.oCPU.CX.LowUInt8 = this.oCPU.ReadUInt8(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x10));
			this.oCPU.AX.UInt16 = this.oCPU.SHL_UInt16(this.oCPU.AX.UInt16, this.oCPU.CX.LowUInt8);
			this.oCPU.AX.UInt16 = this.oCPU.NOT_UInt16(this.oCPU.AX.UInt16);
			this.oCPU.CWD(this.oCPU.AX, this.oCPU.DX);
			this.oParent.GameData.Cities[cityID].ImprovementFlags0 &= this.oCPU.AX.UInt16;
			this.oParent.GameData.Cities[cityID].ImprovementFlags1 &= this.oCPU.DX.UInt16;

			// Instruction address 0x0000:0x089a, size: 5
			this.oParent.Array_30b8[0] = this.oParent.GameData.GetImprovementType(this.oCPU.ReadInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x10)) + 1).Name;

			// Instruction address 0x0000:0x08a6, size: 5
			this.oParent.CAPI.strcpy(0xba06, this.oParent.LanguageTools.F0_2f4d_044f_GetTextFromKingSection("*D5"));

			goto L0bc6;

		L08b1:
			this.oCPU.TEST_UInt16(this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0xc)), 0x400);
			if (this.oCPU.Flags.NE) goto L08bb;
			goto L0bc6;

		L08bb:
			this.oCPU.AX.UInt16 = 0x1c;
			this.oCPU.IMUL_UInt16(this.oCPU.AX, this.oCPU.DX, (ushort)cityID);
			this.oCPU.SI.UInt16 = this.oCPU.AX.UInt16;
			this.oCPU.TEST_UInt16(this.oParent.GameData.Cities[cityID].ImprovementFlags0, 0x2);
			if (this.oCPU.Flags.E) goto L08cd;
			goto L0bc6;

		L08cd:
			// Instruction address 0x0000:0x08d1, size: 5
			this.oParent.CAPI.strcpy(0xba06, this.oParent.LanguageTools.F0_2f4d_044f_GetTextFromKingSection("*D6"));

			this.oParent.GameData.Cities[cityID].FoodCount = 0;
			this.oParent.GameData.Cities[cityID].ShieldsCount = 0;
			goto L0bc6;

		L08e8:
			// Instruction address 0x0000:0x08ef, size: 5
			this.oParent.CityWorker.F0_1d12_0045_ProcessCityState(cityID, -1);

			this.oCPU.WriteUInt8(this.oCPU.DS.UInt16, 0xba06, 0x0);

			this.oCPU.AX.UInt16 = (ushort)((short)this.oParent.Var_70e4_CityUnhappyCitizenCount);
			
			if (this.oParent.Var_70e2_CityHappyCitizenCount >= this.oParent.Var_70e4_CityUnhappyCitizenCount)
				goto L0bc6;

			// Instruction address 0x0000:0x090c, size: 5
			this.oCPU.AX.UInt16 = (ushort)(this.oParent.CAPI.RNG.Next(3));

			this.oCPU.AX.UInt16 = this.oCPU.OR_UInt16(this.oCPU.AX.UInt16, this.oCPU.AX.UInt16);
			if (this.oCPU.Flags.E) goto L0924;
			this.oCPU.CMP_UInt16(this.oCPU.AX.UInt16, 0x1);
			if (this.oCPU.Flags.E) goto L0963;
			this.oCPU.CMP_UInt16(this.oCPU.AX.UInt16, 0x2);
			if (this.oCPU.Flags.E) goto L0968;
			goto L0934;

		L0924:
			// Instruction address 0x0000:0x092c, size: 5
			this.oParent.CAPI.strcpy(0xba06, "Rioting in ");

		L0934:
			// Instruction address 0x0000:0x0937, size: 5
			this.oParent.CAPI.strcat(0xba06, this.oParent.Actions.F0_2459_08c6_GetCityName(cityID));

			// Instruction address 0x0000:0x0947, size: 5
			this.oParent.CAPI.strcat(0xba06, "!\nCitizens demand ");

			this.oCPU.AX.UInt16 = 0x1c;
			this.oCPU.IMUL_UInt16(this.oCPU.AX, this.oCPU.DX, (ushort)cityID);
			this.oCPU.BX.UInt16 = this.oCPU.AX.UInt16;
			this.oCPU.TEST_UInt16(this.oParent.GameData.Cities[cityID].ImprovementFlags0, 0x8);
			if (this.oCPU.Flags.NE) goto L096d;
			
			// Instruction address 0x0000:0x09c8, size: 5
			this.oParent.CAPI.strcat(0xba06, "TEMPLE\n");

			goto L09c3;

		L0963:
			// Instruction address 0x0000:0x092c, size: 5
			this.oParent.CAPI.strcpy(0xba06, "Corruption in ");

			goto L0934;

		L0968:
			// Instruction address 0x0000:0x092c, size: 5
			this.oParent.CAPI.strcpy(0xba06, "Scandal in ");

			goto L0934;

		L096d:
			this.oCPU.AX.UInt16 = 0x1c;
			this.oCPU.IMUL_UInt16(this.oCPU.AX, this.oCPU.DX, (ushort)cityID);
			this.oCPU.BX.UInt16 = this.oCPU.AX.UInt16;
			this.oCPU.TEST_UInt16(this.oParent.GameData.Cities[cityID].ImprovementFlags0, 0x40);
			if (this.oCPU.Flags.NE) goto L0981;
			
			// Instruction address 0x0000:0x09c8, size: 5
			this.oParent.CAPI.strcat(0xba06, "COURTHOUSE\n");

			goto L09c3;

		L0981:
			this.oCPU.AX.UInt16 = 0x1c;
			this.oCPU.IMUL_UInt16(this.oCPU.AX, this.oCPU.DX, (ushort)cityID);
			this.oCPU.BX.UInt16 = this.oCPU.AX.UInt16;

			this.oCPU.TEST_UInt16(this.oParent.GameData.Cities[cityID].ImprovementFlags0, 0x10);
			if (this.oCPU.Flags.NE) goto L0995;
			
			// Instruction address 0x0000:0x09c8, size: 5
			this.oParent.CAPI.strcat(0xba06, "MARKETPLACE\n");

			goto L09c3;

		L0995:
			this.oCPU.AX.UInt16 = 0x1c;
			this.oCPU.IMUL_UInt16(this.oCPU.AX, this.oCPU.DX, (ushort)cityID);
			this.oCPU.SI.UInt16 = this.oCPU.AX.UInt16;

			if (this.oParent.Technology.F0_1ade_22b5_PlayerHasTechnology(this.oParent.GameData.Cities[cityID].PlayerID, TechnologyAdvanceEnum.Religion) &&
				!this.oParent.GameData.Cities[cityID].HasImprovement(ImprovementEnum.Cathedral))
			{
				// Instruction address 0x0000:0x09c8, size: 5
				this.oParent.CAPI.strcat(0xba06, "CATHEDRAL\n");
			}
			else
			{
				// Instruction address 0x0000:0x09c8, size: 5
				this.oParent.CAPI.strcat(0xba06, "lower taxes.\n");
			}

		L09c3:
			this.oCPU.AX.UInt16 = 0x1c;
			this.oCPU.IMUL_UInt16(this.oCPU.AX, this.oCPU.DX, (ushort)cityID);
			this.oCPU.SI.UInt16 = this.oCPU.AX.UInt16;

			this.oCPU.AX.UInt16 = 0x1;
			this.oCPU.CX.LowUInt8 = (byte)this.oParent.GameData.Cities[cityID].PlayerID;
			this.oCPU.AX.UInt16 = this.oCPU.SHL_UInt16(this.oCPU.AX.UInt16, this.oCPU.CX.LowUInt8);
			this.oCPU.TEST_UInt16(this.oCPU.AX.UInt16, (ushort)this.oParent.GameData.PlayerFlags);
			if (this.oCPU.Flags.E) goto L0a14;

			// Instruction address 0x0000:0x09f3, size: 5
			this.oParent.MenuBoxDialog.ShowDialog(this.oCPU.ReadString(this.oCPU.DS.UInt16, 0xba06), 100, 80);

			// Instruction address 0x0000:0x0a07, size: 5
			this.oParent.PlayerTurn.F0_1403_3ed7_DrawVisibleCell(this.oParent.GameData.Cities[cityID].Position.X, this.oParent.GameData.Cities[cityID].Position.Y);

			this.oCPU.WriteUInt8(this.oCPU.DS.UInt16, 0xba06, 0x0);

		L0a14:
			this.oCPU.AX.UInt16 = 0x1c;
			this.oCPU.IMUL_UInt16(this.oCPU.AX, this.oCPU.DX, (ushort)cityID);
			this.oCPU.SI.UInt16 = this.oCPU.AX.UInt16;
			this.oParent.GameData.Cities[cityID].FoodCount = 0;
			this.oParent.GameData.Cities[cityID].ShieldsCount = 0;
			this.oCPU.WriteUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x6), 0x4);
			this.oCPU.WriteUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0xe), 0xffff);
			this.oCPU.AX.UInt16 = (ushort)((short)this.oParent.Var_70e4_CityUnhappyCitizenCount);
			
			if (this.oParent.Var_70e2_CityHappyCitizenCount > this.oParent.Var_70e4_CityUnhappyCitizenCount)
				goto L0bc6;

			this.oCPU.TEST_UInt16(this.oParent.GameData.Cities[cityID].ImprovementFlags0, 0x1);
			if (this.oCPU.Flags.E) goto L0a48;
			goto L0bc6;

		L0a48:
			if (this.oParent.GameData.Players[this.oParent.GameData.Cities[cityID].PlayerID].CityCount > 3)
				goto L0a5b;
			goto L0bc6;

		L0a5b:
			this.oCPU.WriteUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x2), 0x0);
			goto L0acd;

		L0a62:
			this.oCPU.AX.UInt16 = 0x1c;
			this.oCPU.IMUL_UInt16(this.oCPU.AX, this.oCPU.DX, (ushort)cityID);
			this.oCPU.SI.UInt16 = this.oCPU.AX.UInt16;
			this.oCPU.AX.UInt16 = 0x1c;
			this.oCPU.IMUL_UInt16(this.oCPU.AX, this.oCPU.DX, this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x2)));
			this.oCPU.DI.UInt16 = this.oCPU.AX.UInt16;

			// Instruction address 0x0000:0x0a88, size: 5
			this.oCPU.AX.Int16 = (short)this.oParent.MapManagement.GetDistance(this.oParent.GameData.Cities[cityID].Position,
				this.oParent.GameData.Cities[this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x2))].Position);

			this.oCPU.WriteUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x4), this.oCPU.AX.UInt16);

			// Instruction address 0x0000:0x0a9a, size: 5
			this.oParent.CityWorker.F0_1d12_0045_ProcessCityState(this.oCPU.ReadInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x2)), -1);

			this.oCPU.AX.UInt16 = (ushort)((short)this.oParent.Var_70e2_CityHappyCitizenCount);
			this.oCPU.AX.UInt16 = this.oCPU.SUB_UInt16(this.oCPU.AX.UInt16, (ushort)((short)this.oParent.Var_70e4_CityUnhappyCitizenCount));
			this.oCPU.CX.LowUInt8 = 0x5;
			this.oCPU.AX.UInt16 = this.oCPU.SHL_UInt16(this.oCPU.AX.UInt16, this.oCPU.CX.LowUInt8);
			this.oCPU.CWD(this.oCPU.AX, this.oCPU.DX);
			this.oCPU.CX.UInt16 = this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x4));
			this.oCPU.IDIV_UInt16(this.oCPU.AX, this.oCPU.DX, this.oCPU.CX.UInt16);
			this.oCPU.WriteUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x8), this.oCPU.AX.UInt16);
			this.oCPU.AX.UInt16 = this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x6));
			this.oCPU.CMP_UInt16(this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x8)), this.oCPU.AX.UInt16);
			if (this.oCPU.Flags.LE) goto L0aca;
			this.oCPU.AX.UInt16 = this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x8));
			this.oCPU.WriteUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x6), this.oCPU.AX.UInt16);
			this.oCPU.AX.UInt16 = this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x2));
			this.oCPU.WriteUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0xe), this.oCPU.AX.UInt16);

		L0aca:
			this.oCPU.WriteUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x2), this.oCPU.INC_UInt16(this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x2))));

		L0acd:
			this.oCPU.CMP_UInt16(this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x2)), 0x80);
			if (this.oCPU.Flags.GE) goto L0aee;
			this.oCPU.AX.UInt16 = (ushort)cityID;
			this.oCPU.CMP_UInt16(this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x2)), this.oCPU.AX.UInt16);
			if (this.oCPU.Flags.E) goto L0aee;
			this.oCPU.AX.UInt16 = 0x1c;
			this.oCPU.IMUL_UInt16(this.oCPU.AX, this.oCPU.DX, this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x2)));
			this.oCPU.BX.UInt16 = this.oCPU.AX.UInt16;
			
			if (this.oParent.GameData.Cities[this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x2))].Status == CityStatusEnum.Invalid) goto L0aee;
			goto L0a62;

		L0aee:
			this.oCPU.CMP_UInt16(this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0xe)), 0xffff);
			if (this.oCPU.Flags.NE) goto L0af7;
			goto L0bc6;

		L0af7:
			this.oCPU.AX.UInt16 = 0x1c;
			this.oCPU.IMUL_UInt16(this.oCPU.AX, this.oCPU.DX, (ushort)cityID);
			this.oCPU.SI.UInt16 = this.oCPU.AX.UInt16;
			this.oCPU.AX.UInt16 = 0x1c;
			this.oCPU.IMUL_UInt16(this.oCPU.AX, this.oCPU.DX, this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0xe)));
			this.oCPU.BX.UInt16 = this.oCPU.AX.UInt16;
			if (this.oParent.GameData.Cities[this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0xe))].PlayerID !=
				this.oParent.GameData.Cities[cityID].PlayerID) goto L0b14;
			goto L0bc6;

		L0b14:
			this.oCPU.AX.UInt16 = this.oParent.GameData.MapVisibility[this.oParent.GameData.Cities[cityID].Position.X, this.oParent.GameData.Cities[cityID].Position.Y];
			this.oCPU.DX.UInt16 = 0x1;
			this.oCPU.CX.LowUInt8 = (byte)(this.oParent.GameData.HumanPlayerID & 0xff);
			this.oCPU.DX.UInt16 = this.oCPU.SHL_UInt16(this.oCPU.DX.UInt16, this.oCPU.CX.LowUInt8);
			this.oCPU.TEST_UInt16(this.oCPU.AX.UInt16, this.oCPU.DX.UInt16);
			if (this.oCPU.Flags.E) goto L0b92;

			// Instruction address 0x0000:0x0b43, size: 5
			this.oParent.CAPI.strcpy(0xba06, "Residents of ");

			// Instruction address 0x0000:0x0b4e, size: 5
			this.oParent.CAPI.strcat(0xba06, this.oParent.Actions.F0_2459_08c6_GetCityName(cityID));

			// Instruction address 0x0000:0x0b5e, size: 5
			this.oParent.CAPI.strcat(0xba06, " admire\nthe prosperity of ");

			// Instruction address 0x0000:0x0b69, size: 5
			this.oParent.CAPI.strcat(0xba06, this.oParent.Actions.F0_2459_08c6_GetCityName(this.oCPU.ReadInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0xe))));

			// Instruction address 0x0000:0x0b79, size: 5
			this.oParent.CAPI.strcat(0xba06, ".\n");

			// Instruction address 0x0000:0x0b8a, size: 5
			this.oParent.MenuBoxDialog.ShowDialog(this.oCPU.ReadString(this.oCPU.DS.UInt16, 0xba06), 80, 80);

		L0b92:
			this.oParent.DiplomatActions.F22_0000_0af5_InciteCityRevolt(cityID, 
				this.oParent.GameData.Cities[this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0xe))].PlayerID);
			
			this.oCPU.WriteUInt8(this.oCPU.DS.UInt16, 0xba06, 0x0);

		L0bc6:
			this.oCPU.CMP_UInt8(this.oCPU.ReadUInt8(this.oCPU.DS.UInt16, 0xba06), 0x0);
			if (this.oCPU.Flags.NE) goto L0bd0;
			goto L0ca3;

		L0bd0:
			this.oCPU.AX.UInt16 = 0x1c;
			this.oCPU.IMUL_UInt16(this.oCPU.AX, this.oCPU.DX, (ushort)cityID);
			this.oCPU.SI.UInt16 = this.oCPU.AX.UInt16;
			this.oCPU.AX.UInt16 = 0x1;
			this.oCPU.CX.LowUInt8 = (byte)this.oParent.GameData.Cities[cityID].PlayerID;
			this.oCPU.AX.UInt16 = this.oCPU.SHL_UInt16(this.oCPU.AX.UInt16, this.oCPU.CX.LowUInt8);
			this.oCPU.TEST_UInt16(this.oCPU.AX.UInt16, (ushort)this.oParent.GameData.PlayerFlags);
			if (this.oCPU.Flags.E) goto L0c18;

			this.oParent.Var_2f9e_MessageBoxStyle = MenuBoxReportTypeEnum.DomesticAdvisorReport;

			// Instruction address 0x0000:0x0bf9, size: 5
			this.oParent.MenuBoxDialog.ShowDialog(this.oCPU.ReadString(this.oCPU.DS.UInt16, 0xba06), 100, 80);

			// Instruction address 0x0000:0x0c0d, size: 5
			this.oParent.PlayerTurn.F0_1403_3ed7_DrawVisibleCell(this.oParent.GameData.Cities[cityID].Position.X, this.oParent.GameData.Cities[cityID].Position.Y);

			goto L0ca3;

		L0c18:
			this.oCPU.AX.UInt16 = this.oParent.GameData.MapVisibility[this.oParent.GameData.Cities[cityID].Position.X, this.oParent.GameData.Cities[cityID].Position.Y];
			this.oCPU.DX.UInt16 = 0x1;
			this.oCPU.CX.LowUInt8 = (byte)(this.oParent.GameData.HumanPlayerID & 0xff);
			this.oCPU.DX.UInt16 = this.oCPU.SHL_UInt16(this.oCPU.DX.UInt16, this.oCPU.CX.LowUInt8);
			this.oCPU.TEST_UInt16(this.oCPU.AX.UInt16, this.oCPU.DX.UInt16);
			if (this.oCPU.Flags.E) goto L0ca3;
			this.oCPU.WriteUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x8), 0x0);

		L0c4c:
			this.oCPU.BX.UInt16 = this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x8));
			this.oCPU.WriteUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x8), this.oCPU.INC_UInt16(this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x8))));
			this.oCPU.CMP_UInt8(this.oCPU.ReadUInt8(this.oCPU.DS.UInt16, (ushort)(this.oCPU.BX.UInt16 + 0xba06)), 0xa);
			if (this.oCPU.Flags.NE) goto L0c4c;
			this.oCPU.BX.UInt16 = this.oCPU.ReadUInt16(this.oCPU.SS.UInt16, (ushort)(this.oCPU.BP.UInt16 - 0x8));
			this.oCPU.WriteUInt8(this.oCPU.DS.UInt16, (ushort)(this.oCPU.BX.UInt16 + 0xba06), 0x0);

			this.oParent.Var_2f9e_MessageBoxStyle = MenuBoxReportTypeEnum.TravelersReport;

			// Instruction address 0x0000:0x0c73, size: 5
			this.oParent.MenuBoxDialog.ShowDialog(this.oCPU.ReadString(this.oCPU.DS.UInt16, 0xba06), 100, 64);

			// Instruction address 0x0000:0x0c9b, size: 5
			this.oParent.MapManagement.F0_2aea_0008_DrawVisibleMap(oParent.GameData.HumanPlayerID,
				this.oParent.GameData.Cities[cityID].Position.X - 10,
				this.oParent.GameData.Cities[cityID].Position.Y - 6);

		L0ca3:
			this.oCPU.SI.UInt16 = this.oCPU.POP_UInt16();
			this.oCPU.DI.UInt16 = this.oCPU.POP_UInt16();
			this.oCPU.SP.UInt16 = this.oCPU.BP.UInt16;
			this.oCPU.BP.UInt16 = this.oCPU.POP_UInt16();
			// Far return
			this.oCPU.Log.ExitBlock("F20_0000_0540");
		}
	}
}
