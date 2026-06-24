using OpenCivOne.Graphics;
using OpenCivOne.UI;

namespace OpenCivOne
{
	public class GameTools
	{
		private OpenCivOneGame parent;

		private bool Var_2fec_PaletteCycleEnabled = false;

		public GameTools(OpenCivOneGame parent)
		{
			this.parent = parent;
		}

		/// <summary>
		/// Randomizes a default Random Number Generator
		/// </summary>
		public void F0_2dc4_0042_Randomize()
		{
			// function body
			this.parent.GameData.RandomSeed = (ushort)(this.parent.CAPI.time(0) & 0x7fff);
			this.parent.CAPI.srand(this.parent.GameData.RandomSeed);
		}

		/// <summary>
		/// Check and adjusts value to given range
		/// </summary>
		/// <param name="value"></param>
		/// <param name="minValue"></param>
		/// <param name="maxValue"></param>
		/// <returns></returns>
		/// <exception cref="Exception"></exception>
		public int F0_2dc4_007c_CheckValueRange(int value, int minValue, int maxValue)
		{
			// function body
			if (minValue > maxValue)
			{
				// do nothing, just return minimum value
				this.parent.Log.WriteLine($"CheckValueRange({value}, {minValue}, {maxValue}), Minimum value is greater than maximum value.");
				return minValue;
				//throw new Exception("Minimum value is greater than maximum value");
			}

			return Math.Min(Math.Max(value, minValue), maxValue);
		}

		/// <summary>
		/// Tests if city is present at given coordinates
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns>City ID, otherwise -1</returns>
		public int F0_2dc4_00ba_GetCityByLocation(int x, int y)
		{
			//this.oCPU.Log.EnterBlock($"F0_2dc4_00ba({x}, {y})");

			// function body
			for (int i = 0; i < 128; i++)
			{
				City city = this.parent.GameData.Cities[i];

				if (city.StatusFlag != 0xff && city.Position.X == x && city.Position.Y == y)
				{
					return i;
				}
			}

			return -1;
		}

		/// <summary>
		/// Finds a nearest city
		/// </summary>
		/// <param name="pt"></param>
		/// <returns></returns>
		public int F0_2dc4_0102_FindNearestCity(GPoint pt)
		{
			return F0_2dc4_0102_FindNearestCity(pt.X, pt.Y);
		}

		/// <summary>
		/// Finds a nearest city
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		public int F0_2dc4_0102_FindNearestCity(int x, int y)
		{
			//this.oCPU.Log.EnterBlock($"F0_2dc4_0102({xPos}, {yPos})");

			// function body
			int minDistance = 999;
			int cityID = -1;

			for (int i = 0; i < 128; i++)
			{
				City city = this.parent.GameData.Cities[i];

				if (city.StatusFlag != 0xff)
				{
					// Instruction address 0x2dc4:0x0142, size: 3
					int distance = F0_2dc4_0289_GetShortestDistance(x, y, city.Position);

					if (distance < minDistance)
					{
						minDistance = distance;
						cityID = i;
					}
				}
			}

			return cityID;
		}

		/// <summary>
		/// Find nearest unit from a given player, except a current unit
		/// </summary>
		/// <param name="playerID"></param>
		/// <param name="currentUnitID"></param>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		public int F0_2dc4_0177_FindNearestPlayerUnit(int playerID, int currentUnitID, int x, int y)
		{
			//this.oCPU.Log.EnterBlock($"F0_2dc4_0177({playerID}, {unitID}, {x}, {y})");

			// function body
			int minDistance = 999;
			int nearestUnitID = -1;

			for (int i = 0; i < 128; i++)
			{
				if (this.parent.GameData.Players[playerID].Units[i].UnitType != UnitTypeEnum.None && i != currentUnitID)
				{
					// Instruction address 0x2dc4:0x01b2, size: 3
					int distance = F0_2dc4_0289_GetShortestDistance(x, y,
						this.parent.GameData.Players[playerID].Units[i].Position.X, this.parent.GameData.Players[playerID].Units[i].Position.Y);

					if (distance < minDistance)
					{
						minDistance = distance;
						nearestUnitID = i;
					}
				}
			}

			return nearestUnitID;
		}

