namespace Xeinaemm;

public class Settings : ModSettings
{
	private static bool _enableDebugLogging;
	private static int _tickRateMultiplier;
	private static bool _enableTickRateMultiplier = true;

	public static bool EnableDebugLogging => _enableDebugLogging;
	public static int TickRateMultiplier => _tickRateMultiplier;
	public static bool EnableTickRateMultiplier => _enableTickRateMultiplier;

	public static void DoSettingsWindowContents(Rect inRect)
	{
		var ls = new Listing_Standard();
		ls.Begin(inRect);
		ls.CheckboxLabeled("Enable debug mode", ref _enableDebugLogging, "When enabled, game outputs detailed debug information to the game log.");
		ls.CheckboxLabeled("Enable tick rate multiplier", ref _enableTickRateMultiplier, "When enabled, allows adjust multiplier of update tick rates.");

		if (_enableTickRateMultiplier)
			_tickRateMultiplier = (int)ls.SliderLabeled($"Tick rates multiplier: {_tickRateMultiplier}", _tickRateMultiplier, 1f, 360f);

		ls.Gap();
		if (Widgets.ButtonText(new Rect(0f, ls.CurHeight, 180f, 29f), "Reset settings"))
		{
			_enableDebugLogging = false;
			_tickRateMultiplier = 60;
			_enableTickRateMultiplier = true;
		}
		ls.End();
	}

	public override void ExposeData()
	{
		base.ExposeData();
		Scribe_Values.Look(ref _enableDebugLogging, "enableDebugLogging", false);
		Scribe_Values.Look(ref _tickRateMultiplier, "tickRateMultiplier", 60);
		Scribe_Values.Look(ref _enableTickRateMultiplier, "enableTickRateMultiplier", true);
	}
}