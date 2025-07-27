global using HarmonyLib;
global using RimWorld;
global using RimWorld.Planet;
global using System.Collections.Concurrent;
global using System.Diagnostics;
global using System.Runtime.CompilerServices;
global using UnityEngine;
global using Verse;
global using Verse.AI;
global using Xeinaemm.Common;

namespace Xeinaemm;

public class Modbase : Mod
{
	public Modbase(ModContentPack content) : base(content)
	{
		Instance = this;
		Settings = GetSettings<Settings>();
	}

	public override void DoSettingsWindowContents(Rect inRect) => Settings.DoSettingsWindowContents(inRect);
	public override string SettingsCategory() => "RimWorld Performance";
	public static Modbase Instance { get; private set; }
	public static Settings Settings { get; private set; }
}