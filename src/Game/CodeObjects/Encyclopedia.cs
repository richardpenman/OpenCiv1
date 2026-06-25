using System.Text;
using IRB.Collections.Generic;
using IRB.VirtualCPU;
using OpenCivOne.Graphics;

namespace OpenCivOne
{
	public class Encyclopedia
	{
		private OpenCivOneGame parent;

		private string[] Array_3c64_GameConcepts = ["Veteran Units", "Fortify", "Irrigation", "Mining", "Roads", "RailRoads", 
			"Anarchy", "Despotism", "Monarchy", "Communism", "Republic", "Democracy", "Fortress", "Pillage", "Disband", 
			"Pollution", "Sentry", "Luxuries", "Taxes", "Science", "Trade Routes"];
		private int Var_6808 = 0;

		private int[] technologyAdvanceFileID = [24, 32, 11, 65, 26, 36, 13, 59, 53, 44, 39, 5, 1, 51, 4, 35, 16, 50, 52, 38, 29, 49, 22, 0, 21, 10, 
			33, 60, 19, 37, 45, 12, 15, 43, 30, 40, 2, 41, 17, 27, 18, 3, 61, 56, 57, 6, 34, 55, 14, 62, 20, 58, 23, 8, 64, 66, 63, 31, 9, 
			48, 42, 7, 47, 54, 28, 25, 46, -1, 4, 21, 19, 14];
		private int[] unitTypeFileID = [4, 21, 19, 14, 24, 15, 13, 18, 16, 17, 8, 9, 12, 26, 20, 6, 1, 22, 25, 23, 11, 0, 10, 5, 2, 3, 27, 7];
		private int[] terrainTypeFileID = [6, 7, 8, 9, 10, 11, 0, 2, 3, 1, 4, 5];

		public Encyclopedia(OpenCivOneGame parent)
		{
			this.parent = parent;
		}

		/// <summary>
		/// Shows one of the Encyclopedia topics and handles player input
		/// </summary>
		/// <param name="topicType">Encyclopedia topic to show</param>
		public void F8_0000_0000_ShowEncyclopediaByTopic(EncyclopediaTopicEnum topicType)
		{
			//this.oCPU.Log.EnterBlock($"F8_0000_0000_ShowEncyclopedia({windowIndex})");

			// function body
			// Instruction address 0x0000:0x0006, size: 5
			this.parent.GameTools.F0_2dc4_065f_StopPaletteCycleSlots();

			int itemStartIndex = 0;
			int result = 0;

			while (result != -2)
			{
				result = F8_0000_0066_ShowEncyclopediaWindow(topicType, itemStartIndex);

				if (result == -1)
				{
					if (this.Var_6808 == 0)
					{
						itemStartIndex = 0;
					}
					else
					{
						itemStartIndex += 78;
					}
				}
			}

			// Instruction address 0x0000:0x0053, size: 5
			this.parent.Segment_1238.F0_1238_1b44();

			// Instruction address 0x0000:0x0058, size: 5
			this.parent.GameTools.F0_2dc4_0626_StartPaletteCycleSlots();
		}

