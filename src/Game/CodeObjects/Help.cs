using OpenCivOne.Graphics;

namespace OpenCivOne
{
	public class Help
	{
		private OpenCivOneGame parent;

		public Help(OpenCivOneGame parent)
		{
			this.parent = parent;
		}

		/// <summary>
		/// Displays help content for a topic (if found)
		/// </summary>
		/// <param name="stringPtr"></param>
		public void F4_0000_0000_DisplayHelpContent(string topic)
		{
			//this.oCPU.Log.EnterBlock($"F4_0000_0000(0x{key:x4})");

			// function body
			string content = this.parent.LanguageTools.F0_2f4d_01ad_GetTextBySectionAndKey("HELP", "*" + topic.ToUpper());

			if (!string.IsNullOrEmpty(content))
			{
				this.parent.Graphics.F0_VGA_07d8_DrawImage(this.parent.Var_aa_Screen0_Rectangle, 0, 0, 320, 200, this.parent.Var_19d4_Screen1_Rectangle, 0, 0);

				this.parent.DrawTools.FillRectangle(this.parent.Var_aa_Screen0_Rectangle, 0, 0, 320, 200, 2);

				this.parent.DrawTools.F0_1182_00b3_DrawCenteredStringToScreen0(topic, 160, 192, 0);

				this.parent.DrawTools.DrawTextBlock(32, 32, content, 256, 15);

				this.parent.Segment_2459.F0_2459_0918_WaitForKeyPressOrMouseClick();

				this.parent.Graphics.F0_VGA_07d8_DrawImage(this.parent.Var_19d4_Screen1_Rectangle, 0, 0, 320, 200, this.parent.Var_aa_Screen0_Rectangle, 0, 0);
			}
		
			this.parent.Var_2f9c_MenuBoxHelpRequested = false;
		}

		/// <summary>
		/// Shows Instant Advice Popup for currently active unit location
		/// </summary>
		/// <param name="playerID"></param>
		/// <param name="unitID"></param>
		public void F4_0000_00af_ShowInstantAdvicePopup(int playerID, int unitID)
		{
			//this.oCPU.Log.EnterBlock($"F4_0000_00af({playerID}, {unitID})");

			// function body			
			if (unitID < 128)
			{
				TerrainTypeEnum terrainType = this.parent.MapManagement.GetTerrainType(this.parent.GameData.Players[playerID].Units[unitID].Position.X,
					this.parent.GameData.Players[playerID].Units[unitID].Position.Y);

				bool showInstantAdvice = (this.parent.GameData.Terrains[(int)terrainType].Food + this.parent.GameData.Terrains[(int)terrainType].Production > 3) ? true : false;

				// Instruction address 0x0000:0x0120, size: 5
				int buildScore = this.parent.MapManagement.GetBuildLocationScore(this.parent.GameData.Players[playerID].Units[unitID].Position.X,
					this.parent.GameData.Players[playerID].Units[unitID].Position.Y);

				if (buildScore < 10)
				{
					showInstantAdvice = false;
				}

				for (int i = 1; i < 9; i++)
				{
					GPoint direction = this.parent.MoveDirections[i];

					if (this.parent.MapManagement.GetBuildLocationScore(this.parent.GameData.Players[playerID].Units[unitID].Position.X + direction.X,
						this.parent.GameData.Players[playerID].Units[unitID].Position.Y + direction.Y) > buildScore)
					{
						showInstantAdvice = false;
					}
				}

				if (showInstantAdvice)
				{
					this.parent.MapManagement.F0_2aea_0008_DrawVisibleMap(this.parent.GameData.HumanPlayerID,
						this.parent.GameData.Players[playerID].Units[unitID].Position.X - 7, this.parent.GameData.Players[playerID].Units[unitID].Position.Y - 6);

					F4_0000_02d3_ShowInstantAdvicePopup("*BUILDCITY");
				}
				else
				{
					int nearestCityID = this.parent.GameTools.F0_2dc4_0102_FindNearestCity(this.parent.GameData.Players[playerID].Units[unitID].Position.X, 
						this.parent.GameData.Players[playerID].Units[unitID].Position.Y);
					int nearestCityDistance = this.parent.GameTools.F0_2dc4_0289_GetShortestDistance(this.parent.GameData.Players[playerID].Units[unitID].Position,
						this.parent.GameData.Cities[nearestCityID].Position);

					if (nearestCityID != -1 && this.parent.GameData.Cities[nearestCityID].PlayerID == playerID && nearestCityDistance == 1)
					{
						TerrainImprovementFlagsEnum terrainImprovements = this.parent.MapManagement.F0_2aea_1585_GetVisibleTerrainImprovements(
							this.parent.GameData.Players[playerID].Units[unitID].Position.X, this.parent.GameData.Players[playerID].Units[unitID].Position.Y);

						// Instruction address 0x0000:0x0240, size: 5
						this.parent.Array_30b8[0] = this.parent.Segment_2459.F0_2459_08c6_GetCityName(nearestCityID);

						if (terrainType == TerrainTypeEnum.Hills && !terrainImprovements.HasFlag(TerrainImprovementFlagsEnum.Irrigation) && 
							!terrainImprovements.HasFlag(TerrainImprovementFlagsEnum.Mines))
						{
							F4_0000_02d3_ShowInstantAdvicePopup("*MINING");
						}

						if (terrainType == TerrainTypeEnum.Plains &&
							this.parent.MapManagement.CanIrrigateCell(this.parent.GameData.Players[playerID].Units[unitID].Position.X, this.parent.GameData.Players[playerID].Units[unitID].Position.Y) &&
							(!terrainImprovements.HasFlag(TerrainImprovementFlagsEnum.Irrigation) && !terrainImprovements.HasFlag(TerrainImprovementFlagsEnum.Mines)))
						{
							F4_0000_02d3_ShowInstantAdvicePopup("*IRRIGATE");
						}
						else if (this.parent.GameData.Terrains[(int)terrainType].MovementCost == 1 &&
								!terrainImprovements.HasFlag(TerrainImprovementFlagsEnum.Road) &&
								terrainType != TerrainTypeEnum.River &&
								this.parent.GameData.Players[playerID].ActiveUnits[0] < 2)
						{
							F4_0000_02d3_ShowInstantAdvicePopup("*ROAD");
						}
					}
				}
			}
		}

