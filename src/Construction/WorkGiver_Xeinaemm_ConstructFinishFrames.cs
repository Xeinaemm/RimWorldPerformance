namespace Xeinaemm.Construction;
public class WorkGiver_Xeinaemm_ConstructFinishFrames : WorkGiver_ConstructFinishFrames
{
	public override IEnumerable<Thing> PotentialWorkThingsGlobal(Pawn pawn) => ConstructionCache.CalculatePotentialWork(pawn);

	public override Job JobOnThing(Pawn pawn, Thing thing, bool forced = false)
	{
		if (!ConstructionCache.BlockingCache[pawn.Map].IsEmpty && ConstructionCache.BlockingCache[pawn.Map].TryDequeue(out var blocker))
			return blocker.HandleBlockingThingJob(pawn, forced);
		if (!ConstructionCache.Cache[pawn.Map].IsEmpty && ConstructionCache.Cache[pawn.Map].TryDequeue(out var frame))
			return JobMaker.MakeJob(XeinaemmConstructionDefs.JobFinishFrame, frame);
		return null;
	}
}
