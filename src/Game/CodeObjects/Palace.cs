using Avalonia.Media;
using IRB.VirtualCPU;
using OpenCivOne.Graphics;

namespace OpenCivOne
{
	public class Palace
	{
		private OpenCivOneGame parent;

		private GRectangle[,,] bitmapRectangles = {
			// Level 0
			{ { new(1, 66, 50, 34), new(53, 69, 17, 31), new(85, 69, 17, 31), new(), new() }, 
				{ new(), new(), new(), new(), new() }, 
				{ new(), new(), new(), new(), new() } },
			// Level 1
			{ { new(0, 57, 52, 43), new(53, 64, 24, 36), new(78, 64, 24, 36), new(113, 47, 16, 53), new(134, 47, 16, 53) },
				{ new(160, 56, 52, 44), new(213, 66, 24, 34), new(238, 66, 24, 34), new(164, 161, 31, 39), new(196, 161, 31, 39) },
				{ new(2, 151, 48, 49), new(53, 166, 24, 34), new(78, 166, 24, 34), new(113, 149, 18, 51), new(132, 149, 18, 51) } },
			// Level 2
			{ { new(0, 36, 52, 64), new(53, 59, 24, 41), new(78, 59, 24, 41), new(109, 47, 20, 53), new(134, 47, 20, 53) },
				{ new(160, 51, 52, 49), new(213, 61, 24, 39), new(238, 61, 24, 39), new(163, 155, 32, 45), new(196, 155, 32, 45) },
				{ new(2, 126, 48, 74), new(53, 153, 24, 47), new(78, 153, 24, 47), new(113, 131, 18, 69), new(132, 131, 18, 69) } },
			// Level 3
			{ { new(0, 34, 52, 66), new(53, 52, 24, 48), new(78, 52, 24, 48), new(105, 26, 24, 74), new(134, 26, 24, 74) }, 
				{ new(160, 40, 52, 60), new(213, 58, 24, 42), new(238, 58, 24, 42), new(161, 151, 34, 49), new(196, 151, 34, 49) }, 
				{ new(2, 115, 48, 85), new(53, 139, 24, 61), new(78, 139, 24, 61), new(113, 119, 18, 81), new(132, 119, 18, 81) } },
			// Level 4
			{ { new(0, 18, 52, 82), new(53, 41, 24, 59), new(78, 41, 24, 59), new(105, 22, 24, 78), new(134, 22, 24, 78) }, 
				{ new(160, 32, 52, 68), new(213, 55, 24, 45), new(238, 55, 24, 45), new(160, 150, 35, 50), new(196, 150, 35, 50) }, 
				{ new(2, 105, 48, 95), new(53, 139, 24, 61), new(78, 139, 24, 61), new(113, 111, 18, 89), new(132, 111, 18, 89) } }};

		private GRectangle[,,] bitmapOffsets = {
			// Level 0
			{ { new(), new(), new(), new(), new() },
				{ new(), new(), new(), new(), new() },
				{ new(), new(), new(), new(), new() } },
			// Level 1
			{ { new(2, 0, -4, 0), new(), new(), new(0, 1, 0, 0), new(0, 1, 0, 0) },
				{ new(4, 0, -8, 0), new(), new(), new(0, 0, -2, 0), new(-2, 0, 0, 0) },
				{ new(), new(), new(), new(0, 1, -2, 0), new(-2, 1, 0, 0) } },
			// Level 2
			{ { new(2, 0, -4, 0), new(), new(), new(0, 1, 0, 0), new(0, 1, 0, 0) },
				{ new(2, 0, -4, 0), new(), new(), new(0, 0, -2, 0), new(-2, 0, 0, 0) },
				{ new(), new(), new(), new(0, 1, -2, 0), new(-2, 1, 0, 0) } },
			// Level 3
			{ { new(2, 0, -4, 0), new(0, 3, 0, 0), new(0, 3, 0, 0), new(0, 1, 0, 0), new(0, 1, 0, 0) },
				{ new(2, 0, -4, 0), new(), new(), new(0, 0, -3, 0), new(-3, 0, 0, 0) },
				{ new(), new(), new(), new(0, 1, -2, 0), new(-2, 1, 0, 0) }, },
			// Level 4
			{ { new(2, 0, -4, 0), new(0, 3, 0, 0), new(0, 3, 0, 0), new(0, 1, 0, 0), new(0, 1, 0, 0) },
				{ new(2, 0, -4, 0), new(), new(), new(0, 0, -3, 0), new(-4, 0, 0, 0) },
				{ new(), new(), new(), new(0, 1, -2, 0), new(-2, 1, 0, 0) } }};

		private int[,,] bitmaps = new int[5, 3, 5];

		public Palace(OpenCivOneGame parent)
		{
			this.parent = parent;

			// Bitmaps aren't loaded
			for (int i = 0; i < 5; i++)
			{
				for (int j = 0; j < 3; j++)
				{
					for (int k = 0; k < 5; k++)
					{
						bitmaps[i, j, k] = -1;
					}
				}
			}
		}

