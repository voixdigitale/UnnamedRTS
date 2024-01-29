using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AttackCommand : Command
{
    public AttackCommand(PlayerController player) : base(player) {
        IsCoroutine = false;
    }

    private Unit target;

    public override void Execute() {
        AttackUnit(target);
        return;
    }

    public override bool ValidateInput(RaycastHit? inputHit, object[] args) {
        if (!inputHit.HasValue)
            return false;
        RaycastHit hit = (RaycastHit)inputHit;

        if (hit.collider.gameObject.CompareTag("Unit")) {
            target = hit.collider.GetComponent<Unit>();

            return true;
        }

        return false;
    }

    void AttackUnit(Unit target) {
        player.SpawnSelectionMarker(target.transform.position, 1f, Color.red);

        List<Vector3> destinations = UnitPlacements.GetSurroundingDestinations(target.transform.position, player.GetCurrentSelectedCount<Unit>());

        foreach (Unit attacker in player.GetSelectedUnits<Unit>()) {
            Vector3 destination = destinations.First();
            destinations.Remove(destination);

            attacker.Attack(target, destination);
        }
    }
}
