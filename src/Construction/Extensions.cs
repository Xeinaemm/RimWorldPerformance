namespace Xeinaemm.Construction;
internal static class Extensions
{
	internal static bool IsValidFrame(this Thing thing, Pawn pawn) =>
		pawn.Faction == Faction.OfPlayerSilentFail &&
		thing is Frame frame &&
		frame.IsCompleted() &&
		GenConstruct.CanTouchTargetFromValidCell(frame, pawn) &&
		GenConstruct.CanConstruct(frame, pawn, checkSkills: true);

	internal static Job ConstructFinishFramesJob(this Pawn pawn, bool forced)
	{
		ConstructionCache.CalculatePotentialWork(pawn);
		if (!ConstructionCache.BlockingCache[pawn.Map].IsEmpty && ConstructionCache.BlockingCache[pawn.Map].TryDequeue(out var blocker))
			return GenConstruct.HandleBlockingThingJob(blocker, pawn, forced);
		if (!ConstructionCache.Cache[pawn.Map].IsEmpty && ConstructionCache.Cache[pawn.Map].TryDequeue(out var frame))
			return JobMaker.MakeJob(JobDefOf.FinishFrame, frame);
		return null;
	}

	internal static IEnumerable<Toil> CheckRoofs(IEnumerable<Toil> toils, JobDriver __instance)
	{
		if (__instance.pawn.IsAllowedRace())
		{
			yield return new Toil()
			{
				initAction = () => __instance.job.targetA.Thing.Position.GetNeighbors(radius: 7, excludeCenter: false).ForEach(cell =>
				{
					if (cell.InBounds(__instance.pawn.Map) && cell.CanRemoveRoof(__instance.pawn))
					{
						if (!__instance.pawn.Map.areaManager.NoRoof[cell])
							__instance.pawn.Map.areaManager.NoRoof[cell] = true;

						if (__instance.pawn.Map.areaManager.BuildRoof[cell])
							__instance.pawn.Map.areaManager.BuildRoof[cell] = false;
						__instance.pawn.Map.areaManager.AreaManagerUpdate();
					}
				}),
				defaultCompleteMode = ToilCompleteMode.Instant
			};
		}

		foreach (var toil in toils)
			yield return toil;
	}

	private static List<IntVec3> GetNeighbors(this IntVec3 center, int radius, bool excludeCenter = true)
	{
		var neighbors = new List<IntVec3>();
		for (var dz = -radius; dz <= radius; dz++)
			for (var dx = -radius; dx <= radius; dx++)
			{
				var candidate = new IntVec3(center.x + dx, 0, center.z + dz);
				if (excludeCenter && candidate == center)
					continue;

				// Use Euclidean distance
				if (Math.Sqrt((dx * dx) + (dz * dz)) <= radius)
					neighbors.Add(candidate);
			}

		return neighbors;
	}

	private static bool CanRemoveRoof(this IntVec3 cell, Pawn pawn) =>
		cell.Roofed(pawn.Map) &&
		!cell.GetRoof(pawn.Map).isThickRoof &&
		pawn.CanReach(cell, PathEndMode.ClosestTouch, Danger.Some);
}