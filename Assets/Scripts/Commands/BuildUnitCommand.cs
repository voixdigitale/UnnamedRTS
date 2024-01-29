using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildUnitCommand : Command
{
    private UnitSO unitData;
    private Building unitBuilding;


    public BuildUnitCommand(PlayerController player, UnitSO unitData, Building building) : base(player) {
        IsCoroutine = false;
        RequiresValidation = false;
        this.unitData = unitData;
        this.player = player;
        this.unitBuilding = building;
    }

    public override void Execute() {
        BuildUnit();
    }

    public override bool ValidateInput(RaycastHit? hit, object[] args) {
        return true;
    }

    public void BuildUnit() {
            unitBuilding.TrySpawningUnit(unitData);
    }
}