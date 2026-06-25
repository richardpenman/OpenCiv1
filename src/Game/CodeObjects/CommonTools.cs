using System;
using Avalonia.Media;
using IRB.Collections.Generic;
using IRB.VirtualCPU;
using OpenCivOne.Graphics;

namespace OpenCivOne
{
	public class CommonTools
	{
		private OpenCivOneGame parent;
		private VCPU CPU;
		private GDriver graphics;

		private bool inTimer = false;
		private Timer? timer = null;

		private BDictionary<int, PaletteCycleSlot> paletteCycleSlots = new BDictionary<int, PaletteCycleSlot>();

		private bool transformFlag = false;
		private int transformValue = 0;
		private int transformCount = 0;
		private TransformColor[] transformColors = new TransformColor[0];

		public CommonTools(OpenCivOneGame parent)
		{
			this.parent = parent;
			this.CPU = parent.CPU;
			this.graphics = parent.Graphics;
		}

		/// <summary>
		/// Initializes main game timer
		/// </summary>
		public void StartGameMainTimer()
		{
			//this.oCPU.Log.EnterBlock("F0_1000_0000_InitializeTimer()");

			// function body
			if (this.timer == null)
			{
				this.CPU.EnableTimer = true;
				this.timer = new Timer(MainGameTimer, null, 10, 10);
			}
		}

		/// <summary>
		/// Stops main game timer
		/// </summary>
		public void StopGameMainTimer()
		{
			//this.oCPU.Log.EnterBlock("F0_1000_0051_InitTimer()");

			// function body
			if (this.timer != null)
			{
				this.timer.Dispose();
				this.timer = null;
			}
		}

		/// <summary>
		/// Timer function, it should fire approximately every 20 ms
		/// </summary>
		private void MainGameTimer(object? state)
		{
			lock (VCPU.GraphicsLock)
			{
				this.inTimer = true;

				if (this.transformFlag)
				{
					// Instruction address 0x1000:0x0349, size: 5
					TransformPaletteTimer();
				}
				else
				{
					// Instruction address 0x1000:0x034e, size: 5
					CyclePaletteTimer();
				}

				this.parent.Var_5c_TickCount++;

				// sound driver call
				// Instruction address 0x1000:0x01e7, size: 5
				SoundWorkerTimer();

				this.inTimer = false;
			}
		}

		/// <summary>
		/// Processes palette cycles and updates current palette
		/// </summary>
		/// <exception cref="Exception"></exception>
		public void CyclePaletteTimer()
		{
			for (int i = 0; i < this.paletteCycleSlots.Count; i++)
			{
				PaletteCycleSlot slot = this.paletteCycleSlots[i].Value;

				if (slot.Active)
				{
					slot.SpeedCount++;

					if (slot.SpeedCount > slot.Speed)
					{
						slot.SpeedCount = 0;
						slot.CurrentPosition = (slot.CurrentPosition + 1) % slot.Palette.Length;

						for (int j = 0; j < slot.Palette.Length; j++)
						{
							int index = (slot.CurrentPosition + j) % slot.Palette.Length;

							this.graphics.SetPaletteColor((byte)(slot.StartPosition + index), slot.Palette[j]);
						}
					}
				}
			}
		}

		/// <summary>
		/// ?
		/// </summary>
		/// <exception cref="Exception"></exception>
		public void TransformPaletteTimer()
		{
			if (this.transformFlag)
			{
				this.transformCount++;

				for (int i = 0; i < this.transformColors.Length; i++)
				{
					this.graphics.SetPaletteColor((byte)i, this.transformColors[i].GetNextColor(this.transformCount).ToColor());
				}

				if (this.transformCount >= this.transformValue)
				{
					for (int i = 0; i < this.transformColors.Length; i++)
					{
						this.graphics.SetPaletteColor((byte)i, this.transformColors[i].ToHSV.ToColor());
					}

					this.transformFlag = false;
				}
			}
		}

		/// <summary>
		/// Reset timer count
		/// </summary>
		/// <returns></returns>
		public void ResetWaitTimer()
		{
			// function body
			this.parent.Var_5c_TickCount = 0;
		}

