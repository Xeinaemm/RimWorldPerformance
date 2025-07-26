namespace Xeinaemm.Hauling;

[DefOf]
internal static class XeinaemmHaulingDefs
{
	internal static WorkGiverDef HaulGeneral = DefDatabase<WorkGiverDef>.GetNamed("Xeinaemm_HaulGeneral");
	internal static JobDef HaulFromInventory = DefDatabase<JobDef>.GetNamed("Xeinaemm_HaulFromInventory");
	internal static JobDef HaulToInventory = DefDatabase<JobDef>.GetNamed("Xeinaemm_HaulToInventory");

	static XeinaemmHaulingDefs() => DefOfHelper.EnsureInitializedInCtor(typeof(XeinaemmHaulingDefs));
}