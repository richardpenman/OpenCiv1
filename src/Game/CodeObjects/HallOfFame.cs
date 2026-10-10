namespace OpenCivOne
{
	public class HallOfFame
	{
		private OpenCivOneGame parent;

		HallOfFameEntry[] entries = new HallOfFameEntry[6];

		public HallOfFame(OpenCivOneGame parent)
		{
			this.parent = parent;

			for (int i = 0; i < 6; i++)
			{
				entries[i] = new HallOfFameEntry();
			}
		}

		/// <summary>
		/// Initializes Hall Of Fame table
		/// </summary>
		public void F3_0000_0000_InitHallOfFame()
		{
			//this.oCPU.Log.EnterBlock("F3_0000_0000()");

			// function body
			for (int i = 0; i < 6; i++)
			{
				entries[i] = new HallOfFameEntry();
			}
		}

		/// <summary>
		/// Reads Hall Of Fame data from 'FAME.DTA'
		/// </summary>
		public void F3_0000_002b_ReadHallOfFameFromFile()
		{
			//this.oCPU.Log.EnterBlock("F3_0000_002b()");

			// function body
			F3_0000_0000_InitHallOfFame();

			// Instruction address 0x0000:0x0041, size: 5
			FileStream file = new FileStream($"{this.parent.ResourcePath}FAME.DTA", FileMode.Open, FileAccess.Read);

			for (int i = 0; i < 6; i++)
			{
				entries[i] = HallOfFameEntry.ReadFromStream(file);
			}

			file.Close();
		}

		/// <summary>
		/// Writes HallOf Fame Data to 'FAME.DTA'
		/// </summary>
		public void F3_0000_0083_WriteHallOfFameToFile()
		{
			//this.oCPU.Log.EnterBlock("F3_0000_0083()");

			// function body
			// Instruction address 0x0000:0x0095, size: 5
			FileStream file = new FileStream($"{this.parent.ResourcePath}FAME.DTA", FileMode.OpenOrCreate, FileAccess.Write);

			for (int i = 0; i < 6; i++)
			{
				entries[i].WriteToStream(file);
			}

			file.Close();			
		}

		/// <summary>
		/// Shows Hall Of Fame window
		/// </summary>
		/// <param name="playerIndex"></param>
		/// <returns></returns>
		public bool F3_0000_00d7_ShowHallOfFameWindow(int playerIndex)
		{
			//this.oCPU.Log.EnterBlock($"F3_0000_00d7({playerIndex})");

			// function body
			// Instruction address 0x0000:0x00f3, size: 5
			this.parent.DrawTools.FillRectangle(this.parent.Var_aa_Screen0_Rectangle, 0, 0, 320, 200, 15);

			// Instruction address 0x0000:0x010a, size: 5
			this.parent.DrawTools.DrawCenteredStringToScreen0("CIVILIZATION", 160, 16, 0);

			// Instruction address 0x0000:0x0121, size: 5
			this.parent.DrawTools.DrawCenteredStringToScreen0("HALL OF FAME", 160, 24, 0);

			// Instruction address 0x0000:0x013d, size: 5
			this.parent.Graphics.F0_VGA_0599_DrawLine(this.parent.Var_aa_Screen0_Rectangle, 80, 32, 240, 32, 14);

			int y = 40;

			for (int i = 0; i < 6; i++)
			{
				HallOfFameEntry entry = entries[i];

				int x = 8;

				if (entry.TotalScore <= 0)
				{
					// Instruction address 0x0000:0x0211, size: 5
					this.parent.DrawTools.DrawStringToScreen0(" ---", 8, y, 0);

					y += 26;
				}
				else
				{
					// Instruction address 0x0000:0x037c, size: 5
					this.parent.DrawTools.DrawStringToScreen0($"{i + 1}. {entry.PlayerName}, {this.parent.Array_33a2_GameDifficultyNames[entry.GameDifficulty]}" +
						$" of the {entry.NationName} to {this.parent.GameEngine.YearToString(entry.FinalYear)}", x, y, 0);

					y += 8;

					// Instruction address 0x0000:0x0438, size: 5
					this.parent.DrawTools.DrawStringToScreen0(
						$"Population: {this.parent.GameEngine.PopulationValueToString(entry.TotalPopulation)}, Score: {entry.TotalScore}", x + 12, y, 7);

					y += 8;

					// Instruction address 0x0000:0x04ab, size: 5
					this.parent.DrawTools.DrawStringToScreen0(
						$"    --- CIVILIZATION RATING: {(entry.TotalScore * (entry.GameDifficulty + 1)) / 50}% ---", x + 12, y, 12);

					y += 10;

					if (i == playerIndex)
					{
						// Instruction address 0x0000:0x04e8, size: 5
						this.parent.Graphics.F0_VGA_009a_ReplaceColor(this.parent.Var_aa_Screen0_Rectangle, x - 4, y - 28, 296, 26, 15, 14);
					}
				}
			}

			if (playerIndex != -1)
			{
				// Instruction address 0x0000:0x0189, size: 5
				this.parent.DrawTools.DrawStringToScreen0("'C\x0083lear'", 252, 189, 0);

				// Instruction address 0x0000:0x01a5, size: 5
				this.parent.DrawTools.DrawRectangle(236, 187, 60, 10, 11);
			}
		
			// Instruction address 0x0000:0x01ad, size: 5
			int keyPress = this.parent.Actions.F0_2459_0918_WaitForKeyPressOrMouseClick();

			return (playerIndex != -1 && (keyPress == 'c' || keyPress == 'C')) ? true : false;
		}

		/// <summary>
		/// Add new entry to a Hall Of Fame if rating is better than any of the current entries
		/// </summary>
		public void F3_0000_0513_AddNewEntryToHallOfFame()
		{
			//this.oCPU.Log.EnterBlock("F3_0000_0513()");

			// function body
			while (true)
			{
				HallOfFameEntry entry = new HallOfFameEntry();
				Player humanPlayer = this.parent.GameData.Players[this.parent.GameData.HumanPlayerID];

				entry.TotalScore = humanPlayer.TotalPlayerScore;
				entry.GameDifficulty = this.parent.GameData.DifficultyLevel;
				entry.AlwaysZero = 0; // always zero
				entry.FinalYear = this.parent.GameData.Year;
				entry.AchievementsScore = humanPlayer.AchievementScore;
				entry.TotalCitizenCount = humanPlayer.TotalCitizenCount;
				entry.TotalPopulation = this.parent.GameEngine.GetPlayerPopulationCount(this.parent.GameData.HumanPlayerID);
				entry.PlayerName = humanPlayer.Name;
				entry.NationName = humanPlayer.Nation;

				int rank = -1;

				for (int i = 0; i < 6; i++)
				{
					if (entry.Rating > entries[i].Rating)
					{
						for (int j = 4; j > i; j--)
						{
							entries[j + 1] = entries[j];
						}

						entries[i] = entry;
						rank = i;
						break;
					}
				}

				if (!F3_0000_00d7_ShowHallOfFameWindow(rank))
					return;

				F3_0000_0000_InitHallOfFame();
			}
		}

		private static string[] famousPersons = { "Dan Quayle", "Emperor Nero", "Ferdinand Marcos",
			"Neville Chamberlain", "Louis the XVI", "Kaiser Wilhelm", "Eric the Red",
			"Otto von Bismarck", "Simon Bolivar", "Vladimir Lenin", "Charles De Gaulle",
			"Cleopatra", "Winston Churchill", "Suleyman the Magnificent", "Franklin Roosevelt",
			"Shogun Tokugawa", "Thomas Jefferson", "King Charlemagne", "Emperor Augustus",
			"Solomon the Wise"};

		/// <summary>
		/// Shows famous persons list and compare them to the player score
		/// </summary>
		/// <param name="personIndex"></param>
		/// <param name="score"></param>
		public void F3_0000_065d_ShowFamousPersonsWindow(int personIndex, int score)
		{
			//this.oCPU.Log.EnterBlock($"F3_0000_065d({param1}, {param2})");

			// function body
			if (score != 0)
			{
				int local_2 = (score * (this.parent.GameData.DifficultyLevel + 1)) / 50;
				
				personIndex = -1;

				for (int i = 1; i < 21; i++)
				{
					if ((i * i) / 3 < local_2)
					{
						personIndex = i - 1;
					}
				}
			}

			if (personIndex >= 0)
			{
				if (personIndex > 19)
				{
					personIndex = 19;
				}
			
				this.parent.Graphics.F0_VGA_07d8_DrawImage(this.parent.Var_aa_Screen0_Rectangle, 0, 0, 320, 200, this.parent.Var_19d4_Screen1_Rectangle, 0, 0);

				// Instruction address 0x0000:0x06c8, size: 5
				this.parent.CommonTools.PlayTune(34, 0);

				// Instruction address 0x0000:0x06e4, size: 5
				this.parent.DrawTools.FillRectangle(this.parent.Var_aa_Screen0_Rectangle, 0, 0, 320, 200, 15);

				// Instruction address 0x0000:0x06ec, size: 5
				this.parent.UnitManagement.F0_1866_260e_DrawRandomFoodIcons();

				// Instruction address 0x0000:0x070e, size: 5
				this.parent.Graphics.F0_VGA_07d8_DrawImage(this.parent.Var_19e8_Screen2_Rectangle, 200, 140, 40, 60, this.parent.Var_aa_Screen0_Rectangle, 10, 10);

				// Instruction address 0x0000:0x0736, size: 5
				this.parent.Graphics.F0_VGA_07d8_DrawImage(this.parent.Var_19e8_Screen2_Rectangle, 240, 140, 40, 60, this.parent.Var_aa_Screen0_Rectangle, 270, 10);

				this.parent.Var_aa_Screen0_Rectangle.FontID = 7;

				// Instruction address 0x0000:0x0757, size: 5
				this.parent.DrawTools.FillRectangleWithPattern(80, 4, 160, 27);

				// Instruction address 0x0000:0x0777, size: 5
				this.parent.DrawTools.DrawShadowRectangle(80, 4, 159, 26, 11, 8);

				if (score == 0)
				{
					// Instruction address 0x0000:0x07b0, size: 5
					this.parent.DrawTools.DrawCenteredStringToScreen0("Sire, your fame now", 160, 6, 15);

					// Instruction address 0x0000:0x07e0, size: 5
					this.parent.DrawTools.DrawCenteredStringToScreen0("exceeds even that of", 160, 14, 15);

					// Instruction address 0x0000:0x08ea, size: 5
					this.parent.DrawTools.DrawCenteredStringToScreen0($"{famousPersons[personIndex]}!", 160, 22, 15);
				}
				else
				{
					int rating = (score * (this.parent.GameData.DifficultyLevel + 1)) / 50;

					// Instruction address 0x0000:0x083c, size: 5
					this.parent.DrawTools.DrawCenteredStringToScreen0("Sire, your Civilization", 160, 6, 15);

					// Instruction address 0x0000:0x089c, size: 5
					this.parent.DrawTools.DrawCenteredStringToScreen0($"Rating of {rating}% exceeds even", 160, 14, 15);

					// Instruction address 0x0000:0x08ea, size: 5
					this.parent.DrawTools.DrawCenteredStringToScreen0($"{famousPersons[personIndex]}!", 160, 22, 15);
				}

				for (int i = 0; i <= personIndex; i++)
				{
					if (i != personIndex)
					{
						// Instruction address 0x0000:0x0912, size: 5
						this.parent.DrawTools.DrawCenteredStringToScreen0(famousPersons[i], 160, 184 - (i * 8), 7);
					}
					else
					{
						// Instruction address 0x0000:0x0912, size: 5
						this.parent.DrawTools.DrawCenteredStringToScreen0(famousPersons[i], 160, 184 - (i * 8), 0);
					}
				}

				// Instruction address 0x0000:0x097c, size: 5
				this.parent.Graphics.F0_VGA_009a_ReplaceColor(this.parent.Var_aa_Screen0_Rectangle, 80, 183 - personIndex * 8, 160, 8, 15, 14);

				// Instruction address 0x0000:0x0984, size: 5
				this.parent.Actions.F0_2459_0918_WaitForKeyPressOrMouseClick();

				// Instruction address 0x0000:0x098d, size: 5
				this.parent.CommonTools.PlayTune(1, 0);

				this.parent.Var_aa_Screen0_Rectangle.FontID = 1;

				this.parent.Graphics.F0_VGA_07d8_DrawImage(this.parent.Var_19d4_Screen1_Rectangle, 0, 0, 320, 200, this.parent.Var_aa_Screen0_Rectangle, 0, 0);
			}
		}
	}
}
