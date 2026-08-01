using OpenCivOne.Graphics;
using OpenCivOne.UI;

namespace OpenCivOne
{
	public class Tools
	{
		private OpenCivOneGame parent;

		private bool Var_2fec_PaletteCycleEnabled = false;

		public Tools(OpenCivOneGame parent)
		{
			this.parent = parent;
		}

		/// <summary>
		/// Randomizes a default Random Number Generator
		/// </summary>
		public void RandomizeRNG()
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
		public int CheckValueRange(int value, int minValue, int maxValue)
		{
			// function body
			if (minValue > maxValue)
			{
				// do nothing, just return minimum value
				//this.parent.Log.WriteLine($"CheckValueRange({value}, {minValue}, {maxValue}), Minimum value is greater than maximum value.");
				//return minValue;
				throw new Exception("Minimum value is greater than maximum value");
			}

			return Math.Min(Math.Max(value, minValue), maxValue);
		}

		/// <summary>
		/// Tests if city is present at given coordinates
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns>City ID, otherwise -1</returns>
		public int GetCityByLocation(int x, int y)
		{
			//this.oCPU.Log.EnterBlock($"F0_2dc4_00ba({x}, {y})");

			// function body
			for (int i = 0; i < 128; i++)
			{
				City city = this.parent.GameData.Cities[i];

				if (city.Status != CityStatusEnum.Invalid && city.Position.X == x && city.Position.Y == y)
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
		public int FindNearestCity(GPoint pt)
		{
			return FindNearestCity(pt.X, pt.Y);
		}

		/// <summary>
		/// Finds a nearest city
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		public int FindNearestCity(int x, int y)
		{
			//this.oCPU.Log.EnterBlock($"F0_2dc4_0102({xPos}, {yPos})");

			// function body
			int minDistance = 999;
			int cityID = -1;

			for (int i = 0; i < 128; i++)
			{
				City city = this.parent.GameData.Cities[i];

				if (city.Status != CityStatusEnum.Invalid)
				{
					// Instruction address 0x2dc4:0x0142, size: 3
					int distance = this.parent.MapManagement.GetDistance(x, y, city.Position);

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
		public int FindNearestPlayerUnit(int playerID, int currentUnitID, int x, int y)
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
					int distance = this.parent.MapManagement.GetDistance(x, y, this.parent.GameData.Players[playerID].Units[i].Position);

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
		/// Get a sum of total population count for a given player
		/// </summary>
		/// <param name="playerID"></param>
		/// <returns></returns>
		public int GetPlayerPopulationCount(int playerID)
		{
			//this.oCPU.Log.EnterBlock($"F0_2dc4_02cd({playerID})");

			// function body
			int population = 0;

			for (int i = 0; i < 128; i++)
			{
				if (this.parent.GameData.Cities[i].PlayerID == playerID && this.parent.GameData.Cities[i].Status != CityStatusEnum.Invalid)
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
		public string PopulationValueToString(int value)
		{
			// function body
			return $"{(value * 1000):###,###,##0}";
		}

		/// <summary>
		/// Reads the palette from file and applies it to screen 0
		/// </summary>
		/// <param name="filenamePtr"></param>
		public void ReadAndSetPalette(string filename)
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
		public void FreeNativeResource(int bitmapID, string text)
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
		public void AddDefaultPaletteCycleSlots()
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
		public void StartPaletteCycleSlots()
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
		public void StopPaletteCycleSlots()
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
