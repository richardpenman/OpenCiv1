namespace OpenCivOne.Tests
{
	/// <summary>
	/// Section names of the original game's text files. The tests write placeholder files with these sections, so the
	/// game can look up its text without the original (copyrighted) files.
	/// </summary>
	internal static class TextResourceKeys
	{
		public static readonly Dictionary<string, string[]> Sections = new()
		{
			["KING"] = new[]
			{
				"*NEWSA", "*NEWSB", "*NEWSC", "*NEWSD", "*NEWSE", "*NEWSF", "*NEWSG", "*NEWSH",
				"*NEWSI", "*NEWSJ", "*NEWSK", "*NEWSL", "*NEWSM", "*NEWSN", "*EXCHANGE", "*CANCEL",
				"*TRIBUTE", "*TAKECIV", "*PEACE", "*ATTITUDE", "*PROVOKE", "*REJECT", "*RID", "*MOBILIZE",
				"*THREAT", "*NOCONTACT", "*ATWAR", "*MERCENARY", "*UNFORTUNATE", "*PALACE", "*HELPME", "*CRUSADE",
				"*GIVECIV", "*GIVECASH", "*GIVEMORE", "*GROVEL", "*TREATY", "*GAPE", "*LEADER", "*ESCHISM",
				"*QUIZ", "*QUIZ2", "*CORRECT", "*INCORRECT", "*INIT", "*TAXES", "*SPACE", "*SPACE2",
				"*ARCH", "*FAME", "*LOMEM", "*D1", "*D2", "*D3", "*D4", "*D5",
				"*D6", "*REV", "*UMAX", "*PMAX", "*END",
			},
			["BLURB0"] = new[]
			{
				"*NAVIGATION", "*CONSTRUCTION", "*HORSEBACK RIDING", "*CEREMONIAL BURIAL", "*POTTERY", "*ALPHABET", "*ADVANCED FLIGHT", "*ASTRONOMY",
				"*ATOMIC THEORY", "*AUTOMOBILE", "*BANKING", "*BRIDGE BUILDING", "*BRONZE WORKING", "*CHEMISTRY", "*CHIVALRY", "*CODE OF LAWS",
				"*COMBUSTION", "*COMMUNISM", "*COMPUTERS", "*CONSCRIPTION", "*CURRENCY", "*DEMOCRACY", "*ELECTRICITY", "*ELECTRONICS",
				"*ENGINEERING", "*EXPLOSIVES", "*FEUDALISM", "*FLIGHT", "*FUSION POWER", "*GENETIC ENGINEERING", "*GUNPOWDER", "*INDUSTRIALIZATION",
				"*INVENTION", "*IRON WORKING", "*LABOR UNION", "*LITERACY", "*MAGNETISM", "*MAPMAKING", "*MASONRY", "*MASS PRODUCTION",
				"*MATHEMATICS", "*MEDICINE", "*METALLURGY", "*MONARCHY", "*MYSTICISM", "*NUCLEAR FISSION", "*NUCLEAR POWER", "*PHILOSOPHY",
				"*PHYSICS", "*PLASTICS", "*RAILROAD", "*RECYCLING", "*REFINING", "*RELIGION", "*ROBOTICS", "*ROCKETRY",
				"*SPACE FLIGHT", "*STEAM ENGINE", "*STEEL", "*SUPERCONDUCTOR", "*THE CORPORATION", "*THE REPUBLIC", "*THE WHEEL", "*THEORY OF GRAVITY",
				"*TRADE", "*UNIVERSITY", "*WRITING", "*END",
			},
			["BLURB1"] = new[]
			{
				"*PALACE", "*PALACE2", "*BARRACKS", "*BARRACKS2", "*GRANARY", "*GRANARY2", "*TEMPLE", "*TEMPLE2",
				"*MARKETPLACE", "*MARKETPLACE2", "*LIBRARY", "*LIBRARY2", "*COURTHOUSE", "*COURTHOUSE2", "*CITY WALLS", "*CITY WALLS2",
				"*AQUEDUCT", "*AQUEDUCT2", "*BANK", "*BANK2", "*CATHEDRAL", "*CATHEDRAL2", "*UNIVERSITY", "*UNIVERSITY2",
				"*COLOSSEUM", "*COLOSSEUM2", "*RECYCLING CNTR.", "*RECYCLING CNTR.2", "*FACTORY", "*FACTORY2", "*MFG. PLANT", "*MFG. PLANT2",
				"*POWER PLANT", "*POWER PLANT2", "*HYDRO PLANT", "*HYDRO PLANT2", "*NUCLEAR PLANT", "*NUCLEAR PLANT2", "*MASS TRANSIT", "*MASS TRANSIT2",
				"*SDI DEFENSE", "*SDI DEFENSE2", "*SS MODULE", "*SS MODULE2", "*SS COMPONENT", "*SS COMPONENT2", "*SS STRUCTURAL", "*SS STRUCTURAL2",
				"*PYRAMIDS", "*PYRAMIDS2", "*HANGING GARDENS", "*HANGING GARDENS2", "*COLOSSUS", "*COLOSSUS2", "*LIGHTHOUSE", "*LIGHTHOUSE2",
				"*GREAT LIBRARY", "*GREAT LIBRARY2", "*ORACLE", "*ORACLE2", "*GREAT WALL", "*GREAT WALL2", "*MAGELLAN'S EXPEDITION", "*MAGELLAN'S EXPEDITION2",
				"*MICHELANGELO'S CHAPEL", "*MICHELANGELO'S CHAPEL2", "*COPERNICUS' OBSERVATORY", "*COPERNICUS' OBSERVATORY2", "*SHAKESPEARE'S THEATRE", "*SHAKESPEARE'S THEATRE2", "*ISAAC NEWTON'S COLLEGE", "*ISAAC NEWTON'S COLLEGE2",
				"*J.S.BACH'S CATHEDRAL", "*J.S.BACH'S CATHEDRAL2", "*DARWIN'S VOYAGE", "*DARWIN'S VOYAGE2", "*HOOVER DAM", "*HOOVER DAM2", "*WOMEN'S SUFFRAGE", "*WOMEN'S SUFFRAGE2",
				"*MANHATTAN PROJECT", "*MANHATTAN PROJECT2", "*UNITED NATIONS", "*UNITED NATIONS2", "*APOLLO PROGRAM", "*APOLLO PROGRAM2", "*SETI PROGRAM", "*SETI PROGRAM2",
				"*CURE FOR CANCER", "*CURE FOR CANCER2", "*END",
			},
			["BLURB2"] = new[]
			{
				"*PHALANX", "*PHALANX2", "*KNIGHTS", "*KNIGHTS2", "*TRIREME", "*TRIREME2", "*SAIL", "*SAIL2",
				"*FRIGATE", "*FRIGATE2", "*IRONCLAD", "*IRONCLAD2", "*TRANSPORT", "*TRANSPORT2", "*FIGHTER", "*FIGHTER2",
				"*BOMBER", "*BOMBER2", "*CARRIER", "*CARRIER2", "*SUBMARINE", "*SUBMARINE2", "*CRUISER", "*CRUISER2",
				"*BATTLESHIP", "*BATTLESHIP2", "*NUCLEAR", "*NUCLEAR2", "*DIPLOMAT", "*DIPLOMAT2", "*CARAVAN", "*CARAVAN2",
				"*ARMOR", "*ARMOR2", "*ARTILLERY", "*ARTILLERY2", "*CANNON", "*CANNON2", "*CATAPULT", "*CATAPULT2",
				"*CAVALRY", "*CAVALRY2", "*CHARIOT", "*CHARIOT2", "*LEGION", "*LEGION2", "*MECH. INF.", "*MECH. INF.2",
				"*MILITIA", "*MILITIA2", "*MUSKETEERS", "*MUSKETEERS2", "*RIFLEMEN", "*RIFLEMEN2", "*SETTLERS", "*SETTLERS2",
				"*END",
			},
			["BLURB3"] = new[]
			{
				"*END",
			},
			["BLURB4"] = new[]
			{
				"*ROADS", "*ROADS2", "*RAILROADS", "*RAILROADS2", "*IRRIGATION", "*IRRIGATION2", "*MINING", "*MINING2",
				"*FORTIFY", "*FORTIFY2", "*FORTRESS", "*FORTRESS2", "*PILLAGE", "*PILLAGE2", "*DISBAND", "*DISBAND2",
				"*VETERAN UNITS", "*VETERAN UNITS2", "*ANARCHY", "*ANARCHY2", "*DESPOTISM", "*DESPOTISM2", "*MONARCHY", "*MONARCHY2",
				"*COMMUNISM", "*COMMUNISM2", "*REPUBLIC", "*REPUBLIC2", "*DEMOCRACY", "*DEMOCRACY2", "*POLLUTION", "*POLLUTION2",
				"*SENTRY", "*SENTRY2", "*LUXURIES", "*LUXURIES2", "*TAXES", "*TAXES2", "*SCIENCE", "*SCIENCE2",
				"*TRADE ROUTES", "*TRADE ROUTES2", "*END",
			},
			["HELP"] = new[]
			{
				"*FIRSTMOVE", "*BUILDCITY", "*FIRSTPRODUCT", "*FIRSTCIV", "*FIRSTUNIT1", "*FIRSTUNIT2", "*MINING", "*IRRIGATE",
				"*ROAD", "*MONARCHY", "*LAND", "*TEMPERATURE", "*CLIMATE", "*AGE", "*HELP1", "*HELP2",
				"*DISORDER", "*SCHISM", "*END",
			},
			["PRODUCE"] = new[]
			{
				"*Temple", "*Cathedral", "*Colosseum", "*Barracks", "*Granary", "*MarketPlace", "*Bank", "*Library",
				"*University", "*Courthouse", "*City Walls", "*Aqueduct", "*Mass Transit", "*Recycling Cntr.", "*Factory", "*Mfg. Plant",
				"*Power Plant", "*Hydro Plant", "*Nuclear Plant", "*SDI Defense", "*SS Structural", "*SS Component", "*SS Module", "*Settlers",
				"*Militia", "*Phalanx", "*Musketeers", "*Riflemen", "*Mech. Inf.", "*Legion", "*Cavalry", "*Knights",
				"*Catapult", "*Cannon", "*Chariot", "*Armor", "*Artillery", "*Fighter", "*Bomber", "*Trireme",
				"*Sail", "*Frigate", "*Ironclad", "*Cruiser", "*Battleship", "*Submarine", "*Carrier", "*Transport",
				"*Nuclear", "*Diplomat", "*Caravan", "*END",
			},
			["ERROR"] = new[]
			{
				"*ZOC", "*TRIREME", "*FUEL", "*AMPHIB", "*FIGHTER", "*OCCUPY", "*PILLAGE", "*SETTLERS",
				"*NOWATER", "*NOMINE", "*NOIRR", "*ADDCITY", "*END",
			},
		};
	}
}
