namespace OpenCivOne
{
	[Flags]
	public enum UnitStatusEnum
	{
		None = 0,
		Sentry = 0x1,
		SettlerBuildRoadOrRail = 0x2,
		Fortifying = 0x4,
		Fortified = 0x8,
		AIUnknownFlag = 0x10,
		Veteran = 0x20,
		SettlerBuildIrrigation = 0x40,
		SettlerBuildMineOrForest = 0x80,
		SettlerBuildFortress = 0xc0,
		SettlerCleanPollution = 0x82
	}
}
