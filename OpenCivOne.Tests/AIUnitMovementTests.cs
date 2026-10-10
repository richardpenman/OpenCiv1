using OpenCivOne.Graphics;

namespace OpenCivOne.Tests
{
	/// <summary>
	/// Movement rules that only apply to computer players. Expected results follow the original game's code.
	/// </summary>
	public class AIUnitMovementTests
	{
		private static int FindComputerPlayer(GameData data)
		{
			for (int i = 1; i < 8; i++)
			{
				if (i != data.HumanPlayerID && (data.ActiveCivilizations & (1 << i)) != 0)
					return i;
			}

			throw new InvalidOperationException("The save game has no computer player");
		}

		/// <summary>
		/// Finds two neighbouring squares of the given terrain, with no units or cities within three squares.
		/// </summary>
		private static (GPoint From, int Direction, GPoint To) FindQuietSquares(GameHarness harness, bool water)
		{
			MapManagement map = harness.Game.MapManagement;

			bool IsQuiet(int x, int y)
			{
				for (int dy = -3; dy <= 3; dy++)
				{
					for (int dx = -3; dx <= 3; dx++)
					{
						int cx = map.AdjustXPosition(x + dx);
						int cy = y + dy;

						if (!map.ValidateMapCoordinates(cx, cy) || map.F0_2aea_1458_GetCellActiveUnitID(cx, cy) != -1 ||
							map.GetCityByLocation(cx, cy) != -1)
							return false;
					}
				}

				return true;
			}

			for (int y = 4; y < 46; y++)
			{
				for (int x = 0; x < 80; x++)
				{
					if ((map.GetTerrainType(x, y) == TerrainTypeEnum.Water) == water &&
						(map.GetTerrainType(x + 1, y) == TerrainTypeEnum.Water) == water && IsQuiet(x, y))
					{
						return (new GPoint(x, y), 3, new GPoint(x + 1, y));
					}
				}
			}

			throw new InvalidOperationException("No quiet squares found");
		}

		[Theory]
		[InlineData(UnitTypeEnum.Trireme, true, false)]
		[InlineData(UnitTypeEnum.Fighter, true, true)]
		[InlineData(UnitTypeEnum.Militia, false, false)]
		public void MovingClearsSentryExceptForAircraft(UnitTypeEnum unitType, bool water, bool keepsSentry)
		{
			using GameHarness harness = GameHarness.Load("BugScenarios");
			int playerID = FindComputerPlayer(harness.Game.GameData);
			var (from, direction, to) = FindQuietSquares(harness, water);

			int unitID = harness.Game.UnitManagement.F0_1866_0cf5_CreateUnit(playerID, unitType, from.X, from.Y);
			Unit unit = harness.Game.GameData.Players[playerID].Units[unitID];
			unit.Status = UnitStatusEnum.Sentry;
			unit.RemainingMoves = 3;

			harness.MoveUnit(playerID, unitID, direction);

			Assert.Equal(to, unit.Position);
			Assert.Equal(keepsSentry, unit.TestStatusFlags(UnitStatusEnum.Sentry));
		}

		[Theory]
		[InlineData(UnitTypeEnum.Fighter, false)]
		[InlineData(UnitTypeEnum.Militia, false)]
		[InlineData(UnitTypeEnum.Trireme, true)]
		public void ComputerDoesNotJoinLargeStackOutsideCity(UnitTypeEnum unitType, bool canJoin)
		{
			using GameHarness harness = GameHarness.Load("BugScenarios");
			int playerID = FindComputerPlayer(harness.Game.GameData);
			bool water = unitType == UnitTypeEnum.Trireme;
			var (from, direction, to) = FindQuietSquares(harness, water);

			// Four units already stand on the destination square
			UnitTypeEnum stackType = water ? UnitTypeEnum.Trireme : UnitTypeEnum.Militia;

			for (int i = 0; i < 4; i++)
			{
				harness.Game.UnitManagement.F0_1866_0cf5_CreateUnit(playerID, stackType, to.X, to.Y);
			}

			int unitID = harness.Game.UnitManagement.F0_1866_0cf5_CreateUnit(playerID, unitType, from.X, from.Y);
			Unit unit = harness.Game.GameData.Players[playerID].Units[unitID];
			unit.RemainingMoves = 3;
			unit.SpecialMoves = 1;

			harness.MoveUnit(playerID, unitID, direction);

			Assert.Equal(canJoin ? to : from, unit.Position);
		}

		[Fact]
		public void AircraftReturnsTowardsNearestCityOrCarrier()
		{
			using GameHarness harness = GameHarness.Load("BugScenarios");
			GameData data = harness.Game.GameData;
			int playerID = FindComputerPlayer(data);
			var (from, _, _) = FindQuietSquares(harness, false);

			int unitID = harness.Game.UnitManagement.F0_1866_0cf5_CreateUnit(playerID, UnitTypeEnum.Fighter, from.X, from.Y);
			Unit fighter = data.Players[playerID].Units[unitID];
			fighter.RemainingMoves = 30;
			fighter.SpecialMoves = 1;
			int distanceBefore = DistanceToNearestBase(harness, playerID, fighter.Position);

			int retryCount = 0;
			harness.Game.PlayerTurn.HomeCityCommand(playerID, unitID, ref retryCount);

			Assert.Equal(distanceBefore - 1, DistanceToNearestBase(harness, playerID, fighter.Position));
		}

		private static int DistanceToNearestBase(GameHarness harness, int playerID, GPoint position)
		{
			IEnumerable<GPoint> cities = harness.Game.GameData.Cities
				.Where(city => city.Status != CityStatusEnum.Invalid && city.PlayerID == playerID)
				.Select(city => city.Position);
			IEnumerable<GPoint> carriers = harness.Game.GameData.Players[playerID].Units
				.Where(unit => unit.UnitType == UnitTypeEnum.Carrier)
				.Select(unit => unit.Position);

			return cities.Concat(carriers).Min(target =>
			{
				int dx = Math.Abs(position.X - target.X);

				return Math.Max(Math.Min(dx, 80 - dx), Math.Abs(position.Y - target.Y));
			});
		}
	}
}