		/// <summary>
		/// Show specified Encyclopedia window.
		/// </summary>
		/// <param name="topicType">window to show</param>
		/// <param name="topicStartIndex"></param>
		/// <returns></returns>
		private int F8_0000_0066_ShowEncyclopediaWindow(EncyclopediaTopicEnum topicType, int topicStartIndex)
		{
			//this.oCPU.Log.EnterBlock($"F8_0000_0066({itemStartIndex}, {windowIndex})");

			// function body
			string topicText;
			string oldTopicText = "a";
			int x;
			int y;
			int selectedTopicIndex;
			int topicCount;

			// Instruction address 0x0000:0x0082, size: 5
			this.parent.DrawTools.FillRectangle(this.parent.Var_aa_Screen0_Rectangle, 0, 0, 320, 200, 14);

			// Instruction address 0x0000:0x00b2, size: 5
			this.parent.DrawTools.FillRectangle(this.parent.Var_aa_Screen0_Rectangle, 60, 2, 200, 9, 15);

			// Instruction address 0x0000:0x00c9, size: 5
			this.parent.DrawTools.F0_1182_00b3_DrawCenteredStringToScreen0("Game Encyclopedia", 161, 4, 0);

			// Instruction address 0x0000:0x00e1, size: 5
			this.parent.DrawTools.F0_1182_00b3_DrawCenteredStringToScreen0("Game Encyclopedia", 160, 3, 10);

			// Instruction address 0x0000:0x00f9, size: 5
			this.parent.DrawTools.F0_1182_005c_DrawStringToScreen0("EXIT", 286, 4, 12);

			if (topicType == EncyclopediaTopicEnum.AllTopics)
			{
				// Instruction address 0x0000:0x0117, size: 5
				this.parent.DrawTools.F0_1182_005c_DrawStringToScreen0("MORE", 8, 4, 12);
			}

			// Instruction address 0x0000:0x0137, size: 5
			this.parent.DrawTools.FillRectangle(this.parent.Var_aa_Screen0_Rectangle, 2, 14, 316, 184, 15);

			BKeyValuePair<EncyclopediaTopicEnum, int>[] displayItems = new BKeyValuePair<EncyclopediaTopicEnum, int>[200];
			int topicNameMaximumWidth = ((topicType != EncyclopediaTopicEnum.AllTopics && topicType != EncyclopediaTopicEnum.TechnologyAdvance) ? 150 : 100);
			y = 16;
			topicCount = 0;

			for (x = 10; x < 300;)
			{
				// Instruction address 0x0000:0x0167, size: 5
				topicText = "zzz";

				int topicIndex = -1;
				EncyclopediaTopicEnum itemTopic = EncyclopediaTopicEnum.AllTopics;

				if (topicType == EncyclopediaTopicEnum.AllTopics || topicType == EncyclopediaTopicEnum.TechnologyAdvance)
				{
					for (int j = 0; j < 67; j++)
					{
						// Instruction address 0x0000:0x019b, size: 5
						if (string.Compare(this.parent.GameData.TechnologyAdvances[j].Name, oldTopicText, true) > 0 &&
							string.Compare(this.parent.GameData.TechnologyAdvances[j].Name, topicText, true) < 0)
						{
							itemTopic = EncyclopediaTopicEnum.TechnologyAdvance;
							topicIndex = j;
							topicText = this.parent.GameData.TechnologyAdvances[j].Name;
						}
					}
				}

				if (topicType == EncyclopediaTopicEnum.AllTopics || topicType == EncyclopediaTopicEnum.CityImprovement)
				{
					for (int j = 1; j < 46; j++)
					{
						if (string.Compare(this.parent.GameData.GetImprovementType(j).Name, oldTopicText, true) > 0 &&
							string.Compare(this.parent.GameData.GetImprovementType(j).Name, topicText, true) < 0)
						{
							itemTopic = EncyclopediaTopicEnum.CityImprovement;
							topicIndex = j;

							// Instruction address 0x0000:0x0228, size: 5
							topicText = this.parent.GameData.GetImprovementType(j).Name;
						}
					}
				}

				if (topicType == EncyclopediaTopicEnum.AllTopics || topicType == EncyclopediaTopicEnum.UnitType)
				{
					for (int j = 0; j < 28; j++)
					{
						if (string.Compare(this.parent.GameData.Units[j].Name, oldTopicText, true) > 0 &&
							string.Compare(this.parent.GameData.Units[j].Name, topicText, true) < 0)
						{
							itemTopic = EncyclopediaTopicEnum.UnitType;
							topicIndex = j;

							// Instruction address 0x0000:0x0288, size: 5
							topicText = this.parent.GameData.Units[j].Name;
						}
					}
				}

				if (topicType == EncyclopediaTopicEnum.AllTopics || topicType == EncyclopediaTopicEnum.TerrainType)
				{
					for (int j = 0; j < 12; j++)
					{
						// Instruction address 0x0000:0x02bb, size: 5
						if (string.Compare(this.parent.GameData.Terrains[j].Name, oldTopicText, true) > 0 &&
							string.Compare(this.parent.GameData.Terrains[j].Name, topicText, true) < 0)
						{
							itemTopic = EncyclopediaTopicEnum.TerrainType;
							topicIndex = j;

							// Instruction address 0x0000:0x02e8, size: 5
							topicText = this.parent.GameData.Terrains[j].Name;
						}
					}
				}

				if (topicType == EncyclopediaTopicEnum.AllTopics || topicType == EncyclopediaTopicEnum.GameConcept)
				{
					for (int j = 0; j < 21; j++)
					{
						if (string.Compare(Array_3c64_GameConcepts[j], oldTopicText, true) > 0 &&
							string.Compare(Array_3c64_GameConcepts[j], topicText, true) < 0)
						{
							itemTopic = EncyclopediaTopicEnum.GameConcept;
							topicIndex = j;

							// Instruction address 0x0000:0x0348, size: 5
							topicText = Array_3c64_GameConcepts[j];
						}
					}
				}

				if (itemTopic == EncyclopediaTopicEnum.AllTopics)
					break;

				if (topicCount >= topicStartIndex)
				{
					// Instruction address 0x0000:0x0396, size: 5
					this.parent.DrawTools.F0_1182_005c_DrawStringToScreen0(this.parent.LanguageTools.F0_2f4d_04f7_TrimStringToWidth(topicText, topicNameMaximumWidth), x, y, 0);

					displayItems[topicCount - topicStartIndex] = new BKeyValuePair<EncyclopediaTopicEnum, int>(itemTopic, topicIndex);

					y += 7;

					if (y > 192)
					{
						x += topicNameMaximumWidth;
						y = 16;
					}
				}

				topicCount++;
				oldTopicText = topicText;
			}

			if (x >= 300)
			{
				this.Var_6808 = 1;
			}
			else
			{
				this.Var_6808 = 0;
			}

			while (true)
			{
				MouseEvent mouseEvent = this.parent.GetMouseEvent();

				if (mouseEvent.Buttons != MouseButtonsEnum.None || this.parent.CAPI.kbhit() != 0)
				{
					x = (mouseEvent.Position.X - 10) / topicNameMaximumWidth;

					if (mouseEvent.Position.Y >= 16)
					{
						y = (mouseEvent.Position.Y - 16) / 7;
					}
					else
					{
						y = -1;
					}

					selectedTopicIndex = (26 * x) + y;

					if (mouseEvent.Buttons == MouseButtonsEnum.Right)
					{
						y = -1;
						x = 2;
					}

					if (this.parent.CAPI.kbhit() != 0)
					{
						// Instruction address 0x0000:0x0480, size: 5
						this.parent.MenuBoxDialog.F0_2d05_0ac9_GetNavigationKey();

						y = -1;
						x = 2;
					}

					if (y < 0 || topicCount - topicStartIndex > selectedTopicIndex)
					{
						if (y >= 0)
						{
							F8_0000_062a_DisplayEncyclopediaTopic(displayItems[selectedTopicIndex].Key, displayItems[selectedTopicIndex].Value);
						}
						else
						{
							if (x != 0)
							{
								selectedTopicIndex = -2;
							}
							else
							{
								selectedTopicIndex = -1;
							}
						}

						// Instruction address 0x0000:0x061c, size: 5
						this.parent.CommonTools.EmptyKeyboardBufferAndClearMouse();

						return selectedTopicIndex;
					}

					return 0;
				}
			}
		}

