namespace Xeinaemm.Common;

[SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Reflection")]
[SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Reflection")]
internal static class TickRatePatches
{
	[HarmonyPatch(typeof(WorldObject), "UpdateRateTicks", MethodType.Getter)]
	private static class Xeinaemm_WorldObject_UpdateRateTicks_Patch
	{
		private static void Postfix(ref int __result) => __result = Settings.TickRateMultiplier;
	}

	[HarmonyPatch(typeof(Thing), "UpdateRateTicks", MethodType.Getter)]
	private static class Xeinaemm_Thing_UpdateRateTicks_Patch
	{
		private static void Postfix(ref int __result) => __result = Settings.TickRateMultiplier;
	}

	[HarmonyPatch(typeof(Thing), "MaxTickIntervalRate", MethodType.Getter)]
	private static class Xeinaemm_Thing_MaxTickIntervalRate_Patch
	{
		private static void Postfix(ref int __result) => __result = Settings.TickRateMultiplier;
	}
}