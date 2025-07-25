namespace Xeinaemm.Hauling;

[DefOf]
internal static class XeinaemmDefs
{
	internal static WorkGiverDef Xeinaemm_HaulGeneral = DefDatabase<WorkGiverDef>.GetNamed("Xeinaemm_HaulGeneral");
	internal static JobDef Xeinaemm_HaulFromInventory = DefDatabase<JobDef>.GetNamed("Xeinaemm_HaulFromInventory");
	internal static JobDef Xeinaemm_HaulToInventory = DefDatabase<JobDef>.GetNamed("Xeinaemm_HaulToInventory");

	static XeinaemmDefs() => DefOfHelper.EnsureInitializedInCtor(typeof(XeinaemmDefs));
}