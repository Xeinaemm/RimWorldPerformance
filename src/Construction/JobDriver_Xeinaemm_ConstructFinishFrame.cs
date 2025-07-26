namespace Xeinaemm.Construction;

public class JobDriver_Xeinaemm_ConstructFinishFrame : JobDriver, IBuildableDriver
{
	private Frame Frame => (Frame)job.GetTarget(TargetIndex.A).Thing;

	private bool IsBuildingAttachment => ((GenConstruct.BuiltDefOf(Frame.def) as ThingDef)?.building)?.isAttachment ?? false;

	public override bool TryMakePreToilReservations(bool errorOnFailed) => pawn.Reserve(job.targetA, job, 1, -1, null, errorOnFailed);

	public bool TryGetBuildableRect(out CellRect rect)
	{
		rect = Frame.OccupiedRect();
		return true;
	}

	protected override IEnumerable<Toil> MakeNewToils()
	{
		yield return IsBuildingAttachment
			? Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.OnCell).FailOnDespawnedNullOrForbidden(TargetIndex.A)
			: Toils_Goto.GotoBuild(TargetIndex.A).FailOnDespawnedNullOrForbidden(TargetIndex.A);

		yield return new Toil()
		{
			initAction = () => GenClamor.DoClamor(pawn, 15f, ClamorDefOf.Construction),
			tickIntervalAction = delta =>
			{
				var frame = Frame;
				if (frame.resourceContainer.Count > 0 && pawn.skills != null)
				{
					pawn.skills.Learn(SkillDefOf.Construction, 0.25f * delta);
				}
				pawn.rotationTracker.FaceTarget(IsBuildingAttachment ? GenConstruct.GetWallAttachedTo(frame) : frame);
				var num = pawn.GetStatValue(StatDefOf.ConstructionSpeed) * 1.7f * delta;
				if (frame.Stuff != null)
				{
					num *= frame.Stuff.GetStatValueAbstract(StatDefOf.ConstructionSpeedFactor);
				}
				if (pawn.Faction == Faction.OfPlayer)
				{
					var statValue = pawn.GetStatValue(StatDefOf.ConstructSuccessChance);
					if (!TutorSystem.TutorialMode && Rand.Value < 1f - Math.Pow(statValue, num / (float)frame.WorkToBuild))
					{
						frame.FailConstruction(pawn);
						ReadyForNextToil();
						return;
					}
				}
				if (frame.def.entityDefToBuild is TerrainDef)
				{
					Map.snowGrid.SetDepth(frame.Position, 0f);
					Map.sandGrid?.SetDepth(frame.Position, 0f);
				}
				frame.workDone += num;
				if (frame.workDone >= (float)frame.WorkToBuild)
				{
					frame.CompleteConstruction(pawn);
					ReadyForNextToil();
				}
			},
			defaultCompleteMode = ToilCompleteMode.Delay,
			defaultDuration = 5000,
			activeSkill = () => SkillDefOf.Construction,
			handlingFacing = true,
		}
		.WithEffect(() => ((Frame)pawn.jobs.curJob.GetTarget(TargetIndex.A).Thing).ConstructionEffect, TargetIndex.A)
		.FailOnDespawnedNullOrForbidden(TargetIndex.A)
		.FailOn(() => !GenConstruct.CanConstruct(Frame, pawn));
	}
}