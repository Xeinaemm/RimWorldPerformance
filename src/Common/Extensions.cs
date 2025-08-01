namespace Xeinaemm.Common;
internal static class Extensions
{
	internal static bool IsAllowedRace(this Pawn pawn) =>
		(pawn.RaceProps.Humanlike || pawn.RaceProps.Animal || pawn.RaceProps.IsMechanoid) &&
		pawn.Faction == Faction.OfPlayerSilentFail;

	internal static void TickRateMultiplier(ref int __result)
	{
		if (Settings.EnableTickRateMultiplier)
			__result *= Settings.TickRateMultiplier;
	}

	internal static void TickRateInterval(ref int __result)
	{
		if (Settings.EnableTickRateMultiplier)
			__result = 360;
	}
}
