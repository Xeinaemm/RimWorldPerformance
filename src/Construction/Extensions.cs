namespace Xeinaemm.Construction;
internal static class Extensions
{
	//internal static bool IsValidFrame(this Thing thing, Pawn pawn) =>
	//	pawn.Faction == Faction.OfPlayerSilentFail &&
	//	thing is Frame frame &&
	//	frame.IsCompleted() &&
	//	GenConstruct.FirstBlockingThing(thing, pawn) == null &&
	//	GenConstruct.CanTouchTargetFromValidCell(frame, pawn) &&
	//	GenConstruct.CanConstruct(frame, pawn, checkSkills: true);

	//internal static Job ConstructFinishFramesJob(this Pawn pawn, bool forced)
	//{
	//	Task.Run(() => ConstructionCache.CalculatePotentialWork(pawn));
	//	if (ConstructionCache.Cache[pawn.Map].TryDequeue(out var frame))
	//		return JobMaker.MakeJob(JobDefOf.FinishFrame, frame);
	//	return null;
	//}

	//internal static List<Thing> FilterEnclosure(this IEnumerable<Thing> points, Pawn pawn)
	//{
	//	if (!ConstructionCache.Prohibited.ContainsKey(pawn.Map))
	//		ConstructionCache.Prohibited.TryAdd(pawn.Map, []);
	//	var result = new List<Thing>();
	//	foreach (var center in points)
	//	{
	//		if (center.WouldEnclose(pawn, [.. result.Select(x => x.Position)]))
	//			continue;
	//		result.Add(center);
	//	}
	//	return result;
	//}

	//internal static IEnumerable<Thing> GetConstructable(this Pawn pawn)
	//{
	//	foreach (var item in pawn.Map.listerThings.ThingsInGroup(ThingRequestGroup.BuildingFrame))
	//		yield return item;

	//	foreach (var item in pawn.Map.listerThings.ThingsInGroup(ThingRequestGroup.Blueprint))
	//		yield return item;
	//}

	//internal static void DesignateBlockers(this Pawn pawn)
	//{
	//	foreach (var x in pawn.GetConstructable())
	//	{
	//		var thing = GenConstruct.FirstBlockingThing(x, pawn);
	//		if (thing == null || pawn.Map.designationManager.HasMapDesignationOn(thing))
	//			continue;

	//		if (thing.def.category == ThingCategory.Plant)
	//			pawn.Map.designationManager.AddDesignation(new Designation(thing, DesignationDefOf.CutPlant));
	//		else if (thing.def.category == ThingCategory.Item && thing.def.EverHaulable)
	//			pawn.Map.designationManager.AddDesignation(new Designation(thing, DesignationDefOf.Haul));
	//		else if (thing.def.category == ThingCategory.Building && thing.def.mineable)
	//			pawn.Map.designationManager.AddDesignation(new Designation(thing, DesignationDefOf.Mine));
	//		else if (thing.def.category == ThingCategory.Building)
	//			pawn.Map.designationManager.AddDesignation(new Designation(thing, DesignationDefOf.Deconstruct));
	//	}
	//}

	//private static bool WouldEnclose(this Thing center, Pawn pawn, List<IntVec3> blockers)
	//{
	//	if (ConstructionCache.Prohibited[pawn.Map].Contains(center.Position))
	//		return true;
	//	foreach (var cell in center.Position.GetNeighbors(radius: 1))
	//	{
	//		if (cell.Impassable(pawn.Map) || !cell.InBounds(pawn.Map))
	//			continue;
	//		blockers.Add(center.Position);
	//		if (cell.WouldEnclose(pawn, blockers))
	//			return true;
	//	}

	//	return false;
	//}

	//private static bool WouldEnclose(this IntVec3 point, Pawn pawn, List<IntVec3> blockers)
	//{
	//	if (ConstructionCache.Prohibited[pawn.Map].Contains(point))
	//		return true;
	//	var reached = new HashSet<IntVec3>(blockers);
	//	var queue = new Queue<IntVec3>();
	//	reached.Add(point);
	//	queue.Enqueue(point);

	//	while (queue.Count > 0)
	//	{
	//		var candidate = queue.Dequeue();
	//		foreach (var cell in candidate.GetNeighbors(radius: 1))
	//		{
	//			if (cell.Impassable(pawn.Map) || !cell.InBounds(pawn.Map) || reached.Contains(cell))
	//				continue;
	//			if (cell.GetThingList(pawn.Map).Any(x => x is Blueprint or Frame))
	//				queue.Enqueue(cell);
	//			else
	//				return false;
	//		}
	//	}
	//	ConstructionCache.Prohibited[pawn.Map].Add(point);
	//	return true;
	//}

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