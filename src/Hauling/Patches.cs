namespace Xeinaemm.Hauling;

[SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Reflection")]
[SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Reflection")]
internal static class Patches
{
	/// <summary>
	/// The Combat Extended compatibility patch includes an infinite maximum pickup and a filter for quests.
	/// </summary>
	[HarmonyPatch(typeof(PawnUtility), nameof(PawnUtility.GetMaxAllowedToPickUp), [typeof(Pawn), typeof(ThingDef)])]
	private static class Xeinaemm_GetMaxAllowedToPickUp_Patch
	{
		private static bool Prefix(Pawn pawn, ThingDef thingDef, ref int __result)
		{
			if (!ModCompatibilityCheck.CombatExtendedIsActive)
			{
				__result = int.MaxValue;
				return pawn.IsQuestLodger();
			}
			return true;
		}
	}

	/// <summary>
	/// Combat Extended compatibility patch includes a filter for quests.
	/// </summary>
	[HarmonyPatch(typeof(PawnUtility), nameof(PawnUtility.CanPickUp))]
	private static class Xeinaemm_CanPickUp_Patch
	{
		private static bool Prefix(Pawn pawn, ThingDef thingDef, ref bool __result)
		{
			if (!ModCompatibilityCheck.CombatExtendedIsActive)
			{
				__result = !pawn.IsQuestLodger();
				return false;
			}
			return true;
		}
	}

	/// <summary>
	/// Patch that prevents the game from saving mod-specific information and cleans up the mechanism.
	/// The main purpose is to prevent corruption of the save files during the uninstallation of the mod.
	/// </summary>
	[HarmonyPatch(typeof(Game), nameof(Game.ExposeData))]
	private static class Xeinaemm_ExposeData_Patch
	{
		private static void Prefix(Game __instance) => __instance.CheckForGameChanges();
	}

	[HarmonyPatch(typeof(JobGiver_Haul), "TryGiveJob")]
	private static class Xeinaemm_JobGiver_Haul_Patch
	{
		private static void Prefix(Pawn pawn)
		{
			pawn.CheckIfShouldUnloadInventory();
			pawn.CheckUrgentHaul();
		}

		private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			var originalMethod = AccessTools.Method(typeof(HaulAIUtility), nameof(HaulAIUtility.HaulToStorageJob));
			var replacementMethod = AccessTools.Method(typeof(Patches), nameof(HaulToStorageJobByRace));

			foreach (var instruction in instructions)
				yield return instruction.Calls(originalMethod) ? new CodeInstruction(OpCodes.Call, replacementMethod) : instruction;
		}
	}

	private static Job HaulToStorageJobByRace(Pawn p, Thing t, bool forced) =>
		p.RaceProps.IsAllowedRace() ? p.HaulToInventory() : HaulAIUtility.HaulToStorageJob(p, t, forced);
}
