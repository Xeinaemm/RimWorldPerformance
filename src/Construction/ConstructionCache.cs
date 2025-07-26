namespace Xeinaemm.Construction;

internal static class ConstructionCache
{
	private const int TICK_RATE_DELAY = 360;
	internal static ConcurrentDictionary<Map, ConcurrentQueue<Frame>> Cache { get; private set; } = [];
	internal static ConcurrentDictionary<Map, ConcurrentQueue<Frame>> BlockingCache { get; private set; } = [];
	internal static ConcurrentDictionary<Map, int> NextTick { get; private set; } = [];

	internal static ConcurrentQueue<Frame> CalculatePotentialWork(Pawn pawn)
	{
		var currentTick = Find.TickManager.TicksGame;
		if (NextTick.TryGetValue(pawn.Map, out var tick) && currentTick < tick)
			return BlockingCache[pawn.Map].IsEmpty ? Cache[pawn.Map] : BlockingCache[pawn.Map];

		var cache = new ConcurrentQueue<Frame>([.. pawn.Map.listerThings.ThingsInGroup(ThingRequestGroup.BuildingFrame)
					.Where(x => x.IsValidFrame(pawn) && GenConstruct.FirstBlockingThing(x, pawn) == null)
					.OrderBy(x => Math.Atan2(x.Position.x - pawn.Map.Center.x, x.Position.y - pawn.Map.Center.y))
					.FilterEnclosedCandidates(pawn)
					.Cast<Frame>()]);
		Cache.AddOrUpdate(pawn.Map, cache, (key, oldValue) => cache);

		var blockingCache = new ConcurrentQueue<Frame>([.. pawn.Map.listerThings.ThingsInGroup(ThingRequestGroup.BuildingFrame)
					.Where(x => x.IsValidFrame(pawn) && GenConstruct.FirstBlockingThing(x, pawn) != null)
					.OrderBy(x => Math.Atan2(x.Position.x - pawn.Map.Center.x, x.Position.y - pawn.Map.Center.y))
					.Cast<Frame>()]);
		BlockingCache.AddOrUpdate(pawn.Map, blockingCache, (key, oldValue) => blockingCache);

		var nextTick = BlockingCache[pawn.Map].IsEmpty ? currentTick + Math.Max(Cache.Count, TICK_RATE_DELAY) : currentTick + TICK_RATE_DELAY;
		NextTick.AddOrUpdate(pawn.Map, nextTick, (key, oldValue) => nextTick);

		return BlockingCache[pawn.Map].IsEmpty ? Cache[pawn.Map] : BlockingCache[pawn.Map];
	}

	internal static void ForceCleanup()
	{
		Cache.Clear();
		BlockingCache.Clear();
		NextTick.Clear();
	}
}