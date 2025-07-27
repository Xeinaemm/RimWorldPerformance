namespace Xeinaemm.Hauling;

internal static class Extensions
{
	private const int MAX_NOT_URGENT_THINGS_PER_JOB = 50;
	private const int MAX_URGENT_THINGS_PER_JOB = 5;
	private static readonly object _lockObject = new();

	private static bool CombatExtendedIsActive { get; } =
		ModsConfig.ActiveModsInLoadOrder.Any(m => m.Name.Contains("Combat Extended", StringComparison.InvariantCultureIgnoreCase));

	private static bool AllowToolIsActive { get; } =
		ModsConfig.ActiveModsInLoadOrder.Any(m => m.Name.Contains("Allow Tool", StringComparison.InvariantCultureIgnoreCase));

	internal static void CheckIfShouldUnloadInventory(this Pawn pawn) =>
		Task.Run(() =>
		{
			if (!pawn.IsInValidHaulingState() || pawn.inventory.innerContainer.Count == 0)
				return;

			if ((pawn.jobs.curJob != null && pawn.jobs.curJob.def == XeinaemmHaulingDefs.Xeinaemm_HaulFromInventory)
				|| pawn.jobs.jobQueue.Any(x => x.job.def == XeinaemmHaulingDefs.Xeinaemm_HaulFromInventory))
				return;

			lock (_lockObject)
			{
				var job = new Job(XeinaemmHaulingDefs.Xeinaemm_HaulFromInventory);
				pawn.inventory.innerContainer.RemoveWhere(x => x == null);
				foreach (var thing in pawn.inventory.innerContainer)
					pawn.FindBestBetterStorageFor(thing, job);

				if (job.targetQueueA.Count == 0)
					return;
				pawn.jobs.jobQueue.EnqueueFirst(job);
				return;
			}
		});

	internal static Job HaulToInventory(this Pawn pawn, Thing thing, bool forced)
	{
		if (pawn.IsInValidHaulingState())
		{
			var job = new Job(XeinaemmHaulingDefs.Xeinaemm_HaulToInventory);
			pawn.GetClosestAndEnqueue(job);
			return job.targetQueueA.Count > 0 ? job : null;
		}
		else
			return HaulAIUtility.HaulToStorageJob(pawn, thing, forced);
	}

	private static void CheckUrgentHaul(this Pawn pawn) =>
		Task.Run(() =>
		{
			if (HaulCache.UrgentCache[pawn.Map].IsEmpty)
				return;
			var job = new Job(XeinaemmHaulingDefs.Xeinaemm_HaulToInventory);
			pawn.GetUrgentAndEnqueue(job);
			if (job.targetQueueA.Count > 0)
				pawn.jobs.jobQueue.EnqueueFirst(job);
		});

	internal static bool IsInValidHaulingState(this Pawn pawn) =>
		pawn.IsAllowedRace() && !pawn.IsQuestLodger() && !CombatExtendedIsActive;

	internal static bool IsUrgent(this Thing thing, Map map) =>
		AllowToolIsActive && map.designationManager.DesignationOn(thing)?.def == DefDatabase<DesignationDef>.GetNamedSilentFail("HaulUrgentlyDesignation");

	internal static bool IsCorrupted(this Thing thing, Pawn pawn) =>
		thing is null or Corpse || !thing.Spawned || thing.Destroyed || !HaulAIUtility.PawnCanAutomaticallyHaul(pawn, thing, false);

	internal static bool GetMaxAllowedToPickUp(Pawn pawn, ThingDef thingDef, ref int __result)
	{
		if (!CombatExtendedIsActive)
		{
			__result = int.MaxValue;
			return pawn.IsQuestLodger();
		}
		return true;
	}

	internal static bool CanPickUp(Pawn pawn, ThingDef thingDef, ref bool __result)
	{
		if (!CombatExtendedIsActive)
		{
			__result = !pawn.IsQuestLodger();
			return false;
		}
		return true;
	}

