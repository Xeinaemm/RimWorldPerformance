namespace Xeinaemm.Hauling;

public class JobDriver_Xeinaemm_HaulFromInventory : JobDriver
{
	public override bool TryMakePreToilReservations(bool errorOnFailed)
	{
		pawn.ReserveAsManyAsPossible(job.GetTargetQueue(TargetIndex.B), job);
		return true;
	}

	protected override IEnumerable<Toil> MakeNewToils()
	{
		var nextTarget = Toils_General.Do(() =>
		{
			job.SetTarget(TargetIndex.A, job.targetQueueA[0]);
			job.targetQueueA.RemoveAt(0);
			job.SetTarget(TargetIndex.B, job.targetQueueB[0]);
			job.targetQueueB.RemoveAt(0);
			job.count = job.countQueue[0];
			job.countQueue.RemoveAt(0);

			if (pawn.carryTracker.CarriedThing != null)
			{
				pawn.carryTracker.TryDropCarriedThing(pawn.Position, ThingPlaceMode.Near, out var dropped);
				dropped.SetForbidden(false, false);
			}
			pawn.inventory.innerContainer.RemoveWhere(x => x == null);
		});
		yield return nextTarget;
		yield return Toils_General.Do(() => pawn.inventory.innerContainer.TryTransferToContainer(TargetThingA, pawn.carryTracker.innerContainer, job.count));

		var carryToCell = Toils_Goto.Goto(TargetIndex.B, PathEndMode.ClosestTouch);
		yield return Toils_Jump.JumpIf(carryToCell, () => !TargetB.HasThing);

		yield return Toils_Goto.Goto(TargetIndex.B, PathEndMode.ClosestTouch);
		yield return Toils_Haul.DepositHauledThingInContainer(TargetIndex.B, TargetIndex.None);
		yield return Toils_Jump.JumpIfHaveTargetInQueue(TargetIndex.A, nextTarget);

		yield return carryToCell;
		yield return Toils_Haul.PlaceHauledThingInCell(TargetIndex.B, carryToCell, true);
		yield return Toils_Jump.JumpIfHaveTargetInQueue(TargetIndex.A, nextTarget);
	}
}