		/// <summary>
		/// ?
		/// </summary>
		/// <param name="topicType"></param>
		/// <param name="topicIndex"></param>
		public void F8_0000_062a_DisplayEncyclopediaTopic(EncyclopediaTopicEnum topicType, int topicIndex)
		{
			//this.oCPU.Log.EnterBlock($"F8_0000_062a({topicIndex}, {topicID})");

			// function body
			if (topicType == EncyclopediaTopicEnum.AllTopics)
				return;

			// Instruction address 0x0000:0x063a, size: 5
			this.parent.GameTools.F0_2dc4_065f_StopPaletteCycleSlots();

			int colour = 15;

			if (this.parent.Var_3c62 != 0)
			{
				colour = 255;
			}
			else
			{
				colour = 253;

				// Instruction address 0x0000:0x0696, size: 5
				this.parent.Graphics.SetPaletteColor(253, GBitmap.Color18ToColor(61, 61, 61));
			}

			// Instruction address 0x0000:0x06b1, size: 5
			this.parent.DrawTools.FillRectangle(this.parent.Var_aa_Screen0_Rectangle, 0, 0, 320, 200, (byte)colour);

			// Instruction address 0x0000:0x06b9, size: 5
			this.parent.UnitManagement.F0_1866_260e();

			this.parent.Var_aa_Screen0_Rectangle.FontID = 7;

			string topicName = "";
			
			int xPosition = 204;

			if (topicType == EncyclopediaTopicEnum.UnitType)
			{
				xPosition = 224;
			}

			if (topicType == EncyclopediaTopicEnum.GameConcept || 
				(topicType != EncyclopediaTopicEnum.CityImprovement && this.parent.Var_3c62 != 0) || 
				(topicIndex > 24 || this.parent.Var_3c62 != 0))
			{
				xPosition = 160;
			}

			switch (topicType)
			{
				case EncyclopediaTopicEnum.TechnologyAdvance:
					if (this.parent.Var_3c62 == 0)
					{
						int fileID = F8_0000_16c4_GetTechnologyAdvanceFileID(topicIndex);

						// Instruction address 0x0000:0x0760, size: 5
						this.parent.ImageTools.F0_2fa1_01a2_LoadBitmapOrPalette(1, 0, 0, $"ICONPG{(fileID / 9) + 1}.PIC", 0);

						// Instruction address 0x0000:0x076c, size: 5
						this.parent.GameTools.F0_2dc4_047d_ReadAndSetPalette("ICONPG1.PAL");

						F8_0000_16f7_DrawEncyclopediaImage(((fileID % 3) * 111) + 1, (((fileID % 9) / 3) * 69) + 1, 110, 68, 62, 42);
					}

					// Instruction address 0x0000:0x07da, size: 5
					this.parent.DrawTools.F0_1182_00b3_DrawCenteredStringToScreen0("Technology Advance", xPosition, 36, 7);

					topicName = this.parent.GameData.TechnologyAdvances[topicIndex].Name;

					break;

				case EncyclopediaTopicEnum.CityImprovement:
					if (topicIndex > 1 && topicIndex <= 21 && this.parent.Var_3c62 == 0)
					{
						// Instruction address 0x0000:0x0928, size: 5
						this.parent.ImageTools.F0_2fa1_01a2_LoadBitmapOrPalette(1, 0, 0, "CITYPIX2.PIC", 0);

						// Instruction address 0x0000:0x0934, size: 5
						this.parent.GameTools.F0_2dc4_047d_ReadAndSetPalette("HILL.PAL");

						// Instruction address 0x0000:0x0973, size: 5
						int bitmapID = this.parent.Graphics.F0_VGA_0b85_ScreenToBitmap(1, (((topicIndex - 2) >> 2) * 50) + 1, (((topicIndex - 2) & 0x3) * 50) + 1, 49, 49);

						// Instruction address 0x0000:0x0992, size: 5
						this.parent.Graphics.F0_VGA_0c3e_DrawBitmapToScreen(this.parent.Var_aa_Screen0_Rectangle, 37, 17, bitmapID);

						// Instruction address 0x0000:0x099d, size: 5
						this.parent.GameTools.F0_2dc4_0523_FreeResource(bitmapID, "Encyclopedia");
					}

					if (topicIndex > 24)
					{
						// Instruction address 0x0000:0x07da, size: 5
						this.parent.DrawTools.F0_1182_00b3_DrawCenteredStringToScreen0("Wonder of the World", xPosition, 36, 7);
					}
					else
					{
						// Instruction address 0x0000:0x07da, size: 5
						this.parent.DrawTools.F0_1182_00b3_DrawCenteredStringToScreen0("City Improvement", xPosition, 36, 7);
					}

					topicName = this.parent.GameData.GetImprovementType(topicIndex).Name;

					break;

				case EncyclopediaTopicEnum.UnitType:
					if (this.parent.Var_3c62 == 0)
					{
						int fileID = F8_0000_1790_GetUnitTypeFileID(topicIndex);

						// Instruction address 0x0000:0x0a14, size: 5
						this.parent.ImageTools.F0_2fa1_01a2_LoadBitmapOrPalette(1, 0, 0, $"ICONPG{(char)('A' + (fileID / 6))}.PIC", 0);

						// Instruction address 0x0000:0x0a20, size: 5
						this.parent.GameTools.F0_2dc4_047d_ReadAndSetPalette("ICONPGA.PAL");

						F8_0000_16f7_DrawEncyclopediaImage(((fileID & 1) * 160) + 1, ((fileID % 6) >> 1) * 61, 158, 61, 88, 42);
					}

					// Instruction address 0x0000:0x0a87, size: 5
					this.parent.DrawTools.F0_1182_00b3_DrawCenteredStringToScreen0("Unit Type", xPosition, 36, 7);

					// Instruction address 0x0000:0x0aa4, size: 5
					this.parent.Graphics.F0_VGA_0c3e_DrawBitmapToScreen(this.parent.Var_aa_Screen0_Rectangle,
						216, 48, this.parent.Array_d4ce[96 + topicIndex]);

					topicName = this.parent.GameData.Units[topicIndex].Name;

					break;

				case EncyclopediaTopicEnum.TerrainType:
					if (this.parent.Var_3c62 == 0)
					{
						byte[] palette;

						int fileID = F8_0000_17c3_GetTerrainTypeFileID(topicIndex);

						// Instruction address 0x0000:0x0ae8, size: 5
						this.parent.ImageTools.F0_2fa1_01a2_LoadBitmapOrPalette(1, 0, 0, $"ICONPGT{((fileID / 6) + 1)}.PIC", 0);

						// Instruction address 0x0000:0x0aff, size: 5
						this.parent.ImageTools.F0_2fa1_01a2_LoadBitmapOrPalette(-1, 0, 0, "SP256.PAL", out palette);

						// Instruction address 0x0000:0x0b0b, size: 5
						this.parent.Graphics.F0_VGA_0162_SetColorsFromColorStruct(palette);

						// Instruction address 0x0000:0x0b1d, size: 5
						this.parent.Graphics.SetPaletteColor(253, GBitmap.Color18ToColor(61, 61, 61));

						F8_0000_16f7_DrawEncyclopediaImage(((fileID % 3) * 107) + 1, (((fileID % 6) / 3) * 87) + 5, 106, 77, 76, 46);
					}

					// Instruction address 0x0000:0x07da, size: 5
					this.parent.DrawTools.F0_1182_00b3_DrawCenteredStringToScreen0("Terrain Type", xPosition, 36, 7);

					// Instruction address 0x0000:0x0b76, size: 5
					topicName = this.parent.GameData.Terrains[topicIndex].Name;

					break;

				case EncyclopediaTopicEnum.GameConcept:
					// Instruction address 0x0000:0x07da, size: 5
					this.parent.DrawTools.F0_1182_00b3_DrawCenteredStringToScreen0("Game Concept", xPosition, 36, 7);

					// Instruction address 0x0000:0x0b9c, size: 5
					topicName = Array_3c64_GameConcepts[topicIndex];

					break;
			}

			this.parent.Var_aa_Screen0_Rectangle.FontID = 6;

			// Instruction address 0x0000:0x0802, size: 5
			this.parent.DrawTools.F0_1182_00b3_DrawCenteredStringToScreen0(topicName, xPosition, 20, 0);

			this.parent.Var_aa_Screen0_Rectangle.FontID = 7;
			
			int yPosition = 76;

			if (this.parent.GameData.GameSettingFlags.EncyclopediaText && topicType != EncyclopediaTopicEnum.TerrainType)
			{
				string content = this.parent.LanguageTools.F0_2f4d_01ad_GetTextBySectionAndKey($"BLURB{((int)topicType) - 1}", $"*{topicName.ToUpper()}");

				// Instruction address 0x0000:0x0855, size: 5
				if (!string.IsNullOrEmpty(content))
				{
					// Instruction address 0x0000:0x0870, size: 5
					this.parent.DrawTools.DrawTextBlock(12, 76, content, 296, 1);

					// Instruction address 0x0000:0x0878, size: 5
					this.parent.Segment_2459.F0_2459_0918_WaitForKeyPressOrMouseClick();

					// Instruction address 0x0000:0x0894, size: 5
					this.parent.DrawTools.FillRectangle(this.parent.Var_aa_Screen0_Rectangle, 8, 76, 304, 116, (byte)colour);
				}
			}

			if (topicType != EncyclopediaTopicEnum.TechnologyAdvance && topicType != EncyclopediaTopicEnum.TerrainType)
			{
				// Instruction address 0x0000:0x08d6, size: 5
				yPosition = this.parent.DrawTools.DrawTextBlock(12, yPosition,
					this.parent.LanguageTools.F0_2f4d_01ad_GetTextBySectionAndKey($"BLURB{((int)topicType - 1)}", $"*{topicName.ToUpper()}2"), 288, 1);

				yPosition += 8;
			}

			switch (topicType)
			{
				case EncyclopediaTopicEnum.TechnologyAdvance:
					yPosition += 8;

					TechnologyAdvanceDefinition tech1 = this.parent.GameData.TechnologyAdvances[topicIndex];

					if (tech1.RequiresTechnologyAdvance1 != TechnologyAdvanceEnum.None)
					{
						// Instruction address 0x0000:0x0c37, size: 5
						this.parent.DrawTools.F0_1182_005c_DrawStringToScreen0(
							$"Requires {this.parent.GameData.TechnologyAdvances[(int)tech1.RequiresTechnologyAdvance1].Name}" +
								$"{((tech1.RequiresTechnologyAdvance2 != TechnologyAdvanceEnum.None) ? $" and {this.parent.GameData.TechnologyAdvances[(int)tech1.RequiresTechnologyAdvance2].Name}" : "")}",
							32, yPosition, 1);
					}

					yPosition += 16;

					// Instruction address 0x0000:0x0c52, size: 5
					this.parent.DrawTools.F0_1182_005c_DrawStringToScreen0("Allows: ", 32, yPosition, 1);

					yPosition += 8;

					int oldYPosition = yPosition;

					for (int i = 0; i < 72; i++)
					{
						tech1 = this.parent.GameData.TechnologyAdvances[i];

						if (tech1.RequiresTechnologyAdvance1 == (TechnologyAdvanceEnum)topicIndex)
						{
							// Instruction address 0x0000:0x0cd9, size: 5
							this.parent.DrawTools.F0_1182_005c_DrawStringToScreen0(
								$"{tech1.Name}" +
								$"{((tech1.RequiresTechnologyAdvance2 != TechnologyAdvanceEnum.None) ? $" (with {this.parent.GameData.TechnologyAdvances[(int)tech1.RequiresTechnologyAdvance2].Name})" : "")}",
								40, yPosition, 9);

							yPosition += 8;
						}

						if (tech1.RequiresTechnologyAdvance2 == (TechnologyAdvanceEnum)topicIndex)
						{
							// Instruction address 0x0000:0x0d55, size: 5
							this.parent.DrawTools.F0_1182_005c_DrawStringToScreen0(
								$"{tech1.Name}" +
								$"{((tech1.RequiresTechnologyAdvance1 != TechnologyAdvanceEnum.None) ? $" (with {this.parent.GameData.TechnologyAdvances[(int)tech1.RequiresTechnologyAdvance1].Name})" : "")}",
								40, yPosition, 9);

							yPosition += 8;
						}
					}

					yPosition += 4;

					for (int i = 0; i < 28; i++)
					{
						if (this.parent.GameData.Units[i].RequiredTechnology == (TechnologyAdvanceEnum)topicIndex)
						{
							// Instruction address 0x0000:0x0da3, size: 5
							this.parent.Graphics.F0_VGA_0c3e_DrawBitmapToScreen(this.parent.Var_aa_Screen0_Rectangle,
								40, yPosition - 4, this.parent.Array_d4ce[64 + i + (this.parent.GameData.HumanPlayerID * 32)]);

							// Instruction address 0x0000:0x0ddc, size: 5
							this.parent.DrawTools.F0_1182_005c_DrawStringToScreen0($"{this.parent.GameData.Units[i].Name} unit", 60, yPosition, 12);

							yPosition += 12;
						}
					}

					for (int i = 1; i < 46; i++)
					{
						if ((int)this.parent.GameData.GetImprovementType(i).RequiresTechnology == topicIndex)
						{
							// Instruction address 0x0000:0x0e4a, size: 5
							this.parent.CityWorker.F0_1d12_7045(i, 40, yPosition - 2);

							// Instruction address 0x0000:0x0e17, size: 5
							this.parent.DrawTools.F0_1182_005c_DrawStringToScreen0(
								$"{this.parent.GameData.GetImprovementType(i).Name}{((i > 24) ? " Wonder" : " improvement")}", 60, yPosition, 2);

							yPosition += 12;
						}
					}

					if (oldYPosition + 4 == yPosition)
					{
						// Instruction address 0x0000:0x0e8c, size: 5
						this.parent.DrawTools.F0_1182_005c_DrawStringToScreen0("Allows: ", 32, yPosition - 12, (byte)colour);
					}

					int technologyAcquiredFrom = this.parent.GameData.Players[this.parent.GameData.HumanPlayerID].TechnologyAcquiredFrom[topicIndex];

					switch (technologyAcquiredFrom)
					{
						case -2:
							// Instruction address 0x0000:0x0f20, size: 5
							this.parent.DrawTools.F0_1182_00b3_DrawCenteredStringToScreen0("(from Great Library)", xPosition, 48, 7);

							break;

						case -1:
							break;

						case 0:
							// Instruction address 0x0000:0x0f20, size: 5
							this.parent.DrawTools.F0_1182_00b3_DrawCenteredStringToScreen0("(Discovered)", xPosition, 48, 7);

							break;

						default:
							// Instruction address 0x0000:0x0f20, size: 5
							this.parent.DrawTools.F0_1182_00b3_DrawCenteredStringToScreen0(
								$"(Taken from {this.parent.GameData.Players[technologyAcquiredFrom].Nation})", xPosition, 48, 7);

							break;
					}
					break;

				case EncyclopediaTopicEnum.CityImprovement:
					if (this.parent.GameData.GetImprovementType(topicIndex).RequiresTechnology != TechnologyAdvanceEnum.None)
					{
						// Instruction address 0x0000:0x0f75, size: 5
						this.parent.DrawTools.F0_1182_005c_DrawStringToScreen0(
							$"Requires {this.parent.GameData.TechnologyAdvances[(int)this.parent.GameData.GetImprovementType(topicIndex).RequiresTechnology].Name}", 12, yPosition, 9);
					}
					else
					{
						this.parent.DrawTools.F0_1182_005c_DrawStringToScreen0($"Requires no Advancement", 12, yPosition, 9);

					}

					yPosition += 8;

					// Instruction address 0x0000:0x0fd5, size: 5
					this.parent.DrawTools.F0_1182_005c_DrawStringToScreen0(
						$"Cost: {10 * this.parent.GameData.GetImprovementType(topicIndex).Cost} shields.", 12, yPosition, 9);

					yPosition += 8;

					// Instruction address 0x0000:0x101e, size: 5
					this.parent.DrawTools.F0_1182_005c_DrawStringToScreen0(
						$"Maintenance: {this.parent.GameData.GetImprovementType(topicIndex).MaintenanceCost} shields.", 12, yPosition, 12);

					yPosition += 8;

					break;

				case EncyclopediaTopicEnum.UnitType:
					// Instruction address 0x0000:0x1081, size: 5
					this.parent.DrawTools.F0_1182_005c_DrawStringToScreen0(
						$"Required technology: " +
						$"{((this.parent.GameData.Units[topicIndex].RequiredTechnology == TechnologyAdvanceEnum.None) ? "None" :
						this.parent.GameData.TechnologyAdvances[(int)this.parent.GameData.Units[topicIndex].RequiredTechnology].Name)}" +
						$"{((topicIndex == 25) ? " and Nuclear Fission" : "")}",
						100, yPosition, 9);

