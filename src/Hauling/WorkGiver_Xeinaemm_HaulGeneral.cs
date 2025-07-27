namespace Xeinaemm.Hauling;
public class WorkGiver_Xeinaemm_HaulGeneral : WorkGiver_HaulGeneral
{
	public override IEnumerable<Thing> PotentialWorkThingsGlobal(Pawn pawn) => HaulCache.CalculatePotentialWork(pawn);
	public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false) => true;
	public override Job JobOnThing(Pawn pawn, Thing thing, bool forced = false) => pawn.HaulToInventory(thing, forced);
}