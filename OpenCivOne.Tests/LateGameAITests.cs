using System.Security.Cryptography;

namespace OpenCivOne.Tests
{
	/// <summary>
	/// Plays many turns of a game with five computer players (Saves/AITURNS), to catch crashes, hangs and broken unit
	/// stacks in code that only runs later in a game.
	/// </summary>
	public class LateGameAITests
	{
		private static readonly TimeSpan Timeout = TimeSpan.FromMinutes(5);

		[Theory]
		[InlineData(1, 60)]
		[InlineData(2, 60)]
		[InlineData(3, 60)]
		[InlineData(8, 150)]
		public void ComputerPlayersPlayWithoutErrors(int seed, int turns)
		{
			using GameHarness harness = GameHarness.Load("AITurns");
			harness.MakeAllPlayersComputerControlled();
			harness.SeedRandom(seed);

			harness.PlayTurns(turns, Timeout, turn =>
			{
				string? error = FindBrokenUnitStack(harness.Game.GameData);

				Assert.True(error == null, $"Turn {turn}: {error}");
			});
		}

		[Fact]
		public void SameSeedPlaysTheSameGame()
		{
			Assert.Equal(PlayAndHashSaveGame(seed: 7, turns: 20), PlayAndHashSaveGame(seed: 7, turns: 20));
		}

		/// <summary>
		/// Pins down the game logic: 50 turns with a fixed seed must end in exactly this saved game. Code that should not change
		/// the game's behaviour, such as a refactoring, must keep this test passing. When a change is meant to alter the
		/// game's behaviour, check the change in the game and then update the expected hash.
		/// </summary>
		[Fact]
		public void PlaysTheRecordedGame()
		{
			Assert.Equal("50907F0EAD5AF527F0A46955E0B1D877D74E31485AA7D0FDC9ACF3C54090C7A0", PlayAndHashSaveGame(seed: 42, turns: 50));
		}

		private static string PlayAndHashSaveGame(int seed, int turns)
		{
			using GameHarness harness = GameHarness.Load("AITurns");
			harness.MakeAllPlayersComputerControlled();
			harness.SeedRandom(seed);

			harness.PlayTurns(turns, Timeout);

			Assert.True(harness.Game.LoadAndSave.SaveGameData("CIVIL9.SVE"));

			return Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(Path.Combine(harness.Directory, "CIVIL9.SVE"))));
		}

		/// <summary>
		/// Each player's units on a square form a circular list through NextUnitID. Returns a description of the first
		/// list that is broken, or null.
		/// </summary>
		private static string? FindBrokenUnitStack(GameData data)
		{
			for (int playerID = 0; playerID < 8; playerID++)
			{
				Unit[] units = data.Players[playerID].Units;

				for (int unitID = 0; unitID < 128; unitID++)
				{
					Unit unit = units[unitID];

					if (unit.UnitType == UnitTypeEnum.None || unit.NextUnitID == -1)
						continue;

					int nextUnitID = unit.NextUnitID;

					for (int steps = 0; nextUnitID != unitID; steps++)
					{
						if (nextUnitID < 0 || nextUnitID >= 128 || steps >= 128)
							return $"player {playerID} unit {unitID}: the stack does not lead back to the unit";

						Unit next = units[nextUnitID];

						if (next.UnitType == UnitTypeEnum.None || next.Position != unit.Position)
							return $"player {playerID} unit {unitID} at {unit.Position}: the stack links to unit {nextUnitID} ({next.UnitType} at {next.Position})";

						nextUnitID = next.NextUnitID;
					}
				}
			}

			return null;
		}
	}
}