					yPosition += 8;

					// Instruction address 0x0000:0x10e9, size: 5
					this.parent.DrawTools.F0_1182_005c_DrawStringToScreen0(
						$"Cost: {10 * this.parent.GameData.Units[topicIndex].Cost} shields.", 100, yPosition, 9);

					yPosition += 8;

					// Instruction address 0x0000:0x1135, size: 5
					this.parent.DrawTools.F0_1182_005c_DrawStringToScreen0($"Attack Strength: {this.parent.GameData.Units[topicIndex].AttackStrength}", 100, yPosition, 12);

					yPosition += 8;

					// Instruction address 0x0000:0x1181, size: 5
					this.parent.DrawTools.F0_1182_005c_DrawStringToScreen0(
						$"Defense Strength: {this.parent.GameData.Units[topicIndex].DefenseStrength}", 100, yPosition, 12);

					yPosition += 8;

					// Instruction address 0x0000:0x101e, size: 5
					this.parent.DrawTools.F0_1182_005c_DrawStringToScreen0(
						$"Moves: {this.parent.GameData.Units[topicIndex].MoveCount}", 100, yPosition, 0);

					yPosition += 8;

					break;

				case EncyclopediaTopicEnum.TerrainType:
					for (int i = topicIndex; i <= topicIndex + 12; i += 12)
					{
						if (i == topicIndex || this.parent.GameData.Terrains[topicIndex].Food != 2)
						{
							// Instruction address 0x0000:0x1206, size: 5
							this.parent.DrawTools.F0_1182_005c_DrawStringToScreen0(this.parent.GameData.Terrains[i].Name, 12, yPosition, 1);

							yPosition += 8;

							if (this.parent.GameData.Terrains[i].Food == 0 && this.parent.GameData.TerrainModifications[topicIndex].IrrigationEffect < -1)
							{
								StringBuilder description = new();

								description.Append($"Food: {this.parent.GameData.Terrains[i].Food}{((this.parent.GameData.Terrains[i].Food >= 3) ? "*" : "")}");

								if (this.parent.GameData.TerrainModifications[topicIndex].IrrigationEffect < -1)
								{
									// Instruction address 0x0000:0x1296, size: 5
									description.Append($" ({this.parent.GameData.Terrains[i].Food - this.parent.GameData.TerrainModifications[topicIndex].IrrigationEffect - 1}");
									description.Append((this.parent.GameData.Terrains[i].Food >= 2) ? "*" : "");
									description.Append(" with Irrigation)");
								}

								// Instruction address 0x0000:0x12fb, size: 5
								description.Append(" units.");

								// Instruction address 0x0000:0x1312, size: 5
								this.parent.DrawTools.F0_1182_005c_DrawStringToScreen0(description.ToString(), 16, yPosition, 9);

								yPosition += 8;
							}

							if (this.parent.GameData.Terrains[i].Production != 0 || this.parent.GameData.TerrainModifications[topicIndex].MiningEffect < -1)
							{
								StringBuilder description = new();

								description.Append($"Production: {((topicIndex == 2 || topicIndex == 11) ? "0/" : "")}{this.parent.GameData.Terrains[i].Production}");

								if (this.parent.GameData.TerrainModifications[topicIndex].MiningEffect < -1)
								{
									description.Append($" ({this.parent.GameData.Terrains[i].Production - this.parent.GameData.TerrainModifications[topicIndex].MiningEffect - 1}");

									if (this.parent.GameData.Terrains[i].Production - this.parent.GameData.TerrainModifications[topicIndex].MiningEffect - 1 >= 3)
									{
										description.Append("*");
									}

									description.Append(" with Mining)");
								}

								// Instruction address 0x0000:0x1410, size: 5
								description.Append(" units.");

								// Instruction address 0x0000:0x1427, size: 5
								this.parent.DrawTools.F0_1182_005c_DrawStringToScreen0(description.ToString(), 16, yPosition, 9);

								yPosition += 8;
							}

							if (this.parent.GameData.Terrains[i].Trade != 0 || topicIndex <= 2)
							{
								StringBuilder description = new();

								description.Append($"Trade: {this.parent.GameData.Terrains[i].Trade}{((this.parent.GameData.Terrains[i].Trade != 0) ? "%" : "")}");
								
								if (this.parent.GameData.Terrains[i].Trade >= 3)
								{
									description.Append("*");
								}

								if (topicIndex <= 2)
								{
									description.Append($" ({this.parent.GameData.Terrains[i].Trade + 1}% with Roads)");
								}

								// Instruction address 0x0000:0x151d, size: 5
								this.parent.DrawTools.F0_1182_005c_DrawStringToScreen0(description.ToString(), 16, yPosition, 9);

								yPosition += 8;
							}

							if (i == 7)
							{
								// Instruction address 0x0000:0x153e, size: 5
								this.parent.DrawTools.F0_1182_005c_DrawStringToScreen0("nothing", 16, yPosition, 9);

								yPosition += 8;
							}

							yPosition += 4;
						}
					}

