using System;

namespace OpenCivOne
{
	public class Player
	{
		public string Name = "";
		public short NationalityID = 0;
		public string Nation = "";
		public string Nationality = "";

		public short Coins = 0;
		public short GovernmentType = 0;
		public short TaxRate = 0;
		public short MilitaryPower = 0;
		public short ContactPlayerCountdown = 0;
		public short XStart = 0;

		// Statistics
		public short CityCount = 0;
		public short TotalCitySize = 0;
		public short MapCellCount = 0;

		// Strategic
		public DiplomacyFlagsEnum[] Diplomacy = new DiplomacyFlagsEnum[8];
		public PlayerContinent[] Continents = new PlayerContinent[16];
		public ContinentPolicy[] ContinentPolicies = new ContinentPolicy[16];
		public short Ranking = 0;
		public short CumulativeEpicRanking = 0;
		public short Score = 0;

		// Technology
		public short ScienceTaxRate = 0;
		public short ResearchProgress = 0;
		public short DiscoveredTechnologyCount = 0;
		public short ResearchTechnologyID = -1; // None
		public short FutureTechnologyCount = 0;
		public ushort[] DiscoveredTechnologyFlags = new ushort[5];
		public short[] TechnologyAcquiredFrom = new short[72];

		// Units
		public short UnitCount = 0;
		public short SettlerCount = 0;
		public Unit[] Units = new Unit[129];
		public UnitPolicy[] UnitPolicies = new UnitPolicy[32];
		public short[] ActiveUnits = new short[(int)UnitTypeEnum.Max];
		public short[] UnitsDestroyed = new short[8];
		public short[] UnitsInProduction = new short[(int)UnitTypeEnum.Max];
		public short[] LostUnits = new short[(int)UnitTypeEnum.Max];

		// Spaceship
		public sbyte[] SpaceshipData = new sbyte[180];
		public short SpaceshipPopulation = 0;
		public short SpaceshipETAYear = 0;
		public short SpaceshipLaunchYear = 0;
		public short SpaceshipSuccessRate = 0;

		// Palace
		public short PalaceLevel = 0;
		public short[] PalaceData1 = new short[15];
		public short[] PalaceData2 = new short[13];

		public Player()
		{
			for (int i = 0; i < this.Diplomacy.Length; i++)
			{
				this.Diplomacy[i] = DiplomacyFlagsEnum.None;
			}

			for (int i = 0; i < this.Continents.Length; i++)
			{
				this.Continents[i] = new PlayerContinent();
			}

			for (int i = 0; i < this.DiscoveredTechnologyFlags.Length; i++)
			{
				this.DiscoveredTechnologyFlags[i] = 0;
			}

			for (int i = 0; i < this.TechnologyAcquiredFrom.Length; i++)
			{
				this.TechnologyAcquiredFrom[i] = 0;
			}

			for (int i = 0; i < this.ActiveUnits.Length; i++)
			{
				this.ActiveUnits[i] = 0;
			}

			for (int i = 0; i < this.UnitsDestroyed.Length; i++)
			{
				this.UnitsDestroyed[i] = 0;
			}

			for (int i = 0; i < this.Units.Length; i++)
			{
				this.Units[i] = new Unit();
			}

			for (int i = 0; i < this.UnitsInProduction.Length; i++)
			{
				this.UnitsInProduction[i] = 0;
			}

			for (int i = 0; i < this.LostUnits.Length; i++)
			{
				this.LostUnits[i] = 0;
			}

			for (int i = 0; i < this.UnitPolicies.Length; i++)
			{
				this.UnitPolicies[i] = new UnitPolicy();
			}

			for (int i = 0; i < this.ContinentPolicies.Length; i++)
			{
				this.ContinentPolicies[i] = new ContinentPolicy();
			}

			for (int i = 0; i < this.SpaceshipData.Length; i++)
			{
				this.SpaceshipData[i] = -1;
			}

			for (int i = 0; i < this.PalaceData1.Length; i++)
			{
				this.PalaceData1[i] = 0;
			}

			for (int i = 0; i < this.PalaceData2.Length; i++)
			{
				this.PalaceData2[i] = 0;
			}
		}
	}
}
