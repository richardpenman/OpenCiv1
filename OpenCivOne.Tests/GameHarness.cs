using IRB.VirtualCPU;
using OpenCivOne.Graphics;
using OpenCivOne.Resources;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.ExceptionServices;

namespace OpenCivOne.Tests
{
	/// <summary>
	/// Runs the game logic without a window, so saved games can be loaded and units moved from tests.
	/// </summary>
	internal sealed class GameHarness : IDisposable
	{
		private static readonly FieldInfo ResourcePathField =
			typeof(OpenCivOneGame).GetField("gameResourcePath", BindingFlags.NonPublic | BindingFlags.Instance) ??
			throw new MissingFieldException(nameof(OpenCivOneGame), "gameResourcePath");

		private static readonly MethodInfo MoveUnitMethod =
			typeof(PlayerTurn).GetMethod("MoveUnit", BindingFlags.NonPublic | BindingFlags.Instance) ??
			throw new MissingMethodException(nameof(PlayerTurn), "MoveUnit");

		public OpenCivOneGame Game { get; }

		/// <summary>
		/// A private copy of the save folder, so tests can write saves without touching the fixtures.
		/// </summary>
		public string Directory { get; }

		private GameHarness(string saveName)
		{
			this.Directory = Path.Combine(Path.GetTempPath(), "OpenCivOne.Tests", Guid.NewGuid().ToString("N"));
			System.IO.Directory.CreateDirectory(this.Directory);

			foreach (string extension in new[] { ".SVE", ".MAP" })
			{
				string file = saveName.ToUpperInvariant() + extension;

				File.Copy(Path.Combine(AppContext.BaseDirectory, "Saves", file), Path.Combine(this.Directory, file));
			}

			WritePlaceholderResources(this.Directory);

			this.Game = new OpenCivOneGame(null!);

			// Same emulated memory and stack setup as OpenCivOneGame.Start, without starting the game loop
			VCPU cpu = this.Game.CPU;
			cpu.Memory.AllocateMemoryBlock(0xff00, 0x100, VCPUMemoryFlagsEnum.ReadWrite);
			cpu.Memory.AllocateMemoryBlock(VCPU.ToLinearAddress(0x1000, 0), 0x3a0e0, VCPUMemoryFlagsEnum.ReadWrite);
			byte[] resources = CommonResources.BinaryResources;
			cpu.Memory.WriteBlock(VCPU.ToLinearAddress(0x35cf, 0), resources, 0, resources.Length);
			cpu.DS.UInt16 = 0x3b01;
			cpu.ES.UInt16 = 0x3b01;
			cpu.SS.UInt16 = 0x3b01;
			cpu.SP.UInt16 = 0xf0c0;

			ResourcePathField.SetValue(this.Game, this.Directory + Path.DirectorySeparatorChar);

			// Screens the game draws to; nothing is shown
			for (int i = 0; i < 3; i++)
			{
				this.Game.Graphics.AllocateScreen(i);
			}
		}

