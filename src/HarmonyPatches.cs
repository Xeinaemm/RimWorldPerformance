namespace Xeinaemm;
[StaticConstructorOnStartup]
internal static class HarmonyPatches
{
	static HarmonyPatches()
	{
		var harmony = new Harmony("xeinaemm.rimworld.performance");

		if (Settings.EnableDebugLogging)
			Harmony.DEBUG = true;

		harmony.Patch(AccessTools.PropertyGetter(typeof(Thing), "MaxTickIntervalRate"),
			postfix: new HarmonyMethod(typeof(Extensions), nameof(Extensions.TickRate)));
		harmony.Patch(AccessTools.PropertyGetter(typeof(Thing), "UpdateRateTicks"),
			postfix: new HarmonyMethod(typeof(Extensions), nameof(Extensions.TickRate)));
		harmony.Patch(AccessTools.PropertyGetter(typeof(WorldObject), "UpdateRateTicks"),
			postfix: new HarmonyMethod(typeof(Extensions), nameof(Extensions.TickRate)));

		harmony.Patch(AccessTools.Method(typeof(JobDriver_Deconstruct), "MakeNewToils"),
			postfix: new HarmonyMethod(typeof(Construction.Extensions), nameof(Construction.Extensions.CheckRoofs)));
		harmony.Patch(AccessTools.Method(typeof(JobDriver_RemoveBuilding), "MakeNewToils"),
			postfix: new HarmonyMethod(typeof(Construction.Extensions), nameof(Construction.Extensions.CheckRoofs)));
		harmony.Patch(AccessTools.Method(typeof(JobDriver_Mine), "MakeNewToils"),
			postfix: new HarmonyMethod(typeof(Construction.Extensions), nameof(Construction.Extensions.CheckRoofs)));

		harmony.Patch(AccessTools.Method(typeof(JobGiver_Haul), "TryGiveJob"),
			postfix: new HarmonyMethod(typeof(Hauling.Extensions), nameof(Hauling.Extensions.TryGiveJob)));
		harmony.Patch(AccessTools.Method(typeof(PawnUtility), nameof(PawnUtility.GetMaxAllowedToPickUp), [typeof(Pawn), typeof(ThingDef)]),
			prefix: new HarmonyMethod(typeof(Hauling.Extensions), nameof(Hauling.Extensions.GetMaxAllowedToPickUp)));
		harmony.Patch(AccessTools.Method(typeof(PawnUtility), nameof(PawnUtility.CanPickUp)),
			prefix: new HarmonyMethod(typeof(Hauling.Extensions), nameof(Hauling.Extensions.CanPickUp)));
	}
}