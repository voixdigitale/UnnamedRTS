using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AttackCommand : Command
{
    public AttackCommand() {
        IsCoroutine = false;
    }

    private Unit target;

    public override void Execute() {
        AttackUnit(target);
        return;
    }

    public override bool ValidateInput(RaycastHit? inputHit) {
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
        SelectionManager.Instance.SpawnSelectionMarker(target.transform.position, 1f, Color.red);

        List<Vector3> destinations = UnitPlacements.GetSurroundingDestinations(target.transform.position, SelectionManager.Instance.currentSelection.Count);

        foreach (Unit attacker in SelectionManager.Instance.currentSelection.OfType<Unit>()) {
            Vector3 destination = destinations.First();
            destinations.Remove(destination);

            attacker.Attack(target, destination);
        }
    }
}
