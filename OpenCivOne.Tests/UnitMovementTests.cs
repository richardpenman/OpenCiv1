using OpenCivOne.Graphics;

namespace OpenCivOne.Tests
{
	/// <summary>
	/// Unit movement scenarios in Saves/BugScenarios (the human player is the Mongols).
	/// Expected results follow the original game's code.
	/// </summary>
	public class UnitMovementTests
	{
		private const int Up = 1;
		private const int UpRight = 2;
		private const int Right = 3;

		private static (GameHarness Harness, int PlayerID, Player Player) LoadScenarios()
		{
			GameHarness harness = GameHarness.Load("BugScenarios");
			int playerID = harness.UseEliminatedCivilizationAsHuman();

			return (harness, playerID, harness.Game.GameData.Players[playerID]);
		}

		[Fact]
		public void TriremeLeavingCityCarriesSentriedUnits()
		{
			var (harness, playerID, player) = LoadScenarios();
			using GameHarness _ = harness;

			// Trireme #34 in Samarkand (71,17), which has four sentried Militia. A Trireme carries two units.
			harness.MoveUnit(playerID, 34, Right);

			Assert.Equal(new GPoint(72, 17), player.Units[34].Position);
			Assert.Equal(2, player.Units.Count(unit => unit.UnitType == UnitTypeEnum.Militia && unit.Position == new GPoint(72, 17)));
		}

		[Fact]
		public void CarrierCarriesFighter()
		{
			var (harness, playerID, player) = LoadScenarios();
			using GameHarness _ = harness;

			// Carrier #37 with Fighter #38 at sea (72,15)
			harness.MoveUnit(playerID, 37, UpRight);

			Assert.Equal(new GPoint(73, 14), player.Units[37].Position);
			Assert.Equal(new GPoint(73, 14), player.Units[38].Position);
		}

		[Fact]
		public void TriremeAtSeaCarriesUnits()
		{
			var (harness, playerID, player) = LoadScenarios();
			using GameHarness _ = harness;

			// Trireme #39 with Militia #40 at sea (71,19)
			harness.MoveUnit(playerID, 39, Right);

			Assert.Equal(new GPoint(72, 19), player.Units[39].Position);
			Assert.Equal(new GPoint(72, 19), player.Units[40].Position);
		}

		[Fact]
		public void FighterFlyingFromSeaToLandKeepsItsMoves()
		{
			var (harness, playerID, player) = LoadScenarios();
			using GameHarness _ = harness;

			// Fighter #41 over the sea (74,16), 10 moves; flies onto land
			harness.MoveUnit(playerID, 41, UpRight);

			Assert.Equal(new GPoint(75, 15), player.Units[41].Position);
			Assert.Equal(27, player.Units[41].RemainingMoves);
		}

		[Fact]
		public void BattleshipOnlyRevealsWaterTwoSquaresAway()
		{
			using GameHarness harness = GameHarness.Load("BugScenarios");
			GameData data = harness.Game.GameData;
			int playerID = data.HumanPlayerID;
			Unit battleship = data.Players[playerID].Units[52];

			// Battleship #52 (sight range 2) moves from (52,26) to (53,27), which has unexplored land two squares away.
			// Moving a human unit reveals the map and then redraws it, so call the reveal directly.
			GPoint position = new GPoint(53, 27);
			int unexploredLandBefore = CountUnexploredLandTwoSquaresAway(harness, playerID, position);
			Assert.True(unexploredLandBefore > 0);

			battleship.Position = position;
			harness.Game.UnitManagement.F0_1866_01dc_UpdateUnitVisibility(position.X, position.Y, playerID, 52, true);

			Assert.Equal(unexploredLandBefore, CountUnexploredLandTwoSquaresAway(harness, playerID, position));
		}

		private static int CountUnexploredLandTwoSquaresAway(GameHarness harness, int playerID, GPoint position)
		{
			MapManagement map = harness.Game.MapManagement;
			int count = 0;

			for (int i = 9; i < 25; i++)
			{
				int x = map.AdjustXPosition(position.X + harness.Game.MoveDirections[i].X);
				int y = position.Y + harness.Game.MoveDirections[i].Y;

				if (map.ValidateMapCoordinates(x, y) && map.GetTerrainType(x, y) != TerrainTypeEnum.Water &&
					(harness.Game.GameData.MapVisibility[x, y] & (1 << playerID)) == 0)
				{
					count++;
				}
			}

			return count;
		}
	}
}
