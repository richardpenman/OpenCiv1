namespace OpenCivOne
{
	[Flags]
	public enum DiplomacyFlagsEnum
	{
		None = 0,
		Contact = 0x1,
		Peace = 0x2,
		Allied = 0x4,
		Vendetta = 0x8,
		FormalContact = 0x10,
		EverFought = 0x20,
		EmbassyEstablished = 0x40,
		TributeDemanded = 0x80,
		PeaceTreaty = 0x100,
		warDeclared = 0x200
	}
}