		/// <summary>
		/// Draw and upgrade chosen castle segment
		/// </summary>
		public void F17_0000_0000_UpgradePalace(int playerID)
		{
			//this.oCPU.Log.EnterBlock("F17_0000_0000()");

			// function body
			Player player = this.parent.GameData.Players[playerID];

			F17_0000_07ec_DrawPalace(player, false);

			// Instruction address 0x0000:0x006d, size: 5
			this.parent.DrawTools.FillRectangle(this.parent.Var_aa_Screen0_Rectangle, 0, 0, 320, 200, 0);

			// Instruction address 0x0000:0x0075, size: 5
			this.parent.Tools.StopPaletteCycleSlots();

			// Instruction address 0x0000:0x0091, size: 5
			this.parent.CommonTools.PlayTune(this.parent.GameData.Nations[player.NationalityID].LongTune, 3);

			// Instruction address 0x0000:0x009d, size: 5
			this.parent.ImageTools.F0_2fa1_01a2_LoadBitmapOrPalette(-1, 0, 0, "CBACK.PIC", 1);

			// Instruction address 0x0000:0x00bd, size: 5
			this.parent.Graphics.F0_VGA_07d8_DrawImage(this.parent.Var_19d4_Screen1_Rectangle, 0, 0, 320, 200, this.parent.Var_aa_Screen0_Rectangle, 0, 0);

			// Instruction address 0x0000:0x00d2, size: 5
			this.parent.MenuBoxDialog.F0_2d05_0031_ShowMenuBox(this.parent.LanguageTools.F0_2f4d_044f_GetTextFromKingSection("*PALACE"), 20, 16, true, false, true);

			// Instruction address 0x0000:0x00fa, size: 5
			this.parent.Graphics.F0_VGA_07d8_DrawImage(this.parent.Var_19d4_Screen1_Rectangle, 0, 0, 320, 200, this.parent.Var_aa_Screen0_Rectangle, 0, 0);

			// Instruction address 0x0000:0x011a, size: 5
			//this.oParent.Graphics.F0_VGA_07d8_DrawImage(this.oParent.Var_aa_Screen0_Rectangle, 0, 0, 320, 200, this.oParent.Var_19d4_Screen1_Rectangle, 0, 0);

			this.parent.Var_db38 = 1;

			// Instruction address 0x0000:0x0134, size: 5
			this.parent.MenuBoxDialog.F0_2d05_0031_ShowMenuBox("Which section shall we improve?", 50, 16, true, false, true);

			bool canUpgradePalace = false;

			for (int i = 1; i < 8; i++)
			{
				if (player.PalaceLevels[i] < 4 && (player.PalaceLevels[i - 1] > 0 || player.PalaceLevels[i] >= 0 || player.PalaceLevels[i + 1] > 0))
				{
					// Instruction address 0x0000:0x01ae, size: 5
					this.parent.DrawTools.DrawStringWithShadow($"{i}", i * 48 - 36, 144, 14);

					canUpgradePalace = true;
				}
			}

			for (int i = 0; i < 3; i++)
			{
				if (player.PalaceLevels[9 + i] < 3)
				{
					// Instruction address 0x0000:0x0209, size: 5
					this.parent.DrawTools.DrawStringWithShadow($"{(char)('A' + i)}", i * 120 + 40, 160, 14);

					canUpgradePalace = true;
				}
			}

			if (canUpgradePalace)
			{
				int selectedPalaceSection;

				while (true)
				{
					MouseEvent mouseEvent = this.parent.GetMouseEvent();

					if (mouseEvent.Buttons == MouseButtonsEnum.None && this.parent.CAPI.kbhit() == 0) continue;

					if (this.parent.CAPI.kbhit() != 0)
					{
						// Instruction address 0x0000:0x02a0, size: 5
						selectedPalaceSection = this.parent.CommonTools.GetNavigationKey();

						#region For debugging, clear and redraw palace
						/*if (selectedPalaceSection == 'c' || selectedPalaceSection == 'C')
						{
							player.PalaceBuildCount = 0;

							for (int i = 0; i < 12; i++)
							{
								player.PalaceLevels[i] = (short)(((i > 2 && i < 6) || i > 8) ? 0 : -1);
								player.PalaceTypes[i] = 0;
							}

							F17_0000_07ec_DrawPalace(player, false);
							
							this.parent.Graphics.F0_VGA_07d8_DrawImage(this.parent.Var_19d4_Screen1_Rectangle, 0, 0, 320, 200, this.parent.Var_aa_Screen0_Rectangle, 0, 0);

							for (int i = 1; i < 8; i++)
							{
								if (player.PalaceLevels[i] < 4 && (player.PalaceLevels[i - 1] > 0 || player.PalaceLevels[i] >= 0 || player.PalaceLevels[i + 1] > 0))
								{
									// Instruction address 0x0000:0x01ae, size: 5
									this.parent.DrawTools.DrawStringWithShadow($"{i}", i * 48 - 36, 144, 14);
								}
							}

							for (int i = 0; i < 3; i++)
							{
								if (player.PalaceLevels[9 + i] < 3)
								{
									// Instruction address 0x0000:0x0209, size: 5
									this.parent.DrawTools.DrawStringWithShadow($"{(char)('A' + i)}", i * 120 + 40, 160, 14);
								}
							}

							continue;
						}

						if (selectedPalaceSection == 'd' || selectedPalaceSection == 'D')
						{
							F17_0000_07ec_DrawPalace(player, false);

							this.parent.Graphics.F0_VGA_07d8_DrawImage(this.parent.Var_19d4_Screen1_Rectangle, 0, 0, 320, 200, this.parent.Var_aa_Screen0_Rectangle, 0, 0);

							for (int i = 1; i < 8; i++)
							{
								if (player.PalaceLevels[i] < 4 && (player.PalaceLevels[i - 1] > 0 || player.PalaceLevels[i] >= 0 || player.PalaceLevels[i + 1] > 0))
								{
									// Instruction address 0x0000:0x01ae, size: 5
									this.parent.DrawTools.DrawStringWithShadow($"{i}", i * 48 - 36, 144, 14);
								}
							}

							for (int i = 0; i < 3; i++)
							{
								if (player.PalaceLevels[9 + i] < 3)
								{
									// Instruction address 0x0000:0x0209, size: 5
									this.parent.DrawTools.DrawStringWithShadow($"{(char)('A' + i)}", i * 120 + 40, 160, 14);
								}
							}

							continue;
						}*/
						#endregion

						if (selectedPalaceSection < '8')
						{
							selectedPalaceSection -= '0';
						}
						else
						{
							selectedPalaceSection -= 'X';
						}

						if (selectedPalaceSection < 1 || selectedPalaceSection > 11) continue;
					}
					else
					{
						if (mouseEvent.Position.Y >= 154)
						{
							selectedPalaceSection = mouseEvent.Position.X / 107 + 9;
						}
						else
						{
							selectedPalaceSection = (mouseEvent.Position.X + 12) / 48 + 1;
						}
					}

					if (player.PalaceLevels[selectedPalaceSection] > 3 || (selectedPalaceSection > 8 && player.PalaceLevels[selectedPalaceSection] > 2))
						continue;

					break;
				}

				if (selectedPalaceSection < 9)
				{
					for (; selectedPalaceSection < 4; selectedPalaceSection++)
					{
						if (player.PalaceLevels[selectedPalaceSection + 1] > 0 || player.PalaceLevels[selectedPalaceSection] != -1)
						{
							break;
						}
					}

					for (; selectedPalaceSection > 4; selectedPalaceSection--)
					{
						if (player.PalaceLevels[selectedPalaceSection - 1] > 0 || player.PalaceLevels[selectedPalaceSection] != -1)
						{
							break;
						}
					}
				}

				// Instruction address 0x0000:0x032b, size: 5
				this.parent.Graphics.F0_VGA_07d8_DrawImage(this.parent.Var_19d4_Screen1_Rectangle, 0, 0, 320, 200, this.parent.Var_aa_Screen0_Rectangle, 0, 0);

				// Instruction address 0x0000:0x0347, size: 5
				int newPalaceLevel = this.parent.Tools.CheckValueRange(player.PalaceLevels[selectedPalaceSection] + 1, 1, 4);
				int newPalaceSectionType = -1;

				if (selectedPalaceSection < 9)
				{
					// Determine first and last palace piece
					int minimumIndex = 0;
					int maximumIndex = 8;

					while (player.PalaceLevels[minimumIndex] == -1)
					{
						minimumIndex++;
					}

					while (player.PalaceLevels[maximumIndex] == -1)
					{
						maximumIndex--;
					}

					// Readjust minimum and maximum based on a selectedPalaceSection
					if (selectedPalaceSection < minimumIndex)
					{
						minimumIndex = selectedPalaceSection;
					}

					if (selectedPalaceSection > maximumIndex)
					{
						maximumIndex = selectedPalaceSection;
					}

					LoadPalaceBitmaps();

					// Instruction address 0x0000:0x03fd, size: 5
					this.parent.DrawTools.FillRectangle(this.parent.Var_aa_Screen0_Rectangle, 16, 2, 288, 112, 9);

					// Instruction address 0x0000:0x0415, size: 5
					this.parent.DrawTools.DrawCenteredStringToScreen0("Select section type", 160, 3, 15);

					int yMedian = 110;

					// Draw three distinct styles to upgrade palace section to
					for (int i = 0; i < 3; i++)
					{
						int xPosition = 20 + i * 100;

						// Instruction address 0x0000:0x043e, size: 5
						this.parent.DrawTools.FillRectangle(this.parent.Var_aa_Screen0_Rectangle, xPosition, 10, 80, 102, 1);

						xPosition += 40;

						if (selectedPalaceSection < 4)
						{
							if (selectedPalaceSection == minimumIndex)
							{
								if (minimumIndex > 1)
								{
									// grab tower piece
									int bitmapID = this.bitmaps[newPalaceLevel, i, 3];
									GBitmap bitmap = this.parent.Graphics.Bitmaps.GetValueByKey(bitmapID);
									GRectangle rect = bitmapOffsets[newPalaceLevel, i, 3];

									// grab inner piece
									int bitmapID1 = this.bitmaps[newPalaceLevel, i, 1];
									GBitmap bitmap1 = this.parent.Graphics.Bitmaps.GetValueByKey(bitmapID1);
									GRectangle rect1 = bitmapOffsets[newPalaceLevel, i, 1];

									xPosition -= ((bitmap.Width + rect.Right) + (bitmap1.Width + rect1.Left)) / 2;

									this.parent.Graphics.F0_VGA_0c3e_DrawBitmapToScreen(this.parent.Var_aa_Screen0_Rectangle, 
										xPosition, yMedian - bitmap.Height + rect.Bottom, bitmapID);

									xPosition += bitmap.Width + rect.Right + rect1.Left;

									this.parent.Graphics.F0_VGA_0c3e_DrawBitmapToScreen(this.parent.Var_aa_Screen0_Rectangle,
										xPosition, yMedian - bitmap1.Height + rect1.Bottom, bitmapID1);
								}
								else
								{
									// grab tower piece
									int bitmapID = this.bitmaps[newPalaceLevel, i, 3];
									GBitmap bitmap = this.parent.Graphics.Bitmaps.GetValueByKey(bitmapID);
									GRectangle rect = bitmapOffsets[newPalaceLevel, i, 3];

									this.parent.Graphics.F0_VGA_0c3e_DrawBitmapToScreen(this.parent.Var_aa_Screen0_Rectangle,
										xPosition - bitmap.Width / 2,
										yMedian - bitmap.Height + rect.Bottom, bitmapID);
								}
							}
							else
							{
								// We can have shadow here, but we don't have to worry about that in selection menu
								if (minimumIndex > 1)
								{
									// grab inner piece
									int bitmapID = this.bitmaps[newPalaceLevel, i, 2];
									GBitmap bitmap = this.parent.Graphics.Bitmaps.GetValueByKey(bitmapID);
									GRectangle rect = bitmapOffsets[newPalaceLevel, i, 2];

									this.parent.Graphics.F0_VGA_0c3e_DrawBitmapToScreen(this.parent.Var_aa_Screen0_Rectangle,
										xPosition - bitmap.Width / 2,
										yMedian - bitmap.Height + rect.Bottom, bitmapID);
								}
								else
								{
									// grab inner piece
									int bitmapID = this.bitmaps[newPalaceLevel, i, 2];
									GBitmap bitmap = this.parent.Graphics.Bitmaps.GetValueByKey(bitmapID);
									GRectangle rect = bitmapOffsets[newPalaceLevel, i, 2];

									xPosition -= bitmap.Width + rect.Width;

									this.parent.Graphics.F0_VGA_0c3e_DrawBitmapToScreen(this.parent.Var_aa_Screen0_Rectangle,
										xPosition,
										yMedian - bitmap.Height + rect.Bottom, bitmapID);

									xPosition += bitmap.Width + rect.Right + rect.Left;

									this.parent.Graphics.F0_VGA_0c3e_DrawBitmapToScreen(this.parent.Var_aa_Screen0_Rectangle,
										xPosition,
										yMedian - bitmap.Height + rect.Bottom, bitmapID);
								}
							}
						}
						else if (selectedPalaceSection > 4)
						{
							if (selectedPalaceSection == maximumIndex)
							{
								if (maximumIndex < 7)
								{
									// grab inner piece
									int bitmapID = this.bitmaps[newPalaceLevel, i, 2];
									GBitmap bitmap = this.parent.Graphics.Bitmaps.GetValueByKey(bitmapID);
									GRectangle rect = bitmapOffsets[newPalaceLevel, i, 2];

									// grab tower piece
									int bitmapID1 = this.bitmaps[newPalaceLevel, i, 4];
									GBitmap bitmap1 = this.parent.Graphics.Bitmaps.GetValueByKey(bitmapID1);
									GRectangle rect1 = bitmapOffsets[newPalaceLevel, i, 4];

									xPosition -= ((bitmap.Width + rect.Right) + (bitmap1.Width + rect1.Left)) / 2;

									this.parent.Graphics.F0_VGA_0c3e_DrawBitmapToScreen(this.parent.Var_aa_Screen0_Rectangle,
										xPosition, yMedian - bitmap.Height + rect.Bottom, bitmapID);

									xPosition += bitmap.Width + rect.Right + rect1.Left;

									this.parent.Graphics.F0_VGA_0c3e_DrawBitmapToScreen(this.parent.Var_aa_Screen0_Rectangle,
										xPosition, yMedian - bitmap1.Height + rect1.Bottom, bitmapID1);
								}
								else
								{
									// grab tower piece
									int bitmapID = this.bitmaps[newPalaceLevel, i, 4];
									GBitmap bitmap = this.parent.Graphics.Bitmaps.GetValueByKey(bitmapID);
									GRectangle rect = bitmapOffsets[newPalaceLevel, i, 4];

									this.parent.Graphics.F0_VGA_0c3e_DrawBitmapToScreen(this.parent.Var_aa_Screen0_Rectangle,
										xPosition - bitmap.Width / 2,
										yMedian - bitmap.Height + rect.Bottom, bitmapID);
								}
							}
							else
							{
								// We can have shadow here, but we don't have to worry about that in selection menu
								if (maximumIndex < 7)
								{
									// grab inner piece
									int bitmapID = this.bitmaps[newPalaceLevel, i, 2];
									GBitmap bitmap = this.parent.Graphics.Bitmaps.GetValueByKey(bitmapID);
									GRectangle rect = bitmapOffsets[newPalaceLevel, i, 2];

									this.parent.Graphics.F0_VGA_0c3e_DrawBitmapToScreen(this.parent.Var_aa_Screen0_Rectangle,
										xPosition - bitmap.Width / 2,
										yMedian - bitmap.Height + rect.Bottom, bitmapID);
								}
								else
								{
									// grab inner piece
									int bitmapID = this.bitmaps[newPalaceLevel, i, 2];
									GBitmap bitmap = this.parent.Graphics.Bitmaps.GetValueByKey(bitmapID);
									GRectangle rect = bitmapOffsets[newPalaceLevel, i, 2];

									xPosition -= bitmap.Width + rect.Width;

									this.parent.Graphics.F0_VGA_0c3e_DrawBitmapToScreen(this.parent.Var_aa_Screen0_Rectangle,
										xPosition,
										yMedian - bitmap.Height + rect.Bottom, bitmapID);

									xPosition += bitmap.Width + rect.Right + rect.Left;

									this.parent.Graphics.F0_VGA_0c3e_DrawBitmapToScreen(this.parent.Var_aa_Screen0_Rectangle,
										xPosition,
										yMedian - bitmap.Height + rect.Bottom, bitmapID);
								}
							}
						}
						else
						{
							int centerBitmapID = this.bitmaps[newPalaceLevel, i, 0];
							GBitmap centerBitmap = this.parent.Graphics.Bitmaps.GetValueByKey(centerBitmapID);
							GRectangle centerRect = bitmapOffsets[newPalaceLevel, i, 0];

							this.parent.Graphics.F0_VGA_0c3e_DrawBitmapToScreen(this.parent.Var_aa_Screen0_Rectangle, 
								xPosition - ((centerBitmap.Width + centerRect.Width) / 2 + centerRect.Left),
								yMedian - centerBitmap.Height + centerRect.Bottom, centerBitmapID);
						}

						// Instruction address 0x0000:0x057b, size: 5
						this.parent.DrawTools.DrawStringWithShadow($"{i + 1}", 56 + i * 100, 12, 15);
					}

					// Instruction address 0x0000:0x06be, size: 5
					this.parent.CommonTools.ClearKeyboardAndMouseEvents();

					while (true)
					{
						MouseEvent mouseEvent = this.parent.GetMouseEvent();

						if (mouseEvent.Buttons == MouseButtonsEnum.None && this.parent.CAPI.kbhit() == 0) continue;

						if (this.parent.CAPI.kbhit() != 0)
						{
							// Instruction address 0x0000:0x06f5, size: 5
							newPalaceSectionType = this.parent.CommonTools.GetNavigationKey() - '1';

							if (newPalaceSectionType < 0 || newPalaceSectionType > 2) continue;
						}
						else
						{
							newPalaceSectionType = mouseEvent.Position.X / 107;
						}

						break;
					}

					FreePalaceBitmaps();

					F17_0000_07ec_DrawPalace(player, false);

					// Instruction address 0x0000:0x073f, size: 5
					this.parent.Graphics.F0_VGA_07d8_DrawImage(this.parent.Var_19d4_Screen1_Rectangle, 0, 0, 320, 200, this.parent.Var_aa_Screen0_Rectangle, 0, 0);
				}

				player.PalaceLevels[selectedPalaceSection] = (short)newPalaceLevel;
				player.PalaceTypes[selectedPalaceSection] = (short)newPalaceSectionType;

				F17_0000_07ec_DrawPalace(player, false);

				// Instruction address 0x0000:0x0788, size: 5
				this.parent.CommonTools.ClearKeyboardAndMouseEvents();

				// Instruction address 0x0000:0x0780, size: 5
				this.parent.Graphics.F0_VGA_06b7_DrawScreenToScreen0WithEffect(1);

				// Instruction address 0x0000:0x078d, size: 5
				this.parent.Segment_2459.F0_2459_0918_WaitForKeyPressOrMouseClick();
			}

			// Instruction address 0x0000:0x07a1, size: 5
			this.parent.CommonTools.PlayTune(1, 0);

			// Instruction address 0x0000:0x07dc, size: 5
			this.parent.Segment_1238.F0_1238_1b44();
		}

