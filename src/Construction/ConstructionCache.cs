namespace Xeinaemm.Construction;

internal static class ConstructionCache
{
	private const int TICK_RATE_DELAY = 360;
	internal static ConcurrentDictionary<Map, ConcurrentQueue<Thing>> Cache { get; private set; } = [];
	internal static ConcurrentDictionary<Map, ConcurrentQueue<Thing>> BlockingCache { get; private set; } = [];
	internal static ConcurrentDictionary<Map, int> NextTick { get; private set; } = [];

	internal static ConcurrentQueue<Thing> CalculatePotentialWork(Pawn pawn)
	{
		var currentTick = Find.TickManager.TicksGame;
		if (NextTick.TryGetValue(pawn.Map, out var tick) && currentTick < tick)
			return BlockingCache[pawn.Map].IsEmpty ? Cache[pawn.Map] : BlockingCache[pawn.Map];

		var cache = new ConcurrentQueue<Thing>([.. pawn.Map.listerThings.ThingsInGroup(ThingRequestGroup.BuildingFrame)
					.Where(x => x.IsValidFrame(pawn) && GenConstruct.FirstBlockingThing(x, pawn) == null)
					.OrderBy(x => Math.Atan2(x.Position.x - pawn.Map.Center.x, x.Position.z - pawn.Map.Center.z))]);
		Cache.AddOrUpdate(pawn.Map, cache, (key, oldValue) => cache);

		var blockingCache = new ConcurrentQueue<Thing>([.. pawn.Map.listerThings.ThingsInGroup(ThingRequestGroup.BuildingFrame)
					.Where(x => x.IsValidFrame(pawn) && GenConstruct.FirstBlockingThing(x, pawn) != null)
					.OrderBy(x => Math.Atan2(x.Position.x - pawn.Map.Center.x, x.Position.z - pawn.Map.Center.z))]);
		BlockingCache.AddOrUpdate(pawn.Map, blockingCache, (key, oldValue) => blockingCache);

		var nextTick = BlockingCache[pawn.Map].IsEmpty ? currentTick + Math.Max(Cache.Count, TICK_RATE_DELAY) : currentTick + TICK_RATE_DELAY;
		NextTick.AddOrUpdate(pawn.Map, nextTick, (key, oldValue) => nextTick);

		return BlockingCache[pawn.Map].IsEmpty ? Cache[pawn.Map] : BlockingCache[pawn.Map];
	}
}