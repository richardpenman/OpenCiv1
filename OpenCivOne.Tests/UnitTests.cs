namespace OpenCivOne.Tests
{
	public class UnitTests
	{
		[Fact]
		public void TestStatusFlagsIsTrueWhenFlagsAreSet()
		{
			Unit unit = new Unit();
			unit.Status = UnitStatusEnum.Sentry | UnitStatusEnum.Veteran;

			Assert.True(unit.TestStatusFlags(UnitStatusEnum.Sentry));
			Assert.True(unit.TestStatusFlags(UnitStatusEnum.Sentry | UnitStatusEnum.Veteran));
			Assert.False(unit.TestStatusFlags(UnitStatusEnum.Fortified));
			Assert.False(unit.TestStatusFlags(UnitStatusEnum.Sentry | UnitStatusEnum.Fortified));
		}

		[Fact]
		public void ClearStatusFlagsClearsOnlyGivenFlags()
		{
			Unit unit = new Unit();
			unit.Status = UnitStatusEnum.Sentry | UnitStatusEnum.Veteran;

			unit.ClearStatusFlags(UnitStatusEnum.Sentry | UnitStatusEnum.Fortified);

			Assert.Equal(UnitStatusEnum.Veteran, unit.Status);
		}
	}
}
