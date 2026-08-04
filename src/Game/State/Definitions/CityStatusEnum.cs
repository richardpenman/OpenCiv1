namespace OpenCivOne
{
	[Flags]
	public enum CityStatusEnum
	{
		Invalid = -1,
		None = 0,

		/// <summary>
		/// City is in civil disorder. Production is halted, the city can't be rushed
		/// and the cost of bribing the city is halved.
		/// </summary>
		CivilDisorder = 0x1,

		/// <summary>
		/// City has an adjacent Water cell and can build units with a Water movement type.
		/// Set when the city is founded. For the AI players the adjacent water area must be
		/// larger than 256 cells, while for the human player any adjacent water cell counts.
		/// </summary>
		OceanAccess = 0x2,

		/// <summary>
		/// The two resource free fortified units of the city are currently present on the map.
		/// They are stored back into the city when no enemy unit is near.
		/// </summary>
		FortifiedUnitsRestored = 0x4,

		/// <summary>
		/// City has an adjacent River or Mountains cell, which is a requirement for building a Hydro Plant.
		/// Set when the city is founded.
		/// </summary>
		NearRiverOrMountains = 0x8,

		/// <summary>
		/// Production of the city is chosen automatically instead of by its owner.
		/// Shown as the 'AUTO' button in the city view and toggled with the 'A' key.
		/// It is also set on an enemy city after it has been attacked.
		/// </summary>
		AutoProduction = 0x10,

		/// <summary>
		/// A technology has already been stolen from this city by a Diplomat.
		/// Each city can be robbed of a technology only once.
		/// </summary>
		TechnologyStolen = 0x20,

		/// <summary>
		/// City is celebrating ('We Love The King Day'). Under a Republic or a Democracy
		/// the city grows each turn, and the cell resource output is increased.
		/// </summary>
		Celebrating = 0x40,

		/// <summary>
		/// An improvement has already been sold in this city in the current turn.
		/// Only one improvement can be sold per city per turn, this flag is cleared at the start of a city turn.
		/// </summary>
		ImprovementSoldThisTurn = 0x80
	}
}
