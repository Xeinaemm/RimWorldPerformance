namespace Xeinaemm.Hauling;
public class JobDriver_Xeinaemm_HaulToInventory : JobDriver
{
	public override bool TryMakePreToilReservations(bool errorOnFailed)
	{
		pawn.ReserveAsManyAsPossible(job.GetTargetQueue(TargetIndex.A), job);
		return true;
	}

	protected override IEnumerable<Toil> MakeNewToils()
	{
		var nextTarget = Toils_JobTransforms.ExtractNextTargetFromQueue(TargetIndex.A);
		yield return nextTarget;
		yield return Toils_Goto.Goto(TargetIndex.A, PathEndMode.ClosestTouch);
		yield return Toils_General.Do(() =>
		{
			var count = Math.Min(job.count, MassUtility.CountToPickUpUntilOverEncumbered(pawn, TargetThingA));
			if (count <= 0)
			{
				pawn.CheckIfShouldUnloadInventory();
				pawn.ClearReservationsForJob(job);
				EndJobWith(JobCondition.Succeeded);
				return;
			}
			pawn.carryTracker.TryStartCarry(TargetThingA, count);
			if (!pawn.IsCarrying())
				return;
			pawn.carryTracker.innerContainer.TryTransferToContainer(pawn.carryTracker.CarriedThing, pawn.inventory.innerContainer);
		});
		yield return Toils_Jump.JumpIfHaveTargetInQueue(TargetIndex.A, nextTarget);
	}
}