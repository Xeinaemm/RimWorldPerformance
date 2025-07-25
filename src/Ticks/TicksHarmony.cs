namespace Xeinaemm.Ticks;
public static class TicksHarmony
{
	public static void ApplyTicksPatches(this Harmony harmony)
	{
		harmony.Patch(original: AccessTools.PropertyGetter(typeof(Thing), nameof(Thing.UpdateRateTicks)),
			postfix: new HarmonyMethod(typeof(HarmonyPatches), nameof(UpdateRateTicks_Thing_Postfix)));
		harmony.Patch(original: AccessTools.PropertyGetter(typeof(Thing), nameof(Thing.MaxTickIntervalRate)),
			postfix: new HarmonyMethod(typeof(HarmonyPatches), nameof(MaxTickIntervalRate_Thing_Postfix)));

		harmony.Patch(original: AccessTools.PropertyGetter(typeof(WorldObject), nameof(WorldObject.UpdateRateTicks)),
			postfix: new HarmonyMethod(typeof(HarmonyPatches), nameof(UpdateRateTicks_WorldObject_Postfix)));
	}

	[SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Reflection")]
	public static void UpdateRateTicks_Thing_Postfix(Thing __instance, ref int __result)
	{
		if (!Settings.EnableTicksMultiplier)
			return;
		__result *= Settings.TicksMultiplier;
	}
	public static void UpdateRateTicks_WorldObject_Postfix(ref int __result)
	{
		if (!Settings.EnableTicksMultiplier)
			return;
		__result *= Settings.TicksMultiplier;
	}

	[SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Reflection")]
	public static void MaxTickIntervalRate_Thing_Postfix(Thing __instance, ref int __result)
	{
		if (!Settings.EnableTicksMultiplier)
			return;
		__result = 60;
	}
}