using System.Globalization;

namespace OpenCivOne
{
	/// <summary>
	/// The civilization score screen
	/// </summary>
	public class CivilizationScore
	{
		private OpenCivOneGame oParent;

		// The space between the rows of citizen icons. The original reduces it, for the rest of the game, whenever the
		// score doesn't fit on the screen.
		private int rowSpacing = 8;

		// Where the next citizen icon is drawn
		private int citizenIconX = 0;
		private int citizenIconY = 0;

		public CivilizationScore(OpenCivOneGame parent)
		{
			this.oParent = parent;
		}

		/// <summary>
		/// Calculates the human player's score, and shows the Civilization Score window
		/// </summary>
		/// <param name="playerID">The player whose citizens and wonders are counted</param>
		/// <param name="show">Shows the score window, otherwise only calculates the score</param>
		/// <returns>The score</returns>
		public int ShowCivilizationScore(short playerID, bool show)
		{
			GameData gameData = this.oParent.GameData;
			Player player = gameData.Players[playerID];
			Player humanPlayer = gameData.Players[gameData.HumanPlayerID];

			// The score is drawn off screen, and copied to the screen when it is complete
			if (show)
			{
				this.oParent.Var_aa_Screen0_Rectangle.ScreenID = 1;
			}

			while (true)
			{
				if (show)
				{
					this.oParent.AdvisorReports.ShowReportHeader("CIVILIZATION SCORE", 3);
				}

				// The score is added up in the barbarian player's Score
				gameData.Players[0].Score = 0;

				if (show && (gameData.SpaceshipFlags & 0x100) != 0)
				{
					this.oParent.DrawTools.DrawCenteredStringToScreen0("SCORING COMPLETED", 160, 100, 15);

					this.oParent.Var_aa_Screen0_Rectangle.ScreenID = 0;
					this.oParent.Graphics.F0_VGA_07d8_DrawImage(this.oParent.Var_19d4_Screen1_Rectangle, 0, 0, 320, 200, this.oParent.Var_aa_Screen0_Rectangle, 0, 0);
					this.oParent.Actions.F0_2459_0918_WaitForKeyPressOrMouseClick();
					this.oParent.GameEngine.F0_1238_1b44_RedrawMapWithBlankingEffect();

					return 0;
				}

				int rowY = 26;
				int headerY = 26;
				short citizenScore = 0;

				this.citizenIconX = 999;
				this.citizenIconY = 26;

				// Citizens: happy ones count twice, unhappy ones not at all. The cities are calculated with the player's
				// taxes all going to luxuries.
				short savedTaxRate = player.TaxRate;
				player.TaxRate = (short)(6 - player.ScienceTaxRate);

				for (int cityID = 0; cityID < 128; cityID++)
				{
					City city = gameData.Cities[cityID];

					if (city.Status == CityStatusEnum.Invalid || city.PlayerID != playerID)
						continue;

					this.oParent.CityWorker.F0_1d12_0045_ProcessCityState(cityID, -1);

					int happyCount = this.oParent.Var_70e2_CityHappyCitizenCount;
					int unhappyCount = this.oParent.Var_70e4_CityUnhappyCitizenCount;
					int specialistCount = this.oParent.Var_e8b8;

					if (show)
					{
						for (int i = 0; i < happyCount; i++)
						{
							DrawCitizenIcon(i & 1);
						}

						for (int i = 0; (short)(city.ActualSize - happyCount - unhappyCount - specialistCount) > i; i++)
						{
							DrawCitizenIcon((i & 1) + 2);
						}

						for (int i = 0; i < unhappyCount; i++)
						{
							DrawCitizenIcon((i & 1) + 4);
						}

						for (int i = 0; i < specialistCount; i++)
						{
							DrawCitizenIcon(this.oParent.CityWorker.F0_1d12_6da1_GetSpecialWorkerFlags(i) + 5);
						}
					}

					citizenScore = (short)(citizenScore + city.ActualSize + happyCount - unhappyCount);
				}

				player.TaxRate = savedTaxRate;
				humanPlayer.TotalCitizenCount = citizenScore;

				if (show)
				{
					this.oParent.DrawTools.DrawStringWithShadow(
						string.Create(CultureInfo.InvariantCulture, $"{player.Nationality} Citizens ({humanPlayer.TotalCitizenCount})"), 16, headerY, 15);
				}

				// Wonders: 20 points each
				rowY = this.citizenIconY + 16;
				headerY = rowY;

				int wonderX = 999;

				humanPlayer.AchievementScore = 0;

				for (int wonderID = 1; wonderID <= 21; wonderID++)
				{
					int wonderCityID = this.oParent.GameData.WonderCityID[wonderID];

					if (wonderCityID == -1 || wonderCityID == 128 || gameData.Cities[wonderCityID].PlayerID != playerID)
						continue;

					wonderX += 152;

					if (wonderX > 300)
					{
						rowY += this.oParent.CommonTools.CheckValueRange(this.rowSpacing + 2, 8, 99);
						wonderX = 16;
					}

					if (show && rowY < 180)
					{
						this.oParent.CityWorker.F0_1d12_7045_DrawWonderOrImprovement(wonderID + 24, wonderX, rowY - 2);
						this.oParent.DrawTools.DrawStringToScreen0(gameData.Wonders[wonderID].Name, wonderX + 20, rowY, 11);
					}

					humanPlayer.AchievementScore += 20;
				}

				if (show)
				{
					this.oParent.DrawTools.DrawStringWithShadow(
						string.Create(CultureInfo.InvariantCulture, $"{player.Nationality} Achievements ({humanPlayer.AchievementScore})"), 16, headerY, 15);
				}

				gameData.Players[0].Score = (short)(humanPlayer.AchievementScore + humanPlayer.TotalCitizenCount);

				rowY += 10;

				// Spaceship: half the population times the chance of success, if it has arrived
				if (humanPlayer.SpaceshipPopulation != 0 && humanPlayer.SpaceshipSuccessRate != 0)
				{
					int spaceshipScore = Math.Max((short)(humanPlayer.SpaceshipSuccessRate * humanPlayer.SpaceshipPopulation) / 2, 0);
					string spaceship = string.Create(CultureInfo.InvariantCulture, $"SpaceShip: ({spaceshipScore}");

					if (humanPlayer.SpaceshipETAYear == gameData.Year)
					{
						gameData.Players[0].Score += (short)spaceshipScore;
					}
					else
					{
						spaceship += " possible";
					}

					spaceship += ")";

					if (show && rowY < 184)
					{
						this.oParent.DrawTools.DrawStringWithShadow(spaceship, 16, rowY, 15);
					}

					rowY += 10;
				}

				string text = "";

				if (gameData.PollutedSquareCount != 0)
				{
					text = string.Create(CultureInfo.InvariantCulture, $"Pollution: ({(short)(-10 * gameData.PollutedSquareCount)})   ");
					gameData.Players[0].Score -= (short)(10 * gameData.PollutedSquareCount);
				}

				if (gameData.Year > 0 && gameData.PeaceTurnCount != 0)
				{
					int peaceScore = this.oParent.CommonTools.CheckValueRange(gameData.PeaceTurnCount * 3, 0, 100);

					text += string.Create(CultureInfo.InvariantCulture, $"Peace: (+{peaceScore}) ");
					gameData.Players[0].Score += (short)peaceScore;
				}

				if (humanPlayer.FutureTechnologyCount != 0)
				{
					text += string.Create(CultureInfo.InvariantCulture, $"Future Tech: (+{(short)(5 * humanPlayer.FutureTechnologyCount)})");
					gameData.Players[0].Score += (short)(5 * humanPlayer.FutureTechnologyCount);
				}

				if (text.Length > 0 && show && rowY < 184)
				{
					this.oParent.DrawTools.DrawStringWithShadow(text, 16, rowY, 15);
					rowY += 10;
				}

				// When the game ends early, the score is the bonus for the turns that were left, if it is higher
				short bonusScore = -1;

				if (this.oParent.Var_b884 != 0)
				{
					bonusScore = (short)(((0x226 - gameData.TurnCount) * 2) + (100 * gameData.AIOpponentCount) + 400);
				}

				if (gameData.Players[0].Score < 0)
				{
					gameData.Players[0].Score = 0;
				}

				if (bonusScore < gameData.Players[0].Score)
				{
					text = string.Create(CultureInfo.InvariantCulture, $"Total Score: {gameData.Players[0].Score}");
				}
				else
				{
					text = string.Create(CultureInfo.InvariantCulture, $"Bonus Score: {bonusScore}");
					gameData.Players[0].Score = bonusScore;
				}

				if (show && rowY < 186)
				{
					this.oParent.DrawTools.DrawStringWithShadow(text, 16, rowY, 15);
				}

				if (rowY >= 186)
				{
					// The score doesn't fit, try again with the rows closer together
					this.rowSpacing--;
					continue;
				}

				humanPlayer.TotalPlayerScore = gameData.Players[0].Score;

				if (show)
				{
					// The score bar
					this.oParent.DrawTools.FillRectangle(this.oParent.Var_aa_Screen0_Rectangle, 35, 192, 250, 7, 8);

					if (gameData.Players[0].Score > 0)
					{
						this.oParent.DrawTools.FillRectangle(this.oParent.Var_aa_Screen0_Rectangle, 35, 192,
							this.oParent.CommonTools.CheckValueRange(gameData.Players[0].Score / 4, 0, 250), 7, 11);
					}

					this.oParent.Var_aa_Screen0_Rectangle.ScreenID = 0;
					this.oParent.Graphics.F0_VGA_07d8_DrawImage(this.oParent.Var_19d4_Screen1_Rectangle, 0, 0, 320, 200, this.oParent.Var_aa_Screen0_Rectangle, 0, 0);
					this.oParent.Actions.F0_2459_0918_WaitForKeyPressOrMouseClick();
					this.oParent.GameEngine.F0_1238_1b44_RedrawMapWithBlankingEffect();
				}

				return gameData.Players[0].Score;
			}
		}

		/// <summary>
		/// Draws the next citizen icon, in rows across the screen
		/// </summary>
		/// <param name="icon"></param>
		private void DrawCitizenIcon(int icon)
		{
			this.citizenIconX += 8;

			if (this.citizenIconX > 292)
			{
				this.citizenIconY += this.rowSpacing;
				this.citizenIconX = ((this.citizenIconY >> 1) & 4) + 16;
			}

			if (this.citizenIconY <= 160)
			{
				this.oParent.Graphics.F0_VGA_0c3e_DrawBitmapToScreen(this.oParent.Var_aa_Screen0_Rectangle, this.citizenIconX, this.citizenIconY,
					this.oParent.Array_6e96[icon]);
			}
		}
	}
}