		/// <summary>
		/// Waits for specified number of main timer ticks
		/// </summary>
		/// <param name="waitTime"></param>
		public void WaitTimer(int waitTime)
		{
			//this.oCPU.Log.EnterBlock($"F0_1182_0134_WaitTime({waitTime})");

			ResetWaitTimer();

			int iTime = Math.Max(waitTime * 12, 1);

			Thread.Sleep(iTime);
			this.CPU.DoEvents();

			/*waitTime = (short)(Math.Ceiling(0.6 * waitTime));

			while (this.oParent.Var_5c_TickCount < waitTime)
			{
				Thread.Sleep(1);
			}//*/
		}

		/// <summary>
		/// ?
		/// </summary>
		/// <param name="index"></param>
		/// <param name="speed"></param>
		/// <param name="fromColorIndex"></param>
		/// <param name="toColorIndex"></param>
		/// <exception cref="Exception"></exception>
		public void AddPaletteCycleSlot(int index, int speed, byte fromColorIndex, byte toColorIndex)
		{
			//this.oCPU.Log.EnterBlock($"F0_1000_0382_AddPaletteCycleSlot({index}, {speed}, {fromColorIndex}, {toColorIndex})");

			if (index < 0 || index > 8)
				throw new ArgumentOutOfRangeException("Argument index is out of range");

			if (fromColorIndex > toColorIndex)
				throw new ArgumentOutOfRangeException("Argument fromColorIndex is greater than toColorIndex");

			while (this.inTimer)
			{
				Thread.Sleep(1);
			}

			// function body
			lock (VCPU.GraphicsLock)
			{
				if (this.paletteCycleSlots.ContainsKey(index))
				{
					// restore old slot palette
					PaletteCycleSlot oldSlot = this.paletteCycleSlots.GetValueByKey(index);
					
					if (oldSlot.Active)
					{
						for (int i = 0; i < oldSlot.Palette.Length; i++)
						{
							this.graphics.SetPaletteColor((byte)(oldSlot.StartPosition + i), oldSlot.Palette[i]);
						}
					}

					// prepare new slot
					Color[] palette = new Color[(toColorIndex - fromColorIndex) + 1];
					for (int i = 0; i < palette.Length; i++)
					{
						palette[i] = this.graphics.GetPaletteColor((byte)(fromColorIndex + i));
					}

					PaletteCycleSlot newSlot = new PaletteCycleSlot(speed, fromColorIndex, palette);
					//newSlot.Active = oldSlot.Active;

					this.paletteCycleSlots.SetValueByKey(index, newSlot);
				}
				else
				{
					Color[] palette = new Color[(toColorIndex - fromColorIndex) + 1];
					for (int i = 0; i < palette.Length; i++)
					{
						palette[i] = this.graphics.GetPaletteColor((byte)(fromColorIndex + i));
					}

					PaletteCycleSlot slot = new PaletteCycleSlot(speed, fromColorIndex, palette);

					this.paletteCycleSlots.Add(index, slot);
				}
			}
		}

		/// <summary>
		/// ?
		/// </summary>
		/// <param name="index"></param>
		public void StartPaletteCycleSlot(int index)
		{
			//this.oCPU.Log.EnterBlock($"F0_1000_03fa_StartPaletteCycleSlot({index})");

			// function body
			if (this.paletteCycleSlots.ContainsKey(index))
			{
				while (this.inTimer)
				{
					Thread.Sleep(1);
				}

				lock (VCPU.GraphicsLock)
				{
					this.paletteCycleSlots.GetValueByKey(index).Active = true;
				}
			}
			else
			{
				//throw new Exception($"Attempt to start undefined PaletteCycleSlot({index})");
			}
		}

		/// <summary>
		/// ?
		/// </summary>
		/// <param name="index"></param>
		public void StopPaletteCycleSlot(int index)
		{
			//this.oCPU.Log.EnterBlock($"F0_1000_042b_StopPaletteCycleSlot({index})");

			// function body
			if (this.paletteCycleSlots.ContainsKey(index))
			{
				while (this.inTimer)
				{
					Thread.Sleep(1);
				}

				lock (VCPU.GraphicsLock)
				{
					PaletteCycleSlot slot = this.paletteCycleSlots.GetValueByKey(index);

					if (slot.Active)
					{
						for (int i = 0; i < slot.Palette.Length; i++)
						{
							this.graphics.SetPaletteColor((byte)(slot.StartPosition + i), slot.Palette[i]);
						}
					}

					slot.Active = false;
				}
			}
		}