	internal static void TryGiveJob(Pawn pawn)
	{
		pawn.CheckIfShouldUnloadInventory();
		pawn.CheckUrgentHaul();
	}

	private static void FindBestBetterStorageFor(this Pawn pawn, Thing thing, Job job)
	{
		if (pawn == null || pawn.Map == null || pawn.Faction == null || thing == null || job == null)
			return;
		job.targetQueueA ??= [];
		job.targetQueueB ??= [];
		job.countQueue ??= [];
		if (StoreUtility.TryFindBestBetterStorageFor(thing, pawn, pawn.Map, StoreUtility.CurrentStoragePriorityOf(thing), pawn.Faction, out var storeCell, out var dest))
		{
			LocalTargetInfo storeTarget = null;
			var count = 99999;
			if (dest is ISlotGroupParent)
			{
				storeTarget = storeCell;
				var slotGroup = pawn.Map.haulDestinationManager.SlotGroupAt(storeCell);
				if (slotGroup != null)
				{
					var num = 0;
					var statValue = pawn.GetStatValue(StatDefOf.CarryingCapacity, true);
					foreach (var cell in slotGroup.CellsList)
						if (StoreUtility.IsGoodStoreCell(cell, pawn.Map, thing, pawn, pawn.Faction))
						{
							var thing2 = pawn.Map.thingGrid.ThingAt(cell, thing.def);
							if (thing2 != null && thing2 != thing)
								num += Math.Max(thing.def.stackLimit - thing2.stackCount, 0);
							else
								num += thing.def.stackLimit;
							if (num >= statValue)
								break;
						}
					count = num;
				}
			}
			else if (dest is Thing container && container.TryGetInnerInteractableThingOwner() != null)
			{
				storeTarget = container;
				count = Math.Min(thing.stackCount, container.TryGetInnerInteractableThingOwner().GetCountCanAccept(thing, true));
			}
			if (storeTarget == null)
				return;
			job.targetQueueA.Add(thing);
			job.targetQueueB.Add(storeTarget);
			job.countQueue.Add(count);
		}
	}

	private static void GetUrgentAndEnqueue(this Pawn pawn, Job job, Thing previousThing = null)
	{
		job.targetQueueA ??= [];
		job.countQueue ??= [];

		while (!HaulCache.UrgentCache[pawn.Map].IsEmpty)
		{
			if (job.targetQueueA.Count >= MAX_URGENT_THINGS_PER_JOB)
				break;
			if (!HaulCache.UrgentCache[pawn.Map].TryDequeue(out var candidate) || candidate.IsCorrupted(pawn))
				continue;
			if (previousThing != null && (previousThing.Position - candidate.Position).LengthHorizontalSquared > 144f)
				continue;
			var count = Math.Min(candidate.stackCount, MassUtility.CountToPickUpUntilOverEncumbered(pawn, candidate));
			if (count <= 0)
				return;

			job.targetQueueA.Add(candidate);
			job.countQueue.Add(count);
			pawn.GetUrgentAndEnqueue(job, candidate);
		}
	}

	private static void GetClosestAndEnqueue(this Pawn pawn, Job job, Thing previousThing = null)
	{
		job.targetQueueA ??= [];
		job.countQueue ??= [];

		while (!HaulCache.Cache[pawn.Map].IsEmpty)
		{
			if (job.targetQueueA.Count >= MAX_NOT_URGENT_THINGS_PER_JOB)
				break;
			if (!HaulCache.Cache[pawn.Map].TryDequeue(out var candidate) || candidate.IsCorrupted(pawn))
				continue;
			if (previousThing != null && (previousThing.Position - candidate.Position).LengthHorizontalSquared > 144f)
				continue;
			var count = Math.Min(candidate.stackCount, MassUtility.CountToPickUpUntilOverEncumbered(pawn, candidate));
			if (count <= 0)
				return;
			job.targetQueueA.Add(candidate);
			job.countQueue.Add(count);
			pawn.GetClosestAndEnqueue(job, candidate);
		}
	}
}