namespace Xeinaemm.Hauling;
internal static class ModCompatibilityCheck
{
	internal static bool CombatExtendedIsActive { get; } = ModsConfig.ActiveModsInLoadOrder.Any(m => m.Name.Contains("Combat Extended", StringComparison.InvariantCultureIgnoreCase));

	internal static bool AllowToolIsActive { get; } = ModsConfig.ActiveModsInLoadOrder.Any(m => m.Name.Contains("Allow Tool", StringComparison.InvariantCultureIgnoreCase));
}