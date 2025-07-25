namespace Xeinaemm.Hauling;

[DefOf]
public static class XeinaemmDefs
{
	public static DesignationDef AllowToolsHaulUrgently = DefDatabase<DesignationDef>.GetNamedSilentFail("HaulUrgentlyDesignation");
	public static WorkGiverDef Xeinaemm_HaulGeneral = DefDatabase<WorkGiverDef>.GetNamed("Xeinaemm_HaulGeneral");
	public static JobDef Xeinaemm_HaulFromInventory = DefDatabase<JobDef>.GetNamed("Xeinaemm_HaulFromInventory");
	public static JobDef Xeinaemm_HaulToInventory = DefDatabase<JobDef>.GetNamed("Xeinaemm_HaulToInventory");
}