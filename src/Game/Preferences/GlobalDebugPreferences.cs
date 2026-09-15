
namespace OpenCivOne
{
	public class GlobalDebugPreferences
	{
		private bool _debugMode = false;
		private bool _fastMode=false;
		private bool _saveEveryTurn = false;

		public GlobalDebugPreferences() { }

		public bool DebugMode { get => this._debugMode; set => this._debugMode = value; }

		public bool FastMode { get => this._fastMode; set => this._fastMode = value; }

		public bool SaveEveryTurn { get => this._saveEveryTurn; set => this._saveEveryTurn = value; }
	}
}
