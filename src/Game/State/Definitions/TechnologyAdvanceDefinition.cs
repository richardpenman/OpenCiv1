namespace OpenCivOne
{
	public class TechnologyAdvanceDefinition
	{
		public TechnologyAdvanceEnum Value;
		public string Name;
		public TechnologyAdvanceEnum RequiresTechnologyAdvance1;
		public TechnologyAdvanceEnum RequiresTechnologyAdvance2;

		public TechnologyAdvanceDefinition(TechnologyAdvanceEnum value, string name, TechnologyAdvanceEnum requiresTechnologyAdvance1, TechnologyAdvanceEnum requiresTechnologyAdvance2)
		{
			this.Value = value;
			this.Name = name;
			this.RequiresTechnologyAdvance1 = requiresTechnologyAdvance1;
			this.RequiresTechnologyAdvance2 = requiresTechnologyAdvance2;
		}
	}
}