		/// <summary>
		/// Calculates a shortest distance between two points
		/// </summary>
		/// <param name="pt1"></param>
		/// <param name="pt2"></param>
		/// <returns></returns>
		public int F0_2dc4_0289_GetShortestDistance(GPoint pt1, GPoint pt2)
		{
			// function body
			return F0_2dc4_0289_GetShortestDistance(pt1.X - pt2.X, pt1.Y - pt2.Y);
		}

		/// <summary>
		/// Calculates a shortest distance between two points
		/// </summary>
		/// <param name="pt1"></param>
		/// <param name="x1"></param>
		/// <param name="y1"></param>
		/// <returns></returns>
		public int F0_2dc4_0289_GetShortestDistance(GPoint pt1, int x1, int y1)
		{
			// function body
			return F0_2dc4_0289_GetShortestDistance(pt1.X - x1, pt1.Y - y1);
		}

		/// <summary>
		/// Calculates a shortest distance between two points
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="pt2"></param>
		/// <returns></returns>
		public int F0_2dc4_0289_GetShortestDistance(int x, int y, GPoint pt2)
		{
			// function body
			return F0_2dc4_0289_GetShortestDistance(x - pt2.X, y - pt2.Y);
		}

		/// <summary>
		/// Calculates a shortest distance between two points
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="x1"></param>
		/// <param name="y1"></param>
		/// <returns></returns>
		public int F0_2dc4_0289_GetShortestDistance(int x, int y, int x1, int y1)
		{
			// function body
			return F0_2dc4_0289_GetShortestDistance(x - x1, y - y1);
		}

		/// <summary>
		/// Calculates a shortest distance
		/// </summary>
		/// <param name="width"></param>
		/// <param name="height"></param>
		/// <returns></returns>
		public int F0_2dc4_0289_GetShortestDistance(int width, int height)
		{
			// function body			
			width = Math.Abs(width);
			height = Math.Abs(height);

			// take into account either map direction from median
			if (width > 40)
			{
				width = 80 - width;
			}

			if (width > height)
			{
				return (height / 2) + width;
			}
			else
			{
				return (width / 2) + height;
			}
		}

		/// <summary>
		/// Get a sum of total population count for a given player
		/// </summary>
		/// <param name="playerID"></param>
		/// <returns></returns>
		public int F0_2dc4_02cd_GetPlayerTotalPopulationCount(int playerID)
		{
			//this.oCPU.Log.EnterBlock($"F0_2dc4_02cd({playerID})");

			// function body
			int population = 0;

			for (int i = 0; i < 128; i++)
			{
				if (this.parent.GameData.Cities[i].PlayerID == playerID && this.parent.GameData.Cities[i].StatusFlag != 0xff)
				{
					for (int j = 1; j <= this.parent.GameData.Cities[i].ActualSize; j++)
					{
						population += j;
					}
				}
			}

			return population;
		}

		/// <summary>
		/// Converts population value to string
		/// </summary>
		/// <param name="value"></param>
		public string F0_2dc4_0337_PopulationValueToString(int value)
		{
			// function body
			return $"{(value * 1000):###,###,##0}";
		}

		/// <summary>
		/// Reads the palette from file and applies it to screen 0
		/// </summary>
		/// <param name="filenamePtr"></param>
		public void F0_2dc4_047d_ReadAndSetPalette(string filename)
		{
			//this.oCPU.Log.EnterBlock($"F0_2dc4_047d_ReadAndSetPalette(0x{filename:x4})");

			// function body
			byte[] palette;

			// Instruction address 0x2dc4:0x0492, size: 5
			this.parent.ImageTools.F0_2fa1_01a2_LoadBitmapOrPalette(-1, 0, 0, filename, out palette);

			// 0xbdf4 - 0xbdee = 6

			for (int i = 0; i < 48; i++)
			{
				palette[6 + i] = this.parent.Var_6b34[i];
			}

			// Instruction address 0x2dc4:0x04b9, size: 5
			this.parent.Graphics.F0_VGA_0162_SetColorsFromColorStruct(palette);

			// Instruction address 0x2dc4:0x04d2, size: 3
			this.parent.Graphics.SetPaletteColor(45, GBitmap.Color18ToColor(51, 39, 25));
		}

