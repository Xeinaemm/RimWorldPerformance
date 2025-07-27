namespace Xeinaemm.Common;
internal static class Extensions
{
	internal static bool IsAllowedRace(this Pawn pawn) =>
		(pawn.RaceProps.Humanlike || pawn.RaceProps.Animal || pawn.RaceProps.IsMechanoid) &&
		pawn.Faction == Faction.OfPlayerSilentFail;

	internal static void TickRate(ref int __result)
	{
		if (Settings.EnableTickRateMultiplier)
			__result *= Settings.TickRateMultiplier;
	}
}
