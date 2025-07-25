namespace Xeinaemm;

public class Settings : ModSettings
{
	private static bool _enableDebugLogging;
	private static bool _enableTicksMultiplier = true;
	private static int _ticksMultiplier = 10;

	public static bool EnableDebugLogging => _enableDebugLogging;
	public static bool EnableTicksMultiplier => _enableTicksMultiplier;
	public static int TicksMultiplier => _ticksMultiplier;

	public static void DoSettingsWindowContents(Rect inRect)
	{
		var ls = new Listing_Standard();
		ls.Begin(inRect);
		ls.CheckboxLabeled("Enable debug mode", ref _enableDebugLogging, "When enabled, game outputs detailed debug information to the game log.");
		ls.CheckboxLabeled("Enable ticks multiplier", ref _enableTicksMultiplier, "When enabled, game will update less frequently, which will save TPS.");

		if (_enableTicksMultiplier)
		{
			ls.Label("Ticks multiplier");
			_ticksMultiplier = (int)ls.Slider(_ticksMultiplier, 1f, 100f);
			_ticksMultiplier = Math.Clamp(_ticksMultiplier, 1, 100);
		}
		ls.Gap();
		if (Widgets.ButtonText(new Rect(0f, ls.CurHeight, 180f, 29f), "Reset settings"))
		{
			_enableDebugLogging = false;
			_enableTicksMultiplier = true;
			_ticksMultiplier = 10;
		}
		ls.End();
	}

	public override void ExposeData()
	{
		base.ExposeData();
		Scribe_Values.Look(ref _enableDebugLogging, "enableDebugLogging", false);
		Scribe_Values.Look(ref _enableTicksMultiplier, "enableTicksMultiplier", true);
		Scribe_Values.Look(ref _ticksMultiplier, "ticksMultiplierRate", 10);
	}
}