using System.Runtime.CompilerServices;
using System.Security.Cryptography;
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

		/// <summary>
		/// The game states themselves record what the computer players did in 50 and 120 turns, which covers the code
		/// they run: city production and growth, disasters, founding cities, building spaceships and so on.
		/// </summary>
		[Theory]
		[MemberData(nameof(StateNames))]
		public void ComputerPlayersPlayTheSameGame(string state)
		{
			using IncrementalHash hash = IncrementalHash.CreateHash(HashAlgorithmName.SHA256);

			foreach (string extension in new[] { ".SVE", ".MAP" })
			{
				hash.AppendData(File.ReadAllBytes(Path.Combine(this.states.Directory, state.ToUpperInvariant() + extension)));
			}

			Compare($"{state}/GameState", Convert.ToHexString(hash.GetHashAndReset())[..16]);
		}

		[Theory]
		[MemberData(nameof(StateNames))]
		public void AdvisorReports(string state)
		{
			Check(state, "ScienceReport", (game, player) => game.AdvisorReports.ScienceReport(player), Escape, Escape, Escape);
			Check(state, "MilitaryReport", (game, player) => game.AdvisorReports.MilitaryReport(player), Escape, Escape, Escape);
			Check(state, "TradeReport", (game, player) => game.AdvisorReports.TradeReport(player), Escape, Escape, Escape);
			Check(state, "IntelligenceReport", (game, player) => game.AdvisorReports.IntelligenceReport(), Escape, Escape, Escape);
			Check(state, "AttitudeReport", (game, player) => game.AdvisorReports.AttitudeReport(player), Escape, Escape, Escape);
			Check(state, "CityStatus", (game, player) => game.AdvisorReports.CityStatus(player), Escape, Escape, Escape);

			for (short otherPlayer = 1; otherPlayer < 8; otherPlayer++)
			{
				short playerID = otherPlayer;
				Check(state, $"ForeignAffairs{playerID}", (game, player) => game.AdvisorReports.PlayerIntelligenceReport(playerID), Escape, Escape, Escape);
			}

			// Every player's reports, from that player's point of view, including the pages after the first
			int[] pages = Enumerable.Repeat(Escape, 8).ToArray();

			for (short otherPlayer = 1; otherPlayer < 8; otherPlayer++)
			{
				short playerID = otherPlayer;

				Check(state, $"Player{playerID}/ScienceReport", (game, player) => game.AdvisorReports.ScienceReport(playerID), playerID, pages);
				Check(state, $"Player{playerID}/MilitaryReport", (game, player) => game.AdvisorReports.MilitaryReport(playerID), playerID, pages);
				Check(state, $"Player{playerID}/TradeReport", (game, player) => game.AdvisorReports.TradeReport(playerID), playerID, pages);
				Check(state, $"Player{playerID}/AttitudeReport", (game, player) => game.AdvisorReports.AttitudeReport(playerID), playerID, pages);
				Check(state, $"Player{playerID}/CityStatus", (game, player) => game.AdvisorReports.CityStatus(playerID), playerID, pages);

				// The same with an embassy with every civilization, so that the details can be opened
				Check(state, $"Player{playerID}/IntelligenceReportWithEmbassies", (game, player) =>
				{
					for (int i = 0; i < 8; i++)
					{
						game.GameData.Players[playerID].Diplomacy[i] |= DiplomacyFlagsEnum.EmbassyEstablished;
					}

					game.AdvisorReports.IntelligenceReport();
				}, playerID, '1', Escape, '2', Escape, '3', Escape, '4', Escape, '5', Escape, '6', Escape, '7', Escape, Escape);

				// Open the details of each civilization in turn with the keys 1 to 7
				Check(state, $"Player{playerID}/IntelligenceReport", (game, player) => game.AdvisorReports.IntelligenceReport(), playerID,
					'1', Escape, '2', Escape, '3', Escape, '4', Escape, '5', Escape, '6', Escape, '7', Escape, Escape);
			}
		}

		[Theory]
		[MemberData(nameof(StateNames))]
		public void AdvisorReportsWithManyCities(string state)
		{
			// Every city belongs to the human player, so the reports have several pages
			int[] pages = Enumerable.Repeat(Escape, 8).ToArray();

			void GiveAllCities(OpenCivOneGame game, short player)
			{
				foreach (City city in game.GameData.Cities)
				{
					if (city.Status != CityStatusEnum.Invalid)
					{
						city.PlayerID = player;
					}
				}
			}

			Check(state, "AllCities/TradeReport", (game, player) => { GiveAllCities(game, player); game.AdvisorReports.TradeReport(player); }, pages);
			Check(state, "AllCities/AttitudeReport", (game, player) => { GiveAllCities(game, player); game.AdvisorReports.AttitudeReport(player); }, pages);
			Check(state, "AllCities/CityStatus", (game, player) => { GiveAllCities(game, player); game.AdvisorReports.CityStatus(player); }, pages);
		}

		[Theory]
		[MemberData(nameof(StateNames))]
		public void CityEventsAndScore(string state)
		{
			int[] keys = Enumerable.Repeat(Enter, 6).ToArray();
			GameData data;

			using (GameHarness harness = GameHarness.Load(this.states.Directory, state))
			{
				data = harness.Game.GameData;
			}

			// Disasters and riots in every city, with the city's owner as the human player so that messages are shown
			for (int cityID = 0; cityID < 128; cityID++)
			{
				City city = data.Cities[cityID];

				if (city.Status == CityStatusEnum.Invalid)
					continue;

				short cityIDCopy = (short)cityID;

				for (int seed = 0; seed < 3; seed++)
				{
					int seedCopy = seed;

					Check(state, $"CityEvent/{cityID}/{seed}", (game, player) =>
					{
						game.GameData.PlayerFlags = (short)(1 << player);
						game.CAPI.srand(seedCopy);
						game.CityEvents.RandomCityEvent(cityIDCopy);
					}, city.PlayerID, keys);
				}
			}

			// A computer player founds cities on empty land
			int founded = 0;

			for (int y = 4; y < 46 && founded < 6; y += 3)
			{
				for (int x = 0; x < 80 && founded < 6; x += 7)
				{
					if (data.Cities.Any(city => city.Status != CityStatusEnum.Invalid && Math.Abs(city.Position.X - x) <= 3 && Math.Abs(city.Position.Y - y) <= 3))
						continue;

					int xCopy = x;
					int yCopy = y;
					int founder = (founded % 7) + 1;
					founded++;

					Check(state, $"FoundCity/{x},{y}", (game, player) =>
					{
						if (game.MapManagement.GetTerrainType(xCopy, yCopy) != TerrainTypeEnum.Water)
						{
							game.CityEvents.FoundCity(founder, xCopy, yCopy, 1);
						}
					}, Escape);
				}
			}

			// Every city and wonder belongs to the human player, so that the score doesn't fit on the screen, with every
			// kind of extra score
			Check(state, "CivilizationScoreFull", (game, player) =>
			{
				GameData gameData = game.GameData;

				foreach (City city in gameData.Cities)
				{
					if (city.Status != CityStatusEnum.Invalid)
					{
						city.PlayerID = player;
					}
				}

				Player human = gameData.Players[player];
				human.SpaceshipPopulation = 120;
				human.SpaceshipSuccessRate = 80;
				human.SpaceshipETAYear = gameData.Year;
				human.FutureTechnologyCount = 3;
				gameData.PollutedSquareCount = 7;
				gameData.PeaceTurnCount = 20;
				game.Var_b884 = 1;

				game.CivilizationScore.ShowCivilizationScore(player, true);
			}, Escape, Escape, Escape);

			// Large cities: the citizens need more rows than fit, so the rows are moved closer together
			Check(state, "CivilizationScoreTooLong", (game, player) =>
			{
				foreach (City city in game.GameData.Cities)
				{
					if (city.Status != CityStatusEnum.Invalid)
					{
						city.PlayerID = player;
						city.ActualSize = 30;
					}
				}

				game.CivilizationScore.ShowCivilizationScore(player, true);
				game.CivilizationScore.ShowCivilizationScore(player, true);
			}, Escape, Escape, Escape, Escape, Escape, Escape);

			Check(state, "CivilizationScore", (game, player) => game.CivilizationScore.ShowCivilizationScore(player, true), Escape, Escape, Escape);
			Check(state, "CivilizationScoreEndOfGame", (game, player) => game.CivilizationScore.ShowCivilizationScore(player, false), Escape, Escape, Escape);
		}

		private void Check(string state, string name, Action<OpenCivOneGame, short> action, params int[] keys)
		{
			Check(state, name, action, this.states.HumanPlayerID, keys);
		}

		/// <summary>
		/// Runs the action with the given player as the human player
		/// </summary>
		private void Check(string state, string name, Action<OpenCivOneGame, short> action, short humanPlayerID, params int[] keys)
		{
			using GameHarness harness = GameHarness.Load(this.states.Directory, state);
			harness.Game.GameData.HumanPlayerID = humanPlayerID;

			List<string> screens = harness.RunWithKeys(() => action(harness.Game, humanPlayerID), TimeSpan.FromMinutes(1), keys);

			string actual = string.Join(" ", screens.Select(hash => hash[..16])) + " | " + harness.HashGameState()[..16];

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
