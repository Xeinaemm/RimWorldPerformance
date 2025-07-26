namespace Xeinaemm.Construction;

[DefOf]
internal static class XeinaemmConstructionDefs
{
	internal static WorkGiverDef WorkFinishFrame = DefDatabase<WorkGiverDef>.GetNamed("WorkGiver_Xeinaemm_ConstructFinishFrames");
	internal static JobDef JobFinishFrame = DefDatabase<JobDef>.GetNamed("JobDriver_Xeinaemm_ConstructFinishFrame");

	static XeinaemmConstructionDefs() => DefOfHelper.EnsureInitializedInCtor(typeof(XeinaemmConstructionDefs));
}