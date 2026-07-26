namespace OpenCivOne
{
	public class HallOfFameEntry
	{
		public int TotalScore = -1;
		public int GameDifficulty = 0;
		public int AlwaysZero = 0;
		public int FinalYear = 0;
		public int AchievementsScore = 0;
		public int TotalCitizenCount = 0;
		public int TotalPopulation = 0;
		public string PlayerName = "";
		public string NationName = "";

		public int Rating
		{
			get => (this.TotalScore * (this.GameDifficulty + 1)) / 50;
		}

		public static HallOfFameEntry ReadFromStream(Stream stream)
		{
			HallOfFameEntry entry = new HallOfFameEntry();

			entry.TotalScore = LoadAndSave.ReadInt16(stream);
			entry.GameDifficulty = LoadAndSave.ReadInt16(stream);
			entry.AlwaysZero = LoadAndSave.ReadInt16(stream);
			entry.FinalYear = LoadAndSave.ReadInt16(stream);
			entry.AchievementsScore = LoadAndSave.ReadInt16(stream);
			entry.TotalCitizenCount = LoadAndSave.ReadInt16(stream);
			entry.TotalPopulation = LoadAndSave.ReadInt16(stream);
			entry.PlayerName = LoadAndSave.ReadString(stream, 16);
			entry.NationName = LoadAndSave.ReadString(stream, 16);

			return entry;
		}

		public void WriteToStream(Stream stream)
		{
			LoadAndSave.WriteInt16(stream, (short)this.TotalScore);
			LoadAndSave.WriteInt16(stream, (short)this.GameDifficulty);
			LoadAndSave.WriteInt16(stream, (short)this.AlwaysZero);
			LoadAndSave.WriteInt16(stream, (short)this.FinalYear);
			LoadAndSave.WriteInt16(stream, (short)this.AchievementsScore);
			LoadAndSave.WriteInt16(stream, (short)this.TotalCitizenCount);
			LoadAndSave.WriteInt16(stream, (short)this.TotalPopulation);
			LoadAndSave.WriteString(stream, this.PlayerName, 16);
			LoadAndSave.WriteString(stream, this.NationName, 16);
		}
	}
}