					// Instruction address 0x0000:0x156f, size: 5
					this.parent.DrawTools.F0_1182_005c_DrawStringToScreen0("*  -1 if government is Despotism/Anarchy.", 16, yPosition, 9);

					yPosition += 8;

					// Instruction address 0x0000:0x158a, size: 5
					this.parent.DrawTools.F0_1182_005c_DrawStringToScreen0("%  +1 if government is Republic/Democracy.", 16, yPosition, 9);

					yPosition += 12;

					// Instruction address 0x0000:0x15ed, size: 5
					this.parent.DrawTools.F0_1182_005c_DrawStringToScreen0(
						$"Movement cost: {this.parent.GameData.Terrains[topicIndex].MovementCost} MP", 12, yPosition, 12);

					yPosition += 8;

					// Instruction address 0x0000:0x101e, size: 5
					this.parent.DrawTools.F0_1182_005c_DrawStringToScreen0(
						$"Defense bonus: +{(50 * this.parent.GameData.Terrains[topicIndex].DefenseBonus) - 100}%", 12, yPosition, 12);

					yPosition += 8;

					break;
			}

			this.parent.Var_aa_Screen0_Rectangle.FontID = 1;

			// Instruction address 0x0000:0x1663, size: 5
			this.parent.Segment_2459.F0_2459_0918_WaitForKeyPressOrMouseClick();

