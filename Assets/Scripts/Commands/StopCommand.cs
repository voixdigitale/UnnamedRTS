using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Photon.Realtime;

public class StopCommand : Command
{
    public StopCommand(PlayerController player) : base(player)
    {
        IsCoroutine = false;
        RequiresValidation = false;
    }

    public override void Execute()
    {
        StopUnits();
    }

    public override bool ValidateInput(RaycastHit? hit, object[] args)
    {
        return true;
    }

    public void StopUnits()
    {
        foreach (Unit unit in player.GetSelectedUnits<Unit>())
        {
            unit.Stop();
        }
    }
}
