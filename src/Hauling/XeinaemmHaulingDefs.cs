namespace Xeinaemm.Hauling;

[DefOf]
public static class XeinaemmHaulingDefs
{
	public static JobDef Xeinaemm_HaulFromInventory;
	public static JobDef Xeinaemm_HaulToInventory;

	static XeinaemmHaulingDefs() => DefOfHelper.EnsureInitializedInCtor(typeof(XeinaemmHaulingDefs));
}