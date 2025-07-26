namespace Xeinaemm.Common;
internal static class Extensions
{
	internal static void CheckForGameChanges(this Game game)
	{
		HaulCache.ForceCleanup();
		ConstructionCache.ForceCleanup();

		foreach (var (pawn, job) in game.Maps
				.SelectMany(map => map.mapPawns.AllPawns)
				.SelectMany(pawn => pawn.jobs.AllJobs()
				.Where(t => t.def == XeinaemmHaulingDefs.HaulFromInventory || t.def == XeinaemmHaulingDefs.HaulToInventory)
				.Select(job => (pawn, job))))
			pawn.jobs.EndCurrentOrQueuedJob(job, JobCondition.InterruptForced);
	}
}
