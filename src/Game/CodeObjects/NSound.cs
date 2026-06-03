using IRB.VirtualCPU;

namespace OpenCivOne
{
	public class NSound
	{
		private OpenCivOneGame parent;
		private VCPU CPU;
		private ushort bufferPosition = 0;

		public NSound(OpenCivOneGame parent)
		{
			this.parent = parent;
			this.CPU = parent.CPU;
		}

		/// <summary>
		/// ?
		/// </summary>
		/// <returns></returns>
		public ushort F0_0000_0048_InitSound()
		{
			//this.oCPU.Log.EnterBlock("'F0_0000_0048_InitSound'(Cdecl, Far) at 0x0000:0x0048");

			// function body
			bufferPosition = 0;
			this.CPU.AX.UInt16 = 0;

			return this.CPU.AX.UInt16;
		}

		/// <summary>
		/// ?
		/// </summary>
		/// <returns></returns>
		public ushort F0_0000_0055_SoundWorker()
		{
			//this.oCPU.Log.EnterBlock("'F0_0000_0055_SoundWorker'(Cdecl, Far) at 0x0000:0x0055");

			// function body
			bufferPosition++;
			
			return 0;
		}

		/// <summary>
		/// ?
		/// </summary>
		public void F0_0000_005c_FastSoundWorker()
		{
			//this.oCPU.Log.EnterBlock("'F0_0000_005c_FastSoundWorker'(Cdecl, Far) at 0x0000:0x005c");

			// function body
		}

		/// <summary>
		/// ?
		/// </summary>
		/// <returns></returns>
		public ushort F0_0000_005d_SoundTimer()
		{
			//this.oCPU.Log.EnterBlock("'F0_0000_005d_SoundTimer'(Cdecl, Far) at 0x0000:0x005d");

			// function body
			this.CPU.AX.UInt16 = bufferPosition;

			return this.CPU.AX.UInt16;
		}

		/// <summary>
		/// ?
		/// </summary>
		/// <param name="tune"></param>
		/// <param name="param2"></param>
		public void F0_0000_0062_PlayTune(short tune, ushort param2)
		{
			//this.oCPU.Log.EnterBlock("'F0_0000_0062_PlayTune'(Cdecl, Far) at 0x0000:0x0062");

			// function body
			bufferPosition = 0;
		}

		/// <summary>
		/// ?
		/// </summary>
		/// <returns></returns>
		public ushort F0_0000_006a_CloseSound()
		{
			//this.oCPU.Log.EnterBlock("'F0_0000_006a_CloseSound'(Cdecl, Far) at 0x0000:0x006a");

			// function body
			this.CPU.AX.UInt16 = 0;

			return this.CPU.AX.UInt16;
		}
	}
}
