global using HarmonyLib;
global using RimWorld;
global using RimWorld.Planet;
global using System;
global using System.Collections.Concurrent;
global using System.Collections.Generic;
global using System.Diagnostics;
global using System.Diagnostics.CodeAnalysis;
global using System.IO;
global using System.Linq;
global using System.Reflection.Emit;
global using System.Runtime.CompilerServices;
global using System.Threading.Tasks;
global using UnityEngine;
global using Verse;
global using Verse.AI;
global using Xeinaemm.Hauling;
global using Xeinaemm.Ticks;

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