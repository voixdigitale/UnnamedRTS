using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MoveCommand : Command
{
    public MoveCommand(PlayerController player) : base(player) {
        IsCoroutine = true;
    }

    private Vector3 destination;

    public override void Execute() {
        return;
    }

    public override IEnumerator CommandCoroutine() {
        return MoveUnits();
    }

    public override bool ValidateInput(RaycastHit? inputHit) {
        if (!inputHit.HasValue) return false;

        RaycastHit hit = (RaycastHit)inputHit;

        if (hit.collider.gameObject.CompareTag("Ground")) {
            destination = hit.point;

            return true;
        }

        return false;
    }

    IEnumerator MoveUnits() {
        List<Vector3> destinations = UnitPlacements.GetGroupDestinations(destination, player.GetCurrentSelectedCount<Unit>(), 1.2f);

        foreach (Unit unit in player.GetSelectedUnits<Unit>()) {
            Vector3 destination = destinations.First();
            destinations.Remove(destination);

            player.SpawnSelectionMarker(destination);

            //Adding a bit of randomness to the movement
            yield return new WaitForSeconds(Random.Range(0, 30) / 100);

            unit.SetAgentDestination(destination);
        }
    }
}
