using OpenCivOne.UI;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace OpenCivOne
{
	/// <summary>
	/// This class encapsulates Global settings for the game
	/// </summary>
	public class GlobalPreferences
	{
		private GlobalDebugPreferences _debugSettings = new GlobalDebugPreferences();
		private bool _saveGameWarning = false;
		public GlobalPreferences() { }

		/// <summary>
		/// Settings used for Debugging the game
		/// </summary>
		public GlobalDebugPreferences Debug { get => this._debugSettings; set => this._debugSettings = value; }

		[XmlIgnore]
		[JsonIgnore]
		public bool SaveGameWarning { get => this._saveGameWarning; set => this._saveGameWarning = value; }


		private static string GetFullSettingsPath(string path)
		{
			return $"{path}{Path.DirectorySeparatorChar}Preferences.json";
		}

		public static GlobalPreferences DeSerializeFromJSON(OpenCivOneGame game, string path)
		{
			string fullPath = GetFullSettingsPath(path);
			GlobalPreferences newPreferences = new GlobalPreferences();

			try
			{
				if (File.Exists(fullPath))
				{
					GlobalPreferences? preferences = JsonSerializer.Deserialize<GlobalPreferences>(File.ReadAllText(fullPath));

					if (preferences != null)
					{
						return preferences;
					}

					MessageBox.Show(game.MainWindow, $"Can not read '{Path.GetFileName(fullPath)}', the file exists, but could not be read", "Error", 
						MessageBoxIcon.Error, MessageBoxButtons.OK);

					newPreferences.SaveGameWarning = true;
				}
			}
			catch(Exception e)
			{
				MessageBox.Show(game.MainWindow, $"Can not read '{Path.GetFileName(fullPath)}'.\nException message: '{e.Message}'", "Error",
					MessageBoxIcon.Error, MessageBoxButtons.OK);
			}

			return newPreferences;
		}

		public void SerializeToJSON(OpenCivOneGame game, string path)
		{
			string fullPath = GetFullSettingsPath(path);

			if (!this._saveGameWarning || MessageBox.Show(game.MainWindow, $"There was an error reading '{Path.GetFileName(fullPath)}'. Are you sure you want to try to overwrite it?", 
				"Overwrite preferences", MessageBoxIcon.Question, MessageBoxButtons.YesNo) == MessageBoxResult.Yes)
			{
				try
				{
					File.WriteAllText(fullPath, JsonSerializer.Serialize(this));
				}
				catch (Exception e)
				{
					MessageBox.Show(game.MainWindow, $"Can not write '{Path.GetFileName(fullPath)}'.\nException message: '{e.Message}'", "Error",
						MessageBoxIcon.Error, MessageBoxButtons.OK);
				}
			}
		}
	}
}