		/// <summary>
		/// Shows instant advice message
		/// </summary>
		/// <param name="topic">The topic for the message</param>
		public void F4_0000_02d3_ShowInstantAdvicePopup(string topic)
		{
			//this.oCPU.Log.EnterBlock($"F4_0000_02d3_ShowInstantAdvicePopup(0x{stringPtr:x4})");

			// function body
			if (this.parent.GameData.GameSettingFlags.InstantAdvice && this.parent.GameData.Year < 0)
			{
				string content = this.parent.LanguageTools.F0_2f4d_0471_ReplaceKeywords(this.parent.LanguageTools.F0_2f4d_01ad_GetTextBySectionAndKey("HELP", topic));

				this.parent.Graphics.F0_VGA_07d8_DrawImage(this.parent.Var_aa_Screen0_Rectangle, 0, 0, 320, 200, this.parent.Var_19d4_Screen1_Rectangle, 0, 0);

				int windowHeight = this.parent.DrawTools.GetTextBlockHeight(content, 25, 256) + 4;

				this.parent.DrawTools.FillRectangle(this.parent.Var_aa_Screen0_Rectangle, 56, 16, 208, windowHeight, 2);

				this.parent.DrawTools.DrawRectangle(56, 16, 208, windowHeight, 10);

				this.parent.Var_aa_Screen0_Rectangle.FontID = 2;

				this.parent.DrawTools.F0_1182_00b3_DrawCenteredStringToScreen0("--- Civilization Note ---", 160, 19, 0);

				this.parent.DrawTools.DrawTextBlock(64, 25, content, 256, 15);

				this.parent.Var_aa_Screen0_Rectangle.FontID = 1;

				this.parent.Segment_2459.F0_2459_0918_WaitForKeyPressOrMouseClick();

				this.parent.Graphics.F0_VGA_07d8_DrawImage(this.parent.Var_19d4_Screen1_Rectangle, 0, 0, 320, 200, this.parent.Var_aa_Screen0_Rectangle, 0, 0);
			}
		}

		/// <summary>
		/// Shows civilization note warning message
		/// </summary>
		/// <param name="topic">The topic for the warning</param>
		public void F4_0000_03aa_ShowInstantWarningPopup(string topic)
		{
			//this.oCPU.Log.EnterBlock($"F4_0000_03aa_ShowInstantWarningPopup(0x{stringPtr:x4})");

			// function body			
			if (this.parent.Var_6b90 == this.parent.GameData.HumanPlayerID)
			{
				string content = this.parent.LanguageTools.F0_2f4d_0471_ReplaceKeywords(this.parent.LanguageTools.F0_2f4d_01ad_GetTextBySectionAndKey("ERROR", topic));

				this.parent.Graphics.F0_VGA_07d8_DrawImage(this.parent.Var_aa_Screen0_Rectangle, 0, 0, 320, 200, this.parent.Var_19d4_Screen1_Rectangle, 0, 0);

				int windowHeight = this.parent.DrawTools.GetTextBlockHeight(content, 25, 256) + 4;

				this.parent.DrawTools.FillRectangle(this.parent.Var_aa_Screen0_Rectangle, 56, 16, 208, windowHeight, 4);

				this.parent.DrawTools.DrawRectangle(56, 16, 208, windowHeight, 12);

				this.parent.Var_aa_Screen0_Rectangle.FontID = 2;

				this.parent.DrawTools.F0_1182_00b3_DrawCenteredStringToScreen0("--- Civilization Note ---", 160, 19, 0);

				this.parent.DrawTools.DrawTextBlock(64, 25, content, 256, 15);

				this.parent.Var_aa_Screen0_Rectangle.FontID = 1;

				this.parent.Segment_2459.F0_2459_0918_WaitForKeyPressOrMouseClick();

				this.parent.Graphics.F0_VGA_07d8_DrawImage(this.parent.Var_19d4_Screen1_Rectangle, 0, 0, 320, 200, this.parent.Var_aa_Screen0_Rectangle, 0, 0);
			}
		}
	}
}
