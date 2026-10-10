using IRB.VirtualCPU;
using OpenCivOne.Resources;
using System.Reflection;

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