		/// <summary>
		/// Draw castle
		/// </summary>
		/// <param name="flag"></param>
		public void F17_0000_07ec_DrawPalace(int playerID, bool flag)
		{
			F17_0000_07ec_DrawPalace(this.parent.GameData.Players[playerID], flag);
		}

		/// <summary>
		/// Draw castle
		/// </summary>
		/// <param name="flag"></param>
		public void F17_0000_07ec_DrawPalace(Player player, bool flag)
		{
			//this.oCPU.Log.EnterBlock($"F17_0000_07ec({flag})");

			// function body
			this.parent.Var_aa_Screen0_Rectangle.ScreenID = 1;

			if (flag)
			{
				// Instruction address 0x0000:0x089f, size: 5
				this.parent.ImageTools.F0_2fa1_01a2_LoadBitmapOrPalette(1, 0, 0, "SAD.PIC", 0);
			}
			else
			{
				// Instruction address 0x0000:0x089f, size: 5
				this.parent.ImageTools.F0_2fa1_01a2_LoadBitmapOrPalette(1, 0, 0, "CBACK.PIC", 0);
			}

			// Draw earth, gravel, stone or marble path
			if (player.PalaceLevels[10] > 0)
			{
				// Instruction address 0x0000:0x08d8, size: 5
				this.parent.ImageTools.F0_2fa1_01a2_LoadBitmapOrPalette(1, 0, 135, $"CBACKS{(char)('0' + player.PalaceLevels[10])}.PIC", 0);
			}

			int xPosition;

			LoadPalaceBitmaps();

			// Determine first and last palace piece
			int minimumIndex = 0;
			int maximumIndex = 8;

			while (player.PalaceLevels[minimumIndex] == -1)
			{
				minimumIndex++;
			}

			while (player.PalaceLevels[maximumIndex] == -1)
			{
				maximumIndex--;
			}

			int centerBitmapID = this.bitmaps[player.PalaceLevels[4], player.PalaceTypes[4], 0];
			GBitmap centerBitmap = this.parent.Graphics.Bitmaps.GetValueByKey(centerBitmapID);
			GRectangle centerRect = bitmapOffsets[player.PalaceLevels[4], player.PalaceTypes[4], 0];

			int xMedian = 160;
			int yMedian = 135; // 136;

			// Left section, deduce center piece center width
			xPosition = xMedian - (centerBitmap.Width + centerRect.Width) / 2;

			// Draw Left section
			bool leftTowerPerspective = false;

			for (int i = 3; i > 0; i--)
			{
				if (i >= minimumIndex)
				{
					bool leftmostPiece = i == minimumIndex;
					bool shadow = (leftmostPiece || (player.PalaceLevels[i - 1] > player.PalaceLevels[i])) && (minimumIndex != 3 || player.PalaceLevels[i] > 0);

					// special case for latest element where only tower is upgraded
					if (i != 1)
					{
						// grab inner piece
						int bitmapID = this.bitmaps[player.PalaceLevels[i], player.PalaceTypes[i], (leftmostPiece && shadow) ? 1 : 2];
						GBitmap bitmap = this.parent.Graphics.Bitmaps.GetValueByKey(bitmapID);
						GRectangle rect = bitmapOffsets[player.PalaceLevels[i], player.PalaceTypes[i], (leftmostPiece && shadow) ? 1 : 2];

						// special case for side tower
						if ((!leftTowerPerspective && minimumIndex == 2 && i == 3) || (!leftTowerPerspective && minimumIndex == 1 && i == 2))
						{
							// grab tower piece
							int bitmapID1 = this.bitmaps[player.PalaceLevels[minimumIndex], player.PalaceTypes[minimumIndex], 3];
							GBitmap bitmap1 = this.parent.Graphics.Bitmaps.GetValueByKey(bitmapID1);
							GRectangle rect1 = bitmapOffsets[player.PalaceLevels[minimumIndex], player.PalaceTypes[minimumIndex], 3];

							int xLocation1 = xPosition - (bitmap.Width - rect.Width) - bitmap1.Width / 2;

							// with y offset of -2 we want to make a distinction and an appearance of perspective
							this.parent.Graphics.F0_VGA_0c3e_DrawBitmapToScreen(this.parent.Var_aa_Screen0_Rectangle, xLocation1,
								yMedian - bitmap1.Height - 2, bitmapID1);

							leftTowerPerspective = true;
						}

						this.parent.Graphics.F0_VGA_0c3e_DrawBitmapToScreen(this.parent.Var_aa_Screen0_Rectangle, xPosition - bitmap.Width - rect.Right,
							yMedian - bitmap.Height + rect.Bottom, bitmapID);

						xPosition -= bitmap.Width - rect.Width;
					}

					if (i != minimumIndex)
					{
						if (minimumIndex == 1)
						{
							// we are not the latest element
							// grab inner piece
							int bitmapID = this.bitmaps[player.PalaceLevels[i], player.PalaceTypes[i], (shadow || i == 2) ? 1 : 2];
							GBitmap bitmap = this.parent.Graphics.Bitmaps.GetValueByKey(bitmapID);
							GRectangle rect = bitmapOffsets[player.PalaceLevels[i], player.PalaceTypes[i], (shadow || i == 2) ? 1 : 2];

							this.parent.Graphics.F0_VGA_0c3e_DrawBitmapToScreen(this.parent.Var_aa_Screen0_Rectangle, xPosition - bitmap.Width - rect.Right,
								yMedian - bitmap.Height + rect.Bottom, bitmapID);

							xPosition -= bitmap.Width - rect.Width;
						}
					}
					else
					{
						// grab tower piece
						int bitmapID = this.bitmaps[player.PalaceLevels[i], player.PalaceTypes[i], 3];
						GBitmap bitmap = this.parent.Graphics.Bitmaps.GetValueByKey(bitmapID);
						GRectangle rect = bitmapOffsets[player.PalaceLevels[i], player.PalaceTypes[i], 3];

						this.parent.Graphics.F0_VGA_0c3e_DrawBitmapToScreen(this.parent.Var_aa_Screen0_Rectangle, xPosition - bitmap.Width - rect.Right,
							yMedian - bitmap.Height + rect.Bottom, bitmapID);

						xPosition -= bitmap.Width - rect.Width;
					}
				}
			}

			xPosition = xMedian - (centerBitmap.Width + centerRect.Width) / 2 + centerBitmap.Width + centerRect.Width;

			// Draw Right section
			bool rightTowerPerspective = false;

			for (int i = 5; i < 8; i++)
			{
				if (i <= maximumIndex)
				{
					bool leftmostPiece = i == 5;
					bool shadow = leftmostPiece || player.PalaceLevels[i - 1] > player.PalaceLevels[i];

					// special case for latest element where only tower is upgraded
					if (i != 7)
					{
						// grab inner piece
						int bitmapID = this.bitmaps[player.PalaceLevels[i], player.PalaceTypes[i], shadow ? 1 : 2];
						GBitmap bitmap = this.parent.Graphics.Bitmaps.GetValueByKey(bitmapID);
						GRectangle rect = bitmapOffsets[player.PalaceLevels[i], player.PalaceTypes[i], shadow ? 1 : 2];

						// special case for side tower
						if ((!rightTowerPerspective && maximumIndex == 6 && i == 5) || (!rightTowerPerspective && maximumIndex == 7 && i == 6))
						{
							// grab tower piece
							int bitmapID1 = this.bitmaps[player.PalaceLevels[maximumIndex], player.PalaceTypes[maximumIndex], 4];
							GBitmap bitmap1 = this.parent.Graphics.Bitmaps.GetValueByKey(bitmapID1);
							GRectangle rect1 = bitmapOffsets[player.PalaceLevels[maximumIndex], player.PalaceTypes[maximumIndex], 4];

							int xLocation1 = xPosition + (bitmap.Width + rect.Width) - bitmap1.Width / 2;

							// with y offset of -2 we want to make a distinction and an appearance of perspective
							this.parent.Graphics.F0_VGA_0c3e_DrawBitmapToScreen(this.parent.Var_aa_Screen0_Rectangle, xLocation1,
								yMedian - bitmap1.Height - 2, bitmapID1);

							rightTowerPerspective = true;
						}

						this.parent.Graphics.F0_VGA_0c3e_DrawBitmapToScreen(this.parent.Var_aa_Screen0_Rectangle, xPosition + rect.Right,
							yMedian - bitmap.Height + rect.Bottom, bitmapID);

						xPosition += bitmap.Width + rect.Width;
					}

					if (i != maximumIndex)
					{
						// we are the inner element
						if (maximumIndex == 7)
						{
							// grab inner piece
							int bitmapID = this.bitmaps[player.PalaceLevels[i], player.PalaceTypes[i], 2];
							GBitmap bitmap = this.parent.Graphics.Bitmaps.GetValueByKey(bitmapID);
							GRectangle rect = bitmapOffsets[player.PalaceLevels[i], player.PalaceTypes[i], 2];

							this.parent.Graphics.F0_VGA_0c3e_DrawBitmapToScreen(this.parent.Var_aa_Screen0_Rectangle, xPosition + rect.Right,
								yMedian - bitmap.Height + rect.Bottom, bitmapID);

							xPosition += bitmap.Width + rect.Width;
						}
					}
					else
					{
						// grab tower piece
						int bitmapID = this.bitmaps[player.PalaceLevels[i], player.PalaceTypes[i], 4];
						GBitmap bitmap = this.parent.Graphics.Bitmaps.GetValueByKey(bitmapID);
						GRectangle rect = bitmapOffsets[player.PalaceLevels[i], player.PalaceTypes[i], 4];

						this.parent.Graphics.F0_VGA_0c3e_DrawBitmapToScreen(this.parent.Var_aa_Screen0_Rectangle, xPosition + rect.Right,
							yMedian - bitmap.Height + rect.Bottom, bitmapID);

						xPosition += bitmap.Width + rect.Width;
					}
				}
			}

			// Draw Center section
			xPosition = xMedian - (centerBitmap.Width + centerRect.Width) / 2 - centerRect.Left;

			this.parent.Graphics.F0_VGA_0c3e_DrawBitmapToScreen(this.parent.Var_aa_Screen0_Rectangle, xPosition,
				yMedian - centerBitmap.Height + centerRect.Bottom + player.PalaceLevels[4], centerBitmapID);

			FreePalaceBitmaps();

			// Draw grass, hedge, trees or statues
			if (player.PalaceLevels[9] != 0 || player.PalaceLevels[11] != 0)
			{
				if (player.PalaceLevels[9] != 0)
				{
					// Instruction address 0x0000:0x0c0b, size: 5
					int bitmapID = this.parent.ImageTools.F0_2fa1_044c_LoadIcon($"CBRUSH{(char)('.' + (player.PalaceLevels[9] * 2))}.PIC");

					// Instruction address 0x0000:0x0c2e, size: 5
					this.parent.Graphics.F0_VGA_0c3e_DrawBitmapToScreen(this.parent.Var_aa_Screen0_Rectangle,
						0, ((player.PalaceLevels[9] <= 1) ? 105 : 94), bitmapID);

					this.parent.Tools.FreeNativeResource(bitmapID, "DrawPalace 3");
				}

				if (player.PalaceLevels[11] != 0)
				{
					// Instruction address 0x0000:0x0c4b, size: 5
					int bitmapID = this.parent.ImageTools.F0_2fa1_044c_LoadIcon($"CBRUSH{(char)('/' + (player.PalaceLevels[11] * 2))}.PIC");

					// Instruction address 0x0000:0x0c6f, size: 5
					this.parent.Graphics.F0_VGA_0c3e_DrawBitmapToScreen(this.parent.Var_aa_Screen0_Rectangle,
						184, ((player.PalaceLevels[11] <= 1) ? 105 : 94), bitmapID);

					this.parent.Tools.FreeNativeResource(bitmapID, "DrawPalace 3");
				}
			}

			this.parent.Var_aa_Screen0_Rectangle.ScreenID = 0;

			if (flag)
			{
				if (this.parent.Var_deba > 7)
				{
					// Instruction address 0x0000:0x0cb9, size: 5
					this.parent.CommonTools.TransformPaletteToColor(5, Color.FromRgb(0, 0, 0));
				}

				// Instruction address 0x0000:0x0cd4, size: 5
				this.parent.DrawTools.FillRectangle(this.parent.Var_aa_Screen0_Rectangle, 0, 0, 320, 200, 0);

				// Instruction address 0x0000:0x0ce0, size: 5
				this.parent.ImageTools.F0_2fa1_01a2_LoadBitmapOrPalette(-1, 0, 0, "SAD.PIC", 1);
			}
		}

