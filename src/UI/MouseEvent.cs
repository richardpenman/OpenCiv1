using IRB.VirtualCPU;
using OpenCivOne.Graphics;

namespace OpenCivOne
{
	public struct MouseEvent
	{
		public GPoint Position;
		public MouseButtonsEnum Buttons;

		public MouseEvent(GPoint position, MouseButtonsEnum buttons)
		{
			this.Position = position;
			this.Buttons = buttons;
		}
	}
}
