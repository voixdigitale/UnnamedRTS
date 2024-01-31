using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildBarracksCommand : Command, IPlaceable
{
    private BuildingSO buildingData;
    private Vector3 placementPosition;

    public BuildBarracksCommand(PlayerController player, BuildingSO buildingData) : base(player)
    {
        IsCoroutine = false;
        this.buildingData = buildingData;
    }

    public override bool ValidateInput(RaycastHit? hit, object[] args)
    {
        if (args[0] is not BuildingPreview) return false;

        BuildingPreview preview = (BuildingPreview)args[0];
        
        if (preview.IsValid())
        {
            placementPosition = preview.transform.position;
            return true;
        }

        return false;
    }

    public override void Execute()
    {
        player.TryPlaceBuilding(buildingData, placementPosition);
    }
    public BuildingSO GetBuildingData()
    {
        return buildingData;
    }
}
