using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using UnityEngine;

public class GatherCommand : Command
{
    Resource resource;

    public GatherCommand() {
        IsCoroutine = true;
    }

    public override void Execute() {
        GatherResource();
    }

    public override bool ValidateInput(RaycastHit? inputHit) {
        if (!inputHit.HasValue)
            return false;

        RaycastHit hit = (RaycastHit)inputHit;

        if (hit.collider.gameObject.CompareTag("Resource")) {
            resource = hit.collider.GetComponent<Resource>();

            return true;
        }

        return false;
    }

    void GatherResource() {
        List<Vector3> destinations = UnitMovement.GetSurroundingDestinations(resource.transform.position, SelectionManager.Instance.currentSelection.Count);

        foreach (Gatherer unit in SelectionManager.Instance.currentSelection.OfType<Gatherer>()) {
            Vector3 destination = destinations.First();
            destinations.Remove(destination);

            unit.Gather(resource, destination);
        }
    }
}
