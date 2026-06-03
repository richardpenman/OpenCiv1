using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenCivOne
{
	public class ImprovementDefinition
	{
		public int ID = -1;

		// Total size: 30 bytes
		public string Name = ""; // (24 bytes)
		public int Cost = 0;
		public int MaintenanceCost = 0;
		public TechnologyAdvanceEnum RequiresTechnology = TechnologyAdvanceEnum.None;
		public TechnologyAdvanceEnum ObsoletesAfterTechnology = TechnologyAdvanceEnum.None;

		public ImprovementDefinition()
		{ }

		public ImprovementDefinition(int id, string name, int cost, int maintenanceCost, TechnologyAdvanceEnum requiresTechnology, TechnologyAdvanceEnum obsoletesAfterTechnology)
		{
			this.ID = id;
			this.Name = name;
			this.Cost = cost;
			this.MaintenanceCost = maintenanceCost;
			this.RequiresTechnology = requiresTechnology;
			this.ObsoletesAfterTechnology = obsoletesAfterTechnology;
		}
	}
}