		/// <summary>
		/// Load Palace bitmaps
		/// </summary>
		private void LoadPalaceBitmaps()
		{
			//this.oCPU.Log.EnterBlock($"F17_0000_0e95({param1}, {param2})");

			// function body
			for (int i = 0; i < 5; i++)
			{
				GBitmap? palace = GBitmap.FromPICFile(this.parent.ResourcePath + CAPI.GetDOSFileName($"CASTLE{(char)('0' + i)}.PIC"), true);

				if (palace != null)
				{
					for (int j = 0; j < 3; j++)
					{
						for (int k = 0; k < 5; k++)
						{
							GRectangle rect = this.bitmapRectangles[i, j, k];

							this.bitmaps[i, j, k] = this.parent.Graphics.BitmapToBitmap(palace, rect.X, rect.Y, rect.Width, rect.Height);
						}
					}
				}
			}
		}

		/// <summary>
		/// Frees Palace bitmaps and releases memory
		/// </summary>
		private void FreePalaceBitmaps()
		{
			for (int i = 0; i < 5; i++)
			{
				for (int j = 0; j < 3; j++)
				{
					for (int k = 0; k < 5; k++)
					{
						if (this.bitmaps[i, j, k] != -1)
						{
							this.parent.Tools.FreeNativeResource(this.bitmaps[i, j, k], "FreeCastleBitmaps()");

							this.bitmaps[i, j, k] = -1;
						}
					}
				}
			}
		}
	}
}
