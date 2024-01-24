using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using UnityEngine;

public class GatherCommand : Command
{
    Resource resource;

    public GatherCommand(PlayerController player) : base(player) {
        IsCoroutine = false;
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
        player.SpawnSelectionMarker(resource.transform.position, 2f);

        List<Vector3> destinations = UnitPlacements.GetSurroundingDestinations(resource.transform.position, player.GetCurrentSelectedCount<Gatherer>());

        foreach (Gatherer unit in player.GetSelectedUnits<Gatherer>()) {
            Vector3 destination = destinations.First();
            destinations.Remove(destination);

            unit.Gather(resource, destination);
        }
    }
}
