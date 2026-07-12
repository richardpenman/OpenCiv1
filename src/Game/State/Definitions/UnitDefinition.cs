using System;

namespace OpenCivOne
{
	public class UnitDefinition
	{
		// Total size: 34 bytes
		public UnitTypeEnum UnitType = UnitTypeEnum.None;
		public string Name = ""; // (12 bytes)
		public TechnologyAdvanceEnum CancelTechnology = TechnologyAdvanceEnum.None;
		public UnitMovementTypeEnum MovementType = UnitMovementTypeEnum.Land;
		public short MoveCount = 0;
		public short TurnsOutside = 0;
		public short AttackStrength = 0;
		public short DefenseStrength = 0;
		public short Cost = 0;
		public short SightRange = 0;
		public short TransportCapacity = 0;
		public UnitRoleTypeEnum UnitRoleType = UnitRoleTypeEnum.Settler;
		public TechnologyAdvanceEnum RequiredTechnology = TechnologyAdvanceEnum.None;

		public UnitDefinition()
		{ }

		public UnitDefinition(UnitTypeEnum unitType, string name, TechnologyAdvanceEnum cancelTechnology, UnitMovementTypeEnum movementType,
			short moveCount, short turnsOutside, short attackStrength, short defenseStrength,
			short cost, short sightRange, short transportCapacity, UnitRoleTypeEnum aiRole,
			TechnologyAdvanceEnum requiredTechnology)
		{
			this.UnitType= unitType;
			this.Name = name;
			this.CancelTechnology = cancelTechnology;
			this.MovementType = movementType;
			this.MoveCount = moveCount;
			this.TurnsOutside = turnsOutside;
			this.AttackStrength = attackStrength;
			this.DefenseStrength = defenseStrength;
			this.Cost = cost;
			this.SightRange = sightRange;
			this.TransportCapacity = transportCapacity;
			this.UnitRoleType = aiRole;
			this.RequiredTechnology = requiredTechnology;
		}

		public static UnitDefinition FromStream(Stream stream)
		{
			UnitDefinition ud = new UnitDefinition();

			ud.Name = LoadAndSave.ReadString(stream, 12);
			ud.CancelTechnology = (TechnologyAdvanceEnum)LoadAndSave.ReadInt16(stream);
			ud.MovementType = (UnitMovementTypeEnum)LoadAndSave.ReadInt16(stream);
			ud.MoveCount = LoadAndSave.ReadInt16(stream);
			ud.TurnsOutside = LoadAndSave.ReadInt16(stream);
			ud.AttackStrength = LoadAndSave.ReadInt16(stream);
			ud.DefenseStrength = LoadAndSave.ReadInt16(stream);
			ud.Cost = LoadAndSave.ReadInt16(stream);
			ud.SightRange = LoadAndSave.ReadInt16(stream);
			ud.TransportCapacity = LoadAndSave.ReadInt16(stream);
			ud.UnitRoleType = (UnitRoleTypeEnum)LoadAndSave.ReadInt16(stream);
			ud.RequiredTechnology = (TechnologyAdvanceEnum)LoadAndSave.ReadInt16(stream);

			return ud;
		}

		public void ToStream(Stream stream)
		{
			LoadAndSave.WriteString(stream, this.Name, 12);
			LoadAndSave.WriteInt16(stream, (short)this.CancelTechnology);
			LoadAndSave.WriteInt16(stream, (short)this.MovementType);
			LoadAndSave.WriteInt16(stream, this.MoveCount);
			LoadAndSave.WriteInt16(stream, this.TurnsOutside);
			LoadAndSave.WriteInt16(stream, this.AttackStrength);
			LoadAndSave.WriteInt16(stream, this.DefenseStrength);
			LoadAndSave.WriteInt16(stream, this.Cost);
			LoadAndSave.WriteInt16(stream, this.SightRange);
			LoadAndSave.WriteInt16(stream, this.TransportCapacity);
			LoadAndSave.WriteInt16(stream, (short)this.UnitRoleType);
			LoadAndSave.WriteInt16(stream, (short)this.RequiredTechnology);
		}
	}
}
