//namespace Xeinaemm.Construction;

//internal static class ConstructionCache
//{
//	private const int TICK_RATE_DELAY = 360;
//	internal static ConcurrentDictionary<Map, ConcurrentQueue<Thing>> Cache { get; private set; } = [];
//	internal static ConcurrentDictionary<Map, int> NextTick { get; private set; } = [];
//	internal static ConcurrentDictionary<Map, HashSet<IntVec3>> Prohibited { get; private set; } = [];

//	internal static void CalculatePotentialWork(Pawn pawn)
//	{
//		var currentTick = Find.TickManager.TicksGame;
//		if (NextTick.TryGetValue(pawn.Map, out var tick) && currentTick < tick)
//			return;

//		if (Cache[pawn.Map].IsEmpty)
//			Prohibited.AddOrUpdate(pawn.Map, [], (kay, oldValue) => []);

//		var cache = new ConcurrentQueue<Thing>([.. pawn.GetConstructable()
//					.OrderBy(x => x.Position.x).ThenBy(y => y.Position.y)
//					.FilterEnclosure(pawn)
//					.Where(x => x.IsValidFrame(pawn))]);
//		Cache.AddOrUpdate(pawn.Map, cache, (key, oldValue) => cache);

//		pawn.DesignateBlockers();

//		var nextTick = currentTick + Math.Max(Cache.Count, TICK_RATE_DELAY);
//		NextTick.AddOrUpdate(pawn.Map, nextTick, (key, oldValue) => nextTick);
//	}
//}