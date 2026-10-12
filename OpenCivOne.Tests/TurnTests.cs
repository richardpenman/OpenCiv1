namespace OpenCivOne.Tests
{
	public class TurnTests
	{
		// Samarkand has Magellan's Expedition. The Lighthouse gives the same bonus, but is obsolete in this save game.
		[Fact]
		public void MagellansExpeditionGivesShipsAnExtraMove()
		{
			using GameHarness harness = GameHarness.Load("BugScenarios");
			GameData data = harness.Game.GameData;
			int playerID = data.HumanPlayerID;
			Player player = data.Players[playerID];
			Assert.True(harness.Game.CityWorker.F0_1d12_6c97_PlayerHasWonder(playerID, WonderEnum.MagellansExpedition));

			harness.Game.PlayerTurn.ResetUnitMoves(playerID);

			// Trireme #34, Militia #35, Fighter #41
			Assert.Equal((data.Units[(int)UnitTypeEnum.Trireme].MoveCount + 1) * 3, player.Units[34].RemainingMoves);
			Assert.Equal(data.Units[(int)UnitTypeEnum.Militia].MoveCount * 3, player.Units[35].RemainingMoves);
			Assert.Equal(data.Units[(int)UnitTypeEnum.Fighter].MoveCount * 3, player.Units[41].RemainingMoves);
		}
	}
}