		/// <summary>
		/// Transform current palette to another palette
		/// </summary>
		/// <param name="speed"></param>
		/// <param name="palettePtr"></param>
		public void TransformPalette(int speed, ushort palettePtr)
		{
			//this.oCPU.Log.EnterBlock($"F0_1000_04aa_TransformPalette({speed}, 0x{palettePtr:x4})");

			// function body
			if (speed < 1 || speed > 50)
				throw new ArgumentOutOfRangeException("The argument speed is out of range");

			this.transformColors = new TransformColor[256];
			palettePtr += 6;

			lock (VCPU.GraphicsLock)
			{
				while (this.inTimer)
				{
					Thread.Sleep(1);
				}

				this.transformValue = speed * 10;
				this.transformCount = 0;

				for (int i = 0; i < 256; i++)
				{
					HSVColor from = HSVColor.FromColor(this.graphics.GetPaletteColor((byte)i));
					HSVColor to = HSVColor.FromColor(GBitmap.Color18ToColor(this.CPU.ReadUInt8(this.CPU.DS.UInt16, palettePtr),
						this.CPU.ReadUInt8(this.CPU.DS.UInt16, (ushort)(palettePtr + 1)),
						this.CPU.ReadUInt8(this.CPU.DS.UInt16, (ushort)(palettePtr + 2))));

					this.transformColors[i] = new TransformColor(from, to, this.transformValue);

					palettePtr += 3;
				}

				this.transformFlag = true;
			}

			while (this.transformFlag)
			{
				this.CPU.DoEvents();
				Thread.Sleep(1);
			}
		}

		/// <summary>
		/// Transform current palette to another palette
		/// </summary>
		/// <param name="speed"></param>
		/// <param name="palettePtr"></param>
		public void TransformPalette(int speed, byte[] palette)
		{
			//this.oCPU.Log.EnterBlock($"F0_1000_04aa_TransformPalette({speed}, 0x{palettePtr:x4})");

			// function body
			int position = 0;

			if (speed < 1 || speed > 50)
				throw new ArgumentOutOfRangeException("The argument speed is out of range");

			this.transformColors = new TransformColor[256];
			position += 6;

			lock (VCPU.GraphicsLock)
			{
				while (this.inTimer)
				{
					Thread.Sleep(1);
				}

				this.transformValue = speed * 10;
				this.transformCount = 0;

				for (int i = 0; i < 256; i++)
				{
					HSVColor from = HSVColor.FromColor(this.graphics.GetPaletteColor((byte)i));
					HSVColor to = HSVColor.FromColor(GBitmap.Color18ToColor(palette[position], palette[position + 1], palette[position + 2]));

					this.transformColors[i] = new TransformColor(from, to, this.transformValue);

					position += 3;
				}

				this.transformFlag = true;
			}

			while (this.transformFlag)
			{
				this.CPU.DoEvents();
				Thread.Sleep(1);
			}
		}

		/// <summary>
		/// Transform entire palette to one color
		/// </summary>
		/// <param name="speed"></param>
		/// <param name="color"></param>
		public void TransformPaletteToColor(int speed, Color color)
		{
			//this.oCPU.Log.EnterBlock($"F0_1000_04d4_TransformPaletteToColor({speed}, {color})");

			// function body
			HSVColor to = HSVColor.FromColor(color);
			this.transformColors = new TransformColor[256];

			lock (VCPU.GraphicsLock)
			{
				while (this.inTimer)
				{
					Thread.Sleep(1);
				}

				this.transformValue = speed * 10;
				this.transformCount = 0;

				for (int i = 0; i < 256; i++)
				{
					HSVColor from = HSVColor.FromColor(this.graphics.GetPaletteColor((byte)i));

					this.transformColors[i] = new TransformColor(from, to, this.transformValue);
				}

				this.transformFlag = true;
			}

			while (this.transformFlag)
			{
				this.CPU.DoEvents();
				Thread.Sleep(1);
			}
		}

		/// <summary>
		/// ?
		/// </summary>
		/// <param name="screenID"></param>
		public void F0_1000_0846(int screenID)
		{
			//this.oCPU.Log.EnterBlock($"F0_1000_0846({screenID})");

			// function body
			//this.oParent.Graphics.F0_VGA_063c(screenID);
			this.parent.Graphics.F0_VGA_06b7_DrawScreenToMainScreenWithEffect(screenID);
		}

		/// <summary>
		/// Init sound engine
		/// </summary>
		public void InitSoundEngine()
		{
			//this.oCPU.Log.EnterBlock("Sound overlay 'F0_1000_0a2b_InitSound'");

			// Instruction address 0x1000:0x0a2b, size: 5
			this.parent.Sound.F0_0000_0048_InitSound();
			//this.oCPU.Log.ExitBlock("Sound overlay 'F0_1000_0a2b_InitSound'");
		}