		// The original game's images and palettes, from the list OpenCivOneGame.Start checks for
		private static readonly string[] ImageFiles =
		{
			"ADSCREEN.PIC", "ARCH.PIC", "BACK0A.PIC", "BACK0M.PIC", "BACK1A.PIC", "BACK1M.PIC", "BACK2A.PIC", "BACK2M.PIC",
			"BACK3A.PIC", "BIRTH0.PIC", "BIRTH1.PIC", "BIRTH2.PIC", "BIRTH3.PIC", "BIRTH4.PIC", "BIRTH5.PIC", "BIRTH6.PIC",
			"BIRTH7.PIC", "BIRTH8.PIC", "CASTLE0.PIC", "CASTLE1.PIC", "CASTLE2.PIC", "CASTLE3.PIC", "CASTLE4.PIC", "CBACK.PIC",
			"CBACKS1.PIC", "CBACKS2.PIC", "CBACKS3.PIC", "CBRUSH0.PIC", "CBRUSH1.PIC", "CBRUSH2.PIC", "CBRUSH3.PIC", "CBRUSH4.PIC",
			"CBRUSH5.PIC", "CITYPIX1.PIC", "CITYPIX2.PIC", "CITYPIX3.PIC", "CUSTOM.PIC", "DIFFS.PIC", "DISCOVR1.PIC", "DISCOVR2.PIC",
			"DOCKER.PIC", "GOVT0A.PIC", "GOVT0M.PIC", "GOVT1A.PIC", "GOVT1M.PIC", "GOVT2A.PIC", "GOVT2M.PIC", "GOVT3A.PIC",
			"HILL.PIC", "ICONPG1.PIC", "ICONPG2.PIC", "ICONPG3.PIC", "ICONPG4.PIC", "ICONPG5.PIC", "ICONPG6.PIC", "ICONPG7.PIC",
			"ICONPG8.PIC", "ICONPGA.PIC", "ICONPGB.PIC", "ICONPGC.PIC", "ICONPGD.PIC", "ICONPGE.PIC", "ICONPGT1.PIC", "ICONPGT2.PIC",
			"INVADER2.PIC", "INVADER3.PIC", "INVADERS.PIC", "KING00.PIC", "KING01.PIC", "KING02.PIC", "KING03.PIC", "KING04.PIC",
			"KING05.PIC", "KING06.PIC", "KING07.PIC", "KING08.PIC", "KING09.PIC", "KING10.PIC", "KING11.PIC", "KING12.PIC",
			"KING13.PIC", "KINK00.PIC", "KINK03.PIC", "LOGO.PIC", "LOVE1.PIC", "LOVE2.PIC", "MAP.PIC", "NUKE1.PIC",
			"PLANET1.PIC", "PLANET2.PIC", "POP.PIC", "RIOT.PIC", "RIOT2.PIC", "SAD.PIC", "SETTLERS.PIC", "SLAG2.PIC",
			"SLAM1.PIC", "SLAM2.PIC", "SP257.PIC", "SP299.PIC", "SPACEST.PIC", "SPRITES.PIC", "TER257.PIC", "TORCH.PIC",
			"WONDERS.PIC", "WONDERS2.PIC",
		};

		private static readonly string[] PaletteFiles =
		{
			"BACK0A.PAL", "BACK0M.PAL", "BACK1A.PAL", "BACK1M.PAL", "BACK2A.PAL", "BACK2M.PAL", "BACK3A.PAL", "BIRTH0.PAL",
			"BIRTH1.PAL", "BIRTH2.PAL", "BIRTH3.PAL", "BIRTH4.PAL", "BIRTH5.PAL", "BIRTH6.PAL", "BIRTH7.PAL", "BIRTH8.PAL",
			"DISCOVR1.PAL", "DISCOVR2.PAL", "HILL.PAL", "ICONPG1.PAL", "ICONPGA.PAL", "KING00.PAL", "KING01.PAL", "KING02.PAL",
			"KING03.PAL", "KING04.PAL", "KING05.PAL", "KING06.PAL", "KING07.PAL", "KING08.PAL", "KING09.PAL", "KING10.PAL",
			"KING11.PAL", "KING12.PAL", "KING13.PAL", "SLAM1.PAL", "SP256.PAL", "SP257.PAL",
		};

		/// <summary>
		/// Writes stand-ins for the original game's text, image and palette files, which are copyrighted and not in the
		/// repository. The text files have the original section names with placeholder text, the images are blank and the
		/// palettes are black. None of them affects the game logic.
		/// </summary>
		private static void WritePlaceholderResources(string directory)
		{
			foreach (KeyValuePair<string, string[]> section in TextResourceKeys.Sections)
			{
				using FileStream stream = new FileStream(Path.Combine(directory, $"{section.Key}.TXT"), FileMode.Create);

				// The text starts after a 0x212 byte index
				stream.Write(new byte[0x212]);

				foreach (string key in section.Value)
				{
					stream.Write(System.Text.Encoding.ASCII.GetBytes($"{key}\r\nText.\r\n"));
				}
			}

			// One palette block ("M0", 770 bytes): colors 0 to 255, all black
			byte[] palette = new byte[4 + 770];
			palette[0] = (byte)'M';
			palette[1] = (byte)'0';
			palette[2] = 770 & 0xff;
			palette[3] = 770 >> 8;
			palette[5] = 0xff;

			foreach (string file in PaletteFiles)
			{
				File.WriteAllBytes(Path.Combine(directory, file), palette);
			}

			string blankImage = Path.Combine(directory, ImageFiles[0]);
			new GBitmap().SaveToPIC(blankImage, true);

			foreach (string file in ImageFiles.Skip(1))
			{
				File.Copy(blankImage, Path.Combine(directory, file));
			}
		}

