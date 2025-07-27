namespace Xeinaemm.Hauling;

internal static class HaulCache
{
	private const int TICK_RATE_DELAY = 360;
	internal static ConcurrentDictionary<Map, ConcurrentQueue<Thing>> Cache { get; private set; } = [];
	internal static ConcurrentDictionary<Map, ConcurrentQueue<Thing>> UrgentCache { get; private set; } = [];
	internal static ConcurrentDictionary<Map, int> NextTick { get; private set; } = [];

	internal static ConcurrentQueue<Thing> CalculatePotentialWork(Pawn pawn)
	{
		var currentTick = Find.TickManager.TicksGame;
		if (NextTick.TryGetValue(pawn.Map, out var tick) && currentTick < tick)
			return UrgentCache[pawn.Map].IsEmpty ? Cache[pawn.Map] : UrgentCache[pawn.Map];

		//Make sure things are sorted counterclockwise from the center.
		//Although Atan2 pathfinding is more efficient than a simple horizontal square sort (~3 mins vs ~4 mins),
		//it takes 10 pawns a 10-30 seconds longer than a Hilbert sort for every 10,000 things.
		var cache = new ConcurrentQueue<Thing>([.. pawn.Map.listerHaulables.ThingsPotentiallyNeedingHauling()
					.Where(x => !x.IsCorrupted(pawn) && !x.IsUrgent(pawn.Map))
					.OrderBy(x => Math.Atan2(x.Position.x - pawn.Map.Center.x, x.Position.z - pawn.Map.Center.z))]);
		Cache.AddOrUpdate(pawn.Map, cache, (key, oldValue) => cache);

		var urgentCache = new ConcurrentQueue<Thing>([.. pawn.Map.listerHaulables.ThingsPotentiallyNeedingHauling()
					.Where(x => !x.IsCorrupted(pawn) && x.IsUrgent(pawn.Map))
					.OrderBy(x => Math.Atan2(x.Position.x - pawn.Map.Center.x, x.Position.z - pawn.Map.Center.z))]);
		UrgentCache.AddOrUpdate(pawn.Map, urgentCache, (key, oldValue) => urgentCache);

		var nextTick = UrgentCache[pawn.Map].IsEmpty ? currentTick + Math.Max(Cache.Count, TICK_RATE_DELAY) : currentTick + TICK_RATE_DELAY;
		NextTick.AddOrUpdate(pawn.Map, nextTick, (key, oldValue) => nextTick);

		return UrgentCache[pawn.Map].IsEmpty ? Cache[pawn.Map] : UrgentCache[pawn.Map];
	}
}