namespace OpenCivOne.Tests
{
	public class SaveGameTests
	{
		[Fact]
		public void LoadsSaveGame()
		{
			using GameHarness harness = GameHarness.Load("BugScenarios");
			GameData data = harness.Game.GameData;

			Assert.Equal((short)580, data.Year);
			Assert.Equal("Mongols", data.Players[data.HumanPlayerID].Nation);
		}

		[Fact]
		public void SavingUnchangedGameWritesIdenticalFiles()
		{
			using GameHarness harness = GameHarness.Load("BugScenarios");
			byte[] originalSave = File.ReadAllBytes(Path.Combine(harness.Directory, "BUGSCENARIOS.SVE"));
			byte[] originalMap = File.ReadAllBytes(Path.Combine(harness.Directory, "BUGSCENARIOS.MAP"));

			Assert.True(harness.Game.LoadAndSave.SaveGameData("CIVIL1.SVE"));

			Assert.Equal(originalSave, File.ReadAllBytes(Path.Combine(harness.Directory, "CIVIL1.SVE")));
			Assert.Equal(originalMap, File.ReadAllBytes(Path.Combine(harness.Directory, "CIVIL1.MAP")));
		}
	}
}
