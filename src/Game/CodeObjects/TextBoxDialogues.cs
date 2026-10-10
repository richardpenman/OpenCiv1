using OpenCivOne.UI;

namespace OpenCivOne
{
	public class TextBoxDialogues
	{
		private OpenCivOneGame parent;

		public TextBoxDialogues(OpenCivOneGame parent)
		{
			this.parent = parent;
		}

		/// <summary>
		/// Shows popup dialog allowing player to enter a custom name for a city.
		/// Updates name of a city with specified ID.
		/// </summary>
		/// <param name="cityID"></param>
		/// <returns>1 if player entered custom city name, 0 in case of rejected input</returns>
		public int F23_0000_0000_CityNameDialog(int cityID)
		{

			// function body
			if (cityID >= 0 && cityID < 128)
			{
				string oldName = this.parent.GameData.CityNames[this.parent.GameData.Cities[cityID].NameID].TrimEnd(' ', '\x0');
				string? newName = EditBox.ShowEditBoxDialog(this.parent.MainWindow, "City name", "The name of your city", oldName, 12, false);

				if (!string.IsNullOrEmpty(newName))
				{
					this.parent.GameData.CityNames[this.parent.GameData.Cities[cityID].NameID] = newName;
				}

				return string.IsNullOrEmpty(newName) ? 0 : 1;
			}

			return 0;
			
		}

		/// <summary>
		/// Shows dialog allowing player to enter its name.
		/// Updates human player name.
		/// </summary>
		public void F23_0000_00d6_PlayerNameDialog()
		{

			// function body
			string? playerName = EditBox.ShowEditBoxDialog(this.parent.MainWindow, "Your name", "Please enter your name", 
				this.parent.GameData.Players[this.parent.GameData.HumanPlayerID].Name, 13, false);

			this.parent.GameData.Players[this.parent.GameData.HumanPlayerID].Name = 
				(string.IsNullOrEmpty(playerName) ? this.parent.GameData.Players[this.parent.GameData.HumanPlayerID].Name : playerName);

		}

		/// <summary>
		/// ?
		/// </summary>
		public void F23_0000_0173_TribeNameDialog()
		{

			// function body

			string? nationality = EditBox.ShowEditBoxDialog(this.parent.MainWindow, "Nation name", "For example 'Zulu' (not 'Zulus')",
				 this.parent.GameData.Players[this.parent.GameData.HumanPlayerID].Nationality, 10, false);

			this.parent.GameData.Players[this.parent.GameData.HumanPlayerID].Nationality =
				(string.IsNullOrEmpty(nationality) ? this.parent.GameData.Players[this.parent.GameData.HumanPlayerID].Nationality : nationality);

			// !!! To Do: Would need to change this, as different languages have different declinations rules, for now we will not add "s" to the end
			this.parent.GameData.Players[this.parent.GameData.HumanPlayerID].Nation = this.parent.GameData.Players[this.parent.GameData.HumanPlayerID].Nationality;

		}

		/// <summary>
		/// Shows find city dialog. Handles city name input and search.
		/// Centers map on a city if found.
		/// !!! To Do: Replace this TextBox dialog with a list of known cities.
		/// </summary>
		public void F23_0000_025b_FindCityDialog()
		{

			// function body

			string? cityName = EditBox.ShowEditBoxDialog(this.parent.MainWindow, "City name", "City name to search for", null, 12, true);


			if (!string.IsNullOrEmpty(cityName))
			{
				int cityID = -1;

				// Search all city names
				for (int j = 0; j < 128; j++)
				{
					int cityNameID = this.parent.GameData.Cities[j].NameID;

					if (this.parent.GameData.Cities[j].Status != CityStatusEnum.Invalid && cityNameID >= 0 && cityNameID < 256 &&
						cityName.StartsWith(this.parent.GameData.CityNames[cityNameID], StringComparison.OrdinalIgnoreCase))
					{
						cityID = j;
						break;
					}
				}

				if (cityID != -1)
				{
					City city = this.parent.GameData.Cities[cityID];

					if ((this.parent.GameData.MapVisibility[city.Position.X, city.Position.Y] & (1 << this.parent.GameData.HumanPlayerID)) != 0)
					{
						// Center map at the found city
						this.parent.MapManagement.F0_2aea_0008_DrawVisibleMap(this.parent.GameData.HumanPlayerID, city.Position.X - 7, city.Position.Y - 6);

						return;
					}
				}

				// Could not find the city by name, or the city is not visible
				MessageBox.Show(this.parent.MainWindow, $"Can not find the city named '{cityName}'.", "Unknown city", MessageBoxIcon.Information);
			}
		}
	}
}
