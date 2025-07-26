namespace Xeinaemm.Construction;
internal static class Extensions
{
	internal static bool IsValidFrame(this Thing thing, Pawn pawn) =>
		pawn.Faction == Faction.OfPlayerSilentFail &&
		thing is Frame frame &&
		frame.IsCompleted() &&
		GenConstruct.CanTouchTargetFromValidCell(frame, pawn) &&
		GenConstruct.CanConstruct(frame, pawn, checkSkills: true);

	internal static Job HandleBlockingThingJob(this Thing thing, Pawn worker, bool forced = false)
	{
		if (thing.def.category == ThingCategory.Plant)
		{
			if (!PlantUtility.PawnWillingToCutPlant_Job(thing, worker) || PlantUtility.TreeMarkedForExtraction(thing))
				return null;
			if (worker.CanReserveAndReach(thing, PathEndMode.ClosestTouch, worker.NormalMaxDanger(), 1, -1, null, forced))
				return JobMaker.MakeJob(JobDefOf.CutPlant, thing);
		}
		else if (thing.def.category == ThingCategory.Item && thing.def.EverHaulable)
		{
			return HaulAIUtility.HaulAsideJobFor(worker, thing);
		}
		else if (thing.def.category == ThingCategory.Building)
		{
			if ((bool)((Building)thing).DeconstructibleBy(worker.Faction))
			{
				if (worker.WorkTypeIsDisabled(WorkTypeDefOf.Construction) ||
					(worker.workSettings != null && !worker.workSettings.WorkIsActive(WorkTypeDefOf.Construction)) ||
					(!forced && thing.IsForbidden(worker)))
					return null;
				if (worker.CanReserveAndReach(thing, PathEndMode.Touch, worker.NormalMaxDanger(), 1, -1, null, forced))
					return new Job(JobDefOf.Deconstruct, thing)
					{
						ignoreDesignations = true
					};
			}
			if (thing.def.mineable)
			{
				if (worker.WorkTypeIsDisabled(WorkTypeDefOf.Mining) || (worker.workSettings != null && !worker.workSettings.WorkIsActive(WorkTypeDefOf.Mining)))
					return null;
				if (worker.CanReserveAndReach(thing, PathEndMode.Touch, worker.NormalMaxDanger(), 1, -1, null, forced))
					return new Job(JobDefOf.Mine, thing)
					{
						ignoreDesignations = true
					};
			}
		}
		return null;
	}

	internal static List<Thing> FilterEnclosedCandidates(this IEnumerable<Thing> candidates, Pawn pawn)
	{
		var filtered = new List<Thing>();

		foreach (var candidate in candidates)
		{
			var grid = pawn.Map.AllCells.ToList();
			var visited = new HashSet<(int, int)>();
			var isReachable = false;
			Queue<IntVec3> queue = new();

			// Temporarily block candidate
			grid.RemoveAll(vector => vector.x == candidate.Position.x && vector.y == candidate.Position.y);

			// Start flood fill from border cells
			foreach (var cell in grid)
				if (cell.x == 0 || cell.x == pawn.Map.Size.x - 1 || cell.y == 0 || cell.y == pawn.Map.Size.y - 1)
					queue.Enqueue(cell);

			while (queue.Count > 0)
			{
				var vector = queue.Dequeue();

				if (visited.Contains((vector.x, vector.y)))
					continue;
				visited.Add((vector.x, vector.y));

				if (vector.x == candidate.Position.x && vector.y == candidate.Position.y && pawn.Map.pathing.Normal.pathGrid.Walkable(candidate.Position))
				{
					isReachable = true;
					break;
				}

				foreach (var (dx, dy) in new (int dx, int dy)[] { (1, 0), (-1, 0), (0, 1), (0, -1) })
				{
					var nx = vector.x + dx;
					var ny = vector.y + dy;
					if (nx < 0 || ny < 0 || nx >= pawn.Map.Size.x || ny >= pawn.Map.Size.y || visited.Contains((nx, ny)))
						continue;

					var neighbor = grid.FirstOrDefault(vector => vector.x == nx && vector.y == ny);
					if (neighbor != default)
						queue.Enqueue(neighbor);
				}
			}

			if (isReachable)
				filtered.Add(candidate);
		}
		return filtered;
	}
}