		/// <summary>
		/// Play the selected music or effect
		/// </summary>
		/// <param name="tune"></param>
		/// <param name="param2"></param>
		public void PlayTune(short tune, ushort param2)
		{
			if (this.parent.GameData.GameSettingFlags.Sound)
			{
				// Instruction address 0x1000:0x0a32, size: 5
				this.parent.Sound.F0_0000_0062_PlayTune(tune, param2);
			}
		}

		/// <summary>
		/// Dispose of sound engine
		/// </summary>
		/// <returns></returns>
		public void CloseSoundEngine()
		{
			//this.oCPU.Log.EnterBlock("Sound overlay 'F0_1000_0a39_CloseSound'");

			// Instruction address 0x1000:0x0a39, size: 5
			this.parent.Sound.F0_0000_006a_CloseSound();
			//this.oCPU.Log.ExitBlock("Sound overlay 'F0_1000_0a39_CloseSound'");
		}

		/// <summary>
		/// Refreshes the sound engine status
		/// </summary>
		/// <returns></returns>
		public ushort SoundWorkerTimer()
		{
			//this.oCPU.Log.EnterBlock("Sound overlay 'F0_1000_0a40_SoundWorker'");

			// Instruction address 0x1000:0x0a40, size: 5
			return this.parent.Sound.F0_0000_0055_SoundWorker();
		}

		/// <summary>
		/// Refreshes the fast sound engine status
		/// </summary>
		public void FastSoundWorkerTimer()
		{
			//this.oCPU.Log.EnterBlock("Sound overlay 'F0_1000_0a47_FastSoundWorker'");

			// function body
			// Instruction address 0x1000:0x0a47, size: 5
			this.parent.Sound.F0_0000_005c_FastSoundWorker();
			//this.oCPU.Log.ExitBlock("Sound overlay 'F0_1000_0a47_FastSoundWorker'");
		}

		/// <summary>
		/// Advances the sound engine timer
		/// </summary>
		/// <returns></returns>
		public ushort SoundEngineTimer()
		{
			//this.oCPU.Log.EnterBlock("Sound overlay 'F0_1000_0a4e_SoundTimer'");

			// Instruction address 0x1000:0x0a4e, size: 5
			return this.parent.Sound.F0_0000_005d_SoundTimer();
		}

		/// <summary>
		/// Sets the mouse position and icon
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="bitmapPtr"></param>
		public void SetMousePositionAndIcon(int x, int y, int bitmapPtr)
		{
			//this.oCPU.Log.EnterBlock($"F0_1000_1697({x}, {y}, {bitmapPtr})");

			// function body
			this.parent.Var_5878_MouseIconXOffset = x;
			this.parent.Var_587a_MouseIconYOffset = y;
			this.parent.Var_5876_MouseIcon = bitmapPtr;
		}

		/// <summary>
		/// Updates current mouse position and pressed button state.
		/// </summary>
		public void UpdateMouseState()
		{
			// function body
			this.CPU.DoEvents();

			// Instruction address 0x11a8:0x022a, size: 5
			//this.parent.Var_db3a_MouseButton = this.parent.Var_5872_MouseNewButtons;
			//this.parent.Var_db3c_MouseXPos = this.parent.Var_586e_MouseNewX;
			//this.parent.Var_db3e_MouseYPos = this.parent.Var_5870_MouseNewY;
		}

		/// <summary>
		/// Handles mouse events
		/// </summary>
		public void HandleMouseEvent()
		{
			//this.oCPU.Log.EnterBlock("F0_1000_17db_MouseEvent()");

			// function body
			//this.parent.Var_5872_MouseNewButtons = this.CPU.BX.Int16;
			//this.parent.Var_586e_MouseNewX = this.CPU.CX.Int16;
			//this.parent.Var_5870_MouseNewY = this.CPU.DX.Int16;
		}

		/// <summary>
		/// Empties keyboard buffer and clears the mouse state
		/// </summary>
		public void EmptyKeyboardBufferAndClearMouse()
		{
			this.CPU.DoEvents();

			while (this.parent.CAPI.kbhit() != 0)
			{
				this.parent.MenuBoxDialog.F0_2d05_0ac9_GetNavigationKey();
			}

			this.parent.EmptyMouseEvents();
		}
	}
}