			this.parent.Var_3c62 = 0;

			if (this.parent.Var_3934 == -1)
			{
				// Instruction address 0x0000:0x16ac, size: 5
				this.parent.DrawTools.FillRectangle(this.parent.Var_aa_Screen0_Rectangle, 8, 8, 304, 184, (byte)colour);

				// Instruction address 0x0000:0x16b4, size: 5
				this.parent.GameTools.F0_2dc4_0626_StartPaletteCycleSlots();
			}		
		}

		/// <summary>
		/// ?
		/// </summary>
		/// <param name="topicIndex"></param>
		/// <returns></returns>
		public int F8_0000_16c4_GetTechnologyAdvanceFileID(int topicIndex)
		{
			//this.oCPU.Log.EnterBlock($"F8_0000_16c4({topicID})");

			// function body
			for (int i = 0; i < 72; i++)
			{
				if (technologyAdvanceFileID[i] == topicIndex)
				{
					return i;
				}
			}

			return -1;
		}

		/// <summary>
		/// ?
		/// </summary>
		/// <param name="topicIndex"></param>
		/// <returns></returns>
		public int F8_0000_1790_GetUnitTypeFileID(int topicIndex)
		{
			//this.oCPU.Log.EnterBlock($"F8_0000_1790({topicID})");

			// function body
			for (int i = 0; i < 28; i++)
			{
				if (unitTypeFileID[i] == topicIndex)
				{
					return i;
				}
			}

			return -1;
		}

		/// <summary>
		/// ?
		/// </summary>
		/// <param name="topicIndex"></param>
		/// <returns></returns>
		public int F8_0000_17c3_GetTerrainTypeFileID(int topicIndex)
		{
			//this.oCPU.Log.EnterBlock($"F8_0000_17c3({topicID})");

			// function body
			for (int i = 0; i < 12; i++)
			{
				if (terrainTypeFileID[i] == topicIndex)
				{
					return i;
				}
			}

			return -1;
		}

		/// <summary>
		/// Draws encyclopedia image from Screen 0
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="width"></param>
		/// <param name="height"></param>
		/// <param name="xOffset"></param>
		/// <param name="yOffset"></param>
		public void F8_0000_16f7_DrawEncyclopediaImage(int x, int y, int width, int height, int xOffset, int yOffset)
		{
			//this.oCPU.Log.EnterBlock($"F8_0000_16f7({x}, {y}, {width}, {height}, {xOffset}, {yOffset})");

			// function body
			if (x + width > 319)
			{
				width = 319 - x;
			}

			if (y + height > 199)
			{
				height = 199 - y;
			}
			
			if (xOffset < width / 2)
			{
				x += (width / 2) - xOffset;
				width = xOffset * 2;
			}
		
			// Instruction address 0x0000:0x1783, size: 5
			this.parent.Graphics.F0_VGA_07d8_DrawImage(this.parent.Var_19d4_Screen1_Rectangle,
				x, y, width, height, this.parent.Var_aa_Screen0_Rectangle, xOffset - (width / 2), yOffset - (height /2));
		}
	}
}
