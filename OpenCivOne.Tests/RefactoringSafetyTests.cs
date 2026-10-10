using System.Runtime.CompilerServices;
using System.Text.Json;

namespace OpenCivOne.Tests
{
	/// <summary>
	/// Game states for RefactoringSafetyTests, built once per test run: the AITURNS save as it is, and after the computer
	/// players have played 50 and 120 turns.
	/// </summary>
	public sealed class GameStatesFixture : IDisposable
	{
		public string Directory { get; } = Path.Combine(Path.GetTempPath(), "OpenCivOne.Tests", "States-" + Guid.NewGuid().ToString("N"));

		/// <summary>
		/// The player who is human in the AITURNS save
		/// </summary>
		public short HumanPlayerID { get; }

		public static readonly (string Name, int Seed, int Turns)[] States =
		{
			("Start", 0, 0),
			("Turn50", 42, 50),
			("Turn120", 8, 120),
		};

		public GameStatesFixture()
		{
			System.IO.Directory.CreateDirectory(this.Directory);

			foreach (var (name, seed, turns) in States)
			{
				using GameHarness harness = GameHarness.Load("AITurns");
				this.HumanPlayerID = harness.Game.GameData.HumanPlayerID;

				if (turns > 0)
				{
					harness.MakeAllPlayersComputerControlled();
					harness.SeedRandom(seed);
					harness.PlayTurns(turns, TimeSpan.FromMinutes(5));
				}

				Assert.True(harness.Game.LoadAndSave.SaveGameData("CIVIL9.SVE"));

				foreach (string extension in new[] { ".SVE", ".MAP" })
				{
					File.Copy(Path.Combine(harness.Directory, "CIVIL9" + extension), Path.Combine(this.Directory, name.ToUpperInvariant() + extension));
				}
			}
		}

		public void Dispose()
		{
			try
			{
				System.IO.Directory.Delete(this.Directory, true);
			}
			catch (IOException)
			{
			}
		}
	}

	/// <summary>
	/// Records what game code draws and how it changes the game, so that rewriting code (for example replacing the emulated
	/// x86 code with C#) can be checked to behave exactly the same. Each check loads a game state, runs one function with
	/// the given keys, and compares the screens each time the function waited for a key, and the game state afterwards,
	/// with the values in Expected/RefactoringSafety.json.
	///
	/// To record new expected values (only when a change is meant to alter what the game does or draws), run the tests
	/// with the environment variable RECORD_EXPECTED=1 and review the changes to the JSON file.
	/// </summary>
	public class RefactoringSafetyTests : IClassFixture<GameStatesFixture>
	{
		private const int Escape = 0x1b;
		private const int Enter = 0x0d;

		private static readonly object ExpectedLock = new();
		private readonly GameStatesFixture states;

		public RefactoringSafetyTests(GameStatesFixture states)
		{
			this.states = states;
		}

		public static IEnumerable<object[]> StateNames => GameStatesFixture.States.Select(state => new object[] { state.Name });

		[Theory]
		[MemberData(nameof(StateNames))]
		public void AdvisorReports(string state)
		{
			Check(state, "ScienceReport", (game, player) => game.Overlay_14.F14_0000_014b_ScienceReport(player), Escape, Escape, Escape);
			Check(state, "MilitaryReport", (game, player) => game.Overlay_14.F14_0000_03ad_MilitaryReport(player), Escape, Escape, Escape);
			Check(state, "TradeReport", (game, player) => game.Overlay_14.F14_0000_07f1_TradeReport(player), Escape, Escape, Escape);
			Check(state, "IntelligenceReport", (game, player) => game.Overlay_14.F14_0000_0d43_IntelligenceReport(), Escape, Escape, Escape);
			Check(state, "AttitudeReport", (game, player) => game.Overlay_14.F14_0000_15f4_AttitudeReport(player), Escape, Escape, Escape);
			Check(state, "CityStatus", (game, player) => game.Overlay_14.F14_0000_186f_CityStatus(player), Escape, Escape, Escape);

			for (short otherPlayer = 1; otherPlayer < 8; otherPlayer++)
			{
				short playerID = otherPlayer;
				Check(state, $"ForeignAffairs{playerID}", (game, player) => game.Overlay_14.F14_0000_1164(playerID), Escape, Escape, Escape);
			}
		}

		private void Check(string state, string name, Action<OpenCivOneGame, short> action, params int[] keys)
		{
			using GameHarness harness = GameHarness.Load(this.states.Directory, state);
			harness.Game.GameData.HumanPlayerID = this.states.HumanPlayerID;

			List<string> screens = harness.RunWithKeys(() => action(harness.Game, this.states.HumanPlayerID), TimeSpan.FromMinutes(1), keys);

			string actual = string.Join(" ", screens.Select(hash => hash.Substring(0, 16))) + " | " + harness.HashGameState().Substring(0, 16);

			Compare($"{state}/{name}", actual);
		}

		private static void Compare(string key, string actual)
		{
			lock (ExpectedLock)
			{
				string path = ExpectedFilePath();
				SortedDictionary<string, string> expected = File.Exists(path) ?
					JsonSerializer.Deserialize<SortedDictionary<string, string>>(File.ReadAllText(path)) ?? new() : new();

				if (Environment.GetEnvironmentVariable("RECORD_EXPECTED") == "1")
				{
					expected[key] = actual;
					File.WriteAllText(path, JsonSerializer.Serialize(expected, new JsonSerializerOptions { WriteIndented = true }) + "\n");
					return;
				}

				Assert.True(expected.ContainsKey(key), $"No expected value recorded for {key}; run the tests with RECORD_EXPECTED=1");
				Assert.True(expected[key] == actual, $"{key} drew or changed the game differently.\nExpected: {expected[key]}\nActual:   {actual}");
			}
		}

		private static string ExpectedFilePath([CallerFilePath] string sourceFile = "")
		{
			return Path.Combine(Path.GetDirectoryName(sourceFile)!, "Expected", "RefactoringSafety.json");
		}
	}
}
