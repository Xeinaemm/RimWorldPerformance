namespace Xeinaemm.Hauling;
public static class HaulingHarmony
{
	public static void ApplyHaulingPatches(this Harmony harmony)
	{
		if (!ModCompatibilityCheck.CombatExtendedIsActive)
		{
			harmony.Patch(original: AccessTools.Method(typeof(PawnUtility), nameof(PawnUtility.GetMaxAllowedToPickUp), [typeof(Pawn), typeof(ThingDef)]),
				prefix: new HarmonyMethod(typeof(HarmonyPatches), nameof(MaxAllowedToPickUpPrefix)));

			harmony.Patch(original: AccessTools.Method(typeof(PawnUtility), nameof(PawnUtility.CanPickUp)),
				prefix: new HarmonyMethod(typeof(HarmonyPatches), nameof(CanBeMadeToDropStuff)));
		}

		harmony.Patch(original: AccessTools.Method(typeof(Game), nameof(Game.ExposeSmallComponents)),
			prefix: new HarmonyMethod(typeof(HarmonyPatches), nameof(Game_ExposeSmallComponents_Prefix)));
		harmony.Patch(original: AccessTools.Method(typeof(WorkGiver_Haul), nameof(WorkGiver_Haul.ShouldSkip)),
			prefix: new HarmonyMethod(typeof(HarmonyPatches), nameof(ShouldSkip_Prefix)));

		harmony.Patch(original: AccessTools.Method(typeof(JobDriver_HaulToCell), nameof(JobDriver_HaulToCell.MakeNewToils)),
			postfix: new HarmonyMethod(typeof(HarmonyPatches), nameof(JobDriver_HaulToCell_PostFix)));
		harmony.Patch(original: AccessTools.Method(typeof(JobDriver_HaulToContainer), nameof(JobDriver_HaulToContainer.MakeNewToils)),
			postfix: new HarmonyMethod(typeof(HarmonyPatches), nameof(JobDriver_HaulToContainer_PostFix)));
		harmony.Patch(original: AccessTools.Method(typeof(JobGiver_Wander), nameof(JobGiver_WanderAnywhere.TryGiveJob)),
			postfix: new HarmonyMethod(typeof(HarmonyPatches), nameof(Wander_Postfix)));
		harmony.Patch(AccessTools.Method(typeof(JobGiver_Haul), nameof(JobGiver_Haul.TryGiveJob)),
			transpiler: new(typeof(HarmonyPatches), nameof(JobGiver_Haul_TryGiveJob_Transpiler)));

	}

	private static bool ShouldSkip_Prefix(WorkGiver_Haul __instance, ref bool __result, Pawn pawn)
	{
		if (__instance is not WorkGiver_HaulCorpses)
			return true;

		if (pawn.Map.listerThings.ThingsInGroup(ThingRequestGroup.Corpse).Count < 1)
		{
			__result = true;
			return false;
		}

		return true;
	}

	private static void JobDriver_HaulToCell_PostFix(JobDriver_HaulToCell __instance)
	{
		__instance.pawn.CheckUrgentHaul();
		__instance.pawn.CheckIfShouldUnloadInventory();
	}

	private static void JobDriver_HaulToContainer_PostFix(JobDriver_HaulToCell __instance)
	{
		__instance.pawn.CheckUrgentHaul();
		__instance.pawn.CheckIfShouldUnloadInventory();
	}

	public static void Wander_Postfix(Pawn pawn) => pawn.CheckIfShouldUnloadInventory();

	private static bool MaxAllowedToPickUpPrefix(Pawn pawn, ref int __result)
	{
		__result = int.MaxValue;
		return pawn.IsQuestLodger();
	}

	private static bool CanBeMadeToDropStuff(Pawn pawn, ref bool __result)
	{
		__result = !pawn.IsQuestLodger();
		return false;
	}

	private static IEnumerable<CodeInstruction> JobGiver_Haul_TryGiveJob_Transpiler(IEnumerable<CodeInstruction> instructions)
	{
		var originalMethod = AccessTools.Method(typeof(HaulAIUtility), nameof(HaulAIUtility.HaulToStorageJob), [typeof(Pawn), typeof(Thing), typeof(bool)]);
		var replacementMethod = AccessTools.Method(typeof(HarmonyPatches), nameof(HaulToStorageJobByRace));
		foreach (var instruction in instructions)
			yield return instruction.Calls(originalMethod) ? new CodeInstruction(OpCodes.Call, replacementMethod) : instruction;
	}

	private static Job HaulToStorageJobByRace(Pawn p, Thing t, bool forced) =>
		p.RaceProps.IsAllowedRace() ? p.HaulToInventory() : HaulAIUtility.HaulToStorageJob(p, t, forced);

	private static void Game_ExposeSmallComponents_Prefix(Game __instance)
	{
		if (__instance == null || __instance.Maps == null || __instance.Maps.Count == 0)
			return;
		__instance.Maps.CheckForGameChanges();
	}
}