		/// <summary>
		/// Loads OpenCivOne.Tests/Saves/{SAVENAME}.SVE and .MAP. The game upper-cases save file names, so the files must be upper case.
		/// </summary>
		public static GameHarness Load(string saveName)
		{
			GameHarness harness = new GameHarness(saveName);

			Assert.True(harness.Game.LoadAndSave.LoadGameData($"{saveName}.SVE"), $"Could not load save game {saveName}");

			return harness;
		}

		/// <summary>
		/// Seeds the game's random number generator, so combat and other random events repeat between runs.
		/// </summary>
		public void SeedRandom(int seed)
		{
			this.Game.CAPI.srand(seed);
		}

		/// <summary>
		/// Moves a unit one square, as if the player pressed a direction key.
		/// Directions: 1 up, 2 up-right, 3 right, 4 down-right, 5 down, 6 down-left, 7 left, 8 up-left.
		/// </summary>
		/// <remarks>
		/// Moves made by the human player redraw the map, which needs a window. The caller should first hand
		/// the human player role to an eliminated civilization with <see cref="UseEliminatedCivilizationAsHuman"/>.
		/// </remarks>
		public void MoveUnit(int playerID, int unitID, int direction)
		{
			try
			{
				MoveUnitMethod.Invoke(this.Game.PlayerTurn, new object?[] { playerID, this.Game.GameData.Players[playerID].Units[unitID], direction, 0 });
			}
			catch (TargetInvocationException e) when (e.InnerException != null)
			{
				throw e.InnerException;
			}
		}

		/// <summary>
		/// Makes an eliminated civilization the human player, so that moving the real player's units runs the
		/// same game logic without drawing to the screen or starting diplomacy dialogs.
		/// </summary>
		/// <returns>The player ID that was the human player.</returns>
		public int UseEliminatedCivilizationAsHuman()
		{
			int humanPlayerID = this.Game.GameData.HumanPlayerID;

			for (int i = 1; i < 8; i++)
			{
				if ((this.Game.GameData.ActiveCivilizations & (1 << i)) == 0)
				{
					this.Game.GameData.HumanPlayerID = (short)i;

					return humanPlayerID;
				}
			}

			throw new InvalidOperationException("The save game has no eliminated civilization to use as a stand-in human player");
		}

		/// <summary>
		/// Hands every civilization to the computer: the human player role goes to an eliminated civilization, and the
		/// game is set not to show the human's messages, animations or autosaves.
		/// </summary>
		public void MakeAllPlayersComputerControlled()
		{
			UseEliminatedCivilizationAsHuman();

			GameData data = this.Game.GameData;
			data.PlayerFlags = 0;
			data.GameSettingFlags.Animations = false;
			data.GameSettingFlags.Sound = false;
			data.GameSettingFlags.AutoSave = false;
			this.Game.GlobalPreferences.Debug.FastMode = true;
		}

		/// <summary>
		/// Plays whole game turns. Any message box the game shows is answered with Enter.
		/// </summary>
		/// <param name="turns">Number of turns to play</param>
		/// <param name="timeout">Fails if the turns take longer than this, for example when the game hangs</param>
		/// <param name="afterTurn">Called after each turn with the number of turns played</param>
		public void PlayTurns(int turns, TimeSpan timeout, Action<int>? afterTurn = null)
		{
			Exception? error = null;
			bool finished = false;
			Thread game = new Thread(() =>
			{
				try
				{
					for (int turn = 1; turn <= turns; turn++)
					{
						this.Game.GameEngine.F0_1238_0092_GameTurn();
						afterTurn?.Invoke(turn);
					}
				}
				catch (Exception e)
				{
					error = e;
				}
				finally
				{
					finished = true;
				}
			}, 64 * 1024 * 1024);

			game.IsBackground = true;
			game.Start();

			Stopwatch stopwatch = Stopwatch.StartNew();

			while (!finished && stopwatch.Elapsed < timeout)
			{
				lock (OpenCivOneGame.KeyboardAndMouseLock)
				{
					if (this.Game.Keys.Count == 0)
					{
						this.Game.Keys.Enqueue(0x0d);
					}
				}

				game.Join(2);
			}

			if (!finished)
			{
				Assert.Fail($"The game turns did not finish within {timeout}");
			}

			if (error != null)
			{
				ExceptionDispatchInfo.Capture(error).Throw();
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
}
