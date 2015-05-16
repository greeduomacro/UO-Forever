namespace VitaNex.FX
{
	public static class EffectsOverride
	{
		public static void Configure()
		{
			VitaNexCore.OnInitialized += () =>
			{
				WaterWaveEffect.DisplayElemental = false;
				FireWaveEffect.DisplayElemental = false;
				EarthWaveEffect.DisplayElemental = false;
				AirWaveEffect.DisplayElemental = false;
			};
		}
	}
}