using System;
using OpenCivOne.Graphics;

namespace OpenCivOne
{
	public class Unit
	{
		public int ID = -1;
		public UnitTypeEnum UnitType = UnitTypeEnum.None;
		public UnitStatusEnum Status = UnitStatusEnum.None;
		public GPoint Position = new GPoint(-1);
		public short RemainingMoves = 0;
		public short SpecialMoves = 0;
		public short NextUnitID = 0;
		public short PlayerID = 0;
		public short HomeCityID = 0;
		
		public ushort VisibleByPlayer = 0;

		private GPoint goToDestination = new GPoint(-1);
		public short GoToNextDirection = 0;
		public Stack<GPoint> GoToPath = new Stack<GPoint>();

		public void ClearStatusFlags(UnitStatusEnum status)
		{
			this.Status |= status;
			this.Status ^= status;
		}

		public bool TestStatusFlags(UnitStatusEnum status)
		{
			return (this.Status & status) != status;
		}

		public static Unit FromStream(int id, Stream stream)
		{
			Unit unit = new Unit();

			unit.ID = id;
			unit.Status = (UnitStatusEnum)LoadAndSave.ReadUInt8(stream);
			unit.Position = new((sbyte)LoadAndSave.ReadUInt8(stream), (sbyte)LoadAndSave.ReadUInt8(stream));
			unit.UnitType = (UnitTypeEnum)((sbyte)LoadAndSave.ReadUInt8(stream));
			unit.RemainingMoves = (sbyte)LoadAndSave.ReadUInt8(stream);
			unit.SpecialMoves = (sbyte)LoadAndSave.ReadUInt8(stream);
			unit.goToDestination = new((sbyte)LoadAndSave.ReadUInt8(stream), (sbyte)LoadAndSave.ReadUInt8(stream));
			unit.GoToNextDirection = (sbyte)LoadAndSave.ReadUInt8(stream);
			unit.VisibleByPlayer = LoadAndSave.ReadUInt8(stream);
			unit.NextUnitID = (sbyte)LoadAndSave.ReadUInt8(stream);
			unit.HomeCityID = (sbyte)LoadAndSave.ReadUInt8(stream);

			return unit;
		}

		public void ToStream(Stream stream)
		{
			stream.WriteByte((byte)this.Status);
			stream.WriteByte((byte)((sbyte)this.Position.X));
			stream.WriteByte((byte)((sbyte)this.Position.Y));
			stream.WriteByte((byte)((sbyte)this.UnitType));
			stream.WriteByte((byte)((sbyte)this.RemainingMoves));
			stream.WriteByte((byte)((sbyte)this.SpecialMoves));
			stream.WriteByte((byte)((sbyte)this.goToDestination.X));
			stream.WriteByte((byte)((sbyte)this.goToDestination.Y));
			stream.WriteByte((byte)((sbyte)this.GoToNextDirection));
			stream.WriteByte((byte)(this.VisibleByPlayer & 0xff));
			stream.WriteByte((byte)((sbyte)this.NextUnitID));
			stream.WriteByte((byte)((sbyte)this.HomeCityID));
		}

		public GPoint GoToDestination
		{
			get => this.goToDestination;
			set
			{
				if (value != this.goToDestination)
				{
					this.GoToPath.Clear();
				}

				this.goToDestination = value;
			}
		}
	}
}
