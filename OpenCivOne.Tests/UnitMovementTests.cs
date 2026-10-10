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
		public void FighterLandingOnCarrierEndsTurnAndRefuels()
		{
			var (harness, playerID, player) = LoadScenarios();
			using GameHarness _ = harness;
			Unit fighter = player.Units[43];
			fighter.SpecialMoves = 0;

			// Fighter #43 at (74,18) flies onto Carrier #42 at (75,18)
			harness.MoveUnit(playerID, 43, Right);

			Assert.Equal(new GPoint(75, 18), fighter.Position);
			Assert.Equal(0, fighter.RemainingMoves);
			Assert.Equal(harness.Game.GameData.Units[(int)UnitTypeEnum.Fighter].TurnsOutside, fighter.SpecialMoves);
		}

		[Fact]
		public void BomberTurnEndsAfterAttacking()
		{
			// Bomber #44 at (69,15) attacks the barbarian Militia at (70,14)
			Unit bomber = AttackAndWin(44, UpRight);

			Assert.Equal(0, bomber.RemainingMoves);
		}

		[Fact]
		public void CruiserKeepsMovesAfterAttacking()
		{
			// Cruiser #46 at (67,13), 6 moves, attacks the barbarian Militia at (68,13)
			Unit cruiser = AttackAndWin(46, Right);

			Assert.Equal(15, cruiser.RemainingMoves);
		}

		/// <summary>
		/// Combat is random, so repeat the attack with different seeds until the attacker survives.
		/// </summary>
		private static Unit AttackAndWin(int unitID, int direction)
		{
			for (int seed = 1; seed <= 20; seed++)
			{
				var (harness, playerID, player) = LoadScenarios();
				using GameHarness _ = harness;

				harness.SeedRandom(seed);
				harness.MoveUnit(playerID, unitID, direction);

				if (player.Units[unitID].UnitType != UnitTypeEnum.None)
				{
					return player.Units[unitID];
				}
			}

			Assert.Fail($"Unit #{unitID} lost every attack");

			return null!;
		}

		[Fact]
		public void TriremeNextToLandIsNeverLost()
		{
			// Triremes #47 to #50 end their move next to land
			for (int seed = 1; seed <= 10; seed++)
			{
				using GameHarness harness = GameHarness.Load("BugScenarios");
				int playerID = harness.Game.GameData.HumanPlayerID;
				harness.SeedRandom(seed);

				for (int unitID = 47; unitID <= 50; unitID++)
				{
					harness.Game.PlayerTurn.CheckTriremeLostAtSea(playerID, unitID);

					Assert.Equal(UnitTypeEnum.Trireme, harness.Game.GameData.Players[playerID].Units[unitID].UnitType);
				}
			}
		}

		[Fact]
		public void TriremeAwayFromLandCanBeLost()
		{
			int lost = 0;

			for (int seed = 1; seed <= 10; seed++)
			{
				using GameHarness harness = GameHarness.Load("BugScenarios");
				int playerID = harness.Game.GameData.HumanPlayerID;
				harness.SeedRandom(seed);

				// A new Trireme on the open sea at (5,25), with no land next to it
				int unitID = harness.Game.UnitManagement.F0_1866_0cf5_CreateUnit(playerID, UnitTypeEnum.Trireme, 5, 25);
				Unit trireme = harness.Game.GameData.Players[playerID].Units[unitID];

				harness.Game.PlayerTurn.CheckTriremeLostAtSea(playerID, unitID);

				if (trireme.UnitType == UnitTypeEnum.None)
				{
					lost++;
				}
			}

			Assert.InRange(lost, 1, 9);
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
