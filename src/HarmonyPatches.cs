namespace Xeinaemm;
[StaticConstructorOnStartup]
internal static class HarmonyPatches
{
	static HarmonyPatches()
	{
		var harmony = new Harmony("xeinaemm.rimworld.performance");

		if (Settings.EnableDebugLogging)
			Harmony.DEBUG = true;
		harmony.PatchAll();
	}
}