		/// <summary>
		/// Free resource, shows Memory error dialog if an error happens
		/// </summary>
		/// <param name="bitmapID"></param>
		/// <param name="stringPtr"></param>
		public void F0_2dc4_0523_FreeResource(int bitmapID, string text)
		{
			//this.oCPU.Log.EnterBlock($"F0_2dc4_0523_FreeResource({bitmapID}, \"{text}\")");

			// function body
			// ignore unallocated bitmaps
			if (bitmapID != 0)
			{
				// Instruction address 0x2dc4:0x0529, size: 5
				if (this.parent.Graphics.Bitmaps.ContainsKey(bitmapID))
				{
					this.parent.Graphics.Bitmaps.RemoveByKey(bitmapID);
				}
				else
				{
					// Instruction address 0x2dc4:0x0570, size: 5
					if (string.IsNullOrEmpty(text))
					{
						MessageBox.Show("Unspecified memory error", "Memory error", MessageBoxIcon.Error, MessageBoxButtons.OK);
					}
					else
					{
						MessageBox.Show(text, "Memory error", MessageBoxIcon.Error, MessageBoxButtons.OK);
					}
				}
			}
		}

		/// <summary>
		/// Adds a default palette cycle slots
		/// </summary>
		public void F0_2dc4_05dd_AddPaletteCycleSlots()
		{
			//this.oCPU.Log.EnterBlock("F0_2dc4_05dd()");

			// function body
			// Instruction address 0x2dc4:0x05ed, size: 5
			this.parent.CommonTools.AddPaletteCycleSlot(1, 15, 96, 103);

			// Instruction address 0x2dc4:0x0605, size: 5
			this.parent.CommonTools.AddPaletteCycleSlot(2, 15, 104, 111);

			// Instruction address 0x2dc4:0x061d, size: 5
			this.parent.CommonTools.AddPaletteCycleSlot(3, 15, 112, 127);
		}

		/// <summary>
		/// Starts default palette cycle slots
		/// </summary>
		public void F0_2dc4_0626_StartPaletteCycleSlots()
		{
			// function body
			if (!this.Var_2fec_PaletteCycleEnabled)
			{
				// Instruction address 0x2dc4:0x0638, size: 5
				this.parent.CommonTools.StartPaletteCycleSlot(1);

				// Instruction address 0x2dc4:0x0644, size: 5
				this.parent.CommonTools.StartPaletteCycleSlot(2);

				// Instruction address 0x2dc4:0x0650, size: 5
				this.parent.CommonTools.StartPaletteCycleSlot(3);
			}

			this.Var_2fec_PaletteCycleEnabled = true;
		}

		/// <summary>
		/// Stops default palette cycle slots
		/// </summary>
		public void F0_2dc4_065f_StopPaletteCycleSlots()
		{
			//this.oCPU.Log.EnterBlock("F0_2dc4_065f()");

			// function body
			if (this.Var_2fec_PaletteCycleEnabled)
			{
				// Instruction address 0x2dc4:0x0671, size: 5
				this.parent.CommonTools.StopPaletteCycleSlot(1);

				// Instruction address 0x2dc4:0x067d, size: 5
				this.parent.CommonTools.StopPaletteCycleSlot(2);

				// Instruction address 0x2dc4:0x0689, size: 5
				this.parent.CommonTools.StopPaletteCycleSlot(3);
			}

			this.Var_2fec_PaletteCycleEnabled = false;
		}
	}
}
